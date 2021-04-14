using System;
using System.CodeDom.Compiler;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Microsoft.Exchange.Cluster.Replay.Monitoring.Client
{
	[GeneratedCode("System.ServiceModel", "4.0.0.0")]
	[ServiceContract(Namespace = "http://Microsoft.Exchange.HA.Monitoring/InternalMonitoringService/v1/", ConfigurationName = "Microsoft.Exchange.Cluster.Replay.Monitoring.Client.InternalMonitoringService")]
	internal interface InternalMonitoringService
	{
		[OperationContract(Action = "http://Microsoft.Exchange.HA.Monitoring/InternalMonitoringService/v1/InternalMonitoringService/GetVersion", ReplyAction = "http://Microsoft.Exchange.HA.Monitoring/InternalMonitoringService/v1/InternalMonitoringService/GetVersionResponse")]
		ServiceVersion GetVersion();

		[OperationContract(Action = "http://Microsoft.Exchange.HA.Monitoring/InternalMonitoringService/v1/InternalMonitoringService/GetVersion", ReplyAction = "http://Microsoft.Exchange.HA.Monitoring/InternalMonitoringService/v1/InternalMonitoringService/GetVersionResponse")]
		Task<ServiceVersion> GetVersionAsync();

		[OperationContract(Action = "http://Microsoft.Exchange.HA.Monitoring/InternalMonitoringService/v1/InternalMonitoringService/PublishDagHealthInfo", ReplyAction = "http://Microsoft.Exchange.HA.Monitoring/InternalMonitoringService/v1/InternalMonitoringService/PublishDagHealthInfoResponse")]
		void PublishDagHealthInfo(HealthInfoPersisted healthInfo);

		[OperationContract(Action = "http://Microsoft.Exchange.HA.Monitoring/InternalMonitoringService/v1/InternalMonitoringService/PublishDagHealthInfo", ReplyAction = "http://Microsoft.Exchange.HA.Monitoring/InternalMonitoringService/v1/InternalMonitoringService/PublishDagHealthInfoResponse")]
		Task PublishDagHealthInfoAsync(HealthInfoPersisted healthInfo);

		[OperationContract(Action = "http://Microsoft.Exchange.HA.Monitoring/InternalMonitoringService/v1/InternalMonitoringService/GetDagHealthInfoUpdateTimeUtc", ReplyAction = "http://Microsoft.Exchange.HA.Monitoring/InternalMonitoringService/v1/InternalMonitoringService/GetDagHealthInfoUpdateTimeUtcResponse")]
		DateTime GetDagHealthInfoUpdateTimeUtc();

		[OperationContract(Action = "http://Microsoft.Exchange.HA.Monitoring/InternalMonitoringService/v1/InternalMonitoringService/GetDagHealthInfoUpdateTimeUtc", ReplyAction = "http://Microsoft.Exchange.HA.Monitoring/InternalMonitoringService/v1/InternalMonitoringService/GetDagHealthInfoUpdateTimeUtcResponse")]
		Task<DateTime> GetDagHealthInfoUpdateTimeUtcAsync();

		[OperationContract(Action = "http://Microsoft.Exchange.HA.Monitoring/InternalMonitoringService/v1/InternalMonitoringService/GetDagHealthInfo", ReplyAction = "http://Microsoft.Exchange.HA.Monitoring/InternalMonitoringService/v1/InternalMonitoringService/GetDagHealthInfoResponse")]
		HealthInfoPersisted GetDagHealthInfo();

		[OperationContract(Action = "http://Microsoft.Exchange.HA.Monitoring/InternalMonitoringService/v1/InternalMonitoringService/GetDagHealthInfo", ReplyAction = "http://Microsoft.Exchange.HA.Monitoring/InternalMonitoringService/v1/InternalMonitoringService/GetDagHealthInfoResponse")]
		Task<HealthInfoPersisted> GetDagHealthInfoAsync();
	}
}
