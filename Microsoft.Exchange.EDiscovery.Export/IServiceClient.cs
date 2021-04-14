using System;

namespace Microsoft.Exchange.EDiscovery.Export
{
	internal interface IServiceClient<FunctionalInterfaceType> : IDisposable
	{
		Uri ServiceEndpoint { get; }

		FunctionalInterfaceType FunctionalInterface { get; }

		bool Connect();
	}
}
