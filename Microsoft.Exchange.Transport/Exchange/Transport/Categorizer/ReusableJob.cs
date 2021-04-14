using System;
using System.Collections.Generic;
using Microsoft.Exchange.Transport.MessageDepot;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class ReusableJob : Job
	{
		private ReusableJob(long id, ThrottlingContext context, QueuedRecipientsByAgeToken token, CatScheduler scheduler) : base(id, context, token, ReusableJob.GetStages(scheduler))
		{
			this.scheduler = scheduler;
		}

		public static ReusableJob NewJob(CatScheduler scheduler, ThrottlingContext context, QueuedRecipientsByAgeToken token)
		{
			long nextJobId = Job.nextJobId;
			Job.nextJobId = nextJobId + 1L;
			return new ReusableJob(nextJobId, context, token, scheduler);
		}

		public override bool TryGetDeferToken(TransportMailItem mailItem, out AcquireToken deferToken)
		{
			deferToken = null;
			return false;
		}

		public override void MarkDeferred(TransportMailItem mailItem)
		{
		}

		protected override bool IsRetired
		{
			get
			{
				return this.scheduler.Retired;
			}
		}

		protected override void CompletedInternal(TransportMailItem mailItem)
		{
			this.scheduler.RunningJobCompleted(this, mailItem);
		}

		protected override void PendingInternal()
		{
			this.scheduler.MoveRunningJobToPending(this);
		}

		protected override void GoneAsyncInternal()
		{
			this.scheduler.CheckAndScheduleJobThread();
		}

		protected override void RetireInternal(TransportMailItem mailItem)
		{
			this.scheduler.RunningJobRetired(this, mailItem);
		}

		private static IList<StageInfo> GetStages(CatScheduler scheduler)
		{
			if (scheduler != null)
			{
				return scheduler.Stages;
			}
			return null;
		}

		private readonly CatScheduler scheduler;
	}
}
