using System;

namespace Microsoft.Mapi
{
	[Flags]
	internal enum InTransitStatus
	{
		NotInTransit = 0,
		MoveSource = 1,
		MoveDestination = 2,
		OnlineMove = 16,
		AllowLargeItems = 32,
		ForPublicFolderMove = 64,
		SyncSource = 17,
		SyncDestination = 18
	}
}
