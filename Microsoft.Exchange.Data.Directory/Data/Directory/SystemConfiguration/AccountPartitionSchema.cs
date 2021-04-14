using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class AccountPartitionSchema : ADObjectSchema
	{
		public static readonly ADPropertyDefinition ProvisioningFlags = SharedPropertyDefinitions.ProvisioningFlags;

		public static readonly ADPropertyDefinition TrustedDomainLink = new ADPropertyDefinition("ms-Exch-Trusted-Domain-Link", ExchangeObjectVersion.Exchange2007, typeof(ADObjectId), "msExchTrustedDomainLink", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition IsLocalForest = new ADPropertyDefinition("IsLocalForest", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			AccountPartitionSchema.ProvisioningFlags
		}, new CustomFilterBuilderDelegate(AccountPartition.IsLocalForestFilterBuilder), ADObject.FlagGetterDelegate(AccountPartitionSchema.ProvisioningFlags, 1), ADObject.FlagSetterDelegate(AccountPartitionSchema.ProvisioningFlags, 1), null, null);

		public static readonly ADPropertyDefinition PartitionId = new ADPropertyDefinition("PartitionId", ExchangeObjectVersion.Exchange2007, typeof(PartitionId), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			AccountPartitionSchema.ProvisioningFlags,
			AccountPartitionSchema.TrustedDomainLink,
			ADObjectSchema.Id,
			ADObjectSchema.ObjectState
		}, null, new GetterDelegate(AccountPartition.PartitionIdGetter), null, null, null);

		public static readonly ADPropertyDefinition EnabledForProvisioning = new ADPropertyDefinition("EnabledForProvisioning", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			AccountPartitionSchema.ProvisioningFlags
		}, null, ADObject.FlagGetterDelegate(AccountPartitionSchema.ProvisioningFlags, 2), ADObject.FlagSetterDelegate(AccountPartitionSchema.ProvisioningFlags, 2), null, null);

		public static readonly ADPropertyDefinition IsSecondary = new ADPropertyDefinition("IsSecondary", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			AccountPartitionSchema.ProvisioningFlags
		}, new CustomFilterBuilderDelegate(AccountPartition.IsSecondaryFilterBuilder), ADObject.FlagGetterDelegate(AccountPartitionSchema.ProvisioningFlags, 4), ADObject.FlagSetterDelegate(AccountPartitionSchema.ProvisioningFlags, 4), null, null);
	}
}
