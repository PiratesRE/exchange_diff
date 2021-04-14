using System;

namespace Microsoft.Exchange.Transport.Sync.Migration.Rpc
{
	[Flags]
	internal enum SubscriptionStatusChangedResponse
	{
		OK = 1,
		ActionRequired = 40960,
		Delete = 40961,
		Disable = 40962
	}
}
