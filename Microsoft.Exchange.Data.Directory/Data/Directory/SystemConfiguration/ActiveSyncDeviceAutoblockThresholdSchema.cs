using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class ActiveSyncDeviceAutoblockThresholdSchema : ADConfigurationObjectSchema
	{
		public static readonly ADPropertyDefinition BehaviorType = new ADPropertyDefinition("BehaviorType", ExchangeObjectVersion.Exchange2010, typeof(AutoblockThresholdType), "msExchActiveSyncDeviceAutoblockThresholdType", ADPropertyDefinitionFlags.None, AutoblockThresholdType.UserAgentsChanges, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition BehaviorTypeIncidenceLimit = new ADPropertyDefinition("BehaviorTypeIncidenceLimit", ExchangeObjectVersion.Exchange2010, typeof(int), "msExchActiveSyncDeviceAutoblockThresholdIncidenceLimit", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new UIImpactStringLengthConstraint(0, 65535)
		}, null, null);

		public static readonly ADPropertyDefinition BehaviorTypeIncidenceDuration = new ADPropertyDefinition("BehaviorTypeIncidenceDuration", ExchangeObjectVersion.Exchange2010, typeof(EnhancedTimeSpan), "msExchActiveSyncDeviceAutoblockThresholdIncidenceDuration", ADPropertyDefinitionFlags.PersistDefaultValue, EnhancedTimeSpan.Zero, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.Zero, EnhancedTimeSpan.FromSeconds(2147483647.0)),
			new EnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneMinute)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DeviceBlockDuration = new ADPropertyDefinition("DeviceBlockDuration", ExchangeObjectVersion.Exchange2010, typeof(EnhancedTimeSpan), "msExchActiveSyncDeviceAutoBlockDuration", ADPropertyDefinitionFlags.PersistDefaultValue, EnhancedTimeSpan.Zero, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.Zero, EnhancedTimeSpan.FromSeconds(2147483647.0)),
			new EnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneMinute)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AdminEmailInsert = new ADPropertyDefinition("AdminEmailInsert", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchMobileOTANotificationMailInsert2", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
