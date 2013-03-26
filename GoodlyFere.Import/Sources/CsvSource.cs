#region Usings

using System;
using System.Data;
using System.IO;
using System.Linq;
using Common.Logging;
using CsvHelper;
using GoodlyFere.Import.Interfaces;

#endregion

namespace GoodlyFere.Import.Sources
{
    public class CsvSource : ISource
    {
        private static readonly ILog Log = LogManager.GetLogger<CsvSource>();

        #region Constants and Fields

        private string _csvFilePath;

        #endregion

        #region Constructors and Destructors

        public CsvSource(string csvFilePath)
        {
            _csvFilePath = csvFilePath;
        }

        #endregion

        #region Public Methods

        public DataTable GetData()
        {
            Log.InfoFormat("Beginning CSV read from file {0}", _csvFilePath);

            DataTable table = new DataTable();
            if (!File.Exists(_csvFilePath))
            {
                Log.Error("CSV file does not exist.");
                throw new ArgumentException("csvFilePath is not an existing file.");
            }

            int count = 0;
            using (var reader = new StreamReader(_csvFilePath))
            {
                using (var csv = new CsvReader(reader))
                {
                    foreach (string header in csv.FieldHeaders)
                    {
                        table.Columns.Add(header, typeof(string));
                    }

                    while (csv.Read())
                    {
                        DataRow row = table.NewRow();
                        row.ItemArray = csv.CurrentRecord.Cast<object>().ToArray();
                        count++;
                    }
                }
            }

            Log.InfoFormat("Done reading {0} CSV file records.", count);

            return table;
        }

        #endregion
    }
}