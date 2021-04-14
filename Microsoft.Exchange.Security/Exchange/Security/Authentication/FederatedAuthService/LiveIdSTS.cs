using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.XPath;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Security;

namespace Microsoft.Exchange.Security.Authentication.FederatedAuthService
{
	internal class LiveIdSTS : LiveIdSTSBase
	{
		private LiveIdSTS()
		{
		}

		internal LiveIdSTS(int traceId, LiveIdInstanceType instance, NamespaceStats stats) : base(traceId, instance, stats)
		{
		}

		public LiveIdSTS(int traceId, LiveIdInstanceType instance, AuthServiceStaticConfig config, NamespaceStats stats) : base(traceId, instance, stats)
		{
			if (config.defaultInstance == LiveIdInstanceType.Consumer)
			{
				base.TokenConsumer = config.siteName;
				this.LogonUri = config.liveRst2Login;
			}
			else if (instance == LiveIdInstanceType.Consumer)
			{
				base.ProfilePolicy = "LBI_FED_STS_CLEAR";
				base.TokenConsumer = config.msoSiteName;
				this.LogonUri = config.liveRst2Login;
			}
			else
			{
				base.TokenConsumer = config.siteName;
				this.LogonUri = config.MsoRst2LoginUrl;
			}
			this.reqStatusForResponseDump = config.ReqStatusForResponseDump;
			this.authStatusForResponseDump = config.AuthStatusForResponseDump;
			base.EnableRemoteRPS = config.EnableRemoteRPSForCompactTicket;
		}

		public LiveIdSTS(int traceId, LiveIdInstanceType instance, AuthServiceStaticConfig config, NamespaceStats stats, bool offlineAuthEnabled) : this(traceId, instance, config, stats)
		{
			this.offlineAuthEnabled = offlineAuthEnabled;
		}

		public override string LogonUri
		{
			get
			{
				return this.uri;
			}
			set
			{
				this.uri = value;
				this.uriBytes = Encoding.UTF8.GetBytes(value);
			}
		}

		public override string StsTag
		{
			get
			{
				return "RST2";
			}
		}

