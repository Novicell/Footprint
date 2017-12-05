using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using MongoDB.Driver;
using ncBehaviouralTargeting.Library.Configuration;
using ncBehaviouralTargeting.Library.DataSources.Mongo;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using Umbraco.Web;
using NcbtSegment = ncBehaviouralTargeting.Library.Models.Segment;

namespace ncBehaviouralTargeting.Library.DataSources
{
    internal static class TrackingManager
    {
        static bool isInitialized;

        static readonly object padlock = new object();

        static readonly LinkedList<ITrackingAggregator> Aggregators = new LinkedList<ITrackingAggregator>();
        static readonly LinkedList<ISegmentReader> Readers = new LinkedList<ISegmentReader>();
        static readonly LinkedList<ITrackingWriter> Writers = new LinkedList<ITrackingWriter>();
        static readonly LinkedList<ISegmentWriter> Segmenters = new LinkedList<ISegmentWriter>();
        static ITrackingQueryable queryable;

        static bool CanAggregate => IsDataSourceAvailable && Aggregators.Count > 0;
        static bool CanRead => IsDataSourceAvailable && Readers.Count > 0;
        static bool CanWrite => IsDataSourceAvailable && Writers.Count > 0;
        static bool CanSegment => IsDataSourceAvailable && Segmenters.Count > 0;
        static bool CanQuery => IsDataSourceAvailable && queryable != null;

        static SqlConnection ConnectionFactory()
        {
            var connectionStringName = ConfigurationHelper.Settings.Sql.ConnectionStringName;
            var connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            var connection = new SqlConnection(connectionString);
            connection.Open();
            return connection;
        }

        static bool IsDataSourceAvailable
        {
            get
            {
                if (ConfigurationHelper.Settings.IsDisabled)
                {
                    return false;
                }

                lock (padlock)
                {
                    if (isInitialized)
                    {
                        return true;
                    }

                    Readers.AddLast(new RequestTrackingDataSource(() => UmbracoContext.Current));

                    if (ConfigurationHelper.Settings.Cookies.ElementInformation.IsPresent)
                    {
                        var cookie = new CookieTrackingDataSource(() => new HttpContextWrapper(HttpContext.Current));
                        Readers.AddLast(cookie);
                        Segmenters.AddLast(cookie);
                    }

                    //if (ConfigurationHelper.Settings.Segment.ElementInformation.IsPresent)
                    //{
                    //    var segment = new SegmentTrackingDataSource(ConfigurationHelper.Settings.Segment.WriteKey);
                    //    Segmenters.AddLast(segment);
                    //    Writers.AddLast(segment);
                    //}

                    if (ConfigurationHelper.Settings.Sql.ElementInformation.IsPresent)
                    {
                        var sql = new SqlTrackingDataSource(ConnectionFactory);
                        Aggregators.AddLast(sql);
                        Readers.AddLast(sql);
                        Segmenters.AddLast(sql);
                    }

                    if (ConfigurationHelper.Settings.MongoDb.ElementInformation.IsPresent)
                    {
                        var mongoClient = new MongoClient(ConfigurationHelper.Settings.MongoDb.ConnectionString);
                        var mongoDatabase = mongoClient.GetDatabase(ConfigurationHelper.Settings.MongoDb.Database);
                        var mongoCollection = mongoDatabase.GetCollection<dynamic>(ConfigurationHelper.Settings.MongoDb.Collection);
                        var mongo = new MongoTrackingDataSource(mongoCollection);
                        var mongoWrapper = new MongoVisitorIdTrackingDataStore(mongo, ConnectionFactory);
                        Aggregators.AddLast(mongoWrapper);
                        Readers.AddLast(mongoWrapper);
                        Segmenters.AddLast(mongoWrapper);
                        Writers.AddLast(mongoWrapper);

                        queryable = new MongoTrackingQueryable(mongoCollection);
                    }

                    if (ConfigurationHelper.Settings.CustomerIo.ElementInformation.IsPresent)
                    {
                        var siteId = ConfigurationHelper.Settings.CustomerIo.SiteId;
                        var apiKey = ConfigurationHelper.Settings.CustomerIo.ApiKey;
                        var customerIo = new CustomerIoTrackingDataSource(siteId, apiKey);
                        Segmenters.AddLast(customerIo);
                        Writers.AddLast(customerIo);
                    }

                    isInitialized = true;
                    return isInitialized;
                }
            }
        }

        public static void AddVisitorToSegment(string visitorId, NcbtSegment segment)
        {
            if (!CanSegment)
            {
                return;
            }

            foreach (var segmenter in Segmenters)
            {
                segmenter.AddToSegment(visitorId, segment);
            }
        }

        public static long CountVisitors()
        {
            if (!CanAggregate)
            {
                return 0;
            }

            return Aggregators.Select(x => x.CountVisitors()).Max();
        }

