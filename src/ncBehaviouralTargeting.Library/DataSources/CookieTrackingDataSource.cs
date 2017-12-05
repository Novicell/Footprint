using System;
using System.Linq;
using System.Web;
using NcbtSegment = ncBehaviouralTargeting.Library.Models.Segment;

namespace ncBehaviouralTargeting.Library.DataSources
{
    internal class CookieTrackingDataSource : ISegmentReader, ISegmentWriter
    {
        public const string CookieNameFormat = "ncbt_segment_{0}";

        readonly Func<HttpContextBase> httpContextAccessor;

        public CookieTrackingDataSource(Func<HttpContextBase> httpContextAccessor)
        {
            if (httpContextAccessor == null)
            {
                throw new ArgumentNullException(nameof(httpContextAccessor));
            }

            this.httpContextAccessor = httpContextAccessor;
        }

        public bool IsVisitorInSegment(string visitorId, NcbtSegment segment)
        {
            if (visitorId == null)
            {
                throw new ArgumentNullException(nameof(visitorId));
            }

            if (segment == null)
            {
                throw new ArgumentNullException(nameof(segment));
            }

            var httpContext = httpContextAccessor();
            var requestCookies = httpContext.Request.Cookies;
            var cookieName = string.Format(CookieNameFormat, segment.Alias);
            return requestCookies.AllKeys.Any(x => x == cookieName);
        }

        public void AddToSegment(string visitorId, NcbtSegment segment)
        {
            if (visitorId == null)
            {
                throw new ArgumentNullException(nameof(visitorId));
            }

            AddToSegment(httpContextAccessor(), segment);
        }

        public static void AddToSegment(HttpContextBase httpContext, NcbtSegment segment)
        {
            if (segment == null)
            {
                throw new ArgumentNullException(nameof(segment));
            }

            var responseCookies = httpContext.Response.Cookies;
            var cookieName = string.Format(CookieNameFormat, segment.Alias);
            var cookie = new HttpCookie(cookieName);

            if (segment.Persistence == 0)
            {
                // Persistence: None.
                cookie.Expires = DateTime.MinValue; // Session.
            }
            else if (segment.Persistence < 0)
            {
                // Persistence: Permanent.
                cookie.Expires = DateTime.UtcNow.AddYears(5);
            }
            else if (segment.Persistence > 0)
            {
                // Persistence: X days.
                cookie.Expires = DateTime.UtcNow.AddDays(segment.Persistence);

                if (httpContext.Request.Cookies[cookieName] != null)
                {
                    // Don't update existing cookies that expire after X days.
                    return;
                }
            }

            responseCookies.Add(cookie);
        }
    }
}
