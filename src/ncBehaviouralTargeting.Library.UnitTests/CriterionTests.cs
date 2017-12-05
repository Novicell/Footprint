using System;
using System.Globalization;
using ncBehaviouralTargeting.Library.Models;
using ncBehaviouralTargeting.Library.Models.Enumerations;
using Xunit;

namespace ncBehaviouralTargeting.Library.UnitTests
{
    public class CriterionTests
    {
        [Theory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void Match_Boolean_equals(bool lhs, bool rhs)
        {
            var expected = lhs == rhs;
            var sut = new Criterion
            {
                Operator = new Operator
                {
                    DataType = DataTypeEnum.Boolean,
                    OperatorType = OperatorTypeEnum.Equals
                },
                PropertyValue = rhs.ToString(),
            };

            var actual = sut.Match(lhs);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(OperatorTypeEnum.Contains)]
        [InlineData(OperatorTypeEnum.EndsWith)]
        [InlineData(OperatorTypeEnum.GreaterThan)]
        [InlineData(OperatorTypeEnum.LessThan)]
        [InlineData(OperatorTypeEnum.MatchesRegex)]
        [InlineData(OperatorTypeEnum.StartsWith)]
        public void Match_Boolean_everything_else(OperatorTypeEnum operatorType)
        {
            var sut = new Criterion
            {
                Operator = new Operator
                {
                    DataType = DataTypeEnum.Boolean,
                    OperatorType = operatorType
                },
                PropertyValue = true.ToString()
            };

            var actual = sut.Match(true);

            Assert.False(actual);
        }

        [Theory]
        [InlineData(OperatorTypeEnum.Equals, 2016, 1, 8, 0, true)]
        [InlineData(OperatorTypeEnum.Equals, 2016, 1, 8, +25, false)]
        [InlineData(OperatorTypeEnum.Equals, 2016, 1, 8, -1, false)]
        [InlineData(OperatorTypeEnum.GreaterThan, 2016, 1, 8, 0, false)]
        [InlineData(OperatorTypeEnum.GreaterThan, 2016, 1, 8, +25, false)]
        [InlineData(OperatorTypeEnum.GreaterThan, 2016, 1, 8, -1, true)]
        [InlineData(OperatorTypeEnum.LessThan, 2016, 1, 8, 0, false)]
        [InlineData(OperatorTypeEnum.LessThan, 2016, 1, 8, +25, true)]
        [InlineData(OperatorTypeEnum.LessThan, 2016, 1, 8, -1, false)]
        public void Match_DateTime_operators(OperatorTypeEnum operatorType, int year, int month, int day, int d, bool expected)
        {
            var value = new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);
            var sut = new Criterion
            {
                Operator = new Operator
                {
                    DataType = DataTypeEnum.DateTime,
                    OperatorType = operatorType
                },
                PropertyValue = value.AddDays(d).ToString(CultureInfo.InvariantCulture)
            };

            var actual = sut.Match(value);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(OperatorTypeEnum.Contains)]
        [InlineData(OperatorTypeEnum.EndsWith)]
        [InlineData(OperatorTypeEnum.MatchesRegex)]
        [InlineData(OperatorTypeEnum.StartsWith)]
        public void Match_DateTime_everything_else(OperatorTypeEnum operatorType)
        {
            var value = DateTime.UtcNow;
            var sut = new Criterion
            {
                Operator = new Operator
                {
                    DataType = DataTypeEnum.DateTime,
                    OperatorType = operatorType
                },
                PropertyValue = value.ToString(CultureInfo.InvariantCulture)
            };

            var actual = sut.Match(value);

            Assert.False(actual);
        }

