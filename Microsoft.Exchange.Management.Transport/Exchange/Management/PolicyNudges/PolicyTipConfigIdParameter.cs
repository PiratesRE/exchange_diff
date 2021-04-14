using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.PolicyNudges
{
	[Serializable]
	public class PolicyTipConfigIdParameter : ADIdParameter
	{
		public PolicyTipConfigIdParameter()
		{
		}

		public PolicyTipConfigIdParameter(string identity) : base(identity)
		{
		}

		public PolicyTipConfigIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public PolicyTipConfigIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public static PolicyTipConfigIdParameter Parse(string identity)
		{
			return new PolicyTipConfigIdParameter(identity);
		}
	}
}
