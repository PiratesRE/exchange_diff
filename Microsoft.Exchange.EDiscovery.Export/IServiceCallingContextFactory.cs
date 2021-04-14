using System;
using Microsoft.Exchange.EDiscovery.Export.AutoDiscoverProxy;
using Microsoft.Exchange.EDiscovery.Export.EwsProxy;

namespace Microsoft.Exchange.EDiscovery.Export
{
	internal interface IServiceCallingContextFactory
	{
		ICredentialHandler CredentialHandler { get; set; }

		IServiceCallingContext<DefaultBinding_Autodiscover> AutoDiscoverCallingContext { get; }

		IServiceCallingContext<ExchangeServiceBinding> EwsCallingContext { get; }
	}
}
