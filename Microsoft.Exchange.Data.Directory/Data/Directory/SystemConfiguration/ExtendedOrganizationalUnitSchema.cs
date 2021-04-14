using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class ExtendedOrganizationalUnitSchema : ADConfigurationObjectSchema
	{
		public static readonly ADPropertyDefinition UPNSuffixes = SharedPropertyDefinitions.UPNSuffixes;

		public static readonly ADPropertyDefinition DirSyncStatusAck = new ADPropertyDefinition("DirSyncStatusAck", ExchangeObjectVersion.Exchange2003, typeof(string), "msExchDirsyncStatusAck", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);
	}
}
