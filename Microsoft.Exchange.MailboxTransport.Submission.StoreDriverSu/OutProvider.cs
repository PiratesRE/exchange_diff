using System;
using Microsoft.Exchange.MailboxTransport.Shared.Smtp;

namespace Microsoft.Exchange.MailboxTransport.Submission.StoreDriverSubmission
{
	internal static class OutProvider
	{
		public static IOutProvider OutProviderInstance
		{
			get
			{
				if (OutProvider.outProvider == null)
				{
					if (SubmissionConfiguration.Instance.App.IsWriteToPickupFolderEnabled)
					{
						OutProvider.outProvider = PickupFolderOutProvider.Instance;
					}
					else
					{
						OutProvider.outProvider = SMTPOutProvider.Instance;
					}
				}
				return OutProvider.outProvider;
			}
			set
			{
				OutProvider.outProvider = value;
			}
		}

		public static SmtpMailItemResult SendMessage(SubmissionReadOnlyMailItem submissionReadOnlyMailItem)
		{
			return OutProvider.OutProviderInstance.SendMessage(submissionReadOnlyMailItem);
		}

		private static IOutProvider outProvider;
	}
}
