using System.Collections.Generic;
using NcbtSegment = ncBehaviouralTargeting.Library.Models.Segment;

namespace ncBehaviouralTargeting.Library.DataSources
{
    internal interface ITrackingAggregator
    {
        long CountVisitors();
        long CountVisitorsInSegment(NcbtSegment segment);
        IReadOnlyCollection<dynamic> GetVisitorsInSegment(NcbtSegment segment);
        IReadOnlyCollection<dynamic> GetVisitorsInSegmentWithProperties(NcbtSegment segment, IReadOnlyDictionary<string, bool> properties);
    }
}