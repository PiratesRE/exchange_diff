using System;

namespace Microsoft.Exchange.Servicelets.JobQueue.PublicFolder
{
	internal enum SyncActivity
	{
		ClearFolderProperties,
		CommitBatch,
		CreateFolder,
		DeleteFolder,
		EnumerateHierarchyChanges,
		FixOrphanFolders,
		FxCopyProperties,
		GetChangeManifestInitializeSyncContext,
		GetChangeManifestPersistSyncContext,
		GetDestinationFolderIdSet,
		GetDestinationMailboxFolder,
		GetDestinationSessionSpecificEntryId,
		GetFolderRec,
		GetSourceFolderIdSet,
		GetSourceMailboxFolder,
		GetSourceSessionSpecificEntryId,
		MapSourceToDestinationFolderId,
		MoveFolder,
		ProcessNextBatch,
		SetIcsState,
		SetSecurityDescriptor,
		UpdateDumpsterId,
		UpdateFoldersInBatch
	}
}
