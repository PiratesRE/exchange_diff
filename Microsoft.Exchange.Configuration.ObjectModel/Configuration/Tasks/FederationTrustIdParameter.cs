using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class FederationTrustIdParameter : ADIdParameter
	{
		public FederationTrustIdParameter()
		{
		}

		public FederationTrustIdParameter(ADObjectId adobjectid) : base(adobjectid)
		{
		}

		public FederationTrustIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		protected FederationTrustIdParameter(string identity) : base(identity)
		{
		}

		public static FederationTrustIdParameter Parse(string identity)
		{
			return new FederationTrustIdParameter(identity);
		}
	}
}
