using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class LegacyGwartSchema : ADConfigurationObjectSchema
	{
		public static readonly ADPropertyDefinition GwartLastModified = new ADPropertyDefinition("GwartLastModified", ExchangeObjectVersion.Exchange2003, typeof(DateTime?), DateTimeFormatProvider.UTC, "gWARTLastModified", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);
	}
}
