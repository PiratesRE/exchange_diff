using System;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxLoadBalance.CapacityData
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ConsumerSizeProjection : ICapacityProjection
	{
		public ConsumerSizeProjection(HeatMapCapacityData heatMapData, CapacityProjectionData capacityProjectionData, ByteQuantifiedSize averageMailboxSize, int queryBufferPeriod, double maxConsumerSizePercentage, ILogger logger)
		{
			this.heatMapData = heatMapData;
			this.capacityProjectionData = capacityProjectionData;
			this.averageMailboxSize = averageMailboxSize;
			this.queryBufferPeriod = queryBufferPeriod;
			this.maxConsumerSizePercentage = maxConsumerSizePercentage;
			this.logger = logger;
		}

		public BatchCapacityDatum GetCapacity()
		{
			this.logger.LogVerbose("Calculating projected consumer capacity with inputs: HeatMapData {0}, CapacityProjectionData: {1}, QueryBufferPeriod: {2}, Average Mailbox Size: {3}, Maximum Consumer Size Percentage: {4}.", new object[]
			{
				this.heatMapData,
				this.capacityProjectionData,
				this.queryBufferPeriod,
				this.averageMailboxSize,
				this.maxConsumerSizePercentage
			});
			double num = Math.Pow(1.0 + this.capacityProjectionData.ConsumerGrowthRate, (double)this.capacityProjectionData.GrowthPeriods);
			double num2 = this.heatMapData.ConsumerSize.ToBytes() * num;
			double num3 = this.heatMapData.TotalCapacity.ToBytes() * this.maxConsumerSizePercentage - num2;
			double num4 = num3 / num;
			double num5 = num4 / (double)this.queryBufferPeriod;
			int num6 = (int)Math.Floor(num5 / this.averageMailboxSize.ToBytes());
			if (num6 < 0)
			{
				num6 = 0;
			}
			this.logger.LogVerbose("Calculated projected capacity: {0} bytes and {1} mailboxes.", new object[]
			{
				num5,
				num6
			});
			return new BatchCapacityDatum
			{
				MaximumNumberOfMailboxes = num6
			};
		}

		private readonly ByteQuantifiedSize averageMailboxSize;

		private readonly CapacityProjectionData capacityProjectionData;

		private readonly HeatMapCapacityData heatMapData;

		private readonly int queryBufferPeriod;

		private readonly double maxConsumerSizePercentage;

		private readonly ILogger logger;
	}
}
