#region Usings

using System.Data;
using System.Linq;
using System;

#endregion

namespace GoodlyFere.Import.Interfaces
{
    public interface IConverter
    {
        #region Public Methods

        DataTable ConvertData(DataTable data);

        #endregion
    }
}