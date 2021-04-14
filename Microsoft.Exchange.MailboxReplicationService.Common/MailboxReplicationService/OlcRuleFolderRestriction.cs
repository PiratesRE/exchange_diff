using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class OlcRuleFolderRestriction : OlcRuleRestrictionBase
	{
		[DataMember]
		public bool Recursive { get; set; }

		[DataMember]
		public byte[] FolderId { get; set; }
	}
}
