using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal enum UpdateMovedMailboxOperation
	{
		[EnumMember]
		UpdateMailbox = 1,
		[EnumMember]
		MorphToMailbox,
		[EnumMember]
		MorphToMailUser,
		[EnumMember]
		UpdateArchiveOnly
	}
}
