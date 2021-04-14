using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Flags]
	internal enum ExportFoldersDataToCopyFlags
	{
		None = 0,
		OutputCreateMessages = 1,
		IncludeCopyToStream = 2
	}
}
