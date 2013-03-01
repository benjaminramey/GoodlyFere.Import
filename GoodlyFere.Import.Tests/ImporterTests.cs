#region License

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImporterTests.cs">
// GoodlyFere.Import.Tests
// 
// Copyright (C) 2013 Benjamin Ramey
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
// http://www.gnu.org/licenses/lgpl-2.1-standalone.html
// 
// You can contact me at ben.ramey@gmail.com.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#endregion

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