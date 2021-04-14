using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Mserve
{
	public abstract class XmlSerializer1 : XmlSerializer
	{
		protected override XmlSerializationReader CreateReader()
		{
			return new XmlSerializationReaderRecipientSyncState();
		}

		protected override XmlSerializationWriter CreateWriter()
		{
			return new XmlSerializationWriterRecipientSyncState();
		}
	}
}
