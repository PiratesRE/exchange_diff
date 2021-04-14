using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum CreateMode
	{
		CreateNew = 0,
		OpenIfExists = 1,
		InstantSearch = 2,
		OptimizedConversationSearch = 4,
		OverrideFolderCreationBlock = 8,
		CreatePublicFolderDumpster = 16
	}
}
