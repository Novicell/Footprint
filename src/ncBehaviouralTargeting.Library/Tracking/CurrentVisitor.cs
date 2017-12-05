using System;
using System.Collections.Generic;
using System.Web;
using ncBehaviouralTargeting.Library.DataSources;
using MongoDB.Bson;
using ncBehaviouralTargeting.Library.Helpers;
using ncBehaviouralTargeting.Library.Models;
using Umbraco.Core;
using Umbraco.Web;

namespace ncBehaviouralTargeting.Library.Tracking
{
    public class CurrentVisitor
    {
        public const string NcbtPropertyPrefix = "ncbt.";
        public const string NcbtCookieName = "ncbt_visitorId";

        static HttpRequest Request => HttpContext.Current.Request;
        static HttpResponse Response => HttpContext.Current.Response;

        static readonly List<string> Crawlers = new List<string>
        {
            // Web crawlers
            "googlebot","bingbot","yandexbot","ahrefsbot","msnbot","linkedinbot","exabot","compspybot",
            "yesupbot","paperlibot","tweetmemebot","semrushbot","gigabot","voilabot","adsbot-google",
            "botlink","alkalinebot","araybot","undrip bot","borg-bot","boxseabot","yodaobot","admedia bot",
            "ezooms.bot","confuzzledbot","coolbot","internet cruiser robot","yolinkbot","diibot","musobot",
            "dragonbot","elfinbot","wikiobot","twitterbot","contextad bot","hambot","iajabot","news bot",
            "irobot","socialradarbot","ko_yappo_robot","skimbot","psbot","rixbot","seznambot","careerbot",
            "simbot","solbot","mail.ru_bot","spiderbot","blekkobot","bitlybot","techbot","void-bot",
            "vwbot_k","diffbot","friendfeedbot","archive.org_bot","woriobot","crystalsemanticsbot","wepbot",
            "spbot","tweetedtimes bot","mj12bot","who.is bot","psbot","robot","jbot","bbot","bot",
            // Umbraco crawlers
            "seo-checker"
        };

        static bool IsCrawler
        {
            get
            {
                if (Request.UserAgent == null)
                {
                    return false;
                }

                var userAgent = Request.UserAgent.ToLower();
                return Crawlers.Exists(x => userAgent.Contains(x));
            }
        }

        public static string VisitorId
        {
            get
            {
                var visitorIdCookie = Request.Cookies[NcbtCookieName];
                var visitorId = visitorIdCookie?.Value;

                if (visitorId == null)
                {
                    visitorId = Guid.NewGuid().ToString("N");
                    VisitorId = visitorId;
                }

                return visitorId;
            }
            set
            {
                var httpCookie = Response.Cookies[NcbtCookieName];

                if (httpCookie != null)
                {
                    httpCookie.Expires = DateTime.Now.AddYears(5);
                    httpCookie.Value = value;
                }
                else
                {
                    var cookie = new HttpCookie(NcbtCookieName, value);
                    cookie.Expires = DateTime.Now.AddYears(5);
                    Response.Cookies.Add(cookie);
                }
            }
        }

        public static void AddToSegment(string segmentAlias)
        {
            if (segmentAlias == null)
            {
                throw new ArgumentNullException(nameof(segmentAlias));
            }

            var segment = Segment.GetByAlias(segmentAlias);

            if (segment == null)
            {
                return;
            }

            TrackingManager.AddVisitorToSegment(VisitorId, segment);
        }

        /// <summary>
        /// Saves multiple values on the visitor, if their key does not start with NcbtPropertyPrefix.
        /// </summary>
        /// <param name="values"></param>
        public static void SetProperties(IDictionary<string, object> values)
        {
            if (IsCrawler)
            {
                return;
            }

            var clone = new Dictionary<string, object>(values);

            foreach (var pair in clone)
            {
                if (pair.Key.StartsWith(NcbtPropertyPrefix))
                {
                    clone.Remove(pair.Key);
                }
            }

            var currentVisitorId = TrackingManager.SetVisitorProperty(VisitorId, clone);

            if (!currentVisitorId.IsNullOrWhiteSpace())
            {
                VisitorId = currentVisitorId;
            }
        }

