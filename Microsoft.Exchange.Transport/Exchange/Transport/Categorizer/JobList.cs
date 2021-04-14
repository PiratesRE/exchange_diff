using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.Transport;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class JobList
	{
		public JobList(JobHealthMonitor monitor)
		{
			this.executingJobs = new List<Job>();
			this.pendingJobs = new Queue<Job>();
			this.monitor = monitor;
		}

		public int ExecutingJobCount
		{
			get
			{
				return this.executingJobs.Count;
			}
		}

		public int PendingJobCount
		{
			get
			{
				return this.pendingJobs.Count;
			}
		}

		protected object SyncRoot
		{
			get
			{
				return this.syncRoot;
			}
		}

		public void MoveRunningJobToPending(Job job)
		{
			ExTraceGlobals.SchedulerTracer.TraceDebug<Job>(0L, "Move running Job({0}) to pending", job);
			lock (this.SyncRoot)
			{
				this.executingJobs.Remove(job);
				this.pendingJobs.Enqueue(job);
				this.monitor.JobMovedFromExecutionToPending(job);
			}
		}

		public bool RemoveExecutingJob(Job job)
		{
			bool result;
			lock (this.SyncRoot)
			{
				bool flag2 = this.executingJobs.Remove(job);
				this.monitor.JobFinishedExecution(job);
				result = flag2;
			}
			return result;
		}

		public void Retire()
		{
			this.retired = true;
		}

		public Job[] ToArray()
		{
			Job[] result;
			lock (this.SyncRoot)
			{
				Job[] array = new Job[this.executingJobs.Count + this.pendingJobs.Count];
				this.executingJobs.CopyTo(array);
				this.pendingJobs.CopyTo(array, this.executingJobs.Count);
				result = array;
			}
			return result;
		}

		public void Stop()
		{
			this.Retire();
			while (this.ExecutingJobCount > 0)
			{
				Thread.Sleep(100);
			}
			using (Queue<Job>.Enumerator enumerator = this.pendingJobs.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Job job = enumerator.Current;
					job.Retire();
				}
				goto IL_54;
			}
			IL_4D:
			Thread.Sleep(100);
			IL_54:
			if (this.pendingJobCreation <= 0)
			{
				return;
			}
			goto IL_4D;
		}

		protected Job RegisterNewJob(Func<Job> newJobProviderFunc)
		{
			Job result;
			try
			{
				Interlocked.Increment(ref this.pendingJobCreation);
				if (this.retired)
				{
					result = null;
				}
				else
				{
					Job job = newJobProviderFunc();
					if (job == null)
					{
						result = null;
					}
					else
					{
						lock (this.SyncRoot)
						{
							if (this.retired)
							{
								ExTraceGlobals.SchedulerTracer.TraceDebug(0L, "Scheduler is retired - Dispose new Job");
								job.Retire();
								return null;
							}
							this.executingJobs.Add(job);
							this.monitor.JobBeganExecution(job);
						}
						result = job;
					}
				}
			}
			finally
			{
				Interlocked.Decrement(ref this.pendingJobCreation);
			}
			return result;
		}

		protected List<Job> executingJobs;

		protected Queue<Job> pendingJobs;

		protected volatile bool retired;

		protected volatile int pendingJobCreation;

		private object syncRoot = new object();

		protected JobHealthMonitor monitor;
	}
}
