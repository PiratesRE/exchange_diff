using System;

namespace Microsoft.Exchange.Transport.Sync.Common.Subscription.Pop
{
	[Flags]
	internal enum PopAggregationFlags
	{
		UseSsl = 1,
		UseTls = 2,
		SecurityMask = 3,
		SyncDelete = 16,
		LeaveOnServer = 64,
		EnableSendAs = 256,
		UseBasicAuth = 0,
		UseSpaAuth = 8192,
		AuthenticationMask = 28672
	}
}
