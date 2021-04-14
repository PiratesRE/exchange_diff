using System;
using Microsoft.Exchange.HttpProxy.Routing.Providers;

namespace Microsoft.Exchange.HttpProxy.Routing.RoutingLookups
{
	internal class ExternalDirectoryObjectIdSharedCacheLookup : AnchorMailboxSharedCacheLookup
	{
		public ExternalDirectoryObjectIdSharedCacheLookup(ISharedCache sharedCache) : base(sharedCache, RoutingItemType.ExternalDirectoryObjectId)
		{
		}
	}
}
