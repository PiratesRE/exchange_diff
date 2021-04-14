using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Flags]
	public enum GetFolderMapFlags
	{
		None = 0,
		ForceRefresh = 1
	}
}
