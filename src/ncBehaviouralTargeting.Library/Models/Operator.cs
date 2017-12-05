using ncBehaviouralTargeting.Library.Models.BaseModels;
using ncBehaviouralTargeting.Library.Models.Enumerations;
using NPoco;

namespace ncBehaviouralTargeting.Library.Models
{
    [TableName("ncBtOperator")]
    internal class Operator : BaseEntity<Operator>
    {
        public string DisplayName { get; set; }
        public bool IsInverted { get; set; }
        public OperatorTypeEnum OperatorType { get; set; }
        public DataTypeEnum DataType { get; set; }
    }
}
