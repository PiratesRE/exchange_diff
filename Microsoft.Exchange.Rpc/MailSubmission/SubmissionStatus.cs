using System;

namespace Microsoft.Exchange.Rpc.MailSubmission
{
	internal enum SubmissionStatus
	{
		Success = 1,
		Retry,
		Error
	}
}
