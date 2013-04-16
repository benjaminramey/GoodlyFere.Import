#region Usings

using System.Data;
using System.Data.Common;
using System.Linq;
using System;
using Common.Logging;
using GoodlyFere.Import.Interfaces;

#endregion

namespace GoodlyFere.Import.Destinations
{
    public abstract class SqlDestination<TConnection> : IDestination
        where TConnection : IDbConnection, new()
    {
        #region Constants and Fields

        private static readonly ILog Log = LogManager.GetLogger(typeof(SqlDestination<>));
        private readonly string _connectionString;

        #endregion

        #region Constructors and Destructors

        public SqlDestination(string connectionString)
        {
            _connectionString = connectionString;
        }

        #endregion

        #region Public Methods

        public bool Receive(DataTable data)
        {
            Log.Info("=> Beginning SQL destination insert/updates");

            VerifyTable(data);
            using (var conn = new TConnection())
            {
                OpenConnection(conn);
                SaveData(conn, data);
            }

            Log.Info("=| SQL destination insert/updates complete");

            return true;
        }

        #endregion

        #region Methods

        protected virtual DataAdapter CreateAdapter(TConnection connection, DataTable data)
        {
            throw new NotImplementedException();
        }

        private void OpenConnection(TConnection conn)
        {
            conn.ConnectionString = _connectionString;
            conn.Open();
        }

        private void SaveData(TConnection conn, DataTable data)
        {
            using (DataAdapter adapter = CreateAdapter(conn, data))
            {
                adapter.TableMappings.Add("Table", data.TableName);

                if (data.DataSet == null)
                {
                    DataSet set = new DataSet();
                    set.Tables.Add(data);
                }

                adapter.Update(data.DataSet);
            }
        }

        private void VerifyTable(DataTable data)
        {
            if (string.IsNullOrWhiteSpace(data.TableName))
            {
                throw new ArgumentException("DataTable must have a TableName set.", "data");
            }
        }

        #endregion
    }
}