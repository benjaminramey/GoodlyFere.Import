#region License

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlSourceTests.cs">
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