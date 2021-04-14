using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Flags]
	internal enum VerifyMailboxConnectionFlags
	{
		None = 0,
		MailboxSessionNotRequired = 1
	}
}
