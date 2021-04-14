using System;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	public enum SkippableMergeComponent
	{
		FolderRules,
		FolderACLs,
		InitialConnectionValidation,
		FailOnFirstBadItem = 4,
		ContentVerification,
		KnownCorruptions,
		FailOnCorruptSyncState
	}
}
