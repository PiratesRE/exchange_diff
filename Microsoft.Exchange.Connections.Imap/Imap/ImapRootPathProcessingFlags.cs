using System;

namespace Microsoft.Exchange.Connections.Imap
{
	[Flags]
	internal enum ImapRootPathProcessingFlags
	{
		None = 0,
		FlagsInitialized = 1,
		FlagsDetermined = 2,
		ResponseIncludesRootPathPrefix = 4,
		FolderPathPrefixIsInbox = 8,
		UnableToProcess = 16
	}
}
