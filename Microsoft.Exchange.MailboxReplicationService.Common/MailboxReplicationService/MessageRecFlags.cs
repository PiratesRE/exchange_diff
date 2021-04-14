using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	[Flags]
	internal enum MessageRecFlags
	{
		[EnumMember]
		None = 0,
		[EnumMember]
		Deleted = 1,
		[EnumMember]
		Regular = 2,
		[EnumMember]
		Associated = 4
	}
}
