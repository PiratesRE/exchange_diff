using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Search
{
	internal static class SearchStrings
	{
		internal static string SearchEscalateResponderName(string monitorName)
		{
			return monitorName.Replace("Monitor", "Escalate");
		}

		internal static string SearchRestartServiceResponderName(string monitorName)
		{
			return monitorName.Replace("Monitor", "RestartSearchService");
		}

		internal static string SearchDatabaseFailoverResponderName(string monitorName)
		{
			return monitorName.Replace("Monitor", "DatabaseFailover");
		}

		internal static string SearchRestartHostControllerServiceResponderName(string monitorName)
		{
			return monitorName.Replace("Monitor", "RestartHostControllerService");
		}

		internal static string SearchResumeCatalogResponderName(string monitorName)
		{
			return monitorName.Replace("Monitor", "ResumeCatalog");
		}

		internal static string HostControllerServiceRestartNodeResponderName(string monitorName)
		{
			if (monitorName.Contains("Monitor"))
			{
				return monitorName.Replace("Monitor", "RestartNode");
			}
			return monitorName + "RestartNode";
		}

		internal const string SearchMountedCopyStatusProbeName = "SearchMountedCopyStatusProbe";

		internal const string SearchMountedCopyStatusMonitorName = "SearchMountedCopyStatusMonitor";

		internal const string SearchIndexSuspendedProbeName = "SearchIndexSuspendedProbe";

		internal const string SearchIndexSuspendedMonitorName = "SearchIndexSuspendedMonitor";

		internal const string HostControllerServiceRunningProbeName = "HostControllerServiceRunningProbe";

		internal const string HostControllerServiceRunningMonitorName = "HostControllerServiceRunningMonitor";

		internal const string FastNodeCrashProbeName = "FastNodeCrashProbe";

		internal const string FastNodeCrashMonitorName = "FastNodeCrashMonitor";

		internal const string FastNodeAvailabilityProbeName = "FastNodeAvailabilityProbe";

		internal const string FastNodeAvailabilityMonitorName = "FastNodeAvailabilityMonitor";

		internal const string FastNodeRestartProbeName = "FastNodeRestartProbe";

		internal const string FastNodeRestartMonitorName = "FastNodeRestartMonitor";

		internal const string SearchQueryStxProbeName = "SearchQueryStxProbe";

		internal const string SearchQueryStxMonitorName = "SearchQueryStxMonitor";

		internal const string SearchInstantSearchStxProbeName = "SearchInstantSearchStxProbe";

		internal const string SearchInstantSearchStxMonitorName = "SearchInstantSearchStxMonitor";

		internal const string SearchCatalogAvailabilityProbeName = "SearchCatalogAvailabilityProbe";

		internal const string SearchCatalogAvailabilityMonitorName = "SearchCatalogAvailabilityMonitor";

		internal const string SearchServiceRunningProbeName = "SearchServiceRunningProbe";

		internal const string SearchServiceRunningMonitorName = "SearchServiceRunningMonitor";

		internal const string SearchIndexBacklogProbeName = "SearchIndexBacklogProbe";

		internal const string SearchIndexBacklogMonitorName = "SearchIndexBacklogMonitor";

		internal const string SearchIndexFailureProbeName = "SearchIndexFailureProbe";

		internal const string SearchIndexFailureMonitorName = "SearchIndexFailureMonitor";

		internal const string SearchSingleCopyProbeName = "SearchSingleCopyProbe";

		internal const string SearchSingleCopyMonitorName = "SearchSingleCopyMonitor";

		internal const string SearchLocalMountedCopyStatusProbeName = "SearchLocalMountedCopyStatusProbe";

		internal const string SearchLocalMountedCopyStatusMonitorName = "SearchLocalMountedCopyStatusMonitor";

		internal const string SearchLocalPassiveCopyStatusProbeName = "SearchLocalPassiveCopyStatusProbe";

		internal const string SearchLocalPassiveCopyStatusMonitorName = "SearchLocalPassiveCopyStatusMonitor";

		internal const string SearchCrawlingProgressProbeName = "SearchCrawlingProgressProbe";

		internal const string SearchCrawlingProgressMonitorName = "SearchCrawlingProgressMonitor";

		internal const string SearchQueryFailureProbeName = "SearchQueryFailureProbe";

		internal const string SearchQueryFailureMonitorName = "SearchQueryFailureMonitor";

		internal const string SearchServiceCrashProbeName = "SearchServiceCrashProbe";

		internal const string SearchServiceCrashMonitorName = "SearchServiceCrashMonitor";

		internal const string SearchCatalogSizeProbeName = "SearchCatalogSizeProbe";

		internal const string SearchCatalogSizeMonitorName = "SearchCatalogSizeMonitor";

		internal const string SearchTransportAgentProbeName = "SearchTransportAgentProbe";

		internal const string SearchTransportAgentMonitorName = "SearchTransportAgentMonitor";

		internal const string SearchWordBreakerLoadingProbeName = "SearchWordBreakerLoadingProbe";

		internal const string SearchWordBreakerLoadingMonitorName = "SearchWordBreakerLoadingMonitor";

		internal const string SearchResourceLoadProbeName = "SearchResourceLoadProbe";

		internal const string SearchResourceLoadMonitorName = "SearchResourceLoadMonitor";

		internal const string SearchFeedingControllerFailureMonitorName = "SearchFeedingControllerFailureMonitor";

		internal const string SearchGracefulDegradationManagerFailureMonitorName = "SearchGracefulDegradationManagerFailureMonitor";

		internal const string FastIndexNumDiskPartsMonitorName = "FastIndexNumDiskPartsMonitor";

		internal const string Monitor = "Monitor";

		internal const string FromInvokeMonitoringItemPropertyName = "FromInvokeMonitoringItem";

		internal const string SearchParserServerDegradeProbeName = "SearchParserServerDegradeProbe";

		internal const string SearchMemoryOverThresholdProbeName = "SearchMemoryOverThresholdProbe";

		internal const string SearchMemoryOverThresholdMonitorName = "SearchMemoryOverThresholdMonitor";

		internal const string SearchGracefulDegradationStatusProbeName = "SearchGracefulDegradationStatusProbe";

		internal const string SearchGracefulDegradationStatusMonitorName = "SearchGracefulDegradationStatusMonitor";

		internal const string SearchRopNotSupportedMonitorName = "SearchRopNotSupportedMonitor";

		internal const string SearchCopyStatusHaImpactingProbeName = "SearchCopyStatusHaImpactingProbe";

		internal const string SearchCopyStatusHaImpactingMonitorName = "SearchCopyStatusHaImpactingMonitor";
	}
}
