using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal enum ServerHealthState
	{
		[EnumMember]
		Unknown,
		[EnumMember]
		Healthy,
		[EnumMember]
		NotHealthy
	}
}
