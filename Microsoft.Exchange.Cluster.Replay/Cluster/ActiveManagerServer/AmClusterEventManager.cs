using System;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmClusterEventManager : ChangePoller
	{
		internal AmClusterEventManager(IAmCluster cluster) : base(true)
		{
			this.Cluster = cluster;
		}

		public static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.ClusterEventsTracer;
			}
		}

		internal bool IsMonitoring
		{
			get
			{
				bool isMonitoring;
				lock (this.m_locker)
				{
					isMonitoring = this.m_isMonitoring;
				}
				return isMonitoring;
			}
		}

		private IAmCluster Cluster { get; set; }

		public override void PrepareToStop()
		{
			AmClusterEventManager.Tracer.TraceDebug((long)this.GetHashCode(), "Stopping Cluster event manager");
			base.PrepareToStop();
			lock (this.m_locker)
			{
				if (this.m_cen != null)
				{
					this.m_cen.ForceClose();
				}
			}
		}

		protected override void PollerThread()
		{
			AmTrace.Entering("ClusterEventManager.PollerThread", new object[0]);
			try
			{
				lock (this.m_locker)
				{
					if (!this.m_fShutdown)
					{
						this.CleanupClusterNotify();
						this.m_isMonitoring = this.InitializeClusterNotify();
						if (!this.m_isMonitoring)
						{
							AmTrace.Error("Failed to initialize cluster notification", new object[0]);
						}
					}
				}
				if (!this.m_fShutdown && this.m_isMonitoring)
				{
					this.MonitorClusterEvents();
				}
			}
			catch (ClusterException ex)
			{
				AmTrace.Error("MonitorForClusterEvents() got a cluster api exception: {0}", new object[]
				{
					ex
				});
			}
			catch (AmServerNameResolveFqdnException ex2)
			{
				AmTrace.Error("MonitorForClusterEvents() got a AmGetFqdnFailedADErrorException exception: {0}", new object[]
				{
					ex2
				});
			}
			lock (this.m_locker)
			{
				this.CleanupClusterNotify();
				this.m_isMonitoring = false;
			}
			if (!this.m_fShutdown)
			{
				AmClusterEventManager.Tracer.TraceDebug(0L, "Triggering refresh for error recovery");
				AmSystemManager.Instance.ConfigManager.TriggerRefresh(true);
			}
			AmClusterEventManager.Tracer.TraceDebug(0L, "Leaving ClusterEventManager.PollerThread");
		}

		private void MonitorClusterEvents()
		{
			while (!this.m_fShutdown)
			{
				AmClusterEventInfo amClusterEventInfo;
				bool flag = this.m_cen.WaitForEvent(out amClusterEventInfo, this.DefaultClusterEventNotifierTimeout);
				if (this.m_fShutdown)
				{
					AmClusterEventManager.Tracer.TraceDebug((long)this.GetHashCode(), "MonitorClusterEvents: Detected shutdown flag is set. Exiting from cluster notification monitoring");
					return;
				}
				if (flag)
				{
					if (amClusterEventInfo.IsNotifyHandleClosed)
					{
						AmClusterEventManager.Tracer.TraceDebug((long)this.GetHashCode(), "Cluster notification handle closed. Exiting cluster event monitoring");
						return;
					}
					if (amClusterEventInfo.IsNodeStateChanged)
					{
						AmServerName nodeName = new AmServerName(amClusterEventInfo.ObjectName);
						if (!string.IsNullOrEmpty(amClusterEventInfo.ObjectName))
						{
							Exception ex;
							AmNodeState nodeState = this.Cluster.GetNodeState(nodeName, out ex);
							if (ex != null)
							{
								AmClusterEventManager.Tracer.TraceError<Exception>(0L, "MonitorClusterEvents fails to get node state: {0}", ex);
							}
							AmEvtNodeStateChanged amEvtNodeStateChanged = new AmEvtNodeStateChanged(nodeName, nodeState);
							amEvtNodeStateChanged.Notify();
							if (ex != null)
							{
								throw ex;
							}
						}
						else
						{
							AmTrace.Error("Node state change detected but node name is invalid", new object[0]);
						}
					}
					if (amClusterEventInfo.IsGroupStateChanged)
					{
						AmSystemManager.Instance.ConfigManager.TriggerRefresh(false);
					}
					if (amClusterEventInfo.IsClusterStateChanged)
					{
						AmEvtClusterStateChanged amEvtClusterStateChanged = new AmEvtClusterStateChanged();
						amEvtClusterStateChanged.Notify();
						AmSystemManager.Instance.ConfigManager.TriggerRefresh(true);
						return;
					}
					if (amClusterEventInfo.IsNodeAdded)
					{
						AmEvtNodeAdded amEvtNodeAdded = new AmEvtNodeAdded(new AmServerName(amClusterEventInfo.ObjectName));
						amEvtNodeAdded.Notify();
					}
					if (amClusterEventInfo.IsNodeRemoved)
					{
						AmEvtNodeRemoved amEvtNodeRemoved = new AmEvtNodeRemoved(new AmServerName(amClusterEventInfo.ObjectName));
						amEvtNodeRemoved.Notify();
						AmServerNameCache.Instance.RemoveEntry(amClusterEventInfo.ObjectName);
					}
					AmSystemManager.Instance.NetworkMonitor.ProcessEvent(amClusterEventInfo);
				}
			}
		}

		private bool InitializeClusterNotify()
		{
			ClusterNotifyFlags eventMask = ~(ClusterNotifyFlags.CLUSTER_CHANGE_NODE_PROPERTY | ClusterNotifyFlags.CLUSTER_CHANGE_REGISTRY_NAME | ClusterNotifyFlags.CLUSTER_CHANGE_REGISTRY_ATTRIBUTES | ClusterNotifyFlags.CLUSTER_CHANGE_REGISTRY_VALUE | ClusterNotifyFlags.CLUSTER_CHANGE_REGISTRY_SUBTREE | ClusterNotifyFlags.CLUSTER_CHANGE_RESOURCE_STATE | ClusterNotifyFlags.CLUSTER_CHANGE_RESOURCE_DELETED | ClusterNotifyFlags.CLUSTER_CHANGE_RESOURCE_ADDED | ClusterNotifyFlags.CLUSTER_CHANGE_RESOURCE_PROPERTY | ClusterNotifyFlags.CLUSTER_CHANGE_GROUP_DELETED | ClusterNotifyFlags.CLUSTER_CHANGE_GROUP_ADDED | ClusterNotifyFlags.CLUSTER_CHANGE_GROUP_PROPERTY | ClusterNotifyFlags.CLUSTER_CHANGE_RESOURCE_TYPE_DELETED | ClusterNotifyFlags.CLUSTER_CHANGE_RESOURCE_TYPE_ADDED | ClusterNotifyFlags.CLUSTER_CHANGE_RESOURCE_TYPE_PROPERTY | ClusterNotifyFlags.CLUSTER_CHANGE_CLUSTER_RECONNECT | ClusterNotifyFlags.CLUSTER_CHANGE_QUORUM_STATE | ClusterNotifyFlags.CLUSTER_CHANGE_CLUSTER_PROPERTY);
			this.m_cen = new AmClusterNotify(this.Cluster.Handle);
			this.m_cen.Initialize(eventMask, (IntPtr)1L);
			return true;
		}

		private void CleanupClusterNotify()
		{
			if (this.m_cen != null)
			{
				this.m_cen.Dispose();
				this.m_cen = null;
			}
		}

		private readonly TimeSpan DefaultClusterEventNotifierTimeout = new TimeSpan(0, 0, 30);

		private AmClusterNotify m_cen;

		private bool m_isMonitoring;

		private object m_locker = new object();
	}
}
