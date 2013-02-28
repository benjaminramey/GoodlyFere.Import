#region Usings

using System;
using System.Data;
using System.Linq;
using GoodlyFere.Import.Interfaces;
using NSubstitute;
using Xunit;

#endregion

namespace GoodlyFere.Import.Tests
{
    public class ImporterTests
    {
        #region Constants and Fields

        private readonly IConverter _converter;
        private readonly IDestination _destination;
        private readonly ISource _source;

        #endregion

        #region Constructors and Destructors

        public ImporterTests()
        {
            _source = Substitute.For<ISource>();
            _destination = Substitute.For<IDestination>();
            _converter = Substitute.For<IConverter>();
        }

        #endregion

        #region Public Methods

        [Fact]
        public void DestinationReceivesConvertedData()
        {
            DataTable originalData = new DataTable("table1");
            DataTable convertedData = new DataTable("table2");
            _source.GetData().Returns(originalData);
            _converter.ConvertData(originalData).Returns(convertedData);

            var importer = new Importer(_source, _converter, _destination);
            importer.Run();

            Assert.NotEqual(originalData, convertedData);
            _destination.Received(1).Receive(convertedData);
        }

        [Fact]
        public void ImporterCallsConverterOnce()
        {
            var importer = new Importer(_source, _converter, _destination);

            importer.Run();

            _converter.Received(1).ConvertData(Arg.Any<DataTable>());
        }

        [Fact]
        public void ImporterCallsDestinationReceiveOnce()
        {
            var importer = new Importer(_source, _converter, _destination);

            importer.Run();

            _destination.Received(1).Receive(Arg.Any<DataTable>());
        }

        [Fact]
        public void ImporterCallsSourceForDataOnce()
        {
            var importer = new Importer(_source, _converter, _destination);

            importer.Run();

            _source.Received(1).GetData();
        }

        [Fact]
        public void RunReturnsFalseIfDestinationReceiveFails()
        {
            _destination.Receive(Arg.Any<DataTable>()).Returns(false);

            var importer = new Importer(_source, _converter, _destination);
            bool result = importer.Run();

            Assert.True(!result);
        }

        [Fact]
        public void RunReturnsTrueIfDestinationReceiveSucceeds()
        {
            _destination.Receive(Arg.Any<DataTable>()).Returns(true);

            var importer = new Importer(_source, _converter, _destination);
            bool result = importer.Run();

            Assert.True(result);
        }

        #endregion
    }
}