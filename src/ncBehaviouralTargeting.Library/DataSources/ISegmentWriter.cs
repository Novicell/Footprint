namespace ncBehaviouralTargeting.Library.DataSources
{
    internal interface ISegmentWriter
    {
        void AddToSegment(string visitorId, Models.Segment segment);
    }
}
