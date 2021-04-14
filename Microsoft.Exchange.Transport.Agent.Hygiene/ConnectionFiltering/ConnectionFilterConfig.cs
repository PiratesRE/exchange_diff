using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Transport.Agent.ConnectionFiltering
{
	internal class ConnectionFilterConfig
	{
		public ConnectionFilterConfig(IConfigurationSession session)
		{
			ADPagedReader<IPAllowListProvider> adpagedReader = session.FindAllPaged<IPAllowListProvider>();
			this.AllowListProviders = adpagedReader.ReadAllPages();
			ADPagedReader<IPBlockListProvider> adpagedReader2 = session.FindAllPaged<IPBlockListProvider>();
			this.BlockListProviders = adpagedReader2.ReadAllPages();
			this.AllowListConfig = session.FindSingletonConfigurationObject<IPAllowListConfig>();
			this.BlockListConfig = session.FindSingletonConfigurationObject<IPBlockListConfig>();
			this.AllowListProviderConfig = session.FindSingletonConfigurationObject<IPAllowListProviderConfig>();
			this.BlockListProviderConfig = session.FindSingletonConfigurationObject<IPBlockListProviderConfig>();
		}

		public readonly IPAllowListProvider[] AllowListProviders;

		public readonly IPBlockListProvider[] BlockListProviders;

		public readonly IPAllowListConfig AllowListConfig;

		public readonly IPBlockListConfig BlockListConfig;

		public readonly IPAllowListProviderConfig AllowListProviderConfig;

		public readonly IPBlockListProviderConfig BlockListProviderConfig;
	}
}
