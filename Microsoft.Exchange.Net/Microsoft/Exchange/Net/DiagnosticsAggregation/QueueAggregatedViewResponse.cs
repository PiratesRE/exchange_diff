using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Net.DiagnosticsAggregation
{
	[DataContract]
	internal class QueueAggregatedViewResponse
	{
		public QueueAggregatedViewResponse(List<AggregatedQueueInfo> aggregatedQueues)
		{
			this.AggregatedQueues = aggregatedQueues;
		}

		[DataMember(IsRequired = true)]
		public List<AggregatedQueueInfo> AggregatedQueues { get; private set; }
	}
}
