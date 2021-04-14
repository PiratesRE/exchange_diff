using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class ADSiteSchema : ADConfigurationObjectSchema
	{
		internal static readonly ADPropertyDefinition ADSiteFlags = new ADPropertyDefinition("ADSiteFlags", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchTransportSiteFlags", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition HubSiteEnabled = ADObject.BitfieldProperty("HubSiteEnabled", 0, ADSiteSchema.ADSiteFlags);

		public static readonly ADPropertyDefinition InboundMailDisabled = ADObject.BitfieldProperty("InboundMailDisabled", 1, ADSiteSchema.ADSiteFlags);

		public static readonly ADPropertyDefinition PartnerId = new ADPropertyDefinition("PartnerId", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchPartnerId", ADPropertyDefinitionFlags.PersistDefaultValue, -1, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MinorPartnerId = new ADPropertyDefinition("MinorPartnerId", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchMinorPartnerId", ADPropertyDefinitionFlags.PersistDefaultValue, -1, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		internal static readonly ADPropertyDefinition ResponsibleForSites = new ADPropertyDefinition("ResponsibleForSites", ExchangeObjectVersion.Exchange2007, typeof(ADObjectId), "msExchResponsibleForSites", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.DoNotValidate, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
