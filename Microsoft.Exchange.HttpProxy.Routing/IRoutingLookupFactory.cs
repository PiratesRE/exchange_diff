using System;

namespace Microsoft.Exchange.HttpProxy.Routing
{
	public interface IRoutingLookupFactory
	{
		IRoutingLookup GetLookupForType(RoutingItemType routingEntryType);
	}
}
