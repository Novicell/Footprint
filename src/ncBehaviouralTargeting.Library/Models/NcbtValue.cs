using System.Collections.Generic;

namespace ncBehaviouralTargeting.Library.Models
{
    public class NcbtValue
    {
        public bool IsNcbt { get; set; }
        public int DataTypeId { get; set; }
        public List<NcbtValueSegment> Segments { get; set; }
    }
}