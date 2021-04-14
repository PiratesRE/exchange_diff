using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ServiceModel;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	internal sealed class ServiceProxyPoolsManager<TWebservice> : IDisposable
	{
		private ServiceProxyPoolsManager()
		{
		}

		~ServiceProxyPoolsManager()
		{
			this.Dispose(false);
		}

		public static ServiceProxyPoolsManager<TWebservice> Instance
		{
			get
			{
				return ServiceProxyPoolsManager<TWebservice>.instance;
			}
		}

		public ServiceProxyPool<TWebservice> GetOrCreateProxyPool(ServiceProxyIdentity identity, ExecutionLog logProvider)
		{
			ArgumentValidator.ThrowIfNull("identity", identity);
			ArgumentValidator.ThrowIfNull("logProvider", logProvider);
			return ServiceProxyPoolsManager<TWebservice>.proxyPools.GetOrAddSafe(identity, delegate(ServiceProxyIdentity proxyPool)
			{
				string name = typeof(TWebservice).Name;
				string hostName = proxyPool.ToString();
				return new ServiceProxyPool<TWebservice>(name, hostName, 5U, this.CreateChannelFactoryInstance(identity), logProvider);
			});
		}

		public void Dispose()
		{
			if (!this.disposed)
			{
				this.Dispose(true);
			}
		}

		private ChannelFactory<TWebservice> CreateChannelFactoryInstance(ServiceProxyIdentity identity)
		{
			ChannelFactory<TWebservice> channelFactory = new ChannelFactory<TWebservice>(new WSHttpBinding(SecurityMode.Transport)
			{
				MaxReceivedMessageSize = 26214400L,
				Security = 
				{
					Transport = 
					{
						ClientCredentialType = HttpClientCredentialType.Certificate
					}
				}
			}, identity.EndpointAddress);
			if (channelFactory.Credentials != null)
			{
				channelFactory.Credentials.ClientCertificate.Certificate = identity.Certificate;
			}
			return channelFactory;
		}

		private void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				foreach (KeyValuePair<ServiceProxyIdentity, Lazy<ServiceProxyPool<TWebservice>>> keyValuePair in ServiceProxyPoolsManager<TWebservice>.proxyPools)
				{
					keyValuePair.Value.Value.Dispose();
				}
				this.disposed = true;
				if (disposing)
				{
					GC.SuppressFinalize(this);
				}
			}
		}

		private const uint DefaultMaxNumberOfProxiesInPool = 5U;

		private static ConcurrentDictionary<ServiceProxyIdentity, Lazy<ServiceProxyPool<TWebservice>>> proxyPools = new ConcurrentDictionary<ServiceProxyIdentity, Lazy<ServiceProxyPool<TWebservice>>>();

		private static ServiceProxyPoolsManager<TWebservice> instance = new ServiceProxyPoolsManager<TWebservice>();

		private bool disposed;
	}
}
