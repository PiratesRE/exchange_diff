using System;
using System.IO;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.HttpProxy
{
	internal abstract class OwaEcpProxyRequestHandler<ServiceType> : BEServerCookieProxyRequestHandler<ServiceType> where ServiceType : HttpService
	{
		protected bool IsExplicitSignOn { get; set; }

		protected string ExplicitSignOnAddress { get; set; }

		protected string ExplicitSignOnDomain { get; set; }

		protected abstract string ProxyLogonUri { get; }

		protected virtual string ProxyLogonQueryString
		{
			get
			{
				return null;
			}
		}

		protected override bool WillAddProtocolSpecificCookiesToServerRequest
		{
			get
			{
				return this.proxyLogonResponseCookies != null;
			}
		}

		protected override bool ImplementsOutOfBandProxyLogon
		{
			get
			{
				return true;
			}
		}

		protected override HttpStatusCode StatusCodeSignifyingOutOfBandProxyLogonNeeded
		{
			get
			{
				return (HttpStatusCode)441;
			}
		}

		protected static bool IsServerPageRequest(string localPath)
		{
			return localPath.EndsWith(".asax", StringComparison.OrdinalIgnoreCase) || localPath.EndsWith(".ascx", StringComparison.OrdinalIgnoreCase) || localPath.EndsWith(".ashx", StringComparison.OrdinalIgnoreCase) || localPath.EndsWith(".asmx", StringComparison.OrdinalIgnoreCase) || localPath.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase) || localPath.EndsWith(".browser", StringComparison.OrdinalIgnoreCase) || localPath.EndsWith(".config", StringComparison.OrdinalIgnoreCase) || localPath.EndsWith(".eas", StringComparison.OrdinalIgnoreCase) || localPath.EndsWith(".ics", StringComparison.OrdinalIgnoreCase) || localPath.EndsWith(".lex", StringComparison.OrdinalIgnoreCase) || localPath.EndsWith(".manifest", StringComparison.OrdinalIgnoreCase) || localPath.EndsWith(".master", StringComparison.OrdinalIgnoreCase) || localPath.EndsWith(".owa", StringComparison.OrdinalIgnoreCase) || localPath.EndsWith(".owa2", StringComparison.OrdinalIgnoreCase) || localPath.EndsWith(".svc", StringComparison.OrdinalIgnoreCase) || localPath.EndsWith(".template", StringComparison.OrdinalIgnoreCase) || localPath.EndsWith(".wsdl", StringComparison.OrdinalIgnoreCase) || localPath.EndsWith(".xaml", StringComparison.OrdinalIgnoreCase) || localPath.EndsWith(".xap", StringComparison.OrdinalIgnoreCase) || localPath.EndsWith(".xml", StringComparison.OrdinalIgnoreCase) || localPath.EndsWith(".xsd", StringComparison.OrdinalIgnoreCase);
		}

		protected override DatacenterRedirectStrategy CreateDatacenterRedirectStrategy()
		{
			return new OwaEcpRedirectStrategy(this);
		}

		protected virtual void SetProtocolSpecificProxyLogonRequestParameters(HttpWebRequest request)
		{
		}

		protected override void StartOutOfBandProxyLogon(object extraData)
		{
			lock (base.LockObject)
			{
				ExTraceGlobals.VerboseTracer.TraceDebug<int>((long)this.GetHashCode(), "[OwaEcpProxyRequestHandler::StartOutOfBandProxyLogon]: Context {0}; Remote server returned 441, this means we need to attempt to do a proxy logon", base.TraceContext);
				UriBuilder uriBuilder = new UriBuilder(this.GetTargetBackEndServerUrl());
				uriBuilder.Scheme = Uri.UriSchemeHttps;
				uriBuilder.Path = base.ClientRequest.ApplicationPath + "/" + this.ProxyLogonUri;
				uriBuilder.Query = this.ProxyLogonQueryString;
				base.Logger.AppendGenericInfo("ProxyLogon", uriBuilder.Uri.ToString());
				this.proxyLogonRequest = (HttpWebRequest)WebRequest.Create(uriBuilder.Uri);
				this.proxyLogonRequest.ServicePoint.ConnectionLimit = HttpProxySettings.ServicePointConnectionLimit.Value;
				this.proxyLogonRequest.Method = "POST";
				base.PrepareServerRequest(this.proxyLogonRequest);
				this.SetProtocolSpecificProxyLogonRequestParameters(this.proxyLogonRequest);
				base.PfdTracer.TraceRequest("ProxyLogonRequest", this.proxyLogonRequest);
				UTF8Encoding utf8Encoding = new UTF8Encoding(true, true);
				this.proxyLogonCSC = utf8Encoding.GetBytes(base.HttpContext.GetSerializedAccessTokenString());
				this.proxyLogonRequest.ContentLength = (long)this.proxyLogonCSC.Length;
				this.proxyLogonRequest.BeginGetRequestStream(new AsyncCallback(OwaEcpProxyRequestHandler<ServiceType>.ProxyLogonRequestStreamReadyCallback), base.ServerAsyncState);
				base.State = ProxyRequestHandler.ProxyState.WaitForProxyLogonRequestStream;
			}
		}

		protected bool IsResourceRequest()
		{
			string localPath = base.ClientRequest.Url.LocalPath;
			return BEResourceRequestHandler.IsResourceRequest(localPath);
		}

		protected override bool ShouldCopyCookieToClientResponse(Cookie cookie)
		{
			if (FbaModule.IsCadataCookie(cookie.Name))
			{
				ExTraceGlobals.VerboseTracer.TraceDebug<int, string>((long)this.GetHashCode(), "[OwaEcpProxyRequestHandler::ShouldCopyCookieToClientResponse]: Context {0}; Unexpected cadata cookie {1} from BE", base.TraceContext, cookie.Name);
				return false;
			}
			return true;
		}

		protected override void CopySupplementalCookiesToClientResponse()
		{
			if (this.proxyLogonResponseCookies != null)
			{
				foreach (object obj in this.proxyLogonResponseCookies)
				{
					Cookie cookie = (Cookie)obj;
					if (FbaModule.IsCadataCookie(cookie.Name))
					{
						ExTraceGlobals.VerboseTracer.TraceDebug<int, string>((long)this.GetHashCode(), "[OwaEcpProxyRequestHandler::CopySupplementalCookiesToClientResponse]: Context {0}; Unexpected cadata cookie {1} in proxy logon response from BE", base.TraceContext, cookie.Name);
					}
					else
					{
						base.CopyServerCookieToClientResponse(cookie);
					}
				}
			}
			base.CopySupplementalCookiesToClientResponse();
		}

		protected override bool TryHandleProtocolSpecificResponseErrors(WebException e)
		{
			if (e.Status == WebExceptionStatus.ProtocolError)
			{
				HttpWebResponse httpWebResponse = (HttpWebResponse)e.Response;
				int statusCode = (int)httpWebResponse.StatusCode;
				if (statusCode == 442)
				{
					this.RedirectOn442Response();
					return true;
				}
			}
			return base.TryHandleProtocolSpecificResponseErrors(e);
		}

		protected override void AddProtocolSpecificCookiesToServerRequest(CookieContainer cookieContainer)
		{
			cookieContainer.Add(this.proxyLogonResponseCookies);
		}

		protected override void Cleanup()
		{
			if (this.proxyLogonRequestStream != null)
			{
				this.proxyLogonRequestStream.Flush();
				this.proxyLogonRequestStream.Dispose();
				this.proxyLogonRequestStream = null;
			}
			if (this.proxyLogonResponse != null)
			{
				this.proxyLogonResponse.Close();
				this.proxyLogonResponse = null;
			}
			base.Cleanup();
		}

		protected override Uri UpdateExternalRedirectUrl(Uri originalRedirectUrl)
		{
			UriBuilder uriBuilder = new UriBuilder(originalRedirectUrl);
			if (!string.IsNullOrEmpty(this.ExplicitSignOnAddress))
			{
				uriBuilder.Path = UrlUtilities.GetPathWithExplictLogonHint(originalRedirectUrl, this.ExplicitSignOnAddress);
			}
			return uriBuilder.Uri;
		}

		protected override bool ShouldExcludeFromExplicitLogonParsing()
		{
			bool flag = this.IsResourceRequest();
			Uri url = base.ClientRequest.Url;
			string text = (url.Segments.Length > 2) ? url.Segments[2] : string.Empty;
			string text2 = (url.Segments.Length > 3) ? url.Segments[3] : string.Empty;
			string text3 = base.ClientRequest.Headers["X-OWA-ExplicitLogonUser"];
			bool flag2 = (!string.IsNullOrEmpty(text3) && SmtpAddress.IsValidSmtpAddress(text3)) || text.Contains("@") || text2.Contains("@");
			ExTraceGlobals.VerboseTracer.TraceDebug((long)this.GetHashCode(), "ShouldExcludeFromExplicitLogonParsing: {0}/{1} resource:{2} explicit:{3}", new object[]
			{
				text,
				text2,
				flag ? "T" : "F",
				flag2 ? "T" : "F"
			});
			return flag && !flag2;
		}

		protected override bool IsValidExplicitLogonNode(string node, bool nodeIsLast)
		{
			string text = string.Format("IsValidExplicitLogonNode: {0} last:{1} ", node, nodeIsLast ? "y" : "n");
			bool result;
			if (string.IsNullOrEmpty(node))
			{
				text += "1-F";
				result = false;
			}
			else if (!node.Contains("@") && !node.Contains("."))
			{
				text += "2-F";
				result = false;
			}
			else if (nodeIsLast && OwaEcpProxyRequestHandler<HttpService>.IsServerPageRequest(node))
			{
				ExTraceGlobals.VerboseTracer.TraceDebug<int, string>((long)this.GetHashCode(), "[OwaEcpProxyRequestHandler::IsValidExplicitLogonNode]: Context {0}; rejected explicit logon node: {1}", base.TraceContext, node);
				text += "3-F";
				result = false;
			}
			else
			{
				text += "4-T";
				result = true;
			}
			ExTraceGlobals.VerboseTracer.TraceDebug((long)this.GetHashCode(), text);
			return result;
		}

		protected override UriBuilder GetClientUrlForProxy()
		{
			UriBuilder uriBuilder = new UriBuilder(base.ClientRequest.Url.OriginalString);
			if (this.IsExplicitSignOn)
			{
				string text = HttpUtility.UrlDecode(base.ClientRequest.Url.AbsolutePath);
				string str = HttpUtility.UrlDecode(this.ExplicitSignOnAddress);
				uriBuilder.Path = text.Replace("/" + str, string.Empty);
			}
			return uriBuilder;
		}

		protected override void RedirectIfNeeded(BackEndServer mailboxServer)
		{
			if (mailboxServer == null)
			{
				throw new ArgumentNullException("mailboxServer");
			}
			if (!Utilities.IsPartnerHostedOnly && !VariantConfiguration.InvariantNoFlightingSnapshot.Cafe.NoCrossSiteRedirect.Enabled)
			{
				ServiceTopology currentServiceTopology = ServiceTopology.GetCurrentServiceTopology("f:\\15.00.1497\\sources\\dev\\cafe\\src\\HttpProxy\\RequestHandlers\\OwaEcpProxyRequestHandler.cs", "RedirectIfNeeded", 537);
				Site targetSite = currentServiceTopology.GetSite(mailboxServer.Fqdn, "f:\\15.00.1497\\sources\\dev\\cafe\\src\\HttpProxy\\RequestHandlers\\OwaEcpProxyRequestHandler.cs", "RedirectIfNeeded", 538);
				ADSite localSite = LocalSiteCache.LocalSite;
				if (!localSite.DistinguishedName.Equals(targetSite.DistinguishedName) && (!this.IsLocalRequest(LocalServerCache.LocalServerFqdn) || !this.IsLAMUserAgent(base.ClientRequest.UserAgent)))
				{
					HttpService targetService = currentServiceTopology.FindAny<ServiceType>(ClientAccessType.Internal, (ServiceType internalService) => internalService != null && internalService.IsFrontEnd && internalService.Site.DistinguishedName.Equals(targetSite.DistinguishedName), "f:\\15.00.1497\\sources\\dev\\cafe\\src\\HttpProxy\\RequestHandlers\\OwaEcpProxyRequestHandler.cs", "RedirectIfNeeded", 550);
					if (!this.ShouldExecuteSSORedirect(targetService))
					{
						HttpService httpService = currentServiceTopology.FindAny<ServiceType>(ClientAccessType.External, (ServiceType externalService) => externalService != null && externalService.IsFrontEnd && externalService.Site.DistinguishedName.Equals(targetSite.DistinguishedName), "f:\\15.00.1497\\sources\\dev\\cafe\\src\\HttpProxy\\RequestHandlers\\OwaEcpProxyRequestHandler.cs", "RedirectIfNeeded", 561);
						if (httpService != null)
						{
							Uri url = httpService.Url;
							if (Uri.Compare(url, base.ClientRequest.Url, UriComponents.Host, UriFormat.UriEscaped, StringComparison.OrdinalIgnoreCase) != 0)
							{
								UriBuilder uriBuilder = new UriBuilder(base.ClientRequest.Url);
								uriBuilder.Host = url.Host;
								ExTraceGlobals.VerboseTracer.TraceDebug<string>((long)this.GetHashCode(), "[OwaEcpProxyRequestHandler::RedirectIfNeeded]: Stop processing and redirect to {0}.", uriBuilder.Uri.AbsoluteUri);
								throw new ServerSideTransferException(uriBuilder.Uri.AbsoluteUri, LegacyRedirectTypeOptions.Silent);
							}
						}
					}
				}
			}
		}

		private static void ProxyLogonRequestStreamReadyCallback(IAsyncResult result)
		{
			OwaEcpProxyRequestHandler<ServiceType> owaEcpProxyRequestHandler = AsyncStateHolder.Unwrap<OwaEcpProxyRequestHandler<ServiceType>>(result);
			ExTraceGlobals.VerboseTracer.TraceDebug<int>((long)owaEcpProxyRequestHandler.GetHashCode(), "[OwaEcpProxyRequestHandler::ProxyLogonRequestStreamReadyCallback]: Context {0}", owaEcpProxyRequestHandler.TraceContext);
			if (result.CompletedSynchronously)
			{
				ThreadPool.QueueUserWorkItem(new WaitCallback(owaEcpProxyRequestHandler.OnProxyLogonRequestStreamReady), result);
				return;
			}
			owaEcpProxyRequestHandler.OnProxyLogonRequestStreamReady(result);
		}

		private static void ProxyLogonResponseReadyCallback(IAsyncResult result)
		{
			OwaEcpProxyRequestHandler<ServiceType> owaEcpProxyRequestHandler = AsyncStateHolder.Unwrap<OwaEcpProxyRequestHandler<ServiceType>>(result);
			ExTraceGlobals.VerboseTracer.TraceDebug<int>((long)owaEcpProxyRequestHandler.GetHashCode(), "[OwaEcpProxyRequestHandler::ProxyLogonResponseReadyCallback]: Context {0}", owaEcpProxyRequestHandler.TraceContext);
			if (result.CompletedSynchronously)
			{
				ThreadPool.QueueUserWorkItem(new WaitCallback(owaEcpProxyRequestHandler.OnProxyLogonResponseReady), result);
				return;
			}
			owaEcpProxyRequestHandler.OnProxyLogonResponseReady(result);
		}

		private bool ShouldExecuteSSORedirect(HttpService targetService)
		{
			return !FbaFormPostProxyRequestHandler.DisableSSORedirects && (VdirConfiguration.Instance.InternalAuthenticationMethod & AuthenticationMethod.Fba) == AuthenticationMethod.Fba && (targetService == null || (targetService.AuthenticationMethod & AuthenticationMethod.Fba) == AuthenticationMethod.Fba);
		}

		private void OnProxyLogonRequestStreamReady(object extraData)
		{
			base.CallThreadEntranceMethod(delegate
			{
				IAsyncResult asyncResult = extraData as IAsyncResult;
				lock (this.LockObject)
				{
					try
					{
						this.proxyLogonRequestStream = this.proxyLogonRequest.EndGetRequestStream(asyncResult);
						this.proxyLogonRequestStream.Write(this.proxyLogonCSC, 0, this.proxyLogonCSC.Length);
						this.proxyLogonRequestStream.Flush();
						this.proxyLogonRequestStream.Dispose();
						this.proxyLogonRequestStream = null;
						try
						{
							ConcurrencyGuardHelper.IncrementTargetBackendDagAndForest(this);
							this.proxyLogonRequest.BeginGetResponse(new AsyncCallback(OwaEcpProxyRequestHandler<ServiceType>.ProxyLogonResponseReadyCallback), this.ServerAsyncState);
							this.State = ProxyRequestHandler.ProxyState.WaitForProxyLogonResponse;
						}
						catch (Exception)
						{
							ConcurrencyGuardHelper.DecrementTargetBackendDagAndForest(this);
							throw;
						}
					}
					catch (WebException ex)
					{
						this.CompleteWithError(ex, "[OwaEcpProxyRequestHandler::OnProxyLogonRequestStreamReady]");
					}
					catch (HttpException ex2)
					{
						this.CompleteWithError(ex2, "[OwaEcpProxyRequestHandler::OnProxyLogonRequestStreamReady]");
					}
					catch (HttpProxyException ex3)
					{
						this.CompleteWithError(ex3, "[OwaEcpProxyRequestHandler::OnProxyLogonRequestStreamReady]");
					}
					catch (IOException ex4)
					{
						this.CompleteWithError(ex4, "[OwaEcpProxyRequestHandler::OnProxyLogonRequestStreamReady]");
					}
				}
			});
		}

		private void OnProxyLogonResponseReady(object extraData)
		{
			base.CallThreadEntranceMethod(delegate
			{
				IAsyncResult asyncResult = extraData as IAsyncResult;
				lock (this.LockObject)
				{
					try
					{
						ConcurrencyGuardHelper.DecrementTargetBackendDagAndForest(this);
						this.proxyLogonResponse = (HttpWebResponse)this.proxyLogonRequest.EndGetResponse(asyncResult);
						this.PfdTracer.TraceResponse("ProxyLogonResponse", this.proxyLogonResponse);
						this.proxyLogonResponseCookies = this.proxyLogonResponse.Cookies;
						this.proxyLogonResponse.Close();
						this.proxyLogonResponse = null;
						UserContextCookie userContextCookie = this.TryGetUserContextFromProxyLogonResponse();
						if (userContextCookie != null && userContextCookie.MailboxUniqueKey != null)
						{
							string mailboxUniqueKey = userContextCookie.MailboxUniqueKey;
							if (SmtpAddress.IsValidSmtpAddress(mailboxUniqueKey))
							{
								ProxyAddress proxyAddress = ProxyAddress.Parse("SMTP", mailboxUniqueKey);
								AnchorMailbox anchorMailbox = new SmtpAnchorMailbox(proxyAddress.AddressString, this);
								this.AnchoredRoutingTarget = new AnchoredRoutingTarget(anchorMailbox, this.AnchoredRoutingTarget.BackEndServer);
							}
							else
							{
								try
								{
									SecurityIdentifier sid = new SecurityIdentifier(mailboxUniqueKey);
									AnchorMailbox anchorMailbox2 = new SidAnchorMailbox(sid, this);
									this.AnchoredRoutingTarget = new AnchoredRoutingTarget(anchorMailbox2, this.AnchoredRoutingTarget.BackEndServer);
								}
								catch (ArgumentException)
								{
								}
							}
						}
						ThreadPool.QueueUserWorkItem(new WaitCallback(this.BeginProxyRequest));
						this.State = ProxyRequestHandler.ProxyState.PrepareServerRequest;
					}
					catch (WebException ex)
					{
						if (ex.Status == WebExceptionStatus.ProtocolError && ((HttpWebResponse)ex.Response).StatusCode == (HttpStatusCode)442)
						{
							this.RedirectOn442Response();
						}
						else
						{
							this.CompleteWithError(ex, "[OwaEcpProxyRequestHandler::OnProxyLogonResponseReady]");
						}
					}
					catch (HttpException ex2)
					{
						this.CompleteWithError(ex2, "[OwaEcpProxyRequestHandler::OnProxyLogonResponseReady]");
					}
					catch (IOException ex3)
					{
						this.CompleteWithError(ex3, "[OwaEcpProxyRequestHandler::OnProxyLogonResponseReady]");
					}
					catch (HttpProxyException ex4)
					{
						this.CompleteWithError(ex4, "[OwaEcpProxyRequestHandler::OnProxyLogonResponseReady]");
					}
				}
			});
		}

		private UserContextCookie TryGetUserContextFromProxyLogonResponse()
		{
			foreach (object obj in this.proxyLogonResponseCookies)
			{
				Cookie cookie = (Cookie)obj;
				if (cookie.Name.StartsWith("UserContext"))
				{
					return UserContextCookie.TryCreateFromNetCookie(cookie);
				}
			}
			return null;
		}

		private void RedirectOn442Response()
		{
			ExTraceGlobals.VerboseTracer.TraceDebug<int>((long)this.GetHashCode(), "[OwaEcpProxyRequestHandler::RedirectOn442Response]: Context {0}; The proxy returned 442, this means the user's language or timezone are invalid", base.TraceContext);
			string text = OwaUrl.LanguagePage.GetExplicitUrl(base.ClientRequest).ToString();
			base.PfdTracer.TraceRedirect("EcpOwa442NeedLanguage", text);
			base.ClientResponse.Redirect(text, false);
			base.CompleteForRedirect(text);
		}

		private bool IsLocalRequest(string machineFqdn)
		{
			string host = base.ClientRequest.Url.Host;
			return host.Equals("localhost", StringComparison.OrdinalIgnoreCase) || host.Equals("127.0.0.1", StringComparison.OrdinalIgnoreCase) || host.Equals(Environment.MachineName, StringComparison.OrdinalIgnoreCase) || host.Equals(machineFqdn, StringComparison.OrdinalIgnoreCase);
		}

		private bool IsLAMUserAgent(string requestUserAgent)
		{
			return requestUserAgent.Equals("AMPROBE/LOCAL/CLIENTACCESS", StringComparison.OrdinalIgnoreCase);
		}

		private const int HttpStatusNeedIdentity = 441;

		private const int HttpStatusNeedLanguage = 442;

		private HttpWebRequest proxyLogonRequest;

		private Stream proxyLogonRequestStream;

		private HttpWebResponse proxyLogonResponse;

		private CookieCollection proxyLogonResponseCookies;

		private byte[] proxyLogonCSC;
	}
}
