using System;
using System.IO;
using Mongo2Go;

namespace ncBehaviouralTargeting.Library.IntegrationTests
{
    public class MongoDbFixture : IDisposable
    {
        static readonly string MongoDbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin", "data", "db");

        readonly MongoDbRunner runner;

        public MongoDbFixture()
        {
            runner = MongoDbRunner.Start(MongoDbPath);
        }

        public string ConnectionString => runner.ConnectionString;

        public void Dispose()
        {
            runner.Dispose();
        }
    }
}