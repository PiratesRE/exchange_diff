using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class SharingPolicySchema : ADConfigurationObjectSchema
	{
		public static readonly ADPropertyDefinition Enabled = new ADPropertyDefinition("Enabled", ExchangeObjectVersion.Exchange2010, typeof(bool), "msExchSharingPolicyIsEnabled", ADPropertyDefinitionFlags.PersistDefaultValue, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Domains = new ADPropertyDefinition("Domains", ExchangeObjectVersion.Exchange2010, typeof(SharingPolicyDomain), "msExchSharingPolicyDomains", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Mandatory, null, SharingPolicyDomainsConstraint.Constrains, SharingPolicyDomainsConstraint.Constrains, null, null);

		public static readonly ADPropertyDefinition Default = new ADPropertyDefinition("Default", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			SharingPolicyNonAdProperties.DefaultPropetyDefinition
		}, null, new GetterDelegate(SharingPolicyNonAdProperties.GetDefault), new SetterDelegate(SharingPolicyNonAdProperties.SetDefault), null, null);
	}
}
