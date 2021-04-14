using System;
using Microsoft.Exchange.MailboxTransport.Shared.Smtp;

namespace Microsoft.Exchange.MailboxTransport.Submission.StoreDriverSubmission
{
	internal interface IOutProvider
	{
		SmtpMailItemResult SendMessage(SubmissionReadOnlyMailItem submissionReadOnlyMailItem);
	}
}
