using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetOrganizationConfigCommand : SyntheticCommandWithPipelineInputNoOutput<ADOrganizationConfig>
	{
		private SetOrganizationConfigCommand() : base("Set-OrganizationConfig")
		{
		}

		public SetOrganizationConfigCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetOrganizationConfigCommand SetParameters(SetOrganizationConfigCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetOrganizationConfigCommand SetParameters(SetOrganizationConfigCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetOrganizationConfigCommand SetParameters(SetOrganizationConfigCommand.AdfsAuthenticationRawConfigurationParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetOrganizationConfigCommand SetParameters(SetOrganizationConfigCommand.AdfsAuthenticationParameterParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class IdentityParameters : ParametersBase
		{
			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new OrganizationIdParameter(value) : null);
				}
			}

			public virtual AccountPartitionIdParameter AccountPartition
			{
				set
				{
					base.PowerSharpParameters["AccountPartition"] = value;
				}
			}

			public virtual string MicrosoftExchangeRecipientReplyRecipient
			{
				set
				{
					base.PowerSharpParameters["MicrosoftExchangeRecipientReplyRecipient"] = ((value != null) ? new RecipientIdParameter(value) : null);
				}
			}

			public virtual string HierarchicalAddressBookRoot
			{
				set
				{
					base.PowerSharpParameters["HierarchicalAddressBookRoot"] = ((value != null) ? new UserContactGroupIdParameter(value) : null);
				}
			}

			public virtual string DistributionGroupDefaultOU
			{
				set
				{
					base.PowerSharpParameters["DistributionGroupDefaultOU"] = ((value != null) ? new OrganizationalUnitIdParameter(value) : null);
				}
			}

			public virtual MultiValuedProperty<RecipientIdParameter> ExchangeNotificationRecipients
			{
				set
				{
					base.PowerSharpParameters["ExchangeNotificationRecipients"] = value;
				}
			}

			public virtual MultiValuedProperty<MailboxOrMailUserIdParameter> RemotePublicFolderMailboxes
			{
				set
				{
					base.PowerSharpParameters["RemotePublicFolderMailboxes"] = value;
				}
			}

			public virtual int SCLJunkThreshold
			{
				set
				{
					base.PowerSharpParameters["SCLJunkThreshold"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual bool PublicFoldersLockedForMigration
			{
				set
				{
					base.PowerSharpParameters["PublicFoldersLockedForMigration"] = value;
				}
			}

			public virtual bool PublicFolderMigrationComplete
			{
				set
				{
					base.PowerSharpParameters["PublicFolderMigrationComplete"] = value;
				}
			}

			public virtual bool PublicFolderMailboxesLockedForNewConnections
			{
				set
				{
					base.PowerSharpParameters["PublicFolderMailboxesLockedForNewConnections"] = value;
				}
			}

			public virtual bool PublicFolderMailboxesMigrationComplete
			{
				set
				{
					base.PowerSharpParameters["PublicFolderMailboxesMigrationComplete"] = value;
				}
			}

			public virtual bool PublicComputersDetectionEnabled
			{
				set
				{
					base.PowerSharpParameters["PublicComputersDetectionEnabled"] = value;
				}
			}

			public virtual RmsoSubscriptionStatusFlags RmsoSubscriptionStatus
			{
				set
				{
					base.PowerSharpParameters["RmsoSubscriptionStatus"] = value;
				}
			}

			public virtual ReleaseTrack? ReleaseTrack
			{
				set
				{
					base.PowerSharpParameters["ReleaseTrack"] = value;
				}
			}

			public virtual Uri SharePointUrl
			{
				set
				{
					base.PowerSharpParameters["SharePointUrl"] = value;
				}
			}

			public virtual Uri SiteMailboxCreationURL
			{
				set
				{
					base.PowerSharpParameters["SiteMailboxCreationURL"] = value;
				}
			}

			public virtual bool? CustomerFeedbackEnabled
			{
				set
				{
					base.PowerSharpParameters["CustomerFeedbackEnabled"] = value;
				}
			}

			public virtual IndustryType Industry
			{
				set
				{
					base.PowerSharpParameters["Industry"] = value;
				}
			}

			public virtual string ManagedFolderHomepage
			{
				set
				{
					base.PowerSharpParameters["ManagedFolderHomepage"] = value;
				}
			}

			public virtual EnhancedTimeSpan? DefaultPublicFolderAgeLimit
			{
				set
				{
					base.PowerSharpParameters["DefaultPublicFolderAgeLimit"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> DefaultPublicFolderIssueWarningQuota
			{
				set
				{
					base.PowerSharpParameters["DefaultPublicFolderIssueWarningQuota"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> DefaultPublicFolderProhibitPostQuota
			{
				set
				{
					base.PowerSharpParameters["DefaultPublicFolderProhibitPostQuota"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> DefaultPublicFolderMaxItemSize
			{
				set
				{
					base.PowerSharpParameters["DefaultPublicFolderMaxItemSize"] = value;
				}
			}

			public virtual EnhancedTimeSpan? DefaultPublicFolderDeletedItemRetention
			{
				set
				{
					base.PowerSharpParameters["DefaultPublicFolderDeletedItemRetention"] = value;
				}
			}

			public virtual EnhancedTimeSpan? DefaultPublicFolderMovedItemRetention
			{
				set
				{
					base.PowerSharpParameters["DefaultPublicFolderMovedItemRetention"] = value;
				}
			}

			public virtual MultiValuedProperty<OrganizationSummaryEntry> OrganizationSummary
			{
				set
				{
					base.PowerSharpParameters["OrganizationSummary"] = value;
				}
			}

			public virtual bool ForwardSyncLiveIdBusinessInstance
			{
				set
				{
					base.PowerSharpParameters["ForwardSyncLiveIdBusinessInstance"] = value;
				}
			}

			public virtual ProxyAddressCollection MicrosoftExchangeRecipientEmailAddresses
			{
				set
				{
					base.PowerSharpParameters["MicrosoftExchangeRecipientEmailAddresses"] = value;
				}
			}

			public virtual SmtpAddress MicrosoftExchangeRecipientPrimarySmtpAddress
			{
				set
				{
					base.PowerSharpParameters["MicrosoftExchangeRecipientPrimarySmtpAddress"] = value;
				}
			}

			public virtual bool MicrosoftExchangeRecipientEmailAddressPolicyEnabled
			{
				set
				{
					base.PowerSharpParameters["MicrosoftExchangeRecipientEmailAddressPolicyEnabled"] = value;
				}
			}

			public virtual bool MailTipsExternalRecipientsTipsEnabled
			{
				set
				{
					base.PowerSharpParameters["MailTipsExternalRecipientsTipsEnabled"] = value;
				}
			}

			public virtual uint MailTipsLargeAudienceThreshold
			{
				set
				{
					base.PowerSharpParameters["MailTipsLargeAudienceThreshold"] = value;
				}
			}

			public virtual PublicFoldersDeployment PublicFoldersEnabled
			{
				set
				{
					base.PowerSharpParameters["PublicFoldersEnabled"] = value;
				}
			}

			public virtual bool MailTipsMailboxSourcedTipsEnabled
			{
				set
				{
					base.PowerSharpParameters["MailTipsMailboxSourcedTipsEnabled"] = value;
				}
			}

			public virtual bool MailTipsGroupMetricsEnabled
			{
				set
				{
					base.PowerSharpParameters["MailTipsGroupMetricsEnabled"] = value;
				}
			}

			public virtual bool MailTipsAllTipsEnabled
			{
				set
				{
					base.PowerSharpParameters["MailTipsAllTipsEnabled"] = value;
				}
			}

			public virtual bool ReadTrackingEnabled
			{
				set
				{
					base.PowerSharpParameters["ReadTrackingEnabled"] = value;
				}
			}

			public virtual MultiValuedProperty<string> DistributionGroupNameBlockedWordsList
			{
				set
				{
					base.PowerSharpParameters["DistributionGroupNameBlockedWordsList"] = value;
				}
			}

			public virtual DistributionGroupNamingPolicy DistributionGroupNamingPolicy
			{
				set
				{
					base.PowerSharpParameters["DistributionGroupNamingPolicy"] = value;
				}
			}

			public virtual ProtocolConnectionSettings AVAuthenticationService
			{
				set
				{
					base.PowerSharpParameters["AVAuthenticationService"] = value;
				}
			}

			public virtual ProtocolConnectionSettings SIPAccessService
			{
				set
				{
					base.PowerSharpParameters["SIPAccessService"] = value;
				}
			}

			public virtual ProtocolConnectionSettings SIPSessionBorderController
			{
				set
				{
					base.PowerSharpParameters["SIPSessionBorderController"] = value;
				}
			}

			public virtual bool ExchangeNotificationEnabled
			{
				set
				{
					base.PowerSharpParameters["ExchangeNotificationEnabled"] = value;
				}
			}

			public virtual EnhancedTimeSpan ActivityBasedAuthenticationTimeoutInterval
			{
				set
				{
					base.PowerSharpParameters["ActivityBasedAuthenticationTimeoutInterval"] = value;
				}
			}

			public virtual bool ActivityBasedAuthenticationTimeoutEnabled
			{
				set
				{
					base.PowerSharpParameters["ActivityBasedAuthenticationTimeoutEnabled"] = value;
				}
			}

			public virtual bool ActivityBasedAuthenticationTimeoutWithSingleSignOnEnabled
			{
				set
				{
					base.PowerSharpParameters["ActivityBasedAuthenticationTimeoutWithSingleSignOnEnabled"] = value;
				}
			}

			public virtual string WACDiscoveryEndpoint
			{
				set
				{
					base.PowerSharpParameters["WACDiscoveryEndpoint"] = value;
				}
			}

			public virtual bool IsExcludedFromOnboardMigration
			{
				set
				{
					base.PowerSharpParameters["IsExcludedFromOnboardMigration"] = value;
				}
			}

			public virtual bool IsExcludedFromOffboardMigration
			{
				set
				{
					base.PowerSharpParameters["IsExcludedFromOffboardMigration"] = value;
				}
			}

			public virtual bool IsFfoMigrationInProgress
			{
				set
				{
					base.PowerSharpParameters["IsFfoMigrationInProgress"] = value;
				}
			}

			public virtual bool TenantRelocationsAllowed
			{
				set
				{
					base.PowerSharpParameters["TenantRelocationsAllowed"] = value;
				}
			}

			public virtual Unlimited<int> MaxConcurrentMigrations
			{
				set
				{
					base.PowerSharpParameters["MaxConcurrentMigrations"] = value;
				}
			}

			public virtual bool IsProcessEhaMigratedMessagesEnabled
			{
				set
				{
					base.PowerSharpParameters["IsProcessEhaMigratedMessagesEnabled"] = value;
				}
			}

			public virtual bool AppsForOfficeEnabled
			{
				set
				{
					base.PowerSharpParameters["AppsForOfficeEnabled"] = value;
				}
			}

			public virtual bool? EwsEnabled
			{
				set
				{
					base.PowerSharpParameters["EwsEnabled"] = value;
				}
			}

			public virtual bool? EwsAllowOutlook
			{
				set
				{
					base.PowerSharpParameters["EwsAllowOutlook"] = value;
				}
			}

			public virtual bool? EwsAllowMacOutlook
			{
				set
				{
					base.PowerSharpParameters["EwsAllowMacOutlook"] = value;
				}
			}

			public virtual bool? EwsAllowEntourage
			{
				set
				{
					base.PowerSharpParameters["EwsAllowEntourage"] = value;
				}
			}

			public virtual EwsApplicationAccessPolicy? EwsApplicationAccessPolicy
			{
				set
				{
					base.PowerSharpParameters["EwsApplicationAccessPolicy"] = value;
				}
			}

			public virtual MultiValuedProperty<string> EwsAllowList
			{
				set
				{
					base.PowerSharpParameters["EwsAllowList"] = value;
				}
			}

			public virtual MultiValuedProperty<string> EwsBlockList
			{
				set
				{
					base.PowerSharpParameters["EwsBlockList"] = value;
				}
			}

			public virtual bool CalendarVersionStoreEnabled
			{
				set
				{
					base.PowerSharpParameters["CalendarVersionStoreEnabled"] = value;
				}
			}

			public virtual bool IsGuidPrefixedLegacyDnDisabled
			{
				set
				{
					base.PowerSharpParameters["IsGuidPrefixedLegacyDnDisabled"] = value;
				}
			}

			public virtual MultiValuedProperty<UMLanguage> UMAvailableLanguages
			{
				set
				{
					base.PowerSharpParameters["UMAvailableLanguages"] = value;
				}
			}

			public virtual bool IsMailboxForcedReplicationDisabled
			{
				set
				{
					base.PowerSharpParameters["IsMailboxForcedReplicationDisabled"] = value;
				}
			}

			public virtual int PreferredInternetCodePageForShiftJis
			{
				set
				{
					base.PowerSharpParameters["PreferredInternetCodePageForShiftJis"] = value;
				}
			}

			public virtual int RequiredCharsetCoverage
			{
				set
				{
					base.PowerSharpParameters["RequiredCharsetCoverage"] = value;
				}
			}

			public virtual int ByteEncoderTypeFor7BitCharsets
			{
				set
				{
					base.PowerSharpParameters["ByteEncoderTypeFor7BitCharsets"] = value;
				}
			}

			public virtual bool IsSyncPropertySetUpgradeAllowed
			{
				set
				{
					base.PowerSharpParameters["IsSyncPropertySetUpgradeAllowed"] = value;
				}
			}

			public virtual bool MapiHttpEnabled
			{
				set
				{
					base.PowerSharpParameters["MapiHttpEnabled"] = value;
				}
			}

			public virtual bool OAuth2ClientProfileEnabled
			{
				set
				{
					base.PowerSharpParameters["OAuth2ClientProfileEnabled"] = value;
				}
			}

			public virtual bool IntuneManagedStatus
			{
				set
				{
					base.PowerSharpParameters["IntuneManagedStatus"] = value;
				}
			}

			public virtual HybridConfigurationStatusFlags HybridConfigurationStatus
			{
				set
				{
					base.PowerSharpParameters["HybridConfigurationStatus"] = value;
				}
			}

			public virtual bool ACLableSyncedObjectEnabled
			{
				set
				{
					base.PowerSharpParameters["ACLableSyncedObjectEnabled"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual AccountPartitionIdParameter AccountPartition
			{
				set
				{
					base.PowerSharpParameters["AccountPartition"] = value;
				}
			}

			public virtual string MicrosoftExchangeRecipientReplyRecipient
			{
				set
				{
					base.PowerSharpParameters["MicrosoftExchangeRecipientReplyRecipient"] = ((value != null) ? new RecipientIdParameter(value) : null);
				}
			}

			public virtual string HierarchicalAddressBookRoot
			{
				set
				{
					base.PowerSharpParameters["HierarchicalAddressBookRoot"] = ((value != null) ? new UserContactGroupIdParameter(value) : null);
				}
			}

			public virtual string DistributionGroupDefaultOU
			{
				set
				{
					base.PowerSharpParameters["DistributionGroupDefaultOU"] = ((value != null) ? new OrganizationalUnitIdParameter(value) : null);
				}
			}

			public virtual MultiValuedProperty<RecipientIdParameter> ExchangeNotificationRecipients
			{
				set
				{
					base.PowerSharpParameters["ExchangeNotificationRecipients"] = value;
				}
			}

			public virtual MultiValuedProperty<MailboxOrMailUserIdParameter> RemotePublicFolderMailboxes
			{
				set
				{
					base.PowerSharpParameters["RemotePublicFolderMailboxes"] = value;
				}
			}

			public virtual int SCLJunkThreshold
			{
				set
				{
					base.PowerSharpParameters["SCLJunkThreshold"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual bool PublicFoldersLockedForMigration
			{
				set
				{
					base.PowerSharpParameters["PublicFoldersLockedForMigration"] = value;
				}
			}

			public virtual bool PublicFolderMigrationComplete
			{
				set
				{
					base.PowerSharpParameters["PublicFolderMigrationComplete"] = value;
				}
			}

			public virtual bool PublicFolderMailboxesLockedForNewConnections
			{
				set
				{
					base.PowerSharpParameters["PublicFolderMailboxesLockedForNewConnections"] = value;
				}
			}

			public virtual bool PublicFolderMailboxesMigrationComplete
			{
				set
				{
					base.PowerSharpParameters["PublicFolderMailboxesMigrationComplete"] = value;
				}
			}

			public virtual bool PublicComputersDetectionEnabled
			{
				set
				{
					base.PowerSharpParameters["PublicComputersDetectionEnabled"] = value;
				}
			}

			public virtual RmsoSubscriptionStatusFlags RmsoSubscriptionStatus
			{
				set
				{
					base.PowerSharpParameters["RmsoSubscriptionStatus"] = value;
				}
			}

			public virtual ReleaseTrack? ReleaseTrack
			{
				set
				{
					base.PowerSharpParameters["ReleaseTrack"] = value;
				}
			}

			public virtual Uri SharePointUrl
			{
				set
				{
					base.PowerSharpParameters["SharePointUrl"] = value;
				}
			}

			public virtual Uri SiteMailboxCreationURL
			{
				set
				{
					base.PowerSharpParameters["SiteMailboxCreationURL"] = value;
				}
			}

			public virtual bool? CustomerFeedbackEnabled
			{
				set
				{
					base.PowerSharpParameters["CustomerFeedbackEnabled"] = value;
				}
			}

			public virtual IndustryType Industry
			{
				set
				{
					base.PowerSharpParameters["Industry"] = value;
				}
			}

			public virtual string ManagedFolderHomepage
			{
				set
				{
					base.PowerSharpParameters["ManagedFolderHomepage"] = value;
				}
			}

			public virtual EnhancedTimeSpan? DefaultPublicFolderAgeLimit
			{
				set
				{
					base.PowerSharpParameters["DefaultPublicFolderAgeLimit"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> DefaultPublicFolderIssueWarningQuota
			{
				set
				{
					base.PowerSharpParameters["DefaultPublicFolderIssueWarningQuota"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> DefaultPublicFolderProhibitPostQuota
			{
				set
				{
					base.PowerSharpParameters["DefaultPublicFolderProhibitPostQuota"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> DefaultPublicFolderMaxItemSize
			{
				set
				{
					base.PowerSharpParameters["DefaultPublicFolderMaxItemSize"] = value;
				}
			}

			public virtual EnhancedTimeSpan? DefaultPublicFolderDeletedItemRetention
			{
				set
				{
					base.PowerSharpParameters["DefaultPublicFolderDeletedItemRetention"] = value;
				}
			}

			public virtual EnhancedTimeSpan? DefaultPublicFolderMovedItemRetention
			{
				set
				{
					base.PowerSharpParameters["DefaultPublicFolderMovedItemRetention"] = value;
				}
			}

			public virtual MultiValuedProperty<OrganizationSummaryEntry> OrganizationSummary
			{
				set
				{
					base.PowerSharpParameters["OrganizationSummary"] = value;
				}
			}

			public virtual bool ForwardSyncLiveIdBusinessInstance
			{
				set
				{
					base.PowerSharpParameters["ForwardSyncLiveIdBusinessInstance"] = value;
				}
			}

			public virtual ProxyAddressCollection MicrosoftExchangeRecipientEmailAddresses
			{
				set
				{
					base.PowerSharpParameters["MicrosoftExchangeRecipientEmailAddresses"] = value;
				}
			}

			public virtual SmtpAddress MicrosoftExchangeRecipientPrimarySmtpAddress
			{
				set
				{
					base.PowerSharpParameters["MicrosoftExchangeRecipientPrimarySmtpAddress"] = value;
				}
			}

			public virtual bool MicrosoftExchangeRecipientEmailAddressPolicyEnabled
			{
				set
				{
					base.PowerSharpParameters["MicrosoftExchangeRecipientEmailAddressPolicyEnabled"] = value;
				}
			}

			public virtual bool MailTipsExternalRecipientsTipsEnabled
			{
				set
				{
					base.PowerSharpParameters["MailTipsExternalRecipientsTipsEnabled"] = value;
				}
			}

			public virtual uint MailTipsLargeAudienceThreshold
			{
				set
				{
					base.PowerSharpParameters["MailTipsLargeAudienceThreshold"] = value;
				}
			}

			public virtual PublicFoldersDeployment PublicFoldersEnabled
			{
				set
				{
					base.PowerSharpParameters["PublicFoldersEnabled"] = value;
				}
			}

			public virtual bool MailTipsMailboxSourcedTipsEnabled
			{
				set
				{
					base.PowerSharpParameters["MailTipsMailboxSourcedTipsEnabled"] = value;
				}
			}

			public virtual bool MailTipsGroupMetricsEnabled
			{
				set
				{
					base.PowerSharpParameters["MailTipsGroupMetricsEnabled"] = value;
				}
			}

			public virtual bool MailTipsAllTipsEnabled
			{
				set
				{
					base.PowerSharpParameters["MailTipsAllTipsEnabled"] = value;
				}
			}

			public virtual bool ReadTrackingEnabled
			{
				set
				{
					base.PowerSharpParameters["ReadTrackingEnabled"] = value;
				}
			}

			public virtual MultiValuedProperty<string> DistributionGroupNameBlockedWordsList
			{
				set
				{
					base.PowerSharpParameters["DistributionGroupNameBlockedWordsList"] = value;
				}
			}

			public virtual DistributionGroupNamingPolicy DistributionGroupNamingPolicy
			{
				set
				{
					base.PowerSharpParameters["DistributionGroupNamingPolicy"] = value;
				}
			}

			public virtual ProtocolConnectionSettings AVAuthenticationService
			{
				set
				{
					base.PowerSharpParameters["AVAuthenticationService"] = value;
				}
			}

			public virtual ProtocolConnectionSettings SIPAccessService
			{
				set
				{
					base.PowerSharpParameters["SIPAccessService"] = value;
				}
			}

			public virtual ProtocolConnectionSettings SIPSessionBorderController
			{
				set
				{
					base.PowerSharpParameters["SIPSessionBorderController"] = value;
				}
			}

			public virtual bool ExchangeNotificationEnabled
			{
				set
				{
					base.PowerSharpParameters["ExchangeNotificationEnabled"] = value;
				}
			}

			public virtual EnhancedTimeSpan ActivityBasedAuthenticationTimeoutInterval
			{
				set
				{
					base.PowerSharpParameters["ActivityBasedAuthenticationTimeoutInterval"] = value;
				}
			}

			public virtual bool ActivityBasedAuthenticationTimeoutEnabled
			{
				set
				{
					base.PowerSharpParameters["ActivityBasedAuthenticationTimeoutEnabled"] = value;
				}
			}

			public virtual bool ActivityBasedAuthenticationTimeoutWithSingleSignOnEnabled
			{
				set
				{
					base.PowerSharpParameters["ActivityBasedAuthenticationTimeoutWithSingleSignOnEnabled"] = value;
				}
			}

			public virtual string WACDiscoveryEndpoint
			{
				set
				{
					base.PowerSharpParameters["WACDiscoveryEndpoint"] = value;
				}
			}

			public virtual bool IsExcludedFromOnboardMigration
			{
				set
				{
					base.PowerSharpParameters["IsExcludedFromOnboardMigration"] = value;
				}
			}

			public virtual bool IsExcludedFromOffboardMigration
			{
				set
				{
					base.PowerSharpParameters["IsExcludedFromOffboardMigration"] = value;
				}
			}

			public virtual bool IsFfoMigrationInProgress
			{
				set
				{
					base.PowerSharpParameters["IsFfoMigrationInProgress"] = value;
				}
			}

			public virtual bool TenantRelocationsAllowed
			{
				set
				{
					base.PowerSharpParameters["TenantRelocationsAllowed"] = value;
				}
			}

			public virtual Unlimited<int> MaxConcurrentMigrations
			{
				set
				{
					base.PowerSharpParameters["MaxConcurrentMigrations"] = value;
				}
			}

			public virtual bool IsProcessEhaMigratedMessagesEnabled
			{
				set
				{
					base.PowerSharpParameters["IsProcessEhaMigratedMessagesEnabled"] = value;
				}
			}

			public virtual bool AppsForOfficeEnabled
			{
				set
				{
					base.PowerSharpParameters["AppsForOfficeEnabled"] = value;
				}
			}

			public virtual bool? EwsEnabled
			{
				set
				{
					base.PowerSharpParameters["EwsEnabled"] = value;
				}
			}

			public virtual bool? EwsAllowOutlook
			{
				set
				{
					base.PowerSharpParameters["EwsAllowOutlook"] = value;
				}
			}

			public virtual bool? EwsAllowMacOutlook
			{
				set
				{
					base.PowerSharpParameters["EwsAllowMacOutlook"] = value;
				}
			}

			public virtual bool? EwsAllowEntourage
			{
				set
				{
					base.PowerSharpParameters["EwsAllowEntourage"] = value;
				}
			}

			public virtual EwsApplicationAccessPolicy? EwsApplicationAccessPolicy
			{
				set
				{
					base.PowerSharpParameters["EwsApplicationAccessPolicy"] = value;
				}
			}

			public virtual MultiValuedProperty<string> EwsAllowList
			{
				set
				{
					base.PowerSharpParameters["EwsAllowList"] = value;
				}
			}

			public virtual MultiValuedProperty<string> EwsBlockList
			{
				set
				{
					base.PowerSharpParameters["EwsBlockList"] = value;
				}
			}

			public virtual bool CalendarVersionStoreEnabled
			{
				set
				{
					base.PowerSharpParameters["CalendarVersionStoreEnabled"] = value;
				}
			}

			public virtual bool IsGuidPrefixedLegacyDnDisabled
			{
				set
				{
					base.PowerSharpParameters["IsGuidPrefixedLegacyDnDisabled"] = value;
				}
			}

			public virtual MultiValuedProperty<UMLanguage> UMAvailableLanguages
			{
				set
				{
					base.PowerSharpParameters["UMAvailableLanguages"] = value;
				}
			}

			public virtual bool IsMailboxForcedReplicationDisabled
			{
				set
				{
					base.PowerSharpParameters["IsMailboxForcedReplicationDisabled"] = value;
				}
			}

			public virtual int PreferredInternetCodePageForShiftJis
			{
				set
				{
					base.PowerSharpParameters["PreferredInternetCodePageForShiftJis"] = value;
				}
			}

			public virtual int RequiredCharsetCoverage
			{
				set
				{
					base.PowerSharpParameters["RequiredCharsetCoverage"] = value;
				}
			}

			public virtual int ByteEncoderTypeFor7BitCharsets
			{
				set
				{
					base.PowerSharpParameters["ByteEncoderTypeFor7BitCharsets"] = value;
				}
			}

			public virtual bool IsSyncPropertySetUpgradeAllowed
			{
				set
				{
					base.PowerSharpParameters["IsSyncPropertySetUpgradeAllowed"] = value;
				}
			}

			public virtual bool MapiHttpEnabled
			{
				set
				{
					base.PowerSharpParameters["MapiHttpEnabled"] = value;
				}
			}

			public virtual bool OAuth2ClientProfileEnabled
			{
				set
				{
					base.PowerSharpParameters["OAuth2ClientProfileEnabled"] = value;
				}
			}

			public virtual bool IntuneManagedStatus
			{
				set
				{
					base.PowerSharpParameters["IntuneManagedStatus"] = value;
				}
			}

			public virtual HybridConfigurationStatusFlags HybridConfigurationStatus
			{
				set
				{
					base.PowerSharpParameters["HybridConfigurationStatus"] = value;
				}
			}

			public virtual bool ACLableSyncedObjectEnabled
			{
				set
				{
					base.PowerSharpParameters["ACLableSyncedObjectEnabled"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}
		}

		public class AdfsAuthenticationRawConfigurationParameters : ParametersBase
		{
			public virtual string AdfsAuthenticationConfiguration
			{
				set
				{
					base.PowerSharpParameters["AdfsAuthenticationConfiguration"] = value;
				}
			}

			public virtual AccountPartitionIdParameter AccountPartition
			{
				set
				{
					base.PowerSharpParameters["AccountPartition"] = value;
				}
			}

			public virtual string MicrosoftExchangeRecipientReplyRecipient
			{
				set
				{
					base.PowerSharpParameters["MicrosoftExchangeRecipientReplyRecipient"] = ((value != null) ? new RecipientIdParameter(value) : null);
				}
			}

			public virtual string HierarchicalAddressBookRoot
			{
				set
				{
					base.PowerSharpParameters["HierarchicalAddressBookRoot"] = ((value != null) ? new UserContactGroupIdParameter(value) : null);
				}
			}

			public virtual string DistributionGroupDefaultOU
			{
				set
				{
					base.PowerSharpParameters["DistributionGroupDefaultOU"] = ((value != null) ? new OrganizationalUnitIdParameter(value) : null);
				}
			}

			public virtual MultiValuedProperty<RecipientIdParameter> ExchangeNotificationRecipients
			{
				set
				{
					base.PowerSharpParameters["ExchangeNotificationRecipients"] = value;
				}
			}

			public virtual MultiValuedProperty<MailboxOrMailUserIdParameter> RemotePublicFolderMailboxes
			{
				set
				{
					base.PowerSharpParameters["RemotePublicFolderMailboxes"] = value;
				}
			}

			public virtual int SCLJunkThreshold
			{
				set
				{
					base.PowerSharpParameters["SCLJunkThreshold"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual bool PublicFoldersLockedForMigration
			{
				set
				{
					base.PowerSharpParameters["PublicFoldersLockedForMigration"] = value;
				}
			}

			public virtual bool PublicFolderMigrationComplete
			{
				set
				{
					base.PowerSharpParameters["PublicFolderMigrationComplete"] = value;
				}
			}

			public virtual bool PublicFolderMailboxesLockedForNewConnections
			{
				set
				{
					base.PowerSharpParameters["PublicFolderMailboxesLockedForNewConnections"] = value;
				}
			}

			public virtual bool PublicFolderMailboxesMigrationComplete
			{
				set
				{
					base.PowerSharpParameters["PublicFolderMailboxesMigrationComplete"] = value;
				}
			}

			public virtual bool PublicComputersDetectionEnabled
			{
				set
				{
					base.PowerSharpParameters["PublicComputersDetectionEnabled"] = value;
				}
			}

			public virtual RmsoSubscriptionStatusFlags RmsoSubscriptionStatus
			{
				set
				{
					base.PowerSharpParameters["RmsoSubscriptionStatus"] = value;
				}
			}

			public virtual ReleaseTrack? ReleaseTrack
			{
				set
				{
					base.PowerSharpParameters["ReleaseTrack"] = value;
				}
			}

			public virtual Uri SharePointUrl
			{
				set
				{
					base.PowerSharpParameters["SharePointUrl"] = value;
				}
			}

			public virtual Uri SiteMailboxCreationURL
			{
				set
				{
					base.PowerSharpParameters["SiteMailboxCreationURL"] = value;
				}
			}

			public virtual bool? CustomerFeedbackEnabled
			{
				set
				{
					base.PowerSharpParameters["CustomerFeedbackEnabled"] = value;
				}
			}

			public virtual IndustryType Industry
			{
				set
				{
					base.PowerSharpParameters["Industry"] = value;
				}
			}

			public virtual string ManagedFolderHomepage
			{
				set
				{
					base.PowerSharpParameters["ManagedFolderHomepage"] = value;
				}
			}

			public virtual EnhancedTimeSpan? DefaultPublicFolderAgeLimit
			{
				set
				{
					base.PowerSharpParameters["DefaultPublicFolderAgeLimit"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> DefaultPublicFolderIssueWarningQuota
			{
				set
				{
					base.PowerSharpParameters["DefaultPublicFolderIssueWarningQuota"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> DefaultPublicFolderProhibitPostQuota
			{
				set
				{
					base.PowerSharpParameters["DefaultPublicFolderProhibitPostQuota"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> DefaultPublicFolderMaxItemSize
			{
				set
				{
					base.PowerSharpParameters["DefaultPublicFolderMaxItemSize"] = value;
				}
			}

			public virtual EnhancedTimeSpan? DefaultPublicFolderDeletedItemRetention
			{
				set
				{
					base.PowerSharpParameters["DefaultPublicFolderDeletedItemRetention"] = value;
				}
			}

			public virtual EnhancedTimeSpan? DefaultPublicFolderMovedItemRetention
			{
				set
				{
					base.PowerSharpParameters["DefaultPublicFolderMovedItemRetention"] = value;
				}
			}

			public virtual MultiValuedProperty<OrganizationSummaryEntry> OrganizationSummary
			{
				set
				{
					base.PowerSharpParameters["OrganizationSummary"] = value;
				}
			}

			public virtual bool ForwardSyncLiveIdBusinessInstance
			{
				set
				{
					base.PowerSharpParameters["ForwardSyncLiveIdBusinessInstance"] = value;
				}
			}

			public virtual ProxyAddressCollection MicrosoftExchangeRecipientEmailAddresses
			{
				set
				{
					base.PowerSharpParameters["MicrosoftExchangeRecipientEmailAddresses"] = value;
				}
			}

			public virtual SmtpAddress MicrosoftExchangeRecipientPrimarySmtpAddress
			{
				set
				{
					base.PowerSharpParameters["MicrosoftExchangeRecipientPrimarySmtpAddress"] = value;
				}
			}

			public virtual bool MicrosoftExchangeRecipientEmailAddressPolicyEnabled
			{
				set
				{
					base.PowerSharpParameters["MicrosoftExchangeRecipientEmailAddressPolicyEnabled"] = value;
				}
			}

			public virtual bool MailTipsExternalRecipientsTipsEnabled
			{
				set
				{
					base.PowerSharpParameters["MailTipsExternalRecipientsTipsEnabled"] = value;
				}
			}

			public virtual uint MailTipsLargeAudienceThreshold
			{
				set
				{
					base.PowerSharpParameters["MailTipsLargeAudienceThreshold"] = value;
				}
			}

			public virtual PublicFoldersDeployment PublicFoldersEnabled
			{
				set
				{
					base.PowerSharpParameters["PublicFoldersEnabled"] = value;
				}
			}

			public virtual bool MailTipsMailboxSourcedTipsEnabled
			{
				set
				{
					base.PowerSharpParameters["MailTipsMailboxSourcedTipsEnabled"] = value;
				}
			}

			public virtual bool MailTipsGroupMetricsEnabled
			{
				set
				{
					base.PowerSharpParameters["MailTipsGroupMetricsEnabled"] = value;
				}
			}

			public virtual bool MailTipsAllTipsEnabled
			{
				set
				{
					base.PowerSharpParameters["MailTipsAllTipsEnabled"] = value;
				}
			}

			public virtual bool ReadTrackingEnabled
			{
				set
				{
					base.PowerSharpParameters["ReadTrackingEnabled"] = value;
				}
			}

			public virtual MultiValuedProperty<string> DistributionGroupNameBlockedWordsList
			{
				set
				{
					base.PowerSharpParameters["DistributionGroupNameBlockedWordsList"] = value;
				}
			}

			public virtual DistributionGroupNamingPolicy DistributionGroupNamingPolicy
			{
				set
				{
					base.PowerSharpParameters["DistributionGroupNamingPolicy"] = value;
				}
			}

			public virtual ProtocolConnectionSettings AVAuthenticationService
			{
				set
				{
					base.PowerSharpParameters["AVAuthenticationService"] = value;
				}
			}

			public virtual ProtocolConnectionSettings SIPAccessService
			{
				set
				{
					base.PowerSharpParameters["SIPAccessService"] = value;
				}
			}

			public virtual ProtocolConnectionSettings SIPSessionBorderController
			{
				set
				{
					base.PowerSharpParameters["SIPSessionBorderController"] = value;
				}
			}

			public virtual bool ExchangeNotificationEnabled
			{
				set
				{
					base.PowerSharpParameters["ExchangeNotificationEnabled"] = value;
				}
			}

			public virtual EnhancedTimeSpan ActivityBasedAuthenticationTimeoutInterval
			{
				set
				{
					base.PowerSharpParameters["ActivityBasedAuthenticationTimeoutInterval"] = value;
				}
			}

			public virtual bool ActivityBasedAuthenticationTimeoutEnabled
			{
				set
				{
					base.PowerSharpParameters["ActivityBasedAuthenticationTimeoutEnabled"] = value;
				}
			}

			public virtual bool ActivityBasedAuthenticationTimeoutWithSingleSignOnEnabled
			{
				set
				{
					base.PowerSharpParameters["ActivityBasedAuthenticationTimeoutWithSingleSignOnEnabled"] = value;
				}
			}

			public virtual string WACDiscoveryEndpoint
			{
				set
				{
					base.PowerSharpParameters["WACDiscoveryEndpoint"] = value;
				}
			}

			public virtual bool IsExcludedFromOnboardMigration
			{
				set
				{
					base.PowerSharpParameters["IsExcludedFromOnboardMigration"] = value;
				}
			}

			public virtual bool IsExcludedFromOffboardMigration
			{
				set
				{
					base.PowerSharpParameters["IsExcludedFromOffboardMigration"] = value;
				}
			}

			public virtual bool IsFfoMigrationInProgress
			{
				set
				{
					base.PowerSharpParameters["IsFfoMigrationInProgress"] = value;
				}
			}

			public virtual bool TenantRelocationsAllowed
			{
				set
				{
					base.PowerSharpParameters["TenantRelocationsAllowed"] = value;
				}
			}

			public virtual Unlimited<int> MaxConcurrentMigrations
			{
				set
				{
					base.PowerSharpParameters["MaxConcurrentMigrations"] = value;
				}
			}

			public virtual bool IsProcessEhaMigratedMessagesEnabled
			{
				set
				{
					base.PowerSharpParameters["IsProcessEhaMigratedMessagesEnabled"] = value;
				}
			}

			public virtual bool AppsForOfficeEnabled
			{
				set
				{
					base.PowerSharpParameters["AppsForOfficeEnabled"] = value;
				}
			}

			public virtual bool? EwsEnabled
			{
				set
				{
					base.PowerSharpParameters["EwsEnabled"] = value;
				}
			}

			public virtual bool? EwsAllowOutlook
			{
				set
				{
					base.PowerSharpParameters["EwsAllowOutlook"] = value;
				}
			}

			public virtual bool? EwsAllowMacOutlook
			{
				set
				{
					base.PowerSharpParameters["EwsAllowMacOutlook"] = value;
				}
			}

			public virtual bool? EwsAllowEntourage
			{
				set
				{
					base.PowerSharpParameters["EwsAllowEntourage"] = value;
				}
			}

			public virtual EwsApplicationAccessPolicy? EwsApplicationAccessPolicy
			{
				set
				{
					base.PowerSharpParameters["EwsApplicationAccessPolicy"] = value;
				}
			}

			public virtual MultiValuedProperty<string> EwsAllowList
			{
				set
				{
					base.PowerSharpParameters["EwsAllowList"] = value;
				}
			}

			public virtual MultiValuedProperty<string> EwsBlockList
			{
				set
				{
					base.PowerSharpParameters["EwsBlockList"] = value;
				}
			}

			public virtual bool CalendarVersionStoreEnabled
			{
				set
				{
					base.PowerSharpParameters["CalendarVersionStoreEnabled"] = value;
				}
			}

			public virtual bool IsGuidPrefixedLegacyDnDisabled
			{
				set
				{
					base.PowerSharpParameters["IsGuidPrefixedLegacyDnDisabled"] = value;
				}
			}

			public virtual MultiValuedProperty<UMLanguage> UMAvailableLanguages
			{
				set
				{
					base.PowerSharpParameters["UMAvailableLanguages"] = value;
				}
			}

			public virtual bool IsMailboxForcedReplicationDisabled
			{
				set
				{
					base.PowerSharpParameters["IsMailboxForcedReplicationDisabled"] = value;
				}
			}

			public virtual int PreferredInternetCodePageForShiftJis
			{
				set
				{
					base.PowerSharpParameters["PreferredInternetCodePageForShiftJis"] = value;
				}
			}

			public virtual int RequiredCharsetCoverage
			{
				set
				{
					base.PowerSharpParameters["RequiredCharsetCoverage"] = value;
				}
			}

			public virtual int ByteEncoderTypeFor7BitCharsets
			{
				set
				{
					base.PowerSharpParameters["ByteEncoderTypeFor7BitCharsets"] = value;
				}
			}

			public virtual bool IsSyncPropertySetUpgradeAllowed
			{
				set
				{
					base.PowerSharpParameters["IsSyncPropertySetUpgradeAllowed"] = value;
				}
			}

			public virtual bool MapiHttpEnabled
			{
				set
				{
					base.PowerSharpParameters["MapiHttpEnabled"] = value;
				}
			}

			public virtual bool OAuth2ClientProfileEnabled
			{
				set
				{
					base.PowerSharpParameters["OAuth2ClientProfileEnabled"] = value;
				}
			}

			public virtual bool IntuneManagedStatus
			{
				set
				{
					base.PowerSharpParameters["IntuneManagedStatus"] = value;
				}
			}

			public virtual HybridConfigurationStatusFlags HybridConfigurationStatus
			{
				set
				{
					base.PowerSharpParameters["HybridConfigurationStatus"] = value;
				}
			}

			public virtual bool ACLableSyncedObjectEnabled
			{
				set
				{
					base.PowerSharpParameters["ACLableSyncedObjectEnabled"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}
		}

		public class AdfsAuthenticationParameterParameters : ParametersBase
		{
			public virtual Uri AdfsIssuer
			{
				set
				{
					base.PowerSharpParameters["AdfsIssuer"] = value;
				}
			}

			public virtual MultiValuedProperty<Uri> AdfsAudienceUris
			{
				set
				{
					base.PowerSharpParameters["AdfsAudienceUris"] = value;
				}
			}

			public virtual MultiValuedProperty<string> AdfsSignCertificateThumbprints
			{
				set
				{
					base.PowerSharpParameters["AdfsSignCertificateThumbprints"] = value;
				}
			}

			public virtual string AdfsEncryptCertificateThumbprint
			{
				set
				{
					base.PowerSharpParameters["AdfsEncryptCertificateThumbprint"] = value;
				}
			}

			public virtual AccountPartitionIdParameter AccountPartition
			{
				set
				{
					base.PowerSharpParameters["AccountPartition"] = value;
				}
			}

			public virtual string MicrosoftExchangeRecipientReplyRecipient
			{
				set
				{
					base.PowerSharpParameters["MicrosoftExchangeRecipientReplyRecipient"] = ((value != null) ? new RecipientIdParameter(value) : null);
				}
			}

			public virtual string HierarchicalAddressBookRoot
			{
				set
				{
					base.PowerSharpParameters["HierarchicalAddressBookRoot"] = ((value != null) ? new UserContactGroupIdParameter(value) : null);
				}
			}

			public virtual string DistributionGroupDefaultOU
			{
				set
				{
					base.PowerSharpParameters["DistributionGroupDefaultOU"] = ((value != null) ? new OrganizationalUnitIdParameter(value) : null);
				}
			}

			public virtual MultiValuedProperty<RecipientIdParameter> ExchangeNotificationRecipients
			{
				set
				{
					base.PowerSharpParameters["ExchangeNotificationRecipients"] = value;
				}
			}

			public virtual MultiValuedProperty<MailboxOrMailUserIdParameter> RemotePublicFolderMailboxes
			{
				set
				{
					base.PowerSharpParameters["RemotePublicFolderMailboxes"] = value;
				}
			}

			public virtual int SCLJunkThreshold
			{
				set
				{
					base.PowerSharpParameters["SCLJunkThreshold"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual bool PublicFoldersLockedForMigration
			{
				set
				{
					base.PowerSharpParameters["PublicFoldersLockedForMigration"] = value;
				}
			}

			public virtual bool PublicFolderMigrationComplete
			{
				set
				{
					base.PowerSharpParameters["PublicFolderMigrationComplete"] = value;
				}
			}

			public virtual bool PublicFolderMailboxesLockedForNewConnections
			{
				set
				{
					base.PowerSharpParameters["PublicFolderMailboxesLockedForNewConnections"] = value;
				}
			}

			public virtual bool PublicFolderMailboxesMigrationComplete
			{
				set
				{
					base.PowerSharpParameters["PublicFolderMailboxesMigrationComplete"] = value;
				}
			}

			public virtual bool PublicComputersDetectionEnabled
			{
				set
				{
					base.PowerSharpParameters["PublicComputersDetectionEnabled"] = value;
				}
			}

			public virtual RmsoSubscriptionStatusFlags RmsoSubscriptionStatus
			{
				set
				{
					base.PowerSharpParameters["RmsoSubscriptionStatus"] = value;
				}
			}

			public virtual ReleaseTrack? ReleaseTrack
			{
				set
				{
					base.PowerSharpParameters["ReleaseTrack"] = value;
				}
			}

			public virtual Uri SharePointUrl
			{
				set
				{
					base.PowerSharpParameters["SharePointUrl"] = value;
				}
			}

			public virtual Uri SiteMailboxCreationURL
			{
				set
				{
					base.PowerSharpParameters["SiteMailboxCreationURL"] = value;
				}
			}

			public virtual bool? CustomerFeedbackEnabled
			{
				set
				{
					base.PowerSharpParameters["CustomerFeedbackEnabled"] = value;
				}
			}

			public virtual IndustryType Industry
			{
				set
				{
					base.PowerSharpParameters["Industry"] = value;
				}
			}

			public virtual string ManagedFolderHomepage
			{
				set
				{
					base.PowerSharpParameters["ManagedFolderHomepage"] = value;
				}
			}

			public virtual EnhancedTimeSpan? DefaultPublicFolderAgeLimit
			{
				set
				{
					base.PowerSharpParameters["DefaultPublicFolderAgeLimit"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> DefaultPublicFolderIssueWarningQuota
			{
				set
				{
					base.PowerSharpParameters["DefaultPublicFolderIssueWarningQuota"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> DefaultPublicFolderProhibitPostQuota
			{
				set
				{
					base.PowerSharpParameters["DefaultPublicFolderProhibitPostQuota"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> DefaultPublicFolderMaxItemSize
			{
				set
				{
					base.PowerSharpParameters["DefaultPublicFolderMaxItemSize"] = value;
				}
			}

			public virtual EnhancedTimeSpan? DefaultPublicFolderDeletedItemRetention
			{
				set
				{
					base.PowerSharpParameters["DefaultPublicFolderDeletedItemRetention"] = value;
				}
			}

			public virtual EnhancedTimeSpan? DefaultPublicFolderMovedItemRetention
			{
				set
				{
					base.PowerSharpParameters["DefaultPublicFolderMovedItemRetention"] = value;
				}
			}

			public virtual MultiValuedProperty<OrganizationSummaryEntry> OrganizationSummary
			{
				set
				{
					base.PowerSharpParameters["OrganizationSummary"] = value;
				}
			}

			public virtual bool ForwardSyncLiveIdBusinessInstance
			{
				set
				{
					base.PowerSharpParameters["ForwardSyncLiveIdBusinessInstance"] = value;
				}
			}

			public virtual ProxyAddressCollection MicrosoftExchangeRecipientEmailAddresses
			{
				set
				{
					base.PowerSharpParameters["MicrosoftExchangeRecipientEmailAddresses"] = value;
				}
			}

			public virtual SmtpAddress MicrosoftExchangeRecipientPrimarySmtpAddress
			{
				set
				{
					base.PowerSharpParameters["MicrosoftExchangeRecipientPrimarySmtpAddress"] = value;
				}
			}

			public virtual bool MicrosoftExchangeRecipientEmailAddressPolicyEnabled
			{
				set
				{
					base.PowerSharpParameters["MicrosoftExchangeRecipientEmailAddressPolicyEnabled"] = value;
				}
			}

			public virtual bool MailTipsExternalRecipientsTipsEnabled
			{
				set
				{
					base.PowerSharpParameters["MailTipsExternalRecipientsTipsEnabled"] = value;
				}
			}

			public virtual uint MailTipsLargeAudienceThreshold
			{
				set
				{
					base.PowerSharpParameters["MailTipsLargeAudienceThreshold"] = value;
				}
			}

			public virtual PublicFoldersDeployment PublicFoldersEnabled
			{
				set
				{
					base.PowerSharpParameters["PublicFoldersEnabled"] = value;
				}
			}

			public virtual bool MailTipsMailboxSourcedTipsEnabled
			{
				set
				{
					base.PowerSharpParameters["MailTipsMailboxSourcedTipsEnabled"] = value;
				}
			}

			public virtual bool MailTipsGroupMetricsEnabled
			{
				set
				{
					base.PowerSharpParameters["MailTipsGroupMetricsEnabled"] = value;
				}
			}

			public virtual bool MailTipsAllTipsEnabled
			{
				set
				{
					base.PowerSharpParameters["MailTipsAllTipsEnabled"] = value;
				}
			}

			public virtual bool ReadTrackingEnabled
			{
				set
				{
					base.PowerSharpParameters["ReadTrackingEnabled"] = value;
				}
			}

			public virtual MultiValuedProperty<string> DistributionGroupNameBlockedWordsList
			{
				set
				{
					base.PowerSharpParameters["DistributionGroupNameBlockedWordsList"] = value;
				}
			}

			public virtual DistributionGroupNamingPolicy DistributionGroupNamingPolicy
			{
				set
				{
					base.PowerSharpParameters["DistributionGroupNamingPolicy"] = value;
				}
			}

			public virtual ProtocolConnectionSettings AVAuthenticationService
			{
				set
				{
					base.PowerSharpParameters["AVAuthenticationService"] = value;
				}
			}

			public virtual ProtocolConnectionSettings SIPAccessService
			{
				set
				{
					base.PowerSharpParameters["SIPAccessService"] = value;
				}
			}

			public virtual ProtocolConnectionSettings SIPSessionBorderController
			{
				set
				{
					base.PowerSharpParameters["SIPSessionBorderController"] = value;
				}
			}

			public virtual bool ExchangeNotificationEnabled
			{
				set
				{
					base.PowerSharpParameters["ExchangeNotificationEnabled"] = value;
				}
			}

			public virtual EnhancedTimeSpan ActivityBasedAuthenticationTimeoutInterval
			{
				set
				{
					base.PowerSharpParameters["ActivityBasedAuthenticationTimeoutInterval"] = value;
				}
			}

			public virtual bool ActivityBasedAuthenticationTimeoutEnabled
			{
				set
				{
					base.PowerSharpParameters["ActivityBasedAuthenticationTimeoutEnabled"] = value;
				}
			}

			public virtual bool ActivityBasedAuthenticationTimeoutWithSingleSignOnEnabled
			{
				set
				{
					base.PowerSharpParameters["ActivityBasedAuthenticationTimeoutWithSingleSignOnEnabled"] = value;
				}
			}

			public virtual string WACDiscoveryEndpoint
			{
				set
				{
					base.PowerSharpParameters["WACDiscoveryEndpoint"] = value;
				}
			}

			public virtual bool IsExcludedFromOnboardMigration
			{
				set
				{
					base.PowerSharpParameters["IsExcludedFromOnboardMigration"] = value;
				}
			}

			public virtual bool IsExcludedFromOffboardMigration
			{
				set
				{
					base.PowerSharpParameters["IsExcludedFromOffboardMigration"] = value;
				}
			}

			public virtual bool IsFfoMigrationInProgress
			{
				set
				{
					base.PowerSharpParameters["IsFfoMigrationInProgress"] = value;
				}
			}

			public virtual bool TenantRelocationsAllowed
			{
				set
				{
					base.PowerSharpParameters["TenantRelocationsAllowed"] = value;
				}
			}

			public virtual Unlimited<int> MaxConcurrentMigrations
			{
				set
				{
					base.PowerSharpParameters["MaxConcurrentMigrations"] = value;
				}
			}

			public virtual bool IsProcessEhaMigratedMessagesEnabled
			{
				set
				{
					base.PowerSharpParameters["IsProcessEhaMigratedMessagesEnabled"] = value;
				}
			}

			public virtual bool AppsForOfficeEnabled
			{
				set
				{
					base.PowerSharpParameters["AppsForOfficeEnabled"] = value;
				}
			}

			public virtual bool? EwsEnabled
			{
				set
				{
					base.PowerSharpParameters["EwsEnabled"] = value;
				}
			}

			public virtual bool? EwsAllowOutlook
			{
				set
				{
					base.PowerSharpParameters["EwsAllowOutlook"] = value;
				}
			}

			public virtual bool? EwsAllowMacOutlook
			{
				set
				{
					base.PowerSharpParameters["EwsAllowMacOutlook"] = value;
				}
			}

			public virtual bool? EwsAllowEntourage
			{
				set
				{
					base.PowerSharpParameters["EwsAllowEntourage"] = value;
				}
			}

			public virtual EwsApplicationAccessPolicy? EwsApplicationAccessPolicy
			{
				set
				{
					base.PowerSharpParameters["EwsApplicationAccessPolicy"] = value;
				}
			}

			public virtual MultiValuedProperty<string> EwsAllowList
			{
				set
				{
					base.PowerSharpParameters["EwsAllowList"] = value;
				}
			}

			public virtual MultiValuedProperty<string> EwsBlockList
			{
				set
				{
					base.PowerSharpParameters["EwsBlockList"] = value;
				}
			}

			public virtual bool CalendarVersionStoreEnabled
			{
				set
				{
					base.PowerSharpParameters["CalendarVersionStoreEnabled"] = value;
				}
			}

			public virtual bool IsGuidPrefixedLegacyDnDisabled
			{
				set
				{
					base.PowerSharpParameters["IsGuidPrefixedLegacyDnDisabled"] = value;
				}
			}

			public virtual MultiValuedProperty<UMLanguage> UMAvailableLanguages
			{
				set
				{
					base.PowerSharpParameters["UMAvailableLanguages"] = value;
				}
			}

			public virtual bool IsMailboxForcedReplicationDisabled
			{
				set
				{
					base.PowerSharpParameters["IsMailboxForcedReplicationDisabled"] = value;
				}
			}

			public virtual int PreferredInternetCodePageForShiftJis
			{
				set
				{
					base.PowerSharpParameters["PreferredInternetCodePageForShiftJis"] = value;
				}
			}

			public virtual int RequiredCharsetCoverage
			{
				set
				{
					base.PowerSharpParameters["RequiredCharsetCoverage"] = value;
				}
			}

			public virtual int ByteEncoderTypeFor7BitCharsets
			{
				set
				{
					base.PowerSharpParameters["ByteEncoderTypeFor7BitCharsets"] = value;
				}
			}

			public virtual bool IsSyncPropertySetUpgradeAllowed
			{
				set
				{
					base.PowerSharpParameters["IsSyncPropertySetUpgradeAllowed"] = value;
				}
			}

			public virtual bool MapiHttpEnabled
			{
				set
				{
					base.PowerSharpParameters["MapiHttpEnabled"] = value;
				}
			}

			public virtual bool OAuth2ClientProfileEnabled
			{
				set
				{
					base.PowerSharpParameters["OAuth2ClientProfileEnabled"] = value;
				}
			}

			public virtual bool IntuneManagedStatus
			{
				set
				{
					base.PowerSharpParameters["IntuneManagedStatus"] = value;
				}
			}

			public virtual HybridConfigurationStatusFlags HybridConfigurationStatus
			{
				set
				{
					base.PowerSharpParameters["HybridConfigurationStatus"] = value;
				}
			}

			public virtual bool ACLableSyncedObjectEnabled
			{
				set
				{
					base.PowerSharpParameters["ACLableSyncedObjectEnabled"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}
		}
	}
}
