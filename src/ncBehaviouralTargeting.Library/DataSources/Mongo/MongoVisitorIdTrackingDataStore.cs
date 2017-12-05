using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using MongoDB.Bson;
using ncBehaviouralTargeting.Library.Models;
using Umbraco.Core.Persistence;

namespace ncBehaviouralTargeting.Library.DataSources.Mongo
{
    internal class MongoVisitorIdTrackingDataStore : ISegmentReader, ISegmentWriter, ITrackingWriter, ITrackingAggregator
    {
        readonly MongoTrackingDataSource mongo;
        readonly Func<SqlConnection> connectionFactory;

        public MongoVisitorIdTrackingDataStore(MongoTrackingDataSource mongo, Func<SqlConnection> connectionFactory)
        {
            this.mongo = mongo;
            this.connectionFactory = connectionFactory;
        }

        string GetMongoVisitorId(string visitorId)
        {
            using (var connection = connectionFactory())
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"SELECT MongoDbId FROM {VisitorMongoMapping.DatabaseName} WHERE VisitorId = @visitorId;";
                    command.Parameters.AddWithValue("@visitorId", visitorId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return reader.GetString(0);
                        }
                    }
                }

                // Row does not exist, generate new ID and insert.
                var mongoDbId = ObjectId.GenerateNewId().ToString();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"INSERT INTO {VisitorMongoMapping.DatabaseName} (MongoDbId, VisitorId) VALUES (@mongoDbId, @visitorId);";
                    command.Parameters.AddWithValue("@mongoDbId", mongoDbId);
                    command.Parameters.AddWithValue("@visitorId", visitorId);
                    command.ExecuteNonQuery();
                }

                return mongoDbId;
            }
        }

        public bool IsVisitorInSegment(string visitorId, Segment segment)
        {
            visitorId = GetMongoVisitorId(visitorId);

            if (visitorId == null)
            {
                return false;
            }

            return mongo.IsVisitorInSegment(visitorId, segment);
        }

        public void PushToVisitorProperty(string visitorId, string key, object value)
        {
            visitorId = GetMongoVisitorId(visitorId);

            if (visitorId == null)
            {
                return;
            }

            mongo.PushToVisitorProperty(visitorId, key, value);
        }

        public void SetVisitorProperties(string visitorId, IReadOnlyDictionary<string, object> properties)
        {
            visitorId = GetMongoVisitorId(visitorId);

            if (visitorId == null)
            {
                return;
            }

            mongo.SetVisitorProperties(visitorId, properties);
        }

        public void SetVisitorProperty(string visitorId, string key, object value)
        {
            visitorId = GetMongoVisitorId(visitorId);

            if (visitorId == null)
            {
                return;
            }

            mongo.SetVisitorProperty(visitorId, key, value);
        }

        public long CountVisitors()
        {
            return mongo.CountVisitors();
        }

        public long CountVisitorsInSegment(Segment segment)
        {
            return mongo.CountVisitorsInSegment(segment);
        }

        public IReadOnlyCollection<dynamic> GetVisitorsInSegment(Segment segment)
        {
            return mongo.GetVisitorsInSegment(segment);
        }

        public IReadOnlyCollection<dynamic> GetVisitorsInSegmentWithProperties(Segment segment, IReadOnlyDictionary<string, bool> properties)
        {
            return mongo.GetVisitorsInSegmentWithProperties(segment, properties);
        }

        public void AddToSegment(string visitorId, Segment segment)
        {
            visitorId = GetMongoVisitorId(visitorId);

            if (visitorId == null)
            {
                return;
            }

            mongo.AddToSegment(visitorId, segment);
        }
    }
}