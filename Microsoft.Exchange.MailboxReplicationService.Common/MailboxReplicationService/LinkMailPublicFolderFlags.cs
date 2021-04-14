using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Flags]
	internal enum LinkMailPublicFolderFlags
	{
		None = 0,
		ObjectGuid = 1,
		EntryId = 2
	}
}
