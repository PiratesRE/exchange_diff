using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Microsoft.Exchange.Services.Wcf
{
	[AttributeUsage(AttributeTargets.Class)]
	internal class MessageInspectorBehavior : Attribute, IServiceBehavior
	{
		public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
		{
		}

		public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
		{
			foreach (ChannelDispatcherBase channelDispatcherBase in serviceHostBase.ChannelDispatchers)
			{
				ChannelDispatcher channelDispatcher = (ChannelDispatcher)channelDispatcherBase;
				foreach (EndpointDispatcher endpointDispatcher in channelDispatcher.Endpoints)
				{
					if (endpointDispatcher.ContractName.Equals("IUM12LegacyContract"))
					{
						endpointDispatcher.DispatchRuntime.MessageInspectors.Add(new UMLegacyMessageInspectorManager());
						endpointDispatcher.DispatchRuntime.OperationSelector = new DispatchByBodyElementOperationSelector();
					}
					else if (endpointDispatcher.ContractName.Equals("IJsonServiceContract") || endpointDispatcher.ContractName.Equals("IEWSStreamingContract"))
					{
						endpointDispatcher.DispatchRuntime.MessageInspectors.Add(new JsonMessageInspectorManager());
					}
					else
					{
						endpointDispatcher.DispatchRuntime.MessageInspectors.Add(new MessageInspectorManager());
						endpointDispatcher.DispatchRuntime.OperationSelector = new DispatchByBodyElementOperationSelector();
					}
				}
				channelDispatcher.ErrorHandlers.Add(new ExceptionHandlerInspector());
			}
		}

		public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
		{
		}
	}
}
