using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Net.DiagnosticsAggregation
{
	[DataContract]
	internal class LocalViewResponse
	{
		public LocalViewResponse(ServerSnapshotStatus serverSnapshotStatus)
		{
			this.ServerSnapshotStatus = serverSnapshotStatus;
		}

		[DataMember(IsRequired = true)]
		public ServerSnapshotStatus ServerSnapshotStatus { get; private set; }

		[DataMember]
		public QueueLocalViewResponse QueueLocalViewResponse { get; set; }
	}
}
