using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Commands.MoveItems
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class MoveItemsCommand : EasServerCommand<MoveItemsRequest, MoveItemsResponse, MoveItemsStatus>
	{
		internal MoveItemsCommand(EasConnectionSettings easConnectionSettings) : base(Command.MoveItems, easConnectionSettings)
		{
		}
	}
}
