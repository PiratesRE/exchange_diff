using System;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.MessageSecurity.EdgeSync
{
	internal sealed class EdgeSubscriptionDataSerializer : XmlSerializer1
	{
		public override bool CanDeserialize(XmlReader xmlReader)
		{
			return xmlReader.IsStartElement("EdgeSubscriptionData", "");
		}

		protected override void Serialize(object objectToSerialize, XmlSerializationWriter writer)
		{
			((XmlSerializationWriterEdgeSubscriptionData)writer).Write3_EdgeSubscriptionData(objectToSerialize);
		}

		protected override object Deserialize(XmlSerializationReader reader)
		{
			return ((XmlSerializationReaderEdgeSubscriptionData)reader).Read3_EdgeSubscriptionData();
		}
	}
}
