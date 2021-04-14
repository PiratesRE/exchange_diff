using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class QuarantineJob : LightJobBase
	{
		public QuarantineJob(Guid requestGuid, Guid requestQueueGuid, MapiStore systemMailbox, byte[] messageId) : base(requestGuid, requestQueueGuid, systemMailbox, messageId)
		{
		}

		protected override RequestState RelinquishAction(TransactionalRequestJob requestJob, ReportData report)
		{
			FailureRec failureRec = QuarantinedJobs.Get(base.RequestGuid);
			if (failureRec == null)
			{
				return RequestState.Relinquished;
			}
			report.Append(MrsStrings.JobIsQuarantined, failureRec, ReportEntryFlags.Fatal);
			requestJob.Suspend = true;
			requestJob.Status = RequestStatus.Failed;
			requestJob.TimeTracker.SetTimestamp(RequestJobTimestamp.Failure, new DateTime?(DateTime.UtcNow));
			QuarantinedJobs.Remove(base.RequestGuid);
			RequestJobLog.Write(requestJob);
			return RequestState.Failed;
		}

		protected override void InternalDispose(bool disposing)
		{
			base.InternalDispose(disposing);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<QuarantineJob>(this);
		}
	}
}
