using System;
using Microsoft.Exchange.HttpProxy.Common;
using Microsoft.Exchange.HttpProxy.Routing;
using Microsoft.Exchange.HttpProxy.Routing.Providers;
using Microsoft.Exchange.SharedCache.Client;

namespace Microsoft.Exchange.HttpProxy.RouteSelector
{
	public class RouteSelector : IServerLocatorFactory
	{
		public RouteSelector()
		{
			this.InitializeStaticSharedCacheClients();
			this.Initialize(RouteSelector.anchorMailboxSharedCacheClientInstance, RouteSelector.mailboxServerSharedCacheClientInstance, RouteSelector.locatorServiceLookupFactoryInstance);
		}

		public RouteSelector(ISharedCacheClient anchorMailboxSharedCacheClient, ISharedCacheClient mailboxServerSharedCacheClient, IRoutingLookupFactory locatorServiceLookupFactory)
		{
			if (anchorMailboxSharedCacheClient == null)
			{
				throw new ArgumentNullException("anchorMailboxCacheClient");
			}
			if (mailboxServerSharedCacheClient == null)
			{
				throw new ArgumentNullException("mailboxServerCacheClient");
			}
			if (locatorServiceLookupFactory == null)
			{
				throw new ArgumentNullException("locatorServiceLookupFactory");
			}
			this.Initialize(anchorMailboxSharedCacheClient, mailboxServerSharedCacheClient, locatorServiceLookupFactory);
		}

		IServerLocator IServerLocatorFactory.GetServerLocator(ProtocolType protocolType)
		{
			if (this.locatorInstance == null)
			{
				this.locatorInstance = new ServerLocator(this.anchorMailboxSharedCacheClient, this.mailboxServerSharedCacheClient, this.locatorServiceLookupFactory);
			}
			return this.locatorInstance;
		}

		private void Initialize(ISharedCacheClient anchorMailboxSharedCacheClient, ISharedCacheClient mailboxServerSharedCacheClient, IRoutingLookupFactory locatorServiceLookupFactory)
		{
			this.anchorMailboxSharedCacheClient = anchorMailboxSharedCacheClient;
			this.mailboxServerSharedCacheClient = mailboxServerSharedCacheClient;
			this.locatorServiceLookupFactory = locatorServiceLookupFactory;
		}

		private void InitializeStaticSharedCacheClients()
		{
			if (RouteSelector.anchorMailboxSharedCacheClientInstance == null)
			{
				lock (RouteSelector.CacheClientLock)
				{
					if (RouteSelector.anchorMailboxSharedCacheClientInstance == null)
					{
						RouteSelector.anchorMailboxSharedCacheClientInstance = new SharedCacheClientWrapper(new SharedCacheClient(WellKnownSharedCache.AnchorMailboxCache, "RouteSelector_AnchorMailboxCache_" + HttpProxyGlobals.ProtocolType, 1000, false, ConcurrencyGuards.SharedCache));
						RouteSelector.mailboxServerSharedCacheClientInstance = new SharedCacheClientWrapper(new SharedCacheClient(WellKnownSharedCache.MailboxServerLocator, "RouteSelector_MailboxServerLocator_" + HttpProxyGlobals.ProtocolType, 1000, false, ConcurrencyGuards.SharedCache));
						RouteSelector.locatorServiceLookupFactoryInstance = new RoutingEntryLookupFactory(new MailboxServerLocatorServiceProvider(), new ActiveDirectoryUserProvider(false));
					}
				}
			}
		}

		private const int CacheTimeoutMilliseconds = 1000;

		private static readonly object CacheClientLock = new object();

		private static ISharedCacheClient anchorMailboxSharedCacheClientInstance;

		private static ISharedCacheClient mailboxServerSharedCacheClientInstance;

		private static IRoutingLookupFactory locatorServiceLookupFactoryInstance;

		private ServerLocator locatorInstance;

		private ISharedCacheClient anchorMailboxSharedCacheClient;

		private ISharedCacheClient mailboxServerSharedCacheClient;

		private IRoutingLookupFactory locatorServiceLookupFactory;
	}
}
