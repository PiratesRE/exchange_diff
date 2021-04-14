using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Web.Services.Protocols;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class HttpWebRequestExceptionHandler
	{
		internal static string TranslateExceptionString(Exception exception)
		{
			string result = string.Empty;
			if (exception is GrayException)
			{
				exception = ((exception.InnerException != null) ? exception.InnerException : exception);
				result = exception.GetType().Name + ":" + exception.Message;
			}
			else if (exception is WebException)
			{
				string text = "<No response>";
				WebException ex = (WebException)exception;
				if (ex.Response != null)
				{
					using (Stream responseStream = ex.Response.GetResponseStream())
					{
						if (responseStream != null && responseStream.CanRead)
						{
							if (responseStream.CanSeek)
							{
								responseStream.Seek(0L, SeekOrigin.Begin);
							}
							using (StreamReader streamReader = new StreamReader(responseStream))
							{
								text = streamReader.ReadToEnd();
							}
						}
					}
				}
				result = string.Concat(new string[]
				{
					exception.GetType().Name,
					":",
					exception.Message,
					":",
					text
				});
			}
			else if (exception is ArgumentException || exception is SoapException || exception is InvalidOperationException || exception is TimeoutException || exception is SocketException || exception is LocalizedException)
			{
				result = exception.GetType().Name + ":" + exception.Message;
			}
			return result;
		}

		internal static bool IsUnAuthorizedException(Exception exception)
		{
			WebException ex = exception as WebException;
			if (ex == null)
			{
				return false;
			}
			HttpWebResponse httpWebResponse = ex.Response as HttpWebResponse;
			return httpWebResponse != null && httpWebResponse.StatusCode == HttpStatusCode.Unauthorized;
		}

		internal static bool IsConnectionException(Exception exception, Trace tracer)
		{
			WebException ex = exception as WebException;
			if (ex == null)
			{
				return false;
			}
			HttpWebResponse httpWebResponse = ex.Response as HttpWebResponse;
			if (httpWebResponse != null)
			{
				HttpStatusCode statusCode = httpWebResponse.StatusCode;
				switch (statusCode)
				{
				case HttpStatusCode.Unauthorized:
				case HttpStatusCode.Forbidden:
				case HttpStatusCode.NotFound:
				case HttpStatusCode.MethodNotAllowed:
					break;
				case HttpStatusCode.PaymentRequired:
					goto IL_98;
				default:
					switch (statusCode)
					{
					case HttpStatusCode.InternalServerError:
					case HttpStatusCode.BadGateway:
					case HttpStatusCode.ServiceUnavailable:
					case HttpStatusCode.GatewayTimeout:
						if (tracer != null)
						{
							tracer.TraceError<HttpStatusCode>(0L, "IsConnectionException: request failed due error: {0}. This is an transient error.", httpWebResponse.StatusCode);
						}
						return false;
					case HttpStatusCode.NotImplemented:
					case HttpStatusCode.HttpVersionNotSupported:
						break;
					default:
						goto IL_98;
					}
					break;
				}
				if (tracer != null)
				{
					tracer.TraceError<HttpStatusCode>(0L, "IsConnectionException: request failed due error: {0}. This most likely indicates connection failure.", httpWebResponse.StatusCode);
				}
				return true;
			}
			IL_98:
			WebExceptionStatus status = ex.Status;
			switch (status)
			{
			case WebExceptionStatus.NameResolutionFailure:
			case WebExceptionStatus.ConnectFailure:
				break;
			default:
				switch (status)
				{
				case WebExceptionStatus.ConnectionClosed:
				case WebExceptionStatus.TrustFailure:
				case WebExceptionStatus.ServerProtocolViolation:
				case WebExceptionStatus.KeepAliveFailure:
				case WebExceptionStatus.Timeout:
					goto IL_D8;
				case WebExceptionStatus.SecureChannelFailure:
				case WebExceptionStatus.Pending:
					break;
				default:
					if (status == WebExceptionStatus.RequestProhibitedByProxy)
					{
						goto IL_D8;
					}
					break;
				}
				return false;
			}
			IL_D8:
			if (tracer != null)
			{
				tracer.TraceError<WebExceptionStatus>(0L, "IsConnectionException: request failed due error: {0}. This most likely indicates connection failure.", ex.Status);
			}
			return true;
		}
	}
}
