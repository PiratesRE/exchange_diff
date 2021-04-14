using System;

namespace Microsoft.Exchange.MailboxTransport.Submission.StoreDriverSubmission
{
	internal class SubmissionPoisonContext
	{
		public SubmissionPoisonContext(Guid resourceGuid, long mapiEventCounter)
		{
			this.ResourceGuid = resourceGuid;
			this.MapiEventCounter = mapiEventCounter;
		}

		public Guid ResourceGuid { get; private set; }

		public long MapiEventCounter { get; private set; }

		public override string ToString()
		{
			return this.ResourceGuid + ":" + this.MapiEventCounter;
		}
	}
}
