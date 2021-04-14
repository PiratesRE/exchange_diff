using System;

namespace Microsoft.Exchange.Management.Tasks
{
	public enum CorruptionType
	{
		MessageIdUniqueIndexExists,
		IncorrectRuleMessageClass,
		ExtraJunkmailRule,
		NoAclStampedOnFolder,
		WrongFolderTypeOnRestrictionFolder,
		UndeletedMessageInMidsetDeleted,
		SearchBacklinksUnsorted,
		SearchFolderNotFound,
		SearchBacklinkNotSearchFolder,
		SearchBacklinkIsNotDynamicSearchFolder,
		FolderOutOfSearchScope,
		SearchBacklinksRecursiveMismatch,
		SearchBacklinksDuplicatedFolder,
		AggregateCountMismatch,
		MissingSpecialFolder,
		InvalidImapID
	}
}
