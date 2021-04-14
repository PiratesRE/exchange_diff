using System;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Net.Security;
using System.Runtime.Remoting;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.OnlineMeetings.Autodiscover;

namespace Microsoft.Exchange.Services.OnlineMeetings
{
	internal class UcwaWebRequest
	{
		public UcwaWebRequest(HttpWebRequest webRequest)
		{
			this.webRequest = webRequest;
			this.PreAuthenticate = true;
		}

		public RequestCachePolicy CachePolicy
		{
			get
			{
				return this.webRequest.CachePolicy;
			}
			set
			{
				this.webRequest.CachePolicy = value;
			}
		}

		public AuthenticationLevel AuthenticationLevel
		{
			get
			{
				return this.webRequest.AuthenticationLevel;
			}
			set
			{
				this.webRequest.AuthenticationLevel = value;
			}
		}

		public TokenImpersonationLevel ImpersonationLevel
		{
			get
			{
				return this.webRequest.ImpersonationLevel;
			}
			set
			{
				this.webRequest.ImpersonationLevel = value;
			}
		}

		public bool AllowAutoRedirect
		{
			get
			{
				return this.webRequest.AllowAutoRedirect;
			}
			set
			{
				this.webRequest.AllowAutoRedirect = value;
			}
		}

		public bool AllowWriteStreamBuffering
		{
			get
			{
				return this.webRequest.AllowWriteStreamBuffering;
			}
			set
			{
				this.webRequest.AllowWriteStreamBuffering = value;
			}
		}

		public bool HaveResponse
		{
			get
			{
				return this.webRequest.HaveResponse;
			}
		}

		public bool KeepAlive
		{
			get
			{
				return this.webRequest.KeepAlive;
			}
			set
			{
				this.webRequest.KeepAlive = value;
			}
		}

		public bool Pipelined
		{
			get
			{
				return this.webRequest.Pipelined;
			}
			set
			{
				this.webRequest.Pipelined = value;
			}
		}

		public bool PreAuthenticate
		{
			get
			{
				return this.webRequest.PreAuthenticate;
			}
			set
			{
				this.webRequest.PreAuthenticate = value;
			}
		}

		public bool UnsafeAuthenticatedConnectionSharing
		{
			get
			{
				return this.webRequest.UnsafeAuthenticatedConnectionSharing;
			}
			set
			{
				this.webRequest.UnsafeAuthenticatedConnectionSharing = value;
			}
		}

		public bool SendChunked
		{
			get
			{
				return this.webRequest.SendChunked;
			}
			set
			{
				this.webRequest.SendChunked = value;
			}
		}

		public DecompressionMethods AutomaticDecompression
		{
			get
			{
				return this.webRequest.AutomaticDecompression;
			}
			set
			{
				this.webRequest.AutomaticDecompression = value;
			}
		}

		public int MaximumResponseHeadersLength
		{
			get
			{
				return this.webRequest.MaximumResponseHeadersLength;
			}
			set
			{
				this.webRequest.MaximumResponseHeadersLength = value;
			}
		}

		public X509CertificateCollection ClientCertificates
		{
			get
			{
				return this.webRequest.ClientCertificates;
			}
			set
			{
				this.webRequest.ClientCertificates = value;
			}
		}

		public CookieContainer CookieContainer
		{
			get
			{
				return this.webRequest.CookieContainer;
			}
			set
			{
				this.webRequest.CookieContainer = value;
			}
		}

		public Uri RequestUri
		{
			get
			{
				return this.webRequest.RequestUri;
			}
		}

		public long ContentLength
		{
			get
			{
				return this.webRequest.ContentLength;
			}
			set
			{
				this.webRequest.ContentLength = value;
			}
		}

		public int Timeout
		{
			get
			{
				return this.webRequest.Timeout;
			}
			set
			{
				this.webRequest.Timeout = value;
			}
		}

