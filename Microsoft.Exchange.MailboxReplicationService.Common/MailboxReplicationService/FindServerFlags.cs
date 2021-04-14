using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Flags]
	internal enum FindServerFlags
	{
		None = 0,
		ForceRediscovery = 1,
		AllowMissing = 2,
		FindSystemMailbox = 4
	}
}
