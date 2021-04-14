using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Net.DiagnosticsAggregation
{
	[DataContract]
	internal class AggregatedViewResponse
	{
		public AggregatedViewResponse(List<ServerSnapshotStatus> snapshotStatusOfServers)
		{
			this.SnapshotStatusOfServers = snapshotStatusOfServers;
		}

		[DataMember(IsRequired = true)]
		public List<ServerSnapshotStatus> SnapshotStatusOfServers { get; private set; }

		[DataMember]
		public QueueAggregatedViewResponse QueueAggregatedViewResponse { get; set; }
	}
}
