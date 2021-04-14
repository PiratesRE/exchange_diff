using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class GlobalLocatorServiceTenantSchema : ObjectSchema
	{
		public static readonly SimpleProviderPropertyDefinition ExternalDirectoryOrganizationId = new SimpleProviderPropertyDefinition("ExternalDirectoryOrganizationId", ExchangeObjectVersion.Exchange2012, typeof(Guid?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ADPropertyDefinition DomainNames = new ADPropertyDefinition("DomainNames", ExchangeObjectVersion.Exchange2012, typeof(string), "domainNames", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly SimpleProviderPropertyDefinition ResourceForest = new SimpleProviderPropertyDefinition("ResourceForest", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition AccountForest = new SimpleProviderPropertyDefinition("AccountForest", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition PrimarySite = new SimpleProviderPropertyDefinition("PrimarySite", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition SmtpNextHopDomain = new SimpleProviderPropertyDefinition("SmtpNextHopDomain", ExchangeObjectVersion.Exchange2012, typeof(SmtpDomain), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition TenantFlags = new SimpleProviderPropertyDefinition("TenantFlags", ExchangeObjectVersion.Exchange2012, typeof(GlsTenantFlags), PropertyDefinitionFlags.None, GlsTenantFlags.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition TenantContainerCN = new SimpleProviderPropertyDefinition("TenantContainerCN", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ResumeCache = new SimpleProviderPropertyDefinition("ResumeCache", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition IsOfflineData = new SimpleProviderPropertyDefinition("IsOfflineData", ExchangeObjectVersion.Exchange2012, typeof(bool), PropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
