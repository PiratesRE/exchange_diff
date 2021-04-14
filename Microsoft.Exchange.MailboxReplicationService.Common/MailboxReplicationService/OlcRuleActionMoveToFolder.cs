using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class OlcRuleActionMoveToFolder : OlcRuleActionBase
	{
		[DataMember]
		public byte[] FolderId { get; set; }
	}
}
