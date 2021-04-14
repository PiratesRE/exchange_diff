using System;
using System.Net;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;

namespace Microsoft.Exchange.HttpProxy
{
	internal static class HttpWebHelper
	{
		public static void SetRange(HttpWebRequest destination, string value)
		{
			HttpRangeSpecifier httpRangeSpecifier = HttpRangeSpecifier.Parse(value);
			foreach (HttpRange httpRange in httpRangeSpecifier.RangeCollection)
			{
				if (httpRange.HasFirstBytePosition && httpRange.HasLastBytePosition)
				{
					destination.AddRange(httpRangeSpecifier.RangeUnitSpecifier, httpRange.FirstBytePosition, httpRange.LastBytePosition);
				}
				else if (httpRange.HasFirstBytePosition)
				{
					destination.AddRange(httpRangeSpecifier.RangeUnitSpecifier, httpRange.FirstBytePosition);
				}
				else if (httpRange.HasSuffixLength)
				{
					destination.AddRange(httpRangeSpecifier.RangeUnitSpecifier, -httpRange.SuffixLength);
				}
			}
		}

		public static void SetIfModifiedSince(HttpWebRequest destination, string value)
		{
			DateTime ifModifiedSince;
			if (DateTime.TryParse(value, out ifModifiedSince))
			{
				destination.IfModifiedSince = ifModifiedSince;
				return;
			}
			ExTraceGlobals.VerboseTracer.TraceDebug<string>(0L, "[HttpWebHelper::SetIfModifiedSince] Parse failure for IfModifiedSince header {0}", value);
		}

		public static void SetConnectionHeader(HttpWebRequest destination, string source)
		{
			if (source.IndexOf("keep-alive", StringComparison.OrdinalIgnoreCase) >= 0)
			{
				destination.KeepAlive = true;
				return;
			}
			if (source.IndexOf("close", StringComparison.OrdinalIgnoreCase) == -1)
			{
				destination.Connection = source;
			}
		}

		public static HttpWebHelper.ConnectivityError CheckConnectivityError(WebException e)
		{
			WebExceptionStatus status = e.Status;
			switch (status)
			{
			case WebExceptionStatus.NameResolutionFailure:
				return HttpWebHelper.ConnectivityError.NonRetryable;
			case WebExceptionStatus.ConnectFailure:
			case WebExceptionStatus.SendFailure:
				break;
			case WebExceptionStatus.ReceiveFailure:
				goto IL_42;
			default:
				if (status != WebExceptionStatus.ConnectionClosed)
				{
					switch (status)
					{
					case WebExceptionStatus.KeepAliveFailure:
						break;
					case WebExceptionStatus.Pending:
						goto IL_42;
					case WebExceptionStatus.Timeout:
					case WebExceptionStatus.ProxyNameResolutionFailure:
						return HttpWebHelper.ConnectivityError.NonRetryable;
					default:
						goto IL_42;
					}
				}
				break;
			}
			return HttpWebHelper.ConnectivityError.Retryable;
			IL_42:
			HttpWebResponse httpWebResponse = (HttpWebResponse)e.Response;
			if (httpWebResponse != null && (httpWebResponse.StatusCode == HttpStatusCode.ServiceUnavailable || httpWebResponse.StatusCode == HttpStatusCode.BadGateway || httpWebResponse.StatusCode == HttpStatusCode.GatewayTimeout))
			{
				return HttpWebHelper.ConnectivityError.NonRetryable;
			}
			return HttpWebHelper.ConnectivityError.None;
		}

		public enum ConnectivityError
		{
			None,
			Retryable,
			NonRetryable
		}
	}
}
