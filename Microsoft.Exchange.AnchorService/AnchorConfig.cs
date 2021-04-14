using System;
using System.Configuration;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ConfigurationSettings;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.AnchorService
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AnchorConfig : ConfigSchemaBase, IDiagnosable
	{
		internal AnchorConfig(string applicationName)
		{
			AnchorConfig <>4__this = this;
			this.applicationName = applicationName;
			this.lazyConfigProvider = new Lazy<IConfigProvider>(() => AnchorConfig.AnchorConfigProvider.CreateProvider(applicationName, <>4__this));
		}

		public override string Name
		{
			get
			{
				return this.applicationName;
			}
		}

		[ConfigurationProperty("IsEnabled", DefaultValue = true)]
		public bool IsEnabled
		{
			get
			{
				return this.InternalGetConfig<bool>("IsEnabled");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "IsEnabled");
			}
		}

		[ConfigurationProperty("LoggingLevel", DefaultValue = MigrationEventType.Information)]
		public MigrationEventType LoggingLevel
		{
			get
			{
				return this.InternalGetConfig<MigrationEventType>("LoggingLevel");
			}
			set
			{
				this.InternalSetConfig<MigrationEventType>(value, "LoggingLevel");
			}
		}

		[ConfigurationProperty("LogFilePath", DefaultValue = null)]
		public string LogFilePath
		{
			get
			{
				return this.InternalGetConfig<string>("LogFilePath");
			}
			set
			{
				this.InternalSetConfig<string>(value, "LogFilePath");
			}
		}

		[ConfigurationProperty("LogMaxAge", DefaultValue = "30.00:00:00")]
		[TimeSpanValidator(MinValueString = "1.00:00:00", MaxValueString = "180.0:00:00", ExcludeRange = false)]
		public TimeSpan LogMaxAge
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("LogMaxAge");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "LogMaxAge");
			}
		}

		[ConfigurationProperty("LogMaxDirectorySize", DefaultValue = "50000000")]
		[LongValidator(MinValue = 0L, MaxValue = 1048576000L, ExcludeRange = false)]
		public long LogMaxDirectorySize
		{
			get
			{
				return this.InternalGetConfig<long>("LogMaxDirectorySize");
			}
			set
			{
				this.InternalSetConfig<long>(value, "LogMaxDirectorySize");
			}
		}

		[LongValidator(MinValue = 0L, MaxValue = 1048576000L, ExcludeRange = false)]
		[ConfigurationProperty("LogMaxFileSize", DefaultValue = "50000000")]
		public long LogMaxFileSize
		{
			get
			{
				return this.InternalGetConfig<long>("LogMaxFileSize");
			}
			set
			{
				this.InternalSetConfig<long>(value, "LogMaxFileSize");
			}
		}

		[ConfigurationProperty("FaultInjectionHandler", DefaultValue = null)]
		public string FaultInjectionHandler
		{
			get
			{
				return this.InternalGetConfig<string>("FaultInjectionHandler");
			}
			set
			{
				this.InternalSetConfig<string>(value, "FaultInjectionHandler");
			}
		}

		[ConfigurationProperty("ShouldWakeOnCacheUpdate", DefaultValue = true)]
		public bool ShouldWakeOnCacheUpdate
		{
			get
			{
				return this.InternalGetConfig<bool>("ShouldWakeOnCacheUpdate");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "ShouldWakeOnCacheUpdate");
			}
		}

		[TimeSpanValidator(MinValueString = "00:00:01", MaxValueString = "180.0:00:00", ExcludeRange = false)]
		[ConfigurationProperty("TransientErrorRunDelay", DefaultValue = "00:05:00")]
		public TimeSpan TransientErrorRunDelay
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("TransientErrorRunDelay");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "TransientErrorRunDelay");
			}
		}

		[TimeSpanValidator(MinValueString = "00:00:01", MaxValueString = "180.0:00:00", ExcludeRange = false)]
		[ConfigurationProperty("ActiveRunDelay", DefaultValue = "00:01:00")]
		public TimeSpan ActiveRunDelay
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("ActiveRunDelay");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "ActiveRunDelay");
			}
		}

		[ConfigurationProperty("IdleRunDelay", DefaultValue = "00:05:00")]
		[TimeSpanValidator(MinValueString = "00:00:01", MaxValueString = "180.0:00:00", ExcludeRange = false)]
		public TimeSpan IdleRunDelay
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("IdleRunDelay");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "IdleRunDelay");
			}
		}

		[TimeSpanValidator(MinValueString = "00:00:00", MaxValueString = "365.0:00:00", ExcludeRange = false)]
		[ConfigurationProperty("ScannerTimeDelay", DefaultValue = "00:05:00")]
		public TimeSpan ScannerTimeDelay
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("ScannerTimeDelay");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "ScannerTimeDelay");
			}
		}

		[ConfigurationProperty("ScannerInitialTimeDelay", DefaultValue = "00:00:00")]
		[TimeSpanValidator(MinValueString = "00:00:00", MaxValueString = "7.0:00:00", ExcludeRange = false)]
		public TimeSpan ScannerInitialTimeDelay
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("ScannerInitialTimeDelay");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "ScannerInitialTimeDelay");
			}
		}

		[ConfigurationProperty("SlowOperationThreshold", DefaultValue = "00:05:00")]
		[TimeSpanValidator(MinValueString = "00:00:01", MaxValueString = "365.0:00:00", ExcludeRange = false)]
		public TimeSpan SlowOperationThreshold
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("SlowOperationThreshold");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "SlowOperationThreshold");
			}
		}

		[ConfigurationProperty("MaximumCacheEntrySchedulerRun", DefaultValue = -1)]
		[IntegerValidator(MinValue = -1, ExcludeRange = false)]
		public int MaximumCacheEntrySchedulerRun
		{
			get
			{
				return this.InternalGetConfig<int>("MaximumCacheEntrySchedulerRun");
			}
			set
			{
				this.InternalSetConfig<int>(value, "MaximumCacheEntrySchedulerRun");
			}
		}

		[IntegerValidator(MinValue = -1, ExcludeRange = false)]
		[ConfigurationProperty("MaximumCacheEntryCountPerOrganization", DefaultValue = 1)]
		public int MaximumCacheEntryCountPerOrganization
		{
			get
			{
				return this.InternalGetConfig<int>("MaximumCacheEntryCountPerOrganization");
			}
			set
			{
				this.InternalSetConfig<int>(value, "MaximumCacheEntryCountPerOrganization");
			}
		}

		[ConfigurationProperty("ScannerClearCacheOnRefresh", DefaultValue = true)]
		public bool ScannerClearCacheOnRefresh
		{
			get
			{
				return this.InternalGetConfig<bool>("ScannerClearCacheOnRefresh");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "ScannerClearCacheOnRefresh");
			}
		}

		[ConfigurationProperty("IssueCacheIsEnabled", DefaultValue = true)]
		public bool IssueCacheIsEnabled
		{
			get
			{
				return this.InternalGetConfig<bool>("IssueCacheIsEnabled");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "IssueCacheIsEnabled");
			}
		}

		[TimeSpanValidator(MinValueString = "00:00:01", ExcludeRange = false)]
		[ConfigurationProperty("IssueCacheScanFrequency", DefaultValue = "00:05:00")]
		public TimeSpan IssueCacheScanFrequency
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("IssueCacheScanFrequency");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "IssueCacheScanFrequency");
			}
		}

		[IntegerValidator(MinValue = 0, ExcludeRange = false)]
		[ConfigurationProperty("IssueCacheItemLimit", DefaultValue = 50)]
		public int IssueCacheItemLimit
		{
			get
			{
				return this.InternalGetConfig<int>("IssueCacheItemLimit");
			}
			set
			{
				this.InternalSetConfig<int>(value, "IssueCacheItemLimit");
			}
		}

		[ConfigurationProperty("MonitoringComponentName", DefaultValue = null)]
		public string MonitoringComponentName
		{
			get
			{
				return this.InternalGetConfig<string>("MonitoringComponentName");
			}
			set
			{
				this.InternalSetConfig<string>(value, "MonitoringComponentName");
			}
		}

		[ConfigurationProperty("CacheEntryPoisonNotificationReason", DefaultValue = "CacheEntryIsPoisoned")]
		public string CacheEntryPoisonNotificationReason
		{
			get
			{
				return this.InternalGetConfig<string>("CacheEntryPoisonNotificationReason");
			}
			set
			{
				this.InternalSetConfig<string>(value, "CacheEntryPoisonNotificationReason");
			}
		}

		[ConfigurationProperty("CacheEntryPoisonThreshold", DefaultValue = 10)]
		public int CacheEntryPoisonThreshold
		{
			get
			{
				return this.InternalGetConfig<int>("CacheEntryPoisonThreshold");
			}
			set
			{
				this.InternalSetConfig<int>(value, "CacheEntryPoisonThreshold");
			}
		}

		[ConfigurationProperty("UseWatson", DefaultValue = true)]
		public bool UseWatson
		{
			get
			{
				return this.InternalGetConfig<bool>("UseWatson");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "UseWatson");
			}
		}

		private IConfigProvider Provider
		{
			get
			{
				return this.lazyConfigProvider.Value;
			}
		}

		public T GetConfig<T>([CallerMemberName] string settingName = null)
		{
			AnchorUtil.ThrowOnNullOrEmptyArgument(settingName, "settingName");
			return this.Provider.GetConfig<T>(settingName);
		}

		public T GetConfig<T>(ISettingsContext context, string settingName)
		{
			return this.Provider.GetConfig<T>(context, settingName);
		}

		string IDiagnosable.GetDiagnosticComponentName()
		{
			return string.Format("{0}_{1}", this.Name, this.Provider.GetDiagnosticComponentName());
		}

		XElement IDiagnosable.GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			return this.Provider.GetDiagnosticInfo(parameters);
		}

		internal void UpdateConfig<T>(string settingName, T value)
		{
			base.SetDefaultConfigValue<T>(base.GetConfigurationProperty(settingName, null), value);
		}

		private readonly Lazy<IConfigProvider> lazyConfigProvider;

		private readonly string applicationName;

		public static class Setting
		{
			public const string IsEnabled = "IsEnabled";

			public const string LoggingLevel = "LoggingLevel";

			public const string LogFilePath = "LogFilePath";

			public const string LogMaxAge = "LogMaxAge";

			public const string LogMaxDirectorySize = "LogMaxDirectorySize";

			public const string LogMaxFileSize = "LogMaxFileSize";

			public const string FaultInjectionHandler = "FaultInjectionHandler";

			public const string ShouldWakeOnCacheUpdate = "ShouldWakeOnCacheUpdate";

			public const string TransientErrorRunDelay = "TransientErrorRunDelay";

			public const string ActiveRunDelay = "ActiveRunDelay";

			public const string IdleRunDelay = "IdleRunDelay";

			public const string ScannerTimeDelay = "ScannerTimeDelay";

			public const string ScannerInitialTimeDelay = "ScannerInitialTimeDelay";

			public const string SlowOperationThreshold = "SlowOperationThreshold";

			public const string MaximumCacheEntrySchedulerRun = "MaximumCacheEntrySchedulerRun";

			public const string MaximumCacheEntryCountPerOrganization = "MaximumCacheEntryCountPerOrganization";

			public const string ScannerClearCacheOnRefresh = "ScannerClearCacheOnRefresh";

			public const string IssueCacheIsEnabled = "IssueCacheIsEnabled";

			public const string IssueCacheScanFrequency = "IssueCacheScanFrequency";

			public const string IssueCacheItemLimit = "IssueCacheItemLimit";

			public const string MonitoringComponentName = "MonitoringComponentName";

			public const string CacheEntryPoisonNotificationReason = "CacheEntryPoisonNotificationReason";

			public const string CacheEntryPoisonThreshold = "CacheEntryPoisonThreshold";

			public const string UseWatson = "UseWatson";
		}

		private class AnchorConfigProvider : ConfigProvider
		{
			private AnchorConfigProvider(ConfigSchemaBase schema) : base(schema)
			{
			}

			public static IConfigProvider CreateProvider(string applicationName, AnchorConfig schema)
			{
				ConfigDriverBase configDriverBase = null;
				ConfigDriverBase configDriverBase2 = null;
				ConfigFlags overrideFlags = ConfigProviderBase.OverrideFlags;
				if ((overrideFlags & ConfigFlags.DisallowADConfig) != ConfigFlags.DisallowADConfig)
				{
					configDriverBase = new ADConfigDriver(schema);
				}
				if ((overrideFlags & ConfigFlags.DisallowAppConfig) != ConfigFlags.DisallowAppConfig)
				{
					configDriverBase2 = new AnchorConfig.AnchorAppConfigDriver(schema.Name, schema);
				}
				AnchorConfig.AnchorConfigProvider anchorConfigProvider = new AnchorConfig.AnchorConfigProvider(schema);
				if (configDriverBase != null && configDriverBase2 != null && (overrideFlags & ConfigFlags.LowADConfigPriority) == ConfigFlags.LowADConfigPriority)
				{
					anchorConfigProvider.AddConfigDriver(configDriverBase2);
					anchorConfigProvider.AddConfigDriver(configDriverBase);
				}
				else
				{
					if (configDriverBase != null)
					{
						anchorConfigProvider.AddConfigDriver(configDriverBase);
					}
					if (configDriverBase2 != null)
					{
						anchorConfigProvider.AddConfigDriver(configDriverBase2);
					}
				}
				return anchorConfigProvider;
			}
		}

		private class AnchorAppConfigDriver : AppConfigDriver
		{
			public AnchorAppConfigDriver(string applicationName, IConfigSchema schema) : base(schema, new TimeSpan?(ConfigDriverBase.DefaultErrorThresholdInterval))
			{
				this.applicationName = applicationName;
			}

			public override bool TryGetBoxedSetting(ISettingsContext context, string settingName, Type settingType, out object settingValue)
			{
				settingValue = null;
				AppSettingsSection appSettingsSection = base.Section as AppSettingsSection;
				if (appSettingsSection == null)
				{
					return false;
				}
				string key = string.Format("{0}_{1}", this.applicationName, settingName);
				KeyValueConfigurationElement keyValueConfigurationElement = appSettingsSection.Settings[key];
				if (keyValueConfigurationElement != null)
				{
					settingValue = base.ParseAndValidateConfigValue(settingName, keyValueConfigurationElement.Value, settingType);
					return true;
				}
				return false;
			}

			protected override DisposeTracker InternalGetDisposeTracker()
			{
				return DisposeTracker.Get<AnchorConfig.AnchorAppConfigDriver>(this);
			}

			private readonly string applicationName;
		}
	}
}
