using System;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	public enum SkippableMoveComponent
	{
		FolderRules,
		FolderACLs,
		FolderPromotedProperties,
		FolderViews,
		FolderRestrictions,
		ContentVerification,
		BlockFinalization,
		FailOnFirstBadItem,
		KnownCorruptions = 12,
		FailOnCorruptSyncState = 14
	}
}
