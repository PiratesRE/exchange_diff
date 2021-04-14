using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Flags]
	internal enum EnumerateContentChangesFlags
	{
		None = 0,
		Catchup = 1,
		FirstPage = 2
	}
}
