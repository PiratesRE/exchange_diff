using System;
using System.Collections.Concurrent;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.EventLog;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal class UserWorkloadManagerPerfCounterWrapper
	{
		public UserWorkloadManagerPerfCounterWrapper(BudgetType budgetType)
		{
			try
			{
				this.delayTimeThreshold = (UserWorkloadManagerPerfCounterWrapper.defaultDelayTimeThreshold ?? DefaultThrottlingAlertValues.DelayTimeThreshold(budgetType));
				this.perfCounters = MSExchangeUserWorkloadManager.GetInstance(budgetType.ToString());
				this.perfCounters.DelayTimeThreshold.RawValue = (long)this.delayTimeThreshold;
				this.lastClearTime = TimeProvider.UtcNow;
				this.perfCounters.AverageTaskWaitTime.RawValue = 0L;
				this.perfCounters.MaxTaskQueueLength.RawValue = (long)UserWorkloadManagerPerfCounterWrapper.maxTasksQueued;
				this.perfCounters.MaxWorkerThreadCount.RawValue = (long)UserWorkloadManagerPerfCounterWrapper.maxThreadCount;
				this.perfCounters.MaxDelayedTasks.RawValue = (long)UserWorkloadManagerPerfCounterWrapper.maxDelayCacheTasks;
				this.perfCounters.TaskQueueLength.RawValue = 0L;
				this.perfCounters.TotalNewTaskRejections.RawValue = 0L;
				this.perfCounters.TotalNewTasks.RawValue = 0L;
				this.perfCounters.TotalTaskExecutionFailures.RawValue = 0L;
				this.perfCounters.MaxDelayPerMinute.RawValue = 0L;
				this.perfCounters.TaskTimeoutsPerMinute.RawValue = 0L;
				this.initialized = true;
			}
			catch (Exception ex)
			{
				this.initialized = false;
				Globals.LogEvent(DirectoryEventLogConstants.Tuple_InitializePerformanceCountersFailed, string.Empty, new object[]
				{
					ex.ToString()
				});
				ExTraceGlobals.ClientThrottlingTracer.TraceError<string, string>(0L, "[UserWorkloadManagerPerfCounterWrapper::PerfCounters] Perf counter initialization failed with exception type: {0}, Messsage: {1}", ex.GetType().FullName, ex.Message);
			}
		}

		public static bool ConfigInitialized { get; private set; }

		internal static TimeSpan PerfCounterRefreshWindow
		{
			get
			{
				return UserWorkloadManagerPerfCounterWrapper.perfCounterRefreshWindow;
			}
			set
			{
				UserWorkloadManagerPerfCounterWrapper.perfCounterRefreshWindow = value;
			}
		}

		public static void Initialize(int maxTasksQueued, int maxThreadCount, int maxDelayCacheTasks, int? delayTimeThreshold)
		{
			if (UserWorkloadManagerPerfCounterWrapper.ConfigInitialized)
			{
				if (UserWorkloadManagerPerfCounterWrapper.maxTasksQueued == maxTasksQueued && UserWorkloadManagerPerfCounterWrapper.maxThreadCount == maxThreadCount && UserWorkloadManagerPerfCounterWrapper.maxDelayCacheTasks == maxDelayCacheTasks && UserWorkloadManagerPerfCounterWrapper.defaultDelayTimeThreshold == delayTimeThreshold)
				{
					return;
				}
				throw new InvalidOperationException("WorkloadManager PerformanceCounters were already initialized.");
			}
			else
			{
				if (delayTimeThreshold != null && delayTimeThreshold.Value <= 0)
				{
					throw new ArgumentOutOfRangeException("delayTimeThreshold", delayTimeThreshold.Value, "delayTimeThreshold must be greater than 0.");
				}
				try
				{
					UserWorkloadManagerPerfCounterWrapper.maxTasksQueued = maxTasksQueued;
					UserWorkloadManagerPerfCounterWrapper.maxThreadCount = maxThreadCount;
					UserWorkloadManagerPerfCounterWrapper.maxDelayCacheTasks = maxDelayCacheTasks;
					UserWorkloadManagerPerfCounterWrapper.defaultDelayTimeThreshold = delayTimeThreshold;
					UserWorkloadManagerPerfCounterWrapper.ConfigInitialized = true;
				}
				catch (Exception ex)
				{
					UserWorkloadManagerPerfCounterWrapper.ConfigInitialized = false;
					Globals.LogEvent(DirectoryEventLogConstants.Tuple_InitializePerformanceCountersFailed, string.Empty, new object[]
					{
						ex.ToString()
					});
					ExTraceGlobals.ClientThrottlingTracer.TraceError<string, string>(0L, "[UserWorkloadManagerPerfCounterWrapper::PerfCounters] Perf counter initialization failed with exception type: {0}, Messsage: {1}", ex.GetType().FullName, ex.Message);
				}
				return;
			}
		}

		public void IncrementTimeoutsSeen()
		{
			this.SafeUpdateCounter("TaskTimeoutsPerMinute", delegate
			{
				this.ClearDictionaryIfNecessary();
				this.perfCounters.TaskTimeoutsPerMinute.Increment();
			});
		}

		public void UpdateMaxDelay(BudgetKey key, int delay)
		{
			if (!this.initialized)
			{
				return;
			}
			this.ClearDictionaryIfNecessary();
			bool flag = false;
			lock (this.syncRoot)
			{
				if (this.maxDelay < delay)
				{
					this.maxDelay = delay;
					this.perfCounters.MaxDelayPerMinute.RawValue = (long)delay;
				}
				if (delay >= this.delayTimeThreshold)
				{
					flag = !UserWorkloadManagerPerfCounterWrapper.uniqueDelayedBudgetKeys.ContainsKey(key);
					if (flag)
					{
						UserWorkloadManagerPerfCounterWrapper.uniqueDelayedBudgetKeys[key] = 0;
					}
				}
			}
			if (flag)
			{
				this.perfCounters.UsersDelayedXMillisecondsPerMinute.Increment();
			}
		}

		public MSExchangeUserWorkloadManagerInstance GetCounterForTest()
		{
			if (!this.initialized)
			{
				return null;
			}
			return this.perfCounters;
		}

		public void UpdateAverageTaskWaitTime(long newValue)
		{
			this.SafeUpdateCounter("AverageTaskWaitTime", delegate
			{
				this.perfCounters.AverageTaskWaitTime.RawValue = (long)UserWorkloadManagerPerfCounterWrapper.averageTaskWaitTime.Update((float)newValue);
			});
		}

		public void UpdateTaskQueueLength(long length)
		{
			this.SafeUpdateCounter("TaskQueueLength", delegate
			{
				this.perfCounters.TaskQueueLength.RawValue = length;
			});
		}

		public void UpdateTotalNewTaskRejectionsCount()
		{
			this.SafeUpdateCounter("TotalNewTaskRejections", delegate
			{
				this.perfCounters.TotalNewTaskRejections.Increment();
			});
		}

		public void UpdateTotalNewTasksCount()
		{
			this.SafeUpdateCounter("TotalNewTasks", delegate
			{
				this.perfCounters.TotalNewTasks.Increment();
			});
		}

		public void UpdateTotalTaskExecutionFailuresCount()
		{
			this.SafeUpdateCounter("TotalTaskExecutionFailures", delegate
			{
				this.perfCounters.TotalTaskExecutionFailures.Increment();
			});
		}

		public void UpdateCurrentDelayedTasks(long count)
		{
			this.SafeUpdateCounter("CurrentDelayedTasks", delegate
			{
				this.perfCounters.CurrentDelayedTasks.RawValue = count;
			});
		}

		internal void ForceClearDictionary()
		{
			lock (this.syncRoot)
			{
				UserWorkloadManagerPerfCounterWrapper.uniqueDelayedBudgetKeys.Clear();
				this.maxDelay = 0;
			}
			this.perfCounters.MaxDelayPerMinute.RawValue = 0L;
			this.perfCounters.TaskTimeoutsPerMinute.RawValue = 0L;
			this.perfCounters.UsersDelayedXMillisecondsPerMinute.RawValue = 0L;
		}

		private void SafeUpdateCounter(string counterName, Action updateAction)
		{
			if (this.initialized)
			{
				try
				{
					updateAction();
				}
				catch (InvalidOperationException arg)
				{
					ExTraceGlobals.ClientThrottlingTracer.TraceError<InvalidOperationException>(0L, "Failed to update " + counterName + " performance counter. Error: {0}.", arg);
				}
			}
		}

		private void ClearDictionaryIfNecessary()
		{
			if (TimeProvider.UtcNow - this.lastClearTime > UserWorkloadManagerPerfCounterWrapper.perfCounterRefreshWindow)
			{
				this.lastClearTime = TimeProvider.UtcNow;
				this.ForceClearDictionary();
			}
		}

		private static readonly TimeSpan OneMinute = TimeSpan.FromMinutes(1.0);

		private static RunningAverageFloat averageTaskWaitTime = new RunningAverageFloat(25);

		private static int? defaultDelayTimeThreshold;

		private static TimeSpan perfCounterRefreshWindow = UserWorkloadManagerPerfCounterWrapper.OneMinute;

		private static ConcurrentDictionary<BudgetKey, int> uniqueDelayedBudgetKeys = new ConcurrentDictionary<BudgetKey, int>();

		private static int maxTasksQueued;

		private static int maxThreadCount;

		private static int maxDelayCacheTasks;

		private readonly int delayTimeThreshold;

		private readonly bool initialized;

		private object syncRoot = new object();

		private int maxDelay;

		private DateTime lastClearTime;

		private MSExchangeUserWorkloadManagerInstance perfCounters;
	}
}
