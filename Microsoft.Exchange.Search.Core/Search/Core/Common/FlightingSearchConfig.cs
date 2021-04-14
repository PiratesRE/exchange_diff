using System;
using Microsoft.Exchange.Search.OperatorSchema;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Search.Core.Common
{
	internal class FlightingSearchConfig : SearchConfig
	{
		internal FlightingSearchConfig() : base(Guid.Empty)
		{
		}

		internal FlightingSearchConfig(Guid databaseGuid) : base(databaseGuid)
		{
		}

		protected override void Load(bool resetRegistryCache)
		{
			base.Load(false);
			VariantConfigurationSnapshot snapshot = VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null);
			base.SchemaUpgradingEnabled = base.ReadBool("SchemaUpgradingEnabled", snapshot.Search.SchemaUpgrading.Enabled);
			base.ReadFlagEnabled = base.ReadBool("ReadFlagEnabled", snapshot.Search.ReadFlag.Enabled);
			base.DisableInstantSearch = base.ReadBool("DisableInstantSearch", !snapshot.Search.EnableInstantSearch.Enabled);
			base.UseExecuteAndReadPage = base.ReadBool("UseExecuteAndReadPage", snapshot.Search.UseExecuteAndReadPage.Enabled);
			base.TopNEnabled = base.ReadBool("TopNEnabled", snapshot.Search.EnableTopN.Enabled);
			base.EnableIndexPartsCache = base.ReadBool("EnableIndexPartsCache", snapshot.Search.EnableIndexPartsCache.Enabled);
			base.EnableSingleValueRefiners = base.ReadBool("EnableSingleValueRefiners", snapshot.Search.EnableSingleValueRefiners.Enabled);
			base.GracefulDegradationEnabled = base.ReadBool("GracefulDegradationEnabled", snapshot.Search.EnableGracefulDegradation.Enabled);
			base.MonitorDocumentValidationFailures = base.ReadBool("MonitorDocumentValidationFailures", snapshot.Search.MonitorDocumentValidationFailures.Enabled);
			base.CachePreWarmingEnabled = base.ReadBool("CachePreWarmingEnabled", snapshot.Search.CachePreWarmingEnabled.Enabled);
			base.ProcessItemsWithNullCompositeId = base.ReadBool("ProcessItemsWithNullCompositeId", snapshot.Search.ProcessItemsWithNullCompositeId.Enabled);
			base.MaxCompletionTraversalCount = base.ReadInt("MaxCompletionTraversalCount", snapshot.Search.Completions.MaxCompletionTraversalCount);
			base.TopNExclusionCharacters = base.ReadCharArray("TopNExclusionCharacters", snapshot.Search.Completions.TopNExclusionCharacters);
			base.FinalWordSuggestionsEnabled = base.ReadBool("FinalWordSuggestionsEnabled", snapshot.Search.Completions.FinalWordSuggestionsEnabled);
			base.CrawlerFeederUpdateCrawlingStatusResetCache = base.ReadBool("CrawlerFeederUpdateCrawlingStatusResetCache", snapshot.Search.CrawlerFeederUpdateCrawlingStatusResetCache.Enabled);
			base.CrawlerFeederCollectDocumentsVerifyPendingWatermarks = base.ReadBool("CrawlerFeederCollectDocumentsVerifyPendingWatermarks", snapshot.Search.CrawlerFeederCollectDocumentsVerifyPendingWatermarks.Enabled);
			base.EnableIndexStatusTimestampVerification = base.ReadBool("EnableIndexStatusTimestampVerification", snapshot.Search.EnableIndexStatusTimestampVerification.Enabled);
			base.IndexStatusInvalidateInterval = base.ReadTimeSpan("IndexStatusInvalidateInterval", snapshot.Search.IndexStatusInvalidateInterval.InvalidateInterval);
			base.RemoveOrphanedCatalogs = base.ReadBool("RemoveOrphanedCatalogs", snapshot.Search.RemoveOrphanedCatalogs.Enabled);
			base.SandboxMaxPoolSize = base.ReadInt("SandboxMaxPoolSize", snapshot.Search.MemoryModel.SandboxMaxPoolSize);
			base.LowAvailableSystemWorkingSetMemoryRatio = base.ReadInt("LowAvailableSystemWorkingSetMemoryRatio", snapshot.Search.MemoryModel.LowAvailableSystemWorkingSetMemoryRatio);
			base.SearchMemoryModelBaseCost = base.ReadLong("SearchMemoryModelBaseCost", snapshot.Search.MemoryModel.SearchMemoryModelBaseCost);
			base.BaselineCostPerActiveItem = base.ReadLong("BaselineCostPerActiveItem", snapshot.Search.MemoryModel.BaselineCostPerActiveItem);
			base.BaselineCostPerPassiveItem = base.ReadLong("BaselineCostPerPassiveItem", snapshot.Search.MemoryModel.BaselineCostPerPassiveItem);
			base.InstantSearchCostPerActiveItem = base.ReadLong("InstantSearchCostPerActiveItem", snapshot.Search.MemoryModel.InstantSearchCostPerActiveItem);
			base.RefinersCostPerActiveItem = base.ReadLong("RefinersCostPerActiveItem", snapshot.Search.MemoryModel.RefinersCostPerActiveItem);
			base.DisableGracefulDegradationForInstantSearch = base.ReadBool("DisableGracefulDegradationForInstantSearch", snapshot.Search.MemoryModel.DisableGracefulDegradationForInstantSearch);
			base.DisableGracefulDegradationForAutoSuspend = base.ReadBool("DisableGracefulDegradationForAutoSuspend", snapshot.Search.MemoryModel.DisableGracefulDegradationForAutoSuspend);
			base.TimerForGracefulDegradation = base.ReadInt("TimerForGracefulDegradation", snapshot.Search.MemoryModel.TimerForGracefulDegradation);
			base.MemoryMeasureDrift = base.ReadLong("MemoryMeasureDrift", snapshot.Search.MemoryModel.MemoryMeasureDrift);
			base.MaxRestoreAmount = base.ReadLong("MaxRestoreAmount", snapshot.Search.MemoryModel.MaxRestoreAmount);
			base.ShouldConsiderSearchMemoryUsageBudget = base.ReadBool("ShouldConsiderSearchMemoryUsageBudget", snapshot.Search.MemoryModel.ShouldConsiderSearchMemoryUsageBudget);
			base.SearchMemoryUsageBudget = base.ReadLong("SearchMemoryUsageBudget", snapshot.Search.MemoryModel.SearchMemoryUsageBudget);
			base.SearchMemoryUsageBudgetFloatingAmount = base.ReadLong("SearchMemoryUsageBudgetFloatingAmount", snapshot.Search.MemoryModel.SearchMemoryUsageBudgetFloatingAmount);
			base.QueueSize = base.ReadInt("QueueSize", snapshot.Search.FeederSettings.QueueSize);
			base.DocumentFeederProcessingTimeout = base.ReadTimeSpan("BatchSize", snapshot.Search.DocumentFeederSettings.BatchTimeout);
			base.DocumentFeederLostCallbackTimeout = base.ReadTimeSpan("LostCallbackTimeout", snapshot.Search.DocumentFeederSettings.LostCallbackTimeout);
			base.UseAlphaSchema = base.ReadBool("UseAlphaSchema", snapshot.Search.UseAlphaSchema.Enabled);
			base.UseBetaSchema = base.ReadBool("UseBetaSchema", snapshot.Search.UseBetaSchema.Enabled);
			base.UseMdmFlow = base.ReadBool("UseMdmFlow", snapshot.Search.TransportFlowSettings.UseMdmFlow);
			base.SkipMdmGeneration = base.ReadBool("SkipMdmGeneration", snapshot.Search.TransportFlowSettings.SkipMdmGeneration);
			base.SkipTokenInfoGeneration = base.ReadBool("SkipTokenInfoGeneration", snapshot.Search.TransportFlowSettings.SkipTokenInfoGeneration);
			base.FastQueryPath = base.ReadInt("FastQueryPath", snapshot.Search.InstantSearch.FastQueryPath);
			base.SamplingFrequency = base.ReadInt("SamplingFrequency", snapshot.Search.LanguageDetection.SamplingFrequency);
			base.ResetDbConfigRegistryValues(resetRegistryCache);
		}
	}
}
