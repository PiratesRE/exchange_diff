using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Flags]
	internal enum ReadObjectFlags
	{
		None = 0,
		DontThrowOnCorruptData = 1,
		Refresh = 2,
		LastChunkOnly = 4
	}
}
