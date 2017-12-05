using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using MongoDB.Bson;
using MongoDB.Driver;
using ncBehaviouralTargeting.Library.Configuration;
using ncBehaviouralTargeting.Library.Models;

namespace ncBehaviouralTargeting.Library.DataSources.Mongo
{
    internal class MongoTrackingQueryable : ITrackingQueryable
    {
        readonly IMongoCollection<dynamic> collection;

        public MongoTrackingQueryable(IMongoCollection<dynamic> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            this.collection = collection;
        }

        public QueryResult Query(bool excludeIncompleteProfiles, bool excludeNcbtProperties, IReadOnlyCollection<Segment> segments, int? page)
        {
            FilterDefinition<dynamic> filter;
            var builder = Builders<dynamic>.Filter;

            if (excludeIncompleteProfiles)
            {
                List<FieldDefinition<dynamic>> fields;

                if (ConfigurationHelper.Settings.Profile.ElementInformation.IsPresent)
                {
                    fields = ConfigurationHelper.Settings.Profile.ProfileFields.OfType<FootprintConfigurationSection.ProfileElement.ProfileFieldElement>()
                                                .Select(profileField => new StringFieldDefinition<dynamic>(profileField.Name))
                                                .Cast<FieldDefinition<dynamic>>()
                                                .ToList();
                }
                else
                {
                    fields = new List<FieldDefinition<dynamic>>
                    {
                        new StringFieldDefinition<dynamic>("name"),
                        new StringFieldDefinition<dynamic>("email")
                    };
                }

                filter = builder.Or(
                    fields.Select(x => builder.Exists(x)));
            }
            else
            {
                filter = new BsonDocumentFilterDefinition<dynamic>(new BsonDocument());
            }

            var segmentFilters = new List<FilterDefinition<dynamic>>();

            foreach (var segment in segments)
            {
                var segmentKey = $"ncbt.persistedSegments.{segment.Alias}";

                if (segment.Persistence > 0)
                {
                    segmentFilters.Add(
                        builder.Gte(new StringFieldDefinition<dynamic, DateTime>(segmentKey),
                            DateTime.UtcNow.AddDays(-segment.Persistence)));
                }
                else if (segment.Persistence < 0)
                {
                    segmentFilters.Add(
                        builder.Exists(new StringFieldDefinition<dynamic>(segmentKey)));
                }
            }

            if (segmentFilters.Count > 0)
            {
                filter = builder.And(
                    builder.Or(segmentFilters),
                    filter);
            }

            var totalNumDocuments = collection.CountAsync(filter).Result;

            var fluent = collection.Find(filter)
                                   .Sort(Builders<dynamic>.Sort.Descending(new StringFieldDefinition<dynamic>("_id")));

            if (excludeNcbtProperties)
            {
                var projection = Builders<dynamic>.Projection;

                fluent = fluent.Project<dynamic>(
                    projection.Combine(
                        projection.Exclude("_id"),
                        projection.Exclude("ncbt")));
            }

            if (page.HasValue)
            {
                fluent = fluent.Skip(page * QueryResult.Limit)
                               .Limit(QueryResult.Limit);
            }

            var dictionaries = fluent.ToListAsync().Result
                                     .Cast<IDictionary<string, object>>()
                                     .Select(x => Flatten(x).Where(y => !y.Key.EndsWith(".Timestamp")) // Strip out Timestamps.
                                                            .Select(y => new KeyValuePair<string, object>(y.Key.Replace(".Value", string.Empty), y.Value)) // Drop the ".Value" suffix.
                                                            .ToDictionary(y => y.Key, y => y.Value))
                                     .ToList();

            // The dictionary sets will have different sets of keys, so we normalize those
            // by adding the missing keys that other dictionaries have.
            var allKeys = dictionaries.SelectMany(x => x.Keys)
                                      .Distinct()
                                      .OrderBy(x => x)
                                      .ToList()
                                      .AsReadOnly();

            foreach (var dictionary in dictionaries)
            {
                var missingKeys = allKeys.Except(dictionary.Keys);

                foreach (var missingKey in missingKeys)
                {
                    dictionary.Add(missingKey, "N/A");
                }
            }

            // Order the dictionaries so the keys match the order of the headers.
            var documents = dictionaries.Select(
                x => x.OrderBy(y => y.Key)
                      .ToDictionary(y => y.Key, y => y.Value))
                                        .ToList()
                                        .AsReadOnly();

            return new QueryResult(
                totalNumDocuments,
                allKeys,
                documents);
        }

        static IDictionary<string, object> Flatten(IDictionary<string, object> doc)
        {
            var toReturn = new Dictionary<string, object>();

            foreach (var outerKvp in doc)
            {
                var value = outerKvp.Value as IDictionary<string, object>;

                if (value == null)
                {
                    toReturn.Add(outerKvp.Key, outerKvp.Value);
                }
                else
                {
                    var flatObject = Flatten(value);

                    foreach (var innerKvp in flatObject)
                    {
                        toReturn.Add($"{outerKvp.Key}.{innerKvp.Key}", innerKvp.Value);
                    }
                }
            }

            return toReturn;
        }
    }
}
