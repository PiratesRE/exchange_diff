using System;
using System.Web;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net.Protocols;

namespace Microsoft.Exchange.HttpProxy.Common
{
	internal static class Extensions
	{
		public static string GetFullRawUrl(this HttpRequest httpRequest)
		{
			ArgumentValidator.ThrowIfNull("httpRequest", httpRequest);
			string text = httpRequest.Url.IsDefaultPort ? string.Empty : (":" + httpRequest.Url.Port.ToString());
			return string.Concat(new string[]
			{
				httpRequest.Url.Scheme,
				"://",
				httpRequest.Url.Host,
				text,
				httpRequest.RawUrl
			});
		}

		public static HttpRequestBase GetHttpRequestBase(this HttpRequest request)
		{
			ArgumentValidator.ThrowIfNull("request", request);
			return new HttpRequestWrapper(request);
		}

		public static bool IsProbeRequest(this HttpRequestBase request)
		{
			ArgumentValidator.ThrowIfNull("request", request);
			return request.IsProxyTestProbeRequest() || request.IsOutlookProbeRequest() || (!string.IsNullOrEmpty(request.UserAgent) && (request.UserAgent.IndexOf(Constants.MsExchMonString, StringComparison.OrdinalIgnoreCase) >= 0 || request.UserAgent.IndexOf(Constants.ActiveMonProbe, StringComparison.OrdinalIgnoreCase) >= 0 || request.UserAgent.IndexOf(Constants.LamProbe, StringComparison.OrdinalIgnoreCase) >= 0 || request.UserAgent.IndexOf(Constants.EasProbe, StringComparison.OrdinalIgnoreCase) >= 0));
		}

		public static bool IsProxyTestProbeRequest(this HttpRequestBase request)
		{
			ArgumentValidator.ThrowIfNull("request", request);
			string text = request.Headers[Constants.ProbeHeaderName];
			return !string.IsNullOrEmpty(text) && string.Equals(text.Trim(), WellKnownHeader.LocalProbeHeaderValue, StringComparison.OrdinalIgnoreCase);
		}

		public static bool IsOutlookProbeRequest(this HttpRequestBase request)
		{
			ArgumentValidator.ThrowIfNull("request", request);
			if (!string.IsNullOrEmpty(request.UserAgent) && request.UserAgent.IndexOf("MSRPC", StringComparison.OrdinalIgnoreCase) >= 0)
			{
				string text = request.Headers["cookie"];
				return !string.IsNullOrEmpty(text) && text.IndexOf(Constants.ActiveMonProbe, StringComparison.OrdinalIgnoreCase) >= 0;
			}
			string text2 = request.Headers["X-ClientApplication"];
			return !string.IsNullOrEmpty(text2) && text2.IndexOf("MapiHttpClient", StringComparison.OrdinalIgnoreCase) >= 0;
		}
	}
}
