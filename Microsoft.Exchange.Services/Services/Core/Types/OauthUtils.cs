using System;
using System.Net;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Security.OAuth;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal static class OauthUtils
	{
		public static ICredentials GetOauthCredential(ADUser user)
		{
			return OAuthCredentials.GetOAuthCredentialsForAppActAsToken(user.OrganizationId, user, null);
		}

		public static ICredentials GetOauthCredential(MiniRecipient user)
		{
			return OAuthCredentials.GetOAuthCredentialsForAppActAsToken(user.OrganizationId, user, null);
		}
	}
}
