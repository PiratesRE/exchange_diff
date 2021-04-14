using System;
using Microsoft.Exchange.Data.Reporting;

namespace Microsoft.Exchange.Management.TransportProcessingQuota
{
	[Serializable]
	public class TransportProcessingQuotaDigest
	{
		public Guid ExternalDirectoryOrganizationId { get; set; }

		public bool Throttled { get; set; }

		public ThrottlingSource Source { get; set; }

		public double Cost { get; set; }

		public int MessageCount { get; set; }

		public double MessageAverageSizeKb { get; set; }

		public double StandardDeviation { get; set; }

		public double ThrottlingFactor { get; set; }

		public double PartitionCost { get; set; }

		public int PartitionMessageCount { get; set; }

		public double PartitionMessageAverageSizeKb { get; set; }

		public int PartitionTenantCount { get; set; }

		internal static TransportProcessingQuotaDigest Create(TenantThrottleInfo tenantThrottleInfo)
		{
			return new TransportProcessingQuotaDigest
			{
				ExternalDirectoryOrganizationId = tenantThrottleInfo.TenantId,
				Throttled = tenantThrottleInfo.IsThrottled,
				Source = ((tenantThrottleInfo.ThrottleState == TenantThrottleState.Auto) ? ThrottlingSource.Calculated : ThrottlingSource.Override),
				Cost = tenantThrottleInfo.AverageMessageCostMs,
				MessageCount = tenantThrottleInfo.MessageCount,
				MessageAverageSizeKb = tenantThrottleInfo.AverageMessageSizeKb,
				StandardDeviation = tenantThrottleInfo.StandardDeviation,
				ThrottlingFactor = tenantThrottleInfo.ThrottlingFactor,
				PartitionCost = tenantThrottleInfo.PartitionAverageMessageCostMs,
				PartitionMessageCount = tenantThrottleInfo.PartitionMessageCount,
				PartitionMessageAverageSizeKb = tenantThrottleInfo.PartitionAverageMessageSizeKb,
				PartitionTenantCount = tenantThrottleInfo.PartitionTenantCount
			};
		}
	}
}
