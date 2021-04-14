using System;

namespace Microsoft.Exchange.Hygiene.Cache.Data
{
	internal interface ICachePrimingInfo
	{
		CachePrimingState GetCurrentPrimingState(Type cacheObjectType);

		CacheFailoverMode GetCurrentFailoverMode(Type cacheObjectType, CacheFailoverMode requestedFailoverMode, CachePrimingState primingState = CachePrimingState.Unknown);
	}
}
