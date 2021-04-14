using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class ActiveSyncDeviceAccessRuleSchema : ADConfigurationObjectSchema
	{
		public static readonly ADPropertyDefinition QueryString = new ADPropertyDefinition("QueryString", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchDeviceAccessRuleQueryString", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Characteristic = new ADPropertyDefinition("Characteristic", ExchangeObjectVersion.Exchange2010, typeof(DeviceAccessCharacteristic), "msExchDeviceAccessRuleCharacteristic", ADPropertyDefinitionFlags.PersistDefaultValue, DeviceAccessCharacteristic.DeviceType, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AccessLevel = new ADPropertyDefinition("AccessLevel", ExchangeObjectVersion.Exchange2010, typeof(DeviceAccessLevel), "msExchMobileAccessControl", ADPropertyDefinitionFlags.PersistDefaultValue, DeviceAccessLevel.Allow, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition BackLink = new ADPropertyDefinition("BackLink", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), "msExchDeviceAccessRuleBL", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.FilterOnly | ADPropertyDefinitionFlags.BackLink, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
