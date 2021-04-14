using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Transport.Common;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class JobHealthMonitor
	{
		public int JobAvailabilityPercentage
		{
			get
			{
				return this.jobAvailabilityPercentage;
			}
		}

		public JobHealthMonitor(int totalJobs, TimeSpan jobHealthThreshold, ExPerformanceCounter perfCounter)
		{
			this.totalJobs = totalJobs;
			this.executingJobs = new Dictionary<Job, ExDateTime>();
			this.pendingJobs = new Dictionary<Job, ExDateTime>();
			this.jobHealthThreshold = jobHealthThreshold;
			this.perfCounter = perfCounter;
		}

		public void JobBeganExecution(Job job)
		{
			lock (this.executingJobs)
			{
				TransportHelpers.AttemptAddToDictionary<Job, ExDateTime>(this.executingJobs, job, ExDateTime.UtcNow, null);
			}
		}

		public void JobFinishedExecution(Job job)
		{
			lock (this.executingJobs)
			{
				this.executingJobs.Remove(job);
			}
		}

		public void JobMovedFromExecutionToPending(Job job)
		{
			lock (this.executingJobs)
			{
				ExDateTime valueToAdd = this.executingJobs[job];
				TransportHelpers.AttemptAddToDictionary<Job, ExDateTime>(this.pendingJobs, job, valueToAdd, null);
				this.executingJobs.Remove(job);
			}
		}

		public void JobMovedFromPendingToExecution(Job job)
		{
			lock (this.executingJobs)
			{
				ExDateTime valueToAdd = this.pendingJobs[job];
				TransportHelpers.AttemptAddToDictionary<Job, ExDateTime>(this.executingJobs, job, valueToAdd, null);
				this.pendingJobs.Remove(job);
			}
		}

		internal void UpdateJobUsagePerfCounter(object state)
		{
			ExDateTime t = ExDateTime.UtcNow - this.jobHealthThreshold;
			int num;
			lock (this.executingJobs)
			{
				num = this.totalJobs - this.executingJobs.Count;
				foreach (KeyValuePair<Job, ExDateTime> keyValuePair in this.executingJobs)
				{
					if (keyValuePair.Value >= t)
					{
						num++;
					}
				}
			}
			this.jobAvailabilityPercentage = (int)((double)num / (double)this.totalJobs * 100.0);
			if (this.perfCounter != null)
			{
				this.perfCounter.RawValue = (long)this.jobAvailabilityPercentage;
			}
		}

		private int totalJobs;

		private TimeSpan jobHealthThreshold;

		private int jobAvailabilityPercentage;

		private ExPerformanceCounter perfCounter;

		private Dictionary<Job, ExDateTime> executingJobs;

		private Dictionary<Job, ExDateTime> pendingJobs;
	}
}
