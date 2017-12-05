using System;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using ncBehaviouralTargeting.Library.Configuration;
using ncBehaviouralTargeting.Library.Constants;
using ncBehaviouralTargeting.Library.Models;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence;
using static ncBehaviouralTargeting.Library.Constants.BehaviouralTargetingConstants;

namespace ncBehaviouralTargeting.Library.Tracking
{
    // The point of this class is migration/independence from MongoDB.
    // Right now, the MongoTrackingDataSource has a hardcoded assumption that the visitorId is a MongoDB ObjectId.
    // There are some sites with the beta version installed that have had a lot of visitors, and it would be a shame
    // to just "forget" their IDs, so we convert any ObjectId-visitorId to a GUID and store the relation in the database.
    // (The reason we don't just use ObjectId instead of GUID, since we have a MongoDB assembly reference anyway, is that
    // the client/customer of this package is supposed to be able to set a visitorId himself. If he does change the ID,
    // he holds the responsibility to change the MongoDB mapping in the database.)
    public class VisitorIdExchanger : ApplicationEventHandler
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
                var mongoDbId = GetVisitorIdCookie(app.Request.Cookies);

                ObjectId objectId;
                if (ObjectId.TryParse(mongoDbId, out objectId))
                {
                    var newId = Guid.NewGuid().ToString("N");
                    var mapping = new VisitorMongoMapping(objectId.ToString(), newId);

                    try
                    {
                        var db = ApplicationContext.Current.DatabaseContext.Database;
                        var existing = db.SingleOrDefault<VisitorMongoMapping>(primaryKey: mapping.MongoDbId);

                        if (existing == null)
                        {
                            db.Insert(mapping);
                        }
                        else
                        {
                            newId = existing.VisitorId;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error<VisitorIdExchanger>("Inserting visitorId failed.", ex);
                        return;
                    }

                    ReplaceVisitorIdCookie(app.Response.Cookies, newId);
                }
            };
        }

        static void EnsureTableExists(UmbracoDatabase database)
        {
            if (!database.TableExist(VisitorMongoMapping.DatabaseName))
            {
                database.CreateTable<VisitorMongoMapping>();
            }
        }

        static string GetVisitorIdCookie(HttpCookieCollection cookies)
        {
            return cookies.AllKeys
                          .Where(x => x.Equals(BehaviouralTargetingConstants.NcbtVisitorIdCookieName, StringComparison.InvariantCultureIgnoreCase))
                          .Select(cookies.Get)
                          .Where(x => x != null)
                          .Select(x => x.Value)
                          .FirstOrDefault();
        }

        static void ReplaceVisitorIdCookie(HttpCookieCollection responseCookies, string visitorId)
        {
            responseCookies.Remove(NcbtVisitorIdCookieName);
            responseCookies.Add(new HttpCookie(NcbtVisitorIdCookieName, visitorId)
            {
                Expires = DateTime.UtcNow.AddYears(10)
            });
        }
    }
}