		public override IAsyncResult StartRequestChain(string userId, byte[] wsseToken, AsyncCallback callback, object state)
		{
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.traceId, "LiveIdSTS.Entering StartRequestChain()");
			this.stopwatch = Stopwatch.StartNew();
			int num = this.uriBytes.Length + this.tokenConsumerBytes.Length + base.ProfilePolicyBytes.Length + wsseToken.Length;
			this.traceUserName = userId;
			this.wsseToken = wsseToken;
			ExTraceGlobals.AuthenticationTracer.TraceDebug<string, string>((long)this.traceId, "Constructing WebRequest uri='{0}' TokenConsumer='{1}'", this.LogonUri, base.TokenConsumer);
			this.request = AuthServiceHelper.CreateHttpWebRequest(this.LogonUri);
			if (AuthServiceStaticConfig.Config.MsoSSLEndpointType != MsoEndpointType.OLD)
			{
				AuthServiceHelper.UpdateConnectionSettingsInRequest(ref this.request, base.ConnectionGroupName);
			}
			this.request.ContentType = "application/x-www-form-urlencoded";
			this.request.ContentLength = (long)(LiveIdSTS.constantByteCount + num + WsuTimestamp.EncodedByteCount);
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
				AuthService.orgIdRequestCountLastMinutes.AddValue(1L);
			}
			IAsyncResult result = this.request.BeginGetRequestStream(callback, state);
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.traceId, "Leaving LiveIdSTS.StartRequestChain()");
			return result;
		}

		public override IAsyncResult ProcessRequest(IAsyncResult asyncResult, AsyncCallback callback, object state)
		{
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.traceId, "Entering LiveIdSTS.ProcessRequest()");
			Stream stream = this.request.EndGetRequestStream(asyncResult);
			base.SSLConnectionLatency = this.stopwatch.ElapsedMilliseconds;
			STSBase.WriteBytes(stream, LiveIdSTS.requestBytesP1);
			STSBase.WriteBytes(stream, this.uriBytes);
			STSBase.WriteBytes(stream, LiveIdSTS.requestBytesP2);
			STSBase.WriteBytes(stream, this.wsseToken);
			WsuTimestamp.WriteTimestamp(DateTime.UtcNow, stream);
			STSBase.WriteBytes(stream, LiveIdSTS.requestBytesP3);
			STSBase.WriteBytes(stream, this.tokenConsumerBytes);
			STSBase.WriteBytes(stream, LiveIdSTS.requestBytesP4);
			STSBase.WriteBytes(stream, base.ProfilePolicyBytes);
			STSBase.WriteBytes(stream, LiveIdSTS.requestBytesP5);
			stream.Close();
			if (ExTraceGlobals.AuthenticationTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					STSBase.WriteBytes(memoryStream, LiveIdSTS.requestBytesP1);
					STSBase.WriteBytes(memoryStream, this.uriBytes);
					STSBase.WriteBytes(memoryStream, LiveIdSTS.requestBytesP2);
					STSBase.WriteBytes(memoryStream, Encoding.UTF8.GetBytes("DummyPwd"));
					WsuTimestamp.WriteTimestamp(DateTime.UtcNow, memoryStream);
					STSBase.WriteBytes(memoryStream, LiveIdSTS.requestBytesP3);
					STSBase.WriteBytes(memoryStream, this.tokenConsumerBytes);
					STSBase.WriteBytes(memoryStream, LiveIdSTS.requestBytesP4);
					STSBase.WriteBytes(memoryStream, base.ProfilePolicyBytes);
					STSBase.WriteBytes(memoryStream, LiveIdSTS.requestBytesP5);
					memoryStream.Flush();
					byte[] buffer = memoryStream.GetBuffer();
					base.LogRequest(buffer, this.request, "DummyPwd");
				}
			}
			IAsyncResult result = this.request.BeginGetResponse(callback, state);
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.traceId, "Leaving LiveIdSTS.ProcessRequest()");
			return result;
		}

		public override string ProcessResponse(IAsyncResult asyncResult)
		{
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.traceId, "Entering LiveIdSTS.ProcessResponse()");
			string result = string.Empty;
			bool closeConnectionGroup = false;
			try
			{
				using (WebResponse webResponse = this.request.EndGetResponse(asyncResult))
				{
					this.stopwatch.Stop();
					base.Latency = this.stopwatch.ElapsedMilliseconds;
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
					ExTraceGlobals.AuthenticationTracer.TracePerformance<long>((long)this.traceId, "LiveID STS responded in {0}ms", this.stopwatch.ElapsedMilliseconds);
					if (webResponse != null && webResponse.Headers != null)
					{
						base.LiveServer = webResponse.Headers.Get("PPServer");
					}
					result = this.ParseResponse(webResponse);
				}
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
				closeConnectionGroup = true;
				throw;
			}
			finally
			{
				bool flag = AuthServiceHelper.CloseConnectionGroupIfNeeded(closeConnectionGroup, this.LogonUri, base.ConnectionGroupName, this.traceId);
				if (flag)
				{
					base.ErrorString += "<ConnectionGroupClosed>";
				}
			}
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.traceId, "Leaving LiveIdSTS.ProcessResponse()");
			return result;
		}

		public override void Abort()
		{
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.traceId, "Entering LiveIdSTS.Abort()");
			if (this.request != null)
			{
				ExTraceGlobals.AuthenticationTracer.Information<string, string>((long)this.traceId, "Aborting http logon request to Live ID STS '{0}' for '{1}'", this.LogonUri, this.traceUserName);
				this.request.Abort();
			}
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.traceId, "Leaving LiveIdSTS.Abort()");
		}

		public override string LiveToken()
		{
			return this.ExtractTokenFromXmlResponse(this.xmlResponse, this.nsManager, this.xmlRawResponse);
		}

		private string ParseResponse(WebResponse response)
		{
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.traceId, "Entering LiveIdSTS.ParseResponse()");
			HttpWebResponse httpWebResponse = response as HttpWebResponse;
			if (httpWebResponse == null)
			{
				ExTraceGlobals.AuthenticationTracer.TraceError<string>((long)this.traceId, "LiveID STS response is not an HttpWebResponse. {0}", (response == null) ? "<null>" : response.ToString());
				ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.traceId, "Leaving LiveIdSTS.ParseResponse()");
				return string.Empty;
			}
			ExTraceGlobals.AuthenticationTracer.Information<string, HttpStatusCode>((long)this.traceId, "LiveID STS {0} responded with status {1:d}", this.LogonUri, httpWebResponse.StatusCode);
			string text = string.Empty;
			base.ErrorString = string.Empty;
			if (httpWebResponse.StatusCode != HttpStatusCode.OK)
			{
				base.ErrorString = httpWebResponse.StatusCode.ToString();
			}
			if (httpWebResponse.StatusCode == HttpStatusCode.OK)
			{
				Stream responseStream = response.GetResponseStream();
				try
				{
					string text2 = string.Empty;
					string reqStatus = string.Empty;
					string text3 = string.Empty;
					string text4 = string.Empty;
					using (StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8))
					{
						this.xmlRawResponse = streamReader.ReadToEnd();
						ExTraceGlobals.AuthenticationTracer.TraceDebug<string>((long)this.traceId, "LiveID STS returned response {0}", this.xmlRawResponse);
						this.xmlResponse = new SafeXmlDocument();
						this.xmlResponse.PreserveWhitespace = true;
						try
						{
							this.xmlResponse.LoadXml(this.xmlRawResponse);
						}
						catch (XmlException ex)
						{
							ExTraceGlobals.AuthenticationTracer.TraceError<string, string>((long)this.traceId, "LiveID STS has malformed RST response.  Exception {0} XML response {1}", ex.Message, this.xmlRawResponse);
							throw;
						}
						try
						{
							this.nsManager = new XmlNamespaceManager(this.xmlResponse.NameTable);
							this.nsManager.AddNamespace("psf", "http://schemas.microsoft.com/Passport/SoapServices/SOAPFault");
							this.nsManager.AddNamespace("S", "http://www.w3.org/2003/05/soap-envelope");
							this.nsManager.AddNamespace("wst", "http://schemas.xmlsoap.org/ws/2005/02/trust");
							this.nsManager.AddNamespace("wsp", "http://schemas.xmlsoap.org/ws/2004/09/policy");
							this.nsManager.AddNamespace("wsa", "http://www.w3.org/2005/08/addressing");
							this.nsManager.AddNamespace("wsse", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");
							this.nsManager.AddNamespace("saml", "urn:oasis:names:tc:SAML:1.0:assertion");
							XmlNode xmlNode = this.xmlResponse.SelectSingleNode("/S:Envelope/S:Header/psf:pp/psf:PUID/text()", this.nsManager);
							if (xmlNode != null)
							{
								text = xmlNode.Value.ToString().Trim();
								ExTraceGlobals.AuthenticationTracer.Information<string>((long)this.traceId, "LiveID STS logon succeeded, response has PUID {0}", text);
							}
							else
							{
								ExTraceGlobals.AuthenticationTracer.Information((long)this.traceId, "LiveID STS logon failed.  Response is missing <PUID> node");
								xmlNode = this.xmlResponse.SelectSingleNode("/S:Envelope/S:Body/S:Fault", this.nsManager);
								if (xmlNode != null)
								{
									text3 = xmlNode.OuterXml;
									ExTraceGlobals.AuthenticationTracer.Information<string>((long)this.traceId, "LiveIDSTS returned fault:{0}", text3);
									XmlNode xmlNode2 = this.xmlResponse.SelectSingleNode("/S:Envelope/S:Body/S:Fault/S:Detail/psf:error/psf:internalerror/psf:code/text()", this.nsManager);
									if (xmlNode2 != null && xmlNode2.Value != null)
									{
										text4 = xmlNode2.Value.ToString().Trim();
									}
								}
							}
							xmlNode = this.xmlResponse.SelectSingleNode("/S:Envelope/S:Header/psf:pp/psf:authstate/text()", this.nsManager);
							if (xmlNode != null)
							{
								text2 = xmlNode.Value.ToString().Trim();
							}
							xmlNode = this.xmlResponse.SelectSingleNode("/S:Envelope/S:Header/psf:pp/psf:reqstatus/text()", this.nsManager);
							if (xmlNode != null)
							{
								reqStatus = xmlNode.Value.ToString().Trim();
							}
							else
							{
								base.ErrorString = "missing request status";
							}
							if (string.IsNullOrEmpty(base.LiveServer))
							{
								xmlNode = this.xmlResponse.SelectSingleNode("/S:Envelope/S:Header/psf:pp/psf:serverInfo/text()", this.nsManager);
								if (xmlNode != null)
								{
									base.LiveServer = xmlNode.Value.ToString().Trim();
								}
							}
						}
						catch (XPathException ex2)
						{
							base.ErrorString = string.Format("LiveID STS has malformed RST response. Exception {0} XML response {1}", ex2.ToString(), this.xmlRawResponse);
							ExTraceGlobals.AuthenticationTracer.TraceError((long)this.traceId, base.ErrorString);
							ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.traceId, "Leaving LiveIdSTS.ParseResponse()");
							return string.Empty;
						}
					}
					if (string.IsNullOrEmpty(text))
					{
						ExTraceGlobals.AuthenticationTracer.Information<string, string>((long)this.traceId, "LiveID STS logon failure response has authState {0} and reqStatus {1}", text2, reqStatus);
						if (reqStatus.Equals("0x80048821", StringComparison.OrdinalIgnoreCase))
						{
							base.IsBadCredentials = true;
							Interlocked.Increment(ref this.namespaceStats.BadPassword);
							this.namespaceStats.User = this.traceUserName;
						}
						else if (reqStatus.Equals("0x80048831", StringComparison.OrdinalIgnoreCase) || reqStatus.Equals("0x80048827", StringComparison.OrdinalIgnoreCase))
						{
							base.IsExpiredCreds = true;
						}
						else if (reqStatus.Equals("0x800434d4", StringComparison.OrdinalIgnoreCase) || text4.Equals("0x800434d4", StringComparison.OrdinalIgnoreCase))
						{
							base.AppPasswordRequired = true;
							base.RecoveryUrl = AuthServiceStaticConfig.Config.AppPasswordHelpUrl;
						}
						else if (reqStatus.Equals("0x800478ac", StringComparison.OrdinalIgnoreCase) || text4.Equals("0x800478ac", StringComparison.OrdinalIgnoreCase))
						{
							base.IsAccountNotProvisioned = true;
						}
						if (base.IsBadCredentials || string.IsNullOrEmpty(text3))
						{
							base.ErrorString = reqStatus;
						}
						else
						{
							base.ErrorString = text3;
						}
						if (!string.IsNullOrEmpty(text2) && text2.Equals(this.authStatusForResponseDump, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(reqStatus) && reqStatus.Equals(this.reqStatusForResponseDump, StringComparison.OrdinalIgnoreCase))
						{
							base.ErrorString = this.xmlRawResponse;
						}
						if (!Array.Exists<string>(LiveIdSTS.ignorableErrors, (string val) => string.Equals(reqStatus, val, StringComparison.OrdinalIgnoreCase)))
						{
							STSBase.eventLogger.LogEvent(SecurityEventLogConstants.Tuple_LiveIdServerError, this.traceUserName + reqStatus, new object[]
							{
								this.LogonUri,
								base.LiveServer,
								text2,
								reqStatus,
								text3,
								this.traceUserName
							});
						}
						if (this.offlineAuthEnabled && !base.IsBadCredentials && !base.IsExpiredCreds && !base.IsAccountNotProvisioned && !this.UserRecoveryPossible())
						{
							throw new WebException(string.Format("{0} returns ReqStatus:{1}, AuthState:{2} for user {3}. ", new object[]
							{
								this.LogonUri,
								reqStatus,
								text2,
								this.traceUserName
							}));
						}
					}
					else
					{
						ExTraceGlobals.AuthenticationTracer.TraceDebug<string, string>((long)this.traceId, "LiveID STS logon success response has authState {0} and reqStatus {1}", text2, reqStatus);
					}
				}
				finally
				{
					responseStream.Close();
					responseStream.Dispose();
				}
			}
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.traceId, "Leaving LiveIdSTS.ParseResponse()");
			return text;
		}

		private string ExtractTokenFromXmlResponse(SafeXmlDocument xmlResponseDocument, XmlNamespaceManager xmlNamespaceManager, string rawXml)
		{
			XmlNode xmlNode = xmlResponseDocument.SelectSingleNode("/S:Envelope/S:Body/wst:RequestSecurityTokenResponseCollection", this.nsManager);
			if (xmlNode == null)
			{
				base.ErrorString = string.Format("LiveID STS has malformed RST response (wst:RequestSecurityTokenResponseCollection node missing): {0}.", xmlResponseDocument.InnerText);
				ExTraceGlobals.AuthenticationTracer.TraceError((long)this.traceId, base.ErrorString);
				return null;
			}
			foreach (object obj in xmlNode.ChildNodes)
			{
				XmlNode xmlNode2 = (XmlNode)obj;
				XmlNode xmlNode3 = xmlNode2.SelectSingleNode("wsp:AppliesTo/wsa:EndpointReference/wsa:Address/text()", xmlNamespaceManager);
				if (xmlNode3 == null)
				{
					ExTraceGlobals.AuthenticationTracer.TraceWarning<string>((long)this.traceId, "LiveID STS has malformed RST response (wsa:Address node missing): {0}.", xmlResponseDocument.InnerText);
				}
				else
				{
					string text = xmlNode3.Value.Trim();
					if (text.Equals(base.TokenConsumer, StringComparison.OrdinalIgnoreCase))
					{
						XmlNode xmlNode4 = xmlNode2.SelectSingleNode("wst:RequestedSecurityToken/wsse:BinarySecurityToken", xmlNamespaceManager);
						if (xmlNode4 != null)
						{
							return xmlNode4.InnerXml;
						}
						xmlNode4 = xmlNode2.SelectSingleNode("wst:RequestedSecurityToken/saml:Assertion", xmlNamespaceManager);
						if (xmlNode4 != null)
						{
							return LiveIdSTSBase.UsAsciiEncodedXml(xmlNode4);
						}
						XmlNode xmlNode5 = xmlNode2.SelectSingleNode("psf:pp", xmlNamespaceManager);
						string text2 = string.Empty;
						string text3 = string.Empty;
						if (xmlNode5 != null)
						{
							XmlNode xmlNode6 = xmlNode5.SelectSingleNode("psf:reqstatus/text()", xmlNamespaceManager);
							if (xmlNode6 != null)
							{
								text2 = xmlNode6.Value;
							}
							XmlNode xmlNode7 = xmlNode5.SelectSingleNode("psf:errorstatus/text()", xmlNamespaceManager);
							if (xmlNode7 != null)
							{
								text3 = xmlNode7.Value;
							}
							XmlNode xmlNode8 = xmlNode5.SelectSingleNode("psf:flowurl/text()", xmlNamespaceManager);
							if (xmlNode8 != null)
							{
								base.RecoveryUrl = xmlNode8.Value;
							}
						}
						base.ErrorString = string.Format("LiveID STS RST response does not contain the token for {0}. ReqStatus: {1}, ErrorStatus {2}\n Auxiliary infromation: {3}.", new object[]
						{
							base.TokenConsumer,
							text2,
							text3,
							(xmlNode5 == null) ? "" : xmlNode5.InnerText
						});
						ExTraceGlobals.AuthenticationTracer.TraceError((long)this.traceId, base.ErrorString);
						return null;
					}
				}
			}
			base.ErrorString = string.Format("LiveID STS has malformed RST response - no token for site {0}: {1}.", base.TokenConsumer, xmlResponseDocument.InnerText);
			ExTraceGlobals.AuthenticationTracer.TraceWarning((long)this.traceId, base.ErrorString);
			return null;
		}

		public override bool UserRecoveryPossible()
		{
			bool result = false;
			if (!string.IsNullOrEmpty(base.RecoveryUrl))
			{
				result = true;
			}
			else if (this.xmlResponse != null)
			{
				XmlNode xmlNode = this.xmlResponse.SelectSingleNode("/S:Envelope/S:Header/psf:pp/psf:reqstatus/text()", this.nsManager);
				if (xmlNode != null)
				{
					string status = xmlNode.Value.ToString().Trim();
					if (Array.Exists<string>(LiveIdSTS.recoverableErrors, (string val) => string.Equals(status, val, StringComparison.OrdinalIgnoreCase)))
					{
						result = true;
					}
				}
			}
			return result;
		}

		private const string xmlTemplateP1 = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><s:Envelope xmlns:s=\"http://www.w3.org/2003/05/soap-envelope\" xmlns:wsse=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd\" xmlns:saml=\"urn:oasis:names:tc:SAML:1.0:assertion\" xmlns:wsp=\"http://schemas.xmlsoap.org/ws/2004/09/policy\" xmlns:wsu=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd\" xmlns:wsa=\"http://www.w3.org/2005/08/addressing\" xmlns:wssc=\"http://schemas.xmlsoap.org/ws/2005/02/sc\" xmlns:wst=\"http://schemas.xmlsoap.org/ws/2005/02/trust\"><s:Header><wsa:Action s:mustUnderstand=\"1\">http://schemas.xmlsoap.org/ws/2005/02/trust/RST/Issue</wsa:Action><wsa:To s:mustUnderstand=\"1\">";

		private const string xmlTemplateP2 = "</wsa:To><wsa:MessageID>1178732664</wsa:MessageID><ps:AuthInfo xmlns:ps=\"http://schemas.microsoft.com/Passport/SoapServices/PPCRL\" Id=\"PPAuthInfo\"><ps:HostingApp>{01324f02-f998-4cca-9f2f-08f858cf8977}</ps:HostingApp><ps:BinaryVersion>6</ps:BinaryVersion><ps:UIVersion>1</ps:UIVersion><ps:Cookies></ps:Cookies><ps:RequestParams>AQAAAAIAAABsYwQAAAAxMDMz</ps:RequestParams></ps:AuthInfo><wsse:Security>";

		private const string xmlTemplateP3 = "</wsse:Security></s:Header><s:Body><ps:RequestMultipleSecurityTokens xmlns:ps=\"http://schemas.microsoft.com/Passport/SoapServices/PPCRL\" Id=\"RSTS\"><wst:RequestSecurityToken Id=\"RST0\"><wst:RequestType>http://schemas.xmlsoap.org/ws/2005/02/trust/Issue</wst:RequestType><wsp:AppliesTo><wsa:EndpointReference><wsa:Address>http://Passport.NET/tb</wsa:Address></wsa:EndpointReference></wsp:AppliesTo></wst:RequestSecurityToken><wst:RequestSecurityToken Id=\"RST1\"><wst:RequestType>http://schemas.xmlsoap.org/ws/2005/02/trust/Issue</wst:RequestType><wsp:AppliesTo><wsa:EndpointReference><wsa:Address>";

		private const string xmlTemplateP4 = "</wsa:Address></wsa:EndpointReference></wsp:AppliesTo><wsp:PolicyReference URI=\"";

		private const string xmlTemplateP5 = "\"></wsp:PolicyReference></wst:RequestSecurityToken></ps:RequestMultipleSecurityTokens></s:Body></s:Envelope>";

		private const string RequireAppPassword = "0x800434d4";

		private const string AccountNotProvisioned = "0x800478ac";

		private static readonly string[] ignorableErrors = new string[]
		{
			"0x80048821",
			"0x80048823",
			"0x80048824",
			"0x80048826",
			"0x80048827",
			"0x8004882d",
			"0x80048830",
			"0x80048831",
			"0x800434d4"
		};

		private static readonly string[] recoverableErrors = new string[]
		{
			"0x80048823",
			"0x80048824",
			"0x80048826",
			"0x80048827",
			"0x80048830",
			"0x80048831",
			"0x800434d4"
		};

		private static readonly byte[] requestBytesP1 = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?><s:Envelope xmlns:s=\"http://www.w3.org/2003/05/soap-envelope\" xmlns:wsse=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd\" xmlns:saml=\"urn:oasis:names:tc:SAML:1.0:assertion\" xmlns:wsp=\"http://schemas.xmlsoap.org/ws/2004/09/policy\" xmlns:wsu=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd\" xmlns:wsa=\"http://www.w3.org/2005/08/addressing\" xmlns:wssc=\"http://schemas.xmlsoap.org/ws/2005/02/sc\" xmlns:wst=\"http://schemas.xmlsoap.org/ws/2005/02/trust\"><s:Header><wsa:Action s:mustUnderstand=\"1\">http://schemas.xmlsoap.org/ws/2005/02/trust/RST/Issue</wsa:Action><wsa:To s:mustUnderstand=\"1\">");

		private static readonly byte[] requestBytesP2 = Encoding.UTF8.GetBytes("</wsa:To><wsa:MessageID>1178732664</wsa:MessageID><ps:AuthInfo xmlns:ps=\"http://schemas.microsoft.com/Passport/SoapServices/PPCRL\" Id=\"PPAuthInfo\"><ps:HostingApp>{01324f02-f998-4cca-9f2f-08f858cf8977}</ps:HostingApp><ps:BinaryVersion>6</ps:BinaryVersion><ps:UIVersion>1</ps:UIVersion><ps:Cookies></ps:Cookies><ps:RequestParams>AQAAAAIAAABsYwQAAAAxMDMz</ps:RequestParams></ps:AuthInfo><wsse:Security>");

		private static readonly byte[] requestBytesP3 = Encoding.UTF8.GetBytes("</wsse:Security></s:Header><s:Body><ps:RequestMultipleSecurityTokens xmlns:ps=\"http://schemas.microsoft.com/Passport/SoapServices/PPCRL\" Id=\"RSTS\"><wst:RequestSecurityToken Id=\"RST0\"><wst:RequestType>http://schemas.xmlsoap.org/ws/2005/02/trust/Issue</wst:RequestType><wsp:AppliesTo><wsa:EndpointReference><wsa:Address>http://Passport.NET/tb</wsa:Address></wsa:EndpointReference></wsp:AppliesTo></wst:RequestSecurityToken><wst:RequestSecurityToken Id=\"RST1\"><wst:RequestType>http://schemas.xmlsoap.org/ws/2005/02/trust/Issue</wst:RequestType><wsp:AppliesTo><wsa:EndpointReference><wsa:Address>");

		private static readonly byte[] requestBytesP4 = Encoding.UTF8.GetBytes("</wsa:Address></wsa:EndpointReference></wsp:AppliesTo><wsp:PolicyReference URI=\"");

		private static readonly byte[] requestBytesP5 = Encoding.UTF8.GetBytes("\"></wsp:PolicyReference></wst:RequestSecurityToken></ps:RequestMultipleSecurityTokens></s:Body></s:Envelope>");

		private static readonly int constantByteCount = LiveIdSTS.requestBytesP1.Length + LiveIdSTS.requestBytesP2.Length + LiveIdSTS.requestBytesP3.Length + LiveIdSTS.requestBytesP4.Length + LiveIdSTS.requestBytesP5.Length;

		private byte[] wsseToken;

		private HttpWebRequest request;

		private Stopwatch stopwatch;

		private string traceUserName;

		private SafeXmlDocument xmlResponse;

		private string xmlRawResponse;

		private XmlNamespaceManager nsManager;

		private readonly bool offlineAuthEnabled;

		private readonly string reqStatusForResponseDump;

		private readonly string authStatusForResponseDump;

		private string uri;

		private byte[] uriBytes;
	}
}
