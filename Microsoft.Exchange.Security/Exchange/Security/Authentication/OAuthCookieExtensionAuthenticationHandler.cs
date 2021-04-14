using System;
using System.IdentityModel.Tokens;
using System.Web;
using Microsoft.Exchange.Security.OAuth;

namespace Microsoft.Exchange.Security.Authentication
{
	internal class OAuthCookieExtensionAuthenticationHandler : IOAuthExtensionAuthenticationHandler
	{
		public bool TryHandleRequestPreAuthentication(OAuthExtensionContext context, out bool isAuthenticationNeeded)
		{
			isAuthenticationNeeded = true;
			return false;
		}

		public bool TryGetBearerToken(OAuthExtensionContext context, out string token)
		{
			bool result = false;
			token = null;
			HttpContext httpContext = context.HttpContext;
			V1CallbackTokenCookie v1CallbackTokenCookie;
			if (V1CallbackTokenCookie.TryGetCookieFromHttpRequest(httpContext.Request, out v1CallbackTokenCookie))
			{
				token = v1CallbackTokenCookie.RawCookieValue;
				result = true;
			}
			return result;
		}

		public bool TryHandleRequestPostAuthentication(OAuthExtensionContext context)
		{
			HttpContext httpContext = context.HttpContext;
			if (httpContext.Request.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase) && httpContext.Request.QueryString["wa"] == "wsignin1.0")
			{
				OAuthTokenHandler tokenHandler = context.TokenHandler;
				JwtSecurityToken token = tokenHandler.Token;
				LocalTokenIssuer localTokenIssuer = new LocalTokenIssuer(tokenHandler.GetOAuthIdentity().OrganizationId);
				TokenResult selfIssuedV1CallbackToken = localTokenIssuer.GetSelfIssuedV1CallbackToken(httpContext.Request.Url, token);
				V1CallbackTokenCookie.AddCookieToResponse(httpContext, selfIssuedV1CallbackToken);
			}
			return false;
		}
	}
}
