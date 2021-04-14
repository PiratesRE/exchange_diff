using System;

namespace Microsoft.Exchange.Search.Core.Abstraction
{
	internal enum IndexStatusIndex
	{
		IndexingState,
		ErrorCode,
		Version,
		TimeStamp,
		MailboxesToCrawl,
		SeedingSource,
		AgeOfLastNotificationProcessed,
		RetriableItemsCount,
		Count
	}
}
