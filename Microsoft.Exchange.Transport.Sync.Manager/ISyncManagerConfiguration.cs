using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Manager
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface ISyncManagerConfiguration
	{
		TimeSpan WorkTypeBudgetManagerSlidingWindowLength { get; }

		TimeSpan WorkTypeBudgetManagerSlidingBucketLength { get; }

		TimeSpan WorkTypeBudgetManagerSampleDispatchedWorkFrequency { get; }

		TimeSpan DatabaseBackoffTime { get; }

		int MaxSyncsPerDB { get; }

		TimeSpan DispatchEntryExpirationCheckFrequency { get; }

		TimeSpan DispatchEntryExpirationTime { get; }

		TimeSpan DispatcherDatabaseRefreshFrequency { get; }
	}
}
