using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Flags]
	public enum FolderState
	{
		None = 0,
		Created = 1,
		SearchFolderCopied = 2,
		CatchupFolderComplete = 4,
		CopyMessagesComplete = 8,
		IsGhosted = 16,
		PropertiesNotCopied = 32
	}
}
