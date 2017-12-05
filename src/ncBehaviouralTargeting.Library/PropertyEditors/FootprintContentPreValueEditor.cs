using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.PropertyEditors;

namespace ncBehaviouralTargeting.Library.PropertyEditors
{
    internal class FootprintContentPreValueEditor : PreValueEditor
    {
        internal const string ContentTypePreValueKey = "contentType";
        internal const string ItemsPreValueKey = "items-";

        [PreValueField(ContentTypePreValueKey, "Content property editor", "/App_Plugins/ncFootprint/Editors/Views/FootprintContent.prevalues.html", HideLabel = true)]
        public string[] ContentTypes { get; set; }

        public override IDictionary<string, object> ConvertDbToEditor(IDictionary<string, object> defaultPreVals, PreValueCollection persistedPreVals)
        {
            // Check if we have multivalue prevalues
            if (persistedPreVals.PreValuesAsDictionary.ContainsKey(ContentTypePreValueKey))
            {
                var multiValuePreValues =
                    persistedPreVals.PreValuesAsDictionary.Where(x => x.Key.StartsWith(ItemsPreValueKey)).ToList();
                dynamic contentTypePreValue =
                    JsonConvert.DeserializeObject<dynamic>(persistedPreVals.PreValuesAsDictionary[ContentTypePreValueKey].Value);

                foreach (var preValue in multiValuePreValues)
                {
                    var ids = preValue.Key.Split('-');
                    var preValueId = Convert.ToInt32(ids[1]);
                    var itemId = Convert.ToInt32(ids[2]);
                    contentTypePreValue.preValues[preValueId].value[itemId].value = preValue.Value.Value;
                    contentTypePreValue.preValues[preValueId].value[itemId].sortOrder = preValue.Value.SortOrder;
                    contentTypePreValue.preValues[preValueId].value[itemId].id = preValue.Value.Id;
                }
                multiValuePreValues.ForEach(x => persistedPreVals.PreValuesAsDictionary.Remove(x));
                persistedPreVals.PreValuesAsDictionary[ContentTypePreValueKey].Value = JsonConvert.SerializeObject(contentTypePreValue);
            }

            return base.ConvertDbToEditor(defaultPreVals, persistedPreVals);
        }

        public override IDictionary<string, PreValue> ConvertEditorToDb(IDictionary<string, object> editorValue, PreValueCollection currentValue)
        {
            if (editorValue.ContainsKey(ContentTypePreValueKey)
                && editorValue[ContentTypePreValueKey] != null)
            { 
                // Check if we have a multivalue prevalue type
                JArray preValuesJArray = ((dynamic)editorValue[ContentTypePreValueKey]).preValues;
                var preValues = preValuesJArray.ToObject<List<dynamic>>();
                for (var i = 0; i < preValues.Count; i++)
                {
                    var preValue = preValues[i];
                    if (preValue != null && preValue.view == "multivalues")
                    {
                        // Multivalue detected, unwrap to save each element by itself
                        var val = preValue.value as JArray;
                        //var items = new List<PreValue>();
                        var items = new Dictionary<string, string>();

                        if (val == null)
                        {
                            continue;
                        }

                        try
                        {
                            var index = 0;

                            // Get all values in the array that are not empty 
                            foreach (var item in val.OfType<JObject>()
                                .Where(jItem => jItem["value"] != null)
                                .Select(jItem => new
                                {
                                    idAsString = jItem["id"] == null ? "0" : jItem["id"].ToString(),
                                    valAsString = jItem["value"].ToString()
                                })
                                .Where(x => x.valAsString.IsNullOrWhiteSpace() == false))
                            {
                                var id = 0;
                                int.TryParse(item.idAsString, out id);
                                // Add item
                                //items.Add(new PreValue(id, item.valAsString));
                                editorValue.Add(ItemsPreValueKey + i + "-" + index, item.valAsString);
                                index++;
                            }
                        }
                        catch (Exception ex)
                        {
                            LogHelper.Error<FootprintContentPreValueEditor>(
                                "Could not deserialize the posted multivalues value: " + val, ex);
                        }

                    }
                }
            }

            return base.ConvertEditorToDb(editorValue, currentValue);
        }
    }
}