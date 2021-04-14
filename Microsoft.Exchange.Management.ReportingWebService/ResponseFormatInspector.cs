using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace Microsoft.Exchange.Management.ReportingWebService
{
	public class ResponseFormatInspector : IDispatchMessageInspector
	{
		public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
		{
			if (request.Properties.ContainsKey("UriTemplateMatchResults"))
			{
				HttpRequestMessageProperty httpRequestMessageProperty = (HttpRequestMessageProperty)request.Properties[HttpRequestMessageProperty.Name];
				UriTemplateMatch uriTemplateMatch = (UriTemplateMatch)request.Properties["UriTemplateMatchResults"];
				string value = uriTemplateMatch.QueryParameters["$format"];
				if ("json".Equals(value, StringComparison.OrdinalIgnoreCase))
				{
					uriTemplateMatch.QueryParameters.Remove("$format");
					httpRequestMessageProperty.Headers["Accept"] = "application/json;odata=verbose";
				}
				else if ("Atom".Equals(value, StringComparison.OrdinalIgnoreCase))
				{
					uriTemplateMatch.QueryParameters.Remove("$format");
					httpRequestMessageProperty.Headers["Accept"] = "application/atom+xml";
				}
				else if (!string.IsNullOrEmpty(value))
				{
					ServiceDiagnostics.ThrowError(ReportingErrorCode.InvalidFormatQuery, Strings.InvalidFormatQuery);
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
	}
}
