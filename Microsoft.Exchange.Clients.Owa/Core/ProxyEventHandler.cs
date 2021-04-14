using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Web;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Clients.EventLogs;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[OwaEventNamespace("HttpProxy")]
	internal sealed class ProxyEventHandler : OwaEventHandlerBase
	{
		public static void Initialize()
		{
			CertificateValidationManager.RegisterCallback("OWA", new RemoteCertificateValidationCallback(ProxyEventHandler.SslCertificateValidationCallback));
			CertificateValidationManager.RegisterCallback("OWA_IgnoreCertErrors", new RemoteCertificateValidationCallback(ProxyEventHandler.SslCertificateIgnoreCallback));
			ServicePointManager.DefaultConnectionLimit = Globals.ServicePointConnectionLimit;
		}

		public static bool SslCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			ExTraceGlobals.ProxyCallTracer.TraceDebug(0L, "ProxyEventHandler.SslCertificateValidationCallback");
			if (!OwaRegistryKeys.AllowInternalUntrustedCerts)
			{
				ExTraceGlobals.ProxyDataTracer.TraceDebug<SslPolicyErrors>(0L, "Trusted certs are required for proxying, sslPolicyErrors={0}", sslPolicyErrors);
				return sslPolicyErrors == SslPolicyErrors.None;
			}
			ExTraceGlobals.ProxyDataTracer.TraceDebug<SslPolicyErrors>(0L, "Trusted certs are NOT required for proxying, sslPolicyErrors={0}", sslPolicyErrors);
			return true;
		}

		public static bool SslCertificateIgnoreCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			ExTraceGlobals.ProxyCallTracer.TraceDebug(0L, "ProxyEventHandler.SslCertificateIgnoreCallback");
			ExTraceGlobals.ProxyDataTracer.TraceDebug<SslPolicyErrors>(0L, "Trusted certs are NOT required for proxying, sslPolicyErrors={0}", sslPolicyErrors);
			return true;
		}

		[OwaEvent("ProxyRequest", true)]
		[OwaEventVerb(OwaEventVerb.Post | OwaEventVerb.Get)]
		[OwaEventParameter("pru", typeof(Uri))]
		public IAsyncResult BeginProxyRequest(AsyncCallback callback, object extraData)
		{
			ExTraceGlobals.ProxyCallTracer.TraceDebug((long)this.GetHashCode(), "ProxyEventHandler.BeginProxyRequest");
			base.DontWriteHeaders = true;
			this.asyncResult = new OwaAsyncResult(callback, extraData);
			this.proxyRequestUrl = (Uri)base.GetParameter("pru");
			if (base.OwaContext.TryGetUserContext() != null)
			{
				int num;
				base.UserContext.DangerousBeginUnlockedAction(false, out num);
				if (num != 1)
				{
					ExWatson.SendReport(new InvalidOperationException("Thread held more than 1 lock before async operation"), ReportOptions.None, null);
				}
			}
			this.SendProxyRequest();
			return this.asyncResult;
		}

		[OwaEvent("ProxyRequest", true)]
		public void EndProxyRequest(IAsyncResult asyncResult)
		{
			ExTraceGlobals.ProxyCallTracer.TraceDebug((long)this.GetHashCode(), "ProxyEventHandler.EndProxyRequest");
			this.CommonEndProxyRequest(asyncResult);
		}

		private static bool IsConnectionFailure(WebException exception)
		{
			switch (exception.Status)
			{
			case WebExceptionStatus.NameResolutionFailure:
			case WebExceptionStatus.ConnectFailure:
			case WebExceptionStatus.SendFailure:
			case WebExceptionStatus.ConnectionClosed:
			case WebExceptionStatus.ServerProtocolViolation:
			case WebExceptionStatus.KeepAliveFailure:
			case WebExceptionStatus.Timeout:
			case WebExceptionStatus.ProxyNameResolutionFailure:
				return true;
			case WebExceptionStatus.ProtocolError:
			{
				HttpWebResponse httpWebResponse = (HttpWebResponse)exception.Response;
				return httpWebResponse.StatusCode == HttpStatusCode.ServiceUnavailable || httpWebResponse.StatusCode == HttpStatusCode.BadGateway;
			}
			}
			return false;
		}

		private void CommonEndProxyRequest(IAsyncResult asyncResult)
		{
			ExTraceGlobals.ProxyCallTracer.TraceDebug((long)this.GetHashCode(), "ProxyEventHandler.EndProxyRequest");
			try
			{
				OwaAsyncResult owaAsyncResult = (OwaAsyncResult)asyncResult;
				if (owaAsyncResult.Exception != null)
				{
					ExTraceGlobals.ProxyTracer.TraceDebug((long)this.GetHashCode(), "An exception was thrown during the processing of the async request");
					Utilities.HandleException(base.OwaContext, owaAsyncResult.Exception);
				}
			}
			finally
			{
				this.Dispose();
			}
		}

		private void SendProxyRequest()
		{
			ExTraceGlobals.ProxyCallTracer.TraceDebug((long)this.GetHashCode(), "ProxyEventHandler.SendProxyRequest");
			if (base.OwaContext.SecondCasUri.ProxyPingResult == ProxyPingResult.Unknown)
			{
				this.SendProxyPingRequest();
				return;
			}
			HttpWebRequest proxyRequestInstance = ProxyUtilities.GetProxyRequestInstance(this.OriginalRequest, base.OwaContext, this.proxyRequestUrl);
			proxyRequestInstance.Method = ((base.Verb == OwaEventVerb.Post) ? "POST" : "GET");
			this.proxyRequest = proxyRequestInstance;
			if (base.Verb == OwaEventVerb.Post)
			{
				this.proxyRequest.BeginGetRequestStream(new AsyncCallback(this.GetProxyRequestStreamCallback), this);
				return;
			}
			this.requestTimedOut = false;
			IAsyncResult asyncResult = ProxyUtilities.BeginGetResponse(this.proxyRequest, new AsyncCallback(this.GetProxyResponseCallback), this, out this.requestClock);
			this.timeoutWaitHandle = ThreadPool.RegisterWaitForSingleObject(asyncResult.AsyncWaitHandle, new WaitOrTimerCallback(this.RequestTimeoutCallback), asyncResult, (long)this.HttpContext.Server.ScriptTimeout * 1000L, true);
		}

		private void SendProxyPingRequest()
		{
			ExTraceGlobals.ProxyCallTracer.TraceDebug((long)this.GetHashCode(), "ProxyEventHandler.SendProxyPingRequest");
			this.proxyPingRequest = new ProxyPingRequest();
			this.proxyPingRequest.BeginSend(base.OwaContext, new AsyncCallback(this.SendProxyPingRequestCallback), this);
		}

		private void SendProxyPingRequestCallback(IAsyncResult asyncResult)
		{
			ExTraceGlobals.ProxyCallTracer.TraceDebug((long)this.GetHashCode(), "ProxyEventHandler.SendProxyPingRequestCallback");
			try
			{
				HttpWebResponse httpWebResponse = this.proxyPingRequest.EndSend(asyncResult);
				if (httpWebResponse.StatusCode == (HttpStatusCode)242)
				{
					ExTraceGlobals.ProxyTracer.TraceDebug((long)this.GetHashCode(), "Proxy ping succeeded");
					try
					{
						ServerVersion serverVersion = null;
						if (!this.EnsureCasCompatibility(httpWebResponse, out serverVersion))
						{
							ExTraceGlobals.ProxyCallTracer.TraceDebug((long)this.GetHashCode(), "CAS versions are not compatible");
							OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_ProxyErrorCASCompatibility, base.OwaContext.SecondCasUri.ToString(), new object[]
							{
								base.OwaContext.LocalHostName,
								Globals.LocalHostVersion.ToString(),
								base.OwaContext.SecondCasUri.Uri.Host,
								serverVersion.ToString()
							});
							throw new OwaProxyException("Cas servers have incompatible versions", LocalizedStrings.GetNonEncoded(-1191925045));
						}
						goto IL_113;
					}
					finally
					{
						httpWebResponse.Close();
						httpWebResponse = null;
					}
					goto IL_F4;
					IL_113:
					goto IL_15E;
				}
				IL_F4:
				ExTraceGlobals.ProxyTracer.TraceDebug((long)this.GetHashCode(), "The proxy ping didn't succeed, streaming the response directly to the client...");
				this.CopyProxyResponse(httpWebResponse);
				return;
			}
			catch (OwaAsyncOperationException ex)
			{
				WebException ex2 = ex.InnerException as WebException;
				if (ex2 != null)
				{
					this.HandleProxyPingWebException(ex2, ex);
					return;
				}
				this.asyncResult.CompleteRequest(false, ex);
				return;
			}
			catch (WebException ex3)
			{
				this.HandleProxyPingWebException(ex3, ex3);
				return;
			}
			catch (Exception exception)
			{
				this.asyncResult.CompleteRequest(false, exception);
				return;
			}
			IL_15E:
			base.OwaContext.SecondCasUri.ProxyPingResult = ProxyPingResult.Compatible;
			try
			{
				this.SendProxyRequest();
			}
			catch (Exception exception2)
			{
				this.asyncResult.CompleteRequest(false, exception2);
			}
		}

		private void HandleProxyPingWebException(WebException webException, Exception outerException)
		{
			if (outerException == null)
			{
				outerException = webException;
			}
			if (this.CheckAndHandleConnectionFailure(webException, outerException))
			{
				return;
			}
			if (webException.Status != WebExceptionStatus.ProtocolError)
			{
				this.HandleWebExceptionDefault(webException, outerException);
				return;
			}
			if (this.CheckAndHandleWebException40X(webException, outerException))
			{
				return;
			}
			this.HandleProtocolExceptionDefault(webException);
		}

		private bool EnsureCasCompatibility(HttpWebResponse response, out ServerVersion secondCasVersion)
		{
			ExTraceGlobals.ProxyCallTracer.TraceDebug((long)this.GetHashCode(), "ProxyEventHandler.EnsureCasCompatibility");
			secondCasVersion = null;
			string versionString = response.Headers["X-OWA-Version"];
			secondCasVersion = ServerVersion.CreateFromVersionString(versionString);
			if (secondCasVersion == null)
			{
				throw new OwaInvalidOperationException(string.Format("The format of the header {0}is wrong", "X-OWA-ProxyVersion"));
			}
			int num = ServerVersion.Compare(Globals.LocalHostVersion, secondCasVersion);
			if (num == 0)
			{
				ExTraceGlobals.ProxyTracer.TraceDebug((long)this.GetHashCode(), "Both CAS servers are the same version");
				return true;
			}
			if (num > 0)
			{
				ExTraceGlobals.ProxyTracer.TraceDebug((long)this.GetHashCode(), "Proxy CAS version is larger than second CAS version");
				if (ProxyUtilities.IsVersionFolderInProxy(secondCasVersion))
				{
					ExTraceGlobals.ProxyTracer.TraceDebug((long)this.GetHashCode(), "Proxy CAS has the version folder corresponding to the second CAS version");
					return true;
				}
				ExTraceGlobals.ProxyTracer.TraceDebug((long)this.GetHashCode(), "Proxy doesn't have the version folder corresponding to the second CAS version");
			}
			return false;
		}

		private void GetProxyRequestStreamCallback(IAsyncResult asyncResult)
		{
			try
			{
				this.proxyRequestStream = this.proxyRequest.EndGetRequestStream(asyncResult);
				this.OriginalRequestStream.Seek(0L, SeekOrigin.Begin);
				this.proxyStreamCopy = new ProxyStreamCopy(this.OriginalRequestStream, this.proxyRequestStream, StreamCopyMode.SyncReadAsyncWrite);
				this.proxyStreamCopy.BeginCopy(new AsyncCallback(this.CopyOriginalRequestStreamCallback), this);
			}
			catch (WebException webException)
			{
				if (!this.CheckAndHandleConnectionFailure(webException))
				{
					this.HandleWebExceptionDefault(webException);
				}
			}
			catch (IOException)
			{
				this.asyncResult.CompleteRequest(false, null);
			}
			catch (Exception exception)
			{
				this.asyncResult.CompleteRequest(false, exception);
			}
		}

		private void CopyOriginalRequestStreamCallback(IAsyncResult result)
		{
			try
			{
				this.proxyStreamCopy.EndCopy(result);
				this.copyingOriginalRequestComplete = true;
				this.requestTimedOut = false;
				IAsyncResult asyncResult = ProxyUtilities.BeginGetResponse(this.proxyRequest, new AsyncCallback(this.GetProxyResponseCallback), this, out this.requestClock);
				this.timeoutWaitHandle = ThreadPool.RegisterWaitForSingleObject(asyncResult.AsyncWaitHandle, new WaitOrTimerCallback(this.RequestTimeoutCallback), asyncResult, (long)this.HttpContext.Server.ScriptTimeout * 1000L, true);
			}
			catch (WebException webException)
			{
				if (!this.CheckAndHandleConnectionFailure(webException))
				{
					this.HandleWebExceptionDefault(webException);
				}
			}
			catch (Exception exception)
			{
				this.asyncResult.CompleteRequest(false, exception);
			}
		}

		private void GetProxyResponseCallback(IAsyncResult asyncResult)
		{
			ExTraceGlobals.ProxyCallTracer.TraceDebug((long)this.GetHashCode(), "ProxyEventHandler.GetProxyResponseCallback");
			lock (this)
			{
				if (!this.requestTimedOut)
				{
					try
					{
						if (this.timeoutWaitHandle != null)
						{
							this.timeoutWaitHandle.Unregister(null);
							this.timeoutWaitHandle = null;
						}
						HttpWebResponse response = ProxyUtilities.EndGetResponse(this.proxyRequest, asyncResult, this.requestClock);
						this.CopyProxyResponse(response);
					}
					catch (WebException ex)
					{
						if (!this.CheckAndHandleConnectionFailure(ex))
						{
							if (ex.Status == WebExceptionStatus.ProtocolError)
							{
								if (!this.CheckAndHandleWebException40X(ex))
								{
									HttpWebResponse httpWebResponse = (HttpWebResponse)ex.Response;
									int statusCode = (int)httpWebResponse.StatusCode;
									if (statusCode == 441)
									{
										ExTraceGlobals.ProxyTracer.TraceDebug((long)this.GetHashCode(), "The proxy returned 441, this means we need to attempt to do a proxy logon");
										try
										{
											this.SendProxyLogonRequest();
											return;
										}
										catch (Exception exception)
										{
											this.asyncResult.CompleteRequest(false, exception);
											return;
										}
									}
									if (statusCode == 442)
									{
										ExTraceGlobals.ProxyTracer.TraceDebug((long)this.GetHashCode(), "The proxy returned 442, this means the user's language or timezone are invalid");
										this.OriginalResponse.Redirect(OwaUrl.LanguagePage.GetExplicitUrl(this.OriginalRequest), false);
										base.OwaContext.HttpContext.ApplicationInstance.CompleteRequest();
										this.asyncResult.CompleteRequest(false);
									}
									else
									{
										this.HandleProtocolExceptionDefault(ex);
									}
								}
							}
							else
							{
								this.HandleWebExceptionDefault(ex);
							}
						}
					}
					catch (Exception exception2)
					{
						this.asyncResult.CompleteRequest(false, exception2);
					}
				}
			}
		}

		internal void HandleProtocolExceptionDefault(WebException webException)
		{
			if (webException.Status != WebExceptionStatus.ProtocolError)
			{
				throw new ArgumentException("The exception status must be Protocol Error");
			}
			ExTraceGlobals.ProxyTracer.TraceDebug((long)this.GetHashCode(), "The proxy returned a failed HTTP status, we will stream the response directly back to the client. Http Code:" + webException.Status);
			try
			{
				HttpWebResponse response = (HttpWebResponse)webException.Response;
				this.CopyProxyResponse(response);
			}
			catch (Exception exception)
			{
				this.asyncResult.CompleteRequest(false, exception);
			}
		}

		private void SendProxyLogonRequest()
		{
			ExTraceGlobals.ProxyCallTracer.TraceDebug((long)this.GetHashCode(), "ProxyEventHandler.SendProxyLogonRequest");
			this.proxyLogonAttempts++;
			if (this.proxyLogonAttempts > 2)
			{
				this.asyncResult.Exception = new OwaInvalidOperationException("Both attempts to send the proxy request failed -- the server returned HTTP " + 441 + " (need identity) twice in a row");
				this.asyncResult.CompleteRequest(false);
				return;
			}
			this.proxyLogonRequest = new ProxyLogonRequest();
			SerializedClientSecurityContext serializedContext = SerializedClientSecurityContext.CreateFromOwaIdentity(base.OwaContext.LogonIdentity);
			this.proxyLogonRequest.BeginSend(base.OwaContext, this.OriginalRequest, serializedContext, new AsyncCallback(this.ProxyLogonCallback), this);
		}

		private void ProxyLogonCallback(IAsyncResult asyncResult)
		{
			ExTraceGlobals.ProxyCallTracer.TraceDebug((long)this.GetHashCode(), "ProxyEventHandler.ProxyLogonCallback");
			try
			{
				HttpWebResponse httpWebResponse = this.proxyLogonRequest.EndSend(asyncResult);
				if (httpWebResponse.StatusCode != (HttpStatusCode)241)
				{
					ExTraceGlobals.ProxyTracer.TraceDebug((long)this.GetHashCode(), "The proxy logon didn't succeed, streaming the response directly to the client...");
					this.CopyProxyResponse(httpWebResponse);
					return;
				}
				httpWebResponse.Close();
			}
			catch (OwaAsyncOperationException ex)
			{
				WebException ex2 = ex.InnerException as WebException;
				if (ex2 != null)
				{
					this.HandleProxyLogonWebException(ex2, ex);
					return;
				}
				this.asyncResult.CompleteRequest(false, ex);
				return;
			}
			catch (WebException ex3)
			{
				this.HandleProxyLogonWebException(ex3, ex3);
				return;
			}
			catch (Exception exception)
			{
				this.asyncResult.CompleteRequest(false, exception);
				return;
			}
			try
			{
				this.SendProxyRequest();
			}
			catch (Exception exception2)
			{
				this.asyncResult.CompleteRequest(false, exception2);
			}
		}

		private void HandleProxyLogonWebException(WebException webException, Exception outerException)
		{
			if (outerException == null)
			{
				outerException = webException;
			}
			if (this.CheckAndHandleConnectionFailure(webException, outerException))
			{
				return;
			}
			if (webException.Status != WebExceptionStatus.ProtocolError)
			{
				this.HandleWebExceptionDefault(webException, outerException);
				return;
			}
			if (this.CheckAndHandleWebException40X(webException, outerException))
			{
				return;
			}
			HttpWebResponse httpWebResponse = (HttpWebResponse)webException.Response;
			int statusCode = (int)httpWebResponse.StatusCode;
			if (statusCode == 442)
			{
				ExTraceGlobals.ProxyTracer.TraceDebug((long)this.GetHashCode(), "The proxy logon didn't succeed because the second CAS returned 442, this means either the user's timeZoneKeyName or the culture are invalid.  Redirecting client to language page...");
				this.OriginalResponse.Redirect(OwaUrl.LanguagePage.GetExplicitUrl(this.OriginalRequest), false);
				base.OwaContext.HttpContext.ApplicationInstance.CompleteRequest();
				this.asyncResult.CompleteRequest(false);
				return;
			}
			this.HandleProtocolExceptionDefault(webException);
		}

		private void CopyProxyResponse(HttpWebResponse response)
		{
			this.copyProxyAttempts++;
			this.proxyResponse = response;
			this.proxyResponseStream = this.proxyResponse.GetResponseStream();
			bool flag = false;
			for (int i = 0; i < this.proxyResponse.Headers.Count; i++)
			{
				string text = this.proxyResponse.Headers.Keys[i];
				if (ProxyUtilities.ShouldCopyProxyResponseHeader(text))
				{
					this.OriginalResponse.AddHeader(text, this.proxyResponse.Headers[text]);
				}
				if (string.Compare(text, "mailboxCrossSiteFailover", true, CultureInfo.InvariantCulture) == 0)
				{
					flag = true;
					Utilities.DeleteCookie(this.OriginalResponse, UserContextCookie.GetUserContextCookie(base.OwaContext).CookieName);
				}
			}
			if (!flag && Globals.OwaVDirType == OWAVDirType.OWA)
			{
				ProxyUtilities.UpdateProxyUserContextIdFromResponse(this.proxyResponse, base.OwaContext.UserContext);
			}
			ProxyUtilities.UpdateProxyClientDataCollectingCookieFromResponse(this.proxyResponse, this.OriginalResponse);
			this.OriginalResponse.StatusCode = (int)this.proxyResponse.StatusCode;
			this.OriginalResponse.ContentType = this.proxyResponse.ContentType;
			if ("chunked".Equals(this.proxyResponse.Headers["Transfer-Encoding"], StringComparison.OrdinalIgnoreCase))
			{
				this.OriginalResponse.BufferOutput = false;
			}
			this.proxyStreamCopy = new ProxyStreamCopy(this.proxyResponseStream, this.OriginalResponseStream, StreamCopyMode.AsyncReadSyncWrite);
			this.proxyStreamCopy.BeginCopy(new AsyncCallback(this.CopyProxyResponseStreamCallback), this);
		}

		private void CopyProxyResponseStreamCallback(IAsyncResult asyncResult)
		{
			try
			{
				int num = this.proxyStreamCopy.EndCopy(asyncResult);
				this.copyingProxyResponseComplete = true;
				if (num > 0 && Globals.ArePerfCountersEnabled)
				{
					OwaSingleCounters.ProxyResponseBytes.IncrementBy((long)num);
				}
			}
			catch (WebException webException)
			{
				if (this.CheckAndHandleConnectionFailure(webException))
				{
					return;
				}
				this.HandleWebExceptionDefault(webException);
				return;
			}
			catch (IOException)
			{
				this.asyncResult.CompleteRequest(false);
				return;
			}
			catch (Exception exception)
			{
				this.asyncResult.CompleteRequest(false, exception);
				return;
			}
			this.asyncResult.CompleteRequest(false);
		}

		[OwaEventVerb(OwaEventVerb.Post)]
		[OwaEvent("LanguagePost", true)]
		[OwaEventParameter("opt", typeof(bool))]
		[OwaEventParameter("identity", typeof(OwaIdentity))]
		[OwaEventParameter("tzid", typeof(string))]
		[OwaEventParameter("culture", typeof(CultureInfo))]
		[OwaEventParameter("destination", typeof(string), false, true)]
		public IAsyncResult BeginProxyLanguagePostRequest(AsyncCallback callback, object extraData)
		{
			ExTraceGlobals.ProxyCallTracer.TraceDebug((long)this.GetHashCode(), "ProxyEventHandler.BeginProxyLanguagePostRequest");
			base.DontWriteHeaders = true;
			this.asyncResult = new OwaAsyncResult(callback, extraData);
			if (base.UserContext != null)
			{
				int num;
				base.UserContext.DangerousBeginUnlockedAction(false, out num);
			}
			OwaIdentity identity = (OwaIdentity)base.GetParameter("identity");
			string timeZoneKeyName = (string)base.GetParameter("tzid");
			CultureInfo culture = (CultureInfo)base.GetParameter("culture");
			bool isOptimized = (bool)base.GetParameter("opt");
			string destination = (string)base.GetParameter("destination");
			this.proxyLanguagePostRequest = new ProxyLanguagePostRequest();
			this.proxyLanguagePostRequest.BeginSend(base.OwaContext, this.OriginalRequest, identity, culture, timeZoneKeyName, isOptimized, destination, new AsyncCallback(this.SendLanguagePostCallback), this);
			return this.asyncResult;
		}

		[OwaEvent("LanguagePost", true)]
		public void EndProxyLanguagePostRequest(IAsyncResult asyncResult)
		{
			ExTraceGlobals.ProxyCallTracer.TraceDebug((long)this.GetHashCode(), "ProxyEventHandler.EndProxyLanguagePostRequest");
			this.CommonEndProxyRequest(asyncResult);
		}

		private void SendLanguagePostCallback(IAsyncResult asyncResult)
		{
			ExTraceGlobals.ProxyCallTracer.TraceDebug((long)this.GetHashCode(), "ProxyEventHandler.SendLanguagePostCallback");
			try
			{
				HttpWebResponse response = this.proxyLanguagePostRequest.EndSend(asyncResult);
				this.CopyProxyResponse(response);
			}
			catch (WebException ex)
			{
				if (!this.CheckAndHandleConnectionFailure(ex))
				{
					if (ex.Status == WebExceptionStatus.ProtocolError)
					{
						if (!this.CheckAndHandleWebException40X(ex))
						{
							this.HandleProtocolExceptionDefault(ex);
						}
					}
					else
					{
						this.HandleWebExceptionDefault(ex);
					}
				}
			}
			catch (OwaAsyncOperationException ex2)
			{
				WebException ex3 = ex2.InnerException as WebException;
				if (ex3 != null)
				{
					this.HandleProtocolExceptionDefault(ex3);
				}
				else
				{
					this.asyncResult.CompleteRequest(false, ex2);
				}
			}
			catch (Exception exception)
			{
				this.asyncResult.CompleteRequest(false, exception);
			}
		}

		private bool CheckAndHandleWebException40X(WebException webException)
		{
			return this.CheckAndHandleWebException40X(webException, null);
		}

		private bool CheckAndHandleWebException40X(WebException webException, Exception outerException)
		{
			if (webException.Status != WebExceptionStatus.ProtocolError)
			{
				return false;
			}
			HttpWebResponse httpWebResponse = (HttpWebResponse)webException.Response;
			if (httpWebResponse.StatusCode == HttpStatusCode.Forbidden)
			{
				OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_ProxyErrorSslConnection, base.OwaContext.SecondCasUri.ToString(), new object[]
				{
					base.OwaContext.LocalHostName,
					base.OwaContext.SecondCasUri.ToString(),
					base.OwaContext.SecondCasUri.ToString()
				});
				OwaProxyException exception = new OwaProxyException("The CAS server is most likely not configured for SSL (it returned a 403)", LocalizedStrings.GetNonEncoded(-750997814), outerException, false);
				this.asyncResult.CompleteRequest(false, exception);
				return true;
			}
			if (httpWebResponse.StatusCode == HttpStatusCode.Unauthorized)
			{
				OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_ProxyErrorAuthenticationToCas2Failure, base.OwaContext.SecondCasUri.ToString(), new object[]
				{
					base.OwaContext.LocalHostName,
					base.OwaContext.SecondCasUri.ToString()
				});
				OwaProxyException exception2 = new OwaProxyException("The proxy CAS failed to authenticate to the second CAS (it returned a 401)", LocalizedStrings.GetNonEncoded(-1102013722), outerException, false);
				this.asyncResult.CompleteRequest(false, exception2);
				return true;
			}
			return false;
		}

		private void HandleWebExceptionDefault(WebException webException)
		{
			this.HandleWebExceptionDefault(webException, null);
		}

		private void HandleWebExceptionDefault(WebException webException, Exception outerException)
		{
			if (webException.Status == WebExceptionStatus.TrustFailure)
			{
				OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_ProxyErrorSslTrustFailure, base.OwaContext.SecondCasUri.ToString(), new object[]
				{
					base.OwaContext.LocalHostName,
					base.OwaContext.SecondCasUri.ToString()
				});
				OwaProxyException exception = new OwaProxyException("Error establishing SSL connection", LocalizedStrings.GetNonEncoded(-750997814), outerException, false);
				this.asyncResult.CompleteRequest(false, exception);
				return;
			}
			this.asyncResult.CompleteRequest(false, outerException);
		}

		private bool CheckAndHandleConnectionFailure(WebException webException)
		{
			return this.CheckAndHandleConnectionFailure(webException, null);
		}

		private bool CheckAndHandleConnectionFailure(WebException webException, Exception outerException)
		{
			ExTraceGlobals.ProxyCallTracer.TraceDebug((long)this.GetHashCode(), "ProxyEventHandler.HandleConnectionFailure");
			if (outerException == null)
			{
				outerException = webException;
			}
			if (!ProxyEventHandler.IsConnectionFailure(webException))
			{
				return false;
			}
			ExTraceGlobals.ProxyTracer.TraceDebug((long)this.GetHashCode(), "The proxy attempt failed, so we'll fail over to another CAS");
			this.attemptedProxyUriCount++;
			if (base.OwaContext.ProxyUriQueue.Count == 1 || this.attemptedProxyUriCount > base.OwaContext.ProxyUriQueue.Count)
			{
				OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_ProxyErrorCASFailoverAllAttemptsFailed, base.OwaContext.SecondCasUri.ToString(), new object[]
				{
					base.OwaContext.LocalHostName,
					base.OwaContext.SecondCasUri.ToString(),
					string.Format("webExceptionStatus={0}", webException.Status)
				});
				OwaProxyException exception = new OwaProxyException("None of the CAS servers are responding", LocalizedStrings.GetNonEncoded(-200732695), outerException, false);
				this.asyncResult.CompleteRequest(false, exception);
				return true;
			}
			ProxyUri secondCasUri = base.OwaContext.SecondCasUri;
			this.GetNextFailoverCas();
			OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_ProxyErrorCASFailoverTryNextOne, secondCasUri.ToString(), new object[]
			{
				base.OwaContext.LocalHostName,
				secondCasUri.ToString(),
				base.OwaContext.SecondCasUri.ToString(),
				string.Format("webExceptionStatus={0}", webException.Status)
			});
			bool result;
			try
			{
				this.SendProxyRequest();
				result = true;
			}
			catch (Exception exception2)
			{
				this.asyncResult.CompleteRequest(false, exception2);
				result = true;
			}
			return result;
		}

		private void GetNextFailoverCas()
		{
			ExTraceGlobals.ProxyCallTracer.TraceDebug((long)this.GetHashCode(), "ProxyEventHandler.GetNextFailoverCas");
			bool flag = false;
			try
			{
				base.OwaContext.UserContext.Lock();
				flag = true;
				ProxyUriQueue proxyUriQueue = base.UserContext.ProxyUriQueue;
				if (proxyUriQueue.Head != base.OwaContext.SecondCasUri)
				{
					base.OwaContext.SecondCasUri = proxyUriQueue.Head;
				}
				else
				{
					ProxyUri proxyUri = proxyUriQueue.Pop();
					if (!proxyUri.IsValid)
					{
						ProxyUtilities.ThrowMalformedCasUriException(base.OwaContext, proxyUri.ToString());
					}
					ProxyUtilities.EnsureProxyUrlSslPolicy(base.OwaContext, proxyUri);
					base.OwaContext.SecondCasUri = proxyUri;
					ExTraceGlobals.ProxyTracer.TraceDebug<ProxyUri>((long)this.GetHashCode(), "SecondCasUri = {0}", base.OwaContext.SecondCasUri);
				}
				this.proxyRequestUrl = new UriBuilder(this.proxyRequestUrl)
				{
					Scheme = base.OwaContext.SecondCasUri.Uri.Scheme,
					Host = base.OwaContext.SecondCasUri.Uri.Host,
					Port = base.OwaContext.SecondCasUri.Uri.Port
				}.Uri;
				this.proxyLogonAttempts = 0;
			}
			finally
			{
				if (base.OwaContext.UserContext.LockedByCurrentThread() && flag)
				{
					base.OwaContext.UserContext.Unlock();
				}
			}
		}

		private HttpRequest OriginalRequest
		{
			get
			{
				return base.OwaContext.HttpContext.Request;
			}
		}

		private HttpResponse OriginalResponse
		{
			get
			{
				return base.OwaContext.HttpContext.Response;
			}
		}

		private Stream OriginalRequestStream
		{
			get
			{
				return this.OriginalRequest.InputStream;
			}
		}

		private Stream OriginalResponseStream
		{
			get
			{
				return this.OriginalResponse.OutputStream;
			}
		}

		protected override void InternalDispose(bool isDisposing)
		{
			if (isDisposing)
			{
				if (this.proxyResponse != null)
				{
					this.proxyResponse.Close();
					this.proxyResponse = null;
				}
				if (this.proxyResponseStream != null)
				{
					this.proxyResponseStream.Close();
					this.proxyResponseStream = null;
				}
				try
				{
					if (this.proxyRequestStream != null)
					{
						this.proxyRequestStream.Close();
					}
				}
				catch (WebException)
				{
					if (this.copyingOriginalRequestComplete && this.copyingProxyResponseComplete)
					{
						throw;
					}
				}
				finally
				{
					this.proxyRequestStream = null;
					if (this.proxyLogonRequest != null)
					{
						this.proxyLogonRequest.Dispose();
						this.proxyLogonRequest = null;
					}
					if (this.proxyLanguagePostRequest != null)
					{
						this.proxyLanguagePostRequest.Dispose();
						this.proxyLanguagePostRequest = null;
					}
					if (this.timeoutWaitHandle != null)
					{
						this.timeoutWaitHandle.Unregister(null);
						this.timeoutWaitHandle = null;
					}
					if (this.proxyPingRequest != null)
					{
						this.proxyPingRequest.Dispose();
						this.proxyPingRequest = null;
					}
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ProxyEventHandler>(this);
		}

		private void RequestTimeoutCallback(object state, bool timedOut)
		{
			ExTraceGlobals.ProxyCallTracer.TraceDebug(0L, "ProxyEventHandler.RequestTimeoutCallback");
			if (!timedOut)
			{
				ExTraceGlobals.ProxyTracer.TraceDebug(0L, "Request is completed, aborting timeout");
				return;
			}
			lock (this)
			{
				if (this.asyncResult.IsCompleted)
				{
					ExTraceGlobals.ProxyTracer.TraceDebug(0L, "Request is completed, aborting timeout");
				}
				else
				{
					ExTraceGlobals.ProxyTracer.TraceDebug(0L, "Async request timed out");
					this.requestTimedOut = true;
					this.proxyRequest.Abort();
					this.asyncResult.CompleteRequest(false, new OwaAsyncRequestTimeoutException("ProxyEventHandler request timeout", null));
				}
			}
		}

		internal const string ProxyRequestUrlParameter = "pru";

		internal const string LogonIdentityParameter = "identity";

		internal const string TimeZoneKeyNameParameter = "tzid";

		internal const string CultureParameter = "culture";

		internal const string IsOptimizedParameter = "opt";

		internal const string DestinationParameter = "destination";

		private HttpWebRequest proxyRequest;

		private HttpWebResponse proxyResponse;

		private Stream proxyRequestStream;

		private Stream proxyResponseStream;

		private bool copyingOriginalRequestComplete;

		private bool copyingProxyResponseComplete;

		private ProxyLogonRequest proxyLogonRequest;

		private ProxyLanguagePostRequest proxyLanguagePostRequest;

		private ProxyPingRequest proxyPingRequest;

		private Uri proxyRequestUrl;

		private OwaAsyncResult asyncResult;

		private ProxyStreamCopy proxyStreamCopy;

		private Stopwatch requestClock;

		private bool requestTimedOut;

		private RegisteredWaitHandle timeoutWaitHandle;

		private int attemptedProxyUriCount = 1;

		private int copyProxyAttempts;

		private int proxyLogonAttempts;
	}
}
