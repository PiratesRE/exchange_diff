using System;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal interface IProxyHubSelectorComponent : ITransportComponent
	{
		IProxyHubSelector ProxyHubSelector { get; }

		void SetLoadTimeDependencies(IMailRouter router, ITransportConfiguration configuration);
	}
}
