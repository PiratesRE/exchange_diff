using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class ADAvailabilityForeignConnectorVirtualDirectorySchema : ExchangeVirtualDirectorySchema
	{
		public static readonly ADPropertyDefinition AvailabilityForeignConnectorType = new ADPropertyDefinition("AvailabilityForeignConnectorType", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchAvailabilityForeignConnectorType", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AvailabilityForeignConnectorDomains = new ADPropertyDefinition("AvailabilityForeignConnectorDomains", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchAvailabilityForeignConnectorDomain", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
