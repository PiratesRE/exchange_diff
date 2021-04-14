using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.WorkloadManagement;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal abstract class SystemTaskBase
	{
		protected SystemTaskBase(SystemWorkloadBase workload) : this(workload, null)
		{
		}

		protected SystemTaskBase(SystemWorkloadBase workload, ResourceReservation reservation)
		{
			if (workload == null)
			{
				throw new ArgumentNullException("workload");
			}
			this.Identity = Guid.NewGuid();
			this.Workload = workload;
			this.ResourceReservation = reservation;
			this.CreationTime = DateTime.UtcNow;
		}

		public Guid Identity { get; private set; }

		public SystemWorkloadBase Workload { get; private set; }

		public DateTime CreationTime { get; private set; }

		public TimeSpan ExecutionTime
		{
			get
			{
				return this.elapsedExecutionTime;
			}
		}

		public TimeSpan SuspendTime
		{
			get
			{
				return DateTime.UtcNow - this.CreationTime - this.ExecutionTime;
			}
		}

		public ResourceReservation ResourceReservation { get; set; }

		internal Thread Thread
		{
			get
			{
				return this.thread;
			}
		}

		public virtual void Complete()
		{
		}

		public override string ToString()
		{
			return this.Identity.ToString();
		}

		internal void InternalComplete()
		{
			this.CompleteStep();
			this.Complete();
			this.Workload.InternalCompleteTask(this);
		}

		internal void InternalYield(ActivityContextState state)
		{
			this.activityContextState = state;
			this.CompleteStep();
			this.Workload.InternalYieldTask(this);
		}

		internal ActivityContextState InternalResume()
		{
			ActivityContextState result = this.activityContextState;
			this.activityContextState = null;
			return result;
		}

		internal TaskStepResult InternalExecute()
		{
			this.thread = Thread.CurrentThread;
			return this.TimedExecution<TaskStepResult>(() => this.Execute());
		}

		protected abstract TaskStepResult Execute();

		private void CompleteStep()
		{
			this.thread = null;
			this.ResourceReservation.Dispose();
			this.ResourceReservation = null;
		}

		private T TimedExecution<T>(Func<T> method)
		{
			DateTime utcNow = DateTime.UtcNow;
			T result;
			try
			{
				result = method();
			}
			finally
			{
				TimeSpan timeSpan = DateTime.UtcNow - utcNow;
				if (timeSpan > TimeSpan.Zero)
				{
					this.elapsedExecutionTime += timeSpan;
				}
				TimeSpan timeSpan2 = TimeSpan.FromMilliseconds(timeSpan.TotalMilliseconds * this.ResourceReservation.DelayFactor);
				if (timeSpan2 > TimeSpan.Zero)
				{
					if (timeSpan2 > SystemTaskBase.MaxDelay)
					{
						ExTraceGlobals.SchedulerTracer.TraceDebug<TimeSpan, TimeSpan>((long)this.GetHashCode(), "[SystemTaskBase.TimedExecution] Calculated delay {0} exceeded maximum {1}. Using maximum instead.", timeSpan2, SystemTaskBase.MaxDelay);
						timeSpan2 = SystemTaskBase.MaxDelay;
					}
					WorkloadManagementLogger.SetThrottlingValues(timeSpan2, false, null, null);
					Thread.Sleep(timeSpan2);
				}
			}
			return result;
		}

		private static readonly TimeSpan MaxDelay = TimeSpan.FromSeconds(5.0);

		private TimeSpan elapsedExecutionTime = TimeSpan.Zero;

		private ActivityContextState activityContextState;

		private Thread thread;
	}
}
