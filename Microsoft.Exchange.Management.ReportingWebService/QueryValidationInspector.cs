using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace Microsoft.Exchange.Management.ReportingWebService
{
	public class QueryValidationInspector : IDispatchMessageInspector
	{
		public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
		{
			if (request.Properties.ContainsKey("UriTemplateMatchResults"))
			{
				HttpRequestMessageProperty httpRequestMessageProperty = (HttpRequestMessageProperty)request.Properties[HttpRequestMessageProperty.Name];
				UriTemplateMatch uriTemplateMatch = (UriTemplateMatch)request.Properties["UriTemplateMatchResults"];
				string text = uriTemplateMatch.QueryParameters["$orderby"];
				if (!string.IsNullOrEmpty(text) && text.IndexOf('\'') >= 0)
				{
					ServiceDiagnostics.ThrowError(ReportingErrorCode.InvalidQueryException, "Single quotation marks is not supported in $orderby.");
				}
			}
			return null;
		}

		public void BeforeSendReply(ref Message reply, object correlationState)
		{
		}

		private const string OrderbyQueryParameter = "$orderby";

		private const string UriTemplateMatchResultsQuery = "UriTemplateMatchResults";
	}
}
