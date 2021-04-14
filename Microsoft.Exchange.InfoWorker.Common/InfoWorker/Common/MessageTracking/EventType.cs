using System;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	public enum EventType
	{
		SmtpReceive,
		SmtpSend,
		Fail,
		Deliver,
		Resolve,
		Expand,
		Redirect,
		Submit,
		Defer,
		InitMessageCreated,
		ModeratorRejected,
		ModeratorApprove,
		Pending,
		Transferred
	}
}
