using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	[Flags]
	internal enum MappedPrincipalFields
	{
		None = 0,
		MailboxGuid = 1,
		ObjectSid = 2,
		ObjectGuid = 4,
		ObjectDN = 8,
		LegacyDN = 16,
		ProxyAddresses = 32,
		Alias = 64,
		DisplayName = 128
	}
}
