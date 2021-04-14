using System;

namespace Microsoft.Exchange.MailboxTransport.Submission.StoreDriverSubmission
{
	internal class QuarantineInfoContext
	{
		public QuarantineInfoContext(DateTime quarantineStartTime)
		{
			this.QuarantineStartTime = quarantineStartTime;
		}

		public DateTime QuarantineStartTime { get; private set; }

		public const string QuarantineStartName = "QuarantineStart";
	}
}
