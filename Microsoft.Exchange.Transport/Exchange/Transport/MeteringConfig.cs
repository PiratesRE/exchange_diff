using System;
using Microsoft.Exchange.Data.Metering;

namespace Microsoft.Exchange.Transport
{
	internal class MeteringConfig : ICountTrackerConfig
	{
		public MeteringConfig()
		{
			this.Enabled = TransportAppConfig.GetConfigBool("MeteringEnabled", true);
			this.MaxEntityCount = TransportAppConfig.GetConfigInt("MeteringMaxEntityCount", 1, int.MaxValue, 10000);
			this.MaxEntitiesPerGroup = TransportAppConfig.GetConfigInt("MeteringMaxEntitiesPerGroup", 1, int.MaxValue, 500);
			this.PromotionInterval = TransportAppConfig.GetConfigTimeSpan("MeteringPromotionInterval", TimeSpan.Zero, TimeSpan.MaxValue, TimeSpan.FromSeconds(5.0));
			this.IdleCachedConfigCleanupInterval = TransportAppConfig.GetConfigTimeSpan("MeteringIdleCachedConfigCleanupInterval", TimeSpan.Zero, TimeSpan.MaxValue, TimeSpan.FromMinutes(1.0));
		}

		public bool Enabled { get; private set; }

		public int MaxEntityCount { get; private set; }

		public int MaxEntitiesPerGroup { get; private set; }

		public TimeSpan PromotionInterval { get; private set; }

		public TimeSpan IdleCachedConfigCleanupInterval { get; private set; }
	}
}
