using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	public enum SyncStateClearReason
	{
		MoveComplete = 1,
		MailboxSignatureChange,
		CleanupOrphanedMailbox,
		DeleteReplica,
		JobCanceled,
		MergeComplete
	}
}
