using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net.Protocols;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal class HttpWebResponseWrapper
	{
		private HttpWebResponseWrapper()
		{
		}

		public static HttpWebResponseWrapper Create(HttpWebRequestWrapper request, HttpWebResponse response)
		{
			if (response == null)
			{
				throw new ArgumentNullException("response");
			}
			HttpWebResponseWrapper httpWebResponseWrapper = new HttpWebResponseWrapper();
			httpWebResponseWrapper.Request = request;
			httpWebResponseWrapper.StatusCode = response.StatusCode;
			httpWebResponseWrapper.ProtocolVersion = response.ProtocolVersion;
			httpWebResponseWrapper.Headers = response.Headers;
			httpWebResponseWrapper.ReceivedTime = ExDateTime.Now;
			if (response.Headers[HttpResponseHeader.TransferEncoding] == "chunked")
			{
				using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
				{
					string s = streamReader.ReadToEnd();
					httpWebResponseWrapper.responseBody = Encoding.UTF8.GetBytes(s);
					httpWebResponseWrapper.ContentLength = (long)httpWebResponseWrapper.responseBody.Length;
					goto IL_109;
				}
			}
			httpWebResponseWrapper.ContentLength = response.ContentLength;
			if (response.ContentLength > 0L)
			{
				using (Stream responseStream = response.GetResponseStream())
				{
					httpWebResponseWrapper.responseBody = new byte[response.ContentLength];
					int num = 0;
					while ((long)num < response.ContentLength)
					{
						num += responseStream.Read(httpWebResponseWrapper.responseBody, num, (int)response.ContentLength - num);
					}
				}
			}
			IL_109:
			httpWebResponseWrapper.StreamReadFinishedTime = ExDateTime.Now;
			return httpWebResponseWrapper;
		}

		internal static HttpWebResponseWrapper Create(HttpWebRequestWrapper request, HttpStatusCode statusCode, Version protocolVersion, WebHeaderCollection headers, string responseBody)
		{
			HttpWebResponseWrapper httpWebResponseWrapper = new HttpWebResponseWrapper();
			httpWebResponseWrapper.Request = request;
			httpWebResponseWrapper.StatusCode = statusCode;
			httpWebResponseWrapper.ProtocolVersion = protocolVersion;
			httpWebResponseWrapper.Headers = headers;
			httpWebResponseWrapper.ReceivedTime = ExDateTime.Now;
			if (responseBody != null)
			{
				httpWebResponseWrapper.body = responseBody;
				httpWebResponseWrapper.ContentLength = (long)responseBody.Length;
			}
			httpWebResponseWrapper.StreamReadFinishedTime = ExDateTime.Now;
			return httpWebResponseWrapper;
		}

		public HttpWebRequestWrapper Request { get; private set; }

		public HttpStatusCode StatusCode { get; private set; }

		public Version ProtocolVersion { get; private set; }

		public ExDateTime ReceivedTime { get; private set; }

		public ExDateTime StreamReadFinishedTime { get; private set; }

		public TimeSpan TotalLatency
		{
			get
			{
				return this.StreamReadFinishedTime.Subtract(this.Request.SentTime);
			}
		}

		public TimeSpan ResponseLatency
		{
			get
			{
				return this.ReceivedTime.Subtract(this.Request.SentTime);
			}
		}

		public TimeSpan? CasLatency
		{
			get
			{
				return this.ReadLatencyHeader("X-DiagInfoIisLatency");
			}
		}

		public TimeSpan? RpcLatency
		{
			get
			{
				return this.ReadLatencyHeader("X-DiagInfoRpcLatency");
			}
		}

		public TimeSpan? LdapLatency
		{
			get
			{
				return this.ReadLatencyHeader(new string[]
				{
					"X-DiagInfoLdapLatency",
					"X-AuthDiagInfoLdapLatency"
				});
			}
		}

		public TimeSpan? MservLatency
		{
			get
			{
				return this.ReadLatencyHeader("X-AuthDiagInfoMservLookupLatency");
			}
		}

		public WebHeaderCollection Headers { get; private set; }

		public long ContentLength { get; private set; }

		public string Body
		{
			get
			{
				if (this.body == null && this.responseBody != null)
				{
					using (Stream stream = new MemoryStream(this.responseBody))
					{
						using (StreamReader streamReader = new StreamReader(stream))
						{
							this.body = streamReader.ReadToEnd();
						}
					}
				}
				return this.body;
			}
		}

		public string RespondingFrontEndServer
		{
			get
			{
				string result = null;
				if (this.Headers["PPServer"] != null || this.Headers["MSNSERVER"] != null)
				{
					string input = this.Headers["PPServer"] ?? this.Headers["MSNSERVER"];
					Regex regex = new Regex("H:\\s*(?<server>[^\\s]*)", RegexOptions.Compiled);
					Match match = regex.Match(input);
					if (match.Success)
					{
						result = match.Result("${server}");
					}
					else
					{
						result = this.Headers["PPServer"];
					}
				}
				else if (this.Headers[this.ExchangeFrontEndServerHeader] != null)
				{
					result = this.Headers[this.ExchangeFrontEndServerHeader];
				}
				else if (this.Headers["X-DiagInfo"] != null)
				{
					result = this.Headers["X-DiagInfo"];
				}
				return result;
			}
		}

		public bool? IsE14CasServer
		{
			get
			{
				if (this.Headers[this.ExchangeFrontEndServerHeader] != null)
				{
					return new bool?(false);
				}
				if (this.Headers["X-DiagInfo"] != null)
				{
					return new bool?(true);
				}
				return null;
			}
		}

		public string ProcessingServer
		{
			get
			{
				if (this.Headers[this.ExchangeBackEndServerHeader] != null)
				{
					return this.Headers[this.ExchangeBackEndServerHeader];
				}
				if (this.Headers[this.ExchangeTargetBackendServerHeader] != null)
				{
					return this.Headers[this.ExchangeTargetBackendServerHeader];
				}
				if (this.Headers[this.ExchangeFrontEndServerHeader] != null)
				{
					return this.Headers[this.ExchangeFrontEndServerHeader];
				}
				return this.RespondingFrontEndServer;
			}
		}

		public string MailboxServer
		{
			get
			{
				if (this.Headers[this.ExchangeTargetBackendServerHeader] != null)
				{
					return this.Headers[this.ExchangeTargetBackendServerHeader];
				}
				return this.Headers["X-DiagInfoMailbox"];
			}
		}

		public string DomainController
		{
			get
			{
				return this.Headers["X-DiagInfoDomainController"];
			}
		}

		public string ARRServer
		{
			get
			{
				return this.Headers["X-DiagInfoARR"];
			}
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
			stringBuilder.AppendFormat("HTTP/{0} {1} {2}{3}", new object[]
			{
				this.ProtocolVersion,
				(int)this.StatusCode,
				this.StatusCode,
				Environment.NewLine
			});
			foreach (object obj in this.Headers)
			{
				string text = (string)obj;
				string text2 = this.Headers[text];
				if ((formatOptions & RequestResponseStringFormatOptions.TruncateCookies) == RequestResponseStringFormatOptions.TruncateCookies && text.Equals("Set-Cookie", StringComparison.OrdinalIgnoreCase))
				{
					Regex regex = new Regex("=(?<CookieStart>[^,;]{25,25})(?<CookieValue>[^,;]+)", RegexOptions.Compiled);
					text2 = regex.Replace(text2, "=${CookieStart}...");
				}
				stringBuilder.AppendFormat("{0}: {1}{2}", text, text2, Environment.NewLine);
			}
			if ((formatOptions & RequestResponseStringFormatOptions.NoBody) != RequestResponseStringFormatOptions.NoBody && this.responseBody != null && !string.IsNullOrEmpty(this.Body))
			{
				stringBuilder.Append(Environment.NewLine);
				stringBuilder.Append(this.Body);
				stringBuilder.Append(Environment.NewLine);
			}
			stringBuilder.AppendFormat("Response time: {0}s{1}", this.TotalLatency.TotalSeconds, Environment.NewLine);
			return stringBuilder.ToString();
		}

		private TimeSpan? ReadLatencyHeader(string[] headerNames)
		{
			TimeSpan? timeSpan = null;
			foreach (string headerName in headerNames)
			{
				TimeSpan? timeSpan2 = this.ReadLatencyHeader(headerName);
				if (timeSpan2 != null)
				{
					if (timeSpan == null)
					{
						timeSpan = timeSpan2;
					}
					else
					{
						timeSpan += timeSpan2;
					}
				}
			}
			return timeSpan;
		}

		private TimeSpan? ReadLatencyHeader(string headerName)
		{
			string text = this.Headers[headerName];
			if (text == null)
			{
				return null;
			}
			int num;
			if (!int.TryParse(text, out num))
			{
				return null;
			}
			return new TimeSpan?(TimeSpan.FromMilliseconds((double)num));
		}

		private const string ExchangeLegacyServerHeader = "X-DiagInfo";

		private readonly string ExchangeFrontEndServerHeader = WellKnownHeader.XFEServer;

		private readonly string ExchangeBackEndServerHeader = WellKnownHeader.XBEServer;

		private readonly string ExchangeTargetBackendServerHeader = WellKnownHeader.XCalculatedBETarget;

		private string body;

		private byte[] responseBody;
	}
}
