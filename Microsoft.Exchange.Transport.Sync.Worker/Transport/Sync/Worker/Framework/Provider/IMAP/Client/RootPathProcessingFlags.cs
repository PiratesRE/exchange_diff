using System;

namespace Microsoft.Exchange.Transport.Sync.Worker.Framework.Provider.IMAP.Client
{
	[Flags]
	internal enum RootPathProcessingFlags
	{
		None = 0,
		FlagsInitialized = 1,
		FlagsDetermined = 2,
		ResponseIncludesRootPathPrefix = 4,
		FolderPathPrefixIsInbox = 8,
		UnableToProcess = 16
	}
}
