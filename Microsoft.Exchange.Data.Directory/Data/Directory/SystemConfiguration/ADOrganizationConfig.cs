using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public sealed class ADOrganizationConfig : Organization
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return ADOrganizationConfig.schema;
			}
		}

		internal override QueryFilter ImplicitFilter
		{
			get
			{
				return new OrFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, Organization.MostDerivedClass),
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, ExchangeConfigurationUnit.MostDerivedClass)
				});
			}
		}

		[Parameter(Mandatory = false)]
		public bool PublicFoldersLockedForMigration
		{
			get
			{
				return (bool)this[OrganizationSchema.PublicFoldersLockedForMigration];
			}
			set
			{
				this[OrganizationSchema.PublicFoldersLockedForMigration] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool PublicFolderMigrationComplete
		{
			get
			{
				return (bool)this[OrganizationSchema.PublicFolderMigrationComplete];
			}
			set
			{
				this[OrganizationSchema.PublicFolderMigrationComplete] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool PublicFolderMailboxesLockedForNewConnections
		{
			get
			{
				return (bool)this[OrganizationSchema.PublicFolderMailboxesLockedForNewConnections];
			}
			set
			{
				this[OrganizationSchema.PublicFolderMailboxesLockedForNewConnections] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool PublicFolderMailboxesMigrationComplete
		{
			get
			{
				return (bool)this[OrganizationSchema.PublicFolderMailboxesMigrationComplete];
			}
			set
			{
				this[OrganizationSchema.PublicFolderMailboxesMigrationComplete] = value;
			}
		}

		public string ServicePlan
		{
			get
			{
				return (string)this[ADOrganizationConfigSchema.ServicePlan];
			}
			internal set
			{
				this[ADOrganizationConfigSchema.ServicePlan] = value;
			}
		}

		public string TargetServicePlan
		{
			get
			{
				return (string)this[ADOrganizationConfigSchema.TargetServicePlan];
			}
			internal set
			{
				this[ADOrganizationConfigSchema.TargetServicePlan] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool PublicComputersDetectionEnabled
		{
			get
			{
				return (bool)this[OrganizationSchema.PublicComputersDetectionEnabled];
			}
			set
			{
				this[OrganizationSchema.PublicComputersDetectionEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RmsoSubscriptionStatusFlags RmsoSubscriptionStatus
		{
			get
			{
				return (RmsoSubscriptionStatusFlags)this[OrganizationSchema.RmsoSubscriptionStatus];
			}
			set
			{
				this[OrganizationSchema.RmsoSubscriptionStatus] = (int)value;
			}
		}

		[Parameter(Mandatory = false)]
		public ReleaseTrack? ReleaseTrack
		{
			get
			{
				return (ReleaseTrack?)this[OrganizationSchema.ReleaseTrack];
			}
			set
			{
				this[OrganizationSchema.ReleaseTrack] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Uri SharePointUrl
		{
			get
			{
				return (Uri)this[ADOrganizationConfigSchema.SharePointUrl];
			}
			set
			{
				this[ADOrganizationConfigSchema.SharePointUrl] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override Uri SiteMailboxCreationURL
		{
			get
			{
				return (Uri)this[OrganizationSchema.SiteMailboxCreationURL];
			}
			set
			{
				this[OrganizationSchema.SiteMailboxCreationURL] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override bool? CustomerFeedbackEnabled
		{
			get
			{
				return base.CustomerFeedbackEnabled;
			}
			set
			{
				base.CustomerFeedbackEnabled = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override IndustryType Industry
		{
			get
			{
				return base.Industry;
			}
			set
			{
				base.Industry = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override string ManagedFolderHomepage
		{
			get
			{
				return base.ManagedFolderHomepage;
			}
			set
			{
				base.ManagedFolderHomepage = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override EnhancedTimeSpan? DefaultPublicFolderAgeLimit
		{
			get
			{
				return base.DefaultPublicFolderAgeLimit;
			}
			set
			{
				base.DefaultPublicFolderAgeLimit = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override Unlimited<ByteQuantifiedSize> DefaultPublicFolderIssueWarningQuota
		{
			get
			{
				return base.DefaultPublicFolderIssueWarningQuota;
			}
			set
			{
				base.DefaultPublicFolderIssueWarningQuota = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override Unlimited<ByteQuantifiedSize> DefaultPublicFolderProhibitPostQuota
		{
			get
			{
				return base.DefaultPublicFolderProhibitPostQuota;
			}
			set
			{
				base.DefaultPublicFolderProhibitPostQuota = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override Unlimited<ByteQuantifiedSize> DefaultPublicFolderMaxItemSize
		{
			get
			{
				return base.DefaultPublicFolderMaxItemSize;
			}
			set
			{
				base.DefaultPublicFolderMaxItemSize = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override EnhancedTimeSpan? DefaultPublicFolderDeletedItemRetention
		{
			get
			{
				return base.DefaultPublicFolderDeletedItemRetention;
			}
			set
			{
				base.DefaultPublicFolderDeletedItemRetention = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override EnhancedTimeSpan? DefaultPublicFolderMovedItemRetention
		{
			get
			{
				return base.DefaultPublicFolderMovedItemRetention;
			}
			set
			{
				base.DefaultPublicFolderMovedItemRetention = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override MultiValuedProperty<OrganizationSummaryEntry> OrganizationSummary
		{
			get
			{
				return base.OrganizationSummary;
			}
			set
			{
				base.OrganizationSummary = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override bool ForwardSyncLiveIdBusinessInstance
		{
			get
			{
				return base.ForwardSyncLiveIdBusinessInstance;
			}
			set
			{
				base.ForwardSyncLiveIdBusinessInstance = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override ProxyAddressCollection MicrosoftExchangeRecipientEmailAddresses
		{
			get
			{
				return base.MicrosoftExchangeRecipientEmailAddresses;
			}
			set
			{
				base.MicrosoftExchangeRecipientEmailAddresses = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override SmtpAddress MicrosoftExchangeRecipientPrimarySmtpAddress
		{
			get
			{
				return base.MicrosoftExchangeRecipientPrimarySmtpAddress;
			}
			set
			{
				base.MicrosoftExchangeRecipientPrimarySmtpAddress = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override bool MicrosoftExchangeRecipientEmailAddressPolicyEnabled
		{
			get
			{
				return base.MicrosoftExchangeRecipientEmailAddressPolicyEnabled;
			}
			set
			{
				base.MicrosoftExchangeRecipientEmailAddressPolicyEnabled = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override bool MailTipsExternalRecipientsTipsEnabled
		{
			get
			{
				return base.MailTipsExternalRecipientsTipsEnabled;
			}
			set
			{
				base.MailTipsExternalRecipientsTipsEnabled = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override uint MailTipsLargeAudienceThreshold
		{
			get
			{
				return base.MailTipsLargeAudienceThreshold;
			}
			set
			{
				base.MailTipsLargeAudienceThreshold = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override PublicFoldersDeployment PublicFoldersEnabled
		{
			get
			{
				return base.PublicFoldersEnabled;
			}
			set
			{
				base.PublicFoldersEnabled = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override bool MailTipsMailboxSourcedTipsEnabled
		{
			get
			{
				return base.MailTipsMailboxSourcedTipsEnabled;
			}
			set
			{
				base.MailTipsMailboxSourcedTipsEnabled = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override bool MailTipsGroupMetricsEnabled
		{
			get
			{
				return base.MailTipsGroupMetricsEnabled;
			}
			set
			{
				base.MailTipsGroupMetricsEnabled = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override bool MailTipsAllTipsEnabled
		{
			get
			{
				return base.MailTipsAllTipsEnabled;
			}
			set
			{
				base.MailTipsAllTipsEnabled = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override bool ReadTrackingEnabled
		{
			get
			{
				return base.ReadTrackingEnabled;
			}
			set
			{
				base.ReadTrackingEnabled = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override MultiValuedProperty<string> DistributionGroupNameBlockedWordsList
		{
			get
			{
				return base.DistributionGroupNameBlockedWordsList;
			}
			set
			{
				base.DistributionGroupNameBlockedWordsList = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override DistributionGroupNamingPolicy DistributionGroupNamingPolicy
		{
			get
			{
				return base.DistributionGroupNamingPolicy;
			}
			set
			{
				base.DistributionGroupNamingPolicy = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override ProtocolConnectionSettings AVAuthenticationService
		{
			get
			{
				return base.AVAuthenticationService;
			}
			set
			{
				base.AVAuthenticationService = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override ProtocolConnectionSettings SIPAccessService
		{
			get
			{
				return base.SIPAccessService;
			}
			set
			{
				base.SIPAccessService = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override ProtocolConnectionSettings SIPSessionBorderController
		{
			get
			{
				return base.SIPSessionBorderController;
			}
			set
			{
				base.SIPSessionBorderController = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override bool ExchangeNotificationEnabled
		{
			get
			{
				return base.ExchangeNotificationEnabled;
			}
			set
			{
				base.ExchangeNotificationEnabled = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override EnhancedTimeSpan ActivityBasedAuthenticationTimeoutInterval
		{
			get
			{
				return base.ActivityBasedAuthenticationTimeoutInterval;
			}
			set
			{
				base.ActivityBasedAuthenticationTimeoutInterval = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override bool ActivityBasedAuthenticationTimeoutEnabled
		{
			get
			{
				return base.ActivityBasedAuthenticationTimeoutEnabled;
			}
			set
			{
				base.ActivityBasedAuthenticationTimeoutEnabled = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override bool ActivityBasedAuthenticationTimeoutWithSingleSignOnEnabled
		{
			get
			{
				return base.ActivityBasedAuthenticationTimeoutWithSingleSignOnEnabled;
			}
			set
			{
				base.ActivityBasedAuthenticationTimeoutWithSingleSignOnEnabled = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override string WACDiscoveryEndpoint
		{
			get
			{
				return base.WACDiscoveryEndpoint;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.WACDiscoveryEndpoint = string.Empty;
					return;
				}
				Uri uri;
				if (!Uri.TryCreate(value, UriKind.Absolute, out uri))
				{
					throw new ArgumentException(DirectoryStrings.WACDiscoveryEndpointShouldBeAbsoluteUri(value));
				}
				base.WACDiscoveryEndpoint = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override bool IsExcludedFromOnboardMigration
		{
			get
			{
				return base.IsExcludedFromOnboardMigration;
			}
			set
			{
				base.IsExcludedFromOnboardMigration = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override bool IsExcludedFromOffboardMigration
		{
			get
			{
				return base.IsExcludedFromOffboardMigration;
			}
			set
			{
				base.IsExcludedFromOffboardMigration = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override bool IsFfoMigrationInProgress
		{
			get
			{
				return base.IsFfoMigrationInProgress;
			}
			set
			{
				base.IsFfoMigrationInProgress = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override bool TenantRelocationsAllowed
		{
			get
			{
				return base.TenantRelocationsAllowed;
			}
			set
			{
				base.TenantRelocationsAllowed = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override Unlimited<int> MaxConcurrentMigrations
		{
			get
			{
				return base.MaxConcurrentMigrations;
			}
			set
			{
				base.MaxConcurrentMigrations = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override bool IsProcessEhaMigratedMessagesEnabled
		{
			get
			{
				return base.IsProcessEhaMigratedMessagesEnabled;
			}
			set
			{
				base.IsProcessEhaMigratedMessagesEnabled = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override bool AppsForOfficeEnabled
		{
			get
			{
				return base.AppsForOfficeEnabled;
			}
			set
			{
				base.AppsForOfficeEnabled = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override bool? EwsEnabled
		{
			get
			{
				return base.EwsEnabled;
			}
			set
			{
				base.EwsEnabled = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override bool? EwsAllowOutlook
		{
			get
			{
				return base.EwsAllowOutlook;
			}
			set
			{
				base.EwsAllowOutlook = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override bool? EwsAllowMacOutlook
		{
			get
			{
				return base.EwsAllowMacOutlook;
			}
			set
			{
				base.EwsAllowMacOutlook = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override bool? EwsAllowEntourage
		{
			get
			{
				return base.EwsAllowEntourage;
			}
			set
			{
				base.EwsAllowEntourage = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override EwsApplicationAccessPolicy? EwsApplicationAccessPolicy
		{
			get
			{
				return base.EwsApplicationAccessPolicy;
			}
			set
			{
				base.EwsApplicationAccessPolicy = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override MultiValuedProperty<string> EwsAllowList
		{
			get
			{
				return base.EwsAllowList;
			}
			set
			{
				base.EwsAllowList = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override MultiValuedProperty<string> EwsBlockList
		{
			get
			{
				return base.EwsBlockList;
			}
			set
			{
				base.EwsBlockList = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override bool CalendarVersionStoreEnabled
		{
			get
			{
				return base.CalendarVersionStoreEnabled;
			}
			set
			{
				base.CalendarVersionStoreEnabled = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override bool IsGuidPrefixedLegacyDnDisabled
		{
			get
			{
				return base.IsGuidPrefixedLegacyDnDisabled;
			}
			set
			{
				base.IsGuidPrefixedLegacyDnDisabled = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override MultiValuedProperty<UMLanguage> UMAvailableLanguages
		{
			get
			{
				return base.UMAvailableLanguages;
			}
			set
			{
				base.UMAvailableLanguages = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override bool IsMailboxForcedReplicationDisabled
		{
			get
			{
				return base.IsMailboxForcedReplicationDisabled;
			}
			set
			{
				base.IsMailboxForcedReplicationDisabled = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "AdfsAuthenticationRawConfiguration")]
		public override string AdfsAuthenticationConfiguration
		{
			get
			{
				return base.AdfsAuthenticationConfiguration;
			}
			set
			{
				base.AdfsAuthenticationConfiguration = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override int PreferredInternetCodePageForShiftJis
		{
			get
			{
				return base.PreferredInternetCodePageForShiftJis;
			}
			set
			{
				base.PreferredInternetCodePageForShiftJis = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override int RequiredCharsetCoverage
		{
			get
			{
				return base.RequiredCharsetCoverage;
			}
			set
			{
				base.RequiredCharsetCoverage = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override int ByteEncoderTypeFor7BitCharsets
		{
			get
			{
				return base.ByteEncoderTypeFor7BitCharsets;
			}
			set
			{
				base.ByteEncoderTypeFor7BitCharsets = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "AdfsAuthenticationParameter")]
		public override Uri AdfsIssuer
		{
			get
			{
				return base.AdfsIssuer;
			}
			set
			{
				base.AdfsIssuer = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "AdfsAuthenticationParameter")]
		public override MultiValuedProperty<Uri> AdfsAudienceUris
		{
			get
			{
				return base.AdfsAudienceUris;
			}
			set
			{
				base.AdfsAudienceUris = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "AdfsAuthenticationParameter")]
		public override MultiValuedProperty<string> AdfsSignCertificateThumbprints
		{
			get
			{
				return base.AdfsSignCertificateThumbprints;
			}
			set
			{
				base.AdfsSignCertificateThumbprints = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "AdfsAuthenticationParameter")]
		public override string AdfsEncryptCertificateThumbprint
		{
			get
			{
				return base.AdfsEncryptCertificateThumbprint;
			}
			set
			{
				base.AdfsEncryptCertificateThumbprint = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override bool IsSyncPropertySetUpgradeAllowed
		{
			get
			{
				return base.IsSyncPropertySetUpgradeAllowed;
			}
			set
			{
				base.IsSyncPropertySetUpgradeAllowed = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override bool MapiHttpEnabled
		{
			get
			{
				return base.MapiHttpEnabled;
			}
			set
			{
				base.MapiHttpEnabled = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override bool OAuth2ClientProfileEnabled
		{
			get
			{
				return base.OAuth2ClientProfileEnabled;
			}
			set
			{
				base.OAuth2ClientProfileEnabled = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override bool IntuneManagedStatus
		{
			get
			{
				return base.IntuneManagedStatus;
			}
			set
			{
				base.IntuneManagedStatus = value;
			}
		}

		[Parameter(Mandatory = false)]
		public HybridConfigurationStatusFlags HybridConfigurationStatus
		{
			get
			{
				return (HybridConfigurationStatusFlags)this[OrganizationSchema.HybridConfigurationStatus];
			}
			set
			{
				this[OrganizationSchema.HybridConfigurationStatus] = (int)value;
			}
		}

		[Parameter(Mandatory = false)]
		public override bool ACLableSyncedObjectEnabled
		{
			get
			{
				return (bool)this[OrganizationSchema.ACLableSyncedObjectEnabled];
			}
			set
			{
				this[OrganizationSchema.ACLableSyncedObjectEnabled] = value;
			}
		}

		private const string AdfsAuthenticationRawConfiguration = "AdfsAuthenticationRawConfiguration";

		private const string AdfsAuthenticationParameter = "AdfsAuthenticationParameter";

		private static readonly ADOrganizationConfigSchema schema = ObjectSchema.GetInstance<ADOrganizationConfigSchema>();
	}
}
