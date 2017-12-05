using System;
using System.Collections.Generic;
using ncBehaviouralTargeting.Library.Models;
using ncBehaviouralTargeting.Library.Models.Enumerations;
using Xunit;

namespace ncBehaviouralTargeting.Library.UnitTests
{
    public class SegmentTests
    {
        [Fact]
        public void Match_throws_ArgumentNullException()
        {
            var sut = new Segment();

            Func<object> testCode = () => sut.Match(null);

            Assert.Throws<ArgumentNullException>(testCode);
        }

        [Theory]
        [InlineData("Chrome", false)]
        [InlineData("Safari", false)]
        [InlineData("Firefox", true)]
        public void Match_with_one_criteria_group_must_match_it(string browser, bool expected)
        {
            var sut = new Segment
            {
                CriterionGroups = new List<CriterionGroup>
                {
                    new CriterionGroup
                    {
                        Criterions = new List<Criterion>
                        {
                            new Criterion
                            {
                                PropertyAlias = "ncbt.browser",
                                PropertyValue = "Firefox",
                                Operator = new Operator
                                {
                                    DataType = DataTypeEnum.String,
                                    OperatorType = OperatorTypeEnum.Equals
                                }
                            }
                        }
                    }
                }
            };

            var actual = sut.Match(new Dictionary<string, object>
            {
                ["ncbt.browser"] = browser
            });

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("?utm_source=facebook", "Firefox", true)]
        [InlineData("?utm_source=facebook", "Chrome", false)]
        [InlineData("?utm_source=newsletter", "Firefox", false)]
        [InlineData("?utm_source=newsletter", "Chrome", false)]
        public void Match_with_two_criteria_groups_should_match_both(string queryString, string browser, bool expected)
        {
            var sut = new Segment
            {
                CriterionGroups = new List<CriterionGroup>
                {
                    new CriterionGroup
                    {
                        Criterions = new List<Criterion>
                        {
                            new Criterion
                            {
                                PropertyAlias = "ncbt.queryString",
                                PropertyValue = "utm_source=facebook",
                                Operator = new Operator { DataType = DataTypeEnum.String, OperatorType = OperatorTypeEnum.Contains }
                            }
                        }
                    },
                    new CriterionGroup
                    {
                        Criterions = new List<Criterion>
                        {
                            new Criterion
                            {
                                PropertyAlias = "ncbt.browser",
                                PropertyValue = "Firefox",
                                Operator = new Operator
                                {
                                    DataType = DataTypeEnum.String,
                                    OperatorType = OperatorTypeEnum.Equals
                                }
                            }
                        }
                    }
                }
            };

            var actual = sut.Match(new Dictionary<string, object>
            {
                ["ncbt.queryString"] = queryString,
                ["ncbt.browser"] = browser
            });

            Assert.Equal(expected, actual);
        }
    }
}
