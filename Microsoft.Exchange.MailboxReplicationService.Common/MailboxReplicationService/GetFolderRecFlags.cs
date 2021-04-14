using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Flags]
	internal enum GetFolderRecFlags
	{
		None = 0,
		PromotedProperties = 1,
		Views = 2,
		Restrictions = 4,
		NoProperties = 8
	}
}
