using System;

namespace Microsoft.Exchange.Transport.Sync.Common.Rpc.Cache
{
	internal enum SubscriptionCacheAction : uint
	{
		None,
		Validate,
		Delete,
		Fix
	}
}
