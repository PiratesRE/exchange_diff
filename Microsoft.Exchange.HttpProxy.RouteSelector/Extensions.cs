using System;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.HttpProxy.Common;
using Microsoft.Exchange.HttpProxy.Routing;
using Microsoft.Exchange.HttpProxy.Routing.RoutingDestinations;
using Microsoft.Exchange.HttpProxy.Routing.RoutingEntries;
using Microsoft.Exchange.HttpProxy.Routing.RoutingKeys;

namespace Microsoft.Exchange.HttpProxy.RouteSelector
{
	public static class Extensions
	{
		public static IRoutingKey CreateRoutingKey(this IRoutingDestination destination)
		{
			DatabaseGuidRoutingDestination databaseGuidRoutingDestination = destination as DatabaseGuidRoutingDestination;
			if (databaseGuidRoutingDestination != null)
			{
				return new DatabaseGuidRoutingKey(databaseGuidRoutingDestination.DatabaseGuid, databaseGuidRoutingDestination.DomainName, databaseGuidRoutingDestination.ResourceForest);
			}
			return null;
		}

		public static bool AddEntry(this ISharedCacheClient cacheClient, IRoutingEntry entry)
		{
			SuccessfulMailboxRoutingEntry firstRoutingEntry = entry as SuccessfulMailboxRoutingEntry;
			if (firstRoutingEntry != null)
			{
				DatabaseGuidRoutingDestination databaseGuidRoutingDestination = entry.Destination as DatabaseGuidRoutingDestination;
				if (databaseGuidRoutingDestination != null)
				{
					string sharedCacheKeyFromRoutingKey = cacheClient.GetSharedCacheKeyFromRoutingKey(entry.Key);
					AnchorMailboxCacheEntry anchorMailboxCacheEntry = new AnchorMailboxCacheEntry();
					anchorMailboxCacheEntry.Database = new ADObjectId(databaseGuidRoutingDestination.DatabaseGuid, databaseGuidRoutingDestination.ResourceForest);
					anchorMailboxCacheEntry.DomainName = databaseGuidRoutingDestination.DomainName;
					DateTime utcNow = DateTime.UtcNow;
					string text;
					return cacheClient.TryInsert(sharedCacheKeyFromRoutingKey, anchorMailboxCacheEntry.ToByteArray(), utcNow, out text);
				}
			}
			else
			{
				SuccessfulDatabaseGuidRoutingEntry successfulDatabaseGuidRoutingEntry = entry as SuccessfulDatabaseGuidRoutingEntry;
				if (successfulDatabaseGuidRoutingEntry != null)
				{
					ServerRoutingDestination serverRoutingDestination = entry.Destination as ServerRoutingDestination;
					if (serverRoutingDestination != null)
					{
						BackEndServer backEndServer = new BackEndServer(serverRoutingDestination.Fqdn, serverRoutingDestination.Version ?? 0);
						string resourceForest;
						if (Utilities.TryExtractForestFqdnFromServerFqdn(backEndServer.Fqdn, out resourceForest))
						{
							MailboxServerCacheEntry value = new MailboxServerCacheEntry(backEndServer, resourceForest, DateTime.UtcNow, false);
							DateTime utcNow2 = DateTime.UtcNow;
							DatabaseGuidRoutingKey databaseGuidRoutingKey = successfulDatabaseGuidRoutingEntry.Key as DatabaseGuidRoutingKey;
							string text2;
							return cacheClient.TryInsert(databaseGuidRoutingKey.DatabaseGuid.ToString(), value, utcNow2, out text2);
						}
					}
				}
			}
			return false;
		}
	}
}
