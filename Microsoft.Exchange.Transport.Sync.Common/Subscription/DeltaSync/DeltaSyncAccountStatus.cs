using System;

namespace Microsoft.Exchange.Transport.Sync.Common.Subscription.DeltaSync
{
	internal enum DeltaSyncAccountStatus
	{
		Normal,
		Blocked = 256,
		HipRequired = 512
	}
}
