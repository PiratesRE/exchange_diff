using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Flags]
	public enum PostMoveCleanupStatusFlags
	{
		None = 0,
		DestinationResetInTransitStatus = 1,
		DestinationSeedMBICache = 2,
		SourceMailboxCleanup = 4,
		AddTargetMailboxDataToReport = 8,
		TargetMailboxCleanup = 11,
		SetRelatedRequestsRehome = 16,
		UpdateSourceMailbox = 32,
		All = 63
	}
}
