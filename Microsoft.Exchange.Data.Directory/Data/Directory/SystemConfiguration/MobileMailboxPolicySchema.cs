using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class MobileMailboxPolicySchema : MailboxPolicySchema
	{
		private static GetterDelegate GetMobileFlagsGetterDelegate(MobileFlagsDefs flag)
		{
			return (IPropertyBag propertyBag) => ((MobileFlagsDefs)propertyBag[MobileMailboxPolicySchema.MobileFlags] & flag) == flag;
		}

		private static SetterDelegate GetMobileFlagsSetterDelegate(MobileFlagsDefs flag)
		{
			return delegate(object value, IPropertyBag propertyBag)
			{
				if ((bool)value)
				{
					propertyBag[MobileMailboxPolicySchema.MobileFlags] = ((MobileFlagsDefs)propertyBag[MobileMailboxPolicySchema.MobileFlags] | flag);
					return;
				}
				propertyBag[MobileMailboxPolicySchema.MobileFlags] = ((MobileFlagsDefs)propertyBag[MobileMailboxPolicySchema.MobileFlags] & ~flag);
			};
		}

		private static GetterDelegate GetMobileAdditionalFlagsGetterDelegate(MobileAdditionalFlagsDefs flag)
		{
			return (IPropertyBag propertyBag) => ((MobileAdditionalFlagsDefs)propertyBag[MobileMailboxPolicySchema.MobileAdditionalFlags] & flag) == flag;
		}

		private static SetterDelegate GetMobileAdditionalFlagsSetterDelegate(MobileAdditionalFlagsDefs flag)
		{
			return delegate(object value, IPropertyBag propertyBag)
			{
				if ((bool)value)
				{
					propertyBag[MobileMailboxPolicySchema.MobileAdditionalFlags] = ((MobileAdditionalFlagsDefs)propertyBag[MobileMailboxPolicySchema.MobileAdditionalFlags] | flag);
					return;
				}
				propertyBag[MobileMailboxPolicySchema.MobileAdditionalFlags] = ((MobileAdditionalFlagsDefs)propertyBag[MobileMailboxPolicySchema.MobileAdditionalFlags] & ~flag);
			};
		}

		public static readonly ADPropertyDefinition AssociatedUsers = new ADPropertyDefinition("AssociatedUsers", ExchangeObjectVersion.Exchange2007, typeof(ADObjectId), "msExchMobilePolicyBL", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.FilterOnly | ADPropertyDefinitionFlags.BackLink, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DevicePolicyRefreshInterval = new ADPropertyDefinition("DevicePolicyRefreshInterval", ExchangeObjectVersion.Exchange2007, typeof(Unlimited<EnhancedTimeSpan>), "msExchMobileDevicePolicyRefreshInterval", ADPropertyDefinitionFlags.None, Unlimited<EnhancedTimeSpan>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.Zero, EnhancedTimeSpan.FromSeconds(2147483647.0)),
			new UnlimitedEnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneSecond)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MaxAttachmentSize = new ADPropertyDefinition("MaxAttachmentSize", ExchangeObjectVersion.Exchange2007, typeof(Unlimited<ByteQuantifiedSize>), "msExchMobileInitialMaxAttachmentSize", ADPropertyDefinitionFlags.None, Unlimited<ByteQuantifiedSize>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<ByteQuantifiedSize>(ByteQuantifiedSize.FromKB(0UL), ByteQuantifiedSize.FromKB(2097151UL))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MaxPasswordFailedAttempts = new ADPropertyDefinition("MaxPasswordFailedAttempts", ExchangeObjectVersion.Exchange2007, typeof(Unlimited<int>), "msExchMobileMaxDevicePasswordFailedAttempts", ADPropertyDefinitionFlags.None, Unlimited<int>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<int>(4, 16)
		}, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<int>(4, 16)
		}, null, null);

		public static readonly ADPropertyDefinition MaxInactivityTimeLock = new ADPropertyDefinition("MaxInactivityTimeLock", ExchangeObjectVersion.Exchange2007, typeof(Unlimited<EnhancedTimeSpan>), "msExchMobileMaxInactivityTimeDeviceLock", ADPropertyDefinitionFlags.None, Unlimited<EnhancedTimeSpan>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.OneMinute, EnhancedTimeSpan.OneHour),
			new UnlimitedEnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneSecond)
		}, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.OneMinute, EnhancedTimeSpan.OneHour),
			new UnlimitedEnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneSecond)
		}, null, null);

		public static readonly ADPropertyDefinition PasswordExpiration = new ADPropertyDefinition("PasswordExpiration", ExchangeObjectVersion.Exchange2007, typeof(Unlimited<EnhancedTimeSpan>), "msExchMobileDevicePasswordExpiration", ADPropertyDefinitionFlags.None, Unlimited<EnhancedTimeSpan>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.OneDay, EnhancedTimeSpan.FromDays(730.0)),
			new UnlimitedEnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneSecond)
		}, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.OneDay, EnhancedTimeSpan.FromDays(730.0)),
			new UnlimitedEnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneSecond)
		}, null, null);

		public static readonly ADPropertyDefinition PasswordHistory = new ADPropertyDefinition("PasswordHistory", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchMobileDeviceNumberOfPreviousPasswordsDisallowed", ADPropertyDefinitionFlags.PersistDefaultValue, 0, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, 50)
		}, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, 50)
		}, null, null);

		public static readonly ADPropertyDefinition MinPasswordLength = new ADPropertyDefinition("MinPasswordLength", ExchangeObjectVersion.Exchange2007, typeof(int?), "msExchMobileMinDevicePasswordLength", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new RangedNullableValueConstraint<int>(1, 16)
		}, new PropertyDefinitionConstraint[]
		{
			new RangedNullableValueConstraint<int>(1, 16)
		}, null, null);

		public static readonly ADPropertyDefinition MobileFlags = new ADPropertyDefinition("MobileFlags", ExchangeObjectVersion.Exchange2007, typeof(MobileFlagsDefs), "msExchMobileFlags", ADPropertyDefinitionFlags.None, MobileFlagsDefs.AttachmentsEnabled | MobileFlagsDefs.AllowSimplePassword | MobileFlagsDefs.WSSAccessEnabled | MobileFlagsDefs.UNCAccessEnabled, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MobileAdditionalFlags = new ADPropertyDefinition("MobileAdditionalFlags", ExchangeObjectVersion.Exchange2007, typeof(MobileAdditionalFlagsDefs), "msExchMobileAdditionalFlags", ADPropertyDefinitionFlags.None, MobileAdditionalFlagsDefs.AllowStorageCard | MobileAdditionalFlagsDefs.AllowCamera | MobileAdditionalFlagsDefs.AllowUnsignedApplications | MobileAdditionalFlagsDefs.AllowUnsignedInstallationPackages | MobileAdditionalFlagsDefs.AllowWiFi | MobileAdditionalFlagsDefs.AllowTextMessaging | MobileAdditionalFlagsDefs.AllowPOPIMAPEmail | MobileAdditionalFlagsDefs.AllowIrDA | MobileAdditionalFlagsDefs.AllowDesktopSync | MobileAdditionalFlagsDefs.AllowHTMLEmail | MobileAdditionalFlagsDefs.AllowSMIMESoftCerts | MobileAdditionalFlagsDefs.AllowBrowser | MobileAdditionalFlagsDefs.AllowConsumerEmail | MobileAdditionalFlagsDefs.AllowRemoteDesktop | MobileAdditionalFlagsDefs.AllowInternetSharing | MobileAdditionalFlagsDefs.AllowMobileOTAUpdate | MobileAdditionalFlagsDefs.IrmEnabled, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AllowBluetooth = new ADPropertyDefinition("AllowBluetooth", ExchangeObjectVersion.Exchange2007, typeof(BluetoothType), "msExchMobileAllowBluetooth", ADPropertyDefinitionFlags.None, BluetoothType.Allow, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MaxCalendarAgeFilter = new ADPropertyDefinition("MaxCalendarAgeFilter", ExchangeObjectVersion.Exchange2007, typeof(CalendarAgeFilterType), "msExchMobileMaxCalendarAgeFilter", ADPropertyDefinitionFlags.None, CalendarAgeFilterType.All, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MaxEmailAgeFilter = new ADPropertyDefinition("MaxEmailAgeFilter", ExchangeObjectVersion.Exchange2007, typeof(EmailAgeFilterType), "msExchMobileMaxEmailAgeFilter", ADPropertyDefinitionFlags.None, EmailAgeFilterType.All, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RequireSignedSMIMEAlgorithm = new ADPropertyDefinition("RequireSignedSMIMEAlgorithm", ExchangeObjectVersion.Exchange2007, typeof(SignedSMIMEAlgorithmType), "msExchMobileRequireSignedSMIMEAlgorithm", ADPropertyDefinitionFlags.None, SignedSMIMEAlgorithmType.SHA1, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RequireEncryptionSMIMEAlgorithm = new ADPropertyDefinition("RequireEncryptionSMIMEAlgorithm", ExchangeObjectVersion.Exchange2007, typeof(EncryptionSMIMEAlgorithmType), "msExchMobileRequireEncryptionSMIMEAlgorithm", ADPropertyDefinitionFlags.None, EncryptionSMIMEAlgorithmType.TripleDES, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AllowSMIMEEncryptionAlgorithmNegotiation = new ADPropertyDefinition("AllowSMIMEEncryptionAlgorithmNegotiation", ExchangeObjectVersion.Exchange2007, typeof(SMIMEEncryptionAlgorithmNegotiationType), "msExchMobileAllowSMIMEEncryptionAlgorithmNegotiation", ADPropertyDefinitionFlags.None, SMIMEEncryptionAlgorithmNegotiationType.AllowAnyAlgorithmNegotiation, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MinPasswordComplexCharacters = new ADPropertyDefinition("MinPasswordComplexCharacters", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchMobileMinDevicePasswordComplexCharacters", ADPropertyDefinitionFlags.PersistDefaultValue, 1, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(1, 4)
		}, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(1, 4)
		}, null, null);

		public static readonly ADPropertyDefinition ADMaxEmailBodyTruncationSize = new ADPropertyDefinition("ADMaxEmailBodyTruncationSize", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchMobileMaxEmailBodyTruncationSize", ADPropertyDefinitionFlags.PersistDefaultValue, -1, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(-1, int.MaxValue)
		}, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(-1, int.MaxValue)
		}, null, null);

		public static readonly ADPropertyDefinition ADMaxEmailHTMLBodyTruncationSize = new ADPropertyDefinition("ADMaxEmailHTMLBodyTruncationSize", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchMobileMaxEmailHTMLBodyTruncationSize", ADPropertyDefinitionFlags.PersistDefaultValue, -1, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(-1, int.MaxValue)
		}, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(-1, int.MaxValue)
		}, null, null);

		public static readonly ADPropertyDefinition UnapprovedInROMApplicationList = new ADPropertyDefinition("UnapprovedInROMApplicationList", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchMobileUnapprovedInROMApplicationList", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new NoLeadingOrTrailingWhitespaceConstraint()
		}, null, null);

		public static readonly ADPropertyDefinition ADApprovedApplicationList = new ADPropertyDefinition("ADApprovedApplicationList", ExchangeObjectVersion.Exchange2007, typeof(ApprovedApplication), "msExchMobileApprovedApplicationList", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.DoNotProvisionalClone, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MobileOTAUpdateMode = new ADPropertyDefinition("MobileOTAUpdateMode", ExchangeObjectVersion.Exchange2007, typeof(MobileOTAUpdateModeType), "msExchMobileOTAUpdateMode", ADPropertyDefinitionFlags.None, MobileOTAUpdateModeType.MinorVersionUpdates, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AllowNonProvisionableDevices = new ADPropertyDefinition("AllowNonProvisionableDevices", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MobileMailboxPolicySchema.MobileFlags
		}, null, MobileMailboxPolicySchema.GetMobileFlagsGetterDelegate(MobileFlagsDefs.AllowNonProvisionableDevices), MobileMailboxPolicySchema.GetMobileFlagsSetterDelegate(MobileFlagsDefs.AllowNonProvisionableDevices), null, null);

		public static readonly ADPropertyDefinition AlphanumericPasswordRequired = new ADPropertyDefinition("AlphanumericPasswordRequired", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MobileMailboxPolicySchema.MobileFlags
		}, null, MobileMailboxPolicySchema.GetMobileFlagsGetterDelegate(MobileFlagsDefs.AlphanumericPasswordRequired), MobileMailboxPolicySchema.GetMobileFlagsSetterDelegate(MobileFlagsDefs.AlphanumericPasswordRequired), null, null);

		public static readonly ADPropertyDefinition AttachmentsEnabled = new ADPropertyDefinition("AttachmentsEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MobileMailboxPolicySchema.MobileFlags
		}, null, MobileMailboxPolicySchema.GetMobileFlagsGetterDelegate(MobileFlagsDefs.AttachmentsEnabled), MobileMailboxPolicySchema.GetMobileFlagsSetterDelegate(MobileFlagsDefs.AttachmentsEnabled), null, null);

		public static readonly ADPropertyDefinition RequireStorageCardEncryption = new ADPropertyDefinition("RequireStorageCardEncryption", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MobileMailboxPolicySchema.MobileFlags
		}, null, MobileMailboxPolicySchema.GetMobileFlagsGetterDelegate(MobileFlagsDefs.RequireStorageCardEncryption), MobileMailboxPolicySchema.GetMobileFlagsSetterDelegate(MobileFlagsDefs.RequireStorageCardEncryption), null, null);

		public static readonly ADPropertyDefinition PasswordEnabled = new ADPropertyDefinition("PasswordEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MobileMailboxPolicySchema.MobileFlags
		}, null, MobileMailboxPolicySchema.GetMobileFlagsGetterDelegate(MobileFlagsDefs.PasswordEnabled), MobileMailboxPolicySchema.GetMobileFlagsSetterDelegate(MobileFlagsDefs.PasswordEnabled), null, null);

		public static readonly ADPropertyDefinition PasswordRecoveryEnabled = new ADPropertyDefinition("PasswordRecoveryEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MobileMailboxPolicySchema.MobileFlags
		}, null, MobileMailboxPolicySchema.GetMobileFlagsGetterDelegate(MobileFlagsDefs.PasswordRecoveryEnabled), MobileMailboxPolicySchema.GetMobileFlagsSetterDelegate(MobileFlagsDefs.PasswordRecoveryEnabled), null, null);

		public static readonly ADPropertyDefinition AllowSimplePassword = new ADPropertyDefinition("AllowSimplePassword", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MobileMailboxPolicySchema.MobileFlags
		}, null, MobileMailboxPolicySchema.GetMobileFlagsGetterDelegate(MobileFlagsDefs.AllowSimplePassword), MobileMailboxPolicySchema.GetMobileFlagsSetterDelegate(MobileFlagsDefs.AllowSimplePassword), null, null);

		public static readonly ADPropertyDefinition WSSAccessEnabled = new ADPropertyDefinition("WSSAccessEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MobileMailboxPolicySchema.MobileFlags
		}, null, MobileMailboxPolicySchema.GetMobileFlagsGetterDelegate(MobileFlagsDefs.WSSAccessEnabled), MobileMailboxPolicySchema.GetMobileFlagsSetterDelegate(MobileFlagsDefs.WSSAccessEnabled), null, null);

		public static readonly ADPropertyDefinition UNCAccessEnabled = new ADPropertyDefinition("UNCAccessEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MobileMailboxPolicySchema.MobileFlags
		}, null, MobileMailboxPolicySchema.GetMobileFlagsGetterDelegate(MobileFlagsDefs.UNCAccessEnabled), MobileMailboxPolicySchema.GetMobileFlagsSetterDelegate(MobileFlagsDefs.UNCAccessEnabled), null, null);

		public static readonly ADPropertyDefinition IsDefault = new ADPropertyDefinition("IsDefault", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MobileMailboxPolicySchema.MobileFlags
		}, null, MobileMailboxPolicySchema.GetMobileFlagsGetterDelegate(MobileFlagsDefs.IsDefault), MobileMailboxPolicySchema.GetMobileFlagsSetterDelegate(MobileFlagsDefs.IsDefault), null, null);

		public static readonly ADPropertyDefinition DenyApplePushNotifications = new ADPropertyDefinition("DenyApplePushNotifications", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MobileMailboxPolicySchema.MobileFlags
		}, null, MobileMailboxPolicySchema.GetMobileFlagsGetterDelegate(MobileFlagsDefs.DenyApplePushNotifications), MobileMailboxPolicySchema.GetMobileFlagsSetterDelegate(MobileFlagsDefs.DenyApplePushNotifications), null, null);

		public static readonly ADPropertyDefinition DenyMicrosoftPushNotifications = new ADPropertyDefinition("DenyMicrosoftPushNotifications", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MobileMailboxPolicySchema.MobileFlags
		}, null, MobileMailboxPolicySchema.GetMobileFlagsGetterDelegate(MobileFlagsDefs.DenyMicrosoftPushNotifications), MobileMailboxPolicySchema.GetMobileFlagsSetterDelegate(MobileFlagsDefs.DenyMicrosoftPushNotifications), null, null);

		public static readonly ADPropertyDefinition DenyGooglePushNotifications = new ADPropertyDefinition("DenyGooglePushNotifications", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MobileMailboxPolicySchema.MobileFlags
		}, null, MobileMailboxPolicySchema.GetMobileFlagsGetterDelegate(MobileFlagsDefs.DenyGooglePushNotifications), MobileMailboxPolicySchema.GetMobileFlagsSetterDelegate(MobileFlagsDefs.DenyGooglePushNotifications), null, null);

		public static readonly ADPropertyDefinition AllowStorageCard = new ADPropertyDefinition("AllowStorageCard", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MobileMailboxPolicySchema.MobileAdditionalFlags
		}, null, MobileMailboxPolicySchema.GetMobileAdditionalFlagsGetterDelegate(MobileAdditionalFlagsDefs.AllowStorageCard), MobileMailboxPolicySchema.GetMobileAdditionalFlagsSetterDelegate(MobileAdditionalFlagsDefs.AllowStorageCard), null, null);

		public static readonly ADPropertyDefinition AllowCamera = new ADPropertyDefinition("AllowCamera", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MobileMailboxPolicySchema.MobileAdditionalFlags
		}, null, MobileMailboxPolicySchema.GetMobileAdditionalFlagsGetterDelegate(MobileAdditionalFlagsDefs.AllowCamera), MobileMailboxPolicySchema.GetMobileAdditionalFlagsSetterDelegate(MobileAdditionalFlagsDefs.AllowCamera), null, null);

		public static readonly ADPropertyDefinition RequireDeviceEncryption = new ADPropertyDefinition("RequireDeviceEncryption", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MobileMailboxPolicySchema.MobileAdditionalFlags
		}, null, MobileMailboxPolicySchema.GetMobileAdditionalFlagsGetterDelegate(MobileAdditionalFlagsDefs.RequireDeviceEncryption), MobileMailboxPolicySchema.GetMobileAdditionalFlagsSetterDelegate(MobileAdditionalFlagsDefs.RequireDeviceEncryption), null, null);

		public static readonly ADPropertyDefinition AllowUnsignedApplications = new ADPropertyDefinition("AllowUnsignedApplications", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MobileMailboxPolicySchema.MobileAdditionalFlags
		}, null, MobileMailboxPolicySchema.GetMobileAdditionalFlagsGetterDelegate(MobileAdditionalFlagsDefs.AllowUnsignedApplications), MobileMailboxPolicySchema.GetMobileAdditionalFlagsSetterDelegate(MobileAdditionalFlagsDefs.AllowUnsignedApplications), null, null);

		public static readonly ADPropertyDefinition AllowUnsignedInstallationPackages = new ADPropertyDefinition("AllowUnsignedInstallationPackages", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MobileMailboxPolicySchema.MobileAdditionalFlags
		}, null, MobileMailboxPolicySchema.GetMobileAdditionalFlagsGetterDelegate(MobileAdditionalFlagsDefs.AllowUnsignedInstallationPackages), MobileMailboxPolicySchema.GetMobileAdditionalFlagsSetterDelegate(MobileAdditionalFlagsDefs.AllowUnsignedInstallationPackages), null, null);

		public static readonly ADPropertyDefinition AllowWiFi = new ADPropertyDefinition("AllowWiFi", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MobileMailboxPolicySchema.MobileAdditionalFlags
		}, null, MobileMailboxPolicySchema.GetMobileAdditionalFlagsGetterDelegate(MobileAdditionalFlagsDefs.AllowWiFi), MobileMailboxPolicySchema.GetMobileAdditionalFlagsSetterDelegate(MobileAdditionalFlagsDefs.AllowWiFi), null, null);

		public static readonly ADPropertyDefinition AllowTextMessaging = new ADPropertyDefinition("AllowTextMessaging", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MobileMailboxPolicySchema.MobileAdditionalFlags
		}, null, MobileMailboxPolicySchema.GetMobileAdditionalFlagsGetterDelegate(MobileAdditionalFlagsDefs.AllowTextMessaging), MobileMailboxPolicySchema.GetMobileAdditionalFlagsSetterDelegate(MobileAdditionalFlagsDefs.AllowTextMessaging), null, null);

		public static readonly ADPropertyDefinition AllowPOPIMAPEmail = new ADPropertyDefinition("AllowPOPIMAPEmail", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MobileMailboxPolicySchema.MobileAdditionalFlags
		}, null, MobileMailboxPolicySchema.GetMobileAdditionalFlagsGetterDelegate(MobileAdditionalFlagsDefs.AllowPOPIMAPEmail), MobileMailboxPolicySchema.GetMobileAdditionalFlagsSetterDelegate(MobileAdditionalFlagsDefs.AllowPOPIMAPEmail), null, null);

		public static readonly ADPropertyDefinition AllowIrDA = new ADPropertyDefinition("AllowIrDA", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MobileMailboxPolicySchema.MobileAdditionalFlags
		}, null, MobileMailboxPolicySchema.GetMobileAdditionalFlagsGetterDelegate(MobileAdditionalFlagsDefs.AllowIrDA), MobileMailboxPolicySchema.GetMobileAdditionalFlagsSetterDelegate(MobileAdditionalFlagsDefs.AllowIrDA), null, null);

		public static readonly ADPropertyDefinition RequireManualSyncWhenRoaming = new ADPropertyDefinition("RequireManualSyncWhenRoaming", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MobileMailboxPolicySchema.MobileAdditionalFlags
		}, null, MobileMailboxPolicySchema.GetMobileAdditionalFlagsGetterDelegate(MobileAdditionalFlagsDefs.RequireManualSyncWhenRoaming), MobileMailboxPolicySchema.GetMobileAdditionalFlagsSetterDelegate(MobileAdditionalFlagsDefs.RequireManualSyncWhenRoaming), null, null);

		public static readonly ADPropertyDefinition AllowDesktopSync = new ADPropertyDefinition("AllowDesktopSync", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MobileMailboxPolicySchema.MobileAdditionalFlags
		}, null, MobileMailboxPolicySchema.GetMobileAdditionalFlagsGetterDelegate(MobileAdditionalFlagsDefs.AllowDesktopSync), MobileMailboxPolicySchema.GetMobileAdditionalFlagsSetterDelegate(MobileAdditionalFlagsDefs.AllowDesktopSync), null, null);

		public static readonly ADPropertyDefinition AllowHTMLEmail = new ADPropertyDefinition("AllowHTMLEmail", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MobileMailboxPolicySchema.MobileAdditionalFlags
		}, null, MobileMailboxPolicySchema.GetMobileAdditionalFlagsGetterDelegate(MobileAdditionalFlagsDefs.AllowHTMLEmail), MobileMailboxPolicySchema.GetMobileAdditionalFlagsSetterDelegate(MobileAdditionalFlagsDefs.AllowHTMLEmail), null, null);

		public static readonly ADPropertyDefinition RequireSignedSMIMEMessages = new ADPropertyDefinition("RequireSignedSMIMEMessages", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MobileMailboxPolicySchema.MobileAdditionalFlags
		}, null, MobileMailboxPolicySchema.GetMobileAdditionalFlagsGetterDelegate(MobileAdditionalFlagsDefs.RequireSignedSMIMEMessages), MobileMailboxPolicySchema.GetMobileAdditionalFlagsSetterDelegate(MobileAdditionalFlagsDefs.RequireSignedSMIMEMessages), null, null);

		public static readonly ADPropertyDefinition RequireEncryptedSMIMEMessages = new ADPropertyDefinition("RequireEncryptedSMIMEMessages", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MobileMailboxPolicySchema.MobileAdditionalFlags
		}, null, MobileMailboxPolicySchema.GetMobileAdditionalFlagsGetterDelegate(MobileAdditionalFlagsDefs.RequireEncryptedSMIMEMessages), MobileMailboxPolicySchema.GetMobileAdditionalFlagsSetterDelegate(MobileAdditionalFlagsDefs.RequireEncryptedSMIMEMessages), null, null);

		public static readonly ADPropertyDefinition AllowSMIMESoftCerts = new ADPropertyDefinition("AllowSMIMESoftCerts", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MobileMailboxPolicySchema.MobileAdditionalFlags
		}, null, MobileMailboxPolicySchema.GetMobileAdditionalFlagsGetterDelegate(MobileAdditionalFlagsDefs.AllowSMIMESoftCerts), MobileMailboxPolicySchema.GetMobileAdditionalFlagsSetterDelegate(MobileAdditionalFlagsDefs.AllowSMIMESoftCerts), null, null);

		public static readonly ADPropertyDefinition AllowBrowser = new ADPropertyDefinition("AllowBrowser", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MobileMailboxPolicySchema.MobileAdditionalFlags
		}, null, MobileMailboxPolicySchema.GetMobileAdditionalFlagsGetterDelegate(MobileAdditionalFlagsDefs.AllowBrowser), MobileMailboxPolicySchema.GetMobileAdditionalFlagsSetterDelegate(MobileAdditionalFlagsDefs.AllowBrowser), null, null);

		public static readonly ADPropertyDefinition AllowConsumerEmail = new ADPropertyDefinition("AllowConsumerEmail", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MobileMailboxPolicySchema.MobileAdditionalFlags
		}, null, MobileMailboxPolicySchema.GetMobileAdditionalFlagsGetterDelegate(MobileAdditionalFlagsDefs.AllowConsumerEmail), MobileMailboxPolicySchema.GetMobileAdditionalFlagsSetterDelegate(MobileAdditionalFlagsDefs.AllowConsumerEmail), null, null);

		public static readonly ADPropertyDefinition AllowRemoteDesktop = new ADPropertyDefinition("AllowRemoteDesktop", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MobileMailboxPolicySchema.MobileAdditionalFlags
		}, null, MobileMailboxPolicySchema.GetMobileAdditionalFlagsGetterDelegate(MobileAdditionalFlagsDefs.AllowRemoteDesktop), MobileMailboxPolicySchema.GetMobileAdditionalFlagsSetterDelegate(MobileAdditionalFlagsDefs.AllowRemoteDesktop), null, null);

		public static readonly ADPropertyDefinition AllowInternetSharing = new ADPropertyDefinition("AllowInternetSharing", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MobileMailboxPolicySchema.MobileAdditionalFlags
		}, null, MobileMailboxPolicySchema.GetMobileAdditionalFlagsGetterDelegate(MobileAdditionalFlagsDefs.AllowInternetSharing), MobileMailboxPolicySchema.GetMobileAdditionalFlagsSetterDelegate(MobileAdditionalFlagsDefs.AllowInternetSharing), null, null);

		public static readonly ADPropertyDefinition MaxEmailBodyTruncationSize = new ADPropertyDefinition("MaxEmailBodyTruncationSize", ExchangeObjectVersion.Exchange2007, typeof(Unlimited<int>), null, ADPropertyDefinitionFlags.Calculated, Unlimited<int>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<int>(0, int.MaxValue)
		}, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<int>(0, int.MaxValue)
		}, new ProviderPropertyDefinition[]
		{
			MobileMailboxPolicySchema.ADMaxEmailBodyTruncationSize
		}, null, delegate(IPropertyBag propertyBag)
		{
			Unlimited<int> unlimitedValue;
			if ((int)propertyBag[MobileMailboxPolicySchema.ADMaxEmailBodyTruncationSize] == -1)
			{
				unlimitedValue = Unlimited<int>.UnlimitedValue;
			}
			else
			{
				unlimitedValue = new Unlimited<int>((int)propertyBag[MobileMailboxPolicySchema.ADMaxEmailBodyTruncationSize]);
			}
			return unlimitedValue;
		}, delegate(object value, IPropertyBag propertyBag)
		{
			if (((Unlimited<int>)value).IsUnlimited)
			{
				propertyBag[MobileMailboxPolicySchema.ADMaxEmailBodyTruncationSize] = -1;
				return;
			}
			propertyBag[MobileMailboxPolicySchema.ADMaxEmailBodyTruncationSize] = ((Unlimited<int>)value).Value;
		}, null, null);

		public static readonly ADPropertyDefinition MaxEmailHTMLBodyTruncationSize = new ADPropertyDefinition("MaxEmailHTMLBodyTruncationSize", ExchangeObjectVersion.Exchange2007, typeof(Unlimited<int>), null, ADPropertyDefinitionFlags.Calculated, Unlimited<int>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<int>(0, int.MaxValue)
		}, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<int>(0, int.MaxValue)
		}, new ProviderPropertyDefinition[]
		{
			MobileMailboxPolicySchema.ADMaxEmailHTMLBodyTruncationSize
		}, null, delegate(IPropertyBag propertyBag)
		{
			Unlimited<int> unlimitedValue;
			if ((int)propertyBag[MobileMailboxPolicySchema.ADMaxEmailHTMLBodyTruncationSize] == -1)
			{
				unlimitedValue = Unlimited<int>.UnlimitedValue;
			}
			else
			{
				unlimitedValue = new Unlimited<int>((int)propertyBag[MobileMailboxPolicySchema.ADMaxEmailHTMLBodyTruncationSize]);
			}
			return unlimitedValue;
		}, delegate(object value, IPropertyBag propertyBag)
		{
			if (((Unlimited<int>)value).IsUnlimited)
			{
				propertyBag[MobileMailboxPolicySchema.ADMaxEmailHTMLBodyTruncationSize] = -1;
				return;
			}
			propertyBag[MobileMailboxPolicySchema.ADMaxEmailHTMLBodyTruncationSize] = ((Unlimited<int>)value).Value;
		}, null, null);

		public static readonly ADPropertyDefinition AllowExternalDeviceManagement = new ADPropertyDefinition("ExternallyDeviceManaged", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MobileMailboxPolicySchema.MobileAdditionalFlags
		}, null, MobileMailboxPolicySchema.GetMobileAdditionalFlagsGetterDelegate(MobileAdditionalFlagsDefs.AllowExternalDeviceManagement), MobileMailboxPolicySchema.GetMobileAdditionalFlagsSetterDelegate(MobileAdditionalFlagsDefs.AllowExternalDeviceManagement), null, null);

		public static readonly ADPropertyDefinition AllowMobileOTAUpdate = new ADPropertyDefinition("AllowMobileOTAUpdate", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MobileMailboxPolicySchema.MobileAdditionalFlags
		}, null, MobileMailboxPolicySchema.GetMobileAdditionalFlagsGetterDelegate(MobileAdditionalFlagsDefs.AllowMobileOTAUpdate), MobileMailboxPolicySchema.GetMobileAdditionalFlagsSetterDelegate(MobileAdditionalFlagsDefs.AllowMobileOTAUpdate), null, null);

		public static readonly ADPropertyDefinition IrmEnabled = new ADPropertyDefinition("IrmEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MobileMailboxPolicySchema.MobileAdditionalFlags
		}, null, MobileMailboxPolicySchema.GetMobileAdditionalFlagsGetterDelegate(MobileAdditionalFlagsDefs.IrmEnabled), MobileMailboxPolicySchema.GetMobileAdditionalFlagsSetterDelegate(MobileAdditionalFlagsDefs.IrmEnabled), null, null);
	}
}
