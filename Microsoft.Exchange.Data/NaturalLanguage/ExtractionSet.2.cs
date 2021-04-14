using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.NaturalLanguage
{
	public abstract class ExtractionSet<TExtraction> : ExtractionSet, IXmlSerializable
	{
		protected ExtractionSet(IExtractionSerializer<TExtraction> extractionSerializer)
		{
			this.Version = ExtractionVersions.CurrentVersion;
			this.serializer = extractionSerializer;
		}

		public Version Version { get; private set; }

		public IEnumerable<TExtraction> Extractions { get; set; }

		public override bool IsEmpty
		{
			get
			{
				return this.Extractions == null || !this.Extractions.Any<TExtraction>();
			}
		}

		public XmlSchema GetSchema()
		{
			return null;
		}

		public void ReadXml(XmlReader reader)
		{
			if (reader.IsEmptyElement)
			{
				reader.ReadStartElement();
				return;
			}
			reader.ReadStartElement();
			reader.ReadStartElement("Version");
			string version = reader.ReadContentAsString();
			reader.ReadEndElement();
			this.Version = new Version(version);
			this.Extractions = this.serializer.ReadXml(reader, this.Version);
			reader.ReadEndElement();
		}

		public void WriteXml(XmlWriter writer)
		{
			if (this.Extractions != null && this.Extractions.Any<TExtraction>())
			{
				writer.WriteStartElement("Version");
				writer.WriteString(this.Version.ToString());
				writer.WriteEndElement();
				this.serializer.WriteXml(writer, this.Extractions.ToArray<TExtraction>());
			}
		}

		private const string VersionElementName = "Version";

		private IExtractionSerializer<TExtraction> serializer;
	}
}
