using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Metering
{
	internal class CountTrackerConfig : ICountTrackerConfig
	{
		public CountTrackerConfig(int maxEntityCount, int maxEntitiesPerGroup, TimeSpan promotionInterval, TimeSpan idleCachedConfigCleanupInterval)
		{
			ArgumentValidator.ThrowIfOutOfRange<int>("maxEntityCount", maxEntityCount, 1, int.MaxValue);
			ArgumentValidator.ThrowIfOutOfRange<int>("maxEntitiesPerGroup", maxEntitiesPerGroup, 1, maxEntityCount);
			ArgumentValidator.ThrowIfOutOfRange<TimeSpan>("promotionInterval", promotionInterval, TimeSpan.FromSeconds(5.0), TimeSpan.MaxValue);
			ArgumentValidator.ThrowIfOutOfRange<TimeSpan>("idleCachedConfigCleanupInterval", idleCachedConfigCleanupInterval, TimeSpan.FromSeconds(5.0), TimeSpan.MaxValue);
			this.MaxEntityCount = maxEntityCount;
			this.MaxEntitiesPerGroup = maxEntitiesPerGroup;
			this.PromotionInterval = promotionInterval;
			this.IdleCachedConfigCleanupInterval = idleCachedConfigCleanupInterval;
		}

		public int MaxEntityCount { get; private set; }

		public int MaxEntitiesPerGroup { get; private set; }

		public TimeSpan PromotionInterval { get; private set; }

		public TimeSpan IdleCachedConfigCleanupInterval { get; private set; }
	}
}
