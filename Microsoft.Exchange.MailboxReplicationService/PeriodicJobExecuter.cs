using System;
using System.Threading;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class PeriodicJobExecuter
	{
		internal PeriodicJobExecuter(string jobName, PeriodicJobExecuter.JobPollerCallback callback, double variationInPeriod)
		{
			this.jobPollerCallback = callback;
			this.jobName = jobName;
			this.variation = variationInPeriod;
			this.jobDoneEvent = new ManualResetEvent(false);
		}

		internal void Start()
		{
			this.ScheduleWorkItem();
		}

		private bool HandleFailure(Exception failure)
		{
			if (failure == null)
			{
				return true;
			}
			if (failure is ServiceIsStoppingPermanentException)
			{
				return false;
			}
			if (CommonUtils.ClassifyException(failure).Length > 0)
			{
				return true;
			}
			MRSService.LogEvent(MRSEventLogConstants.Tuple_PeriodicTaskStoppingExecution, new object[]
			{
				this.jobName,
				CommonUtils.FullExceptionMessage(failure)
			});
			return false;
		}

		public void ScheduleWorkItem()
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback(this.DoJobAndScheduleNextJob));
		}

		public void WaitForJobToBeDone()
		{
			if (!this.jobDoneEvent.WaitOne(TimeSpan.FromSeconds(this.period.TotalSeconds * 3.0), true))
			{
				MrsTracer.Service.Error("{0} job did not finish within the alloted time period of 3 x {1}.", new object[]
				{
					this.jobName,
					this.period
				});
			}
		}

		public void DoJobAndScheduleNextJob(object o)
		{
			Exception failure = null;
			CommonUtils.CatchKnownExceptions(delegate
			{
				CommonUtils.CheckForServiceStopping();
				this.period = this.jobPollerCallback();
			}, delegate(Exception e)
			{
				failure = e;
			});
			if (!this.HandleFailure(failure))
			{
				this.jobDoneEvent.Set();
				return;
			}
			int num = (int)CommonUtils.Randomize(this.period.TotalSeconds, this.variation);
			Thread.Sleep(TimeSpan.FromSeconds((double)num));
			this.ScheduleWorkItem();
		}

		private readonly string jobName;

		private readonly double variation;

		private TimeSpan period = TimeSpan.FromMinutes(1.0);

		private PeriodicJobExecuter.JobPollerCallback jobPollerCallback;

		private ManualResetEvent jobDoneEvent;

		internal delegate TimeSpan JobPollerCallback();
	}
}
