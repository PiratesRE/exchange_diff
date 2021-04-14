using System;

namespace Microsoft.Exchange.HttpProxy.RouteSelector
{
	internal interface IServerLocatorFactory
	{
		IServerLocator GetServerLocator(ProtocolType protocolType);
	}
}
