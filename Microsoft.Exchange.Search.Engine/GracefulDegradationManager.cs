using System;
using System.Collections.Generic;
using Microsoft.Exchange.Search.Core.Diagnostics;
using Microsoft.Exchange.Search.Mdb;
using Microsoft.Exchange.Search.OperatorSchema;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Search.Engine
{
	internal class GracefulDegradationManager
	{
		internal static DateTime RecentGracefulDegradationExecutionTime
		{
			get
			{
				return GracefulDegradationManager.recentGracefulDegradationExecutionTime;
			}
			private set
			{
				GracefulDegradationManager.recentGracefulDegradationExecutionTime = value;
			}
		}

		internal static void DetermineFeatureStatusAndUpdate(IEnumerable<MdbInfo> allMDBs, ISearchServiceConfig config, IDiagnosticsSession diagnosticsSession)
		{
			SearchFeatureState currentState = SearchFeatureState.GetCurrentState(config, diagnosticsSession, allMDBs);
			long andLogUsageBySearchMemoryModel = GracefulDegradationManager.GetAndLogUsageBySearchMemoryModel(currentState, diagnosticsSession);
			currentState.SearchMemoryModel.SearchMemoryUsageDrift = currentState.SearchMemoryModel.SearchMemoryUsage - andLogUsageBySearchMemoryModel;
			if (config.GracefulDegradationEnabled)
			{
				if (config.TimerForGracefulDegradation < 1)
				{
					GracefulDegradationManager.counterForGracefulDegradationExecution = 1;
					diagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Warnings, "Config TimerForGracefulDegradation: unexpected value {0}.", new object[]
					{
						config.TimerForGracefulDegradation
					});
				}
				if (GracefulDegradationManager.counterForGracefulDegradationExecution > 1)
				{
					GracefulDegradationManager.counterForGracefulDegradationExecution--;
					diagnosticsSession.TraceDebug<int>("Graceful degradation skipped: next execution in {0} minutes", GracefulDegradationManager.counterForGracefulDegradationExecution * config.SyncMdbsInterval.Minutes);
					return;
				}
				SearchFeatureState nextState = currentState.GetNextState();
				SearchFeatureState allOnState = SearchFeatureState.GetAllOnState(config, diagnosticsSession, allMDBs);
				if (!nextState.Equals(allOnState) && allOnState.SearchMemoryModel.IsUnderSearchBudget())
				{
					string memoryUsageInfo = string.Format("Search memory usage: {0}; Search memory budget: {1}; Expected search memory usage with all features on: {2}; Current available memory: {3}.", new object[]
					{
						SearchMemoryModel.GetSearchMemoryUsage(),
						currentState.SearchMemoryModel.SearchDesiredFreeMemory,
						allOnState.SearchMemoryModel.SearchMemoryUsage,
						currentState.SearchMemoryModel.AvailPhys
					});
					EventNotificationItem.Publish(ExchangeComponent.Search.Name, "GracefulDegradationManagerFailure", string.Empty, Strings.GracefulDegradationManagerException(memoryUsageInfo, new CatalogItemStatistics(currentState.MdbInfos).ToString()), ResultSeverityLevel.Error, false);
				}
				nextState.WriteCurrentStateToRegistry();
				GracefulDegradationManager.recentGracefulDegradationExecutionTime = DateTime.UtcNow;
				GracefulDegradationManager.counterForGracefulDegradationExecution = config.TimerForGracefulDegradation;
			}
		}

		private static long GetAndLogUsageBySearchMemoryModel(SearchFeatureState searchFeatureState, IDiagnosticsSession diagnosticsSession)
		{
			CatalogItemStatistics catalogItemStatistics = new CatalogItemStatistics(searchFeatureState.MdbInfos);
			long expectedSearchMemoryUsage = searchFeatureState.SearchMemoryModel.GetExpectedSearchMemoryUsage(catalogItemStatistics.ActiveItems, catalogItemStatistics.PassiveItemsCatalogSuspendedOff, catalogItemStatistics.ActiveItemsInstantSearchOn, catalogItemStatistics.ActiveItemsRefinersOn);
			float searchMemoryDriftRatio = (float)(searchFeatureState.SearchMemoryModel.SearchMemoryUsage - expectedSearchMemoryUsage) / (float)searchFeatureState.Config.MemoryMeasureDrift;
			diagnosticsSession.LogGracefulDegradationInfo(DiagnosticsLoggingTag.Informational, searchFeatureState.SearchMemoryModel.TotalPhys, searchFeatureState.SearchMemoryModel.AvailPhys, searchFeatureState.SearchMemoryModel.SearchMemoryUsage, expectedSearchMemoryUsage, searchMemoryDriftRatio, CatalogItemStatistics.GenerateFeatureStateLoggingInfo(searchFeatureState.MdbInfos), new object[0]);
			return expectedSearchMemoryUsage;
		}

		internal const string GracefulDegradationManagerFailure = "GracefulDegradationManagerFailure";

		internal const string SearchMemoryModelFailure = "SearchMemoryModelFailure";

		private static DateTime recentGracefulDegradationExecutionTime = DateTime.MinValue;

		private static int counterForGracefulDegradationExecution = 1;
	}
}
