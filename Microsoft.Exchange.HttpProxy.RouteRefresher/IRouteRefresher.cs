using System;
using Microsoft.Exchange.HttpProxy.RouteSelector;

namespace Microsoft.Exchange.HttpProxy.RouteRefresher
{
	internal interface IRouteRefresher
	{
		void Initialize(IRouteRefresherDiagnostics diagnostics, ISharedCacheClient anchorMailboxCacheClient, ISharedCacheClient mailboxServerCacheClient);

		void ProcessRoutingUpdates(string headerValue);
	}
}
