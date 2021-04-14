using System;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	internal static class DiagnosticQueryStrings
	{
		public static string UnableToLockMailbox(int mailboxNumber)
		{
			return string.Format("Unable to lock mailbox with MailboxNumber = {0}.", mailboxNumber);
		}

		public static string MailboxStateNotFound(int mailboxNumber)
		{
			return string.Format("MailboxState is not found for mailbox with MailboxNumber = {0}.", mailboxNumber);
		}
	}
}
