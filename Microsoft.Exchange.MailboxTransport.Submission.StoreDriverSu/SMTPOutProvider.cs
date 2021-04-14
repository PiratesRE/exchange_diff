using System;
using Microsoft.Exchange.MailboxTransport.Shared.Smtp;

namespace Microsoft.Exchange.MailboxTransport.Submission.StoreDriverSubmission
{
	internal class SMTPOutProvider : IOutProvider
	{
		private SMTPOutProvider()
		{
		}

		public static SMTPOutProvider Instance
		{
			get
			{
				return SMTPOutProvider.instance;
			}
		}

		public SmtpMailItemResult SendMessage(SubmissionReadOnlyMailItem submissionReadOnlyMailItem)
		{
			if (submissionReadOnlyMailItem.Recipients.Count <= 0)
			{
				throw new ArgumentException("SMTPOutProvider-SendMessage: submissionReadOnlyMailItem.Recipients.Count should be greater than 0.");
			}
			return SmtpMailItemSender.Instance.Send(submissionReadOnlyMailItem, SubmissionConfiguration.Instance.App.UseLocalHubOnly, SubmissionConfiguration.Instance.App.SmtpOutWaitTimeOut);
		}

		private const string NextHopDomain = "MailboxTransportSubmissionInternalProxy";

		private static readonly SMTPOutProvider instance = new SMTPOutProvider();
	}
}