        [Theory]
        [InlineData(OperatorTypeEnum.Equals, 0.0d, 0.0d, true)]
        [InlineData(OperatorTypeEnum.Equals, 0.0d, 1.0d, false)]
        [InlineData(OperatorTypeEnum.Equals, 0.0d, -1.0d, false)]
        [InlineData(OperatorTypeEnum.GreaterThan, 0.0d, 0.0d, false)]
        [InlineData(OperatorTypeEnum.GreaterThan, 0.0d, 1.0d, false)]
        [InlineData(OperatorTypeEnum.GreaterThan, 0.0d, -1.0d, true)]
        [InlineData(OperatorTypeEnum.LessThan, 0.0d, 0.0d, false)]
        [InlineData(OperatorTypeEnum.LessThan, 0.0d, 1.0d, true)]
        [InlineData(OperatorTypeEnum.LessThan, 0.0d, -1.0d, false)]
        public void Match_Double_operators(OperatorTypeEnum operatorType, double left, double right, bool expected)
        {
            var sut = new Criterion
            {
                Operator = new Operator
                {
                    DataType = DataTypeEnum.Double,
                    OperatorType = operatorType
                },
                PropertyValue = right.ToString(CultureInfo.InvariantCulture)
            };

            var actual = sut.Match(left);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(OperatorTypeEnum.Contains)]
        [InlineData(OperatorTypeEnum.EndsWith)]
        [InlineData(OperatorTypeEnum.MatchesRegex)]
        [InlineData(OperatorTypeEnum.StartsWith)]
        public void Match_Double_everything_else(OperatorTypeEnum operatorType)
        {
            var value = 42.0d;
            var sut = new Criterion
            {
                Operator = new Operator
                {
                    DataType = DataTypeEnum.Double,
                    OperatorType = operatorType
                },
                PropertyValue = value.ToString(CultureInfo.InvariantCulture)
            };

            var actual = sut.Match(value);

            Assert.False(actual);
        }

        [Theory]
        [InlineData(OperatorTypeEnum.Equals, 0, 0, true)]
        [InlineData(OperatorTypeEnum.Equals, 0, 1, false)]
        [InlineData(OperatorTypeEnum.Equals, 0, -1, false)]
        [InlineData(OperatorTypeEnum.GreaterThan, 0, 0, false)]
        [InlineData(OperatorTypeEnum.GreaterThan, 0, 1, false)]
        [InlineData(OperatorTypeEnum.GreaterThan, 0, -1, true)]
        [InlineData(OperatorTypeEnum.LessThan, 0, 0, false)]
        [InlineData(OperatorTypeEnum.LessThan, 0, 1, true)]
        [InlineData(OperatorTypeEnum.LessThan, 0, -1, false)]
        public void Match_Int_operators(OperatorTypeEnum operatorType, int left, int right, bool expected)
        {
            var sut = new Criterion
            {
                Operator = new Operator
                {
                    DataType = DataTypeEnum.Int,
                    OperatorType = operatorType
                },
                PropertyValue = right.ToString(CultureInfo.InvariantCulture)
            };

            var actual = sut.Match(left);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(OperatorTypeEnum.Contains)]
        [InlineData(OperatorTypeEnum.EndsWith)]
        [InlineData(OperatorTypeEnum.MatchesRegex)]
        [InlineData(OperatorTypeEnum.StartsWith)]
        public void Match_Int_everything_else(OperatorTypeEnum operatorType)
        {
            var value = 42;
            var sut = new Criterion
            {
                Operator = new Operator
                {
                    DataType = DataTypeEnum.Int,
                    OperatorType = operatorType
                },
                PropertyValue = value.ToString(CultureInfo.InvariantCulture)
            };

            var actual = sut.Match(value);

            Assert.False(actual);
        }

