#region Usings

using System;
using System.Data.SqlClient;
using System.Linq;

#endregion

namespace GoodlyFere.Import.Sources
{
    public class MsSqlSource : SqlSource<SqlConnection>
    {
        #region Constructors and Destructors

        public MsSqlSource(string testConnectionString, string query)
            : base(testConnectionString, query)
        {
        }

        #endregion
    }
}