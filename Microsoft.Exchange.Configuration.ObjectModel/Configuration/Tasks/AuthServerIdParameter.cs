using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class AuthServerIdParameter : ADIdParameter
	{
		public AuthServerIdParameter()
		{
		}

		public AuthServerIdParameter(string identity) : base(identity)
		{
		}

		public AuthServerIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public AuthServerIdParameter(PartnerApplication app) : base(app.Id)
		{
		}

		public AuthServerIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public static AuthServerIdParameter Parse(string identity)
		{
			return new AuthServerIdParameter(identity);
		}

		internal override ADPropertyDefinition[] AdditionalMatchingProperties
		{
			get
			{
				return new ADPropertyDefinition[]
				{
					AuthServerSchema.IssuerIdentifier
				};
			}
		}
	}
}
