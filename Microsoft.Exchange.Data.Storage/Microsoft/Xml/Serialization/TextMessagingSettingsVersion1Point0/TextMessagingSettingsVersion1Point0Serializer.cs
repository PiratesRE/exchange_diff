using System;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Xml.Serialization.TextMessagingSettingsVersion1Point0
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class TextMessagingSettingsVersion1Point0Serializer : XmlSerializer1
	{
		public override bool CanDeserialize(XmlReader xmlReader)
		{
			return xmlReader.IsStartElement("TextMessagingSettings", "");
		}

		protected override void Serialize(object objectToSerialize, XmlSerializationWriter writer)
		{
			((XmlSerializationWriterTextMessagingSettingsVersion1Point0)writer).Write9_TextMessagingSettings(objectToSerialize);
		}

		protected override object Deserialize(XmlSerializationReader reader)
		{
			return ((XmlSerializationReaderTextMessagingSettingsVersion1Point0)reader).Read9_TextMessagingSettings();
		}
	}
}
