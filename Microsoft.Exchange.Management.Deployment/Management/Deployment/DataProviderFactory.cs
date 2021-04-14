using System;
using Microsoft.Exchange.Management.Deployment.HybridConfigurationDetection;
using Microsoft.Exchange.Setup.Common;

namespace Microsoft.Exchange.Management.Deployment
{
	internal class DataProviderFactory : IDataProviderFactory
	{
		public IADDataProvider ADDataProvider
		{
			get
			{
				if (this.adProvider == null)
				{
					this.adProvider = new ADProvider(5, 5, new TimeSpan(0, 0, 2));
				}
				return this.adProvider;
			}
		}

		public IManagedMethodProvider ManagedMethodProvider
		{
			get
			{
				if (this.managedMethodProvider == null)
				{
					this.managedMethodProvider = new ManagedMethodProvider();
				}
				return this.managedMethodProvider;
			}
		}

		public IMonadDataProvider MonadDataProvider
		{
			get
			{
				if (this.monadProvider == null)
				{
					this.monadProvider = new MonadProvider();
				}
				return this.monadProvider;
			}
		}

		public INativeMethodProvider NativeMethodProvider
		{
			get
			{
				if (this.nativeMethodProvider == null)
				{
					this.nativeMethodProvider = new NativeMethodProvider();
				}
				return this.nativeMethodProvider;
			}
		}

		public IRegistryDataProvider RegistryDataProvider
		{
			get
			{
				if (this.registryDataProvider == null)
				{
					this.registryDataProvider = new RegistryDataProvider();
				}
				return this.registryDataProvider;
			}
		}

		public IWMIDataProvider WMIDataProvider
		{
			get
			{
				if (this.wmiDataProvider == null)
				{
					this.wmiDataProvider = new WMIProvider();
				}
				return this.wmiDataProvider;
			}
		}

		public IWebAdminDataProvider WebAdminDataProvider
		{
			get
			{
				if (this.webAdminDataProvider == null)
				{
					this.webAdminDataProvider = new WebAdminDataProvider();
				}
				return this.webAdminDataProvider;
			}
		}

		public IHybridConfigurationDetection HybridConfigurationDetectionProvider
		{
			get
			{
				if (this.hybridConfigurationDetectionProvider == null)
				{
					BridgeLogger logger = new BridgeLogger(SetupLogger.Logger);
					this.hybridConfigurationDetectionProvider = new HybridConfigurationDetection(logger);
				}
				return this.hybridConfigurationDetectionProvider;
			}
		}

		private ADProvider adProvider;

		private MonadProvider monadProvider;

		private ManagedMethodProvider managedMethodProvider;

		private NativeMethodProvider nativeMethodProvider;

		private RegistryDataProvider registryDataProvider;

		private WMIProvider wmiDataProvider;

		private WebAdminDataProvider webAdminDataProvider;

		private HybridConfigurationDetection hybridConfigurationDetectionProvider;
	}
}
