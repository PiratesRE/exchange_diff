using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Security.AntiXss;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Net;
using Microsoft.Exchange.Net.WebApplicationClient;

namespace Microsoft.Exchange.Net.LiveIDAuthentication
{
	internal sealed class LiveIDAuthenticationClient : DisposeTrackableBase
	{
		public LiveIDAuthenticationClient(Uri liveRSTEndpoint) : this(100000, WebRequest.DefaultWebProxy, liveRSTEndpoint)
		{
		}

		public LiveIDAuthenticationClient(int timeout, IWebProxy proxy, Uri liveRSTEndpoint)
		{
			this.httpClient = new HttpClient();
			this.httpClient.SendingRequest += this.httpClient_SendingRequest;
			this.httpClient.ResponseReceived += this.httpClient_ResponseReceived;
			this.httpSessionConfig = new HttpSessionConfig();
			this.httpSessionConfig.Timeout = timeout;
			this.httpSessionConfig.Method = "POST";
			this.httpSessionConfig.ContentType = "application/soap+xml; charset=UTF-8";
			this.httpSessionConfig.Proxy = proxy;
			this.httpSessionConfig.AllowAutoRedirect = true;
			this.authServerUri = liveRSTEndpoint;
		}

		public event EventHandler<HttpWebRequestEventArgs> SendingRequest;

		private void httpClient_SendingRequest(object sender, HttpWebRequestEventArgs e)
		{
			EventHandler<HttpWebRequestEventArgs> sendingRequest = this.SendingRequest;
			if (sendingRequest != null)
			{
				sendingRequest(this, e);
			}
		}

		public event EventHandler<HttpWebResponseEventArgs> ResponseReceived;

		private void httpClient_ResponseReceived(object sender, HttpWebResponseEventArgs e)
		{
			EventHandler<HttpWebResponseEventArgs> responseReceived = this.ResponseReceived;
			if (responseReceived != null)
			{
				responseReceived(this, e);
			}
		}

		internal Uri AuthServerUri
		{
			get
			{
				base.CheckDisposed();
				return this.authServerUri;
			}
			set
			{
				base.CheckDisposed();
				this.authServerUri = value;
			}
		}

		internal static int MaximumUriRedirections
		{
			get
			{
				return LiveIDAuthenticationClient.maximumUriRedirections;
			}
			set
			{
				LiveIDAuthenticationClient.maximumUriRedirections = value;
			}
		}

		public ICancelableAsyncResult BeginGetToken(string applicationId, string username, string password, string authPolicy, string serviceEndpointAddress, CancelableAsyncCallback callback, object asyncState)
		{
			base.CheckDisposed();
			LiveIDAuthenticationClient.Tracer.TraceDebug((long)this.GetHashCode(), "Begin Get Token [AppId:Username:AuthPolicy:ServiceEndpointAddress] = [{0}:{1}:{2}:{3}]", new object[]
			{
				applicationId,
				username,
				authPolicy,
				serviceEndpointAddress
			});
			string text = AntiXssEncoder.HtmlEncode(applicationId, false);
			string text2 = AntiXssEncoder.HtmlEncode(username, false);
			string text3 = AntiXssEncoder.HtmlEncode(password, false);
			string text4 = AntiXssEncoder.HtmlEncode(serviceEndpointAddress, false);
			string text5 = AntiXssEncoder.HtmlEncode(authPolicy, false);
			string formatedRequestString = string.Format(CultureInfo.InvariantCulture, "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n            <Envelope   xmlns=\"http://schemas.xmlsoap.org/soap/envelope/\" \r\n                        xmlns:wsse=\"http://schemas.xmlsoap.org/ws/2003/06/secext\" \r\n                        xmlns:saml=\"urn:oasis:names:tc:SAML:1.0:assertion\" \r\n                        xmlns:wsp=\"http://schemas.xmlsoap.org/ws/2002/12/policy\" \r\n                        xmlns:wsu=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd\" \r\n                        xmlns:wsa=\"http://schemas.xmlsoap.org/ws/2004/03/addressing\" \r\n                        xmlns:wssc=\"http://schemas.xmlsoap.org/ws/2004/04/sc\" \r\n                        xmlns:wst=\"http://schemas.xmlsoap.org/ws/2004/04/trust\">\r\n            <Header>\r\n                <ps:AuthInfo xmlns:ps=\"http://schemas.microsoft.com/Passport/SoapServices/PPCRL\" Id=\"PPAuthInfo\">\r\n                    <ps:HostingApp>{0:B}</ps:HostingApp>\r\n                    <ps:BinaryVersion>4</ps:BinaryVersion>\r\n                    <ps:UIVersion>1</ps:UIVersion>\r\n                    <ps:Cookies></ps:Cookies>\r\n                    <ps:RequestParams>AQAAAAIAAABsYwQAAAAzMDg0</ps:RequestParams>\r\n                </ps:AuthInfo>\r\n                <wsse:Security>\r\n                    <wsse:UsernameToken Id=\"user\">\r\n                        <wsse:Username>{1}</wsse:Username>\r\n                        <wsse:Password>{2}</wsse:Password>\r\n                    </wsse:UsernameToken>\r\n                </wsse:Security>\r\n            </Header>\r\n            <Body>\r\n            <ps:RequestMultipleSecurityTokens xmlns:ps=\"http://schemas.microsoft.com/Passport/SoapServices/PPCRL\" Id=\"RSTS\">\r\n                <wst:RequestSecurityToken Id=\"RST0\">\r\n                    <wst:RequestType>http://schemas.xmlsoap.org/ws/2004/04/security/trust/Issue</wst:RequestType>\r\n                    <wsp:AppliesTo>\r\n                        <wsa:EndpointReference>\r\n                            <wsa:Address>http://Passport.NET/tb</wsa:Address>\r\n                        </wsa:EndpointReference>\r\n                    </wsp:AppliesTo>\r\n                </wst:RequestSecurityToken>\r\n                <wst:RequestSecurityToken Id=\"RST1\">\r\n                    <wst:RequestType>http://schemas.xmlsoap.org/ws/2004/04/security/trust/Issue</wst:RequestType>\r\n                    <wsp:AppliesTo>\r\n                        <wsa:EndpointReference>\r\n                            <wsa:Address>{3}</wsa:Address>\r\n                        </wsa:EndpointReference>\r\n                    </wsp:AppliesTo>\r\n                    <wsse:PolicyReference URI=\"{4}\"></wsse:PolicyReference>\r\n                </wst:RequestSecurityToken>\r\n            </ps:RequestMultipleSecurityTokens>\r\n            </Body>\r\n            </Envelope>", new object[]
			{
				text,
				text2,
				text3,
				text4,
				text5
			});
			return this.BeginSendAuthenticationRequest(formatedRequestString, callback, asyncState);
		}

