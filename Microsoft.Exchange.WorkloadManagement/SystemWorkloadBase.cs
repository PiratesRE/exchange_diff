using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.WorkloadManagement;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.WorkloadManagement
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class SystemWorkloadBase
	{
		public bool Registered
		{
			get
			{
				bool result;
				lock (this.instanceLock)
				{
					result = (this.ClassificationBlock != null);
				}
				return result;
			}
		}

		public WorkloadClassification Classification
		{
			get
			{
				WorkloadClassification result;
				lock (this.instanceLock)
				{
					result = ((this.ClassificationBlock == null) ? VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).WorkloadManagement.GetObject<IWorkloadSettings>(this.WorkloadType, new object[0]).Classification : this.ClassificationBlock.WorkloadClassification);
				}
				return result;
			}
		}

		public abstract WorkloadType WorkloadType { get; }

		public abstract string Id { get; }

		public abstract int TaskCount { get; }

		public abstract int BlockedTaskCount { get; }

		public bool IsEnabled
		{
			get
			{
				VariantConfigurationSnapshot snapshot = VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null);
				IWorkloadSettings @object = snapshot.WorkloadManagement.GetObject<IWorkloadSettings>(this.WorkloadType, new object[0]);
				if (!@object.Enabled)
				{
					return false;
				}
				if (@object.EnabledDuringBlackout)
				{
					return true;
				}
				IBlackoutSettings blackout = snapshot.WorkloadManagement.Blackout;
				if (!(blackout.StartTime != blackout.EndTime))
				{
					return true;
				}
				DateTime utcNow = DateTime.UtcNow;
				DateTime t = utcNow.Date + blackout.StartTime;
				DateTime t2 = utcNow.Date + blackout.EndTime;
				if (t >= t2)
				{
					t2 = t2.AddDays(1.0);
				}
				return !(t < utcNow) || !(utcNow < t2);
			}
		}

		public bool Paused
		{
			get
			{
				bool result;
				lock (this.instanceLock)
				{
					if (this.pausedUntil != null)
					{
						TimeSpan t = this.pausedUntil.Value - DateTime.UtcNow;
						if (t < TimeSpan.Zero)
						{
							this.pausedUntil = null;
							result = false;
						}
						else
						{
							result = true;
						}
					}
					else
					{
						result = false;
					}
				}
				return result;
			}
		}

		public int MaxConcurrency
		{
			get
			{
				return VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).WorkloadManagement.GetObject<IWorkloadSettings>(this.WorkloadType, new object[0]).MaxConcurrency;
			}
		}

		internal static TimeSpan PauseDuration
		{
			get
			{
				return SystemWorkloadBase.pauseDuration;
			}
			set
			{
				SystemWorkloadBase.pauseDuration = value;
			}
		}

		internal ClassificationBlock ClassificationBlock
		{
			get
			{
				return this.classificationBlock;
			}
			set
			{
				lock (this.instanceLock)
				{
					if (this.perfCounter != null)
					{
						this.perfCounter.Remove();
					}
					this.classificationBlock = value;
					this.perfCounter = ((value == null) ? WorkloadPerfCounterWrapper.Empty : new WorkloadPerfCounterWrapper(this));
				}
				this.perfCounter.UpdateActiveState(true);
			}
		}

		internal int ActiveThreadCount
		{
			get
			{
				return this.activeThreads;
			}
		}

		public override string ToString()
		{
			return this.WorkloadType.ToString();
		}

		internal void Wake()
		{
			lock (this.instanceLock)
			{
				this.pausedUntil = null;
			}
			if (this.perfCounter != null)
			{
				this.perfCounter.UpdateActiveState(true);
			}
		}

		internal void Pause()
		{
			lock (this.instanceLock)
			{
				this.pausedUntil = new DateTime?(DateTime.UtcNow.Add(SystemWorkloadBase.PauseDuration));
			}
			if (this.perfCounter != null)
			{
				this.perfCounter.UpdateActiveState(false);
			}
		}

		internal SystemTaskBase[] GetRunningTasks()
		{
			SystemTaskBase[] result;
			lock (this.instanceLock)
			{
				if (this.runningTasks == null || this.runningTasks.Count < 1)
				{
					result = null;
				}
				else
				{
					SystemTaskBase[] array = new SystemTaskBase[this.runningTasks.Count];
					this.runningTasks.Values.CopyTo(array, 0);
					result = array;
				}
			}
			return result;
		}

		internal SystemTaskBase InternalGetTask()
		{
			if (!this.IsEnabled)
			{
				return null;
			}
			int num = 0;
			SystemTaskBase systemTaskBase = null;
			bool flag = false;
			lock (this.instanceLock)
			{
				if (this.runningTasks.Count + this.requestedTaskCount + 1 <= this.MaxConcurrency)
				{
					this.requestedTaskCount++;
					flag = true;
				}
			}
			if (!flag)
			{
				ExTraceGlobals.SchedulerTracer.TraceDebug<string>((long)this.GetHashCode(), "[SystemWorkloadBase.GetTask] Maximum limit for workload {0} is reached.", this.Id);
				return null;
			}
			try
			{
				systemTaskBase = this.GetTask(this.resourceReservationContext);
			}
			finally
			{
				if (systemTaskBase == null)
				{
					lock (this.instanceLock)
					{
						this.requestedTaskCount--;
					}
				}
			}
			if (systemTaskBase == null)
			{
				ExTraceGlobals.SchedulerTracer.TraceDebug<string>((long)this.GetHashCode(), "[SystemWorkloadBase.GetTask] Workload {0} returned null for GetTask.", this.Id);
				return null;
			}
			lock (this.instanceLock)
			{
				try
				{
					if (systemTaskBase.ResourceReservation == null || !systemTaskBase.ResourceReservation.IsActive)
					{
						throw new InvalidOperationException(string.Format("Task {0} from workload {1} has invalid resource reservation. Resources should be reserved for a task before task is returned from GetTask.", systemTaskBase, systemTaskBase.Workload));
					}
					this.runningTasks.Add(systemTaskBase.Identity, systemTaskBase);
					num = this.runningTasks.Count;
				}
				finally
				{
					this.requestedTaskCount--;
				}
			}
			if (this.perfCounter != null)
			{
				this.perfCounter.UpdateActiveTasks((long)num);
				this.perfCounter.UpdateBlockedTasks((long)this.BlockedTaskCount);
				this.perfCounter.UpdateQueuedTasks((long)this.TaskCount);
			}
			return systemTaskBase;
		}

		internal void InternalCompleteTask(SystemTaskBase task)
		{
			this.RemoveRunningTask(task);
			this.CompleteTask(task);
			this.perfCounter.UpdateTaskCompletion(task.ExecutionTime);
		}

		internal void InternalYieldTask(SystemTaskBase task)
		{
			this.RemoveRunningTask(task);
			this.YieldTask(task);
			this.perfCounter.UpdateTaskYielded();
		}

		internal void UpdateTaskStepLength(TimeSpan newStepLength)
		{
			this.perfCounter.UpdateTaskStepLength(newStepLength);
		}

		internal void IncrementActiveThreadCount()
		{
			Interlocked.Increment(ref this.activeThreads);
		}

		internal void DecrementActiveThreadCount()
		{
			Interlocked.Decrement(ref this.activeThreads);
		}

		internal void SetResourceReservationContext(ResourceReservationContext context)
		{
			if (context != null && this.resourceReservationContext != null)
			{
				throw new InvalidOperationException("Resource reservation context is already set.");
			}
			this.resourceReservationContext = context;
		}

		protected virtual void CompleteTask(SystemTaskBase task)
		{
		}

		protected virtual void YieldTask(SystemTaskBase task)
		{
		}

		protected abstract SystemTaskBase GetTask(ResourceReservationContext context);

		private void RemoveRunningTask(SystemTaskBase task)
		{
			int count;
			lock (this.instanceLock)
			{
				this.runningTasks.Remove(task.Identity);
				count = this.runningTasks.Count;
			}
			this.perfCounter.UpdateActiveTasks((long)count);
		}

		private static readonly TimeSpan DefaultPauseDuration = TimeSpan.FromSeconds(10.0);

		private static TimeSpan pauseDuration = SystemWorkloadBase.DefaultPauseDuration;

		private WorkloadPerfCounterWrapper perfCounter = WorkloadPerfCounterWrapper.Empty;

		private object instanceLock = new object();

		private Dictionary<Guid, SystemTaskBase> runningTasks = new Dictionary<Guid, SystemTaskBase>();

		private int activeThreads;

		private int requestedTaskCount;

		private DateTime? pausedUntil;

		private ClassificationBlock classificationBlock;

		private ResourceReservationContext resourceReservationContext;
	}
}
