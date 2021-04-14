using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Flags]
	internal enum EnumerateHierarchyChangesFlags
	{
		None = 0,
		Catchup = 1,
		FirstPage = 2
	}
}
