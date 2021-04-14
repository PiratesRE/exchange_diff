using System;

namespace Microsoft.Exchange.HttpProxy.Routing
{
	public interface IRoutingLookup
	{
		IRoutingEntry GetRoutingEntry(IRoutingKey routingKey, IRoutingDiagnostics diagnostics);
	}
}
