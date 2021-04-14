using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class AvailabilityConfigSchema : ADConfigurationObjectSchema
	{
		public static readonly ADPropertyDefinition PerUserAccount = new ADPropertyDefinition("PerUserAccount", ExchangeObjectVersion.Exchange2007, typeof(ADObjectId), "msExchAvailabilityPerUserAccount", ADPropertyDefinitionFlags.DoNotValidate, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition OrgWideAccount = new ADPropertyDefinition("OrgWideAccount", ExchangeObjectVersion.Exchange2007, typeof(ADObjectId), "msExchAvailabilityOrgWideAccount", ADPropertyDefinitionFlags.DoNotValidate, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
