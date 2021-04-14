using System;
using Microsoft.Exchange.Data.Directory.ExchangeTopology;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class ServiceDiscovery
	{
		public static ExchangeTopologyScope ADNotificationScope
		{
			get
			{
				return ServiceCache.Scope;
			}
			set
			{
				ServiceCache.Scope = value;
			}
		}

		internal static IExchangeTopologyBridge ExchangeTopologyBridge
		{
			get
			{
				return ServiceDiscovery.topologyBridge;
			}
		}

		internal static void PurgeServiceCache()
		{
			ServiceCache.Purge();
		}

		internal static void SetTestExchangeTopologyBridge(IExchangeTopologyBridge testExchangeTopologyBridge)
		{
			ServiceDiscovery.topologyBridge = (testExchangeTopologyBridge ?? new ExchangeTopologyBridge());
			ServiceDiscovery.PurgeServiceCache();
		}

		private static IExchangeTopologyBridge topologyBridge = new ExchangeTopologyBridge();
	}
}
