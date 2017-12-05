using System.Collections.Generic;

namespace ncBehaviouralTargeting.Library.DataSources
{
    internal interface ITrackingWriter
    {
        void PushToVisitorProperty(string visitorId, string key, object value);
        void SetVisitorProperties(string visitorId, IReadOnlyDictionary<string, object> properties);
        void SetVisitorProperty(string visitorId, string key, object value);
    }
}