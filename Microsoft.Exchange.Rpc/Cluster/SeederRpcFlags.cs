using System;

namespace Microsoft.Exchange.Rpc.Cluster
{
	[Flags]
	internal enum SeederRpcFlags
	{
		None = 0,
		SkipSettingReseedAutoReseedState = 1,
		CIAutoReseedReasonBehindBacklog = 2,
		CIAutoReseedReasonBehindRetry = 4,
		CIAutoReseedReasonFailedAndSuspended = 8,
		CIAutoReseedReasonUpgrade = 16,
		CatalogCorruptionWhenFeedingStarts = 32,
		CatalogCorruptionWhenFeedingCompletes = 64,
		EventsMissingWithNotificationsWatermark = 128,
		CrawlOnNonPreferredActiveWithNotificationsWatermark = 256,
		CrawlOnNonPreferredActiveWithTooManyNotificationEvents = 512,
		CrawlOnPassive = 1024,
		Unknown = 2048
	}
}
