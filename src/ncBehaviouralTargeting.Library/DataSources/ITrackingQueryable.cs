using System.Collections.Generic;
using ncBehaviouralTargeting.Library.Models;

namespace ncBehaviouralTargeting.Library.DataSources
{
    internal interface ITrackingQueryable
    {
        QueryResult Query(bool excludeIncompleteProfiles, bool excludeNcbtProperties, IReadOnlyCollection<Segment> segment, int? page);
    }
}
