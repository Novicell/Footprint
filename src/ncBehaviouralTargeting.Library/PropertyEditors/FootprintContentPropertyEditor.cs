using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.PropertyEditors;

namespace ncBehaviouralTargeting.Library.PropertyEditors
{
    [PropertyEditor(PropertyEditorAlias, "Footprint Content", "/App_Plugins/ncFootprint/Editors/Views/FootprintContent.html", ValueType = "JSON", HideLabel = true)]
    public class FootprintContentPropertyEditor : PropertyEditor
    {
        internal const string ContentTypeAliasPropertyKey = "ncFootprintContentTypeAlias";

        public const string PropertyEditorAlias = "Novicell.Footprint.FootprintContent";

        private IDictionary<string, object> _defaultPreValues;
        public override IDictionary<string, object> DefaultPreValues
        {
            get { return _defaultPreValues; }
            set { _defaultPreValues = value; }
        }

        public FootprintContentPropertyEditor()
        {
            // Setup default values
            _defaultPreValues = new Dictionary<string, object>
            {
                {FootprintContentPreValueEditor.ContentTypePreValueKey, ""}
            };
        }

        #region Pre Value Editor

        protected override PreValueEditor CreatePreValueEditor()
        {
            return new FootprintContentPreValueEditor();
        }

        #endregion

        #region Value Editor

        protected override PropertyValueEditor CreateValueEditor()
        {
            return new FootprintContentPropertyValueEditor(base.CreateValueEditor());
        }

        #endregion
    }
}
