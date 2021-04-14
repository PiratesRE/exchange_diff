using System;

namespace Microsoft.Exchange.Assistants
{
	internal abstract class MailboxData
	{
		protected MailboxData(Guid mailboxGuid, Guid databaseGuid, string displayName)
		{
			if (databaseGuid == Guid.Empty)
			{
				throw new ArgumentException("databaseGuid");
			}
			this.mailboxGuid = mailboxGuid;
			this.displayName = displayName;
			this.databaseGuid = databaseGuid;
		}

		public Guid MailboxGuid
		{
			get
			{
				return this.mailboxGuid;
			}
		}

		public string DisplayName
		{
			get
			{
				return this.displayName;
			}
		}

		public Guid DatabaseGuid
		{
			get
			{
				return this.databaseGuid;
			}
		}

		public bool Equals(MailboxData other)
		{
			return other != null && this.DatabaseGuid == other.DatabaseGuid;
		}

		public override int GetHashCode()
		{
			return this.DatabaseGuid.GetHashCode();
		}

		private readonly Guid mailboxGuid;

		private readonly string displayName;

		private readonly Guid databaseGuid;
	}
}
