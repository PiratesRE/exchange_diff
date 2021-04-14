using System;
using Microsoft.Exchange.Diagnostics.Components.Transport;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class CatSchedulerJobList : JobList
	{
		public CatSchedulerJobList(CatScheduler scheduler, JobHealthMonitor monitor) : base(monitor)
		{
			this.scheduler = scheduler;
		}

		public Job GetNextJobToRun()
		{
			lock (base.SyncRoot)
			{
				if (this.executingJobs.Count >= CatScheduler.MaxExecutingJobs)
				{
					ExTraceGlobals.SchedulerTracer.TraceDebug(0L, "GetNextJobToRun: max executing jobs count reached");
					return null;
				}
				if (this.retired)
				{
					ExTraceGlobals.SchedulerTracer.TraceDebug(0L, "Scheduler is retired -  don't add new jobs");
					return null;
				}
				if (this.pendingJobs.Count > 0)
				{
					Job job = this.pendingJobs.Dequeue();
					this.executingJobs.Add(job);
					this.monitor.JobMovedFromPendingToExecution(job);
					ExTraceGlobals.SchedulerTracer.TraceDebug<Job>(0L, "Found pending job {0}", job);
					return job;
				}
			}
			return base.RegisterNewJob(() => this.scheduler.CreateNewJob());
		}

		private CatScheduler scheduler;
	}
}
