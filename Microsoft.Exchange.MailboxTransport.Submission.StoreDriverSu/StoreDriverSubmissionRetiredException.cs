using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MailboxTransport.Submission.StoreDriverSubmission
{
	internal class StoreDriverSubmissionRetiredException : LocalizedException
	{
		public StoreDriverSubmissionRetiredException() : base(default(LocalizedString))
		{
		}
	}
}
