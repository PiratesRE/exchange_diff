using System;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal class ADUserSchema : ADMailboxRecipientSchema
	{
		internal static SetterDelegate EnumFlagSetterDelegate(ProviderPropertyDefinition propertyDefinition, int mask)
		{
			return delegate(object value, IPropertyBag bag)
			{
				int num = (int)bag[propertyDefinition];
				int num2 = ((bool)value) ? (num | mask) : (num & ~mask);
				if (propertyDefinition.Type != null && propertyDefinition.Type.IsEnum)
				{
					bag[propertyDefinition] = Enum.ToObject(propertyDefinition.Type, num2);
					return;
				}
				bag[propertyDefinition] = num2;
			};
		}

		internal static object MobileSyncEnabledGetter(IPropertyBag propertyBag)
		{
			object obj = propertyBag[ADUserSchema.MobileFeaturesEnabled];
			if (obj == null)
			{
				return true;
			}
			MobileFeaturesEnabled mobileFeaturesEnabled = (MobileFeaturesEnabled)obj;
			return (mobileFeaturesEnabled & Microsoft.Exchange.Data.Directory.Recipient.MobileFeaturesEnabled.AirSyncDisabled) == Microsoft.Exchange.Data.Directory.Recipient.MobileFeaturesEnabled.None;
		}

		internal static void MobileSyncEnabledSetter(object value, IPropertyBag propertyBag)
		{
			bool flag = (bool)value;
			object obj = propertyBag[ADUserSchema.MobileFeaturesEnabled];
			if (obj == null)
			{
				obj = Microsoft.Exchange.Data.Directory.Recipient.MobileFeaturesEnabled.None;
			}
			if (flag)
			{
				propertyBag[ADUserSchema.MobileFeaturesEnabled] = ((MobileFeaturesEnabled)obj & ~Microsoft.Exchange.Data.Directory.Recipient.MobileFeaturesEnabled.AirSyncDisabled);
				return;
			}
			propertyBag[ADUserSchema.MobileFeaturesEnabled] = ((MobileFeaturesEnabled)obj | Microsoft.Exchange.Data.Directory.Recipient.MobileFeaturesEnabled.AirSyncDisabled);
		}

		internal static object OWAforDevicesEnabledGetter(IPropertyBag propertyBag)
		{
			object obj = propertyBag[ADUserSchema.MobileFeaturesEnabled];
			if (obj == null)
			{
				return true;
			}
			MobileFeaturesEnabled mobileFeaturesEnabled = (MobileFeaturesEnabled)obj;
			return (mobileFeaturesEnabled & Microsoft.Exchange.Data.Directory.Recipient.MobileFeaturesEnabled.MowaDisabled) == Microsoft.Exchange.Data.Directory.Recipient.MobileFeaturesEnabled.None;
		}

		internal static void OWAforDevicesEnabledSetter(object value, IPropertyBag propertyBag)
		{
			bool flag = (bool)value;
			object obj = propertyBag[ADUserSchema.MobileFeaturesEnabled];
			if (obj == null)
			{
				obj = Microsoft.Exchange.Data.Directory.Recipient.MobileFeaturesEnabled.None;
			}
			if (flag)
			{
				propertyBag[ADUserSchema.MobileFeaturesEnabled] = ((MobileFeaturesEnabled)obj & ~Microsoft.Exchange.Data.Directory.Recipient.MobileFeaturesEnabled.MowaDisabled);
				return;
			}
			propertyBag[ADUserSchema.MobileFeaturesEnabled] = ((MobileFeaturesEnabled)obj | Microsoft.Exchange.Data.Directory.Recipient.MobileFeaturesEnabled.MowaDisabled);
		}

		internal static object MobileHasDevicePartnershipGetter(IPropertyBag propertyBag)
		{
			object obj = propertyBag[ADUserSchema.MobileMailboxFlags];
			if (obj == null)
			{
				return false;
			}
			MobileMailboxFlags mobileMailboxFlags = (MobileMailboxFlags)obj;
			return (mobileMailboxFlags & Microsoft.Exchange.Data.Directory.Recipient.MobileMailboxFlags.HasDevicePartnership) != Microsoft.Exchange.Data.Directory.Recipient.MobileMailboxFlags.None;
		}

		internal static void MobileHasDevicePartnershipSetter(object value, IPropertyBag propertyBag)
		{
			bool flag = (bool)value;
			object obj = propertyBag[ADUserSchema.MobileMailboxFlags];
			if (obj == null)
			{
				obj = Microsoft.Exchange.Data.Directory.Recipient.MobileMailboxFlags.None;
			}
			if (flag)
			{
				propertyBag[ADUserSchema.MobileMailboxFlags] = ((MobileMailboxFlags)obj | Microsoft.Exchange.Data.Directory.Recipient.MobileMailboxFlags.HasDevicePartnership);
				return;
			}
			propertyBag[ADUserSchema.MobileMailboxFlags] = ((MobileMailboxFlags)obj & ~Microsoft.Exchange.Data.Directory.Recipient.MobileMailboxFlags.HasDevicePartnership);
		}

		public static readonly ADPropertyDefinition Birthdate = new ADPropertyDefinition("Birthdate", ExchangeObjectVersion.Exchange2010, typeof(DateTime?), null, ADPropertyDefinitionFlags.NonADProperty, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, MbxRecipientSchema.Birthdate);

		public static readonly ADPropertyDefinition Country = new ADPropertyDefinition("Country", ExchangeObjectVersion.Exchange2010, typeof(string), null, ADPropertyDefinitionFlags.NonADProperty, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, MbxRecipientSchema.Country);

		public static readonly ADPropertyDefinition Gender = new ADPropertyDefinition("Gender", ExchangeObjectVersion.Exchange2010, typeof(string), null, ADPropertyDefinitionFlags.NonADProperty, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, MbxRecipientSchema.Gender);

		public static readonly ADPropertyDefinition MemberName = new ADPropertyDefinition("MemberName", ExchangeObjectVersion.Exchange2010, typeof(string), null, ADPropertyDefinitionFlags.NonADProperty, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, MbxRecipientSchema.MemberName);

		public static readonly ADPropertyDefinition Occupation = new ADPropertyDefinition("Occupation", ExchangeObjectVersion.Exchange2010, typeof(string), null, ADPropertyDefinitionFlags.NonADProperty, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, MbxRecipientSchema.Occupation);

		public static readonly ADPropertyDefinition Region = new ADPropertyDefinition("Region", ExchangeObjectVersion.Exchange2010, typeof(string), null, ADPropertyDefinitionFlags.NonADProperty, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, MbxRecipientSchema.Region);

		public static readonly ADPropertyDefinition Timezone = new ADPropertyDefinition("Timezone", ExchangeObjectVersion.Exchange2010, typeof(string), null, ADPropertyDefinitionFlags.NonADProperty, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, MbxRecipientSchema.Timezone);

		public static readonly ADPropertyDefinition BirthdayPrecision = new ADPropertyDefinition("BirthdayPrecision", ExchangeObjectVersion.Exchange2010, typeof(string), null, ADPropertyDefinitionFlags.NonADProperty, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, MbxRecipientSchema.BirthdayPrecision);

		public static readonly ADPropertyDefinition NameVersion = new ADPropertyDefinition("NameVersion", ExchangeObjectVersion.Exchange2010, typeof(string), null, ADPropertyDefinitionFlags.NonADProperty, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, MbxRecipientSchema.NameVersion);

		public static readonly ADPropertyDefinition OptInUser = new ADPropertyDefinition("OptInUser", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.NonADProperty, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, MbxRecipientSchema.OptInUser);

		public static readonly ADPropertyDefinition IsMigratedConsumerMailbox = new ADPropertyDefinition("IsMigratedConsumerMailbox", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.NonADProperty, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, MbxRecipientSchema.IsMigratedConsumerMailbox);

		public static readonly ADPropertyDefinition MigrationDryRun = new ADPropertyDefinition("MigrationDryRun", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.NonADProperty, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, MbxRecipientSchema.MigrationDryRun);

		public static readonly ADPropertyDefinition IsPremiumConsumerMailbox = new ADPropertyDefinition("IsPremiumConsumerMailbox", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.NonADProperty, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, MbxRecipientSchema.IsPremiumConsumerMailbox);

		public static readonly ADPropertyDefinition AlternateSupportEmailAddresses = new ADPropertyDefinition("AlternateSupportEmailAddresses", ExchangeObjectVersion.Exchange2010, typeof(string), null, ADPropertyDefinitionFlags.NonADProperty, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, MbxRecipientSchema.AlternateSupportEmailAddresses);

		public static readonly ADPropertyDefinition ExchangeUserAccountControl = new ADPropertyDefinition("ExchangeUserAccountControl", ExchangeObjectVersion.Exchange2003, typeof(UserAccountControlFlags), "msExchUserAccountControl", ADPropertyDefinitionFlags.PersistDefaultValue, UserAccountControlFlags.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition LocaleID = new ADPropertyDefinition("LocaleID", ExchangeObjectVersion.Exchange2003, typeof(int), "localeID", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, MbxRecipientSchema.LocaleID);

		public static readonly ADPropertyDefinition MobileFeaturesEnabled = new ADPropertyDefinition("MobileFeaturesEnabled", ExchangeObjectVersion.Exchange2003, typeof(MobileFeaturesEnabled), "msExchOmaAdminWirelessEnable", ADPropertyDefinitionFlags.None, Microsoft.Exchange.Data.Directory.Recipient.MobileFeaturesEnabled.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, MbxRecipientSchema.MobileFeaturesEnabled);

		public static readonly ADPropertyDefinition MobileMailboxFlags = new ADPropertyDefinition("MobileMailboxFlags", ExchangeObjectVersion.Exchange2007, typeof(MobileMailboxFlags), "msExchMobileMailboxFlags", ADPropertyDefinitionFlags.None, Microsoft.Exchange.Data.Directory.Recipient.MobileMailboxFlags.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition MobileAdminExtendedSettings = new ADPropertyDefinition("MobileAdminExtendedSettings", ExchangeObjectVersion.Exchange2003, typeof(string), "msExchOmaAdminExtendedSettings", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition ActiveSyncAllowedDeviceIDs = new ADPropertyDefinition("ActiveSyncAllowedDeviceIDs", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchMobileAllowedDeviceIds", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.DoNotProvisionalClone, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, MbxRecipientSchema.ActiveSyncAllowedDeviceIDs);

		public static readonly ADPropertyDefinition ActiveSyncBlockedDeviceIDs = new ADPropertyDefinition("ActiveSyncBlockedDeviceIDs", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchMobileBlockedDeviceIds", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.DoNotProvisionalClone, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, MbxRecipientSchema.ActiveSyncBlockedDeviceIDs);

		public static readonly ADPropertyDefinition ActiveSyncMailboxPolicy = new ADPropertyDefinition("ActiveSyncMailboxPolicy", ExchangeObjectVersion.Exchange2007, typeof(ADObjectId), "msExchMobileMailboxPolicyLink", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition ActiveSyncMailboxPolicyIsDefaulted = new ADPropertyDefinition("ActiveSyncMailboxPolicyIsDefaulted", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.TaskPopulated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition CatchAllRecipientBL = new ADPropertyDefinition("CatchAllRecipientBL", ExchangeObjectVersion.Exchange2012, typeof(ADObjectId), "msExchCatchAllRecipientBL", ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.DoNotProvisionalClone | ADPropertyDefinitionFlags.BackLink, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition ActiveSyncDebugLogging = new ADPropertyDefinition("ActiveSyncDebugLogging", ExchangeObjectVersion.Exchange2007, typeof(int?), "msExchMobileDebugLogging", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, MbxRecipientSchema.ActiveSyncDebugLogging);

		public static readonly ADPropertyDefinition PasswordLastSetRaw = new ADPropertyDefinition("PasswordLastSetRaw", ExchangeObjectVersion.Exchange2003, typeof(long?), "pwdLastSet", ADPropertyDefinitionFlags.DoNotProvisionalClone, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition PrimaryGroupId = new ADPropertyDefinition("PrimaryGroupId", ExchangeObjectVersion.Exchange2003, typeof(int?), "primaryGroupId", ADPropertyDefinitionFlags.DoNotProvisionalClone, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition UnicodePassword = new ADPropertyDefinition("UnicodePassword", ExchangeObjectVersion.Exchange2003, typeof(byte[]), "unicodePwd", ADPropertyDefinitionFlags.Binary, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition QueryBaseDN = new ADPropertyDefinition("QueryBaseDN", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), "msExchQueryBaseDN", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition UMEnabledFlags = new ADPropertyDefinition("UMEnabledFlags", ExchangeObjectVersion.Exchange2007, typeof(UMEnabledFlags), "msExchUMEnabledFlags", ADPropertyDefinitionFlags.None, Microsoft.Exchange.Data.Directory.Recipient.UMEnabledFlags.Default, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition UMEnabledFlags2 = new ADPropertyDefinition("UMEnabledFlags2", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchUMEnabledFlags2", ADPropertyDefinitionFlags.PersistDefaultValue, -1, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition AdminDisplayName = ADConfigurationObjectSchema.AdminDisplayName;

		public static readonly ADPropertyDefinition AdminDisplayVersion = new ADPropertyDefinition("AdminDisplayVersion", ExchangeObjectVersion.Exchange2003, typeof(ServerVersion), null, ADPropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition IntendedMailboxPlan = new ADPropertyDefinition("IntendedMailboxPlan", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), "msExchIntendedMailboxPlanLink", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition IntendedMailboxPlanName = new ADPropertyDefinition("IntendedMailboxPlanName", ExchangeObjectVersion.Exchange2010, typeof(string), null, ADPropertyDefinitionFlags.TaskPopulated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition UMMailboxPolicy = new ADPropertyDefinition("UMMailboxPolicy", ExchangeObjectVersion.Exchange2007, typeof(ADObjectId), "msExchUMTemplateLink", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition OperatorNumber = new ADPropertyDefinition("OperatorNumber", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchUMOperatorNumber", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new UIImpactStringLengthConstraint(0, 20)
		}, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition PhoneProviderId = new ADPropertyDefinition("PhoneProviderId", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchUMPhoneProvider", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, MbxRecipientSchema.PhoneProviderId);

		public static readonly ADPropertyDefinition RMSComputerAccounts = new ADPropertyDefinition("RMSComputerAccounts", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), null, "msExchRMSComputerAccountsLink", null, "msExchRMSComputerAccountsLinkSL", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition UMPinChecksum = new ADPropertyDefinition("UMPinChecksum", ExchangeObjectVersion.Exchange2007, typeof(byte[]), "msExchUMPinChecksum", ADPropertyDefinitionFlags.Binary, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new ByteArrayLengthConstraint(160)
		}, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition UMServerWritableFlags = new ADPropertyDefinition("UMServerWritableFlags", ExchangeObjectVersion.Exchange2007, typeof(UMServerWritableFlagsBits), "msExchUMServerWritableFlags", ADPropertyDefinitionFlags.None, UMServerWritableFlagsBits.MissedCallNotificationEnabled, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition CallAnsweringAudioCodecLegacy = new ADPropertyDefinition("CallAnsweringAudioCodecLegacy", ExchangeObjectVersion.Exchange2007, typeof(AudioCodecEnum?), "msExchUMAudioCodec", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new NullableEnumValueDefinedConstraint(typeof(AudioCodecEnum))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition CallAnsweringAudioCodec2 = new ADPropertyDefinition("CallAnsweringAudioCodec2", ExchangeObjectVersion.Exchange2010, typeof(AudioCodecEnum?), "msExchUMAudioCodec2", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AccessTelephoneNumbers = new ADPropertyDefinition("AccessTelephoneNumbers", ExchangeObjectVersion.Exchange2010, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition CallAnsweringRulesExtensions = new ADPropertyDefinition("CallAnsweringRulesExtensions", ExchangeObjectVersion.Exchange2010, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition UserAccountControl = new ADPropertyDefinition("UserAccountControl", ExchangeObjectVersion.Exchange2003, typeof(UserAccountControlFlags), "userAccountControl", ADPropertyDefinitionFlags.DoNotProvisionalClone, UserAccountControlFlags.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition UserPrincipalNameRaw = new ADPropertyDefinition("UserPrincipalNameRaw", ExchangeObjectVersion.Exchange2003, typeof(string), "userPrincipalName", ADPropertyDefinitionFlags.DoNotProvisionalClone, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new MandatoryStringLengthConstraint(1, 1024),
			new RegexConstraint("^.*@[^@]+$", DataStrings.UserPrincipalNamePatternDescription)
		}, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition UserPrincipalName = new ADPropertyDefinition("UserPrincipalName", ExchangeObjectVersion.Exchange2003, typeof(string), "userPrincipalName", ADPropertyDefinitionFlags.Calculated, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(0, 1024)
		}, new ProviderPropertyDefinition[]
		{
			ADObjectSchema.Id,
			ADUserSchema.UserPrincipalNameRaw
		}, new CustomFilterBuilderDelegate(ADObject.DummyCustomFilterBuilderDelegate), new GetterDelegate(ADUser.UserPrincipalNameGetter), new SetterDelegate(ADUser.UserPrincipalNameSetter), null, null);

		public static readonly ADPropertyDefinition AltSecurityIdentities = IADSecurityPrincipalSchema.AltSecurityIdentities;

		public static readonly ADPropertyDefinition NetID = IADSecurityPrincipalSchema.NetID;

		public static readonly ADPropertyDefinition OriginalNetID = IADSecurityPrincipalSchema.OriginalNetID;

		public static readonly ADPropertyDefinition NetIDSuffix = IADSecurityPrincipalSchema.NetIDSuffix;

		public static readonly ADPropertyDefinition ConsumerNetID = IADSecurityPrincipalSchema.ConsumerNetID;

		public static readonly ADPropertyDefinition CertificateSubject = IADSecurityPrincipalSchema.CertificateSubject;

		public static readonly ADPropertyDefinition PreviousDatabase = IADMailStorageSchema.PreviousDatabase;

		public static readonly ADPropertyDefinition ElcExpirationSuspensionEndDate = IADMailStorageSchema.ElcExpirationSuspensionEndDate;

		public static readonly ADPropertyDefinition ElcExpirationSuspensionStartDate = IADMailStorageSchema.ElcExpirationSuspensionStartDate;

		public static readonly ADPropertyDefinition ElcMailboxFlags = IADMailStorageSchema.ElcMailboxFlags;

		public static readonly ADPropertyDefinition RetentionComment = IADMailStorageSchema.RetentionComment;

		public static readonly ADPropertyDefinition RetentionUrl = IADMailStorageSchema.RetentionUrl;

		public static readonly ADPropertyDefinition ElcPolicyTemplate = IADMailStorageSchema.ElcPolicyTemplate;

		public static readonly ADPropertyDefinition ManagedFolderMailboxPolicy = IADMailStorageSchema.ManagedFolderMailboxPolicy;

		public static readonly ADPropertyDefinition RetentionPolicy = IADMailStorageSchema.RetentionPolicy;

		public static readonly ADPropertyDefinition ShouldUseDefaultRetentionPolicy = IADMailStorageSchema.ShouldUseDefaultRetentionPolicy;

		public static readonly ADPropertyDefinition SharingPolicy = IADMailStorageSchema.SharingPolicy;

		public static readonly ADPropertyDefinition RemoteAccountPolicy = IADMailStorageSchema.RemoteAccountPolicy;

		public static readonly ADPropertyDefinition UseDatabaseRetentionDefaults = IADMailStorageSchema.UseDatabaseRetentionDefaults;

		public static readonly ADPropertyDefinition RetainDeletedItemsUntilBackup = IADMailStorageSchema.RetainDeletedItemsUntilBackup;

		public static readonly ADPropertyDefinition MailboxContainerGuid = IADMailStorageSchema.MailboxContainerGuid;

		public static readonly ADPropertyDefinition AggregatedMailboxGuids = IADMailStorageSchema.AggregatedMailboxGuids;

		public static readonly ADPropertyDefinition UnifiedMailbox = IADMailStorageSchema.UnifiedMailbox;

		public static readonly ADPropertyDefinition PreviousExchangeGuid = IADMailStorageSchema.PreviousExchangeGuid;

		public static readonly ADPropertyDefinition RecoverableItemsQuota = IADMailStorageSchema.RecoverableItemsQuota;

		public static readonly ADPropertyDefinition RecoverableItemsWarningQuota = IADMailStorageSchema.RecoverableItemsWarningQuota;

		public static readonly ADPropertyDefinition CalendarLoggingQuota = IADMailStorageSchema.CalendarLoggingQuota;

		public static readonly ADPropertyDefinition ApprovalApplications = IADMailStorageSchema.ApprovalApplications;

		public static readonly ADPropertyDefinition SharingPartnerIdentities = IADMailStorageSchema.SharingPartnerIdentities;

		public static readonly ADPropertyDefinition SharingAnonymousIdentities = IADMailStorageSchema.SharingAnonymousIdentities;

		public static readonly ADPropertyDefinition DatabaseName = IADMailStorageSchema.DatabaseName;

		public static readonly ADPropertyDefinition LitigationHoldEnabled = IADMailStorageSchema.LitigationHoldEnabled;

		public static readonly ADPropertyDefinition SingleItemRecoveryEnabled = IADMailStorageSchema.SingleItemRecoveryEnabled;

		public static readonly ADPropertyDefinition ElcExpirationSuspensionEnabled = IADMailStorageSchema.ElcExpirationSuspensionEnabled;

		public static readonly ADPropertyDefinition CalendarRepairDisabled = new ADPropertyDefinition("CalendarRepairDisabled", ExchangeObjectVersion.Exchange2010, typeof(bool), "msExchCalendarRepairDisabled", ADPropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, MbxRecipientSchema.CalendarRepairDisabled);

		public static readonly ADPropertyDefinition StorageGroupName = IADMailStorageSchema.StorageGroupName;

		public static readonly ADPropertyDefinition SecurityProtocol = new ADPropertyDefinition("SecurityProtocol", ExchangeObjectVersion.Exchange2003, typeof(byte[]), "securityProtocol", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Binary, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition OwaMailboxPolicy = new ADPropertyDefinition("OwaMailboxPolicy", ExchangeObjectVersion.Exchange2007, typeof(ADObjectId), "msExchOWAPolicy", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition MaxSafeSenders = new ADPropertyDefinition("MaxSafeSenders", ExchangeObjectVersion.Exchange2003, typeof(int?), "msExchMaxSafeSenders", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new RangedNullableValueConstraint<int>(0, int.MaxValue)
		}, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, MbxRecipientSchema.MaxSafeSenders);

		public static readonly ADPropertyDefinition MaxBlockedSenders = new ADPropertyDefinition("MaxBlockedSenders", ExchangeObjectVersion.Exchange2003, typeof(int?), "msExchMaxBlockedSenders", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new RangedNullableValueConstraint<int>(0, int.MaxValue)
		}, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, MbxRecipientSchema.MaxBlockedSenders);

		public static readonly ADPropertyDefinition RTCSIPPrimaryUserAddress = new ADPropertyDefinition("RTCSIPPrimaryUserAddress", ExchangeObjectVersion.Exchange2003, typeof(string), "msRTCSIP-PrimaryUserAddress", ADPropertyDefinitionFlags.ReadOnly, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RemoteRecipientType = IADMailStorageSchema.RemoteRecipientType;

		public static readonly ADPropertyDefinition ArchiveDatabaseRaw = IADMailStorageSchema.ArchiveDatabaseRaw;

		public static readonly ADPropertyDefinition ArchiveDatabase = IADMailStorageSchema.ArchiveDatabase;

		public static readonly ADPropertyDefinition ArchiveGuid = IADMailStorageSchema.ArchiveGuid;

		public static readonly ADPropertyDefinition ArchiveName = IADMailStorageSchema.ArchiveName;

		public static readonly ADPropertyDefinition ArchiveQuota = IADMailStorageSchema.ArchiveQuota;

		public static readonly ADPropertyDefinition ArchiveWarningQuota = IADMailStorageSchema.ArchiveWarningQuota;

		public static readonly ADPropertyDefinition ArchiveDomain = IADMailStorageSchema.ArchiveDomain;

		public static readonly ADPropertyDefinition ArchiveStatus = IADMailStorageSchema.ArchiveStatus;

		public static readonly ADPropertyDefinition ArchiveState = IADMailStorageSchema.ArchiveState;

		public static readonly ADPropertyDefinition DisabledArchiveGuid = IADMailStorageSchema.DisabledArchiveGuid;

		public static readonly ADPropertyDefinition DisabledArchiveDatabase = IADMailStorageSchema.DisabledArchiveDatabase;

		public static readonly ADPropertyDefinition IsAuxMailbox = IADMailStorageSchema.IsAuxMailbox;

		public static readonly ADPropertyDefinition AuxMailboxParentObjectId = IADMailStorageSchema.AuxMailboxParentObjectId;

		public static readonly ADPropertyDefinition AuxMailboxParentObjectIdBL = IADMailStorageSchema.AuxMailboxParentObjectIdBL;

		public static readonly ADPropertyDefinition MailboxRelationType = IADMailStorageSchema.MailboxRelationType;

		public static readonly ADPropertyDefinition MailboxMoveTargetMDB = IADMailStorageSchema.MailboxMoveTargetMDB;

		public static readonly ADPropertyDefinition MailboxMoveSourceMDB = IADMailStorageSchema.MailboxMoveSourceMDB;

		public static readonly ADPropertyDefinition MailboxMoveTargetArchiveMDB = IADMailStorageSchema.MailboxMoveTargetArchiveMDB;

		public static readonly ADPropertyDefinition MailboxMoveSourceArchiveMDB = IADMailStorageSchema.MailboxMoveSourceArchiveMDB;

		public static readonly ADPropertyDefinition MailboxMoveFlags = IADMailStorageSchema.MailboxMoveFlags;

		public static readonly ADPropertyDefinition MailboxMoveRemoteHostName = IADMailStorageSchema.MailboxMoveRemoteHostName;

		public static readonly ADPropertyDefinition MailboxMoveBatchName = IADMailStorageSchema.MailboxMoveBatchName;

		public static readonly ADPropertyDefinition MailboxMoveStatus = IADMailStorageSchema.MailboxMoveStatus;

		public static readonly ADPropertyDefinition MailboxRelease = SharedPropertyDefinitions.MailboxRelease;

		public static readonly ADPropertyDefinition ArchiveRelease = IADMailStorageSchema.ArchiveRelease;

		public static readonly ADPropertyDefinition CalendarVersionStoreDisabled = IADMailStorageSchema.CalendarVersionStoreDisabled;

		public static readonly ADPropertyDefinition SIPResourceIdentifier = new ADPropertyDefinition("SIPResourceIdentifier", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.TaskPopulated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition PhoneNumber = new ADPropertyDefinition("PhoneNumber", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.TaskPopulated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SourceAnchor = new ADPropertyDefinition("SourceAnchor", ExchangeObjectVersion.Exchange2010, typeof(string), "ms-MSCustomerObjectGUIDString", ADPropertyDefinitionFlags.ReadOnly, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TeamMailboxClosedTime = IADMailStorageSchema.TeamMailboxClosedTime;

		public static readonly ADPropertyDefinition SharePointSiteInfo = IADMailStorageSchema.SharePointSiteInfo;

		public static readonly ADPropertyDefinition SharePointLinkedBy = IADMailStorageSchema.SharePointLinkedBy;

		public static readonly ADPropertyDefinition Owners = IADMailStorageSchema.Owners;

		public static readonly ADPropertyDefinition SiteMailboxMessageDedupEnabled = IADMailStorageSchema.SiteMailboxMessageDedupEnabled;

		public static readonly ADPropertyDefinition TeamMailboxShowInMyClient = new ADPropertyDefinition("TeamMailboxShowInMyClient", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.TaskPopulated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TeamMailboxUserMembership = new ADPropertyDefinition("TeamMailboxUserMembership", ExchangeObjectVersion.Exchange2010, typeof(string), null, ADPropertyDefinitionFlags.TaskPopulated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TeamMailboxShowInClientList = IADMailStorageSchema.TeamMailboxShowInClientList;

		public static readonly ADPropertyDefinition LitigationHoldDate = IADMailStorageSchema.LitigationHoldDate;

		public static readonly ADPropertyDefinition LitigationHoldOwner = IADMailStorageSchema.LitigationHoldOwner;

		public static readonly ADPropertyDefinition SatchmoClusterIp = IADMailStorageSchema.SatchmoClusterIp;

		public static readonly ADPropertyDefinition SatchmoDGroup = IADMailStorageSchema.SatchmoDGroup;

		public static readonly ADPropertyDefinition PrimaryMailboxSource = IADMailStorageSchema.PrimaryMailboxSource;

		public static readonly ADPropertyDefinition FblEnabled = IADMailStorageSchema.FblEnabled;

		public static readonly ADPropertyDefinition AccountDisabled = new ADPropertyDefinition("AccountDisabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADUserSchema.ExchangeUserAccountControl
		}, null, ADObject.FlagGetterDelegate(ADUserSchema.ExchangeUserAccountControl, 2), ADUserSchema.EnumFlagSetterDelegate(ADUserSchema.ExchangeUserAccountControl, 2), null, null);

		public static readonly ADPropertyDefinition StsRefreshTokensValidFrom = new ADPropertyDefinition("StsRefreshTokensValidFrom", ExchangeObjectVersion.Exchange2010, typeof(DateTime?), "msExchStsRefreshTokensValidFrom", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition PasswordLastSet = new ADPropertyDefinition("PasswordLastSet", ExchangeObjectVersion.Exchange2003, typeof(DateTime?), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADUserSchema.PasswordLastSetRaw
		}, null, new GetterDelegate(ADUser.PasswordLastSetGetter), null, null, null);

		public static readonly ADPropertyDefinition PersistedCapabilities = SharedPropertyDefinitions.PersistedCapabilities;

		public static readonly ADPropertyDefinition TeamMailboxMembers = new ADPropertyDefinition("TeamMailboxMembers", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADUserSchema.Owners,
			ADMailboxRecipientSchema.DelegateListLink
		}, null, new GetterDelegate(TeamMailbox.MembersGetter), null, null, null);

		public static readonly ADPropertyDefinition SiteMailboxWebCollectionUrl = new ADPropertyDefinition("SiteMailboxWebCollectionUrl", ExchangeObjectVersion.Exchange2010, typeof(Uri), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADUserSchema.SharePointSiteInfo
		}, null, new GetterDelegate(TeamMailbox.WebCollectionUrlGetter), null, null, null);

		public static readonly ADPropertyDefinition SiteMailboxWebId = new ADPropertyDefinition("SiteMailboxWebId", ExchangeObjectVersion.Exchange2010, typeof(Guid), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated | ADPropertyDefinitionFlags.Binary, System.Guid.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADUserSchema.SharePointSiteInfo
		}, null, new GetterDelegate(TeamMailbox.WebIdGetter), null, null, null);

		public static readonly ADPropertyDefinition CallAnsweringAudioCodec = new ADPropertyDefinition("CallAnsweringAudioCodec", ExchangeObjectVersion.Exchange2010, typeof(AudioCodecEnum?), null, ADPropertyDefinitionFlags.Calculated, null, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(AudioCodecEnum))
		}, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADUserSchema.CallAnsweringAudioCodec2,
			ADUserSchema.CallAnsweringAudioCodecLegacy
		}, null, UMDialPlanSchema.AudioCodecGetterDelegate(ADUserSchema.CallAnsweringAudioCodec2, ADUserSchema.CallAnsweringAudioCodecLegacy, null), UMDialPlanSchema.AudioCodecSetterDelegate(ADUserSchema.CallAnsweringAudioCodec2, ADUserSchema.CallAnsweringAudioCodecLegacy), null, null);

		public static readonly ADPropertyDefinition ActiveSyncEnabled = new ADPropertyDefinition("ActiveSyncEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADUserSchema.MobileFeaturesEnabled
		}, new CustomFilterBuilderDelegate(ADUser.ActiveSyncEnabledFilterBuilder), new GetterDelegate(ADUserSchema.MobileSyncEnabledGetter), new SetterDelegate(ADUserSchema.MobileSyncEnabledSetter), null, MbxRecipientSchema.ActiveSyncEnabled);

		public static readonly ADPropertyDefinition HasActiveSyncDevicePartnership = new ADPropertyDefinition("HasActiveSyncDevicePartnership", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADUserSchema.MobileMailboxFlags
		}, new CustomFilterBuilderDelegate(ADUser.HasActiveSyncDevicePartnershipFilterBuilder), new GetterDelegate(ADUserSchema.MobileHasDevicePartnershipGetter), new SetterDelegate(ADUserSchema.MobileHasDevicePartnershipSetter), null, MbxRecipientSchema.HasActiveSyncDevicePartnership);

		public static readonly ADPropertyDefinition OWAforDevicesEnabled = new ADPropertyDefinition("OWAforDevicesEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADUserSchema.MobileFeaturesEnabled
		}, new CustomFilterBuilderDelegate(ADUser.OWAforDevicesEnabledFilterBuilder), new GetterDelegate(ADUserSchema.OWAforDevicesEnabledGetter), new SetterDelegate(ADUserSchema.OWAforDevicesEnabledSetter), null, MbxRecipientSchema.OWAforDevicesEnabled);

		public static readonly ADPropertyDefinition ResetPasswordOnNextLogon = new ADPropertyDefinition("ResetPasswordOnNextLogon", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ADPropertyDefinition[]
		{
			ADUserSchema.PasswordLastSetRaw
		}, null, new GetterDelegate(ADUser.ResetPasswordOnNextLogonGetter), new SetterDelegate(ADUser.ResetPasswordOnNextLogonSetter), null, MbxRecipientSchema.ResetPasswordOnNextLogon);

		public static readonly ADPropertyDefinition IsPilotMailboxPlan = ADObject.BitfieldProperty("IsPilotMailboxPlan", 9, SharedPropertyDefinitions.ProvisioningFlags);

		public static readonly ADPropertyDefinition UMEnabled = new ADPropertyDefinition("UMEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADUserSchema.UMEnabledFlags
		}, new CustomFilterBuilderDelegate(ADRecipient.UMEnabledFilterBuilder), (IPropertyBag propertyBag) => ((UMEnabledFlags)propertyBag[ADUserSchema.UMEnabledFlags] & Microsoft.Exchange.Data.Directory.Recipient.UMEnabledFlags.UMEnabled) == Microsoft.Exchange.Data.Directory.Recipient.UMEnabledFlags.UMEnabled, delegate(object value, IPropertyBag propertyBag)
		{
			if ((bool)value)
			{
				propertyBag[ADUserSchema.UMEnabledFlags] = ((UMEnabledFlags)propertyBag[ADUserSchema.UMEnabledFlags] | Microsoft.Exchange.Data.Directory.Recipient.UMEnabledFlags.UMEnabled);
				return;
			}
			propertyBag[ADUserSchema.UMEnabledFlags] = ((UMEnabledFlags)propertyBag[ADUserSchema.UMEnabledFlags] & ~Microsoft.Exchange.Data.Directory.Recipient.UMEnabledFlags.UMEnabled);
		}, null, null);

		public static readonly ADPropertyDefinition DowngradeHighPriorityMessagesEnabled = new ADPropertyDefinition("DowngradeHighPriorityMessagesEnabled", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADUserSchema.SecurityProtocol
		}, null, new GetterDelegate(ADUser.DowngradeHighPriorityMessagesEnabledGetter), new SetterDelegate(ADUser.DowngradeHighPriorityMessagesEnabledSetter), null, MbxRecipientSchema.DowngradeHighPriorityMessagesEnabled);

		public static readonly ADPropertyDefinition TUIAccessToCalendarEnabled = new ADPropertyDefinition("TUIAccessToCalendarEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADUserSchema.UMEnabledFlags
		}, null, (IPropertyBag propertyBag) => ((UMEnabledFlags)propertyBag[ADUserSchema.UMEnabledFlags] & Microsoft.Exchange.Data.Directory.Recipient.UMEnabledFlags.TUIAccessToCalendarEnabled) == Microsoft.Exchange.Data.Directory.Recipient.UMEnabledFlags.TUIAccessToCalendarEnabled, delegate(object value, IPropertyBag propertyBag)
		{
			if ((bool)value)
			{
				propertyBag[ADUserSchema.UMEnabledFlags] = ((UMEnabledFlags)propertyBag[ADUserSchema.UMEnabledFlags] | Microsoft.Exchange.Data.Directory.Recipient.UMEnabledFlags.TUIAccessToCalendarEnabled);
				return;
			}
			propertyBag[ADUserSchema.UMEnabledFlags] = ((UMEnabledFlags)propertyBag[ADUserSchema.UMEnabledFlags] & ~Microsoft.Exchange.Data.Directory.Recipient.UMEnabledFlags.TUIAccessToCalendarEnabled);
		}, null, null);

		public static readonly ADPropertyDefinition FaxEnabled = new ADPropertyDefinition("FaxEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADUserSchema.UMEnabledFlags
		}, null, (IPropertyBag propertyBag) => ((UMEnabledFlags)propertyBag[ADUserSchema.UMEnabledFlags] & Microsoft.Exchange.Data.Directory.Recipient.UMEnabledFlags.FaxEnabled) == Microsoft.Exchange.Data.Directory.Recipient.UMEnabledFlags.FaxEnabled, delegate(object value, IPropertyBag propertyBag)
		{
			if ((bool)value)
			{
				propertyBag[ADUserSchema.UMEnabledFlags] = ((UMEnabledFlags)propertyBag[ADUserSchema.UMEnabledFlags] | Microsoft.Exchange.Data.Directory.Recipient.UMEnabledFlags.FaxEnabled);
				return;
			}
			propertyBag[ADUserSchema.UMEnabledFlags] = ((UMEnabledFlags)propertyBag[ADUserSchema.UMEnabledFlags] & ~Microsoft.Exchange.Data.Directory.Recipient.UMEnabledFlags.FaxEnabled);
		}, null, null);

		public static readonly ADPropertyDefinition TUIAccessToEmailEnabled = new ADPropertyDefinition("TUIAccessToEmailEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADUserSchema.UMEnabledFlags
		}, null, (IPropertyBag propertyBag) => ((UMEnabledFlags)propertyBag[ADUserSchema.UMEnabledFlags] & Microsoft.Exchange.Data.Directory.Recipient.UMEnabledFlags.TUIAccessToEmailEnabled) == Microsoft.Exchange.Data.Directory.Recipient.UMEnabledFlags.TUIAccessToEmailEnabled, delegate(object value, IPropertyBag propertyBag)
		{
			if ((bool)value)
			{
				propertyBag[ADUserSchema.UMEnabledFlags] = ((UMEnabledFlags)propertyBag[ADUserSchema.UMEnabledFlags] | Microsoft.Exchange.Data.Directory.Recipient.UMEnabledFlags.TUIAccessToEmailEnabled);
				return;
			}
			propertyBag[ADUserSchema.UMEnabledFlags] = ((UMEnabledFlags)propertyBag[ADUserSchema.UMEnabledFlags] & ~Microsoft.Exchange.Data.Directory.Recipient.UMEnabledFlags.TUIAccessToEmailEnabled);
		}, null, null);

		public static readonly ADPropertyDefinition SubscriberAccessEnabled = new ADPropertyDefinition("SubscriberAccessEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADUserSchema.UMEnabledFlags
		}, null, (IPropertyBag propertyBag) => ((UMEnabledFlags)propertyBag[ADUserSchema.UMEnabledFlags] & Microsoft.Exchange.Data.Directory.Recipient.UMEnabledFlags.SubscriberAccessEnabled) == Microsoft.Exchange.Data.Directory.Recipient.UMEnabledFlags.SubscriberAccessEnabled, delegate(object value, IPropertyBag propertyBag)
		{
			if ((bool)value)
			{
				propertyBag[ADUserSchema.UMEnabledFlags] = ((UMEnabledFlags)propertyBag[ADUserSchema.UMEnabledFlags] | Microsoft.Exchange.Data.Directory.Recipient.UMEnabledFlags.SubscriberAccessEnabled);
				return;
			}
			propertyBag[ADUserSchema.UMEnabledFlags] = ((UMEnabledFlags)propertyBag[ADUserSchema.UMEnabledFlags] & ~Microsoft.Exchange.Data.Directory.Recipient.UMEnabledFlags.SubscriberAccessEnabled);
		}, null, MbxRecipientSchema.SubscriberAccessEnabled);

		public static readonly ADPropertyDefinition MissedCallNotificationEnabled = new ADPropertyDefinition("MissedCallNotificationEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADUserSchema.UMServerWritableFlags
		}, null, (IPropertyBag propertyBag) => ((UMServerWritableFlagsBits)propertyBag[ADUserSchema.UMServerWritableFlags] & UMServerWritableFlagsBits.MissedCallNotificationEnabled) == UMServerWritableFlagsBits.MissedCallNotificationEnabled, delegate(object value, IPropertyBag propertyBag)
		{
			if ((bool)value)
			{
				propertyBag[ADUserSchema.UMServerWritableFlags] = ((UMServerWritableFlagsBits)propertyBag[ADUserSchema.UMServerWritableFlags] | UMServerWritableFlagsBits.MissedCallNotificationEnabled);
				return;
			}
			propertyBag[ADUserSchema.UMServerWritableFlags] = ((UMServerWritableFlagsBits)propertyBag[ADUserSchema.UMServerWritableFlags] & ~UMServerWritableFlagsBits.MissedCallNotificationEnabled);
		}, null, null);

		public static readonly ADPropertyDefinition UMSMSNotificationOption = new ADPropertyDefinition("UMSMSNotificationOption", ExchangeObjectVersion.Exchange2010, typeof(UMSMSNotificationOptions), null, ADPropertyDefinitionFlags.Calculated, UMSMSNotificationOptions.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADUserSchema.UMServerWritableFlags
		}, null, delegate(IPropertyBag propertyBag)
		{
			bool flag = ((UMServerWritableFlagsBits)propertyBag[ADUserSchema.UMServerWritableFlags] & UMServerWritableFlagsBits.SMSVoiceMailNotificationEnabled) == UMServerWritableFlagsBits.SMSVoiceMailNotificationEnabled;
			bool flag2 = ((UMServerWritableFlagsBits)propertyBag[ADUserSchema.UMServerWritableFlags] & UMServerWritableFlagsBits.SMSMissedCallNotificationEnabled) == UMServerWritableFlagsBits.SMSMissedCallNotificationEnabled;
			if (flag && flag2)
			{
				return UMSMSNotificationOptions.VoiceMailAndMissedCalls;
			}
			if (flag)
			{
				return UMSMSNotificationOptions.VoiceMail;
			}
			return UMSMSNotificationOptions.None;
		}, delegate(object value, IPropertyBag propertyBag)
		{
			UMServerWritableFlagsBits umserverWritableFlagsBits = (UMServerWritableFlagsBits)propertyBag[ADUserSchema.UMServerWritableFlags];
			UMServerWritableFlagsBits umserverWritableFlagsBits2 = UMServerWritableFlagsBits.SMSVoiceMailNotificationEnabled;
			UMServerWritableFlagsBits umserverWritableFlagsBits3 = UMServerWritableFlagsBits.SMSMissedCallNotificationEnabled;
			umserverWritableFlagsBits &= ~umserverWritableFlagsBits2;
			umserverWritableFlagsBits &= ~umserverWritableFlagsBits3;
			UMSMSNotificationOptions umsmsnotificationOptions = (UMSMSNotificationOptions)value;
			if (umsmsnotificationOptions == UMSMSNotificationOptions.VoiceMail)
			{
				umserverWritableFlagsBits |= umserverWritableFlagsBits2;
			}
			else if (umsmsnotificationOptions == UMSMSNotificationOptions.VoiceMailAndMissedCalls)
			{
				umserverWritableFlagsBits |= umserverWritableFlagsBits2;
				umserverWritableFlagsBits |= umserverWritableFlagsBits3;
			}
			propertyBag[ADUserSchema.UMServerWritableFlags] = umserverWritableFlagsBits;
		}, null, null);

		public static readonly ADPropertyDefinition PinlessAccessToVoiceMailEnabled = new ADPropertyDefinition("PinlessAccessToVoiceMailEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADUserSchema.UMServerWritableFlags
		}, null, (IPropertyBag propertyBag) => ((UMServerWritableFlagsBits)propertyBag[ADUserSchema.UMServerWritableFlags] & UMServerWritableFlagsBits.PinlessAccessToVoiceMailEnabled) == UMServerWritableFlagsBits.PinlessAccessToVoiceMailEnabled, delegate(object value, IPropertyBag propertyBag)
		{
			if ((bool)value)
			{
				propertyBag[ADUserSchema.UMServerWritableFlags] = ((UMServerWritableFlagsBits)propertyBag[ADUserSchema.UMServerWritableFlags] | UMServerWritableFlagsBits.PinlessAccessToVoiceMailEnabled);
				return;
			}
			propertyBag[ADUserSchema.UMServerWritableFlags] = ((UMServerWritableFlagsBits)propertyBag[ADUserSchema.UMServerWritableFlags] & ~UMServerWritableFlagsBits.PinlessAccessToVoiceMailEnabled);
		}, null, null);

		public static readonly ADPropertyDefinition AnonymousCallersCanLeaveMessages = new ADPropertyDefinition("AnonymousCallersCanLeaveMessages", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADUserSchema.UMEnabledFlags
		}, null, (IPropertyBag propertyBag) => ((UMEnabledFlags)propertyBag[ADUserSchema.UMEnabledFlags] & Microsoft.Exchange.Data.Directory.Recipient.UMEnabledFlags.AnonymousCallersCanLeaveMessages) == Microsoft.Exchange.Data.Directory.Recipient.UMEnabledFlags.AnonymousCallersCanLeaveMessages, delegate(object value, IPropertyBag propertyBag)
		{
			if ((bool)value)
			{
				propertyBag[ADUserSchema.UMEnabledFlags] = ((UMEnabledFlags)propertyBag[ADUserSchema.UMEnabledFlags] | Microsoft.Exchange.Data.Directory.Recipient.UMEnabledFlags.AnonymousCallersCanLeaveMessages);
				return;
			}
			propertyBag[ADUserSchema.UMEnabledFlags] = ((UMEnabledFlags)propertyBag[ADUserSchema.UMEnabledFlags] & ~Microsoft.Exchange.Data.Directory.Recipient.UMEnabledFlags.AnonymousCallersCanLeaveMessages);
		}, null, null);

		public static readonly ADPropertyDefinition ASREnabled = new ADPropertyDefinition("ASREnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADUserSchema.UMEnabledFlags
		}, null, (IPropertyBag propertyBag) => ((UMEnabledFlags)propertyBag[ADUserSchema.UMEnabledFlags] & Microsoft.Exchange.Data.Directory.Recipient.UMEnabledFlags.ASREnabled) == Microsoft.Exchange.Data.Directory.Recipient.UMEnabledFlags.ASREnabled, delegate(object value, IPropertyBag propertyBag)
		{
			if ((bool)value)
			{
				propertyBag[ADUserSchema.UMEnabledFlags] = ((UMEnabledFlags)propertyBag[ADUserSchema.UMEnabledFlags] | Microsoft.Exchange.Data.Directory.Recipient.UMEnabledFlags.ASREnabled);
				return;
			}
			propertyBag[ADUserSchema.UMEnabledFlags] = ((UMEnabledFlags)propertyBag[ADUserSchema.UMEnabledFlags] & ~Microsoft.Exchange.Data.Directory.Recipient.UMEnabledFlags.ASREnabled);
		}, null, null);

		public static readonly ADPropertyDefinition VoiceMailAnalysisEnabled = new ADPropertyDefinition("VoiceMailAnalysisEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADUserSchema.UMEnabledFlags
		}, null, (IPropertyBag propertyBag) => ((UMEnabledFlags)propertyBag[ADUserSchema.UMEnabledFlags] & Microsoft.Exchange.Data.Directory.Recipient.UMEnabledFlags.VoiceMailAnalysisEnabled) == Microsoft.Exchange.Data.Directory.Recipient.UMEnabledFlags.VoiceMailAnalysisEnabled, delegate(object value, IPropertyBag propertyBag)
		{
			if ((bool)value)
			{
				propertyBag[ADUserSchema.UMEnabledFlags] = ((UMEnabledFlags)propertyBag[ADUserSchema.UMEnabledFlags] | Microsoft.Exchange.Data.Directory.Recipient.UMEnabledFlags.VoiceMailAnalysisEnabled);
				return;
			}
			propertyBag[ADUserSchema.UMEnabledFlags] = ((UMEnabledFlags)propertyBag[ADUserSchema.UMEnabledFlags] & ~Microsoft.Exchange.Data.Directory.Recipient.UMEnabledFlags.VoiceMailAnalysisEnabled);
		}, null, null);

		public static readonly ADPropertyDefinition PlayOnPhoneEnabled = new ADPropertyDefinition("PlayOnPhoneEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADUserSchema.UMEnabledFlags2
		}, null, ADObject.FlagGetterDelegate(ADUserSchema.UMEnabledFlags2, 1), ADObject.FlagSetterDelegate(ADUserSchema.UMEnabledFlags2, 1), null, null);

		public static readonly ADPropertyDefinition CallAnsweringRulesEnabled = new ADPropertyDefinition("CallAnsweringRulesEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADUserSchema.UMEnabledFlags2
		}, null, ADObject.FlagGetterDelegate(ADUserSchema.UMEnabledFlags2, 4), ADObject.FlagSetterDelegate(ADUserSchema.UMEnabledFlags2, 4), null, null);

		public static readonly ADPropertyDefinition Company = ADOrgPersonSchema.Company;

		public static readonly ADPropertyDefinition Co = ADOrgPersonSchema.Co;

		internal static readonly ADPropertyDefinition C = ADOrgPersonSchema.C;

		internal static readonly ADPropertyDefinition CountryCode = ADOrgPersonSchema.CountryCode;

		public static readonly ADPropertyDefinition Department = ADOrgPersonSchema.Department;

		public static readonly ADPropertyDefinition DirectReports = ADOrgPersonSchema.DirectReports;

		public static readonly ADPropertyDefinition Fax = ADOrgPersonSchema.Fax;

		public static readonly ADPropertyDefinition FirstName = ADOrgPersonSchema.FirstName;

		public static readonly ADPropertyDefinition HomePhone = ADOrgPersonSchema.HomePhone;

		public static readonly ADPropertyDefinition Initials = ADOrgPersonSchema.Initials;

		public static readonly ADPropertyDefinition LanguagesRaw = ADOrgPersonSchema.LanguagesRaw;

		public static readonly ADPropertyDefinition LastName = ADOrgPersonSchema.LastName;

		public static readonly ADPropertyDefinition City = ADOrgPersonSchema.City;

		public static readonly ADPropertyDefinition Manager = ADOrgPersonSchema.Manager;

		public static readonly ADPropertyDefinition MobilePhone = ADOrgPersonSchema.MobilePhone;

		public static readonly ADPropertyDefinition Office = ADOrgPersonSchema.Office;

		public static readonly ADPropertyDefinition OtherFax = ADOrgPersonSchema.OtherFax;

		public static readonly ADPropertyDefinition OtherHomePhone = ADOrgPersonSchema.OtherHomePhone;

		public static readonly ADPropertyDefinition OtherTelephone = ADOrgPersonSchema.OtherTelephone;

		public static readonly ADPropertyDefinition OtherMobile = ADOrgPersonSchema.OtherMobile;

		public static readonly ADPropertyDefinition Pager = ADOrgPersonSchema.Pager;

		public static readonly ADPropertyDefinition Phone = ADOrgPersonSchema.Phone;

		public static readonly ADPropertyDefinition PostalCode = ADOrgPersonSchema.PostalCode;

		public static readonly ADPropertyDefinition PostOfficeBox = ADOrgPersonSchema.PostOfficeBox;

		public static readonly ADPropertyDefinition StateOrProvince = ADOrgPersonSchema.StateOrProvince;

		public static readonly ADPropertyDefinition StreetAddress = ADOrgPersonSchema.StreetAddress;

		public static readonly ADPropertyDefinition TelephoneAssistant = ADOrgPersonSchema.TelephoneAssistant;

		public static readonly ADPropertyDefinition Title = ADOrgPersonSchema.Title;

		public static readonly ADPropertyDefinition ViewDepth = ADOrgPersonSchema.ViewDepth;

		public static readonly ADPropertyDefinition RtcSipLine = ADOrgPersonSchema.RtcSipLine;

		public static readonly ADPropertyDefinition UMCallingLineIds = ADOrgPersonSchema.UMCallingLineIds;

		public static readonly ADPropertyDefinition VoiceMailSettings = ADOrgPersonSchema.VoiceMailSettings;

		public static readonly ADPropertyDefinition CountryOrRegion = ADOrgPersonSchema.CountryOrRegion;

		public static readonly ADPropertyDefinition Languages = ADOrgPersonSchema.Languages;

		public static readonly ADPropertyDefinition SanitizedPhoneNumbers = ADOrgPersonSchema.SanitizedPhoneNumbers;
	}
}
