using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.MapiHttpClient;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.RpcClientAccess;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ClientSessionContext
	{
		public ClientSessionContext(MapiHttpBindingInfo bindingInfo, string vdirPath, IntPtr contextHandle)
		{
			Util.ThrowOnNullArgument(bindingInfo, "bindingInfo");
			Util.ThrowOnNullArgument(vdirPath, "vdirPath");
			if (contextHandle == IntPtr.Zero)
			{
				throw new ArgumentException("contextHandle cannot be IntPtr.Zero");
			}
			this.bindingInfo = bindingInfo;
			this.contextHandle = contextHandle;
			this.clientInfo = string.Format("{0}:{1}", ClientSessionContext.clientInfoGroupId, this.contextHandle.ToInt64().ToString());
			string text = vdirPath;
			this.requestPath = this.bindingInfo.BuildRequestPath(ref text);
			this.vdirPath = text;
			if (this.bindingInfo.AdditionalHttpHeaders != null && this.bindingInfo.AdditionalHttpHeaders.Count > 0)
			{
				this.additionalHttpHeaders = this.bindingInfo.AdditionalHttpHeaders;
			}
		}

		public IntPtr ContextHandle
		{
			get
			{
				return this.contextHandle;
			}
		}

		public string VdirPath
		{
			get
			{
				return this.vdirPath;
			}
		}

		public string RequestPath
		{
			get
			{
				return this.requestPath;
			}
		}

		public bool NeedsRefresh
		{
			get
			{
				return ExDateTime.UtcNow >= this.nextRefresh;
			}
		}

		public TimeSpan? DesiredExpiration
		{
			get
			{
				return this.bindingInfo.Expiration;
			}
		}

		public TimeSpan? ActualExpiration
		{
			get
			{
				return this.actualExpiration;
			}
		}

		public ExDateTime? LastCall
		{
			get
			{
				return this.lastCall;
			}
		}

		public TimeSpan? LastElapsedTime
		{
			get
			{
				return this.lastElapsedTime;
			}
		}

		public ExDateTime? Expires
		{
			get
			{
				return this.expires;
			}
		}

		public string RequestGroupId
		{
			get
			{
				return this.requestGroupId;
			}
		}

		public Dictionary<string, string> Cookies
		{
			get
			{
				return new Dictionary<string, string>(this.cookieValues);
			}
		}

		public WebHeaderCollection ResponseHeaders
		{
			get
			{
				return new WebHeaderCollection
				{
					this.responseHeaders
				};
			}
		}

		public HttpStatusCode? LastResponseStatusCode { get; private set; }

		public string LastResponseStatusDescription { get; private set; }

		public void SetAdditionalRequestHeaders(HttpWebRequest request)
		{
			if (this.additionalHttpHeaders != null && this.additionalHttpHeaders.Count > 0)
			{
				foreach (string name in this.additionalHttpHeaders.AllKeys)
				{
					request.Headers.Remove(name);
				}
				request.Headers.Add(this.additionalHttpHeaders);
			}
		}

		internal HttpWebRequest CreateRequest(out string requestId)
		{
			requestId = string.Empty;
			HttpWebRequest httpWebRequest = WebRequest.CreateHttp(new Uri(this.requestPath));
			httpWebRequest.ConnectionGroupName = this.connectionGroupName;
			httpWebRequest.Method = "POST";
			httpWebRequest.ContentType = "application/octet-stream";
			httpWebRequest.UserAgent = "MapiHttpClient";
			httpWebRequest.KeepAlive = true;
			httpWebRequest.PreAuthenticate = true;
			httpWebRequest.Expect = null;
			if (this.bindingInfo.Credentials != null)
			{
				httpWebRequest.Credentials = this.bindingInfo.Credentials;
			}
			else
			{
				httpWebRequest.UseDefaultCredentials = true;
			}
			if (this.bindingInfo.IgnoreCertificateErrors)
			{
				httpWebRequest.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(ClientSessionContext.CertificateValidationCallback);
			}
			ServicePoint servicePoint = httpWebRequest.ServicePoint;
			servicePoint.UseNagleAlgorithm = false;
			servicePoint.ReceiveBufferSize = 262144;
			servicePoint.Expect100Continue = false;
			servicePoint.ConnectionLimit = 65000;
			httpWebRequest.CookieContainer = new CookieContainer();
			lock (this.updateLock)
			{
				foreach (KeyValuePair<string, string> keyValuePair in this.cookieValues)
				{
					if (!string.IsNullOrWhiteSpace(keyValuePair.Value))
					{
						string value = ClientSessionContext.EscapeCookieIfNeeded(keyValuePair.Value);
						httpWebRequest.CookieContainer.Add(new Cookie(keyValuePair.Key, value, this.vdirPath, this.bindingInfo.ServerFqdn));
					}
				}
			}
			int num = Interlocked.Increment(ref this.nextRequestNumber);
			requestId = string.Format("{0}:{1}", this.requestGroupId, num);
			httpWebRequest.Headers.Add("X-RequestId", requestId);
			httpWebRequest.Headers.Add("X-ClientInfo", this.clientInfo);
			this.nextRefresh = ExDateTime.UtcNow + Constants.SessionContextRefreshInterval;
			this.actualExpiration = null;
			this.lastCall = new ExDateTime?(ExDateTime.UtcNow);
			return httpWebRequest;
		}

		internal void Update(HttpWebResponse response)
		{
			lock (this.updateLock)
			{
				this.LastResponseStatusCode = new HttpStatusCode?(response.StatusCode);
				this.LastResponseStatusDescription = response.StatusDescription;
				StringBuilder stringBuilder = null;
				string arg = string.Empty;
				if (ExTraceGlobals.ClientSessionContextTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					stringBuilder = new StringBuilder();
				}
				if (response.Cookies != null)
				{
					foreach (object obj2 in response.Cookies)
					{
						Cookie cookie = (Cookie)obj2;
						if (!string.IsNullOrWhiteSpace(cookie.Value))
						{
							this.cookieValues[cookie.Name] = cookie.Value;
							if (stringBuilder != null)
							{
								stringBuilder.Append(string.Format("{0}{1}={2}", arg, cookie.Name, cookie.Value));
								arg = ", ";
							}
						}
						else
						{
							this.cookieValues.Remove(cookie.Name);
						}
					}
				}
				if (response.Headers != null)
				{
					this.responseHeaders.Clear();
					this.responseHeaders.Add(response.Headers);
				}
				if (stringBuilder != null)
				{
					ExTraceGlobals.ClientSessionContextTracer.TraceInformation<IntPtr, string, string>(44960, 0L, "ClientSessionContext: Update cookies; ContextHandle={0}, RequestGroupId={1}, Cookies=[{2}]", this.contextHandle, this.requestGroupId, stringBuilder.ToString());
				}
			}
		}

		internal void UpdateRefresh(TimeSpan expiration)
		{
			this.actualExpiration = new TimeSpan?(expiration);
			this.nextRefresh = ExDateTime.UtcNow.AddMilliseconds(expiration.TotalMilliseconds / 2.0);
			this.expires = this.lastCall + expiration;
			ExTraceGlobals.ClientSessionContextTracer.TraceInformation<IntPtr, string, ExDateTime?>(61344, 0L, "ClientSessionContext: Update expiration information; ContextHandle={0}, RequestGroupId={1}, Expires={2}", this.contextHandle, this.requestGroupId, this.expires);
		}

		internal void UpdateElapsedTime(TimeSpan? elapsedTime)
		{
			this.lastElapsedTime = elapsedTime;
		}

		internal void Reset()
		{
			ServicePoint servicePoint = ServicePointManager.FindServicePoint(new Uri(this.requestPath));
			if (servicePoint != null)
			{
				servicePoint.CloseConnectionGroup(this.connectionGroupName);
			}
			string text = Guid.NewGuid().ToString("D");
			ExTraceGlobals.ClientSessionContextTracer.TraceInformation(33184, 0L, "ClientSessionContext: Reset connection group; ContextHandle={0}, RequestGroupId={1}, ConnectionGroupName={2}, NewConnectionGroupName={3}", new object[]
			{
				this.contextHandle,
				this.requestGroupId,
				this.connectionGroupName,
				text
			});
			this.connectionGroupName = text;
		}

		private static string EscapeCookieIfNeeded(string cookieValue)
		{
			if (!string.IsNullOrWhiteSpace(cookieValue) && cookieValue.Length >= 2 && cookieValue[0] != '"' && cookieValue[cookieValue.Length - 1] != '"' && cookieValue.IndexOfAny(ClientSessionContext.cookieCharsRequiringQuoteWrap) != -1)
			{
				cookieValue = string.Format("\"{0}\"", cookieValue);
			}
			return cookieValue;
		}

		private static bool CertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			return true;
		}

		private static readonly char[] cookieCharsRequiringQuoteWrap = new char[]
		{
			';',
			','
		};

		private static readonly string clientInfoGroupId = Guid.NewGuid().ToString("D");

		private readonly MapiHttpBindingInfo bindingInfo;

		private readonly IntPtr contextHandle;

		private readonly string vdirPath;

		private readonly string requestPath;

		private readonly object updateLock = new object();

		private readonly Dictionary<string, string> cookieValues = new Dictionary<string, string>();

		private readonly WebHeaderCollection responseHeaders = new WebHeaderCollection();

		private readonly string requestGroupId = Guid.NewGuid().ToString("D");

		private readonly WebHeaderCollection additionalHttpHeaders;

		private readonly string clientInfo;

		private string connectionGroupName = Guid.NewGuid().ToString("D");

		private int nextRequestNumber;

		private ExDateTime nextRefresh = ExDateTime.MaxValue;

		private TimeSpan? actualExpiration = null;

		private ExDateTime? lastCall = null;

		private TimeSpan? lastElapsedTime = null;

		private ExDateTime? expires = null;
	}
}
