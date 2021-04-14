using System;

namespace Microsoft.Exchange.Server.Storage.StoreIntegrityCheck
{
	public enum CorruptionType
	{
		[MapToManagement(null, false)]
		MessageIdUniqueIndexExists,
		[MapToManagement(null, false)]
		IncorrectRuleMessageClass,
		[MapToManagement(null, false)]
		ExtraJunkmailRule,
		[MapToManagement(null, false)]
		NoAclStampedOnFolder,
		[MapToManagement(null, false)]
		WrongFolderTypeOnRestrictionFolder,
		[MapToManagement(null, false)]
		UndeletedMessageInMidsetDeleted,
		[MapToManagement(null, false)]
		SearchBacklinksUnsorted,
		[MapToManagement(null, false)]
		SearchFolderNotFound,
		[MapToManagement(null, false)]
		SearchBacklinkNotSearchFolder,
		[MapToManagement(null, false)]
		SearchBacklinkIsNotDynamicSearchFolder,
		[MapToManagement(null, false)]
		FolderOutOfSearchScope,
		[MapToManagement(null, false)]
		SearchBacklinksRecursiveMismatch,
		[MapToManagement(null, false)]
		SearchBacklinksDuplicatedFolder,
		[MapToManagement(null, false)]
		AggregateCountMismatch,
		[MapToManagement(null, false)]
		MissingSpecialFolder,
		[MapToManagement(null, false)]
		InvalidImapID,
		[MapToManagement(null, true)]
		FolderHierarchyRootCountMismatch,
		[MapToManagement(null, true)]
		FolderHierarchyTotalFolderCountMismatch,
		[MapToManagement(null, true)]
		FolderChildrenCountMismatch,
		[MapToManagement(null, true)]
		FolderInformationFidMismatch,
		[MapToManagement(null, true)]
		FolderInformationIsSearchFolderMismatch,
		[MapToManagement(null, true)]
		FolderInformationDisplayNameMismatch,
		[MapToManagement(null, true)]
		FolderInformationIsPartOfContentIndexingMismatch,
		[MapToManagement(null, true)]
		FolderInformationMessageCountMismatch
	}
}
