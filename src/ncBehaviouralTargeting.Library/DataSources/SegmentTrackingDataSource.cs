//using System;
//using System.Collections.Generic;
//using Segment.Model;

//namespace ncBehaviouralTargeting.Library.DataSources
//{
//    internal class SegmentTrackingDataSource : ITrackingWriter, ISegmenter
//    {
//        readonly Func<Segment.Client> clientFactory;

//        public SegmentTrackingDataSource(string writeKey)
//        {
//            clientFactory = () => new Segment.Client(writeKey);
//        }

//        public void PushToVisitorProperty(string visitorId, string key, object value)
//        {
//            throw new InvalidOperationException();
//        }

//        public void SetVisitorProperties(string visitorId, IReadOnlyDictionary<string, object> properties)
//        {
//            var traits = new Traits();

//            foreach (var property in properties)
//            {
//                traits.Add(property.Key, property.Value);
//            }

//            using (var client = clientFactory())
//            {
//                client.Identify(visitorId, traits);
//            }
//        }

//        public void SetVisitorProperty(string visitorId, string key, object value)
//        {
//            SetVisitorProperties(visitorId, new Dictionary<string, object> { [key] = value });
//        }

//        public void AddToSegment(string visitorId, Models.Segment segment)
//        {
//            var traits = new Traits
//            {
//                { "name", segment.DisplayName }
//            };

//            using (var client = clientFactory())
//            {
//                client.Group(visitorId, segment.Alias, traits);
//            }
//        }
//    }
//}
