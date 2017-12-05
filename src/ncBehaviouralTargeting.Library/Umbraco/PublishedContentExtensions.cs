using System.Collections.Generic;
using System.Linq;
using ncBehaviouralTargeting.Library.Constants;
using ncBehaviouralTargeting.Library.Models;
using ncBehaviouralTargeting.Library.Tracking;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace ncBehaviouralTargeting.Library.Umbraco
{
    public static class PublishedContentExtensions
    {
        /// <summary>
        /// Checks if a property contains segmented data for a specific segment.
        /// If the <paramref name="segmentAlias"/> is null or empty, the method
        /// will check if the property contains segmented data for any segment.
        /// </summary>
        public static bool HasSegmentedValue(this IPublishedContent content, string propertyAlias, string segmentAlias = "", bool recursive = false)
        {
            // Default segment if none provided.
            if (segmentAlias.IsNullOrWhiteSpace())
            {
                segmentAlias = BehaviouralTargetingConstants.SegmentDefaultAlias;
            }

            // Make sure the property has a value.
            if (!content.HasValue(propertyAlias, recursive))
            {
                return false;
            }

            // Fetch property.
            var prop = content.GetProperty(propertyAlias, recursive);

            // Quick and dirty test if it's JSON.
            if (!IsJson(prop.DataValue.ToString()))
            {
                // Not JSON, therefore not a valid segmented property.
                return false;
            }

            // Extract prop value from property.
            dynamic propValue = JsonConvert.DeserializeObject(prop.DataValue.ToString());

            if (propValue != null && propValue.isNcbt == true)
            {
                // Convert to our model.
                var ncbtModel = prop.Value as NcbtValue;

                if (ncbtModel != null)
                {
                    return ncbtModel.Segments.Any(x => x.Alias == segmentAlias);
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the segmented value for the given property.
        /// It will get the value of the first segment that matches the visitor.
        /// </summary>
        public static object GetSegmentedValue(this IPublishedContent content, string propertyAlias, bool recursive = false, object defaultValue = null)
        {
            return content.GetSegmentedValue<object>(propertyAlias, recursive, defaultValue);
        }

        /// <summary>
        /// Gets the segmented value for the given property.
        /// It will get the value of the first segment that matches the visitor.
        /// </summary>
        public static T GetSegmentedValue<T>(this IPublishedContent content, string propertyAlias, bool recursive = false, T defaultValue = default(T))
        {
            return content.InternalGetSegmentedValue<T>(propertyAlias, recursive, defaultValue);
        }

        static T InternalGetSegmentedValue<T>(this IPublishedContent content, string propertyAlias, bool recursive = false, T defaultValue = default(T))
        {
            // Make sure the property have a segmented value
            if (content.HasSegmentedValue(propertyAlias, recursive: recursive))
            {
                // Fetch property
                var prop = content.GetProperty(propertyAlias, recursive);

                // Convert to our model
                var ncbtModel = prop.Value as NcbtValue;
                if (ncbtModel != null)
                {
                    // Find the active segment for the user
                    NcbtValueSegment segmentModel = null;
                    foreach (var segment in ncbtModel.Segments.OrderBy(s => s.SortOrder).Where(x => x.Alias != BehaviouralTargetingConstants.SegmentDefaultAlias))
                    {
                        // Check if user is in segment
                        if (CurrentVisitor.IsInSegment(segment.Alias))
                        {
                            // Match!
                            segmentModel = segment;
                            break;
                        }
                    }
                    // Fall back to default segment if none found
                    segmentModel = segmentModel ??
                                   ncbtModel.Segments.First(
                                       x => x.Alias == BehaviouralTargetingConstants.SegmentDefaultAlias);

                    // Check if multivalue is possible
                    if (segmentModel.Value is JArray)
                    {
                        // We have a possible multivalue, get the prevalues
                        var preValueIter = umbraco.library.GetPreValues(ncbtModel.DataTypeId);
                        // Get value ids from segment model
                        var modelIds = ((JArray) segmentModel.Value).Values<int>().ToList();
                        var modelValues = new List<object>();

                        // Go through prevalues
                        while (preValueIter.MoveNext())
                        {
                            // TODO: Check if multivalue view present in selected editor prevalue

                            // Get prevalues from xpath nav
                            var preValuesXpathNav = preValueIter.Current;
                            // Check for each prevalue id in the model
                            modelIds.ForEach(preValueId =>
                            {
                                // Try using xpath
                                var preValueValueXpathNav = preValuesXpathNav.Select("//preValue[@id='" + preValueId + "']");
                                if (preValueValueXpathNav.Count != 0)
                                {
                                    // We found our prevalue!
                                    preValueValueXpathNav.MoveNext();
                                    modelValues.Add(preValueValueXpathNav.Current.Value);
                                }
                            });
                        }

                        // Assign values back to model in CSV format, as Umbraco does
                        segmentModel.Value = string.Join(",", modelValues);
                    }

                    // Return value
                    return (T)segmentModel.Value;
                }

                // Not a segmented value, return if the type matches anyways
                if (prop.Value is T)
                {
                    return (T)prop.Value;
                }

                return default(T);
            }

            // Not a segmented value, call Umbracos GetPropertyValue
            return content.GetPropertyValue(propertyAlias, recursive, defaultValue);
        }

        static bool IsJson(string input)
        {
            input = input.Trim();
            return input.StartsWith("{") && input.EndsWith("}")
                   || input.StartsWith("[") && input.EndsWith("]");
        }
    }
}
