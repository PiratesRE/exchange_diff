using System;
using Microsoft.Exchange.HttpProxy.Routing.Providers;
using Microsoft.Exchange.HttpProxy.Routing.RoutingDestinations;
using Microsoft.Exchange.HttpProxy.Routing.RoutingEntries;
using Microsoft.Exchange.HttpProxy.Routing.RoutingKeys;

namespace Microsoft.Exchange.HttpProxy.Routing.RoutingLookups
{
	internal class DatabaseGuidSharedCacheLookup : IRoutingLookup
	{
		public DatabaseGuidSharedCacheLookup(ISharedCache sharedCache)
		{
			if (sharedCache == null)
			{
				throw new ArgumentNullException("sharedCache");
			}
			this.sharedCache = sharedCache;
		}

		IRoutingEntry IRoutingLookup.GetRoutingEntry(IRoutingKey routingKey, IRoutingDiagnostics diagnostics)
		{
			if (routingKey == null)
			{
				throw new ArgumentNullException("routingKey");
			}
			if (diagnostics == null)
			{
				throw new ArgumentNullException("diagnostics");
			}
			DatabaseGuidRoutingKey databaseGuidRoutingKey = routingKey as DatabaseGuidRoutingKey;
			if (databaseGuidRoutingKey == null)
			{
				string message = string.Format("Routing key type {0} is not supported", routingKey.GetType());
				throw new ArgumentException(message, "routingKey");
			}
			return this.GetDatabaseGuidRoutingEntry(databaseGuidRoutingKey, diagnostics);
		}

		public DatabaseGuidRoutingEntry GetDatabaseGuidRoutingEntry(DatabaseGuidRoutingKey databaseGuidRoutingKey, IRoutingDiagnostics diagnostics)
		{
			if (databaseGuidRoutingKey == null)
			{
				throw new ArgumentNullException("databaseGuidRoutingKey");
			}
			DatabaseGuidRoutingEntry result;
			try
			{
				MailboxServerCacheEntry mailboxServerCacheEntry;
				if (this.sharedCache.TryGet<MailboxServerCacheEntry>(databaseGuidRoutingKey.DatabaseGuid.ToString(), out mailboxServerCacheEntry, diagnostics))
				{
					result = new SuccessfulDatabaseGuidRoutingEntry(databaseGuidRoutingKey, new ServerRoutingDestination(mailboxServerCacheEntry.BackEndServer.Fqdn, new int?(mailboxServerCacheEntry.BackEndServer.Version)), DateTime.UtcNow.ToFileTimeUtc());
				}
				else
				{
					result = null;
				}
			}
			catch (SharedCacheException ex)
			{
				ErrorRoutingDestination destination = new ErrorRoutingDestination(ex.Message);
				result = new FailedDatabaseGuidRoutingEntry(databaseGuidRoutingKey, destination, DateTime.UtcNow.ToFileTimeUtc());
			}
			return result;
		}

		private readonly ISharedCache sharedCache;
	}
}
