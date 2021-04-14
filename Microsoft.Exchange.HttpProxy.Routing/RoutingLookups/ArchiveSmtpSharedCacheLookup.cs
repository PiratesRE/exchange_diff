using System;
using Microsoft.Exchange.HttpProxy.Routing.Providers;

namespace Microsoft.Exchange.HttpProxy.Routing.RoutingLookups
{
	internal class ArchiveSmtpSharedCacheLookup : AnchorMailboxSharedCacheLookup
	{
		public ArchiveSmtpSharedCacheLookup(ISharedCache sharedCache) : base(sharedCache, RoutingItemType.ArchiveSmtp)
		{
		}
	}
}
