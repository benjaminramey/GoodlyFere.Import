#region License

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlSource.cs">
// GoodlyFere.Import
// 
// Copyright (C) 2013 Benjamin Ramey
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
// http://www.gnu.org/licenses/lgpl-2.1-standalone.html
// 
// You can contact me at ben.ramey@gmail.com.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#endregion

#region Usings

using System.Data;
using System.Linq;
using System;
using Common.Logging;
using GoodlyFere.Import.Interfaces;

#endregion

namespace GoodlyFere.Import.Sources
{
    public abstract class SqlSource<T> : ISource
        where T : IDbConnection, new()
    {
        #region Constants and Fields

        private static readonly ILog Log = LogManager.GetLogger(typeof(SqlSource<>));

        #endregion

        #region Constructors and Destructors

        protected SqlSource(string testConnectionString, string query)
        {
            ConnectionString = testConnectionString;
            Query = query;
        }

        #endregion

        #region Properties

        protected string Query { get; set; }
        protected string ConnectionString { get; set; }

        #endregion

        #region Public Methods

        public virtual DataTable GetData()
        {
            Log.Info("Beginning SQL data retrieval.");

            DataTable table = new DataTable();
            using (var conn = new T())
            {
                OpenConnection(conn);
                RetrieveData(conn, table);
            }

            Log.Info("SQL data retrieval is done.");
            return table;
        }

        #endregion

        #region Methods

        protected virtual void OpenConnection(T conn)
        {
            if (conn.Equals(null))
            {
                Log.Error("Connection info is null.  Could not open connection.");
                throw new Exception("Could not create database connection.");
            }

            Log.InfoFormat("Opening connection to {0}", conn.Database);
            conn.ConnectionString = ConnectionString;
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

            Log.InfoFormat("{0} rows retrieved.", table.Rows.Count);
        }

        #endregion
    }
}