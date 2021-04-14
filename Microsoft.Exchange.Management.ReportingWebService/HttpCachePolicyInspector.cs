using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace Microsoft.Exchange.Management.ReportingWebService
{
	public class HttpCachePolicyInspector : IDispatchMessageInspector
	{
		public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
		{
			return null;
		}

		public void BeforeSendReply(ref Message reply, object correlationState)
		{
			if (reply != null && reply.Properties.ContainsKey(HttpResponseMessageProperty.Name))
			{
				HttpResponseMessageProperty httpResponseMessageProperty = (HttpResponseMessageProperty)reply.Properties[HttpResponseMessageProperty.Name];
				httpResponseMessageProperty.Headers.Set("Cache-Control", "no-cache, no-store");
			}
		}
	}
}
