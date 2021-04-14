using System;
using System.Threading;

namespace Microsoft.Exchange.EDiscovery.Export
{
	internal interface IServiceClientFactory
	{
		ICredentialHandler CredentialHandler { get; set; }

		IServiceClient<ISourceDataProvider> CreateSourceDataProvider(Uri serviceEndpoint, CancellationToken cancellationToken);

		IServiceClient<IAutoDiscoverClient> CreateAutoDiscoverClient(Uri serviceEndpoint, CancellationToken cancellationToken);
	}
}
