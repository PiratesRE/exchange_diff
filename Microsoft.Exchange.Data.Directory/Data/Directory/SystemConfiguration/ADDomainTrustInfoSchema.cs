using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class ADDomainTrustInfoSchema : ADNonExchangeObjectSchema
	{
		public static readonly ADPropertyDefinition TargetName = new ADPropertyDefinition("TargetName", ExchangeObjectVersion.Exchange2003, typeof(string), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADObjectSchema.RawName
		}, new CustomFilterBuilderDelegate(ADObject.DummyCustomFilterBuilderDelegate), new GetterDelegate(ADObject.NameGetter), null, null, null);

		public static readonly ADPropertyDefinition TrustDirection = new ADPropertyDefinition("TrustDirection", ExchangeObjectVersion.Exchange2003, typeof(TrustDirectionFlag), "trustDirection", ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.FilterOnly, TrustDirectionFlag.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TrustType = new ADPropertyDefinition("TrustType", ExchangeObjectVersion.Exchange2003, typeof(TrustTypeFlag), "trustType", ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.FilterOnly, TrustTypeFlag.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TrustAttributes = new ADPropertyDefinition("TrustAttributes", ExchangeObjectVersion.Exchange2003, typeof(TrustAttributeFlag), "trustAttributes", ADPropertyDefinitionFlags.ReadOnly, TrustAttributeFlag.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
