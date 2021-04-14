using System;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.MailboxTransport.Shared.Smtp;

namespace Microsoft.Exchange.MailboxTransport.Submission.StoreDriverSubmission
{
	internal class PickupFolderOutProvider : IOutProvider
	{
		private PickupFolderOutProvider()
		{
		}

		public static PickupFolderOutProvider Instance
		{
			get
			{
				return PickupFolderOutProvider.instance;
			}
		}

		public SmtpMailItemResult SendMessage(SubmissionReadOnlyMailItem submissionReadOnlyMailItem)
		{
			PickupFolder pickupFolder = new PickupFolder();
			SmtpResponse smtpResponse;
			string text;
			pickupFolder.WriteMessage(submissionReadOnlyMailItem.MailItem, submissionReadOnlyMailItem.Recipients, out smtpResponse, out text);
			return null;
		}

		private static readonly PickupFolderOutProvider instance = new PickupFolderOutProvider();
	}
}
