using System.Collections.Generic;
using System.Linq;
using ncBehaviouralTargeting.Library.Helpers;
using ncBehaviouralTargeting.Library.Umbraco;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.Editors;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;
using Umbraco.Web.PropertyEditors;

namespace ncBehaviouralTargeting.Library.PropertyEditors
{
    internal class FootprintContentPropertyValueEditor : PropertyValueEditorWrapper
    {
        public FootprintContentPropertyValueEditor(PropertyValueEditor wrapped)
            : base(wrapped)
        {
        }

        internal ServiceContext Services
        {
            get { return ApplicationContext.Current.Services; }
        }
    }
}