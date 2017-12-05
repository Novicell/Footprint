using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using ncBehaviouralTargeting.Library.DataSources.Mongo;
using Xunit;

namespace ncBehaviouralTargeting.Library.IntegrationTests
{
    public class MongoTrackingDataSourceTests : IClassFixture<MongoDbFixture>
    {
        readonly IMongoCollection<dynamic> collection;

        public MongoTrackingDataSourceTests(MongoDbFixture runner)
        {
            var client = new MongoClient(runner.ConnectionString);
            var database = client.GetDatabase($"ncbt_{DateTimeOffset.UtcNow.ToFileTime()}");
            collection = database.GetCollection<dynamic>("ncbt");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(42)]
        [InlineData(1337)]
        public void CountVisitors_counts_n_documents(int n)
        {
            var sut = new MongoTrackingDataSource(collection);

            for (var i = 0; i < n; i++)
            {
                collection.InsertOne(new object());
            }

            Assert.Equal(n, collection.Count(_ => true));

            var actual = sut.CountVisitors();

            Assert.Equal(n, actual);
        }

        [Theory]
        [InlineData(25)]
        [InlineData("Novicell")]
        [InlineData(true)]
        public void SetVisitorProperty_updates_document(object expected)
        {
            var visitorId = ObjectId.GenerateNewId().ToString();
            var key = "queryString";
            collection.InsertOne(GetInitialDocument());
            var sut = new MongoTrackingDataSource(collection);

            sut.SetVisitorProperty(visitorId, key, expected);

            var retrievedDocument = FindDocument(visitorId);
            var actual = retrievedDocument[key].Value;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SetVisitorProperties_updates_document()
        {
            var visitorId = ObjectId.GenerateNewId().ToString();
            collection.InsertOne(GetInitialDocument());
            var sut = new MongoTrackingDataSource(collection);
            var expected = new Dictionary<string, object>
            {
                { "queryString", "?utm_source=facebook" },
                { "other", 42 }
            };

            sut.SetVisitorProperties(visitorId, expected);

            var retrievedDocument = FindDocument(visitorId);
            foreach (var val in expected)
            {
                Assert.Equal(val.Value, retrievedDocument[val.Key].Value);
            }
        }

        IDictionary<string, dynamic> FindDocument(string visitorId)
        {
            var query = $"{{ '_id': ObjectId('{visitorId}') }}";
            var queryDocument = BsonDocument.Parse(query);
            var results = collection.Find(queryDocument);
            return results.Single();
        }

        static IDictionary<string, dynamic> GetInitialDocument()
        {
            return new Dictionary<string, dynamic>
            {
                ["queryString"] = new Dictionary<string, dynamic>
                {
                    ["Value"] = "",
                    ["Timestamp"] = "ISODate('2016-01-03T13:00:14.215Z')"
                },
                ["browser"] = new Dictionary<string, dynamic>
                {
                    ["Value"] = "Firefox",
                    ["Timestamp"] = "ISODate('2015-12-26T23:55:01.000Z')"
                }
            };
        }
    }
}
