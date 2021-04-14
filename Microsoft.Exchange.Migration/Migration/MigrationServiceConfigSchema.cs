using System;
using System.Configuration;
using System.IO;
using Microsoft.Exchange.Data.ConfigurationSettings;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Migration.Logging;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MigrationServiceConfigSchema : ConfigSchemaBase
	{
		public MigrationServiceConfigSchema()
		{
			string text = Path.GetDirectoryName(MigrationLog.DefaultLogPath);
			text += "\\MigrationReports";
			base.SetDefaultConfigValue<string>("MigrationReportingLoggingFolder", text);
			base.SetDefaultConfigValue<string>("SyncMigrationEnabledMigrationTypes", (MigrationType.IMAP | MigrationType.ExchangeOutlookAnywhere | MigrationType.ExchangeRemoteMove | MigrationType.ExchangeLocalMove | MigrationType.PublicFolder).ToString());
			if (CommonUtils.IsMultiTenantEnabled())
			{
				base.SetDefaultConfigValue<bool>("MigrationReportingLoggingEnabled", true);
			}
		}

		public override string Name
		{
			get
			{
				return "MigrationService";
			}
		}

		[ConfigurationProperty("SyncMigrationIsEnabled", DefaultValue = true)]
		public bool IsServiceletEnabled
		{
			get
			{
				return this.InternalGetConfig<bool>("IsServiceletEnabled");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "IsServiceletEnabled");
			}
		}

		[ConfigurationProperty("SyncMigrationPollingBatchSize", DefaultValue = 10)]
		[IntegerValidator(MinValue = 0, MaxValue = 256, ExcludeRange = false)]
		public int PollingBatchSize
		{
			get
			{
				return this.InternalGetConfig<int>("PollingBatchSize");
			}
			set
			{
				this.InternalSetConfig<int>(value, "PollingBatchSize");
			}
		}

		[TimeSpanValidator(MinValueString = "00:00:00", MaxValueString = "1.0:00:00", ExcludeRange = false)]
		[ConfigurationProperty("SyncMigrationInitialSyncStartPollingTimeout", DefaultValue = "00:05:00")]
		public TimeSpan InitialSyncPollingInterval
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("InitialSyncPollingInterval");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "InitialSyncPollingInterval");
			}
		}

		[TimeSpanValidator(MinValueString = "00:00:00", MaxValueString = "7.00:00:00", ExcludeRange = false)]
		[ConfigurationProperty("SyncMigrationPollingTimeout", DefaultValue = "1.0:00:00")]
		public TimeSpan IncrementalSyncPollingInterval
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("IncrementalSyncPollingInterval");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "IncrementalSyncPollingInterval");
			}
		}

		[TimeSpanValidator(MinValueString = "00:00:00", MaxValueString = "365.00:00:00", ExcludeRange = false)]
		[ConfigurationProperty("ReportInterval", DefaultValue = "1.0:00:00")]
		public TimeSpan ReportInterval
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("ReportInterval");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "ReportInterval");
			}
		}

		[ConfigurationProperty("ReportMaxAttachmentSize", DefaultValue = 1073741824)]
		[IntegerValidator(MinValue = 0, ExcludeRange = false)]
		public int ReportMaxAttachmentSize
		{
			get
			{
				return this.InternalGetConfig<int>("ReportMaxAttachmentSize");
			}
			set
			{
				this.InternalSetConfig<int>(value, "ReportMaxAttachmentSize");
			}
		}

		[TimeSpanValidator(MinValueString = "00:00:00", MaxValueString = "7.00:00:00", ExcludeRange = false)]
		[ConfigurationProperty("SyncMigrationInitialSyncTimeOutForFailingSubscriptions", DefaultValue = "01:20:00")]
		public TimeSpan InitialSyncSubscriptionTimeout
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("InitialSyncSubscriptionTimeout");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "InitialSyncSubscriptionTimeout");
			}
		}

		[TimeSpanValidator(MinValueString = "00:00:00", MaxValueString = "7.00:00:00", ExcludeRange = false)]
		[ConfigurationProperty("MRSInitialSyncSubscriptionTimeout", DefaultValue = "05:00:00")]
		public TimeSpan MRSInitialSyncSubscriptionTimeout
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("MRSInitialSyncSubscriptionTimeout");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "MRSInitialSyncSubscriptionTimeout");
			}
		}

		[TimeSpanValidator(MinValueString = "00:00:00", MaxValueString = "365.00:00:00", ExcludeRange = false)]
		[ConfigurationProperty("SyncMigrationTimeOutForFailingSubscriptions", DefaultValue = "365.00:00:00")]
		public TimeSpan IncrementalSyncSubscriptionTimeout
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("IncrementalSyncSubscriptionTimeout");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "IncrementalSyncSubscriptionTimeout");
			}
		}

		[TimeSpanValidator(MinValueString = "-00:00:01", MaxValueString = "1.00:00:00", ExcludeRange = false)]
		[ConfigurationProperty("SyncMigrationLazyCountRescanPollingTimeout", DefaultValue = "00:10:00")]
		public TimeSpan LazyCountRescanPollingInterval
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("LazyCountRescanPollingInterval");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "LazyCountRescanPollingInterval");
			}
		}

		[ConfigurationProperty("SyncMigrationCancellationBatchSize", DefaultValue = 100)]
		[IntegerValidator(MinValue = 1, MaxValue = 100000, ExcludeRange = false)]
		public int CancellationBatchSize
		{
			get
			{
				return this.InternalGetConfig<int>("CancellationBatchSize");
			}
			set
			{
				this.InternalSetConfig<int>(value, "CancellationBatchSize");
			}
		}

		[ConfigurationProperty("TransitionBatchSize", DefaultValue = 75)]
		[IntegerValidator(MinValue = 1, MaxValue = 100000, ExcludeRange = false)]
		public int TransitionBatchSize
		{
			get
			{
				return this.InternalGetConfig<int>("TransitionBatchSize");
			}
			set
			{
				this.InternalSetConfig<int>(value, "TransitionBatchSize");
			}
		}

		[ConfigurationProperty("ProcessingBatchSize", DefaultValue = 100)]
		[IntegerValidator(MinValue = 1, MaxValue = 100000, ExcludeRange = false)]
		public int ProcessingBatchSize
		{
			get
			{
				return this.InternalGetConfig<int>("ProcessingBatchSize");
			}
			set
			{
				this.InternalSetConfig<int>(value, "ProcessingBatchSize");
			}
		}

		[ConfigurationProperty("ProcessingSessionSize", DefaultValue = 5)]
		[IntegerValidator(MinValue = 1, MaxValue = 100000, ExcludeRange = false)]
		public int ProcessingSessionSize
		{
			get
			{
				return this.InternalGetConfig<int>("ProcessingSessionSize");
			}
			set
			{
				this.InternalSetConfig<int>(value, "ProcessingSessionSize");
			}
		}

		[TimeSpanValidator(MinValueString = "00:00:00", MaxValueString = "7.00:00:00", ExcludeRange = false)]
		[ConfigurationProperty("SyncMigrationProcessorIdleRunDelay", DefaultValue = "00:00:30")]
		public TimeSpan ProcessorIdleRunDelay
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("ProcessorIdleRunDelay");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "ProcessorIdleRunDelay");
			}
		}

		[TimeSpanValidator(MinValueString = "00:00:00", MaxValueString = "7.00:00:00", ExcludeRange = false)]
		[ConfigurationProperty("SyncMigrationProcessorActiveRunDelay", DefaultValue = "00:00:30")]
		public TimeSpan ProcessorActiveRunDelay
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("ProcessorActiveRunDelay");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "ProcessorActiveRunDelay");
			}
		}

		[TimeSpanValidator(MinValueString = "00:00:00", MaxValueString = "7.00:00:00", ExcludeRange = false)]
		[ConfigurationProperty("SyncMigrationProcessorTransientErrorRunDelay", DefaultValue = "00:00:30")]
		public TimeSpan ProcessorTransientErrorRunDelay
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("ProcessorTransientErrorRunDelay");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "ProcessorTransientErrorRunDelay");
			}
		}

		[TimeSpanValidator(MinValueString = "00:00:00", MaxValueString = "12:00:00", ExcludeRange = false)]
		[ConfigurationProperty("SyncMigrationProcessorMaxWaitingJobDelay", DefaultValue = "00:30:00")]
		public TimeSpan ProcessorMaxWaitingJobDelay
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("ProcessorMaxWaitingJobDelay");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "ProcessorMaxWaitingJobDelay");
			}
		}

		[ConfigurationProperty("SyncMigrationProcessorAverageWaitingJobDelay", DefaultValue = "00:02:00")]
		[TimeSpanValidator(MinValueString = "00:00:00", MaxValueString = "12:00:00", ExcludeRange = false)]
		public TimeSpan ProcessorAverageWaitingJobDelay
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("ProcessorAverageWaitingJobDelay");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "ProcessorAverageWaitingJobDelay");
			}
		}

		[TimeSpanValidator(MinValueString = "00:00:00", MaxValueString = "12:00:00", ExcludeRange = false)]
		[ConfigurationProperty("SyncMigrationProcessorSyncedJobItemDelay", DefaultValue = "00:15:00")]
		public TimeSpan ProcessorSyncedJobItemDelay
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("ProcessorSyncedJobItemDelay");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "ProcessorSyncedJobItemDelay");
			}
		}

		[IntegerValidator(MinValue = 1, MaxValue = 512, ExcludeRange = false)]
		[ConfigurationProperty("SyncMigrationServiceRpcSkeletonMaxThreads", DefaultValue = 8)]
		public int MigrationServiceRpcSkeletonMaxThreads
		{
			get
			{
				return this.InternalGetConfig<int>("MigrationServiceRpcSkeletonMaxThreads");
			}
			set
			{
				this.InternalSetConfig<int>(value, "MigrationServiceRpcSkeletonMaxThreads");
			}
		}

		[ConfigurationProperty("SyncMigrationNotificationRpcSkeletonMaxThreads", DefaultValue = 8)]
		[IntegerValidator(MinValue = 1, MaxValue = 512, ExcludeRange = false)]
		public int MigrationNotificationRpcSkeletonMaxThreads
		{
			get
			{
				return this.InternalGetConfig<int>("MigrationNotificationRpcSkeletonMaxThreads");
			}
			set
			{
				this.InternalSetConfig<int>(value, "MigrationNotificationRpcSkeletonMaxThreads");
			}
		}

		[ConfigurationProperty("ProvisioningMaxNumThreads", DefaultValue = 5)]
		[IntegerValidator(MinValue = 1, MaxValue = 50, ExcludeRange = false)]
		public int ProvisioningMaxNumThreads
		{
			get
			{
				return this.InternalGetConfig<int>("ProvisioningMaxNumThreads");
			}
			set
			{
				this.InternalSetConfig<int>(value, "ProvisioningMaxNumThreads");
			}
		}

		[ConfigurationProperty("MigrationSourceMailboxLegacyExchangeDNStampingEnabled", DefaultValue = true)]
		public bool IsMigrationSourceMailboxLegacyExchangeDNStampingEnabled
		{
			get
			{
				return this.InternalGetConfig<bool>("IsMigrationSourceMailboxLegacyExchangeDNStampingEnabled");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "IsMigrationSourceMailboxLegacyExchangeDNStampingEnabled");
			}
		}

		[TimeSpanValidator(MinValueString = "00:00:01", MaxValueString = "365.00:00:00", ExcludeRange = false)]
		[ConfigurationProperty("MigrationDelayedSubscriptionThreshold", DefaultValue = "04:00:00")]
		public TimeSpan MigrationDelayedSubscriptionThreshold
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("MigrationDelayedSubscriptionThreshold");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "MigrationDelayedSubscriptionThreshold");
			}
		}

		[ConfigurationProperty("MaxConcurrentMigrations", DefaultValue = 10)]
		[IntegerValidator(MinValue = 0, MaxValue = 256, ExcludeRange = false)]
		public int MaxConcurrentMigrations
		{
			get
			{
				return this.InternalGetConfig<int>("MaxConcurrentMigrations");
			}
			set
			{
				this.InternalSetConfig<int>(value, "MaxConcurrentMigrations");
			}
		}

		[IntegerValidator(MinValue = 0, MaxValue = 1000, ExcludeRange = false)]
		[ConfigurationProperty("MigrationProxyRpcEndpointMaxConcurrentRpcCount", DefaultValue = 100)]
		public int MigrationProxyRpcEndpointMaxConcurrentRpcCount
		{
			get
			{
				return this.InternalGetConfig<int>("MigrationProxyRpcEndpointMaxConcurrentRpcCount");
			}
			set
			{
				this.InternalSetConfig<int>(value, "MigrationProxyRpcEndpointMaxConcurrentRpcCount");
			}
		}

		[ConfigurationProperty("MaxRowsToProcessInOnePass", DefaultValue = 500)]
		[IntegerValidator(MinValue = 0, MaxValue = 100000, ExcludeRange = false)]
		public int MaxRowsToProcessInOnePass
		{
			get
			{
				return this.InternalGetConfig<int>("MaxRowsToProcessInOnePass");
			}
			set
			{
				this.InternalSetConfig<int>(value, "MaxRowsToProcessInOnePass");
			}
		}

		[TimeSpanValidator(MinValueString = "00:00:05", MaxValueString = "1.00:00:00", ExcludeRange = false)]
		[ConfigurationProperty("MaxTimeToProcessInOnePass", DefaultValue = "00:08:00")]
		public TimeSpan MaxTimeToProcessInOnePass
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("MaxTimeToProcessInOnePass");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "MaxTimeToProcessInOnePass");
			}
		}

		[ConfigurationProperty("SyncMigrationMaxJobItemsToProcessForReportGeneration", DefaultValue = 5000)]
		[IntegerValidator(MinValue = 1, MaxValue = 100000, ExcludeRange = false)]
		public int MaxJobItemsToProcessForReportGeneration
		{
			get
			{
				return this.InternalGetConfig<int>("MaxJobItemsToProcessForReportGeneration");
			}
			set
			{
				this.InternalSetConfig<int>(value, "MaxJobItemsToProcessForReportGeneration");
			}
		}

		[IntegerValidator(MinValue = 0, MaxValue = 2, ExcludeRange = false)]
		[ConfigurationProperty("MigrationUseDKMForEncryption", DefaultValue = 0)]
		public int MigrationUseDKMForEncryption
		{
			get
			{
				return this.InternalGetConfig<int>("MigrationUseDKMForEncryption");
			}
			set
			{
				this.InternalSetConfig<int>(value, "MigrationUseDKMForEncryption");
			}
		}

		[ConfigurationProperty("MaxItemsToProvisionInOnePass", DefaultValue = 10)]
		[IntegerValidator(MinValue = 0, MaxValue = 100, ExcludeRange = false)]
		public int MaxItemsToProvisionInOnePass
		{
			get
			{
				return this.InternalGetConfig<int>("MaxItemsToProvisionInOnePass");
			}
			set
			{
				this.InternalSetConfig<int>(value, "MaxItemsToProvisionInOnePass");
			}
		}

		[ConfigurationProperty("MigrationSourceExchangeMailboxMaximumCount", DefaultValue = 2000)]
		[IntegerValidator(MinValue = 0, MaxValue = 100000, ExcludeRange = false)]
		public int MigrationSourceExchangeMailboxMaximumCount
		{
			get
			{
				return this.InternalGetConfig<int>("MigrationSourceExchangeMailboxMaximumCount");
			}
			set
			{
				this.InternalSetConfig<int>(value, "MigrationSourceExchangeMailboxMaximumCount");
			}
		}

		[ConfigurationProperty("MigrationSourceExchangeRecipientMaximumCount", DefaultValue = 50000)]
		[IntegerValidator(MinValue = 0, MaxValue = 100000, ExcludeRange = false)]
		public int MigrationSourceExchangeRecipientMaximumCount
		{
			get
			{
				return this.InternalGetConfig<int>("MigrationSourceExchangeRecipientMaximumCount");
			}
			set
			{
				this.InternalSetConfig<int>(value, "MigrationSourceExchangeRecipientMaximumCount");
			}
		}

		[IntegerValidator(MinValue = 0, MaxValue = 100000, ExcludeRange = false)]
		[ConfigurationProperty("MigrationSourceStagedExchangeCSVMailboxMaximumCount", DefaultValue = 10000)]
		public int MigrationSourceStagedExchangeCSVMailboxMaximumCount
		{
			get
			{
				return this.InternalGetConfig<int>("MigrationSourceStagedExchangeCSVMailboxMaximumCount");
			}
			set
			{
				this.InternalSetConfig<int>(value, "MigrationSourceStagedExchangeCSVMailboxMaximumCount");
			}
		}

		[IntegerValidator(MinValue = 1, MaxValue = 100000, ExcludeRange = false)]
		[ConfigurationProperty("MigrationPoisonedCountThreshold", DefaultValue = 5)]
		public int MigrationPoisonedCountThreshold
		{
			get
			{
				return this.InternalGetConfig<int>("MigrationPoisonedCountThreshold");
			}
			set
			{
				this.InternalSetConfig<int>(value, "MigrationPoisonedCountThreshold");
			}
		}

		[ConfigurationProperty("MigrationTransientErrorCountThreshold", DefaultValue = 10)]
		[IntegerValidator(MinValue = 0, MaxValue = 100000, ExcludeRange = false)]
		public int MigrationTransientErrorCountThreshold
		{
			get
			{
				return this.InternalGetConfig<int>("MigrationTransientErrorCountThreshold");
			}
			set
			{
				this.InternalSetConfig<int>(value, "MigrationTransientErrorCountThreshold");
			}
		}

		[ConfigurationProperty("SyncMigrationProcessorMinWaitingJobDelay", DefaultValue = "00:00:10")]
		[TimeSpanValidator(MinValueString = "00:00:00", MaxValueString = "12:00:00", ExcludeRange = false)]
		public TimeSpan ProcessorMinWaitingJobDelay
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("ProcessorMinWaitingJobDelay");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "ProcessorMinWaitingJobDelay");
			}
		}

		[TimeSpanValidator(MinValueString = "00:00:00", ExcludeRange = false)]
		[ConfigurationProperty("MigrationTransientErrorIntervalThreshold", DefaultValue = "00:30:00")]
		public TimeSpan MigrationTransientErrorIntervalThreshold
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("MigrationTransientErrorIntervalThreshold");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "MigrationTransientErrorIntervalThreshold");
			}
		}

		[IntegerValidator(MinValue = 0, MaxValue = 100, ExcludeRange = false)]
		[ConfigurationProperty("SyncMigrationFailureRatioForAutoCancel", DefaultValue = 10)]
		public int FailureRatioForAutoCancel
		{
			get
			{
				return this.InternalGetConfig<int>("FailureRatioForAutoCancel");
			}
			set
			{
				this.InternalSetConfig<int>(value, "FailureRatioForAutoCancel");
			}
		}

		[ConfigurationProperty("SyncMigrationAbsoluteFailureCountForAutoCancel", DefaultValue = 10000)]
		[IntegerValidator(MinValue = 1, MaxValue = 100000, ExcludeRange = false)]
		public int AbsoluteFailureCountForAutoCancel
		{
			get
			{
				return this.InternalGetConfig<int>("AbsoluteFailureCountForAutoCancel");
			}
			set
			{
				this.InternalSetConfig<int>(value, "AbsoluteFailureCountForAutoCancel");
			}
		}

		[IntegerValidator(MinValue = 1, MaxValue = 100000, ExcludeRange = false)]
		[ConfigurationProperty("SyncMigrationMinimumFailureCountForAutoCancel", DefaultValue = 500)]
		public int MinimumFailureCountForAutoCancel
		{
			get
			{
				return this.InternalGetConfig<int>("MinimumFailureCountForAutoCancel");
			}
			set
			{
				this.InternalSetConfig<int>(value, "MinimumFailureCountForAutoCancel");
			}
		}

		[IntegerValidator(MinValue = 1, MaxValue = 1000, ExcludeRange = false)]
		[ConfigurationProperty("MaxNumberOfMailEnabledPublicFoldersToProcessInOnePass", DefaultValue = 1)]
		public int MaxNumberOfMailEnabledPublicFoldersToProcessInOnePass
		{
			get
			{
				return this.InternalGetConfig<int>("MaxNumberOfMailEnabledPublicFoldersToProcessInOnePass");
			}
			set
			{
				this.InternalSetConfig<int>(value, "MaxNumberOfMailEnabledPublicFoldersToProcessInOnePass");
			}
		}

		[ConfigurationProperty("IMAPSessionVersion", DefaultValue = 5L)]
		[LongValidator(MinValue = 1L, MaxValue = 5L, ExcludeRange = false)]
		public long IMAPSessionVersion
		{
			get
			{
				return this.InternalGetConfig<long>("IMAPSessionVersion");
			}
			set
			{
				this.InternalSetConfig<long>(value, "IMAPSessionVersion");
			}
		}

		[ConfigurationProperty("MigrationMaximumJobItemsPerBatch", DefaultValue = 200000)]
		[IntegerValidator(MinValue = 0, ExcludeRange = false)]
		public int MigrationMaximumJobItemsPerBatch
		{
			get
			{
				return this.InternalGetConfig<int>("MigrationMaximumJobItemsPerBatch");
			}
			set
			{
				this.InternalSetConfig<int>(value, "MigrationMaximumJobItemsPerBatch");
			}
		}

		[LongValidator(MinValue = 1L, MaxValue = 5L, ExcludeRange = false)]
		[ConfigurationProperty("ExchangeSessionVersion", DefaultValue = 5L)]
		public long ExchangeSessionVersion
		{
			get
			{
				return this.InternalGetConfig<long>("ExchangeSessionVersion");
			}
			set
			{
				this.InternalSetConfig<long>(value, "ExchangeSessionVersion");
			}
		}

		[LongValidator(MinValue = 1L, MaxValue = 5L, ExcludeRange = false)]
		[ConfigurationProperty("BulkProvisioningSessionVersion", DefaultValue = 1L)]
		public long BulkProvisioningSessionVersion
		{
			get
			{
				return this.InternalGetConfig<long>("BulkProvisioningSessionVersion");
			}
			set
			{
				this.InternalSetConfig<long>(value, "BulkProvisioningSessionVersion");
			}
		}

		[LongValidator(MinValue = 4L, MaxValue = 5L, ExcludeRange = false)]
		[ConfigurationProperty("LocalMoveSessionVersion", DefaultValue = 4L)]
		public long LocalMoveSessionVersion
		{
			get
			{
				return this.InternalGetConfig<long>("LocalMoveSessionVersion");
			}
			set
			{
				this.InternalSetConfig<long>(value, "LocalMoveSessionVersion");
			}
		}

		[LongValidator(MinValue = 4L, MaxValue = 5L, ExcludeRange = false)]
		[ConfigurationProperty("RemoteMoveSessionVersion", DefaultValue = 4L)]
		public long RemoteMoveSessionVersion
		{
			get
			{
				return this.InternalGetConfig<long>("RemoteMoveSessionVersion");
			}
			set
			{
				this.InternalSetConfig<long>(value, "RemoteMoveSessionVersion");
			}
		}

		[LongValidator(MinValue = 1L, MaxValue = 5L, ExcludeRange = false)]
		[ConfigurationProperty("SessionCurrentVersion", DefaultValue = 2L)]
		public long SessionCurrentVersion
		{
			get
			{
				return this.InternalGetConfig<long>("SessionCurrentVersion");
			}
			set
			{
				this.InternalSetConfig<long>(value, "SessionCurrentVersion");
			}
		}

		[ConfigurationProperty("SyncMigrationEnabledMigrationTypes")]
		public string SyncMigrationEnabledMigrationsTypes
		{
			get
			{
				return this.InternalGetConfig<string>("SyncMigrationEnabledMigrationsTypes");
			}
			set
			{
				this.InternalSetConfig<string>(value, "SyncMigrationEnabledMigrationsTypes");
			}
		}

		[ConfigurationProperty("MigrationSlowOperationThreshold", DefaultValue = "00:05:00")]
		[TimeSpanValidator(MinValueString = "00:00:01", ExcludeRange = false)]
		public TimeSpan MigrationSlowOperationThreshold
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("MigrationSlowOperationThreshold");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "MigrationSlowOperationThreshold");
			}
		}

		[ConfigurationProperty("MigrationSourceNspiHttpPort", DefaultValue = 6004)]
		[IntegerValidator(MinValue = 1, MaxValue = 100000, ExcludeRange = false)]
		public int MigrationNspiPort
		{
			get
			{
				return this.InternalGetConfig<int>("MigrationNspiPort");
			}
			set
			{
				this.InternalSetConfig<int>(value, "MigrationNspiPort");
			}
		}

		[ConfigurationProperty("MigrationSourceRfrHttpPort", DefaultValue = 6002)]
		[IntegerValidator(MinValue = 1, MaxValue = 100000, ExcludeRange = false)]
		public int MigrationNspiRfrPort
		{
			get
			{
				return this.InternalGetConfig<int>("MigrationNspiRfrPort");
			}
			set
			{
				this.InternalSetConfig<int>(value, "MigrationNspiRfrPort");
			}
		}

		[ConfigurationProperty("MigrationGroupMembersBatchSize", DefaultValue = 100)]
		[IntegerValidator(MinValue = 1, MaxValue = 100000, ExcludeRange = false)]
		public int MigrationGroupMembersBatchSize
		{
			get
			{
				return this.InternalGetConfig<int>("MigrationGroupMembersBatchSize");
			}
			set
			{
				this.InternalSetConfig<int>(value, "MigrationGroupMembersBatchSize");
			}
		}

		[IntegerValidator(MinValue = 0, MaxValue = 100000, ExcludeRange = false)]
		[ConfigurationProperty("MaximumNumberOfBatchesPerSession", DefaultValue = 100)]
		public int MaximumNumberOfBatchesPerSession
		{
			get
			{
				return this.InternalGetConfig<int>("MaximumNumberOfBatchesPerSession");
			}
			set
			{
				this.InternalSetConfig<int>(value, "MaximumNumberOfBatchesPerSession");
			}
		}

		[ConfigurationProperty("MigrationReportingLoggingEnabled", DefaultValue = true)]
		public bool MigrationReportingLoggingEnabledKey
		{
			get
			{
				return this.InternalGetConfig<bool>("MigrationReportingLoggingEnabledKey");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "MigrationReportingLoggingEnabledKey");
			}
		}

		[ConfigurationProperty("MigrationReportingLoggingFolder", DefaultValue = "")]
		public string MigrationReportingLoggingFolder
		{
			get
			{
				return this.InternalGetConfig<string>("MigrationReportingLoggingFolder");
			}
			set
			{
				this.InternalSetConfig<string>(value, "MigrationReportingLoggingFolder");
			}
		}

		[ConfigurationProperty("MigrationReportingMaxLogAge", DefaultValue = "10.0:00:00")]
		[TimeSpanValidator(MinValueString = "1.00:00:00", MaxValueString = "180.0:00:00", ExcludeRange = false)]
		public TimeSpan MigrationReportingMaxLogAge
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("MigrationReportingMaxLogAge");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "MigrationReportingMaxLogAge");
			}
		}

		[ConfigurationProperty("MigrationReportingJobMaxDirSize", DefaultValue = 1073741824L)]
		[LongValidator(MinValue = 16777216L, ExcludeRange = false)]
		public long MigrationReportingJobMaxDirSize
		{
			get
			{
				return this.InternalGetConfig<long>("MigrationReportingJobMaxDirSize");
			}
			set
			{
				this.InternalSetConfig<long>(value, "MigrationReportingJobMaxDirSize");
			}
		}

		[ConfigurationProperty("MigrationReportingJobItemMaxDirSize", DefaultValue = 1073741824L)]
		[LongValidator(MinValue = 16777216L, ExcludeRange = false)]
		public long MigrationReportingJobItemMaxDirSize
		{
			get
			{
				return this.InternalGetConfig<long>("MigrationReportingJobItemMaxDirSize");
			}
			set
			{
				this.InternalSetConfig<long>(value, "MigrationReportingJobItemMaxDirSize");
			}
		}

		[LongValidator(MinValue = 16777216L, ExcludeRange = false)]
		[ConfigurationProperty("MigrationReportingEndpointMaxDirSizeKey", DefaultValue = 1073741824L)]
		public long MigrationReportingEndpointMaxDirSize
		{
			get
			{
				return this.InternalGetConfig<long>("MigrationReportingEndpointMaxDirSize");
			}
			set
			{
				this.InternalSetConfig<long>(value, "MigrationReportingEndpointMaxDirSize");
			}
		}

		[ConfigurationProperty("MigrationReportingJobMaxFileSize", DefaultValue = 1073741824L)]
		[LongValidator(MinValue = 8388608L, ExcludeRange = false)]
		public long MigrationReportingJobMaxFileSize
		{
			get
			{
				return this.InternalGetConfig<long>("MigrationReportingJobMaxFileSize");
			}
			set
			{
				this.InternalSetConfig<long>(value, "MigrationReportingJobMaxFileSize");
			}
		}

		[ConfigurationProperty("MigrationReportingJobItemMaxFileSize", DefaultValue = 1073741824L)]
		[LongValidator(MinValue = 8388608L, ExcludeRange = false)]
		public long MigrationReportingJobItemMaxFileSize
		{
			get
			{
				return this.InternalGetConfig<long>("MigrationReportingJobItemMaxFileSize");
			}
			set
			{
				this.InternalSetConfig<long>(value, "MigrationReportingJobItemMaxFileSize");
			}
		}

		[ConfigurationProperty("MigrationReportingEndpointMaxFileSize", DefaultValue = 1073741824L)]
		[LongValidator(MinValue = 8388608L, ExcludeRange = false)]
		public long MigrationReportingEndpointMaxFileSize
		{
			get
			{
				return this.InternalGetConfig<long>("MigrationReportingEndpointMaxFileSize");
			}
			set
			{
				this.InternalSetConfig<long>(value, "MigrationReportingEndpointMaxFileSize");
			}
		}

		[ConfigurationProperty("MigrationErrorTransitionThreshold", DefaultValue = 2)]
		[IntegerValidator(MinValue = 1, MaxValue = 100000, ExcludeRange = false)]
		public int MigrationErrorTransitionThreshold
		{
			get
			{
				return this.InternalGetConfig<int>("MigrationErrorTransitionThreshold");
			}
			set
			{
				this.InternalSetConfig<int>(value, "MigrationErrorTransitionThreshold");
			}
		}

		[ConfigurationProperty("MigrationUpgradeConstraintExpirationPeriod", DefaultValue = "14.0:00:00")]
		[TimeSpanValidator(MinValueString = "00:00:00", ExcludeRange = false)]
		public TimeSpan MigrationUpgradeConstraintExpirationPeriod
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("MigrationUpgradeConstraintExpirationPeriod");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "MigrationUpgradeConstraintExpirationPeriod");
			}
		}

		[ConfigurationProperty("MigrationUpgradeConstraintEnforcementPeriod", DefaultValue = "1.0:00:00")]
		[TimeSpanValidator(MinValueString = "00:00:00", ExcludeRange = false)]
		public TimeSpan MigrationUpgradeConstraintEnforcementPeriod
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("MigrationUpgradeConstraintEnforcementPeriod");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "MigrationUpgradeConstraintEnforcementPeriod");
			}
		}

		[TimeSpanValidator(MinValueString = "00:00:00", ExcludeRange = false)]
		[ConfigurationProperty("SuspendedCacheEntryDelay", DefaultValue = "12:00:00")]
		public TimeSpan MigrationSuspendedCacheEntryDelay
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("MigrationSuspendedCacheEntryDelay");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "MigrationSuspendedCacheEntryDelay");
			}
		}

		[ConfigurationProperty("BlockedMigrationFeatures", DefaultValue = MigrationFeature.None)]
		public MigrationFeature BlockedMigrationFeatures
		{
			get
			{
				return this.InternalGetConfig<MigrationFeature>("BlockedMigrationFeatures");
			}
			set
			{
				this.InternalSetConfig<MigrationFeature>(value, "BlockedMigrationFeatures");
			}
		}

		[ConfigurationProperty("PublishedMigrationFeatures", DefaultValue = MigrationFeature.MultiBatch)]
		public MigrationFeature PublishedMigrationFeatures
		{
			get
			{
				return this.InternalGetConfig<MigrationFeature>("PublishedMigrationFeatures");
			}
			set
			{
				this.InternalSetConfig<MigrationFeature>(value, "PublishedMigrationFeatures");
			}
		}

		[ConfigurationProperty("MigrationAsyncNotificationEnabled", DefaultValue = true)]
		public bool UseAsyncNotifications
		{
			get
			{
				return this.InternalGetConfig<bool>("UseAsyncNotifications");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "UseAsyncNotifications");
			}
		}

		[ConfigurationProperty("MigrationJobStoppedThreshold", DefaultValue = "30.0:00:00")]
		[TimeSpanValidator(MinValueString = "00:00:01", ExcludeRange = false)]
		public TimeSpan MigrationJobStoppedThreshold
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("MigrationJobStoppedThreshold");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "MigrationJobStoppedThreshold");
			}
		}

		[ConfigurationProperty("MigrationJobInactiveThreshold", DefaultValue = "30.0:00:00")]
		[TimeSpanValidator(MinValueString = "00:00:01", ExcludeRange = false)]
		public TimeSpan MigrationJobInactiveThreshold
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("MigrationJobInactiveThreshold");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "MigrationJobInactiveThreshold");
			}
		}

		[TimeSpanValidator(MinValueString = "00:00:00", ExcludeRange = false)]
		[ConfigurationProperty("EndpointCountsRefreshThreshold", DefaultValue = "00:00:30")]
		public TimeSpan EndpointCountsRefreshThreshold
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("EndpointCountsRefreshThreshold");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "EndpointCountsRefreshThreshold");
			}
		}

		[ConfigurationProperty("CacheEntrySuspendedDuration", DefaultValue = "06:00:00")]
		[TimeSpanValidator(MinValueString = "00:00:00", ExcludeRange = false)]
		public TimeSpan CacheEntrySuspendedDuration
		{
			get
			{
				return this.InternalGetConfig<TimeSpan>("CacheEntrySuspendedDuration");
			}
			set
			{
				this.InternalSetConfig<TimeSpan>(value, "CacheEntrySuspendedDuration");
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
		[ConfigurationProperty("IssueCacheScanFrequency", DefaultValue = "02:00:00")]
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

		[IntegerValidator(MinValue = 0, MaxValue = 100000, ExcludeRange = false)]
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

		[IntegerValidator(MinValue = 0, MaxValue = 2147483647, ExcludeRange = false)]
		[ConfigurationProperty("MigrationIncrementalSyncFailureThreshold", DefaultValue = 30)]
		public int MigrationIncrementalSyncFailureThreshold
		{
			get
			{
				return this.InternalGetConfig<int>("MigrationIncrementalSyncFailureThreshold");
			}
			set
			{
				this.InternalSetConfig<int>(value, "MigrationIncrementalSyncFailureThreshold");
			}
		}

		[IntegerValidator(MinValue = 0, MaxValue = 2147483647, ExcludeRange = false)]
		[ConfigurationProperty("MigrationPublicFolderCompletionFailureThreshold", DefaultValue = 5)]
		public int MigrationPublicFolderCompletionFailureThreshold
		{
			get
			{
				return this.InternalGetConfig<int>("MigrationPublicFolderCompletionFailureThreshold");
			}
			set
			{
				this.InternalSetConfig<int>(value, "MigrationPublicFolderCompletionFailureThreshold");
			}
		}

		[IntegerValidator(MinValue = 1, MaxValue = 100000, ExcludeRange = false)]
		[ConfigurationProperty("MaxReportItemsPerJob", DefaultValue = 10)]
		public int MaxReportItemsPerJob
		{
			get
			{
				return this.InternalGetConfig<int>("MaxReportItemsPerJob");
			}
			set
			{
				this.InternalSetConfig<int>(value, "MaxReportItemsPerJob");
			}
		}

		[ConfigurationProperty("SendGenericWatson", DefaultValue = false)]
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

		[ConfigurationProperty("ReportInitial", DefaultValue = true)]
		public bool ReportInitial
		{
			get
			{
				return this.InternalGetConfig<bool>("ReportInitial");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "ReportInitial");
			}
		}

		public const int MaxIntValue = 100000;

		public const long OneGBSize = 1073741824L;

		public const long SixteenMBSize = 16777216L;

		public const long EightMBSize = 8388608L;
	}
}
