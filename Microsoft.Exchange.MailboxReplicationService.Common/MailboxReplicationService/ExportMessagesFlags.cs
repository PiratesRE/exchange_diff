using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Flags]
	internal enum ExportMessagesFlags
	{
		None = 0,
		OneByOne = 1
	}
}
