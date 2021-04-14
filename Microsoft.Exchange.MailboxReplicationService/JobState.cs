using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal enum JobState
	{
		Runnable,
		Failed,
		Waiting,
		Finished
	}
}
