using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "ClientPerformance")]
	public interface IClientPerformanceService
	{
		[OperationContract]
		string ReportWatson(ClientWatson report);

		[OperationContract]
		bool LogClientDatapoint(Datapoint[] datapoints);
	}
}
