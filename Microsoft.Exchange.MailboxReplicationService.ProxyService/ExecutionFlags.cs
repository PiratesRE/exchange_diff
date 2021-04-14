using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Flags]
	internal enum ExecutionFlags
	{
		Default = 0,
		ThrottlingNotRequired = 1,
		NoLock = 2
	}
}
