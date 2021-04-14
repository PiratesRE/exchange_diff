using System;
using Microsoft.Exchange.Data.ConfigurationSettings;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SlimTenantConfigImpl
	{
		public static T GetConfig<T>(string settingName)
		{
			T config;
			using (IConfigProvider configProvider = ConfigProvider.CreateADProvider(new SlimTenantConfigSchema(), null))
			{
				configProvider.Initialize();
				config = configProvider.GetConfig<T>(settingName);
			}
			return config;
		}
	}
}
