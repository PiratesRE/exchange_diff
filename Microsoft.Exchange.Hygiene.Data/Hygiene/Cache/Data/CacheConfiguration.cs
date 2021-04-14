using System;
using System.Configuration;

namespace Microsoft.Exchange.Hygiene.Cache.Data
{
	internal class CacheConfiguration : ConfigurationSection
	{
		public static string SectionName
		{
			get
			{
				return "CacheConfiguration";
			}
		}

		public static CacheConfiguration Instance
		{
			get
			{
				return CacheConfiguration.instance;
			}
			internal set
			{
				CacheConfiguration.instance = value;
			}
		}

		[ConfigurationProperty("MaxRetryCount", IsRequired = false, DefaultValue = 10)]
		public int MaxRetryCount
		{
			get
			{
				return (int)base["MaxRetryCount"];
			}
			set
			{
				base["MaxRetryCount"] = value;
			}
		}

		[ConfigurationProperty("RetrySleepInterval", IsRequired = false, DefaultValue = "00:00:00.250")]
		public TimeSpan RetrySleepInterval
		{
			get
			{
				return (TimeSpan)base["RetrySleepInterval"];
			}
			set
			{
				base["RetrySleepInterval"] = value;
			}
		}

		[ConfigurationProperty("SlowResponseTime", IsRequired = false, DefaultValue = "00:00:03")]
		public TimeSpan SlowResponseTime
		{
			get
			{
				return (TimeSpan)base["SlowResponseTime"];
			}
			set
			{
				base["SlowResponseTime"] = value;
			}
		}

		[ConfigurationProperty("WatchdogTimeout", IsRequired = false, DefaultValue = "00:00:05")]
		public TimeSpan WatchdogTimeout
		{
			get
			{
				return (TimeSpan)base["WatchdogTimeout"];
			}
			set
			{
				base["WatchdogTimeout"] = value;
			}
		}

		[ConfigurationProperty("HealthyToStaleThreshold", IsRequired = false, DefaultValue = "00:05:00")]
		public TimeSpan HealthyToStaleThreshold
		{
			get
			{
				return (TimeSpan)base["HealthyToStaleThreshold"];
			}
			set
			{
				base["HealthyToStaleThreshold"] = value;
			}
		}

		[ConfigurationProperty("StaleToUnhealthyThreshold", IsRequired = false, DefaultValue = "00:20:00")]
		public TimeSpan StaleToUnhealthyThreshold
		{
			get
			{
				return (TimeSpan)base["StaleToUnhealthyThreshold"];
			}
			set
			{
				base["StaleToUnhealthyThreshold"] = value;
			}
		}

		[ConfigurationProperty("FailoverModeForPrimingState", IsRequired = false, DefaultValue = CacheFailoverMode.CacheThenDatabase)]
		public CacheFailoverMode FailoverModeForPrimingState
		{
			get
			{
				return (CacheFailoverMode)base["FailoverModeForPrimingState"];
			}
			set
			{
				base["FailoverModeForPrimingState"] = value;
			}
		}

		[ConfigurationProperty("FailoverModeForStaleState", IsRequired = false, DefaultValue = CacheFailoverMode.CacheThenDatabase)]
		public CacheFailoverMode FailoverModeForStaleState
		{
			get
			{
				return (CacheFailoverMode)base["FailoverModeForStaleState"];
			}
			set
			{
				base["FailoverModeForStaleState"] = value;
			}
		}

		[ConfigurationProperty("FailoverModeForUnhealthyState", IsRequired = false, DefaultValue = CacheFailoverMode.CacheThenDatabase)]
		public CacheFailoverMode FailoverModeForUnhealthyState
		{
			get
			{
				return (CacheFailoverMode)base["FailoverModeForUnhealthyState"];
			}
			set
			{
				base["FailoverModeForUnhealthyState"] = value;
			}
		}

		[ConfigurationProperty("FailoverModeForHealthyState", IsRequired = false, DefaultValue = CacheFailoverMode.CacheThenDatabase)]
		public CacheFailoverMode FailoverModeForHealthyState
		{
			get
			{
				return (CacheFailoverMode)base["FailoverModeForHealthyState"];
			}
			set
			{
				base["FailoverModeForHealthyState"] = value;
			}
		}

