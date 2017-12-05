using System;
using System.Collections.Generic;
using ncBehaviouralTargeting.Library.Models;
using ncBehaviouralTargeting.Library.Models.Enumerations;
using Xunit;

namespace ncBehaviouralTargeting.Library.UnitTests
{
    public class CriterionGroupTests
    {
        [Fact]
        public void Match_throws_ArgumentNullException_when_properties_is_null()
        {
            var sut = new CriterionGroup();

            Func<object> test = () => sut.Match(null);

            Assert.Throws<ArgumentNullException>(test);
        }

        [Fact]
        public void Match_returns_true_when_Criterions_is_null()
        {
            var sut = new CriterionGroup { Criterions = null };

            var actual = sut.Match(new Dictionary<string, object>());

            Assert.True(actual);
        }

        [Fact]
        public void Match_returns_true_when_Criterions_is_empty()
        {
            var sut = new CriterionGroup { Criterions = new List<Criterion>() };

            var actual = sut.Match(new Dictionary<string, object>());

            Assert.True(actual);
        }

        [Theory]
        [InlineData("http://facebook.com/", "Firefox", true)]
        [InlineData("google.com", "Chrome", false)]
        [InlineData("www.microsoft.dk", "Firefox", false)]
        [InlineData("http://www.twitter.com", "Chrome", false)]
        public void Match_returns_true_if_one_of_n_criterions_matches(string referrer, string queryString, bool expected)
        {
            var sut = new CriterionGroup
            {
                Criterions = new List<Criterion>
                {
                    new Criterion
                    {
                        PropertyAlias = "httpReferrer",
                        PropertyValue = "facebook",
                        Operator = new Operator
                        {
                            DataType = DataTypeEnum.String,
                            OperatorType = OperatorTypeEnum.Contains
                        }
                    },
                    new Criterion
                    {
                        PropertyAlias = "queryString",
                        PropertyValue = "utm_source=facebook",
                        Operator = new Operator
                        {
                            DataType = DataTypeEnum.String,
                            OperatorType = OperatorTypeEnum.Contains
                        }
                    }
                }
            };

            var actual = sut.Match(new Dictionary<string, object>
            {
                ["httpReferrer"] = referrer,
                ["queryString"] = queryString
            });

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Match_returns_false_when_properties_does_not_contain_criterion_for_property()
        {
            var sut = new CriterionGroup
            {
                Criterions = new List<Criterion>
                {
                    new Criterion
                    {
                        PropertyAlias = "blahroiewhwoer",
                        PropertyValue = "does not matter",
                        Operator = new Operator
                        {
                            DataType = DataTypeEnum.String,
                            OperatorType = OperatorTypeEnum.Equals,
                            IsInverted = true
                        }
                    }
                }
            };

            var actual = sut.Match(new Dictionary<string, object>
            {
                ["something"] = 24
            });

            Assert.False(actual);
        }
    }
}
