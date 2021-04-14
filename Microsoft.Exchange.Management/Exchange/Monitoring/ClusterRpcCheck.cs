using System;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	internal class ClusterRpcCheck : ReplicationCheck
	{
		public ClusterRpcCheck(string serverName, IEventManager eventManager, string momeventsource) : base("ClusterService", CheckId.ClusterService, Strings.ClusterRpcCheckDesc, CheckCategory.SystemHighPriority, eventManager, momeventsource, serverName)
		{
		}

		public ClusterRpcCheck(string serverName, IEventManager eventManager, string momeventsource, uint? ignoreTransientErrorsThreshold) : base("ClusterService", CheckId.ClusterService, Strings.ClusterRpcCheckDesc, CheckCategory.SystemHighPriority, eventManager, momeventsource, serverName, ignoreTransientErrorsThreshold)
		{
		}

		protected override void InternalRun()
		{
			AmServerName serverName = new AmServerName(base.ServerName);
			if (DagHelper.IsNodeClustered(serverName))
			{
				try
				{
					this.m_Cluster = AmCluster.OpenByName(serverName);
					if (this.m_Cluster == null)
					{
						base.Fail(Strings.CouldNotConnectToCluster(base.ServerName));
					}
					return;
				}
				catch (ClusterException ex)
				{
					base.Fail(Strings.CouldNotConnectToClusterError(base.ServerName, ex.Message));
					return;
				}
			}
			ExTraceGlobals.HealthChecksTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Server {1} is not clustered! Skipping check {0}.", base.Title, base.ServerName);
			base.Skip();
		}

		public AmCluster GetClusterHandle()
		{
			return this.m_Cluster;
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			base.InternalDispose(calledFromDispose);
			if (this.m_Cluster != null)
			{
				this.m_Cluster.Dispose();
			}
		}

		private AmCluster m_Cluster;
	}
}
