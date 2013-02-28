#region Usings

using System;
using System.Data;
using System.Linq;
using GoodlyFere.Import.Interfaces;

#endregion

namespace GoodlyFere.Import.Converters
{
    public class DoNothingConverter : IConverter
    {
        #region Public Methods

        public DataTable ConvertData(DataTable data)
        {
            return data;
        }

        #endregion
    }
}