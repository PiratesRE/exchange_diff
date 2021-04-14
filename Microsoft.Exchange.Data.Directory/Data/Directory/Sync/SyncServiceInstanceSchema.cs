using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	internal class SyncServiceInstanceSchema : ADConfigurationObjectSchema
	{
		public static readonly ADPropertyDefinition AccountPartition = new ADPropertyDefinition("AccountPartition", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), "msExchAccountForestLink", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

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

		public static readonly ADPropertyDefinition IsEnabled = ADObject.BitfieldProperty("IsEnabled", 0, SharedPropertyDefinitions.ProvisioningFlags);

		public static readonly ADPropertyDefinition UseCentralConfig = ADObject.BitfieldProperty("UseCentralConfig", 3, SharedPropertyDefinitions.ProvisioningFlags);

		public static readonly ADPropertyDefinition IsHalted = ADObject.BitfieldProperty("IsHalted", 1, SharedPropertyDefinitions.ProvisioningFlags);

		public static readonly ADPropertyDefinition IsHaltRecoveryDisabled = ADObject.BitfieldProperty("IsHaltRecoveryDisabled", 2, SharedPropertyDefinitions.ProvisioningFlags);

		public static readonly ADPropertyDefinition IsMultiObjectCookieEnabled = ADObject.BitfieldProperty("IsMultiObjectCookieEnabled", 4, SharedPropertyDefinitions.ProvisioningFlags);

		public static readonly ADPropertyDefinition IsNewCookieBlocked = ADObject.BitfieldProperty("IsNewCookieBlocked", 5, SharedPropertyDefinitions.ProvisioningFlags);

		public static readonly ADPropertyDefinition NewTenantMinVersion = new ADPropertyDefinition("NewTenantMinVersion", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchSyncServiceInstanceNewTenantMinVersion", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition NewTenantMaxVersion = new ADPropertyDefinition("NewTenantMaxVersion", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchSyncServiceInstanceNewTenantMaxVersion", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TargetServerMaxVersion = new ADPropertyDefinition("TargetServerMaxVersion", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchCapabilityIdentifiers", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TargetServerMinVersion = new ADPropertyDefinition("TargetServerMinVersion", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchSetupStatus", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ForwardSyncConfigurationXML = new ADPropertyDefinition("ForwardSyncConfigurationXML", ExchangeObjectVersion.Exchange2003, typeof(string), "msExchConfigurationXML", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition IsUsedForTenantToServiceInstanceAssociation = ADObject.BitfieldProperty("IsUsedForTenantToServiceInstanceAssociation", 6, SharedPropertyDefinitions.ProvisioningFlags);

		public enum ArbitrationProvisioningBitShifts
		{
			IsEnabled,
			IsHalted,
			IsHaltRecoveryDisabled,
			IsCentralConfigEnabled,
			IsMultiObjectCookieEnabled,
			IsNewCookieBlocked,
			IsUsedForTenantToServiceInstanceAssociation
		}
	}
}
