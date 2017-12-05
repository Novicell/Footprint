using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using ncBehaviouralTargeting.Library.Models;
using NcbtSegment = ncBehaviouralTargeting.Library.Models.Segment;

namespace ncBehaviouralTargeting.Library.DataSources
{
    internal class SqlTrackingDataSource : ISegmentReader, ISegmentWriter, ITrackingAggregator
    {
        readonly Func<SqlConnection> connectionFactory;

        public SqlTrackingDataSource(Func<SqlConnection> connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }

        public bool IsVisitorInSegment(string visitorId, NcbtSegment segment)
        {
            var row = GetVisitorSegments(visitorId, segment);

            if (row == null)
            {
                return false;
            }

            if (segment.Persistence > 0)
            {
                return DateTime.UtcNow.AddDays(segment.Persistence) > row.CreatedUtc;
            }

            return segment.Persistence < 0;
        }

        public void AddToSegment(string visitorId, NcbtSegment segment)
        {
            var row = GetVisitorSegments(visitorId, segment);

            if (row == null)
            {
                using (var connection = connectionFactory())
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO dbo.ncBtVisitorSegment (VisitorId, SegmentAlias, CreatedUtc) VALUES (@visitorId, @segmentAlias, @createdUtc);";
                    command.Parameters.AddWithValue("@visitorId", visitorId);
                    command.Parameters.AddWithValue("@segmentAlias", segment.Alias);
                    command.Parameters.AddWithValue("@createdUtc", DateTime.UtcNow);
                    command.ExecuteNonQuery();
                }
            }
        }

        VisitorSegment GetVisitorSegments(string visitorId, Segment segment)
        {
            using (var connection = connectionFactory())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = $@"
                    SELECT VisitorId, SegmentAlias, CreatedUtc FROM {VisitorSegment.TableName}
                    WHERE {nameof(VisitorSegment.VisitorId)} = @visitorId
                    AND {nameof(VisitorSegment.SegmentAlias)} = @segmentAlias;";

                command.Parameters.AddWithValue("@visitorId", visitorId);
                command.Parameters.AddWithValue("@segmentAlias", segment.Alias);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return new VisitorSegment(
                            reader.GetString(0),
                            reader.GetString(1),
                            reader.GetDateTime(2));
                    }
                }
            }

            return null;
        }

        public long CountVisitors()
        {
            using (var connection = connectionFactory())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"SELECT COUNT(DISTINCT {nameof(VisitorSegment.VisitorId)}) FROM {VisitorSegment.TableName};";
                return (int) command.ExecuteScalar();
            }
        }

        public long CountVisitorsInSegment(NcbtSegment segment)
        {
            var persistence = segment.Persistence;

            // If nothing is "persisted" (though it is) we don't include them
            // in the stats.
            if (persistence == 0)
            {
                return 0;
            }

            using (var connection = connectionFactory())
            using (var command = connection.CreateCommand())
            {
                command.Parameters.AddWithValue("@segmentAlias", segment.Alias);

                if (persistence < 0)
                {
                    command.CommandText = $"SELECT COUNT(*) FROM {VisitorSegment.TableName} WHERE {nameof(VisitorSegment.SegmentAlias)} = @segmentAlias;";
                    return (int) command.ExecuteScalar();
                }

                command.CommandText = $"SELECT {nameof(VisitorSegment.CreatedUtc)} FROM {VisitorSegment.TableName} WHERE {nameof(VisitorSegment.SegmentAlias)} = @segmentAlias;";

                using (var reader = command.ExecuteReader())
                {
                    var rows = 0;
                    var now = DateTime.Now;

                    while (reader.Read())
                    {
                        var persistedAt = reader.GetDateTime(0);

                        if ((now - persistedAt).TotalDays <= persistence)
                        {
                            rows++;
                        }
                    }

                    return rows;
                }
            }
        }

        public IReadOnlyCollection<dynamic> GetVisitorsInSegment(Segment segment)
        {
            var result = new List<VisitorSegment>();
            var persistence = segment.Persistence;

            // If nothing is "persisted" (though it is) we don't include them
            // in the stats.
            if (persistence == 0)
            {
                return new List<dynamic>().AsReadOnly();
            }

            using (var connection = connectionFactory())
            using (var command = connection.CreateCommand())
            {
                command.Parameters.AddWithValue("@segmentAlias", segment.Alias);
                command.CommandText = $@"
                        SELECT VisitorId, SegmentAlias, CreatedUtc FROM {VisitorSegment.TableName}
                        WHERE {nameof(VisitorSegment.SegmentAlias)} = @segmentAlias";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new VisitorSegment(
                            reader.GetString(0),
                            reader.GetString(1),
                            reader.GetDateTime(2)));
                    }
                }

                if (persistence >= 0)
                {
                    var now = DateTime.Now;
                    result = result.Where(x => (now - x.CreatedUtc).TotalDays <= persistence).ToList();
                }

            }

            return result.AsReadOnly();
        }

        public IReadOnlyCollection<dynamic> GetVisitorsInSegmentWithProperties(Segment segment, IReadOnlyDictionary<string, bool> properties)
        {
            // We don't store properties in SQL database. Yet..?
            return new List<dynamic>().AsReadOnly();
        }
    }
}