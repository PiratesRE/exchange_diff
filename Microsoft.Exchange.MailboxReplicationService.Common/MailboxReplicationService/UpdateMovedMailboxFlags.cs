using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Flags]
	internal enum UpdateMovedMailboxFlags
	{
		None = 0,
		SkipMailboxReleaseCheck = 1,
		SkipProvisioningCheck = 2,
		MakeExoPrimary = 4
	}
}
