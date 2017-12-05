using NPoco;

namespace ncBehaviouralTargeting.Library.Models.BaseModels
{
    internal abstract class BaseDbEntity
    {
        protected static Database CreateConnection { get { return new Database("umbracoDbDSN"); } }

        protected static void ExecuteStoredProcedure(string storedProcedure, Database currentConnection = null)
        {
            using (var connection = currentConnection ?? CreateConnection)
            {
                connection.Execute(";exec " + storedProcedure);
            }
        }
    }
}
