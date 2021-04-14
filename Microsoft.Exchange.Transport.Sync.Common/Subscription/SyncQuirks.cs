using System;

namespace Microsoft.Exchange.Transport.Sync.Common.Subscription
{
	[Flags]
	internal enum SyncQuirks
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
