using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.XPath;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Security;

namespace Microsoft.Exchange.Security.Authentication.FederatedAuthService
{
	internal class LiveIdXmlAuth : LiveIdSTSBase
	{
		static LiveIdXmlAuth()
		{
			LiveIdXmlAuth.s_userAgent = string.Format("{0}/{1} {2}", LiveIdSTSBase.SiteName, "15.0.0", Environment.OSVersion.ToString());
		}

		public LiveIdXmlAuth(int traceId, AuthServiceStaticConfig config, NamespaceStats stats, string clientIP, string clientInfo) : base(traceId, LiveIdInstanceType.Consumer, stats)
		{
			this.LogonUri = config.liveidXmlAuth;
			base.ProfilePolicy = config.MSAProfilePolicy;
			this.clientIP = clientIP;
			if (string.IsNullOrEmpty(clientInfo))
			{
				this.clientInfo = config.siteName;
			}
			else
			{
				this.clientInfo = clientInfo;
			}
			this.siteId = config.MSASiteId;
			this.authStatusForResponseDump = config.AuthStatusForResponseDump;
		}

		public override string StsTag
		{
			get
			{
				return "XmlAuth";
			}
		}

		public override IAsyncResult StartRequestChain(string userId, byte[] password, AsyncCallback callback, object state)
		{
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.traceId, "LiveIdXmlAuth.Entering StartRequestChain()");
			this.stopwatch = Stopwatch.StartNew();
			this.traceUserName = userId;
			this.LogonUri = string.Format("{0}{1}id={2}&kpp=2&eip={3}&wp={4}", new object[]
			{
				this.LogonUri,
				(this.LogonUri.IndexOf('?') < 0) ? "?" : "&",
				this.siteId,
				this.clientIP,
				base.ProfilePolicy
			});
			ExTraceGlobals.AuthenticationTracer.TraceDebug<string, string>((long)this.traceId, "Constructing WebRequest uri='{0}' TokenConsumer='{1}'", this.LogonUri, LiveIdSTSBase.SiteName);
			int num = this.ConstructRequestContent(userId, password, this.clientInfo, null, null);
			ExTraceGlobals.AuthenticationTracer.TraceDebug((long)this.traceId, "contentLength = " + num);
			this.request = AuthServiceHelper.CreateHttpWebRequest(this.LogonUri);
			this.request.ContentType = "text/xml";
			this.request.ContentLength = (long)num;
			this.request.UserAgent = LiveIdXmlAuth.s_userAgent;
			if (base.ExtraHeaders != null)
			{
				this.request.Headers.Add(base.ExtraHeaders);
			}
			Interlocked.Increment(ref LiveIdSTSBase.numberOfLiveIdRequests);
			STSBase.counters.NumberOfLiveIdStsRequests.RawValue = (long)LiveIdSTSBase.numberOfLiveIdRequests;
			IAsyncResult result = this.request.BeginGetRequestStream(callback, state);
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.traceId, "Leaving LiveIdXmlAuth.StartRequestChain()");
			return result;
		}

		public override IAsyncResult ProcessRequest(IAsyncResult asyncResult, AsyncCallback callback, object state)
		{
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.traceId, "Entering LiveIdXmlAuth.ProcessRequest()");
			Stream stream = this.request.EndGetRequestStream(asyncResult);
			base.SSLConnectionLatency = this.stopwatch.ElapsedMilliseconds;
			STSBase.WriteBytes(stream, this.s_requestP1);
			STSBase.WriteBytes(stream, this.s_requestP2);
			STSBase.WriteBytes(stream, LiveIdXmlAuth.s_requestP3);
			if (this.s_requestP4 != null)
			{
				STSBase.WriteBytes(stream, this.s_requestP4);
			}
			STSBase.WriteBytes(stream, LiveIdXmlAuth.s_requestP5);
			STSBase.WriteBytes(stream, this.s_requestP6);
			stream.Close();
			if (ExTraceGlobals.AuthenticationTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					STSBase.WriteBytes(memoryStream, this.s_requestP1);
					string s = string.Format("<SignInName>{0}</SignInName><Password>{1}</Password>", this.traceUserName, "DummyPassword");
					STSBase.WriteBytes(memoryStream, Encoding.UTF8.GetBytes(s));
					STSBase.WriteBytes(memoryStream, LiveIdXmlAuth.s_requestP3);
					if (this.s_requestP4 != null)
					{
						STSBase.WriteBytes(memoryStream, this.s_requestP4);
					}
					STSBase.WriteBytes(memoryStream, LiveIdXmlAuth.s_requestP5);
					STSBase.WriteBytes(memoryStream, this.s_requestP6);
					memoryStream.Flush();
					base.LogRequest(memoryStream.GetBuffer(), this.request, null);
				}
			}
			IAsyncResult result = this.request.BeginGetResponse(callback, state);
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.traceId, "Leaving LiveIdXmlAuth.ProcessRequest()");
			return result;
		}

		public override string ProcessResponse(IAsyncResult asyncResult)
		{
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.traceId, "Entering LiveIdXmlAuth.ProcessResponse()");
			string result = string.Empty;
			bool closeConnectionGroup = false;
			try
			{
				using (WebResponse webResponse = this.request.EndGetResponse(asyncResult))
				{
					this.stopwatch.Stop();
					base.Latency = this.stopwatch.ElapsedMilliseconds;
					STSBase.counters.AverageLiveIdResponseTime.IncrementBy(this.stopwatch.ElapsedMilliseconds);
					STSBase.counters.AverageLiveIdResponseTimeBase.Increment();
					ExTraceGlobals.AuthenticationTracer.TracePerformance<long>((long)this.traceId, "LiveIdXmlAuth responded in {0}ms", this.stopwatch.ElapsedMilliseconds);
					if (webResponse != null && webResponse.Headers != null)
					{
						base.LiveServer = webResponse.Headers.Get("PPServer");
					}
					result = this.ParseResponse(webResponse as HttpWebResponse);
				}
			}
			catch (WebException ex)
			{
				HttpWebResponse httpWebResponse = (HttpWebResponse)ex.Response;
				if (httpWebResponse == null || httpWebResponse.StatusCode != HttpStatusCode.BadRequest)
				{
					base.ErrorString += ex.ToString();
					string str;
					if (ex.Status == WebExceptionStatus.TrustFailure && AuthService.CertErrorCache.TryGetValue(this.request, out str))
					{
						base.ErrorString += str;
						AuthService.CertErrorCache.Remove(this.request);
					}
					closeConnectionGroup = true;
					throw;
				}
				result = this.ParseResponse(httpWebResponse);
			}
			finally
			{
				bool flag = AuthServiceHelper.CloseConnectionGroupIfNeeded(closeConnectionGroup, this.LogonUri, base.ConnectionGroupName, this.traceId);
				if (flag)
				{
					base.ErrorString += "<ConnectionGroupClosed>";
				}
			}
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.traceId, "Leaving LiveIdXmlAuth.ProcessResponse()");
			return result;
		}

		private string ParseResponse(HttpWebResponse httpResponse)
		{
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.traceId, "Entering LiveIdXmlAuth.ParseResponse()");
			ExTraceGlobals.AuthenticationTracer.Information<string, HttpStatusCode>((long)this.traceId, "LiveIdXmlAuth {0} responded with status {1:d}", this.LogonUri, httpResponse.StatusCode);
			base.ErrorString = string.Empty;
			if (httpResponse == null)
			{
				return null;
			}
			Stream responseStream = httpResponse.GetResponseStream();
			try
			{
				using (StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8))
				{
					this.xmlRawResponse = streamReader.ReadToEnd();
					ExTraceGlobals.AuthenticationTracer.TraceDebug<string>((long)this.traceId, "LiveIdXmlAuth returned response {0}", this.xmlRawResponse);
					this.xmlResponse = new SafeXmlDocument();
					this.xmlResponse.PreserveWhitespace = true;
					try
					{
						this.xmlResponse.LoadXml(this.xmlRawResponse);
					}
					catch (XmlException ex)
					{
						ExTraceGlobals.AuthenticationTracer.TraceError<string, string>((long)this.traceId, "LiveIdXmlAuth has malformed RST response.  Exception {0} XML response {1}", ex.Message, this.xmlRawResponse);
						base.ErrorString = ex.ToString();
						throw;
					}
					XmlNode xmlNode = this.xmlResponse.SelectSingleNode("/LoginResponse/@Success");
					if (xmlNode == null)
					{
						base.ErrorString = string.Format("Server:{0}. The response from LiveIdXmlAuth does not contain SuccessNode.", base.LiveServer);
						throw new XmlException(base.ErrorString);
					}
					if (!xmlNode.Value.Equals("True", StringComparison.OrdinalIgnoreCase))
					{
						XmlNode xmlNode2 = this.xmlResponse.SelectSingleNode("/LoginResponse/Error/@Code");
						if (xmlNode2 == null)
						{
							base.ErrorString = string.Format("Server:{0}. Cannot find error node.", base.LiveServer);
							throw new XmlException(base.ErrorString);
						}
						this.errorCode = xmlNode2.Value.Trim();
						ExTraceGlobals.AuthenticationTracer.Information<string>((long)this.traceId, "LiveIdXmlAuth logon failure response has error", this.errorCode);
						if (this.errorCode.Equals("e5b", StringComparison.OrdinalIgnoreCase) || this.errorCode.Equals("e5a", StringComparison.OrdinalIgnoreCase) || this.errorCode.Equals("e11", StringComparison.OrdinalIgnoreCase))
						{
							base.IsBadCredentials = true;
							Interlocked.Increment(ref this.namespaceStats.BadPassword);
							this.namespaceStats.User = this.traceUserName;
						}
						else if (this.errorCode.Equals("e20a", StringComparison.OrdinalIgnoreCase))
						{
							base.IsExpiredCreds = true;
						}
						base.ErrorString = this.errorCode;
						if (!LiveIdXmlAuth.ignorableErrors.Contains(this.errorCode, StringComparer.OrdinalIgnoreCase))
						{
							STSBase.eventLogger.LogEvent(SecurityEventLogConstants.Tuple_LiveIdServerError, this.traceUserName + this.errorCode, new object[]
							{
								this.LogonUri,
								base.LiveServer,
								this.errorCode,
								0,
								base.ErrorString,
								this.traceUserName
							});
						}
						if (!string.IsNullOrEmpty(this.errorCode) && this.errorCode.Equals(this.authStatusForResponseDump, StringComparison.OrdinalIgnoreCase))
						{
							base.ErrorString = this.xmlRawResponse;
						}
						return null;
					}
					else
					{
						try
						{
							this.compactTicket = this.GetRpsTicket(this.xmlResponse);
							if (string.IsNullOrEmpty(this.compactTicket))
							{
								base.ErrorString = "Missing Compact Token." + this.xmlRawResponse;
								return null;
							}
						}
						catch (XPathException ex2)
						{
							base.ErrorString = string.Format("LiveID STS has malformed RST response. Exception {0} XML response {1}", ex2.ToString(), this.xmlRawResponse);
							ExTraceGlobals.AuthenticationTracer.TraceError((long)this.traceId, base.ErrorString);
							ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.traceId, "Leaving LiveIdXmlAuth.ParseResponse()");
							return null;
						}
					}
				}
			}
			finally
			{
				responseStream.Close();
				responseStream.Dispose();
			}
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.traceId, "Leaving LiveIdXmlAuth.ParseResponse()");
			return this.compactTicket;
		}

		private string GetRpsTicket(XmlDocument response)
		{
			XmlNode xmlNode = response.SelectSingleNode("/LoginResponse/Redirect");
			if (xmlNode == null)
			{
				return null;
			}
			string innerText = xmlNode.InnerText;
			string[] array = innerText.Split(new char[]
			{
				'&'
			});
			string text = null;
			foreach (string text2 in array)
			{
				if (text2.StartsWith("t=", StringComparison.OrdinalIgnoreCase))
				{
					text = text2;
					break;
				}
			}
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			return null;
		}

		public override void Abort()
		{
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.traceId, "Entering LiveIdXmlAuth.Abort()");
			if (this.request != null)
			{
				ExTraceGlobals.AuthenticationTracer.Information<string, string>((long)this.traceId, "Aborting http logon request to LiveId XmlAuth '{0}' for '{1}'", this.LogonUri, this.traceUserName);
				this.request.Abort();
			}
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.traceId, "Leaving LiveIdXmlAuth.Abort()");
		}

		public override string LiveToken()
		{
			return this.compactTicket;
		}

		public override bool UserRecoveryPossible()
		{
			return LiveIdXmlAuth.recoverableErrors.Contains(this.errorCode, StringComparer.OrdinalIgnoreCase) || (this.errorCode != null && this.errorCode.StartsWith("-2"));
		}

		public override void ProcessResponse(string userName, IAsyncResult asyncResult, out string compactTokenInRps, out RPSProfile rpsProfile, out string errorString)
		{
			compactTokenInRps = null;
			rpsProfile = null;
			errorString = null;
			new StringBuilder(256);
			compactTokenInRps = this.ProcessResponse(asyncResult);
			if (!string.IsNullOrEmpty(compactTokenInRps))
			{
				this.stopwatch = Stopwatch.StartNew();
				try
				{
					bool flag = MSATokenValidationClient.Instance.ParseCompactToken(2, compactTokenInRps, LiveIdSTSBase.SiteName, LiveIdSTSBase.RpsTicketLifetime, out rpsProfile, out errorString);
					if (flag)
					{
						if ((rpsProfile.AuthFlags & 11U) == 11U)
						{
							base.IsUnfamiliarLocation = true;
							rpsProfile = null;
						}
						else
						{
							rpsProfile.HasSignedTOU = RPSCommon.HasUserSignedTOU(rpsProfile.TokenFlags, userName);
						}
					}
				}
				finally
				{
					this.stopwatch.Stop();
					base.RPSParseLatency = this.stopwatch.ElapsedMilliseconds;
					STSBase.counters.AverageRPSCallLatency.IncrementBy(this.stopwatch.ElapsedMilliseconds);
					STSBase.counters.AverageRPSCallLatencyBase.Increment();
				}
			}
		}

		private int ConstructRequestContent(string username, byte[] password, string clientInfo, string deviceId, string deviceSyncKey)
		{
			string empty = string.Empty;
			int num = LiveIdXmlAuth.s_requestP5.Length + LiveIdXmlAuth.s_requestP3.Length;
			if (!string.IsNullOrWhiteSpace(deviceId))
			{
				string arg = string.Empty;
				if (!string.IsNullOrWhiteSpace(deviceSyncKey))
				{
					arg = string.Format("<SyncKey>{0}</SyncKey>", deviceSyncKey);
				}
				string.Format("<Device><ID>{0}</ID>{1}</Device>", deviceId, arg);
				this.s_requestP4 = Encoding.UTF8.GetBytes(deviceId);
				num += this.s_requestP4.Length;
			}
			if (LiveIdXmlAuth.requestTemplateP1Dict.ContainsKey(clientInfo))
			{
				this.s_requestP1 = LiveIdXmlAuth.requestTemplateP1Dict[clientInfo];
			}
			else
			{
				string s = string.Format("<?xml version=\"1.0\"?><LoginRequest><ClientInfo name=\"{0}\" version=\"1.35\" /><User>", clientInfo);
				this.s_requestP1 = Encoding.UTF8.GetBytes(s);
				LiveIdXmlAuth.requestTemplateP1Dict[clientInfo] = this.s_requestP1;
			}
			num += this.s_requestP1.Length;
			string s2 = string.Format("<SignInName>{0}</SignInName><Password>{1}</Password>", username, Encoding.Default.GetString(password));
			this.s_requestP2 = Encoding.UTF8.GetBytes(s2);
			num += this.s_requestP2.Length;
			this.s_requestP6 = Encoding.UTF8.GetBytes(string.Format("<!--{0}-->", "PPPPPPPPPPPPPPPP".Substring(0, Math.Max(0, "PPPPPPPPPPPPPPPP".Length - password.Length))));
			return num + this.s_requestP6.Length;
		}

		private const int UnfamiliarLocation = 11;

		private const string s_requestTemplateFormatP1 = "<?xml version=\"1.0\"?><LoginRequest><ClientInfo name=\"{0}\" version=\"1.35\" /><User>";

		private const string s_requestTemplateFormatP2 = "<SignInName>{0}</SignInName><Password>{1}</Password>";

		private const string s_requestTemplateFormatP3 = "<SavePassword>false</SavePassword></User>";

		private const string s_deviceNodeTemplateFormat = "<Device><ID>{0}</ID>{1}</Device>";

		private const string s_SyncKeyNodeTemplateFormat = "<SyncKey>{0}</SyncKey>";

		private const string s_requestTemplateFormatP5 = "</LoginRequest>";

		private const string s_signInNodePath = "/LoginRequest/User/SignInName";

		private const string s_passwordNodePath = "/LoginRequest/User/Password";

		private const string s_clientInfoNodePath = "/LoginRequest/ClientInfo/@name";

		private const string s_paddingString = "PPPPPPPPPPPPPPPP";

		private const string s_sanitizedPassword = "***";

		private const string s_userAgentFormat = "{0}/{1} {2}";

		private const string s_contentType = "text/xml";

		private const string s_xmlLoginUriFormat = "{0}{1}id={2}&kpp=2&eip={3}&wp={4}";

		private const string s_successNodePath = "/LoginResponse/@Success";

		private const string s_errorCodeNodePath = "/LoginResponse/Error/@Code";

		private const string s_redirectNodePath = "/LoginResponse/Redirect";

		private const string s_tCookiePrefix = "t=";

		private static ConcurrentDictionary<string, byte[]> requestTemplateP1Dict = new ConcurrentDictionary<string, byte[]>(10, 10, StringComparer.InvariantCulture);

		private static readonly string[] recoverableErrors = new string[]
		{
			"e10",
			"e20a",
			"-2147207999",
			"0x800434C1",
			"-2147208051",
			"0x8004348D",
			"-2147208000",
			"0x800434C0"
		};

		private static readonly string[] ignorableErrors = new string[]
		{
			"e5a",
			"e5b",
			"e10",
			"e11",
			"e12",
			"e20a"
		};

		private HttpWebRequest request;

		private Stopwatch stopwatch;

		private string traceUserName;

		private static readonly string s_userAgent;

		private readonly string clientIP;

		private readonly string clientInfo;

		private readonly int siteId;

		private string xmlRawResponse;

		private SafeXmlDocument xmlResponse;

		private readonly string authStatusForResponseDump;

		private string errorCode;

		private string compactTicket;

		private byte[] s_requestP1;

		private byte[] s_requestP2;

		private static readonly byte[] s_requestP3 = Encoding.UTF8.GetBytes("<SavePassword>false</SavePassword></User>");

		private byte[] s_requestP4;

		private static readonly byte[] s_requestP5 = Encoding.UTF8.GetBytes("</LoginRequest>");

		private byte[] s_requestP6;

		private static class XmlAuthErrors
		{
			public const string EmptyMemberNameOrPassword = "e1";

			public const string EmptyMemberName = "e2";

			public const string EmptyPassword = "e3";

			public const string EmptyDomainName = "e4";

			public const string ChildAccount = "e6";

			public const string RequestingServerNotAuthorized = "e8";

			public const string AccountLocked = "e10";

			public const string TooLongMemberNameOrPassword = "e11";

			public const string AlreadySignedInUser = "e12";

			public const string WrongPassword = "e5a";

			public const string WrongMemberName = "e5b";

			public const string UnknownError = "e5d";

			public const string WrongPostUrl = "e13a";

			public const string ExpiredPassword = "e20a";

			public const string WrongUserDomain = "80041034";

			public const string CompromisedUser1 = "-2147207999";

			public const string CompromisedUser2 = "0x800434C1";

			public const string EASIAccount1 = "-2147208051";

			public const string EASIAccount2 = "0x8004348D";

			public const string ServiceAbuseMode1 = "-2147208000";

			public const string ServiceAbuseMode2 = "0x800434C0";
		}
	}
}
