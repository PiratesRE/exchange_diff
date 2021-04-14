using System;

namespace Microsoft.Exchange.Monitoring
{
	public enum CheckId
	{
		Undefined,
		ClusterService = 1000,
		ActiveManager,
		DnsRegistrationStatus,
		ReplayService,
		DagMembersUp,
		NodePaused,
		ClusterNetwork,
		FileShareQuorum,
		QuorumGroup,
		DatabasesFailed,
		DatabasesSuspended,
		DatabasesInitializing,
		RcrDatabasesMounted,
		DatabasesCopyKeepingUp,
		DatabasesReplayKeepingUp,
		TasksRpcListener,
		TcpListener,
		ThirdPartyReplication,
		DatabasesDisconnected,
		DatabasesRedundancy,
		DatabasesAvailability,
		ServerLocatorService,
		MonitoringService
	}
}
