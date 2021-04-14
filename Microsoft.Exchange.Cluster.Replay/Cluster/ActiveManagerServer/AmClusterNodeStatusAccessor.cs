using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmClusterNodeStatusAccessor : IDisposable
	{
		public AmClusterNodeStatusAccessor(IAmCluster cluster, AmServerName nodeName, DxStoreKeyAccessMode mode = DxStoreKeyAccessMode.Read)
		{
			this.ServerName = nodeName;
			string keyName = string.Format("{0}\\{1}", "ExchangeActiveManager\\NodeState", nodeName.NetbiosName);
			using (IDistributedStoreKey clusterKey = DistributedStore.Instance.GetClusterKey(cluster.Handle, null, null, mode, false))
			{
				this.distributedStoreKey = clusterKey.OpenKey(keyName, mode, false, null);
			}
		}

		public static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.ClusterEventsTracer;
			}
		}

		public AmServerName ServerName { get; private set; }

		public static AmClusterNodeNetworkStatus Read(IAmCluster cluster, AmServerName srvName, out Exception ex)
		{
			ex = null;
			AmClusterNodeNetworkStatus amClusterNodeNetworkStatus = null;
			try
			{
				using (AmClusterNodeStatusAccessor amClusterNodeStatusAccessor = new AmClusterNodeStatusAccessor(cluster, srvName, DxStoreKeyAccessMode.Read))
				{
					amClusterNodeNetworkStatus = amClusterNodeStatusAccessor.Read();
				}
			}
			catch (SerializationException ex2)
			{
				ex = ex2;
			}
			catch (ClusterException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				AmClusterNodeStatusAccessor.Tracer.TraceError<AmServerName, Exception>(0L, "AmClusterNodeNetworkStatus.Read({0}) failed: {1}", srvName, ex);
			}
			else if (amClusterNodeNetworkStatus == null)
			{
				AmClusterNodeStatusAccessor.Tracer.TraceError<AmServerName>(0L, "AmClusterNodeNetworkStatus.Read({0}) No status has yet been published", srvName);
			}
			return amClusterNodeNetworkStatus;
		}

		public static Exception Write(IAmCluster cluster, AmServerName srvName, AmClusterNodeNetworkStatus status)
		{
			Exception ex = null;
			try
			{
				using (AmClusterNodeStatusAccessor amClusterNodeStatusAccessor = new AmClusterNodeStatusAccessor(cluster, srvName, DxStoreKeyAccessMode.CreateIfNotExist))
				{
					amClusterNodeStatusAccessor.Write(status);
				}
			}
			catch (SerializationException ex2)
			{
				ex = ex2;
			}
			catch (ClusterException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				AmClusterNodeStatusAccessor.Tracer.TraceError<AmServerName, AmClusterNodeNetworkStatus, Exception>(0L, "AmClusterNodeNetworkStatus.Write({0},{1}) failed: {2}", srvName, status, ex);
			}
			return ex;
		}

		public AmClusterNodeNetworkStatus Read()
		{
			string value = this.distributedStoreKey.GetValue("NetworkStatus", null, null);
			if (!string.IsNullOrEmpty(value))
			{
				return AmClusterNodeNetworkStatus.Deserialize(value);
			}
			return null;
		}

		public void Write(AmClusterNodeNetworkStatus state)
		{
			state.LastUpdate = DateTime.UtcNow;
			string propertyValue = state.Serialize();
			this.distributedStoreKey.SetValue("NetworkStatus", propertyValue, false, null);
			AmClusterNodeStatusAccessor.Tracer.TraceDebug<AmServerName, AmClusterNodeNetworkStatus>(0L, "AmClusterNodeNetworkStatus.Write({0}):{1}", this.ServerName, state);
		}

		public void Dispose()
		{
			if (!this.m_fDisposed)
			{
				this.Dispose(true);
				GC.SuppressFinalize(this);
			}
		}

		public void Dispose(bool disposing)
		{
			lock (this)
			{
				if (!this.m_fDisposed)
				{
					if (disposing)
					{
						this.distributedStoreKey.Dispose();
					}
					this.m_fDisposed = true;
				}
			}
		}

		private const string RootKeyName = "ExchangeActiveManager\\NodeState";

		private const string ValueName = "NetworkStatus";

		private IDistributedStoreKey distributedStoreKey;

		private bool m_fDisposed;
	}
}
