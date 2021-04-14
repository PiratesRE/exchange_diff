using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Flags]
	public enum MailboxRestoreType
	{
		None = 0,
		Disabled = 1,
		SoftDeleted = 2,
		Recovery = 4,
		SoftDeletedRecipient = 8,
		PublicFolderMailbox = 16
	}
}
