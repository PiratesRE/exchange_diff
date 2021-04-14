using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Cluster.Replay.Monitoring
{
	[DataContract(Name = "DbCopyHealthInfo", Namespace = "http://Microsoft.Exchange.HA.Monitoring/InternalMonitoringService/v1/")]
	public class DbCopyHealthInfoPersisted
	{
		public Guid DbGuid { get; private set; }

		[DataMember(Name = "FQDN", Order = 1)]
		public string ServerFqdn { get; private set; }

		[DataMember(Order = 2)]
		public TransientErrorInfoPersisted CopyFoundInAD { get; set; }

		[DataMember(Order = 3)]
		public TransientErrorInfoPersisted CopyStatusRetrieved { get; set; }

		[DataMember(Order = 4)]
		public TransientErrorInfoPersisted CopyIsAvailable { get; set; }

		[DataMember(Order = 5)]
		public TransientErrorInfoPersisted CopyIsRedundant { get; set; }

		[DataMember(Order = 6)]
		public TransientErrorInfoPersisted CopyStatusHealthy { get; set; }

		[DataMember(Order = 7)]
		public DateTime LastCopyStatusTransitionTime { get; set; }

		[DataMember(Order = 8)]
		public TransientErrorInfoPersisted CopyStatusActive { get; set; }

		[DataMember(Order = 9)]
		public TransientErrorInfoPersisted CopyStatusMounted { get; set; }

		[DataMember(Order = 10)]
		public DateTime LastTouchedTime { get; set; }

		public DbCopyHealthInfoPersisted(Guid dbGuid, string serverFqdn)
		{
			this.DbGuid = dbGuid;
			this.ServerFqdn = serverFqdn;
		}
	}
}
