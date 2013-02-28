#region Usings

using System;
using System.Data.SQLite;
using System.Linq;
using GoodlyFere.Import.Sources;
using Xunit;

#endregion

namespace GoodlyFere.Import.Tests.SourceTests
{
    public class SqlSourceTests
    {
        #region Constructors and Destructors

        public SqlSourceTests()
        {
            TestDataHelper.CreateWidgetsTable();
        }

        #endregion

        #region Public Methods

        [Fact]
        public void GetsExpectedData()
        {
            string query = "select * from Widgets";
            var source = new SQLiteSource(TestDataHelper.TestConnectionString, query);
            var results = source.GetData();

            Assert.NotNull(results);
            Assert.Equal(TestDataHelper.Names.Length, results.Rows.Count);
            for (int i = 0; i < TestDataHelper.Names.Length; i++)
            {
                Assert.Equal(TestDataHelper.Names[i], results.Rows[i]["Name"]);
            }
        }

        [Fact]
        public void GetsExpectedData2()
        {
            string expectedName = "bob";
            string query = string.Format("select * from Widgets where name = '{0}'", expectedName);
            var source = new SQLiteSource(TestDataHelper.TestConnectionString, query);
            var results = source.GetData();

            Assert.NotNull(results);
            Assert.Equal(TestDataHelper.Names.Count(n => n == expectedName), results.Rows.Count);
            Assert.Equal(expectedName, results.Rows[0]["Name"]);
        }

        #endregion
    }

    public class SQLiteSource : SqlSource<SQLiteConnection>
    {
        #region Constructors and Destructors

        public SQLiteSource(string testConnectionString, string query)
            : base(testConnectionString, query)
        {
        }

        #endregion
    }
}