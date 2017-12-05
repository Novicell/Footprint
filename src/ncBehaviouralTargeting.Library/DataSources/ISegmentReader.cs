using NcbtSegment = ncBehaviouralTargeting.Library.Models.Segment;

namespace ncBehaviouralTargeting.Library.DataSources
{
    internal interface ISegmentReader
    {
        bool IsVisitorInSegment(string visitorId, NcbtSegment segment);
    }
}
