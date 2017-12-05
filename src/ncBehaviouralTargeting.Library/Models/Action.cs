using ncBehaviouralTargeting.Library.Models.BaseModels;
using ncBehaviouralTargeting.Library.Models.Enumerations;
using NPoco;

namespace ncBehaviouralTargeting.Library.Models
{
    [TableName("ncBtAction")]
    internal class Action : BaseEntity<Action>
    {
        public string Alias { get; set; }
        public string DisplayName { get; set; }
        public int SegmentId { get; set; }
        public ActionTypeEnum ActionType { get; set; }
        public int EmailPropertyId { get; set; }
        public string EmailSubject { get; set; }
        public int EmailNodeId { get; set; }
    }
}
