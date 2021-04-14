using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using System.Web;

namespace Microsoft.Exchange.Management.ReportingWebService
{
	public class RewriteBaseUrlMessageInspector : IDispatchMessageInspector
	{
		public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
		{
			if (WebOperationContext.Current != null && WebOperationContext.Current.IncomingRequest.UriTemplateMatch != null)
			{
				string text = WebOperationContext.Current.IncomingRequest.Headers["msExchProxyUri"];
				if (!string.IsNullOrEmpty(text))
				{
					Uri uri = new Uri(new Uri(text), HttpContext.Current.Request.Url.PathAndQuery);
					UriBuilder uriBuilder = new UriBuilder(WebOperationContext.Current.IncomingRequest.UriTemplateMatch.BaseUri);
					UriBuilder uriBuilder2 = new UriBuilder(WebOperationContext.Current.IncomingRequest.UriTemplateMatch.RequestUri);
					uriBuilder.Host = uri.Host;
					uriBuilder.Port = uri.Port;
					uriBuilder2.Host = uri.Host;
					uriBuilder2.Port = uri.Port;
					OperationContext.Current.IncomingMessageProperties["MicrosoftDataServicesRootUri"] = uriBuilder.Uri;
					OperationContext.Current.IncomingMessageProperties["MicrosoftDataServicesRequestUri"] = uriBuilder2.Uri;
				}
			}
			return null;
		}

		public void BeforeSendReply(ref Message reply, object correlationState)
		{
		}

		private const string FormatQueryParameter = "$format";

		private const string AcceptHeader = "Accept";

		private const string UriTemplateMatchResultsQuery = "UriTemplateMatchResults";

		private const string ProxyUri = "msExchProxyUri";
	}
}