		public AuthenticationResult EndGetToken(ICancelableAsyncResult asyncResult)
		{
			return this.EndSendAuthenticationRequest(asyncResult);
		}

		public ICancelableAsyncResult BeginGetRst2TicketFromCredentials(string applicationId, string username, string password, string authPolicy, string serviceEndpointAddress, CancelableAsyncCallback callback, object asyncState)
		{
			base.CheckDisposed();
			LiveIDAuthenticationClient.Tracer.TraceDebug((long)this.GetHashCode(), "Begin Get RST2 Ticket [AppId:Username:AuthPolicy:ServiceEndpointAddress] = [{0}:{1}:{2}:{3}]", new object[]
			{
				applicationId,
				username,
				authPolicy,
				serviceEndpointAddress
			});
			string text = AntiXssEncoder.HtmlEncode(applicationId, false);
			string text2 = AntiXssEncoder.HtmlEncode(username, false);
			string text3 = AntiXssEncoder.HtmlEncode(password, false);
			string text4 = AntiXssEncoder.HtmlEncode(serviceEndpointAddress, false);
			string text5 = AntiXssEncoder.HtmlEncode(authPolicy, false);
			string text6 = AntiXssEncoder.HtmlEncode(this.authServerUri.ToString(), false);
			string formatedRequestString = string.Format(CultureInfo.InvariantCulture, "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<s:Envelope xmlns:s=\"http://www.w3.org/2003/05/soap-envelope\" xmlns:wsse=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd\" xmlns:wsp=\"http://schemas.xmlsoap.org/ws/2004/09/policy\" xmlns:wsu=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd\" xmlns:wsa=\"http://www.w3.org/2005/08/addressing\" xmlns:wst=\"http://schemas.xmlsoap.org/ws/2005/02/trust\">\r\n    <s:Header>\r\n        <wsa:Action s:mustUnderstand=\"1\">http://schemas.xmlsoap.org/ws/2005/02/trust/RST/Issue</wsa:Action>\r\n        <wsa:To s:mustUnderstand=\"1\">{5}</wsa:To>\r\n        <wsa:MessageID>80730701</wsa:MessageID>\r\n        <ps:AuthInfo xmlns:ps=\"http://schemas.microsoft.com/Passport/SoapServices/PPCRL\" Id=\"PPAuthInfo\">\r\n            <ps:HostingApp>{0:B}</ps:HostingApp>\r\n            <ps:BinaryVersion>5</ps:BinaryVersion>\r\n            <ps:UIVersion>1</ps:UIVersion>\r\n            <ps:Cookies></ps:Cookies>\r\n            <ps:RequestParams>AQAAAAIAAABsYwQAAAAxMDMz</ps:RequestParams>\r\n        </ps:AuthInfo>\r\n        <wsse:Security>\r\n            <wsse:UsernameToken wsu:Id=\"user\">\r\n                <wsse:Username>{1}</wsse:Username>\r\n                <wsse:Password>{2}</wsse:Password>\r\n            </wsse:UsernameToken>\r\n        </wsse:Security>\r\n    </s:Header>\r\n    <s:Body>\r\n        <ps:RequestMultipleSecurityTokens xmlns:ps=\"http://schemas.microsoft.com/Passport/SoapServices/PPCRL\" Id=\"RSTS\">\r\n            <wst:RequestSecurityToken Id=\"RST0\">\r\n                <wst:RequestType>http://schemas.xmlsoap.org/ws/2005/02/trust/Issue</wst:RequestType>\r\n                <wsp:AppliesTo>\r\n                    <wsa:EndpointReference>\r\n                        <wsa:Address>http://Passport.NET/tb</wsa:Address>\r\n                    </wsa:EndpointReference>\r\n                </wsp:AppliesTo>\r\n            </wst:RequestSecurityToken>\r\n            <wst:RequestSecurityToken Id=\"RST1\">\r\n                <wst:RequestType>http://schemas.xmlsoap.org/ws/2005/02/trust/Issue</wst:RequestType>\r\n                <wsp:AppliesTo>\r\n                    <wsa:EndpointReference>\r\n                        <wsa:Address>{3}</wsa:Address>\r\n                    </wsa:EndpointReference>\r\n                </wsp:AppliesTo>\r\n                <wsp:PolicyReference URI=\"{4}\"></wsp:PolicyReference>\r\n            </wst:RequestSecurityToken>\r\n        </ps:RequestMultipleSecurityTokens>\r\n    </s:Body>\r\n</s:Envelope>", new object[]
			{
				text,
				text2,
				text3,
				text4,
				text5,
				text6
			});
			return this.BeginSendAuthenticationRequest(formatedRequestString, callback, asyncState);
		}

