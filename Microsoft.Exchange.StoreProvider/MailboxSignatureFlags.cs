using System;

namespace Microsoft.Mapi
{
	[Flags]
	internal enum MailboxSignatureFlags
	{
		None = 0,
		GetLegacy = 1,
		GetMailboxSignature = 2,
		GetNamedPropertyMapping = 4,
		GetReplidGuidMapping = 8,
		GetMappingMetadata = 16,
		GetMailboxShape = 32
	}
}