        [Theory]
        [InlineData(OperatorTypeEnum.Equals, 0L, 0L, true)]
        [InlineData(OperatorTypeEnum.Equals, 0L, 1L, false)]
        [InlineData(OperatorTypeEnum.Equals, 0L, -1L, false)]
        [InlineData(OperatorTypeEnum.GreaterThan, 0L, 0L, false)]
        [InlineData(OperatorTypeEnum.GreaterThan, 0L, 1L, false)]
        [InlineData(OperatorTypeEnum.GreaterThan, 0L, -1L, true)]
        [InlineData(OperatorTypeEnum.LessThan, 0L, 0L, false)]
        [InlineData(OperatorTypeEnum.LessThan, 0L, 1L, true)]
        [InlineData(OperatorTypeEnum.LessThan, 0L, -1L, false)]
        public void Match_Long_operators(OperatorTypeEnum operatorType, long left, long right, bool expected)
        {
            var sut = new Criterion
            {
                Operator = new Operator
                {
                    DataType = DataTypeEnum.Long,
                    OperatorType = operatorType
                },
                PropertyValue = right.ToString(CultureInfo.InvariantCulture)
            };

            var actual = sut.Match(left);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(OperatorTypeEnum.Contains)]
        [InlineData(OperatorTypeEnum.EndsWith)]
        [InlineData(OperatorTypeEnum.MatchesRegex)]
        [InlineData(OperatorTypeEnum.StartsWith)]
        public void Match_Long_everything_else(OperatorTypeEnum operatorType)
        {
            var value = 42L;
            var sut = new Criterion
            {
                Operator = new Operator
                {
                    DataType = DataTypeEnum.Long,
                    OperatorType = operatorType
                },
                PropertyValue = value.ToString(CultureInfo.InvariantCulture)
            };

            var actual = sut.Match(value);

            Assert.False(actual);
        }

        [Theory]
        [InlineData(OperatorTypeEnum.Contains, "?utm_source=facebook&something=else", "utm_source=facebook", true)]
        [InlineData(OperatorTypeEnum.Contains, "?utm_source=newsletter&something=else", "utm_source=facebook", false)]
        [InlineData(OperatorTypeEnum.Contains, "?something=else", "utm_source=facebook", false)]
        [InlineData(OperatorTypeEnum.EndsWith, "iPhone", "Phone", true)]
        [InlineData(OperatorTypeEnum.EndsWith, "Android", "Phone", false)]
        [InlineData(OperatorTypeEnum.EndsWith, "Blackberry", "Phone", false)]
        [InlineData(OperatorTypeEnum.EndsWith, "Chrome", "Chrome", true)]
        [InlineData(OperatorTypeEnum.EndsWith, "Safari", "Chrome", false)]
        [InlineData(OperatorTypeEnum.EndsWith, "Firefox", "Chrome", false)]
        [InlineData(OperatorTypeEnum.MatchesRegex, "Mozilla/5.0", @".+?[/\s][\d.]+", true)]
        [InlineData(OperatorTypeEnum.MatchesRegex, "Not really", @".+?[/\s][\d.]+", false)]
        [InlineData(OperatorTypeEnum.MatchesRegex, "r2352", @".+?[/\s][\d.]+", false)]
        [InlineData(OperatorTypeEnum.StartsWith, "192.168.0.23", "192.168.", true)]
        [InlineData(OperatorTypeEnum.StartsWith, "10.0.0.1", "192.168.", false)]
        [InlineData(OperatorTypeEnum.StartsWith, "183.126.99.25", "192.168.", false)]
        public void Match_String_operators(OperatorTypeEnum operatorType, string left, string right, bool expected)
        {
            var sut = new Criterion
            {
                Operator = new Operator
                {
                    DataType = DataTypeEnum.String,
                    OperatorType = operatorType
                },
                PropertyValue = right
            };

            var actual = sut.Match(left);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(OperatorTypeEnum.GreaterThan)]
        [InlineData(OperatorTypeEnum.LessThan)]
        public void Match_String_everything_else(OperatorTypeEnum operatorType)
        {
            var value = "blaroiew";
            var sut = new Criterion
            {
                Operator = new Operator
                {
                    DataType = DataTypeEnum.String,
                    OperatorType = operatorType
                },
                PropertyValue = value.ToString(CultureInfo.InvariantCulture)
            };

            var actual = sut.Match(value);

            Assert.False(actual);
        }


        [Fact]
        public void Match_with_inverted_operator()
        {
            var value = "blaroiew";
            var sut = new Criterion
            {
                Operator = new Operator
                {
                    DataType = DataTypeEnum.String,
                    IsInverted = true,
                    OperatorType = OperatorTypeEnum.Equals
                },
                PropertyValue = value.ToString(CultureInfo.InvariantCulture)
            };

            var actual = sut.Match(value);

            Assert.False(actual);
        }
    }
}
