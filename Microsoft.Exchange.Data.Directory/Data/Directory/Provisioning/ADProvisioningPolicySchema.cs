using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Provisioning
{
	internal class ADProvisioningPolicySchema : ADConfigurationObjectSchema
	{
		public static readonly ADPropertyDefinition TargetObjects = new ADPropertyDefinition("TargetObjects", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchProvisioningPolicyTargetObjects", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.WriteOnce, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition PolicyType = new ADPropertyDefinition("PolicyType", ExchangeObjectVersion.Exchange2010, typeof(ProvisioningPolicyType), "msExchProvisioningPolicyType", ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.PersistDefaultValue | ADPropertyDefinitionFlags.WriteOnce, ProvisioningPolicyType.Template, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(ProvisioningPolicyType))
		}, null, null);

		public static readonly ADPropertyDefinition Scopes = new ADPropertyDefinition("Scopes", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), "msExchProvisioningPolicyScopeLinks", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.WriteOnce, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
