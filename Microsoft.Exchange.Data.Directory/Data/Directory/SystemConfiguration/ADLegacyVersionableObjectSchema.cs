using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal abstract class ADLegacyVersionableObjectSchema : ADConfigurationObjectSchema
	{
		public static readonly ADPropertyDefinition MinAdminVersion = new ADPropertyDefinition("MinAdminVersion", ExchangeObjectVersion.Exchange2003, typeof(int?), "msExchMinAdminVersion", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
