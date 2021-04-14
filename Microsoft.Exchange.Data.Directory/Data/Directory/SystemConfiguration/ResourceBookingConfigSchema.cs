using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class ResourceBookingConfigSchema : ADConfigurationObjectSchema
	{
		public static readonly ADPropertyDefinition ResourcePropertySchema = new ADPropertyDefinition("ResourcePropertySchema", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchResourcePropertySchema", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new ResourcePropertyConstraint()
		}, null, null);
	}
}
