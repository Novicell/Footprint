using System;
using System.Collections.Generic;
using System.Web;
using Umbraco.Web;

namespace ncBehaviouralTargeting.Library.DataSources
{
    internal class RequestTrackingDataSource : ISegmentReader
    {
        const string NcbtPropertyPrefix = "ncbt.";

        readonly Func<UmbracoContext> umbracoContextAccessor;

        public RequestTrackingDataSource(Func<UmbracoContext> umbracoContextAccessor)
        {
            this.umbracoContextAccessor = umbracoContextAccessor;
        }

        public bool IsVisitorInSegment(string visitorId, Models.Segment segment)
        {
            var umbracoContext = umbracoContextAccessor();
            var httpContext = umbracoContext.HttpContext;
            var request = httpContext.Request;

            var properties = new Dictionary<string, object>
            {
                { NcbtPropertyPrefix + "isMobile", request.Browser.IsMobileDevice },
                { NcbtPropertyPrefix + "platform", request.Browser.Platform },
                { NcbtPropertyPrefix + "browser", request.Browser.Browser },
                { NcbtPropertyPrefix + "userAgent", request.UserAgent },
                { NcbtPropertyPrefix + "userIp", GetClientIp(request) },
                { NcbtPropertyPrefix + "userDnsName", request.UserHostName },
                { NcbtPropertyPrefix + "queryString", request.Url?.Query },
                { NcbtPropertyPrefix + "httpReferrer", request.UrlReferrer?.AbsoluteUri ?? "" }
            };

            if (request.Browser.IsMobileDevice)
            {
                properties.Add(NcbtPropertyPrefix + "mobileDeviceManufacturer", request.Browser.MobileDeviceManufacturer);
                properties.Add(NcbtPropertyPrefix + "mobileDeviceModel", request.Browser.MobileDeviceModel);
            }

            if (umbracoContext.PageId.HasValue)
            {
                properties.Add(NcbtPropertyPrefix + "pageId", UmbracoContext.Current.PageId);
            }

            return segment.Match(properties);
        }

        static string GetClientIp(HttpRequestBase request)
        {
            // CloudFlare.
            var clientIp = request.ServerVariables["HTTP_CF_CONNECTING_IP"];

            if (!string.IsNullOrEmpty(clientIp))
            {
                return clientIp;
            }

            clientIp = request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            // Normal.
            if (string.IsNullOrEmpty(clientIp))
            {
                clientIp = request.ServerVariables["REMOTE_ADDR"];
            }
            else
            {
                // Proxy.
                var forwardedIps = clientIp.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                clientIp = forwardedIps[forwardedIps.Length - 1];
            }

            return clientIp;
        }
    }
}