using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmClusterMonitor
	{
		public static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.ClusterEventsTracer;
			}
		}

		internal IAmCluster Cluster { get; private set; }

		public void ReportHasADAccess(bool hasAccess)
		{
			bool flag = false;
			AmClusterNodeNetworkStatus amClusterNodeNetworkStatus = new AmClusterNodeNetworkStatus();
			amClusterNodeNetworkStatus.HasADAccess = hasAccess;
			if (this.Cluster == null || this.m_cem == null)
			{
				AmClusterMonitor.Tracer.TraceError(0L, "ReportHasADAccess fails because we aren't initialized or running in a DAG");
				return;
			}
			try
			{
				using (AmClusterNodeStatusAccessor amClusterNodeStatusAccessor = new AmClusterNodeStatusAccessor(this.Cluster, AmServerName.LocalComputerName, DxStoreKeyAccessMode.CreateIfNotExist))
				{
					AmClusterNodeNetworkStatus amClusterNodeNetworkStatus2 = amClusterNodeStatusAccessor.Read();
					if (amClusterNodeNetworkStatus2 != null)
					{
						if (amClusterNodeNetworkStatus2.ClusterErrorOverride && amClusterNodeNetworkStatus.HasADAccess && !AmSystemManager.Instance.NetworkMonitor.AreAnyMapiNicsUp(AmServerName.LocalComputerName))
						{
							amClusterNodeNetworkStatus.ClusterErrorOverride = true;
						}
						if (!amClusterNodeNetworkStatus.IsEqual(amClusterNodeNetworkStatus2))
						{
							flag = true;
						}
					}
					else
					{
						flag = true;
					}
					if (flag)
					{
						amClusterNodeStatusAccessor.Write(amClusterNodeNetworkStatus);
					}
				}
				if (flag)
				{
					if (amClusterNodeNetworkStatus.IsHealthy)
					{
						if (amClusterNodeNetworkStatus.ClusterErrorOverride)
						{
							ReplayCrimsonEvents.AmMapiAccessExpectedByAD.Log();
						}
						else
						{
							ReplayCrimsonEvents.AmADStatusRecordedAsAccessible.Log();
						}
					}
					else
					{
						ReplayCrimsonEvents.AmADStatusRecordedAsFailed.Log();
					}
				}
			}
			catch (ClusterException ex)
			{
				AmClusterMonitor.Tracer.TraceError<ClusterException>(0L, "ReportNodeState failed: {0}", ex);
				ReplayCrimsonEvents.AmNodeStatusUpdateFailed.Log<AmClusterNodeNetworkStatus, string>(amClusterNodeNetworkStatus, ex.Message);
			}
			catch (SerializationException ex2)
			{
				AmClusterMonitor.Tracer.TraceError<SerializationException>(0L, "ReportNodeState failed: {0}", ex2);
				ReplayCrimsonEvents.AmNodeStatusUpdateFailed.Log<AmClusterNodeNetworkStatus, string>(amClusterNodeNetworkStatus, ex2.ToString());
			}
		}

		public void Start(IAmCluster cluster)
		{
			if (this.m_cem == null || !this.m_cem.IsMonitoring || !object.ReferenceEquals(this.Cluster, cluster))
			{
				AmClusterMonitor.Tracer.TraceDebug(0L, "Starting AmClusterEventManager");
				this.Cluster = cluster;
				AmSystemManager.Instance.NetworkMonitor.UseCluster(this.Cluster);
				AmSystemManager.Instance.NetworkMonitor.RefreshMapiNetwork();
				if (this.m_cem != null)
				{
					this.m_cem.Stop();
				}
				this.m_cem = new AmClusterEventManager(this.Cluster);
				this.m_cem.Start();
			}
		}

		public void Stop()
		{
			if (this.m_cem != null)
			{
				AmClusterMonitor.Tracer.TraceDebug(0L, "Stopping AmClusterEventManager");
				this.m_cem.Stop();
				this.m_cem = null;
				this.Cluster = null;
				if (AmSystemManager.Instance.NetworkMonitor != null)
				{
					AmSystemManager.Instance.NetworkMonitor.UseCluster(this.Cluster);
					return;
				}
			}
			else
			{
				AmClusterMonitor.Tracer.TraceDebug(0L, "Ignoring cluster event manager stop since it is not active");
			}
		}

		private AmClusterEventManager m_cem;
	}
}
