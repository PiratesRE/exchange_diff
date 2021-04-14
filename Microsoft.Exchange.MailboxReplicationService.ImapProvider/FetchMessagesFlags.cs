using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Flags]
	internal enum FetchMessagesFlags
	{
		None = 0,
		FetchByUid = 1,
		FetchBySeqNum = 2,
		IncludeExtendedData = 4
	}
}