		public AuthenticationResult EndGetRst2TicketFromCredentials(ICancelableAsyncResult asyncResult)
		{
			return this.EndSendAuthenticationRequest(asyncResult);
		}

		public ICancelableAsyncResult BeginGetRst2TicketFromSaml(string applicationId, string samlAssertionXml, string authPolicy, string serviceEndpointAddress, CancelableAsyncCallback callback, object asyncState)
		{
			base.CheckDisposed();
			LiveIDAuthenticationClient.Tracer.TraceDebug((long)this.GetHashCode(), "Begin Get RST2 Ticket [AppId:SamlAssertion:AuthPolicy:ServiceEndpointAddress] = [{0}:{1}:{2}:{3}]", new object[]
			{
				applicationId,
				samlAssertionXml,
				authPolicy,
				serviceEndpointAddress
			});
			string text = AntiXssEncoder.HtmlEncode(applicationId, false);
			string text2 = AntiXssEncoder.HtmlEncode(serviceEndpointAddress, false);
			string text3 = AntiXssEncoder.HtmlEncode(authPolicy, false);
			string text4 = AntiXssEncoder.HtmlEncode(this.authServerUri.ToString(), false);
			string formatedRequestString = string.Format(CultureInfo.InvariantCulture, "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<s:Envelope xmlns:s=\"http://www.w3.org/2003/05/soap-envelope\" xmlns:wsse=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd\" xmlns:wsp=\"http://schemas.xmlsoap.org/ws/2004/09/policy\" xmlns:wsu=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd\" xmlns:wsa=\"http://www.w3.org/2005/08/addressing\" xmlns:wst=\"http://schemas.xmlsoap.org/ws/2005/02/trust\">\r\n    <s:Header>\r\n        <wsa:Action s:mustUnderstand=\"1\">http://schemas.xmlsoap.org/ws/2005/02/trust/RST/Issue</wsa:Action>\r\n        <wsa:To s:mustUnderstand=\"1\">{4}</wsa:To>\r\n        <wsa:MessageID>80730701</wsa:MessageID>\r\n        <ps:AuthInfo xmlns:ps=\"http://schemas.microsoft.com/Passport/SoapServices/PPCRL\" Id=\"PPAuthInfo\">\r\n            <ps:HostingApp>{0:B}</ps:HostingApp>\r\n            <ps:BinaryVersion>5</ps:BinaryVersion>\r\n            <ps:UIVersion>1</ps:UIVersion>\r\n            <ps:Cookies></ps:Cookies>\r\n            <ps:RequestParams>AQAAAAIAAABsYwQAAAAxMDMz</ps:RequestParams>\r\n        </ps:AuthInfo>\r\n        <wsse:Security>\r\n            {1}\r\n        </wsse:Security>\r\n    </s:Header>\r\n\r\n    <s:Body>\r\n        <ps:RequestMultipleSecurityTokens xmlns:ps=\"http://schemas.microsoft.com/Passport/SoapServices/PPCRL\" Id=\"RSTS\">\r\n            <wst:RequestSecurityToken Id=\"RST0\">\r\n                <wst:RequestType>http://schemas.xmlsoap.org/ws/2005/02/trust/Issue</wst:RequestType>\r\n                <wsp:AppliesTo>\r\n                    <wsa:EndpointReference>\r\n                        <wsa:Address>http://Passport.NET/tb</wsa:Address>\r\n                    </wsa:EndpointReference>\r\n                </wsp:AppliesTo>\r\n            </wst:RequestSecurityToken>\r\n            <wst:RequestSecurityToken Id=\"RST1\">\r\n                <wst:RequestType>http://schemas.xmlsoap.org/ws/2005/02/trust/Issue</wst:RequestType>\r\n                <wsp:AppliesTo>\r\n                    <wsa:EndpointReference>\r\n                        <wsa:Address>{2}</wsa:Address>\r\n                    </wsa:EndpointReference>\r\n                </wsp:AppliesTo>\r\n                <wsp:PolicyReference URI=\"{3}\"></wsp:PolicyReference>\r\n            </wst:RequestSecurityToken>\r\n        </ps:RequestMultipleSecurityTokens>\r\n    </s:Body>\r\n</s:Envelope>", new object[]
			{
				text,
				samlAssertionXml,
				text2,
				text3,
				text4
			});
			return this.BeginSendAuthenticationRequest(formatedRequestString, callback, asyncState);
		}

