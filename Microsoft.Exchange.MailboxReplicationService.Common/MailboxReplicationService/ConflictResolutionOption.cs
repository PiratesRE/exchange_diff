using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	public enum ConflictResolutionOption
	{
		KeepSourceItem = 1,
		KeepLatestItem,
		KeepAll
	}
}
