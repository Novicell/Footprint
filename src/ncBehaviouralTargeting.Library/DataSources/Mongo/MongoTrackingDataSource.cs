using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using MongoDB.Driver;
using ncBehaviouralTargeting.Library.Models;
using ncBehaviouralTargeting.Library.Models.Enumerations;
using Umbraco.Core;
using NcbtSegment = ncBehaviouralTargeting.Library.Models.Segment;

namespace ncBehaviouralTargeting.Library.DataSources.Mongo
{
    internal class MongoTrackingDataSource : ISegmentReader, ITrackingWriter, ITrackingAggregator, ISegmentWriter
    {
        readonly IMongoCollection<dynamic> collection;

        public MongoTrackingDataSource(IMongoCollection<dynamic> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            this.collection = collection;
        }

        public long CountVisitors()
        {
            return collection.CountAsync(new BsonDocument(false)).Result;
        }

        public long CountVisitorsInSegment(NcbtSegment segment)
        {
            if (segment == null || !segment.HasCriterions)
            {
                return 0;
            }

            return collection.CountAsync(BsonDocument.Parse(GenerateSegmentMatchQuery(segment))).Result;
        }

        public IReadOnlyCollection<dynamic> GetVisitorsInSegment(NcbtSegment segment)
        {
            if (segment == null || !segment.HasCriterions)
            {
                return new List<dynamic>();
            }

            return collection.Find(BsonDocument.Parse(GenerateSegmentMatchQuery(segment)))
                                   .ToListAsync()
                                   .Result;
        }

        public IReadOnlyCollection<dynamic> GetVisitorsInSegmentWithProperties(NcbtSegment segment, IReadOnlyDictionary<string, bool> properties)
        {
            if (segment == null || !segment.HasCriterions)
            {
                return new List<dynamic>();
            }

            return collection.Find(BsonDocument.Parse(GenerateSegmentWithPropertiesMatchQuery(segment, properties)))
                                   .ToListAsync()
                                   .Result;
        }

        public virtual bool IsVisitorInSegment(string visitorId, NcbtSegment segment)
        {
            if (visitorId.IsNullOrWhiteSpace())
            {
                return false;
            }

            if (segment == null || !segment.HasCriterions)
            {
                return false;
            }

            return collection.CountAsync(BsonDocument.Parse(GenerateIsVisitorInSegmentQuery(visitorId, segment)))
                             .Result > 0;
        }

        public void SetVisitorProperties(string visitorId, IReadOnlyDictionary<string, object> properties)
        {
            if (visitorId == null)
            {
                throw new ArgumentNullException(nameof(visitorId));
            }

            if (properties == null)
            {
                throw new ArgumentNullException(nameof(properties));
            }

            var visitorObjectId = GetObjectId(visitorId);
            var update = new BsonDocument("$set", new BsonDocument(properties.ToValuesWithTimestamps()));
            Upsert(visitorObjectId, update);
        }

        public void SetVisitorProperty(string visitorId, string key, object value)
        {
            if (visitorId == null)
            {
                throw new ArgumentNullException(nameof(visitorId));
            }

            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            SetVisitorProperties(visitorId, new Dictionary<string, object> { [key] = value });
        }

        public void PushToVisitorProperty(string visitorId, string key, object value)
        {
            if (visitorId == null)
            {
                throw new ArgumentNullException(nameof(visitorId));
            }

            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var visitorObjectId = GetObjectId(visitorId);
            var dictionary = new Dictionary<string, object> { { key, value } };
            var update = new BsonDocument("$push", new BsonDocument(dictionary.ToValuesWithTimestamps()));
            Upsert(visitorObjectId, update);
        }

        public void AddToSegment(string visitorId, NcbtSegment segment)
        {
            if (visitorId == null)
            {
                throw new ArgumentNullException(nameof(visitorId));
            }

            if (segment == null)
            {
                throw new ArgumentNullException(nameof(segment));
            }

            var options = new UpdateOptions { IsUpsert = true };
            var update = new BsonDocument("$set", new BsonDocument(new Dictionary<string, object> { { "ncbt.persistedSegments." + segment.Alias, DateTime.UtcNow } }));
            var filter = BsonDocument.Parse(GenerateNeedsPersistenceQuery(visitorId, segment));
            collection.UpdateOneAsync(filter, update, options);
        }

