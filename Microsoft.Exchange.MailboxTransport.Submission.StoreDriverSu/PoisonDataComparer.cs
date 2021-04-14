using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.MailboxTransport.Submission.StoreDriverSubmission
{
	internal class PoisonDataComparer : IComparer<KeyValuePair<SubmissionPoisonContext, DateTime>>
	{
		public int Compare(KeyValuePair<SubmissionPoisonContext, DateTime> x, KeyValuePair<SubmissionPoisonContext, DateTime> y)
		{
			return DateTime.Compare(x.Value, y.Value);
		}
	}
}
