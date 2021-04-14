using System;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Directory;
using Microsoft.Exchange.MailboxLoadBalance.LoadBalance;
using Microsoft.Exchange.MailboxLoadBalance.ServiceSupport;

namespace Microsoft.Exchange.MailboxLoadBalance.Clients
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class CachingClientFactory : IClientFactory
	{
		public CachingClientFactory(TimeSpan cacheDuration, IClientFactory clientFactory, ILogger logger)
		{
			this.physicalDatabaseCache = new ExactTimeoutCache<Guid, CachedPhysicalDatabase>(new RemoveItemDelegate<Guid, CachedPhysicalDatabase>(this.RemoveClient<Guid, CachedPhysicalDatabase>), new ShouldRemoveDelegate<Guid, CachedPhysicalDatabase>(this.ShouldRemoveEntry<Guid, CachedPhysicalDatabase>), null, 10000, true, CacheFullBehavior.ExpireExisting);
			this.loadBalanceClientCache = new ExactTimeoutCache<Guid, CachedLoadBalanceClient>(new RemoveItemDelegate<Guid, CachedLoadBalanceClient>(this.RemoveClient<Guid, CachedLoadBalanceClient>), new ShouldRemoveDelegate<Guid, CachedLoadBalanceClient>(this.ShouldRemoveEntry<Guid, CachedLoadBalanceClient>), null, 10000, true, CacheFullBehavior.ExpireExisting);
			this.injectorClientCache = new ExactTimeoutCache<Guid, CachedInjectorClient>(new RemoveItemDelegate<Guid, CachedInjectorClient>(this.RemoveClient<Guid, CachedInjectorClient>), new ShouldRemoveDelegate<Guid, CachedInjectorClient>(this.ShouldRemoveEntry<Guid, CachedInjectorClient>), null, 10000, true, CacheFullBehavior.ExpireExisting);
			this.cacheDuration = cacheDuration;
			this.clientFactory = clientFactory;
			this.logger = logger;
		}

		ILoadBalanceService IClientFactory.GetLoadBalanceClientForServer(DirectoryServer server, bool allowFallbackToLocal)
		{
			return this.GetClient<CachedLoadBalanceClient, DirectoryServer>(this.loadBalanceClientCache, server, () => new CachedLoadBalanceClient(this.clientFactory.GetLoadBalanceClientForServer(server, allowFallbackToLocal)));
		}

		ILoadBalanceService IClientFactory.GetLoadBalanceClientForDatabase(DirectoryDatabase database)
		{
			return this.GetClient<CachedLoadBalanceClient, DirectoryDatabase>(this.loadBalanceClientCache, database, () => new CachedLoadBalanceClient(this.clientFactory.GetLoadBalanceClientForDatabase(database)));
		}

		IInjectorService IClientFactory.GetInjectorClientForDatabase(DirectoryDatabase database)
		{
			return this.GetClient<CachedInjectorClient, DirectoryDatabase>(this.injectorClientCache, database, () => new CachedInjectorClient(this.clientFactory.GetInjectorClientForDatabase(database)));
		}

		IPhysicalDatabase IClientFactory.GetPhysicalDatabaseConnection(DirectoryDatabase database)
		{
			return this.GetClient<CachedPhysicalDatabase, DirectoryDatabase>(this.physicalDatabaseCache, database, () => new CachedPhysicalDatabase(this.clientFactory.GetPhysicalDatabaseConnection(database)));
		}

		ILoadBalanceService IClientFactory.GetLoadBalanceClientForCentralServer()
		{
			return this.clientFactory.GetLoadBalanceClientForCentralServer();
		}

		private void RemoveClient<TKey, TCachedClient>(TKey key, TCachedClient value, RemoveReason reason) where TCachedClient : CachedClient
		{
			this.logger.Log(MigrationEventType.Instrumentation, "Removing {0} entry for {1}.", new object[]
			{
				value.GetType().Name,
				key
			});
			value.Cleanup();
		}

		private bool ShouldRemoveEntry<TKey, TCachedClient>(TKey key, TCachedClient value) where TCachedClient : CachedClient
		{
			bool canRemoveFromCache = value.CanRemoveFromCache;
			this.logger.Log(MigrationEventType.Instrumentation, "Can remove {0} entry for {1}? {2}.", new object[]
			{
				value.GetType().Name,
				key,
				canRemoveFromCache
			});
			return canRemoveFromCache;
		}

		private TCacheEntry GetClient<TCacheEntry, TKey>(ExactTimeoutCache<Guid, TCacheEntry> cache, TKey key, Func<TCacheEntry> createNewEntry) where TCacheEntry : CachedClient where TKey : DirectoryObject
		{
			TCacheEntry tcacheEntry;
			if (cache.TryGetValue(key.Guid, out tcacheEntry))
			{
				if (tcacheEntry.IsValid)
				{
					this.logger.Log(MigrationEventType.Instrumentation, "Returning cached {0} client for {1}", new object[]
					{
						typeof(TCacheEntry).Name,
						key.Name
					});
					tcacheEntry.IncrementReferenceCount();
					return tcacheEntry;
				}
				cache.Remove(key.Guid);
			}
			this.logger.Log(MigrationEventType.Instrumentation, "Creating new {0} client for {1}", new object[]
			{
				typeof(TCacheEntry).Name,
				key.Name
			});
			tcacheEntry = createNewEntry();
			cache.TryInsertSliding(key.Guid, tcacheEntry, this.cacheDuration);
			tcacheEntry.IncrementReferenceCount();
			return tcacheEntry;
		}

		private readonly TimeSpan cacheDuration;

		private readonly IClientFactory clientFactory;

		private readonly ILogger logger;

		private readonly ExactTimeoutCache<Guid, CachedPhysicalDatabase> physicalDatabaseCache;

		private readonly ExactTimeoutCache<Guid, CachedLoadBalanceClient> loadBalanceClientCache;

		private readonly ExactTimeoutCache<Guid, CachedInjectorClient> injectorClientCache;
	}
}
