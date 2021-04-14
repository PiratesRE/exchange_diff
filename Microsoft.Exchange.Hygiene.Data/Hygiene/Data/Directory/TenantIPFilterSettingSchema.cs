using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	internal class TenantIPFilterSettingSchema : ADConfigurationObjectSchema
	{
		public static readonly ADPropertyDefinition AllowedIPRanges = new ADPropertyDefinition("AllowedIPRanges", ExchangeObjectVersion.Exchange2007, typeof(IPRange), "AllowedIPRanges", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Mandatory, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition BlockedIPRanges = new ADPropertyDefinition("BlockedIPRanges", ExchangeObjectVersion.Exchange2007, typeof(IPRange), "BlockedIPRanges", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Mandatory, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
