#region Usings

using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Common.Logging;
using CsvHelper;
using CsvHelper.Configuration;
using GoodlyFere.Import.Interfaces;

#endregion

namespace GoodlyFere.Import.Sources
{
    public class CsvSource : ISource
    {
        #region Constants and Fields

        private static readonly ILog Log = LogManager.GetLogger<CsvSource>();

        private readonly string _csvFilePath;
        private readonly string _delimiter;
        private readonly bool _hasHeaders;

        #endregion

        #region Constructors and Destructors

        public CsvSource(string csvFilePath, string hasHeaders, string delimiter)
        {
            _csvFilePath = csvFilePath;
            _hasHeaders = bool.Parse(hasHeaders);
            _delimiter = delimiter;
        }

        #endregion

        #region Public Methods

        public DataTable GetData()
        {
            Log.InfoFormat("Beginning CSV read from file {0}", _csvFilePath);

            DataTable table = new DataTable();
            EnsureFileExists();
            ReadCsvFile(table);

            return table;
        }

        #endregion

        #region Methods

        private static int BuildTableRows(DataTable table, CsvReader csv)
        {
            int count = 0;
            do
            {
                DataRow row = table.NewRow();
                row.ItemArray = csv.CurrentRecord.Cast<object>().ToArray();
                table.Rows.Add(row);
                count++;
            }
            while (csv.Read());
            return count;
        }

        private void BuildTableColumns(CsvReader csv, DataTable table)
        {
            if (_hasHeaders)
            {
                foreach (string header in csv.FieldHeaders)
                {
                    table.Columns.Add(header, typeof(string));
                }
            }
            else
            {
                table.Columns.AddRange(
                    csv.CurrentRecord
                       .Select((val, idx) => new DataColumn("column" + idx, typeof(string)))
                       .ToArray());
            }
        }

        private void EnsureFileExists()
        {
            if (File.Exists(_csvFilePath))
            {
                return;
            }

            Log.Error("CSV file does not exist.");
            throw new ArgumentException("csvFilePath is not an existing file.");
        }

        private void ReadCsvFile(DataTable table)
        {
            int count = 0;
            using (var reader = new StreamReader(_csvFilePath))
            {
                CsvConfiguration config = new CsvConfiguration
                    {
                        Delimiter = _delimiter,
                        Encoding = Encoding.UTF8,
                        HasHeaderRecord = _hasHeaders
                    };

                using (var csv = new CsvReader(reader, config))
                {
                    if (!csv.Read())
                    {
                        throw new Exception("Could not read CSV file.");
                    }

                    BuildTableColumns(csv, table);
                    count = BuildTableRows(table, csv);
                }
            }

            Log.InfoFormat("Done reading {0} CSV file records.", count);
        }

        #endregion
    }
}