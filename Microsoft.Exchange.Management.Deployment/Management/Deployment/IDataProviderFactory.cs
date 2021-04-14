using System;
using Microsoft.Exchange.Management.Deployment.HybridConfigurationDetection;

namespace Microsoft.Exchange.Management.Deployment
{
	internal interface IDataProviderFactory
	{
		IADDataProvider ADDataProvider { get; }

		IManagedMethodProvider ManagedMethodProvider { get; }

		IMonadDataProvider MonadDataProvider { get; }

		INativeMethodProvider NativeMethodProvider { get; }

		IRegistryDataProvider RegistryDataProvider { get; }

		IWMIDataProvider WMIDataProvider { get; }

		IWebAdminDataProvider WebAdminDataProvider { get; }

		IHybridConfigurationDetection HybridConfigurationDetectionProvider { get; }
	}
}
