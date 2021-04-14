using System;
using System.Globalization;
using System.Security.Principal;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Security.Authentication
{
	internal class CompositeIdentityAuthenticator : BackendAuthenticator
	{
		protected override string[] RequiredFields
		{
			get
			{
				return CompositeIdentityAuthenticator.requiredFields;
			}
		}

		protected override void InternalGetAuthIdentifier(CommonAccessToken token, out string authIdentifier)
		{
			authIdentifier = null;
			string token2;
			if (token.ExtensionData.TryGetValue("PrimaryIdentityToken", out token2))
			{
				CommonAccessToken token3 = CommonAccessToken.Deserialize(token2);
				if (!BackendAuthenticator.TryGetAuthIdentifierFromUserSid(token3, out authIdentifier))
				{
					authIdentifier = null;
				}
			}
		}

		protected override void InternalRehydrate(CommonAccessToken token, bool wantAuthIdentifier, out string authIdentifier, out IPrincipal principal, ref IAccountValidationContext accountValidationContext)
		{
			throw new NotImplementedException();
		}

		protected override void InternalRehydrate(CommonAccessToken token, bool wantAuthIdentifier, out string authIdentifier, out IPrincipal principal)
		{
			int num;
			if (!int.TryParse(token.ExtensionData["SecondaryIdentityTokensCount"], out num))
			{
				throw new BackendRehydrationException(SecurityStrings.InvalidExtensionDataKey("SecondaryIdentityTokensCount"));
			}
			string s;
			int num2;
			if (!token.ExtensionData.TryGetValue("CanaryIdentityIndex", out s) || !int.TryParse(s, out num2) || num2 < -1 || num2 >= num)
			{
				throw new BackendRehydrationException(SecurityStrings.InvalidExtensionDataKey("CanaryIdentityIndex"));
			}
			IdentityRef identity = this.GetIdentity(token, "PrimaryIdentityToken", num2 == -1);
			IdentityRef[] array = new IdentityRef[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = this.GetIdentity(token, string.Format(CultureInfo.InvariantCulture, "SecondaryIdentityToken{0}", new object[]
				{
					i
				}), i == num2);
			}
			if (wantAuthIdentifier)
			{
				authIdentifier = identity.Identity.GetSecurityIdentifier().ToString();
			}
			else
			{
				authIdentifier = null;
			}
			principal = new GenericPrincipal(new CompositeIdentity(identity, array), null);
		}

		private IdentityRef GetIdentity(CommonAccessToken compositeToken, string tokenKey, bool isUsedForCanary)
		{
			string token;
			if (!compositeToken.ExtensionData.TryGetValue(tokenKey, out token))
			{
				throw new BackendRehydrationException(SecurityStrings.MissingExtensionDataKey(tokenKey));
			}
			CommonAccessToken commonAccessToken = CommonAccessToken.Deserialize(token);
			AuthenticationAuthority authenticationAuthority;
			if (!Enum.TryParse<AuthenticationAuthority>(commonAccessToken.ExtensionData["AuthenticationAuthority"], out authenticationAuthority))
			{
				throw new BackendRehydrationException(SecurityStrings.InvalidExtensionDataKey("AuthenticationAuthority"));
			}
			return new IdentityRef(commonAccessToken, authenticationAuthority, isUsedForCanary);
		}

		private static readonly string[] requiredFields = new string[]
		{
			"PrimaryIdentityToken",
			"SecondaryIdentityTokensCount"
		};
	}
}
