using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal class HttpWebRequestWrapper
	{
		public Uri RequestUri { get; private set; }

		public TestId StepId { get; private set; }

		public Uri RequestIpAddressUri { get; internal set; }

		public string Method { get; private set; }

		public WebHeaderCollection Headers { get; private set; }

		public List<DynamicHeader> DynamicHeaders { get; private set; }

		public RequestBody RequestBody { get; set; }

		public string ConnectionGroupName { get; set; }

		public Version ProtocolVersion
		{
			get
			{
				return this.protocolVersion;
			}
			private set
			{
				this.protocolVersion = value;
			}
		}

		public ExDateTime SentTime { get; set; }

		public TimeSpan DnsLatency { get; set; }

		public string TargetVipName { get; set; }

		public string TargetVipForestName { get; set; }

		public static HttpWebRequestWrapper CreateRequest(TestId stepId, Uri requestUri, string method, RequestBody body, ExCookieContainer cookieContainer, Dictionary<string, string> persistentHeaders, string userAgent, List<DynamicHeader> dynamicHeaders = null)
		{
			HttpWebRequestWrapper httpWebRequestWrapper = new HttpWebRequestWrapper();
			httpWebRequestWrapper.StepId = stepId;
			httpWebRequestWrapper.RequestUri = requestUri;
			httpWebRequestWrapper.Method = method;
			httpWebRequestWrapper.Headers = new WebHeaderCollection();
			httpWebRequestWrapper.DynamicHeaders = dynamicHeaders;
			httpWebRequestWrapper.Headers.Add("User-Agent", userAgent);
			httpWebRequestWrapper.Headers.Add("Accept", "*/*");
			httpWebRequestWrapper.Headers.Add("Cache-Control", "no-cache");
			if (cookieContainer != null)
			{
				string cookieHeader = cookieContainer.GetCookieHeader(new Uri(requestUri.GetLeftPart(UriPartial.Authority) + requestUri.AbsolutePath));
				if (!string.IsNullOrEmpty(cookieHeader))
				{
					httpWebRequestWrapper.Headers.Add("Cookie", cookieHeader);
				}
			}
			if (body != null)
			{
				httpWebRequestWrapper.RequestBody = body;
			}
			foreach (string text in persistentHeaders.Keys)
			{
				httpWebRequestWrapper.Headers.Add(text, persistentHeaders[text]);
			}
			if (httpWebRequestWrapper.DynamicHeaders != null)
			{
				foreach (DynamicHeader dynamicHeader in httpWebRequestWrapper.DynamicHeaders)
				{
					dynamicHeader.UpdateHeaders(httpWebRequestWrapper);
				}
			}
			return httpWebRequestWrapper;
		}

		public HttpWebRequest CreateHttpWebRequest()
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(this.RequestIpAddressUri);
			httpWebRequest.Host = this.RequestUri.Host;
			httpWebRequest.Method = this.Method;
			string[] allKeys = this.Headers.AllKeys;
			int i = 0;
			while (i < allKeys.Length)
			{
				string text = allKeys[i];
				string text2 = this.Headers[text];
				string a;
				if ((a = text) == null)
				{
					goto IL_A0;
				}
				if (!(a == "User-Agent"))
				{
					if (!(a == "Accept"))
					{
						if (!(a == "Content-Type"))
						{
							goto IL_A0;
						}
						httpWebRequest.ContentType = text2;
					}
					else
					{
						httpWebRequest.Accept = text2;
					}
				}
				else
				{
					httpWebRequest.UserAgent = text2;
				}
				IL_AD:
				i++;
				continue;
				IL_A0:
				httpWebRequest.Headers.Add(text, text2);
				goto IL_AD;
			}
			if (this.DynamicHeaders != null)
			{
				foreach (DynamicHeader dynamicHeader in this.DynamicHeaders)
				{
					dynamicHeader.UpdateHeaders(httpWebRequest);
				}
			}
			if (this.Method == "POST" && this.RequestBody == null)
			{
				httpWebRequest.ContentLength = 0L;
			}
			return httpWebRequest;
		}

		public string ToStringNoBody()
		{
			return this.ToString(RequestResponseStringFormatOptions.NoBody);
		}

		public override string ToString()
		{
			return this.ToString(RequestResponseStringFormatOptions.None);
		}

		public string ToString(RequestResponseStringFormatOptions formatOptions)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("{0} {1} HTTP/{2}{3}", new object[]
			{
				this.Method,
				this.RequestUri,
				this.ProtocolVersion,
				Environment.NewLine
			});
			foreach (object obj in this.Headers)
			{
				string text = (string)obj;
				string text2 = this.Headers[text];
				if ((formatOptions & RequestResponseStringFormatOptions.TruncateCookies) == RequestResponseStringFormatOptions.TruncateCookies && text.Equals("Cookie", StringComparison.OrdinalIgnoreCase))
				{
					Regex regex = new Regex("=(?<CookieStart>[^,;]{25,25})(?<CookieValue>[^,;]+)", RegexOptions.Compiled);
					text2 = regex.Replace(text2, "=${CookieStart}...");
				}
				stringBuilder.AppendFormat("{0}: {1}{2}", text, text2, Environment.NewLine);
			}
			if ((formatOptions & RequestResponseStringFormatOptions.NoBody) == RequestResponseStringFormatOptions.NoBody && this.RequestBody != null)
			{
				stringBuilder.Append(Environment.NewLine + this.RequestBody.ToString() + Environment.NewLine);
			}
			return stringBuilder.ToString();
		}

		private const string DefaultAcceptHeader = "*/*";

		private const string DefaultCacheHeader = "no-cache";

		private Version protocolVersion = new Version("1.1");
	}
}
