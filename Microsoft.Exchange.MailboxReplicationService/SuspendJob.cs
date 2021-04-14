using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class SuspendJob : LightJobBase
	{
		public SuspendJob(Guid requestGuid, Guid requestQueueGuid, MapiStore systemMailbox, byte[] messageId) : base(requestGuid, requestQueueGuid, systemMailbox, messageId)
		{
		}

		protected override RequestState RelinquishAction(TransactionalRequestJob requestJob, ReportData report)
		{
			requestJob.Status = RequestStatus.Suspended;
			requestJob.TimeTracker.SetTimestamp(RequestJobTimestamp.Suspended, new DateTime?(DateTime.UtcNow));
			report.Append(MrsStrings.ReportSuspendingJob);
			return RequestState.Suspended;
		}

		protected override void InternalDispose(bool disposing)
		{
			base.InternalDispose(disposing);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<SuspendJob>(this);
		}
	}
}
