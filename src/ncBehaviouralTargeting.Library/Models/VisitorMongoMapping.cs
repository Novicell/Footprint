using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace ncBehaviouralTargeting.Library.Models
{
    [TableName(DatabaseName)]
    [PrimaryKey(nameof (MongoDbId), autoIncrement = false)]
    internal class VisitorMongoMapping
    {
        public const string DatabaseName = "ncBtVisitorMongo";

        public VisitorMongoMapping()
        {
        }

        public VisitorMongoMapping(string mongoDbId, string visitorId)
        {
            MongoDbId = mongoDbId;
            VisitorId = visitorId;
        }

        [PrimaryKeyColumn(AutoIncrement = false, Clustered = false)]
        public string MongoDbId { get; set; }

        [Index(IndexTypes.UniqueNonClustered)]
        public string VisitorId { get; set; }
    }
}