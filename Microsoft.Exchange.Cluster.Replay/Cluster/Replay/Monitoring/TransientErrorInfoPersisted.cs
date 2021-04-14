using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Cluster.Replay.Monitoring
{
	[DataContract(Name = "TransientErrorInfo", Namespace = "http://Microsoft.Exchange.HA.Monitoring/InternalMonitoringService/v1/")]
	public class TransientErrorInfoPersisted
	{
		[DataMember(Name = "CES", Order = 1)]
		public ErrorTypePersisted CurrentErrorState { get; set; }

		[DataMember(Name = "LST", Order = 2)]
		public string LastSuccessTransitionUtc { get; set; }

		[DataMember(Name = "LFT", Order = 3)]
		public string LastFailureTransitionUtc { get; set; }
	}
}
