using System;

namespace Microsoft.Exchange.Rpc.MultiMailboxSearch
{
	[Serializable]
	internal sealed class MultiMailboxSearchMailboxInfo : MultiMailboxSearchBase
	{
		internal MultiMailboxSearchMailboxInfo(int version, Guid mailboxGuid, byte[] folderRestriction) : base(version)
		{
			this.mailboxGuid = mailboxGuid;
			this.folderRestriction = folderRestriction;
		}

		internal MultiMailboxSearchMailboxInfo(Guid mailboxGuid, byte[] folderRestriction) : base(MultiMailboxSearchBase.CurrentVersion)
		{
			this.mailboxGuid = mailboxGuid;
			this.folderRestriction = folderRestriction;
		}

		internal MultiMailboxSearchMailboxInfo(int version, Guid mailboxGuid) : base(version)
		{
			this.mailboxGuid = mailboxGuid;
			this.folderRestriction = null;
		}

		internal MultiMailboxSearchMailboxInfo(Guid mailboxGuid) : base(MultiMailboxSearchBase.CurrentVersion)
		{
			this.mailboxGuid = mailboxGuid;
			this.folderRestriction = null;
		}

		internal Guid MailboxGuid
		{
			get
			{
				return this.mailboxGuid;
			}
			set
			{
				this.mailboxGuid = value;
			}
		}

		internal byte[] FolderRestriction
		{
			get
			{
				return this.folderRestriction;
			}
			set
			{
				this.folderRestriction = value;
			}
		}

		private Guid mailboxGuid;

		private byte[] folderRestriction;
	}
}
