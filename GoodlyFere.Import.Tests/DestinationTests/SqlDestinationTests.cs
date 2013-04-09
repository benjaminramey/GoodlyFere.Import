#region Usings

using System;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using GoodlyFere.Import.Destinations;
using GoodlyFere.Import.Interfaces;
using Xunit;

#endregion

namespace GoodlyFere.Import.Tests.DestinationTests
{
    public class SqlDestinationTests
    {
        #region Constructors and Destructors

        public SqlDestinationTests()
        {
            TestDataHelper.CreateWidgetsTable();
        }

        #endregion

        #region Public Methods

        [Fact]
        public void AcceptsConnectionStringConstructorParameter()
        {
            Assert.DoesNotThrow(() => Activator.CreateInstance(typeof(TestDestination), "string here"));
        }

        [Fact]
        public void GenericTypeParamMustBeIDbConnection()
        {
            Assert.Throws<ArgumentException>(
                () => Activator.CreateInstance(typeof(SqlDestination<>).MakeGenericType(typeof(string))));
        }

        [Fact]
        public void GenericTypeParamMustBeNewable()
        {
            Assert.Throws<ArgumentException>(
                () => Activator.CreateInstance(typeof(SqlDestination<>).MakeGenericType(typeof(IDbConnection))));
        }

        [Fact]
        public void ImplementsIDestinationInterface()
        {
            Assert.True(typeof(IDestination).IsAssignableFrom(typeof(SqlDestination<>)));
        }

        [Fact]
        public void InsertsNewRows()
        {
            var dest = new TestDestination(TestDataHelper.TestConnectionString);
            var newTable = TestDataHelper.GetWidgetsTable();
            var row = newTable.NewRow();

            row["Name"] = "test name 1";
            newTable.Rows.Add(row);
            row = newTable.NewRow();
            row["Name"] = "test name 1";
            newTable.Rows.Add(row);
            row = newTable.NewRow();
            row["Name"] = "test name 1";
            newTable.Rows.Add(row);

            dest.Receive(newTable);

            var updatedTable = TestDataHelper.GetWidgetsTable();

            Assert.Equal(newTable.Rows.Count, updatedTable.Rows.Count);
        }

        [Fact]
        public void UpdatesExistingRows()
        {
            string expectedName = "test update 1";
            var dest = new TestDestination(TestDataHelper.TestConnectionString);
            var newTable = TestDataHelper.GetWidgetsTable();

            newTable.Rows[0]["Name"] = expectedName;

            dest.Receive(newTable);
            var updatedTable = TestDataHelper.GetWidgetsTable();

            Assert.Equal(newTable.Rows.Count, updatedTable.Rows.Count);
            Assert.Equal(expectedName, updatedTable.Rows[0]["Name"].ToString());
        }

        [Fact]
        public void DeletesDeletedRows()
        {
            var dest = new TestDestination(TestDataHelper.TestConnectionString);
            var newTable = TestDataHelper.GetWidgetsTable();
            int originalCount = newTable.Rows.Count;

            newTable.Rows[0].Delete();

            dest.Receive(newTable);
            var updatedTable = TestDataHelper.GetWidgetsTable();

            Assert.Equal(originalCount - 1, updatedTable.Rows.Count);
        }

        [Fact]
        public void RequiresConnectionStringConstructorParam()
        {
            Assert.Throws<MissingMethodException>(
                () => Activator.CreateInstance(typeof(SqlDestination<SQLiteConnection>)));
        }

        [Fact]
        public void VerifiesDataTableName()
        {
            var dest = new TestDestination(string.Empty);
            DataTable namelessTable = new DataTable();

            Assert.Throws<ArgumentException>(() => dest.Receive(namelessTable));
        }

        #endregion
    }

    public class TestDestination : SqlDestination<SQLiteConnection>
    {
        #region Constructors and Destructors

        public TestDestination(string connectionString)
            : base(connectionString)
        {
        }

        protected override DataAdapter CreateAdapter(SQLiteConnection connection, DataTable data)
        {
            var command = connection.CreateCommand();
            var adapter = new SQLiteDataAdapter();
            var builder = new SQLiteCommandBuilder(adapter);

            command.CommandText = string.Format("select * from {0}", data.TableName);

            adapter.SelectCommand = command;
            adapter.InsertCommand = builder.GetInsertCommand();
            adapter.DeleteCommand = builder.GetDeleteCommand();
            adapter.UpdateCommand = builder.GetUpdateCommand();
            
            return adapter;
        }

        #endregion
    }
}