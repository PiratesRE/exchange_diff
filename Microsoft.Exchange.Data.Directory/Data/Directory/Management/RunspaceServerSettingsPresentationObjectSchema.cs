using System;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal class RunspaceServerSettingsPresentationObjectSchema : SimpleProviderObjectSchema
	{
		public static readonly SimpleProviderPropertyDefinition UserPreferredGlobalCatalog = new SimpleProviderPropertyDefinition("UserPreferredGlobalCatalog", ExchangeObjectVersion.Exchange2010, typeof(Fqdn), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition DefaultGlobalCatalog = new SimpleProviderPropertyDefinition("DefaultGlobalCatalog", ExchangeObjectVersion.Exchange2010, typeof(Fqdn), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition DefaultConfigurationDomainController = new SimpleProviderPropertyDefinition("DefaultConfigurationDomainController", ExchangeObjectVersion.Exchange2010, typeof(Fqdn), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition DefaultPreferredDomainControllers = new SimpleProviderPropertyDefinition("DefaultPreferredDomainControllers", ExchangeObjectVersion.Exchange2010, typeof(Fqdn), PropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition DefaultConfigurationDomainControllersForAllForests = new SimpleProviderPropertyDefinition("DefaultConfigurationDomainControllersForAllForests", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition DefaultGlobalCatalogsForAllForests = new SimpleProviderPropertyDefinition("DefaultGlobalCatalogsForAllForests", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition UserPreferredConfigurationDomainController = new SimpleProviderPropertyDefinition("UserPreferredConfigurationDomainController", ExchangeObjectVersion.Exchange2010, typeof(Fqdn), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition UserPreferredDomainControllers = new SimpleProviderPropertyDefinition("UserPreferredDomainControllers", ExchangeObjectVersion.Exchange2010, typeof(Fqdn), PropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition RecipientViewRoot = new SimpleProviderPropertyDefinition("RecipientViewRoot", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ViewEntireForest = new SimpleProviderPropertyDefinition("ViewEntireForest", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.None, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition WriteOriginatingChangeTimestamp = new SimpleProviderPropertyDefinition("WriteOriginatingChangeTimestamp", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.None, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition WriteShadowProperties = new SimpleProviderPropertyDefinition("WriteShadowProperties", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.None, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
