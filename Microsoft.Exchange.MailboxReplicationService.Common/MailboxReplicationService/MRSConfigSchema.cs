using System;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using Microsoft.Exchange.Data.ConfigurationSettings;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	internal class MRSConfigSchema : ConfigSchemaBase
	{
		public override string Name
		{
			get
			{
				return "MRS";
			}
		}

		public override string SectionName
		{
			get
			{
				return "MRSConfiguration";
			}
		}

		[ConfigurationProperty("LoggingPath", DefaultValue = "")]
		public string LoggingPath
		{
			get
			{
				string text = this.InternalGetConfig<string>("LoggingPath");
				if (string.IsNullOrEmpty(text))
				{
					text = MRSConfigSchema.DefaultLoggingPath;
				}
				return text;
			}
			set
			{
				this.InternalSetConfig<string>(value, "LoggingPath");
			}
		}

		[ConfigurationProperty("MrsVersion", DefaultValue = 1f)]
		public float MrsVersion
		{
			get
			{
				return this.InternalGetConfig<float>("MrsVersion");
			}
			set
			{
				this.InternalSetConfig<float>(value, "MrsVersion");
			}
		}

		[TimeSpanValidator(MinValueString = "00:00:00", MaxValueString = "99999.00:00:00", ExcludeRange = false)]
		[ConfigurationProperty("MaxLogAge", DefaultValue = "30.00:00:00")]
		public TimeSpan MaxLogAge
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("MaxLogAge");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "MaxLogAge");
			}
		}

		[ConfigurationProperty("RequestLogEnabled", DefaultValue = false)]
		public bool RequestLogEnabled
		{
			get
			{
				return this.InternalGetConfig<bool>("RequestLogEnabled");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "RequestLogEnabled");
			}
		}

		[LongValidator(MinValue = 0L, MaxValue = 1048576000L, ExcludeRange = false)]
		[ConfigurationProperty("RequestLogMaxDirSize", DefaultValue = "50000000")]
		public long RequestLogMaxDirSize
		{
			get
			{
				return this.InternalGetConfig<long>("RequestLogMaxDirSize");
			}
			set
			{
				this.InternalSetConfig<long>(value, "RequestLogMaxDirSize");
			}
		}

		[LongValidator(MinValue = 0L, MaxValue = 10485760L, ExcludeRange = false)]
		[ConfigurationProperty("RequestLogMaxFileSize", DefaultValue = "500000")]
		public long RequestLogMaxFileSize
		{
			get
			{
				return this.InternalGetConfig<long>("RequestLogMaxFileSize");
			}
			set
			{
				this.InternalSetConfig<long>(value, "RequestLogMaxFileSize");
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

		[ConfigurationProperty("IsJobPickupEnabled", DefaultValue = true)]
		public bool IsJobPickupEnabled
		{
			get
			{
				return this.InternalGetConfig<bool>("IsJobPickupEnabled");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "IsJobPickupEnabled");
			}
		}

		[IntegerValidator(MinValue = 0, MaxValue = 1000, ExcludeRange = false)]
		[ConfigurationProperty("MaxRetries", DefaultValue = "60")]
		public int MaxRetries
		{
			get
			{
				return this.InternalGetConfig<int>("MaxRetries");
			}
			set
			{
				this.InternalSetConfig<int>(value, "MaxRetries");
			}
		}

		[ConfigurationProperty("WLMResourceStatsLogEnabled", DefaultValue = false)]
		public bool WLMResourceStatsLogEnabled
		{
			get
			{
				return this.InternalGetConfig<bool>("WLMResourceStatsLogEnabled");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "WLMResourceStatsLogEnabled");
			}
		}

		[ConfigurationProperty("WLMResourceStatsLogMaxDirSize", DefaultValue = "50000000")]
		[LongValidator(MinValue = 0L, MaxValue = 1048576000L, ExcludeRange = false)]
		public long WLMResourceStatsLogMaxDirSize
		{
			get
			{
				return this.InternalGetConfig<long>("WLMResourceStatsLogMaxDirSize");
			}
			set
			{
				this.InternalSetConfig<long>(value, "WLMResourceStatsLogMaxDirSize");
			}
		}

		[ConfigurationProperty("WLMResourceStatsLogMaxFileSize", DefaultValue = "500000")]
		[LongValidator(MinValue = 0L, MaxValue = 10485760L, ExcludeRange = false)]
		public long WLMResourceStatsLogMaxFileSize
		{
			get
			{
				return this.InternalGetConfig<long>("WLMResourceStatsLogMaxFileSize");
			}
			set
			{
				this.InternalSetConfig<long>(value, "WLMResourceStatsLogMaxFileSize");
			}
		}

		[TimeSpanValidator(MinValueString = "00:00:00", MaxValueString = "10.00:00:00", ExcludeRange = false)]
		[ConfigurationProperty("WLMResourceStatsLoggingPeriod", DefaultValue = "00:30:00")]
		public TimeSpan WLMResourceStatsLoggingPeriod
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("WLMResourceStatsLoggingPeriod");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "WLMResourceStatsLoggingPeriod");
			}
		}

		[ConfigurationProperty("ShowJobPickupStatusInRequestStatisticsMessage", DefaultValue = true)]
		public bool ShowJobPickupStatusInRequestStatisticsMessage
		{
			get
			{
				return this.InternalGetConfig<bool>("ShowJobPickupStatusInRequestStatisticsMessage");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "ShowJobPickupStatusInRequestStatisticsMessage");
			}
		}

		[ConfigurationProperty("MRSSettingsLogEnabled", DefaultValue = false)]
		public bool MRSSettingsLogEnabled
		{
			get
			{
				return this.InternalGetConfig<bool>("MRSSettingsLogEnabled");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "MRSSettingsLogEnabled");
			}
		}

		[TypeConverter(typeof(MRSSettingsLogCollection.MRSSettingsLogCollectionConverter))]
		[ConfigurationProperty("MRSSettingsLogList", DefaultValue = null)]
		public MRSSettingsLogCollection MRSSettingsLogList
		{
			get
			{
				return this.InternalGetConfig<MRSSettingsLogCollection>("MRSSettingsLogList");
			}
			set
			{
				this.InternalSetConfig<MRSSettingsLogCollection>(value, "MRSSettingsLogList");
			}
		}

		[LongValidator(MinValue = 0L, MaxValue = 1048576000L, ExcludeRange = false)]
		[ConfigurationProperty("MRSSettingsLogMaxDirSize", DefaultValue = "50000000")]
		public long MRSSettingsLogMaxDirSize
		{
			get
			{
				return this.InternalGetConfig<long>("MRSSettingsLogMaxDirSize");
			}
			set
			{
				this.InternalSetConfig<long>(value, "MRSSettingsLogMaxDirSize");
			}
		}

		[LongValidator(MinValue = 0L, MaxValue = 10485760L, ExcludeRange = false)]
		[ConfigurationProperty("MRSSettingsLogMaxFileSize", DefaultValue = "500000")]
		public long MRSSettingsLogMaxFileSize
		{
			get
			{
				return this.InternalGetConfig<long>("MRSSettingsLogMaxFileSize");
			}
			set
			{
				this.InternalSetConfig<long>(value, "MRSSettingsLogMaxFileSize");
			}
		}

		[TimeSpanValidator(MinValueString = "00:00:00", MaxValueString = "10.00:00:00", ExcludeRange = false)]
		[ConfigurationProperty("MRSSettingsLoggingPeriod", DefaultValue = "08:00:00")]
		public TimeSpan MRSSettingsLoggingPeriod
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("MRSSettingsLoggingPeriod");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "MRSSettingsLoggingPeriod");
			}
		}

		[ConfigurationProperty("MRSScheduledLogsCheckFrequency", DefaultValue = "00:10:00")]
		[TimeSpanValidator(MinValueString = "00:00:00", MaxValueString = "10.00:00:00", ExcludeRange = false)]
		public TimeSpan MRSScheduledLogsCheckFrequency
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("MRSScheduledLogsCheckFrequency");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "MRSScheduledLogsCheckFrequency");
			}
		}

		[TimeSpanValidator(MinValueString = "00:00:10", MaxValueString = "00:30:00", ExcludeRange = false)]
		[ConfigurationProperty("RetryDelay", DefaultValue = "00:00:30")]
		public TimeSpan RetryDelay
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("RetryDelay");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "RetryDelay");
			}
		}

		[IntegerValidator(MinValue = 0, MaxValue = 600, ExcludeRange = false)]
		[ConfigurationProperty("MaxCleanupRetries", DefaultValue = "480")]
		public int MaxCleanupRetries
		{
			get
			{
				return this.InternalGetConfig<int>("MaxCleanupRetries");
			}
			set
			{
				this.InternalSetConfig<int>(value, "MaxCleanupRetries");
			}
		}

		[TimeSpanValidator(MinValueString = "00:00:10", MaxValueString = "05:00:00", ExcludeRange = false)]
		[ConfigurationProperty("MaxStallRetryPeriod", DefaultValue = "00:15:00")]
		public TimeSpan MaxStallRetryPeriod
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("MaxStallRetryPeriod");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "MaxStallRetryPeriod");
			}
		}

		[ConfigurationProperty("ExportBufferSizeKB", DefaultValue = "128")]
		[IntegerValidator(MinValue = 1, MaxValue = 131072, ExcludeRange = false)]
		public int ExportBufferSizeKB
		{
			get
			{
				return this.InternalGetConfig<int>("ExportBufferSizeKB");
			}
			set
			{
				this.InternalSetConfig<int>(value, "ExportBufferSizeKB");
			}
		}

		[ConfigurationProperty("ExportBufferSizeOverrideKB", DefaultValue = "0")]
		[IntegerValidator(MinValue = 0, MaxValue = 131072, ExcludeRange = false)]
		public int ExportBufferSizeOverrideKB
		{
			get
			{
				return this.InternalGetConfig<int>("ExportBufferSizeOverrideKB");
			}
			set
			{
				this.InternalSetConfig<int>(value, "ExportBufferSizeOverrideKB");
			}
		}

		[ConfigurationProperty("MinBatchSize", DefaultValue = "300")]
		[IntegerValidator(MinValue = 1, MaxValue = 10000, ExcludeRange = false)]
		public int MinBatchSize
		{
			get
			{
				return this.InternalGetConfig<int>("MinBatchSize");
			}
			set
			{
				this.InternalSetConfig<int>(value, "MinBatchSize");
			}
		}

		[ConfigurationProperty("MinBatchSizeKB", DefaultValue = "7168")]
		[IntegerValidator(MinValue = 1, MaxValue = 131072, ExcludeRange = false)]
		public int MinBatchSizeKB
		{
			get
			{
				return this.InternalGetConfig<int>("MinBatchSizeKB");
			}
			set
			{
				this.InternalSetConfig<int>(value, "MinBatchSizeKB");
			}
		}

		[ConfigurationProperty("MaxMoveHistoryLength", DefaultValue = "5")]
		[IntegerValidator(MinValue = 0, MaxValue = 100, ExcludeRange = false)]
		public int MaxMoveHistoryLength
		{
			get
			{
				return this.InternalGetConfig<int>("MaxMoveHistoryLength");
			}
			set
			{
				this.InternalSetConfig<int>(value, "MaxMoveHistoryLength");
			}
		}

		[ConfigurationProperty("MaxActiveMovesPerSourceMDB", DefaultValue = "20")]
		[IntegerValidator(MinValue = 0, MaxValue = 100, ExcludeRange = false)]
		public int MaxActiveMovesPerSourceMDB
		{
			get
			{
				return this.InternalGetConfig<int>("MaxActiveMovesPerSourceMDB");
			}
			set
			{
				this.InternalSetConfig<int>(value, "MaxActiveMovesPerSourceMDB");
			}
		}

		[ConfigurationProperty("MaxActiveMovesPerTargetMDB", DefaultValue = "20")]
		[IntegerValidator(MinValue = 0, MaxValue = 100, ExcludeRange = false)]
		public int MaxActiveMovesPerTargetMDB
		{
			get
			{
				return this.InternalGetConfig<int>("MaxActiveMovesPerTargetMDB");
			}
			set
			{
				this.InternalSetConfig<int>(value, "MaxActiveMovesPerTargetMDB");
			}
		}

		[IntegerValidator(MinValue = 0, MaxValue = 1000, ExcludeRange = false)]
		[ConfigurationProperty("MaxActiveMovesPerSourceServer", DefaultValue = "100")]
		public int MaxActiveMovesPerSourceServer
		{
			get
			{
				return this.InternalGetConfig<int>("MaxActiveMovesPerSourceServer");
			}
			set
			{
				this.InternalSetConfig<int>(value, "MaxActiveMovesPerSourceServer");
			}
		}

		[IntegerValidator(MinValue = 0, MaxValue = 1000, ExcludeRange = false)]
		[ConfigurationProperty("MaxActiveMovesPerTargetServer", DefaultValue = "100")]
		public int MaxActiveMovesPerTargetServer
		{
			get
			{
				return this.InternalGetConfig<int>("MaxActiveMovesPerTargetServer");
			}
			set
			{
				this.InternalSetConfig<int>(value, "MaxActiveMovesPerTargetServer");
			}
		}

		[ConfigurationProperty("MaxActiveJobsPerSourceMailbox", DefaultValue = "5")]
		[IntegerValidator(MinValue = 0, MaxValue = 100, ExcludeRange = false)]
		public int MaxActiveJobsPerSourceMailbox
		{
			get
			{
				return this.InternalGetConfig<int>("MaxActiveJobsPerSourceMailbox");
			}
			set
			{
				this.InternalSetConfig<int>(value, "MaxActiveJobsPerSourceMailbox");
			}
		}

		[IntegerValidator(MinValue = 0, MaxValue = 100, ExcludeRange = false)]
		[ConfigurationProperty("MaxActiveJobsPerTargetMailbox", DefaultValue = "2")]
		public int MaxActiveJobsPerTargetMailbox
		{
			get
			{
				return this.InternalGetConfig<int>("MaxActiveJobsPerTargetMailbox");
			}
			set
			{
				this.InternalSetConfig<int>(value, "MaxActiveJobsPerTargetMailbox");
			}
		}

		[IntegerValidator(MinValue = 0, MaxValue = 1024, ExcludeRange = false)]
		[ConfigurationProperty("MaxTotalRequestsPerMRS", DefaultValue = "100")]
		public int MaxTotalRequestsPerMRS
		{
			get
			{
				return this.InternalGetConfig<int>("MaxTotalRequestsPerMRS");
			}
			set
			{
				this.InternalSetConfig<int>(value, "MaxTotalRequestsPerMRS");
			}
		}

		[ConfigurationProperty("FullScanMoveJobsPollingPeriod", DefaultValue = "00:15:00")]
		[TimeSpanValidator(MinValueString = "00:03:00", MaxValueString = "1.00:00:00", ExcludeRange = false)]
		public TimeSpan FullScanMoveJobsPollingPeriod
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("FullScanMoveJobsPollingPeriod");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "FullScanMoveJobsPollingPeriod");
			}
		}

		[ConfigurationProperty("FullScanLightJobsPollingPeriod", DefaultValue = "00:15:00")]
		[TimeSpanValidator(MinValueString = "00:00:30", MaxValueString = "1.00:00:00", ExcludeRange = false)]
		public TimeSpan FullScanLightJobsPollingPeriod
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("FullScanLightJobsPollingPeriod");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "FullScanLightJobsPollingPeriod");
			}
		}

		[ConfigurationProperty("ADInconsistencyCleanUpPeriod", DefaultValue = "1.00:00:00")]
		[TimeSpanValidator(MinValueString = "00:00:00", MaxValueString = "10.00:00:00", ExcludeRange = false)]
		public TimeSpan ADInconsistencyCleanUpPeriod
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("ADInconsistencyCleanUpPeriod");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "ADInconsistencyCleanUpPeriod");
			}
		}

		[ConfigurationProperty("HeavyJobPickupPeriod", DefaultValue = "00:00:05")]
		[TimeSpanValidator(MinValueString = "00:00:00", MaxValueString = "10.00:00:00", ExcludeRange = false)]
		public TimeSpan HeavyJobPickupPeriod
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("HeavyJobPickupPeriod");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "HeavyJobPickupPeriod");
			}
		}

		[TimeSpanValidator(MinValueString = "00:00:00", MaxValueString = "10.00:00:00", ExcludeRange = false)]
		[ConfigurationProperty("LightJobPickupPeriod", DefaultValue = "00:00:10")]
		public TimeSpan LightJobPickupPeriod
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("LightJobPickupPeriod");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "LightJobPickupPeriod");
			}
		}

		[ConfigurationProperty("MinimumDatabaseScanInterval", DefaultValue = "00:01:00")]
		[TimeSpanValidator(MinValueString = "00:00:00", MaxValueString = "00:30:00", ExcludeRange = false)]
		public TimeSpan MinimumDatabaseScanInterval
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("MinimumDatabaseScanInterval");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "MinimumDatabaseScanInterval");
			}
		}

		[TimeSpanValidator(MinValueString = "00:01:00", MaxValueString = "12:00:00", ExcludeRange = false)]
		[ConfigurationProperty("ReservationExpirationInterval", DefaultValue = "00:05:00")]
		public TimeSpan ReservationExpirationInterval
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("ReservationExpirationInterval");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "ReservationExpirationInterval");
			}
		}

		[TimeSpanValidator(MinValueString = "00:00:00", MaxValueString = "12:00:00", ExcludeRange = false)]
		[ConfigurationProperty("JobStuckDetectionTime", DefaultValue = "03:00:00")]
		public TimeSpan JobStuckDetectionTime
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("JobStuckDetectionTime");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "JobStuckDetectionTime");
			}
		}

		[TimeSpanValidator(MinValueString = "00:00:00", MaxValueString = "12:00:00", ExcludeRange = false)]
		[ConfigurationProperty("JobStuckDetectionWarmupTime", DefaultValue = "01:00:00")]
		public TimeSpan JobStuckDetectionWarmupTime
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("JobStuckDetectionWarmupTime");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "JobStuckDetectionWarmupTime");
			}
		}

		[ConfigurationProperty("BackoffIntervalForProxyConnectionLimitReached", DefaultValue = "00:05:00")]
		[TimeSpanValidator(MinValueString = "00:00:30", MaxValueString = "1.00:00:00", ExcludeRange = false)]
		public TimeSpan BackoffIntervalForProxyConnectionLimitReached
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("BackoffIntervalForProxyConnectionLimitReached");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "BackoffIntervalForProxyConnectionLimitReached");
			}
		}

		[TimeSpanValidator(MinValueString = "00:00:01", MaxValueString = "02:00:00", ExcludeRange = false)]
		[ConfigurationProperty("DataGuaranteeCheckPeriod", DefaultValue = "00:00:05")]
		public TimeSpan DataGuaranteeCheckPeriod
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("DataGuaranteeCheckPeriod");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "DataGuaranteeCheckPeriod");
			}
		}

		[ConfigurationProperty("EnableDataGuaranteeCheck", DefaultValue = true)]
		public bool EnableDataGuaranteeCheck
		{
			get
			{
				return this.InternalGetConfig<bool>("EnableDataGuaranteeCheck");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "EnableDataGuaranteeCheck");
			}
		}

		[TimeSpanValidator(MinValueString = "00:00:00", MaxValueString = "12:00:00", ExcludeRange = false)]
		[ConfigurationProperty("DataGuaranteeTimeout", DefaultValue = "00:10:00")]
		public TimeSpan DataGuaranteeTimeout
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("DataGuaranteeTimeout");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "DataGuaranteeTimeout");
			}
		}

		[TimeSpanValidator(MinValueString = "00:00:00", MaxValueString = "12:00:00", ExcludeRange = false)]
		[ConfigurationProperty("DataGuaranteeLogRollDelay", DefaultValue = "00:01:00")]
		public TimeSpan DataGuaranteeLogRollDelay
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("DataGuaranteeLogRollDelay");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "DataGuaranteeLogRollDelay");
			}
		}

		[ConfigurationProperty("DataGuaranteeRetryInterval", DefaultValue = "00:15:00")]
		[TimeSpanValidator(MinValueString = "00:00:00", MaxValueString = "12:00:00", ExcludeRange = false)]
		public TimeSpan DataGuaranteeRetryInterval
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("DataGuaranteeRetryInterval");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "DataGuaranteeRetryInterval");
			}
		}

		[TimeSpanValidator(MinValueString = "00:00:00", MaxValueString = "7.00:00:00", ExcludeRange = false)]
		[ConfigurationProperty("DataGuaranteeMaxWait", DefaultValue = "1.00:00:00")]
		public TimeSpan DataGuaranteeMaxWait
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("DataGuaranteeMaxWait");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "DataGuaranteeMaxWait");
			}
		}

		[TimeSpanValidator(MinValueString = "00:00:01", MaxValueString = "00:01:00", ExcludeRange = false)]
		[ConfigurationProperty("DelayCheckPeriod", DefaultValue = "00:00:05")]
		public TimeSpan DelayCheckPeriod
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("DelayCheckPeriod");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "DelayCheckPeriod");
			}
		}

		[TimeSpanValidator(MinValueString = "00:00:00", MaxValueString = "5.00:00:00", ExcludeRange = false)]
		[ConfigurationProperty("CanceledRequestAge", DefaultValue = "1.00:00:00")]
		public TimeSpan CanceledRequestAge
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("CanceledRequestAge");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "CanceledRequestAge");
			}
		}

		[TimeSpanValidator(MinValueString = "00:00:05", MaxValueString = "1.00:00:00", ExcludeRange = false)]
		[ConfigurationProperty("OfflineMoveTransientFailureRelinquishPeriod", DefaultValue = "01:00:00")]
		public TimeSpan OfflineMoveTransientFailureRelinquishPeriod
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("OfflineMoveTransientFailureRelinquishPeriod");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "OfflineMoveTransientFailureRelinquishPeriod");
			}
		}

		[ConfigurationProperty("DisableMrsProxyBuffering", DefaultValue = false)]
		public bool DisableMrsProxyBuffering
		{
			get
			{
				return this.InternalGetConfig<bool>("DisableMrsProxyBuffering");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "DisableMrsProxyBuffering");
			}
		}

		[TimeSpanValidator(MinValueString = "00:00:00", MaxValueString = "12:00:00", ExcludeRange = false)]
		[ConfigurationProperty("MailboxLockoutTimeout", DefaultValue = "02:00:00")]
		public TimeSpan MailboxLockoutTimeout
		{
			get
			{
				TimeSpan timeSpan = this.InternalGetConfig<TimeSpan>("MailboxLockoutTimeout");
				if (timeSpan != TimeSpan.Zero && timeSpan < TimeSpan.FromMinutes(1.0))
				{
					timeSpan = TimeSpan.FromMinutes(1.0);
				}
				return timeSpan;
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "MailboxLockoutTimeout");
			}
		}

		[TimeSpanValidator(MinValueString = "00:00:30", MaxValueString = "12:00:00", ExcludeRange = false)]
		[ConfigurationProperty("MailboxLockoutRetryInterval", DefaultValue = "00:05:00")]
		public TimeSpan MailboxLockoutRetryInterval
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("MailboxLockoutRetryInterval");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "MailboxLockoutRetryInterval");
			}
		}

		[TimeSpanValidator(MinValueString = "00:00:00", MaxValueString = "12:00:00", ExcludeRange = false)]
		[ConfigurationProperty("WlmThrottlingJobTimeout", DefaultValue = "00:05:00")]
		public TimeSpan WlmThrottlingJobTimeout
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("WlmThrottlingJobTimeout");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "WlmThrottlingJobTimeout");
			}
		}

		[TimeSpanValidator(MinValueString = "00:00:00", MaxValueString = "12:00:00", ExcludeRange = false)]
		[ConfigurationProperty("WlmThrottlingJobRetryInterval", DefaultValue = "00:10:00")]
		public TimeSpan WlmThrottlingJobRetryInterval
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("WlmThrottlingJobRetryInterval");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "WlmThrottlingJobRetryInterval");
			}
		}

		[ConfigurationProperty("MRSProxyLongOperationTimeout", DefaultValue = "00:20:00")]
		[TimeSpanValidator(MinValueString = "00:01:00", MaxValueString = "02:00:00", ExcludeRange = false)]
		public TimeSpan MRSProxyLongOperationTimeout
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("MRSProxyLongOperationTimeout");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "MRSProxyLongOperationTimeout");
			}
		}

		[ConfigurationProperty("ContentVerificationIgnoreFAI", DefaultValue = false)]
		public bool ContentVerificationIgnoreFAI
		{
			get
			{
				return this.InternalGetConfig<bool>("ContentVerificationIgnoreFAI");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "ContentVerificationIgnoreFAI");
			}
		}

		[ConfigurationProperty("ContentVerificationIgnorableMsgClasses", DefaultValue = "")]
		public string ContentVerificationIgnorableMsgClasses
		{
			get
			{
				return this.InternalGetConfig<string>("ContentVerificationIgnorableMsgClasses");
			}
			set
			{
				this.InternalSetConfig<string>(value, "ContentVerificationIgnorableMsgClasses");
			}
		}

		[ConfigurationProperty("ContentVerificationMissingItemThreshold", DefaultValue = 0)]
		[IntegerValidator(MinValue = 0, MaxValue = 2147483647, ExcludeRange = false)]
		public int ContentVerificationMissingItemThreshold
		{
			get
			{
				return this.InternalGetConfig<int>("ContentVerificationMissingItemThreshold");
			}
			set
			{
				this.InternalSetConfig<int>(value, "ContentVerificationMissingItemThreshold");
			}
		}

		[ConfigurationProperty("DisableContentVerification", DefaultValue = false)]
		public bool DisableContentVerification
		{
			get
			{
				return this.InternalGetConfig<bool>("DisableContentVerification");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "DisableContentVerification");
			}
		}

		[IntegerValidator(MinValue = 0, MaxValue = 100, ExcludeRange = false)]
		[ConfigurationProperty("PoisonLimit", DefaultValue = 5)]
		public int PoisonLimit
		{
			get
			{
				return this.InternalGetConfig<int>("PoisonLimit");
			}
			set
			{
				this.InternalSetConfig<int>(value, "PoisonLimit");
			}
		}

		[IntegerValidator(MinValue = 0, MaxValue = 100, ExcludeRange = false)]
		[ConfigurationProperty("HardPoisonLimit", DefaultValue = 20)]
		public int HardPoisonLimit
		{
			get
			{
				return this.InternalGetConfig<int>("HardPoisonLimit");
			}
			set
			{
				this.InternalSetConfig<int>(value, "HardPoisonLimit");
			}
		}

		[ConfigurationProperty("FailureHistoryLength", DefaultValue = 60)]
		[IntegerValidator(MinValue = 0, MaxValue = 1000, ExcludeRange = false)]
		public int FailureHistoryLength
		{
			get
			{
				return this.InternalGetConfig<int>("FailureHistoryLength");
			}
			set
			{
				this.InternalSetConfig<int>(value, "FailureHistoryLength");
			}
		}

		[TimeSpanValidator(MinValueString = "00:00:00", MaxValueString = "7.00:00:00", ExcludeRange = false)]
		[ConfigurationProperty("LongRunningJobRelinquishInterval", DefaultValue = "04:00:00")]
		public TimeSpan LongRunningJobRelinquishInterval
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("LongRunningJobRelinquishInterval");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "LongRunningJobRelinquishInterval");
			}
		}

		[ConfigurationProperty("DisableDynamicThrottling", DefaultValue = true)]
		public bool DisableDynamicThrottling
		{
			get
			{
				return this.InternalGetConfig<bool>("DisableDynamicThrottling");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "DisableDynamicThrottling");
			}
		}

		[ConfigurationProperty("UseExtendedAclInformation", DefaultValue = true)]
		public bool UseExtendedAclInformation
		{
			get
			{
				return this.InternalGetConfig<bool>("UseExtendedAclInformation");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "UseExtendedAclInformation");
			}
		}

		[ConfigurationProperty("SkipWordBreaking", DefaultValue = false)]
		public bool SkipWordBreaking
		{
			get
			{
				return this.InternalGetConfig<bool>("SkipWordBreaking");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "SkipWordBreaking");
			}
		}

		[ConfigurationProperty("SkipKnownCorruptionsDefault", DefaultValue = false)]
		public bool SkipKnownCorruptionsDefault
		{
			get
			{
				return this.InternalGetConfig<bool>("SkipKnownCorruptionsDefault");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "SkipKnownCorruptionsDefault");
			}
		}

		[ConfigurationProperty("IgnoreHealthMonitor", DefaultValue = false)]
		public bool IgnoreHealthMonitor
		{
			get
			{
				return this.InternalGetConfig<bool>("IgnoreHealthMonitor");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "IgnoreHealthMonitor");
			}
		}

		[TimeSpanValidator(MinValueString = "1", MaxValueString = "18250", ExcludeRange = false)]
		[ConfigurationProperty("OldItemAge", DefaultValue = "366")]
		public TimeSpan OldItemAge
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("OldItemAge");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "OldItemAge");
			}
		}

		[ConfigurationProperty("BadItemLimitOldNonContact", DefaultValue = 0)]
		[IntegerValidator(MinValue = 0, MaxValue = 2147483647, ExcludeRange = false)]
		public int BadItemLimitOldNonContact
		{
			get
			{
				return this.InternalGetConfig<int>("BadItemLimitOldNonContact");
			}
			set
			{
				this.InternalSetConfig<int>(value, "BadItemLimitOldNonContact");
			}
		}

		[ConfigurationProperty("BadItemLimitContact", DefaultValue = 0)]
		[IntegerValidator(MinValue = 0, MaxValue = 2147483647, ExcludeRange = false)]
		public int BadItemLimitContact
		{
			get
			{
				return this.InternalGetConfig<int>("BadItemLimitContact");
			}
			set
			{
				this.InternalSetConfig<int>(value, "BadItemLimitContact");
			}
		}

		[IntegerValidator(MinValue = 0, MaxValue = 2147483647, ExcludeRange = false)]
		[ConfigurationProperty("BadItemLimitDistributionList", DefaultValue = 0)]
		public int BadItemLimitDistributionList
		{
			get
			{
				return this.InternalGetConfig<int>("BadItemLimitDistributionList");
			}
			set
			{
				this.InternalSetConfig<int>(value, "BadItemLimitDistributionList");
			}
		}

		[ConfigurationProperty("BadItemLimitDefault", DefaultValue = 0)]
		[IntegerValidator(MinValue = 0, MaxValue = 2147483647, ExcludeRange = false)]
		public int BadItemLimitDefault
		{
			get
			{
				return this.InternalGetConfig<int>("BadItemLimitDefault");
			}
			set
			{
				this.InternalSetConfig<int>(value, "BadItemLimitDefault");
			}
		}

		[ConfigurationProperty("BadItemLimitInDumpster", DefaultValue = 0)]
		[IntegerValidator(MinValue = 0, MaxValue = 2147483647, ExcludeRange = false)]
		public int BadItemLimitInDumpster
		{
			get
			{
				return this.InternalGetConfig<int>("BadItemLimitInDumpster");
			}
			set
			{
				this.InternalSetConfig<int>(value, "BadItemLimitInDumpster");
			}
		}

		[IntegerValidator(MinValue = 0, MaxValue = 2147483647, ExcludeRange = false)]
		[ConfigurationProperty("BadItemLimitCalendarRecurrenceCorruption", DefaultValue = 0)]
		public int BadItemLimitCalendarRecurrenceCorruption
		{
			get
			{
				return this.InternalGetConfig<int>("BadItemLimitCalendarRecurrenceCorruption");
			}
			set
			{
				this.InternalSetConfig<int>(value, "BadItemLimitCalendarRecurrenceCorruption");
			}
		}

		[IntegerValidator(MinValue = 0, MaxValue = 2147483647, ExcludeRange = false)]
		[ConfigurationProperty("BadItemLimitStartGreaterThanEndCalendarCorruption", DefaultValue = 0)]
		public int BadItemLimitStartGreaterThanEndCalendarCorruption
		{
			get
			{
				return this.InternalGetConfig<int>("BadItemLimitStartGreaterThanEndCalendarCorruption");
			}
			set
			{
				this.InternalSetConfig<int>(value, "BadItemLimitStartGreaterThanEndCalendarCorruption");
			}
		}

		[ConfigurationProperty("BadItemLimitConflictEntryIdCorruption", DefaultValue = 0)]
		[IntegerValidator(MinValue = 0, MaxValue = 2147483647, ExcludeRange = false)]
		public int BadItemLimitConflictEntryIdCorruption
		{
			get
			{
				return this.InternalGetConfig<int>("BadItemLimitConflictEntryIdCorruption");
			}
			set
			{
				this.InternalSetConfig<int>(value, "BadItemLimitConflictEntryIdCorruption");
			}
		}

		[ConfigurationProperty("BadItemLimitRecipientCorruption", DefaultValue = 0)]
		[IntegerValidator(MinValue = 0, MaxValue = 2147483647, ExcludeRange = false)]
		public int BadItemLimitRecipientCorruption
		{
			get
			{
				return this.InternalGetConfig<int>("BadItemLimitRecipientCorruption");
			}
			set
			{
				this.InternalSetConfig<int>(value, "BadItemLimitRecipientCorruption");
			}
		}

		[ConfigurationProperty("BadItemLimitUnifiedMessagingReportRecipientCorruption", DefaultValue = 0)]
		[IntegerValidator(MinValue = 0, MaxValue = 2147483647, ExcludeRange = false)]
		public int BadItemLimitUnifiedMessagingReportRecipientCorruption
		{
			get
			{
				return this.InternalGetConfig<int>("BadItemLimitUnifiedMessagingReportRecipientCorruption");
			}
			set
			{
				this.InternalSetConfig<int>(value, "BadItemLimitUnifiedMessagingReportRecipientCorruption");
			}
		}

		[IntegerValidator(MinValue = 0, MaxValue = 2147483647, ExcludeRange = false)]
		[ConfigurationProperty("BadItemLimitNonCanonicalAclCorruption", DefaultValue = 0)]
		public int BadItemLimitNonCanonicalAclCorruption
		{
			get
			{
				return this.InternalGetConfig<int>("BadItemLimitNonCanonicalAclCorruption");
			}
			set
			{
				this.InternalSetConfig<int>(value, "BadItemLimitNonCanonicalAclCorruption");
			}
		}

		[ConfigurationProperty("BadItemLimitStringArrayCorruption", DefaultValue = 0)]
		[IntegerValidator(MinValue = 0, MaxValue = 2147483647, ExcludeRange = false)]
		public int BadItemLimitStringArrayCorruption
		{
			get
			{
				return this.InternalGetConfig<int>("BadItemLimitStringArrayCorruption");
			}
			set
			{
				this.InternalSetConfig<int>(value, "BadItemLimitStringArrayCorruption");
			}
		}

		[IntegerValidator(MinValue = 0, MaxValue = 2147483647, ExcludeRange = false)]
		[ConfigurationProperty("BadItemLimitInvalidMultivalueElementCorruption", DefaultValue = 0)]
		public int BadItemLimitInvalidMultivalueElementCorruption
		{
			get
			{
				return this.InternalGetConfig<int>("BadItemLimitInvalidMultivalueElementCorruption");
			}
			set
			{
				this.InternalSetConfig<int>(value, "BadItemLimitInvalidMultivalueElementCorruption");
			}
		}

		[ConfigurationProperty("BadItemLimitNonUnicodeValueCorruption", DefaultValue = 0)]
		[IntegerValidator(MinValue = 0, MaxValue = 2147483647, ExcludeRange = false)]
		public int BadItemLimitNonUnicodeValueCorruption
		{
			get
			{
				return this.InternalGetConfig<int>("BadItemLimitNonUnicodeValueCorruption");
			}
			set
			{
				this.InternalSetConfig<int>(value, "BadItemLimitNonUnicodeValueCorruption");
			}
		}

		[IntegerValidator(MinValue = 0, MaxValue = 2147483647, ExcludeRange = false)]
		[ConfigurationProperty("BadItemLimitFolderPropertyMismatchCorruption", DefaultValue = 0)]
		public int BadItemLimitFolderPropertyMismatchCorruption
		{
			get
			{
				return this.InternalGetConfig<int>("BadItemLimitFolderPropertyMismatchCorruption");
			}
			set
			{
				this.InternalSetConfig<int>(value, "BadItemLimitFolderPropertyMismatchCorruption");
			}
		}

		[ConfigurationProperty("BadItemLimiFolderPropertyCorruption", DefaultValue = 0)]
		[IntegerValidator(MinValue = 0, MaxValue = 2147483647, ExcludeRange = false)]
		public int BadItemLimiFolderPropertyCorruption
		{
			get
			{
				return this.InternalGetConfig<int>("BadItemLimiFolderPropertyCorruption");
			}
			set
			{
				this.InternalSetConfig<int>(value, "BadItemLimiFolderPropertyCorruption");
			}
		}

		[TimeSpanValidator(MinValueString = "00:00:10", MaxValueString = "05:00:00", ExcludeRange = false)]
		[ConfigurationProperty("InProgressRequestJobLogInterval", DefaultValue = "00:15:00")]
		public TimeSpan InProgressRequestJobLogInterval
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("InProgressRequestJobLogInterval");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "InProgressRequestJobLogInterval");
			}
		}

		[ConfigurationProperty("FailureLogEnabled", DefaultValue = false)]
		public bool FailureLogEnabled
		{
			get
			{
				return this.InternalGetConfig<bool>("FailureLogEnabled");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "FailureLogEnabled");
			}
		}

		[LongValidator(MinValue = 0L, MaxValue = 1048576000L, ExcludeRange = false)]
		[ConfigurationProperty("FailureLogMaxDirSize", DefaultValue = "50000000")]
		public long FailureLogMaxDirSize
		{
			get
			{
				return this.InternalGetConfig<long>("FailureLogMaxDirSize");
			}
			set
			{
				this.InternalSetConfig<long>(value, "FailureLogMaxDirSize");
			}
		}

		[ConfigurationProperty("FailureLogMaxFileSize", DefaultValue = "500000")]
		[LongValidator(MinValue = 0L, MaxValue = 10485760L, ExcludeRange = false)]
		public long FailureLogMaxFileSize
		{
			get
			{
				return this.InternalGetConfig<long>("FailureLogMaxFileSize");
			}
			set
			{
				this.InternalSetConfig<long>(value, "FailureLogMaxFileSize");
			}
		}

		[ConfigurationProperty("CommonFailureLogEnabled", DefaultValue = false)]
		public bool CommonFailureLogEnabled
		{
			get
			{
				return this.InternalGetConfig<bool>("CommonFailureLogEnabled");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "CommonFailureLogEnabled");
			}
		}

		[LongValidator(MinValue = 0L, MaxValue = 1048576000L, ExcludeRange = false)]
		[ConfigurationProperty("CommonFailureLogMaxDirSize", DefaultValue = "50000000")]
		public long CommonFailureLogMaxDirSize
		{
			get
			{
				return this.InternalGetConfig<long>("CommonFailureLogMaxDirSize");
			}
			set
			{
				this.InternalSetConfig<long>(value, "CommonFailureLogMaxDirSize");
			}
		}

		[LongValidator(MinValue = 0L, MaxValue = 10485760L, ExcludeRange = false)]
		[ConfigurationProperty("CommonFailureLogMaxFileSize", DefaultValue = "500000")]
		public long CommonFailureLogMaxFileSize
		{
			get
			{
				return this.InternalGetConfig<long>("CommonFailureLogMaxFileSize");
			}
			set
			{
				this.InternalSetConfig<long>(value, "CommonFailureLogMaxFileSize");
			}
		}

		[ConfigurationProperty("TraceLogMaxDirSize", DefaultValue = "50000000")]
		[LongValidator(MinValue = 0L, MaxValue = 1048576000L, ExcludeRange = false)]
		public long TraceLogMaxDirSize
		{
			get
			{
				return this.InternalGetConfig<long>("TraceLogMaxDirSize");
			}
			set
			{
				this.InternalSetConfig<long>(value, "TraceLogMaxDirSize");
			}
		}

		[ConfigurationProperty("TraceLogMaxFileSize", DefaultValue = "500000")]
		[LongValidator(MinValue = 0L, MaxValue = 10485760L, ExcludeRange = false)]
		public long TraceLogMaxFileSize
		{
			get
			{
				return this.InternalGetConfig<long>("TraceLogMaxFileSize");
			}
			set
			{
				this.InternalSetConfig<long>(value, "TraceLogMaxFileSize");
			}
		}

		[ConfigurationProperty("TraceLogLevels", DefaultValue = "")]
		public string TraceLogLevels
		{
			get
			{
				return this.InternalGetConfig<string>("TraceLogLevels");
			}
			set
			{
				this.InternalSetConfig<string>(value, "TraceLogLevels");
			}
		}

		[ConfigurationProperty("TraceLogTracers", DefaultValue = "")]
		public string TraceLogTracers
		{
			get
			{
				return this.InternalGetConfig<string>("TraceLogTracers");
			}
			set
			{
				this.InternalSetConfig<string>(value, "TraceLogTracers");
			}
		}

		[ConfigurationProperty("BadItemLogEnabled", DefaultValue = false)]
		public bool BadItemLogEnabled
		{
			get
			{
				return this.InternalGetConfig<bool>("BadItemLogEnabled");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "BadItemLogEnabled");
			}
		}

		[ConfigurationProperty("BadItemLogMaxDirSize", DefaultValue = "50000000")]
		[LongValidator(MinValue = 0L, MaxValue = 1048576000L, ExcludeRange = false)]
		public long BadItemLogMaxDirSize
		{
			get
			{
				return this.InternalGetConfig<long>("BadItemLogMaxDirSize");
			}
			set
			{
				this.InternalSetConfig<long>(value, "BadItemLogMaxDirSize");
			}
		}

		[ConfigurationProperty("BadItemLogMaxFileSize", DefaultValue = "500000")]
		[LongValidator(MinValue = 0L, MaxValue = 10485760L, ExcludeRange = false)]
		public long BadItemLogMaxFileSize
		{
			get
			{
				return this.InternalGetConfig<long>("BadItemLogMaxFileSize");
			}
			set
			{
				this.InternalSetConfig<long>(value, "BadItemLogMaxFileSize");
			}
		}

		[ConfigurationProperty("SessionStatisticsLogEnabled", DefaultValue = false)]
		public bool SessionStatisticsLogEnabled
		{
			get
			{
				return this.InternalGetConfig<bool>("SessionStatisticsLogEnabled");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "SessionStatisticsLogEnabled");
			}
		}

		[ConfigurationProperty("SessionStatisticsLogMaxDirSize", DefaultValue = "50000000")]
		[LongValidator(MinValue = 0L, MaxValue = 1048576000L, ExcludeRange = false)]
		public long SessionStatisticsLogMaxDirSize
		{
			get
			{
				return this.InternalGetConfig<long>("SessionStatisticsLogMaxDirSize");
			}
			set
			{
				this.InternalSetConfig<long>(value, "SessionStatisticsLogMaxDirSize");
			}
		}

		[ConfigurationProperty("SessionStatisticsLogMaxFileSize", DefaultValue = "500000")]
		[LongValidator(MinValue = 0L, MaxValue = 10485760L, ExcludeRange = false)]
		public long SessionStatisticsLogMaxFileSize
		{
			get
			{
				return this.InternalGetConfig<long>("SessionStatisticsLogMaxFileSize");
			}
			set
			{
				this.InternalSetConfig<long>(value, "SessionStatisticsLogMaxFileSize");
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

		[ConfigurationProperty("MaxIncrementalChanges", DefaultValue = 1000)]
		[IntegerValidator(MinValue = 0, MaxValue = 2147483647, ExcludeRange = false)]
		public int MaxIncrementalChanges
		{
			get
			{
				return this.InternalGetConfig<int>("MaxIncrementalChanges");
			}
			set
			{
				this.InternalSetConfig<int>(value, "MaxIncrementalChanges");
			}
		}

		[ConfigurationProperty("IssueCacheItemLimit", DefaultValue = 50)]
		[IntegerValidator(MinValue = 0, MaxValue = 2147483647, ExcludeRange = false)]
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

		[ConfigurationProperty("IssueCacheScanFrequency", DefaultValue = "2:00:00")]
		[TimeSpanValidator(MinValueString = "00:01:00", MaxValueString = "365.00:00:00", ExcludeRange = false)]
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

		[ConfigurationProperty("CheckInitialProvisioningForMoves", DefaultValue = "true")]
		public bool CheckInitialProvisioningForMoves
		{
			get
			{
				return this.InternalGetConfig<bool>("CheckInitialProvisioningForMoves");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "CheckInitialProvisioningForMoves");
			}
		}

		[ConfigurationProperty("SendGenericWatson", DefaultValue = "false")]
		public bool SendGenericWatson
		{
			get
			{
				return this.InternalGetConfig<bool>("SendGenericWatson");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "SendGenericWatson");
			}
		}

		[IntegerValidator(MinValue = 1, MaxValue = 2147483647, ExcludeRange = false)]
		[ConfigurationProperty("CrawlerPageSize", DefaultValue = 10)]
		public int CrawlerPageSize
		{
			get
			{
				return this.InternalGetConfig<int>("CrawlerPageSize");
			}
			set
			{
				this.InternalSetConfig<int>(value, "CrawlerPageSize");
			}
		}

		[ConfigurationProperty("EnumerateMessagesPageSize", DefaultValue = 500)]
		[IntegerValidator(MinValue = 1, MaxValue = 2147483647, ExcludeRange = false)]
		public int EnumerateMessagesPageSize
		{
			get
			{
				return this.InternalGetConfig<int>("EnumerateMessagesPageSize");
			}
			set
			{
				this.InternalSetConfig<int>(value, "EnumerateMessagesPageSize");
			}
		}

		[IntegerValidator(MinValue = 1, MaxValue = 2147483647, ExcludeRange = false)]
		[ConfigurationProperty("MaxFolderOpened", DefaultValue = 10)]
		public int MaxFolderOpened
		{
			get
			{
				return this.InternalGetConfig<int>("MaxFolderOpened");
			}
			set
			{
				this.InternalSetConfig<int>(value, "MaxFolderOpened");
			}
		}

		[TimeSpanValidator(MinValueString = "00:00:01", MaxValueString = "00:01:00", ExcludeRange = false)]
		[ConfigurationProperty("CrawlAndCopyFolderTimeout", DefaultValue = "00:00:10")]
		public TimeSpan CrawlAndCopyFolderTimeout
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("CrawlAndCopyFolderTimeout");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "CrawlAndCopyFolderTimeout");
			}
		}

		[ConfigurationProperty("CopyInferenceProperties", DefaultValue = true)]
		public bool CopyInferenceProperties
		{
			get
			{
				return this.InternalGetConfig<bool>("CopyInferenceProperties");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "CopyInferenceProperties");
			}
		}

		[IntegerValidator(MinValue = 8192, MaxValue = 2147483647, ExcludeRange = false)]
		[ConfigurationProperty("MrsBindingMaxMessageSize", DefaultValue = 35000000)]
		public int MrsBindingMaxMessageSize
		{
			get
			{
				return this.InternalGetConfig<int>("MrsBindingMaxMessageSize");
			}
			set
			{
				this.InternalSetConfig<int>(value, "MrsBindingMaxMessageSize");
			}
		}

		[ConfigurationProperty("DCNameValidityInterval", DefaultValue = "02:00:00")]
		[TimeSpanValidator(MinValueString = "00:00:00", MaxValueString = "7.00:00:00", ExcludeRange = false)]
		public TimeSpan DCNameValidityInterval
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("DCNameValidityInterval");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "DCNameValidityInterval");
			}
		}

		[ConfigurationProperty("ActivatedJobIncrementalSyncInterval", DefaultValue = "00:01:00")]
		[TimeSpanValidator(MinValueString = "00:00:05", MaxValueString = "1.00:00:00", ExcludeRange = false)]
		public TimeSpan ActivatedJobIncrementalSyncInterval
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("ActivatedJobIncrementalSyncInterval");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "ActivatedJobIncrementalSyncInterval");
			}
		}

		[TimeSpanValidator(MinValueString = "00:01:00", MaxValueString = "1.00:00:00", ExcludeRange = false)]
		[ConfigurationProperty("DeactivateJobInterval", DefaultValue = "00:17:00")]
		public TimeSpan DeactivateJobInterval
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("DeactivateJobInterval");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "DeactivateJobInterval");
			}
		}

		[ConfigurationProperty("AllAggregationSyncJobsInteractive", DefaultValue = "false")]
		public bool AllAggregationSyncJobsInteractive
		{
			get
			{
				return this.InternalGetConfig<bool>("AllAggregationSyncJobsInteractive");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "AllAggregationSyncJobsInteractive");
			}
		}

		[ConfigurationProperty("GetActionsPageSize", DefaultValue = 1000)]
		[IntegerValidator(MinValue = 0, MaxValue = 2147483647, ExcludeRange = false)]
		public int GetActionsPageSize
		{
			get
			{
				return this.InternalGetConfig<int>("GetActionsPageSize");
			}
			set
			{
				this.InternalSetConfig<int>(value, "GetActionsPageSize");
			}
		}

		[TimeSpanValidator(MinValueString = "00:00:00", MaxValueString = "365.00:00:00", ExcludeRange = false)]
		[ConfigurationProperty("ReconnectAbandonInterval", DefaultValue = "2.00:00:00")]
		public TimeSpan ReconnectAbandonInterval
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("ReconnectAbandonInterval");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "ReconnectAbandonInterval");
			}
		}

		[ConfigurationProperty("DisableAutomaticRepair", DefaultValue = "false")]
		public bool DisableAutomaticRepair
		{
			get
			{
				return this.InternalGetConfig<bool>("DisableAutomaticRepair");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "DisableAutomaticRepair");
			}
		}

		[TimeSpanValidator(MinValueString = "00:00:00", MaxValueString = "1.00:00:00", ExcludeRange = false)]
		[ConfigurationProperty("AutomaticRepairAbandonInterval", DefaultValue = "03:00:00")]
		public TimeSpan AutomaticRepairAbandonInterval
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("AutomaticRepairAbandonInterval");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "AutomaticRepairAbandonInterval");
			}
		}

		[ConfigurationProperty("AdditionalSidsForMrsProxyAuthorization", DefaultValue = null)]
		internal string AdditionalSidsForMrsProxyAuthorization
		{
			get
			{
				return this.InternalGetConfig<string>("AdditionalSidsForMrsProxyAuthorization");
			}
			set
			{
				this.InternalSetConfig<string>(value, "AdditionalSidsForMrsProxyAuthorization");
			}
		}

		[ConfigurationProperty("ProxyClientTrustedCertificateThumbprints", DefaultValue = null)]
		internal string ProxyClientTrustedCertificateThumbprints
		{
			get
			{
				return this.InternalGetConfig<string>("ProxyClientTrustedCertificateThumbprints");
			}
			set
			{
				this.InternalSetConfig<string>(value, "ProxyClientTrustedCertificateThumbprints");
			}
		}

		[ConfigurationProperty("AllowRestoreFromConnectedMailbox", DefaultValue = false)]
		internal bool AllowRestoreFromConnectedMailbox
		{
			get
			{
				return this.InternalGetConfig<bool>("AllowRestoreFromConnectedMailbox");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "AllowRestoreFromConnectedMailbox");
			}
		}

		[ConfigurationProperty("CheckMailUserPlanQuotasForOnboarding", DefaultValue = true)]
		internal bool CheckMailUserPlanQuotasForOnboarding
		{
			get
			{
				return this.InternalGetConfig<bool>("CheckMailUserPlanQuotasForOnboarding");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "CheckMailUserPlanQuotasForOnboarding");
			}
		}

		[ConfigurationProperty("CacheJobQueues", DefaultValue = false)]
		internal bool CacheJobQueues
		{
			get
			{
				return this.InternalGetConfig<bool>("CacheJobQueues");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "CacheJobQueues");
			}
		}

		[ConfigurationProperty("QuarantineEnabled", DefaultValue = false)]
		internal bool QuarantineEnabled
		{
			get
			{
				return this.InternalGetConfig<bool>("QuarantineEnabled");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "QuarantineEnabled");
			}
		}

		[ConfigurationProperty("StalledByHigherPriorityJobsTimeout", DefaultValue = "3.00:00:00")]
		[TimeSpanValidator(MinValueString = "00:00:00", MaxValueString = "365.00:00:00", ExcludeRange = false)]
		public TimeSpan StalledByHigherPriorityJobsTimeout
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("StalledByHigherPriorityJobsTimeout");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "StalledByHigherPriorityJobsTimeout");
			}
		}

		[ConfigurationProperty("CanStoreCreatePFDumpster", DefaultValue = true)]
		internal bool CanStoreCreatePFDumpster
		{
			get
			{
				return this.InternalGetConfig<bool>("CanStoreCreatePFDumpster");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "CanStoreCreatePFDumpster");
			}
		}

		[ConfigurationProperty("DisableContactSync", DefaultValue = false)]
		internal bool DisableContactSync
		{
			get
			{
				return this.InternalGetConfig<bool>("DisableContactSync");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "DisableContactSync");
			}
		}

		[TimeSpanValidator(MinValueString = "00:05:00", MaxValueString = "1.00:00:00", ExcludeRange = false)]
		[ConfigurationProperty("ServerBusyBackoffExpired", DefaultValue = "01:00:00")]
		public TimeSpan ServerBusyBackoffExpired
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("ServerBusyBackoffExpired");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "ServerBusyBackoffExpired");
			}
		}

		[ConfigurationProperty("CanExportFoldersInBatch", DefaultValue = true)]
		internal bool CanExportFoldersInBatch
		{
			get
			{
				return this.InternalGetConfig<bool>("CanExportFoldersInBatch");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "CanExportFoldersInBatch");
			}
		}

		[ConfigurationProperty("ChangesPerIcsManifestPage", DefaultValue = 500)]
		[IntegerValidator(MinValue = 1, MaxValue = 50000, ExcludeRange = false)]
		internal int ChangesPerIcsManifestPage
		{
			get
			{
				return this.InternalGetConfig<int>("ChangesPerIcsManifestPage");
			}
			set
			{
				this.InternalSetConfig<int>(value, "ChangesPerIcsManifestPage");
			}
		}

		[ConfigurationProperty("FoldersPerHierarchySyncBatch", DefaultValue = 500)]
		[IntegerValidator(MinValue = 1, MaxValue = 50000, ExcludeRange = false)]
		internal int FoldersPerHierarchySyncBatch
		{
			get
			{
				return this.InternalGetConfig<int>("FoldersPerHierarchySyncBatch");
			}
			set
			{
				this.InternalSetConfig<int>(value, "FoldersPerHierarchySyncBatch");
			}
		}

		[ConfigurationProperty("ProxyClientCertificateSubject", DefaultValue = "CN=outlook.com, OU=Exchange, O=Microsoft Corporation, L=Redmond, S=Washington, C=US")]
		internal string ProxyClientCertificateSubject
		{
			get
			{
				return this.InternalGetConfig<string>("ProxyClientCertificateSubject");
			}
			set
			{
				this.InternalSetConfig<string>(value, "ProxyClientCertificateSubject");
			}
		}

		[ConfigurationProperty("ProxyServiceCertificateEndpointEnabled", DefaultValue = false)]
		internal bool ProxyServiceCertificateEndpointEnabled
		{
			get
			{
				return this.InternalGetConfig<bool>("ProxyServiceCertificateEndpointEnabled");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "ProxyServiceCertificateEndpointEnabled");
			}
		}

		[ConfigurationProperty("CrossResourceForestEnabled", DefaultValue = false)]
		internal bool CrossResourceForestEnabled
		{
			get
			{
				return this.InternalGetConfig<bool>("CrossResourceForestEnabled");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "CrossResourceForestEnabled");
			}
		}

		[ConfigurationProperty("OwnerLogonToMergeDestination", DefaultValue = true)]
		internal bool OwnerLogonToMergeDestination
		{
			get
			{
				return this.InternalGetConfig<bool>("OwnerLogonToMergeDestination");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "OwnerLogonToMergeDestination");
			}
		}

		[ConfigurationProperty("WlmWorkloadType", DefaultValue = WorkloadType.MailboxReplicationService)]
		public WorkloadType WlmWorkloadType
		{
			get
			{
				return this.InternalGetConfig<WorkloadType>("WlmWorkloadType");
			}
			set
			{
				this.InternalSetConfig<WorkloadType>(value, "WlmWorkloadType");
			}
		}

		[ConfigurationProperty("DisabledFeatures", DefaultValue = MRSConfigurableFeatures.None)]
		public MRSConfigurableFeatures DisabledFeatures
		{
			get
			{
				return this.InternalGetConfig<MRSConfigurableFeatures>("DisabledFeatures");
			}
			set
			{
				this.InternalSetConfig<MRSConfigurableFeatures>(value, "DisabledFeatures");
			}
		}

		public MRSConfigSchema()
		{
			if (CommonUtils.IsMultiTenantEnabled())
			{
				base.SetDefaultConfigValue<bool>("RequestLogEnabled", true);
				base.SetDefaultConfigValue<bool>("FailureLogEnabled", true);
				base.SetDefaultConfigValue<bool>("CommonFailureLogEnabled", true);
				base.SetDefaultConfigValue<bool>("BadItemLogEnabled", true);
				base.SetDefaultConfigValue<bool>("SessionStatisticsLogEnabled", true);
				base.SetDefaultConfigValue<bool>("WLMResourceStatsLogEnabled", true);
				base.SetDefaultConfigValue<bool>("ShowJobPickupStatusInRequestStatisticsMessage", false);
				base.SetDefaultConfigValue<bool>("MRSSettingsLogEnabled", true);
				base.SetDefaultConfigValue<MRSSettingsLogCollection>("MRSSettingsLogList", new MRSSettingsLogCollection(string.Join(";", new string[]
				{
					"IsJobPickupEnabled",
					"MaxActiveMovesPerSourceMDB",
					"MaxActiveMovesPerTargetMDB",
					"MaxActiveMovesPerSourceServer",
					"MaxActiveMovesPerTargetServer",
					"MaxActiveJobsPerSourceMailbox",
					"MaxActiveJobsPerTargetMailbox",
					"MaxTotalRequestsPerMRS",
					"DisableDynamicThrottling",
					"IgnoreHealthMonitor"
				})));
			}
		}

		protected override ExchangeConfigurationSection ScopeSchema
		{
			get
			{
				return MRSConfigSchema.scopeSchema;
			}
		}

		protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
		{
			if (ConfigBase<MRSConfigSchema>.Provider.IsInitialized)
			{
				MrsTracer.Service.Warning("Ignoring unrecognized configuration attribute {0}={1}", new object[]
				{
					name,
					value
				});
			}
			return base.OnDeserializeUnrecognizedAttribute(name, value);
		}

		private static readonly MRSConfigSchema.MrsScopeSchema scopeSchema = new MRSConfigSchema.MrsScopeSchema();

		public static readonly string DefaultLoggingPath = Path.Combine(CommonUtils.SafeInstallPath, "Logging\\MailboxReplicationService");

		[Serializable]
		public static class Setting
		{
			public const string MrsVersion = "MrsVersion";

			public const string IsEnabled = "IsEnabled";

			public const string IsJobPickupEnabled = "IsJobPickupEnabled";

			public const string MaxRetries = "MaxRetries";

			public const string MaxCleanupRetries = "MaxCleanupRetries";

			public const string MaxStallRetryPeriod = "MaxStallRetryPeriod";

			public const string RetryDelay = "RetryDelay";

			public const string ExportBufferSizeKBDefaultValue = "128";

			public const string ExportBufferSizeKB = "ExportBufferSizeKB";

			public const string ExportBufferSizeOverrideKB = "ExportBufferSizeOverrideKB";

			public const string MinBatchSize = "MinBatchSize";

			public const string MinBatchSizeKB = "MinBatchSizeKB";

			public const string MaxMoveHistoryLength = "MaxMoveHistoryLength";

			public const string MaxActiveMovesPerSourceMDB = "MaxActiveMovesPerSourceMDB";

			public const string MaxActiveMovesPerTargetMDB = "MaxActiveMovesPerTargetMDB";

			public const string MaxActiveMovesPerSourceServer = "MaxActiveMovesPerSourceServer";

			public const string MaxActiveMovesPerTargetServer = "MaxActiveMovesPerTargetServer";

			public const string MaxTotalRequestsPerMRS = "MaxTotalRequestsPerMRS";

			public const string MaxActiveJobsPerSourceMailbox = "MaxActiveJobsPerSourceMailbox";

			public const string MaxActiveJobsPerTargetMailbox = "MaxActiveJobsPerTargetMailbox";

			public const string FullScanMoveJobsPollingPeriod = "FullScanMoveJobsPollingPeriod";

			public const string FullScanLightJobsPollingPeriod = "FullScanLightJobsPollingPeriod";

			public const string ADInconsistencyCleanUpPeriod = "ADInconsistencyCleanUpPeriod";

			public const string HeavyJobPickupPeriod = "HeavyJobPickupPeriod";

			public const string LightJobPickupPeriod = "LightJobPickupPeriod";

			public const string MinimumDatabaseScanInterval = "MinimumDatabaseScanInterval";

			public const string MRSAbandonedMoveJobDetectionTime = "MRSAbandonedMoveJobDetectionTime";

			public const string JobStuckDetectionTime = "JobStuckDetectionTime";

			public const string JobStuckDetectionWarmupTime = "JobStuckDetectionWarmupTime";

			public const string BackoffIntervalForProxyConnectionLimitReached = "BackoffIntervalForProxyConnectionLimitReached";

			public const string DataGuaranteeCheckPeriod = "DataGuaranteeCheckPeriod";

			public const string EnableDataGuaranteeCheck = "EnableDataGuaranteeCheck";

			public const string DisableMrsProxyBuffering = "DisableMrsProxyBuffering";

			public const string DataGuaranteeTimeout = "DataGuaranteeTimeout";

			public const string DataGuaranteeLogRollDelay = "DataGuaranteeLogRollDelay";

			public const string DataGuaranteeRetryInterval = "DataGuaranteeRetryInterval";

			public const string DataGuaranteeMaxWait = "DataGuaranteeMaxWait";

			public const string DelayCheckPeriod = "DelayCheckPeriod";

			public const string CanceledRequestAge = "CanceledRequestAge";

			public const string OfflineMoveTransientFailureRelinquishPeriod = "OfflineMoveTransientFailureRelinquishPeriod";

			public const string MailboxLockoutTimeout = "MailboxLockoutTimeout";

			public const string MailboxLockoutRetryInterval = "MailboxLockoutRetryInterval";

			public const string WlmThrottlingJobTimeout = "WlmThrottlingJobTimeout";

			public const string WlmThrottlingJobRetryInterval = "WlmThrottlingJobRetryInterval";

			public const string MRSProxyLongOperationTimeoutDefaultValue = "00:20:00";

			public const string MRSProxyLongOperationTimeout = "MRSProxyLongOperationTimeout";

			public const string ContentVerificationMissingItemThreshold = "ContentVerificationMissingItemThreshold";

			public const string ContentVerificationIgnoreFAI = "ContentVerificationIgnoreFAI";

			public const string ContentVerificationIgnorableMsgClasses = "ContentVerificationIgnorableMsgClasses";

			public const string DisableContentVerification = "DisableContentVerification";

			public const string PoisonLimit = "PoisonLimit";

			public const string HardPoisonLimit = "HardPoisonLimit";

			public const string ReservationExpirationInterval = "ReservationExpirationInterval";

			public const string DisableDynamicThrottling = "DisableDynamicThrottling";

			public const string UseExtendedAclInformation = "UseExtendedAclInformation";

			public const string SkipWordBreaking = "SkipWordBreaking";

			public const string SkipKnownCorruptionsDefault = "SkipKnownCorruptionsDefault";

			public const string IgnoreHealthMonitor = "IgnoreHealthMonitor";

			public const string FailureHistoryLength = "FailureHistoryLength";

			public const string LongRunningJobRelinquishInterval = "LongRunningJobRelinquishInterval";

			public const string LoggingPath = "LoggingPath";

			public const string MaxLogAge = "MaxLogAge";

			public const string RequestLogEnabled = "RequestLogEnabled";

			public const string RequestLogMaxDirSize = "RequestLogMaxDirSize";

			public const string RequestLogMaxFileSize = "RequestLogMaxFileSize";

			public const string FailureLogEnabled = "FailureLogEnabled";

			public const string FailureLogMaxDirSize = "FailureLogMaxDirSize";

			public const string FailureLogMaxFileSize = "FailureLogMaxFileSize";

			public const string CommonFailureLogEnabled = "CommonFailureLogEnabled";

			public const string CommonFailureLogMaxDirSize = "CommonFailureLogMaxDirSize";

			public const string CommonFailureLogMaxFileSize = "CommonFailureLogMaxFileSize";

			public const string TraceLogMaxDirSize = "TraceLogMaxDirSize";

			public const string TraceLogMaxFileSize = "TraceLogMaxFileSize";

			public const string TraceLogLevels = "TraceLogLevels";

			public const string TraceLogTracers = "TraceLogTracers";

			public const string WLMResourceStatsLogEnabled = "WLMResourceStatsLogEnabled";

			public const string WLMResourceStatsLogMaxDirSize = "WLMResourceStatsLogMaxDirSize";

			public const string WLMResourceStatsLogMaxFileSize = "WLMResourceStatsLogMaxFileSize";

			public const string WLMResourceStatsLoggingPeriod = "WLMResourceStatsLoggingPeriod";

			public const string ShowJobPickupStatusInRequestStatisticsMessage = "ShowJobPickupStatusInRequestStatisticsMessage";

			public const string MRSSettingsLogEnabled = "MRSSettingsLogEnabled";

			public const string MRSSettingsLogList = "MRSSettingsLogList";

			public const string MRSSettingsLogMaxDirSize = "MRSSettingsLogMaxDirSize";

			public const string MRSSettingsLogMaxFileSize = "MRSSettingsLogMaxFileSize";

			public const string MRSSettingsLoggingPeriod = "MRSSettingsLoggingPeriod";

			public const string MRSScheduledLogsCheckFrequency = "MRSScheduledLogsCheckFrequency";

			public const string OldItemAge = "OldItemAge";

			public const string BadItemLimitOldNonContact = "BadItemLimitOldNonContact";

			public const string BadItemLimitContact = "BadItemLimitContact";

			public const string BadItemLimitDistributionList = "BadItemLimitDistributionList";

			public const string BadItemLimitDefault = "BadItemLimitDefault";

			public const string BadItemLimitCalendarRecurrenceCorruption = "BadItemLimitCalendarRecurrenceCorruption";

			public const string BadItemLimitInDumpster = "BadItemLimitInDumpster";

			public const string BadItemLimitStartGreaterThanEndCalendarCorruption = "BadItemLimitStartGreaterThanEndCalendarCorruption";

			public const string BadItemLimitConflictEntryIdCorruption = "BadItemLimitConflictEntryIdCorruption";

			public const string BadItemLimitRecipientCorruption = "BadItemLimitRecipientCorruption";

			public const string BadItemLimitUnifiedMessagingReportRecipientCorruption = "BadItemLimitUnifiedMessagingReportRecipientCorruption";

			public const string BadItemLimitNonCanonicalAclCorruption = "BadItemLimitNonCanonicalAclCorruption";

			public const string BadItemLimitStringArrayCorruption = "BadItemLimitStringArrayCorruption";

			public const string BadItemLimitInvalidMultivalueElementCorruption = "BadItemLimitInvalidMultivalueElementCorruption";

			public const string BadItemLimitNonUnicodeValueCorruption = "BadItemLimitNonUnicodeValueCorruption";

			public const string BadItemLimitFolderPropertyMismatchCorruption = "BadItemLimitFolderPropertyMismatchCorruption";

			public const string BadItemLimiFolderPropertyCorruption = "BadItemLimiFolderPropertyCorruption";

			public const string BadItemLogEnabled = "BadItemLogEnabled";

			public const string BadItemLogMaxDirSize = "BadItemLogMaxDirSize";

			public const string BadItemLogMaxFileSize = "BadItemLogMaxFileSize";

			public const string SessionStatisticsLogEnabled = "SessionStatisticsLogEnabled";

			public const string SessionStatisticsLogMaxDirSize = "SessionStatisticsLogMaxDirSize";

			public const string SessionStatisticsLogMaxFileSize = "SessionStatisticsLogMaxFileSize";

			public const string InProgressRequestJobLogInterval = "InProgressRequestJobLogInterval";

			public const string IssueCacheIsEnabled = "IssueCacheIsEnabled";

			public const string MaxIncrementalChanges = "MaxIncrementalChanges";

			public const string IssueCacheItemLimit = "IssueCacheItemLimit";

			public const string IssueCacheScanFrequency = "IssueCacheScanFrequency";

			public const string CheckInitialProvisioningForMoves = "CheckInitialProvisioningForMoves";

			public const string SendGenericWatson = "SendGenericWatson";

			public const string CrawlerPageSize = "CrawlerPageSize";

			public const string EnumerateMessagesPageSize = "EnumerateMessagesPageSize";

			public const string MaxFolderOpened = "MaxFolderOpened";

			public const string CrawlAndCopyFolderTimeout = "CrawlAndCopyFolderTimeout";

			public const string CopyInferenceProperties = "CopyInferenceProperties";

			public const string MrsBindingMaxMessageSize = "MrsBindingMaxMessageSize";

			public const string DCNameValidityInterval = "DCNameValidityInterval";

			public const string DeactivateJobInterval = "DeactivateJobInterval";

			public const string ActivatedJobIncrementalSyncInterval = "ActivatedJobIncrementalSyncInterval";

			public const string AllAggregationSyncJobsInteractive = "AllAggregationSyncJobsInteractive";

			public const string GetActionsPageSize = "GetActionsPageSize";

			public const string ReconnectAbandonInterval = "ReconnectAbandonInterval";

			public const string DisableAutomaticRepair = "DisableAutomaticRepair";

			public const string AutomaticRepairAbandonInterval = "AutomaticRepairAbandonInterval";

			public const string AdditionalSidsForMrsProxyAuthorization = "AdditionalSidsForMrsProxyAuthorization";

			public const string ProxyClientTrustedCertificateThumbprints = "ProxyClientTrustedCertificateThumbprints";

			public const string AllowRestoreFromConnectedMailbox = "AllowRestoreFromConnectedMailbox";

			public const string CheckMailUserPlanQuotasForOnboarding = "CheckMailUserPlanQuotasForOnboarding";

			public const string CacheJobQueues = "CacheJobQueues";

			public const string StalledByHigherPriorityJobsTimeout = "StalledByHigherPriorityJobsTimeout";

			public const string CanStoreCreatePFDumpster = "CanStoreCreatePFDumpster";

			public const string DisableContactSync = "DisableContactSync";

			public const string QuarantineEnabled = "QuarantineEnabled";

			public const string ServerBusyBackoffExpired = "ServerBusyBackoffExpired";

			public const string CanExportFoldersInBatch = "CanExportFoldersInBatch";

			public const string ChangesPerIcsManifestPage = "ChangesPerIcsManifestPage";

			public const string FoldersPerHierarchySyncBatch = "FoldersPerHierarchySyncBatch";

			public const string ProxyClientCertificateSubject = "ProxyClientCertificateSubject";

			public const string ProxyServiceCertificateEndpointEnabled = "ProxyServiceCertificateEndpointEnabled";

			public const string CrossResourceForestEnabled = "CrossResourceForestEnabled";

			public const string WlmWorkloadType = "WlmWorkloadType";

			public const string OwnerLogonToMergeDestination = "OwnerLogonToMergeDestination";

			public const string DisabledFeatures = "DisabledFeatures";
		}

		[Serializable]
		public static class Scope
		{
			public const string RequestWorkloadType = "RequestWorkloadType";

			public const string WlmHealthMonitor = "WlmHealthMonitor";

			public const string FailureType = "FailureType";

			public const string WorkloadType = "WorkloadType";

			public const string RequestType = "RequestType";

			public const string SyncProtocol = "SyncProtocol";
		}

		[Serializable]
		private class MrsScopeSchema : ExchangeConfigurationSection
		{
			[ConfigurationProperty("RequestWorkloadType", DefaultValue = RequestWorkloadType.None)]
			public RequestWorkloadType RequestWorkloadType
			{
				get
				{
					return this.InternalGetConfig<RequestWorkloadType>("RequestWorkloadType");
				}
				set
				{
					this.InternalSetConfig<RequestWorkloadType>(value, "RequestWorkloadType");
				}
			}

			[ConfigurationProperty("WlmHealthMonitor", DefaultValue = "")]
			public string WlmHealthMonitor
			{
				get
				{
					return this.InternalGetConfig<string>("WlmHealthMonitor");
				}
				set
				{
					this.InternalSetConfig<string>(value, "WlmHealthMonitor");
				}
			}

			[ConfigurationProperty("FailureType", DefaultValue = "")]
			public string FailureType
			{
				get
				{
					return this.InternalGetConfig<string>("FailureType");
				}
				set
				{
					this.InternalSetConfig<string>(value, "FailureType");
				}
			}

			[ConfigurationProperty("WorkloadType", DefaultValue = WorkloadType.MailboxReplicationService)]
			public WorkloadType WorkloadType
			{
				get
				{
					return this.InternalGetConfig<WorkloadType>("WorkloadType");
				}
				set
				{
					this.InternalSetConfig<WorkloadType>(value, "WorkloadType");
				}
			}

			[ConfigurationProperty("RequestType", DefaultValue = MRSRequestType.Move)]
			public MRSRequestType RequestType
			{
				get
				{
					return this.InternalGetConfig<MRSRequestType>("RequestType");
				}
				set
				{
					this.InternalSetConfig<MRSRequestType>(value, "RequestType");
				}
			}

			[ConfigurationProperty("SyncProtocol", DefaultValue = SyncProtocol.None)]
			public SyncProtocol SyncProtocol
			{
				get
				{
					return this.InternalGetConfig<SyncProtocol>("SyncProtocol");
				}
				set
				{
					this.InternalSetConfig<SyncProtocol>(value, "SyncProtocol");
				}
			}
		}
	}
}
