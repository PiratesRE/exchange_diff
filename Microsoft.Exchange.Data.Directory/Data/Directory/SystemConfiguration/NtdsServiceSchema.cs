using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class NtdsServiceSchema : ADNonExchangeObjectSchema
	{
		public static readonly ADPropertyDefinition Heuristics = new ADPropertyDefinition("Heuristics", ExchangeObjectVersion.Exchange2003, typeof(string), "dsHeuristics", ADPropertyDefinitionFlags.ReadOnly, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TombstoneLifetime = new ADPropertyDefinition("TombstoneLifetime", ExchangeObjectVersion.Exchange2003, typeof(int), "tombstoneLifetime", ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.PersistDefaultValue, 60, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DoListObject = new ADPropertyDefinition("DoListObject", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			NtdsServiceSchema.Heuristics
		}, null, new GetterDelegate(NtdsService.DoListObjectGetter), null, null, null);
	}
}
