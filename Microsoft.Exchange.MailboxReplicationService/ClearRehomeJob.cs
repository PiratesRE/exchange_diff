using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class ClearRehomeJob : LightJobBase
	{
		public ClearRehomeJob(Guid requestGuid, Guid requestQueueGuid, MapiStore systemMailbox, byte[] messageId) : base(requestGuid, requestQueueGuid, systemMailbox, messageId)
		{
		}

		protected override RequestState RelinquishAction(TransactionalRequestJob requestJob, ReportData report)
		{
			requestJob.RehomeRequest = false;
			return RequestState.Relinquished;
		}

		protected override void InternalDispose(bool disposing)
		{
			base.InternalDispose(disposing);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ClearRehomeJob>(this);
		}
	}
}
