using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class SyncDaemonArbitrationConfigSchema : ADConfigurationObjectSchema
	{
		public static readonly ADPropertyDefinition MinVersion = new ADPropertyDefinition("MinVersion", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchSyncDaemonMinVersion", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MaxVersion = new ADPropertyDefinition("MaxVersion", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchSyncDaemonMaxVersion", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ActiveInstanceSleepInterval = new ADPropertyDefinition("ActiveInstanceSleepInterval", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchActiveInstanceSleepInterval", ADPropertyDefinitionFlags.PersistDefaultValue, 0, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, int.MaxValue)
		}, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, int.MaxValue)
		}, null, null);

		public static readonly ADPropertyDefinition PassiveInstanceSleepInterval = new ADPropertyDefinition("PassiveInstanceSleepInterval", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchPassiveInstanceSleepInterval", ADPropertyDefinitionFlags.PersistDefaultValue, 0, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, int.MaxValue)
		}, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, int.MaxValue)
		}, null, null);

		public static readonly ADPropertyDefinition IsEnabled = new ADPropertyDefinition("IsEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			SharedPropertyDefinitions.ProvisioningFlags
		}, null, ADObject.FlagGetterDelegate(SharedPropertyDefinitions.ProvisioningFlags, 1), ADObject.FlagSetterDelegate(SharedPropertyDefinitions.ProvisioningFlags, 1), null, null);

		public static readonly ADPropertyDefinition IsHalted = new ADPropertyDefinition("IsHalted", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			SharedPropertyDefinitions.ProvisioningFlags
		}, null, ADObject.FlagGetterDelegate(SharedPropertyDefinitions.ProvisioningFlags, 2), ADObject.FlagSetterDelegate(SharedPropertyDefinitions.ProvisioningFlags, 2), null, null);

		public static readonly ADPropertyDefinition IsHaltRecoveryDisabled = new ADPropertyDefinition("IsHaltRecoveryDisabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			SharedPropertyDefinitions.ProvisioningFlags
		}, null, ADObject.FlagGetterDelegate(SharedPropertyDefinitions.ProvisioningFlags, 4), ADObject.FlagSetterDelegate(SharedPropertyDefinitions.ProvisioningFlags, 4), null, null);
	}
}
