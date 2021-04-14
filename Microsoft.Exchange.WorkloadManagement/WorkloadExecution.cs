using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.WorkloadManagement;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal class WorkloadExecution : DisposeTrackableBase
	{
		public WorkloadExecution(ITaskProviderManager taskProviderManager) : this(taskProviderManager, true)
		{
		}

		public WorkloadExecution(ITaskProviderManager taskProviderManager, bool useTimer)
		{
			if (taskProviderManager == null)
			{
				throw new ArgumentNullException("taskProviderManager");
			}
			this.taskProviderManager = taskProviderManager;
			if (useTimer && this.RefreshCycle > TimeSpan.Zero)
			{
				using (ActivityContext.SuppressThreadScope())
				{
					this.threadSchedulerTimer = new Timer(new TimerCallback(this.PeriodicScheduler), null, this.RefreshCycle, this.RefreshCycle);
				}
			}
		}

		public TimeSpan RefreshCycle
		{
			get
			{
				return VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).WorkloadManagement.SystemWorkloadManager.RefreshCycle;
			}
		}

		public int MaxQueuedTaskCount
		{
			get
			{
				return VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).WorkloadManagement.SystemWorkloadManager.MaxConcurrency;
			}
		}

		public WorkloadExecutionStatus Status
		{
			get
			{
				if (!this.shouldStop)
				{
					return WorkloadExecutionStatus.Started;
				}
				if (this.queuedTaskCount != 0)
				{
					return WorkloadExecutionStatus.Stopping;
				}
				return WorkloadExecutionStatus.Stopped;
			}
		}

		public int QueuedTaskCount
		{
			get
			{
				return this.queuedTaskCount;
			}
		}

		public int ActiveTaskCount
		{
			get
			{
				return this.activeTaskCount;
			}
		}

		public void Stop()
		{
			this.shouldStop = true;
		}

		public void Start()
		{
			this.shouldStop = false;
		}

		internal void QueueTaskExecution()
		{
			for (int i = 0; i < this.taskProviderManager.GetProviderCount(); i++)
			{
				if (!this.shouldStop)
				{
					WorkloadExecution.TaskData nextTaskData = this.GetNextTaskData();
					if (nextTaskData != null)
					{
						ExTraceGlobals.ExecutionTracer.TraceDebug<Guid>((long)this.GetHashCode(), "Queueing task {0} for execution.", nextTaskData.Task.Identity);
						using (ActivityContext.SuppressThreadScope())
						{
							ThreadPool.QueueUserWorkItem(new WaitCallback(this.ExecuteTaskCallback), nextTaskData);
							break;
						}
					}
				}
			}
		}

		protected override void InternalDispose(bool isDisposing)
		{
			if (isDisposing)
			{
				this.Stop();
				if (this.threadSchedulerTimer != null)
				{
					this.threadSchedulerTimer.Dispose();
					this.threadSchedulerTimer = null;
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<WorkloadExecution>(this);
		}

		protected virtual void ExecuteTask(SystemTaskBase task)
		{
			using (ActivityScope activityScope = WorkloadExecution.GetActivityScope(task))
			{
				DateTime utcNow = DateTime.UtcNow;
				TaskStepResult taskStepResult = task.InternalExecute();
				task.Workload.UpdateTaskStepLength(DateTime.UtcNow - utcNow);
				switch (taskStepResult)
				{
				case TaskStepResult.Complete:
					WorkloadExecution.CompleteTask(task, activityScope);
					break;
				case TaskStepResult.Yield:
					WorkloadExecution.YieldTask(task, activityScope);
					break;
				}
			}
		}

		private static ActivityScope GetActivityScope(SystemTaskBase task)
		{
			ActivityContextState activityContextState = task.InternalResume();
			ActivityScope activityScope;
			if (activityContextState != null)
			{
				activityScope = ActivityContext.Resume(activityContextState, task);
			}
			else
			{
				activityScope = ActivityContext.Start(task);
				activityScope.Component = task.Workload.WorkloadType.ToString();
				activityScope.ComponentInstance = task.Workload.Id;
				WorkloadManagementLogger.SetWorkloadMetadataValues(task.Workload.WorkloadType.ToString(), task.Workload.Classification.ToString(), false, false, activityScope);
			}
			return activityScope;
		}

		private static void CompleteTask(SystemTaskBase task, ActivityScope activityScope)
		{
			activityScope.End();
			ActivityContext.ClearThreadScope();
			task.InternalComplete();
		}

		private static void YieldTask(SystemTaskBase task, ActivityScope activityScope)
		{
			ActivityContextState state = activityScope.Suspend();
			ActivityContext.ClearThreadScope();
			task.InternalYield(state);
		}

		private WorkloadExecution.TaskData GetNextTaskData()
		{
			int num = Interlocked.Increment(ref this.queuedTaskCount);
			ITaskProvider taskProvider = null;
			WorkloadExecution.TaskData taskData = null;
			try
			{
				if (num <= this.MaxQueuedTaskCount)
				{
					taskProvider = this.taskProviderManager.GetNextProvider();
					if (taskProvider != null)
					{
						SystemTaskBase nextTask = taskProvider.GetNextTask();
						if (nextTask != null)
						{
							taskData = new WorkloadExecution.TaskData(taskProvider, nextTask);
						}
					}
				}
			}
			finally
			{
				if (taskData == null)
				{
					Interlocked.Decrement(ref this.queuedTaskCount);
					if (taskProvider != null)
					{
						taskProvider.Dispose();
					}
				}
			}
			return taskData;
		}

		private void ExecuteTaskCallback(object state)
		{
			Interlocked.Increment(ref this.activeTaskCount);
			WorkloadExecution.TaskData taskData = (WorkloadExecution.TaskData)state;
			try
			{
				taskData.Task.Workload.IncrementActiveThreadCount();
				this.ExecuteTask(taskData.Task);
			}
			finally
			{
				taskData.Task.Workload.DecrementActiveThreadCount();
				taskData.Dispose();
				Interlocked.Decrement(ref this.activeTaskCount);
				Interlocked.Decrement(ref this.queuedTaskCount);
			}
			this.QueueTaskExecution();
		}

		private void PeriodicScheduler(object o)
		{
			ExTraceGlobals.ExecutionTracer.TraceDebug((long)this.GetHashCode(), "Periodic scheduler is queueing a new task for execution");
			this.QueueTaskExecution();
		}

		private ITaskProviderManager taskProviderManager;

		private Timer threadSchedulerTimer;

		private int queuedTaskCount;

		private int activeTaskCount;

		private volatile bool shouldStop = true;

		private class TaskData : DisposeTrackableBase
		{
			public TaskData(ITaskProvider taskProvider, SystemTaskBase task)
			{
				this.TaskProvider = taskProvider;
				this.Task = task;
			}

			public ITaskProvider TaskProvider { get; private set; }

			public SystemTaskBase Task { get; private set; }

			protected override void InternalDispose(bool disposing)
			{
				if (disposing)
				{
					this.TaskProvider.Dispose();
				}
			}

			protected override DisposeTracker InternalGetDisposeTracker()
			{
				return DisposeTracker.Get<WorkloadExecution.TaskData>(this);
			}
		}
	}
}
