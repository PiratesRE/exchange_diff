using System;
using System.Collections.Concurrent;
using System.ServiceModel.Description;

namespace Microsoft.Exchange.DxStore.Common
{
	public class ManagerClientFactory : IDisposable
	{
		public ManagerClientFactory(InstanceManagerConfig managerConfig, WcfTimeout timeout = null)
		{
			this.FactoryByTarget = new ConcurrentDictionary<string, Tuple<CachedChannelFactory<IDxStoreManager>, DxStoreManagerClient>>();
			this.managerConfig = managerConfig;
			this.DefaultTimeout = timeout;
		}

		public ConcurrentDictionary<string, Tuple<CachedChannelFactory<IDxStoreManager>, DxStoreManagerClient>> FactoryByTarget { get; set; }

		public WcfTimeout DefaultTimeout { get; set; }

		public DxStoreManagerClient LocalClient
		{
			get
			{
				return this.GetClient(null);
			}
		}

		public DxStoreManagerClient GetClient(string target)
		{
			if (string.IsNullOrEmpty(target))
			{
				target = this.managerConfig.Self;
			}
			Tuple<CachedChannelFactory<IDxStoreManager>, DxStoreManagerClient> orAdd = this.FactoryByTarget.GetOrAdd(target, delegate(string server)
			{
				ServiceEndpoint endpoint = this.managerConfig.GetEndpoint(server, false, this.DefaultTimeout);
				CachedChannelFactory<IDxStoreManager> cachedChannelFactory = new CachedChannelFactory<IDxStoreManager>(endpoint);
				DxStoreManagerClient item = new DxStoreManagerClient(cachedChannelFactory, (this.DefaultTimeout != null) ? this.DefaultTimeout.Operation : null);
				return Tuple.Create<CachedChannelFactory<IDxStoreManager>, DxStoreManagerClient>(cachedChannelFactory, item);
			});
			return orAdd.Item2;
		}

		public void Dispose()
		{
			if (!this.isDisposed)
			{
				this.Dispose(true);
				GC.SuppressFinalize(this);
			}
		}

		protected void Dispose(bool disposing)
		{
			lock (this)
			{
				if (!this.isDisposed)
				{
					if (disposing)
					{
						foreach (Tuple<CachedChannelFactory<IDxStoreManager>, DxStoreManagerClient> tuple in this.FactoryByTarget.Values)
						{
							if (tuple != null && tuple.Item1 != null)
							{
								tuple.Item1.Dispose();
							}
						}
					}
					this.isDisposed = true;
				}
			}
		}

		private readonly InstanceManagerConfig managerConfig;

		private bool isDisposed;
	}
}
