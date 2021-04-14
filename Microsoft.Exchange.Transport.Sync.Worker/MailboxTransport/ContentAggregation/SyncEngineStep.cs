using System;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	internal enum SyncEngineStep
	{
		PreSyncStepInEnumerateChangesMode,
		PreSyncStepInCheckForChangesMode,
		AuthenticateCloudInCheckForChangesMode,
		AuthenticateCloudInEnumerateChangesMode,
		CheckForChangesInCloud,
		EnumCloud,
		ApplyNative,
		AcknowledgeCloud,
		PostSyncStepInCheckForChangesMode,
		PostSyncStepInEnumerateChangesMode,
		GetCloudStatistcsInEnumerateChangesMode,
		End
	}
}
