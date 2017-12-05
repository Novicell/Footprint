using System.Web.Http;
using ncBehaviouralTargeting.Library.Models;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace ncBehaviouralTargeting.Library.ApiControllers
{
    [PluginController("ncFootprintApi")]
    public class SegmentOperatorController : UmbracoAuthorizedApiController
    {
        [HttpGet]
        public dynamic GetById(int id)
        {
            return Operator.GetById(id);
        }

        [HttpGet]
        public dynamic GetAllLight()
        {
            return Operator.GetAllLight();
        }
    }
}
