using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SetSyncRequestCommand : NewSyncRequestCommandBase
	{
		public SetSyncRequestCommand() : base("Set-SyncRequest", new Type[0])
		{
		}

		public const string CmdletName = "Set-SyncRequest";
	}
}
