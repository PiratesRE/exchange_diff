using System;

namespace Microsoft.Exchange.Connections.Eas.Commands.MoveItems
{
	[Flags]
	public enum MoveItemsStatus
	{
		InvalidSourceId = 4097,
		InvalidDestinationId = 4098,
		Success = 3,
		SourceDestinationIdentical = 4100,
		CannotMove = 4101,
		Retry = 263,
		CompositeStatusError = 510
	}
}
