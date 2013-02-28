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
                da.TableMappings.Add("Widgets", "Widget");

                da.Fill(ds, "Widgets");
                return ds.Tables["Widget"];
            }
        }

        #endregion
    }
}