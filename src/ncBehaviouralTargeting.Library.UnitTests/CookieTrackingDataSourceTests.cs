using System;
using System.Linq;
using System.Web;
using Moq;
using ncBehaviouralTargeting.Library.DataSources;
using ncBehaviouralTargeting.Library.Models;
using Xunit;
using Action = System.Action;

namespace ncBehaviouralTargeting.Library.UnitTests
{
    public class CookieTrackingDataSourceTests
    {
        [Fact]
        public void Constructor_throws_ArgumentNullException_when_httpContextAccessor_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => new CookieTrackingDataSource(null));
        }

        [Fact]
        public void IsVisitorInSegment_throws_ArgumentNullException_when_visitorId_is_null()
        {
            var sut = new CookieTrackingDataSource(() => null);

            Action test = () => sut.IsVisitorInSegment(null, new Segment());

            Assert.Throws<ArgumentNullException>(test);
        }

        [Fact]
        public void IsVisitorInSegment_throws_ArgumentNullException_when_segment_is_null()
        {
            var sut = new CookieTrackingDataSource(() => null);

            Action test = () => sut.IsVisitorInSegment("visitorId", null);

            Assert.Throws<ArgumentNullException>(test);
        }

        [Theory]
        [InlineData("facebook", true)]
        [InlineData("newsletter", false)]
        [InlineData("customer", true)]
        [InlineData("something", false)]
        [InlineData("new", true)]
        [InlineData("false", false)]
        public void IsVisitorInSegment_is_true_when_segment_has_cookie(string segmentAlias, bool expected)
        {
            var httpRequestBaseMock = new Mock<HttpRequestBase>();
            httpRequestBaseMock.Setup(x => x.Cookies)
                               .Returns(new HttpCookieCollection
                               {
                                   new HttpCookie(string.Format(CookieTrackingDataSource.CookieNameFormat, "facebook")),
                                   new HttpCookie(string.Format(CookieTrackingDataSource.CookieNameFormat, "customer")),
                                   new HttpCookie(string.Format(CookieTrackingDataSource.CookieNameFormat, "new"))
                               });
            var httpContextBaseMock = new Mock<HttpContextBase>();
            httpContextBaseMock.Setup(x => x.Request).Returns(httpRequestBaseMock.Object);
            var sut = new CookieTrackingDataSource(() => httpContextBaseMock.Object);

            var actual = sut.IsVisitorInSegment("visitorId", new Segment { Alias = segmentAlias });

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AddToSegment_throws_ArgumentNullException_when_visitorId_is_null()
        {
            var sut = new CookieTrackingDataSource(() => new FakeHttpContext());

            Action test = () => sut.AddToSegment((string) null, new Segment());

            Assert.Throws<ArgumentNullException>(test);
        }

        [Fact]
        public void AddToSegment_throws_ArgumentNullException_when_segment_is_null()
        {
            var sut = new CookieTrackingDataSource(() => new FakeHttpContext());

            Action test = () => sut.AddToSegment("visitorId", null);

            Assert.Throws<ArgumentNullException>(test);
        }

        [Theory]
        [InlineData("facebook")]
        [InlineData("newsletter")]
        [InlineData("newCustomer")]
        public void AddToSegment_adds_segment_cookie(string segmentAlias)
        {
            var httpContextBase = new FakeHttpContext();
            var sut = new CookieTrackingDataSource(() => httpContextBase);

            sut.AddToSegment("visitorId", new Segment { Alias = segmentAlias, Persistence = -1 });

            var expected = string.Format(CookieTrackingDataSource.CookieNameFormat, segmentAlias);
            Assert.Contains(expected, httpContextBase.Response.Cookies.AllKeys);
        }

        [Fact]
        public void AddToSegment_with_permanent_persistence_adds_cookie_with_5_year_expiration()
        {
            var segmentAlias = "facebook";
            var httpContextBase = new FakeHttpContext();
            var sut = new CookieTrackingDataSource(() => httpContextBase);

            sut.AddToSegment("visitorId", new Segment { Alias = segmentAlias, Persistence = -1 });

            var cookie = httpContextBase.Response.Cookies.Get(string.Format(CookieTrackingDataSource.CookieNameFormat, segmentAlias));
            Assert.True(DateTime.UtcNow.AddYears(5) - cookie.Expires < TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void AddToSegment_with_no_persistence_adds_cookie_with_immediate_expiration()
        {
            var segmentAlias = "facebook";
            var httpContextBase = new FakeHttpContext();
            var sut = new CookieTrackingDataSource(() => httpContextBase);

            sut.AddToSegment("visitorId", new Segment { Alias = segmentAlias, Persistence = 0 });

            var cookie = httpContextBase.Response.Cookies.Get(string.Format(CookieTrackingDataSource.CookieNameFormat, segmentAlias));
            Assert.Equal(DateTime.MinValue, cookie.Expires);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1000)]
        public void AddToSegment_with_X_days_persistence_adds_cookie_with_X_days_expiration(int days)
        {
            var segmentAlias = "facebook";
            var httpContextBase = new FakeHttpContext();
            var sut = new CookieTrackingDataSource(() => httpContextBase);

            sut.AddToSegment("visitorId", new Segment { Alias = segmentAlias, Persistence = days });

            var cookie = httpContextBase.Response.Cookies.Get(string.Format(CookieTrackingDataSource.CookieNameFormat, segmentAlias));
            Assert.True(DateTime.UtcNow.AddDays(days) - cookie.Expires < TimeSpan.FromSeconds(1));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1000)]
        public void AddToSegment_with_X_days_persistence_does_not_update_existing_cookies(int days)
        {
            var segmentAlias = "facebook";
            var cookieName = string.Format(CookieTrackingDataSource.CookieNameFormat, segmentAlias);
            var httpContextBase = new FakeHttpContext();
            var existingCookie = new HttpCookie(cookieName) { Expires = DateTime.UtcNow.AddDays(days) };
            httpContextBase.Request.Cookies.Add(existingCookie);
            var sut = new CookieTrackingDataSource(() => httpContextBase);

            sut.AddToSegment("visitorId", new Segment { Alias = segmentAlias, Persistence = days });

            Assert.Null(httpContextBase.Response.Cookies.Get(cookieName));
        }

        public class FakeHttpContext : HttpContextBase
        {
            public override HttpRequestBase Request { get; } = new FakeHttpRequest();
            public override HttpResponseBase Response { get; } = new FakeHttpResponse();
        }

        public class FakeHttpRequest : HttpRequestBase
        {
            public override HttpCookieCollection Cookies { get; } = new HttpCookieCollection();
        }

        public class FakeHttpResponse : HttpResponseBase
        {
            public override HttpCookieCollection Cookies { get; } = new HttpCookieCollection();
        }
    }
}