		public AuthenticationResult EndGetRst2TicketFromSaml(ICancelableAsyncResult asyncResult)
		{
			return this.EndSendAuthenticationRequest(asyncResult);
		}

		private static void DisposeIfNotNull(IDisposable disposableObject)
		{
			if (disposableObject != null)
			{
				disposableObject.Dispose();
			}
		}

		private ICancelableAsyncResult BeginSendAuthenticationRequest(string formatedRequestString, CancelableAsyncCallback callback, object asyncState)
		{
			AuthenticationAsyncResult authenticationAsyncResult = new AuthenticationAsyncResult(callback, asyncState, this);
			this.sessionClose = false;
			this.currentUrlRedirections = 0;
			this.httpSessionConfig.RequestStream = new MemoryStream(Encoding.UTF8.GetBytes(formatedRequestString));
			this.pendingAsyncResult = this.httpClient.BeginDownload(this.AuthServerUri, this.httpSessionConfig, new CancelableAsyncCallback(this.DownloadCallback), authenticationAsyncResult);
			return authenticationAsyncResult;
		}

		private AuthenticationResult EndSendAuthenticationRequest(ICancelableAsyncResult asyncResult)
		{
			base.CheckDisposed();
			AuthenticationAsyncResult authenticationAsyncResult = (AuthenticationAsyncResult)asyncResult;
			return authenticationAsyncResult.EndProcess();
		}

