using System;

namespace Microsoft.Exchange.InfoWorker.Common.Search
{
	internal enum MailboxState
	{
		NotStarted,
		NormalCrawlInProgress,
		Done = 4,
		DeletionPending,
		InTransit,
		Failed = 100
	}
}
