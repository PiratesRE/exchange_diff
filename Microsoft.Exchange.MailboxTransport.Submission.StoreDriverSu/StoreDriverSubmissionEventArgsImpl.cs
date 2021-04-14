using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.StoreDriver;

namespace Microsoft.Exchange.MailboxTransport.Submission.StoreDriverSubmission
{
	internal class StoreDriverSubmissionEventArgsImpl : StoreDriverSubmissionEventArgs
	{
		internal StoreDriverSubmissionEventArgsImpl(MailItem mailItem, SubmissionItem submissionItem, MailItemSubmitter mailItemSubmitter)
		{
			this.mailItem = mailItem;
			this.submissionItem = submissionItem;
			this.mailItemSubmitter = mailItemSubmitter;
		}

		public override MailItem MailItem
		{
			get
			{
				return this.mailItem;
			}
		}

		internal SubmissionItem SubmissionItem
		{
			get
			{
				return this.submissionItem;
			}
		}

		internal MailItemSubmitter MailItemSubmitter
		{
			get
			{
				return this.mailItemSubmitter;
			}
		}

		private MailItem mailItem;

		private SubmissionItem submissionItem;

		private MailItemSubmitter mailItemSubmitter;
	}
}
