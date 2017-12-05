using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ncBehaviouralTargeting.Library.Attributes;
using ncBehaviouralTargeting.Library.DataSources;
using ncBehaviouralTargeting.Library.DataSources.Mongo;

namespace ncBehaviouralTargeting.Website
{
    public class Global : Umbraco.Web.UmbracoApplication
    {
        public void Init(HttpApplication application)
        {
            //RegisterGlobalFilters(GlobalFilters.Filters);
        }

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            // Load footprints
            filters.Add(new FootprintTrackingFilterAttribute());
        }
    }
}