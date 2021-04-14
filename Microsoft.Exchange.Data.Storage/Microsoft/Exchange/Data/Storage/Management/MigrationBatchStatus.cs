using System;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public enum MigrationBatchStatus
	{
		[LocDescription(ServerStrings.IDs.MigrationBatchStatusCreated)]
		Created,
		[LocDescription(ServerStrings.IDs.MigrationBatchStatusSyncing)]
		Syncing,
		[LocDescription(ServerStrings.IDs.MigrationBatchStatusStopping)]
		Stopping,
		[LocDescription(ServerStrings.IDs.MigrationBatchStatusStopped)]
		Stopped,
		[LocDescription(ServerStrings.IDs.MigrationBatchStatusCompleted)]
		Completed,
		[LocDescription(ServerStrings.IDs.MigrationBatchStatusFailed)]
		Failed,
		[LocDescription(ServerStrings.IDs.MigrationBatchStatusRemoving)]
		Removing,
		[LocDescription(ServerStrings.IDs.MigrationBatchStatusSynced)]
		Synced,
		[LocDescription(ServerStrings.IDs.MigrationBatchStatusIncrementalSyncing)]
		IncrementalSyncing,
		[LocDescription(ServerStrings.IDs.MigrationBatchStatusCompleting)]
		Completing,
		[LocDescription(ServerStrings.IDs.MigrationBatchStatusCompletedWithErrors)]
		CompletedWithErrors,
		[LocDescription(ServerStrings.IDs.MigrationBatchStatusSyncedWithErrors)]
		SyncedWithErrors,
		[LocDescription(ServerStrings.IDs.MigrationBatchStatusCorrupted)]
		Corrupted,
		[LocDescription(ServerStrings.IDs.MigrationBatchStatusWaiting)]
		Waiting,
		[LocDescription(ServerStrings.IDs.MigrationBatchStatusStarting)]
		Starting
	}
}