        void Upsert(ObjectId objectId, BsonDocument update)
        {
            var options = new UpdateOptions { IsUpsert = true };
            var filter = new BsonDocument("_id", objectId);
            collection.UpdateOneAsync(filter, update, options);
        }

        static ObjectId GetObjectId(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return ObjectId.GenerateNewId();
            }

            return ObjectId.Parse(id);
        }

        static string GenerateIsVisitorInSegmentQuery(string visitorId, NcbtSegment segment)
        {
            // Build query
            var queryBuilder = new StringBuilder();

            queryBuilder.Append("{ '$and':[ ");

                queryBuilder.Append("{ '_id': ObjectId('" + visitorId + "') }");
                queryBuilder.Append(",");
            if (segment.Persistence != 0)
            {
                queryBuilder.Append("{ '$or':[ ");
                if (segment.Persistence > 0)
                {
                    queryBuilder.Append(
                        "{ 'ncbt.persistedSegments.{SEGMENT}': { '$gte': {DATE} } }"
                        .Replace("{SEGMENT}", segment.Alias)
                        .Replace("{DATE}", $"ISODate(\"{DateTime.UtcNow.AddDays(-segment.Persistence).ToString("O")}\")"));
                }
                else
                {
                    queryBuilder.Append("{ 'ncbt.persistedSegments.{SEGMENT}': { $exists: true } }".Replace("{SEGMENT}", segment.Alias));
                }
                queryBuilder.Append(",");
                queryBuilder.Append(GenerateSegmentMatchQuery(segment));
                queryBuilder.Append("] },"); // End '$or' array
            }
            else
            {
                queryBuilder.Append(GenerateSegmentMatchQuery(segment));
            }
            queryBuilder.Append("] }"); // End '$and' array

            // Return query
            return queryBuilder.ToString();
        }

        /// <summary>
        /// Generates a query that matches a user against a specific segment and checks if that segment needs to be inserted or updated on the user
        /// </summary>
        /// <param name="visitorId">Id of the visitor</param>
        /// <param name="segment">Segment to match</param>
        /// <returns>Query</returns>
        static string GenerateNeedsPersistenceQuery(string visitorId, NcbtSegment segment)
        {
            var queryBuilder = new StringBuilder();

            queryBuilder.Append("{ '$and':[ ");

            queryBuilder.Append("{ '_id': ObjectId('" + visitorId + "') }");
            if (segment.Persistence != 0)
            {
                queryBuilder.Append(",");
                queryBuilder.Append("{ '$or':[ ");
                    queryBuilder.Append(
                        "{ 'ncbt.persistedSegments.{SEGMENT}': { '$lt': {DATE} } }"
                        .Replace("{SEGMENT}", segment.Alias)
                        .Replace("{DATE}", $"ISODate(\"{DateTime.UtcNow.AddDays(-(segment.Persistence >= 0 ? segment.Persistence : 0)).ToString("O")}\")"));
                    queryBuilder.Append(",");
                    queryBuilder.Append("{ 'ncbt.persistedSegments.{SEGMENT}': { $exists: false } }".Replace("{SEGMENT}", segment.Alias));
                queryBuilder.Append("] },"); // End '$or' array
            }
            else
            {
                queryBuilder.Append(GenerateSegmentMatchQuery(segment));
            }
            queryBuilder.Append("] }"); // End '$and' array

            // Return query
            return queryBuilder.ToString();
        }

        /// <summary>
        /// Generates a query that matches against a specific segment
        /// </summary>
        /// <param name="segment">Segment to match</param>
        /// <returns>Query</returns>
        static string GenerateSegmentMatchQuery(NcbtSegment segment)
        {
            // Build query
            var queryBuilder = new StringBuilder();

            queryBuilder.Append("{ '$and':[ ");

            segment.CriterionGroups.ForEach(cg =>
            {
                queryBuilder.Append("{ '$or':[ ");

                cg.Criterions.ForEach(c =>
                {
                    queryBuilder.Append("{ '" + c.PropertyAlias + ".Value':");

                    queryBuilder.Append(GenerateCriterionMatchQuery(c));

                    queryBuilder.Append(" },"); // End criterion object. Mongo doesn't care about the extra comma on the last object in the array
                });

                queryBuilder.Append("] },"); // End '$or' array
            });

            queryBuilder.Append("] }"); // End '$and' array

            // Return query
            return queryBuilder.ToString();
        }

