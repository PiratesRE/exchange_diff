using System;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Xml.Serialization.TextMessagingHostingData
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class TextMessagingHostingDataSerializer : XmlSerializer1
	{
		public override bool CanDeserialize(XmlReader xmlReader)
		{
			return xmlReader.IsStartElement("TextMessagingHostingData", "");
		}

		protected override void Serialize(object objectToSerialize, XmlSerializationWriter writer)
		{
			((XmlSerializationWriterTextMessagingHostingData)writer).Write20_TextMessagingHostingData(objectToSerialize);
		}

		protected override object Deserialize(XmlSerializationReader reader)
		{
			return ((XmlSerializationReaderTextMessagingHostingData)reader).Read20_TextMessagingHostingData();
		}
	}
}
