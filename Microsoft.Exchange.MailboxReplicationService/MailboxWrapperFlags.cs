using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Flags]
	internal enum MailboxWrapperFlags
	{
		Source = 1,
		Target = 2,
		PST = 4,
		Archive = 16
	}
}
