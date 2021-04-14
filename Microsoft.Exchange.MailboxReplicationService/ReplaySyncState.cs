using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.MailboxReplicationService
{
	public sealed class ReplaySyncState : XMLSerializableBase
	{
		[XmlElement(ElementName = "ProviderState")]
		public string ProviderState { get; set; }

		public static ReplaySyncState Deserialize(string data)
		{
			return XMLSerializableBase.Deserialize<ReplaySyncState>(data, true);
		}
	}
}
