using System;
using System.Configuration;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Band;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.MailboxLoadBalance.Config
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class LoadBalanceADSettings : AnchorConfig, ILoadBalanceSettings
	{
		public LoadBalanceADSettings() : base("MailboxLoadBalance")
		{
		}

		[ConfigurationProperty("AutomaticDatabaseDrainEnabled", DefaultValue = true)]
		public bool AutomaticDatabaseDrainEnabled
		{
			get
			{
				return base.GetConfig<bool>("AutomaticDatabaseDrainEnabled");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "AutomaticDatabaseDrainEnabled");
			}
		}

		[IntegerValidator(ExcludeRange = false, MinValue = 0, MaxValue = 100)]
		[ConfigurationProperty("AutomaticDrainStartFileSizePercent", DefaultValue = 20)]
		public int AutomaticDrainStartFileSizePercent
		{
			get
			{
				return base.GetConfig<int>("AutomaticDrainStartFileSizePercent");
			}
			set
			{
				this.InternalSetConfig<int>(value, "AutomaticDrainStartFileSizePercent");
			}
		}

		[ConfigurationProperty("AutomaticLoadBalancingEnabled", DefaultValue = true)]
		public bool AutomaticLoadBalancingEnabled
		{
			get
			{
				return base.GetConfig<bool>("AutomaticLoadBalancingEnabled");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "AutomaticLoadBalancingEnabled");
			}
		}

		[ConfigurationProperty("BatchBatchSizeReducer", DefaultValue = "FactorBased")]
		public BatchSizeReducerType BatchBatchSizeReducer
		{
			get
			{
				return base.GetConfig<BatchSizeReducerType>("BatchBatchSizeReducer");
			}
			set
			{
				this.InternalSetConfig<BatchSizeReducerType>(value, "BatchBatchSizeReducer");
			}
		}

		[ConfigurationProperty("BuildLocalCacheOnStartup", DefaultValue = true)]
		public bool BuildLocalCacheOnStartup
		{
			get
			{
				return base.GetConfig<bool>("BuildLocalCacheOnStartup");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "BuildLocalCacheOnStartup");
			}
		}

		[IntegerValidator(ExcludeRange = false, MinValue = 0, MaxValue = 2147483647)]
		[ConfigurationProperty("CapacityGrowthPeriods", DefaultValue = 6)]
		public int CapacityGrowthPeriods
		{
			get
			{
				return base.GetConfig<int>("CapacityGrowthPeriods");
			}
			set
			{
				this.InternalSetConfig<int>(value, "CapacityGrowthPeriods");
			}
		}

		[TimeSpanValidator(MinValueString = "00:00:00", MaxValueString = "00:30:00", ExcludeRange = false)]
		[ConfigurationProperty("ClientCacheTimeToLive", DefaultValue = "00:00:00")]
		public TimeSpan ClientCacheTimeToLive
		{
			get
			{
				return base.GetConfig<TimeSpan>("ClientCacheTimeToLive");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "ClientCacheTimeToLive");
			}
		}

		[ConfigurationProperty("ConsumerGrowthRate", DefaultValue = 3)]
		[IntegerValidator(ExcludeRange = false, MinValue = 0, MaxValue = 100)]
		public int ConsumerGrowthRate
		{
			get
			{
				return base.GetConfig<int>("ConsumerGrowthRate");
			}
			set
			{
				this.InternalSetConfig<int>(value, "ConsumerGrowthRate");
			}
		}

		[LongValidator(ExcludeRange = false, MinValue = 0L, MaxValue = 2147483647L)]
		[ConfigurationProperty("DefaultDatabaseMaxSizeGb", DefaultValue = 500L)]
		public long DefaultDatabaseMaxSizeGb
		{
			get
			{
				return base.GetConfig<long>("DefaultDatabaseMaxSizeGb");
			}
			set
			{
				this.InternalSetConfig<long>(value, "DefaultDatabaseMaxSizeGb");
			}
		}

		[IntegerValidator(ExcludeRange = false, MinValue = 0, MaxValue = 2147483647)]
		[ConfigurationProperty("DefaultDatabaseRelativeLoadCapacity", DefaultValue = 180)]
		public int DefaultDatabaseRelativeLoadCapacity
		{
			get
			{
				return base.GetConfig<int>("DefaultDatabaseRelativeLoadCapacity");
			}
			set
			{
				this.InternalSetConfig<int>(value, "DefaultDatabaseRelativeLoadCapacity");
			}
		}

		[ConfigurationProperty("DisableWlm", DefaultValue = false)]
		public bool DisableWlm
		{
			get
			{
				return base.GetConfig<bool>("DisableWlm");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "DisableWlm");
			}
		}

		[ConfigurationProperty("DontCreateMoveRequests", DefaultValue = false)]
		public bool DontCreateMoveRequests
		{
			get
			{
				return base.GetConfig<bool>("DontCreateMoveRequests");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "DontCreateMoveRequests");
			}
		}

		[ConfigurationProperty("DontRemoveSoftDeletedMailboxes", DefaultValue = false)]
		public bool DontRemoveSoftDeletedMailboxes
		{
			get
			{
				return base.GetConfig<bool>("DontRemoveSoftDeletedMailboxes");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "DontRemoveSoftDeletedMailboxes");
			}
		}

		[ConfigurationProperty("ExcludedMailboxProcessors", DefaultValue = "")]
		public string ExcludedMailboxProcessors
		{
			get
			{
				return base.GetConfig<string>("ExcludedMailboxProcessors");
			}
			set
			{
				this.InternalSetConfig<string>(value, "ExcludedMailboxProcessors");
			}
		}

		[IntegerValidator(ExcludeRange = false, MinValue = 0, MaxValue = 2147483647)]
		[ConfigurationProperty("InjectionBatchSize", DefaultValue = 500)]
		public int InjectionBatchSize
		{
			get
			{
				return base.GetConfig<int>("InjectionBatchSize");
			}
			set
			{
				this.InternalSetConfig<int>(value, "InjectionBatchSize");
			}
		}

		[ConfigurationProperty("IsEnabled", DefaultValue = false)]
		public new bool IsEnabled
		{
			get
			{
				return base.GetConfig<bool>("IsEnabled") && !this.LoadBalanceBlocked;
			}
			set
			{
				this.InternalSetConfig<bool>(value, "IsEnabled");
			}
		}

		public bool LoadBalanceBlocked
		{
			get
			{
				return !VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Mrs.AutomaticMailboxLoadBalancing.Enabled;
			}
		}

		[TimeSpanValidator]
		[ConfigurationProperty("LocalCacheRefreshPeriod", DefaultValue = "06:00:00")]
		public TimeSpan LocalCacheRefreshPeriod
		{
			get
			{
				return base.GetConfig<TimeSpan>("LocalCacheRefreshPeriod");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "LocalCacheRefreshPeriod");
			}
		}

		[ConfigurationProperty("MailboxProcessorsEnabled", DefaultValue = true)]
		public bool MailboxProcessorsEnabled
		{
			get
			{
				return base.GetConfig<bool>("MailboxProcessorsEnabled");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "MailboxProcessorsEnabled");
			}
		}

		[ConfigurationProperty("MaxDatabaseDiskUtilizationPercent", DefaultValue = 15)]
		[IntegerValidator(ExcludeRange = false, MinValue = 0, MaxValue = 100)]
		public int MaxDatabaseDiskUtilizationPercent
		{
			get
			{
				return base.GetConfig<int>("MaxDatabaseDiskUtilizationPercent");
			}
			set
			{
				this.InternalSetConfig<int>(value, "MaxDatabaseDiskUtilizationPercent");
			}
		}

		[LongValidator(ExcludeRange = false, MinValue = 0L, MaxValue = 2147483647L)]
		[ConfigurationProperty("MaximumAmountOfDataPerRoundGb", DefaultValue = 450L)]
		public long MaximumAmountOfDataPerRoundGb
		{
			get
			{
				return base.GetConfig<long>("MaximumAmountOfDataPerRoundGb");
			}
			set
			{
				this.InternalSetConfig<long>(value, "MaximumAmountOfDataPerRoundGb");
			}
		}

		[ConfigurationProperty("MaximumBatchSizeGb", DefaultValue = 5120L)]
		[LongValidator(ExcludeRange = false, MinValue = 0L, MaxValue = 2147483647L)]
		public long MaximumBatchSizeGb
		{
			get
			{
				return base.GetConfig<long>("MaximumBatchSizeGb");
			}
			set
			{
				this.InternalSetConfig<long>(value, "MaximumBatchSizeGb");
			}
		}

		[ConfigurationProperty("MaximumConsumerMailboxSizePercent", DefaultValue = 21)]
		[IntegerValidator(ExcludeRange = false, MinValue = 0, MaxValue = 100)]
		public int MaximumConsumerMailboxSizePercent
		{
			get
			{
				return base.GetConfig<int>("MaximumConsumerMailboxSizePercent");
			}
			set
			{
				this.InternalSetConfig<int>(value, "MaximumConsumerMailboxSizePercent");
			}
		}

		[ConfigurationProperty("MaximumNumberOfRunspaces", DefaultValue = 5)]
		[IntegerValidator(ExcludeRange = false, MinValue = 0, MaxValue = 100)]
		public int MaximumNumberOfRunspaces
		{
			get
			{
				return base.GetConfig<int>("MaximumNumberOfRunspaces");
			}
			set
			{
				this.InternalSetConfig<int>(value, "MaximumNumberOfRunspaces");
			}
		}

		[LongValidator(ExcludeRange = false, MinValue = 0L, MaxValue = 2147483647L)]
		[ConfigurationProperty("MaximumPendingMoveCount", DefaultValue = 1000L)]
		public long MaximumPendingMoveCount
		{
			get
			{
				return (long)base.GetConfig<int>("MaximumPendingMoveCount");
			}
			set
			{
				this.InternalSetConfig<long>(value, "MaximumPendingMoveCount");
			}
		}

		[ConfigurationProperty("MinimumSoftDeletedMailboxCleanupAge", DefaultValue = "7.00:00:00")]
		[TimeSpanValidator(MinValueString = "7.00:00:00", MaxValueString = "365.00:00:00", ExcludeRange = false)]
		public TimeSpan MinimumSoftDeletedMailboxCleanupAge
		{
			get
			{
				return base.GetConfig<TimeSpan>("MinimumSoftDeletedMailboxCleanupAge");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "MinimumSoftDeletedMailboxCleanupAge");
			}
		}

		[ConfigurationProperty("NonMovableOrganizationIds")]
		public string NonMovableOrganizationIds
		{
			get
			{
				return base.GetConfig<string>("NonMovableOrganizationIds") ?? string.Empty;
			}
			set
			{
				this.InternalSetConfig<string>(value, "NonMovableOrganizationIds");
			}
		}

		[ConfigurationProperty("OrganizationGrowthRate", DefaultValue = 9)]
		[IntegerValidator(ExcludeRange = false, MinValue = 0, MaxValue = 100)]
		public int OrganizationGrowthRate
		{
			get
			{
				return base.GetConfig<int>("OrganizationGrowthRate");
			}
			set
			{
				this.InternalSetConfig<int>(value, "OrganizationGrowthRate");
			}
		}

		[ConfigurationProperty("QueryBufferPeriodDays", DefaultValue = 10)]
		[IntegerValidator(ExcludeRange = false, MinValue = 1, MaxValue = 2147483647)]
		public int QueryBufferPeriodDays
		{
			get
			{
				return base.GetConfig<int>("QueryBufferPeriodDays");
			}
			set
			{
				this.InternalSetConfig<int>(value, "QueryBufferPeriodDays");
			}
		}

		[ConfigurationProperty("ReservedCapacityInGb", DefaultValue = 1024)]
		[IntegerValidator(ExcludeRange = false, MinValue = 0, MaxValue = 2147483647)]
		public int ReservedCapacityInGb
		{
			get
			{
				return base.GetConfig<int>("ReservedCapacityInGb");
			}
			set
			{
				this.InternalSetConfig<int>(value, "ReservedCapacityInGb");
			}
		}

		[ConfigurationProperty("SoftDeletedCleanupEnabled", DefaultValue = false)]
		public bool SoftDeletedCleanupEnabled
		{
			get
			{
				return base.GetConfig<bool>("SoftDeletedCleanupEnabled");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "SoftDeletedCleanupEnabled");
			}
		}

		[ConfigurationProperty("SoftDeletedCleanupThreshold", DefaultValue = 80)]
		[IntegerValidator(ExcludeRange = false, MinValue = 0, MaxValue = 100)]
		public int SoftDeletedCleanupThreshold
		{
			get
			{
				return base.GetConfig<int>("SoftDeletedCleanupThreshold");
			}
			set
			{
				this.InternalSetConfig<int>(value, "SoftDeletedCleanupThreshold");
			}
		}

		[ConfigurationProperty("TransientFailureMaxRetryCount", DefaultValue = 5)]
		[IntegerValidator(ExcludeRange = false, MinValue = 0, MaxValue = 2147483647)]
		public int TransientFailureMaxRetryCount
		{
			get
			{
				return base.GetConfig<int>("TransientFailureMaxRetryCount");
			}
			set
			{
				this.InternalSetConfig<int>(value, "TransientFailureMaxRetryCount");
			}
		}

		[ConfigurationProperty("TransientFailureRetryDelay", DefaultValue = "00:00:30")]
		[TimeSpanValidator(MinValueString = "00:00:00", MaxValueString = "00:30:00", ExcludeRange = false)]
		public TimeSpan TransientFailureRetryDelay
		{
			get
			{
				return base.GetConfig<TimeSpan>("TransientFailureRetryDelay");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "TransientFailureRetryDelay");
			}
		}

		[ConfigurationProperty("UseCachingActiveManager", DefaultValue = true)]
		public bool UseCachingActiveManager
		{
			get
			{
				return base.GetConfig<bool>("UseCachingActiveManager");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "UseCachingActiveManager");
			}
		}

		[ConfigurationProperty("UseDatabaseSelectorForMoveInjection", DefaultValue = true)]
		public bool UseDatabaseSelectorForMoveInjection
		{
			get
			{
				return base.GetConfig<bool>("UseDatabaseSelectorForMoveInjection");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "UseDatabaseSelectorForMoveInjection");
			}
		}

		[ConfigurationProperty("UseHeatMapProvisioning", DefaultValue = false)]
		public bool UseHeatMapProvisioning
		{
			get
			{
				return base.GetConfig<bool>("UseHeatMapProvisioning");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "UseHeatMapProvisioning");
			}
		}

		[ConfigurationProperty("UseParallelDiscovery", DefaultValue = true)]
		public bool UseParallelDiscovery
		{
			get
			{
				return base.GetConfig<bool>("UseParallelDiscovery");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "UseParallelDiscovery");
			}
		}

		[IntegerValidator(ExcludeRange = false, MinValue = 0, MaxValue = 100)]
		[ConfigurationProperty("WeightDeviationPercent", DefaultValue = 2)]
		public int WeightDeviationPercent
		{
			get
			{
				return base.GetConfig<int>("WeightDeviationPercent");
			}
			set
			{
				this.InternalSetConfig<int>(value, "WeightDeviationPercent");
			}
		}

		[TimeSpanValidator(MinValueString = "00:00:00", MaxValueString = "365.00:00:00", ExcludeRange = false)]
		[ConfigurationProperty("MinimumTimeInDatabaseForItemUpgrade", DefaultValue = "14.00:00:00")]
		public TimeSpan MinimumTimeInDatabaseForItemUpgrade
		{
			get
			{
				return base.GetConfig<TimeSpan>("MinimumTimeInDatabaseForItemUpgrade");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "MinimumTimeInDatabaseForItemUpgrade");
			}
		}

		[ConfigurationProperty("DisabledMailboxPolicies", DefaultValue = "")]
		public string DisabledMailboxPolicies
		{
			get
			{
				return base.GetConfig<string>("DisabledMailboxPolicies");
			}
			set
			{
				this.InternalSetConfig<string>(value, "DisabledMailboxPolicies");
			}
		}

		TimeSpan ILoadBalanceSettings.get_IdleRunDelay()
		{
			return base.IdleRunDelay;
		}

		string ILoadBalanceSettings.get_LogFilePath()
		{
			return base.LogFilePath;
		}

		long ILoadBalanceSettings.get_LogMaxDirectorySize()
		{
			return base.LogMaxDirectorySize;
		}

		long ILoadBalanceSettings.get_LogMaxFileSize()
		{
			return base.LogMaxFileSize;
		}

		public static readonly LoadBalanceADSettings DefaultContext = new LoadBalanceADSettings();

		public static readonly Hookable<ILoadBalanceSettings> Instance = Hookable<ILoadBalanceSettings>.Create(true, LoadBalanceADSettings.DefaultContext);
	}
}
