using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net.JobQueues;

namespace Microsoft.Exchange.Data.Storage.LinkedFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class TeamMailboxSyncConfiguration : Configuration
	{
		public TimeSpan CacheAbsoluteExpiry { get; private set; }

		public TimeSpan CacheSlidingExpiry { get; private set; }

		public int CacheBucketCount { get; private set; }

		public int CacheBucketSize { get; private set; }

		public int SharePointQueryPageSize { get; private set; }

		public TimeSpan MinSyncInterval { get; private set; }

		public bool UseOAuth { get; private set; }

		public bool HttpDebugEnabled { get; private set; }

		public TeamMailboxSyncConfiguration(TimeSpan cacheAbsoluteExpiry, TimeSpan cacheSlidingExpiry, int cacheBucketCount, int cacheBucketSize, int maxAllowedQueueLength, int maxAllowedPendingJobCount, TimeSpan dispatcherWakeUpInterval, TimeSpan minSyncInterval, int sharePointQueryPageSize, bool useOAuth, bool httpDebugEnabled) : base(maxAllowedQueueLength, maxAllowedPendingJobCount, dispatcherWakeUpInterval)
		{
			this.CacheAbsoluteExpiry = cacheAbsoluteExpiry;
			this.CacheSlidingExpiry = cacheSlidingExpiry;
			this.CacheBucketSize = cacheBucketSize;
			this.CacheBucketCount = cacheBucketCount;
			this.MinSyncInterval = minSyncInterval;
			this.SharePointQueryPageSize = sharePointQueryPageSize;
			this.UseOAuth = useOAuth;
			this.HttpDebugEnabled = httpDebugEnabled;
		}
	}
}
