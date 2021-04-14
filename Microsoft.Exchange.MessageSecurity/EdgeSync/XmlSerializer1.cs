using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.MessageSecurity.EdgeSync
{
	internal abstract class XmlSerializer1 : XmlSerializer
	{
		protected override XmlSerializationReader CreateReader()
		{
			return new XmlSerializationReaderEdgeSubscriptionData();
		}

		protected override XmlSerializationWriter CreateWriter()
		{
			return new XmlSerializationWriterEdgeSubscriptionData();
		}
	}
}
