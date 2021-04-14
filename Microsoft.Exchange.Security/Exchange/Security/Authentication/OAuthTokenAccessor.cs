using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Security.OAuth;

namespace Microsoft.Exchange.Security.Authentication
{
	internal class OAuthTokenAccessor : CommonAccessTokenAccessor
	{
		private OAuthTokenAccessor(CommonAccessToken token) : base(token)
		{
		}

		public override AccessTokenType TokenType
		{
			get
			{
				return AccessTokenType.OAuth;
			}
		}

		public static OAuthTokenAccessor Create(OAuthIdentity identity)
		{
			if (identity == null)
			{
				throw new ArgumentNullException("identity");
			}
			CommonAccessToken commonAccessToken = new CommonAccessToken(AccessTokenType.OAuth);
			commonAccessToken.Version = 2;
			commonAccessToken.ExtensionData["OrganizationIdBase64"] = CommonAccessTokenAccessor.SerializeOrganizationId(identity.OrganizationId);
			commonAccessToken.ExtensionData["AppOnly"] = identity.IsAppOnly.ToString();
			identity.OAuthApplication.AddExtensionDataToCommonAccessToken(commonAccessToken);
			if (!identity.IsAppOnly)
			{
				identity.ActAsUser.AddExtensionDataToCommonAccessToken(commonAccessToken);
			}
			return new OAuthTokenAccessor(commonAccessToken);
		}

		public static readonly int MinVersion = new ServerVersion(15, 0, 788, 0).ToInt();
	}
}
