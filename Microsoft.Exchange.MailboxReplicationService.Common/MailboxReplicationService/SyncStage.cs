using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	public enum SyncStage
	{
		[EnumMember]
		Error = -1,
		[EnumMember]
		None,
		[EnumMember]
		CreatingMailbox,
		[EnumMember]
		MailboxCreated,
		[EnumMember]
		CreatingFolderHierarchy,
		[EnumMember]
		CreatingInitialSyncCheckpoint,
		[EnumMember]
		LoadingMessages,
		[EnumMember]
		CopyingMessages,
		[EnumMember]
		IncrementalSync = 10,
		[EnumMember]
		FinalIncrementalSync,
		[EnumMember]
		Cleanup,
		[EnumMember]
		SyncFinished = 100,
		CleanupResetTargetMailbox = 1001,
		CleanupSeedTargetMBICache,
		CleanupDeleteSourceMailbox,
		CleanupUnableToRehomeRelatedRequests,
		CleanupUpdateMovedMailboxWarning,
		CleanupUnableToComputeTargetAddress,
		CleanupUnableToUpdateSourceMailbox,
		BadItem,
		CleanupUnableToGuaranteeUnlock,
		CleanupUnableToLoadTargetMailbox
	}
}
