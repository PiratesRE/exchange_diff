using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Flags]
	internal enum GetMailboxSettingsFlags
	{
		Initial = 1,
		Finalize = 2
	}
}
