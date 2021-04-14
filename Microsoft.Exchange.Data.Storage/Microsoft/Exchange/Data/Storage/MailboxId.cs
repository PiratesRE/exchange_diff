using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MailboxId
	{
		public MailboxId(string smtpAddress) : this(smtpAddress, false)
		{
		}

		public MailboxId(string smtpAddress, bool isArchive)
		{
			this.smtpAddress = smtpAddress;
			this.mailboxGuid = null;
			this.isVersionDependent = false;
			this.isArchive = isArchive;
		}

		public MailboxId(Guid mailboxGuid) : this(mailboxGuid, false)
		{
		}

		public MailboxId(Guid mailboxGuid, bool isArchive)
		{
			if (Guid.Empty == mailboxGuid)
			{
				throw new NonExistentMailboxGuidException(mailboxGuid);
			}
			this.smtpAddress = null;
			this.mailboxGuid = mailboxGuid.ToString().ToLowerInvariant();
			this.isVersionDependent = false;
			this.isArchive = isArchive;
		}

		public MailboxId(MailboxSession mailboxSession)
		{
			this.smtpAddress = mailboxSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString();
			this.mailboxGuid = mailboxSession.MailboxOwner.MailboxInfo.MailboxGuid.ToString().ToLowerInvariant();
			this.isArchive = mailboxSession.MailboxOwner.MailboxInfo.IsArchive;
			this.isVersionDependent = true;
		}

		public string SmtpAddress
		{
			get
			{
				return this.smtpAddress;
			}
		}

		public string MailboxGuid
		{
			get
			{
				return this.mailboxGuid;
			}
		}

		public bool IsVersionDependent
		{
			get
			{
				return this.isVersionDependent;
			}
		}

		public bool IsArchive
		{
			get
			{
				return this.isArchive;
			}
		}

		private string smtpAddress;

		private string mailboxGuid;

		private bool isVersionDependent;

		private bool isArchive;
	}
}
