using System;
using System.Globalization;
using Microsoft.Exchange.Data.Storage.VersionedXml;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class TextMessagingAccountSchema : VersionedXmlConfigurationObjectSchema
	{
		public static readonly SimpleProviderPropertyDefinition RawTextMessagingSettings = new SimpleProviderPropertyDefinition("RawTextMessagingSettings", ExchangeObjectVersion.Exchange2010, typeof(TextMessagingSettingsVersion1Point0), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition TextMessagingSettings = new SimpleProviderPropertyDefinition("TextMessagingSettings", ExchangeObjectVersion.Exchange2010, typeof(TextMessagingSettingsVersion1Point0), PropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			TextMessagingAccountSchema.RawTextMessagingSettings
		}, null, new GetterDelegate(TextMessagingAccount.TextMessagingSettingsGetter), new SetterDelegate(TextMessagingAccount.TextMessagingSettingsSetter));

		public static readonly SimpleProviderPropertyDefinition CountryRegionId = new SimpleProviderPropertyDefinition("CountryRegionId", ExchangeObjectVersion.Exchange2010, typeof(RegionInfo), PropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			TextMessagingAccountSchema.RawTextMessagingSettings
		}, null, new GetterDelegate(TextMessagingAccount.CountryRegionIdGetter), new SetterDelegate(TextMessagingAccount.CountryRegionIdSetter));

		public static readonly SimpleProviderPropertyDefinition MobileOperatorId = new SimpleProviderPropertyDefinition("MobileOperatorId", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.Calculated, -1, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			TextMessagingAccountSchema.RawTextMessagingSettings
		}, null, new GetterDelegate(TextMessagingAccount.MobileOperatorIdGetter), new SetterDelegate(TextMessagingAccount.MobileOperatorIdSetter));

		public static readonly SimpleProviderPropertyDefinition NotificationPhoneNumber = new SimpleProviderPropertyDefinition("NotificationPhoneNumber", ExchangeObjectVersion.Exchange2010, typeof(E164Number), PropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			TextMessagingAccountSchema.RawTextMessagingSettings
		}, null, new GetterDelegate(TextMessagingAccount.NotificationPhoneNumberGetter), new SetterDelegate(TextMessagingAccount.NotificationPhoneNumberSetter));

		public static readonly SimpleProviderPropertyDefinition NotificationPhoneNumberVerified = new SimpleProviderPropertyDefinition("NotificationPhoneNumberVerified", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.ReadOnly | PropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			TextMessagingAccountSchema.RawTextMessagingSettings
		}, null, new GetterDelegate(TextMessagingAccount.NotificationPhoneNumberVerifiedGetter), null);

		public static readonly SimpleProviderPropertyDefinition EasEnabled = new SimpleProviderPropertyDefinition("EasEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.ReadOnly | PropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			TextMessagingAccountSchema.RawTextMessagingSettings
		}, null, new GetterDelegate(TextMessagingAccount.EasEnabledGetter), null);

		public static readonly SimpleProviderPropertyDefinition EasPhoneNumber = new SimpleProviderPropertyDefinition("EasPhoneNumber", ExchangeObjectVersion.Exchange2010, typeof(E164Number), PropertyDefinitionFlags.ReadOnly | PropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			TextMessagingAccountSchema.RawTextMessagingSettings
		}, null, new GetterDelegate(TextMessagingAccount.EasPhoneNumberGetter), null);

		public static readonly SimpleProviderPropertyDefinition EasDeviceProtocol = new SimpleProviderPropertyDefinition("EasDeviceProtocol", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.ReadOnly | PropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			TextMessagingAccountSchema.RawTextMessagingSettings
		}, null, new GetterDelegate(TextMessagingAccount.EasDeviceProtocolGetter), null);

		public static readonly SimpleProviderPropertyDefinition EasDeviceType = new SimpleProviderPropertyDefinition("EasDeviceType", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.ReadOnly | PropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			TextMessagingAccountSchema.RawTextMessagingSettings
		}, null, new GetterDelegate(TextMessagingAccount.EasDeviceTypeGetter), null);

		public static readonly SimpleProviderPropertyDefinition EasDeviceId = new SimpleProviderPropertyDefinition("EasDeviceId", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.ReadOnly | PropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			TextMessagingAccountSchema.RawTextMessagingSettings
		}, null, new GetterDelegate(TextMessagingAccount.EasDeviceIdGetter), null);

		public static readonly SimpleProviderPropertyDefinition EasDeviceName = new SimpleProviderPropertyDefinition("EasDeviceName", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.ReadOnly | PropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			TextMessagingAccountSchema.RawTextMessagingSettings
		}, null, new GetterDelegate(TextMessagingAccount.EasDeviceFriendlyNameGetter), null);
	}
}
