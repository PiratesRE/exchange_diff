using System;

namespace Microsoft.Exchange.Management.Search
{
	internal enum EventId
	{
		TestSuccessful = 1000,
		TestFailed,
		DetailedStatus,
		TestFailedWithPassive,
		ServiceNotRunning,
		ServerIsNull,
		ADError,
		MapiError,
		SCError,
		RecoveryMailboxDatabaseNotTested,
		ServerNoMdbs,
		MdbSysMbxIsNull,
		ActiveManagerError,
		TaskBaseError,
		MapiStoreError,
		GetNonIpmSubTreeFolderError,
		CreateTestFolderError,
		CreateSearchFolderError,
		CatalogInStateNew,
		CatalogInStateCrawling,
		TestTimeOutError,
		MailboxInStateNotStarted,
		MailboxInStateNormalCrawlInProgress,
		MailboxInStateDeletionPending,
		MailboxInStateInTransit,
		MailboxInStateFailed,
		CIIsDisabled,
		MailboxNotArchived,
		CatalogInUnhealthyState,
		CatalogBacklog
	}
}
