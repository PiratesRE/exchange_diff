using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Flags]
	internal enum FastTransferFlags
	{
		None = 0,
		PassThrough = 1
	}
}
