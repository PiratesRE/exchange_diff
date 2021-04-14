using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class OnPremisesOrganizationSchema : ADConfigurationObjectSchema
	{
		public static readonly ADPropertyDefinition OrganizationGuid = new ADPropertyDefinition("OrganizationGuid", ExchangeObjectVersion.Exchange2007, typeof(Guid), "msExchOnPremisesOrganizationGuid", ADPropertyDefinitionFlags.WriteOnce | ADPropertyDefinitionFlags.Binary, System.Guid.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition HybridDomains = new ADPropertyDefinition("HybridDomains", ExchangeObjectVersion.Exchange2007, typeof(SmtpDomain), "msExchCoexistenceDomains", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition InboundConnectorLink = new ADPropertyDefinition("InboundConnectorLink", ExchangeObjectVersion.Exchange2007, typeof(ADObjectId), "msExchOnPremisesInboundConnectorLink", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition OutboundConnectorLink = new ADPropertyDefinition("OutboundConnectorLink", ExchangeObjectVersion.Exchange2007, typeof(ADObjectId), "msExchOnPremisesOutboundConnectorLink", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition OrganizationRelationshipLink = new ADPropertyDefinition("OrganizationRelationshipLink", ExchangeObjectVersion.Exchange2007, typeof(ADObjectId), "msExchTrustedDomainLink", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition OrganizationName = new ADPropertyDefinition("OrganizationName", ExchangeObjectVersion.Exchange2007, typeof(string), "AdminDescription", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new UIImpactStringLengthConstraint(0, 1024)
		}, null, null);
	}
}
