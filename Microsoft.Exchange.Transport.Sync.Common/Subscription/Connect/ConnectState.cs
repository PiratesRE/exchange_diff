using System;

namespace Microsoft.Exchange.Transport.Sync.Common.Subscription.Connect
{
	public enum ConnectState
	{
		Disconnected,
		Connected,
		ConnectedNeedsToken,
		Disabled,
		Delayed
	}
}
