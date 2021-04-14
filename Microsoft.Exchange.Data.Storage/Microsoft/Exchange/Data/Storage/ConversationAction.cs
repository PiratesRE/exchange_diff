using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum ConversationAction : uint
	{
		None = 0U,
		AlwaysMove = 1U,
		AlwaysDelete = 2U,
		AlwaysCategorize = 8U,
		AlwaysClutterOrUnclutter = 16U,
		AlwaysMoveOrDelete = 3U,
		All = 27U
	}
}
