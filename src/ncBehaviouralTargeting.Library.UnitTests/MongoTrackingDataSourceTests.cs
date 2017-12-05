using System;
using System.Collections.Generic;
using MongoDB.Driver;
using Moq;
using ncBehaviouralTargeting.Library.DataSources.Mongo;
using ncBehaviouralTargeting.Library.Models;
using Xunit;
using Action = System.Action;

namespace ncBehaviouralTargeting.Library.UnitTests
{
    public class MongoTrackingDataSourceTests
    {
        readonly Mock<IMongoCollection<dynamic>> collectionMock;

        public MongoTrackingDataSourceTests()
        {
            collectionMock = new Mock<IMongoCollection<dynamic>>();
        }

        [Fact]
        public void Constructor_throws_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("collection", () => new MongoTrackingDataSource(null));
        }

        [Fact]
        public void SetVisitorProperties_throws_ArgumentNullException_when_visitorId_is_null()
        {
            var sut = new MongoTrackingDataSource(collectionMock.Object);

            Action test = () => sut.SetVisitorProperties(null, new Dictionary<string, object>());

            Assert.Throws<ArgumentNullException>(test);
        }

        [Fact]
        public void SetVisitorProperties_throws_ArgumentNullException_when_key_is_null()
        {
            var sut = new MongoTrackingDataSource(collectionMock.Object);

            Action test = () => sut.SetVisitorProperties("visitorId", null);

            Assert.Throws<ArgumentNullException>(test);
        }

        [Fact]
        public void SetVisitorProperty_throws_ArgumentNullException_when_visitorId_is_null()
        {
            var sut = new MongoTrackingDataSource(collectionMock.Object);

            Action test = () => sut.SetVisitorProperty(null, "key", "value");

            Assert.Throws<ArgumentNullException>(test);
        }

        [Fact]
        public void SetVisitorProperty_throws_ArgumentNullException_when_key_is_null()
        {
            var sut = new MongoTrackingDataSource(collectionMock.Object);

            Action test = () => sut.SetVisitorProperty("visitorId", null, "value");

            Assert.Throws<ArgumentNullException>(test);
        }

        [Fact]
        public void PushToVisitorProperty_throws_ArgumentNullException_when_visitorId_is_null()
        {
            var sut = new MongoTrackingDataSource(collectionMock.Object);

            Action test = () => sut.PushToVisitorProperty(null, "key", "value");

            Assert.Throws<ArgumentNullException>(test);
        }

        [Fact]
        public void PushToVisitorProperty_throws_ArgumentNullException_when_key_is_null()
        {
            var sut = new MongoTrackingDataSource(collectionMock.Object);

            Action test = () => sut.PushToVisitorProperty("visitorId", null, "value");

            Assert.Throws<ArgumentNullException>(test);
        }

        [Fact]
        public void AddToSegment_throws_ArgumentNullException_when_visitorId_is_null()
        {
            var sut = new MongoTrackingDataSource(collectionMock.Object);

            Action test = () => sut.AddToSegment(null, new Segment());

            Assert.Throws<ArgumentNullException>(test);
        }

        [Fact]
        public void AddToSegment_throws_ArgumentNullException_when_key_is_null()
        {
            var sut = new MongoTrackingDataSource(collectionMock.Object);

            Action test = () => sut.AddToSegment("visitorId", null);

            Assert.Throws<ArgumentNullException>(test);
        }
    }
}
