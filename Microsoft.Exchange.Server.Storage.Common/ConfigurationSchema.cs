using System;
using System.Collections.Generic;
using System.Configuration;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public abstract class ConfigurationSchema
	{
		static ConfigurationSchema()
		{
			ConfigurationSchema.DoNothingAndSilenceStyleCop();
		}

		public static bool IsZeroBox { get; set; }

		public static IReadOnlyList<ConfigurationSchema> RegisteredConfigurations
		{
			get
			{
				return ConfigurationSchema.registeredConfigurations;
			}
		}

		public static string LocalDatabaseRegistryKey { get; private set; }

		internal abstract ConfigurationProperty ConfigurationProperty { get; }

		public static DateTime NextRefreshTimeUTC
		{
			get
			{
				return ConfigurationSchema.nextRefreshTime;
			}
		}

		public static void Initialize()
		{
			StoreConfigContext.Initialize();
		}

		public static void SetDatabaseContext(Guid? databaseGuid, Guid? dagOrServerGuid)
		{
			StoreConfigContext.Default.SetDatabaseContext(databaseGuid, dagOrServerGuid);
			ConfigurationSchema.LocalDatabaseRegistryKey = string.Format("SYSTEM\\CurrentControlSet\\Services\\MSExchangeIS\\{0}\\{1}-{2}", Environment.MachineName, "Private", databaseGuid);
			ConfigurationSchema.ReloadTask(null, null, () => true);
			ConfigurationSchema.reloadTask = new RecurringTask<object>(new Task<object>.TaskCallback(ConfigurationSchema.ReloadTask), null, TimeSpan.FromMinutes(15.0), TimeSpan.FromMinutes(15.0), true);
		}

		private static void DoNothingAndSilenceStyleCop()
		{
		}

		private static void ReloadTask(TaskExecutionDiagnosticsProxy diagnosticsContext, object context, Func<bool> shouldContinue)
		{
			int num = 0;
			while (num < ConfigurationSchema.registeredConfigurations.Count && shouldContinue())
			{
				ConfigurationSchema.registeredConfigurations[num].Reload();
				num++;
			}
			ConfigurationSchema.nextRefreshTime = DateTime.UtcNow + TimeSpan.FromMinutes(15.0);
		}

		private static bool TryTimeSpanFromMilliseconds(string value, out TimeSpan result)
		{
			double value2;
			if (double.TryParse(value, out value2))
			{
				result = TimeSpan.FromMilliseconds(value2);
				return true;
			}
			result = TimeSpan.Zero;
			return false;
		}

		private static bool TryTimeSpanFromSeconds(string value, out TimeSpan result)
		{
			double value2;
			if (double.TryParse(value, out value2))
			{
				result = TimeSpan.FromSeconds(value2);
				return true;
			}
			result = TimeSpan.Zero;
			return false;
		}

		private static bool TryTimeSpanFromMinutes(string value, out TimeSpan result)
		{
			double value2;
			if (double.TryParse(value, out value2))
			{
				result = TimeSpan.FromMinutes(value2);
				return true;
			}
			result = TimeSpan.Zero;
			return false;
		}

		private static bool ValueGreaterThanZeroButNotUnlimited(UnlimitedItems value)
		{
			return value > UnlimitedItems.Zero && !value.IsUnlimited;
		}

		private static bool ValueGreaterThanZero(UnlimitedItems value)
		{
			return value > UnlimitedItems.Zero;
		}

		private static bool ValueGreaterThanZero(long value)
		{
			return value > 0L;
		}

		private static bool ValueGreaterThanZero(TimeSpan value)
		{
			return value > TimeSpan.Zero;
		}

		private static Func<T, T> NoPostProcess<T>()
		{
			return null;
		}

		private static long ObjectLimitsPostProcess(long value, long defaultValue)
		{
			if (value < -1L)
			{
				value = 0L;
			}
			if (value == 0L)
			{
				value = defaultValue;
			}
			return value;
		}

		protected ConfigurationSchema(string name)
		{
			this.name = name;
			if (ConfigurationSchema.registeredConfigurations == null)
			{
				ConfigurationSchema.registeredConfigurations = new List<ConfigurationSchema>();
			}
			ConfigurationSchema.registeredConfigurations.Add(this);
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public abstract void Reload();

		public static ConfigurationSchema<int> MaxBreadcrumbs = new ReadOnceConfigurationSchema<int>("MaxBreadcrumbs", 1000);

		public static ConfigurationSchema<bool> TrackAllLockAcquisition = new ReadOnceConfigurationSchema<bool>("TrackAllLockAcquisition", DefaultSettings.Get.TrackAllLockAcquisition);

		public static ConfigurationSchema<string> LogPath = new ReadOnceConfigurationSchema<string>("LogPath", "%ExchangeInstallDir%\\Logging\\Store\\");

		public static ConfigurationSchema<TimeSpan> ThreadHangDetectionTimeout = new ReadOnceConfigurationSchema<TimeSpan>("ThreadHangDetectionTimeout", DefaultSettings.Get.ThreadHangDetectionTimeout, new ConfigurationSchema<TimeSpan>.TryParse(ConfigurationSchema.TryTimeSpanFromSeconds), ConfigurationSchema.NoPostProcess<TimeSpan>(), new Func<TimeSpan, bool>(ConfigurationSchema.ValueGreaterThanZero));

		public static ConfigurationSchema<bool> RetailAssertOnUnexpectedJetErrors = new ReadOnceConfigurationSchema<bool>("RetailAssertOnUnexpectedJetErrors", DefaultSettings.Get.RetailAssertOnUnexpectedJetErrors);

		public static ConfigurationSchema<bool> RetailAssertOnJetVsom = new ReadOnceConfigurationSchema<bool>("RetailAssertOnJetVsom", DefaultSettings.Get.RetailAssertOnJetVsom);

		public static ConfigurationSchema<int> AttachmentBlobMaxSupportedDescendantCountRead = new ConfigurationSchema<int>("AttachmentBlobMaxSupportedDescendantCountRead", 100000, (int value) => Math.Max(100000, value));

		public static ConfigurationSchema<int> AttachmentBlobMaxSupportedDescendantCountWrite = new ConfigurationSchema<int>("AttachmentBlobMaxSupportedDescendantCountWrite", 10000, (int value) => Math.Min(ConfigurationSchema.AttachmentBlobMaxSupportedDescendantCountRead.Value, Math.Max(1000, value)));

		public static ConfigurationSchema<TimeSpan> MaintenanceControlPeriod = new ReadOnceConfigurationSchema<TimeSpan>("MaintenanceControlPeriod", DefaultSettings.Get.MaintenanceControlPeriod, ConfigurationSchema.NoPostProcess<TimeSpan>(), new Func<TimeSpan, bool>(ConfigurationSchema.ValueGreaterThanZero));

		public static ConfigurationSchema<string> WorkerEnvironmentSettings = new ConfigurationSchema<string>("WorkerEnvironmentSettings", null);

		public static ConfigurationSchema<int> MailboxLockMaximumWaitCount = new ReadOnceConfigurationSchema<int>("MailboxLockMaximumWaitCount", DefaultSettings.Get.MailboxLockMaximumWaitCount);

		public static ConfigurationSchema<bool> EnableTraceBreadCrumbs = new ConfigurationSchema<bool>("EnableTraceBreadCrumbs", DefaultSettings.Get.EnableTraceBreadCrumbs);

		public static ConfigurationSchema<bool> EnableTraceDiagnosticQuery = new ConfigurationSchema<bool>("EnableTraceDiagnosticQuery", DefaultSettings.Get.EnableTraceDiagnosticQuery);

		public static ConfigurationSchema<bool> EnableTraceFullTextIndexQuery = new ConfigurationSchema<bool>("EnableTraceFullTextIndexQuery", DefaultSettings.Get.EnableTraceFullTextIndexQuery);

		public static ConfigurationSchema<bool> EnableTraceHeavyClientActivity = new ConfigurationSchema<bool>("EnableTraceHeavyClientActivity", DefaultSettings.Get.EnableTraceHeavyClientActivity);

		public static ConfigurationSchema<bool> EnableTraceLockContention = new ConfigurationSchema<bool>("EnableTraceLockContention", DefaultSettings.Get.EnableTraceLockContention);

		public static ConfigurationSchema<bool> EnableTraceLongOperation = new ConfigurationSchema<bool>("EnableTraceLongOperation", DefaultSettings.Get.EnableTraceLongOperation);

		public static ConfigurationSchema<bool> EnableTraceReferenceData = new ConfigurationSchema<bool>("EnableTraceReferenceData", DefaultSettings.Get.EnableTraceReferenceData);

		public static ConfigurationSchema<bool> EnableTraceRopResource = new ConfigurationSchema<bool>("EnableTraceRopResource", DefaultSettings.Get.EnableTraceRopResource);

		public static ConfigurationSchema<bool> EnableTraceRopSummary = new ConfigurationSchema<bool>("EnableTraceRopSummary", DefaultSettings.Get.EnableTraceRopSummary);

		public static ConfigurationSchema<bool> EnableTraceSyntheticCounters = new ConfigurationSchema<bool>("EnableTraceSyntheticCounters", DefaultSettings.Get.EnableTraceSyntheticCounters);

		public static ConfigurationSchema<TimeSpan> TraceIntervalRopLockContention = new ConfigurationSchema<TimeSpan>("TraceIntervalRopLockContention", DefaultSettings.Get.TraceIntervalRopLockContention, new ConfigurationSchema<TimeSpan>.TryParse(ConfigurationSchema.TryTimeSpanFromMilliseconds));

		public static ConfigurationSchema<TimeSpan> TraceIntervalRopResource = new ConfigurationSchema<TimeSpan>("TraceIntervalRopResource", DefaultSettings.Get.TraceIntervalRopResource, new ConfigurationSchema<TimeSpan>.TryParse(ConfigurationSchema.TryTimeSpanFromMilliseconds));

		public static ConfigurationSchema<TimeSpan> TraceIntervalRopSummary = new ConfigurationSchema<TimeSpan>("TraceIntervalRopSummary", DefaultSettings.Get.TraceIntervalRopSummary, new ConfigurationSchema<TimeSpan>.TryParse(ConfigurationSchema.TryTimeSpanFromMilliseconds));

		public static ConfigurationSchema<TimeSpan> DiagnosticsThresholdDatabaseTime = new ConfigurationSchema<TimeSpan>("DiagnosticsThresholdDatabaseTime", DefaultSettings.Get.DiagnosticsThresholdDatabaseTime, new ConfigurationSchema<TimeSpan>.TryParse(ConfigurationSchema.TryTimeSpanFromMilliseconds), ConfigurationSchema.NoPostProcess<TimeSpan>(), new Func<TimeSpan, bool>(ConfigurationSchema.ValueGreaterThanZero));

		public static ConfigurationSchema<int> DiagnosticsThresholdDirectoryCalls = new ConfigurationSchema<int>("DiagnosticsThresholdDirectoryCalls", DefaultSettings.Get.DiagnosticsThresholdDirectoryCalls);

		public static ConfigurationSchema<TimeSpan> DiagnosticsThresholdDirectoryTime = new ConfigurationSchema<TimeSpan>("DiagnosticsThresholdDirectoryTime", DefaultSettings.Get.DiagnosticsThresholdDirectoryTime, new ConfigurationSchema<TimeSpan>.TryParse(ConfigurationSchema.TryTimeSpanFromMilliseconds), ConfigurationSchema.NoPostProcess<TimeSpan>(), new Func<TimeSpan, bool>(ConfigurationSchema.ValueGreaterThanZero));

		public static ConfigurationSchema<int> DiagnosticsThresholdHeavyActivityRpcCount = new ConfigurationSchema<int>("DiagnosticsThresholdHeavyActivityRpcCount", DefaultSettings.Get.DiagnosticsThresholdHeavyActivityRpcCount);

		public static ConfigurationSchema<TimeSpan> DiagnosticsThresholdInstantSearchFolderView = new ConfigurationSchema<TimeSpan>("DiagnosticsThresholdInstantSearchFolderView", DefaultSettings.Get.DiagnosticsThresholdInstantSearchFolderView, new ConfigurationSchema<TimeSpan>.TryParse(ConfigurationSchema.TryTimeSpanFromMilliseconds), ConfigurationSchema.NoPostProcess<TimeSpan>(), new Func<TimeSpan, bool>(ConfigurationSchema.ValueGreaterThanZero));

		public static ConfigurationSchema<TimeSpan> DiagnosticsThresholdInteractionTime = new ConfigurationSchema<TimeSpan>("DiagnosticsThresholdInteractionTime", DefaultSettings.Get.DiagnosticsThresholdInteractionTime, new ConfigurationSchema<TimeSpan>.TryParse(ConfigurationSchema.TryTimeSpanFromMilliseconds), ConfigurationSchema.NoPostProcess<TimeSpan>(), new Func<TimeSpan, bool>(ConfigurationSchema.ValueGreaterThanZero));

		public static ConfigurationSchema<TimeSpan> DiagnosticsThresholdLockTime = new ConfigurationSchema<TimeSpan>("DiagnosticsThresholdLockTime", DefaultSettings.Get.DiagnosticsThresholdLockTime, new ConfigurationSchema<TimeSpan>.TryParse(ConfigurationSchema.TryTimeSpanFromMilliseconds), ConfigurationSchema.NoPostProcess<TimeSpan>(), new Func<TimeSpan, bool>(ConfigurationSchema.ValueGreaterThanZero));

		public static ConfigurationSchema<int> DiagnosticsThresholdPagesDirtied = new ConfigurationSchema<int>("DiagnosticsThresholdPagesDirtied", DefaultSettings.Get.DiagnosticsThresholdPagesDirtied);

		public static ConfigurationSchema<int> DiagnosticsThresholdPagesPreread = new ConfigurationSchema<int>("DiagnosticsThresholdPagesPreread", DefaultSettings.Get.DiagnosticsThresholdPagesPreread);

		public static ConfigurationSchema<int> DiagnosticsThresholdPagesRead = new ConfigurationSchema<int>("DiagnosticsThresholdPagesRead", DefaultSettings.Get.DiagnosticsThresholdPagesRead);

		public static ConfigurationSchema<TimeSpan> DiagnosticsThresholdChunkElapsedTime = new ConfigurationSchema<TimeSpan>("DiagnosticsThresholdChunkElapsedTime", DefaultSettings.Get.DiagnosticsThresholdChunkElapsedTime, new ConfigurationSchema<TimeSpan>.TryParse(ConfigurationSchema.TryTimeSpanFromMilliseconds), ConfigurationSchema.NoPostProcess<TimeSpan>(), new Func<TimeSpan, bool>(ConfigurationSchema.ValueGreaterThanZero));

		public static ConfigurationSchema<int> MaximumNumberOfExceptions = new ConfigurationSchema<int>("MaximumNumberOfExceptions", DefaultSettings.Get.MaximumNumberOfExceptions, (int value) => Math.Min(100, value));

		public static ConfigurationSchema<UnlimitedItems> MailboxMessagesPerFolderCountWarningQuota = new ConfigurationSchema<UnlimitedItems>("MailboxMessagesPerFolderCountWarningQuota", DefaultSettings.Get.MailboxMessagesPerFolderCountWarningQuota, new ConfigurationSchema<UnlimitedItems>.TryParse(UnlimitedItems.TryParse), ConfigurationSchema.NoPostProcess<UnlimitedItems>(), new Func<UnlimitedItems, bool>(ConfigurationSchema.ValueGreaterThanZero));

		public static ConfigurationSchema<UnlimitedItems> MailboxMessagesPerFolderCountReceiveQuota = new ConfigurationSchema<UnlimitedItems>("MailboxMessagesPerFolderCountReceiveQuota", DefaultSettings.Get.MailboxMessagesPerFolderCountReceiveQuota, new ConfigurationSchema<UnlimitedItems>.TryParse(UnlimitedItems.TryParse), ConfigurationSchema.NoPostProcess<UnlimitedItems>(), new Func<UnlimitedItems, bool>(ConfigurationSchema.ValueGreaterThanZero));

		public static ConfigurationSchema<UnlimitedItems> DumpsterMessagesPerFolderCountWarningQuota = new ConfigurationSchema<UnlimitedItems>("DumpsterMessagesPerFolderCountWarningQuota", DefaultSettings.Get.DumpsterMessagesPerFolderCountWarningQuota, new ConfigurationSchema<UnlimitedItems>.TryParse(UnlimitedItems.TryParse), ConfigurationSchema.NoPostProcess<UnlimitedItems>(), new Func<UnlimitedItems, bool>(ConfigurationSchema.ValueGreaterThanZero));

		public static ConfigurationSchema<UnlimitedItems> DumpsterMessagesPerFolderCountReceiveQuota = new ConfigurationSchema<UnlimitedItems>("DumpsterMessagesPerFolderCountReceiveQuota", DefaultSettings.Get.DumpsterMessagesPerFolderCountReceiveQuota, new ConfigurationSchema<UnlimitedItems>.TryParse(UnlimitedItems.TryParse), ConfigurationSchema.NoPostProcess<UnlimitedItems>(), new Func<UnlimitedItems, bool>(ConfigurationSchema.ValueGreaterThanZero));

		public static ConfigurationSchema<UnlimitedItems> FolderHierarchyChildrenCountWarningQuota = new ConfigurationSchema<UnlimitedItems>("FolderHierarchyChildrenCountWarningQuota", DefaultSettings.Get.FolderHierarchyChildrenCountWarningQuota, new ConfigurationSchema<UnlimitedItems>.TryParse(UnlimitedItems.TryParse), ConfigurationSchema.NoPostProcess<UnlimitedItems>(), new Func<UnlimitedItems, bool>(ConfigurationSchema.ValueGreaterThanZero));

		public static ConfigurationSchema<UnlimitedItems> FolderHierarchyChildrenCountReceiveQuota = new ConfigurationSchema<UnlimitedItems>("FolderHierarchyChildrenCountReceiveQuota", DefaultSettings.Get.FolderHierarchyChildrenCountReceiveQuota, new ConfigurationSchema<UnlimitedItems>.TryParse(UnlimitedItems.TryParse), ConfigurationSchema.NoPostProcess<UnlimitedItems>(), new Func<UnlimitedItems, bool>(ConfigurationSchema.ValueGreaterThanZero));

		public static ConfigurationSchema<UnlimitedItems> FolderHierarchyDepthWarningQuota = new ConfigurationSchema<UnlimitedItems>("FolderHierarchyDepthWarningQuota", DefaultSettings.Get.FolderHierarchyDepthWarningQuota, new ConfigurationSchema<UnlimitedItems>.TryParse(UnlimitedItems.TryParse), ConfigurationSchema.NoPostProcess<UnlimitedItems>(), new Func<UnlimitedItems, bool>(ConfigurationSchema.ValueGreaterThanZero));

		public static ConfigurationSchema<UnlimitedItems> FolderHierarchyDepthReceiveQuota = new ConfigurationSchema<UnlimitedItems>("FolderHierarchyDepthReceiveQuota", DefaultSettings.Get.FolderHierarchyDepthReceiveQuota, new ConfigurationSchema<UnlimitedItems>.TryParse(UnlimitedItems.TryParse), ConfigurationSchema.NoPostProcess<UnlimitedItems>(), new Func<UnlimitedItems, bool>(ConfigurationSchema.ValueGreaterThanZero));

		public static ConfigurationSchema<UnlimitedItems> FoldersCountWarningQuota = new ConfigurationSchema<UnlimitedItems>("FoldersCountWarningQuota", DefaultSettings.Get.FoldersCountWarningQuota, new ConfigurationSchema<UnlimitedItems>.TryParse(UnlimitedItems.TryParse), ConfigurationSchema.NoPostProcess<UnlimitedItems>(), new Func<UnlimitedItems, bool>(ConfigurationSchema.ValueGreaterThanZero));

		public static ConfigurationSchema<UnlimitedItems> FoldersCountReceiveQuota = new ConfigurationSchema<UnlimitedItems>("FoldersCountReceiveQuota", DefaultSettings.Get.FoldersCountReceiveQuota, new ConfigurationSchema<UnlimitedItems>.TryParse(UnlimitedItems.TryParse), ConfigurationSchema.NoPostProcess<UnlimitedItems>(), new Func<UnlimitedItems, bool>(ConfigurationSchema.ValueGreaterThanZero));

		public static ConfigurationSchema<UnlimitedItems> MaxChildCountForDumpsterHierarchyPublicFolder = new ConfigurationSchema<UnlimitedItems>("MaxChildCountForDumpsterHierarchyPublicFolder", DefaultSettings.Get.MaxChildCountForDumpsterHierarchyPublicFolder, new ConfigurationSchema<UnlimitedItems>.TryParse(UnlimitedItems.TryParse), ConfigurationSchema.NoPostProcess<UnlimitedItems>(), new Func<UnlimitedItems, bool>(ConfigurationSchema.ValueGreaterThanZeroButNotUnlimited));

		public static ConfigurationSchema<int> DestinationMailboxReservedCounterRangeConstant = new ConfigurationSchema<int>("DestinationMailboxReservedCounterRangeConstant", DefaultSettings.Get.DestinationMailboxReservedCounterRangeConstant);

		public static ConfigurationSchema<int> DestinationMailboxReservedCounterRangeGradient = new ConfigurationSchema<int>("DestinationMailboxReservedCounterRangeGradient", DefaultSettings.Get.DestinationMailboxReservedCounterRangeGradient);

		public static ConfigurationSchema<Dictionary<Guid, MailboxShape>> MailboxShapeQuotas = new ConfigurationSchema<Dictionary<Guid, MailboxShape>>("MailboxShapeQuotas", null, new ConfigurationSchema<Dictionary<Guid, MailboxShape>>.TryParse(MailboxShape.TryParse));

		public static ConfigurationSchema<bool> EnablePropertiesPromotionValidation = new ConfigurationSchema<bool>("EnablePropertiesPromotionValidation", DefaultSettings.Get.EnablePropertiesPromotionValidation);

		public static ConfigurationSchema<TimeSpan> SearchTraceFastOperationThreshold = new ConfigurationSchema<TimeSpan>("SearchTraceFastOperationThreshold", DefaultSettings.Get.SearchTraceFastOperationThreshold, new ConfigurationSchema<TimeSpan>.TryParse(ConfigurationSchema.TryTimeSpanFromMilliseconds), ConfigurationSchema.NoPostProcess<TimeSpan>(), new Func<TimeSpan, bool>(ConfigurationSchema.ValueGreaterThanZero));

		public static ConfigurationSchema<TimeSpan> SearchTraceFastTotalThreshold = new ConfigurationSchema<TimeSpan>("SearchTraceFastTotalThreshold", DefaultSettings.Get.SearchTraceFastTotalThreshold, new ConfigurationSchema<TimeSpan>.TryParse(ConfigurationSchema.TryTimeSpanFromMilliseconds), ConfigurationSchema.NoPostProcess<TimeSpan>(), new Func<TimeSpan, bool>(ConfigurationSchema.ValueGreaterThanZero));

		public static ConfigurationSchema<TimeSpan> SearchTraceStoreOperationThreshold = new ConfigurationSchema<TimeSpan>("SearchTraceStoreOperationThreshold", DefaultSettings.Get.SearchTraceStoreOperationThreshold, new ConfigurationSchema<TimeSpan>.TryParse(ConfigurationSchema.TryTimeSpanFromMilliseconds), ConfigurationSchema.NoPostProcess<TimeSpan>(), new Func<TimeSpan, bool>(ConfigurationSchema.ValueGreaterThanZero));

		public static ConfigurationSchema<TimeSpan> SearchTraceStoreTotalThreshold = new ConfigurationSchema<TimeSpan>("SearchTraceStoreTotalThreshold", DefaultSettings.Get.SearchTraceStoreTotalThreshold, new ConfigurationSchema<TimeSpan>.TryParse(ConfigurationSchema.TryTimeSpanFromMilliseconds), ConfigurationSchema.NoPostProcess<TimeSpan>(), new Func<TimeSpan, bool>(ConfigurationSchema.ValueGreaterThanZero));

		public static ConfigurationSchema<TimeSpan> SearchTraceFirstLinkedThreshold = new ConfigurationSchema<TimeSpan>("SearchTraceFirstLinkedThreshold", DefaultSettings.Get.SearchTraceFirstLinkedThreshold, new ConfigurationSchema<TimeSpan>.TryParse(ConfigurationSchema.TryTimeSpanFromMilliseconds), ConfigurationSchema.NoPostProcess<TimeSpan>(), new Func<TimeSpan, bool>(ConfigurationSchema.ValueGreaterThanZero));

		public static ConfigurationSchema<TimeSpan> SearchTraceGetRestrictionThreshold = new ConfigurationSchema<TimeSpan>("SearchTraceGetRestrictionThreshold", DefaultSettings.Get.SearchTraceGetRestrictionThreshold, new ConfigurationSchema<TimeSpan>.TryParse(ConfigurationSchema.TryTimeSpanFromMilliseconds), ConfigurationSchema.NoPostProcess<TimeSpan>(), new Func<TimeSpan, bool>(ConfigurationSchema.ValueGreaterThanZero));

		public static ConfigurationSchema<TimeSpan> SearchTraceGetQueryPlanThreshold = new ConfigurationSchema<TimeSpan>("SearchTraceGetQueryPlanThreshold", DefaultSettings.Get.SearchTraceGetQueryPlanThreshold, new ConfigurationSchema<TimeSpan>.TryParse(ConfigurationSchema.TryTimeSpanFromMilliseconds), ConfigurationSchema.NoPostProcess<TimeSpan>(), new Func<TimeSpan, bool>(ConfigurationSchema.ValueGreaterThanZero));

		public static ConfigurationSchema<int> MaximumActivePropertyPromotions = new ConfigurationSchema<int>("MaximumActivePropertyPromotions", DefaultSettings.Get.MaximumActivePropertyPromotions, (int value) => Math.Max(1, Math.Min(100, value)));

		public static ConfigurationSchema<int> PerUserCacheSize = new ReadOnceConfigurationSchema<int>("PerUserCacheSize", 1000);

		public static ConfigurationSchema<TimeSpan> PerUserCacheExpiration = new ReadOnceConfigurationSchema<TimeSpan>("PerUserCacheExpirationInMinutes", TimeSpan.FromMinutes(10.0), new ConfigurationSchema<TimeSpan>.TryParse(ConfigurationSchema.TryTimeSpanFromMinutes), ConfigurationSchema.NoPostProcess<TimeSpan>(), new Func<TimeSpan, bool>(ConfigurationSchema.ValueGreaterThanZero));

		public static ConfigurationSchema<bool> EnableOptimizedConversationSearch = new ConfigurationSchema<bool>("EnableOptimizedConversationSearch", true);

		public static ConfigurationSchema<long> SubobjectCleanupUrgentMaintenanceNumberOfItemsThreshold = new ReadOnceConfigurationSchema<long>("SubobjectCleanupUrgentMaintenanceNumberOfItemsThreshold", DefaultSettings.Get.SubobjectCleanupUrgentMaintenanceNumberOfItemsThreshold);

		public static ConfigurationSchema<long> SubobjectCleanupUrgentMaintenanceTotalSizeThreshold = new ReadOnceConfigurationSchema<long>("SubobjectCleanupUrgentMaintenanceTotalSizeThreshold", DefaultSettings.Get.SubobjectCleanupUrgentMaintenanceTotalSizeThreshold);

		public static ConfigurationSchema<bool> DirectIndexUpdateInstrumentation = new ReadOnceConfigurationSchema<bool>("DirectIndexUpdateInstrumentation", false);

		public static ConfigurationSchema<bool> IndexUpdateBreadcrumbsInstrumentation = new ReadOnceConfigurationSchema<bool>("IndexUpdateBreadcrumbsInstrumentation", DefaultSettings.Get.IndexUpdateBreadcrumbsInstrumentation);

		public static ConfigurationSchema<bool> ChunkedIndexPopulationEnabled = new ReadOnceConfigurationSchema<bool>("ChunkedIndexPopulationEnabled", DefaultSettings.Get.ChunkedIndexPopulationEnabled);

		public static ConfigurationSchema<int> ChunkedIndexPopulationMinChunkTimeMilliseconds = new ReadOnceConfigurationSchema<int>("ChunkedIndexPopulationMinChunkTimeMilliseconds", 10);

		public static ConfigurationSchema<int> ChunkedIndexPopulationMaxChunkTimeMilliseconds = new ReadOnceConfigurationSchema<int>("ChunkedIndexPopulationMaxChunkTimeMilliseconds", 2000);

		public static ConfigurationSchema<int> ChunkedIndexPopulationFolderSizeThreshold = new ReadOnceConfigurationSchema<int>("ChunkedIndexPopulationFolderSizeThreshold", 300);

		public static ConfigurationSchema<int> EseStageFlighting = new ReadOnceConfigurationSchema<int>("EseStageFlighting", DefaultSettings.Get.EseStageFlighting);

		public static ConfigurationSchema<int> ChunkedIndexPopulationMaxWaiters = new ReadOnceConfigurationSchema<int>("ChunkedIndexPopulationMaxWaiters", 2);

		public static ConfigurationSchema<TimeSpan> InstanceStatusTimeout = new ReadOnceConfigurationSchema<TimeSpan>("InstanceStatusTimeoutSeconds", TimeSpan.FromSeconds(10.0), new ConfigurationSchema<TimeSpan>.TryParse(ConfigurationSchema.TryTimeSpanFromSeconds), (TimeSpan value) => TimeSpan.FromTicks(Math.Max(0L, Math.Min(TimeSpan.FromHours(1.0).Ticks, value.Ticks))));

		public static ConfigurationSchema<bool> DisableIndexCorruptionAssertThrottling = new ReadOnceConfigurationSchema<bool>("DisableIndexCorruptionAssertThrottling", StoreEnvironment.IsTestEnvironment);

		public static ConfigurationSchema<int> MapiMessageMaxSupportedAttachmentCount = new ConfigurationSchema<int>("MapiMessageMaxSupportedAttachmentCount", 1024);

		public static ConfigurationSchema<int> MapiMessageMaxSupportedDescendantCount = new ConfigurationSchema<int>("MapiMessageMaxSupportedDescendantCount", 2048);

		public static ConfigurationSchema<bool> EnableMaterializedRestriction = new ConfigurationSchema<bool>("EnableMaterializedRestriction", DefaultSettings.Get.EnableMaterializedRestriction);

		public static ConfigurationSchema<ulong> MinRangeSizeForCnRestriction = new ConfigurationSchema<ulong>("MinRangeSizeForCnRestriction", 10000UL);

		public static ConfigurationSchema<int> GetChangedMessagesBatchSize = new ConfigurationSchema<int>("GetChangedMessagesBatchSize", 100);

		public static ConfigurationSchema<TimeSpan> IntegrityCheckJobAgeoutTimeSpan = new ConfigurationSchema<TimeSpan>("IntegrityCheckJobAgeoutTimeSpan", DefaultSettings.Get.IntegrityCheckJobAgeoutTimeSpan, ConfigurationSchema.NoPostProcess<TimeSpan>(), new Func<TimeSpan, bool>(ConfigurationSchema.ValueGreaterThanZero));

		public static ConfigurationSchema<int> IntegrityCheckJobStorageCapacity = new ConfigurationSchema<int>("IntegrityCheckJobStorageCapacity", 1000);

		public static ConfigurationSchema<TimeSpan> DiscoverPotentiallyExpiredMailboxesTaskPeriod = new ReadOnceConfigurationSchema<TimeSpan>("DiscoverPotentiallyExpiredMailboxesTaskPeriod", DefaultSettings.Get.DiscoverPotentiallyExpiredMailboxesTaskPeriod, ConfigurationSchema.NoPostProcess<TimeSpan>(), new Func<TimeSpan, bool>(ConfigurationSchema.ValueGreaterThanZero));

		public static ConfigurationSchema<TimeSpan> DiscoverPotentiallyDisabledMailboxesTaskPeriod = new ReadOnceConfigurationSchema<TimeSpan>("DiscoverPotentiallyDisabledMailboxesTaskPeriod", DefaultSettings.Get.DiscoverPotentiallyDisabledMailboxesTaskPeriod, ConfigurationSchema.NoPostProcess<TimeSpan>(), new Func<TimeSpan, bool>(ConfigurationSchema.ValueGreaterThanZero));

		public static ConfigurationSchema<TimeSpan> IdleAccessibleMailboxTime = new ConfigurationSchema<TimeSpan>("IdleAccessibleMailboxTime", DefaultSettings.Get.IdleAccessibleMailboxesTime, ConfigurationSchema.NoPostProcess<TimeSpan>(), new Func<TimeSpan, bool>(ConfigurationSchema.ValueGreaterThanZero));

		public static ConfigurationSchema<int> ProcessorNumberOfColumnResults = new ConfigurationSchema<int>("ProcessorNumberOfColumnResults", DefaultSettings.Get.ProcessorNumberOfColumnResults);

		public static ConfigurationSchema<int> ProcessorNumberOfPropertyResults = new ConfigurationSchema<int>("ProcessorNumberOfPropertyResults", DefaultSettings.Get.ProcessorNumberOfPropertyResults);

		public static ConfigurationSchema<int> DynamicSearchFolderPerScopeCountReceiveQuota = new ConfigurationSchema<int>("DynamicSearchFolderPerScopeCountReceiveQuota", DefaultSettings.Get.DynamicSearchFolderPerScopeCountReceiveQuota);

		public static ConfigurationSchema<bool> DisableSchemaUpgraders = new ConfigurationSchema<bool>("DisableSchemaUpgraders", false);

		public static ConfigurationSchema<ComponentVersion> MaximumRequestableSchemaVersion = new ConfigurationSchema<ComponentVersion>("MaximumRequestableSchemaVersion", DefaultSettings.Get.MaximumRequestableDatabaseSchemaVersion, new ConfigurationSchema<ComponentVersion>.TryParse(ComponentVersion.TryParse));

		public static ConfigurationSchema<int> DefaultMailboxSharedObjectPropertyBagDataCacheSize = new ConfigurationSchema<int>("DefaultMailboxSharedObjectPropertyBagDataCacheSize", DefaultSettings.Get.DefaultMailboxSharedObjectPropertyBagDataCacheSize);

		public static ConfigurationSchema<int> PublicFolderMailboxSharedObjectPropertyBagDataCacheSize = new ConfigurationSchema<int>("PublicFolderMailboxSharedObjectPropertyBagDataCacheSize", DefaultSettings.Get.PublicFolderMailboxSharedObjectPropertyBagDataCacheSize);

		public static ConfigurationSchema<int> SharedObjectPropertyBagDataCacheCleanupMultiplier = new ConfigurationSchema<int>("SharedObjectPropertyBagDataCacheCleanupMultiplier", DefaultSettings.Get.SharedObjectPropertyBagDataCacheCleanupMultiplier);

		public static ConfigurationSchema<TimeSpan> SharedObjectPropertyBagDataCacheTimeToLive = new ConfigurationSchema<TimeSpan>("SharedObjectPropertyBagDataCacheTimeToLive", TimeSpan.FromMinutes(15.0), ConfigurationSchema.NoPostProcess<TimeSpan>(), new Func<TimeSpan, bool>(ConfigurationSchema.ValueGreaterThanZero));

		public static ConfigurationSchema<int> LogicalIndexCacheSize = new ConfigurationSchema<int>("LogicalIndexCacheSize", 24);

		public static ConfigurationSchema<TimeSpan> LogicalIndexCacheTimeToLive = new ConfigurationSchema<TimeSpan>("LogicalIndexCacheTimeToLive", TimeSpan.FromMinutes(15.0), ConfigurationSchema.NoPostProcess<TimeSpan>(), new Func<TimeSpan, bool>(ConfigurationSchema.ValueGreaterThanZero));

		public static ConfigurationSchema<TimeSpan> MaxIdleCleanupPeriod = new ReadOnceConfigurationSchema<TimeSpan>("MaxIdleCleanupPeriod", TimeSpan.FromDays(30.0), ConfigurationSchema.NoPostProcess<TimeSpan>(), new Func<TimeSpan, bool>(ConfigurationSchema.ValueGreaterThanZero));

		public static ConfigurationSchema<int> StopMaintenanceThreshold = new ReadOnceConfigurationSchema<int>("StopMaintenanceThreshold", 50000);

		public static ConfigurationSchema<int> WlmMaintenanceThreshold = new ReadOnceConfigurationSchema<int>("WlmMaintenanceThreshold", 100000);

		public static ConfigurationSchema<int> NumberOfRecordsToMaintain = new ReadOnceConfigurationSchema<int>("NumberOfRecordsToMaintain", 5000);

		public static ConfigurationSchema<int> NumberOfRecordsToReadFromMaintenanceTable = new ReadOnceConfigurationSchema<int>("NumberOfRecordsToReadFromMaintenanceTable", 20000);

		public static ConfigurationSchema<TimeSpan> MaintenanceTimePeriodToKeep = new ReadOnceConfigurationSchema<TimeSpan>("MaintenanceTimePeriodToKeep", TimeSpan.FromDays(30.0), ConfigurationSchema.NoPostProcess<TimeSpan>(), new Func<TimeSpan, bool>(ConfigurationSchema.ValueGreaterThanZero));

		public static ConfigurationSchema<int> WlmMinNumberOfChunksToProceed = new ReadOnceConfigurationSchema<int>("WlmMinNumberOfChunksToProceed", 1);

		public static ConfigurationSchema<TimeSpan> TombstoneMailboxExpirationPeriod = new ReadOnceConfigurationSchema<TimeSpan>("TombstoneMailboxExpirationPeriod", TimeSpan.FromDays(30.0), ConfigurationSchema.NoPostProcess<TimeSpan>(), new Func<TimeSpan, bool>(ConfigurationSchema.ValueGreaterThanZero));

		public static ConfigurationSchema<int> PromotionChunkSize = new ReadOnceConfigurationSchema<int>("PromotionChunkSize", 250);

		public static ConfigurationSchema<int> ActiveMailboxCacheSize = new ConfigurationSchema<int>("ActiveMailboxCacheSize", 1000);

		public static ConfigurationSchema<TimeSpan> ActiveMailboxCacheTimeToLive = new ConfigurationSchema<TimeSpan>("ActiveMailboxCacheTimeToLive", TimeSpan.FromMinutes(30.0), ConfigurationSchema.NoPostProcess<TimeSpan>(), new Func<TimeSpan, bool>(ConfigurationSchema.ValueGreaterThanZero));

		public static ConfigurationSchema<int> ActiveMailboxCacheCleanupThreshold = new ConfigurationSchema<int>("ActiveMailboxCacheCleanupThreshold", 5);

		public static ConfigurationSchema<bool> AggresiveUpdateMailboxTableSizeStatistics = new ReadOnceConfigurationSchema<bool>("AggresiveUpdateMailboxTableSizeStatistics", false);

		public static ConfigurationSchema<int> StoreQueryLimitRows = new ReadOnceConfigurationSchema<int>("StoreQueryLimitRows", DefaultSettings.Get.StoreQueryLimitRows);

		public static ConfigurationSchema<TimeSpan> StoreQueryLimitTime = new ReadOnceConfigurationSchema<TimeSpan>("StoreQueryLimitTime", DefaultSettings.Get.StoreQueryLimitTime);

		public static ConfigurationSchema<bool> EnableReadFromPassive = new ReadOnceConfigurationSchema<bool>("EnableReadFromPassive", DefaultSettings.Get.EnableReadFromPassive);

		public static ConfigurationSchema<TimeSpan> InferenceLogRetentionPeriod = new ReadOnceConfigurationSchema<TimeSpan>("InferenceLogRetentionPeriod", DefaultSettings.Get.InferenceLogRetentionPeriod, ConfigurationSchema.NoPostProcess<TimeSpan>(), new Func<TimeSpan, bool>(ConfigurationSchema.ValueGreaterThanZero));

		public static ConfigurationSchema<UnlimitedItems> InferenceLogMaxRows = new ConfigurationSchema<UnlimitedItems>("InferenceLogMaxRows", DefaultSettings.Get.InferenceLogMaxRows, new ConfigurationSchema<UnlimitedItems>.TryParse(UnlimitedItems.TryParse), ConfigurationSchema.NoPostProcess<UnlimitedItems>(), new Func<UnlimitedItems, bool>(ConfigurationSchema.ValueGreaterThanZero));

		public static ConfigurationSchema<bool> EnableDbDivergenceDetection = new ReadOnceConfigurationSchema<bool>("EnableDbDivergenceDetection", DefaultSettings.Get.EnableDbDivergenceDetection);

		public static ConfigurationSchema<bool> EnableConversationMasterIndex = new ReadOnceConfigurationSchema<bool>("EnableConversationMasterIndex", DefaultSettings.Get.EnableConversationMasterIndex);

		public static ConfigurationSchema<bool> ForceConversationMasterIndexUpgrade = new ReadOnceConfigurationSchema<bool>("ForceConversationMasterIndexUpgrade", DefaultSettings.Get.ForceConversationMasterIndexUpgrade);

		public static ConfigurationSchema<bool> ForceRimQueryMaterialization = new ReadOnceConfigurationSchema<bool>("ForceRimQueryMaterialization", DefaultSettings.Get.ForceRimQueryMaterialization);

		public static ConfigurationSchema<TimeSpan> LazyTransactionCommitTimeout = new ReadOnceConfigurationSchema<TimeSpan>("LazyTransactionCommitTimeout", DefaultSettings.Get.LazyTransactionCommitTimeout);

		public static ConfigurationSchema<bool> ScheduledISIntegEnabled = new ConfigurationSchema<bool>("ScheduledISIntegEnabled", DefaultSettings.Get.ScheduledISIntegEnabled);

		public static ConfigurationSchema<string> ScheduledISIntegDetectOnly = new ConfigurationSchema<string>("ScheduledISIntegDetectOnly", DefaultSettings.Get.ScheduledISIntegDetectOnly);

		public static ConfigurationSchema<string> ScheduledISIntegDetectAndFix = new ConfigurationSchema<string>("ScheduledISIntegDetectAndFix", DefaultSettings.Get.ScheduledISIntegDetectAndFix);

		public static ConfigurationSchema<int> StoreQueryMaximumResultSize = new ConfigurationSchema<int>("StoreQueryMaximumResultSize", 5242880);

		public static ConfigurationSchema<uint> DatabaseSizeLimitGB = new ConfigurationSchema<uint>("DatabaseSizeLimitGB", 0U, "SYSTEM\\CurrentControlSet\\Services\\MSExchangeIS\\{0}\\{1}-{2}", "Database Size Limit in Gb");

		public static ConfigurationSchema<uint> DatabaseWarningThresholdPercent = new ConfigurationSchema<uint>("DatabaseWarningThresholdPercent", 10U, "SYSTEM\\CurrentControlSet\\Services\\MSExchangeIS\\{0}\\{1}-{2}", "Database Size Buffer in Percentage");

		public static ConfigurationSchema<bool> SkipMoveEventExclusion = new ConfigurationSchema<bool>("SkipMoveEventExclusion", false, "SYSTEM\\CurrentControlSet\\Services\\MSExchangeIS\\ParametersSystem", "SkipMoveEventExclusion");

		public static ConfigurationSchema<bool> DisableSearchFolderAgeOut = new ConfigurationSchema<bool>("DisableSearchFolderAgeOut", false, "SYSTEM\\CurrentControlSet\\Services\\MSExchangeIS\\ParametersSystem", "Disable Search Folder Age-Out");

		public static ConfigurationSchema<bool> CheckStreamSize = new ConfigurationSchema<bool>("CheckStreamSize", true, "SYSTEM\\CurrentControlSet\\Services\\MSExchangeIS\\ParametersSystem", "Check stream size");

		public static ConfigurationSchema<long> PerUserSessionLimit = new ConfigurationSchema<long>("PerUserSessionLimit", 32L, (long value) => ConfigurationSchema.ObjectLimitsPostProcess(value, 32L), "SYSTEM\\CurrentControlSet\\Services\\MSExchangeIS\\ParametersSystem", "Maximum Allowed Sessions Per User");

		public static ConfigurationSchema<long> PerAdminSessionLimit = new ConfigurationSchema<long>("PerAdminSessionLimit", 65536L, (long value) => ConfigurationSchema.ObjectLimitsPostProcess(value, 65536L), "SYSTEM\\CurrentControlSet\\Services\\MSExchangeIS\\ParametersSystem", "Maximum Allowed Sessions Per Administrator");

		public static ConfigurationSchema<long> PerOtherSessionLimit = new ConfigurationSchema<long>("PerOtherSessionLimit", 16L, (long value) => ConfigurationSchema.ObjectLimitsPostProcess(value, 16L), "SYSTEM\\CurrentControlSet\\Services\\MSExchangeIS\\ParametersSystem", "Maximum Allowed Service Sessions Per User");

		public static ConfigurationSchema<long> PerServiceSessionLimit = new ConfigurationSchema<long>("PerServiceSessionLimit", 10000L, (long value) => ConfigurationSchema.ObjectLimitsPostProcess(value, 10000L), "SYSTEM\\CurrentControlSet\\Services\\MSExchangeIS\\ParametersSystem", "Maximum Allowed Exchange Sessions Per Service");

		public static ConfigurationSchema<long> PerSessionFolderLimit = new ConfigurationSchema<long>("PerSessionFolderLimit", 500L, (long value) => ConfigurationSchema.ObjectLimitsPostProcess(value, 500L), "SYSTEM\\CurrentControlSet\\Services\\MSExchangeIS\\ParametersSystem\\MaxObjsPerMapiSession", "objtFolder");

		public static ConfigurationSchema<long> PerSessionMessageLimit = new ConfigurationSchema<long>("PerSessionMessageLimit", 250L, (long value) => ConfigurationSchema.ObjectLimitsPostProcess(value, 250L), "SYSTEM\\CurrentControlSet\\Services\\MSExchangeIS\\ParametersSystem\\MaxObjsPerMapiSession", "objtMessage");

		public static ConfigurationSchema<long> PerSessionAttachmentLimit = new ConfigurationSchema<long>("PerSessionAttachmentLimit", 500L, (long value) => ConfigurationSchema.ObjectLimitsPostProcess(value, 500L), "SYSTEM\\CurrentControlSet\\Services\\MSExchangeIS\\ParametersSystem\\MaxObjsPerMapiSession", "objtAttachment");

		public static ConfigurationSchema<long> PerSessionStreamLimit = new ConfigurationSchema<long>("PerSessionStreamLimit", 250L, (long value) => ConfigurationSchema.ObjectLimitsPostProcess(value, 250L), "SYSTEM\\CurrentControlSet\\Services\\MSExchangeIS\\ParametersSystem\\MaxObjsPerMapiSession", "objtStream");

		public static ConfigurationSchema<long> PerSessionNotifyLimit = new ConfigurationSchema<long>("PerSessionNotifyLimit", 500000L, (long value) => ConfigurationSchema.ObjectLimitsPostProcess(value, 500000L), "SYSTEM\\CurrentControlSet\\Services\\MSExchangeIS\\ParametersSystem\\MaxObjsPerMapiSession", "objtNotify");

		public static ConfigurationSchema<long> PerSessionFolderViewLimit = new ConfigurationSchema<long>("PerSessionFolderViewLimit", 500L, (long value) => ConfigurationSchema.ObjectLimitsPostProcess(value, 500L), "SYSTEM\\CurrentControlSet\\Services\\MSExchangeIS\\ParametersSystem\\MaxObjsPerMapiSession", "objtFolderView");

		public static ConfigurationSchema<long> PerSessionMessageViewLimit = new ConfigurationSchema<long>("PerSessionMessageViewLimit", 500L, (long value) => ConfigurationSchema.ObjectLimitsPostProcess(value, 500L), "SYSTEM\\CurrentControlSet\\Services\\MSExchangeIS\\ParametersSystem\\MaxObjsPerMapiSession", "objtMessageView");

		public static ConfigurationSchema<long> PerSessionAttachmentViewLimit = new ConfigurationSchema<long>("PerSessionAttachmentViewLimit", 500L, (long value) => ConfigurationSchema.ObjectLimitsPostProcess(value, 500L), "SYSTEM\\CurrentControlSet\\Services\\MSExchangeIS\\ParametersSystem\\MaxObjsPerMapiSession", "objtAttachView");

		public static ConfigurationSchema<long> PerSessionACLViewLimit = new ConfigurationSchema<long>("PerSessionACLViewLimit", 500L, (long value) => ConfigurationSchema.ObjectLimitsPostProcess(value, 500L), "SYSTEM\\CurrentControlSet\\Services\\MSExchangeIS\\ParametersSystem\\MaxObjsPerMapiSession", "objtACLView");

		public static ConfigurationSchema<long> PerSessionFxSrcLimit = new ConfigurationSchema<long>("PerSessionFxSrcLimit", 500L, (long value) => ConfigurationSchema.ObjectLimitsPostProcess(value, 500L), "SYSTEM\\CurrentControlSet\\Services\\MSExchangeIS\\ParametersSystem\\MaxObjsPerMapiSession", "objtFXSrcStrm");

		public static ConfigurationSchema<long> PerSessionFxDstLimit = new ConfigurationSchema<long>("PerSessionFxDstLimit", 500L, (long value) => ConfigurationSchema.ObjectLimitsPostProcess(value, 500L), "SYSTEM\\CurrentControlSet\\Services\\MSExchangeIS\\ParametersSystem\\MaxObjsPerMapiSession", "objtFXDstStrm");

		public static ConfigurationSchema<ushort> MAPINamedPropsQuota = new ConfigurationSchema<ushort>("MAPINamedPropsQuota", 16384, (ushort value) => Math.Min(value, 32767), "SYSTEM\\CurrentControlSet\\Services\\MSExchangeIS\\{0}\\{1}-{2}", "Named Props Quota");

		public static ConfigurationSchema<int> MailboxMaintenanceIntervalMinutes = new ReadOnceConfigurationSchema<int>("MailboxMaintenanceIntervalMinutes", (int)TimeSpan.FromDays(1.0).TotalMinutes, "SYSTEM\\CurrentControlSet\\Services\\MSExchangeIS\\ParametersSystem", "MailboxMaintenanceIntervalMinutes");

		public static ConfigurationSchema<bool> EnableDatabaseUnusedSpaceScrubbing = new ReadOnceConfigurationSchema<bool>("EnableDatabaseUnusedSpaceScrubbing", DefaultSettings.Get.EnableDatabaseUnusedSpaceScrubbing);

		public static ConfigurationSchema<TimeSpan> ScheduledISIntegExecutePeriod = new ReadOnceConfigurationSchema<TimeSpan>("ScheduledISIntegExecutePeriod", DefaultSettings.Get.ScheduledISIntegExecutePeriod, ConfigurationSchema.NoPostProcess<TimeSpan>(), (TimeSpan value) => value > (StoreEnvironment.IsTestEnvironment ? TimeSpan.FromDays(0.0) : TimeSpan.FromDays(1.0)));

		public static ConfigurationSchema<int> ConfigurableSharedLockStage = new ConfigurationSchema<int>("ConfigurableSharedLockStage", DefaultSettings.Get.ConfigurableSharedLockStage);

		public static ConfigurationSchema<TimeSpan> EseLrukCorrInterval = new ReadOnceConfigurationSchema<TimeSpan>("EseLrukCorrInterval", DefaultSettings.Get.EseLrukCorrInterval);

		public static ConfigurationSchema<TimeSpan> EseLrukTimeout = new ReadOnceConfigurationSchema<TimeSpan>("EseLrukTimeout", DefaultSettings.Get.EseLrukTimeout);

		public static ConfigurationSchema<bool> DisableBypassTempStream = new ConfigurationSchema<bool>("DisableBypassTempStream", DefaultSettings.Get.DisableBypassTempStream);

		public static ConfigurationSchema<int> MaxIcsStatePropertySize = new ConfigurationSchema<int>("MaxIcsStatePropertySize", 11534336);

		public static ConfigurationSchema<TimeSpan> ADOperationTimeout = new ReadOnceConfigurationSchema<TimeSpan>("ADOperationTimeout", TimeSpan.FromMinutes(30.0));

		public static ConfigurationSchema<TimeSpan> DatabaseOperationTimeout = new ReadOnceConfigurationSchema<TimeSpan>("DatabaseOperationTimeout", TimeSpan.FromMinutes(10.0));

		public static ConfigurationSchema<int> MaxNumberOfMapiProperties = new ConfigurationSchema<int>("MaxNumberOfMapiProperties", 7168);

		public static ConfigurationSchema<bool> CleanupTempStreamBuffersOnRelease = new ReadOnceConfigurationSchema<bool>("CleanupTempStreamBuffersOnRelease", true);

		public static ConfigurationSchema<bool> CheckQuotaOnMessageCreate = new ConfigurationSchema<bool>("CheckQuotaOnMessageCreate", DefaultSettings.Get.CheckQuotaOnMessageCreate);

		public static ConfigurationSchema<bool> ValidatePublicFolderMailboxTypeMatch = new ConfigurationSchema<bool>("ValidatePublicFolderMailboxTypeMatch", true);

		public static ConfigurationSchema<int> MailboxInfoCacheSize = new ReadOnceConfigurationSchema<int>("MailboxInfoCacheSize", DefaultSettings.Get.MailboxInfoCacheSize);

		public static ConfigurationSchema<TimeSpan> MailboxInfoCacheTTL = new ReadOnceConfigurationSchema<TimeSpan>("MailboxInfoCacheTTL", TimeSpan.FromMinutes(15.0));

		public static ConfigurationSchema<int> ForeignMailboxInfoCacheSize = new ReadOnceConfigurationSchema<int>("ForeignMailboxInfoCacheSize", DefaultSettings.Get.ForeignMailboxInfoCacheSize);

		public static ConfigurationSchema<TimeSpan> ForeignMailboxInfoCacheTTL = new ReadOnceConfigurationSchema<TimeSpan>("ForeignMailboxInfoCacheTTL", TimeSpan.FromMinutes(15.0));

		public static ConfigurationSchema<int> AddressInfoCacheSize = new ReadOnceConfigurationSchema<int>("AddressInfoCacheSize", DefaultSettings.Get.AddressInfoCacheSize);

		public static ConfigurationSchema<TimeSpan> AddressInfoCacheTTL = new ReadOnceConfigurationSchema<TimeSpan>("AddressInfoCacheTTL", TimeSpan.FromMinutes(15.0));

		public static ConfigurationSchema<int> ForeignAddressInfoCacheSize = new ReadOnceConfigurationSchema<int>("ForeignAddressInfoCacheSize", DefaultSettings.Get.ForeignAddressInfoCacheSize);

		public static ConfigurationSchema<TimeSpan> ForeignAddressInfoCacheTTL = new ReadOnceConfigurationSchema<TimeSpan>("ForeignAddressInfoCacheTTL", TimeSpan.FromMinutes(15.0));

		public static ConfigurationSchema<int> DatabaseInfoCacheSize = new ReadOnceConfigurationSchema<int>("DatabaseInfoCacheSize", DefaultSettings.Get.DatabaseInfoCacheSize);

		public static ConfigurationSchema<TimeSpan> DatabaseInfoCacheTTL = new ReadOnceConfigurationSchema<TimeSpan>("DatabaseInfoCacheTTL", TimeSpan.FromMinutes(30.0));

		public static ConfigurationSchema<int> OrganizationContainerCacheSize = new ReadOnceConfigurationSchema<int>("OrganizationContainerCacheSize", DefaultSettings.Get.OrganizationContainerCacheSize);

		public static ConfigurationSchema<TimeSpan> OrganizationContainerCacheTTL = new ReadOnceConfigurationSchema<TimeSpan>("OrganizationContainerCacheTTL", TimeSpan.FromMinutes(15.0));

		public static ConfigurationSchema<bool> UseDirectorySharedCache = new ConfigurationSchema<bool>("UseDirectorySharedCache", DefaultSettings.Get.UseDirectorySharedCache);

		public static ConfigurationSchema<bool> SeparateDirectoryCaches = new ConfigurationSchema<bool>("SeparateDirectoryCaches", true);

		public static ConfigurationSchema<int> MaterializedRestrictionSearchFolderConfigStage = new ReadOnceConfigurationSchema<int>("MaterializedRestrictionSearchFolderConfigStage", DefaultSettings.Get.MaterializedRestrictionSearchFolderConfigStage, ConfigurationSchema.NoPostProcess<int>(), (int value) => value >= 0 && value <= 1);

		public static ConfigurationSchema<bool> AllowRecursiveFolderHierarchyRowCountApproximation = new ConfigurationSchema<bool>("AllowRecursiveFolderHierarchyRowCountApproximation", true);

		public static ConfigurationSchema<TimeSpan> ServerInfoCacheTTL = new ReadOnceConfigurationSchema<TimeSpan>("ServerInfoCacheTTL", TimeSpan.FromMinutes(10.0));

		public static ConfigurationSchema<TimeSpan> TransportInfoCacheTTL = new ReadOnceConfigurationSchema<TimeSpan>("TransportInfoCacheTTL", TimeSpan.FromMinutes(10.0));

		public static ConfigurationSchema<bool> AttachmentMessageSaveChunking = new ConfigurationSchema<bool>("AttachmentMessageSaveChunking", DefaultSettings.Get.AttachmentMessageSaveChunking);

		public static ConfigurationSchema<int> AttachmentMessageSaveChunkingMinSize = new ReadOnceConfigurationSchema<int>("AttachmentMessageSaveChunkingMinSize", 262144);

		public static ConfigurationSchema<bool> EnableSetSenderSpoofingFix = new ConfigurationSchema<bool>("EnableSetSenderSpoofingFix", true);

		public static ConfigurationSchema<bool> FixMessageCreatorSidOnMailboxMove = new ConfigurationSchema<bool>("FixMessageCreatorSidOnMailboxMove", true);

		public static ConfigurationSchema<int> TimeZoneBlobTruncationLength = new ConfigurationSchema<int>("TimeZoneBlobTruncationLength", DefaultSettings.Get.TimeZoneBlobTruncationLength, ConfigurationSchema.NoPostProcess<int>(), (int value) => value >= 510 && value <= 5120);

		public static ConfigurationSchema<int> MaxHitsForFullTextIndexSearches = new ConfigurationSchema<int>("MaxHitsForFullTextIndexSearches", 250, ConfigurationSchema.NoPostProcess<int>(), (int value) => value > 0 && value <= 1000000);

		public static ConfigurationSchema<bool> MultipleSyncSourceClientsForPublicFolderMailbox = new ConfigurationSchema<bool>("MultipleSyncSourceClientsForPublicFolderMailbox", true);

		private static List<ConfigurationSchema> registeredConfigurations;

		private static Task reloadTask;

		private static DateTime nextRefreshTime;

		private readonly string name;
	}
}
