using System;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	[Flags]
	internal enum MailboxMiscFlags
	{
		None = 0,
		QuotaExceeded = 1,
		Gateway = 2,
		Mailbox = 4,
		SDNotInSyncWithDS = 8,
		CreatedByMove = 16,
		ArchiveMailbox = 32,
		DisabledMailbox = 64,
		SoftDeletedMailbox = 128,
		MRSSoftDeletedMailbox = 256,
		MRSPreservingMailboxSignature = 512
	}
}
