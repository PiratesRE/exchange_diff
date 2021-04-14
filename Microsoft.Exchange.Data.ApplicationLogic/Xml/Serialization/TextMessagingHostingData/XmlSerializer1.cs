using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Xml.Serialization.TextMessagingHostingData
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class XmlSerializer1 : XmlSerializer
	{
		protected override XmlSerializationReader CreateReader()
		{
			return new XmlSerializationReaderTextMessagingHostingData();
		}

		protected override XmlSerializationWriter CreateWriter()
		{
			return new XmlSerializationWriterTextMessagingHostingData();
		}
	}
}
