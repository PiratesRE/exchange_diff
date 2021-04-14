using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Microsoft.Exchange.MailboxReplicationService
{
	public class HTTPStatus200FaultBehavior : BehaviorExtensionElement, IEndpointBehavior
	{
		public override Type BehaviorType
		{
			get
			{
				return typeof(HTTPStatus200FaultBehavior);
			}
		}

		void IEndpointBehavior.ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
		{
			HTTPStatus200FaultBehavior.HTTPStatus200FaultMessageInspector item = new HTTPStatus200FaultBehavior.HTTPStatus200FaultMessageInspector();
			endpointDispatcher.DispatchRuntime.MessageInspectors.Add(item);
		}

		void IEndpointBehavior.AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
		{
		}

		void IEndpointBehavior.ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
		{
		}

		void IEndpointBehavior.Validate(ServiceEndpoint endpoint)
		{
		}

		protected override object CreateBehavior()
		{
			return new HTTPStatus200FaultBehavior();
		}

		public class HTTPStatus200FaultMessageInspector : IDispatchMessageInspector
		{
			void IDispatchMessageInspector.BeforeSendReply(ref Message reply, object correlationState)
			{
				if (reply.IsFault)
				{
					HttpResponseMessageProperty httpResponseMessageProperty = new HttpResponseMessageProperty();
					httpResponseMessageProperty.StatusCode = HttpStatusCode.OK;
					reply.Properties[HttpResponseMessageProperty.Name] = httpResponseMessageProperty;
				}
			}

			object IDispatchMessageInspector.AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
			{
				return null;
			}
		}
	}
}
