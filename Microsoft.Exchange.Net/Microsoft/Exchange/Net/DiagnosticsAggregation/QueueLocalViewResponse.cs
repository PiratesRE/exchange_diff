using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Net.DiagnosticsAggregation
{
	[DataContract]
	internal class QueueLocalViewResponse
	{
		public QueueLocalViewResponse(List<LocalQueueInfo> localQueues, DateTime timestamp)
		{
			this.LocalQueues = localQueues;
			this.Timestamp = timestamp;
		}

		[DataMember(IsRequired = true)]
		public DateTime Timestamp { get; private set; }

		[DataMember(IsRequired = true)]
		public List<LocalQueueInfo> LocalQueues { get; private set; }
	}
}
