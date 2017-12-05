using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using ncBehaviouralTargeting.Library.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.PropertyEditors;

namespace ncBehaviouralTargeting.Library.PropertyEditors
{
    internal class FootprintContentValidator : IPropertyValidator
    {
        public IEnumerable<ValidationResult> Validate(object rawValue, PreValueCollection preValues, PropertyEditor editor)
        {
            yield break;
            // Not implemented
        }
    }
}