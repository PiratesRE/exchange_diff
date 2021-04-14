using System;

namespace Microsoft.Exchange.Data.Directory.Cache
{
	[Flags]
	internal enum KeyType
	{
		None = 0,
		Name = 1,
		DistinguishedName = 2,
		Guid = 4,
		ExternalDirectoryOrganizationId = 8,
		DomainName = 16,
		Sid = 32,
		ExchangeGuid = 64,
		AggregatedMailboxGuid = 128,
		ArchiveGuid = 256,
		LegacyExchangeDN = 512,
		EmailAddresses = 1024,
		MasterAccountSid = 2048,
		SidHistory = 4096,
		OrgCUDN = 8192,
		NetId = 16384
	}
}
