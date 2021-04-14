using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Net;
using System.Web;
using Microsoft.Exchange.Autodiscover.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Net.Protocols;

namespace Microsoft.Exchange.Autodiscover
{
	internal sealed class ProxyRequestData
	{
		static ProxyRequestData()
		{
			string text = ConfigurationManager.AppSettings[ProxyRequestData.proxyRequestTimeOutInMilliSecondsKey];
			if (string.IsNullOrEmpty(text) || !int.TryParse(text, out ProxyRequestData.proxyRequestTimeOutInMilliSeconds))
			{
				ProxyRequestData.proxyRequestTimeOutInMilliSeconds = (int)TimeSpan.FromMinutes(1.0).TotalMilliseconds;
			}
		}

		public ProxyRequestData(HttpRequest request, HttpResponse response, ref Stream bodyStream)
		{
			this.originalUri = request.Url;
			this.headers = request.Headers;
			this.userAgent = request.Headers.Get("UserAgent");
			this.authorization = request.Headers.Get("Authorization");
			this.httpMethod = request.HttpMethod;
			this.contentType = request.ContentType;
			this.originalClientIP = request.Headers.Get("X-Autodiscover-OriginalClientIP");
			if (string.IsNullOrEmpty(this.originalClientIP))
			{
				this.originalClientIP = request.UserHostAddress;
			}
			this.forwardForValue = this.headers.Get(WellKnownHeader.XForwardedFor);
			this.requestBody = this.GetRequestBody(bodyStream);
			bodyStream = null;
			this.Response = response;
		}

		public Stream RequestStream
		{
			get
			{
				return new MemoryStream(this.requestBody);
			}
		}

		public string AutodiscoverProxyHeader { get; private set; }

		public HttpResponse Response { get; private set; }

		public static bool IsIncomingProxyRequest(HttpRequest request)
		{
			return request.Headers.Get("X-Autodiscover-Proxy") != null;
		}

		public HttpWebRequest CloneRequest(string redirectHost)
		{
			ExTraceGlobals.AuthenticationTracer.TraceDebug<Uri, string>(0L, "ProxyRequestData::CloneRequest. Entry. originalRequest = {0}, redirectHost = {1}.", this.originalUri, redirectHost);
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(new UriBuilder(this.originalUri)
			{
				Host = redirectHost
			}.Uri);
			httpWebRequest.Method = this.httpMethod;
			httpWebRequest.Headers["Authorization"] = this.authorization;
			httpWebRequest.ContentType = this.contentType;
			httpWebRequest.UserAgent = this.userAgent;
			httpWebRequest.Timeout = ProxyRequestData.proxyRequestTimeOutInMilliSeconds;
			httpWebRequest.Proxy = null;
			httpWebRequest.AllowAutoRedirect = false;
			foreach (object obj in this.headers)
			{
				string text = (string)obj;
				if (text.StartsWith("X-MS-", StringComparison.OrdinalIgnoreCase))
				{
					httpWebRequest.Headers[text] = this.headers[text];
					RequestDetailsLoggerBase<RequestDetailsLogger>.Current.AppendGenericInfo(text, this.headers[text]);
				}
			}
			if (!string.IsNullOrEmpty(this.forwardForValue))
			{
				httpWebRequest.Headers[WellKnownHeader.XForwardedFor] = this.forwardForValue;
				RequestDetailsLoggerBase<RequestDetailsLogger>.Current.AppendGenericInfo(WellKnownHeader.XForwardedFor, this.forwardForValue);
			}
			if (FaultInjection.TraceTest<bool>((FaultInjection.LIDs)2804297021U))
			{
				this.AppendProxyHeader(httpWebRequest, redirectHost);
			}
			this.AppendProxyHeader(httpWebRequest, redirectHost);
			using (Stream requestStream = httpWebRequest.GetRequestStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(requestStream))
				{
					binaryWriter.Write(this.requestBody);
					binaryWriter.Flush();
				}
				ExTraceGlobals.AuthenticationTracer.TraceDebug<int>(0L, "ProxyRequestData::CloneRequest. Exit. RequestBody = {0}.", this.requestBody.Length);
			}
			return httpWebRequest;
		}

		public bool IsOriginalRequestProxyRequest()
		{
			return this.headers.Get("X-Autodiscover-Proxy") != null;
		}

		public override string ToString()
		{
			return string.Format("[{0}\t{1}\t{2}]", this.originalUri, this.originalClientIP, this.headers.Get("X-Autodiscover-Proxy"));
		}

		private static void StreamCopy(Stream source, Stream destination)
		{
			byte[] array = new byte[1024];
			for (int i = source.Read(array, 0, array.Length); i > 0; i = source.Read(array, 0, array.Length))
			{
				destination.Write(array, 0, i);
			}
		}

		private byte[] GetRequestBody(Stream bodyStream)
		{
			byte[] result;
			using (BinaryReader binaryReader = new BinaryReader(bodyStream))
			{
				result = binaryReader.ReadBytes((int)bodyStream.Length);
			}
			return result;
		}

		private void AppendProxyHeader(HttpWebRequest request, string redirectServer)
		{
			string text = request.Headers.Get("X-Autodiscover-Proxy") ?? string.Empty;
			if (text.IndexOf(redirectServer + ";", 0, StringComparison.OrdinalIgnoreCase) >= 0)
			{
				ExTraceGlobals.AuthenticationTracer.TraceError<string, string>(0L, "ProxyRequestData::AppendProxyHeader. The redirect server has been visited before. We are now in a loop. header = {0}, redirectServer = {1}", text, redirectServer);
				throw new ProxyLoopException(redirectServer);
			}
			this.AutodiscoverProxyHeader = string.Format("{0}{1};", text, redirectServer);
			request.Headers["X-Autodiscover-Proxy"] = this.AutodiscoverProxyHeader;
		}

		internal const string XAutodiscoverProxyHeader = "X-Autodiscover-Proxy";

		private const string XAutodiscoverOriginalClientIPHeader = "X-Autodiscover-OriginalClientIP";

		private const string XMSHeadersPrefix = "X-MS-";

		private static string proxyRequestTimeOutInMilliSecondsKey = "ProxyRequestTimeOutInMilliSeconds";

		private static int proxyRequestTimeOutInMilliSeconds = -1;

		private readonly Uri originalUri;

		private readonly NameValueCollection headers;

		private readonly string userAgent;

		private readonly string authorization;

		private readonly byte[] requestBody;

		private readonly string httpMethod;

		private readonly string contentType;

		private readonly string originalClientIP;

		private readonly string forwardForValue;
	}
}
