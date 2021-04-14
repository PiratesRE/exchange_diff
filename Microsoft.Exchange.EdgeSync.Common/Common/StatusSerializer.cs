using System;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EdgeSync.Common
{
	internal sealed class StatusSerializer : XmlSerializer1
	{
		public override bool CanDeserialize(XmlReader xmlReader)
		{
			return xmlReader.IsStartElement("Status", "");
		}

		protected override void Serialize(object objectToSerialize, XmlSerializationWriter writer)
		{
			((XmlSerializationWriterStatus)writer).Write5_Status(objectToSerialize);
		}

		protected override object Deserialize(XmlSerializationReader reader)
		{
			return ((XmlSerializationReaderStatus)reader).Read5_Status();
		}
	}
}
