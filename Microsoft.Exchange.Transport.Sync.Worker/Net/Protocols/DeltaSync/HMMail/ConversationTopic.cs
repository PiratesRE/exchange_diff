using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.HMMail
{
	[XmlRoot(ElementName = "ConversationTopic", Namespace = "HMMAIL:", IsNullable = false)]
	[Serializable]
	public class ConversationTopic : stringWithEncodingType
	{
	}
}
