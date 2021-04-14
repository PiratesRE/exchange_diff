using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Provisioning;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class RecipientEnforcementProvisioningPolicyIdParameter : ProvisioningPolicyIdParameter
	{
		public RecipientEnforcementProvisioningPolicyIdParameter()
		{
		}

		public RecipientEnforcementProvisioningPolicyIdParameter(string identity) : base(identity)
		{
		}

		public RecipientEnforcementProvisioningPolicyIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public RecipientEnforcementProvisioningPolicyIdParameter(ADProvisioningPolicy policy) : base(policy.Id)
		{
		}

		public RecipientEnforcementProvisioningPolicyIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public new static ProvisioningPolicyIdParameter Parse(string identity)
		{
			return new RecipientEnforcementProvisioningPolicyIdParameter(identity);
		}

		protected override SharedTenantConfigurationMode SharedTenantConfigurationMode
		{
			get
			{
				return SharedTenantConfigurationMode.Static;
			}
		}
	}
}
