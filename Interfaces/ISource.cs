#region Usings

using System.Data;
using System.Linq;
using System;

#endregion

namespace GoodlyFere.Import.Interfaces
{
    public interface ISource
    {
        #region Public Methods

        DataTable GetData();

        #endregion
    }
}