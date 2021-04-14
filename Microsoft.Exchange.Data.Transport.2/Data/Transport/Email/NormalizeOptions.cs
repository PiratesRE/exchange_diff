using System;

namespace Microsoft.Exchange.Data.Transport.Email
{
	[Flags]
	internal enum NormalizeOptions
	{
		NormalizeMimeStructure = 1,
		NormalizeMessageId = 2,
		NormalizeCte = 4,
		MergeAddressHeaders = 8,
		RemoveDuplicateHeaders = 16,
		NormalizeMime = 65535,
		DropTnefRecipientTable = 65536,
		DropTnefSenderProperties = 131072,
		NormalizeTnef = 2147418112,
		All = 2147483647
	}
}
