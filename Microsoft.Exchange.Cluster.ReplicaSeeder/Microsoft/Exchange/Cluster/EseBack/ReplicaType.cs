using System;

namespace Microsoft.Exchange.Cluster.EseBack
{
	public enum ReplicaType : uint
	{
		Unknown,
		StandbyReplica,
		ClusterReplica,
		LocalReplica = 4U
	}
}
