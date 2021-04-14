using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Flags]
	public enum SyncStateFlags
	{
		Default = 0,
		Replay = 1
	}
}
