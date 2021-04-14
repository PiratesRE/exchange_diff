using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Flags]
	internal enum FolderHierarchyFlags
	{
		None = 0,
		PublicFolderMailbox = 1
	}
}
