using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	public enum BadItemKind
	{
		MissingItem,
		CorruptItem,
		LargeItem,
		CorruptSearchFolderCriteria,
		CorruptFolderACL,
		CorruptFolderRule,
		MissingFolder,
		MisplacedFolder,
		CorruptFolderProperty,
		CorruptFolderRestriction,
		CorruptInferenceProperties,
		CorruptMailboxSetting,
		FolderPropertyMismatch
	}
}
