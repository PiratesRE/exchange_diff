using System;
using Microsoft.Exchange.HttpProxy.Common;
using Microsoft.Exchange.HttpProxy.RouteSelector;
using Microsoft.Exchange.HttpProxy.Routing;
using Microsoft.Exchange.HttpProxy.Routing.Serialization;
using Microsoft.Exchange.SharedCache.Client;

namespace Microsoft.Exchange.HttpProxy.RouteRefresher
{
	internal class RouteRefresher : IRouteRefresher
	{
		public RouteRefresher(IRouteRefresherDiagnostics diagnostics)
		{
			RouteRefresher.InitializeStaticCacheClients();
			this.Initialize(diagnostics, RouteRefresher.anchorMailboxCacheClientInstance, RouteRefresher.mailboxServerCacheClientInstance);
		}

		public RouteRefresher(ISharedCacheClient anchorMailboxCacheClient, ISharedCacheClient mailboxServerCacheClient, IRouteRefresherDiagnostics diagnostics)
		{
			this.Initialize(diagnostics, anchorMailboxCacheClient, mailboxServerCacheClient);
		}

		public void ProcessRoutingUpdates(string headerValue)
		{
			if (string.IsNullOrEmpty(headerValue))
			{
				throw new ArgumentNullException("headerValue");
			}
			string[] array = headerValue.Split(new char[]
			{
				','
			});
			foreach (string text in array)
			{
				try
				{
					IRoutingEntry routingEntry = RoutingEntryHeaderSerializer.Deserialize(text);
					if (ServerLocator.IsMailboxServerCacheKey(routingEntry.Key) && routingEntry.Destination.RoutingItemType == RoutingItemType.Server)
					{
						this.routeRefresherDiagnostics.IncrementTotalMailboxServerCacheUpdateAttempts();
						if (this.mailboxServerCacheClient.AddEntry(routingEntry))
						{
							string value = "MailboxServerCacheUpdate:" + text;
							this.routeRefresherDiagnostics.AddGenericInfo(value);
							this.routeRefresherDiagnostics.IncrementSuccessfulMailboxServerCacheUpdates();
						}
						else
						{
							string value2 = "MailboxServerCacheFailure:" + text;
							this.routeRefresherDiagnostics.AddErrorInfo(value2);
						}
					}
					else if (ServerLocator.IsAnchorMailboxCacheKey(routingEntry.Key) && routingEntry.Destination.RoutingItemType == RoutingItemType.DatabaseGuid)
					{
						this.routeRefresherDiagnostics.IncrementTotalAnchorMailboxCacheUpdateAttempts();
						if (this.anchorMailboxCacheClient.AddEntry(routingEntry))
						{
							string value3 = "AnchorMailboxCacheUpdate:" + text;
							this.routeRefresherDiagnostics.AddGenericInfo(value3);
							this.routeRefresherDiagnostics.IncrementSuccessfulAnchorMailboxCacheUpdates();
						}
						else
						{
							string value4 = "AnchorMailboxCacheFailure:" + text;
							this.routeRefresherDiagnostics.AddErrorInfo(value4);
						}
					}
					else
					{
						string value5 = "UnrecognizedRoutingEntry:" + text;
						this.routeRefresherDiagnostics.AddErrorInfo(value5);
					}
				}
				catch (ArgumentException)
				{
					string value6 = "DeserializationException:" + text;
					this.routeRefresherDiagnostics.AddErrorInfo(value6);
				}
			}
		}

		public void Initialize(IRouteRefresherDiagnostics diagnostics, ISharedCacheClient anchorMailboxCacheClient, ISharedCacheClient mailboxServerCacheClient)
		{
			this.routeRefresherDiagnostics = diagnostics;
			this.anchorMailboxCacheClient = anchorMailboxCacheClient;
			this.mailboxServerCacheClient = mailboxServerCacheClient;
		}

		private static void InitializeStaticCacheClients()
		{
			if (RouteRefresher.anchorMailboxCacheClientInstance == null)
			{
				lock (RouteRefresher.CacheClientLock)
				{
					if (RouteRefresher.anchorMailboxCacheClientInstance == null)
					{
						RouteRefresher.anchorMailboxCacheClientInstance = new SharedCacheClientWrapper(new SharedCacheClient(WellKnownSharedCache.AnchorMailboxCache, "RouteRefresher_AnchorMailboxCache_" + HttpProxyGlobals.ProtocolType, 1000, false, ConcurrencyGuards.SharedCache));
						RouteRefresher.mailboxServerCacheClientInstance = new SharedCacheClientWrapper(new SharedCacheClient(WellKnownSharedCache.MailboxServerLocator, "RouteRefresher_MailboxServerLocator_" + HttpProxyGlobals.ProtocolType, 1000, false, ConcurrencyGuards.SharedCache));
					}
				}
			}
		}

		private const int CacheTimeoutMilliseconds = 1000;

		private static readonly object CacheClientLock = new object();

		private static ISharedCacheClient anchorMailboxCacheClientInstance;

		private static ISharedCacheClient mailboxServerCacheClientInstance;

		private ISharedCacheClient anchorMailboxCacheClient;

		private ISharedCacheClient mailboxServerCacheClient;

		private IRouteRefresherDiagnostics routeRefresherDiagnostics;
	}
}
