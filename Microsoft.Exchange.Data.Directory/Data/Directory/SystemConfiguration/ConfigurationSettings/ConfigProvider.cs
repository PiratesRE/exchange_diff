using System;
using Microsoft.Exchange.Data.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ConfigProvider : ConfigProviderBase
	{
		protected ConfigProvider(IConfigSchema schema) : base(schema)
		{
		}

		public static IConfigProvider CreateProvider(IConfigSchema schema)
		{
			return ConfigProvider.CreateProvider(schema, new TimeSpan?(ConfigDriverBase.DefaultErrorThresholdInterval));
		}

		public static IConfigProvider CreateProvider(IConfigSchema schema, TimeSpan? errorThresholdInterval)
		{
			ConfigFlags overrideFlags = ConfigProviderBase.OverrideFlags;
			if ((overrideFlags & ConfigFlags.DisallowADConfig) == ConfigFlags.DisallowADConfig)
			{
				return ConfigProvider.CreateAppProvider(schema);
			}
			if ((overrideFlags & ConfigFlags.DisallowAppConfig) == ConfigFlags.DisallowAppConfig)
			{
				return ConfigProvider.CreateADProvider(schema);
			}
			ConfigProvider configProvider = new ConfigProvider(schema);
			ConfigDriverBase configDriver = new ADConfigDriver(schema, errorThresholdInterval);
			ConfigDriverBase configDriver2 = new AppConfigDriver(schema, errorThresholdInterval);
			if ((overrideFlags & ConfigFlags.LowADConfigPriority) == ConfigFlags.LowADConfigPriority)
			{
				configProvider.AddConfigDriver(configDriver2);
				configProvider.AddConfigDriver(configDriver);
			}
			else
			{
				configProvider.AddConfigDriver(configDriver);
				configProvider.AddConfigDriver(configDriver2);
			}
			return configProvider;
		}

		public static IConfigProvider CreateADProvider(IConfigSchema schema)
		{
			return ConfigProvider.CreateADProvider(schema, new TimeSpan?(ConfigDriverBase.DefaultErrorThresholdInterval));
		}

		public static IConfigProvider CreateADProvider(IConfigSchema schema, TimeSpan? errorThresholdInterval)
		{
			ConfigFlags overrideFlags = ConfigProviderBase.OverrideFlags;
			if ((overrideFlags & ConfigFlags.DisallowADConfig) == ConfigFlags.DisallowADConfig)
			{
				return ConfigProvider.CreateDefaultValueProvider(schema);
			}
			ConfigProvider configProvider = new ConfigProvider(schema);
			configProvider.AddConfigDriver(new ADConfigDriver(schema, errorThresholdInterval));
			return configProvider;
		}

		public static IConfigProvider CreateAppProvider(IConfigSchema schema)
		{
			return ConfigProvider.CreateAppProvider(schema, new TimeSpan?(ConfigDriverBase.DefaultErrorThresholdInterval));
		}

		public static IConfigProvider CreateAppProvider(IConfigSchema schema, TimeSpan? errorThresholdInterval)
		{
			ConfigFlags overrideFlags = ConfigProviderBase.OverrideFlags;
			if ((overrideFlags & ConfigFlags.DisallowAppConfig) == ConfigFlags.DisallowAppConfig)
			{
				return ConfigProvider.CreateDefaultValueProvider(schema);
			}
			ConfigProvider configProvider = new ConfigProvider(schema);
			configProvider.AddConfigDriver(new AppConfigDriver(schema, errorThresholdInterval));
			return configProvider;
		}

		public static IConfigProvider CreateDefaultValueProvider(IConfigSchema schema)
		{
			return new ConfigProvider(schema);
		}

		public override T GetConfig<T>(string settingName)
		{
			return base.GetConfig<T>(SettingsContextBase.EffectiveContext, settingName);
		}
	}
}
