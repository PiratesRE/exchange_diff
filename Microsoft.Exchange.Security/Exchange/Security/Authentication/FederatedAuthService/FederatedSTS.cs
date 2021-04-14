using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics.Components.Security;

namespace Microsoft.Exchange.Security.Authentication.FederatedAuthService
{
	internal class FederatedSTS : STSBase
	{
		public string FederatedLogonURI { get; set; }

		public string TokenIssuerURI { get; set; }

		public bool IsFederatedStsADFSRulesDenied { get; set; }

		public bool IsForbidden { get; set; }

		public int MaxResponseSize
		{
			get
			{
				if (this.maxResponseSize >= 1)
				{
					return this.maxResponseSize;
				}
				return 1;
			}
			set
			{
				this.maxResponseSize = value;
			}
		}

		private FederatedSTS()
		{
		}

		public FederatedSTS(int traceId, LiveIdInstanceType instance, NamespaceStats stats) : base(traceId, instance, stats)
		{
		}

		public override string StsTag
		{
			get
			{
				return "ADFS";
			}
		}

		public IAsyncResult StartRequestChain(byte[] userId, byte[] password, AsyncCallback callback, object state)
		{
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.traceId, "Entering FederatedSTS.StartRequestChain()");
			if (userId == null)
			{
				ExTraceGlobals.AuthenticationTracer.TraceError((long)this.traceId, "userId is null");
				throw new ArgumentNullException("userId");
			}
			if (password == null)
			{
				ExTraceGlobals.AuthenticationTracer.TraceError((long)this.traceId, "password is null");
				throw new ArgumentNullException("password");
			}
			Interlocked.Increment(ref FederatedSTS.numberOfOutgoingRequests);
			STSBase.counters.NumberOfOutgoingFedStsRequests.RawValue = (long)FederatedSTS.numberOfOutgoingRequests;
			this.remoteUserName = userId;
			this.remotePassword = password;
			this.traceUserName = Encoding.UTF8.GetString(this.remoteUserName);
			this.messageId = new UniqueId();
			ExTraceGlobals.AuthenticationTracer.Information<string, string>((long)this.traceId, "Creating http logon request to federated STS '{0}' for '{1}'", this.FederatedLogonURI, this.traceUserName);
			this.stopwatch = Stopwatch.StartNew();
			this.adfsRequest = AuthServiceHelper.CreateHttpWebRequest(this.FederatedLogonURI);
			this.adfsRequest.ContentType = "application/soap+xml; charset=utf-8";
			if (base.ExtraHeaders != null)
			{
				this.adfsRequest.Headers.Add(base.ExtraHeaders);
			}
			IAsyncResult result = this.adfsRequest.BeginGetRequestStream(callback, state);
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.traceId, "Leaving FederatedSTS.StartRequestChain()");
			return result;
		}

		public IAsyncResult ProcessRequest(IAsyncResult asyncResult, AsyncCallback callback, object state)
		{
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.traceId, "Entering FederatedSTS.ProcessRequest()");
			Stream stream = this.adfsRequest.EndGetRequestStream(asyncResult);
			STSBase.WriteBytes(stream, FederatedSTS.stsRequestBytesP1);
			STSBase.WriteBytes(stream, Encoding.UTF8.GetBytes(this.FederatedLogonURI));
			STSBase.WriteBytes(stream, FederatedSTS.stsRequestBytesP2);
			STSBase.WriteBytes(stream, Encoding.UTF8.GetBytes(this.messageId.ToString()));
			STSBase.WriteBytes(stream, FederatedSTS.stsRequestBytesP3);
			STSBase.WriteBytes(stream, this.remoteUserName);
			STSBase.WriteBytes(stream, FederatedSTS.stsRequestBytesP4);
			STSBase.WriteBytes(stream, this.remotePassword);
			STSBase.WriteBytes(stream, FederatedSTS.stsRequestBytesP5);
			STSBase.WriteBytes(stream, Encoding.UTF8.GetBytes((DateTime.UtcNow + base.ClockSkew).ToString("o")));
			STSBase.WriteBytes(stream, FederatedSTS.stsRequestBytesP6);
			STSBase.WriteBytes(stream, Encoding.UTF8.GetBytes((DateTime.UtcNow + base.ClockSkew).AddMinutes(5.0).ToString("o")));
			STSBase.WriteBytes(stream, FederatedSTS.stsRequestBytesP7);
			STSBase.WriteBytes(stream, Encoding.UTF8.GetBytes(this.TokenIssuerURI));
			STSBase.WriteBytes(stream, FederatedSTS.stsRequestBytesP8);
			stream.Close();
			IAsyncResult result = this.adfsRequest.BeginGetResponse(callback, state);
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.traceId, "Leaving FederatedSTS.ProcessRequest()");
			return result;
		}

		public byte[] ProcessResponse(IAsyncResult asyncResult)
		{
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.traceId, "Entering FederatedSTS.ProcessResponse()");
			ExTraceGlobals.AuthenticationTracer.Information<string, string>((long)this.traceId, "Processing response to http logon request to federated STS '{0}' for '{1}'", this.FederatedLogonURI, this.traceUserName);
			byte[] result = null;
			WebResponse webResponse = null;
			try
			{
				try
				{
					webResponse = this.adfsRequest.EndGetResponse(asyncResult);
				}
				catch (WebException ex)
				{
					if (ex.Status == WebExceptionStatus.ProtocolError && ((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.InternalServerError)
					{
						webResponse = ex.Response;
					}
					else if ((HttpWebResponse)ex.Response != null && ((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.Unauthorized)
					{
						base.IsBadCredentials = true;
						Interlocked.Increment(ref this.namespaceStats.BadPassword);
						base.ErrorString += ex.ToString();
					}
					else
					{
						if ((HttpWebResponse)ex.Response == null || ((HttpWebResponse)ex.Response).StatusCode != HttpStatusCode.Forbidden)
						{
							string str;
							if (ex.Status == WebExceptionStatus.TrustFailure && AuthService.CertErrorCache.TryGetValue(this.adfsRequest, out str))
							{
								base.ErrorString += str;
								AuthService.CertErrorCache.Remove(this.adfsRequest);
							}
							throw;
						}
						base.ErrorString += ex.ToString();
						this.IsForbidden = true;
					}
				}
				finally
				{
					this.stopwatch.Stop();
					base.Latency = this.stopwatch.ElapsedMilliseconds;
				}
				STSBase.counters.AverageFedStsResponseTime.IncrementBy(this.stopwatch.ElapsedMilliseconds);
				STSBase.counters.AverageFedStsResponseTimeBase.Increment();
				ExTraceGlobals.AuthenticationTracer.TracePerformance<string, long>((long)this.traceId, "Federated STS '{0}' responded in {1}ms", this.FederatedLogonURI, this.stopwatch.ElapsedMilliseconds);
				if (webResponse == null)
				{
					return result;
				}
				HttpWebResponse httpWebResponse = (HttpWebResponse)webResponse;
				TimeSpan clockSkew = base.ClockSkew;
				if (base.CalculateClockSkew(httpWebResponse))
				{
					ExTraceGlobals.AuthenticationTracer.TraceDebug<string, int>((long)this.traceId, "Clock skew between ADFS server {0} and local server is {1} seconds", this.FederatedLogonURI, base.ClockSkew.Seconds);
				}
				using (Stream responseStream = httpWebResponse.GetResponseStream())
				{
					using (StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8))
					{
						SafeXmlDocument safeXmlDocument = new SafeXmlDocument();
						safeXmlDocument.PreserveWhitespace = true;
						char[] array = new char[this.MaxResponseSize];
						int num = -1;
						int num2 = 0;
						while (!streamReader.EndOfStream && num2 < this.MaxResponseSize && num != 0)
						{
							num = streamReader.Read(array, num2, this.MaxResponseSize - num2);
							num2 += num;
						}
						if (!streamReader.EndOfStream)
						{
							Interlocked.Increment(ref this.namespaceStats.TokenSize);
							this.namespaceStats.User = this.traceUserName;
							ExTraceGlobals.AuthenticationTracer.TraceError<int>((long)this.traceId, "Federated STS returning more data than expected. More than {0} bytes", num2);
							base.ErrorString = "Federated STS returned too much data";
							return result;
						}
						string text = new string(array, 0, num2);
						ExTraceGlobals.AuthenticationTracer.TraceDebug<HttpStatusCode, string>((long)this.traceId, "Federated STS returned response {0:d} {1}", httpWebResponse.StatusCode, text);
						try
						{
							safeXmlDocument.LoadXml(text);
						}
						catch (XmlException ex2)
						{
							base.ErrorString = string.Format("Federated STS '{0}' has malformed RST response for user \"{1}\".  Exception {2} XML response {3}", new object[]
							{
								this.FederatedLogonURI,
								this.traceUserName,
								ex2.ToString(),
								text
							});
							ExTraceGlobals.AuthenticationTracer.TraceError((long)this.traceId, base.ErrorString);
							throw;
						}
						XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(safeXmlDocument.NameTable);
						xmlNamespaceManager.AddNamespace("a", "http://www.w3.org/2005/08/addressing");
						xmlNamespaceManager.AddNamespace("s", "http://www.w3.org/2003/05/soap-envelope");
						xmlNamespaceManager.AddNamespace("t", "http://schemas.xmlsoap.org/ws/2005/02/trust");
						xmlNamespaceManager.AddNamespace("saml", "urn:oasis:names:tc:SAML:1.0:assertion");
						XmlNode xmlNode = safeXmlDocument.SelectSingleNode("/s:Envelope/s:Body/t:RequestSecurityTokenResponse/t:RequestedSecurityToken/saml:Assertion", xmlNamespaceManager);
						if (xmlNode != null)
						{
							if (httpWebResponse.StatusCode == HttpStatusCode.OK)
							{
								result = Encoding.UTF8.GetBytes(xmlNode.OuterXml);
								ExTraceGlobals.AuthenticationTracer.Information<string, string>((long)this.traceId, "Successfully processed response to http logon request to federated STS '{0}' for '{1}'", this.FederatedLogonURI, this.traceUserName);
								return result;
							}
							ExTraceGlobals.AuthenticationTracer.Information<string, string, HttpStatusCode>((long)this.traceId, "Assertion attribute is found in response to http logon request to federated STS '{0}' for '{1}', but the status code is {2} .", this.FederatedLogonURI, this.traceUserName, httpWebResponse.StatusCode);
						}
						XmlNode xmlNode2 = safeXmlDocument.SelectSingleNode("/s:Envelope/s:Body/s:Fault", xmlNamespaceManager);
						if (xmlNode2 != null)
						{
							base.ErrorString = xmlNode2.OuterXml;
							XmlNode xmlNode3 = safeXmlDocument.SelectSingleNode("/s:Envelope/s:Body/s:Fault/s:Code/s:Subcode/s:Value", xmlNamespaceManager);
							XmlNode xmlNode4 = safeXmlDocument.SelectSingleNode("/s:Envelope/s:Body/s:Fault/s:Reason", xmlNamespaceManager);
							if (xmlNode3 == null)
							{
								base.ErrorString = "FederatedSTS missing value node:" + text;
								Interlocked.Increment(ref this.namespaceStats.Failed);
								this.namespaceStats.User = this.traceUserName;
								ExTraceGlobals.AuthenticationTracer.TraceError<string, string, string>((long)this.traceId, "Federated STS '{0}' has missing Value for user {1}. XML = {2}", this.FederatedLogonURI, this.traceUserName, text);
								ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.traceId, "Leaving FederatedSTS.ProcessResponse()");
							}
							else
							{
								SafeXmlDocument safeXmlDocument2 = new SafeXmlDocument();
								safeXmlDocument2.PreserveWhitespace = true;
								safeXmlDocument2.LoadXml(xmlNode3.OuterXml);
								new XmlNamespaceManager(safeXmlDocument2.NameTable);
								if (!string.IsNullOrEmpty(safeXmlDocument2.InnerText.Trim()) && safeXmlDocument2.InnerText.Trim().EndsWith(":FailedAuthentication", StringComparison.OrdinalIgnoreCase))
								{
									XmlNode xmlNode5 = safeXmlDocument.SelectSingleNode("/s:Envelope/s:Body/s:Fault/s:Reason/s:Text", xmlNamespaceManager);
									if (xmlNode5 != null)
									{
										string innerText = xmlNode5.InnerText;
										if (!string.IsNullOrEmpty(innerText) && innerText.IndexOf("MSIS3126:", StringComparison.OrdinalIgnoreCase) >= 0)
										{
											this.IsFederatedStsADFSRulesDenied = true;
											base.ErrorString = "FederatedSTS ADFS authorization rules denied the request:" + text;
											Interlocked.Increment(ref this.namespaceStats.ADFSRulesDeny);
										}
										else
										{
											base.IsBadCredentials = true;
											Interlocked.Increment(ref this.namespaceStats.BadPassword);
										}
									}
									else
									{
										base.IsBadCredentials = true;
										Interlocked.Increment(ref this.namespaceStats.BadPassword);
									}
									this.namespaceStats.User = this.traceUserName;
								}
								else if (!string.IsNullOrEmpty(safeXmlDocument2.InnerText.Trim()) && safeXmlDocument2.InnerText.Trim().EndsWith(":FailedAuthentication.ExpiredPassword", StringComparison.OrdinalIgnoreCase))
								{
									base.IsExpiredCreds = true;
									if (xmlNode4 == null)
									{
										base.ErrorString = "FederatedSTS missing reason node:" + text;
										Interlocked.Increment(ref this.namespaceStats.Failed);
										this.namespaceStats.User = this.traceUserName;
										ExTraceGlobals.AuthenticationTracer.TraceError<string, string, string>((long)this.traceId, "Federated STS '{0}' has missing reason node for user {1}. XML = {2}", this.FederatedLogonURI, this.traceUserName, text);
										ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.traceId, "Leaving FederatedSTS.ProcessResponse()");
										return result;
									}
									string innerText2 = xmlNode4.InnerText;
									if (!string.IsNullOrEmpty(innerText2.Trim()))
									{
										int num3 = innerText2.IndexOf("https:", StringComparison.OrdinalIgnoreCase);
										if (num3 >= 0)
										{
											base.RecoveryUrl = innerText2.Substring(num3).Trim();
										}
									}
								}
								else if (!string.IsNullOrEmpty(safeXmlDocument2.InnerText.Trim()) && safeXmlDocument2.InnerText.Trim().EndsWith(":InvalidSecurity", StringComparison.OrdinalIgnoreCase) && (base.ClockSkew - clockSkew).Duration() >= base.ClockSkewThreshold)
								{
									ExTraceGlobals.AuthenticationTracer.Information<string, int>((long)this.traceId, "Clock skew for ADFS endpoint '{0}' changed more than '{1}' minutes - possible clock skew failure", this.FederatedLogonURI, base.ClockSkew.Minutes);
									base.PossibleClockSkew = true;
								}
								ExTraceGlobals.AuthenticationTracer.Information<string, string, string>((long)this.traceId, "Logon failed to federated STS '{0}' for '{1}' reason {2}", this.FederatedLogonURI, this.traceUserName, base.ErrorString);
								ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.traceId, "Leaving FederatedSTS.ProcessResponse()");
							}
						}
						else
						{
							Interlocked.Increment(ref this.namespaceStats.Failed);
							this.namespaceStats.User = this.traceUserName;
							if (xmlNode != null)
							{
								base.ErrorString = "FederatedSTS response contains saml:Assertion, but returns " + httpWebResponse.StatusCode;
							}
							else
							{
								base.ErrorString = string.Format("HttpStatus: {0}. FederatedSTS missing fault node:{1}", httpWebResponse.StatusCode, text);
								ExTraceGlobals.AuthenticationTracer.TraceError<string, string, string>((long)this.traceId, "Federated STS '{0}' has missing Value for user {1}. XML = {2}", this.FederatedLogonURI, this.traceUserName, text);
							}
						}
					}
				}
			}
			finally
			{
				if (webResponse != null)
				{
					((IDisposable)webResponse).Dispose();
				}
			}
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.traceId, "Leaving FederatedSTS.ProcessResponse()");
			return result;
		}

		public void Abort()
		{
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.traceId, "Entering FederatedSTS.Abort()");
			if (this.adfsRequest != null)
			{
				ExTraceGlobals.AuthenticationTracer.Information<string, string>((long)this.traceId, "Aborting http logon request to federated STS '{0}' for '{1}'", this.FederatedLogonURI, this.traceUserName);
				this.adfsRequest.Abort();
			}
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.traceId, "Leaving FederatedSTS.Abort()");
		}

		private const string LiveConnectorRequestP1 = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><s:Envelope xmlns:s=\"http://www.w3.org/2003/05/soap-envelope\" xmlns:wsse=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd\" xmlns:saml=\"urn:oasis:names:tc:SAML:1.0:assertion\" xmlns:wsp=\"http://schemas.xmlsoap.org/ws/2004/09/policy\" xmlns:wsu=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd\" xmlns:wsa=\"http://www.w3.org/2005/08/addressing\" xmlns:wssc=\"http://schemas.xmlsoap.org/ws/2005/02/sc\" xmlns:wst=\"http://schemas.xmlsoap.org/ws/2005/02/trust\"><s:Header><wsa:Action s:mustUnderstand=\"1\">http://schemas.xmlsoap.org/ws/2005/02/trust/RST/Issue</wsa:Action><wsa:To s:mustUnderstand=\"1\">";

		private const string LiveConnectorRequestP2 = "</wsa:To><wsa:MessageID>";

		private const string LiveConnectorRequestP3 = "</wsa:MessageID><ps:AuthInfo xmlns:ps=\"http://schemas.microsoft.com/Passport/SoapServices/PPCRL\" Id=\"PPAuthInfo\"><ps:HostingApp>{DF60E2DF-88AD-4526-AE21-83D130EF0F68}</ps:HostingApp><ps:BinaryVersion>6</ps:BinaryVersion><ps:UIVersion>1</ps:UIVersion><ps:Cookies></ps:Cookies><ps:RequestParams>AQAAAAIAAABsYwQAAAAxMDMz</ps:RequestParams></ps:AuthInfo><wsse:Security><wsse:UsernameToken wsu:Id=\"user\"><wsse:Username>";

		private const string LiveConnectorRequestP4 = "</wsse:Username><wsse:Password>";

		private const string LiveConnectorRequestP5 = "</wsse:Password></wsse:UsernameToken><wsu:Timestamp Id=\"Timestamp\"><wsu:Created>";

		private const string LiveConnectorRequestP6 = "</wsu:Created><wsu:Expires>";

		private const string LiveConnectorRequestP7 = "</wsu:Expires></wsu:Timestamp></wsse:Security></s:Header><s:Body><wst:RequestSecurityToken Id=\"RST0\"><wst:RequestType>http://schemas.xmlsoap.org/ws/2005/02/trust/Issue</wst:RequestType><wsp:AppliesTo><wsa:EndpointReference><wsa:Address>";

		private const string LiveConnectorRequestP8 = "</wsa:Address></wsa:EndpointReference></wsp:AppliesTo><wst:KeyType>http://schemas.xmlsoap.org/ws/2005/05/identity/NoProofKey</wst:KeyType></wst:RequestSecurityToken></s:Body></s:Envelope>";

		private static int numberOfOutgoingRequests;

		private static readonly byte[] stsRequestBytesP1 = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?><s:Envelope xmlns:s=\"http://www.w3.org/2003/05/soap-envelope\" xmlns:wsse=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd\" xmlns:saml=\"urn:oasis:names:tc:SAML:1.0:assertion\" xmlns:wsp=\"http://schemas.xmlsoap.org/ws/2004/09/policy\" xmlns:wsu=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd\" xmlns:wsa=\"http://www.w3.org/2005/08/addressing\" xmlns:wssc=\"http://schemas.xmlsoap.org/ws/2005/02/sc\" xmlns:wst=\"http://schemas.xmlsoap.org/ws/2005/02/trust\"><s:Header><wsa:Action s:mustUnderstand=\"1\">http://schemas.xmlsoap.org/ws/2005/02/trust/RST/Issue</wsa:Action><wsa:To s:mustUnderstand=\"1\">");

		private static readonly byte[] stsRequestBytesP2 = Encoding.UTF8.GetBytes("</wsa:To><wsa:MessageID>");

		private static readonly byte[] stsRequestBytesP3 = Encoding.UTF8.GetBytes("</wsa:MessageID><ps:AuthInfo xmlns:ps=\"http://schemas.microsoft.com/Passport/SoapServices/PPCRL\" Id=\"PPAuthInfo\"><ps:HostingApp>{DF60E2DF-88AD-4526-AE21-83D130EF0F68}</ps:HostingApp><ps:BinaryVersion>6</ps:BinaryVersion><ps:UIVersion>1</ps:UIVersion><ps:Cookies></ps:Cookies><ps:RequestParams>AQAAAAIAAABsYwQAAAAxMDMz</ps:RequestParams></ps:AuthInfo><wsse:Security><wsse:UsernameToken wsu:Id=\"user\"><wsse:Username>");

		private static readonly byte[] stsRequestBytesP4 = Encoding.UTF8.GetBytes("</wsse:Username><wsse:Password>");

		private static readonly byte[] stsRequestBytesP5 = Encoding.UTF8.GetBytes("</wsse:Password></wsse:UsernameToken><wsu:Timestamp Id=\"Timestamp\"><wsu:Created>");

		private static readonly byte[] stsRequestBytesP6 = Encoding.UTF8.GetBytes("</wsu:Created><wsu:Expires>");

		private static readonly byte[] stsRequestBytesP7 = Encoding.UTF8.GetBytes("</wsu:Expires></wsu:Timestamp></wsse:Security></s:Header><s:Body><wst:RequestSecurityToken Id=\"RST0\"><wst:RequestType>http://schemas.xmlsoap.org/ws/2005/02/trust/Issue</wst:RequestType><wsp:AppliesTo><wsa:EndpointReference><wsa:Address>");

		private static readonly byte[] stsRequestBytesP8 = Encoding.UTF8.GetBytes("</wsa:Address></wsa:EndpointReference></wsp:AppliesTo><wst:KeyType>http://schemas.xmlsoap.org/ws/2005/05/identity/NoProofKey</wst:KeyType></wst:RequestSecurityToken></s:Body></s:Envelope>");

		private UniqueId messageId;

		private int maxResponseSize = 131072;

		private HttpWebRequest adfsRequest;

		private Stopwatch stopwatch;

		private byte[] remoteUserName;

		private byte[] remotePassword;

		private string traceUserName;
	}
}
