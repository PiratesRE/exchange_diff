using System;
using System.IO;
using System.Net;
using System.Security;

namespace Microsoft.Exchange.Net
{
	internal struct DownloadResult
	{
		public DownloadResult(Exception exception)
		{
			this.exception = exception;
			this.responseStream = null;
			this.lastModified = null;
			this.eTag = string.Empty;
			this.bytesDownloaded = 0L;
			this.responseUri = null;
			this.lastKnownRequestedUri = null;
			this.statusCode = HttpStatusCode.OK;
			this.responseHeaders = null;
			this.isRetryable = null;
			this.cookies = null;
		}

		public Exception Exception
		{
			get
			{
				return this.exception;
			}
		}

		public Stream ResponseStream
		{
			get
			{
				return this.responseStream;
			}
			internal set
			{
				this.responseStream = value;
			}
		}

		public DateTime? LastModified
		{
			get
			{
				return this.lastModified;
			}
			internal set
			{
				this.lastModified = value;
			}
		}

		public string ETag
		{
			get
			{
				return this.eTag;
			}
			internal set
			{
				this.eTag = value;
			}
		}

		public long BytesDownloaded
		{
			get
			{
				return this.bytesDownloaded;
			}
			internal set
			{
				this.bytesDownloaded = value;
			}
		}

		public HttpStatusCode StatusCode
		{
			get
			{
				return this.statusCode;
			}
			internal set
			{
				this.statusCode = value;
			}
		}

		public WebHeaderCollection ResponseHeaders
		{
			get
			{
				return this.responseHeaders;
			}
			internal set
			{
				this.responseHeaders = value;
			}
		}

		public CookieCollection Cookies
		{
			get
			{
				return this.cookies;
			}
			internal set
			{
				this.cookies = value;
			}
		}

		public Uri ResponseUri
		{
			get
			{
				return this.responseUri;
			}
			internal set
			{
				this.responseUri = value;
			}
		}

		public Uri LastKnownRequestedUri
		{
			get
			{
				return this.lastKnownRequestedUri;
			}
			internal set
			{
				this.lastKnownRequestedUri = value;
			}
		}

		public bool IsCanceled
		{
			get
			{
				return this.exception is DownloadCanceledException;
			}
		}

		public bool IsSucceeded
		{
			get
			{
				return this.exception == null;
			}
		}

		public bool IsRetryable
		{
			get
			{
				if (this.isRetryable == null)
				{
					this.isRetryable = new bool?(DownloadResult.IsRetryableException(this.exception));
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
			return this.exception.GetType().FullName;
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

		private Exception exception;

		private Stream responseStream;

		private DateTime? lastModified;

		private string eTag;

		private long bytesDownloaded;

		private Uri responseUri;

		private Uri lastKnownRequestedUri;

		private HttpStatusCode statusCode;

		private WebHeaderCollection responseHeaders;

		private CookieCollection cookies;

		private bool? isRetryable;
	}
}
