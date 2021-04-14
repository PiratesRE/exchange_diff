using System;
using System.Net;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal static class AttachmentDataProviderUtilities
	{
		public static AttachmentResultCode GetResultCodeFromWebException(WebException exception, DataProviderCallLogEvent logEvent)
		{
			HttpWebResponse httpWebResponse = exception.Response as HttpWebResponse;
			if (httpWebResponse != null)
			{
				logEvent.ErrorResponseHeaders = httpWebResponse.Headers;
			}
			WebExceptionStatus status = exception.Status;
			if (status == WebExceptionStatus.ProtocolError)
			{
				if (httpWebResponse != null)
				{
					HttpStatusCode statusCode = httpWebResponse.StatusCode;
					if (statusCode <= HttpStatusCode.NotFound)
					{
						if (statusCode == HttpStatusCode.Unauthorized)
						{
							return AttachmentResultCode.AccessDenied;
						}
						if (statusCode == HttpStatusCode.NotFound)
						{
							return AttachmentResultCode.NotFound;
						}
					}
					else if (statusCode == HttpStatusCode.RequestTimeout || statusCode == HttpStatusCode.GatewayTimeout)
					{
						return AttachmentResultCode.Timeout;
					}
				}
				return AttachmentResultCode.GenericFailure;
			}
			if (status == WebExceptionStatus.Timeout)
			{
				return AttachmentResultCode.Timeout;
			}
			return AttachmentResultCode.GenericFailure;
		}

		public const string ClientRequestIdHeader = "client-request-id";
	}
}
