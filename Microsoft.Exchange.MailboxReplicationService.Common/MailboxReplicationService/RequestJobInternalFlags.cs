using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Flags]
	public enum RequestJobInternalFlags
	{
		None = 0,
		RestartFromScratch = 1,
		ForceOfflineMove = 2,
		SkipFolderRules = 4,
		SkipFolderACLs = 8,
		SkipFolderPromotedProperties = 16,
		SkipFolderViews = 32,
		SkipFolderRestrictions = 64,
		PreventCompletion = 128,
		SkipInitialConnectionValidation = 256,
		SkipContentVerification = 512,
		BlockFinalization = 1024,
		SkipStorageProviderForSource = 4096,
		SkipPreFinalSyncDataProcessing = 8192,
		FailOnFirstBadItem = 32768,
		SkipMailboxReleaseCheck = 65536,
		SkipKnownCorruptions = 131072,
		IncrementallyUpdateGlobalCounterRanges = 262144,
		ExecutedByTransportSync = 524288,
		SkipProvisioningCheck = 1048576,
		FailOnCorruptSyncState = 2097152,
		SkipConvertingSourceToMeu = 4194304,
		ResolveServer = 8388608,
		UseTcp = 16777216,
		CrossResourceForest = 67108864,
		SkipWordBreaking = 134217728,
		UseCertificateAuthentication = 268435456,
		InvalidateContentIndexAnnotations = 536870912
	}
}
