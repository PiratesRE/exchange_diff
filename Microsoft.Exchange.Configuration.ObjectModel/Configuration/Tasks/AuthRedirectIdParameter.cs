using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class AuthRedirectIdParameter : ADIdParameter, IIdentityParameter
	{
		public AuthRedirectIdParameter()
		{
		}

		public AuthRedirectIdParameter(string identity) : base(identity)
		{
		}

		public AuthRedirectIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public AuthRedirectIdParameter(AuthRedirect authRedirect) : base(authRedirect.Id)
		{
		}

		public AuthRedirectIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public static AuthRedirectIdParameter Parse(string identity)
		{
			return new AuthRedirectIdParameter(identity);
		}

		internal override ADPropertyDefinition[] AdditionalMatchingProperties
		{
			get
			{
				return new ADPropertyDefinition[]
				{
					AuthRedirectSchema.AuthScheme
				};
			}
		}
	}
}
