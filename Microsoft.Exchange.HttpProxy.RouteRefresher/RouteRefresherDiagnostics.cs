using System;
using Microsoft.Exchange.HttpProxy.Common;

namespace Microsoft.Exchange.HttpProxy.RouteRefresher
{
	public class RouteRefresherDiagnostics : IRouteRefresherDiagnostics
	{
		public RouteRefresherDiagnostics(RequestLogger baseLogger)
		{
			this.baseLogger = baseLogger;
		}

		public void AddErrorInfo(object value)
		{
			this.baseLogger.AppendErrorInfo("RouteRefresher", value);
		}

		public void AddGenericInfo(object value)
		{
			this.baseLogger.AppendGenericInfo("RouteRefresher", value);
		}

		public void IncrementSuccessfulMailboxServerCacheUpdates()
		{
			PerfCounters.HttpProxyCacheCountersInstance.RouteRefresherSuccessfulMailboxServerCacheUpdates.Increment();
		}

		public void IncrementTotalMailboxServerCacheUpdateAttempts()
		{
			PerfCounters.HttpProxyCacheCountersInstance.RouteRefresherTotalMailboxServerCacheUpdateAttempts.Increment();
		}

		public void IncrementSuccessfulAnchorMailboxCacheUpdates()
		{
			PerfCounters.HttpProxyCacheCountersInstance.RouteRefresherSuccessfulAnchorMailboxCacheUpdates.Increment();
		}

		public void IncrementTotalAnchorMailboxCacheUpdateAttempts()
		{
			PerfCounters.HttpProxyCacheCountersInstance.RouteRefresherTotalAnchorMailboxCacheUpdateAttempts.Increment();
		}

		public void LogRouteRefresherLatency(Action operationToTrack)
		{
			this.baseLogger.LatencyTracker.LogLatency(LogKey.RouteRefresherLatency, operationToTrack);
		}

		private readonly RequestLogger baseLogger;
	}
}
