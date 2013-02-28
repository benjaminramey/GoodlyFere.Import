#region Usings

using System.Data;
using System.Linq;
using System;
using GoodlyFere.Import.Interfaces;

#endregion

namespace GoodlyFere.Import
{
    public class Importer
    {
        #region Constants and Fields

        private readonly IConverter _converter;
        private readonly IDestination _destination;
        private readonly ISource _source;

        #endregion

        #region Constructors and Destructors

        public Importer(ISource source, IConverter converter, IDestination destination)
        {
            _source = source;
            _converter = converter;
            _destination = destination;
        }

        #endregion

        #region Public Methods

        public bool Run()
        {
            DataTable data = _source.GetData();
            DataTable convertedData = _converter.ConvertData(data);

            bool receiveResult = _destination.Receive(convertedData);

            return receiveResult;
        }

        #endregion
    }
}