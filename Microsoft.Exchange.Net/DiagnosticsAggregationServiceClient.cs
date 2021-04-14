using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Microsoft.Exchange.Net.DiagnosticsAggregation;

[DebuggerStepThrough]
[GeneratedCode("System.ServiceModel", "4.0.0.0")]
internal class DiagnosticsAggregationServiceClient : ClientBase<global::IDiagnosticsAggregationService>, global::IDiagnosticsAggregationService
{
	public DiagnosticsAggregationServiceClient()
	{
	}

	public DiagnosticsAggregationServiceClient(string endpointConfigurationName) : base(endpointConfigurationName)
	{
	}

	public DiagnosticsAggregationServiceClient(string endpointConfigurationName, string remoteAddress) : base(endpointConfigurationName, remoteAddress)
	{
	}

	public DiagnosticsAggregationServiceClient(string endpointConfigurationName, EndpointAddress remoteAddress) : base(endpointConfigurationName, remoteAddress)
	{
	}

	public DiagnosticsAggregationServiceClient(Binding binding, EndpointAddress remoteAddress) : base(binding, remoteAddress)
	{
	}

	public LocalViewResponse GetLocalView(LocalViewRequest request)
	{
		return base.Channel.GetLocalView(request);
	}

	public IAsyncResult BeginGetLocalView(LocalViewRequest request, AsyncCallback callback, object asyncState)
	{
		return base.Channel.BeginGetLocalView(request, callback, asyncState);
	}

	public LocalViewResponse EndGetLocalView(IAsyncResult result)
	{
		return base.Channel.EndGetLocalView(result);
	}

	public AggregatedViewResponse GetAggregatedView(AggregatedViewRequest request)
	{
		return base.Channel.GetAggregatedView(request);
	}

	public IAsyncResult BeginGetAggregatedView(AggregatedViewRequest request, AsyncCallback callback, object asyncState)
	{
		return base.Channel.BeginGetAggregatedView(request, callback, asyncState);
	}

	public AggregatedViewResponse EndGetAggregatedView(IAsyncResult result)
	{
		return base.Channel.EndGetAggregatedView(result);
	}
}
