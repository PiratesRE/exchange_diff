using System;
using System.Net;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net.WebApplicationClient
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	public static class WebExceptionExtensions
	{
		public static WebExceptionTroubleshootingID GetTroubleshootingID(this WebException exception)
		{
			WebExceptionStatus status = exception.Status;
			if (status != WebExceptionStatus.ConnectFailure)
			{
				switch (status)
				{
				case WebExceptionStatus.ProtocolError:
				{
					HttpWebResponse httpWebResponse = (HttpWebResponse)exception.Response;
					if (httpWebResponse != null)
					{
						return httpWebResponse.GetTroubleshootingID();
					}
					break;
				}
				case WebExceptionStatus.TrustFailure:
					return WebExceptionTroubleshootingID.TrustFailure;
				}
				return WebExceptionTroubleshootingID.Uncategorized;
			}
			return WebExceptionTroubleshootingID.ServiceUnavailable;
		}

		public static WebExceptionTroubleshootingID GetTroubleshootingID(this HttpWebResponse proxyResponse)
		{
			HttpStatusCode statusCode = proxyResponse.StatusCode;
			switch (statusCode)
			{
			case HttpStatusCode.Unauthorized:
				return WebExceptionTroubleshootingID.Unauthorized;
			case HttpStatusCode.PaymentRequired:
				break;
			case HttpStatusCode.Forbidden:
				return WebExceptionTroubleshootingID.Forbidden;
			case HttpStatusCode.NotFound:
				if (proxyResponse.Server.StartsWith("Microsoft-HTTPAPI"))
				{
					return WebExceptionTroubleshootingID.ServiceUnavailable;
				}
				break;
			default:
				if (statusCode == HttpStatusCode.ServiceUnavailable)
				{
					return WebExceptionTroubleshootingID.ServiceUnavailable;
				}
				break;
			}
			return WebExceptionTroubleshootingID.Uncategorized;
		}
	}
}
