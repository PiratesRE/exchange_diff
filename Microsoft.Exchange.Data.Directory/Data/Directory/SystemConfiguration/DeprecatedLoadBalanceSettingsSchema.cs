using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(ConfigScopes.Global)]
	internal class DeprecatedLoadBalanceSettingsSchema : ADConfigurationObjectSchema
	{
		public static readonly ADPropertyDefinition IncludedMailboxDatabases = new ADPropertyDefinition("IncludedMailboxDatabases", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), "msExchIncludedMailboxDatabases", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.ValidateInFirstOrganization, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition UseIncludedMailboxDatabases = new ADPropertyDefinition("UseIncludedMailboxDatabases", ExchangeObjectVersion.Exchange2010, typeof(bool), "msExchUseIncludedMailboxDatabases", ADPropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExcludedMailboxDatabases = new ADPropertyDefinition("ExcludedMailboxDatabases", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), "msExchExcludedMailboxDatabases", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.ValidateInFirstOrganization, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition UseExcludedMailboxDatabases = new ADPropertyDefinition("UseExcludedMailboxDatabases", ExchangeObjectVersion.Exchange2010, typeof(bool), "msExchUseExcludedMailboxDatabases", ADPropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
