using System;
using System.CodeDom.Compiler;
using System.ServiceModel;
using Microsoft.Exchange.Net.DiagnosticsAggregation;

[GeneratedCode("System.ServiceModel", "4.0.0.0")]
[ServiceContract(Namespace = "http://schemas.microsoft.com/exchange/services/2010/DiagnosticsAggregation", ConfigurationName = "IDiagnosticsAggregationService")]
internal interface IDiagnosticsAggregationService
{
	[FaultContract(typeof(DiagnosticsAggregationFault), Action = "http://schemas.microsoft.com/exchange/services/2010/DiagnosticsAggregation/IDiagnosticsAggregationService/GetLocalViewDiagnosticsAggregationFaultFault", Name = "DiagnosticsAggregationFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.Net.DiagnosticsAggregation")]
	[OperationContract(Action = "http://schemas.microsoft.com/exchange/services/2010/DiagnosticsAggregation/IDiagnosticsAggregationService/GetLocalView", ReplyAction = "http://schemas.microsoft.com/exchange/services/2010/DiagnosticsAggregation/IDiagnosticsAggregationService/GetLocalViewResponse")]
	LocalViewResponse GetLocalView(LocalViewRequest request);

	[OperationContract(AsyncPattern = true, Action = "http://schemas.microsoft.com/exchange/services/2010/DiagnosticsAggregation/IDiagnosticsAggregationService/GetLocalView", ReplyAction = "http://schemas.microsoft.com/exchange/services/2010/DiagnosticsAggregation/IDiagnosticsAggregationService/GetLocalViewResponse")]
	IAsyncResult BeginGetLocalView(LocalViewRequest request, AsyncCallback callback, object asyncState);

	LocalViewResponse EndGetLocalView(IAsyncResult result);

	[OperationContract(Action = "http://schemas.microsoft.com/exchange/services/2010/DiagnosticsAggregation/IDiagnosticsAggregationService/GetAggregatedView", ReplyAction = "http://schemas.microsoft.com/exchange/services/2010/DiagnosticsAggregation/IDiagnosticsAggregationService/GetAggregatedViewResponse")]
	[FaultContract(typeof(DiagnosticsAggregationFault), Action = "http://schemas.microsoft.com/exchange/services/2010/DiagnosticsAggregation/IDiagnosticsAggregationService/GetAggregatedViewDiagnosticsAggregationFaultFault", Name = "DiagnosticsAggregationFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.Net.DiagnosticsAggregation")]
	AggregatedViewResponse GetAggregatedView(AggregatedViewRequest request);

	[OperationContract(AsyncPattern = true, Action = "http://schemas.microsoft.com/exchange/services/2010/DiagnosticsAggregation/IDiagnosticsAggregationService/GetAggregatedView", ReplyAction = "http://schemas.microsoft.com/exchange/services/2010/DiagnosticsAggregation/IDiagnosticsAggregationService/GetAggregatedViewResponse")]
	IAsyncResult BeginGetAggregatedView(AggregatedViewRequest request, AsyncCallback callback, object asyncState);

	AggregatedViewResponse EndGetAggregatedView(IAsyncResult result);
}
