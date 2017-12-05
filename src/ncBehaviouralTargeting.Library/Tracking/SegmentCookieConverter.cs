using System;
using System.Linq;
using System.Web;
using ncBehaviouralTargeting.Library.DataSources;
using Umbraco.Core;

namespace ncBehaviouralTargeting.Library.Tracking
{
    /// <summary>
    /// ApplicationEventHandler for migrating old versions of Footprint.
    /// Old versions used a cookie called ncbt_segments that contained all the segments a visitor was in.
    /// In newer versions, we use a cookie per segment with a separate expiration date, so here we split
    /// the old cookie to the new cookies when a request begins.
    /// </summary>
    public class SegmentCookieConverter : ApplicationEventHandler
    {
        const string NcbtSegmentsCookieName = "ncbt_segments";

        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            UmbracoApplicationBase.ApplicationInit += UmbracoApplicationBase_ApplicationInit;
        }

        static void UmbracoApplicationBase_ApplicationInit(object sender, EventArgs e)
        {
            var app = (HttpApplication) sender;

            app.BeginRequest += (s, args) =>
            {
                var cookies = app.Request.Cookies;
                var segmentAliases = cookies.AllKeys
                                            .Where(x => x.Equals(NcbtSegmentsCookieName))
                                            .Select(cookies.Get)
                                            .Where(x => x != null)
                                            .SelectMany(x => x.Values.AllKeys)
                                            .Where(x => x != null)
                                            .Distinct();

                foreach (var segmentAlias in segmentAliases)
                {
                    var segment = Models.Segment.GetByAlias(segmentAlias);

                    if (segment == null)
                    {
                        continue;
                    }

                    CookieTrackingDataSource.AddToSegment(new HttpContextWrapper(app.Context), segment);
                }

                // Expire the old cookie.
                app.Response.Cookies.Add(new HttpCookie(NcbtSegmentsCookieName) { Expires = DateTime.UtcNow.AddDays(-1) });
            };
        }
    }
}
