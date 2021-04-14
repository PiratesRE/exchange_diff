using System;

namespace Microsoft.Mapi
{
	[Flags]
	internal enum MailboxMiscFlags
	{
		None = 0,
		CreatedByMove = 16,
		ArchiveMailbox = 32,
		DisabledMailbox = 64,
		SoftDeletedMailbox = 128,
		MRSSoftDeletedMailbox = 256
	}
}
