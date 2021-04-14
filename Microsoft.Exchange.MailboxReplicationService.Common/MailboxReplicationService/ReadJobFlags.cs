using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Flags]
	internal enum ReadJobFlags
	{
		None = 0,
		SkipValidation = 1,
		SkipReadingMailboxRequestIndexEntries = 2
	}
}
