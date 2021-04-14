using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class ResumeJob : LightJobBase
	{
		public ResumeJob(Guid requestGuid, Guid requestQueueGuid, MapiStore systemMailbox, byte[] messageId) : base(requestGuid, requestQueueGuid, systemMailbox, messageId)
		{
		}

		protected override RequestState RelinquishAction(TransactionalRequestJob requestJob, ReportData report)
		{
			requestJob.TimeTracker.SetTimestamp(RequestJobTimestamp.Failure, null);
			requestJob.TimeTracker.SetTimestamp(RequestJobTimestamp.Suspended, null);
			requestJob.FailureCode = null;
			requestJob.FailureType = null;
			requestJob.FailureSide = null;
			requestJob.Message = LocalizedString.Empty;
			if (requestJob.SyncStage <= SyncStage.None)
			{
				requestJob.Status = RequestStatus.Queued;
				requestJob.TimeTracker.CurrentState = RequestState.Queued;
			}
			else
			{
				requestJob.Status = RequestStatus.InProgress;
				requestJob.TimeTracker.CurrentState = RequestState.InitialSeeding;
			}
			report.Append(MrsStrings.ReportJobResumed(requestJob.Status.ToString()));
			return RequestState.Relinquished;
		}

		protected override void AfterRelinquishAction()
		{
			MRSService.Tickle(base.RequestGuid, this.RequestQueueGuid, MoveRequestNotification.Created);
		}

		protected override void InternalDispose(bool disposing)
		{
			base.InternalDispose(disposing);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ResumeJob>(this);
		}
	}
}
