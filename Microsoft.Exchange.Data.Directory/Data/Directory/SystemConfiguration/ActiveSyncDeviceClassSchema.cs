using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class ActiveSyncDeviceClassSchema : ADConfigurationObjectSchema
	{
		public static readonly ADPropertyDefinition LastUpdateTime = new ADPropertyDefinition("LastUpdateTime", ExchangeObjectVersion.Exchange2010, typeof(DateTime?), "msExchLastUpdateTime", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DeviceType = MobileDeviceSchema.DeviceType;

		public static readonly ADPropertyDefinition DeviceModel = MobileDeviceSchema.DeviceModel;
	}
}
