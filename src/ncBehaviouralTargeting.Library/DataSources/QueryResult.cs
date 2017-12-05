using System.Collections.Generic;

namespace ncBehaviouralTargeting.Library.DataSources
{
    public class QueryResult
    {
        public const int Limit = 100;

        public QueryResult(long total, IReadOnlyList<string> headers, IReadOnlyList<dynamic> documents)
        {
            Total = total;
            Headers = headers;
            Documents = documents;
        }

        public long Total { get; }
        public int Take => Limit;
        public IReadOnlyList<string> Headers { get; }
        public IReadOnlyList<dynamic> Documents { get; }
    }
}