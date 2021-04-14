using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class HostedConnectionFilterPolicySchema : ADConfigurationObjectSchema
	{
		private static QueryFilter IsDefaultFilterBuilder(SinglePropertyFilter filter)
		{
			return ADObject.BoolFilterBuilder(filter, new BitMaskAndFilter(HostedConnectionFilterPolicySchema.ConnectionFilterFlags, 1UL));
		}

		internal const int IsDefaultShift = 0;

		internal const int EnableSafeListShift = 2;

		internal const int DirectoryBasedEdgeBlockModeShift = 3;

		internal const int DirectoryBasedEdgeBlockModeLength = 2;

		public static readonly ADPropertyDefinition ConnectionFilterFlags = new ADPropertyDefinition("ConnectionFilterFlags", ExchangeObjectVersion.Exchange2012, typeof(int), "msExchSpamFlags", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition IPAllowList = new ADPropertyDefinition("IPAllowList", ExchangeObjectVersion.Exchange2012, typeof(IPRange), "msExchSpamAllowedIPRanges", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new IPv6AddressesNotAllowedConstraint(),
			new IPRangeConstraint(256UL)
		}, null, null);

		public static readonly ADPropertyDefinition IPBlockList = new ADPropertyDefinition("IPBlockList", ExchangeObjectVersion.Exchange2012, typeof(IPRange), "msExchSpamBlockedIPRanges", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new IPv6AddressesNotAllowedConstraint(),
			new IPRangeConstraint(256UL)
		}, null, null);

		public static readonly ADPropertyDefinition IsDefault = new ADPropertyDefinition("IsDefault", ExchangeObjectVersion.Exchange2012, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			HostedConnectionFilterPolicySchema.ConnectionFilterFlags
		}, new CustomFilterBuilderDelegate(HostedConnectionFilterPolicySchema.IsDefaultFilterBuilder), ADObject.FlagGetterDelegate(HostedConnectionFilterPolicySchema.ConnectionFilterFlags, 1), ADObject.FlagSetterDelegate(HostedConnectionFilterPolicySchema.ConnectionFilterFlags, 1), null, null);

		public static readonly ADPropertyDefinition EnableSafeList = ADObject.BitfieldProperty("EnableSafeList", 2, HostedConnectionFilterPolicySchema.ConnectionFilterFlags);

		public static readonly ADPropertyDefinition DirectoryBasedEdgeBlockMode = ADObject.BitfieldProperty("DirectoryBasedEdgeBlockMode", 3, 2, HostedConnectionFilterPolicySchema.ConnectionFilterFlags);
	}
}
