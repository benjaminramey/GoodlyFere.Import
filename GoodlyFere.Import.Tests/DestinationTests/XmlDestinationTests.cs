#region License

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlDestinationTests.cs">
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
using System.Xml;
using System.Xml.Xsl;
using GoodlyFere.Import.Destinations;
using Xunit;

#endregion

namespace GoodlyFere.Import.Tests.DestinationTests
{
    public class XmlDestinationTests
    {
        #region Constants and Fields

        private const string PathToResultXml = @"xmldestination_result.xml";
        private const string PathToXslt = @"App_Data\xmldestination_test.xslt";

        #endregion

        #region Constructors and Destructors

        public XmlDestinationTests()
        {
            TestDataHelper.CreateWidgetsTable();
        }

        #endregion

        #region Public Methods

        [Fact]
        public void ProducesExpectedXml()
        {
            var destination = new XmlDestination(PathToXslt, PathToResultXml);
            DataTable table = TestDataHelper.GetWidgetsTable();
            destination.Receive(table);

            string expectedXmlPath = "xmldestinaton_result_expected.xml";
            string intermediateXmlPath = "xmldestination_result_intermediate.xml";
            table.WriteXml(intermediateXmlPath);
            XslCompiledTransform transform = new XslCompiledTransform();
            transform.Load(PathToXslt);
            transform.Transform(intermediateXmlPath, expectedXmlPath);

            XmlDocument expectedXml = new XmlDocument();
            expectedXml.Load(expectedXmlPath);
            XmlDocument actualXml = new XmlDocument();
            actualXml.Load(PathToResultXml);

            Assert.Equal(expectedXml.OuterXml, actualXml.OuterXml);
        }

        #endregion
    }
}