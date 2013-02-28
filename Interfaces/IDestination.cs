#region Usings

using System.Data;
using System.Linq;
using System;

#endregion

namespace GoodlyFere.Import.Interfaces
{
    public interface IDestination
    {
        #region Public Methods

        bool Receive(DataTable data);

        #endregion
    }
}