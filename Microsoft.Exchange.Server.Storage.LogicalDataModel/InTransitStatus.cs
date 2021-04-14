using System;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	[Flags]
	public enum InTransitStatus
	{
		NotInTransit = 0,
		SourceOfMove = 1,
		DestinationOfMove = 2,
		DirectionMask = 15,
		OnlineMove = 16,
		AllowLargeItem = 32,
		ForPublicFolderMove = 64,
		ControlMask = -16,
		KnownControlFlags = 112
	}
}