		protected override void InternalDispose(bool disposing)
		{
			lock (this.syncRoot)
			{
				if (disposing)
				{
					this.TryCancel();
					if (this.httpClient != null)
					{
						this.httpClient.Dispose();
						this.httpClient = null;
					}
					LiveIDAuthenticationClient.Tracer.TraceDebug((long)this.GetHashCode(), "Authentication Client Disposed");
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<LiveIDAuthenticationClient>(this);
		}

		internal bool TryCancel()
		{
			base.CheckDisposed();
			return this.TryStopProcess();
		}

		private void DownloadCallback(ICancelableAsyncResult asyncResult)
		{
			AuthenticationAsyncResult authenticationAsyncResult = null;
			BaseAuthenticationToken authToken = null;
			Exception ex = null;
			bool flag = false;
			Uri uri = null;
			lock (this.syncRoot)
			{
				if (this.sessionClose)
				{
					return;
				}
				this.pendingAsyncResult = null;
				authenticationAsyncResult = (AuthenticationAsyncResult)asyncResult.AsyncState;
				if (!asyncResult.CompletedSynchronously)
				{
					authenticationAsyncResult.SetAsync();
				}
				DownloadResult downloadResult = this.httpClient.EndDownload(asyncResult);
				if (downloadResult.IsSucceeded)
				{
					Exception innerException = null;
					if (!this.TryParseResponse(downloadResult.ResponseStream, out authToken, out innerException, out flag, out uri))
					{
						ex = new AuthenticationException(innerException);
					}
				}
				else if (downloadResult.IsRetryable)
				{
					ex = new DownloadTransientException(downloadResult.Exception);
				}
				else
				{
					ex = new DownloadPermanentException(downloadResult.Exception);
				}
			}
			if (flag)
			{
				LiveIDAuthenticationClient.Tracer.TraceDebug<Uri>((long)this.GetHashCode(), "Uri Redirect Response received, with redirect uri value: {0} ", uri);
				if (this.currentUrlRedirections < LiveIDAuthenticationClient.maximumUriRedirections)
				{
					this.currentUrlRedirections++;
					this.httpSessionConfig.RequestStream.Position = 0L;
					this.pendingAsyncResult = this.httpClient.BeginDownload(uri, this.httpSessionConfig, new CancelableAsyncCallback(this.DownloadCallback), authenticationAsyncResult);
					return;
				}
				LiveIDAuthenticationClient.Tracer.TraceError<int>((long)this.GetHashCode(), "Max number of URL Redirections({0}) reached", LiveIDAuthenticationClient.maximumUriRedirections);
				ex = new DownloadTransientException(new MaximumUriRedirectionsReachedException(LiveIDAuthenticationClient.maximumUriRedirections));
			}
			if (this.TryStopProcess())
			{
				if (ex == null)
				{
					authenticationAsyncResult.ProcessCompleted(authToken);
					return;
				}
				LiveIDAuthenticationClient.Tracer.TraceError<Type, string>((long)this.GetHashCode(), "Authentication Failed with Exception ({0}) : {1}", ex.GetType(), ex.Message);
				authenticationAsyncResult.ProcessCompleted(ex);
			}
		}

		private bool TryParseResponse(Stream stream, out BaseAuthenticationToken authToken, out Exception exception, out bool isUrlRedirectResponse, out Uri redirectUrl)
		{
			XmlNodeList xmlNodeList = null;
			authToken = null;
			exception = null;
			isUrlRedirectResponse = false;
			redirectUrl = null;
			bool result;
			try
			{
				SafeXmlDocument safeXmlDocument = new SafeXmlDocument();
				safeXmlDocument.PreserveWhitespace = true;
				safeXmlDocument.Load(stream);
				LiveIDAuthenticationClient.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Received XML Blob: {0}", safeXmlDocument.InnerXml);
				XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(safeXmlDocument.NameTable);
				xmlNamespaceManager.AddNamespace("wsse", "http://schemas.xmlsoap.org/ws/2003/06/secext");
				xmlNamespaceManager.AddNamespace("wsse2", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");
				xmlNamespaceManager.AddNamespace("wsu", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd");
				xmlNamespaceManager.AddNamespace("wst", "http://schemas.xmlsoap.org/ws/2004/04/trust");
				xmlNamespaceManager.AddNamespace("wst2", "http://schemas.xmlsoap.org/ws/2005/02/trust");
				xmlNamespaceManager.AddNamespace("psf", "http://schemas.microsoft.com/Passport/SoapServices/SOAPFault");
				xmlNamespaceManager.AddNamespace("saml", "urn:oasis:names:tc:SAML:1.0:assertion");
				xmlNodeList = safeXmlDocument.GetElementsByTagName("redirectUrl", "http://schemas.microsoft.com/Passport/SoapServices/SOAPFault");
				if (xmlNodeList != null && xmlNodeList.Count == 1)
				{
					isUrlRedirectResponse = true;
					result = Uri.TryCreate(xmlNodeList[0].InnerText, UriKind.RelativeOrAbsolute, out redirectUrl);
				}
				else
				{
					LiveIDAuthenticationClient.DisposeIfNotNull(xmlNodeList);
					xmlNodeList = LiveIDAuthenticationClient.GetElementsByTagNameMultipleNamespaces(safeXmlDocument, "BinarySecurityToken", new string[]
					{
						"http://schemas.xmlsoap.org/ws/2003/06/secext",
						"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"
					});
					if (xmlNodeList != null && xmlNodeList.Count >= 1)
					{
						XmlNode xmlNode = xmlNodeList[0];
						LiveIDAuthenticationClient.DisposeIfNotNull(xmlNodeList);
						xmlNodeList = safeXmlDocument.GetElementsByTagName("Expires", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd");
						if (xmlNodeList == null || xmlNodeList.Count < 2)
						{
							result = false;
						}
						else
						{
							XmlNode xmlNode2 = xmlNodeList[1];
							LiveIDAuthenticationClient.DisposeIfNotNull(xmlNodeList);
							xmlNodeList = safeXmlDocument.GetElementsByTagName("PUID", "http://schemas.microsoft.com/Passport/SoapServices/SOAPFault");
							if (xmlNodeList == null || xmlNodeList.Count != 1)
							{
								result = false;
							}
							else
							{
								XmlNode xmlNode3 = xmlNodeList[0];
								LiveIDAuthenticationClient.DisposeIfNotNull(xmlNodeList);
								xmlNodeList = LiveIDAuthenticationClient.GetElementsByTagNameMultipleNamespaces(safeXmlDocument, "BinarySecret", new string[]
								{
									"http://schemas.xmlsoap.org/ws/2004/04/trust",
									"http://schemas.xmlsoap.org/ws/2005/02/trust"
								});
								XmlNode xmlNode4 = null;
								if (xmlNodeList != null && xmlNodeList.Count > 1)
								{
									xmlNode4 = xmlNodeList[1];
								}
								if (xmlNode == null || xmlNode2 == null || xmlNode3 == null)
								{
									result = false;
								}
								else
								{
									authToken = new AuthenticationToken(xmlNode.InnerText, DateTime.Parse(xmlNode2.InnerText), (xmlNode4 != null) ? xmlNode4.InnerText : null, xmlNode3.InnerText);
									result = true;
								}
							}
						}
					}
					else
					{
						LiveIDAuthenticationClient.DisposeIfNotNull(xmlNodeList);
						xmlNodeList = safeXmlDocument.GetElementsByTagName("Assertion", "urn:oasis:names:tc:SAML:1.0:assertion");
						if (xmlNodeList != null && xmlNodeList.Count >= 1)
						{
							XmlNode xmlNode5 = xmlNodeList[0];
							authToken = new SamlAuthenticationToken(xmlNode5.OuterXml);
							result = true;
						}
						else
						{
							result = false;
						}
					}
				}
			}
			catch (XmlException ex)
			{
				exception = ex;
				result = false;
			}
			catch (ArgumentException ex2)
			{
				exception = ex2;
				result = false;
			}
			finally
			{
				LiveIDAuthenticationClient.DisposeIfNotNull(xmlNodeList);
			}
			return result;
		}

		private static XmlNodeList GetElementsByTagNameMultipleNamespaces(SafeXmlDocument document, string tagName, params string[] namespaces)
		{
			XmlNodeList xmlNodeList = null;
			foreach (string namespaceURI in namespaces)
			{
				LiveIDAuthenticationClient.DisposeIfNotNull(xmlNodeList);
				xmlNodeList = document.GetElementsByTagName(tagName, namespaceURI);
				if (xmlNodeList != null && xmlNodeList.Count > 0)
				{
					return xmlNodeList;
				}
			}
			return xmlNodeList;
		}

		private bool TryStopProcess()
		{
			lock (this.syncRoot)
			{
				if (this.sessionClose)
				{
					LiveIDAuthenticationClient.Tracer.TraceDebug((long)this.GetHashCode(), "Process Stop Close already initiated");
					return false;
				}
				this.sessionClose = true;
				if (this.pendingAsyncResult != null)
				{
					this.pendingAsyncResult.Cancel();
					this.pendingAsyncResult = null;
				}
				if (this.httpSessionConfig.RequestStream != null)
				{
					this.httpSessionConfig.RequestStream.Close();
				}
			}
			return true;
		}

		private const string wsse = "http://schemas.xmlsoap.org/ws/2003/06/secext";

		private const string wsse2 = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd";

		private const string wsu = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd";

		private const string wst = "http://schemas.xmlsoap.org/ws/2004/04/trust";

		private const string wst2 = "http://schemas.xmlsoap.org/ws/2005/02/trust";

		private const string psf = "http://schemas.microsoft.com/Passport/SoapServices/SOAPFault";

		private const string saml = "urn:oasis:names:tc:SAML:1.0:assertion";

		private const string SoapEnvelope = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n            <Envelope   xmlns=\"http://schemas.xmlsoap.org/soap/envelope/\" \r\n                        xmlns:wsse=\"http://schemas.xmlsoap.org/ws/2003/06/secext\" \r\n                        xmlns:saml=\"urn:oasis:names:tc:SAML:1.0:assertion\" \r\n                        xmlns:wsp=\"http://schemas.xmlsoap.org/ws/2002/12/policy\" \r\n                        xmlns:wsu=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd\" \r\n                        xmlns:wsa=\"http://schemas.xmlsoap.org/ws/2004/03/addressing\" \r\n                        xmlns:wssc=\"http://schemas.xmlsoap.org/ws/2004/04/sc\" \r\n                        xmlns:wst=\"http://schemas.xmlsoap.org/ws/2004/04/trust\">\r\n            <Header>\r\n                <ps:AuthInfo xmlns:ps=\"http://schemas.microsoft.com/Passport/SoapServices/PPCRL\" Id=\"PPAuthInfo\">\r\n                    <ps:HostingApp>{0:B}</ps:HostingApp>\r\n                    <ps:BinaryVersion>4</ps:BinaryVersion>\r\n                    <ps:UIVersion>1</ps:UIVersion>\r\n                    <ps:Cookies></ps:Cookies>\r\n                    <ps:RequestParams>AQAAAAIAAABsYwQAAAAzMDg0</ps:RequestParams>\r\n                </ps:AuthInfo>\r\n                <wsse:Security>\r\n                    <wsse:UsernameToken Id=\"user\">\r\n                        <wsse:Username>{1}</wsse:Username>\r\n                        <wsse:Password>{2}</wsse:Password>\r\n                    </wsse:UsernameToken>\r\n                </wsse:Security>\r\n            </Header>\r\n            <Body>\r\n            <ps:RequestMultipleSecurityTokens xmlns:ps=\"http://schemas.microsoft.com/Passport/SoapServices/PPCRL\" Id=\"RSTS\">\r\n                <wst:RequestSecurityToken Id=\"RST0\">\r\n                    <wst:RequestType>http://schemas.xmlsoap.org/ws/2004/04/security/trust/Issue</wst:RequestType>\r\n                    <wsp:AppliesTo>\r\n                        <wsa:EndpointReference>\r\n                            <wsa:Address>http://Passport.NET/tb</wsa:Address>\r\n                        </wsa:EndpointReference>\r\n                    </wsp:AppliesTo>\r\n                </wst:RequestSecurityToken>\r\n                <wst:RequestSecurityToken Id=\"RST1\">\r\n                    <wst:RequestType>http://schemas.xmlsoap.org/ws/2004/04/security/trust/Issue</wst:RequestType>\r\n                    <wsp:AppliesTo>\r\n                        <wsa:EndpointReference>\r\n                            <wsa:Address>{3}</wsa:Address>\r\n                        </wsa:EndpointReference>\r\n                    </wsp:AppliesTo>\r\n                    <wsse:PolicyReference URI=\"{4}\"></wsse:PolicyReference>\r\n                </wst:RequestSecurityToken>\r\n            </ps:RequestMultipleSecurityTokens>\r\n            </Body>\r\n            </Envelope>";

		private const string Rst2UsernamePasswordSoapEnvelope = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<s:Envelope xmlns:s=\"http://www.w3.org/2003/05/soap-envelope\" xmlns:wsse=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd\" xmlns:wsp=\"http://schemas.xmlsoap.org/ws/2004/09/policy\" xmlns:wsu=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd\" xmlns:wsa=\"http://www.w3.org/2005/08/addressing\" xmlns:wst=\"http://schemas.xmlsoap.org/ws/2005/02/trust\">\r\n    <s:Header>\r\n        <wsa:Action s:mustUnderstand=\"1\">http://schemas.xmlsoap.org/ws/2005/02/trust/RST/Issue</wsa:Action>\r\n        <wsa:To s:mustUnderstand=\"1\">{5}</wsa:To>\r\n        <wsa:MessageID>80730701</wsa:MessageID>\r\n        <ps:AuthInfo xmlns:ps=\"http://schemas.microsoft.com/Passport/SoapServices/PPCRL\" Id=\"PPAuthInfo\">\r\n            <ps:HostingApp>{0:B}</ps:HostingApp>\r\n            <ps:BinaryVersion>5</ps:BinaryVersion>\r\n            <ps:UIVersion>1</ps:UIVersion>\r\n            <ps:Cookies></ps:Cookies>\r\n            <ps:RequestParams>AQAAAAIAAABsYwQAAAAxMDMz</ps:RequestParams>\r\n        </ps:AuthInfo>\r\n        <wsse:Security>\r\n            <wsse:UsernameToken wsu:Id=\"user\">\r\n                <wsse:Username>{1}</wsse:Username>\r\n                <wsse:Password>{2}</wsse:Password>\r\n            </wsse:UsernameToken>\r\n        </wsse:Security>\r\n    </s:Header>\r\n    <s:Body>\r\n        <ps:RequestMultipleSecurityTokens xmlns:ps=\"http://schemas.microsoft.com/Passport/SoapServices/PPCRL\" Id=\"RSTS\">\r\n            <wst:RequestSecurityToken Id=\"RST0\">\r\n                <wst:RequestType>http://schemas.xmlsoap.org/ws/2005/02/trust/Issue</wst:RequestType>\r\n                <wsp:AppliesTo>\r\n                    <wsa:EndpointReference>\r\n                        <wsa:Address>http://Passport.NET/tb</wsa:Address>\r\n                    </wsa:EndpointReference>\r\n                </wsp:AppliesTo>\r\n            </wst:RequestSecurityToken>\r\n            <wst:RequestSecurityToken Id=\"RST1\">\r\n                <wst:RequestType>http://schemas.xmlsoap.org/ws/2005/02/trust/Issue</wst:RequestType>\r\n                <wsp:AppliesTo>\r\n                    <wsa:EndpointReference>\r\n                        <wsa:Address>{3}</wsa:Address>\r\n                    </wsa:EndpointReference>\r\n                </wsp:AppliesTo>\r\n                <wsp:PolicyReference URI=\"{4}\"></wsp:PolicyReference>\r\n            </wst:RequestSecurityToken>\r\n        </ps:RequestMultipleSecurityTokens>\r\n    </s:Body>\r\n</s:Envelope>";

		private const string Rst2SamlEnvelope = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<s:Envelope xmlns:s=\"http://www.w3.org/2003/05/soap-envelope\" xmlns:wsse=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd\" xmlns:wsp=\"http://schemas.xmlsoap.org/ws/2004/09/policy\" xmlns:wsu=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd\" xmlns:wsa=\"http://www.w3.org/2005/08/addressing\" xmlns:wst=\"http://schemas.xmlsoap.org/ws/2005/02/trust\">\r\n    <s:Header>\r\n        <wsa:Action s:mustUnderstand=\"1\">http://schemas.xmlsoap.org/ws/2005/02/trust/RST/Issue</wsa:Action>\r\n        <wsa:To s:mustUnderstand=\"1\">{4}</wsa:To>\r\n        <wsa:MessageID>80730701</wsa:MessageID>\r\n        <ps:AuthInfo xmlns:ps=\"http://schemas.microsoft.com/Passport/SoapServices/PPCRL\" Id=\"PPAuthInfo\">\r\n            <ps:HostingApp>{0:B}</ps:HostingApp>\r\n            <ps:BinaryVersion>5</ps:BinaryVersion>\r\n            <ps:UIVersion>1</ps:UIVersion>\r\n            <ps:Cookies></ps:Cookies>\r\n            <ps:RequestParams>AQAAAAIAAABsYwQAAAAxMDMz</ps:RequestParams>\r\n        </ps:AuthInfo>\r\n        <wsse:Security>\r\n            {1}\r\n        </wsse:Security>\r\n    </s:Header>\r\n\r\n    <s:Body>\r\n        <ps:RequestMultipleSecurityTokens xmlns:ps=\"http://schemas.microsoft.com/Passport/SoapServices/PPCRL\" Id=\"RSTS\">\r\n            <wst:RequestSecurityToken Id=\"RST0\">\r\n                <wst:RequestType>http://schemas.xmlsoap.org/ws/2005/02/trust/Issue</wst:RequestType>\r\n                <wsp:AppliesTo>\r\n                    <wsa:EndpointReference>\r\n                        <wsa:Address>http://Passport.NET/tb</wsa:Address>\r\n                    </wsa:EndpointReference>\r\n                </wsp:AppliesTo>\r\n            </wst:RequestSecurityToken>\r\n            <wst:RequestSecurityToken Id=\"RST1\">\r\n                <wst:RequestType>http://schemas.xmlsoap.org/ws/2005/02/trust/Issue</wst:RequestType>\r\n                <wsp:AppliesTo>\r\n                    <wsa:EndpointReference>\r\n                        <wsa:Address>{2}</wsa:Address>\r\n                    </wsa:EndpointReference>\r\n                </wsp:AppliesTo>\r\n                <wsp:PolicyReference URI=\"{3}\"></wsp:PolicyReference>\r\n            </wst:RequestSecurityToken>\r\n        </ps:RequestMultipleSecurityTokens>\r\n    </s:Body>\r\n</s:Envelope>";

		private const int DefaultTimeout = 100000;

		private const string SoapRequestContentType = "application/soap+xml; charset=UTF-8";

		private static readonly Trace Tracer = ExTraceGlobals.LiveIDAuthenticationClientTracer;

		private Uri authServerUri;

		private static int maximumUriRedirections = 50;

		private HttpClient httpClient;

		private ICancelableAsyncResult pendingAsyncResult;

		private HttpSessionConfig httpSessionConfig;

		private int currentUrlRedirections;

		private object syncRoot = new object();

		private bool sessionClose;
	}
}
