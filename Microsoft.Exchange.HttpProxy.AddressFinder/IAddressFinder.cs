using System;
using Microsoft.Exchange.HttpProxy.Routing;

namespace Microsoft.Exchange.HttpProxy.AddressFinder
{
	internal interface IAddressFinder
	{
		IRoutingKey[] Find(AddressFinderSource source, IAddressFinderDiagnostics diagnostics);
	}
}
