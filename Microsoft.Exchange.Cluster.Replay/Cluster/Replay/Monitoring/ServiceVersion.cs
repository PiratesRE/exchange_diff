using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Cluster.Replay.Monitoring
{
	[DataContract(Name = "ServiceVersion", Namespace = "http://Microsoft.Exchange.HA.Monitoring/InternalMonitoringService/v1/")]
	public class ServiceVersion
	{
		[DataMember(Name = "Version", IsRequired = true, Order = 0)]
		public long Version
		{
			get
			{
				return this.m_version;
			}
			set
			{
				this.m_version = value;
			}
		}

		public const int VERSION_V1 = 1;

		private long m_version;
	}
}
