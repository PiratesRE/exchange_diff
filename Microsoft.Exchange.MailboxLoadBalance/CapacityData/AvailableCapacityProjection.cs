using System;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxLoadBalance.CapacityData
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class AvailableCapacityProjection : ICapacityProjection
	{
		public AvailableCapacityProjection(HeatMapCapacityData heatMapData, CapacityProjectionData capacityData, int queryBufferPeriod, ByteQuantifiedSize averageMailboxSize, ILogger logger)
		{
			this.heatMapData = heatMapData;
			this.capacityData = capacityData;
			this.queryBufferPeriod = queryBufferPeriod;
			this.averageMailboxSize = averageMailboxSize;
			this.logger = logger;
		}

		public BatchCapacityDatum GetCapacity()
		{
			this.logger.LogVerbose("Calculating projected available capacity with inputs: HeatMapData {0}, CapacityProjectionData: {1}, QueryBufferPeriod: {2}, Average Mailbox Size: {3}.", new object[]
			{
				this.heatMapData,
				this.capacityData,
				this.queryBufferPeriod,
				this.averageMailboxSize
			});
			double num = Math.Pow(1.0 + this.capacityData.ConsumerGrowthRate, (double)this.capacityData.GrowthPeriods);
			double num2 = this.heatMapData.ConsumerSize.ToBytes() * num;
			double num3 = this.heatMapData.OrganizationSize.ToBytes() * Math.Pow(1.0 + this.capacityData.OrganizationGrowthRate, (double)this.capacityData.GrowthPeriods);
			double num4 = this.heatMapData.TotalCapacity.ToBytes() - num2 - num3;
			double num5 = num4 / num - this.capacityData.ReservedCapacity.ToBytes();
			double num6 = num5 / (double)this.queryBufferPeriod;
			int num7 = (int)Math.Floor(num6 / this.averageMailboxSize.ToBytes());
			if (num7 < 0)
			{
				num7 = 0;
			}
			this.logger.LogVerbose("Calculated projected capacity: {0} bytes and {1} mailboxes.", new object[]
			{
				num6,
				num7
			});
			return new BatchCapacityDatum
			{
				MaximumNumberOfMailboxes = num7
			};
		}

		private readonly ByteQuantifiedSize averageMailboxSize;

		private readonly ILogger logger;

		private readonly CapacityProjectionData capacityData;

		private readonly HeatMapCapacityData heatMapData;

		private readonly int queryBufferPeriod;
	}
}
