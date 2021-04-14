using System;
using System.Web;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net.Protocols;

namespace Microsoft.Exchange.Services.OData
{
	internal static class HttpContextExtensions
	{
		internal static Uri GetServiceRootUri(this HttpContext httpContext)
		{
			Uri requestUri = httpContext.GetRequestUri();
			return new UriBuilder
			{
				Host = requestUri.Host,
				Scheme = requestUri.Scheme,
				Port = requestUri.Port,
				Path = "/EWS/OData/"
			}.Uri;
		}

		internal static Uri GetRequestUri(this HttpContext httpContext)
		{
			ArgumentValidator.ThrowIfNull("httpContext", httpContext);
			Uri result = httpContext.Request.Url;
			string text = httpContext.Request.Headers[WellKnownHeader.MsExchProxyUri];
			if (!string.IsNullOrEmpty(text))
			{
				result = new Uri(text);
			}
			return result;
		}
	}
}
