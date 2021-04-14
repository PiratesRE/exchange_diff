using System;
using Microsoft.Exchange.HttpProxy.Routing.Providers;
using Microsoft.Exchange.HttpProxy.Routing.RoutingDestinations;
using Microsoft.Exchange.HttpProxy.Routing.RoutingEntries;
using Microsoft.Exchange.HttpProxy.Routing.Serialization;

namespace Microsoft.Exchange.HttpProxy.Routing.RoutingLookups
{
	internal abstract class AnchorMailboxSharedCacheLookup : IRoutingLookup
	{
		protected AnchorMailboxSharedCacheLookup(ISharedCache sharedCache, RoutingItemType validItemType)
		{
			if (sharedCache == null)
			{
				throw new ArgumentNullException("sharedCache");
			}
			this.sharedCache = sharedCache;
			this.validItemType = validItemType;
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
			if (routingKey.RoutingItemType != this.validItemType)
			{
				string message = string.Format("Routing key type {0} is not supported in {1}", RoutingEntryHeaderSerializer.RoutingTypeToString(routingKey.RoutingItemType), base.GetType());
				throw new ArgumentException(message, "routingKey");
			}
			string sharedCacheKeyFromRoutingKey = this.sharedCache.GetSharedCacheKeyFromRoutingKey(routingKey);
			try
			{
				byte[] bytes;
				if (this.sharedCache.TryGet(sharedCacheKeyFromRoutingKey, out bytes, diagnostics))
				{
					AnchorMailboxCacheEntry anchorMailboxCacheEntry = new AnchorMailboxCacheEntry();
					anchorMailboxCacheEntry.FromByteArray(bytes);
					if (anchorMailboxCacheEntry.Database != null)
					{
						DatabaseGuidRoutingDestination destination = new DatabaseGuidRoutingDestination(anchorMailboxCacheEntry.Database.ObjectGuid, anchorMailboxCacheEntry.DomainName, anchorMailboxCacheEntry.Database.PartitionFQDN);
						return new SuccessfulMailboxRoutingEntry(routingKey, destination, DateTime.Now.ToFileTimeUtc());
					}
				}
			}
			catch (SharedCacheException ex)
			{
				ErrorRoutingDestination destination2 = new ErrorRoutingDestination(ex.Message);
				return new FailedMailboxRoutingEntry(routingKey, destination2, DateTime.UtcNow.ToFileTimeUtc());
			}
			return null;
		}

		private readonly ISharedCache sharedCache;

		private readonly RoutingItemType validItemType;
	}
}
