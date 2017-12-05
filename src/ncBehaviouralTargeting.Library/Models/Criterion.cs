using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using ncBehaviouralTargeting.Library.Models.BaseModels;
using ncBehaviouralTargeting.Library.Models.Enumerations;
using NPoco;

namespace ncBehaviouralTargeting.Library.Models
{
    [TableName("ncBtCriterion")]
    internal class Criterion : BaseEntity<Criterion>
    {
        public int CriterionGroupId { get; set; }
        public int OperatorId { get; set; }
        public string PropertyAlias { get; set; }
        public string PropertyValue { get; set; }
        public int SortOrder { get; set; }

        [Ignore]
        public Operator Operator { get; set; }



        public void SetLinks(List<Operator> operators)
        {
            Operator = operators.FirstOrDefault(o => o.Id == OperatorId);
        }


        public static Criterion GetById(int id)
        {
            using (var connection = CreateConnection)
            {
                var tuple = connection.FetchMultiple<Criterion, Operator>(";exec CriterionGetByIdStp @Id", new { Id = id });
                var item = tuple.Item1.FirstOrDefault();
                if (item == null)
                {
                    return null;
                }
                item.SetLinks(tuple.Item2);
                return item;
            }
        }

        public bool Match(object propertyValue)
        {
            var otherPropertyValue = PropertyValue ?? string.Empty;
            var criterionMatch = false;

            switch (Operator.DataType)
            {
                case DataTypeEnum.Boolean:
                {
                    var value = (bool) propertyValue;
                    var other = bool.Parse(otherPropertyValue);

                    switch (Operator.OperatorType)
                    {
                        case OperatorTypeEnum.Equals:
                            criterionMatch = value == other;
                            break;
                    }

                    break;
                }

                case DataTypeEnum.DateTime:
                {
                    var value = (DateTime) propertyValue;
                    var other = DateTime.Parse(otherPropertyValue, CultureInfo.InvariantCulture);

                    switch (Operator.OperatorType)
                    {
                        case OperatorTypeEnum.Equals:
                            criterionMatch = value.Equals(other);
                            break;

                        case OperatorTypeEnum.GreaterThan:
                            criterionMatch = value > other;
                            break;

                        case OperatorTypeEnum.LessThan:
                            criterionMatch = value < other;
                            break;
                    }

                    break;
                }

                case DataTypeEnum.Double:
                {
                    var value = (double) propertyValue;
                    var other = double.Parse(otherPropertyValue);

                    switch (Operator.OperatorType)
                    {
                        case OperatorTypeEnum.Equals:
                            criterionMatch = value.Equals(other);
                            break;

                        case OperatorTypeEnum.GreaterThan:
                            criterionMatch = value > other;
                            break;

                        case OperatorTypeEnum.LessThan:
                            criterionMatch = value < other;
                            break;
                    }

                    break;
                }

                case DataTypeEnum.Int:
                {
                    var value = (int) propertyValue;
                    var other = int.Parse(otherPropertyValue);

                    switch (Operator.OperatorType)
                    {
                        case OperatorTypeEnum.Equals:
                            criterionMatch = value.Equals(other);
                            break;

                        case OperatorTypeEnum.GreaterThan:
                            criterionMatch = value > other;
                            break;

                        case OperatorTypeEnum.LessThan:
                            criterionMatch = value < other;
                            break;
                    }

                    break;
                }

                case DataTypeEnum.Long:
                {
                    var value = (long) propertyValue;
                    var other = long.Parse(otherPropertyValue);

                    switch (Operator.OperatorType)
                    {
                        case OperatorTypeEnum.Equals:
                            criterionMatch = value.Equals(other);
                            break;

                        case OperatorTypeEnum.GreaterThan:
                            criterionMatch = value > other;
                            break;

                        case OperatorTypeEnum.LessThan:
                            criterionMatch = value < other;
                            break;
                    }

                    break;
                }

                case DataTypeEnum.String:
                {
                    var value = ((string) propertyValue).ToLowerInvariant();
                    var other = otherPropertyValue.ToLowerInvariant();

                    switch (Operator.OperatorType)
                    {
                        case OperatorTypeEnum.Contains:
                            criterionMatch = value.Contains(other);
                            break;

                        case OperatorTypeEnum.EndsWith:
                            criterionMatch = value.EndsWith(other);
                            break;

                        case OperatorTypeEnum.Equals:
                            criterionMatch = value.Equals(other);
                            break;

                        case OperatorTypeEnum.MatchesRegex:
                            criterionMatch = Regex.IsMatch(value, other);
                            break;

                        case OperatorTypeEnum.StartsWith:
                            criterionMatch = value.StartsWith(other);
                            break;
                    }

                    break;
                }
            }

            if (Operator.IsInverted)
            {
                criterionMatch = !criterionMatch;
            }

            return criterionMatch;
        }
    }
}
