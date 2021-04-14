using System;
using System.ServiceModel;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ExchangeTopology;
using Microsoft.Exchange.Data.Directory.TopologyDiscovery;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ExchangeTopologyBridge : IExchangeTopologyBridge
	{
		public TimeSpan CacheTimerRefreshTimeout
		{
			get
			{
				return ExchangeTopologyBridge.cacheTimerRefreshTimeout;
			}
		}

		public TimeSpan CacheExpirationTimeout
		{
			get
			{
				return ExchangeTopologyBridge.cacheExpirationTimeout;
			}
		}

		public TimeSpan GetServiceTopologyDefaultTimeout
		{
			get
			{
				return ExchangeTopologyBridge.getServiceTopologyDefaultTimeout;
			}
		}

		public TimeSpan NotificationDelayTimeout
		{
			get
			{
				return ExchangeTopologyBridge.notificationDelayTimeout;
			}
		}

		public TimeSpan MinExpirationTimeForCacheDueToCacheMiss
		{
			get
			{
				return ExchangeTopologyBridge.minExpirationTimeForCacheDueToCacheMiss;
			}
		}

		public ExchangeTopology ReadExchangeTopology(DateTime timestamp, ExchangeTopologyScope topologyScope, bool forceRefresh)
		{
			ExchangeTopology result;
			try
			{
				using (TopologyServiceClient topologyServiceClient = TopologyServiceClient.CreateClient("localhost"))
				{
					byte[][] exchangeTopology = topologyServiceClient.GetExchangeTopology(timestamp, topologyScope, forceRefresh);
					if (exchangeTopology == null)
					{
						result = null;
					}
					else
					{
						ExchangeTopologyDiscovery.Simple topology = ExchangeTopologyDiscovery.Simple.Deserialize(exchangeTopology);
						ExchangeTopologyDiscovery topologyDiscovery = ExchangeTopologyDiscovery.Simple.CreateFrom(topology);
						result = ExchangeTopologyDiscovery.Populate(topologyDiscovery);
					}
				}
			}
			catch (DataSourceOperationException innerException)
			{
				ServiceDiscoveryPermanentException ex = new ServiceDiscoveryPermanentException(ServerStrings.ExReadExchangeTopologyFailed, innerException);
				ExTraceGlobals.ServiceDiscoveryTracer.TraceError<ServiceDiscoveryPermanentException>(0L, "ExchangeTopologyBridge::ReadExchangeTopology. Read ExchangeTopology failed. Throwing exception: {0}.", ex);
				throw ex;
			}
			catch (Exception ex2)
			{
				if (!(ex2 is CommunicationException) && !(ex2 is TimeoutException) && !(ex2 is InvalidOperationException))
				{
					throw;
				}
				ServiceDiscoveryTransientException ex3 = new ServiceDiscoveryTransientException(ServerStrings.ExReadExchangeTopologyFailed, ex2);
				ExTraceGlobals.ServiceDiscoveryTracer.TraceError<ServiceDiscoveryTransientException>(0L, "ExchangeTopologyBridge::ReadExchangeTopology. Read ExchangeTopology failed. Throwing exception: {0}.", ex3);
				throw ex3;
			}
			return result;
		}

		public IRegisteredExchangeTopologyNotification RegisterExchangeTopologyNotification(ADNotificationCallback callback, ExchangeTopologyScope scope)
		{
			return new RegisteredExchangeTopologyNotification(callback, scope);
		}

		private static readonly TimeSpan cacheTimerRefreshTimeout = new TimeSpan(0, 15, 0);

		private static readonly TimeSpan cacheExpirationTimeout = new TimeSpan(24, 0, 0);

		private static readonly TimeSpan getServiceTopologyDefaultTimeout = new TimeSpan(0, 1, 0);

		private static readonly TimeSpan notificationDelayTimeout = new TimeSpan(0, 5, 0);

		private static readonly TimeSpan minExpirationTimeForCacheDueToCacheMiss = new TimeSpan(0, 5, 0);
	}
}
