using System;

namespace Microsoft.Exchange.Rpc.MailSubmission
{
	internal enum AddResubmitRequestStatus
	{
		Success = 1,
		Retry,
		Error,
		AccessError,
		InvalidOperation,
		Disabled,
		MaxRunningRequestsExceeded,
		MaxRecentRequestsExceeded,
		DuplicateRequest
	}
}
