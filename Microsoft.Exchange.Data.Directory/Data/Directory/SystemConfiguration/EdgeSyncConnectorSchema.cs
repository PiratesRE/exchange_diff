using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class EdgeSyncConnectorSchema : ADConfigurationObjectSchema
	{
		public static readonly ADPropertyDefinition Enabled = new ADPropertyDefinition("Enabled", ExchangeObjectVersion.Exchange2007, typeof(bool), "Enabled", ADPropertyDefinitionFlags.PersistDefaultValue, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SynchronizationProvider = new ADPropertyDefinition("SynchronizationProvider", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchEdgeSyncSynchronizationProvider", ADPropertyDefinitionFlags.Mandatory, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AssemblyPath = new ADPropertyDefinition("AssemblyPath", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchEdgeSyncProviderAssemblyPath", ADPropertyDefinitionFlags.Mandatory, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
