using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	public enum SyncStateError
	{
		None,
		NullSyncState,
		NullIcsSyncState,
		WrongRequestGuid,
		CorruptSyncState,
		CorruptIcsSyncState,
		NullReplaySyncState,
		CorruptReplaySyncState
	}
}
