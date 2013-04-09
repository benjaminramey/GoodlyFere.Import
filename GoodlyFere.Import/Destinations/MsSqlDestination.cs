#region Usings

using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;

#endregion

namespace GoodlyFere.Import.Destinations
{
    public class MsSqlDestination : SqlDestination<SqlConnection>
    {
        #region Constructors and Destructors

        public MsSqlDestination(string connectionString)
            : base(connectionString)
        {
        }

        #endregion

        #region Methods

        protected override DataAdapter CreateAdapter(SqlConnection connection, DataTable data)
        {
            var command = connection.CreateCommand();
            var adapter = new SqlDataAdapter(command);
            var builder = new SqlCommandBuilder(adapter);

            command.CommandText = string.Format("select * from {0}", data.TableName);

            adapter.InsertCommand = builder.GetInsertCommand();
            adapter.DeleteCommand = builder.GetDeleteCommand();
            adapter.UpdateCommand = builder.GetUpdateCommand();

            return adapter;
        }

        #endregion
    }
}