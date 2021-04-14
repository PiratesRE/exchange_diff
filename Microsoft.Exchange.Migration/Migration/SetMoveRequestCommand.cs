using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SetMoveRequestCommand : NewMoveRequestCommandBase
	{
		public SetMoveRequestCommand(ICollection<Type> ignoredExceptions) : base("Set-MoveRequest", ignoredExceptions)
		{
		}

		public const string CmdletName = "Set-MoveRequest";
	}
}
