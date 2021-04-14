using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Cluster.Replay.Monitoring
{
	[DataContract(Name = "DbHealthInfo", Namespace = "http://Microsoft.Exchange.HA.Monitoring/InternalMonitoringService/v1/")]
	public class DbHealthInfoPersisted
	{
		[DataMember(Order = 1)]
		public Guid DbGuid { get; private set; }

		[DataMember(Order = 2)]
		public string DbName { get; private set; }

		[DataMember(Order = 3)]
		public TransientErrorInfoPersisted DbFoundInAD { get; set; }

		[DataMember(Order = 4)]
		public TransientErrorInfoPersisted IsAtLeast2RedundantCopy { get; set; }

		[DataMember(Order = 5)]
		public TransientErrorInfoPersisted IsAtLeast3RedundantCopy { get; set; }

		[DataMember(Order = 6)]
		public TransientErrorInfoPersisted IsAtLeast4RedundantCopy { get; set; }

		[DataMember(Order = 7)]
		public TransientErrorInfoPersisted IsAtLeast2AvailableCopy { get; set; }

		[DataMember(Order = 8)]
		public TransientErrorInfoPersisted IsAtLeast3AvailableCopy { get; set; }

		[DataMember(Order = 9)]
		public TransientErrorInfoPersisted IsAtLeast4AvailableCopy { get; set; }

		[DataMember(Order = 10)]
		public List<DbCopyHealthInfoPersisted> DbCopies { get; set; }

		[DataMember]
		public TransientErrorInfoPersisted SkippedFromMonitoring { get; set; }

		[DataMember]
		public TransientErrorInfoPersisted IsAtLeast1RedundantCopy { get; set; }

		[DataMember]
		public TransientErrorInfoPersisted IsAtLeast1AvailableCopy { get; set; }

		public DbHealthInfoPersisted(Guid dbGuid, string dbName)
		{
			this.DbGuid = dbGuid;
			this.DbName = dbName;
			this.DbCopies = new List<DbCopyHealthInfoPersisted>(5);
		}
	}
}
