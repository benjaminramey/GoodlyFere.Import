#region Usings

using System.Data;
using System.Linq;
using System;
using GoodlyFere.Import.Interfaces;

#endregion

namespace GoodlyFere.Import.Sources
{
    public abstract class SqlSource<T> : ISource
        where T : IDbConnection, new()
    {
        #region Constructors and Destructors

        protected SqlSource(string testConnectionString, string query)
        {
            TestConnectionString = testConnectionString;
            Query = query;
        }

        #endregion

        #region Properties

        protected string Query { get; set; }
        protected string TestConnectionString { get; set; }

        #endregion

        #region Public Methods

        public virtual DataTable GetData()
        {
            DataTable table = new DataTable();
            using (var conn = new T())
            {
                OpenConnection(conn);
                RetrieveData(conn, table);
            }

            return table;
        }

        #endregion

        #region Methods

        protected virtual void OpenConnection(T conn)
        {
            if (conn.Equals(null))
            {
                throw new Exception("Could not create database connection.");
            }

            conn.ConnectionString = TestConnectionString;
            conn.Open();
        }

        protected virtual void RetrieveData(T conn, DataTable table)
        {
            IDbCommand command = conn.CreateCommand();
            command.CommandText = Query;

            using (IDataReader reader = command.ExecuteReader())
            {
                if (reader == null)
                {
                    throw new Exception(string.Format("Could not open data reader to database. Query: {0}", Query));
                }

                table.Load(reader, LoadOption.OverwriteChanges);
            }
        }

        #endregion
    }
}