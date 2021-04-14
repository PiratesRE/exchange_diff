using System;

namespace Microsoft.Exchange.Security.Authentication
{
	internal interface IOAuthExtensionAuthenticationHandler
	{
		bool TryHandleRequestPreAuthentication(OAuthExtensionContext context, out bool isAuthenticationNeeded);

		bool TryGetBearerToken(OAuthExtensionContext context, out string token);

		bool TryHandleRequestPostAuthentication(OAuthExtensionContext context);
	}
}
