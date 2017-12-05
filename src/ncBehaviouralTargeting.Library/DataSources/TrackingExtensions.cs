using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;

namespace ncBehaviouralTargeting.Library.DataSources
{
    internal static class TrackingExtensions
    {
        /// <summary>
        /// Appends a timestamp to all values in the 'values'-Dictionary
        /// </summary>
        /// <param name="values">Dictionary to be converted</param>
        /// <returns>A Dictionary of Dictionaries, where the inner dictionary contains two keys "Timestamp" and "Value"</returns>
        public static IReadOnlyDictionary<string, object> ToValuesWithTimestamps(this IReadOnlyDictionary<string, object> values)
        {
            return values.Select(AddTimestamp).ToDictionary(x => x.Key, x => x.Value);
        }

        static KeyValuePair<string, object> AddTimestamp(KeyValuePair<string, object> kvp)
        {
            return new KeyValuePair<string, object>(kvp.Key, new Dictionary<string, object>
            {
                ["Value"] = kvp.Value,
                ["Timestamp"] = DateTime.UtcNow
            });
        }
    }
}
