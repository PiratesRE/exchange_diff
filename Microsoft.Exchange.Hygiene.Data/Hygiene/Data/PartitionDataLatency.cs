using System;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal class PartitionDataLatency
	{
		public PartitionDataLatency(int physicalInstanceId, DataLatencyDetailCollection[] latencyDetail)
		{
			if (latencyDetail == null)
			{
				throw new ArgumentNullException("latencyDetail");
			}
			this.PhysicalInstanceId = physicalInstanceId;
			this.LatencyDetail = latencyDetail;
		}

		public int PhysicalInstanceId { get; private set; }

		public DataLatencyDetailCollection[] LatencyDetail { get; private set; }
	}
}
