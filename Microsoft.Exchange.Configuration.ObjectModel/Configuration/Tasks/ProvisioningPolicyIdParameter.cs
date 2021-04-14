using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Provisioning;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class ProvisioningPolicyIdParameter : ADIdParameter
	{
		public ProvisioningPolicyIdParameter()
		{
		}

		public ProvisioningPolicyIdParameter(string identity) : base(identity)
		{
		}

		public ProvisioningPolicyIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public ProvisioningPolicyIdParameter(ADProvisioningPolicy policy) : base(policy.Id)
		{
		}

		public ProvisioningPolicyIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public static ProvisioningPolicyIdParameter Parse(string identity)
		{
			return new ProvisioningPolicyIdParameter(identity);
		}
	}
}
