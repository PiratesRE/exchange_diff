using System;
using System.Collections.Concurrent;
using System.ServiceModel.Description;

namespace Microsoft.Exchange.DxStore.Common
{
	public class InstanceClientFactory : IDisposable
	{
		public InstanceClientFactory(InstanceGroupConfig groupCfg, WcfTimeout timeout = null)
		{
			this.FactoryByTarget = new ConcurrentDictionary<string, Tuple<CachedChannelFactory<IDxStoreInstance>, DxStoreInstanceClient>>();
			this.groupCfg = groupCfg;
			this.DefaultTimeout = timeout;
		}

		public ConcurrentDictionary<string, Tuple<CachedChannelFactory<IDxStoreInstance>, DxStoreInstanceClient>> FactoryByTarget { get; set; }

		public WcfTimeout DefaultTimeout { get; set; }

		public DxStoreInstanceClient LocalClient
		{
			get
			{
				return this.GetClient(null);
			}
		}

		public DxStoreInstanceClient GetClient(string target)
		{
			if (string.IsNullOrEmpty(target))
			{
				target = this.groupCfg.Self;
			}
			Tuple<CachedChannelFactory<IDxStoreInstance>, DxStoreInstanceClient> orAdd = this.FactoryByTarget.GetOrAdd(target, delegate(string server)
			{
				ServiceEndpoint storeInstanceEndpoint = this.groupCfg.GetStoreInstanceEndpoint(server, false, false, this.DefaultTimeout);
				CachedChannelFactory<IDxStoreInstance> cachedChannelFactory = new CachedChannelFactory<IDxStoreInstance>(storeInstanceEndpoint);
				DxStoreInstanceClient item = new DxStoreInstanceClient(cachedChannelFactory, (this.DefaultTimeout != null) ? this.DefaultTimeout.Operation : null);
				return Tuple.Create<CachedChannelFactory<IDxStoreInstance>, DxStoreInstanceClient>(cachedChannelFactory, item);
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
						foreach (Tuple<CachedChannelFactory<IDxStoreInstance>, DxStoreInstanceClient> tuple in this.FactoryByTarget.Values)
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

		private readonly InstanceGroupConfig groupCfg;

		private bool isDisposed;
	}
}
