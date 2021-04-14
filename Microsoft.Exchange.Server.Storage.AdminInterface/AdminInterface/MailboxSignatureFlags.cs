using System;

namespace Microsoft.Exchange.Server.Storage.AdminInterface
{
	[Flags]
	internal enum MailboxSignatureFlags : uint
	{
		None = 0U,
		GetLegacy = 1U,
		GetMailboxSignature = 2U,
		GetNamedPropertyMapping = 4U,
		GetReplidGuidMapping = 8U,
		GetMappingMetadata = 16U,
		GetMailboxShape = 32U,
		AcceptableFlagsMask = 63U
	}
}
