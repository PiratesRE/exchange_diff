using System;
using System.IO;
using System.Web;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;
using Microsoft.Exchange.Security.OAuth;

namespace Microsoft.Exchange.HttpProxy
{
	internal class EwsProxyRequestHandler : EwsAutodiscoverProxyRequestHandler
	{
		internal EwsProxyRequestHandler() : this(false)
		{
		}

		internal EwsProxyRequestHandler(bool isOwa14EwsProxyRequest)
		{
			this.isOwa14EwsProxyRequest = isOwa14EwsProxyRequest;
		}

		protected override bool WillContentBeChangedDuringStreaming
		{
			get
			{
				return !base.IsWsSecurityRequest && base.ClientRequest.CanHaveBody() && (this.isOwa14EwsProxyRequest || base.ProxyToDownLevel || this.proxyForSameOrgExchangeOAuthCallToLowerVersion);
			}
		}

		protected override ClientAccessType ClientAccessType
		{
			get
			{
				return ClientAccessType.InternalNLBBypass;
			}
		}

		protected override bool ShouldBlockCurrentOAuthRequest()
		{
			return !this.proxyForSameOrgExchangeOAuthCallToLowerVersion && base.ShouldBlockCurrentOAuthRequest();
		}

		protected override void DoProtocolSpecificBeginRequestLogging()
		{
		}

		protected override StreamProxy BuildRequestStreamProxy(StreamProxy.StreamProxyType streamProxyType, Stream source, Stream target, byte[] buffer)
		{
			if (base.IsWsSecurityRequest || !base.ClientRequest.CanHaveBody())
			{
				return base.BuildRequestStreamProxy(streamProxyType, source, target, buffer);
			}
			if (!this.isOwa14EwsProxyRequest && !base.ProxyToDownLevel && !this.proxyForSameOrgExchangeOAuthCallToLowerVersion)
			{
				return base.BuildRequestStreamProxy(streamProxyType, source, target, buffer);
			}
			string requestVersionToAdd = null;
			if (this.isOwa14EwsProxyRequest)
			{
				if ("12.1".Equals(base.HttpContext.Request.QueryString["rv"]))
				{
					requestVersionToAdd = "Exchange2007_SP1";
				}
				else
				{
					requestVersionToAdd = "Exchange2010_SP1";
				}
			}
			return new EwsRequestStreamProxy(streamProxyType, source, target, buffer, this, base.ProxyToDownLevel || this.proxyForSameOrgExchangeOAuthCallToLowerVersion, this.proxyForSameOrgExchangeOAuthCallToLowerVersionWithNoSidUser, requestVersionToAdd);
		}

		protected override Uri GetTargetBackEndServerUrl()
		{
			Uri targetBackEndServerUrl = base.GetTargetBackEndServerUrl();
			if (this.isOwa14EwsProxyRequest)
			{
				return new UriBuilder(targetBackEndServerUrl)
				{
					Path = "/ews/exchange.asmx",
					Query = string.Empty
				}.Uri;
			}
			if (targetBackEndServerUrl.AbsolutePath.IndexOf("ews/Nego2", StringComparison.OrdinalIgnoreCase) >= 0)
			{
				return new UriBuilder(targetBackEndServerUrl)
				{
					Path = "/ews/exchange.asmx"
				}.Uri;
			}
			OAuthIdentity oauthIdentity = base.HttpContext.User.Identity as OAuthIdentity;
			if (oauthIdentity != null && !oauthIdentity.IsAppOnly && oauthIdentity.IsKnownFromSameOrgExchange && base.HttpContext.Request.UserAgent.StartsWith("ASProxy/CrossForest", StringComparison.InvariantCultureIgnoreCase))
			{
				if (FaultInjection.TraceTest<bool>((FaultInjection.LIDs)3548785981U))
				{
					throw new InvalidOAuthTokenException(OAuthErrors.TestOnlyExceptionDuringProxyDownLevelCheckNullSid, null, null);
				}
				this.proxyForSameOrgExchangeOAuthCallToLowerVersion = (base.ProxyToDownLevel || FaultInjection.TraceTest<bool>((FaultInjection.LIDs)2357603645U) || FaultInjection.TraceTest<bool>((FaultInjection.LIDs)3431345469U));
				if (this.proxyForSameOrgExchangeOAuthCallToLowerVersion || oauthIdentity.ActAsUser.IsUserVerified)
				{
					this.proxyForSameOrgExchangeOAuthCallToLowerVersionWithNoSidUser = (FaultInjection.TraceTest<bool>((FaultInjection.LIDs)3431345469U) || oauthIdentity.ActAsUser.Sid == null);
				}
			}
			return targetBackEndServerUrl;
		}

		protected override void OnInitializingHandler()
		{
			base.OnInitializingHandler();
			if (HttpProxyGlobals.ProtocolType == ProtocolType.Ews && !base.ClientRequest.IsAuthenticated)
			{
				base.IsWsSecurityRequest = base.ClientRequest.IsAnyWsSecurityRequest();
			}
		}

		protected override AnchorMailbox ResolveAnchorMailbox()
		{
			string text = base.ClientRequest.Headers[Constants.PreferServerAffinityHeader];
			if (!string.IsNullOrEmpty(text) && text.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase))
			{
				HttpCookie httpCookie = base.ClientRequest.Cookies[Constants.BackEndOverrideCookieName];
				string text2 = (httpCookie == null) ? null : httpCookie.Value;
				if (!string.IsNullOrEmpty(text2))
				{
					try
					{
						BackEndServer backendServer = BackEndServer.FromString(text2);
						base.Logger.Set(HttpProxyMetadata.RoutingHint, Constants.BackEndOverrideCookieName);
						return new ServerInfoAnchorMailbox(backendServer, this);
					}
					catch (ArgumentException arg)
					{
						base.Logger.AppendGenericError("Unable to parse TargetServer: {0}", text2);
						ExTraceGlobals.ExceptionTracer.TraceDebug<string, ArgumentException>((long)this.GetHashCode(), "[EwsProxyRequestHandler::ResolveAnchorMailbox]: exception hit where target server was '{0}': {1}", text2, arg);
					}
				}
			}
			return base.ResolveAnchorMailbox();
		}

		protected override void CopySupplementalCookiesToClientResponse()
		{
			if (base.AnchoredRoutingTarget != null)
			{
				string value = base.ServerResponse.Headers["X-FromBackend-ServerAffinity"];
				if (!string.IsNullOrEmpty(value) && base.ClientRequest.Cookies[Constants.BackEndOverrideCookieName] == null)
				{
					HttpCookie httpCookie = new HttpCookie(Constants.BackEndOverrideCookieName, base.AnchoredRoutingTarget.BackEndServer.ToString());
					httpCookie.HttpOnly = true;
					httpCookie.Secure = base.ClientRequest.IsSecureConnection;
					base.ClientResponse.Cookies.Add(httpCookie);
				}
			}
			base.CopySupplementalCookiesToClientResponse();
		}

		private const string Owa14EwsProxyRequestVersionHeader = "rv";

		private const string Owa14EwsProxyE12SP1Version = "12.1";

		private const string Exchange2007SP1Version = "Exchange2007_SP1";

		private const string Exchange2010SP1Version = "Exchange2010_SP1";

		private const string Nego2PathPrefix = "ews/Nego2";

		private const string EwsPath = "/ews/exchange.asmx";

		private readonly bool isOwa14EwsProxyRequest;

		private bool proxyForSameOrgExchangeOAuthCallToLowerVersion;

		private bool proxyForSameOrgExchangeOAuthCallToLowerVersionWithNoSidUser;
	}
}
