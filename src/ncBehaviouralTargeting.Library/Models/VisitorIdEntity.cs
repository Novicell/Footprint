using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace ncBehaviouralTargeting.Library.Models
{
    [TableName(TableName)]
    [PrimaryKey(nameof(Id), autoIncrement = false)]
    internal class Visitor
    {
        public const string TableName = "ncBtVisitors";

        public Visitor()
        {
        }

        public Visitor(string id)
        {
            Id = id;
        }

        [PrimaryKeyColumn(AutoIncrement = false, Clustered = false)]
        public string Id { get; set; }
    }
}
