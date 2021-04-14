using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Security;
using Microsoft.Passport.RPS;

namespace Microsoft.Exchange.Security.Authentication.FederatedAuthService
{
	internal class LiveIdSTSSamlForm : LiveIdSTSBase
	{
		private LiveIdSTSSamlForm()
		{
		}

		internal LiveIdSTSSamlForm(int traceId, LiveIdInstanceType instance, NamespaceStats stats) : base(traceId, instance, stats)
		{
		}

		public LiveIdSTSSamlForm(int traceId, LiveIdInstanceType instance, AuthServiceStaticConfig config, NamespaceStats stats) : base(traceId, instance, stats)
		{
			if (config.defaultInstance == LiveIdInstanceType.Consumer)
			{
				base.TokenConsumer = config.siteName;
				this.LogonUri = config.liveHttpPostLogin;
				this.uriFormat = "{0}?wctx=wp%3d{1}%26wa%3Dwsignin1.0%26wreply%3dhttps:%2F%2F{2}%2Fowa%2F";
				this.liveTokenType = LiveIdSTSSamlForm.TokenType.Compact;
				return;
			}
			if (instance == LiveIdInstanceType.Consumer)
			{
				base.TokenConsumer = config.msoSiteName;
				this.LogonUri = config.liveHttpPostLogin;
				this.uriFormat = "{0}?wctx=wa%3Dwsignin1.0%26wtrealm%3d{2}";
				this.liveTokenType = LiveIdSTSSamlForm.TokenType.Saml;
				return;
			}
			base.TokenConsumer = config.siteName;
			this.LogonUri = config.MsoHttpPostLogin;
			this.uriFormat = "{0}?wctx=wp%3d{1}%26wa%3Dwsignin1.0%26wreply%3dhttps:%2F%2F{2}%2Fowa%2F";
			this.liveTokenType = LiveIdSTSSamlForm.TokenType.Compact;
		}

		public override string StsTag
		{
			get
			{
				return "SAMLPOST";
			}
		}

