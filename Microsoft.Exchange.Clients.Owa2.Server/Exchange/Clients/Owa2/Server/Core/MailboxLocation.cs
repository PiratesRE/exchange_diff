using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal sealed class MailboxLocation : NotificationLocation
	{
		public MailboxLocation(Guid mailboxGuid)
		{
			if (mailboxGuid == Guid.Empty)
			{
				throw new ArgumentException("Mailbox guid cannot be empty.", "mailboxGuid");
			}
			this.mailboxGuid = mailboxGuid;
		}

		public static MailboxLocation FromMailboxContext(IMailboxContext mailboxContext)
		{
			if (mailboxContext != null && mailboxContext.ExchangePrincipal != null)
			{
				return new MailboxLocation(mailboxContext.ExchangePrincipal.MailboxInfo.MailboxGuid);
			}
			return null;
		}

		public override KeyValuePair<string, object> GetEventData()
		{
			return new KeyValuePair<string, object>("MailboxGuid", this.mailboxGuid);
		}

		public override int GetHashCode()
		{
			return MailboxLocation.TypeHashCode ^ this.mailboxGuid.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			MailboxLocation mailboxLocation = obj as MailboxLocation;
			return mailboxLocation != null && this.mailboxGuid.Equals(mailboxLocation.mailboxGuid);
		}

		public override string ToString()
		{
			return this.mailboxGuid.ToString();
		}

		private const string EventKey = "MailboxGuid";

		private static readonly int TypeHashCode = typeof(MailboxLocation).GetHashCode();

		private readonly Guid mailboxGuid;
	}
}
