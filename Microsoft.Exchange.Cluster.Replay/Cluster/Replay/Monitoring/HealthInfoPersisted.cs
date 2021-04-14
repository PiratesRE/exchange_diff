using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Cluster.Common;

namespace Microsoft.Exchange.Cluster.Replay.Monitoring
{
	[DataContract(Name = "HealthInfo", Namespace = "http://Microsoft.Exchange.HA.Monitoring/InternalMonitoringService/v1/")]
	public class HealthInfoPersisted
	{
		[DataMember(Name = "CreateTime", Order = 1)]
		public string CreateTimeUtcStr { get; set; }

		[DataMember(Name = "LastUpdateTime", Order = 2)]
		public string LastUpdateTimeUtcStr { get; set; }

		[DataMember(Order = 3)]
		public List<ServerHealthInfoPersisted> Servers { get; set; }

		[DataMember(Order = 4)]
		public List<DbHealthInfoPersisted> Databases { get; set; }

		public HealthInfoPersisted()
		{
			this.Databases = new List<DbHealthInfoPersisted>(160);
			this.Servers = new List<ServerHealthInfoPersisted>(16);
		}

		public DateTime GetLastUpdateTimeUtc()
		{
			DateTime result;
			DateTimeHelper.TryParseIntoDateTime(this.LastUpdateTimeUtcStr, ref result);
			return result;
		}

		public DateTime GetCreateTimeUtc()
		{
			DateTime result;
			DateTimeHelper.TryParseIntoDateTime(this.CreateTimeUtcStr, ref result);
			return result;
		}
	}
}
