using System;
using System.Reflection;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.DataProvider
{
	internal abstract class ConfigDataProviderFactory
	{
		internal static EnvironmentStrategy Environment
		{
			get
			{
				return ConfigDataProviderFactory.environment;
			}
			set
			{
				ConfigDataProviderFactory.environment = value;
			}
		}

		internal static ConfigDataProviderFactory CacheDefault
		{
			get
			{
				if (ConfigDataProviderFactory.cacheInstance == null)
				{
					if (!ConfigDataProviderFactory.environment.IsForefrontForOffice() && !ConfigDataProviderFactory.environment.IsForefrontDALOverrideUseSQL())
					{
						throw new NotImplementedException("On-premise provider factory not yet implemented");
					}
					Assembly assembly = Assembly.Load("Microsoft.Exchange.Hygiene.CacheDataProvider");
					Type type = assembly.GetType("Microsoft.Exchange.Hygiene.Cache.DataProvider.CacheDataProvider+Factory");
					ConfigDataProviderFactory.cacheInstance = (ConfigDataProviderFactory)Activator.CreateInstance(type);
				}
				return ConfigDataProviderFactory.cacheInstance;
			}
			set
			{
				ConfigDataProviderFactory.cacheInstance = value;
			}
		}

		internal static ConfigDataProviderFactory CacheFallbackDefault
		{
			get
			{
				if (ConfigDataProviderFactory.compositeInstance == null)
				{
					if (!ConfigDataProviderFactory.environment.IsForefrontForOffice() && !ConfigDataProviderFactory.environment.IsForefrontDALOverrideUseSQL())
					{
						throw new NotImplementedException("On-premise provider factory not yet implemented");
					}
					Assembly assembly = Assembly.Load("Microsoft.Exchange.Hygiene.CacheDataProvider");
					Type type = assembly.GetType("Microsoft.Exchange.Hygiene.Cache.DataProvider.CompositeDataProvider+Factory");
					ConfigDataProviderFactory.compositeInstance = (ConfigDataProviderFactory)Activator.CreateInstance(type);
				}
				return ConfigDataProviderFactory.compositeInstance;
			}
			set
			{
				ConfigDataProviderFactory.compositeInstance = value;
			}
		}

		internal static ConfigDataProviderFactory OpticsDefault
		{
			get
			{
				if (ConfigDataProviderFactory.opticsInstance == null)
				{
					if (ConfigDataProviderFactory.environment.IsForefrontForOffice() || ConfigDataProviderFactory.environment.IsForefrontDALOverrideUseSQL())
					{
						Assembly assembly = Assembly.Load("Microsoft.Exchange.Data.StreamingOptics.Apps");
						Type type = assembly.GetType("Microsoft.Exchange.Data.StreamingOptics.Apps.OpticsDataProvider+Factory");
						ConfigDataProviderFactory.opticsInstance = (ConfigDataProviderFactory)Activator.CreateInstance(type);
					}
					else
					{
						ConfigDataProviderFactory.opticsInstance = ConfigDataProviderFactory.CreateFactoryInstanceUsingDALWebService();
					}
				}
				return ConfigDataProviderFactory.opticsInstance;
			}
			set
			{
				ConfigDataProviderFactory.opticsInstance = value;
			}
		}

		internal static ConfigDataProviderFactory KEStoreDefault
		{
			get
			{
				if (ConfigDataProviderFactory.keStoreInstance == null)
				{
					if (ConfigDataProviderFactory.environment.IsForefrontForOffice() || ConfigDataProviderFactory.environment.IsForefrontDALOverrideUseSQL())
					{
						Assembly assembly = Assembly.Load("Microsoft.Exchange.AntiSpam.KEStore.Core");
						Type type = assembly.GetType("Microsoft.Exchange.AntiSpam.KEStore.Core.KEStoreDataProvider+DataProviderFactory");
						ConfigDataProviderFactory.keStoreInstance = (ConfigDataProviderFactory)Activator.CreateInstance(type);
					}
					else
					{
						ConfigDataProviderFactory.keStoreInstance = ConfigDataProviderFactory.CreateFactoryInstanceUsingDALWebService();
					}
				}
				return ConfigDataProviderFactory.keStoreInstance;
			}
			set
			{
				ConfigDataProviderFactory.keStoreInstance = value;
			}
		}

		internal static ConfigDataProviderFactory Default
		{
			get
			{
				if (ConfigDataProviderFactory.defaultInstance == null)
				{
					if (ConfigDataProviderFactory.environment.IsForefrontForOffice() || ConfigDataProviderFactory.environment.IsForefrontDALOverrideUseSQL())
					{
						ConfigDataProviderFactory.defaultInstance = ConfigDataProviderFactory.CreateFactoryInstanceUsingDALSQL();
					}
					else
					{
						ConfigDataProviderFactory.defaultInstance = ConfigDataProviderFactory.CreateFactoryInstanceUsingDALWebService();
					}
				}
				return ConfigDataProviderFactory.defaultInstance;
			}
			set
			{
				ConfigDataProviderFactory.defaultInstance = value;
			}
		}

		internal static IConfigDataProvider CreateDataProvider(DatabaseType store)
		{
			switch (store)
			{
			case DatabaseType.Optics:
				return ConfigDataProviderFactory.OpticsDefault.Create(store);
			case DatabaseType.KEStore:
				return ConfigDataProviderFactory.KEStoreDefault.Create(store);
			default:
				return ConfigDataProviderFactory.Default.Create(store);
			}
		}

		internal abstract IConfigDataProvider Create(DatabaseType store);

		private static ConfigDataProviderFactory CreateFactoryInstanceUsingDALSQL()
		{
			Assembly assembly = Assembly.Load("Microsoft.Exchange.Hygiene.WebStoreDataProvider");
			Type type = assembly.GetType("Microsoft.Exchange.Hygiene.WebStoreDataProvider.WstDataProviderFactory");
			return (ConfigDataProviderFactory)Activator.CreateInstance(type);
		}

		private static ConfigDataProviderFactory CreateFactoryInstanceUsingDALWebService()
		{
			Assembly assembly = Assembly.Load("Microsoft.Exchange.Hygiene.WebserviceDataProvider");
			Type type = assembly.GetType("Microsoft.Exchange.Hygiene.WebserviceDataProvider.WebserviceDataProviderFactory");
			return (ConfigDataProviderFactory)Activator.CreateInstance(type);
		}

		private static ConfigDataProviderFactory cacheInstance;

		private static ConfigDataProviderFactory compositeInstance;

		private static ConfigDataProviderFactory opticsInstance;

		private static ConfigDataProviderFactory keStoreInstance;

		private static ConfigDataProviderFactory defaultInstance;

		private static EnvironmentStrategy environment = new EnvironmentStrategy();
	}
}