		public int ReadWriteTimeout
		{
			get
			{
				return this.webRequest.ReadWriteTimeout;
			}
			set
			{
				this.webRequest.ReadWriteTimeout = value;
			}
		}

		public Uri Address
		{
			get
			{
				return this.webRequest.Address;
			}
		}

		public HttpContinueDelegate ContinueDelegate
		{
			get
			{
				return this.webRequest.ContinueDelegate;
			}
			set
			{
				this.webRequest.ContinueDelegate = value;
			}
		}

		public ServicePoint ServicePoint
		{
			get
			{
				return this.webRequest.ServicePoint;
			}
		}

		public string Host
		{
			get
			{
				return this.webRequest.Host;
			}
			set
			{
				this.webRequest.Host = value;
			}
		}

		public int MaximumAutomaticRedirections
		{
			get
			{
				return this.webRequest.MaximumAutomaticRedirections;
			}
			set
			{
				this.webRequest.MaximumAutomaticRedirections = value;
			}
		}

		public string Method
		{
			get
			{
				return this.webRequest.Method;
			}
			set
			{
				this.webRequest.Method = value;
			}
		}

		public ICredentials Credentials
		{
			get
			{
				return this.webRequest.Credentials;
			}
			set
			{
				this.webRequest.Credentials = value;
			}
		}

		public bool UseDefaultCredentials
		{
			get
			{
				return this.webRequest.UseDefaultCredentials;
			}
			set
			{
				this.webRequest.UseDefaultCredentials = value;
			}
		}

		public string ConnectionGroupName
		{
			get
			{
				return this.webRequest.ConnectionGroupName;
			}
			set
			{
				this.webRequest.ConnectionGroupName = value;
			}
		}

		public WebHeaderCollection Headers
		{
			get
			{
				return this.webRequest.Headers;
			}
			set
			{
				this.webRequest.Headers = value;
			}
		}

		public IWebProxy Proxy
		{
			get
			{
				return this.webRequest.Proxy;
			}
			set
			{
				this.webRequest.Proxy = value;
			}
		}

		public Version ProtocolVersion
		{
			get
			{
				return this.webRequest.ProtocolVersion;
			}
			set
			{
				this.webRequest.ProtocolVersion = value;
			}
		}

		public string ContentType
		{
			get
			{
				return this.webRequest.ContentType;
			}
			set
			{
				this.webRequest.ContentType = value;
			}
		}

		public string MediaType
		{
			get
			{
				return this.webRequest.MediaType;
			}
			set
			{
				this.webRequest.MediaType = value;
			}
		}

		public string TransferEncoding
		{
			get
			{
				return this.webRequest.TransferEncoding;
			}
			set
			{
				this.webRequest.TransferEncoding = value;
			}
		}

		public string Connection
		{
			get
			{
				return this.webRequest.Connection;
			}
			set
			{
				this.webRequest.Connection = value;
			}
		}

		public string Accept
		{
			get
			{
				return this.webRequest.Accept;
			}
			set
			{
				this.webRequest.Accept = value;
			}
		}

		public string Referer
		{
			get
			{
				return this.webRequest.Referer;
			}
			set
			{
				this.webRequest.Referer = value;
			}
		}

		public string UserAgent
		{
			get
			{
				return this.webRequest.UserAgent;
			}
			set
			{
				this.webRequest.UserAgent = value;
			}
		}

		public string Expect
		{
			get
			{
				return this.webRequest.Expect;
			}
			set
			{
				this.webRequest.Expect = value;
			}
		}

		public DateTime IfModifiedSince
		{
			get
			{
				return this.webRequest.IfModifiedSince;
			}
			set
			{
				this.webRequest.IfModifiedSince = value;
			}
		}

		public DateTime Date
		{
			get
			{
				return this.webRequest.Date;
			}
			set
			{
				this.webRequest.Date = value;
			}
		}

