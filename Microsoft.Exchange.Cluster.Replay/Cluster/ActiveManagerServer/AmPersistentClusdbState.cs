using System;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.Shared;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmPersistentClusdbState : IDisposable
	{
		internal AmPersistentClusdbState(IAmCluster cluster, string keyName)
		{
			using (IDistributedStoreKey clusterKey = DistributedStore.Instance.GetClusterKey(cluster.Handle, null, null, DxStoreKeyAccessMode.Write, false))
			{
				string keyName2 = "ExchangeActiveManager\\" + keyName;
				this.m_regHandle = clusterKey.OpenKey(keyName2, DxStoreKeyAccessMode.CreateIfNotExist, false, null);
			}
		}

		internal T ReadProperty<T>(string propertyName, out bool doesEntryExist)
		{
			return this.m_regHandle.GetValue(propertyName, default(T), out doesEntryExist, null);
		}

		internal void WriteProperty<T>(string propertyName, T propertyValue)
		{
			this.m_regHandle.SetValue(propertyName, propertyValue, false, null);
		}

		internal void DeleteProperty(string propertyName)
		{
			this.m_regHandle.DeleteValue(propertyName, true, null);
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
					if (disposing && this.m_regHandle != null)
					{
						this.m_regHandle.Dispose();
					}
					this.m_fDisposed = true;
				}
			}
		}

		private bool m_fDisposed;

		private IDistributedStoreKey m_regHandle;
	}
}
