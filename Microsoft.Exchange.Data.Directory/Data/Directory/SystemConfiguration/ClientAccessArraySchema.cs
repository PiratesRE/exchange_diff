using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class ClientAccessArraySchema : ADConfigurationObjectSchema
	{
		public static readonly ADPropertyDefinition ExchangeLegacyDN = new ADPropertyDefinition("ExchangeLegacyDN", ExchangeObjectVersion.Exchange2003, typeof(string), "legacyExchangeDN", ADPropertyDefinitionFlags.DoNotProvisionalClone, string.Empty, new PropertyDefinitionConstraint[]
		{
			new ValidLegacyDNConstraint()
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Site = ServerSchema.ServerSite;

		public static readonly ADPropertyDefinition Fqdn = new ADPropertyDefinition("Fqdn", ExchangeObjectVersion.Exchange2003, typeof(string), null, ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ServerSchema.NetworkAddress
		}, new CustomFilterBuilderDelegate(Server.FqdnFilterBuilder), new GetterDelegate(Server.FqdnGetter), new SetterDelegate(ClientAccessArray.FqdnSetter), null, null);

		internal static readonly ADPropertyDefinition NetworkAddress = ServerSchema.NetworkAddress;

		public static readonly ADPropertyDefinition ArrayDefinition = new ADPropertyDefinition("ArrayDefinition", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchServerRedundantMachines", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Servers = new ADPropertyDefinition("Servers", ExchangeObjectVersion.Exchange2007, typeof(ADObjectId), "msExchServerAssociationBL", ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.BackLink, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ServerCount = new ADPropertyDefinition("ServerCount", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchMigrationLogDirectorySizeQuota", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