		[ConfigurationProperty("FailoverModeForPrimingWithFileState", IsRequired = false, DefaultValue = CacheFailoverMode.DatabaseOnly)]
		public CacheFailoverMode FailoverModeForPrimingWithFileState
		{
			get
			{
				return (CacheFailoverMode)base["FailoverModeForPrimingWithFileState"];
			}
			set
			{
				base["FailoverModeForPrimingWithFileState"] = value;
			}
		}

		[ConfigurationProperty("FailoverToCacheOnPermanentDALException", IsRequired = false, DefaultValue = true)]
		public bool FailoverToCacheOnPermanentDALException
		{
			get
			{
				return (bool)base["FailoverToCacheOnPermanentDALException"];
			}
			set
			{
				base["FailoverToCacheOnPermanentDALException"] = value;
			}
		}

		[ConfigurationProperty("FailoverToCacheOnTransientDALException", IsRequired = false, DefaultValue = true)]
		public bool FailoverToCacheOnTransientDALException
		{
			get
			{
				return (bool)base["FailoverToCacheOnTransientDALException"];
			}
			set
			{
				base["FailoverToCacheOnTransientDALException"] = value;
			}
		}

		[ConfigurationProperty("CachePrimingInfoThreadInterval", IsRequired = false, DefaultValue = "00:00:01")]
		public TimeSpan CachePrimingInfoThreadInterval
		{
			get
			{
				return (TimeSpan)base["CachePrimingInfoThreadInterval"];
			}
			set
			{
				base["CachePrimingInfoThreadInterval"] = value;
			}
		}

		[ConfigurationProperty("CachePrimingInfoThreadExpiration", IsRequired = false, DefaultValue = "00:00:10")]
		public TimeSpan CachePrimingInfoThreadExpiration
		{
			get
			{
				return (TimeSpan)base["CachePrimingInfoThreadExpiration"];
			}
			set
			{
				base["CachePrimingInfoThreadExpiration"] = value;
			}
		}

		[ConfigurationProperty("TracerTokensEnabled", IsRequired = false, DefaultValue = false)]
		public bool TracerTokensEnabled
		{
			get
			{
				return (bool)base["TracerTokensEnabled"];
			}
			set
			{
				base["TracerTokensEnabled"] = value;
			}
		}

		[ConfigurationProperty("TracerTokenAgeThreshold", IsRequired = false, DefaultValue = "00:02:00")]
		public TimeSpan TracerTokenAgeThreshold
		{
			get
			{
				return (TimeSpan)base["TracerTokenAgeThreshold"];
			}
			set
			{
				base["TracerTokenAgeThreshold"] = value;
			}
		}

		[ConfigurationProperty("NamedRegionEnabled", IsRequired = false, DefaultValue = false)]
		public bool NamedRegionEnabled
		{
			get
			{
				return (bool)base["NamedRegionEnabled"];
			}
			set
			{
				base["NamedRegionEnabled"] = value;
			}
		}

		[ConfigurationProperty("StatisticBasedFailoverEnabled", IsRequired = false, DefaultValue = false)]
		public bool StatisticBasedFailoverEnabled
		{
			get
			{
				return (bool)base["StatisticBasedFailoverEnabled"];
			}
			set
			{
				base["StatisticBasedFailoverEnabled"] = value;
			}
		}

		[LongValidator(MinValue = 0L)]
		[ConfigurationProperty("MinimumLogicalOperationThreshold", IsRequired = false, DefaultValue = 100L)]
		public long MinimumLogicalOperationThreshold
		{
			get
			{
				return (long)base["MinimumLogicalOperationThreshold"];
			}
			set
			{
				base["MinimumLogicalOperationThreshold"] = value;
			}
		}

		[ConfigurationProperty("StatisticsSlidingWindowDuration", IsRequired = false, DefaultValue = "00:15:00")]
		public TimeSpan StatisticsSlidingWindowDuration
		{
			get
			{
				return (TimeSpan)base["StatisticsSlidingWindowDuration"];
			}
			set
			{
				base["StatisticsSlidingWindowDuration"] = value;
			}
		}

