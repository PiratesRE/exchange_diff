using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class PublicFolderRecipient
	{
		public PublicFolderRecipient(string mailboxName, Guid mailboxGuid, ADObjectId database, SmtpAddress primarySmtpAddress, ADObjectId objectId, bool isLocalRecipient)
		{
			this.MailboxName = mailboxName;
			this.MailboxGuid = mailboxGuid;
			this.PrimarySmtpAddress = primarySmtpAddress;
			this.Database = database;
			this.ObjectId = objectId;
			this.IsLocal = isLocalRecipient;
		}

		public string MailboxName { get; private set; }

		public Guid MailboxGuid { get; private set; }

		public ADObjectId Database { get; private set; }

		public SmtpAddress PrimarySmtpAddress { get; private set; }

		public ADObjectId ObjectId { get; private set; }

		public bool IsLocal { get; private set; }

		public override int GetHashCode()
		{
			return this.ObjectId.ObjectGuid.GetHashCode();
		}

		public long ItemSize
		{
			get
			{
				long num = 0L;
				if (this.Database != null)
				{
					num += (long)this.Database.GetBytes().Length;
				}
				num += (long)(string.IsNullOrEmpty(this.MailboxName) ? 0 : this.MailboxName.Length);
				num += 16L;
				SmtpAddress primarySmtpAddress = this.PrimarySmtpAddress;
				num += (long)this.PrimarySmtpAddress.Length;
				if (this.ObjectId != null)
				{
					num += (long)this.ObjectId.GetBytes().Length;
				}
				return num + 1L;
			}
		}
	}
}
