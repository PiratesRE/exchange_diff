using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class ADSiteLinkSchema : ADConfigurationObjectSchema
	{
		public static readonly ADPropertyDefinition ADCost = new ADPropertyDefinition("ADCost", ExchangeObjectVersion.Exchange2003, typeof(int), "cost", ADPropertyDefinitionFlags.PersistDefaultValue, 100, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, int.MaxValue)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExchangeCost = new ADPropertyDefinition("ExchangeCost", ExchangeObjectVersion.Exchange2007, typeof(int?), "msExchCost", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new RangedNullableValueConstraint<int>(0, int.MaxValue)
		}, new PropertyDefinitionConstraint[]
		{
			new RangedNullableValueConstraint<int>(1, 99999)
		}, null, null);

		public static readonly ADPropertyDefinition Sites = new ADPropertyDefinition("Sites", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), "siteList", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.DoNotValidate, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Cost = new ADPropertyDefinition("Cost", ExchangeObjectVersion.Exchange2007, typeof(int), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADSiteLinkSchema.ADCost,
			ADSiteLinkSchema.ExchangeCost
		}, null, new GetterDelegate(ADSiteLink.CostGetter), null, null, null);

		public static readonly ADPropertyDefinition MaxMessageSize = new ADPropertyDefinition("MaxMessageSize", ExchangeObjectVersion.Exchange2007, typeof(Unlimited<ByteQuantifiedSize>), ByteQuantifiedSize.KilobyteQuantifierProvider, "delivContLength", ADPropertyDefinitionFlags.None, Unlimited<ByteQuantifiedSize>.UnlimitedValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
