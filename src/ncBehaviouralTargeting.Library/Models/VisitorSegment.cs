using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace ncBehaviouralTargeting.Library.Models
{
    [TableName(TableName)]
    [PrimaryKey(PrimaryKey, autoIncrement = false)]
    internal class VisitorSegment
    {
        public const string TableName = "ncBtVisitorSegment";
        public const string PrimaryKey = nameof (VisitorId) + "," + nameof (SegmentAlias);

        public VisitorSegment()
        {
        }

        public VisitorSegment(string visitorId, string segmentAlias, DateTime createdUtc)
        {
            VisitorId = visitorId;
            SegmentAlias = segmentAlias;
            CreatedUtc = createdUtc;
        }

        [PrimaryKeyColumn(AutoIncrement = false, Clustered = false, OnColumns = PrimaryKey)]
        public string VisitorId { get; set; }

        [PrimaryKeyColumn(AutoIncrement = false, Clustered = false, OnColumns = PrimaryKey)]
        public string SegmentAlias { get; set; }

        public DateTime CreatedUtc { get; set; }
    }
}