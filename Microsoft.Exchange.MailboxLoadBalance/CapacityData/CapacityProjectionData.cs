using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Config;

namespace Microsoft.Exchange.MailboxLoadBalance.CapacityData
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class CapacityProjectionData
	{
		public CapacityProjectionData(double consumerGrowthRate, double organizationGrowthRate, int growthPeriods, ByteQuantifiedSize reservedCapacity)
		{
			this.ConsumerGrowthRate = consumerGrowthRate;
			this.OrganizationGrowthRate = organizationGrowthRate;
			this.GrowthPeriods = growthPeriods;
			this.ReservedCapacity = reservedCapacity;
		}

		public double ConsumerGrowthRate { get; set; }

		public int GrowthPeriods { get; set; }

		public double OrganizationGrowthRate { get; set; }

		public ByteQuantifiedSize ReservedCapacity { get; set; }

		public static CapacityProjectionData FromSettings(ILoadBalanceSettings settings)
		{
			ByteQuantifiedSize reservedCapacity = ByteQuantifiedSize.FromGB((ulong)((long)settings.ReservedCapacityInGb));
			double consumerGrowthRate = (double)settings.ConsumerGrowthRate / 100.0;
			double organizationGrowthRate = (double)settings.OrganizationGrowthRate / 100.0;
			return new CapacityProjectionData(consumerGrowthRate, organizationGrowthRate, settings.CapacityGrowthPeriods, reservedCapacity);
		}

		public override string ToString()
		{
			return string.Format("Consumer growth rate of {0}, Org growth rate of {1}, {2} growth periods and {3} of reserved capcity.", new object[]
			{
				this.ConsumerGrowthRate,
				this.OrganizationGrowthRate,
				this.GrowthPeriods,
				this.ReservedCapacity
			});
		}
	}
}
