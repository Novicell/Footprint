using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using Umbraco.Core.Logging;
using NcbtSegment = ncBehaviouralTargeting.Library.Models.Segment;

namespace ncBehaviouralTargeting.Library.DataSources
{
    internal class CustomerIoTrackingDataSource : ISegmentWriter, ITrackingWriter
    {
        // We had trouble with slow loading times on http://www.novicell.dk/
        // when this class was enabled (before 2016/02/08), because it would
        // block the request while sending properties to the http://customer.io API.
        // The solution is to use a separate thread for this IO work which can be queued
        // in the BlockingCollection<Request> and the IO thread will block until work
        // is ready. We don't use ThreadPool (!) because ASP.NET uses it as well,
        // and we are IO-bound, not CPU-bound -- short, intensive CPU-work is perfect
        // for the ThreadPool (but maybe not in ASP.NET apps).
        readonly BlockingCollection<Request> queue;
        readonly Thread thread;
        readonly HttpClient http;

        public CustomerIoTrackingDataSource(string siteId, string apiKey)
        {
            if (siteId == null)
            {
                throw new ArgumentNullException(nameof(siteId));
            }

            if (apiKey == null)
            {
                throw new ArgumentNullException(nameof(apiKey));
            }

            http = new HttpClient();
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Basic", Convert.ToBase64String(
                    Encoding.GetEncoding("iso-8859-1").GetBytes(
                        $"{siteId}:{apiKey}")));

            queue = new BlockingCollection<Request>();
            thread = new Thread(Start) { IsBackground = true };
            thread.Start();
        }

        void Start()
        {
            const string format = "https://track.customer.io/api/v1/customers/{0}";

            while (true)
            {
                try
                {
                    var request = queue.Take();
                    var url = string.Format(format, request.VisitorId);
                    var content = new FormUrlEncodedContent(request.Properties);
                    http.PutAsync(url, content).Wait();
                }
                catch (Exception ex)
                {
                    LogHelper.Error<CustomerIoTrackingDataSource>("Could not send properties.", ex);
                }
            }
        }

        public void AddToSegment(string visitorId, NcbtSegment segment)
        {
            try
            {
                BlockingEnqueue(visitorId, new Dictionary<string, string>
                {
                    [segment.Alias] = DateTimeOffset.UtcNow.ToString("O")
                });
            }
            catch (Exception ex)
            {
                LogHelper.Error<CustomerIoTrackingDataSource>($"Could not put {visitorId} into {segment.Alias}.", ex);
            }
        }

        public void PushToVisitorProperty(string visitorId, string key, object value)
        {
            SetVisitorProperties(visitorId,
                new Dictionary<string, object> { [key] = value });
        }

        public void SetVisitorProperties(string visitorId, IReadOnlyDictionary<string, object> properties)
        {
            try
            {
                BlockingEnqueue(visitorId, properties.ToDictionary(x => x.Key, x => x.Value?.ToString()));
            }
            catch (Exception ex)
            {
                LogHelper.Error<CustomerIoTrackingDataSource>($"Could not write {visitorId} properties.", ex);
            }
        }

        public void SetVisitorProperty(string visitorId, string key, object value)
        {
            SetVisitorProperties(visitorId,
                new Dictionary<string, object> { [key] = value });
        }

        void BlockingEnqueue(string visitorId, IReadOnlyDictionary<string, string> properties)
        {
            queue.Add(new Request(visitorId, properties));
        }

        sealed class Request
        {
            public Request(string visitorId, IReadOnlyDictionary<string, string> properties)
            {
                VisitorId = visitorId;
                Properties = properties;
            }

            public string VisitorId { get; }
            public IReadOnlyDictionary<string, string> Properties { get; }
        }
    }
}
