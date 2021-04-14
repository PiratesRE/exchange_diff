using System;

namespace Microsoft.Exchange.Data.MailboxSignature
{
	[Flags]
	internal enum MailboxSignatureSectionType : short
	{
		None = 0,
		BasicInformation = 1,
		MappingMetadata = 2,
		NamedPropertyMapping = 4,
		ReplidGuidMapping = 8,
		TenantHint = 16,
		MailboxShape = 32,
		MailboxTypeVersion = 64,
		PartitionInformation = 128,
		UserInformation = 256
	}
}
