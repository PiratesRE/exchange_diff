using System;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public class OWAWebHttpBehavior : WebHttpBehavior
	{
		protected override WebHttpDispatchOperationSelector GetOperationSelector(ServiceEndpoint endpoint)
		{
			return new OWADispatchOperationSelector(endpoint);
		}

		protected override IDispatchMessageFormatter GetRequestDispatchFormatter(OperationDescription operationDescription, ServiceEndpoint endpoint)
		{
			return base.GetRequestDispatchFormatter(operationDescription, endpoint);
		}

		protected override IClientMessageFormatter GetRequestClientFormatter(OperationDescription operationDescription, ServiceEndpoint endpoint)
		{
			return base.GetRequestClientFormatter(operationDescription, endpoint);
		}

		protected override void AddServerErrorHandlers(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
		{
			endpointDispatcher.ChannelDispatcher.ErrorHandlers.Clear();
			endpointDispatcher.ChannelDispatcher.ErrorHandlers.Add(new OWAFaultHandler());
		}
	}
}
