using System;
using System.Web.Http;
using ncBehaviouralTargeting.Library.DataSources;
using ncBehaviouralTargeting.Library.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;
using NcbtSegment = ncBehaviouralTargeting.Library.Models.Segment;

namespace ncBehaviouralTargeting.Library.ApiControllers
{
    [PluginController("ncFootprintApi")]
    public class SegmentController : UmbracoAuthorizedApiController
    {
        [HttpGet]
        public dynamic GetById(int id)
        {
            return NcbtSegment.GetById(id);
        }

        [HttpGet]
        public dynamic GetLightById(int id)
        {
            return NcbtSegment.GetLightById(id);
        }

        [HttpGet]
        public dynamic GetAllLight()
        {
            return NcbtSegment.GetAllLight();
        }

        [HttpPost]
        public dynamic Save(dynamic node)
        {
            try
            {
                // Try casting to a segment
                var segment = JsonConvert.DeserializeObject<NcbtSegment>(((JObject)node).ToString());

                // Save segment
                var success = segment.Save();

                // Return updated segment
                return NcbtSegment.GetById(segment.Id);
            }
            catch(Exception e)
            {
                return e;
            }
        }

        [HttpDelete]
        public void Delete(int id)
        {
            NcbtSegment.DeleteById(id);
        }

        [HttpPost]
        public dynamic GetVisitors(dynamic node)
        {
            try
            {
                // Try casting to a segment
                var segment = JsonConvert.DeserializeObject<NcbtSegment>(((JObject)node).ToString());
                // Return all visitors matching the segment
                return new
                {
                totalNumberOfVisitors = TrackingManager.CountVisitors(),
                matchingVisitors = TrackingManager.GetVisitorsInSegment(segment.Alias)
                };
            }
            catch (Exception e)
            {
                return e;
            }
        }

        [HttpPost]
        public dynamic GetNumberOfVisitors(dynamic node)
        {
            try
            {
                // Try casting to a segment
                var segment = JsonConvert.DeserializeObject<NcbtSegment>(((JObject)node).ToString());
                // Return all visitors matching the segment
                return new
                {
                    totalNumberOfVisitors = TrackingManager.CountVisitors(),
                    numberOfmatchingVisitors = TrackingManager.CountVisitorsInSegmentInternal(node)
                };
            }
            catch (Exception e)
            {
                return e;
            }
        }
    }
}
