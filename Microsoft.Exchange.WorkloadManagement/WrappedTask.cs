using System;
using System.Diagnostics;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal class WrappedTask
	{
		public WrappedTask(ITask task)
		{
			this.hashCode = base.GetHashCode();
			this.Initialize(task);
		}

		public ITask Task { get; private set; }

		internal TaskExecuteResult PreviousStepResult { get; set; }

		internal TimeSpan TotalTime
		{
			get
			{
				return WrappedTask.GetElapsed(this.startTickCount);
			}
		}

		internal TimeSpan ExecuteTime
		{
			get
			{
				return this.executeElapsed + WrappedTask.GetElapsed(this.executeStart);
			}
		}

		internal TimeSpan QueueTime
		{
			get
			{
				return this.queueElapsed + WrappedTask.GetElapsed(this.queueStart);
			}
		}

		internal TimeSpan DelayTime
		{
			get
			{
				return this.delayElapsed + WrappedTask.GetElapsed(this.delayStart);
			}
		}

		internal int QueuedCount
		{
			get
			{
				return this.queueCount;
			}
		}

		internal int DelayedCount
		{
			get
			{
				return this.delayCount;
			}
		}

		internal int ExecuteCount
		{
			get
			{
				return this.executeCount;
			}
		}

		internal TaskLocation TaskLocation
		{
			get
			{
				if (this.delayStart != null)
				{
					return TaskLocation.DelayCache;
				}
				if (this.queueStart != null)
				{
					return TaskLocation.Queue;
				}
				return TaskLocation.Thread;
			}
		}

		internal ResourceKey[] PreviousStepResources { get; set; }

		internal TimeSpan UnaccountedForDelay { get; set; }

		public void Complete()
		{
			WorkloadManagementLogger.SetQueueTime(this.QueueTime, null);
			TimeSpan queueAndDelayTime;
			TimeSpan totalTime;
			this.GetTaskTimes(out queueAndDelayTime, out totalTime);
			this.Task.Complete(queueAndDelayTime, totalTime);
		}

		public TaskExecuteResult Execute()
		{
			TimeSpan totalTime;
			TimeSpan queueAndDelayTime;
			this.GetTaskTimes(out queueAndDelayTime, out totalTime);
			TaskExecuteResult result = TaskExecuteResult.Undefined;
			this.LocalTimedCall(delegate
			{
				result = this.Task.Execute(queueAndDelayTime, totalTime);
			});
			return result;
		}

		public void Timeout()
		{
			WorkloadManagementLogger.SetQueueTime(this.QueueTime, null);
			TimeSpan queueAndDelayTime;
			TimeSpan totalTime;
			this.GetTaskTimes(out queueAndDelayTime, out totalTime);
			this.Task.Timeout(queueAndDelayTime, totalTime);
		}

		public void Cancel()
		{
			this.Task.Cancel();
		}

		public TaskExecuteResult CancelStep(LocalizedException exception)
		{
			TaskExecuteResult result = TaskExecuteResult.Undefined;
			this.LocalTimedCall(delegate
			{
				result = this.Task.CancelStep(exception);
				if (result == TaskExecuteResult.Undefined)
				{
					throw new ArgumentException("[WrappedTask::CancelStep] ITask.CancelStep cannot return TaskExecuteResult.Undefined.");
				}
			});
			return result;
		}

		public override int GetHashCode()
		{
			return this.hashCode;
		}

		internal WLMTaskDetails GetTaskDetails()
		{
			WLMTaskDetails wlmtaskDetails = new WLMTaskDetails();
			if (this.Task != null)
			{
				lock (this.instanceLock)
				{
					if (this.Task != null)
					{
						wlmtaskDetails.Description = this.Task.Description;
						TimeSpan totalTime = this.TotalTime;
						wlmtaskDetails.TotalTime = totalTime.ToString();
						wlmtaskDetails.StartTimeUTC = DateTime.UtcNow - totalTime;
						wlmtaskDetails.ExecuteTime = this.ExecuteTime.ToString();
						wlmtaskDetails.ExecuteCount = this.ExecuteCount;
						wlmtaskDetails.QueueTime = this.QueueTime.ToString();
						wlmtaskDetails.QueueCount = this.QueuedCount;
						wlmtaskDetails.DelayTime = this.DelayTime.ToString();
						wlmtaskDetails.DelayCount = this.DelayedCount;
						wlmtaskDetails.Location = this.TaskLocation.ToString();
						wlmtaskDetails.BudgetOwner = this.Task.Budget.Owner.ToString();
					}
				}
			}
			return wlmtaskDetails;
		}

		internal void Initialize(ITask task)
		{
			lock (this.instanceLock)
			{
				this.Task = task;
				this.taskTimeout = (task as ITaskTimeout);
				this.startTickCount = new long?(Stopwatch.GetTimestamp());
				this.PreviousStepResult = TaskExecuteResult.Undefined;
				this.UnaccountedForDelay = TimeSpan.Zero;
				this.executeStart = null;
				this.queueStart = null;
				this.delayStart = null;
				this.executeElapsed = TimeSpan.Zero;
				this.queueElapsed = TimeSpan.Zero;
				this.delayElapsed = TimeSpan.Zero;
				this.queueCount = 0;
				this.delayCount = 0;
				this.executeCount = 0;
			}
		}

		internal void StartQueue()
		{
			this.queueStart = new long?(Stopwatch.GetTimestamp());
			this.queueCount++;
		}

		internal void EndQueue()
		{
			this.queueElapsed += WrappedTask.GetElapsed(this.queueStart);
			this.queueStart = null;
		}

		internal void StartDelay()
		{
			this.delayStart = new long?(Stopwatch.GetTimestamp());
			this.delayCount++;
		}

		internal void EndDelay()
		{
			this.delayElapsed += WrappedTask.GetElapsed(this.delayStart);
			this.delayStart = null;
		}

		internal void StartExecute()
		{
			this.executeStart = new long?(Stopwatch.GetTimestamp());
			this.executeCount++;
		}

		internal void EndExecute()
		{
			this.executeElapsed += WrappedTask.GetElapsed(this.executeStart);
			this.executeStart = null;
		}

		private static long ConvertToDateTimeTicks(long stopwatchTicks)
		{
			return (long)((double)stopwatchTicks * WrappedTask.dateTimeToStopwatchTicksRatio);
		}

		private static TimeSpan GetElapsed(long? start)
		{
			if (start != null)
			{
				try
				{
					long value = WrappedTask.ConvertToDateTimeTicks(Stopwatch.GetTimestamp() - start.Value);
					return TimeSpan.FromTicks(value);
				}
				catch (InvalidOperationException)
				{
					return TimeSpan.Zero;
				}
			}
			return TimeSpan.Zero;
		}

		private TimeSpan GetActionTimeout(CostType costType)
		{
			if (this.taskTimeout != null)
			{
				return this.taskTimeout.GetActionTimeout(costType);
			}
			return Budget.GetMaxActionTime(costType);
		}

		private void GetTaskTimes(out TimeSpan queueAndDelayTime, out TimeSpan totalTime)
		{
			totalTime = this.TotalTime;
			queueAndDelayTime = this.QueueTime + this.DelayTime;
		}

		private void LocalTimedCall(Action action)
		{
			IActivityScope activityScope = null;
			IActivityScope activityScope2 = null;
			IBudget budget = this.Task.Budget;
			bool flag = budget != null && budget.LocalCostHandle == null;
			if (flag)
			{
				this.StartExecute();
				budget.StartLocal(this.Task.Description, default(TimeSpan));
				budget.LocalCostHandle.MaxLiveTime = this.GetActionTimeout(CostType.CAS);
			}
			try
			{
				action();
			}
			finally
			{
				if (flag)
				{
					try
					{
						activityScope = ActivityContext.GetCurrentActivityScope();
						activityScope2 = this.Task.GetActivityScope();
						if (activityScope2 != null)
						{
							ActivityContext.SetThreadScope(activityScope2);
						}
						budget.EndLocal();
					}
					finally
					{
						if (activityScope != null && activityScope != activityScope2)
						{
							ActivityContext.SetThreadScope(activityScope);
						}
						else if (activityScope2 != null)
						{
							ActivityContext.ClearThreadScope();
						}
					}
					this.EndExecute();
				}
			}
		}

		private static readonly double dateTimeToStopwatchTicksRatio = 10000000.0 / (double)Stopwatch.Frequency;

		private readonly int hashCode;

		private TimeSpan executeElapsed = TimeSpan.Zero;

		private long? startTickCount;

		private ITaskTimeout taskTimeout;

		private object instanceLock = new object();

		private long? executeStart = null;

		private long? queueStart = null;

		private long? delayStart = null;

		private TimeSpan queueElapsed = TimeSpan.Zero;

		private TimeSpan delayElapsed = TimeSpan.Zero;

		private int queueCount;

		private int delayCount;

		private int executeCount;
	}
}
