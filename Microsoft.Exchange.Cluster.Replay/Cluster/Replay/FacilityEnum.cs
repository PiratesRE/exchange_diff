using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal enum FacilityEnum
	{
		Invalid,
		TcpListener,
		TasksRPCServer,
		SeedManager,
		ActiveManager,
		VSSWriter,
		FailureItemManager,
		ReplicationManager,
		ThirdPartyReplicationManager,
		ActiveManagerRpcServer,
		SupportApi,
		MonitoringComponent,
		ServerLocatorService,
		HealthStateTracker,
		AutoReseedManager,
		DiskReclaimer,
		CopyStatusLookup,
		ADConfigLookup,
		MonitoringWcfService,
		DistributedStoreManager
	}
}
