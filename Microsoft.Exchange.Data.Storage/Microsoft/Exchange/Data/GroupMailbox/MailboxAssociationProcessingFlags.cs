using System;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[Flags]
	internal enum MailboxAssociationProcessingFlags
	{
		None = 0,
		SyncAllAssociations = 1
	}
}
