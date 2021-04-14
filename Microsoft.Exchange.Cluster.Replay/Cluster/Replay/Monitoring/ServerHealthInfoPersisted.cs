using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Cluster.Replay.Monitoring
{
	[DataContract(Name = "ServerHealthInfo", Namespace = "http://Microsoft.Exchange.HA.Monitoring/InternalMonitoringService/v1/")]
	public class ServerHealthInfoPersisted
	{
		[DataMember(Name = "FQDN", Order = 1)]
		public string ServerFqdn { get; private set; }

		[DataMember(Order = 2)]
		public TransientErrorInfoPersisted ServerFoundInAD { get; set; }

		[DataMember(Order = 3)]
		public TransientErrorInfoPersisted CriticalForMaintainingAvailability { get; set; }

		[DataMember(Order = 4)]
		public TransientErrorInfoPersisted CriticalForMaintainingRedundancy { get; set; }

		[DataMember(Order = 5)]
		public TransientErrorInfoPersisted CriticalForRestoringAvailability { get; set; }

		[DataMember(Order = 6)]
		public TransientErrorInfoPersisted CriticalForRestoringRedundancy { get; set; }

		[DataMember(Order = 7)]
		public TransientErrorInfoPersisted HighForRestoringAvailability { get; set; }

		[DataMember(Order = 8)]
		public TransientErrorInfoPersisted HighForRestoringRedundancy { get; set; }

		public ServerHealthInfoPersisted(string serverFqdn)
		{
			this.ServerFqdn = serverFqdn;
		}
	}
}
