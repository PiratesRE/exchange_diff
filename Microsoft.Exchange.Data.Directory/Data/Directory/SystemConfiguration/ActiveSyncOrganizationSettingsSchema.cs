using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class ActiveSyncOrganizationSettingsSchema : ADContainerSchema
	{
		public static readonly ADPropertyDefinition ConfigurationXMLRaw = XMLSerializableBase.ConfigurationXmlRawProperty();

		public static readonly ADPropertyDefinition ConfigurationXML = XMLSerializableBase.ConfigurationXmlProperty<ActiveSyncOrganizationConfigXml>(ActiveSyncOrganizationSettingsSchema.ConfigurationXMLRaw);

		public static readonly ADPropertyDefinition AccessLevel = new ADPropertyDefinition("AccessLevel", ExchangeObjectVersion.Exchange2010, typeof(DeviceAccessLevel), "msExchMobileAccessControl", ADPropertyDefinitionFlags.PersistDefaultValue, DeviceAccessLevel.Allow, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition UserMailInsert = new ADPropertyDefinition("UserMailInsert", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchMobileUserMailInsert", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ProvisioningFlags = new ADPropertyDefinition("ProvisioningFlags", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchProvisioningFlags", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AllowAccessForUnSupportedPlatform = new ADPropertyDefinition("AllowAccessForUnSupportedPlatform", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ActiveSyncOrganizationSettingsSchema.ProvisioningFlags
		}, null, ADObject.FlagGetterDelegate(1, ActiveSyncOrganizationSettingsSchema.ProvisioningFlags), ADObject.FlagSetterDelegate(1, ActiveSyncOrganizationSettingsSchema.ProvisioningFlags), null, null);

		internal static readonly ADPropertyDefinition AdminMailRecipients = new ADPropertyDefinition("AdminMailRecipients", ExchangeObjectVersion.Exchange2010, typeof(SmtpAddress), "msExchMobileAdminRecipients", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition OtaNotificationMailInsert = new ADPropertyDefinition("OTANotificationMailInsert", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchMobileOTANotificationMailInsert2", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DeviceFiltering = XMLSerializableBase.ConfigXmlProperty<ActiveSyncOrganizationConfigXml, ActiveSyncDeviceFilterArray>("DeviceFilters", ExchangeObjectVersion.Exchange2010, ActiveSyncOrganizationSettingsSchema.ConfigurationXML, null, (ActiveSyncOrganizationConfigXml configXml) => configXml.DeviceFiltering, delegate(ActiveSyncOrganizationConfigXml configXml, ActiveSyncDeviceFilterArray value)
		{
			configXml.DeviceFiltering = value;
		}, null, null);
	}
}
