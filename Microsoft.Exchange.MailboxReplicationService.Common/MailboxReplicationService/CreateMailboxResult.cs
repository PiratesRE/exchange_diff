using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal enum CreateMailboxResult
	{
		[EnumMember]
		Success,
		[EnumMember]
		CleanupNotComplete,
		[EnumMember]
		ObjectNotFound
	}
}
