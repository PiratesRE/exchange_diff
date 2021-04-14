using System;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net.Logging;

namespace Microsoft.Exchange.Net
{
	internal class HttpSessionConfig
	{
		public HttpSessionConfig()
		{
			this.allowAutoRedirect = true;
			this.authenticationLevel = AuthenticationLevel.MutualAuthRequested;
			this.cachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
			this.credentials = CredentialCache.DefaultNetworkCredentials;
			this.defaultMaximumErrorResponseLength = 64;
			this.impersonationLevel = TokenImpersonationLevel.None;
			this.keepAlive = true;
			this.maximumAutomaticRedirections = 50;
			this.maximumResponseHeadersLength = 64;
			this.method = "GET";
			this.pipelined = true;
			this.protocolVersion = HttpVersion.Version11;
			this.proxy = WebRequest.DefaultWebProxy;
			this.timeout = 100000;
			this.userAgent = "MicrosoftExchangeServer-HttpClient";
			this.maximumResponseBodyLength = 6291456L;
			this.expect100Continue = null;
		}

		public HttpSessionConfig(int timeout) : this()
		{
			this.Timeout = timeout;
		}

		public HttpSessionConfig(IWebProxy proxy, ProtocolLog protocolLog) : this()
		{
			this.Proxy = proxy;
			this.ProtocolLog = protocolLog;
		}

		public bool AllowAutoRedirect
		{
			get
			{
				return this.allowAutoRedirect;
			}
			set
			{
				this.allowAutoRedirect = value;
			}
		}

		public AuthenticationLevel AuthenticationLevel
		{
			get
			{
				return this.authenticationLevel;
			}
			set
			{
				this.authenticationLevel = value;
			}
		}

		public RequestCachePolicy CachePolicy
		{
			get
			{
				return this.cachePolicy;
			}
			set
			{
				this.cachePolicy = value;
			}
		}

		public ICredentials Credentials
		{
			get
			{
				return this.credentials;
			}
			set
			{
				this.credentials = value;
			}
		}

		public int DefaultMaximumErrorResponseLength
		{
			get
			{
				return this.defaultMaximumErrorResponseLength;
			}
			set
			{
				ArgumentValidator.ThrowIfOutOfRange<int>("DefaultMaximumErrorResponseLength", value, -1, int.MaxValue);
				this.defaultMaximumErrorResponseLength = value;
			}
		}

		public DateTime? IfModifiedSince
		{
			get
			{
				return this.ifModifiedSince;
			}
			set
			{
				this.ifModifiedSince = value;
			}
		}

		public TokenImpersonationLevel ImpersonationLevel
		{
			get
			{
				return this.impersonationLevel;
			}
			set
			{
				this.impersonationLevel = value;
			}
		}

		public bool KeepAlive
		{
			get
			{
				return this.keepAlive;
			}
			set
			{
				this.keepAlive = value;
			}
		}

		public int MaximumAutomaticRedirections
		{
			get
			{
				return this.maximumAutomaticRedirections;
			}
			set
			{
				ArgumentValidator.ThrowIfOutOfRange<int>("MaximumAutomaticRedirections", value, 1, int.MaxValue);
				this.maximumAutomaticRedirections = value;
			}
		}

		public int MaximumResponseHeadersLength
		{
			get
			{
				return this.maximumResponseHeadersLength;
			}
			set
			{
				ArgumentValidator.ThrowIfOutOfRange<int>("MaximumResponseHeadersLength", value, -1, int.MaxValue);
				this.maximumResponseHeadersLength = value;
			}
		}

		public string Method
		{
			get
			{
				return this.method;
			}
			set
			{
				ArgumentValidator.ThrowIfNull("Method", value);
				HttpSessionConfig.ThrowIfMethodNotImplemented(value);
				this.method = value;
			}
		}

		public bool Pipelined
		{
			get
			{
				return this.pipelined;
			}
			set
			{
				this.pipelined = value;
			}
		}

		public bool PreAuthenticate
		{
			get
			{
				return this.preAuthenticate;
			}
			set
			{
				this.preAuthenticate = value;
			}
		}

		public Version ProtocolVersion
		{
			get
			{
				return this.protocolVersion;
			}
			set
			{
				HttpSessionConfig.ThrowIfInvalidProtocolVersion("ProtocolVersion", value);
				this.protocolVersion = value;
			}
		}

		public IWebProxy Proxy
		{
			get
			{
				return this.proxy;
			}
			set
			{
				ArgumentValidator.ThrowIfNull("Proxy", value);
				this.proxy = value;
			}
		}

		public int Timeout
		{
			get
			{
				return this.timeout;
			}
			set
			{
				ArgumentValidator.ThrowIfOutOfRange<int>("Timeout", value, -1, int.MaxValue);
				this.timeout = value;
			}
		}

		public bool UnsafeAuthenticatedConnectionSharing
		{
			get
			{
				return this.unsafeAuthenticatedConnectionSharing;
			}
			set
			{
				this.unsafeAuthenticatedConnectionSharing = value;
			}
		}

		public string UserAgent
		{
			get
			{
				return this.userAgent;
			}
			set
			{
				this.userAgent = value;
			}
		}

