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