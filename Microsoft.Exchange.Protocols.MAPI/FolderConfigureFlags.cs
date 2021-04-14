using System;

namespace Microsoft.Exchange.Protocols.MAPI
{
	[Flags]
	public enum FolderConfigureFlags
	{
		None = 0,
		CreateSearchFolder = 1,
		CreateIpmFolder = 2,
		CreateLastWriterWinsFolder = 4,
		InstantSearch = 8,
		OptimizedConversationSearch = 16,
		InternalAccess = 32
	}
}
