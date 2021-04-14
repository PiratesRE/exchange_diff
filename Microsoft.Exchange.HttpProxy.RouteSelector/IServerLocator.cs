using System;
using Microsoft.Exchange.HttpProxy.Routing;

namespace Microsoft.Exchange.HttpProxy.RouteSelector
{
	internal interface IServerLocator
	{
		ServerLocatorReturn LocateServer(IRoutingKey[] keys, IRouteSelectorDiagnostics logger);
	}
}
