using System;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class RootDseSchema : ADNonExchangeObjectSchema
	{
		public static readonly ADPropertyDefinition DefaultNamingContext = new ADPropertyDefinition("DefaultNamingContext", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), "defaultNamingContext", ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Mandatory, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ConfigurationNamingContext = new ADPropertyDefinition("ConfigurationNamingContext", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), "configurationNamingContext", ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Mandatory, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Fqdn = new ADPropertyDefinition("Fqdn", ExchangeObjectVersion.Exchange2003, typeof(string), "dnsHostName", ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Mandatory, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition HighestCommittedUSN = new ADPropertyDefinition("HighestCommittedUSN", ExchangeObjectVersion.Exchange2003, typeof(long), "highestCommittedUSN", ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.PersistDefaultValue, 0L, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition IsSynchronized = new ADPropertyDefinition("IsSynchronized", ExchangeObjectVersion.Exchange2003, typeof(bool), "isSynchronized", ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.PersistDefaultValue, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition CurrentTimeRaw = new ADPropertyDefinition("CurrentTimeRaw", ExchangeObjectVersion.Exchange2003, typeof(string), "currentTime", ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Mandatory, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition CurrentTime = new ADPropertyDefinition("CurrentTime", ExchangeObjectVersion.Exchange2003, typeof(ExDateTime), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, ExDateTime.MinValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			RootDseSchema.CurrentTimeRaw
		}, null, new GetterDelegate(RootDse.CurrentTimeGetter), null, null, null);

		public static readonly ADPropertyDefinition NtDsDsa = new ADPropertyDefinition("NtDsDsa", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), "dsServiceName", ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Mandatory, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Site = new ADPropertyDefinition("Site", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			RootDseSchema.NtDsDsa
		}, null, new GetterDelegate(RootDse.SiteGetter), null, null, null);
	}
}
