using System;

namespace Microsoft.Mapi
{
	[Flags]
	internal enum ReadEventsFlags
	{
		None = 0,
		FailIfEventsDeleted = 1,
		IncludeMoveDestinationEvents = 2
	}
}
