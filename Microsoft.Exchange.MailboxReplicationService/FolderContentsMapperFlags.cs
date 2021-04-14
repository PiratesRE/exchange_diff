using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Flags]
	internal enum FolderContentsMapperFlags
	{
		None = 0,
		ImapSync = 1
	}
}
