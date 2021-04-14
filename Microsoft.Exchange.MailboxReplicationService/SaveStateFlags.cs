using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Flags]
	internal enum SaveStateFlags
	{
		Regular = 0,
		Lazy = 1,
		DontSaveRequestJob = 2,
		DontReportSyncStage = 4,
		RelinquishLongRunningJob = 8
	}
}
