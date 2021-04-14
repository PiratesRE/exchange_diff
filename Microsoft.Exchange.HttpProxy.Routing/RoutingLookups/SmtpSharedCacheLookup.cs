using System;
using Microsoft.Exchange.HttpProxy.Routing.Providers;

namespace Microsoft.Exchange.HttpProxy.Routing.RoutingLookups
{
	internal class SmtpSharedCacheLookup : AnchorMailboxSharedCacheLookup
	{
		public SmtpSharedCacheLookup(ISharedCache sharedCache) : base(sharedCache, RoutingItemType.Smtp)
		{
		}
	}
}
