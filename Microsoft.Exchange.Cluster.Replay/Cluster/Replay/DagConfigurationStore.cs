using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.Shared;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class DagConfigurationStore : IDisposable
	{
		public static bool ClusterEventKeyNotificationMatchesNetworkConfig(string changedObjectName)
		{
			int num = string.Compare(changedObjectName, "Exchange\\DagNetwork", StringComparison.OrdinalIgnoreCase);
			return num == 0;
		}

		private AmClusterHandle ClusterHandle
		{
			get
			{
				return this.m_hCluster;
			}
		}

		public void Open(string targetServer)
		{
			this.m_hCluster = ClusapiMethods.OpenCluster(targetServer);
			if (this.m_hCluster == null || this.m_hCluster.IsInvalid)
			{
				Marshal.GetLastWin32Error();
				throw new Win32Exception();
			}
			using (IDistributedStoreKey clusterKey = DistributedStore.Instance.GetClusterKey(this.ClusterHandle, null, targetServer, DxStoreKeyAccessMode.Write, false))
			{
				this.m_regHandle = clusterKey.OpenKey("Exchange\\DagNetwork", DxStoreKeyAccessMode.CreateIfNotExist, false, null);
			}
		}

		public void Open()
		{
			this.Open(null);
		}

		public PersistentDagNetworkConfig LoadNetworkConfig(out string xmlText)
		{
			bool flag = false;
			xmlText = this.m_regHandle.GetValue("Configuration", null, out flag, null);
			if (flag)
			{
				return PersistentDagNetworkConfig.Deserialize(xmlText);
			}
			return null;
		}

		public PersistentDagNetworkConfig LoadNetworkConfig()
		{
			string text = null;
			return this.LoadNetworkConfig(out text);
		}

		public string StoreNetworkConfig(PersistentDagNetworkConfig cfg)
		{
			string text = cfg.Serialize();
			this.StoreNetworkConfig(text);
			return text;
		}

		public void StoreNetworkConfig(string serializedConfig)
		{
			this.m_regHandle.SetValue("Configuration", serializedConfig, false, null);
		}

		protected void CloseHandles()
		{
			if (this.m_regHandle != null)
			{
				this.m_regHandle.Dispose();
				this.m_regHandle = null;
			}
			if (this.m_hCluster != null && !this.m_hCluster.IsInvalid)
			{
				this.m_hCluster.Dispose();
				this.m_hCluster = null;
			}
		}

		public void Dispose()
		{
			if (!this.m_fDisposed)
			{
				this.Dispose(true);
				GC.SuppressFinalize(this);
			}
		}

		protected void Dispose(bool disposing)
		{
			lock (this)
			{
				if (!this.m_fDisposed)
				{
					if (disposing)
					{
						this.CloseHandles();
					}
					this.m_fDisposed = true;
				}
			}
		}

		internal const string RootKeyName = "Exchange\\DagNetwork";

		private const string NetConfigValueName = "Configuration";

		private bool m_fDisposed;

		private IDistributedStoreKey m_regHandle;

		private AmClusterHandle m_hCluster;
	}
}
