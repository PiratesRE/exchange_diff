using System;

namespace Microsoft.Exchange.Transport.Sync.Common.Rpc.Cache
{
	[Serializable]
	internal enum SubscriptionCacheObjectState
	{
		Valid,
		Invalid,
		Missing,
		Unexpected
	}
}
