using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;
using ncBehaviouralTargeting.Library.DataSources;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace ncBehaviouralTargeting.Library.ApiControllers
{
    [PluginController("ncFootprintApi")]
    public class QueryController : UmbracoAuthorizedApiController
    {
        [HttpGet]
        public QueryResult Search(bool excludeIncompleteProfiles = true, bool excludeNcbtProperties = false, string segmentAliases = "", int page = 0)
        {
            return QueryVisitors(excludeIncompleteProfiles, excludeNcbtProperties, segmentAliases, page);
        }

        [HttpGet]
        public HttpResponseMessage ExportAsCsv(bool excludeIncompleteProfiles = true, bool excludeNcbtProperties = false, string segmentAliases = "")
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            var results = QueryVisitors(excludeIncompleteProfiles, excludeNcbtProperties, segmentAliases, null);

            var sb = new StringBuilder();
            sb.AppendLine("sep=,");
            sb.AppendLine(string.Join(",", results.Headers.Select(x => $"\"{x}\"")));

            foreach (IDictionary<string, object> row in results.Documents)
            {
                sb.AppendLine(string.Join(",", row.Select(x => $"\"{x.Value}\"")));
            }

            response.Content = new StringContent(sb.ToString(), Encoding.UTF8, "text/csv");
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = DateTimeOffset.Now.ToFileTime() + ".csv"
            };
            return response;
        }

        static QueryResult QueryVisitors(bool excludeIncompleteProfiles, bool excludeNcbtProperties, string segmentAliases, int? page)
        {
            if (segmentAliases == null)
            {
                segmentAliases = "";
            }

            return TrackingManager.QueryVisitors(excludeIncompleteProfiles, excludeNcbtProperties, segmentAliases.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries), page);
        }
    }
}
