using System;

namespace Microsoft.Mapi
{
	[Flags]
	internal enum SyncConfigFlags
	{
		None = 0,
		Unicode = 1,
		NoDeletions = 2,
		NoSoftDeletions = 4,
		ReadState = 8,
		Associated = 16,
		Normal = 32,
		NoConflicts = 64,
		OnlySpecifiedProps = 128,
		NoForeignKeys = 256,
		LimitedIMessage = 512,
		Catchup = 1024,
		Conversations = 2048,
		MsgSelective = 4096,
		BestBody = 8192,
		IgnoreSpecifiedOnAssociated = 16384,
		ProgressMode = 32768,
		FXRecoverMode = 65536,
		DeferConfig = 131072,
		ForceUnicode = 262144,
		NoChanges = 524288,
		OrderByDeliveryTime = 1048576,
		ReevaluateOnRestrictionChange = 2097152,
		ManifestHierReturnDeletedEntryIds = 4194304,
		UseCpId = 16777216,
		SendPropsErrors = 33554432,
		ManifestMode = 67108864,
		CatchupFull = 134217728
	}
}
