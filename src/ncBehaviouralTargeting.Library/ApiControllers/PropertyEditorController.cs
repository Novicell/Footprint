using System.Linq;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace ncBehaviouralTargeting.Library.ApiControllers
{
    [PluginController("ncFootprintApi")]
    public class PropertyEditorController : UmbracoAuthorizedApiController
    {
        public dynamic GetByAlias(string alias)
        {
            var propertyEditor =
                PropertyEditorResolver.Current.PropertyEditors.FirstOrDefault(
                    r => r.Alias == alias);

            if (propertyEditor != null)
            {
                return new
                {
                    alias = propertyEditor.Alias,
                    dataType = propertyEditor.ValueEditor.ValueType,
                    view = GetPropertyEditorViewPath(propertyEditor.ValueEditor.View),
                    valid = true
                };
            }
            else
            {
                return new
                {
                    alias = string.Empty,
                    dataType = string.Empty,
                    view = string.Empty,
                    valid = false
                };
            }
        }

        private string GetPropertyEditorViewPath(string path)
        {
            if (path.StartsWith("/"))
            {
                // This is an absolute path, so just leave it
                return path;
            }

            if (path.Contains("/"))
            {
                //This is a relative path, so just leave it
                return path;
            }

            // Umbraco view, i.e. views/propertyeditors/fileupload/fileupload.html
            return "views/propertyeditors/" + path + "/" + path + ".html";
        }
    }
}
