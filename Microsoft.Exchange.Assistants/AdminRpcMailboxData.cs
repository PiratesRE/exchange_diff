using System;
using System.Globalization;

namespace Microsoft.Exchange.Assistants
{
	internal class AdminRpcMailboxData : MailboxData
	{
		public AdminRpcMailboxData(Guid mailboxGuid, int mailboxNumber, Guid databaseGuid) : base(mailboxGuid, databaseGuid, mailboxNumber.ToString(CultureInfo.InvariantCulture))
		{
			this.mailboxNumber = mailboxNumber;
		}

		public int MailboxNumber
		{
			get
			{
				return this.mailboxNumber;
			}
		}

		public override bool Equals(object other)
		{
			if (other == null)
			{
				return false;
			}
			AdminRpcMailboxData adminRpcMailboxData = other as AdminRpcMailboxData;
			return adminRpcMailboxData != null && this.Equals(adminRpcMailboxData);
		}

		public bool Equals(AdminRpcMailboxData other)
		{
			return other != null && this.mailboxNumber == other.MailboxNumber && base.Equals(other);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ this.mailboxNumber.GetHashCode();
		}

		private readonly int mailboxNumber;
	}
}
