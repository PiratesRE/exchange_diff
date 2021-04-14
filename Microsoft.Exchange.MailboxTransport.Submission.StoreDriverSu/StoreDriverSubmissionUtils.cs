using System;

namespace Microsoft.Exchange.MailboxTransport.Submission.StoreDriverSubmission
{
	internal static class StoreDriverSubmissionUtils
	{
		public static uint MapSubmissionStatusErrorCodeToPoisonErrorCode(uint errorCode)
		{
			if (errorCode == 1U)
			{
				return 22U;
			}
			if (errorCode == 5U)
			{
				return 23U;
			}
			return 24U;
		}
	}
}
