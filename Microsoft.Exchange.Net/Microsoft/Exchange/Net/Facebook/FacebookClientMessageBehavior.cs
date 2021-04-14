using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net.Facebook
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class FacebookClientMessageBehavior : IEndpointBehavior
	{
		public FacebookClientMessageBehavior(FacebookClientMessageInspector clientInspector)
		{
			ArgumentValidator.ThrowIfNull("ClientInspector", clientInspector);
			this.clientInspector = clientInspector;
		}

		public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
		{
		}

		public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
		{
			if (!clientRuntime.MessageInspectors.Contains(this.clientInspector))
			{
				clientRuntime.MessageInspectors.Add(this.clientInspector);
			}
		}

		public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
		{
		}

		public void Validate(ServiceEndpoint endpoint)
		{
		}

		private FacebookClientMessageInspector clientInspector;
	}
}
