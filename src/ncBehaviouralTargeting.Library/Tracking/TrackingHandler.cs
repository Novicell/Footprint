using System.Web.Mvc;
using ncBehaviouralTargeting.Library.Attributes;
using Umbraco.Core;

namespace ncBehaviouralTargeting.Library.Tracking
{

    public class TrackingHandler : ApplicationEventHandler
    {
        public TrackingHandler()
        {
            RegisterGlobalFilters(GlobalFilters.Filters);
        }

        private static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            // Load footprints
            filters.Add(new FootprintTrackingFilterAttribute());
        }
    }

}
