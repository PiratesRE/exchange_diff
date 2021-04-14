using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Flags]
	internal enum SyncNowNotificationFlags
	{
		ActivateJob = 1,
		Send = 2
	}
}
