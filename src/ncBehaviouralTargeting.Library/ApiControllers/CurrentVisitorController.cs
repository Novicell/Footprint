using ncBehaviouralTargeting.Library.Tracking;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web.Http;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace ncBehaviouralTargeting.Library.ApiControllers
{
    [PluginController("ncFootprintApi")]
    public class CurrentVisitorController : UmbracoApiController
    {
        [HttpPost]
        public HttpResponseMessage AddToSegment(string segmentAlias)
        {
            if (string.IsNullOrEmpty(segmentAlias))
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            CurrentVisitor.AddToSegment(segmentAlias);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [HttpPost]
        public async Task SetProperties(FormDataCollection form)
        {
            IDictionary<string, object> values;

            if (form == null)
            {
                // Try as JSON.
                using (var stream = await Request.Content.ReadAsStreamAsync())
                {
                    stream.Seek(0, SeekOrigin.Begin);

                    using (var reader = new StreamReader(stream))
                    {
                        var json = await reader.ReadToEndAsync();
                        values = JsonConvert.DeserializeObject<IDictionary<string, object>>(json);
                    }
                }
            }
            else
            {
                values = form.ToDictionary(x => x.Key, x => (object) x.Value);
            }

            CurrentVisitor.SetProperties(values);
        }

        [HttpGet]
        public bool IsInSegment(string segmentAlias)
        {
            return CurrentVisitor.IsInSegment(segmentAlias);
        }

        [HttpPost]
        public void SetId(string visitorId)
        {
            CurrentVisitor.VisitorId = visitorId;
        }
    }
}