		public long MaximumResponseBodyLength
		{
			get
			{
				return this.maximumResponseBodyLength;
			}
			set
			{
				ArgumentValidator.ThrowIfOutOfRange<long>("MaximumResponseBodyLength", value, -1L, long.MaxValue);
				this.maximumResponseBodyLength = value;
			}
		}

		public string IfNoneMatch
		{
			get
			{
				return this.ifNoneMatch;
			}
			set
			{
				this.ifNoneMatch = value;
			}
		}

		public string IfHeader
		{
			get
			{
				return this.ifHeader;
			}
			set
			{
				this.ifHeader = value;
			}
		}

		public int MaxETagLength
		{
			get
			{
				return 64;
			}
		}

		public ProtocolLog ProtocolLog
		{
			get
			{
				return this.protocolLog;
			}
			set
			{
				this.protocolLog = value;
			}
		}

		public WebHeaderCollection Headers
		{
			get
			{
				return this.headers;
			}
			set
			{
				this.headers = value;
			}
		}

		public X509CertificateCollection ClientCertificates
		{
			get
			{
				return this.clientCertificates;
			}
			set
			{
				this.clientCertificates = value;
			}
		}

		public string ContentType
		{
			get
			{
				return this.contentType;
			}
			set
			{
				this.contentType = value;
			}
		}

		public CookieContainer CookieContainer
		{
			get
			{
				return this.cookieContainer;
			}
			set
			{
				this.cookieContainer = value;
			}
		}

		public Stream RequestStream
		{
			get
			{
				return this.requestStream;
			}
			set
			{
				this.requestStream = value;
			}
		}

		public int? Rows
		{
			get
			{
				return this.rows;
			}
			set
			{
				this.rows = value;
			}
		}

		public string Accept
		{
			get
			{
				return this.accept;
			}
			set
			{
				this.accept = value;
			}
		}

		public bool? Expect100Continue
		{
			get
			{
				return this.expect100Continue;
			}
			set
			{
				this.expect100Continue = value;
			}
		}

		public bool ReadWebExceptionResponseStream
		{
			get
			{
				return this.readWebExceptionResponseStream;
			}
			set
			{
				this.readWebExceptionResponseStream = value;
			}
		}

		private static void ThrowIfInvalidProtocolVersion(string name, Version protocolVersion)
		{
			if (protocolVersion != HttpVersion.Version10 && protocolVersion != HttpVersion.Version11)
			{
				throw new ArgumentException("The HTTP version is set to a value other than 1.0 or 1.1.", name);
			}
		}

		private static void ThrowIfMethodNotImplemented(string method)
		{
			if (string.Compare(method, "BPROPPATCH", StringComparison.OrdinalIgnoreCase) != 0 && string.Compare(method, "DELETE", StringComparison.OrdinalIgnoreCase) != 0 && string.Compare(method, "GET", StringComparison.OrdinalIgnoreCase) != 0 && string.Compare(method, "HEAD", StringComparison.OrdinalIgnoreCase) != 0 && string.Compare(method, "LOCK", StringComparison.OrdinalIgnoreCase) != 0 && string.Compare(method, "MKCOL", StringComparison.OrdinalIgnoreCase) != 0 && string.Compare(method, "MOVE", StringComparison.OrdinalIgnoreCase) != 0 && string.Compare(method, "POST", StringComparison.OrdinalIgnoreCase) != 0 && string.Compare(method, "PROPFIND", StringComparison.OrdinalIgnoreCase) != 0 && string.Compare(method, "PROPPATCH", StringComparison.OrdinalIgnoreCase) != 0 && string.Compare(method, "PUT", StringComparison.OrdinalIgnoreCase) != 0 && string.Compare(method, "SEARCH", StringComparison.OrdinalIgnoreCase) != 0 && string.Compare(method, "UNLOCK", StringComparison.OrdinalIgnoreCase) != 0 && string.Compare(method, "MERGE", StringComparison.OrdinalIgnoreCase) != 0)
			{
				throw new NotImplementedException("Only BPROPPATCH, DELETE, GET, HEAD, LOCK, MKCOL, MOVE, POST, PROPFIND, PROPPATCH, PUT, SEARCH, UNLOCK and MERGE request methods are implemented.");
			}
		}

		private const int MaxETagLengthValue = 64;

		private bool allowAutoRedirect;

		private AuthenticationLevel authenticationLevel;

		private RequestCachePolicy cachePolicy;

		private ICredentials credentials;

		private int defaultMaximumErrorResponseLength;

		private DateTime? ifModifiedSince;

		private TokenImpersonationLevel impersonationLevel;

		private bool keepAlive;

		private int maximumAutomaticRedirections;

		private int maximumResponseHeadersLength;

		private string method;

		private bool pipelined;

		private bool preAuthenticate;

		private Version protocolVersion;

		private IWebProxy proxy;

		private int timeout;

		private bool unsafeAuthenticatedConnectionSharing;

		private string userAgent;

		private long maximumResponseBodyLength;

		private string ifNoneMatch;

		private string ifHeader;

		private ProtocolLog protocolLog;

		private WebHeaderCollection headers;

		private X509CertificateCollection clientCertificates;

		private string contentType;

		private CookieContainer cookieContainer;

		private Stream requestStream;

		private int? rows;

		private string accept;

		private bool? expect100Continue;

		private bool readWebExceptionResponseStream;
	}
}
