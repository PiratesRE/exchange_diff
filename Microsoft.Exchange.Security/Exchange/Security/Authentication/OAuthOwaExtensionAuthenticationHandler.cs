using System;
using System.IdentityModel.Tokens;
using System.Security.Principal;
using System.Web;
using Microsoft.Exchange.Diagnostics.Components.Security;
using Microsoft.Exchange.Security.OAuth;

namespace Microsoft.Exchange.Security.Authentication
{
	internal class OAuthOwaExtensionAuthenticationHandler : IOAuthExtensionAuthenticationHandler
	{
		public bool TryHandleRequestPreAuthentication(OAuthExtensionContext context, out bool isAuthenticationNeeded)
		{
			HttpContext httpContext = context.HttpContext;
			isAuthenticationNeeded = true;
			if (!ClientAuthenticationHelper.IsOwaRequest(httpContext))
			{
				return false;
			}
			if (AuthCommon.IsFrontEnd && ClientAuthenticationHelper.IsOwaAnonymousRequest(httpContext))
			{
				isAuthenticationNeeded = false;
				return true;
			}
			Uri requestUrlEvenIfProxied = httpContext.Request.GetRequestUrlEvenIfProxied();
			bool flag = Utility.IsResourceRequest(requestUrlEvenIfProxied.LocalPath);
			if (flag && (!AuthCommon.IsFrontEnd || Utility.IsOwaRequestWithRoutingHint(httpContext.Request) || Utility.HasResourceRoutingHint(httpContext.Request) || Utility.IsAnonymousResourceRequest(httpContext.Request)))
			{
				httpContext.User = new WindowsPrincipal(WindowsIdentity.GetAnonymous());
				ExTraceGlobals.OAuthTracer.TraceDebug<Uri>(0L, "This is a resource request: {0}", requestUrlEvenIfProxied);
				isAuthenticationNeeded = false;
				return true;
			}
			return false;
		}

		public bool TryGetBearerToken(OAuthExtensionContext context, out string token)
		{
			token = null;
			return false;
		}

		public bool TryHandleRequestPostAuthentication(OAuthExtensionContext context)
		{
			HttpContext httpContext = context.HttpContext;
			if (!ClientAuthenticationHelper.IsOwaRequest(httpContext))
			{
				return false;
			}
			if (httpContext.Request.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase) && httpContext.Request.QueryString["wa"] == "wsignin1.0")
			{
				ExTraceGlobals.OAuthTracer.TraceDebug(0L, "[OAuthOwaExtensionAuthenticationHandler::TryHandleRequestPostAuthentication] Setting default anchormailbox cookie");
				OAuthTokenHandler tokenHandler = context.TokenHandler;
				this.SetDefaultAnchorMailboxCookie(httpContext, tokenHandler.Token);
				Uri requestUrlEvenIfProxied = httpContext.Request.GetRequestUrlEvenIfProxied();
				string text;
				string url = ClientAuthenticationHelper.ShouldRedirectQueryParamsAsHashes(requestUrlEvenIfProxied, out text) ? text : requestUrlEvenIfProxied.OriginalString;
				httpContext.Response.Redirect(url);
			}
			else if (httpContext.Request.Cookies["DefaultAnchorMailbox"] == null)
			{
				ExTraceGlobals.OAuthTracer.TraceDebug(0L, "[OAuthOwaExtensionAuthenticationHandler::TryHandleRequestPostAuthentication] DefaultAnchorMailbox cookie was not present.");
				httpContext.Response.AppendToLog("pltRoutingCookieMissingOAuth=1");
			}
			return true;
		}

		private void SetDefaultAnchorMailboxCookie(HttpContext httpContext, JwtSecurityToken token)
		{
			string text;
			OAuthCommon.TryGetClaimValue(token, Constants.ClaimTypes.Upn, out text);
			ExTraceGlobals.OAuthTracer.TraceDebug<string>(0L, "Setting upn: {0} as default anchor mailbox", text);
			HttpCookie httpCookie = new HttpCookie("DefaultAnchorMailbox", text);
			httpCookie.HttpOnly = false;
			httpCookie.Secure = httpContext.Request.IsSecureConnection;
			httpCookie.Expires = V1CallbackTokenCookie.TryGetCookieExpiryFromContext(httpContext);
			httpContext.Response.SetCookie(httpCookie);
		}
	}
}
