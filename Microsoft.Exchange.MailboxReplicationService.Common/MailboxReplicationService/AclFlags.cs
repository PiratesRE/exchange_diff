using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Flags]
	internal enum AclFlags
	{
		None = 0,
		FreeBusyAcl = 1,
		FolderAcl = 2
	}
}
