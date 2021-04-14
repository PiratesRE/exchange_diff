using System;
using System.IO;
using System.Net;
using System.Security;

namespace Microsoft.Office.CompliancePolicy
{
	internal sealed class DownloadResult
	{
		public DownloadResult(Exception exception)
		{
			this.Exception = exception;
			this.StatusCode = HttpStatusCode.OK;
			this.ETag = string.Empty;
		}

		public Exception Exception { get; private set; }

		public Stream ResponseStream { get; internal set; }

		public DateTime? LastModified { get; internal set; }

		public string ETag { get; internal set; }

		public long BytesDownloaded { get; internal set; }

		public HttpStatusCode StatusCode { get; internal set; }

		public WebHeaderCollection ResponseHeaders { get; internal set; }

		public CookieCollection Cookies { get; internal set; }

		public Uri ResponseUri { get; internal set; }

		public Uri LastKnownRequestedUri { get; internal set; }

		public bool IsCanceled
		{
			get
			{
				return this.Exception is DownloadCanceledException;
			}
		}

		public bool IsSucceeded
		{
			get
			{
				return this.Exception == null;
			}
		}

		public bool IsRetryable
		{
			get
			{
				if (this.isRetryable == null)
				{
					this.isRetryable = new bool?(DownloadResult.IsRetryableException(this.Exception));
				}
				return this.isRetryable.Value;
			}
		}

		public override string ToString()
		{
			if (this.IsCanceled)
			{
				return "Canceled";
			}
			if (this.IsSucceeded)
			{
				return "Success";
			}
			return this.Exception.GetType().FullName;
		}

		private static bool IsRetryableException(Exception exception)
		{
			if (exception == null || exception is SecurityException || exception is DownloadLimitExceededException || exception is ServerProtocolViolationException || exception is BadRedirectedUriException || exception is UnsupportedUriFormatException)
			{
				return false;
			}
			WebException ex = exception as WebException;
			return ex == null || DownloadResult.IsRetryableWebException(ex);
		}

		private static bool IsRetryableWebException(WebException exception)
		{
			WebExceptionStatus status = exception.Status;
			if (status != WebExceptionStatus.Success)
			{
				switch (status)
				{
				case WebExceptionStatus.ProtocolError:
					return DownloadResult.IsRetryableHttpStatusCode(((HttpWebResponse)exception.Response).StatusCode);
				case WebExceptionStatus.ConnectionClosed:
				case WebExceptionStatus.SecureChannelFailure:
					break;
				case WebExceptionStatus.TrustFailure:
				case WebExceptionStatus.ServerProtocolViolation:
					return false;
				default:
					switch (status)
					{
					case WebExceptionStatus.MessageLengthLimitExceeded:
					case WebExceptionStatus.CacheEntryNotFound:
					case WebExceptionStatus.RequestProhibitedByCachePolicy:
					case WebExceptionStatus.RequestProhibitedByProxy:
						return false;
					}
					break;
				}
				return true;
			}
			return false;
		}

		private static bool IsRetryableHttpStatusCode(HttpStatusCode statusCode)
		{
			if (statusCode != HttpStatusCode.NotFound && statusCode != HttpStatusCode.RequestTimeout)
			{
				switch (statusCode)
				{
				case HttpStatusCode.InternalServerError:
				case HttpStatusCode.BadGateway:
				case HttpStatusCode.ServiceUnavailable:
				case HttpStatusCode.GatewayTimeout:
					return true;
				}
				return false;
			}
			return true;
		}

		private bool? isRetryable;
	}
}
