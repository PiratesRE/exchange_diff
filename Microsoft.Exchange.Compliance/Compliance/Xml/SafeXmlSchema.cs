using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace Microsoft.Exchange.Compliance.Xml
{
	internal class SafeXmlSchema : XmlSchema
	{
		public new static XmlSchema Read(Stream stream, ValidationEventHandler validationEventHandler)
		{
			return XmlSchema.Read(new XmlTextReader(stream)
			{
				EntityHandling = EntityHandling.ExpandCharEntities,
				DtdProcessing = DtdProcessing.Prohibit,
				XmlResolver = null
			}, validationEventHandler);
		}

		public new static XmlSchema Read(TextReader reader, ValidationEventHandler validationEventHandler)
		{
			XmlSchema result;
			using (XmlTextReader xmlTextReader = new XmlTextReader(reader))
			{
				xmlTextReader.EntityHandling = EntityHandling.ExpandCharEntities;
				xmlTextReader.DtdProcessing = DtdProcessing.Prohibit;
				xmlTextReader.XmlResolver = null;
				result = XmlSchema.Read(xmlTextReader, validationEventHandler);
			}
			return result;
		}

		public new static XmlSchema Read(XmlReader reader, ValidationEventHandler validationEventHandler)
		{
			if (reader.Settings != null && reader.Settings.DtdProcessing != DtdProcessing.Prohibit)
			{
				throw new XmlDtdException();
			}
			return XmlSchema.Read(reader, validationEventHandler);
		}
	}
}
