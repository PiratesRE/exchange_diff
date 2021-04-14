using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal enum MoveRequestNotification
	{
		[EnumMember]
		Created = 1,
		[EnumMember]
		Updated,
		[EnumMember]
		Canceled,
		[EnumMember]
		SuspendResume
	}
}