        /// <summary>
        /// Saves a single value on the visitor, if key does not start with NcbtPropertyPrefix.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetProperty(string key, object value)
        {
            if (IsCrawler)
            {
                return;
            }

            if (!key.StartsWith(NcbtPropertyPrefix))
            {
                var currentVisitorId = TrackingManager.SetVisitorProperty(VisitorId, key, value);

                if (!currentVisitorId.IsNullOrWhiteSpace())
                {
                    VisitorId = currentVisitorId;
                }
            }
        }

        /// <summary>
        /// Saves multiple values on the visitor.
        /// </summary>
        /// <param name="values"></param>
        internal static void SetInternalProperty(Dictionary<string, object> values)
        {
            if (IsCrawler) return;

            var currentVisitorId = TrackingManager.SetVisitorProperty(VisitorId, values);
            if (!currentVisitorId.IsNullOrWhiteSpace())
            {
                VisitorId = currentVisitorId;
            }
        }

        /// <summary>
        /// Saves a single value on the visitor.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        internal static void SetInternalProperty(string key, object value)
        {
            if (IsCrawler) return;

            var currentVisitorId = TrackingManager.SetVisitorProperty(VisitorId, key, value);
            if (!currentVisitorId.IsNullOrWhiteSpace())
            {
                VisitorId = currentVisitorId;
            }
        }

        /// <summary>
        /// Adds a value to the array of values on the visitor
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        internal static void PushToVisitorProperty(string key, object value)
        {
            if (IsCrawler) return;

            var currentVisitorId = TrackingManager.PushToVisitorProperty(VisitorId, key, value);
            if (!currentVisitorId.IsNullOrWhiteSpace())
            {
                VisitorId = currentVisitorId;
            }
        }

        internal static void SetVisitorId(string value)
        {
            if (IsCrawler) return;

            VisitorId = value;
        }

        /// <summary>
        /// Checks to see if the visitor is in the segment.
        /// </summary>
        /// <param name="segmentAlias">The alias of the segment</param>
        /// <returns>True if the visitor matches the segment</returns>
        public static bool IsInSegment(string segmentAlias)
        {
            if (IsCrawler) return false;
            if (string.IsNullOrEmpty(VisitorId)) return false;

            return TrackingManager.IsVisitorInSegment(VisitorId, segmentAlias);
        }

        /// <summary>
        /// Save visitor footprint data to the database.
        /// This method should be called on every page load
        /// </summary>
        /// <param name="httpContext">The HTTP context for getting footprints and setting the cookie. If null, HttpContext.Current will be used</param>
        internal static void SetFootprints(HttpContextBase httpContext)
        {
            if (Request.Url.PathAndQuery.StartsWith("/umbraco/"))
            {
                return;
            }

            if (IsCrawler)
            {
                return;
            }

            if (string.IsNullOrEmpty(Request.UserAgent) && Request.Browser.Platform == "Unknown")
            {
                return;
            }

            var values = new Dictionary<string, object>
            {
                { NcbtPropertyPrefix + "isMobile", Request.Browser.IsMobileDevice },
                { NcbtPropertyPrefix + "platform", Request.Browser.Platform },
                { NcbtPropertyPrefix + "browser", Request.Browser.Browser },
                { NcbtPropertyPrefix + "userAgent", Request.UserAgent },
                { NcbtPropertyPrefix + "userIp", NcbtHelper.GetClientIp() },
                { NcbtPropertyPrefix + "userDnsName", Request.UserHostName },
                { NcbtPropertyPrefix + "queryString", Request.Url.Query },
                { NcbtPropertyPrefix + "httpReferrer", Request.UrlReferrer?.AbsoluteUri ?? "" }
            };

            if (Request.Browser.IsMobileDevice)
            {
                values.Add(NcbtPropertyPrefix + "mobileDeviceManufacturer", Request.Browser.MobileDeviceManufacturer);
                values.Add(NcbtPropertyPrefix + "mobileDeviceModel", Request.Browser.MobileDeviceModel);
            }

            if (UmbracoContext.Current != null && UmbracoContext.Current.PageId.HasValue)
            {
                values.Add(NcbtPropertyPrefix + "pageId", UmbracoContext.Current.PageId);
            }

            SetInternalProperty(values);
        }
    }
}
