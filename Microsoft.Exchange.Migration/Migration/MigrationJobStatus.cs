using System;

namespace Microsoft.Exchange.Migration
{
	internal enum MigrationJobStatus
	{
		Created,
		SyncInitializing,
		SyncStarting,
		SyncCompleting,
		SyncCompleted,
		CompletionInitializing,
		CompletionStarting,
		Completing,
		Completed,
		Failed,
		Removed,
		Removing,
		ProvisionStarting,
		Validating,
		Stopped,
		Corrupted
	}
}
