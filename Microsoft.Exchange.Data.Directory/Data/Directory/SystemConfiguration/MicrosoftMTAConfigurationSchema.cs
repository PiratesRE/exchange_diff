using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class MicrosoftMTAConfigurationSchema : ADLegacyVersionableObjectSchema
	{
		public static readonly ADPropertyDefinition ExchangeLegacyDN = ServerSchema.ExchangeLegacyDN;

		public static readonly ADPropertyDefinition LocalDesig = new ADPropertyDefinition("LocalDesig", ExchangeObjectVersion.Exchange2003, typeof(string), "mTALocalDesig", ADPropertyDefinitionFlags.Mandatory, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TransRetryMins = new ADPropertyDefinition("TransRetryMins", ExchangeObjectVersion.Exchange2003, typeof(int), "transRetryMins", ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TransTimeoutMins = new ADPropertyDefinition("TransTimeoutMins", ExchangeObjectVersion.Exchange2003, typeof(int), "transTimeoutMins", ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
