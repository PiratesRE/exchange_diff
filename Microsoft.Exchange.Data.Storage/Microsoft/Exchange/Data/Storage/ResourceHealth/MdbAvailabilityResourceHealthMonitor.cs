using System;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.ResourceHealth
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MdbAvailabilityResourceHealthMonitor : PingerDependentHealthMonitor
	{
		internal MdbAvailabilityResourceHealthMonitor(MdbAvailabilityResourceHealthMonitorKey key) : base(key, key.DatabaseGuid)
		{
			this.dataAvailabilityHealthAverage = new FixedTimeAverage(5000, 5, Environment.TickCount);
		}

		public override ResourceHealthMonitorWrapper CreateWrapper()
		{
			return new DatabaseAvailabilityProviderWrapper(this);
		}

		public void Update(uint dataAvailabilityHealth)
		{
			base.ReceivedUpdate();
			this.LastUpdateUtc = TimeProvider.UtcNow;
			this.dataAvailabilityHealthAverage.Add((dataAvailabilityHealth < 2147483647U) ? dataAvailabilityHealth : 2147483647U);
		}

		protected override int InternalMetricValue
		{
			get
			{
				if (!this.dataAvailabilityHealthAverage.IsEmpty)
				{
					return (int)this.dataAvailabilityHealthAverage.GetValue();
				}
				return -1;
			}
		}

		private const int BucketTimeInMsec = 5000;

		private const int NumberOfBucketsForAveraging = 5;

		private FixedTimeAverage dataAvailabilityHealthAverage;
	}
}
