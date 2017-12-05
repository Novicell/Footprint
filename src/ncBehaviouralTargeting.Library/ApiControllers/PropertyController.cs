using System;
using System.Web.Http;
using ncBehaviouralTargeting.Library.Models;
using Newtonsoft.Json.Linq;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace ncBehaviouralTargeting.Library.ApiControllers
{
    [PluginController("ncFootprintApi")]
    public class PropertyController : UmbracoAuthorizedApiController
    {
        [HttpGet]
        public dynamic GetById(int id)
        {
            return Property.GetById(id);
        }

        [HttpGet]
        public dynamic GetAllLight()
        {
            return Property.GetAllLight();
        }

        [HttpPost]
        public dynamic Save(dynamic node)
        {
            try
            {
                // Try casting to a property
                var property = ((JObject)node).ToObject<Property>();

                // Save property
                property.Save();

                // Return updated property
                return Property.GetById(property.Id);
            }
            catch(Exception e)
            {
                return e;
            }
        }

        [HttpDelete]
        public void Delete(int id)
        {
            Property.DeleteById(id);
        }
    }
}
