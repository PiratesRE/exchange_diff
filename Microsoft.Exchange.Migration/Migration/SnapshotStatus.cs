using System;

namespace Microsoft.Exchange.Migration
{
	internal enum SnapshotStatus
	{
		InProgress,
		Failed,
		AutoSuspended,
		Corrupted,
		Removed,
		CompletedWithWarning,
		Finalized,
		Suspended,
		Synced
	}
}
