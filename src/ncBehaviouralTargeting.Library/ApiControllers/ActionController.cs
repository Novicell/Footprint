using System;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;
using Action = ncBehaviouralTargeting.Library.Models.Action;

namespace ncBehaviouralTargeting.Library.ApiControllers
{
    [PluginController("ncFootprintApi")]
    public class ActionController : UmbracoAuthorizedApiController
    {
        [HttpGet]
        public dynamic GetById(int id)
        {
            return Action.GetById(id);
        }

        [HttpGet]
        public dynamic GetAllLight()
        {
            return Action.GetAllLight();
        }

        [HttpPost]
        public dynamic Save(dynamic node)
        {
            try
            {
                // Try casting to an action
                var action = ((JObject)node).ToObject<Action>();

                // Save action
                action.Save();

                // Return updated action
                return Action.GetById(action.Id);
            }
            catch(Exception e)
            {
                return e;
            }
        }

        [HttpDelete]
        public void Delete(int id)
        {
            Action.DeleteById(id);
        }
    }
}
