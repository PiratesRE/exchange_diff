using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Cluster.Replay.Monitoring
{
	[ServiceContract(Name = "InternalMonitoringService", Namespace = "http://Microsoft.Exchange.HA.Monitoring/InternalMonitoringService/v1/")]
	internal interface IInternalMonitoringService
	{
		[OperationContract]
		ServiceVersion GetVersion();

		[OperationContract]
		void PublishDagHealthInfo(HealthInfoPersisted healthInfo);

		[OperationContract]
		DateTime GetDagHealthInfoUpdateTimeUtc();

		[OperationContract]
		HealthInfoPersisted GetDagHealthInfo();
	}
}
