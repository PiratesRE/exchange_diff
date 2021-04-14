using System;
using Microsoft.Exchange.Common;

namespace Microsoft.Exchange.Servicelets.JobQueue
{
	internal sealed class AppConfig
	{
		public AppConfig()
		{
			this.TMSyncCacheSlidingExpiry = AppConfigLoader.GetConfigTimeSpanValue("TMSyncCacheSlidingExpiry", TimeSpan.FromSeconds(1.0), TimeSpan.MaxValue, TimeSpan.FromMinutes(15.0));
			this.TMSyncCacheAbsoluteExpiry = AppConfigLoader.GetConfigTimeSpanValue("TMSyncCacheAbsoluteExpiry", TimeSpan.FromSeconds(1.0), TimeSpan.MaxValue, TimeSpan.FromHours(4.0));
			this.TMSyncCacheBucketCount = AppConfigLoader.GetConfigIntValue("TMSyncCacheBucketCount", 1, int.MaxValue, 10);
			this.TMSyncCacheBucketSize = AppConfigLoader.GetConfigIntValue("TMSyncCacheBucketSize", 1, int.MaxValue, 100);
			this.TMSyncMaxJobQueueLength = AppConfigLoader.GetConfigIntValue("TMSyncMaxJobQueueLength", 1, int.MaxValue, 100);
			this.TMSyncMaxPendingJobs = AppConfigLoader.GetConfigIntValue("TMSyncMaxPendingJobs", 1, int.MaxValue, 5);
			this.TMSyncSharePointQueryPageSize = AppConfigLoader.GetConfigIntValue("TMSyncSharePointQueryPageSize", 1, int.MaxValue, 100);
			this.TMSyncDispatcherWakeupInterval = AppConfigLoader.GetConfigTimeSpanValue("TMSyncDispatcherWakeupInterval", TimeSpan.FromMilliseconds(100.0), TimeSpan.MaxValue, TimeSpan.FromSeconds(5.0));
			this.TMSyncMinSyncInterval = AppConfigLoader.GetConfigTimeSpanValue("TMSyncMinSyncInterval", TimeSpan.Zero, TimeSpan.MaxValue, TimeSpan.FromSeconds(30.0));
			this.TMSyncUseOAuth = AppConfigLoader.GetConfigBoolValue("TMSyncUseOAuth", true);
			this.TMSyncHttpDebugEnabled = AppConfigLoader.GetConfigBoolValue("TMSyncHttpDebugEnabled", false);
			this.EnqueueRequestTimeout = AppConfigLoader.GetConfigTimeSpanValue("EnqueueRequestTimeout", TimeSpan.FromSeconds(5.0), TimeSpan.MaxValue, TimeSpan.FromSeconds(300.0));
		}

		public TimeSpan TMSyncCacheAbsoluteExpiry { get; private set; }

		public TimeSpan TMSyncCacheSlidingExpiry { get; private set; }

		public int TMSyncCacheBucketCount { get; private set; }

		public int TMSyncCacheBucketSize { get; private set; }

		public int TMSyncMaxJobQueueLength { get; private set; }

		public int TMSyncMaxPendingJobs { get; private set; }

		public TimeSpan TMSyncDispatcherWakeupInterval { get; private set; }

		public TimeSpan TMSyncMinSyncInterval { get; private set; }

		public int TMSyncSharePointQueryPageSize { get; private set; }

		public bool TMSyncUseOAuth { get; private set; }

		public bool TMSyncHttpDebugEnabled { get; private set; }

		public TimeSpan EnqueueRequestTimeout { get; private set; }
	}
}
