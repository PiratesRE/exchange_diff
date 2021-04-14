using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class ADAddressTypeSchema : ADConfigurationObjectSchema
	{
		public static readonly ADPropertyDefinition FileVersion = new ADPropertyDefinition("FileVersion", ExchangeObjectVersion.Exchange2003, typeof(Version), "FileVersion", ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.Binary, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ProxyGeneratorDll = new ADPropertyDefinition("ProxyGeneratorDll", ExchangeObjectVersion.Exchange2003, typeof(string), "ProxyGeneratorDll", ADPropertyDefinitionFlags.Mandatory, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
