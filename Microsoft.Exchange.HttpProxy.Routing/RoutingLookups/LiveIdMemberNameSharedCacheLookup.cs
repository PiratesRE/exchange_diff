using System;
using Microsoft.Exchange.HttpProxy.Routing.Providers;

namespace Microsoft.Exchange.HttpProxy.Routing.RoutingLookups
{
	internal class LiveIdMemberNameSharedCacheLookup : AnchorMailboxSharedCacheLookup
	{
		public LiveIdMemberNameSharedCacheLookup(ISharedCache sharedCache) : base(sharedCache, RoutingItemType.LiveIdMemberName)
		{
		}
	}
}
