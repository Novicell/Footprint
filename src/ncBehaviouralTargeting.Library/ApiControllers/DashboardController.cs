using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using umbraco.cms.businesslogic.packager;
using Umbraco.Core.Configuration;
using Umbraco.Core.Logging;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace ncBehaviouralTargeting.Library.ApiControllers
{
    [PluginController("ncFootprintApi")]
    public class DashboardController : UmbracoAuthorizedApiController
    {
        static readonly Random Random = new Random();

        [HttpGet]
        public async Task<HttpResponseMessage> Get()
        {
            var umbracoVersion = UmbracoVersion.Current;
            var footprintVersion = InstalledPackage.GetAllInstalledPackages()
                                                   .Select(x => x.Data)
                                                   .Concat(CreatedPackage.GetAllCreatedPackages().Select(x => x.Data))
                                                   .Where(x => x.Name == "Novicell Footprint")
                                                   .Select(x => x.Version)
                                                   .FirstOrDefault();

            if (footprintVersion == null)
            {
                LogHelper.Warn<DashboardController>("Footprint version is null.");
            }

            using (var http = new HttpClient())
            {
                return await http.GetAsync($"http://novicell.io/Umbraco/Api/Documentation/Get?umbracoVersion={umbracoVersion}&packageVersion={footprintVersion}&cache={Random.Next()}");
            }
        }
    }
}
