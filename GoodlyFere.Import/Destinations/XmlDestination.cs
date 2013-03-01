#region License

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlDestination.cs">
// GoodlyFere.Import
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