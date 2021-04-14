using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.HMMail
{
	[XmlRoot(ElementName = "ConversationIndex", Namespace = "HMMAIL:", IsNullable = false)]
	[Serializable]
	public class ConversationIndex : stringWithEncodingType
	{
	}
}
