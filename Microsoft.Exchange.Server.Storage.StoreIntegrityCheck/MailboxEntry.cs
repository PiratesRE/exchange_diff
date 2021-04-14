using System;

namespace Microsoft.Exchange.Server.Storage.StoreIntegrityCheck
{
	public sealed class MailboxEntry
	{
		public MailboxEntry(int mailboxNumber, int mailboxPartitionNumber, Guid mailboxGuid, string mailboxOwnerName)
		{
			this.mailboxNumber = mailboxNumber;
			this.mailboxPartitionNumber = mailboxPartitionNumber;
			this.mailboxGuid = mailboxGuid;
			this.mailboxOwnerName = mailboxOwnerName;
		}

		public int MailboxNumber
		{
			get
			{
				return this.mailboxNumber;
			}
		}

		public int MailboxPartitionNumber
		{
			get
			{
				return this.mailboxPartitionNumber;
			}
		}

		public Guid MailboxGuid
		{
			get
			{
				return this.mailboxGuid;
			}
		}

		public string MailboxOwnerName
		{
			get
			{
				return this.mailboxOwnerName;
			}
		}

		private readonly int mailboxNumber;

		private readonly int mailboxPartitionNumber;

		private readonly Guid mailboxGuid;

		private readonly string mailboxOwnerName;
	}
}
