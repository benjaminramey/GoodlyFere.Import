#region License

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestDataHelper.cs">
// GoodlyFere.Import.Tests
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

using System;
using System.Data;
using System.Data.SQLite;
using System.Linq;

#endregion

namespace GoodlyFere.Import.Tests
{
    public static class TestDataHelper
    {
        #region Constants and Fields

        public const string TestConnectionString = @"Data Source=testdb.db;Version=3;";
        public static readonly string[] Names = new[] { "bob", "joe", "sam", "alfred", "ben", "alex" };

        #endregion

        #region Public Methods

        public static void CreateWidgetsTable()
        {
            using (var conn = new SQLiteConnection(TestConnectionString))
            {
                conn.Open();
                var command = conn.CreateCommand();
                command.CommandText = "DROP TABLE IF EXISTS Widgets";
                command.ExecuteNonQuery();
                command.CommandText =
                    "CREATE TABLE Widgets (id integer PRIMARY KEY AUTOINCREMENT, name varchar(50))";
                command.ExecuteNonQuery();

                foreach (string name in Names)
                {
                    command = conn.CreateCommand();
                    command.CommandText = string.Format("INSERT INTO Widgets (name) values('{0}')", name);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static DataTable GetWidgetsTable()
        {
            using (var conn = new SQLiteConnection(TestConnectionString))
            {
                conn.Open();

                DataSet ds = new DataSet();
                SQLiteDataAdapter da = new SQLiteDataAdapter("select * from Widgets", conn);
                
                da.Fill(ds, "Widgets");
                return ds.Tables["Widgets"];
            }
        }

        #endregion
    }
}