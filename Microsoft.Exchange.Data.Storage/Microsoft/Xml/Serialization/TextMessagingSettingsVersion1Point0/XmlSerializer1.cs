using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Xml.Serialization.TextMessagingSettingsVersion1Point0
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class XmlSerializer1 : XmlSerializer
	{
		protected override XmlSerializationReader CreateReader()
		{
			return new XmlSerializationReaderTextMessagingSettingsVersion1Point0();
		}

		protected override XmlSerializationWriter CreateWriter()
		{
			return new XmlSerializationWriterTextMessagingSettingsVersion1Point0();
		}
	}
}