		[ConfigurationProperty("StatisticsBucketDuration", IsRequired = false, DefaultValue = "00:01:00")]
		public TimeSpan StatisticsBucketDuration
		{
			get
			{
				return (TimeSpan)base["StatisticsBucketDuration"];
			}
			set
			{
				base["StatisticsBucketDuration"] = value;
			}
		}

		[ConfigurationProperty("StatisticsRefreshInterval", IsRequired = false, DefaultValue = "00:00:30")]
		public TimeSpan StatisticsRefreshInterval
		{
			get
			{
				return (TimeSpan)base["StatisticsRefreshInterval"];
			}
			set
			{
				base["StatisticsRefreshInterval"] = value;
			}
		}

		[LongValidator(MinValue = 0L, MaxValue = 100L)]
		[ConfigurationProperty("SlowResponsePercentageFailoverThreshold", IsRequired = false, DefaultValue = 50L)]
		public long SlowResponsePercentageFailoverThreshold
		{
			get
			{
				return (long)base["SlowResponsePercentageFailoverThreshold"];
			}
			set
			{
				base["SlowResponsePercentageFailoverThreshold"] = value;
			}
		}

		[LongValidator(MinValue = 0L, MaxValue = 100L)]
		[ConfigurationProperty("LogicalOperationFailurePercentageFailoverThreshold", IsRequired = false, DefaultValue = 5L)]
		public long LogicalOperationFailurePercentageFailoverThreshold
		{
			get
			{
				return (long)base["LogicalOperationFailurePercentageFailoverThreshold"];
			}
			set
			{
				base["LogicalOperationFailurePercentageFailoverThreshold"] = value;
			}
		}

		[LongValidator(MinValue = 0L, MaxValue = 100L)]
		[ConfigurationProperty("PhysicalOperationFailurePercentageFailoverThreshold", IsRequired = false, DefaultValue = 25L)]
		public long PhysicalOperationFailurePercentageFailoverThreshold
		{
			get
			{
				return (long)base["PhysicalOperationFailurePercentageFailoverThreshold"];
			}
			set
			{
				base["PhysicalOperationFailurePercentageFailoverThreshold"] = value;
			}
		}

		[ConfigurationProperty("NotificationsEnabled", IsRequired = false, DefaultValue = false)]
		public bool NotificationsEnabled
		{
			get
			{
				return (bool)base["NotificationsEnabled"];
			}
			set
			{
				base["NotificationsEnabled"] = value;
			}
		}

		[ConfigurationProperty("BloomFilterMode", IsRequired = false, DefaultValue = BloomFilterMode.Disabled)]
		public BloomFilterMode BloomFilterMode
		{
			get
			{
				return (BloomFilterMode)base["BloomFilterMode"];
			}
			set
			{
				base["BloomFilterMode"] = value;
			}
		}

		[ConfigurationProperty("BloomFilterUpdateFrequency", IsRequired = false, DefaultValue = "00:05:00")]
		public TimeSpan BloomFilterUpdateFrequency
		{
			get
			{
				return (TimeSpan)base["BloomFilterUpdateFrequency"];
			}
			set
			{
				base["BloomFilterUpdateFrequency"] = value;
			}
		}

		[ConfigurationProperty("TenantConfigurationCacheMode", IsRequired = false, DefaultValue = TenantConfigurationCacheMode.Disabled)]
		public TenantConfigurationCacheMode TenantConfigurationCacheMode
		{
			get
			{
				return (TenantConfigurationCacheMode)base["TenantConfigurationCacheMode"];
			}
			set
			{
				base["TenantConfigurationCacheMode"] = value;
			}
		}

		[ConfigurationProperty("EntityCacheConfiguration", IsRequired = false)]
		public EntityCacheConfigurationCollection EntityCacheConfigurations
		{
			get
			{
				return (EntityCacheConfigurationCollection)base["EntityCacheConfiguration"];
			}
			set
			{
				base["EntityCacheConfiguration"] = value;
			}
		}

		[ConfigurationProperty("VelocityDisabled", IsRequired = false, DefaultValue = false)]
		public bool VelocityDisabled
		{
			get
			{
				return (bool)base["VelocityDisabled"];
			}
			set
			{
				base["VelocityDisabled"] = value;
			}
		}

