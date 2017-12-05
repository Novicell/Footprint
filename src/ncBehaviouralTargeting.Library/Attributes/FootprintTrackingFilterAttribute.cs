using System.Web.Mvc;
using ncBehaviouralTargeting.Library.Tracking;

namespace ncBehaviouralTargeting.Library.Attributes
{
    public class FootprintTrackingFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.IsChildAction)
            {
                CurrentVisitor.SetFootprints(filterContext.HttpContext);
            }
        }
    }
}
