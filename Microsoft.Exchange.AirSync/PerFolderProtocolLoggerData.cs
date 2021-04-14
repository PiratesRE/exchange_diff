using System;

namespace Microsoft.Exchange.AirSync
{
	internal enum PerFolderProtocolLoggerData
	{
		FolderId,
		FolderDataType,
		SyncType,
		FilterType,
		SmsFilterType,
		ClientSyncKey,
		ServerSyncKey,
		SyncStateKb,
		SyncStateKbLeftCompressed,
		ClientAdds,
		ClientChanges,
		ClientDeletes,
		ClientFetches,
		ClientFailedToConvert,
		ClientFailedToSend,
		ClientSends,
		ServerAdds,
		ServerChanges,
		ServerDeletes,
		ServerSoftDeletes,
		ServerFailedToConvert,
		ServerChangeTrackingRejected,
		PerFolderStatus,
		ServerAssociatedAdds,
		SkippedDeletes,
		BodyRequested,
		BodyPartRequested,
		SyncStateKbCommitted,
		TotalSaveCount,
		ColdSaveCount,
		ColdCopyCount,
		TotalLoadCount,
		MidnightRollover,
		FirstTimeSyncItemsDiscarded,
		ProviderSyncType,
		GetChangesIterations,
		GetChangesTime
	}
}
