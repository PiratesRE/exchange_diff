using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SetMailboxImportRequestCommand : NewMailboxImportRequestCommandBase
	{
		public SetMailboxImportRequestCommand() : base("Set-MailboxImportRequest", new Type[0])
		{
		}

		public const string CmdletName = "Set-MailboxImportRequest";
	}
}
