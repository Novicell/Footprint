using System;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using ncBehaviouralTargeting.Library.Configuration;
using ncBehaviouralTargeting.Library.Models;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using static ncBehaviouralTargeting.Library.Constants.BehaviouralTargetingConstants;
using System.Configuration;
using Umbraco.Core.Logging;

namespace ncBehaviouralTargeting.Library.Tracking
{
    public class VisitorIdPersister : ApplicationEventHandler
    {
        protected override void ApplicationInitialized(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            EnsureTableExists(applicationContext.DatabaseContext.Database);
        }

        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            if (ConfigurationHelper.Settings.IsDisabled || ConfigurationHelper.Settings.Sql.ElementInformation.IsPresent == false)
            {
                return;
            }

            UmbracoApplicationBase.ApplicationInit += UmbracoApplicationBase_ApplicationInit;
        }

        static void UmbracoApplicationBase_ApplicationInit(object sender, EventArgs e)
        {
            var app = (HttpApplication) sender;

            app.BeginRequest += (s, args) =>
            {
                var visitorId = GetVisitorId(app.Request.Cookies);

                if (visitorId == null)
                {
                    return;
                }

                var visitorIdValue = visitorId.ToString();
                var connectionString = ConfigurationManager.ConnectionStrings[ConfigurationHelper.Settings.Sql.ConnectionStringName].ConnectionString;

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = $"SELECT 1 FROM {Visitor.TableName} WHERE Id = @visitorId;";
                        command.Parameters.AddWithValue("@visitorId", visitorIdValue);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return;
                            }
                        }
                    }

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = $"INSERT INTO {Visitor.TableName} (Id) VALUES (@visitorId);";
                        command.Parameters.AddWithValue("@visitorId", visitorIdValue);

                        try
                        {
                            command.ExecuteNonQuery();
                        }
                        catch (SqlException ex)
                        {
                            if (ex.Message.Contains("Violation of PRIMARY KEY constraint 'PK_ncBtVisitors'."))
                            {
                                LogHelper.Debug<VisitorIdPersister>(() => ex.ToString());
                            }
                        }
                    }
                }
            };
        }

        static object GetVisitorId(HttpCookieCollection cookies)
        {
            return cookies.AllKeys.Where(x => x.Equals(NcbtVisitorIdCookieName, StringComparison.InvariantCultureIgnoreCase))
                          .Select(cookies.Get)
                          .Where(x => x != null)
                          .Select(x => x.Value)
                          .FirstOrDefault();
        }

        static void EnsureTableExists(UmbracoDatabase database)
        {
            if (!database.TableExist(Visitor.TableName))
            {
                database.CreateTable<Visitor>();
            }
        }
    }
}