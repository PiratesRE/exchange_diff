using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.MessageSecurity.EdgeSync
{
	internal abstract class XmlSerializer2 : XmlSerializer
	{
		protected override XmlSerializationReader CreateReader()
		{
			return new XmlSerializationReaderEdgeSyncCredential();
		}

		protected override XmlSerializationWriter CreateWriter()
		{
			return new XmlSerializationWriterEdgeSyncCredential();
		}
	}
}
