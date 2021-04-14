using System;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	[Flags]
	public enum EnableSubComponentFlag
	{
		None = 0,
		TransientFailoverSuppressor = 1,
		ServiceKillStatusContainer = 2,
		ServerNameCacheManager = 4,
		DbNodeAttemptTable = 8,
		SystemEventQueue = 16,
		DatabaseQueueManager = 32,
		StoreStateMarker = 64,
		PeriodicEventManager = 128,
		ClusterMonitor = 256,
		NetworkMonitor = 512,
		AmPerfCounterUpdater = 1024,
		PamCachedLastLogUpdater = 2048,
		ClusdbPeriodicCleanup = 4096,
		DataStorePeriodicChecker = 8192,
		All = 16383
	}
}
