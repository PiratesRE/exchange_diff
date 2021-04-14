using System;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmDagConfig
	{
		internal AmDagConfig(ADObjectId id, AmServerName[] memberServers, AmServerName currentPAM, IAmCluster cluster, bool tprEnabled)
		{
			this.Id = id;
			this.MemberServers = memberServers;
			this.CurrentPAM = currentPAM;
			this.Cluster = cluster;
			this.IsThirdPartyReplEnabled = tprEnabled;
		}

		internal bool IsThirdPartyReplEnabled { get; private set; }

		internal ADObjectId Id { get; set; }

		internal AmServerName[] MemberServers { get; set; }

		internal AmServerName CurrentPAM { get; set; }

		internal IAmCluster Cluster { get; set; }

		internal AmNodeState GetNodeState(AmServerName nodeName)
		{
			AmNodeState result = AmNodeState.Unknown;
			try
			{
				IAmClusterNode amClusterNode2;
				IAmClusterNode amClusterNode = amClusterNode2 = this.Cluster.OpenNode(nodeName);
				try
				{
					result = amClusterNode.State;
				}
				finally
				{
					if (amClusterNode2 != null)
					{
						amClusterNode2.Dispose();
					}
				}
			}
			catch (ClusterException ex)
			{
				AmTrace.Error("Failed to open cluster node {0} (error={1})", new object[]
				{
					nodeName,
					ex.Message
				});
			}
			return result;
		}

		internal bool IsNodePubliclyUp(AmServerName nodeName)
		{
			return AmSystemManager.Instance.NetworkMonitor == null || AmSystemManager.Instance.NetworkMonitor.IsNodePubliclyUp(nodeName);
		}
	}
}
