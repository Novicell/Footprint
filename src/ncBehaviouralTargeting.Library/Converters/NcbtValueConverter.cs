using System;
using ncBehaviouralTargeting.Library.Models;
using Newtonsoft.Json;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;

namespace ncBehaviouralTargeting.Library.Converters
{
    [PropertyValueType(typeof(NcbtValue))]
    [PropertyValueCache(PropertyCacheValue.All, PropertyCacheLevel.Content)]
    public class NcbtValueConverter : PropertyValueConverterBase
    {
        public override bool IsConverter(PublishedPropertyType propertyType)
        {
            return propertyType.PropertyEditorAlias.Equals("ncBT.ContentEditor") || propertyType.PropertyEditorAlias.Equals("Novicell.Footprint.FootprintContent");
        }

        public override object ConvertDataToSource(PublishedPropertyType propertyType, object source, bool preview)
        {
            try
            {
                if (source != null && !source.ToString().IsNullOrWhiteSpace())
                {
                    return JsonConvert.DeserializeObject<NcbtValue>(source.ToString());
                }
            }
            catch (JsonSerializationException)
            {
                LogHelper.Debug<NcbtValueConverter>(() => string.Format("'{0}' is not valid JSON.", source));
            }
            catch (JsonReaderException)
            {
                LogHelper.Debug<NcbtValueConverter>(() => string.Format("'{0}' is not valid JSON.", source));
            }
            catch (Exception e)
            {
                LogHelper.Error<NcbtValueConverter>("Error converting Segmented value", e);
            }

            return null;
        }
    }
}
