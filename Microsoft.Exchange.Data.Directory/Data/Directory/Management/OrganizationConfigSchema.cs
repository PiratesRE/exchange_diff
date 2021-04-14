using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal sealed class OrganizationConfigSchema : ADPresentationSchema
	{
		internal override ADObjectSchema GetParentSchema()
		{
			return ObjectSchema.GetInstance<ADOrganizationConfigSchema>();
		}

		public new static readonly ADPropertyDefinition OrganizationId = ADObjectSchema.OrganizationId;

		public static readonly ADPropertyDefinition AdminDisplayName = ADConfigurationObjectSchema.AdminDisplayName;

		public static readonly ADPropertyDefinition LegacyExchangeDN = OrganizationSchema.LegacyExchangeDN;

		public static readonly ADPropertyDefinition Heuristics = OrganizationSchema.Heuristics;

		public static readonly ADPropertyDefinition ResourceAddressLists = OrganizationSchema.ResourceAddressLists;

		public static readonly ADPropertyDefinition IsMixedMode = OrganizationSchema.IsMixedMode;

		public static readonly ADPropertyDefinition IsAddressListPagingEnabled = OrganizationSchema.IsAddressListPagingEnabled;

		public static readonly ADPropertyDefinition ManagedFolderHomepage = OrganizationSchema.ManagedFolderHomepage;

		public static readonly ADPropertyDefinition ForeignForestFQDN = OrganizationSchema.ForeignForestFQDN;

		public static readonly ADPropertyDefinition ForeignForestOrgAdminUSGSid = OrganizationSchema.ForeignForestOrgAdminUSGSid;

		public static readonly ADPropertyDefinition ForeignForestRecipientAdminUSGSid = OrganizationSchema.ForeignForestRecipientAdminUSGSid;

		public static readonly ADPropertyDefinition ForeignForestViewOnlyAdminUSGSid = OrganizationSchema.ForeignForestViewOnlyAdminUSGSid;

		public static readonly ADPropertyDefinition ObjectVersion = OrganizationSchema.ObjectVersion;

		public static readonly ADPropertyDefinition SCLJunkThreshold = OrganizationSchema.SCLJunkThreshold;

		public static readonly ADPropertyDefinition AcceptedDomainNames = OrganizationSchema.AcceptedDomainNames;

		public static readonly ADPropertyDefinition MimeTypes = OrganizationSchema.MimeTypes;

		public static readonly ADPropertyDefinition MicrosoftExchangeRecipientEmailAddresses = OrganizationSchema.MicrosoftExchangeRecipientEmailAddresses;

		public static readonly ADPropertyDefinition MicrosoftExchangeRecipientReplyRecipient = OrganizationSchema.MicrosoftExchangeRecipientReplyRecipient;

		public static readonly ADPropertyDefinition MicrosoftExchangeRecipientPrimarySmtpAddress = OrganizationSchema.MicrosoftExchangeRecipientPrimarySmtpAddress;

		public static readonly ADPropertyDefinition MicrosoftExchangeRecipientEmailAddressPolicyEnabled = OrganizationSchema.MicrosoftExchangeRecipientEmailAddressPolicyEnabled;

		public static readonly ADPropertyDefinition Industry = OrganizationSchema.Industry;

		public static readonly ADPropertyDefinition CustomerFeedbackEnabled = OrganizationSchema.CustomerFeedbackEnabled;

		public static readonly ADPropertyDefinition OrganizationSummary = OrganizationSchema.OrganizationSummary;

		public static readonly ADPropertyDefinition MailTipsExternalRecipientsTipsEnabled = OrganizationSchema.MailTipsExternalRecipientsTipsEnabled;

		public static readonly ADPropertyDefinition MailTipsLargeAudienceThreshold = OrganizationSchema.MailTipsLargeAudienceThreshold;

		public static readonly ADPropertyDefinition MailTipsMailboxSourcedTipsEnabled = OrganizationSchema.MailTipsMailboxSourcedTipsEnabled;

		public static readonly ADPropertyDefinition MailTipsGroupMetricsEnabled = OrganizationSchema.MailTipsGroupMetricsEnabled;

		public static readonly ADPropertyDefinition MailTipsAllTipsEnabled = OrganizationSchema.MailTipsAllTipsEnabled;

		public static readonly ADPropertyDefinition ReadTrackingEnabled = OrganizationSchema.ReadTrackingEnabled;

		public static readonly ADPropertyDefinition DistributionGroupDefaultOU = OrganizationSchema.DistributionGroupDefaultOU;

		public static readonly ADPropertyDefinition DistributionGroupNameBlockedWordsList = OrganizationSchema.DistributionGroupNameBlockedWordsList;

		public static readonly ADPropertyDefinition DistributionGroupNamingPolicy = OrganizationSchema.DistributionGroupNamingPolicy;

		public static readonly ADPropertyDefinition ForwardSyncLiveIdBusinessInstance = OrganizationSchema.ForwardSyncLiveIdBusinessInstance;

		public static readonly ADPropertyDefinition ExchangeNotificationEnabled = OrganizationSchema.ExchangeNotificationEnabled;

		public static readonly ADPropertyDefinition ExchangeNotificationRecipients = OrganizationSchema.ExchangeNotificationRecipients;

		public static readonly ADPropertyDefinition EwsEnabled = OrganizationSchema.EwsEnabled;

		public static readonly ADPropertyDefinition EwsAllowOutlook = OrganizationSchema.EwsAllowOutlook;

		public static readonly ADPropertyDefinition EwsAllowMacOutlook = OrganizationSchema.EwsAllowMacOutlook;

		public static readonly ADPropertyDefinition EwsAllowEntourage = OrganizationSchema.EwsAllowEntourage;

		public static readonly ADPropertyDefinition EwsApplicationAccessPolicy = OrganizationSchema.EwsApplicationAccessPolicy;

		public static readonly ADPropertyDefinition ActivityBasedAuthenticationTimeoutInterval = OrganizationSchema.ActivityBasedAuthenticationTimeoutInterval;

		public static readonly ADPropertyDefinition ActivityBasedAuthenticationTimeoutDisabled = OrganizationSchema.ActivityBasedAuthenticationTimeoutDisabled;

		public static readonly ADPropertyDefinition ActivityBasedAuthenticationTimeoutWithSingleSignOnDisabled = OrganizationSchema.ActivityBasedAuthenticationTimeoutWithSingleSignOnDisabled;

		public static readonly ADPropertyDefinition AppsForOfficeDisabled = OrganizationSchema.AppsForOfficeDisabled;

		public static readonly ADPropertyDefinition IsLicensingEnforced = OrganizationSchema.IsLicensingEnforced;

		public static readonly ADPropertyDefinition IsTenantAccessBlocked = OrganizationSchema.IsTenantAccessBlocked;

		public static readonly ADPropertyDefinition IsDehydrated = OrganizationSchema.IsDehydrated;

		public static readonly ADPropertyDefinition RBACConfigurationVersion = OrganizationSchema.RBACConfigurationVersion;

		public static readonly ADPropertyDefinition HABRootDepartmentLink = OrganizationSchema.HABRootDepartmentLink;

		public static readonly ADPropertyDefinition EwsExceptions = OrganizationSchema.EwsExceptions;

		public static readonly ADPropertyDefinition AVAuthenticationService = OrganizationSchema.AVAuthenticationService;

		public static readonly ADPropertyDefinition SIPAccessService = OrganizationSchema.SIPAccessService;

		public static readonly ADPropertyDefinition SIPSessionBorderController = OrganizationSchema.SIPSessionBorderController;

		public static readonly ADPropertyDefinition IsGuidPrefixedLegacyDnDisabled = OrganizationSchema.IsGuidPrefixedLegacyDnDisabled;

		public static readonly ADPropertyDefinition DefaultPublicFolderMailbox = OrganizationSchema.DefaultPublicFolderMailbox;

		public static readonly ADPropertyDefinition RemotePublicFolderMailboxes = OrganizationSchema.RemotePublicFolderMailboxes;

		public static readonly ADPropertyDefinition UMAvailableLanguages = OrganizationSchema.UMAvailableLanguages;

		public static readonly ADPropertyDefinition MaxConcurrentMigrations = OrganizationSchema.MaxConcurrentMigrations;

		public static readonly ADPropertyDefinition MaxAddressBookPolicies = OrganizationSchema.MaxAddressBookPolicies;

		public static readonly ADPropertyDefinition MaxOfflineAddressBooks = OrganizationSchema.MaxOfflineAddressBooks;

		public static readonly ADPropertyDefinition IsExcludedFromOnboardMigration = OrganizationSchema.IsExcludedFromOnboardMigration;

		public static readonly ADPropertyDefinition IsExcludedFromOffboardMigration = OrganizationSchema.IsExcludedFromOffboardMigration;

		public static readonly ADPropertyDefinition IsFfoMigrationInProgress = OrganizationSchema.IsFfoMigrationInProgress;

		public static readonly ADPropertyDefinition IsProcessEhaMigratedMessagesEnabled = OrganizationSchema.IsProcessEhaMigratedMessagesEnabled;

		public static readonly ADPropertyDefinition PublicFoldersLockedForMigration = OrganizationSchema.PublicFoldersLockedForMigration;

		public static readonly ADPropertyDefinition PublicFolderMigrationComplete = OrganizationSchema.PublicFolderMigrationComplete;

		public static readonly ADPropertyDefinition PublicFolderMailboxesLockedForNewConnections = OrganizationSchema.PublicFolderMailboxesLockedForNewConnections;

		public static readonly ADPropertyDefinition PublicFolderMailboxesMigrationComplete = OrganizationSchema.PublicFolderMailboxesMigrationComplete;

		public static readonly ADPropertyDefinition PublicFoldersEnabled = OrganizationSchema.PublicFoldersEnabled;

		public static readonly ADPropertyDefinition IsMailboxForcedReplicationDisabled = OrganizationSchema.IsMailboxForcedReplicationDisabled;

		public static readonly ADPropertyDefinition AdfsAuthenticationRawConfiguration = OrganizationSchema.AdfsAuthenticationRawConfiguration;

		public static readonly ADPropertyDefinition AdfsIssuer = OrganizationSchema.AdfsIssuer;

		public static readonly ADPropertyDefinition AdfsAudienceUris = OrganizationSchema.AdfsAudienceUris;

		public static readonly ADPropertyDefinition AdfsSignCertificateThumbprints = OrganizationSchema.AdfsSignCertificateThumbprints;

		public static readonly ADPropertyDefinition AdfsEncryptCertificateThumbprint = OrganizationSchema.AdfsEncryptCertificateThumbprint;

		public static readonly ADPropertyDefinition IsSyncPropertySetUpgradeAllowed = OrganizationSchema.IsSyncPropertySetUpgradeAllowed;

		public static readonly ADPropertyDefinition AdminDisplayVersion = OrganizationSchema.AdminDisplayVersion;

		public static readonly ADPropertyDefinition PreviousAdminDisplayVersion = OrganizationSchema.PreviousMailboxRelease;

		public static readonly ADPropertyDefinition IsUpgradingOrganization = OrganizationSchema.IsUpgradingOrganization;

		public static readonly ADPropertyDefinition IsUpdatingServicePlan = OrganizationSchema.IsUpdatingServicePlan;

		public static readonly ADPropertyDefinition ServicePlan = ADOrganizationConfigSchema.ServicePlan;

		public static readonly ADPropertyDefinition TargetServicePlan = ADOrganizationConfigSchema.TargetServicePlan;

		public static readonly ADPropertyDefinition WACDiscoveryEndpoint = OrganizationSchema.WACDiscoveryEndpoint;

		public static readonly ADPropertyDefinition DefaultPublicFolderAgeLimit = OrganizationSchema.DefaultPublicFolderAgeLimit;

		public static readonly ADPropertyDefinition DefaultPublicFolderIssueWarningQuota = OrganizationSchema.DefaultPublicFolderIssueWarningQuota;

		public static readonly ADPropertyDefinition DefaultPublicFolderProhibitPostQuota = OrganizationSchema.DefaultPublicFolderProhibitPostQuota;

		public static readonly ADPropertyDefinition DefaultPublicFolderMaxItemSize = OrganizationSchema.DefaultPublicFolderMaxItemSize;

		public static readonly ADPropertyDefinition DefaultPublicFolderDeletedItemRetention = OrganizationSchema.DefaultPublicFolderDeletedItemRetention;

		public static readonly ADPropertyDefinition DefaultPublicFolderMovedItemRetention = OrganizationSchema.DefaultPublicFolderMovedItemRetention;

		public static readonly ADPropertyDefinition SiteMailboxCreationURL = OrganizationSchema.SiteMailboxCreationURL;

		public static readonly ADPropertyDefinition PreferredInternetCodePageForShiftJis = OrganizationSchema.PreferredInternetCodePageForShiftJis;

		public static readonly ADPropertyDefinition ByteEncoderTypeFor7BitCharsets = OrganizationSchema.ByteEncoderTypeFor7BitCharsets;

		public static readonly ADPropertyDefinition RequiredCharsetCoverage = OrganizationSchema.RequiredCharsetCoverage;

		public static readonly ADPropertyDefinition PublicComputersDetectionEnabled = OrganizationSchema.PublicComputersDetectionEnabled;

		public static readonly ADPropertyDefinition RmsoSubscriptionStatus = OrganizationSchema.RmsoSubscriptionStatus;

		public static readonly ADPropertyDefinition IntuneManagedStatus = OrganizationSchema.IntuneManagedStatus;

		public static readonly ADPropertyDefinition HybridConfigurationStatus = OrganizationSchema.HybridConfigurationStatus;

		public static readonly ADPropertyDefinition ReleaseTrack = OrganizationSchema.ReleaseTrack;

		public static readonly ADPropertyDefinition SharePointUrl = ADOrganizationConfigSchema.SharePointUrl;

		public static readonly ADPropertyDefinition MapiHttpEnabled = OrganizationSchema.MapiHttpEnabled;

		public static readonly ADPropertyDefinition OAuth2ClientProfileEnabled = OrganizationSchema.OAuth2ClientProfileEnabled;

		public static readonly ADPropertyDefinition ACLableSyncedObjectEnabled = OrganizationSchema.ACLableSyncedObjectEnabled;
	}
}
