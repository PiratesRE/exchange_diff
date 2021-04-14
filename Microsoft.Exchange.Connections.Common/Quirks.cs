using System;

namespace Microsoft.Exchange.Connections.Common
{
	[Flags]
	internal enum Quirks
	{
		None = 0,
		EnumerateNativeDeletesOnly = 1,
		EnumerateItemChangeAsDeleteAndAdd = 2,
		ApplyItemChangeAsDeleteAndAdd = 4,
		OnlyDeleteFoldersIfNoSubFolders = 8,
		AllowDirectCloudFolderUpdates = 32,
		DoNotTerminateSlowSyncs = 64
	}
}