        /// <summary>
        /// Generates a query that matches a user against a specific segment taking specific properties into account
        /// </summary>
        /// <param name="segment">Segment to match</param>
        /// <param name="properties">Dictionary of properties to include or not</param>
        /// <returns>Query</returns>
        static string GenerateSegmentWithPropertiesMatchQuery(NcbtSegment segment, IReadOnlyDictionary<string, bool> properties)
        {
            // Build query
            var queryBuilder = new StringBuilder();

            queryBuilder.Append("{ '$and':[ ");

            properties.ForEach(p =>
            {
                if (p.Value)
                {
                    queryBuilder.Append("{ '" + p.Key + "': { $exists: true, $nin: [null] }},");
                }
                else
                {
                    queryBuilder.Append("{ '" + p.Key + "': { $exists: false }},");
                }
            });
            queryBuilder.Append(GenerateSegmentMatchQuery(segment));

            queryBuilder.Append("] }"); // End '$and' array

            // Return query
            return queryBuilder.ToString();
        }

        /// <summary>
        /// Generates a query that matches against a specific criterion
        /// </summary>
        /// <param name="criterion">Segment to match</param>
        /// <returns>Query</returns>
        static string GenerateCriterionMatchQuery(Criterion criterion)
        {
            // Append single quotes to value if datatype is string.
            // This is important when query is not a regex expression.
            string value = (criterion.Operator.DataType == DataTypeEnum.String && criterion.Operator.OperatorType != OperatorTypeEnum.MatchesRegex ? "'{VALUE}'" : "{VALUE}");
            if (criterion.Operator.DataType == DataTypeEnum.DateTime)
            {
                value = $"ISODate(\"{criterion.PropertyValue ?? DateTime.UtcNow.Date.ToString("O")}\")";
            }

            string result;
            switch (criterion.Operator.OperatorType)
            {
                case OperatorTypeEnum.Equals:
                    result = (criterion.Operator.IsInverted ? "{ '$ne': " + value + " }" : value);
                    break;
                case OperatorTypeEnum.Contains:
                    result = (criterion.Operator.IsInverted ? "{ '$not': /{VALUE}/ }" : "{ '$regex': '{VALUE}' }");
                    break;
                case OperatorTypeEnum.StartsWith:
                    result = (criterion.Operator.IsInverted ? "{ '$not': /^{VALUE}/ }" : "{ '$regex': '^{VALUE}' }");
                    break;
                case OperatorTypeEnum.EndsWith:
                    result = (criterion.Operator.IsInverted ? "{ '$not': /{VALUE}$/ }" : "{ '$regex': '{VALUE}$' }");
                    break;
                case OperatorTypeEnum.GreaterThan:
                    result = (criterion.Operator.IsInverted ? "{ '$lte': " + value + " }" : "{ '$gt': " + value + " }");
                    break;
                case OperatorTypeEnum.LessThan:
                    result = (criterion.Operator.IsInverted ? "{ '$gte': " + value + " }" : "{ '$lt': " + value + " }");
                    break;
                case OperatorTypeEnum.MatchesRegex:
                    result = (criterion.Operator.IsInverted ? "{ '$not': /" + value + "/ }" : "{ '$regex': '" + value + "' }");
                    break;
                default:
                    throw new NotImplementedException("The '" + criterion.Operator.OperatorType + "'-operator is not implemented");
            }
            bool doRegexEscape = (criterion.Operator.OperatorType != OperatorTypeEnum.MatchesRegex && (result.Contains("'$regex'") || result.Contains("'$not'")));
            // Return query
            return result.Replace("{VALUE}", (doRegexEscape ? RegexEscapeValue(criterion.PropertyValue) : criterion.PropertyValue));
        }

        static string RegexEscapeValue(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            var characters = new[] { "\\", "*", "+", "?", "|", "{", "[", "(", ")", "^", "$", ".", "#" };
            return characters.Aggregate(value, (current, character) => current.Replace(character, @"\\" + character));
        }
    }
}
