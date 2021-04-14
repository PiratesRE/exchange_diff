using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[Flags]
	internal enum CreateFolderFlags
	{
		None = 0,
		OpenIfExists = 1,
		InstantSearch = 2,
		OptimizedConversationSearch = 4,
		CreatePublicFolderDumpster = 8,
		InternalAccess = 16,
		ReservedForLegacySupport = 128
	}
}
