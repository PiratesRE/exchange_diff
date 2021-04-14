using System;

namespace Microsoft.Exchange.Transport.Sync.Common.Subscription.Imap
{
	[Flags]
	internal enum IMAPAggregationFlags
	{
		UseSsl = 1,
		UseTls = 2,
		SecurityMask = 3,
		ConflictResolutionClientWin = 16,
		ConflictResolutionServerWin = 32,
		UseBasicAuth = 0,
		UseNtlmAuth = 256,
		AuthenticationMask = 256
	}
}
