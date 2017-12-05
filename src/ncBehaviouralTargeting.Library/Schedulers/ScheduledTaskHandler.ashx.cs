using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;
using ncBehaviouralTargeting.Library.Configuration;
using ncBehaviouralTargeting.Library.DataSources;
using ncBehaviouralTargeting.Library.Helpers;
using ncBehaviouralTargeting.Library.Models;
using ncBehaviouralTargeting.Library.Models.Enumerations;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Web;

namespace ncBehaviouralTargeting.Library.Schedulers
{
    public class ScheduledTaskHandler : IHttpHandler
    {
        #region IHttpHandler Members

        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            LogHelper.Info(MethodBase.GetCurrentMethod().DeclaringType, "[ncBT - Scheduler] Start task");

            // Fetch all active actions
            var actions = Action.GetAllLight()
                .Where(x => x.ActionType != ActionTypeEnum.Inactive)
                .ToList();
            // Fetch all segments
            var segments = Models.Segment.GetAll().ToDictionary(x => x.Id, x => x);
            // Fetch all properties
            var properties = Property.GetAllLight().ToDictionary(x => x.Id, x => x);

            // Create UmbracoHelper
            var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);

            // Get sender email
            var senderEmail = ConfigurationHelper.Settings.Email.Sender;

            // Go through all mail actions
            foreach (var action in actions.Where(x => x.ActionType == ActionTypeEnum.Email))
            {
                // Make sure we have a valid segment
                if (!segments.ContainsKey(action.SegmentId))
                    continue;
                // Make sure we have a valid property
                if (!properties.ContainsKey(action.EmailPropertyId))
                    continue;

                // Pull segment
                var segment = segments[action.SegmentId];
                // Pull property
                var emailProperty = properties[action.EmailPropertyId];
                // Construct flag property
                var flagPropertyAlias = "ncbt.flags.action" + action.Id;

                // Fetch visitors in segment, with email property, not having the flag property
                var segmentProperties = new Dictionary<string, bool>
                {
                    {flagPropertyAlias, false},
                    {emailProperty.Alias, true}
                };
                var visitors = TrackingManager.GetVisitorsInSegmentWithProperties(segment.Alias, segmentProperties);
                LogHelper.Info(MethodBase.GetCurrentMethod().DeclaringType, "[ncBT - Scheduler] Action " + action.Alias + " matched " + visitors.Count + " visitors");

                // Get email node
                var node = umbracoHelper.TypedContent(action.EmailNodeId);

                // Go through visitors
                foreach (var visitor in visitors)
                {
                    LogHelper.Info(MethodBase.GetCurrentMethod().DeclaringType, "[ncBT - Scheduler] - Visitor: " + visitor._id);

                    // Get visitor email
                    var visitorEmail = string.Empty;
                    foreach (var visitorProperty in visitor)
                    {
                        // Find property
                        if (visitorProperty.Key == emailProperty.Alias)
                            visitorEmail = visitorProperty.Value.Value.ToString();
                    }
                    // Make sure we're valid
                    if (!visitorEmail.IsNullOrWhiteSpace() && node != null)
                    {
                        // Render mail
                        var emailBody = NcbtEmailHelper.RenderEmail(umbracoHelper, node, visitor);
                        // Send mail to visitor
                        NcbtEmailHelper.SendMail(senderEmail, visitorEmail, action.EmailSubject, emailBody, true);
                        // Add flag property
                        TrackingManager.SetVisitorProperty(visitor._id.ToString(), flagPropertyAlias, "email sent");
                    }
                }
            }

            LogHelper.Info(MethodBase.GetCurrentMethod().DeclaringType, "[ncBT - Scheduler] End task");
        }

        #endregion
    }
}
