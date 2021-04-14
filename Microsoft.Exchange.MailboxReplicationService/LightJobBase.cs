using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal abstract class LightJobBase : DisposeTrackableBase
	{
		public LightJobBase(Guid requestGuid, Guid requestQueueGuid, MapiStore systemMailbox, byte[] messageId)
		{
			this.RequestGuid = requestGuid;
			this.RequestQueueGuid = requestQueueGuid;
			this.SystemMailbox = systemMailbox;
			this.MessageId = messageId;
		}

		protected Guid RequestGuid { get; set; }

		protected virtual Guid RequestQueueGuid { get; set; }

		protected virtual MapiStore SystemMailbox { get; set; }

		protected virtual byte[] MessageId { get; set; }

		public virtual void Run()
		{
			this.RelinquishRequest();
		}

		protected abstract RequestState RelinquishAction(TransactionalRequestJob requestJob, ReportData report);

		protected virtual void AfterRelinquishAction()
		{
		}

		protected override void InternalDispose(bool disposing)
		{
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<LightJobBase>(this);
		}

		protected void RelinquishRequest()
		{
			using (RequestJobProvider rjProvider = new RequestJobProvider(this.RequestQueueGuid, this.SystemMailbox))
			{
				MapiUtils.RetryOnObjectChanged(delegate
				{
					using (TransactionalRequestJob transactionalRequestJob = (TransactionalRequestJob)rjProvider.Read<TransactionalRequestJob>(new RequestJobObjectId(this.RequestGuid, this.RequestQueueGuid, this.MessageId)))
					{
						if (transactionalRequestJob != null)
						{
							ReportData report = new ReportData(transactionalRequestJob.IdentifyingGuid, transactionalRequestJob.ReportVersion);
							transactionalRequestJob.RequestJobState = JobProcessingState.Ready;
							transactionalRequestJob.MRSServerName = null;
							transactionalRequestJob.TimeTracker.CurrentState = this.RelinquishAction(transactionalRequestJob, report);
							report.Append(MrsStrings.ReportRelinquishingJob);
							rjProvider.Save(transactionalRequestJob);
							CommonUtils.CatchKnownExceptions(delegate
							{
								report.Flush(rjProvider.SystemMailbox);
							}, null);
							transactionalRequestJob.UpdateAsyncNotification(report);
							this.AfterRelinquishAction();
						}
					}
				});
			}
		}
	}
}