		public virtual string RequestContentType
		{
			get
			{
				return this.webRequest.ContentType;
			}
			set
			{
				this.webRequest.ContentType = value;
			}
		}

		public virtual string AcceptType
		{
			get
			{
				return this.webRequest.Accept;
			}
			set
			{
				this.webRequest.Accept = value;
			}
		}

		public object GetLifetimeService()
		{
			return this.webRequest.GetLifetimeService();
		}

		public object InitializeLifetimeService()
		{
			return this.webRequest.InitializeLifetimeService();
		}

		public ObjRef CreateObjRef(Type requestedType)
		{
			return this.webRequest.CreateObjRef(requestedType);
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			((ISerializable)this.webRequest).GetObjectData(info, context);
		}

		public IAsyncResult BeginGetRequestStream(AsyncCallback callback, object state)
		{
			return this.webRequest.BeginGetRequestStream(callback, state);
		}

		public Stream EndGetRequestStream(IAsyncResult asyncResult)
		{
			return this.webRequest.EndGetRequestStream(asyncResult);
		}

		public Stream EndGetRequestStream(IAsyncResult asyncResult, out TransportContext context)
		{
			return this.webRequest.EndGetRequestStream(asyncResult, out context);
		}

		public Stream GetRequestStream()
		{
			return this.webRequest.GetRequestStream();
		}

		public Stream GetRequestStream(out TransportContext context)
		{
			return this.webRequest.GetRequestStream(out context);
		}

		public IAsyncResult BeginGetResponse(AsyncCallback callback, object state)
		{
			return this.webRequest.BeginGetResponse(callback, state);
		}

		public WebResponse EndGetResponse(IAsyncResult asyncResult)
		{
			return this.webRequest.EndGetResponse(asyncResult);
		}

		public WebResponse GetResponse()
		{
			return this.webRequest.GetResponse();
		}

		public void Abort()
		{
			this.webRequest.Abort();
		}

		public void AddRange(int from, int to)
		{
			this.webRequest.AddRange(from, to);
		}

		public void AddRange(long from, long to)
		{
			this.webRequest.AddRange(from, to);
		}

		public void AddRange(int range)
		{
			this.webRequest.AddRange(range);
		}

		public void AddRange(long range)
		{
			this.webRequest.AddRange(range);
		}

		public void AddRange(string rangeSpecifier, int from, int to)
		{
			this.webRequest.AddRange(rangeSpecifier, from, to);
		}

		public void AddRange(string rangeSpecifier, long from, long to)
		{
			this.webRequest.AddRange(rangeSpecifier, from, to);
		}

		public void AddRange(string rangeSpecifier, int range)
		{
			this.webRequest.AddRange(rangeSpecifier, range);
		}

		public void AddRange(string rangeSpecifier, long range)
		{
			this.webRequest.AddRange(rangeSpecifier, range);
		}

		public virtual Task<Stream> GetRequestStreamAsync()
		{
			return Task<Stream>.Factory.FromAsync(new Func<AsyncCallback, object, IAsyncResult>(this.webRequest.BeginGetRequestStream), new Func<IAsyncResult, Stream>(this.webRequest.EndGetRequestStream), new object(), TaskCreationOptions.None);
		}

		public virtual Task<WebResponse> GetResponseAsync()
		{
			ExTraceGlobals.OnlineMeetingTracer.TraceInformation<string, string>(0, 0L, "[OnlineMeetings][UcwaWebRequest.GetResponseAsync] Request to {0} contains headers:{1}", this.webRequest.RequestUri.ToString(), this.webRequest.GetRequestHeadersAsString());
			return Task<WebResponse>.Factory.FromAsync(new Func<AsyncCallback, object, IAsyncResult>(this.webRequest.BeginGetResponse), new Func<IAsyncResult, WebResponse>(this.webRequest.EndGetResponse), new object(), TaskCreationOptions.None);
		}

		private readonly HttpWebRequest webRequest;
	}
}
