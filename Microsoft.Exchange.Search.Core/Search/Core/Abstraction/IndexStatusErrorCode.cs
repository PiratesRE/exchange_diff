using System;

namespace Microsoft.Exchange.Search.Core.Abstraction
{
	internal enum IndexStatusErrorCode
	{
		Unknown,
		Success,
		InternalError,
		CrawlingDatabase,
		DatabaseOffline,
		MapiNetworkError,
		CatalogCorruption,
		SeedingCatalog,
		CatalogSuspended,
		CatalogReseed,
		IndexNotEnabled,
		CatalogExcluded,
		ActivationPreferenceSkipped,
		LagCopySkipped,
		RecoveryDatabaseSkipped,
		FastError,
		ServiceNotRunning,
		IndexStatusTimestampTooOld,
		CatalogCorruptionWhenFeedingStarts,
		CatalogCorruptionWhenFeedingCompletes,
		EventsMissingWithNotificationsWatermark,
		CrawlOnNonPreferredActiveWithNotificationsWatermark,
		CrawlOnNonPreferredActiveWithTooManyNotificationEvents,
		CrawlOnPassive
	}
}
