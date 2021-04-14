using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Flags]
	internal enum MsgRecFlags
	{
		None = 0,
		Deleted = 1,
		Regular = 2,
		Associated = 4,
		New = 8,
		AllLegacy = 7
	}
}
