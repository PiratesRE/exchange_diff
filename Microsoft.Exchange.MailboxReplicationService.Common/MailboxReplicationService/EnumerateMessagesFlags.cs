using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Flags]
	[DataContract]
	internal enum EnumerateMessagesFlags
	{
		[EnumMember]
		RegularMessages = 1,
		[EnumMember]
		DeletedMessages = 2,
		[EnumMember]
		IncludeExtendedData = 4,
		[EnumMember]
		ReturnLongTermIDs = 8,
		[EnumMember]
		SkipICSMidSetMissing = 16,
		[EnumMember]
		AllMessages = 3
	}
}
