using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class InboxRuleSettings : ItemPropertiesBase
	{
		[DataMember]
		public OlcInboxRule[] Rules { get; set; }
	}
}
