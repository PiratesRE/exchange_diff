using System;
using System.Collections.Concurrent;
using System.ServiceModel.Description;

namespace Microsoft.Exchange.DxStore.Common
{
	public class AccessClientFactory
	{
		public AccessClientFactory(InstanceGroupConfig groupCfg, WcfTimeout timeout = null)
		{
			this.FactoryByTarget = new ConcurrentDictionary<string, Tuple<CachedChannelFactory<IDxStoreAccess>, IDxStoreAccessClient>>();
			this.groupCfg = groupCfg;
			this.DefaultTimeout = timeout;
			if (groupCfg.Settings.IsUseHttpTransportForClientCommunication)
			{
				HttpConfiguration.Configure(this.groupCfg);
			}
		}

		public ConcurrentDictionary<string, Tuple<CachedChannelFactory<IDxStoreAccess>, IDxStoreAccessClient>> FactoryByTarget { get; set; }

		public WcfTimeout DefaultTimeout { get; set; }

		public IDxStoreAccessClient LocalClient
		{
			get
			{
				return this.GetClient(null);
			}
		}

		public IDxStoreAccessClient GetClient(string target)
		{
			if (string.IsNullOrEmpty(target))
			{
				target = this.groupCfg.Self;
			}
			Tuple<CachedChannelFactory<IDxStoreAccess>, IDxStoreAccessClient> orAdd = this.FactoryByTarget.GetOrAdd(target, delegate(string server)
			{
				CachedChannelFactory<IDxStoreAccess> cachedChannelFactory = null;
				IDxStoreAccessClient item;
				if (this.groupCfg.Settings.IsUseHttpTransportForClientCommunication)
				{
					item = new HttpStoreAccessClient(this.groupCfg.Self, HttpClient.TargetInfo.BuildFromNode(server, this.groupCfg), this.groupCfg.Settings.StoreAccessHttpTimeoutInMSec);
				}
				else
				{
					ServiceEndpoint storeAccessEndpoint = this.groupCfg.GetStoreAccessEndpoint(server, false, false, this.DefaultTimeout);
					cachedChannelFactory = new CachedChannelFactory<IDxStoreAccess>(storeAccessEndpoint);
					item = new WcfStoreAccessClient(cachedChannelFactory, (this.DefaultTimeout != null) ? this.DefaultTimeout.Operation : null);
				}
				return Tuple.Create<CachedChannelFactory<IDxStoreAccess>, IDxStoreAccessClient>(cachedChannelFactory, item);
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
						foreach (Tuple<CachedChannelFactory<IDxStoreAccess>, IDxStoreAccessClient> tuple in this.FactoryByTarget.Values)
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
