using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Flags]
	internal enum MailboxOptions
	{
		None = 0,
		IgnoreExtendedRuleFAIs = 1,
		Finalize = 2
	}
}
