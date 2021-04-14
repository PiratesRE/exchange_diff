using System;

namespace Microsoft.Exchange.Data.Metering
{
	internal interface ICountTrackerConfig
	{
		int MaxEntityCount { get; }

		int MaxEntitiesPerGroup { get; }

		TimeSpan PromotionInterval { get; }

		TimeSpan IdleCachedConfigCleanupInterval { get; }
	}
}
