using Umbraco.Core.Models;
using Umbraco.Web;

namespace ncBehaviouralTargeting.Library.Models
{
    internal class NcbtEmailModel
    {
        public IPublishedContent Content { get; set; }
        public dynamic Visitor { get; set; }
        public UmbracoHelper UmbracoHelper { get; set; }
    }
}
