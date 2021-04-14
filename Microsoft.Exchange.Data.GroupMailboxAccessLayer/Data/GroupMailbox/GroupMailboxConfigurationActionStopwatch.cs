using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.GroupMailbox;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class GroupMailboxConfigurationActionStopwatch : IDisposable
	{
		public GroupMailboxConfigurationActionStopwatch(GroupMailboxConfigurationReport report, GroupMailboxConfigurationAction actionType)
		{
			this.report = report;
			this.actionType = actionType;
			this.activityScope = ActivityContext.GetCurrentActivityScope();
			this.stopwatch = Stopwatch.StartNew();
			this.initialLatencyStatistics = this.CreateLatencyStatisticsSnapshot();
		}

		public void Dispose()
		{
			this.stopwatch.Stop();
			this.report.IsConfigurationExecuted = true;
			LatencyStatistics value = this.CreateLatencyStatisticsSnapshot() - this.initialLatencyStatistics;
			this.report.ConfigurationActionLatencyStatistics[this.actionType] = value;
			GroupMailboxConfigurationActionStopwatch.Tracer.TraceDebug<GroupMailboxConfigurationAction, string>((long)this.GetHashCode(), "Completed {0} in {1}ms.", this.actionType, value.ElapsedTime.TotalMilliseconds.ToString("n0"));
		}

		private LatencyStatistics CreateLatencyStatisticsSnapshot()
		{
			if (this.activityScope != null)
			{
				return new LatencyStatistics
				{
					ElapsedTime = this.stopwatch.Elapsed,
					ADLatency = new AggregatedOperationStatistics?(this.activityScope.TakeStatisticsSnapshot(AggregatedOperationType.ADCalls)),
					RpcLatency = new AggregatedOperationStatistics?(this.activityScope.TakeStatisticsSnapshot(AggregatedOperationType.StoreRPCs))
				};
			}
			return new LatencyStatistics
			{
				ElapsedTime = this.stopwatch.Elapsed
			};
		}

		private static readonly Microsoft.Exchange.Diagnostics.Trace Tracer = ExTraceGlobals.GroupMailboxAccessLayerTracer;

		private readonly Stopwatch stopwatch;

		private readonly IActivityScope activityScope;

		private readonly LatencyStatistics initialLatencyStatistics;

		private readonly GroupMailboxConfigurationReport report;

		private readonly GroupMailboxConfigurationAction actionType;
	}
}