		public override IAsyncResult StartRequestChain(string userId, byte[] samlToken, AsyncCallback callback, object state)
		{
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.traceId, "Entering LiveIdSTSSamlForm.StartRequestChain()");
			this.stopwatch = Stopwatch.StartNew();
			this.traceUserName = userId;
			this.liveSamlPostSrf = string.Format(this.uriFormat, this.LogonUri, base.ProfilePolicy, base.TokenConsumer);
			string stringToEscape = Convert.ToBase64String(samlToken);
			string arg = Uri.EscapeDataString(stringToEscape);
			string s = string.Format("SAMLResponse={0}&RelayState=blablablaa", arg);
			this.requestBytes = Encoding.UTF8.GetBytes(s);
			ExTraceGlobals.AuthenticationTracer.TraceDebug<string>((long)this.traceId, "Constructing WebRequest uri='{0}'", this.liveSamlPostSrf);
			this.request = AuthServiceHelper.CreateHttpWebRequest(this.liveSamlPostSrf);
			if (AuthServiceStaticConfig.Config.MsoSSLEndpointType != MsoEndpointType.OLD)
			{
				AuthServiceHelper.UpdateConnectionSettingsInRequest(ref this.request, base.ConnectionGroupName);
			}
			this.request.ContentType = "application/x-www-form-urlencoded";
			this.request.ContentLength = (long)this.requestBytes.Length;
			if (base.ExtraHeaders != null)
			{
				this.request.Headers.Add(base.ExtraHeaders);
			}
			if (base.Instance == LiveIdInstanceType.Consumer)
			{
				Interlocked.Increment(ref LiveIdSTSBase.numberOfLiveIdRequests);
				STSBase.counters.NumberOfLiveIdStsRequests.RawValue = (long)LiveIdSTSBase.numberOfLiveIdRequests;
			}
			else
			{
				Interlocked.Increment(ref LiveIdSTSBase.numberOfMsoIdRequests);
				STSBase.counters.NumberOfMsoIdStsRequests.RawValue = (long)LiveIdSTSBase.numberOfMsoIdRequests;
			}
			if (ExTraceGlobals.AuthenticationTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				base.LogRequest(this.requestBytes, this.request, null);
			}
			IAsyncResult result = this.request.BeginGetRequestStream(callback, state);
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.traceId, "Leaving LiveIdSTSSamlForm.StartRequestChain()");
			return result;
		}

		public override IAsyncResult ProcessRequest(IAsyncResult asyncResult, AsyncCallback callback, object state)
		{
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.traceId, "Entering LiveIdSTSSamlForm.ProcessRequest()");
			Stream stream = this.request.EndGetRequestStream(asyncResult);
			STSBase.WriteBytes(stream, this.requestBytes);
			stream.Close();
			IAsyncResult result = this.request.BeginGetResponse(callback, state);
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.traceId, "Leaving LiveIdSTSSamlForm.ProcessRequest()");
			return result;
		}

		public override string ProcessResponse(IAsyncResult asyncResult)
		{
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.traceId, "Entering LiveIdSTSSamlForm.ProcessResponse()");
			string result = string.Empty;
			WebResponse webResponse = null;
			bool closeConnectionGroup = false;
			try
			{
				try
				{
					webResponse = this.request.EndGetResponse(asyncResult);
				}
				catch (WebException ex)
				{
					base.ErrorString += ex.ToString();
					string str;
					if (ex.Status == WebExceptionStatus.TrustFailure && AuthService.CertErrorCache.TryGetValue(this.request, out str))
					{
						base.ErrorString += str;
						AuthService.CertErrorCache.Remove(this.request);
					}
					if (ex.Status != WebExceptionStatus.ProtocolError)
					{
						closeConnectionGroup = true;
						throw;
					}
					HttpStatusCode statusCode = ((HttpWebResponse)ex.Response).StatusCode;
					if (statusCode != HttpStatusCode.Found && statusCode != HttpStatusCode.BadRequest)
					{
						closeConnectionGroup = true;
						throw;
					}
					webResponse = ex.Response;
				}
				finally
				{
					this.stopwatch.Stop();
					base.Latency = this.stopwatch.ElapsedMilliseconds;
					bool flag = AuthServiceHelper.CloseConnectionGroupIfNeeded(closeConnectionGroup, this.liveSamlPostSrf, base.ConnectionGroupName, this.traceId);
					if (flag)
					{
						base.ErrorString += "<ConnectionGroupClosed>";
					}
				}
				if (webResponse != null && webResponse.Headers != null)
				{
					base.LiveServer = webResponse.Headers.Get("PPServer");
				}
				if (base.Instance == LiveIdInstanceType.Consumer)
				{
					STSBase.counters.AverageLiveIdResponseTime.IncrementBy(this.stopwatch.ElapsedMilliseconds);
					STSBase.counters.AverageLiveIdResponseTimeBase.Increment();
				}
				else
				{
					STSBase.counters.AverageMsoIdResponseTime.IncrementBy(this.stopwatch.ElapsedMilliseconds);
					STSBase.counters.AverageMsoIdResponseTimeBase.Increment();
				}
				ExTraceGlobals.AuthenticationTracer.TracePerformance<long>((long)this.traceId, "LiveID Login HTTP-POST responded in {0}ms", this.stopwatch.ElapsedMilliseconds);
				result = this.ParseResponse(webResponse);
			}
			finally
			{
				if (webResponse != null)
				{
					((IDisposable)webResponse).Dispose();
				}
			}
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.traceId, "Leaving LiveIdSTSSamlForm.ProcessResponse()");
			return result;
		}

		public override void Abort()
		{
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.traceId, "Entering LiveIdSTSSamlForm.Abort()");
			if (this.request != null)
			{
				ExTraceGlobals.AuthenticationTracer.Information<string, string>((long)this.traceId, "Aborting http logon request to Live ID STS '{0}' for '{1}'", this.LogonUri, this.traceUserName);
				this.request.Abort();
			}
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.traceId, "Leaving LiveIdSTSSamlForm.Abort()");
		}

		public override string LiveToken()
		{
			return this.token;
		}

		private string ParseResponse(WebResponse response)
		{
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.traceId, "Entering LiveIdSTSSamlForm.ParseResponse()");
			HttpWebResponse httpWebResponse = response as HttpWebResponse;
			if (httpWebResponse == null)
			{
				ExTraceGlobals.AuthenticationTracer.TraceError<string>((long)this.traceId, "LiveID STS response is not an HttpWebResponse. {0}", (response == null) ? "<null>" : response.ToString());
				ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.traceId, "Leaving LiveIdSTSSamlForm.ParseResponse()");
				return string.Empty;
			}
			ExTraceGlobals.AuthenticationTracer.Information<string, HttpStatusCode, string>((long)this.traceId, "LiveID STS {0} responded with status {1:d} for user {2}", this.liveSamlPostSrf, httpWebResponse.StatusCode, this.traceUserName);
			string result = string.Empty;
			base.ErrorString = httpWebResponse.StatusCode.ToString();
			Stream responseStream = response.GetResponseStream();
			try
			{
				string text = string.Empty;
				string empty = string.Empty;
				string empty2 = string.Empty;
				string empty3 = string.Empty;
				using (StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8))
				{
					string text2 = streamReader.ReadToEnd();
					Match match = LiveIdSTSSamlForm.hrErrorCodeRegex.Match(text2);
					if (match.Success)
					{
						base.ErrorString += "&";
						base.ErrorString += match.Value.Trim();
					}
					ExTraceGlobals.AuthenticationTracer.TraceDebug<string>((long)this.traceId, "LiveID STS returned response {0}", text2);
					string query = response.ResponseUri.Query;
					if (!string.IsNullOrEmpty(query) && query.StartsWith("?", StringComparison.OrdinalIgnoreCase))
					{
						ExTraceGlobals.AuthenticationTracer.TraceDebug<string, string>((long)this.traceId, "Looking for token on query URI string {0} for user {1}", query, this.traceUserName);
						string query2 = query.Substring(1);
						NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(query2);
						string text3 = nameValueCollection["t"];
						string text4 = nameValueCollection["f"];
						string text5 = nameValueCollection["MSPPError"];
						if (!string.IsNullOrEmpty(text5))
						{
							int num;
							if (int.TryParse(text5, out num))
							{
								base.ErrorString = string.Format("Live returned f={0} MSPPError=0x{1:x}", text4, num);
							}
							else
							{
								base.ErrorString = string.Format("Live returned f={0} MSPPError={1}", text4, text5);
							}
							STSBase.eventLogger.LogEvent(SecurityEventLogConstants.Tuple_LiveIdLoginPostError, text5, new object[]
							{
								this.liveSamlPostSrf,
								text4,
								text5,
								(int)httpWebResponse.StatusCode,
								text2,
								this.traceUserName
							});
							ExTraceGlobals.AuthenticationTracer.TraceWarning<string, string, string>((long)this.traceId, "{0} for user '{1}' body={2}", this.traceUserName, base.ErrorString, text2);
							return string.Empty;
						}
						Func<string, string> func = (string s) => s.Replace('!', '+').Replace('*', '/').Replace('$', '=');
						if (!string.IsNullOrEmpty(text3))
						{
							text = "t=" + func(text3);
							ExTraceGlobals.AuthenticationTracer.Information<string>((long)this.traceId, "Using token from Query string for {0}", this.traceUserName);
						}
					}
					if (string.IsNullOrEmpty(text))
					{
						if (this.liveTokenType == LiveIdSTSSamlForm.TokenType.Saml)
						{
							ExTraceGlobals.AuthenticationTracer.TraceDebug<string>((long)this.traceId, "Looking for SAML 1.0 Assertion token in Live logon http response for user {0}", this.traceUserName);
							Match match2 = LiveIdSTSSamlForm.samlAssertionRegex.Match(text2);
							if (match2.Success)
							{
								string s2 = match2.Groups[1].ToString();
								SafeXmlDocument safeXmlDocument = new SafeXmlDocument();
								safeXmlDocument.PreserveWhitespace = true;
								safeXmlDocument.LoadXml(HttpUtility.HtmlDecode(s2));
								XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(safeXmlDocument.NameTable);
								xmlNamespaceManager.AddNamespace("wst", "http://schemas.xmlsoap.org/ws/2005/02/trust");
								xmlNamespaceManager.AddNamespace("saml", "urn:oasis:names:tc:SAML:1.0:assertion");
								XmlNode xmlNode = safeXmlDocument.SelectSingleNode("/wst:RequestSecurityTokenResponse/wst:RequestedSecurityToken/saml:Assertion", xmlNamespaceManager);
								if (xmlNode != null)
								{
									this.token = LiveIdSTSBase.UsAsciiEncodedXml(xmlNode);
									ExTraceGlobals.AuthenticationTracer.TraceDebug<string, string>((long)this.traceId, "Succesfully pulled SAML assertion from Live logon http response for user {0} : {1}", this.traceUserName, this.token);
									result = "0000000000000000";
								}
							}
							else
							{
								ExTraceGlobals.AuthenticationTracer.TraceWarning<string>((long)this.traceId, "LiveID response is missing hidden input field for ticket for {0}", this.traceUserName);
							}
							ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.traceId, "Leaving LiveIdSTSSamlForm.ParseResponse()");
							return result;
						}
						ExTraceGlobals.AuthenticationTracer.TraceDebug<string>((long)this.traceId, "Looking for compact token in Live logon http response for user {0}", this.traceUserName);
						Match match3 = LiveIdSTSSamlForm.liveFormRegex.Match(text2);
						if (match3.Success)
						{
							text = string.Format("t={0}&p=", match3.Groups[1].Value.Trim());
							ExTraceGlobals.AuthenticationTracer.Information<string>((long)this.traceId, "Using token from http response for {0}", this.traceUserName);
						}
						else
						{
							ExTraceGlobals.AuthenticationTracer.TraceWarning<string>((long)this.traceId, "LiveID response is missing hidden input field for ticket for {0}", this.traceUserName);
						}
					}
				}
				if (string.IsNullOrEmpty(text))
				{
					ExTraceGlobals.AuthenticationTracer.TraceWarning<string>((long)this.traceId, "Cannot parse compact ticket from LiveID Post response for user {0}", this.traceUserName);
					return null;
				}
				this.token = text;
				result = this.ParseCompactTicket(text);
			}
			finally
			{
				responseStream.Close();
				responseStream.Dispose();
			}
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.traceId, "Leaving LiveIdSTSSamlForm.ParseResponse()");
			return result;
		}

		private string ParseCompactTicket(string ticket)
		{
			if (string.IsNullOrEmpty(ticket))
			{
				return null;
			}
			string result;
			using (RPSPropBag rpspropBag = new RPSPropBag(LiveIdSTSBase.RpsSession))
			{
				using (RPSAuth rpsauth = new RPSAuth(LiveIdSTSBase.RpsSession))
				{
					using (RPSTicket rpsticket = rpsauth.Authenticate(LiveIdSTSBase.SiteName, ticket, 2U, rpspropBag))
					{
						using (RPSPropBag rpspropBag2 = new RPSPropBag(LiveIdSTSBase.RpsSession))
						{
							if (!rpsticket.Validate(rpspropBag2))
							{
								int num = (int)rpspropBag2["reasonhr"];
								base.ErrorString = string.Format("Live token failed validation for user {0} with error {1}", this.traceUserName, num);
								ExTraceGlobals.AuthenticationTracer.TraceWarning((long)this.traceId, base.ErrorString);
								return null;
							}
						}
						string arg = (string)rpsticket.Property["memberName"];
						string text = (string)rpsticket.Property["hexPuid"];
						ExTraceGlobals.AuthenticationTracer.Information<string, string, string>((long)this.traceId, "Live SAML Post succeeded with Live member name {0} and PUID {1} for user {2}", arg, text, this.traceUserName);
						result = text;
					}
				}
			}
			return result;
		}

		private static readonly Regex samlAssertionRegex = new Regex("<input [^>]*?type=\"hidden\"[^>]+?name=\"wresult\"[^>]+?value=\"(.+?)\"", RegexOptions.Compiled | RegexOptions.Singleline);

		private static readonly Regex liveFormRegex = new Regex("<input [^>]*?type=\"hidden\"[^>]+?name=\"t\"[^>]+?value=\"(.+?)\"", RegexOptions.Compiled);

		private static readonly Regex hrErrorCodeRegex = new Regex(" HR=[0-9a-fA-F]+ ", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		private string uriFormat;

		private LiveIdSTSSamlForm.TokenType liveTokenType;

		private string liveSamlPostSrf;

		private byte[] requestBytes;

		private HttpWebRequest request;

		private Stopwatch stopwatch;

		private string traceUserName;

		private string token;

		private enum TokenType
		{
			Compact,
			Saml
		}
	}
}
