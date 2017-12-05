using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Reflection;
using ncBehaviouralTargeting.Library.Models;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace ncBehaviouralTargeting.Library.Helpers
{
    internal static class NcbtEmailHelper
    {
        internal static bool SendMail(string fromMail, string toMail, string subject, string body, bool isHtml = false, List<Attachment> attachmentCollection = null, bool reThrowEx = true)
        {
            try
            {
                var mailMessage = new MailMessage(fromMail.Trim(), toMail.Trim())
                {
                    Subject = subject,
                    IsBodyHtml = isHtml,
                    Body = body
                };

                if (attachmentCollection != null)
                {
                    foreach (Attachment attachment in attachmentCollection)
                    {
                        mailMessage.Attachments.Add(attachment);
                    }
                }

                var smtpClient = new SmtpClient();
                smtpClient.Send(mailMessage);
                return true;
            }
            catch (Exception e)
            {
                LogHelper.Error(MethodBase.GetCurrentMethod().DeclaringType, "Error sending mail.", e);
                if (reThrowEx)
                    throw e;
                return false;
            }
        }

        internal static string RenderEmail(UmbracoHelper umbracoHelper, IPublishedContent node, dynamic visitor)
        {
            // Fetch view from node
            var nodeView = node.GetTemplateAlias();

            // Make composite model for view containing node and headers passed
            var model = new NcbtEmailModel
            {
                Content = node,
                Visitor = visitor,
                UmbracoHelper = umbracoHelper
            };

            // Render view
            return RenderRazorView(nodeView, model);
        }

        internal static string RenderRazorView(string viewAlias, NcbtEmailModel model)
        {
            var razorHelper = new RazorHelper();
            return razorHelper.RenderView("~/Views/" + viewAlias + ".cshtml", model);
        }
    }
}
