using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal enum MailboxType
	{
		[EnumMember]
		SourceMailbox,
		[EnumMember]
		DestMailboxIntraOrg,
		[EnumMember]
		DestMailboxCrossOrg
	}
}
