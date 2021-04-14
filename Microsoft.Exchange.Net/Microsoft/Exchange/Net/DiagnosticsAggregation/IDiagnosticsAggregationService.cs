using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Net.DiagnosticsAggregation
{
	[ServiceContract(Namespace = "http://schemas.microsoft.com/exchange/services/2010/DiagnosticsAggregation")]
	internal interface IDiagnosticsAggregationService
	{
		[FaultContract(typeof(DiagnosticsAggregationFault))]
		[OperationContract]
		LocalViewResponse GetLocalView(LocalViewRequest request);

		[OperationContract]
		[FaultContract(typeof(DiagnosticsAggregationFault))]
		AggregatedViewResponse GetAggregatedView(AggregatedViewRequest request);
	}
}
