using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Data.ConfigurationSettings;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal abstract class Job : DisposeTrackableBase, IJob
	{
		public Job()
		{
			this.workItemQueue = new WorkItemQueue();
			this.state = JobState.Runnable;
			this.jobDoneEvent = new ManualResetEvent(true);
			this.creationTime = DateTime.UtcNow;
		}

		public virtual bool IsInteractive
		{
			get
			{
				return false;
			}
		}

		public abstract WorkloadType WorkloadTypeFromJob { get; }

		public abstract ReservationContext Reservation { get; internal set; }

		public bool IsFinished
		{
			get
			{
				return (this.workItemQueue.IsEmpty() && !this.IsInteractive) || (CommonUtils.ServiceIsStopping && this.Reservation == null);
			}
		}

		protected TimeSpan SuspendTime
		{
			get
			{
				return DateTime.UtcNow - this.creationTime - this.executionTime;
			}
		}

		protected int TraceActivityID
		{
			get
			{
				return this.traceActivityID;
			}
			set
			{
				this.traceActivityID = value;
			}
		}

		protected virtual bool IsInFinalization
		{
			get
			{
				return false;
			}
		}

		private protected ExDateTime ScheduledRunTime
		{
			protected get
			{
				ExDateTime scheduledRunTime;
				lock (this.jobLock)
				{
					scheduledRunTime = this.workItemQueue.ScheduledRunTime;
				}
				return scheduledRunTime;
			}
			private set
			{
				lock (this.jobLock)
				{
					this.workItemQueue.ScheduledRunTime = value;
				}
			}
		}

		protected abstract IEnumerable<ResourceKey> ResourceDependencies { get; }

		bool IJob.ShouldWakeup
		{
			get
			{
				ExDateTime utcNow = ExDateTime.UtcNow;
				return this.ScheduledRunTime <= utcNow || CommonUtils.ServiceIsStopping;
			}
		}

		JobState IJob.State
		{
			get
			{
				return this.state;
			}
		}

		public abstract JobSortKey JobSortKey { get; }

		MrsSystemTask IJob.GetTask(SystemWorkloadBase systemWorkload, ResourceReservationContext context)
		{
			return this.GetTask(systemWorkload, context);
		}

		void IJob.ProcessTaskExecutionResult(MrsSystemTask systemTask)
		{
			this.ProcessTaskExecutionResult(systemTask);
		}

		void IJob.RevertToPreviousUnthrottledState()
		{
			this.RevertToPreviousUnthrottledState();
		}

		void IJob.WaitForJobToBeDone()
		{
			this.jobDoneEvent.WaitOne();
		}

		void IJob.ResetJob()
		{
			this.ResetJob();
		}

		public bool WorkItemQueueIsEmpty()
		{
			return this.workItemQueue.IsEmpty();
		}

		public virtual void RevertToPreviousUnthrottledState()
		{
		}

		public override string ToString()
		{
			return base.GetType().Name + this.workItemTrace;
		}

		public abstract void PerformCrashingFailureActions(Exception exception);

		protected override void InternalDispose(bool calledFromDispose)
		{
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<Job>(this);
		}

		protected virtual SettingsContextBase GetConfigSettingsContext()
		{
			return null;
		}

		public abstract IActivityScope GetCurrentActivityScope();

		protected abstract void StartDeferredDelayIfApplicable();

		protected abstract void ProcessSucceededTask(bool ignoreTaskSuccessfulExecutionTime);

		protected abstract void ProcessFailedTask(Exception lastFailure, out bool shouldContinueRunningJob);

		protected virtual void MoveToThrottledState(ResourceKey resource, bool deferDelay)
		{
		}

		protected void WakeupJob()
		{
			base.CheckDisposed();
			MrsTracer.Service.Function("Job({0}).WakeupJob", new object[]
			{
				base.GetType().Name
			});
			lock (this.jobLock)
			{
				if (this.state == JobState.Waiting)
				{
					this.ScheduledRunTime = ExDateTime.MinValue;
				}
			}
		}

		protected virtual bool CanBeCanceledOrSuspended()
		{
			return true;
		}

		protected void ResetWorkItemQueue()
		{
			lock (this.jobLock)
			{
				this.workItemQueue.Clear();
			}
		}

		protected void ScheduleWorkItem(Action action, WorkloadType workloadType = WorkloadType.Unknown)
		{
			this.ScheduleWorkItem(TimeSpan.Zero, action, workloadType);
		}

		protected void ScheduleWorkItem<T>(Action<T> action, T arg, WorkloadType workloadType = WorkloadType.Unknown)
		{
			this.ScheduleWorkItem<T>(TimeSpan.Zero, action, arg, workloadType);
		}

		protected void ScheduleWorkItem<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2, WorkloadType workloadType = WorkloadType.Unknown)
		{
			this.ScheduleWorkItem<T1, T2>(TimeSpan.Zero, action, arg1, arg2, workloadType);
		}

		protected void ScheduleWorkItem<T1, T2, T3>(Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3, WorkloadType workloadType = WorkloadType.Unknown)
		{
			this.ScheduleWorkItem<T1, T2, T3>(TimeSpan.Zero, action, arg1, arg2, arg3, workloadType);
		}

		protected void ScheduleWorkItem<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, WorkloadType workloadType = WorkloadType.Unknown)
		{
			this.ScheduleWorkItem<T1, T2, T3, T4>(TimeSpan.Zero, action, arg1, arg2, arg3, arg4, workloadType);
		}

		protected void ScheduleWorkItem(TimeSpan delay, Action action, WorkloadType workloadType = WorkloadType.Unknown)
		{
			this.ScheduleWorkItem(new WorkItem(delay, action, workloadType));
		}

		protected void ScheduleWorkItem<T>(TimeSpan delay, Action<T> action, T arg, WorkloadType workloadType = WorkloadType.Unknown)
		{
			this.ScheduleWorkItem(new WorkItem(delay, delegate()
			{
				action(arg);
			}, workloadType));
		}

		protected void ScheduleWorkItem<T1, T2>(TimeSpan delay, Action<T1, T2> action, T1 arg1, T2 arg2, WorkloadType workloadType = WorkloadType.Unknown)
		{
			this.ScheduleWorkItem(new WorkItem(delay, delegate()
			{
				action(arg1, arg2);
			}, workloadType));
		}

		protected void ScheduleWorkItem<T1, T2, T3>(TimeSpan delay, Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3, WorkloadType workloadType = WorkloadType.Unknown)
		{
			this.ScheduleWorkItem(new WorkItem(delay, delegate()
			{
				action(arg1, arg2, arg3);
			}, workloadType));
		}

		protected void ScheduleWorkItem<T1, T2, T3, T4>(TimeSpan delay, Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, WorkloadType workloadType = WorkloadType.Unknown)
		{
			this.ScheduleWorkItem(new WorkItem(delay, delegate()
			{
				action(arg1, arg2, arg3, arg4);
			}, workloadType));
		}

		protected void ScheduleWorkItem(WorkItem workitem)
		{
			base.CheckDisposed();
			lock (this.jobLock)
			{
				this.workItemQueue.Add(workitem);
			}
		}

		private void ResetJob()
		{
			lock (this.jobLock)
			{
				this.state = JobState.Runnable;
				this.jobDoneEvent.Reset();
			}
		}

		private MrsSystemTask GetTask(SystemWorkloadBase systemWorkload, ResourceReservationContext context)
		{
			MrsTracer.ActivityID = this.traceActivityID;
			base.CheckDisposed();
			MrsSystemTask result;
			using (SettingsContextBase.ActivateContext(this as ISettingsContextProvider))
			{
				lock (this.jobLock)
				{
					if (this.IsFinished)
					{
						MrsTracer.Service.Debug("Job({0}) is finished.", new object[]
						{
							base.GetType().Name
						});
						this.state = JobState.Finished;
						this.jobDoneEvent.Set();
						result = null;
					}
					else
					{
						WorkItem workItem = null;
						bool flag2 = true;
						foreach (WorkItem workItem2 in this.workItemQueue.GetCandidateWorkItems())
						{
							if (workItem2.ScheduledRunTime <= ExDateTime.UtcNow || CommonUtils.ServiceIsStopping)
							{
								flag2 = false;
								if (this.GetWorkloadType(workItem2.WorkloadType) == systemWorkload.WorkloadType)
								{
									workItem = workItem2;
									break;
								}
							}
						}
						if (workItem == null)
						{
							if (flag2)
							{
								this.state = JobState.Waiting;
							}
							result = null;
						}
						else
						{
							this.RevertToPreviousUnthrottledState();
							IEnumerable<ResourceKey> enumerable = this.ResourceDependencies;
							if (enumerable == null)
							{
								enumerable = Array<ResourceKey>.Empty;
							}
							ResourceKey resource = null;
							ResourceReservation reservation = this.GetReservation(workItem, systemWorkload, context, enumerable, out resource);
							if (reservation != null)
							{
								if (reservation.DelayFactor > 0.0)
								{
									this.MoveToThrottledState(resource, true);
								}
								this.TraceWorkItem(workItem);
								this.workItemQueue.Remove(workItem);
								result = new MrsSystemTask(this, workItem.Callback, systemWorkload, reservation, workItem is JobCheck);
							}
							else
							{
								this.MoveToThrottledState(resource, false);
								result = null;
							}
						}
					}
				}
			}
			return result;
		}

		private ResourceReservation GetReservation(WorkItem workItem, SystemWorkloadBase systemWorkload, ResourceReservationContext context, IEnumerable<ResourceKey> resources, out ResourceKey throttledResource)
		{
			throttledResource = null;
			ResourceReservation result;
			if (CommonUtils.ServiceIsStopping || this.IsInFinalization || workItem is UnthrottledWorkItem)
			{
				result = context.GetUnthrottledReservation(systemWorkload);
			}
			else
			{
				result = context.GetReservation(systemWorkload, resources, out throttledResource);
			}
			return result;
		}

		private void ProcessTaskExecutionResult(MrsSystemTask systemTask)
		{
			this.executionTime += systemTask.ExecutionTime;
			if (systemTask.Failure == null)
			{
				this.ProcessSucceededTask(systemTask.IgnoreTaskSuccessfulExecutionTime);
				return;
			}
			using (SettingsContextBase.ActivateContext(this as ISettingsContextProvider))
			{
				bool flag;
				this.ProcessFailedTask(systemTask.Failure, out flag);
				if (!flag)
				{
					lock (this.jobLock)
					{
						MrsTracer.Service.Error("Job({0}) failed.", new object[]
						{
							base.GetType().Name
						});
						this.state = JobState.Failed;
						this.jobDoneEvent.Set();
					}
					this.StartDeferredDelayIfApplicable();
				}
			}
		}

		private WorkloadType GetWorkloadType(WorkloadType workloadType)
		{
			if (workloadType == WorkloadType.Unknown)
			{
				return this.WorkloadTypeFromJob;
			}
			return workloadType;
		}

		private void TraceWorkItem(WorkItem workItem)
		{
			this.workItemTrace = this.workItemTrace + "=>" + workItem.Callback.Method.Name;
			if (this.workItemTrace.Length > 1024)
			{
				this.workItemTrace = "..." + this.workItemTrace.Substring(this.workItemTrace.Length - 1024 + "...".Length, 1024 - "...".Length);
			}
			MrsTracer.Service.Debug("Workitems trace: {0}", new object[]
			{
				this
			});
		}

		private const int MaxWorkItemTraceSize = 1024;

		private readonly DateTime creationTime;

		private readonly object jobLock = new object();

		private int traceActivityID;

		private WorkItemQueue workItemQueue;

		private string workItemTrace;

		private JobState state;

		private ManualResetEvent jobDoneEvent;

		private TimeSpan executionTime;
	}
}
