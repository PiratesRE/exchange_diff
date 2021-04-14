using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Common;

namespace Microsoft.Exchange.Transport.Scheduler.Processing
{
	internal sealed class JobManager
	{
		public JobManager(ISchedulerMetering metering, Func<DateTime> timeProvider)
		{
			ArgumentValidator.ThrowIfNull("metering", metering);
			ArgumentValidator.ThrowIfNull("timeProvider", timeProvider);
			this.metering = metering;
			this.timeProvider = timeProvider;
		}

		public JobManager(ISchedulerMetering metering) : this(metering, () => DateTime.UtcNow)
		{
		}

		public IEnumerable<JobInfo> CurrentJobs
		{
			get
			{
				return this.currentJobs.Values;
			}
		}

		public void Start(JobInfo jobInfo)
		{
			ArgumentValidator.ThrowIfNull("jobInfo", jobInfo);
			if (this.IsShutdown())
			{
				throw new InvalidProgramException("Job Manager is already shutdown, no new job allowed to be started");
			}
			TransportHelpers.AttemptAddToDictionary<Guid, JobInfo>(this.currentJobs, jobInfo.Id, jobInfo, null);
			this.metering.RecordStart(jobInfo.Scopes, 0L);
		}

		public void End(JobInfo jobInfo)
		{
			ArgumentValidator.ThrowIfNull("jobInfo", jobInfo);
			this.currentJobs.Remove(jobInfo.Id);
			this.metering.RecordEnd(jobInfo.Scopes, this.timeProvider() - jobInfo.StartTime);
			if (this.IsShutdown() && this.currentJobs.Count == 0)
			{
				this.shutdownEventSlim.Set();
			}
		}

		internal void StartShutdown()
		{
			if (Interlocked.CompareExchange(ref this.shutdown, 1, 0) == 0 && this.currentJobs.Count == 0)
			{
				this.shutdownEventSlim.Set();
			}
		}

		internal bool WaitForShutdown(int timeoutMilliseconds = -1)
		{
			return this.shutdownEventSlim.Wait(timeoutMilliseconds);
		}

		private bool IsShutdown()
		{
			return this.shutdown == 1;
		}

		private readonly Func<DateTime> timeProvider;

		private readonly IDictionary<Guid, JobInfo> currentJobs = new Dictionary<Guid, JobInfo>();

		private readonly ISchedulerMetering metering;

		private readonly ManualResetEventSlim shutdownEventSlim = new ManualResetEventSlim(false);

		private int shutdown;
	}
}
