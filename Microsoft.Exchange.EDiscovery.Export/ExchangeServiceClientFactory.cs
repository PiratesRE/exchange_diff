using System;
using System.Threading;

namespace Microsoft.Exchange.EDiscovery.Export
{
	internal sealed class ExchangeServiceClientFactory : IServiceClientFactory
	{
		public ExchangeServiceClientFactory(IServiceCallingContextFactory serviceCallingContextFactory)
		{
			if (serviceCallingContextFactory == null)
			{
				throw new ArgumentNullException("serviceCallingContextFactory");
			}
			this.serviceCallingContextFactory = serviceCallingContextFactory;
		}

		public ICredentialHandler CredentialHandler
		{
			get
			{
				return this.serviceCallingContextFactory.CredentialHandler;
			}
			set
			{
				this.serviceCallingContextFactory.CredentialHandler = value;
			}
		}

		public IServiceClient<ISourceDataProvider> CreateSourceDataProvider(Uri serviceEndpoint, CancellationToken cancellationToken)
		{
			EwsClient ewsClient = new EwsClient(serviceEndpoint, this.serviceCallingContextFactory.EwsCallingContext, cancellationToken);
			if (ConstantProvider.RebindWithAutoDiscoveryEnabled && ConstantProvider.RebindAutoDiscoveryUrl != null && this.serviceCallingContextFactory.AutoDiscoverCallingContext != null)
			{
				AutoDiscoverClient autoDiscoverClient = (AutoDiscoverClient)this.CreateAutoDiscoverClient(ConstantProvider.RebindAutoDiscoveryUrl, cancellationToken);
				autoDiscoverClient.AutoDiscoverInternalUrlOnly = ConstantProvider.RebindAutoDiscoveryInternalUrlOnly;
				ewsClient.AutoDiscoverInterface = autoDiscoverClient.FunctionalInterface;
			}
			return ewsClient;
		}

		public IServiceClient<IAutoDiscoverClient> CreateAutoDiscoverClient(Uri serviceEndpoint, CancellationToken cancellationToken)
		{
			return new AutoDiscoverClient(serviceEndpoint, this.serviceCallingContextFactory.AutoDiscoverCallingContext, cancellationToken);
		}

		private readonly IServiceCallingContextFactory serviceCallingContextFactory;
	}
}
