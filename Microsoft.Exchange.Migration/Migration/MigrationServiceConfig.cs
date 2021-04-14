using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MigrationServiceConfig : ConfigBase<MigrationServiceConfigSchema>
	{
		internal const string IsServiceletEnabledKey = "SyncMigrationIsEnabled";

		internal const string PollingBatchSizeKey = "SyncMigrationPollingBatchSize";

		internal const string InitialSyncPollingIntervalKey = "SyncMigrationInitialSyncStartPollingTimeout";

		internal const string IncrementalSyncPollingIntervalKey = "SyncMigrationPollingTimeout";

		internal const string ReportIntervalKey = "ReportInterval";

		internal const string ReportMaxAttachmentSize = "ReportMaxAttachmentSize";

		internal const string ReportInitial = "ReportInitial";

		internal const string InitialSyncSubscriptionTimeoutKey = "SyncMigrationInitialSyncTimeOutForFailingSubscriptions";

		internal const string MRSInitialSyncSubscriptionTimeoutKey = "MRSInitialSyncSubscriptionTimeout";

		internal const string IncrementalSyncSubscriptionTimeoutKey = "SyncMigrationTimeOutForFailingSubscriptions";

		internal const string LazyCountRescanPollingIntervalKey = "SyncMigrationLazyCountRescanPollingTimeout";

		internal const string CancellationBatchSizeKey = "SyncMigrationCancellationBatchSize";

		internal const string TransitionBatchSizeKey = "TransitionBatchSize";

		internal const string ProcessingBatchSizeKey = "ProcessingBatchSize";

		internal const string ProcessingSessionSizeKey = "ProcessingSessionSize";

		internal const string ProcessorIdleRunDelayKey = "SyncMigrationProcessorIdleRunDelay";

		internal const string ProcessorActiveRunDelayKey = "SyncMigrationProcessorActiveRunDelay";

		internal const string ProcessorTransientErrorRunDelayKey = "SyncMigrationProcessorTransientErrorRunDelay";

		internal const string ProcessorMaxWaitingJobDelay = "SyncMigrationProcessorMaxWaitingJobDelay";

		internal const string ProcessorAverageWaitingJobDelay = "SyncMigrationProcessorAverageWaitingJobDelay";

		internal const string ProcessorSyncedJobItemDelay = "SyncMigrationProcessorSyncedJobItemDelay";

		internal const string ProcessorMinWaitingJobDelay = "SyncMigrationProcessorMinWaitingJobDelay";

		internal const string MigrationServiceRpcSkeletonMaxThreadsKey = "SyncMigrationServiceRpcSkeletonMaxThreads";

		internal const string MigrationNotificationRpcSkeletonMaxThreadsKey = "SyncMigrationNotificationRpcSkeletonMaxThreads";

		internal const string ProvisioningMaxNumThreads = "ProvisioningMaxNumThreads";

		internal const string IsMigrationSourceMailboxLegacyExchangeDNStampingEnabledKey = "MigrationSourceMailboxLegacyExchangeDNStampingEnabled";

		internal const string MigrationDelayedSubscriptionThresholdKey = "MigrationDelayedSubscriptionThreshold";

		internal const string MaxConcurrentMigrationsKey = "MaxConcurrentMigrations";

		internal const string MigrationProxyRpcEndpointMaxConcurrentRpcCountKey = "MigrationProxyRpcEndpointMaxConcurrentRpcCount";

		internal const string MaxRowsToProcessInOnePassKey = "MaxRowsToProcessInOnePass";

		internal const string MaxTimeToProcessInOnePassKey = "MaxTimeToProcessInOnePass";

		internal const string MaxJobItemsToProcessForReportGeneration = "SyncMigrationMaxJobItemsToProcessForReportGeneration";

		internal const string MigrationUseDKMForEncryption = "MigrationUseDKMForEncryption";

		internal const string MaxItemsToProvisionInOnePassKey = "MaxItemsToProvisionInOnePass";

		internal const string MigrationSourceExchangeMailboxMaximumCountKey = "MigrationSourceExchangeMailboxMaximumCount";

		internal const string MigrationSourceExchangeRecipientMaximumCountKey = "MigrationSourceExchangeRecipientMaximumCount";

		internal const string MigrationSourceStagedExchangeCSVMailboxMaximumCountKey = "MigrationSourceStagedExchangeCSVMailboxMaximumCount";

		internal const string MigrationMaximumJobItemsPerBatch = "MigrationMaximumJobItemsPerBatch";

		internal const string MigrationPoisonedCountThresholdKey = "MigrationPoisonedCountThreshold";

		internal const string MigrationTransientErrorCountThresholdKey = "MigrationTransientErrorCountThreshold";

		internal const string MigrationTransientErrorIntervalThresholdKey = "MigrationTransientErrorIntervalThreshold";

		internal const string FailureRatioForAutoCancel = "SyncMigrationFailureRatioForAutoCancel";

		internal const string AbsoluteFailureCountForAutoCancel = "SyncMigrationAbsoluteFailureCountForAutoCancel";

		internal const string MinimumFailureCountForAutoCancel = "SyncMigrationMinimumFailureCountForAutoCancel";

		internal const string MaxNumberOfMailEnabledPublicFoldersToProcessInOnePassKey = "MaxNumberOfMailEnabledPublicFoldersToProcessInOnePass";

		internal const string IMAPSessionVersionKey = "IMAPSessionVersion";

		internal const string ExchangeSessionVersionKey = "ExchangeSessionVersion";

		internal const string BulkProvisioningSessionVersionKey = "BulkProvisioningSessionVersion";

		internal const string LocalMoveSessionVersionKey = "LocalMoveSessionVersion";

		internal const string RemoteMoveSessionVersionKey = "RemoteMoveSessionVersion";

		internal const string SessionCurrentVersionKey = "SessionCurrentVersion";

		internal const string SyncMigrationEnabledMigrationsTypesKey = "SyncMigrationEnabledMigrationTypes";

		internal const string MigrationSlowOperationThreshold = "MigrationSlowOperationThreshold";

		internal const string MigrationNspiPortKey = "MigrationSourceNspiHttpPort";

		internal const string MigrationNspiRfrPortKey = "MigrationSourceRfrHttpPort";

		internal const string MigrationGroupMembersBatchSizeKey = "MigrationGroupMembersBatchSize";

		internal const string MaximumNumberOfBatchesPerSessionKey = "MaximumNumberOfBatchesPerSession";

		internal const string MigrationReportingLoggingEnabledKey = "MigrationReportingLoggingEnabled";

		internal const string MigrationReportingLoggingFolderKey = "MigrationReportingLoggingFolder";

		internal const string MigrationReportingMaxLogAgeKey = "MigrationReportingMaxLogAge";

		internal const string MigrationReportingJobMaxDirSizeKey = "MigrationReportingJobMaxDirSize";

		internal const string MigrationReportingJobItemMaxDirSizeKey = "MigrationReportingJobItemMaxDirSize";

		internal const string MigrationReportingEndpointMaxDirSizeKey = "MigrationReportingEndpointMaxDirSizeKey";

		internal const string MigrationReportingJobMaxFileSizeKey = "MigrationReportingJobMaxFileSize";

		internal const string MigrationReportingJobItemMaxFileSizeKey = "MigrationReportingJobItemMaxFileSize";

		internal const string MigrationReportingEndpointMaxFileSizeKey = "MigrationReportingEndpointMaxFileSize";

		internal const string MigrationErrorTransitionThresholdKey = "MigrationErrorTransitionThreshold";

		internal const string MigrationUpgradeConstraintExpirationPeriod = "MigrationUpgradeConstraintExpirationPeriod";

		internal const string MigrationUpgradeConstraintEnforcementPeriod = "MigrationUpgradeConstraintEnforcementPeriod";

		internal const string MigrationSuspendedCacheEntryDelay = "SuspendedCacheEntryDelay";

		internal const string BlockedMigrationFeatures = "BlockedMigrationFeatures";

		internal const string PublishedMigrationFeatures = "PublishedMigrationFeatures";

		internal const string UseAsyncNotificationsKey = "MigrationAsyncNotificationEnabled";

		internal const string MigrationJobStoppedThresholdKey = "MigrationJobStoppedThreshold";

		internal const string MigrationJobInactiveThresholdKey = "MigrationJobInactiveThreshold";

		internal const string EndpointCountsRefreshThresholdKey = "EndpointCountsRefreshThreshold";

		internal const string CacheEntrySuspendedDurationKey = "CacheEntrySuspendedDuration";

		internal const string IssueCacheIsEnabledKey = "IssueCacheIsEnabled";

		internal const string IssueCacheScanFrequencyKey = "IssueCacheScanFrequency";

		internal const string IssueCacheItemLimitKey = "IssueCacheItemLimit";

		internal const string MigrationIncrementalSyncFailureThreshold = "MigrationIncrementalSyncFailureThreshold";

		internal const string MigrationPublicFolderCompletionFailureThreshold = "MigrationPublicFolderCompletionFailureThreshold";

		internal const string MaxReportItemsPerJob = "MaxReportItemsPerJob";

		internal const string SendGenericWatsonKey = "SendGenericWatson";
	}
}
