using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Microsoft.Exchange.Management.ReportingWebService
{
	[AttributeUsage(AttributeTargets.Class)]
	public class ReportingBehaviorAttribute : Attribute, IServiceBehavior
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
					endpointDispatcher.DispatchRuntime.MessageInspectors.Add(new ResponseFormatInspector());
					endpointDispatcher.DispatchRuntime.MessageInspectors.Add(new RewriteBaseUrlMessageInspector());
					endpointDispatcher.DispatchRuntime.MessageInspectors.Add(new HttpCachePolicyInspector());
					endpointDispatcher.DispatchRuntime.MessageInspectors.Add(new QueryValidationInspector());
				}
			}
		}

		public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
		{
		}
	}
}
