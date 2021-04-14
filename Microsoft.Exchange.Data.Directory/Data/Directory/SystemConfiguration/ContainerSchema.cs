using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class ContainerSchema : ADLegacyVersionableObjectSchema
	{
		public static readonly ADPropertyDefinition EdgeSyncCookies = SharedPropertyDefinitions.EdgeSyncCookies;

		public static ADPropertyDefinition CanaryData0 = new ADPropertyDefinition("msExchCanaryData0", ExchangeObjectVersion.Exchange2007, typeof(byte[]), "msExchCanaryData0", ADPropertyDefinitionFlags.Binary, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static ADPropertyDefinition CanaryData1 = new ADPropertyDefinition("msExchCanaryData1", ExchangeObjectVersion.Exchange2007, typeof(byte[]), "msExchCanaryData1", ADPropertyDefinitionFlags.Binary, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static ADPropertyDefinition CanaryData2 = new ADPropertyDefinition("msExchCanaryData2", ExchangeObjectVersion.Exchange2007, typeof(byte[]), "msExchCanaryData2", ADPropertyDefinitionFlags.Binary, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
