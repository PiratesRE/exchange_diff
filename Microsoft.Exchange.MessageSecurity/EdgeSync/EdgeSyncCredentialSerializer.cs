using System;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.MessageSecurity.EdgeSync
{
	internal sealed class EdgeSyncCredentialSerializer : XmlSerializer2
	{
		public override bool CanDeserialize(XmlReader xmlReader)
		{
			return xmlReader.IsStartElement("EdgeSyncCredential", "");
		}

		protected override void Serialize(object objectToSerialize, XmlSerializationWriter writer)
		{
			((XmlSerializationWriterEdgeSyncCredential)writer).Write3_EdgeSyncCredential(objectToSerialize);
		}

		protected override object Deserialize(XmlSerializationReader reader)
		{
			return ((XmlSerializationReaderEdgeSyncCredential)reader).Read3_EdgeSyncCredential();
		}
	}
}