		[ConfigurationProperty("BloomFilterTracerTokenGranularity", IsRequired = false, DefaultValue = 15)]
		public int BloomFilterTracerTokenGranularity
		{
			get
			{
				return (int)base["BloomFilterTracerTokenGranularity"];
			}
			set
			{
				base["BloomFilterTracerTokenGranularity"] = value;
			}
		}

		public bool BloomFilterTracerTokensEnabled
		{
			get
			{
				return (int)base["BloomFilterTracerTokenGranularity"] > 0;
			}
		}

		public string GetBloomFilterTracerToken(string entityName, DateTime referenceDateTime)
		{
			int bloomFilterTracerTokenGranularity = this.BloomFilterTracerTokenGranularity;
			return string.Format("{0}-{1:yyyyMMddHH}{2:D2}", entityName, referenceDateTime, (int)(Math.Floor((double)referenceDateTime.Minute / (double)bloomFilterTracerTokenGranularity) * (double)bloomFilterTracerTokenGranularity));
		}

		private const string MaxRetryCountKey = "MaxRetryCount";

		private const string RetrySleepIntervalKey = "RetrySleepInterval";

		private const string SlowResponseTimeKey = "SlowResponseTime";

		private const string WatchdogTimeoutKey = "WatchdogTimeout";

		private const string HealthyToStaleThresholdKey = "HealthyToStaleThreshold";

		private const string StaleToUnhealthyThresholdKey = "StaleToUnhealthyThreshold";

		private const string FailoverModeForPrimingStateKey = "FailoverModeForPrimingState";

		private const string FailoverModeForStaleStateKey = "FailoverModeForStaleState";

		private const string FailoverModeForUnhealthyStateKey = "FailoverModeForUnhealthyState";

		private const string FailoverModeForHealthyStateKey = "FailoverModeForHealthyState";

		private const string FailoverModeForPrimingWithFileStateKey = "FailoverModeForPrimingWithFileState";

		private const string FailoverToCacheOnPermanentDALExceptionKey = "FailoverToCacheOnPermanentDALException";

		private const string FailoverToCacheOnTransientDALExceptionKey = "FailoverToCacheOnTransientDALException";

		private const string CachePrimingInfoThreadIntervalKey = "CachePrimingInfoThreadInterval";

		private const string CachePrimingInfoThreadExpirationKey = "CachePrimingInfoThreadExpiration";

		private const string TracerTokensEnabledKey = "TracerTokensEnabled";

		private const string TracerTokenAgeThresholdKey = "TracerTokenAgeThreshold";

		private const string NamedRegionEnabledKey = "NamedRegionEnabled";

		private const string StatisticBasedFailoverEnabledKey = "StatisticBasedFailoverEnabled";

		private const string MinimumLogicalOperationThresholdKey = "MinimumLogicalOperationThreshold";

		private const string StatisticsSlidingWindowDurationKey = "StatisticsSlidingWindowDuration";

		private const string StatisticsBucketDurationKey = "StatisticsBucketDuration";

		private const string StatisticsRefreshIntervalKey = "StatisticsRefreshInterval";

		private const string SlowResponsePercentageFailoverThresholdKey = "SlowResponsePercentageFailoverThreshold";

		private const string LogicalOperationFailurePercentageFailoverThresholdKey = "LogicalOperationFailurePercentageFailoverThreshold";

		private const string PhysicalOperationFailurePercentageFailoverThresholdKey = "PhysicalOperationFailurePercentageFailoverThreshold";

		private const string NotificationsEnabledKey = "NotificationsEnabled";

		private const string BloomFilterModeKey = "BloomFilterMode";

		private const string BloomFilterUpdateFrequencyKey = "BloomFilterUpdateFrequency";

		private const string TenantConfigurationCacheModeKey = "TenantConfigurationCacheMode";

		private const string EntityCacheConfigurationCollectionKey = "EntityCacheConfiguration";

		private const string VelocityDisabledKey = "VelocityDisabled";

		private const string BloomFilterTracerTokenGranularityKey = "BloomFilterTracerTokenGranularity";

		private static CacheConfiguration instance = ((CacheConfiguration)ConfigurationManager.GetSection(CacheConfiguration.SectionName)) ?? new CacheConfiguration();
	}
}
