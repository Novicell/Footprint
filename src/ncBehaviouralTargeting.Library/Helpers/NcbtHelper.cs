using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using ncBehaviouralTargeting.Library.PropertyEditors;
using Newtonsoft.Json.Linq;
using Umbraco.Core;
using Umbraco.Core.Models;

namespace ncBehaviouralTargeting.Library.Helpers
{

    internal static class NcbtHelper
    {
        internal static IDataTypeDefinition GetTargetDataTypeDefinition(int dataTypeId)
        {
            // Get data type service
            var dataTypeService = ApplicationContext.Current.Services.DataTypeService;

            // Get prevalues
            var preValues = dataTypeService.GetPreValuesCollectionByDataTypeId(dataTypeId).PreValuesAsDictionary;
            var contentTypeJson = preValues["contentType"].Value;
            var contentType = Json.Decode(contentTypeJson);
            var dataType = (IEnumerable<IDataTypeDefinition>)dataTypeService.GetDataTypeDefinitionByPropertyEditorAlias(contentType.selectedEditor);
            return dataType.First();
        }

        public static PreValueCollection GetPreValuesCollectionByDataTypeId(int dtdId)
        {
            var preValueCollection = (PreValueCollection)ApplicationContext.Current.ApplicationCache.RuntimeCache.GetCacheItem(
                string.Concat("Novicell.Footprint.GetPreValuesCollectionByDataTypeId_", dtdId),
                () => ApplicationContext.Current.Services.DataTypeService.GetPreValuesCollectionByDataTypeId(dtdId));

            return preValueCollection;
        }

        public static string GetContentTypeAliasFromItem(JObject item)
        {
            var contentTypeAliasProperty = item[FootprintContentPropertyEditor.ContentTypeAliasPropertyKey];
            if (contentTypeAliasProperty == null)
            {
                return null;
            }

            return contentTypeAliasProperty.ToObject<string>();
        }

        public static IContentType GetContentTypeFromItem(JObject item)
        {
            var contentTypeAlias = GetContentTypeAliasFromItem(item);
            if (string.IsNullOrEmpty(contentTypeAlias))
            {
                return null;
            }

            return ApplicationContext.Current.Services.ContentTypeService.GetContentType(contentTypeAlias);
        }

        //http://stackoverflow.com/questions/2670004/ip-address-of-the-client-machine
        //http://stackoverflow.com/questions/200527/userhostaddress-gives-wrong-ips
        public static string GetClientIp()
        {
            if (HttpContext.Current == null) 
                return "";

            // If Cloud Flare 
            string clientIp = HttpContext.Current.Request.ServerVariables["HTTP_CF_CONNECTING_IP"];

            if (string.IsNullOrEmpty(clientIp))
            {
                clientIp = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                // IF PROXY
                if (!string.IsNullOrEmpty(clientIp))
                {
                    string[] forwardedIps = clientIp.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    clientIp = forwardedIps[forwardedIps.Length - 1];
                }
                else // Normal 
                {
                    clientIp = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }
            }

            return clientIp;
        }
    }
}
