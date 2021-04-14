using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Cluster.Replay.Monitoring
{
	[DataContract(Name = "ErrorType", Namespace = "http://Microsoft.Exchange.HA.Monitoring/InternalMonitoringService/v1/")]
	public enum ErrorTypePersisted : short
	{
		[EnumMember]
		Unknown,
		[EnumMember]
		Success,
		[EnumMember]
		Failure
	}
}
