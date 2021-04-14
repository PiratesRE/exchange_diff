using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Flags]
	internal enum OlcCategoryAttributeFlags
	{
		None = 0,
		Disabled = 1,
		Hidden = 2,
		Deleted = 4,
		Harddeleted = 8,
		Reserved1 = 16,
		Reserved2 = 32,
		Reserved3 = 64,
		Reserved4 = 128
	}
}