        public static string SetVisitorProperty(string visitorId, IReadOnlyDictionary<string, object> values)
        {
            if (CanWrite)
            {
                foreach (var writer in Writers)
                {
                    writer.SetVisitorProperties(visitorId, values);
                }
            }

            foreach (var segment in NcbtSegment.GetAllLight())
            {
                if (IsVisitorInSegment(visitorId, segment.Alias))
                {
                    AddVisitorToSegment(visitorId, segment);
                }
            }

            return visitorId;
        }

        public static string SetVisitorProperty(string visitorId, string key, object value)
        {
            return SetVisitorProperty(visitorId, new Dictionary<string, object> { [key] = value });
        }

        public static string PushToVisitorProperty(string visitorId, string key, object value)
        {
            // TODO: Remove?
            if (CanWrite)
            {
                foreach (var writer in Writers)
                {
                    writer.PushToVisitorProperty(visitorId, key, value);
                }
            }

            foreach (var segment in NcbtSegment.GetAllLight())
            {
                if (IsVisitorInSegment(visitorId, segment.Alias))
                {
                    AddVisitorToSegment(visitorId, segment);
                }
            }

            return visitorId;
        }

        public static bool IsVisitorInSegment(string visitorId, string segmentAlias)
        {
            var segment = NcbtSegment.GetByAlias(segmentAlias);

            if (segment == null)
            {
                return false;
            }

            if (CanRead)
            {
                foreach (var reader in Readers)
                {
                    var isInSegment = reader.IsVisitorInSegment(visitorId, segment);

                    if (isInSegment)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Gets a list of visitors in a given segment
        /// </summary>
        /// <param name="segmentAlias">Alias of the segment</param>
        /// <returns>List of visitors in segment</returns>
        public static List<dynamic> GetVisitorsInSegment(string segmentAlias)
        {
            // TODO: Remove?
            if (!CanAggregate)
            {
                return new List<dynamic>();
            }

            var segment = NcbtSegment.GetByAlias(segmentAlias);

            if (segment == null)
            {
                return new List<dynamic>();
            }

            return Aggregators.Select(x => x.GetVisitorsInSegment(segment))
                              .OrderByDescending(x => x.Count)
                              .First()
                              .ToList();
        }

        /// <summary>
        /// Gets a list of visitors in a given segment with the passed property settings<br/>
        /// Dictionary contains aliases and a bool describing whether or not they should be set or not
        /// </summary>
        /// <param name="segmentAlias">Alias of the segment</param>
        /// <param name="properties">Dictionary of properties to include or not</param>
        /// <returns>List of visitors in segment</returns>
        public static List<dynamic> GetVisitorsInSegmentWithProperties(string segmentAlias, Dictionary<string, bool> properties)
        {
            if (!CanAggregate)
            {
                return new List<dynamic>();
            }

            var segment = NcbtSegment.GetByAlias(segmentAlias);

            if (segment == null)
            {
                return new List<dynamic>();
            }

            return Aggregators.Select(x => x.GetVisitorsInSegmentWithProperties(segment, properties))
                              .OrderByDescending(x => x.Count)
                              .First()
                              .ToList();
        }

        /// <summary>
        /// Counts the number of visitors in a given segment
        /// </summary>
        /// <param name="segmentAlias">Alias of the segment</param>
        /// <returns>Number of visitors</returns>
        public static long CountVisitorsInSegment(string segmentAlias)
        {
            if (!CanAggregate)
            {
                return 0;
            }

            var segment = NcbtSegment.GetByAlias(segmentAlias);

            if (segment == null)
            {
                return 0;
            }

            return Aggregators.Select(x => x.CountVisitorsInSegment(segment)).Max();
        }

        /// <summary>
        /// Counts the number of visitors in a given segment<br />
        /// Internal use only
        /// </summary>
        /// <returns>Number of visitors</returns>
        internal static long CountVisitorsInSegmentInternal(dynamic segmentDynamic)
        {
            if (!CanAggregate)
            {
                return 0;
            }

            var segment = JsonConvert.DeserializeObject<NcbtSegment>(((JObject) segmentDynamic).ToString());
            return Aggregators.Select(x => x.CountVisitorsInSegment(segment)).Max();
        }

        public static QueryResult QueryVisitors(bool excludeIncompleteProfiles, bool excludeNcbtProperties, IReadOnlyCollection<string> segmentAliases, int? page)
        {
            if (!CanQuery || page < 0)
            {
                return new QueryResult(0, new string[0], new dynamic[0]);
            }

            var segments = segmentAliases?.Select(NcbtSegment.GetByAlias).ToList() ?? new List<NcbtSegment>();
            return queryable.Query(excludeIncompleteProfiles, excludeNcbtProperties, segments.ToList(), page);
        }
    }
}
