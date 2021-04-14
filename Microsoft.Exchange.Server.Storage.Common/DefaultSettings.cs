using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public static class DefaultSettings
	{
		public static DefaultSettings.DefaultSettingsValues Get
		{
			get
			{
				return DefaultSettings.GetInstance();
			}
		}

		private static DefaultSettings.DefaultSettingsValues GetInstance()
		{
			if (DefaultSettings.isTestEnvironment.Value)
			{
				if (StoreEnvironment.IsPerformanceEnvironment)
				{
					return DefaultSettings.performance;
				}
				return DefaultSettings.test;
			}
			else
			{
				if (DefaultSettings.isDogfoodEnvironment.Value)
				{
					return DefaultSettings.dogfood;
				}
				if (DefaultSettings.isDatacenterEnvironment.Value)
				{
					if (DefaultSettings.isSdfEnvironment.Value)
					{
						return DefaultSettings.sdf;
					}
					return DefaultSettings.datacenter;
				}
				else
				{
					if (DefaultSettings.isDedicatedEnvironment.Value)
					{
						return DefaultSettings.dedicated;
					}
					return DefaultSettings.customer;
				}
			}
		}

		private static DefaultSettings.DefaultSettingsValues test = new DefaultSettings.DefaultSettingsValues
		{
			DestinationMailboxReservedCounterRangeConstant = 3000,
			DestinationMailboxReservedCounterRangeGradient = 3,
			DiagnosticsThresholdChunkElapsedTime = TimeSpan.FromSeconds(2.0),
			DiagnosticsThresholdDatabaseTime = TimeSpan.FromSeconds(2.0),
			DiagnosticsThresholdDirectoryCalls = 50,
			DiagnosticsThresholdDirectoryTime = TimeSpan.FromSeconds(2.0),
			DiagnosticsThresholdHeavyActivityRpcCount = 200,
			DiagnosticsThresholdInstantSearchFolderView = TimeSpan.FromMilliseconds(250.0),
			DiagnosticsThresholdInteractionTime = TimeSpan.FromSeconds(2.0),
			DiagnosticsThresholdLockTime = TimeSpan.FromSeconds(2.0),
			DiagnosticsThresholdPagesDirtied = 150,
			DiagnosticsThresholdPagesPreread = 300,
			DiagnosticsThresholdPagesRead = 150,
			DiscoverPotentiallyDisabledMailboxesTaskPeriod = TimeSpan.FromDays(1.0),
			DiscoverPotentiallyExpiredMailboxesTaskPeriod = TimeSpan.FromDays(1.0),
			DumpsterMessagesPerFolderCountReceiveQuota = new UnlimitedItems(3000000L),
			DumpsterMessagesPerFolderCountWarningQuota = new UnlimitedItems(2750000L),
			DynamicSearchFolderPerScopeCountReceiveQuota = 100,
			EnableMaterializedRestriction = true,
			EnableTraceBreadCrumbs = true,
			EnableTraceDiagnosticQuery = true,
			EnableTraceFullTextIndexQuery = true,
			EnableTraceHeavyClientActivity = true,
			EnableTraceLockContention = true,
			EnableTraceLongOperation = true,
			EnableTraceReferenceData = true,
			EnableTraceRopResource = true,
			EnableTraceRopSummary = true,
			EnableTraceSyntheticCounters = true,
			FolderHierarchyChildrenCountReceiveQuota = new UnlimitedItems(10000L),
			FolderHierarchyChildrenCountWarningQuota = new UnlimitedItems(9000L),
			FolderHierarchyDepthReceiveQuota = new UnlimitedItems(300L),
			FolderHierarchyDepthWarningQuota = new UnlimitedItems(250L),
			FoldersCountWarningQuota = UnlimitedItems.UnlimitedValue,
			FoldersCountReceiveQuota = UnlimitedItems.UnlimitedValue,
			IdleAccessibleMailboxesTime = TimeSpan.FromDays(1.0),
			IdleIndexTimeForEmptyFolderOperation = TimeSpan.FromSeconds(10.0),
			MailboxLockMaximumWaitCount = 10,
			MailboxMessagesPerFolderCountReceiveQuota = new UnlimitedItems(1000000L),
			MailboxMessagesPerFolderCountWarningQuota = new UnlimitedItems(900000L),
			MaintenanceControlPeriod = TimeSpan.FromMinutes(30.0),
			MaxChildCountForDumpsterHierarchyPublicFolder = new UnlimitedItems(60L),
			MaximumMountedDatabases = 100,
			MaximumActivePropertyPromotions = 2,
			MaximumNumberOfExceptions = 10000,
			Name = "Test",
			ProcessorNumberOfColumnResults = 3,
			ProcessorNumberOfPropertyResults = 3,
			QueryVersionRefreshInterval = TimeSpan.FromSeconds(15.0),
			RetailAssertOnJetVsom = false,
			RetailAssertOnUnexpectedJetErrors = true,
			RopSummarySlowThreshold = TimeSpan.FromMilliseconds(70.0),
			TraceIntervalRopLockContention = TimeSpan.FromMinutes(5.0),
			TraceIntervalRopResource = TimeSpan.FromMinutes(5.0),
			TraceIntervalRopSummary = TimeSpan.FromMinutes(5.0),
			SearchTraceFastOperationThreshold = TimeSpan.FromMilliseconds(5.0),
			SearchTraceFastTotalThreshold = TimeSpan.FromMilliseconds(50.0),
			SearchTraceFirstLinkedThreshold = TimeSpan.FromMilliseconds(250.0),
			SearchTraceGetQueryPlanThreshold = TimeSpan.FromMilliseconds(100.0),
			SearchTraceGetRestrictionThreshold = TimeSpan.FromMilliseconds(100.0),
			SearchTraceStoreOperationThreshold = TimeSpan.FromMilliseconds(5.0),
			SearchTraceStoreTotalThreshold = TimeSpan.FromMilliseconds(50.0),
			StoreQueryLimitRows = 10000,
			StoreQueryLimitTime = TimeSpan.FromMinutes(2.0),
			TrackAllLockAcquisition = false,
			SubobjectCleanupUrgentMaintenanceNumberOfItemsThreshold = 100L,
			SubobjectCleanupUrgentMaintenanceTotalSizeThreshold = 1048576L,
			LockManagerCrashingThresholdTimeout = TimeSpan.FromHours(2.0),
			ThreadHangDetectionTimeout = TimeSpan.FromHours(2.0),
			IntegrityCheckJobAgeoutTimeSpan = TimeSpan.FromDays(10.0),
			LastLogRefreshCheckDurationInSeconds = (int)TimeSpan.FromMinutes(2.0).TotalSeconds,
			LastLogUpdateAdvancingLimit = 50L,
			LastLogUpdateLaggingLimit = 1000L,
			MaximumSupportableDatabaseSchemaVersion = new ComponentVersion(0, 131),
			MaximumRequestableDatabaseSchemaVersion = new ComponentVersion(0, 131),
			CheckpointDepthOnPassive = 104857600,
			CheckpointDepthOnActive = 104857600,
			DatabaseCacheSizePercentage = 25,
			CachedClosedTables = 10000,
			DbScanThrottleOnActive = TimeSpan.FromMilliseconds(100.0),
			DbScanThrottleOnPassive = TimeSpan.FromMilliseconds(100.0),
			EnableTestCaseIdLookup = true,
			EnableReadFromPassive = true,
			InferenceLogRetentionPeriod = TimeSpan.FromDays(10.0),
			InferenceLogMaxRows = new UnlimitedItems(100000L),
			ChunkedIndexPopulationEnabled = true,
			EnableDbDivergenceDetection = true,
			LazyTransactionCommitTimeout = TimeSpan.Zero,
			EnableConversationMasterIndex = false,
			ForceConversationMasterIndexUpgrade = false,
			ForceRimQueryMaterialization = true,
			DiagnosticQueryLockTimeout = TimeSpan.FromMinutes(1.0),
			ScheduledISIntegEnabled = true,
			ScheduledISIntegDetectOnly = "FolderACL,MissingSpecialFolders",
			ScheduledISIntegDetectAndFix = "MidsetDeleted,RuleMessageClass,CorruptJunkRule,SearchBacklinks,ImapId,RestrictionFolder,UniqueMidIndex",
			ScheduledISIntegExecutePeriod = TimeSpan.FromDays(30.0),
			EseStageFlighting = 64,
			EnableDatabaseUnusedSpaceScrubbing = false,
			DefaultMailboxSharedObjectPropertyBagDataCacheSize = 24,
			PublicFolderMailboxSharedObjectPropertyBagDataCacheSize = 1000,
			SharedObjectPropertyBagDataCacheCleanupMultiplier = 10,
			ConfigurableSharedLockStage = 6,
			EnableIMAPDuplicateDetection = false,
			IndexUpdateBreadcrumbsInstrumentation = true,
			EseLrukCorrInterval = TimeSpan.FromSeconds(30.0),
			EseLrukTimeout = TimeSpan.FromHours(1.0),
			UserInformationIsEnabled = true,
			EnableUnifiedMailbox = true,
			VersionStoreCleanupMaintenanceTaskSupported = true,
			DisableBypassTempStream = false,
			CheckQuotaOnMessageCreate = true,
			UseDirectorySharedCache = true,
			MailboxInfoCacheSize = 1000,
			ForeignMailboxInfoCacheSize = 1000,
			AddressInfoCacheSize = 1000,
			ForeignAddressInfoCacheSize = 1000,
			DatabaseInfoCacheSize = 100,
			OrganizationContainerCacheSize = 100,
			MaterializedRestrictionSearchFolderConfigStage = 1,
			AttachmentMessageSaveChunking = true,
			TimeZoneBlobTruncationLength = 2560,
			EnablePropertiesPromotionValidation = true
		};

		private static DefaultSettings.DefaultSettingsValues performance = new DefaultSettings.DefaultSettingsValues
		{
			DestinationMailboxReservedCounterRangeConstant = 1000000,
			DestinationMailboxReservedCounterRangeGradient = 10,
			DiagnosticsThresholdChunkElapsedTime = TimeSpan.FromSeconds(2.0),
			DiagnosticsThresholdDatabaseTime = TimeSpan.FromSeconds(2.0),
			DiagnosticsThresholdDirectoryCalls = 50,
			DiagnosticsThresholdDirectoryTime = TimeSpan.FromSeconds(2.0),
			DiagnosticsThresholdHeavyActivityRpcCount = 200,
			DiagnosticsThresholdInstantSearchFolderView = TimeSpan.FromMilliseconds(250.0),
			DiagnosticsThresholdInteractionTime = TimeSpan.FromSeconds(2.0),
			DiagnosticsThresholdLockTime = TimeSpan.FromSeconds(2.0),
			DiagnosticsThresholdPagesDirtied = 150,
			DiagnosticsThresholdPagesPreread = 300,
			DiagnosticsThresholdPagesRead = 150,
			DiscoverPotentiallyDisabledMailboxesTaskPeriod = TimeSpan.FromDays(10.0),
			DiscoverPotentiallyExpiredMailboxesTaskPeriod = TimeSpan.FromDays(1.0),
			DumpsterMessagesPerFolderCountReceiveQuota = UnlimitedItems.UnlimitedValue,
			DumpsterMessagesPerFolderCountWarningQuota = UnlimitedItems.UnlimitedValue,
			DynamicSearchFolderPerScopeCountReceiveQuota = 10000000,
			EnableMaterializedRestriction = true,
			EnableTraceBreadCrumbs = true,
			EnableTraceDiagnosticQuery = true,
			EnableTraceFullTextIndexQuery = true,
			EnableTraceHeavyClientActivity = true,
			EnableTraceLockContention = true,
			EnableTraceLongOperation = true,
			EnableTraceReferenceData = true,
			EnableTraceRopResource = true,
			EnableTraceRopSummary = true,
			EnableTraceSyntheticCounters = true,
			FolderHierarchyChildrenCountReceiveQuota = UnlimitedItems.UnlimitedValue,
			FolderHierarchyChildrenCountWarningQuota = UnlimitedItems.UnlimitedValue,
			FolderHierarchyDepthReceiveQuota = UnlimitedItems.UnlimitedValue,
			FolderHierarchyDepthWarningQuota = UnlimitedItems.UnlimitedValue,
			FoldersCountWarningQuota = UnlimitedItems.UnlimitedValue,
			FoldersCountReceiveQuota = UnlimitedItems.UnlimitedValue,
			IdleAccessibleMailboxesTime = TimeSpan.FromDays(30.0),
			IdleIndexTimeForEmptyFolderOperation = TimeSpan.FromMinutes(10.0),
			MailboxLockMaximumWaitCount = 10,
			MailboxMessagesPerFolderCountReceiveQuota = UnlimitedItems.UnlimitedValue,
			MailboxMessagesPerFolderCountWarningQuota = UnlimitedItems.UnlimitedValue,
			MaintenanceControlPeriod = TimeSpan.FromMinutes(30.0),
			MaxChildCountForDumpsterHierarchyPublicFolder = new UnlimitedItems(1000L),
			MaximumMountedDatabases = 100,
			MaximumActivePropertyPromotions = 2,
			MaximumNumberOfExceptions = 10,
			Name = "PerformanceTest",
			ProcessorNumberOfColumnResults = 3,
			ProcessorNumberOfPropertyResults = 3,
			QueryVersionRefreshInterval = TimeSpan.FromMinutes(2.0),
			RetailAssertOnJetVsom = false,
			RetailAssertOnUnexpectedJetErrors = true,
			RopSummarySlowThreshold = TimeSpan.FromMilliseconds(70.0),
			TraceIntervalRopLockContention = TimeSpan.FromMinutes(5.0),
			TraceIntervalRopResource = TimeSpan.FromMinutes(5.0),
			TraceIntervalRopSummary = TimeSpan.FromMinutes(5.0),
			SearchTraceFastOperationThreshold = TimeSpan.FromMilliseconds(5.0),
			SearchTraceFastTotalThreshold = TimeSpan.FromMilliseconds(50.0),
			SearchTraceFirstLinkedThreshold = TimeSpan.FromMilliseconds(250.0),
			SearchTraceGetQueryPlanThreshold = TimeSpan.FromMilliseconds(100.0),
			SearchTraceGetRestrictionThreshold = TimeSpan.FromMilliseconds(100.0),
			SearchTraceStoreOperationThreshold = TimeSpan.FromMilliseconds(5.0),
			SearchTraceStoreTotalThreshold = TimeSpan.FromMilliseconds(50.0),
			StoreQueryLimitRows = 10000,
			StoreQueryLimitTime = TimeSpan.FromMinutes(2.0),
			TrackAllLockAcquisition = false,
			SubobjectCleanupUrgentMaintenanceNumberOfItemsThreshold = 500000L,
			SubobjectCleanupUrgentMaintenanceTotalSizeThreshold = 53687091200L,
			LockManagerCrashingThresholdTimeout = TimeSpan.FromHours(2.0),
			ThreadHangDetectionTimeout = TimeSpan.FromHours(2.0),
			IntegrityCheckJobAgeoutTimeSpan = TimeSpan.FromDays(10.0),
			LastLogRefreshCheckDurationInSeconds = (int)TimeSpan.FromMinutes(2.0).TotalSeconds,
			LastLogUpdateAdvancingLimit = 50L,
			LastLogUpdateLaggingLimit = 1000L,
			MaximumSupportableDatabaseSchemaVersion = new ComponentVersion(0, 131),
			MaximumRequestableDatabaseSchemaVersion = new ComponentVersion(0, 131),
			CheckpointDepthOnPassive = 52428800,
			CheckpointDepthOnActive = 52428800,
			DatabaseCacheSizePercentage = 20,
			CachedClosedTables = 10000,
			DbScanThrottleOnActive = TimeSpan.FromMilliseconds(100.0),
			DbScanThrottleOnPassive = TimeSpan.FromMilliseconds(100.0),
			EnableTestCaseIdLookup = false,
			EnableReadFromPassive = true,
			InferenceLogRetentionPeriod = TimeSpan.FromDays(10.0),
			InferenceLogMaxRows = new UnlimitedItems(100000L),
			ChunkedIndexPopulationEnabled = true,
			EnableDbDivergenceDetection = true,
			LazyTransactionCommitTimeout = TimeSpan.Zero,
			EnableConversationMasterIndex = false,
			ForceConversationMasterIndexUpgrade = false,
			ForceRimQueryMaterialization = true,
			DiagnosticQueryLockTimeout = TimeSpan.FromMinutes(1.0),
			ScheduledISIntegEnabled = false,
			ScheduledISIntegDetectOnly = string.Empty,
			ScheduledISIntegDetectAndFix = string.Empty,
			ScheduledISIntegExecutePeriod = TimeSpan.FromDays(30.0),
			EseStageFlighting = 64,
			EnableDatabaseUnusedSpaceScrubbing = false,
			DefaultMailboxSharedObjectPropertyBagDataCacheSize = 24,
			PublicFolderMailboxSharedObjectPropertyBagDataCacheSize = 1000,
			SharedObjectPropertyBagDataCacheCleanupMultiplier = 10,
			ConfigurableSharedLockStage = 6,
			EnableIMAPDuplicateDetection = false,
			IndexUpdateBreadcrumbsInstrumentation = false,
			EseLrukCorrInterval = TimeSpan.FromSeconds(30.0),
			EseLrukTimeout = TimeSpan.FromHours(1.0),
			UserInformationIsEnabled = true,
			EnableUnifiedMailbox = false,
			VersionStoreCleanupMaintenanceTaskSupported = false,
			DisableBypassTempStream = false,
			CheckQuotaOnMessageCreate = true,
			UseDirectorySharedCache = true,
			MailboxInfoCacheSize = 1000,
			ForeignMailboxInfoCacheSize = 1000,
			AddressInfoCacheSize = 1000,
			ForeignAddressInfoCacheSize = 1000,
			DatabaseInfoCacheSize = 100,
			OrganizationContainerCacheSize = 100,
			MaterializedRestrictionSearchFolderConfigStage = 1,
			AttachmentMessageSaveChunking = true,
			TimeZoneBlobTruncationLength = 2560,
			EnablePropertiesPromotionValidation = true
		};

		private static DefaultSettings.DefaultSettingsValues dogfood = new DefaultSettings.DefaultSettingsValues
		{
			DestinationMailboxReservedCounterRangeConstant = 10000,
			DestinationMailboxReservedCounterRangeGradient = 1,
			DiagnosticsThresholdChunkElapsedTime = TimeSpan.FromSeconds(2.0),
			DiagnosticsThresholdDatabaseTime = TimeSpan.FromSeconds(2.0),
			DiagnosticsThresholdDirectoryCalls = 50,
			DiagnosticsThresholdDirectoryTime = TimeSpan.FromSeconds(2.0),
			DiagnosticsThresholdHeavyActivityRpcCount = 200,
			DiagnosticsThresholdInstantSearchFolderView = TimeSpan.FromMilliseconds(250.0),
			DiagnosticsThresholdInteractionTime = TimeSpan.FromSeconds(2.0),
			DiagnosticsThresholdLockTime = TimeSpan.FromSeconds(2.0),
			DiagnosticsThresholdPagesDirtied = 150,
			DiagnosticsThresholdPagesPreread = 300,
			DiagnosticsThresholdPagesRead = 150,
			DiscoverPotentiallyDisabledMailboxesTaskPeriod = TimeSpan.FromDays(1.0),
			DiscoverPotentiallyExpiredMailboxesTaskPeriod = TimeSpan.FromDays(1.0),
			DumpsterMessagesPerFolderCountReceiveQuota = new UnlimitedItems(3000000L),
			DumpsterMessagesPerFolderCountWarningQuota = new UnlimitedItems(2750000L),
			DynamicSearchFolderPerScopeCountReceiveQuota = 100,
			EnableMaterializedRestriction = true,
			EnableTraceBreadCrumbs = true,
			EnableTraceDiagnosticQuery = true,
			EnableTraceFullTextIndexQuery = true,
			EnableTraceHeavyClientActivity = true,
			EnableTraceLockContention = true,
			EnableTraceLongOperation = true,
			EnableTraceReferenceData = true,
			EnableTraceRopResource = true,
			EnableTraceRopSummary = true,
			EnableTraceSyntheticCounters = true,
			FolderHierarchyChildrenCountReceiveQuota = new UnlimitedItems(10000L),
			FolderHierarchyChildrenCountWarningQuota = new UnlimitedItems(9000L),
			FolderHierarchyDepthReceiveQuota = new UnlimitedItems(300L),
			FolderHierarchyDepthWarningQuota = new UnlimitedItems(250L),
			FoldersCountWarningQuota = UnlimitedItems.UnlimitedValue,
			FoldersCountReceiveQuota = UnlimitedItems.UnlimitedValue,
			IdleAccessibleMailboxesTime = TimeSpan.FromDays(1.0),
			IdleIndexTimeForEmptyFolderOperation = TimeSpan.FromMinutes(10.0),
			MailboxLockMaximumWaitCount = 10,
			MailboxMessagesPerFolderCountReceiveQuota = new UnlimitedItems(1000000L),
			MailboxMessagesPerFolderCountWarningQuota = new UnlimitedItems(900000L),
			MaintenanceControlPeriod = TimeSpan.FromMinutes(30.0),
			MaxChildCountForDumpsterHierarchyPublicFolder = new UnlimitedItems(1000L),
			MaximumMountedDatabases = 100,
			MaximumActivePropertyPromotions = 2,
			MaximumNumberOfExceptions = 10,
			Name = "Dogfood",
			ProcessorNumberOfColumnResults = 3,
			ProcessorNumberOfPropertyResults = 3,
			QueryVersionRefreshInterval = TimeSpan.FromMinutes(2.0),
			RetailAssertOnJetVsom = false,
			RetailAssertOnUnexpectedJetErrors = true,
			RopSummarySlowThreshold = TimeSpan.FromMilliseconds(70.0),
			TraceIntervalRopLockContention = TimeSpan.FromMinutes(5.0),
			TraceIntervalRopResource = TimeSpan.FromMinutes(5.0),
			TraceIntervalRopSummary = TimeSpan.FromMinutes(5.0),
			SearchTraceFastOperationThreshold = TimeSpan.FromMilliseconds(5.0),
			SearchTraceFastTotalThreshold = TimeSpan.FromMilliseconds(50.0),
			SearchTraceFirstLinkedThreshold = TimeSpan.FromMilliseconds(250.0),
			SearchTraceGetQueryPlanThreshold = TimeSpan.FromMilliseconds(100.0),
			SearchTraceGetRestrictionThreshold = TimeSpan.FromMilliseconds(100.0),
			SearchTraceStoreOperationThreshold = TimeSpan.FromMilliseconds(5.0),
			SearchTraceStoreTotalThreshold = TimeSpan.FromMilliseconds(50.0),
			StoreQueryLimitRows = 1000,
			StoreQueryLimitTime = TimeSpan.FromSeconds(10.0),
			TrackAllLockAcquisition = false,
			SubobjectCleanupUrgentMaintenanceNumberOfItemsThreshold = 100000L,
			SubobjectCleanupUrgentMaintenanceTotalSizeThreshold = 10737418240L,
			LockManagerCrashingThresholdTimeout = TimeSpan.FromHours(2.0),
			ThreadHangDetectionTimeout = TimeSpan.FromHours(2.0),
			IntegrityCheckJobAgeoutTimeSpan = TimeSpan.FromDays(10.0),
			LastLogRefreshCheckDurationInSeconds = (int)TimeSpan.FromMinutes(2.0).TotalSeconds,
			LastLogUpdateAdvancingLimit = 50L,
			LastLogUpdateLaggingLimit = 1000L,
			MaximumSupportableDatabaseSchemaVersion = new ComponentVersion(0, 131),
			MaximumRequestableDatabaseSchemaVersion = new ComponentVersion(0, 131),
			CheckpointDepthOnPassive = 104857600,
			CheckpointDepthOnActive = 104857600,
			DatabaseCacheSizePercentage = 25,
			CachedClosedTables = 10000,
			DbScanThrottleOnActive = TimeSpan.FromMilliseconds(400.0),
			DbScanThrottleOnPassive = TimeSpan.FromMilliseconds(400.0),
			EnableTestCaseIdLookup = false,
			EnableReadFromPassive = false,
			InferenceLogRetentionPeriod = TimeSpan.FromDays(10.0),
			InferenceLogMaxRows = new UnlimitedItems(100000L),
			ChunkedIndexPopulationEnabled = true,
			EnableDbDivergenceDetection = true,
			LazyTransactionCommitTimeout = TimeSpan.Zero,
			EnableConversationMasterIndex = false,
			ForceConversationMasterIndexUpgrade = false,
			ForceRimQueryMaterialization = true,
			DiagnosticQueryLockTimeout = TimeSpan.FromMinutes(1.0),
			ScheduledISIntegEnabled = true,
			ScheduledISIntegDetectOnly = "FolderACL,MissingSpecialFolders",
			ScheduledISIntegDetectAndFix = "MidsetDeleted,RuleMessageClass,CorruptJunkRule,SearchBacklinks,ImapId,RestrictionFolder,UniqueMidIndex",
			ScheduledISIntegExecutePeriod = TimeSpan.FromDays(30.0),
			EseStageFlighting = 1024,
			EnableDatabaseUnusedSpaceScrubbing = false,
			DefaultMailboxSharedObjectPropertyBagDataCacheSize = 24,
			PublicFolderMailboxSharedObjectPropertyBagDataCacheSize = 1000,
			SharedObjectPropertyBagDataCacheCleanupMultiplier = 10,
			ConfigurableSharedLockStage = 6,
			EnableIMAPDuplicateDetection = false,
			IndexUpdateBreadcrumbsInstrumentation = true,
			EseLrukCorrInterval = TimeSpan.FromMilliseconds(128.0),
			EseLrukTimeout = TimeSpan.FromSeconds(100.0),
			UserInformationIsEnabled = false,
			EnableUnifiedMailbox = false,
			VersionStoreCleanupMaintenanceTaskSupported = false,
			DisableBypassTempStream = false,
			CheckQuotaOnMessageCreate = true,
			UseDirectorySharedCache = true,
			MailboxInfoCacheSize = 1000,
			ForeignMailboxInfoCacheSize = 1000,
			AddressInfoCacheSize = 1000,
			ForeignAddressInfoCacheSize = 1000,
			DatabaseInfoCacheSize = 100,
			OrganizationContainerCacheSize = 100,
			MaterializedRestrictionSearchFolderConfigStage = 1,
			AttachmentMessageSaveChunking = true,
			TimeZoneBlobTruncationLength = 2560,
			EnablePropertiesPromotionValidation = true
		};

		private static DefaultSettings.DefaultSettingsValues sdf = new DefaultSettings.DefaultSettingsValues
		{
			DestinationMailboxReservedCounterRangeConstant = 1000000,
			DestinationMailboxReservedCounterRangeGradient = 10,
			DiagnosticsThresholdChunkElapsedTime = TimeSpan.FromSeconds(2.0),
			DiagnosticsThresholdDatabaseTime = TimeSpan.FromSeconds(2.0),
			DiagnosticsThresholdDirectoryCalls = 50,
			DiagnosticsThresholdDirectoryTime = TimeSpan.FromSeconds(2.0),
			DiagnosticsThresholdHeavyActivityRpcCount = 200,
			DiagnosticsThresholdInstantSearchFolderView = TimeSpan.FromMilliseconds(250.0),
			DiagnosticsThresholdInteractionTime = TimeSpan.FromSeconds(2.0),
			DiagnosticsThresholdLockTime = TimeSpan.FromSeconds(2.0),
			DiagnosticsThresholdPagesDirtied = 150,
			DiagnosticsThresholdPagesPreread = 300,
			DiagnosticsThresholdPagesRead = 150,
			DiscoverPotentiallyDisabledMailboxesTaskPeriod = TimeSpan.FromDays(10.0),
			DiscoverPotentiallyExpiredMailboxesTaskPeriod = TimeSpan.FromDays(1.0),
			DumpsterMessagesPerFolderCountReceiveQuota = new UnlimitedItems(3000000L),
			DumpsterMessagesPerFolderCountWarningQuota = new UnlimitedItems(2750000L),
			DynamicSearchFolderPerScopeCountReceiveQuota = 10000,
			EnableMaterializedRestriction = true,
			EnableTraceBreadCrumbs = true,
			EnableTraceDiagnosticQuery = true,
			EnableTraceFullTextIndexQuery = true,
			EnableTraceHeavyClientActivity = true,
			EnableTraceLockContention = true,
			EnableTraceLongOperation = true,
			EnableTraceReferenceData = true,
			EnableTraceRopResource = true,
			EnableTraceRopSummary = true,
			EnableTraceSyntheticCounters = true,
			FolderHierarchyChildrenCountReceiveQuota = new UnlimitedItems(10000L),
			FolderHierarchyChildrenCountWarningQuota = new UnlimitedItems(9000L),
			FolderHierarchyDepthReceiveQuota = new UnlimitedItems(300L),
			FolderHierarchyDepthWarningQuota = new UnlimitedItems(250L),
			FoldersCountWarningQuota = UnlimitedItems.UnlimitedValue,
			FoldersCountReceiveQuota = UnlimitedItems.UnlimitedValue,
			IdleAccessibleMailboxesTime = TimeSpan.FromDays(30.0),
			IdleIndexTimeForEmptyFolderOperation = TimeSpan.FromMinutes(10.0),
			MailboxLockMaximumWaitCount = 10,
			MailboxMessagesPerFolderCountReceiveQuota = new UnlimitedItems(1000000L),
			MailboxMessagesPerFolderCountWarningQuota = new UnlimitedItems(900000L),
			MaintenanceControlPeriod = TimeSpan.FromMinutes(30.0),
			MaxChildCountForDumpsterHierarchyPublicFolder = new UnlimitedItems(1000L),
			MaximumMountedDatabases = 100,
			MaximumActivePropertyPromotions = 2,
			MaximumNumberOfExceptions = 10,
			Name = "DatacenterDogfood",
			ProcessorNumberOfColumnResults = 3,
			ProcessorNumberOfPropertyResults = 3,
			QueryVersionRefreshInterval = TimeSpan.FromMinutes(2.0),
			RetailAssertOnJetVsom = false,
			RetailAssertOnUnexpectedJetErrors = false,
			RopSummarySlowThreshold = TimeSpan.FromMilliseconds(70.0),
			TraceIntervalRopLockContention = TimeSpan.FromMinutes(5.0),
			TraceIntervalRopResource = TimeSpan.FromMinutes(5.0),
			TraceIntervalRopSummary = TimeSpan.FromMinutes(5.0),
			SearchTraceFastOperationThreshold = TimeSpan.FromMilliseconds(5.0),
			SearchTraceFastTotalThreshold = TimeSpan.FromMilliseconds(50.0),
			SearchTraceFirstLinkedThreshold = TimeSpan.FromMilliseconds(250.0),
			SearchTraceGetQueryPlanThreshold = TimeSpan.FromMilliseconds(100.0),
			SearchTraceGetRestrictionThreshold = TimeSpan.FromMilliseconds(100.0),
			SearchTraceStoreOperationThreshold = TimeSpan.FromMilliseconds(5.0),
			SearchTraceStoreTotalThreshold = TimeSpan.FromMilliseconds(50.0),
			StoreQueryLimitRows = 1000,
			StoreQueryLimitTime = TimeSpan.FromSeconds(10.0),
			TrackAllLockAcquisition = false,
			SubobjectCleanupUrgentMaintenanceNumberOfItemsThreshold = 100000L,
			SubobjectCleanupUrgentMaintenanceTotalSizeThreshold = 10737418240L,
			LockManagerCrashingThresholdTimeout = TimeSpan.FromHours(2.0),
			ThreadHangDetectionTimeout = TimeSpan.FromHours(2.0),
			IntegrityCheckJobAgeoutTimeSpan = TimeSpan.FromDays(10.0),
			LastLogRefreshCheckDurationInSeconds = (int)TimeSpan.FromMinutes(2.0).TotalSeconds,
			LastLogUpdateAdvancingLimit = 50L,
			LastLogUpdateLaggingLimit = 1000L,
			MaximumSupportableDatabaseSchemaVersion = new ComponentVersion(0, 131),
			MaximumRequestableDatabaseSchemaVersion = new ComponentVersion(0, 131),
			CheckpointDepthOnPassive = 52428800,
			CheckpointDepthOnActive = 52428800,
			DatabaseCacheSizePercentage = 20,
			CachedClosedTables = 10000,
			DbScanThrottleOnActive = TimeSpan.FromMilliseconds(400.0),
			DbScanThrottleOnPassive = TimeSpan.FromMilliseconds(400.0),
			EnableTestCaseIdLookup = false,
			EnableReadFromPassive = false,
			InferenceLogRetentionPeriod = TimeSpan.FromDays(10.0),
			InferenceLogMaxRows = new UnlimitedItems(100000L),
			ChunkedIndexPopulationEnabled = true,
			EnableDbDivergenceDetection = true,
			LazyTransactionCommitTimeout = TimeSpan.Zero,
			EnableConversationMasterIndex = false,
			ForceConversationMasterIndexUpgrade = false,
			ForceRimQueryMaterialization = true,
			DiagnosticQueryLockTimeout = TimeSpan.FromMinutes(1.0),
			ScheduledISIntegEnabled = true,
			ScheduledISIntegDetectOnly = "FolderACL,MissingSpecialFolders",
			ScheduledISIntegDetectAndFix = "MidsetDeleted,RuleMessageClass,CorruptJunkRule,SearchBacklinks,ImapId,RestrictionFolder,UniqueMidIndex",
			ScheduledISIntegExecutePeriod = TimeSpan.FromDays(30.0),
			EseStageFlighting = 4096,
			EnableDatabaseUnusedSpaceScrubbing = false,
			DefaultMailboxSharedObjectPropertyBagDataCacheSize = 24,
			PublicFolderMailboxSharedObjectPropertyBagDataCacheSize = 1000,
			SharedObjectPropertyBagDataCacheCleanupMultiplier = 10,
			ConfigurableSharedLockStage = 6,
			EnableIMAPDuplicateDetection = false,
			IndexUpdateBreadcrumbsInstrumentation = false,
			EseLrukCorrInterval = TimeSpan.FromSeconds(30.0),
			EseLrukTimeout = TimeSpan.FromHours(1.0),
			UserInformationIsEnabled = true,
			EnableUnifiedMailbox = false,
			VersionStoreCleanupMaintenanceTaskSupported = false,
			DisableBypassTempStream = false,
			CheckQuotaOnMessageCreate = true,
			UseDirectorySharedCache = true,
			MailboxInfoCacheSize = 1000,
			ForeignMailboxInfoCacheSize = 1000,
			AddressInfoCacheSize = 1000,
			ForeignAddressInfoCacheSize = 1000,
			DatabaseInfoCacheSize = 1000,
			OrganizationContainerCacheSize = 100,
			MaterializedRestrictionSearchFolderConfigStage = 1,
			AttachmentMessageSaveChunking = true,
			TimeZoneBlobTruncationLength = 2560,
			EnablePropertiesPromotionValidation = true
		};

		private static DefaultSettings.DefaultSettingsValues datacenter = new DefaultSettings.DefaultSettingsValues
		{
			DestinationMailboxReservedCounterRangeConstant = 1000000,
			DestinationMailboxReservedCounterRangeGradient = 10,
			DiagnosticsThresholdChunkElapsedTime = TimeSpan.FromSeconds(2.0),
			DiagnosticsThresholdDatabaseTime = TimeSpan.FromSeconds(2.0),
			DiagnosticsThresholdDirectoryCalls = 50,
			DiagnosticsThresholdDirectoryTime = TimeSpan.FromSeconds(2.0),
			DiagnosticsThresholdHeavyActivityRpcCount = 200,
			DiagnosticsThresholdInstantSearchFolderView = TimeSpan.FromMilliseconds(250.0),
			DiagnosticsThresholdInteractionTime = TimeSpan.FromSeconds(2.0),
			DiagnosticsThresholdLockTime = TimeSpan.FromSeconds(2.0),
			DiagnosticsThresholdPagesDirtied = 150,
			DiagnosticsThresholdPagesPreread = 300,
			DiagnosticsThresholdPagesRead = 150,
			DiscoverPotentiallyDisabledMailboxesTaskPeriod = TimeSpan.FromDays(10.0),
			DiscoverPotentiallyExpiredMailboxesTaskPeriod = TimeSpan.FromDays(1.0),
			DumpsterMessagesPerFolderCountReceiveQuota = new UnlimitedItems(3000000L),
			DumpsterMessagesPerFolderCountWarningQuota = new UnlimitedItems(2750000L),
			DynamicSearchFolderPerScopeCountReceiveQuota = 10000,
			EnableMaterializedRestriction = true,
			EnableTraceBreadCrumbs = true,
			EnableTraceDiagnosticQuery = true,
			EnableTraceFullTextIndexQuery = true,
			EnableTraceHeavyClientActivity = true,
			EnableTraceLockContention = true,
			EnableTraceLongOperation = true,
			EnableTraceReferenceData = true,
			EnableTraceRopResource = true,
			EnableTraceRopSummary = true,
			EnableTraceSyntheticCounters = true,
			FolderHierarchyChildrenCountReceiveQuota = new UnlimitedItems(10000L),
			FolderHierarchyChildrenCountWarningQuota = new UnlimitedItems(9000L),
			FolderHierarchyDepthReceiveQuota = new UnlimitedItems(300L),
			FolderHierarchyDepthWarningQuota = new UnlimitedItems(250L),
			FoldersCountWarningQuota = UnlimitedItems.UnlimitedValue,
			FoldersCountReceiveQuota = UnlimitedItems.UnlimitedValue,
			IdleAccessibleMailboxesTime = TimeSpan.FromDays(30.0),
			IdleIndexTimeForEmptyFolderOperation = TimeSpan.FromMinutes(10.0),
			MailboxLockMaximumWaitCount = 10,
			MailboxMessagesPerFolderCountReceiveQuota = new UnlimitedItems(1000000L),
			MailboxMessagesPerFolderCountWarningQuota = new UnlimitedItems(900000L),
			MaintenanceControlPeriod = TimeSpan.FromMinutes(30.0),
			MaxChildCountForDumpsterHierarchyPublicFolder = new UnlimitedItems(1000L),
			MaximumMountedDatabases = 100,
			MaximumActivePropertyPromotions = 2,
			MaximumNumberOfExceptions = 10,
			Name = "Datacenter",
			ProcessorNumberOfColumnResults = 3,
			ProcessorNumberOfPropertyResults = 3,
			QueryVersionRefreshInterval = TimeSpan.FromMinutes(2.0),
			RetailAssertOnJetVsom = false,
			RetailAssertOnUnexpectedJetErrors = false,
			RopSummarySlowThreshold = TimeSpan.FromMilliseconds(70.0),
			TraceIntervalRopLockContention = TimeSpan.FromMinutes(5.0),
			TraceIntervalRopResource = TimeSpan.FromMinutes(5.0),
			TraceIntervalRopSummary = TimeSpan.FromMinutes(5.0),
			SearchTraceFastOperationThreshold = TimeSpan.FromMilliseconds(5.0),
			SearchTraceFastTotalThreshold = TimeSpan.FromMilliseconds(50.0),
			SearchTraceFirstLinkedThreshold = TimeSpan.FromMilliseconds(250.0),
			SearchTraceGetQueryPlanThreshold = TimeSpan.FromMilliseconds(100.0),
			SearchTraceGetRestrictionThreshold = TimeSpan.FromMilliseconds(100.0),
			SearchTraceStoreOperationThreshold = TimeSpan.FromMilliseconds(5.0),
			SearchTraceStoreTotalThreshold = TimeSpan.FromMilliseconds(50.0),
			StoreQueryLimitRows = 1000,
			StoreQueryLimitTime = TimeSpan.FromSeconds(10.0),
			TrackAllLockAcquisition = false,
			SubobjectCleanupUrgentMaintenanceNumberOfItemsThreshold = 100000L,
			SubobjectCleanupUrgentMaintenanceTotalSizeThreshold = 10737418240L,
			LockManagerCrashingThresholdTimeout = TimeSpan.FromHours(2.0),
			ThreadHangDetectionTimeout = TimeSpan.FromHours(2.0),
			IntegrityCheckJobAgeoutTimeSpan = TimeSpan.FromDays(10.0),
			LastLogRefreshCheckDurationInSeconds = (int)TimeSpan.FromMinutes(2.0).TotalSeconds,
			LastLogUpdateAdvancingLimit = 50L,
			LastLogUpdateLaggingLimit = 1000L,
			MaximumSupportableDatabaseSchemaVersion = new ComponentVersion(0, 131),
			MaximumRequestableDatabaseSchemaVersion = new ComponentVersion(0, 131),
			CheckpointDepthOnPassive = 52428800,
			CheckpointDepthOnActive = 52428800,
			DatabaseCacheSizePercentage = 20,
			CachedClosedTables = 10000,
			DbScanThrottleOnActive = TimeSpan.FromMilliseconds(400.0),
			DbScanThrottleOnPassive = TimeSpan.FromMilliseconds(400.0),
			EnableTestCaseIdLookup = false,
			EnableReadFromPassive = false,
			InferenceLogRetentionPeriod = TimeSpan.FromDays(10.0),
			InferenceLogMaxRows = new UnlimitedItems(100000L),
			ChunkedIndexPopulationEnabled = true,
			EnableDbDivergenceDetection = true,
			LazyTransactionCommitTimeout = TimeSpan.Zero,
			EnableConversationMasterIndex = false,
			ForceConversationMasterIndexUpgrade = false,
			ForceRimQueryMaterialization = true,
			DiagnosticQueryLockTimeout = TimeSpan.FromMinutes(1.0),
			ScheduledISIntegEnabled = true,
			ScheduledISIntegDetectOnly = string.Empty,
			ScheduledISIntegDetectAndFix = "MidsetDeleted,RuleMessageClass,CorruptJunkRule",
			ScheduledISIntegExecutePeriod = TimeSpan.FromDays(30.0),
			EseStageFlighting = 4194304,
			EnableDatabaseUnusedSpaceScrubbing = false,
			DefaultMailboxSharedObjectPropertyBagDataCacheSize = 24,
			PublicFolderMailboxSharedObjectPropertyBagDataCacheSize = 1000,
			SharedObjectPropertyBagDataCacheCleanupMultiplier = 10,
			ConfigurableSharedLockStage = 6,
			EnableIMAPDuplicateDetection = false,
			IndexUpdateBreadcrumbsInstrumentation = false,
			EseLrukCorrInterval = TimeSpan.FromSeconds(30.0),
			EseLrukTimeout = TimeSpan.FromHours(1.0),
			UserInformationIsEnabled = true,
			EnableUnifiedMailbox = false,
			VersionStoreCleanupMaintenanceTaskSupported = false,
			DisableBypassTempStream = false,
			CheckQuotaOnMessageCreate = false,
			UseDirectorySharedCache = false,
			MailboxInfoCacheSize = 1000,
			ForeignMailboxInfoCacheSize = 1000,
			AddressInfoCacheSize = 1000,
			ForeignAddressInfoCacheSize = 1000,
			DatabaseInfoCacheSize = 1000,
			OrganizationContainerCacheSize = 100,
			MaterializedRestrictionSearchFolderConfigStage = 1,
			AttachmentMessageSaveChunking = true,
			TimeZoneBlobTruncationLength = 2560,
			EnablePropertiesPromotionValidation = true
		};

		private static DefaultSettings.DefaultSettingsValues customer = new DefaultSettings.DefaultSettingsValues
		{
			DestinationMailboxReservedCounterRangeConstant = 1000000,
			DestinationMailboxReservedCounterRangeGradient = 10,
			DiagnosticsThresholdChunkElapsedTime = TimeSpan.FromSeconds(2.0),
			DiagnosticsThresholdDatabaseTime = TimeSpan.FromSeconds(2.0),
			DiagnosticsThresholdDirectoryCalls = 50,
			DiagnosticsThresholdDirectoryTime = TimeSpan.FromSeconds(2.0),
			DiagnosticsThresholdHeavyActivityRpcCount = 200,
			DiagnosticsThresholdInstantSearchFolderView = TimeSpan.FromMilliseconds(250.0),
			DiagnosticsThresholdInteractionTime = TimeSpan.FromSeconds(2.0),
			DiagnosticsThresholdLockTime = TimeSpan.FromSeconds(2.0),
			DiagnosticsThresholdPagesDirtied = 150,
			DiagnosticsThresholdPagesPreread = 300,
			DiagnosticsThresholdPagesRead = 150,
			DiscoverPotentiallyDisabledMailboxesTaskPeriod = TimeSpan.FromDays(1.0),
			DiscoverPotentiallyExpiredMailboxesTaskPeriod = TimeSpan.FromDays(1.0),
			DumpsterMessagesPerFolderCountReceiveQuota = UnlimitedItems.UnlimitedValue,
			DumpsterMessagesPerFolderCountWarningQuota = UnlimitedItems.UnlimitedValue,
			DynamicSearchFolderPerScopeCountReceiveQuota = 100,
			EnableMaterializedRestriction = true,
			EnableTraceBreadCrumbs = false,
			EnableTraceDiagnosticQuery = false,
			EnableTraceFullTextIndexQuery = false,
			EnableTraceHeavyClientActivity = false,
			EnableTraceLockContention = false,
			EnableTraceLongOperation = false,
			EnableTraceReferenceData = false,
			EnableTraceRopResource = false,
			EnableTraceRopSummary = false,
			EnableTraceSyntheticCounters = false,
			FolderHierarchyChildrenCountReceiveQuota = UnlimitedItems.UnlimitedValue,
			FolderHierarchyChildrenCountWarningQuota = UnlimitedItems.UnlimitedValue,
			FolderHierarchyDepthReceiveQuota = UnlimitedItems.UnlimitedValue,
			FolderHierarchyDepthWarningQuota = UnlimitedItems.UnlimitedValue,
			FoldersCountWarningQuota = UnlimitedItems.UnlimitedValue,
			FoldersCountReceiveQuota = UnlimitedItems.UnlimitedValue,
			IdleAccessibleMailboxesTime = TimeSpan.FromDays(1.0),
			IdleIndexTimeForEmptyFolderOperation = TimeSpan.FromMinutes(10.0),
			MailboxLockMaximumWaitCount = 10,
			MailboxMessagesPerFolderCountReceiveQuota = UnlimitedItems.UnlimitedValue,
			MailboxMessagesPerFolderCountWarningQuota = UnlimitedItems.UnlimitedValue,
			MaintenanceControlPeriod = TimeSpan.FromMinutes(30.0),
			MaxChildCountForDumpsterHierarchyPublicFolder = new UnlimitedItems(1000L),
			MaximumMountedDatabases = 100,
			MaximumActivePropertyPromotions = 2,
			MaximumNumberOfExceptions = 10,
			Name = "Customer",
			ProcessorNumberOfColumnResults = 3,
			ProcessorNumberOfPropertyResults = 3,
			QueryVersionRefreshInterval = TimeSpan.FromMinutes(2.0),
			RetailAssertOnJetVsom = false,
			RetailAssertOnUnexpectedJetErrors = false,
			RopSummarySlowThreshold = TimeSpan.FromMilliseconds(70.0),
			TraceIntervalRopLockContention = TimeSpan.FromMinutes(5.0),
			TraceIntervalRopResource = TimeSpan.FromMinutes(5.0),
			TraceIntervalRopSummary = TimeSpan.FromMinutes(5.0),
			SearchTraceFastOperationThreshold = TimeSpan.FromMilliseconds(5.0),
			SearchTraceFastTotalThreshold = TimeSpan.FromMilliseconds(50.0),
			SearchTraceFirstLinkedThreshold = TimeSpan.FromMilliseconds(250.0),
			SearchTraceGetQueryPlanThreshold = TimeSpan.FromMilliseconds(100.0),
			SearchTraceGetRestrictionThreshold = TimeSpan.FromMilliseconds(100.0),
			SearchTraceStoreOperationThreshold = TimeSpan.FromMilliseconds(5.0),
			SearchTraceStoreTotalThreshold = TimeSpan.FromMilliseconds(50.0),
			StoreQueryLimitRows = 1000,
			StoreQueryLimitTime = TimeSpan.FromSeconds(10.0),
			TrackAllLockAcquisition = false,
			SubobjectCleanupUrgentMaintenanceNumberOfItemsThreshold = 100000L,
			SubobjectCleanupUrgentMaintenanceTotalSizeThreshold = 10737418240L,
			LockManagerCrashingThresholdTimeout = TimeSpan.FromHours(2.0),
			ThreadHangDetectionTimeout = TimeSpan.FromHours(2.0),
			IntegrityCheckJobAgeoutTimeSpan = TimeSpan.FromDays(10.0),
			LastLogRefreshCheckDurationInSeconds = (int)TimeSpan.FromMinutes(2.0).TotalSeconds,
			LastLogUpdateAdvancingLimit = 50L,
			LastLogUpdateLaggingLimit = 1000L,
			MaximumSupportableDatabaseSchemaVersion = new ComponentVersion(0, 131),
			MaximumRequestableDatabaseSchemaVersion = new ComponentVersion(0, 131),
			CheckpointDepthOnPassive = 104857600,
			CheckpointDepthOnActive = 104857600,
			DatabaseCacheSizePercentage = 25,
			CachedClosedTables = 30000,
			DbScanThrottleOnActive = TimeSpan.FromMilliseconds(100.0),
			DbScanThrottleOnPassive = TimeSpan.FromMilliseconds(100.0),
			EnableTestCaseIdLookup = false,
			EnableReadFromPassive = false,
			InferenceLogRetentionPeriod = TimeSpan.FromDays(10.0),
			InferenceLogMaxRows = new UnlimitedItems(100000L),
			ChunkedIndexPopulationEnabled = true,
			EnableDbDivergenceDetection = false,
			LazyTransactionCommitTimeout = TimeSpan.Zero,
			EnableConversationMasterIndex = false,
			ForceConversationMasterIndexUpgrade = false,
			ForceRimQueryMaterialization = true,
			DiagnosticQueryLockTimeout = TimeSpan.FromMinutes(1.0),
			ScheduledISIntegEnabled = false,
			ScheduledISIntegDetectOnly = string.Empty,
			ScheduledISIntegDetectAndFix = string.Empty,
			ScheduledISIntegExecutePeriod = TimeSpan.FromDays(30.0),
			EseStageFlighting = 0,
			EnableDatabaseUnusedSpaceScrubbing = true,
			DefaultMailboxSharedObjectPropertyBagDataCacheSize = 24,
			PublicFolderMailboxSharedObjectPropertyBagDataCacheSize = 10000,
			SharedObjectPropertyBagDataCacheCleanupMultiplier = 10,
			ConfigurableSharedLockStage = 6,
			EnableIMAPDuplicateDetection = false,
			IndexUpdateBreadcrumbsInstrumentation = false,
			EseLrukCorrInterval = TimeSpan.FromMilliseconds(128.0),
			EseLrukTimeout = TimeSpan.FromSeconds(100.0),
			UserInformationIsEnabled = false,
			EnableUnifiedMailbox = false,
			VersionStoreCleanupMaintenanceTaskSupported = false,
			DisableBypassTempStream = false,
			CheckQuotaOnMessageCreate = false,
			UseDirectorySharedCache = false,
			MailboxInfoCacheSize = 1000,
			ForeignMailboxInfoCacheSize = 1000,
			AddressInfoCacheSize = 1000,
			ForeignAddressInfoCacheSize = 1000,
			DatabaseInfoCacheSize = 100,
			OrganizationContainerCacheSize = 100,
			MaterializedRestrictionSearchFolderConfigStage = 1,
			AttachmentMessageSaveChunking = true,
			TimeZoneBlobTruncationLength = 2560,
			EnablePropertiesPromotionValidation = false
		};

		private static DefaultSettings.DefaultSettingsValues dedicated = new DefaultSettings.DefaultSettingsValues
		{
			DestinationMailboxReservedCounterRangeConstant = 1000000,
			DestinationMailboxReservedCounterRangeGradient = 10,
			DiagnosticsThresholdChunkElapsedTime = TimeSpan.FromSeconds(2.0),
			DiagnosticsThresholdDatabaseTime = TimeSpan.FromSeconds(2.0),
			DiagnosticsThresholdDirectoryCalls = 50,
			DiagnosticsThresholdDirectoryTime = TimeSpan.FromSeconds(2.0),
			DiagnosticsThresholdHeavyActivityRpcCount = 200,
			DiagnosticsThresholdInstantSearchFolderView = TimeSpan.FromMilliseconds(250.0),
			DiagnosticsThresholdInteractionTime = TimeSpan.FromSeconds(2.0),
			DiagnosticsThresholdLockTime = TimeSpan.FromSeconds(2.0),
			DiagnosticsThresholdPagesDirtied = 150,
			DiagnosticsThresholdPagesPreread = 300,
			DiagnosticsThresholdPagesRead = 150,
			DiscoverPotentiallyDisabledMailboxesTaskPeriod = TimeSpan.FromDays(10.0),
			DiscoverPotentiallyExpiredMailboxesTaskPeriod = TimeSpan.FromDays(1.0),
			DumpsterMessagesPerFolderCountReceiveQuota = UnlimitedItems.UnlimitedValue,
			DumpsterMessagesPerFolderCountWarningQuota = UnlimitedItems.UnlimitedValue,
			DynamicSearchFolderPerScopeCountReceiveQuota = 10000,
			EnableMaterializedRestriction = true,
			EnableTraceBreadCrumbs = true,
			EnableTraceDiagnosticQuery = true,
			EnableTraceFullTextIndexQuery = true,
			EnableTraceHeavyClientActivity = true,
			EnableTraceLockContention = true,
			EnableTraceLongOperation = true,
			EnableTraceReferenceData = true,
			EnableTraceRopResource = true,
			EnableTraceRopSummary = true,
			EnableTraceSyntheticCounters = true,
			FolderHierarchyChildrenCountReceiveQuota = UnlimitedItems.UnlimitedValue,
			FolderHierarchyChildrenCountWarningQuota = UnlimitedItems.UnlimitedValue,
			FolderHierarchyDepthReceiveQuota = UnlimitedItems.UnlimitedValue,
			FolderHierarchyDepthWarningQuota = UnlimitedItems.UnlimitedValue,
			FoldersCountWarningQuota = UnlimitedItems.UnlimitedValue,
			FoldersCountReceiveQuota = UnlimitedItems.UnlimitedValue,
			IdleAccessibleMailboxesTime = TimeSpan.FromDays(30.0),
			IdleIndexTimeForEmptyFolderOperation = TimeSpan.FromMinutes(10.0),
			MailboxLockMaximumWaitCount = 10,
			MailboxMessagesPerFolderCountReceiveQuota = UnlimitedItems.UnlimitedValue,
			MailboxMessagesPerFolderCountWarningQuota = UnlimitedItems.UnlimitedValue,
			MaintenanceControlPeriod = TimeSpan.FromMinutes(30.0),
			MaxChildCountForDumpsterHierarchyPublicFolder = new UnlimitedItems(1000L),
			MaximumMountedDatabases = 100,
			MaximumActivePropertyPromotions = 2,
			MaximumNumberOfExceptions = 10,
			Name = "Dedicated",
			ProcessorNumberOfColumnResults = 3,
			ProcessorNumberOfPropertyResults = 3,
			QueryVersionRefreshInterval = TimeSpan.FromMinutes(2.0),
			RetailAssertOnJetVsom = false,
			RetailAssertOnUnexpectedJetErrors = false,
			RopSummarySlowThreshold = TimeSpan.FromMilliseconds(70.0),
			TraceIntervalRopLockContention = TimeSpan.FromMinutes(5.0),
			TraceIntervalRopResource = TimeSpan.FromMinutes(5.0),
			TraceIntervalRopSummary = TimeSpan.FromMinutes(5.0),
			SearchTraceFastOperationThreshold = TimeSpan.FromMilliseconds(5.0),
			SearchTraceFastTotalThreshold = TimeSpan.FromMilliseconds(50.0),
			SearchTraceFirstLinkedThreshold = TimeSpan.FromMilliseconds(250.0),
			SearchTraceGetQueryPlanThreshold = TimeSpan.FromMilliseconds(100.0),
			SearchTraceGetRestrictionThreshold = TimeSpan.FromMilliseconds(100.0),
			SearchTraceStoreOperationThreshold = TimeSpan.FromMilliseconds(5.0),
			SearchTraceStoreTotalThreshold = TimeSpan.FromMilliseconds(50.0),
			StoreQueryLimitRows = 1000,
			StoreQueryLimitTime = TimeSpan.FromSeconds(10.0),
			TrackAllLockAcquisition = false,
			SubobjectCleanupUrgentMaintenanceNumberOfItemsThreshold = 100000L,
			SubobjectCleanupUrgentMaintenanceTotalSizeThreshold = 10737418240L,
			LockManagerCrashingThresholdTimeout = TimeSpan.FromHours(2.0),
			ThreadHangDetectionTimeout = TimeSpan.FromHours(2.0),
			IntegrityCheckJobAgeoutTimeSpan = TimeSpan.FromDays(10.0),
			LastLogRefreshCheckDurationInSeconds = (int)TimeSpan.FromMinutes(2.0).TotalSeconds,
			LastLogUpdateAdvancingLimit = 50L,
			LastLogUpdateLaggingLimit = 1000L,
			MaximumSupportableDatabaseSchemaVersion = new ComponentVersion(0, 131),
			MaximumRequestableDatabaseSchemaVersion = new ComponentVersion(0, 131),
			CheckpointDepthOnPassive = 104857600,
			CheckpointDepthOnActive = 104857600,
			DatabaseCacheSizePercentage = 25,
			CachedClosedTables = 10000,
			DbScanThrottleOnActive = TimeSpan.FromMilliseconds(400.0),
			DbScanThrottleOnPassive = TimeSpan.FromMilliseconds(400.0),
			EnableTestCaseIdLookup = false,
			EnableReadFromPassive = false,
			InferenceLogRetentionPeriod = TimeSpan.FromDays(10.0),
			InferenceLogMaxRows = new UnlimitedItems(100000L),
			ChunkedIndexPopulationEnabled = true,
			EnableDbDivergenceDetection = false,
			LazyTransactionCommitTimeout = TimeSpan.Zero,
			DiagnosticQueryLockTimeout = TimeSpan.FromMinutes(1.0),
			EnableConversationMasterIndex = false,
			ForceConversationMasterIndexUpgrade = false,
			ScheduledISIntegEnabled = false,
			ScheduledISIntegDetectOnly = string.Empty,
			ScheduledISIntegDetectAndFix = string.Empty,
			ScheduledISIntegExecutePeriod = TimeSpan.FromDays(30.0),
			EseStageFlighting = 0,
			EnableDatabaseUnusedSpaceScrubbing = false,
			DefaultMailboxSharedObjectPropertyBagDataCacheSize = 24,
			PublicFolderMailboxSharedObjectPropertyBagDataCacheSize = 10000,
			SharedObjectPropertyBagDataCacheCleanupMultiplier = 10,
			ConfigurableSharedLockStage = 6,
			EnableIMAPDuplicateDetection = false,
			IndexUpdateBreadcrumbsInstrumentation = false,
			EseLrukCorrInterval = TimeSpan.FromSeconds(30.0),
			EseLrukTimeout = TimeSpan.FromHours(1.0),
			UserInformationIsEnabled = false,
			EnableUnifiedMailbox = false,
			ForceRimQueryMaterialization = true,
			VersionStoreCleanupMaintenanceTaskSupported = false,
			DisableBypassTempStream = false,
			CheckQuotaOnMessageCreate = false,
			UseDirectorySharedCache = false,
			MailboxInfoCacheSize = 1000,
			ForeignMailboxInfoCacheSize = 1000,
			AddressInfoCacheSize = 1000,
			ForeignAddressInfoCacheSize = 1000,
			DatabaseInfoCacheSize = 100,
			OrganizationContainerCacheSize = 100,
			MaterializedRestrictionSearchFolderConfigStage = 1,
			AttachmentMessageSaveChunking = true,
			TimeZoneBlobTruncationLength = 2560,
			EnablePropertiesPromotionValidation = true
		};

		private static Hookable<bool> isTestEnvironment = Hookable<bool>.Create(true, StoreEnvironment.IsTestEnvironment);

		private static Hookable<bool> isDatacenterEnvironment = Hookable<bool>.Create(true, StoreEnvironment.IsDatacenterEnvironment);

		private static Hookable<bool> isSdfEnvironment = Hookable<bool>.Create(true, StoreEnvironment.IsSdfEnvironment);

		private static Hookable<bool> isDogfoodEnvironment = Hookable<bool>.Create(true, StoreEnvironment.IsDogfoodEnvironment);

		private static Hookable<bool> isDedicatedEnvironment = Hookable<bool>.Create(true, StoreEnvironment.IsDedicatedEnvironment);

		internal static class Test
		{
			internal static IDisposable SetIsTestEnvironment(bool isTestEnvironment)
			{
				return DefaultSettings.isTestEnvironment.SetTestHook(isTestEnvironment);
			}

			internal static IDisposable SetIsDatacenterEnvironment(bool isDatacenterEnvironment)
			{
				return DefaultSettings.isDatacenterEnvironment.SetTestHook(isDatacenterEnvironment);
			}

			internal static IDisposable SetIsSdfEnvironment(bool isSdfEnvironment)
			{
				return DefaultSettings.isSdfEnvironment.SetTestHook(isSdfEnvironment);
			}

			internal static IDisposable SetIsDogfoodEnvironment(bool isDogfoodEnvironment)
			{
				return DefaultSettings.isDogfoodEnvironment.SetTestHook(isDogfoodEnvironment);
			}

			internal static IDisposable SetIsDedicatedEnvironment(bool isDedicatedEnvironment)
			{
				return DefaultSettings.isDedicatedEnvironment.SetTestHook(isDedicatedEnvironment);
			}
		}

		public class DefaultSettingsValues
		{
			public int DestinationMailboxReservedCounterRangeConstant { get; internal set; }

			public int DestinationMailboxReservedCounterRangeGradient { get; internal set; }

			public TimeSpan DiagnosticsThresholdDatabaseTime { get; internal set; }

			public int DiagnosticsThresholdDirectoryCalls { get; internal set; }

			public TimeSpan DiagnosticsThresholdDirectoryTime { get; internal set; }

			public int DiagnosticsThresholdHeavyActivityRpcCount { get; internal set; }

			public TimeSpan DiagnosticsThresholdInstantSearchFolderView { get; internal set; }

			public TimeSpan DiagnosticsThresholdInteractionTime { get; internal set; }

			public TimeSpan DiagnosticsThresholdLockTime { get; internal set; }

			public int DiagnosticsThresholdPagesDirtied { get; internal set; }

			public int DiagnosticsThresholdPagesPreread { get; internal set; }

			public int DiagnosticsThresholdPagesRead { get; internal set; }

			public TimeSpan DiagnosticsThresholdChunkElapsedTime { get; internal set; }

			public TimeSpan DiscoverPotentiallyDisabledMailboxesTaskPeriod { get; internal set; }

			public TimeSpan DiscoverPotentiallyExpiredMailboxesTaskPeriod { get; internal set; }

			public UnlimitedItems DumpsterMessagesPerFolderCountReceiveQuota { get; internal set; }

			public UnlimitedItems DumpsterMessagesPerFolderCountWarningQuota { get; internal set; }

			public int DynamicSearchFolderPerScopeCountReceiveQuota { get; internal set; }

			public bool EnableMaterializedRestriction { get; internal set; }

			public bool EnableTraceBreadCrumbs { get; internal set; }

			public bool EnableTraceDiagnosticQuery { get; internal set; }

			public bool EnableTraceFullTextIndexQuery { get; internal set; }

			public bool EnableTraceHeavyClientActivity { get; internal set; }

			public bool EnableTraceLockContention { get; internal set; }

			public bool EnableTraceLongOperation { get; internal set; }

			public bool EnableTraceReferenceData { get; internal set; }

			public bool EnableTraceRopResource { get; internal set; }

			public bool EnableTraceRopSummary { get; internal set; }

			public bool EnableTraceSyntheticCounters { get; internal set; }

			public UnlimitedItems FolderHierarchyChildrenCountReceiveQuota { get; internal set; }

			public UnlimitedItems FolderHierarchyChildrenCountWarningQuota { get; internal set; }

			public UnlimitedItems FolderHierarchyDepthReceiveQuota { get; internal set; }

			public UnlimitedItems FolderHierarchyDepthWarningQuota { get; internal set; }

			public UnlimitedItems FoldersCountWarningQuota { get; internal set; }

			public UnlimitedItems FoldersCountReceiveQuota { get; internal set; }

			public TimeSpan IdleAccessibleMailboxesTime { get; internal set; }

			public TimeSpan IdleIndexTimeForEmptyFolderOperation { get; internal set; }

			public int MailboxLockMaximumWaitCount { get; internal set; }

			public UnlimitedItems MailboxMessagesPerFolderCountReceiveQuota { get; internal set; }

			public UnlimitedItems MailboxMessagesPerFolderCountWarningQuota { get; internal set; }

			public TimeSpan MaintenanceControlPeriod { get; internal set; }

			public UnlimitedItems MaxChildCountForDumpsterHierarchyPublicFolder { get; internal set; }

			public byte MaximumMountedDatabases { get; internal set; }

			public int MaximumActivePropertyPromotions { get; internal set; }

			public int MaximumNumberOfExceptions { get; internal set; }

			public string Name { get; internal set; }

			public int ProcessorNumberOfColumnResults { get; internal set; }

			public int ProcessorNumberOfPropertyResults { get; internal set; }

			public TimeSpan QueryVersionRefreshInterval { get; internal set; }

			public bool RetailAssertOnJetVsom { get; internal set; }

			public bool RetailAssertOnUnexpectedJetErrors { get; internal set; }

			public TimeSpan RopSummarySlowThreshold { get; internal set; }

			public TimeSpan TraceIntervalRopLockContention { get; internal set; }

			public TimeSpan TraceIntervalRopResource { get; internal set; }

			public TimeSpan TraceIntervalRopSummary { get; internal set; }

			public TimeSpan SearchTraceFastOperationThreshold { get; internal set; }

			public TimeSpan SearchTraceFastTotalThreshold { get; internal set; }

			public TimeSpan SearchTraceFirstLinkedThreshold { get; internal set; }

			public TimeSpan SearchTraceGetQueryPlanThreshold { get; internal set; }

			public TimeSpan SearchTraceGetRestrictionThreshold { get; internal set; }

			public TimeSpan SearchTraceStoreOperationThreshold { get; internal set; }

			public TimeSpan SearchTraceStoreTotalThreshold { get; internal set; }

			public int StoreQueryLimitRows { get; internal set; }

			public TimeSpan StoreQueryLimitTime { get; internal set; }

			public bool TrackAllLockAcquisition { get; internal set; }

			public long SubobjectCleanupUrgentMaintenanceNumberOfItemsThreshold { get; internal set; }

			public long SubobjectCleanupUrgentMaintenanceTotalSizeThreshold { get; internal set; }

			public TimeSpan LockManagerCrashingThresholdTimeout { get; internal set; }

			public TimeSpan ThreadHangDetectionTimeout { get; internal set; }

			public TimeSpan IntegrityCheckJobAgeoutTimeSpan { get; internal set; }

			public int LastLogRefreshCheckDurationInSeconds { get; internal set; }

			public long LastLogUpdateAdvancingLimit { get; internal set; }

			public long LastLogUpdateLaggingLimit { get; internal set; }

			public ComponentVersion MaximumSupportableDatabaseSchemaVersion { get; internal set; }

			public ComponentVersion MaximumRequestableDatabaseSchemaVersion { get; internal set; }

			public int CheckpointDepthOnPassive { get; internal set; }

			public int CheckpointDepthOnActive { get; internal set; }

			public int DatabaseCacheSizePercentage { get; internal set; }

			public int CachedClosedTables { get; internal set; }

			public TimeSpan DbScanThrottleOnActive { get; internal set; }

			public TimeSpan DbScanThrottleOnPassive { get; internal set; }

			public bool EnableTestCaseIdLookup { get; internal set; }

			public bool EnableReadFromPassive { get; internal set; }

			public TimeSpan InferenceLogRetentionPeriod { get; internal set; }

			public UnlimitedItems InferenceLogMaxRows { get; internal set; }

			public bool ChunkedIndexPopulationEnabled { get; internal set; }

			public bool EnableDbDivergenceDetection { get; internal set; }

			public bool EnableConversationMasterIndex { get; internal set; }

			public bool ForceConversationMasterIndexUpgrade { get; internal set; }

			public bool ForceRimQueryMaterialization { get; internal set; }

			public bool ScheduledISIntegEnabled { get; internal set; }

			public string ScheduledISIntegDetectOnly { get; internal set; }

			public string ScheduledISIntegDetectAndFix { get; internal set; }

			public TimeSpan ScheduledISIntegExecutePeriod { get; internal set; }

			public TimeSpan LazyTransactionCommitTimeout { get; internal set; }

			public TimeSpan DiagnosticQueryLockTimeout { get; internal set; }

			public int EseStageFlighting { get; internal set; }

			public bool EnableDatabaseUnusedSpaceScrubbing { get; internal set; }

			public int DefaultMailboxSharedObjectPropertyBagDataCacheSize { get; internal set; }

			public int PublicFolderMailboxSharedObjectPropertyBagDataCacheSize { get; internal set; }

			public int SharedObjectPropertyBagDataCacheCleanupMultiplier { get; internal set; }

			public int ConfigurableSharedLockStage { get; internal set; }

			public TimeSpan EseLrukCorrInterval { get; internal set; }

			public TimeSpan EseLrukTimeout { get; internal set; }

			public bool EnableIMAPDuplicateDetection { get; internal set; }

			public bool IndexUpdateBreadcrumbsInstrumentation { get; internal set; }

			public bool UserInformationIsEnabled { get; internal set; }

			public bool EnableUnifiedMailbox { get; internal set; }

			public bool VersionStoreCleanupMaintenanceTaskSupported { get; internal set; }

			public bool DisableBypassTempStream { get; internal set; }

			public bool CheckQuotaOnMessageCreate { get; internal set; }

			public bool UseDirectorySharedCache { get; internal set; }

			public int MailboxInfoCacheSize { get; internal set; }

			public int ForeignMailboxInfoCacheSize { get; internal set; }

			public int AddressInfoCacheSize { get; internal set; }

			public int ForeignAddressInfoCacheSize { get; internal set; }

			public int DatabaseInfoCacheSize { get; internal set; }

			public int OrganizationContainerCacheSize { get; internal set; }

			public int MaterializedRestrictionSearchFolderConfigStage { get; internal set; }

			public bool AttachmentMessageSaveChunking { get; internal set; }

			public int TimeZoneBlobTruncationLength { get; internal set; }

			public bool EnablePropertiesPromotionValidation { get; internal set; }

			public IDisposable HookValue<T>(string name, T newValue)
			{
				name = string.Format("<{0}>k__BackingField", name);
				Hookable<T> hookable = Hookable<T>.Create<DefaultSettings.DefaultSettingsValues>(true, name, this);
				return hookable.SetTestHook(newValue);
			}
		}
	}
}
