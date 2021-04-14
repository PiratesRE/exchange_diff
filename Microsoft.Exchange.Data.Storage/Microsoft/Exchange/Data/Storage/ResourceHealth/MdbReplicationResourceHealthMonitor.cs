using System;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.ResourceHealth
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MdbReplicationResourceHealthMonitor : PingerDependentHealthMonitor
	{
		internal MdbReplicationResourceHealthMonitor(MdbReplicationResourceHealthMonitorKey key) : base(key, key.DatabaseGuid)
		{
			this.dataProtectionHealthAverage = new FixedTimeAverage(5000, 6, Environment.TickCount);
		}

		public override ResourceHealthMonitorWrapper CreateWrapper()
		{
			return new DatabaseReplicationProviderWrapper(this);
		}

		public void Update(uint dataProtectionHealth)
		{
			base.ReceivedUpdate();
			this.LastUpdateUtc = TimeProvider.UtcNow;
			this.dataProtectionHealthAverage.Add((dataProtectionHealth < 2147483647U) ? dataProtectionHealth : 2147483647U);
		}

		protected override int InternalMetricValue
		{
			get
			{
				if (!this.dataProtectionHealthAverage.IsEmpty)
				{
					return (int)this.dataProtectionHealthAverage.GetValue();
				}
				return -1;
			}
		}

		private const int BucketTimeInMsec = 5000;

		private const int NumberOfBucketsForAveraging = 6;

		private FixedTimeAverage dataProtectionHealthAverage;
	}
}
