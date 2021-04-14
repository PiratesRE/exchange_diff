using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics.Components.Security;

namespace Microsoft.Exchange.Security.Authentication.FederatedAuthService
{
	internal class SamlSTS : STSBase
	{
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

		public static bool VerifySamlXml { get; set; } = true;

		private SamlSTS()
		{
		}

		public SamlSTS(int traceId, LiveIdInstanceType instance, NamespaceStats stats) : base(traceId, instance, stats)
		{
		}

		public string ShibbolethLogonURI { get; set; }

		public string TokenIssuerURI { get; set; }

		public string AssertionConsumerService { get; set; }

		public override string StsTag
		{
			get
			{
				return "SHIBB";
			}
		}

		public IAsyncResult StartRequestChain(byte[] ansiUserName, byte[] ansiPassword, AsyncCallback callback, object state)
		{
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.traceId, "Entering SamlSTS.StartRequestChain()");
			if (ansiUserName == null)
			{
				ExTraceGlobals.AuthenticationTracer.TraceError((long)this.traceId, "ansiUserName is null");
				throw new ArgumentNullException("ansiUserName");
			}
			if (ansiPassword == null)
			{
				ExTraceGlobals.AuthenticationTracer.TraceError((long)this.traceId, "ansiPassword is null");
				throw new ArgumentNullException("ansiPassword");
			}
			this.traceUserName = Encoding.Default.GetString(ansiUserName);
			Interlocked.Increment(ref SamlSTS.numberOfOutgoingRequests);
			STSBase.counters.NumberOfOutgoingSamlStsRequests.RawValue = (long)SamlSTS.numberOfOutgoingRequests;
			ExTraceGlobals.AuthenticationTracer.Information<string, string>((long)this.traceId, "Creating http logon request to shibboleth STS '{0}' for \"{1}\"", this.ShibbolethLogonURI, this.traceUserName);
			this.stopwatch = Stopwatch.StartNew();
			byte[] array = null;
			byte[] array2 = null;
			char[] array3 = null;
			try
			{
				byte atSign = Convert.ToByte('@');
				int num = Array.FindIndex<byte>(ansiUserName, (byte s) => s == atSign);
				if (num == -1)
				{
					num = ansiUserName.Length;
				}
				array = new byte[num];
				Array.Copy(ansiUserName, array, num);
				int num2 = SamlSTS.httpAuthBytesP2.Length + array.Length + ansiPassword.Length;
				array2 = new byte[num2];
				array.CopyTo(array2, 0);
				SamlSTS.httpAuthBytesP2.CopyTo(array2, array.Length);
				ansiPassword.CopyTo(array2, array.Length + SamlSTS.httpAuthBytesP2.Length);
				array3 = new char[SamlSTS.httpAuthCharsP1.Length + (num2 + 2) / 3 * 4];
				SamlSTS.httpAuthCharsP1.CopyTo(array3, 0);
				Convert.ToBase64CharArray(array2, 0, array2.Length, array3, SamlSTS.httpAuthCharsP1.Length);
				this.shibbRequest = (HttpWebRequest)WebRequest.Create(this.ShibbolethLogonURI);
				this.shibbRequest.Method = "POST";
				this.shibbRequest.ContentType = "text/xml; charset=utf-8";
				this.shibbRequest.ServicePoint.Expect100Continue = false;
				this.shibbRequest.Headers[HttpRequestHeader.Authorization] = new string(array3);
				this.shibbRequest.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(AuthService.CertificateValidationCallBack);
				if (base.ExtraHeaders != null)
				{
					this.shibbRequest.Headers.Add(base.ExtraHeaders);
				}
			}
			finally
			{
				if (array != null)
				{
					Array.Clear(array, 0, array.Length);
				}
				if (array2 != null)
				{
					Array.Clear(array2, 0, array2.Length);
				}
				if (array3 != null)
				{
					Array.Clear(array3, 0, array2.Length);
				}
			}
			IAsyncResult result = this.shibbRequest.BeginGetRequestStream(callback, state);
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.traceId, "Leaving SamlSTS.StartRequestChain()");
			return result;
		}

		public IAsyncResult ProcessRequest(IAsyncResult asyncResult, AsyncCallback callback, object state)
		{
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.traceId, "Entering SamlSTS.ProcessRequest()");
			Stream stream = this.shibbRequest.EndGetRequestStream(asyncResult);
			string s = string.Format("<S:Envelope xmlns:S=\"http://schemas.xmlsoap.org/soap/envelope/\"><S:Body><samlp:AuthnRequest xmlns:samlp=\"urn:oasis:names:tc:SAML:2.0:protocol\" xmlns:saml=\"urn:oasis:names:tc:SAML:2.0:assertion\" ID=\"_{0}\" IssueInstant=\"{1}\" Version=\"2.0\" {2}><saml:Issuer>{3}</saml:Issuer></samlp:AuthnRequest></S:Body></S:Envelope>", new object[]
			{
				Guid.NewGuid().ToString(),
				(DateTime.UtcNow + base.ClockSkew).ToString("o"),
				this.AssertionConsumerService,
				this.TokenIssuerURI
			});
			STSBase.WriteBytes(stream, Encoding.UTF8.GetBytes(s));
			stream.Close();
			IAsyncResult result = this.shibbRequest.BeginGetResponse(callback, state);
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.traceId, "Leaving SamlSTS.ProcessRequest()");
			return result;
		}

		public byte[] ProcessResponse(IAsyncResult asyncResult)
		{
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.traceId, "Entering SamlSTS.ProcessResponse()");
			ExTraceGlobals.AuthenticationTracer.Information<string, string>((long)this.traceId, "Processing response to http logon request to Saml STS '{0}' for \"{1}\"", this.ShibbolethLogonURI, this.traceUserName);
			byte[] array = null;
			WebResponse webResponse = null;
			byte[] result;
			try
			{
				try
				{
					try
					{
						webResponse = this.shibbRequest.EndGetResponse(asyncResult);
					}
					catch (WebException ex)
					{
						if (ex.Status == WebExceptionStatus.ProtocolError)
						{
							switch (((HttpWebResponse)ex.Response).StatusCode)
							{
							case HttpStatusCode.Unauthorized:
							case HttpStatusCode.Forbidden:
								webResponse = ex.Response;
								base.IsBadCredentials = true;
								goto IL_DC;
							}
							throw;
						}
						string str;
						if (ex.Status == WebExceptionStatus.TrustFailure && AuthService.CertErrorCache.TryGetValue(this.shibbRequest, out str))
						{
							base.ErrorString += str;
							AuthService.CertErrorCache.Remove(this.shibbRequest);
						}
						throw;
					}
					IL_DC:;
				}
				finally
				{
					this.stopwatch.Stop();
					base.Latency = this.stopwatch.ElapsedMilliseconds;
				}
				STSBase.counters.AverageSamlStsResponseTime.IncrementBy(this.stopwatch.ElapsedMilliseconds);
				STSBase.counters.AverageSamlStsResponseTimeBase.Increment();
				ExTraceGlobals.AuthenticationTracer.TracePerformance<string, long>((long)this.traceId, "Shibboleth STS '{0}' responded in {1}ms", this.ShibbolethLogonURI, this.stopwatch.ElapsedMilliseconds);
				HttpWebResponse httpWebResponse = (HttpWebResponse)webResponse;
				TimeSpan clockSkew = base.ClockSkew;
				if (base.CalculateClockSkew(httpWebResponse))
				{
					ExTraceGlobals.AuthenticationTracer.TraceDebug<string, int>((long)this.traceId, "Clock skew between Shibb server {0} and local server is {1} seconds", this.ShibbolethLogonURI, base.ClockSkew.Seconds);
				}
				using (Stream responseStream = httpWebResponse.GetResponseStream())
				{
					using (StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8))
					{
						SafeXmlDocument safeXmlDocument = new SafeXmlDocument();
						safeXmlDocument.PreserveWhitespace = true;
						char[] array2 = new char[this.MaxResponseSize];
						int num = -1;
						int num2 = 0;
						while (!streamReader.EndOfStream && num2 < this.MaxResponseSize && num != 0)
						{
							num = streamReader.Read(array2, num2, this.MaxResponseSize - num2);
							num2 += num;
						}
						if (!streamReader.EndOfStream)
						{
							Interlocked.Increment(ref this.namespaceStats.TokenSize);
							this.namespaceStats.User = this.traceUserName;
							ExTraceGlobals.AuthenticationTracer.TraceError<int>((long)this.traceId, "Shibboleth STS returning more data than expected. More than {0} bytes", num2);
							base.ErrorString = "Shibboleth STS returned too much data";
							return array;
						}
						string text = new string(array2, 0, num2);
						ExTraceGlobals.AuthenticationTracer.TraceDebug<HttpStatusCode, string>((long)this.traceId, "Shibboleth STS returned response {0:d} {1}", httpWebResponse.StatusCode, text);
						if (httpWebResponse.StatusCode != HttpStatusCode.OK)
						{
							base.ErrorString = ((int)httpWebResponse.StatusCode).ToString();
							Interlocked.Increment(ref this.namespaceStats.BadPassword);
							this.namespaceStats.User = this.traceUserName;
							ExTraceGlobals.AuthenticationTracer.Information<string, string>((long)this.traceId, "Logon failed to Shibboleth STS '{0}' for \"{1}\"", this.ShibbolethLogonURI, this.traceUserName);
							ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.traceId, "Leaving SamlSTS.ProcessResponse()");
							return array;
						}
						try
						{
							safeXmlDocument.LoadXml(text);
						}
						catch (XmlException ex2)
						{
							base.ErrorString = string.Format("Shibboleth STS '{0}' has malformed RST response for user \"{1}\".  Exception {2} XML response {3}", new object[]
							{
								this.ShibbolethLogonURI,
								this.traceUserName,
								ex2.ToString(),
								text
							});
							ExTraceGlobals.AuthenticationTracer.TraceError((long)this.traceId, base.ErrorString);
							throw;
						}
						XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(safeXmlDocument.NameTable);
						xmlNamespaceManager.AddNamespace("s", "http://schemas.xmlsoap.org/soap/envelope/");
						xmlNamespaceManager.AddNamespace("saml2p", "urn:oasis:names:tc:SAML:2.0:protocol");
						xmlNamespaceManager.AddNamespace("saml2", "urn:oasis:names:tc:SAML:2.0:assertion");
						xmlNamespaceManager.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
						XmlNode xmlNode = safeXmlDocument.SelectSingleNode("/s:Envelope/s:Body/saml2p:Response", xmlNamespaceManager);
						if (xmlNode == null)
						{
							if (safeXmlDocument.SelectSingleNode("/s:Envelope", xmlNamespaceManager) == null)
							{
								base.ErrorString = "SamlSTS response is XML but not SOAP " + text;
							}
							else
							{
								xmlNode = safeXmlDocument.SelectSingleNode("/s:Envelope/s:Body/s:Fault", xmlNamespaceManager);
								if (xmlNode != null)
								{
									base.ErrorString = xmlNode.OuterXml;
								}
								else
								{
									base.ErrorString = "SamlSTS response is missing saml2p:Response" + text;
								}
								if ((base.ClockSkew - clockSkew).Duration() >= base.ClockSkewThreshold)
								{
									ExTraceGlobals.AuthenticationTracer.Information<string, int>((long)this.traceId, "Clock skew for shibboleth endpoint '{0}' changed more than '{1}' minutes - possible clock skew failure", this.ShibbolethLogonURI, base.ClockSkew.Minutes);
									base.PossibleClockSkew = true;
								}
							}
							Interlocked.Increment(ref this.namespaceStats.Failed);
							this.namespaceStats.User = this.traceUserName;
							ExTraceGlobals.AuthenticationTracer.TraceWarning<string, string, string>((long)this.traceId, "Shibboleth STS '{0}' has missing saml assertion for user \"{1}\". XML = {2}", this.ShibbolethLogonURI, this.traceUserName, text);
							ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.traceId, "Leaving SamlSTS.ProcessResponse()");
							return array;
						}
						array = Encoding.UTF8.GetBytes(xmlNode.OuterXml);
						ExTraceGlobals.AuthenticationTracer.Information<string, string>((long)this.traceId, "Successfully processed response to http logon request to Shibboleth STS '{0}' for \"{1}\"", this.ShibbolethLogonURI, this.traceUserName);
						if (SamlSTS.VerifySamlXml)
						{
							XmlNode xmlNode2 = safeXmlDocument.SelectSingleNode("/s:Envelope/s:Body/saml2p:Response/ds:Signature", xmlNamespaceManager);
							XmlNode xmlNode3 = safeXmlDocument.SelectSingleNode("/s:Envelope/s:Body/saml2p:Response/saml2:Assertion/ds:Signature", xmlNamespaceManager);
							SignedXml signedXml = new SignedXml(safeXmlDocument);
							if (xmlNode2 != null)
							{
								signedXml.LoadXml((XmlElement)xmlNode2);
								if (!signedXml.CheckSignature())
								{
									base.ErrorString = "Saml Response has invalid signature" + text;
									Interlocked.Increment(ref this.namespaceStats.Failed);
									this.namespaceStats.User = this.traceUserName;
									ExTraceGlobals.AuthenticationTracer.TraceError<string, string>((long)this.traceId, "SAML Response failed signature check for server {0} and user \"{1}\"", this.ShibbolethLogonURI, this.traceUserName);
									ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.traceId, "Leaving SamlSTS.ProcessResponse()");
									return null;
								}
								ExTraceGlobals.AuthenticationTracer.TraceDebug<string, string>((long)this.traceId, "SAML Response passed signature check for server {0} and user \"{1}\"", this.ShibbolethLogonURI, this.traceUserName);
							}
							if (xmlNode3 != null)
							{
								signedXml.LoadXml((XmlElement)xmlNode3);
								if (!signedXml.CheckSignature())
								{
									base.ErrorString = "Saml Assertion has invalid signature" + text;
									Interlocked.Increment(ref this.namespaceStats.Failed);
									this.namespaceStats.User = this.traceUserName;
									ExTraceGlobals.AuthenticationTracer.TraceError<string, string>((long)this.traceId, "SAML Assertion failed signature check for server {0} and user \"{1}\"", this.ShibbolethLogonURI, this.traceUserName);
									ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.traceId, "Leaving SamlSTS.ProcessResponse()");
									return null;
								}
								ExTraceGlobals.AuthenticationTracer.TraceDebug<string, string>((long)this.traceId, "SAML Assertion passed signature check for server {0} and user \"{1}\"", this.ShibbolethLogonURI, this.traceUserName);
							}
						}
					}
				}
				ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.traceId, "Leaving SamlSTS.ProcessResponse()");
				result = array;
			}
			finally
			{
				if (webResponse != null)
				{
					((IDisposable)webResponse).Dispose();
				}
			}
			return result;
		}

		public void Abort()
		{
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.traceId, "Entering SamlSTS.Abort()");
			if (this.shibbRequest != null)
			{
				ExTraceGlobals.AuthenticationTracer.Information<string, string>((long)this.traceId, "Aborting http logon request to Shibboleth STS '{0}' for \"{1}\"", this.ShibbolethLogonURI, this.traceUserName);
				this.shibbRequest.Abort();
			}
			ExTraceGlobals.AuthenticationTracer.TraceFunction((long)this.traceId, "Leaving SamlSTS.Abort()");
		}

		private const string shibbBody = "<S:Envelope xmlns:S=\"http://schemas.xmlsoap.org/soap/envelope/\"><S:Body><samlp:AuthnRequest xmlns:samlp=\"urn:oasis:names:tc:SAML:2.0:protocol\" xmlns:saml=\"urn:oasis:names:tc:SAML:2.0:assertion\" ID=\"_{0}\" IssueInstant=\"{1}\" Version=\"2.0\" {2}><saml:Issuer>{3}</saml:Issuer></samlp:AuthnRequest></S:Body></S:Envelope>";

		private static int numberOfOutgoingRequests;

		private static readonly char[] httpAuthCharsP1 = "Basic ".ToCharArray();

		private static readonly byte[] httpAuthBytesP2 = new byte[]
		{
			Convert.ToByte(':')
		};

		private int maxResponseSize = 131072;

		private HttpWebRequest shibbRequest;

		private Stopwatch stopwatch;

		private string traceUserName;
	}
}
