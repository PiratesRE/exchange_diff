using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal sealed class TenantOrganizationPresentationObjectSchema : ADPresentationSchema
	{
		internal override ADObjectSchema GetParentSchema()
		{
			return ObjectSchema.GetInstance<ExchangeConfigurationUnitSchema>();
		}

		public new static readonly ADPropertyDefinition OrganizationId = ExchangeConfigurationUnitSchema.OrganizationId;

		public static readonly ADPropertyDefinition OrganizationStatus = ExchangeConfigurationUnitSchema.OrganizationStatus;

		public static readonly ADPropertyDefinition WhenOrganizationStatusSet = ExchangeConfigurationUnitSchema.WhenOrganizationStatusSet;

		public static readonly ADPropertyDefinition IsUpgradingOrganization = OrganizationSchema.IsUpgradingOrganization;

		public static readonly ADPropertyDefinition IsPilotingOrganization = OrganizationSchema.IsPilotingOrganization;

		public static readonly ADPropertyDefinition IsUpdatingServicePlan = OrganizationSchema.IsUpdatingServicePlan;

		public static readonly ADPropertyDefinition IsFfoMigrationInProgress = OrganizationSchema.IsFfoMigrationInProgress;

		public static readonly ADPropertyDefinition IsUpgradeOperationInProgress = OrganizationSchema.IsUpgradeOperationInProgress;

		public static readonly ADPropertyDefinition ServicePlan = ExchangeConfigurationUnitSchema.ServicePlan;

		public static readonly ADPropertyDefinition TargetServicePlan = ExchangeConfigurationUnitSchema.TargetServicePlan;

		public static readonly ADPropertyDefinition ProgramId = ExchangeConfigurationUnitSchema.ProgramId;

		public static readonly ADPropertyDefinition OfferId = ExchangeConfigurationUnitSchema.OfferId;

		public static readonly ADPropertyDefinition ManagementSiteLink = ExchangeConfigurationUnitSchema.ManagementSiteLink;

		public static readonly ADPropertyDefinition IsFederated = OrganizationSchema.IsFederated;

		public static readonly ADPropertyDefinition SkipToUAndParentalControlCheck = OrganizationSchema.SkipToUAndParentalControlCheck;

		public static readonly ADPropertyDefinition HideAdminAccessWarning = OrganizationSchema.HideAdminAccessWarning;

		public static readonly ADPropertyDefinition ShowAdminAccessWarning = OrganizationSchema.ShowAdminAccessWarning;

		public static readonly ADPropertyDefinition SMTPAddressCheckWithAcceptedDomain = OrganizationSchema.SMTPAddressCheckWithAcceptedDomain;

		public static readonly ADPropertyDefinition ExternalDirectoryOrganizationId = ExchangeConfigurationUnitSchema.ExternalDirectoryOrganizationId;

		public static readonly ADPropertyDefinition SupportedSharedConfigurations = OrganizationSchema.SupportedSharedConfigurations;

		public static readonly ADPropertyDefinition SharedConfigurationInfo = OrganizationSchema.SharedConfigurationInfo;

		public static readonly ADPropertyDefinition EnableAsSharedConfiguration = OrganizationSchema.EnableAsSharedConfiguration;

		public static readonly ADPropertyDefinition ImmutableConfiguration = OrganizationSchema.ImmutableConfiguration;

		public static readonly ADPropertyDefinition IsLicensingEnforced = OrganizationSchema.IsLicensingEnforced;

		public static readonly ADPropertyDefinition IsTenantAccessBlocked = OrganizationSchema.IsTenantAccessBlocked;

		public static readonly ADPropertyDefinition IsDehydrated = OrganizationSchema.IsDehydrated;

		public static readonly ADPropertyDefinition IsStaticConfigurationShared = OrganizationSchema.IsStaticConfigurationShared;

		public static readonly ADPropertyDefinition IsSharingConfiguration = OrganizationSchema.IsSharingConfiguration;

		public static readonly ADPropertyDefinition IsTemplateTenant = OrganizationSchema.IsTemplateTenant;

		public static readonly ADPropertyDefinition ExcludedFromBackSync = OrganizationSchema.ExcludedFromBackSync;

		public static readonly ADPropertyDefinition ExcludedFromForwardSyncEDU2BPOS = OrganizationSchema.ExcludedFromForwardSyncEDU2BPOS;

		public static readonly ADPropertyDefinition MSOSyncEnabled = OrganizationSchema.MSOSyncEnabled;

		public static readonly ADPropertyDefinition MaxAddressBookPolicies = OrganizationSchema.MaxAddressBookPolicies;

		public static readonly ADPropertyDefinition MaxOfflineAddressBooks = OrganizationSchema.MaxOfflineAddressBooks;

		public static readonly ADPropertyDefinition UseServicePlanAsCounterInstanceName = OrganizationSchema.UseServicePlanAsCounterInstanceName;

		public static readonly ADPropertyDefinition AllowDeleteOfExternalIdentityUponRemove = OrganizationSchema.AllowDeleteOfExternalIdentityUponRemove;

		public static readonly ADPropertyDefinition ExchangeUpgradeBucket = ExchangeConfigurationUnitSchema.ExchangeUpgradeBucket;

		public static readonly ADPropertyDefinition AdminDisplayVersion = OrganizationSchema.AdminDisplayVersion;

		public static readonly ADPropertyDefinition DefaultPublicFolderProhibitPostQuota = OrganizationSchema.DefaultPublicFolderProhibitPostQuota;

		public static readonly ADPropertyDefinition DefaultPublicFolderIssueWarningQuota = OrganizationSchema.DefaultPublicFolderIssueWarningQuota;

		public static readonly ADPropertyDefinition AsynchronousOperationIds = OrganizationSchema.AsynchronousOperationIds;

		public static readonly ADPropertyDefinition IsDirSyncRunning = OrganizationSchema.IsDirSyncRunning;

		public static readonly ADPropertyDefinition IsDirSyncStatusPending = OrganizationSchema.IsDirSyncStatusPending;

		public static readonly ADPropertyDefinition DirSyncStatus = OrganizationSchema.DirSyncStatus;

		public static readonly ADPropertyDefinition DirSyncServiceInstance = ExchangeConfigurationUnitSchema.DirSyncServiceInstance;

		public static readonly ADPropertyDefinition SoftDeletedFeatureStatus = OrganizationSchema.SoftDeletedFeatureStatus;

		public static readonly ADPropertyDefinition UpgradeStatus = OrganizationSchema.UpgradeStatus;

		public static readonly ADPropertyDefinition UpgradeRequest = OrganizationSchema.UpgradeRequest;

		public static readonly ADPropertyDefinition UpgradeConstraints = OrganizationSchema.UpgradeConstraints;

		public static readonly ADPropertyDefinition UpgradeMessage = OrganizationSchema.UpgradeMessage;

		public static readonly ADPropertyDefinition UpgradeDetails = OrganizationSchema.UpgradeDetails;

		public static readonly ADPropertyDefinition UpgradeStage = OrganizationSchema.UpgradeStage;

		public static readonly ADPropertyDefinition UpgradeStageTimeStamp = OrganizationSchema.UpgradeStageTimeStamp;

		public static readonly ADPropertyDefinition UpgradeE14MbxCountForCurrentStage = OrganizationSchema.UpgradeE14MbxCountForCurrentStage;

		public static readonly ADPropertyDefinition UpgradeE14RequestCountForCurrentStage = OrganizationSchema.UpgradeE14RequestCountForCurrentStage;

		public static readonly ADPropertyDefinition UpgradeLastE14CountsUpdateTime = OrganizationSchema.UpgradeLastE14CountsUpdateTime;

		public static readonly ADPropertyDefinition UpgradeConstraintsDisabled = OrganizationSchema.UpgradeConstraintsDisabled;

		public static readonly ADPropertyDefinition UpgradeUnitsOverride = OrganizationSchema.UpgradeUnitsOverride;

		public static readonly ADPropertyDefinition DefaultMovePriority = OrganizationSchema.DefaultMovePriority;

		public static readonly ADPropertyDefinition OrganizationName = ExchangeConfigurationUnitSchema.OrganizationName;

		public static readonly ADPropertyDefinition CompanyTags = OrganizationSchema.CompanyTags;

		public static readonly ADPropertyDefinition MailboxRelease = OrganizationSchema.MailboxRelease;

		public static readonly ADPropertyDefinition PreviousMailboxRelease = OrganizationSchema.PreviousMailboxRelease;

		public static readonly ADPropertyDefinition PilotMailboxRelease = OrganizationSchema.PilotMailboxRelease;

		public static readonly ADPropertyDefinition ReleaseTrack = OrganizationSchema.ReleaseTrack;

		public static readonly ADPropertyDefinition PersistedCapabilities = OrganizationSchema.PersistedCapabilities;

		public static readonly ADPropertyDefinition Location = OrganizationSchema.Location;

		public static readonly ADPropertyDefinition RelocationConstraints = OrganizationSchema.RelocationConstraints;

		public static readonly ADPropertyDefinition OriginatedInDifferentForest = OrganizationSchema.OriginatedInDifferentForest;

		public static readonly ADPropertyDefinition IOwnMigrationTenant = ExchangeConfigurationUnitSchema.IOwnMigrationTenant;

		public static readonly ADPropertyDefinition IOwnMigrationStatus = ExchangeConfigurationUnitSchema.IOwnMigrationStatus;

		public static readonly ADPropertyDefinition IOwnMigrationStatusReport = ExchangeConfigurationUnitSchema.IOwnMigrationStatusReport;

		public static readonly ADPropertyDefinition AdminDisplayName = ADConfigurationObjectSchema.AdminDisplayName;
	}
}
