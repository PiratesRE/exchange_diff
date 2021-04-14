using System;

namespace Microsoft.Exchange.Diagnostics.WorkloadManagement
{
	internal struct AggregatedOperationStatistics
	{
		public static AggregatedOperationStatistics operator -(AggregatedOperationStatistics s1, AggregatedOperationStatistics s2)
		{
			return new AggregatedOperationStatistics
			{
				Count = s1.Count - s2.Count,
				TotalMilliseconds = s1.TotalMilliseconds - s2.TotalMilliseconds
			};
		}

		public AggregatedOperationType Type;

		public long Count;

		public double TotalMilliseconds;
	}
}
