using System;
using Microsoft.Exchange.HttpProxy.Routing.Providers;

namespace Microsoft.Exchange.HttpProxy.Routing.RoutingLookups
{
	internal class MailboxGuidSharedCacheLookup : AnchorMailboxSharedCacheLookup
	{
		public MailboxGuidSharedCacheLookup(ISharedCache sharedCache) : base(sharedCache, RoutingItemType.MailboxGuid)
		{
		}
	}
}
