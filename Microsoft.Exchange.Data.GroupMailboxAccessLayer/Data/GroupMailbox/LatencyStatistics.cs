using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	internal struct LatencyStatistics
	{
		public AggregatedOperationStatistics? ADLatency { get; set; }

		public AggregatedOperationStatistics? RpcLatency { get; set; }

		public TimeSpan ElapsedTime { get; set; }

		public static LatencyStatistics operator -(LatencyStatistics s1, LatencyStatistics s2)
		{
			return new LatencyStatistics
			{
				ElapsedTime = s1.ElapsedTime - s2.ElapsedTime,
				ADLatency = s1.ADLatency - s2.ADLatency,
				RpcLatency = s1.RpcLatency - s2.RpcLatency
			};
		}
	}
}
