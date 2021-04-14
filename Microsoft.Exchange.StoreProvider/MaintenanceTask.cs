using System;

namespace Microsoft.Mapi
{
	internal enum MaintenanceTask
	{
		FolderTombstones = 1,
		FolderConflictAging,
		SiteFolderCheck,
		EventHistoryCleanup,
		TombstonesMaintenance,
		PurgeIndices,
		PFExpiry,
		UpdateServerVersions,
		HardDeletes,
		DeletedMailboxCleanup,
		ReReadMDBQuotas,
		ReReadAuditInfo,
		InvCachedDsInfo,
		DbSizeCheck,
		DeliveredToCleanup,
		FolderCleanup,
		AgeOutAllFolders,
		AgeOutAllViews,
		AgeOutAllDVUEntries,
		MdbHealthCheck,
		QuarantinedMailboxCleanup,
		RequestTimeoutTest,
		DeletedCiFailedItemCleanup,
		ISINTEGProvisionedFolder
	}
}
