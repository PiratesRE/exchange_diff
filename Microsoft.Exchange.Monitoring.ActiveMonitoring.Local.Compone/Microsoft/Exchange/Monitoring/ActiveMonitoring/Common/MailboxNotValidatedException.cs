using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	public class MailboxNotValidatedException : Exception
	{
		public string Password { get; private set; }

		public MailboxNotValidatedException(string password) : base("You tried to use an unverified monitoring mailbox without writing your code to handle this situation correctly. Please change your code to use the list of mailboxes with verified credentials provided by the MailboxDatabaseEndpoint.")
		{
			this.Password = password;
		}
	}
}
