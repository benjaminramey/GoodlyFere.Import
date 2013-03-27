#region Usings

using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using GoodlyFere.Import.Sources;
using Xunit;

#endregion

namespace GoodlyFere.Import.Tests.SourceTests
{
    public class CsvSourceTests : IDisposable
    {
        #region Constants and Fields

        private readonly string _csvLines;
        private readonly string _headers;
        private readonly string _headersFile;
        private readonly string _noHeadersFile;
        private readonly string _semicolonCsvLines;
        private readonly string _semicolonFile;

        #endregion

        #region Constructors and Destructors

        public CsvSourceTests()
        {
            _noHeadersFile = @"C:\Windows\Temp\__import\noheaders.csv";
            _headersFile = @"C:\Windows\Temp\__import\headers.csv";
            _semicolonFile = @"C:\Windows\Temp\__import\semicolons.csv";

            _headers = "col 1,col 2,col 3,col 4, col 5";
            _csvLines = "1,2,3,4,5";
            _semicolonCsvLines = "1;2;3;4;5";

            File.WriteAllText(_noHeadersFile, _csvLines, Encoding.UTF8);
            File.WriteAllLines(_headersFile, new[] { _headers, _csvLines }, Encoding.UTF8);
            File.WriteAllText(_semicolonFile, _semicolonCsvLines, Encoding.UTF8);
        }

        #endregion

        #region Public Methods

        public void Dispose()
        {
            File.Delete(_noHeadersFile);
            File.Delete(_headersFile);
            File.Delete(_semicolonFile);
        }

        [Fact]
        public void HeadersFile_ParsesWithHeaderNames()
        {
            var source = new CsvSource(_headersFile, "true", ",");
            DataTable table = source.GetData();

            Assert.Equal(_csvLines.Split(',').Length, table.Columns.Count);
            Assert.Equal(1, table.Rows.Count);

            string[] headerNames = _headers.Split(',');
            for (int idx = 0; idx < headerNames.Length; idx++)
            {
                Assert.Equal(headerNames[idx], table.Columns[idx].ColumnName);
            }
        }

        [Fact]
        public void NoHeadersFile_ParsesCorrectly()
        {
            var source = new CsvSource(_noHeadersFile, "false", ",");
            DataTable table = source.GetData();

            Assert.Equal(_csvLines.Split(',').Length, table.Columns.Count);
            Assert.Equal(1, table.Rows.Count);
        }

        [Fact]
        public void NonExistentFile_Throws()
        {
            var source = new CsvSource(@"c:\does\not\exist.csv", "false", ",");

            Assert.Throws<ArgumentException>(() => source.GetData());
        }

        [Fact]
        public void SayHasHeaders_NoHeaders_Throws()
        {
            var source = new CsvSource(_noHeadersFile, "true", ",");

            Assert.Throws<Exception>(() => source.GetData());
        }

        [Fact]
        public void SemiColonDelimited_ParsesCorrectly()
        {
            var source = new CsvSource(_noHeadersFile, "false", ";");
            DataTable table = source.GetData();

            Assert.Equal(_csvLines.Split(';').Length, table.Columns.Count);
            Assert.Equal(1, table.Rows.Count);
        }

        #endregion
    }
}