using System;
using System.Web;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.HttpProxy.Common
{
	internal static class AspNetHelper
	{
		public static string GetClientIpAsProxyHeader(HttpRequest httpRequest)
		{
			ArgumentValidator.ThrowIfNull("httpRequest", httpRequest);
			string text = httpRequest.Headers[Constants.OriginatingClientIpHeader];
			if (text == null || !HttpProxySettings.TrustClientXForwardedFor.Value)
			{
				text = httpRequest.UserHostAddress;
			}
			else
			{
				text = string.Format("{0},{1}", text, httpRequest.UserHostAddress);
			}
			return text;
		}

		public static string GetClientPortAsProxyHeader(HttpContext httpContext)
		{
			ArgumentValidator.ThrowIfNull("httpContext", httpContext);
			string text = httpContext.Request.Headers[Constants.OriginatingClientPortHeader];
			string text2 = GccUtils.GetClientPort(httpContext);
			if (!string.IsNullOrEmpty(text))
			{
				text2 = string.Format("{0},{1}", text, text2);
			}
			return text2;
		}
	}
}
