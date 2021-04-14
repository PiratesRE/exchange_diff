using System;
using System.Net;
using System.Security.Principal;
using System.Web;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;
using Microsoft.Exchange.HttpProxy.Common;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.HttpProxy
{
	internal class EcpProxyRequestHandler : OwaEcpProxyRequestHandler<EcpService>
	{
		internal bool IsCrossForestDelegated { get; set; }

		protected override string ProxyLogonUri
		{
			get
			{
				string explicitPath = this.GetExplicitPath(base.ClientRequest.Path);
				if (explicitPath != null)
				{
					return explicitPath + "proxyLogon.ecp";
				}
				return "proxyLogon.ecp";
			}
		}

		protected override ClientAccessType ClientAccessType
		{
			get
			{
				return ClientAccessType.Internal;
			}
		}

		internal static void AddDownLevelProxyHeaders(WebHeaderCollection headers, HttpContext context)
		{
			if (!context.Request.IsAuthenticated)
			{
				throw new HttpException(401, "Unable to proxy unauthenticated down-level requests.");
			}
			if (context.User != null)
			{
				IIdentity identity = context.User.Identity;
				if ((identity is WindowsIdentity || identity is ClientSecurityContextIdentity) && null != identity.GetSecurityIdentifier())
				{
					string value = identity.GetSecurityIdentifier().ToString();
					headers["msExchLogonAccount"] = value;
					headers["msExchLogonMailbox"] = value;
					headers["msExchTargetMailbox"] = value;
				}
			}
		}

		internal static bool IsCrossForestDelegatedRequest(HttpRequest request)
		{
			if (!string.IsNullOrEmpty(request.QueryString["SecurityToken"]))
			{
				return true;
			}
			HttpCookie httpCookie = request.Cookies["SecurityToken"];
			return httpCookie != null && !string.IsNullOrEmpty(httpCookie.Value);
		}

		protected override bool ShouldExcludeFromExplicitLogonParsing()
		{
			bool result = base.IsResourceRequest();
			ExTraceGlobals.VerboseTracer.TraceDebug<string>((long)this.GetHashCode(), "[EcpProxyRequestHandler::ShouldExcludeFromExplicitLogonParsing]: request is resource:{0}.", result.ToString());
			return result;
		}

		protected override UriBuilder GetClientUrlForProxy()
		{
			return new UriBuilder(base.ClientRequest.Url);
		}

		protected override void AddProtocolSpecificHeadersToServerRequest(WebHeaderCollection headers)
		{
			headers[Constants.LiveIdEnvironment] = (string)base.HttpContext.Items[Constants.LiveIdEnvironment];
			headers[Constants.LiveIdPuid] = (string)base.HttpContext.Items[Constants.LiveIdPuid];
			headers[Constants.OrgIdPuid] = (string)base.HttpContext.Items[Constants.OrgIdPuid];
			headers[Constants.LiveIdMemberName] = (string)base.HttpContext.Items[Constants.LiveIdMemberName];
			headers["msExchClientPath"] = base.ClientRequest.Path;
			if (this.isSyndicatedAdminManageDownLevelTarget)
			{
				headers["msExchCafeForceRouteToLogonAccount"] = "1";
			}
			if (!this.IsCrossForestDelegated && base.ProxyToDownLevel)
			{
				EcpProxyRequestHandler.AddDownLevelProxyHeaders(headers, base.HttpContext);
				if (base.IsExplicitSignOn)
				{
					string value = null;
					AnchoredRoutingTarget anchoredRoutingTarget = this.isSyndicatedAdminManageDownLevelTarget ? this.originalAnchoredRoutingTarget : base.AnchoredRoutingTarget;
					if (anchoredRoutingTarget != null)
					{
						UserBasedAnchorMailbox userBasedAnchorMailbox = anchoredRoutingTarget.AnchorMailbox as UserBasedAnchorMailbox;
						if (userBasedAnchorMailbox != null)
						{
							ADRawEntry adrawEntry = userBasedAnchorMailbox.GetADRawEntry();
							if (adrawEntry != null)
							{
								SecurityIdentifier securityIdentifier = adrawEntry[ADMailboxRecipientSchema.Sid] as SecurityIdentifier;
								if (securityIdentifier != null)
								{
									value = securityIdentifier.ToString();
								}
							}
						}
					}
					headers["msExchTargetMailbox"] = value;
				}
			}
			base.AddProtocolSpecificHeadersToServerRequest(headers);
		}

		protected override bool ShouldCopyHeaderToServerRequest(string headerName)
		{
			bool flag = !string.Equals(headerName, Constants.MsExchProxyUri, StringComparison.OrdinalIgnoreCase) && !string.Equals(headerName, "msExchLogonAccount", StringComparison.OrdinalIgnoreCase) && !string.Equals(headerName, "msExchLogonMailbox", StringComparison.OrdinalIgnoreCase) && !string.Equals(headerName, "msExchTargetMailbox", StringComparison.OrdinalIgnoreCase) && !string.Equals(headerName, Constants.LiveIdPuid, StringComparison.OrdinalIgnoreCase) && !string.Equals(headerName, Constants.LiveIdMemberName, StringComparison.OrdinalIgnoreCase) && !string.Equals(headerName, "msExchCafeForceRouteToLogonAccount", StringComparison.OrdinalIgnoreCase) && !string.Equals(headerName, Constants.LiveIdEnvironment, StringComparison.OrdinalIgnoreCase) && base.ShouldCopyHeaderToServerRequest(headerName);
			ExTraceGlobals.VerboseTracer.TraceDebug<string, string>((long)this.GetHashCode(), "[EcpProxyRequestHandler::ShouldCopyHeaderToServerRequest]: {0} header '{1}'.", flag ? "copy" : "skip", headerName);
			return flag;
		}

		protected override void HandleLogoffRequest()
		{
			if (base.ClientRequest != null && base.ClientResponse != null && base.ClientRequest.Url.AbsolutePath.EndsWith("logoff.aspx", StringComparison.OrdinalIgnoreCase))
			{
				if (!Utilities.IsPartnerHostedOnly && !VariantConfiguration.InvariantNoFlightingSnapshot.Cafe.NoFormBasedAuthentication.Enabled)
				{
					FbaModule.InvalidateKeyCache(base.ClientRequest);
				}
				Utility.DeleteFbaAuthCookies(base.ClientRequest, base.ClientResponse);
			}
		}

		protected override AnchorMailbox ResolveAnchorMailbox()
		{
			if (base.State != ProxyRequestHandler.ProxyState.CalculateBackEndSecondRound)
			{
				if (!base.AuthBehavior.IsFullyAuthenticated())
				{
					base.HasPreemptivelyCheckedForRoutingHint = true;
					string text = base.HttpContext.Request.Headers["X-UpnAnchorMailbox"];
					if (!string.IsNullOrWhiteSpace(text))
					{
						ExTraceGlobals.VerboseTracer.TraceDebug<int>((long)this.GetHashCode(), "[EcpProxyRequestHandler::ResolveAnchorMailbox]: From Header Routing UPN Hint, context {1}.", base.TraceContext);
						base.Logger.SafeSet(HttpProxyMetadata.RoutingHint, "OwaEcpUpn");
						return new LiveIdMemberNameAnchorMailbox(text, null, this);
					}
					AnchorMailbox anchorMailbox = base.CreateAnchorMailboxFromRoutingHint();
					if (anchorMailbox != null)
					{
						ExTraceGlobals.VerboseTracer.TraceDebug<int>((long)this.GetHashCode(), "[EcpProxyRequestHandler::ResolveAnchorMailbox]: From Header Routing Hint, context {1}.", base.TraceContext);
						return anchorMailbox;
					}
				}
				string text2 = this.TryGetExplicitLogonNode(ExplicitLogonNode.Second);
				bool flag;
				if (!string.IsNullOrEmpty(text2))
				{
					if (SmtpAddress.IsValidSmtpAddress(text2))
					{
						base.IsExplicitSignOn = true;
						base.ExplicitSignOnAddress = text2;
						base.Logger.Set(HttpProxyMetadata.RoutingHint, "ExplicitSignOn-SMTP");
						ExTraceGlobals.VerboseTracer.TraceDebug<string, int>((long)this.GetHashCode(), "[EcpProxyRequestHandler::ResolveAnchorMailbox]: ExplicitSignOn-SMTP. Address {0}, context {1}.", text2, base.TraceContext);
						return new SmtpAnchorMailbox(text2, this);
					}
					if ((Utilities.IsPartnerHostedOnly || VariantConfiguration.InvariantNoFlightingSnapshot.Cafe.SyndicatedAdmin.Enabled) && text2.StartsWith("@"))
					{
						this.isSyndicatedAdmin = true;
						text2 = text2.Substring(1);
						if (SmtpAddress.IsValidDomain(text2))
						{
							string text3 = this.TryGetExplicitLogonNode(ExplicitLogonNode.Third);
							ExTraceGlobals.VerboseTracer.TraceDebug<string, string, int>((long)this.GetHashCode(), "[EcpProxyRequestHandler::ResolveAnchorMailbox]: SyndAdmin, domain {0}, SMTP {1}, context {2}.", text2, text3, base.TraceContext);
							if (!string.IsNullOrEmpty(text3))
							{
								base.IsExplicitSignOn = true;
								base.ExplicitSignOnAddress = text3;
								base.Logger.Set(HttpProxyMetadata.RoutingHint, "SyndAdmin-SMTP");
								return new SmtpAnchorMailbox(text3, this);
							}
							base.Logger.Set(HttpProxyMetadata.RoutingHint, "SyndAdmin-Domain");
							return new DomainAnchorMailbox(text2, this);
						}
					}
				}
				else if (!Utilities.IsPartnerHostedOnly && !VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled)
				{
					string text4 = this.TryGetBackendParameter("TargetServer", out flag);
					if (!string.IsNullOrEmpty(text4))
					{
						base.Logger.Set(HttpProxyMetadata.RoutingHint, "TargetServer" + (flag ? "-UrlQuery" : "-Cookie"));
						ExTraceGlobals.VerboseTracer.TraceDebug<string, string, int>((long)this.GetHashCode(), "[EcpProxyRequestHandler::ResolveAnchorMailbox]: On-Premise, TargetServer parameter {0}, from {1}, context {2}.", text4, flag ? "url query" : "cookie", base.TraceContext);
						return new ServerInfoAnchorMailbox(text4, this);
					}
				}
				string text5 = this.TryGetBackendParameter("ExchClientVer", out flag);
				if (!string.IsNullOrEmpty(text5))
				{
					string text6 = Utilities.NormalizeExchClientVer(text5);
					base.Logger.Set(HttpProxyMetadata.RoutingHint, "ExchClientVer" + (flag ? "-UrlQuery" : "-Cookie"));
					if (!Utilities.IsPartnerHostedOnly && !VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled)
					{
						ExTraceGlobals.VerboseTracer.TraceDebug<string, string, int>((long)this.GetHashCode(), "[EcpProxyRequestHandler::ResolveAnchorMailbox]: On-Premise, Version parameter {0}, from {1}, context {2}.", text5, flag ? "url query" : "cookie", base.TraceContext);
						return base.GetServerVersionAnchorMailbox(text6);
					}
					string text7 = (string)base.HttpContext.Items["AuthenticatedUserOrganization"];
					if (!string.IsNullOrEmpty(text7))
					{
						ExTraceGlobals.VerboseTracer.TraceDebug((long)this.GetHashCode(), "[EcpProxyRequestHandler::ResolveAnchorMailbox]: On-Cloud, Version parameter {0}, from {1}, domain {2}, context {3}.", new object[]
						{
							text6,
							flag ? "url query" : "cookie",
							text7,
							base.TraceContext
						});
						return VersionedDomainAnchorMailbox.GetAnchorMailbox(text7, text6, this);
					}
					ExTraceGlobals.VerboseTracer.TraceDebug<int>((long)this.GetHashCode(), "[EcpProxyRequestHandler::ResolveAnchorMailbox]: AuthenticatedUserOrganization is null. Context {0}.", base.TraceContext);
				}
			}
			ExTraceGlobals.VerboseTracer.TraceDebug<string, int>((long)this.GetHashCode(), "[EcpProxyRequestHandler::ResolveAnchorMailbox]: {0}, context {1}, call base method to do regular anchor mailbox calculation.", (base.State == ProxyRequestHandler.ProxyState.CalculateBackEndSecondRound) ? "Second round" : "Nothing special", base.TraceContext);
			return base.ResolveAnchorMailbox();
		}

		protected override void CopySupplementalCookiesToClientResponse()
		{
			if (this.backendServerFromUrlCookie != null)
			{
				base.CopyServerCookieToClientResponse(this.backendServerFromUrlCookie);
			}
			this.CopyBEResourcePathCookie();
			base.CopySupplementalCookiesToClientResponse();
		}

		protected override bool ShouldRecalculateProxyTarget()
		{
			bool result = false;
			if (this.isSyndicatedAdmin && !this.IsCrossForestDelegated && base.State == ProxyRequestHandler.ProxyState.CalculateBackEnd && base.AnchoredRoutingTarget.BackEndServer != null && base.AnchoredRoutingTarget.BackEndServer.Version < Server.E15MinVersion)
			{
				ExTraceGlobals.VerboseTracer.TraceDebug<int>((long)this.GetHashCode(), "[EcpProxyRequestHandler::ShouldRecalculateProxyTarget]: context {0}, Syndicated admin request. Target tenant is down level, start 2nd round calculation.", base.TraceContext);
				this.isSyndicatedAdminManageDownLevelTarget = true;
				this.originalAnchoredRoutingTarget = base.AnchoredRoutingTarget;
				result = true;
			}
			else
			{
				ExTraceGlobals.VerboseTracer.TraceDebug((long)this.GetHashCode(), "[EcpProxyRequestHandler::ShouldRecalculateProxyTarget]: context {0}, no need to do 2nd round calculation: isSyndicatedAdmin {1}, cross forest {2}, state {3}, BEServer Version {4}, lower than E15MinVer {5}", new object[]
				{
					base.TraceContext,
					this.isSyndicatedAdmin,
					this.IsCrossForestDelegated,
					base.State,
					(base.AnchoredRoutingTarget.BackEndServer != null) ? base.AnchoredRoutingTarget.BackEndServer.Version : -1,
					Server.E15MinVersion
				});
			}
			return result;
		}

		protected override void LogWebException(WebException exception)
		{
			base.LogWebException(exception);
			HttpWebResponse httpWebResponse = (HttpWebResponse)exception.Response;
			if (httpWebResponse != null && !string.IsNullOrEmpty(httpWebResponse.Headers["X-ECP-ERROR"]))
			{
				base.Logger.AppendGenericError("X-ECP-ERROR", httpWebResponse.Headers["X-ECP-ERROR"]);
			}
		}

		private string GetExplicitPath(string requestPath)
		{
			string result = null;
			int num = requestPath.IndexOf('@');
			if (num > 0)
			{
				int num2 = requestPath.IndexOf('@', num + 1);
				if (num2 < 0)
				{
					num2 = num;
				}
				int num3 = num;
				while (num3 > 0 && requestPath[num3] != '/')
				{
					num3--;
				}
				if (num3 > 0)
				{
					int num4 = requestPath.IndexOf('/', num2);
					if (num4 > num3)
					{
						result = requestPath.Substring(num3 + 1, num4 - num3);
					}
				}
			}
			return result;
		}

		private string TryGetBackendParameter(string key, out bool isFromUrl)
		{
			string text = base.ClientRequest.QueryString[key];
			isFromUrl = false;
			if (string.IsNullOrEmpty(text))
			{
				HttpCookie httpCookie = base.ClientRequest.Cookies[key];
				text = ((httpCookie == null) ? null : httpCookie.Value);
			}
			else
			{
				isFromUrl = true;
				this.backendServerFromUrlCookie = new Cookie(key, text)
				{
					HttpOnly = false,
					Secure = true,
					Path = "/"
				};
			}
			return text;
		}

		private void CopyBEResourcePathCookie()
		{
			string text = base.ServerResponse.Headers[Constants.BEResourcePath];
			if (!string.IsNullOrEmpty(text) && base.AnchoredRoutingTarget != null)
			{
				HttpCookie httpCookie = new HttpCookie(Constants.BEResource, base.AnchoredRoutingTarget.BackEndServer.ToString());
				httpCookie.Path = text;
				httpCookie.HttpOnly = true;
				httpCookie.Secure = base.ClientRequest.IsSecureConnection;
				base.ClientResponse.Cookies.Add(httpCookie);
				return;
			}
			if (base.ClientRequest.Url.AbsolutePath.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase) || base.ClientRequest.Url.AbsolutePath.EndsWith(".slab", StringComparison.OrdinalIgnoreCase))
			{
				ExTraceGlobals.VerboseTracer.TraceError<string, string, string>(0L, "[EcpProxyRequestHandler::CopyBEResourcePathCookie] Cannot add X-BEResource cookie to the response of {0}! Header from backend: {1}, backend server: {2}", base.ClientRequest.Url.ToString(), text, (base.AnchoredRoutingTarget == null) ? "null" : base.AnchoredRoutingTarget.BackEndServer.ToString());
			}
		}

		public const string ClientPathHeaderKey = "msExchClientPath";

		private const string LogonAccount = "msExchLogonAccount";

		private const string LogonMailbox = "msExchLogonMailbox";

		private const string TargetMailbox = "msExchTargetMailbox";

		private const string CafeForceRouteToLogonAccountHeaderKey = "msExchCafeForceRouteToLogonAccount";

		private const string EcpErrorHeaderName = "X-ECP-ERROR";

		private const string EcpProxyLogonUri = "proxyLogon.ecp";

		private const string LogoffPage = "logoff.aspx";

		private const string SecurityTokenParamName = "SecurityToken";

		private Cookie backendServerFromUrlCookie;

		private bool isSyndicatedAdmin;

		private bool isSyndicatedAdminManageDownLevelTarget;

		private AnchoredRoutingTarget originalAnchoredRoutingTarget;
	}
}
