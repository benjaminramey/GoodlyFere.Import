#region Usings

using System.Data;
using System.IO;
using System.Linq;
using System;
using System.Xml;
using System.Xml.Xsl;
using GoodlyFere.Import.Interfaces;

#endregion

namespace GoodlyFere.Import.Destinations
{
    public class XmlDestination : IDestination
    {
        #region Constructors and Destructors

        public XmlDestination(string pathToXslt, string pathToResultXml)
        {
            PathToXslt = pathToXslt;
            PathToResultXml = pathToResultXml;
        }

        #endregion

        #region Properties

        protected string PathToResultXml { get; set; }
        protected string PathToXslt { get; set; }

        protected XmlWriterSettings TransformWriterSettings
        {
            get
            {
                return new XmlWriterSettings { OmitXmlDeclaration = false };
            }
        }

        #endregion

        #region Public Methods

        public virtual bool Receive(DataTable data)
        {
            if (string.IsNullOrEmpty(data.TableName))
            {
                data.TableName = "DataTable";
            }

            using (MemoryStream stream = new MemoryStream())
            {
                data.WriteXml(stream);

                stream.Seek(0, SeekOrigin.Begin);
                using (XmlReader reader = new XmlTextReader(stream))
                {
                    TransformXml(reader);
                }
            }

            return true;
        }

        #endregion

        #region Methods

        protected virtual void TransformXml(XmlReader reader)
        {
            XslCompiledTransform transform = new XslCompiledTransform();
            transform.Load(PathToXslt);

            using (XmlWriter writer = XmlWriter.Create(PathToResultXml, TransformWriterSettings))
            {
                transform.Transform(reader, writer);
            }
        }

        #endregion
    }
}