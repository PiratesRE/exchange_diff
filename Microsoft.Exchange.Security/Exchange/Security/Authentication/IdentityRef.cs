using System;
using System.Globalization;
using System.Security.Principal;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Security.Authentication
{
	public class IdentityRef
	{
		public IdentityRef(CommonAccessToken cat, AuthenticationAuthority authenticationAuthority, bool isUsedForCanary) : this(IdentityRef.ExtractIdentityFromCat(cat), authenticationAuthority, isUsedForCanary)
		{
			if (cat == null)
			{
				throw new ArgumentNullException("cat", "You must specify the CommonAccessToken");
			}
			if (string.Equals(cat.TokenType, AccessTokenType.CompositeIdentity.ToString(), StringComparison.Ordinal))
			{
				throw new ArgumentException(string.Format("The CommonAccessToken type must not be '{0}'!", cat.TokenType));
			}
			this.CommonAccessToken = cat;
		}

		public IdentityRef(GenericIdentity identity, AuthenticationAuthority authenticationAuthority, bool isUsedForCanary)
		{
			this.CommonAccessToken = null;
			this.Identity = identity;
			this.Authority = authenticationAuthority;
			this.IsUsedForCanary = isUsedForCanary;
		}

		public CommonAccessToken CommonAccessToken { get; private set; }

		public AuthenticationAuthority Authority { get; private set; }

		public GenericIdentity Identity { get; private set; }

		public bool IsUsedForCanary { get; private set; }

		private static GenericIdentity ExtractIdentityFromCat(CommonAccessToken cat)
		{
			if (cat == null)
			{
				throw new ArgumentNullException("cat", "You must specify the CommonAccessToken");
			}
			if (string.Equals(cat.TokenType, AccessTokenType.CompositeIdentity.ToString(), StringComparison.Ordinal))
			{
				throw new ArgumentException(string.Format("The CommonAccessToken type must not be '{0}'!", cat.TokenType));
			}
			BackendAuthenticator backendAuthenticator = null;
			IPrincipal principal = null;
			string text = null;
			IAccountValidationContext accountValidationContext = null;
			BackendAuthenticator.Rehydrate(cat, ref backendAuthenticator, false, out text, out principal, ref accountValidationContext);
			GenericIdentity genericIdentity = (principal != null) ? (principal.Identity as GenericIdentity) : null;
			if (genericIdentity == null)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "The specified CAT({0}) does not contain a GenericIdentity!", new object[]
				{
					cat.TokenType
				}), "cat");
			}
			return genericIdentity;
		}

		public const int PrimaryIdentityIndex = -1;
	}
}
