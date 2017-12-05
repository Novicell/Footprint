using ncBehaviouralTargeting.Library.Models.BaseModels;
using ncBehaviouralTargeting.Library.Models.Enumerations;
using NPoco;

namespace ncBehaviouralTargeting.Library.Models
{
    [TableName("ncBtProperty")]
    internal class Property : BaseEntity<Property>
    {
        internal Property()
        {
            // Default DataType
            DataType = DataTypeEnum.String;
        }

        public string Alias { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string Examples { get; set; }
        public DataTypeEnum DataType { get; set; }
    }
}
