using System;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class OrganizationSchema : ADLegacyVersionableObjectSchema
	{
		internal static ADPropertyDefinition HeuristicsProperty(string name, HeuristicsFlags mask, ADPropertyDefinition fieldProperty)
		{
			GetterDelegate getterDelegate = (IPropertyBag bag) => (bool)OrganizationSchema.HeuristicsFlagsGetter(mask, fieldProperty, bag);
			SetterDelegate setterDelegate = delegate(object value, IPropertyBag bag)
			{
				OrganizationSchema.HeuristicsFlagsSetter(mask, fieldProperty, value, bag);
			};
			return new ADPropertyDefinition(name, fieldProperty.VersionAdded, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ADPropertyDefinition[]
			{
				fieldProperty
			}, null, getterDelegate, setterDelegate, null, null);
		}

		internal static object HeuristicsFlagsGetter(HeuristicsFlags mask, ADPropertyDefinition fieldProperty, IPropertyBag propertyBag)
		{
			HeuristicsFlags heuristicsFlags = (HeuristicsFlags)propertyBag[fieldProperty];
			return (heuristicsFlags & mask) != HeuristicsFlags.None;
		}

		internal static void HeuristicsFlagsSetter(HeuristicsFlags mask, ADPropertyDefinition fieldProperty, object value, IPropertyBag propertyBag)
		{
			HeuristicsFlags heuristicsFlags = (HeuristicsFlags)propertyBag[fieldProperty];
			if ((bool)value)
			{
				heuristicsFlags |= mask;
			}
			else
			{
				heuristicsFlags &= ~mask;
			}
			propertyBag[fieldProperty] = heuristicsFlags;
		}

		internal const int MaxSupportedMaxConcurrentMigrationsValue = 1000;

		internal const int MailTipsLargeAudienceThresholdShift = 0;

		internal const int MailTipsLargeAudienceThresholdLength = 10;

		internal const int MailTipsExternalRecipientsTipsEnabledShift = 11;

		internal const int MailTipsMailboxSourcedTipsEnabledShift = 12;

		internal const int MailTipsGroupMetricsEnabledShift = 13;

		internal const int MailTipsAllTipsEnabledShift = 14;

		internal const int PreferredInternetCodePageForShiftJisShift = 2;

		internal const int PreferredInternetCodePageForShiftJisLength = 3;

		internal const int RequiredCharsetCoverageRawShift = 5;

		internal const int RequiredCharsetCoverageRawLength = 7;

		internal const int ByteEncoderTypeFor7BitCharsetsShift = 12;

		internal const int ByteEncoderTypeFor7BitCharsetsLength = 7;

		internal const int RequiredCharsetCoverageInitializedShift = 19;

		internal const int IsFederatedShift = 0;

		internal const int IsHotmailMigrationShift = 1;

		internal const int SkipToUAndParentalControlCheckShift = 2;

		internal const int ReadTrackingEnabledShift = 3;

		internal const int BuildMinorShift = 6;

		internal const int BuildMinorLength = 6;

		internal const int BuildMajorShift = 12;

		internal const int BuildMajorLength = 11;

		internal const int IsUpdatingServicePlanShift = 23;

		internal const int IsUpgradingOrganizationShift = 24;

		internal const int HideAdminAccessWarningShift = 25;

		internal const int SMTPAddressCheckWithAcceptedDomainShift = 26;

		internal const int ActivityBasedAuthenticationTimeoutDisabledShift = 27;

		internal const int ActivityBasedAuthenticationTimeoutWithSingleSignOnDisabledShift = 28;

		internal const int MSOSyncEnabledShift = 29;

		internal const int SyncMBXAndDLToMServShift = 30;

		internal const int ForwardSyncLiveIdBusinessInstanceShift = 31;

		internal const int MapiHttpEnabledFlagShift = 10;

		internal const int IntuneManagedStatusFlagShift = 12;

		internal const int RmsoSubscriptionStatusFlagsShift = 7;

		internal const int RmsoSubscriptionStatusFlagsLength = 3;

		internal const int OfflineAuthProvisioningFlagsShift = 3;

		internal const int MaxExchangeNotificationRecipientsCount = 64;

		internal const int MaxIntAsPercent = 100;

		internal const int HybridConfigurationStatusFlagsShift = 13;

		internal const int HybridConfigurationStatusFlagsLength = 4;

		internal const int IsUMGatewayAllowedFlagShift = 17;

		internal const int OAuth2ClientProfileEnabledFlagShift = 18;

		internal const int RealTimeLogServiceEnabledFlagShift = 19;

		internal const int CustomerLockboxEnabledFlagShift = 20;

		internal const int ACLableSyncedObjectEnabledShift = 21;

		internal const string MsoDirSyncStatusPendingPrefix = "Pending";

		internal const int DefaultTimeWindow = 28800;

		internal const int SoftDeletedFeatureShift = 13;

		internal const int SoftDeletedFeatureLength = 3;

		private const int GuidPrefixedLegacyDnShift = 16;

		private const int IsMailboxForcedReplicationDisabledShift = 17;

		private const int IsSyncPropertySetUpgradeAllowedShift = 18;

		internal const int IsProcessEhaMigratedMessagesEnabledShift = 19;

		internal const int IsPilotingOrganizationShift = 20;

		internal const int IsTenantAccessBlockedAllowedShift = 22;

		internal const int IsUpgradeOperationInProgressShift = 23;

		internal static readonly int OrgConfigurationVersion = 16133;

		public static readonly ADPropertyDefinition LegacyExchangeDN = ADObject.LegacyDnProperty(ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.DoNotProvisionalClone);

		public static readonly ADPropertyDefinition Heuristics = new ADPropertyDefinition("Heuristics", ExchangeObjectVersion.Exchange2003, typeof(HeuristicsFlags), "heuristics", ADPropertyDefinitionFlags.None, HeuristicsFlags.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition BlobMimeTypes = new ADPropertyDefinition("BlobMimeTypes", ExchangeObjectVersion.Exchange2003, typeof(byte[]), "msExchMimeTypes", ADPropertyDefinitionFlags.Binary, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MimeTypes = new ADPropertyDefinition("MimeTypes", ExchangeObjectVersion.Exchange2003, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OrganizationSchema.BlobMimeTypes
		}, null, new GetterDelegate(Organization.MimeTypesGetter), new SetterDelegate(Organization.MimeTypesSetter), null, null);

		public static readonly ADPropertyDefinition ResourceAddressLists = new ADPropertyDefinition("ResourceAddressLists", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), "msExchResourceAddressLists", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.DoNotValidate, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition IsMixedMode = new ADPropertyDefinition("IsMixedMode", ExchangeObjectVersion.Exchange2003, typeof(bool), "msExchMixedMode", ADPropertyDefinitionFlags.None, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		[Obsolete("IsPublicFolderContentReplicationDisabled is obsolete.")]
		public static readonly ADPropertyDefinition PublicFolderContentReplicationDisabled = OrganizationSchema.HeuristicsProperty("PublicFolderContentReplicationDisabled", HeuristicsFlags.SuspendFolderReplication, OrganizationSchema.Heuristics);

		public static readonly ADPropertyDefinition PublicFoldersLockedForMigration = OrganizationSchema.HeuristicsProperty("PublicFoldersLockedForMigration", HeuristicsFlags.PublicFoldersLockedForMigration, OrganizationSchema.Heuristics);

		public static readonly ADPropertyDefinition PublicFolderMigrationComplete = OrganizationSchema.HeuristicsProperty("PublicFolderMigrationComplete", HeuristicsFlags.PublicFolderMigrationComplete, OrganizationSchema.Heuristics);

		public static readonly ADPropertyDefinition PublicFolderMailboxesLockedForNewConnections = OrganizationSchema.HeuristicsProperty("PublicFolderMailboxesLockedForNewConnections", HeuristicsFlags.PublicFolderMailboxesLockedForNewConnections, OrganizationSchema.Heuristics);

		public static readonly ADPropertyDefinition PublicFolderMailboxesMigrationComplete = OrganizationSchema.HeuristicsProperty("PublicFolderMailboxesMigrationComplete", HeuristicsFlags.PublicFolderMailboxesMigrationComplete, OrganizationSchema.Heuristics);

		public static readonly ADPropertyDefinition PublicFoldersEnabled = new ADPropertyDefinition("PublicFoldersEnabled", ExchangeObjectVersion.Exchange2003, typeof(PublicFoldersDeployment), null, ADPropertyDefinitionFlags.Calculated, PublicFoldersDeployment.Local, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OrganizationSchema.Heuristics
		}, null, new GetterDelegate(Organization.PublicFoldersEnabledGetter), new SetterDelegate(Organization.PublicFoldersEnabledSetter), null, null);

		public static readonly ADPropertyDefinition IsAddressListPagingEnabled = new ADPropertyDefinition("IsAddressListPagingEnabled", ExchangeObjectVersion.Exchange2003, typeof(bool), "msExchAddressListPagingEnabled", ADPropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ManagedFolderHomepage = new ADPropertyDefinition("ManagedFolderHomepage", ExchangeObjectVersion.Exchange2003, typeof(string), "msExchELCOrganizationalRootURL", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DefaultPublicFolderProhibitPostQuota = LegacyPublicFolderDatabaseSchema.ProhibitPostQuota;

		public static readonly ADPropertyDefinition DefaultPublicFolderIssueWarningQuota = LegacyDatabaseSchema.IssueWarningQuota;

		public static readonly ADPropertyDefinition DefaultPublicFolderMaxItemSize = LegacyMailboxDatabaseSchema.ProhibitSendReceiveQuota;

		public static readonly ADPropertyDefinition DefaultPublicFolderDeletedItemRetention = new ADPropertyDefinition("DefaultPublicFolderDeletedItemRetention", ExchangeObjectVersion.Exchange2003, typeof(EnhancedTimeSpan), "msExchPublicFolderDeletedItemRetention", ADPropertyDefinitionFlags.PersistDefaultValue, EnhancedTimeSpan.FromDays(30.0), new PropertyDefinitionConstraint[]
		{
			new RangedNullableValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.FromDays(1.0), EnhancedTimeSpan.FromDays(24855.0)),
			new NullableEnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneDay)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DefaultPublicFolderMovedItemRetention = new ADPropertyDefinition("DefaultPublicFolderMovedItemRetention", ExchangeObjectVersion.Exchange2003, typeof(EnhancedTimeSpan), "msExchPublicFolderMovedItemRetention", ADPropertyDefinitionFlags.PersistDefaultValue, EnhancedTimeSpan.FromDays(7.0), new PropertyDefinitionConstraint[]
		{
			new RangedNullableValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.FromDays(1.0), EnhancedTimeSpan.FromDays(24855.0)),
			new NullableEnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneDay)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DefaultPublicFolderAgeLimit = new ADPropertyDefinition("DefaultPublicFolderAgeLimit", ExchangeObjectVersion.Exchange2003, typeof(EnhancedTimeSpan?), "msExchOverallAgeLimit", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new RangedNullableValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.FromDays(1.0), EnhancedTimeSpan.FromDays(24855.0)),
			new NullableEnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneDay)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ServiceEndpoints = new ADPropertyDefinition("ServiceEndpoints", ExchangeObjectVersion.Exchange2003, typeof(string), "msExchServiceEndPointURL", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SiteMailboxCreationURL = new ADPropertyDefinition("SiteMailboxCreationURL", ExchangeObjectVersion.Exchange2003, typeof(Uri), null, ADPropertyDefinitionFlags.Calculated, null, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, new ProviderPropertyDefinition[]
		{
			OrganizationSchema.ServiceEndpoints
		}, null, new GetterDelegate(Organization.SiteMailboxCreationURLGetter), new SetterDelegate(Organization.SiteMailboxCreationURLSetter), null, null);

		public static readonly ADPropertyDefinition ForeignForestFQDNRaw = new ADPropertyDefinition("ForeignForestFQDNRaw", ExchangeObjectVersion.Exchange2003, typeof(string), "msExchForeignForestFQDN", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ForeignForestFQDN = new ADPropertyDefinition("ForeignForestFQDN", ExchangeObjectVersion.Exchange2003, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OrganizationSchema.ForeignForestFQDNRaw
		}, null, new GetterDelegate(Organization.ForeignForestFQDNGetter), new SetterDelegate(Organization.ForeignForestFQDNSetter), null, null);

		public static readonly ADPropertyDefinition ForeignForestOrgAdminUSGSid = new ADPropertyDefinition("ForeignForestOrgAdminUSGSid", ExchangeObjectVersion.Exchange2003, typeof(SecurityIdentifier), "msExchForeignForestOrgAdminUSGSid", ADPropertyDefinitionFlags.Binary, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ForeignForestViewOnlyAdminUSGSid = new ADPropertyDefinition("ForeignForestViewOnlyAdminUSGSid", ExchangeObjectVersion.Exchange2003, typeof(SecurityIdentifier), "msExchForeignForestReadOnlyAdminUSGSid", ADPropertyDefinitionFlags.Binary, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ForeignForestRecipientAdminUSGSid = new ADPropertyDefinition("ForeignForestRecipientAdminUSGSid", ExchangeObjectVersion.Exchange2003, typeof(SecurityIdentifier), "msExchForeignForestRecipientAdminUSGSid", ADPropertyDefinitionFlags.Binary, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		[Obsolete("ForeignForestViewOnlyAdminUSGSid is obsolete.")]
		public static readonly ADPropertyDefinition ForeignForestPublicFolderAdminUSGSid = new ADPropertyDefinition("ForeignForestPublicFolderAdminUSGSid", ExchangeObjectVersion.Exchange2003, typeof(SecurityIdentifier), "msExchForeignForestPublicFolderAdminUSGSid", ADPropertyDefinitionFlags.Binary, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ObjectVersion = new ADPropertyDefinition("ObjectVersion", ExchangeObjectVersion.Exchange2003, typeof(int), "objectVersion", ADPropertyDefinitionFlags.PersistDefaultValue, OrganizationSchema.OrgConfigurationVersion, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition BuildNumber = new ADPropertyDefinition("msExchProductID", ExchangeObjectVersion.Exchange2003, typeof(string), "msExchProductID", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SCLJunkThreshold = new ADPropertyDefinition("SCLJunkThreshold", ExchangeObjectVersion.Exchange2003, typeof(int), null, ADPropertyDefinitionFlags.PersistDefaultValue | ADPropertyDefinitionFlags.TaskPopulated, 4, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, 9)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AcceptedDomainNames = new ADPropertyDefinition("AcceptedDomainNames", ExchangeObjectVersion.Exchange2003, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(0, 1123)
		}, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition MicrosoftExchangeRecipientEmailAddresses = new ADPropertyDefinition("MicrosoftExchangeRecipientEmailAddresses", ExchangeObjectVersion.Exchange2003, typeof(ProxyAddress), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(0, 1123)
		}, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition MicrosoftExchangeRecipientReplyRecipient = new ADPropertyDefinition("MicrosoftExchangeRecipientReplyRecipient", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), null, ADPropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition MicrosoftExchangeRecipientPrimarySmtpAddress = new ADPropertyDefinition("MicrosoftExchangeRecipientPrimarySmtpAddress", ExchangeObjectVersion.Exchange2003, typeof(SmtpAddress), null, ADPropertyDefinitionFlags.TaskPopulated, SmtpAddress.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new ValidSmtpAddressConstraint()
		}, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition MicrosoftExchangeRecipientEmailAddressPolicyEnabled = new ADPropertyDefinition("MicrosoftExchangeRecipientEmailAddressPolicyEnabled", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.TaskPopulated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition Industry = new ADPropertyDefinition("Industry", ExchangeObjectVersion.Exchange2003, typeof(IndustryType), "msExchIndustry", ADPropertyDefinitionFlags.PersistDefaultValue, IndustryType.NotSpecified, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition CustomerFeedbackEnabled = new ADPropertyDefinition("CustomerFeedbackEnabled", ExchangeObjectVersion.Exchange2003, typeof(bool?), "msExchCustomerFeedbackEnabled", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition OrganizationSummary = new ADPropertyDefinition("OrganizationSummary", ExchangeObjectVersion.Exchange2003, typeof(OrganizationSummaryEntry), "msExchOrganizationSummary", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MailTipsSettings = new ADPropertyDefinition("MailTipsSettings", ExchangeObjectVersion.Exchange2003, typeof(long), "msExchMailTipsSettings", ADPropertyDefinitionFlags.PersistDefaultValue, 0L, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MailTipsLargeAudienceThreshold = ADObject.BitfieldProperty("MailTipsLargeAudienceThreshold", 0, 10, OrganizationSchema.MailTipsSettings, new RangedValueConstraint<long>(0L, 1000L));

		public static readonly ADPropertyDefinition MailTipsExternalRecipientsTipsEnabled = ADObject.BitfieldProperty("MailTipsExternalRecipientsTipsEnabled", 11, OrganizationSchema.MailTipsSettings);

		public static readonly ADPropertyDefinition MailTipsMailboxSourcedTipsEnabled = ADObject.BitfieldProperty("MailTipsMailboxSourcedTipsEnabled", 12, OrganizationSchema.MailTipsSettings);

		public static readonly ADPropertyDefinition MailTipsGroupMetricsEnabled = ADObject.BitfieldProperty("MailTipsGroupMetricsEnabled", 13, OrganizationSchema.MailTipsSettings);

		public static readonly ADPropertyDefinition MailTipsAllTipsEnabled = ADObject.BitfieldProperty("MailTipsAllTipsEnabled", 14, OrganizationSchema.MailTipsSettings);

		internal static readonly ADPropertyDefinition ContentConversionFlags = new ADPropertyDefinition("msExchContentConversionSettings", ExchangeObjectVersion.Exchange2003, typeof(int), "msExchContentConversionSettings", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition PreferredInternetCodePageForShiftJis = ADObject.BitfieldProperty("PreferredInternetCodePageForShiftJis", 2, 3, OrganizationSchema.ContentConversionFlags);

		public static readonly ADPropertyDefinition ByteEncoderTypeFor7BitCharsets = ADObject.BitfieldProperty("ByteEncoderTypeFor7BitCharsets", 12, 7, OrganizationSchema.ContentConversionFlags);

		public static readonly ADPropertyDefinition RequiredCharsetCoverage = new ADPropertyDefinition("RequiredCharsetCoverage", ExchangeObjectVersion.Exchange2003, typeof(int), null, ADPropertyDefinitionFlags.Calculated, 100, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, 100)
		}, new ProviderPropertyDefinition[]
		{
			OrganizationSchema.ContentConversionFlags
		}, null, new GetterDelegate(Organization.RequiredCharsetCoverageGetter), new SetterDelegate(Organization.RequiredCharsetCoverageSetter), null, null);

		internal static readonly ADPropertyDefinition OrganizationFlags = new ADPropertyDefinition("OrganizationFlags", ExchangeObjectVersion.Exchange2003, typeof(int), "msExchOrganizationFlags", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		internal static readonly ADPropertyDefinition OrganizationFlags2 = new ADPropertyDefinition("OrganizationFlags2", ExchangeObjectVersion.Exchange2003, typeof(int), "msExchOrganizationFlags2", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition IsFederated = ADObject.BitfieldProperty("IsFederated", 0, OrganizationSchema.OrganizationFlags);

		public static readonly ADPropertyDefinition ForwardSyncLiveIdBusinessInstance = ADObject.BitfieldProperty("ForwardSyncLiveIdBusinessInstance", 31, OrganizationSchema.OrganizationFlags);

		public static readonly ADPropertyDefinition IsHotmailMigration = ADObject.BitfieldProperty("IsHotmailMigration", 1, OrganizationSchema.OrganizationFlags);

		public static readonly ADPropertyDefinition SkipToUAndParentalControlCheck = ADObject.BitfieldProperty("SkipToUAndParentalControlCheck", 2, OrganizationSchema.OrganizationFlags);

		public static readonly ADPropertyDefinition HideAdminAccessWarning = ADObject.BitfieldProperty("HideAdminAccessWarning", 25, OrganizationSchema.OrganizationFlags);

		public static readonly ADPropertyDefinition SMTPAddressCheckWithAcceptedDomain = ADObject.BitfieldProperty("SMTPAddressCheckWithAcceptedDomain", 26, OrganizationSchema.OrganizationFlags);

		public static readonly ADPropertyDefinition ActivityBasedAuthenticationTimeoutInterval = new ADPropertyDefinition("ActivityBasedAuthenticationTimeoutInterval", ExchangeObjectVersion.Exchange2003, typeof(EnhancedTimeSpan), "msExchActivityBasedAuthenticationTimeoutInterval", ADPropertyDefinitionFlags.PersistDefaultValue, EnhancedTimeSpan.FromHours(6.0), new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.FromMinutes(5.0), EnhancedTimeSpan.FromHours(8.0)),
			new EnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneSecond)
		}, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.FromMinutes(5.0), EnhancedTimeSpan.FromSeconds(28800.0)),
			new EnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneSecond)
		}, null, null);

		public static readonly ADPropertyDefinition ActivityBasedAuthenticationTimeoutDisabled = ADObject.BitfieldProperty("ActivityBasedAuthenticationTimeoutDisabled", 27, OrganizationSchema.OrganizationFlags);

		public static readonly ADPropertyDefinition ActivityBasedAuthenticationTimeoutWithSingleSignOnDisabled = ADObject.BitfieldProperty("ActivityBasedAuthenticationTimeoutWithSingleSignOnDisabled", 28, OrganizationSchema.OrganizationFlags);

		public static readonly ADPropertyDefinition MSOSyncEnabled = ADObject.BitfieldProperty("MSOSyncEnabled", 29, OrganizationSchema.OrganizationFlags);

		public static readonly ADPropertyDefinition MaxAddressBookPolicies = new ADPropertyDefinition("MaxAddressBookPolicies", ExchangeObjectVersion.Exchange2003, typeof(int?), "msExchMaxABP", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, int.MaxValue)
		}, null, null);

		public static readonly ADPropertyDefinition MaxOfflineAddressBooks = new ADPropertyDefinition("MaxOfflineAddressBooks", ExchangeObjectVersion.Exchange2003, typeof(int?), "msExchMaxOAB", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, int.MaxValue)
		}, null, null);

		public static readonly ADPropertyDefinition SyncMBXAndDLToMServ = ADObject.BitfieldProperty("SyncMBXAndDLToMServ", 30, OrganizationSchema.OrganizationFlags);

		public static readonly ADPropertyDefinition ShowAdminAccessWarning = new ADPropertyDefinition("ShowAdminAccessWarning", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OrganizationSchema.OrganizationFlags
		}, null, new GetterDelegate(Organization.ShowAdminAccessWarningGetter), null, null, null);

		public static readonly ADPropertyDefinition OrgElcMailboxFlags = new ADPropertyDefinition("ElcMailboxFlags", ExchangeObjectVersion.Exchange2003, typeof(int), "msExchELCMailboxFlags", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition CalendarVersionStoreEnabled = new ADPropertyDefinition("CalendarVersionStoreEnabled", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OrganizationSchema.OrgElcMailboxFlags
		}, null, ADObject.FlagGetterDelegate(4, OrganizationSchema.OrgElcMailboxFlags), ADObject.FlagSetterDelegate(4, OrganizationSchema.OrgElcMailboxFlags), null, null);

		public static readonly ADPropertyDefinition ReadTrackingEnabled = ADObject.BitfieldProperty("ReadTrackingEnabled", 3, OrganizationSchema.OrganizationFlags);

		internal static readonly ADPropertyDefinition SIPAccessService = new ADPropertyDefinition("SIPAccessService", ExchangeObjectVersion.Exchange2003, typeof(ProtocolConnectionSettings), "msExchSIPAccessService", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		internal static readonly ADPropertyDefinition AVAuthenticationService = new ADPropertyDefinition("AVAuthenticationService", ExchangeObjectVersion.Exchange2003, typeof(ProtocolConnectionSettings), "msExchAVAuthenticationService", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		internal static readonly ADPropertyDefinition SIPSessionBorderController = new ADPropertyDefinition("SIPSBCService", ExchangeObjectVersion.Exchange2003, typeof(ProtocolConnectionSettings), "msExchSIPSBCService", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition BuildMajor = ADObject.BitfieldProperty("BuildMajor", 12, 11, OrganizationSchema.OrganizationFlags);

		public static readonly ADPropertyDefinition BuildMinor = ADObject.BitfieldProperty("BuildMinor", 6, 6, OrganizationSchema.OrganizationFlags);

		public static readonly ADPropertyDefinition IsUpgradingOrganization = ADObject.BitfieldProperty("IsUpgradingOrganization", 24, OrganizationSchema.OrganizationFlags);

		public static readonly ADPropertyDefinition IsUpdatingServicePlan = ADObject.BitfieldProperty("IsUpdatingServicePlan", 23, OrganizationSchema.OrganizationFlags);

		public static readonly ADPropertyDefinition IsDehydrated = new ADPropertyDefinition("IsDehydrated", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			SharedPropertyDefinitions.ProvisioningFlags
		}, null, ADObject.FlagGetterDelegate(SharedPropertyDefinitions.ProvisioningFlags, 128), ADObject.FlagSetterDelegate(SharedPropertyDefinitions.ProvisioningFlags, 128), null, null);

		public static readonly ADPropertyDefinition IsStaticConfigurationShared = new ADPropertyDefinition("IsStaticConfigurationShared", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			SharedPropertyDefinitions.ProvisioningFlags
		}, null, ADObject.FlagGetterDelegate(SharedPropertyDefinitions.ProvisioningFlags, 256), ADObject.FlagSetterDelegate(SharedPropertyDefinitions.ProvisioningFlags, 256), null, null);

		public static readonly ADPropertyDefinition HABRootDepartmentLink = new ADPropertyDefinition("HABRootDepartmentLink", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), "msExchHABRootDepartmentLink", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition DistributionGroupDefaultOU = new ADPropertyDefinition("DistributionGroupDefaultOU", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), "msExchDistributionGroupDefaultOU", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DistributionGroupNameBlockedWordsList = new ADPropertyDefinition("DistributionGroupNameBlockedWordsList", ExchangeObjectVersion.Exchange2003, typeof(string), "msExchDistributionGroupNameBlockedWordsList", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DistributionGroupNamingPolicyRaw = new ADPropertyDefinition("DistributionGroupNamingPolicy", ExchangeObjectVersion.Exchange2003, typeof(string), "msExchDistributionGroupNamingPolicy", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DistributionGroupNamingPolicy = new ADPropertyDefinition("DistributionGroupNamingPolicy", ExchangeObjectVersion.Exchange2003, typeof(DistributionGroupNamingPolicy), null, ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OrganizationSchema.DistributionGroupNamingPolicyRaw
		}, null, new GetterDelegate(Organization.DistributionGroupNamingPolicyGetter), new SetterDelegate(Organization.DistributionGroupNamingPolicySetter), null, null);

		public static readonly ADPropertyDefinition ExchangeNotificationEnabled = new ADPropertyDefinition("ExchangeNotificationEnabled", ExchangeObjectVersion.Exchange2003, typeof(bool), "msExchNotificationEnabled", ADPropertyDefinitionFlags.None, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExchangeNotificationRecipients = new ADPropertyDefinition("ExchangeNotificationRecipients", ExchangeObjectVersion.Exchange2003, typeof(SmtpAddress), "msExchNotificationAddress", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new CollectionPropertyMaxCountConstraint(64)
		}, null, null);

		internal static readonly ADPropertyDefinition SupportedSharedConfigurations = new ADPropertyDefinition("SupportedSharedConfigurations", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), "msExchSupportedSharedConfigLink", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.DoNotValidate, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		internal static readonly ADPropertyDefinition SupportedSharedConfigurationsBL = new ADPropertyDefinition("SupportedSharedConfigurationsBL", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), "msExchSupportedSharedConfigBL", ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.FilterOnly, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		internal static readonly ADPropertyDefinition SharedConfigurationInfo = new ADPropertyDefinition("SharedConfigurationInfo", ExchangeObjectVersion.Exchange2003, typeof(SharedConfigurationInfo), "msExchSharedConfigServicePlanTag", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MigrationFlags = new ADPropertyDefinition("MigrationFlags", ExchangeObjectVersion.Exchange2003, typeof(int), "msExchMigrationFlags", ADPropertyDefinitionFlags.PersistDefaultValue | ADPropertyDefinitionFlags.DoNotProvisionalClone, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition IsExcludedFromOnboardMigration = new ADPropertyDefinition("IsExcludedFromOnboardMigration", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OrganizationSchema.MigrationFlags
		}, (SinglePropertyFilter filter) => ADObject.BoolFilterBuilder(filter, new BitMaskAndFilter(OrganizationSchema.MigrationFlags, 1UL)), ADObject.FlagGetterDelegate(OrganizationSchema.MigrationFlags, 1), ADObject.FlagSetterDelegate(OrganizationSchema.MigrationFlags, 1), null, null);

		public static readonly ADPropertyDefinition IsExcludedFromOffboardMigration = new ADPropertyDefinition("IsExcludedFromOffboardMigration", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OrganizationSchema.MigrationFlags
		}, (SinglePropertyFilter filter) => ADObject.BoolFilterBuilder(filter, new BitMaskAndFilter(OrganizationSchema.MigrationFlags, 2UL)), ADObject.FlagGetterDelegate(OrganizationSchema.MigrationFlags, 2), ADObject.FlagSetterDelegate(OrganizationSchema.MigrationFlags, 2), null, null);

		public static readonly ADPropertyDefinition IsFfoMigrationInProgress = new ADPropertyDefinition("IsFfoMigrationInProgress", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OrganizationSchema.MigrationFlags
		}, (SinglePropertyFilter filter) => ADObject.BoolFilterBuilder(filter, new BitMaskAndFilter(OrganizationSchema.MigrationFlags, 4UL)), ADObject.FlagGetterDelegate(OrganizationSchema.MigrationFlags, 4), ADObject.FlagSetterDelegate(OrganizationSchema.MigrationFlags, 4), null, null);

		public static readonly ADPropertyDefinition MaxConcurrentMigrations = new ADPropertyDefinition("MaxConcurrentMigrations", ExchangeObjectVersion.Exchange2003, typeof(Unlimited<int>), "msExchMaxConcurrentMigrations", ADPropertyDefinitionFlags.None, Unlimited<int>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<int>(0, 1000)
		}, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<int>(0, 1000)
		}, null, null);

		public static readonly ADPropertyDefinition TenantRelocationsAllowed = new ADPropertyDefinition("TenantRelocationsAllowed", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OrganizationSchema.OrganizationFlags2
		}, (SinglePropertyFilter filter) => ADObject.BoolFilterBuilder(filter, new BitMaskAndFilter(OrganizationSchema.OrganizationFlags2, 1UL)), ADObject.FlagGetterDelegate(OrganizationSchema.OrganizationFlags2, 1), ADObject.FlagSetterDelegate(OrganizationSchema.OrganizationFlags2, 1), null, null);

		public static readonly ADPropertyDefinition ACLableSyncedObjectEnabled = new ADPropertyDefinition("ACLableSyncedObjectEnabled", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OrganizationSchema.OrganizationFlags2
		}, (SinglePropertyFilter filter) => ADObject.BoolFilterBuilder(filter, new BitMaskAndFilter(OrganizationSchema.OrganizationFlags2, 2097152UL)), ADObject.FlagGetterDelegate(OrganizationSchema.OrganizationFlags2, 2097152), ADObject.FlagSetterDelegate(OrganizationSchema.OrganizationFlags2, 2097152), null, null);

		public static readonly ADPropertyDefinition OpenTenantFull = new ADPropertyDefinition("OpenTenantFull", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OrganizationSchema.OrganizationFlags2
		}, (SinglePropertyFilter filter) => ADObject.BoolFilterBuilder(filter, new BitMaskAndFilter(OrganizationSchema.OrganizationFlags2, 64UL)), ADObject.FlagGetterDelegate(OrganizationSchema.OrganizationFlags2, 64), ADObject.FlagSetterDelegate(OrganizationSchema.OrganizationFlags2, 64), null, null);

		public static readonly ADPropertyDefinition ProvisioningFlags = SharedPropertyDefinitions.ProvisioningFlags;

		public static readonly ADPropertyDefinition EnableAsSharedConfiguration = new ADPropertyDefinition("EnableAsSharedConfiguration", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OrganizationSchema.ProvisioningFlags
		}, new CustomFilterBuilderDelegate(Organization.EnableAsSharedConfigurationFilterBuilder), ADObject.FlagGetterDelegate(OrganizationSchema.ProvisioningFlags, 1), ADObject.FlagSetterDelegate(OrganizationSchema.ProvisioningFlags, 1), null, null);

		public static readonly ADPropertyDefinition ImmutableConfiguration = new ADPropertyDefinition("ImmutableConfiguration", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OrganizationSchema.ProvisioningFlags
		}, new CustomFilterBuilderDelegate(Organization.ImmutableConfigurationFilterBuilder), ADObject.FlagGetterDelegate(OrganizationSchema.ProvisioningFlags, 1024), ADObject.FlagSetterDelegate(OrganizationSchema.ProvisioningFlags, 1024), null, null);

		public static readonly ADPropertyDefinition HostingDeploymentEnabled = new ADPropertyDefinition("MultiTenantADEnabled", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OrganizationSchema.ProvisioningFlags
		}, new CustomFilterBuilderDelegate(Organization.HostingDeploymentEnabledFilterBuilder), ADObject.FlagGetterDelegate(OrganizationSchema.ProvisioningFlags, 32), ADObject.FlagSetterDelegate(OrganizationSchema.ProvisioningFlags, 32), null, null);

		public static readonly ADPropertyDefinition IsSharingConfiguration = new ADPropertyDefinition("IsSharingConfiguration", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.TaskPopulated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition IsLicensingEnforced = new ADPropertyDefinition("IsLicensingEnforced", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OrganizationSchema.ProvisioningFlags
		}, new CustomFilterBuilderDelegate(Organization.LicensingEnforcedFilterBuilder), ADObject.FlagGetterDelegate(OrganizationSchema.ProvisioningFlags, 2), ADObject.FlagSetterDelegate(OrganizationSchema.ProvisioningFlags, 2), null, null);

		public static readonly ADPropertyDefinition IsTenantAccessBlocked = new ADPropertyDefinition("IsTenantAccessBlocked", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OrganizationSchema.ProvisioningFlags
		}, new CustomFilterBuilderDelegate(Organization.IsTenantAccessBlockedFilterBuilder), ADObject.FlagGetterDelegate(OrganizationSchema.ProvisioningFlags, 4194304), ADObject.FlagSetterDelegate(OrganizationSchema.ProvisioningFlags, 4194304), null, null);

		public static readonly ADPropertyDefinition IsUpgradeOperationInProgress = new ADPropertyDefinition("IsUpgradeOperationInProgress", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OrganizationSchema.OrganizationFlags2
		}, (SinglePropertyFilter filter) => ADObject.BoolFilterBuilder(filter, new BitMaskAndFilter(OrganizationSchema.OrganizationFlags2, 2UL)), ADObject.FlagGetterDelegate(OrganizationSchema.OrganizationFlags2, 2), ADObject.FlagSetterDelegate(OrganizationSchema.OrganizationFlags2, 2), null, null);

		public static readonly ADPropertyDefinition UseServicePlanAsCounterInstanceName = new ADPropertyDefinition("UseServicePlanAsCounterInstanceName", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OrganizationSchema.ProvisioningFlags
		}, (SinglePropertyFilter filter) => ADObject.BoolFilterBuilder(filter, new BitMaskAndFilter(OrganizationSchema.ProvisioningFlags, 64UL)), ADObject.FlagGetterDelegate(OrganizationSchema.ProvisioningFlags, 64), ADObject.FlagSetterDelegate(OrganizationSchema.ProvisioningFlags, 64), null, null);

		public static readonly ADPropertyDefinition IsTemplateTenant = new ADPropertyDefinition("IsTemplateTenant", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OrganizationSchema.OrganizationFlags2
		}, (SinglePropertyFilter filter) => ADObject.BoolFilterBuilder(filter, new BitMaskAndFilter(OrganizationSchema.OrganizationFlags2, 2048UL)), ADObject.FlagGetterDelegate(OrganizationSchema.OrganizationFlags2, 2048), ADObject.FlagSetterDelegate(OrganizationSchema.OrganizationFlags2, 2048), null, null);

		public static readonly ADPropertyDefinition ExcludedFromBackSync = new ADPropertyDefinition("ExcludedFromBackSync", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OrganizationSchema.ProvisioningFlags
		}, (SinglePropertyFilter filter) => ADObject.BoolFilterBuilder(filter, new BitMaskAndFilter(OrganizationSchema.ProvisioningFlags, 4UL)), ADObject.FlagGetterDelegate(OrganizationSchema.ProvisioningFlags, 4), ADObject.FlagSetterDelegate(OrganizationSchema.ProvisioningFlags, 4), null, null);

		public static readonly ADPropertyDefinition ExcludedFromForwardSyncEDU2BPOS = new ADPropertyDefinition("ExcludedFromForwardSyncEDU2BPOS", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OrganizationSchema.ProvisioningFlags
		}, (SinglePropertyFilter filter) => ADObject.BoolFilterBuilder(filter, new BitMaskAndFilter(OrganizationSchema.ProvisioningFlags, 2048UL)), ADObject.FlagGetterDelegate(OrganizationSchema.ProvisioningFlags, 2048), ADObject.FlagSetterDelegate(OrganizationSchema.ProvisioningFlags, 2048), null, null);

		public static readonly ADPropertyDefinition AllowDeleteOfExternalIdentityUponRemove = new ADPropertyDefinition("AllowDeleteOfExternalIdentityUponRemove", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OrganizationSchema.ProvisioningFlags
		}, (SinglePropertyFilter filter) => ADObject.BoolFilterBuilder(filter, new BitMaskAndFilter(OrganizationSchema.ProvisioningFlags, 16UL)), ADObject.FlagGetterDelegate(OrganizationSchema.ProvisioningFlags, 16), ADObject.FlagSetterDelegate(OrganizationSchema.ProvisioningFlags, 16), null, null);

		public static readonly ADPropertyDefinition AppsForOfficeDisabled = new ADPropertyDefinition("OwaExtensibilityDisabled", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OrganizationSchema.ProvisioningFlags
		}, (SinglePropertyFilter filter) => ADObject.BoolFilterBuilder(filter, new BitMaskAndFilter(OrganizationSchema.ProvisioningFlags, 512UL)), ADObject.FlagGetterDelegate(OrganizationSchema.ProvisioningFlags, 512), ADObject.FlagSetterDelegate(OrganizationSchema.ProvisioningFlags, 512), null, null);

		public static readonly ADPropertyDefinition SoftDeletedFeatureStatus = ADObject.BitfieldProperty("SoftDeletedFeatureStatus", 13, 3, OrganizationSchema.ProvisioningFlags);

		public static readonly ADPropertyDefinition IsGuidPrefixedLegacyDnDisabled = ADObject.BitfieldProperty("IsGuidPrefixedLegacyDnDisabled", 16, OrganizationSchema.ProvisioningFlags);

		public static readonly ADPropertyDefinition IsMailboxForcedReplicationDisabled = ADObject.BitfieldProperty("IsMailboxForcedReplicationDisabled", 17, OrganizationSchema.ProvisioningFlags);

		public static readonly ADPropertyDefinition IsPilotingOrganization = ADObject.BitfieldProperty("IsPilotingOrganization", 20, OrganizationSchema.ProvisioningFlags);

		public static readonly ADPropertyDefinition IsSyncPropertySetUpgradeAllowed = ADObject.BitfieldProperty("IsSyncPropertySetUpgradeAllowed", 18, OrganizationSchema.ProvisioningFlags);

		public static readonly ADPropertyDefinition IsProcessEhaMigratedMessagesEnabled = ADObject.BitfieldProperty("IsProcessEhaMigratedMessagesEnabled", 19, OrganizationSchema.ProvisioningFlags);

		public static readonly ADPropertyDefinition IsDirSyncRunning = new ADPropertyDefinition("IsDirSyncRunning", ExchangeObjectVersion.Exchange2003, typeof(bool), "msExchIsMSODirsyncEnabled", ADPropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DirSyncStatus = new ADPropertyDefinition("DirSyncStatus", ExchangeObjectVersion.Exchange2003, typeof(string), "msExchDirsyncStatus", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition CompanyTags = new ADPropertyDefinition("CompanyTags", ExchangeObjectVersion.Exchange2003, typeof(string), "msExchProvisioningTags", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition Location = new ADPropertyDefinition("Location", ExchangeObjectVersion.Exchange2003, typeof(string), "msExchTenantCountry", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition PersistedCapabilities = new ADPropertyDefinition("PersistedCapabilities", ExchangeObjectVersion.Exchange2003, typeof(Capability), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, new PropertyDefinitionConstraint[]
		{
			new CollectionDelegateConstraint(new CollectionValidationDelegate(ConstraintDelegates.ValidateOrganizationCapabilities))
		}, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			SharedPropertyDefinitions.RawCapabilities
		}, new CustomFilterBuilderDelegate(SharedPropertyDefinitions.CapabilitiesFilterBuilder), new GetterDelegate(SharedPropertyDefinitions.PersistedCapabilitiesGetter), new SetterDelegate(SharedPropertyDefinitions.PersistedCapabilitiesSetter), null, null);

		public static readonly ADPropertyDefinition IsDirSyncStatusPending = new ADPropertyDefinition("IsDirSyncStatusPending", ExchangeObjectVersion.Exchange2003, typeof(bool), "msExchIsDirsyncStatusPending", ADPropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition EwsEnabled = new ADPropertyDefinition("EwsEnabled", ExchangeObjectVersion.Exchange2003, typeof(int?), "msExchEwsEnabled", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition EwsWellKnownApplicationAccessPolicies = new ADPropertyDefinition("EwsWellKnownApplicationAccessPolicies", ExchangeObjectVersion.Exchange2003, typeof(string), "msExchEwsWellKnownApplicationPolicies", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition EwsApplicationAccessPolicy = new ADPropertyDefinition("EwsApplicationAccessPolicy", ExchangeObjectVersion.Exchange2003, typeof(EwsApplicationAccessPolicy?), "msExchEwsApplicationAccessPolicy", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(EwsApplicationAccessPolicy))
		}, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition EwsExceptions = new ADPropertyDefinition("EwsExceptions", ExchangeObjectVersion.Exchange2003, typeof(string), "msExchEwsExceptions", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition EwsAllowOutlook = new ADPropertyDefinition("EwsAllowOutlook", ExchangeObjectVersion.Exchange2003, typeof(bool?), null, ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OrganizationSchema.EwsWellKnownApplicationAccessPolicies
		}, null, CASMailboxHelper.EwsOutlookAccessPoliciesGetterDelegate(), CASMailboxHelper.EwsOutlookAccessPoliciesSetterDelegate(), null, null);

		public static readonly ADPropertyDefinition EwsAllowMacOutlook = new ADPropertyDefinition("EwsAllowMacOutlook", ExchangeObjectVersion.Exchange2003, typeof(bool?), null, ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OrganizationSchema.EwsWellKnownApplicationAccessPolicies
		}, null, CASMailboxHelper.EwsMacOutlookAccessPoliciesGetterDelegate(), CASMailboxHelper.EwsMacOutlookAccessPoliciesSetterDelegate(), null, null);

		public static readonly ADPropertyDefinition EwsAllowEntourage = new ADPropertyDefinition("EwsAllowEntourage", ExchangeObjectVersion.Exchange2003, typeof(bool?), null, ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OrganizationSchema.EwsWellKnownApplicationAccessPolicies
		}, null, CASMailboxHelper.EwsEntourageAccessPoliciesGetterDelegate(), CASMailboxHelper.EwsEntourageAccessPoliciesSetterDelegate(), null, null);

		public static readonly ADPropertyDefinition AsynchronousOperationIds = new ADPropertyDefinition("AsynchronousOperationIds", ExchangeObjectVersion.Exchange2003, typeof(string), "msExchMSOForwardSyncAsyncOperationIds", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RBACConfigurationVersion = new ADPropertyDefinition("RBACConfigurationVersion", ExchangeObjectVersion.Exchange2003, typeof(ExchangeObjectVersion), null, ADPropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AdminDisplayVersion = new ADPropertyDefinition("AdminDisplayVersion", ExchangeObjectVersion.Exchange2003, typeof(ExchangeObjectVersion), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OrganizationSchema.OrganizationFlags,
			OrganizationSchema.ObjectVersion
		}, null, new GetterDelegate(Organization.AdminDisplayVersionGetter), null, null, null);

		public static readonly ADPropertyDefinition DefaultPublicFolderMailbox = new ADPropertyDefinition("DefaultPublicFolderMailbox", ExchangeObjectVersion.Exchange2003, typeof(PublicFolderInformation), "msExchDefaultPublicFolderMailbox", ADPropertyDefinitionFlags.None, PublicFolderInformation.InvalidPublicFolderInformation, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RemotePublicFolderMailboxes = new ADPropertyDefinition("RemotePublicFolderMailboxes", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), "pFContacts", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.DoNotValidate, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ForestMode = new ADPropertyDefinition("ForestMode", ExchangeObjectVersion.Exchange2003, typeof(ForestModeFlags), "msExchForestModeFlag", ADPropertyDefinitionFlags.None, ForestModeFlags.Legacy, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(ForestModeFlags))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition UMAvailableLanguagesRaw = new ADPropertyDefinition("UMAvailableLanguagesRaw", ExchangeObjectVersion.Exchange2003, typeof(int), "msExchUMAvailableLanguages", ADPropertyDefinitionFlags.MultiValued, null, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, 1048576)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition UMAvailableLanguages = new ADPropertyDefinition("UMAvailableLanguages", ExchangeObjectVersion.Exchange2003, typeof(UMLanguage), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OrganizationSchema.UMAvailableLanguagesRaw
		}, null, new GetterDelegate(Organization.UMAvailableLanguagesGetter), new SetterDelegate(Organization.UMAvailableLanguagesSetter), null, null);

		public static readonly ADPropertyDefinition AdfsAuthenticationRawConfiguration = new ADPropertyDefinition("AdfsAuthenticationRawConfiguration", ExchangeObjectVersion.Exchange2003, typeof(string), "msExchAdfsAuthenticationRawConfiguration", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(0, 10240)
		}, null, null);

		public static readonly ADPropertyDefinition AdfsIssuer = new ADPropertyDefinition("AdfsIssuer", ExchangeObjectVersion.Exchange2003, typeof(Uri), null, ADPropertyDefinitionFlags.Calculated, null, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, new ProviderPropertyDefinition[]
		{
			OrganizationSchema.AdfsAuthenticationRawConfiguration
		}, null, new GetterDelegate(Organization.AdfsIssuerGetter), new SetterDelegate(Organization.AdfsIssuerSetter), null, null);

		public static readonly ADPropertyDefinition AdfsAudienceUris = new ADPropertyDefinition("AdfsAudienceUris", ExchangeObjectVersion.Exchange2003, typeof(Uri), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OrganizationSchema.AdfsAuthenticationRawConfiguration
		}, null, new GetterDelegate(Organization.AdfsAudienceUrisGetter), new SetterDelegate(Organization.AdfsAudienceUrisSetter), null, null);

		public static readonly ADPropertyDefinition AdfsSignCertificateThumbprints = new ADPropertyDefinition("AdfsSignCertificateThumbprints", ExchangeObjectVersion.Exchange2003, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OrganizationSchema.AdfsAuthenticationRawConfiguration
		}, null, new GetterDelegate(Organization.AdfsSignCertificateThumbprintsGetter), new SetterDelegate(Organization.AdfsSignCertificateThumbprintsSetter), null, null);

		public static readonly ADPropertyDefinition AdfsEncryptCertificateThumbprint = new ADPropertyDefinition("AdfsEncryptCertificateThumbprint", ExchangeObjectVersion.Exchange2003, typeof(string), null, ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OrganizationSchema.AdfsAuthenticationRawConfiguration
		}, null, new GetterDelegate(Organization.AdfsEncryptCertificateThumbprintGetter), new SetterDelegate(Organization.AdfsEncryptCertificateThumbprintSetter), null, null);

		public static readonly ADPropertyDefinition ConfigurationXMLRaw = XMLSerializableBase.ConfigurationXmlRawProperty();

		public static readonly ADPropertyDefinition ConfigurationXML = XMLSerializableBase.ConfigurationXmlProperty<OrganizationConfigXML>(OrganizationSchema.ConfigurationXMLRaw);

		public static readonly ADPropertyDefinition UpgradeStatus = new ADPropertyDefinition("UpgradeStatus", ExchangeObjectVersion.Exchange2003, typeof(UpgradeStatusTypes), "msExchOrganizationUpgradeStatus", ADPropertyDefinitionFlags.None, UpgradeStatusTypes.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition UpgradeRequest = new ADPropertyDefinition("UpgradeRequest", ExchangeObjectVersion.Exchange2003, typeof(UpgradeRequestTypes), "msExchOrganizationUpgradeRequest", ADPropertyDefinitionFlags.None, UpgradeRequestTypes.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition WACDiscoveryEndpoint = new ADPropertyDefinition("WACDiscoveryEndpoint", ExchangeObjectVersion.Exchange2003, typeof(string), "msExchWACDiscoveryEndpoint", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DefaultMovePriority = XMLSerializableBase.ConfigXmlProperty<OrganizationConfigXML, int>("DefaultMovePriority", ExchangeObjectVersion.Exchange2003, OrganizationSchema.ConfigurationXML, 0, (OrganizationConfigXML configXml) => configXml.DefaultMovePriority, delegate(OrganizationConfigXML configXml, int value)
		{
			configXml.DefaultMovePriority = value;
		}, null, null);

		public static readonly ADPropertyDefinition UpgradeMessage = XMLSerializableBase.ConfigXmlProperty<OrganizationConfigXML, string>("UpgradeMessage", ExchangeObjectVersion.Exchange2003, OrganizationSchema.ConfigurationXML, null, (OrganizationConfigXML configXml) => configXml.UpgradeMessage, delegate(OrganizationConfigXML configXml, string value)
		{
			configXml.UpgradeMessage = value;
		}, null, null);

		public static readonly ADPropertyDefinition UpgradeDetails = XMLSerializableBase.ConfigXmlProperty<OrganizationConfigXML, string>("UpgradeDetails", ExchangeObjectVersion.Exchange2003, OrganizationSchema.ConfigurationXML, null, (OrganizationConfigXML configXml) => configXml.UpgradeDetails, delegate(OrganizationConfigXML configXml, string value)
		{
			configXml.UpgradeDetails = value;
		}, null, null);

		public static readonly ADPropertyDefinition UpgradeConstraints = XMLSerializableBase.ConfigXmlProperty<OrganizationConfigXML, UpgradeConstraintArray>("UpgradeConstraints", ExchangeObjectVersion.Exchange2003, OrganizationSchema.ConfigurationXML, null, (OrganizationConfigXML configXml) => configXml.UpgradeConstraints, delegate(OrganizationConfigXML configXml, UpgradeConstraintArray value)
		{
			configXml.UpgradeConstraints = value;
		}, null, null);

		public static readonly ADPropertyDefinition UpgradeStage = XMLSerializableBase.ConfigXmlProperty<OrganizationConfigXML, UpgradeStage?>("UpgradeStage", ExchangeObjectVersion.Exchange2003, OrganizationSchema.ConfigurationXML, null, (OrganizationConfigXML configXml) => configXml.UpgradeStage, delegate(OrganizationConfigXML configXml, UpgradeStage? value)
		{
			configXml.UpgradeStage = value;
		}, null, null);

		public static readonly ADPropertyDefinition UpgradeStageTimeStamp = XMLSerializableBase.ConfigXmlProperty<OrganizationConfigXML, DateTime?>("UpgradeStageTimeStamp", ExchangeObjectVersion.Exchange2003, OrganizationSchema.ConfigurationXML, null, (OrganizationConfigXML configXml) => configXml.UpgradeStageTimeStamp, delegate(OrganizationConfigXML configXml, DateTime? value)
		{
			configXml.UpgradeStageTimeStamp = value;
		}, null, null);

		public static readonly ADPropertyDefinition UpgradeE14RequestCountForCurrentStage = XMLSerializableBase.ConfigXmlProperty<OrganizationConfigXML, int?>("UpgradeE14RequestCountForCurrentStage", ExchangeObjectVersion.Exchange2003, OrganizationSchema.ConfigurationXML, null, (OrganizationConfigXML configXml) => configXml.UpgradeE14RequestCountForCurrentStage, delegate(OrganizationConfigXML configXml, int? value)
		{
			configXml.UpgradeE14RequestCountForCurrentStage = value;
		}, null, null);

		public static readonly ADPropertyDefinition UpgradeE14MbxCountForCurrentStage = XMLSerializableBase.ConfigXmlProperty<OrganizationConfigXML, int?>("UpgradeE14MbxCountForCurrentStage", ExchangeObjectVersion.Exchange2003, OrganizationSchema.ConfigurationXML, null, (OrganizationConfigXML configXml) => configXml.UpgradeE14MbxCountForCurrentStage, delegate(OrganizationConfigXML configXml, int? value)
		{
			configXml.UpgradeE14MbxCountForCurrentStage = value;
		}, null, null);

		public static readonly ADPropertyDefinition UpgradeUnitsOverride = XMLSerializableBase.ConfigXmlProperty<OrganizationConfigXML, int?>("UpgradeUnitsOverride", ExchangeObjectVersion.Exchange2003, OrganizationSchema.ConfigurationXML, null, (OrganizationConfigXML configXml) => configXml.UpgradeUnitsOverride, delegate(OrganizationConfigXML configXml, int? value)
		{
			configXml.UpgradeUnitsOverride = value;
		}, null, null);

		public static readonly ADPropertyDefinition UpgradeConstraintsDisabled = XMLSerializableBase.ConfigXmlProperty<OrganizationConfigXML, bool?>("UpgradeConstraintsDisabled", ExchangeObjectVersion.Exchange2003, OrganizationSchema.ConfigurationXML, null, (OrganizationConfigXML configXml) => configXml.UpgradeConstraintsDisabled, delegate(OrganizationConfigXML configXml, bool? value)
		{
			configXml.UpgradeConstraintsDisabled = value;
		}, null, null);

		public static readonly ADPropertyDefinition UpgradeLastE14CountsUpdateTime = XMLSerializableBase.ConfigXmlProperty<OrganizationConfigXML, DateTime?>("UpgradeLastE14CountsUpdateTime", ExchangeObjectVersion.Exchange2003, OrganizationSchema.ConfigurationXML, null, (OrganizationConfigXML configXml) => configXml.UpgradeLastE14CountsUpdateTime, delegate(OrganizationConfigXML configXml, DateTime? value)
		{
			configXml.UpgradeLastE14CountsUpdateTime = value;
		}, null, null);

		public static readonly ADPropertyDefinition MailboxRelease = SharedPropertyDefinitions.MailboxRelease;

		public static readonly ADPropertyDefinition PreviousMailboxRelease = XMLSerializableBase.ConfigXmlProperty<OrganizationConfigXML, string>("PreviousMailboxRelease", ExchangeObjectVersion.Exchange2003, OrganizationSchema.ConfigurationXML, null, (OrganizationConfigXML configXml) => configXml.PreviousMailboxRelease, delegate(OrganizationConfigXML configXml, string value)
		{
			configXml.PreviousMailboxRelease = value;
		}, null, null);

		public static readonly ADPropertyDefinition PilotMailboxRelease = XMLSerializableBase.ConfigXmlProperty<OrganizationConfigXML, string>("PilotMailboxRelease", ExchangeObjectVersion.Exchange2003, OrganizationSchema.ConfigurationXML, null, (OrganizationConfigXML configXml) => configXml.PilotMailboxRelease, delegate(OrganizationConfigXML configXml, string value)
		{
			configXml.PilotMailboxRelease = value;
		}, null, null);

		public static readonly ADPropertyDefinition PersistedRelocationConstraints = XMLSerializableBase.ConfigXmlProperty<OrganizationConfigXML, RelocationConstraintArray>("PersistedRelocationConstraints", ExchangeObjectVersion.Exchange2003, OrganizationSchema.ConfigurationXML, null, (OrganizationConfigXML configXml) => configXml.RelocationConstraints, delegate(OrganizationConfigXML configXml, RelocationConstraintArray value)
		{
			configXml.RelocationConstraints = value;
		}, null, null);

		public static readonly ADPropertyDefinition RelocationConstraints = new ADPropertyDefinition("RelocationConstraints", ExchangeObjectVersion.Exchange2003, typeof(RelocationConstraint), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition OriginatedInDifferentForest = new ADPropertyDefinition("OriginatedInDifferentForest", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADObjectSchema.CorrelationIdRaw
		}, null, new GetterDelegate(Organization.OriginatedInDifferentForestGetter), null, null, null);

		public static readonly ADPropertyDefinition ReleaseTrack = XMLSerializableBase.ConfigXmlProperty<OrganizationConfigXML, ReleaseTrack?>("ReleaseTrack", ExchangeObjectVersion.Exchange2003, OrganizationSchema.ConfigurationXML, null, (OrganizationConfigXML configXml) => configXml.ReleaseTrack, delegate(OrganizationConfigXML configXml, ReleaseTrack? value)
		{
			configXml.ReleaseTrack = value;
		}, null, null);

		public static readonly ADPropertyDefinition PublicComputersDetectionEnabled = new ADPropertyDefinition("PublicComputersDetectionEnabled", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			OrganizationSchema.OrganizationFlags2
		}, (SinglePropertyFilter filter) => ADObject.BoolFilterBuilder(filter, new BitMaskAndFilter(OrganizationSchema.OrganizationFlags2, 32UL)), ADObject.FlagGetterDelegate(OrganizationSchema.OrganizationFlags2, 32), ADObject.FlagSetterDelegate(OrganizationSchema.OrganizationFlags2, 32), null, null);

		public static readonly ADPropertyDefinition RmsoSubscriptionStatus = ADObject.BitfieldProperty("RmsoSubscriptionStatus", 7, 3, OrganizationSchema.OrganizationFlags2);

		public static readonly ADPropertyDefinition IntuneManagedStatus = ADObject.BitfieldProperty("IntuneManagedStatus", 12, OrganizationSchema.OrganizationFlags2);

		public static readonly ADPropertyDefinition MapiHttpEnabled = ADObject.BitfieldProperty("MapiHttpEnabled", 10, OrganizationSchema.OrganizationFlags2);

		public static readonly ADPropertyDefinition HybridConfigurationStatus = ADObject.BitfieldProperty("HybridConfigurationStatus", 13, 4, OrganizationSchema.OrganizationFlags2);

		public static readonly ADPropertyDefinition OAuth2ClientProfileEnabled = ADObject.BitfieldProperty("OAuth2ClientProfileEnabled", 18, OrganizationSchema.OrganizationFlags2);
	}
}
