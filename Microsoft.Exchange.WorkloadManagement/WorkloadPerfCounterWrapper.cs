using System;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Diagnostics.Components.WorkloadManagement;
using Microsoft.Exchange.WorkloadManagement.EventLogs;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal class WorkloadPerfCounterWrapper
	{
		public WorkloadPerfCounterWrapper(SystemWorkloadBase workload)
		{
			if (workload == null)
			{
				throw new ArgumentNullException("workload");
			}
			this.Workload = workload;
			string text = null;
			try
			{
				text = ResourceLoadPerfCounterWrapper.GetDefaultInstanceName();
				text = text + "_" + workload.WorkloadType;
				if (!string.Equals(workload.WorkloadType.ToString(), workload.Id, StringComparison.InvariantCultureIgnoreCase))
				{
					text = text + "_" + workload.Id;
				}
				this.perfCounters = MSExchangeWorkloadManagementWorkload.GetInstance(text);
				ExTraceGlobals.CommonTracer.TraceDebug<string>((long)this.GetHashCode(), "[WorkloadPerfCounterWrapper.ctor] Creating perf counter wrapper instance for '{0}'", text);
			}
			catch (Exception ex)
			{
				ExTraceGlobals.CommonTracer.TraceError<string, Exception>((long)this.GetHashCode(), "[WorkloadPerfCounterWrapper.ctor] Failed to create perf counter instance '{0}'.  Exception: {1}", text ?? "<NULL>", ex);
				WorkloadManagerEventLogger.LogEvent(WorkloadManagementEventLogConstants.Tuple_WorkloadPerformanceCounterInitializationFailure, workload.WorkloadType.ToString(), new object[]
				{
					workload.WorkloadType,
					workload.Id,
					ex
				});
				this.perfCounters = null;
			}
			this.UpdateActiveTasks(0L);
			this.UpdateBlockedTasks(0L);
			this.UpdateQueuedTasks(0L);
		}

		private WorkloadPerfCounterWrapper()
		{
			this.Workload = null;
			this.perfCounters = null;
		}

		internal SystemWorkloadBase Workload { get; private set; }

		internal void UpdateActiveTasks(long newCount)
		{
			this.SafeTouchCounter(delegate
			{
				this.perfCounters.ActiveTasks.RawValue = newCount;
			});
		}

		internal void UpdateQueuedTasks(long newCount)
		{
			this.SafeTouchCounter(delegate
			{
				this.perfCounters.QueuedTasks.RawValue = newCount;
			});
		}

		internal void UpdateTaskCompletion(TimeSpan taskLength)
		{
			this.SafeTouchCounter(delegate
			{
				this.tasksPerMinute.Add(1U);
				this.perfCounters.TasksPerMinute.RawValue = (long)((ulong)this.tasksPerMinute.GetValue());
				this.perfCounters.CompletedTasks.Increment();
			});
		}

		internal void UpdateTaskYielded()
		{
			this.SafeTouchCounter(delegate
			{
				this.perfCounters.YieldedTasks.Increment();
			});
		}

		internal void UpdateTaskStepLength(TimeSpan taskStepLength)
		{
			this.SafeTouchCounter(delegate
			{
				this.averageTaskStepLength.Add(Environment.TickCount, (uint)taskStepLength.TotalMilliseconds);
				this.perfCounters.AverageTaskStepLength.RawValue = (long)this.averageTaskStepLength.GetValue();
			});
		}

		internal void UpdateBlockedTasks(long newValue)
		{
			this.SafeTouchCounter(delegate
			{
				this.perfCounters.BlockedTasks.RawValue = newValue;
			});
		}

		internal void UpdateActiveState(bool active)
		{
			this.SafeTouchCounter(delegate
			{
				this.perfCounters.Active.RawValue = (active ? 1L : 0L);
			});
		}

		internal void Remove()
		{
			this.SafeTouchCounter(delegate
			{
				MSExchangeWorkloadManagementWorkload.RemoveInstance(this.perfCounters.Name);
				this.perfCounters = null;
			});
		}

		private void SafeTouchCounter(Action action)
		{
			if (this.perfCounters != null)
			{
				lock (this.instanceLock)
				{
					if (this.perfCounters != null)
					{
						action();
					}
				}
			}
		}

		public static readonly WorkloadPerfCounterWrapper Empty = new WorkloadPerfCounterWrapper();

		private object instanceLock = new object();

		private MSExchangeWorkloadManagementWorkloadInstance perfCounters;

		private FixedTimeSum tasksPerMinute = new FixedTimeSum(5000, 12);

		private FixedTimeAverage averageTaskStepLength = new FixedTimeAverage(5000, 12, Environment.TickCount);
	}
}
