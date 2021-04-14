using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	public class OverallPercentSuccessNoTimeoutMonitor : OverallPercentSuccessMonitor
	{
		protected override Task SetPercentSuccessNumbers(CancellationToken cancellationToken)
		{
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "OverallPercentSuccessMonitor: Getting overall percent success of: {0}.", base.Definition.SampleMask, null, "SetPercentSuccessNumbers", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\WorkItems\\Monitors\\OverallPercentSuccessNoTimeoutMonitor.cs", 42);
			base.GetLastFailedProbeResultId(base.Definition.SampleMask, cancellationToken);
			Task<Dictionary<ResultType, int>> resultTypeCountsForNewProbeResults = base.GetResultTypeCountsForNewProbeResults(base.Definition.SampleMask, false, cancellationToken);
			resultTypeCountsForNewProbeResults.Continue(delegate(Dictionary<ResultType, int> resultTypeCounts)
			{
				int newSampleCount;
				int newFailedCount;
				double newValue;
				this.GetResultStatistics(resultTypeCounts, out newSampleCount, out newFailedCount, out newValue);
				base.Result.NewSampleCount = newSampleCount;
				base.Result.NewFailedCount = newFailedCount;
				base.Result.NewValue = newValue;
				WTFDiagnostics.TraceDebug<int>(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "OverallPercentSuccessMonitor: Processed {0} new result(s).", base.Result.NewSampleCount, null, "SetPercentSuccessNumbers", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\WorkItems\\Monitors\\OverallPercentSuccessNoTimeoutMonitor.cs", 69);
			}, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled);
			Task<Dictionary<ResultType, int>> resultTypeCountsForAllProbeResults = base.GetResultTypeCountsForAllProbeResults(base.Definition.SampleMask, false, cancellationToken);
			return resultTypeCountsForAllProbeResults.Continue(delegate(Dictionary<ResultType, int> resultTypeCounts)
			{
				int totalSampleCount;
				int totalFailedCount;
				double totalValue;
				this.GetResultStatistics(resultTypeCounts, out totalSampleCount, out totalFailedCount, out totalValue);
				base.Result.TotalSampleCount = totalSampleCount;
				base.Result.TotalFailedCount = totalFailedCount;
				base.Result.TotalValue = totalValue;
				WTFDiagnostics.TraceDebug<int>(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "OverallPercentSuccessMonitor: Processed {0} total result(s).", base.Result.TotalSampleCount, null, "SetPercentSuccessNumbers", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\WorkItems\\Monitors\\OverallPercentSuccessNoTimeoutMonitor.cs", 95);
			}, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled);
		}

		protected override void GetResultStatistics(Dictionary<ResultType, int> resultTypeCounts, out int totalCount, out int totalFailedCount, out double percentSuccess)
		{
			resultTypeCounts.TryGetValue(ResultType.Failed, out totalFailedCount);
			int num;
			resultTypeCounts.TryGetValue(ResultType.Succeeded, out num);
			totalCount = totalFailedCount + num;
			percentSuccess = 100.0;
			if (totalCount != 0)
			{
				percentSuccess = (double)(num * 100 / totalCount);
			}
		}
	}
}
