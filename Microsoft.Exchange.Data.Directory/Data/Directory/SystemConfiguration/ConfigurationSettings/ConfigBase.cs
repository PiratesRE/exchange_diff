using System;
using Microsoft.Exchange.Data.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ConfigBase<TSchema> where TSchema : IConfigSchema, new()
	{
		public static IConfigProvider Provider
		{
			get
			{
				if (ConfigBase<TSchema>.provider == null)
				{
					ConfigBase<TSchema>.InitializeConfigProvider(new Func<IConfigSchema, IConfigProvider>(ConfigProvider.CreateADProvider));
				}
				return ConfigBase<TSchema>.provider;
			}
		}

		public static ISettingsContext CurrentContext
		{
			get
			{
				return SettingsContextBase.EffectiveContext;
			}
		}

		public static void InitializeConfigProvider(Func<IConfigSchema, IConfigProvider> configCreator)
		{
			if (configCreator == null)
			{
				throw new ArgumentNullException("configCreator");
			}
			lock (ConfigBase<TSchema>.lockObj)
			{
				if (ConfigBase<TSchema>.provider == null)
				{
					ConfigBase<TSchema>.isInitializing = true;
					try
					{
						IConfigProvider configProvider = configCreator(ConfigBase<TSchema>.Schema);
						configProvider.Initialize();
						IDisposeTrackable disposeTrackable = configProvider as IDisposeTrackable;
						if (disposeTrackable != null)
						{
							disposeTrackable.SuppressDisposeTracker();
						}
						ConfigBase<TSchema>.provider = configProvider;
					}
					finally
					{
						ConfigBase<TSchema>.isInitializing = false;
					}
				}
			}
		}

		public static T GetConfig<T>(string settingName)
		{
			if (ConfigBase<TSchema>.isInitializing)
			{
				return ConfigSchemaBase.GetDefaultConfig<T>(ConfigBase<TSchema>.Schema, settingName);
			}
			return ConfigBase<TSchema>.Provider.GetConfig<T>(SettingsContextBase.EffectiveContext, settingName);
		}

		public static bool TryGetConfig<T>(string settingName, out T settingValue)
		{
			if (ConfigBase<TSchema>.isInitializing)
			{
				settingValue = default(T);
				return false;
			}
			return ConfigBase<TSchema>.Provider.TryGetConfig<T>(SettingsContextBase.EffectiveContext, settingName, out settingValue);
		}

		private static object lockObj = new object();

		private static IConfigProvider provider;

		private static bool isInitializing = false;

		public static readonly TSchema Schema = (default(TSchema) == null) ? Activator.CreateInstance<TSchema>() : default(TSchema);
	}
}
