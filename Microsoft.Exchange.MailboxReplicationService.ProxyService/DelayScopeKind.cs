using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal enum DelayScopeKind
	{
		NoDelay,
		CPUOnly,
		DbRead,
		DbWrite
	}
}
