using System;

namespace Microsoft.Exchange.Management.Deployment
{
	internal class OrganizationSettingsSchema : ServicePlanElementSchema
	{
		// Note: this type is marked as 'beforefieldinit'.
		static OrganizationSettingsSchema()
		{
			string name = "ManagedFoldersPermissions";
			Type typeFromHandle = typeof(bool);
			ServicePlanSkus servicePlanSkus = ServicePlanSkus.Datacenter;
			FeatureCategory[] array = new FeatureCategory[2];
			array[0] = FeatureCategory.AdminPermissions;
			OrganizationSettingsSchema.ManagedFoldersPermissions = new FeatureDefinition(name, typeFromHandle, servicePlanSkus, array);
			OrganizationSettingsSchema.SearchMessagePermissions = new FeatureDefinition("SearchMessagePermissions", typeof(bool), ServicePlanSkus.All, new FeatureCategory[]
			{
				FeatureCategory.AdminPermissions,
				FeatureCategory.RoleGroupRoleAssignment
			});
			OrganizationSettingsSchema.ProfileUpdatePermissions = new FeatureDefinition("ProfileUpdatePermissions", FeatureCategory.AdminPermissions, typeof(bool), ServicePlanSkus.All);
			OrganizationSettingsSchema.OpenDomainProfileUpdatePermissions = new FeatureDefinition("OpenDomainProfileUpdatePermissions", FeatureCategory.AdminPermissions, typeof(bool), ServicePlanSkus.All);
			OrganizationSettingsSchema.JournalingRulesPermissions = new FeatureDefinition("JournalingRulesPermissions", typeof(bool), ServicePlanSkus.All, new FeatureCategory[]
			{
				FeatureCategory.AdminPermissions,
				FeatureCategory.RoleGroupRoleAssignment
			});
			OrganizationSettingsSchema.ModeratedRecipientsPermissions = new FeatureDefinition("ModeratedRecipientsPermissions", FeatureCategory.AdminPermissions, typeof(bool), ServicePlanSkus.All);
			string name2 = "TransportRulesPermissions";
			Type typeFromHandle2 = typeof(bool);
			ServicePlanSkus servicePlanSkus2 = ServicePlanSkus.All;
			FeatureCategory[] array2 = new FeatureCategory[2];
			array2[0] = FeatureCategory.AdminPermissions;
			OrganizationSettingsSchema.TransportRulesPermissions = new FeatureDefinition(name2, typeFromHandle2, servicePlanSkus2, array2);
			OrganizationSettingsSchema.UMAutoAttendantPermissions = new FeatureDefinition("UMAutoAttendantPermissions", FeatureCategory.AdminPermissions, typeof(bool), ServicePlanSkus.Datacenter);
			OrganizationSettingsSchema.ChangeMailboxPlanAssignmentPermissions = new FeatureDefinition("ChangeMailboxPlanAssignmentPermissions", FeatureCategory.AdminPermissions, typeof(bool), ServicePlanSkus.All);
			OrganizationSettingsSchema.TeamMailboxPermissions = new FeatureDefinition("TeamMailboxPermissions", typeof(bool), ServicePlanSkus.All, new FeatureCategory[]
			{
				FeatureCategory.AdminPermissions,
				FeatureCategory.RoleGroupRoleAssignment
			});
			OrganizationSettingsSchema.ConfigCustomizationsPermissions = new FeatureDefinition("ConfigCustomizationsPermissions", FeatureCategory.AdminPermissions, typeof(bool), ServicePlanSkus.Datacenter);
			OrganizationSettingsSchema.UMSMSMsgWaitingPermissions = new FeatureDefinition("UMSMSMsgWaitingPermissions", FeatureCategory.AdminPermissions, typeof(bool), ServicePlanSkus.Datacenter);
			OrganizationSettingsSchema.UMPBXPermissions = new FeatureDefinition("UMPBXPermissions", FeatureCategory.AdminPermissions, typeof(bool), ServicePlanSkus.Datacenter);
			OrganizationSettingsSchema.UMCloudServicePermissions = new FeatureDefinition("UMCloudServicePermissions", FeatureCategory.AdminPermissions, typeof(bool), ServicePlanSkus.Datacenter);
			OrganizationSettingsSchema.OutlookAnywherePermissions = new FeatureDefinition("OutlookAnywherePermissions", FeatureCategory.AdminPermissions, typeof(bool), ServicePlanSkus.All);
			OrganizationSettingsSchema.PopPermissions = new FeatureDefinition("PopPermissions", FeatureCategory.AdminPermissions, typeof(bool), ServicePlanSkus.All);
			OrganizationSettingsSchema.ImapPermissions = new FeatureDefinition("ImapPermissions", FeatureCategory.AdminPermissions, typeof(bool), ServicePlanSkus.All);
			OrganizationSettingsSchema.ActiveSyncPermissions = new FeatureDefinition("ActiveSyncPermissions", FeatureCategory.AdminPermissions, typeof(bool), ServicePlanSkus.All);
			OrganizationSettingsSchema.DeviceFiltersSetupEnabled = new FeatureDefinition("DeviceFiltersSetupEnabled", FeatureCategory.OrgWideConfiguration, typeof(bool), ServicePlanSkus.Datacenter);
			OrganizationSettingsSchema.EwsPermissions = new FeatureDefinition("EwsPermissions", FeatureCategory.AdminPermissions, typeof(bool), ServicePlanSkus.All);
			string name3 = "UMPermissions";
			Type typeFromHandle3 = typeof(bool);
			ServicePlanSkus servicePlanSkus3 = ServicePlanSkus.Datacenter;
			FeatureCategory[] array3 = new FeatureCategory[3];
			array3[0] = FeatureCategory.AdminPermissions;
			array3[1] = FeatureCategory.RoleGroupRoleAssignment;
			OrganizationSettingsSchema.UMPermissions = new FeatureDefinition(name3, typeFromHandle3, servicePlanSkus3, array3);
			OrganizationSettingsSchema.UMFaxPermissions = new FeatureDefinition("UMFaxPermissions", FeatureCategory.AdminPermissions, typeof(bool), ServicePlanSkus.Datacenter);
			OrganizationSettingsSchema.UMOutDialingPermissions = new FeatureDefinition("UMOutDialingPermissions", FeatureCategory.AdminPermissions, typeof(bool), ServicePlanSkus.Datacenter);
			OrganizationSettingsSchema.UMPersonalAutoAttendantPermissions = new FeatureDefinition("UMPersonalAutoAttendantPermissions", FeatureCategory.AdminPermissions, typeof(bool), ServicePlanSkus.Datacenter);
			OrganizationSettingsSchema.MailTipsPermissions = new FeatureDefinition("MailTipsPermissions", typeof(bool), ServicePlanSkus.All, new FeatureCategory[]
			{
				FeatureCategory.AdminPermissions,
				FeatureCategory.RoleGroupRoleAssignment
			});
			OrganizationSettingsSchema.OWAPermissions = new FeatureDefinition("OWAPermissions", FeatureCategory.AdminPermissions, typeof(bool), ServicePlanSkus.All);
			OrganizationSettingsSchema.SMSPermissions = new FeatureDefinition("SMSPermissions", FeatureCategory.AdminPermissions, typeof(bool), ServicePlanSkus.All);
			OrganizationSettingsSchema.SetHiddenFromAddressListPermissions = new FeatureDefinition("SetHiddenFromAddressListPermissions", FeatureCategory.AdminPermissions, typeof(bool), ServicePlanSkus.All);
			OrganizationSettingsSchema.NewUserPasswordManagementPermissions = new FeatureDefinition("NewUserPasswordManagementPermissions", FeatureCategory.AdminPermissions, typeof(bool), ServicePlanSkus.All);
			OrganizationSettingsSchema.NewUserResetPasswordOnNextLogonPermissions = new FeatureDefinition("NewUserResetPasswordOnNextLogonPermissions", FeatureCategory.AdminPermissions, typeof(bool), ServicePlanSkus.All);
			OrganizationSettingsSchema.UserLiveIdManagementPermissions = new FeatureDefinition("UserLiveIdManagementPermissions", FeatureCategory.AdminPermissions, typeof(bool), ServicePlanSkus.Datacenter);
			OrganizationSettingsSchema.MSOIdPermissions = new FeatureDefinition("MSOIdPermissions", FeatureCategory.AdminPermissions, typeof(bool), ServicePlanSkus.Datacenter);
			OrganizationSettingsSchema.ResetUserPasswordManagementPermissions = new FeatureDefinition("ResetUserPasswordManagementPermissions", FeatureCategory.AdminPermissions, typeof(bool), ServicePlanSkus.All);
			OrganizationSettingsSchema.UserMailboxAccessPermissions = new FeatureDefinition("UserMailboxAccessPermissions", FeatureCategory.AdminPermissions, typeof(bool), ServicePlanSkus.All);
			OrganizationSettingsSchema.MailboxRecoveryPermissions = new FeatureDefinition("MailboxRecoveryPermissions", FeatureCategory.AdminPermissions, typeof(bool), ServicePlanSkus.All);
			OrganizationSettingsSchema.PopSyncPermissions = new FeatureDefinition("PopSyncPermissions", FeatureCategory.AdminPermissions, typeof(bool), ServicePlanSkus.Datacenter);
			OrganizationSettingsSchema.AddSecondaryDomainPermissions = new FeatureDefinition("AddSecondaryDomainPermissions", FeatureCategory.AdminPermissions, typeof(bool), ServicePlanSkus.Datacenter);
			OrganizationSettingsSchema.HotmailSyncPermissions = new FeatureDefinition("HotmailSyncPermissions", FeatureCategory.AdminPermissions, typeof(bool), ServicePlanSkus.Datacenter);
			OrganizationSettingsSchema.ImapSyncPermissions = new FeatureDefinition("ImapSyncPermissions", FeatureCategory.AdminPermissions, typeof(bool), ServicePlanSkus.Datacenter);
			OrganizationSettingsSchema.OrganizationalAffinityPermissions = new FeatureDefinition("OrganizationalAffinityPermissions", FeatureCategory.AdminPermissions, typeof(bool), ServicePlanSkus.All);
			OrganizationSettingsSchema.MessageTrackingPermissions = new FeatureDefinition("MessageTrackingPermissions", typeof(bool), ServicePlanSkus.All, new FeatureCategory[]
			{
				FeatureCategory.OrgWideConfiguration,
				FeatureCategory.AdminPermissions,
				FeatureCategory.RoleGroupRoleAssignment
			});
			OrganizationSettingsSchema.ActiveSyncDeviceDataAccessPermissions = new FeatureDefinition("ActiveSyncDeviceDataAccessPermissions", FeatureCategory.AdminPermissions, typeof(bool), ServicePlanSkus.All);
			OrganizationSettingsSchema.MOWADeviceDataAccessPermissions = new FeatureDefinition("MOWADeviceDataAccessPermissions", FeatureCategory.AdminPermissions, typeof(bool), ServicePlanSkus.All);
			OrganizationSettingsSchema.GroupAsGroupSyncPermissions = new FeatureDefinition("GroupAsGroupSyncPermissions", FeatureCategory.AdminPermissions, typeof(bool), ServicePlanSkus.Datacenter);
			OrganizationSettingsSchema.LitigationHoldPermissions = new FeatureDefinition("LitigationHoldPermissions", FeatureCategory.AdminPermissions, typeof(bool), ServicePlanSkus.All);
			OrganizationSettingsSchema.ArchivePermissions = new FeatureDefinition("ArchivePermissions", FeatureCategory.AdminPermissions, typeof(bool), ServicePlanSkus.All);
			OrganizationSettingsSchema.PermissionManagementEnabled = new FeatureDefinition("PermissionManagementEnabled", typeof(bool), ServicePlanSkus.All, new FeatureCategory[]
			{
				FeatureCategory.RoleGroupRoleAssignment
			});
			string name4 = "PrivacyLink";
			Type typeFromHandle4 = typeof(string);
			ServicePlanSkus servicePlanSkus4 = ServicePlanSkus.Datacenter;
			FeatureCategory[] categories = new FeatureCategory[1];
			OrganizationSettingsSchema.PrivacyLink = new FeatureDefinition(name4, typeFromHandle4, servicePlanSkus4, categories);
			OrganizationSettingsSchema.ApplicationImpersonationEnabled = new FeatureDefinition("ApplicationImpersonationEnabled", typeof(bool), ServicePlanSkus.All, new FeatureCategory[]
			{
				FeatureCategory.RoleGroupRoleAssignment
			});
			OrganizationSettingsSchema.MailTipsEnabled = new FeatureDefinition("MailTipsEnabled", FeatureCategory.OrgWideConfiguration, typeof(bool), ServicePlanSkus.All);
			OrganizationSettingsSchema.AddressListsEnabled = new FeatureDefinition("AddressListsEnabled", FeatureCategory.OrgWideConfiguration, typeof(bool), ServicePlanSkus.All);
			OrganizationSettingsSchema.FastRecipientCountingDisabled = new FeatureDefinition("FastRecipientCountingDisabled", FeatureCategory.OrgWideConfiguration, typeof(bool), ServicePlanSkus.Datacenter);
			OrganizationSettingsSchema.OfflineAddressBookEnabled = new FeatureDefinition("OfflineAddressBookEnabled", FeatureCategory.OrgWideConfiguration, typeof(bool), ServicePlanSkus.All);
			OrganizationSettingsSchema.OpenDomainRoutingEnabled = new FeatureDefinition("OpenDomainRoutingEnabled", FeatureCategory.OrgWideConfiguration, typeof(bool), ServicePlanSkus.Datacenter);
			OrganizationSettingsSchema.AddOutlookAcceptedDomains = new FeatureDefinition("AddOutlookAcceptedDomains", FeatureCategory.OrgWideConfiguration, typeof(bool), ServicePlanSkus.Datacenter);
			OrganizationSettingsSchema.GALSyncEnabled = new FeatureDefinition("GALSyncEnabled", typeof(bool), ServicePlanSkus.Datacenter, new FeatureCategory[]
			{
				FeatureCategory.OrgWideConfiguration,
				FeatureCategory.RoleGroupRoleAssignment
			});
			OrganizationSettingsSchema.CommonConfiguration = new FeatureDefinition("CommonConfiguration", FeatureCategory.OrgWideConfiguration, typeof(bool), ServicePlanSkus.All);
			OrganizationSettingsSchema.HideAdminAccessWarningEnabled = new FeatureDefinition("HideAdminAccessWarningEnabled", FeatureCategory.OrgWideConfiguration, typeof(bool), ServicePlanSkus.All);
			OrganizationSettingsSchema.SkipToUAndParentalControlCheckEnabled = new FeatureDefinition("SkipToUAndParentalControlCheckEnabled", FeatureCategory.OrgWideConfiguration, typeof(bool), ServicePlanSkus.All);
			OrganizationSettingsSchema.SMTPAddressCheckWithAcceptedDomainEnabled = new FeatureDefinition("SMTPAddressCheckWithAcceptedDomainEnabled", FeatureCategory.OrgWideConfiguration, typeof(bool), ServicePlanSkus.All);
			OrganizationSettingsSchema.AutoReplyEnabled = new FeatureDefinition("AutoReplyEnabled", FeatureCategory.OrgWideConfiguration, typeof(bool), ServicePlanSkus.All);
			OrganizationSettingsSchema.AutoForwardEnabled = new FeatureDefinition("AutoForwardEnabled", FeatureCategory.OrgWideConfiguration, typeof(bool), ServicePlanSkus.All);
			OrganizationSettingsSchema.MSOSyncEnabled = new FeatureDefinition("MSOSyncEnabled", FeatureCategory.OrgWideConfiguration, typeof(bool), ServicePlanSkus.Datacenter);
			OrganizationSettingsSchema.AllowDeleteOfExternalIdentityUponRemove = new FeatureDefinition("AllowDeleteOfExternalIdentityUponRemove", FeatureCategory.OrgWideConfiguration, typeof(bool), ServicePlanSkus.Datacenter);
			string name5 = "SearchMessageEnabled";
			Type typeFromHandle5 = typeof(bool);
			ServicePlanSkus servicePlanSkus5 = ServicePlanSkus.All;
			FeatureCategory[] categories2 = new FeatureCategory[1];
			OrganizationSettingsSchema.SearchMessageEnabled = new FeatureDefinition(name5, typeFromHandle5, servicePlanSkus5, categories2);
			OrganizationSettingsSchema.OwaInstantMessagingType = new FeatureDefinition("OwaInstantMessagingType", FeatureCategory.OrgWideConfiguration, typeof(string), ServicePlanSkus.Datacenter);
			OrganizationSettingsSchema.RecipientManagementPermissions = new FeatureDefinition("RecipientManagementPermissions", FeatureCategory.AdminPermissions, typeof(bool), ServicePlanSkus.All);
			OrganizationSettingsSchema.DistributionListCountQuota = new FeatureDefinition("DistributionListCountQuota", FeatureCategory.OrgWideConfiguration, typeof(string), ServicePlanSkus.All);
			OrganizationSettingsSchema.MailboxCountQuota = new FeatureDefinition("MailboxCountQuota", FeatureCategory.OrgWideConfiguration, typeof(string), ServicePlanSkus.All);
			OrganizationSettingsSchema.MailUserCountQuota = new FeatureDefinition("MailUserCountQuota", FeatureCategory.OrgWideConfiguration, typeof(string), ServicePlanSkus.All);
			OrganizationSettingsSchema.ContactCountQuota = new FeatureDefinition("ContactCountQuota", FeatureCategory.OrgWideConfiguration, typeof(string), ServicePlanSkus.All);
			OrganizationSettingsSchema.TeamMailboxCountQuota = new FeatureDefinition("TeamMailboxCountQuota", FeatureCategory.OrgWideConfiguration, typeof(string), ServicePlanSkus.All);
			OrganizationSettingsSchema.PublicFolderMailboxCountQuota = new FeatureDefinition("PublicFolderMailboxCountQuota", FeatureCategory.OrgWideConfiguration, typeof(string), ServicePlanSkus.All);
			OrganizationSettingsSchema.MailPublicFolderCountQuota = new FeatureDefinition("MailPublicFolderCountQuota", FeatureCategory.OrgWideConfiguration, typeof(string), ServicePlanSkus.All);
			OrganizationSettingsSchema.SupervisionPermissions = new FeatureDefinition("SupervisionPermissions", typeof(bool), ServicePlanSkus.Datacenter, new FeatureCategory[]
			{
				FeatureCategory.AdminPermissions,
				FeatureCategory.RoleGroupRoleAssignment
			});
			OrganizationSettingsSchema.SupervisionEnabled = new FeatureDefinition("SupervisionEnabled", FeatureCategory.OrgWideConfiguration, typeof(bool), ServicePlanSkus.Datacenter);
			OrganizationSettingsSchema.TemplateTenant = new FeatureDefinition("TemplateTenant", FeatureCategory.OrgWideConfiguration, typeof(bool), ServicePlanSkus.Datacenter);
			OrganizationSettingsSchema.TransportRulesCollectionsEnabled = new FeatureDefinition("TransportRulesCollectionsEnabled", FeatureCategory.OrgWideConfiguration, typeof(bool), ServicePlanSkus.Datacenter);
			string name6 = "SyncAccountsEnabled";
			Type typeFromHandle6 = typeof(bool);
			ServicePlanSkus servicePlanSkus6 = ServicePlanSkus.Datacenter;
			FeatureCategory[] categories3 = new FeatureCategory[1];
			OrganizationSettingsSchema.SyncAccountsEnabled = new FeatureDefinition(name6, typeFromHandle6, servicePlanSkus6, categories3);
			OrganizationSettingsSchema.RecipientMailSubmissionRateQuota = new FeatureDefinition("RecipientMailSubmissionRateQuota", FeatureCategory.OrgWideConfiguration, typeof(string), ServicePlanSkus.All);
			OrganizationSettingsSchema.ImapMigrationPermissions = new FeatureDefinition("ImapMigrationPermissions", FeatureCategory.AdminPermissions, typeof(bool), ServicePlanSkus.Datacenter);
			OrganizationSettingsSchema.HotmailMigrationPermissions = new FeatureDefinition("HotmailMigrationPermissions", FeatureCategory.AdminPermissions, typeof(bool), ServicePlanSkus.Datacenter);
			OrganizationSettingsSchema.ExchangeMigrationPermissions = new FeatureDefinition("ExchangeMigrationPermissions", FeatureCategory.AdminPermissions, typeof(bool), ServicePlanSkus.Datacenter);
			OrganizationSettingsSchema.MultiEngineAVEnabled = new FeatureDefinition("MultiEngineAVEnabled", FeatureCategory.OrgWideConfiguration, typeof(bool), ServicePlanSkus.Datacenter);
			OrganizationSettingsSchema.CommonHydrateableObjectsSharedEnabled = new FeatureDefinition("CommonHydrateableObjectsSharedEnabled", FeatureCategory.OrgWideConfiguration, typeof(bool), ServicePlanSkus.Datacenter);
			OrganizationSettingsSchema.AdvancedHydrateableObjectsSharedEnabled = new FeatureDefinition("AdvancedHydrateableObjectsSharedEnabled", typeof(bool), ServicePlanSkus.Datacenter, new FeatureCategory[]
			{
				FeatureCategory.OrgWideConfiguration,
				FeatureCategory.RoleGroupRoleAssignment
			});
			OrganizationSettingsSchema.ShareableConfigurationEnabled = new FeatureDefinition("ShareableConfigurationEnabled", FeatureCategory.OrgWideConfiguration, typeof(bool), ServicePlanSkus.Datacenter);
			OrganizationSettingsSchema.IRMPremiumFeaturesPermissions = new FeatureDefinition("IRMPremiumFeaturesPermissions", typeof(bool), ServicePlanSkus.Datacenter, new FeatureCategory[]
			{
				FeatureCategory.AdminPermissions,
				FeatureCategory.RoleGroupRoleAssignment
			});
			OrganizationSettingsSchema.RBACManagementPermissions = new FeatureDefinition("RBACManagementPermissions", typeof(bool), ServicePlanSkus.All, new FeatureCategory[]
			{
				FeatureCategory.AdminPermissions
			});
			OrganizationSettingsSchema.PerMBXPlanRoleAssignmentPolicyEnabled = new FeatureDefinition("PerMBXPlanRoleAssignmentPolicyEnabled", typeof(bool), ServicePlanSkus.All, new FeatureCategory[]
			{
				FeatureCategory.OrgWideConfiguration,
				FeatureCategory.AdminPermissions
			});
			OrganizationSettingsSchema.RoleAssignmentPolicyPermissions = new FeatureDefinition("RoleAssignmentPolicyPermissions", typeof(bool), ServicePlanSkus.All, new FeatureCategory[]
			{
				FeatureCategory.AdminPermissions
			});
			OrganizationSettingsSchema.PerMBXPlanOWAPolicyEnabled = new FeatureDefinition("PerMBXPlanOWAPolicyEnabled", typeof(bool), ServicePlanSkus.Datacenter, new FeatureCategory[]
			{
				FeatureCategory.OrgWideConfiguration,
				FeatureCategory.AdminPermissions
			});
			OrganizationSettingsSchema.OWAMailboxPolicyPermissions = new FeatureDefinition("OWAMailboxPolicyPermissions", typeof(bool), ServicePlanSkus.All, new FeatureCategory[]
			{
				FeatureCategory.AdminPermissions
			});
			string name7 = "PerMBXPlanRetentionPolicyEnabled";
			Type typeFromHandle7 = typeof(bool);
			ServicePlanSkus servicePlanSkus7 = ServicePlanSkus.Datacenter;
			FeatureCategory[] categories4 = new FeatureCategory[1];
			OrganizationSettingsSchema.PerMBXPlanRetentionPolicyEnabled = new FeatureDefinition(name7, typeFromHandle7, servicePlanSkus7, categories4);
			string name8 = "ReducedOutOfTheBoxMrmTagsEnabled";
			Type typeFromHandle8 = typeof(bool);
			ServicePlanSkus servicePlanSkus8 = ServicePlanSkus.Datacenter;
			FeatureCategory[] categories5 = new FeatureCategory[1];
			OrganizationSettingsSchema.ReducedOutOfTheBoxMrmTagsEnabled = new FeatureDefinition(name8, typeFromHandle8, servicePlanSkus8, categories5);
			OrganizationSettingsSchema.AddressBookPolicyPermissions = new FeatureDefinition("AddressBookPolicyPermissions", typeof(bool), ServicePlanSkus.All, new FeatureCategory[]
			{
				FeatureCategory.AdminPermissions
			});
			OrganizationSettingsSchema.LicenseEnforcementEnabled = new FeatureDefinition("LicenseEnforcementEnabled", FeatureCategory.OrgWideConfiguration, typeof(bool), ServicePlanSkus.Datacenter);
			OrganizationSettingsSchema.PerimeterSafelistingUIMode = new FeatureDefinition("PerimeterSafelistingUIMode", FeatureCategory.OrgWideConfiguration, typeof(string), ServicePlanSkus.Datacenter);
			OrganizationSettingsSchema.ExchangeHostedFilteringAdminCenterAvailabilityEnabled = new FeatureDefinition("ExchangeHostedFilteringAdminCenterAvailabilityEnabled", FeatureCategory.OrgWideConfiguration, typeof(bool), ServicePlanSkus.Datacenter);
			OrganizationSettingsSchema.ExchangeHostedFilteringPerimeterEnabled = new FeatureDefinition("ExchangeHostedFilteringPerimeterEnabled", FeatureCategory.OrgWideConfiguration, typeof(bool), ServicePlanSkus.Datacenter);
			OrganizationSettingsSchema.ApplicationImpersonationRegularRoleAssignmentEnabled = new FeatureDefinition("ApplicationImpersonationRegularRoleAssignmentEnabled", FeatureCategory.RoleGroupRoleAssignment, typeof(bool), ServicePlanSkus.Datacenter);
			OrganizationSettingsSchema.MailboxImportExportRegularRoleAssignmentEnabled = new FeatureDefinition("MailboxImportExportRegularRoleAssignmentEnabled", FeatureCategory.RoleGroupRoleAssignment, typeof(bool), ServicePlanSkus.Datacenter);
			OrganizationSettingsSchema.MailboxQuotaPermissions = new FeatureDefinition("MailboxQuotaPermissions", FeatureCategory.AdminPermissions, typeof(bool), ServicePlanSkus.Datacenter);
			OrganizationSettingsSchema.MailboxSIRPermissions = new FeatureDefinition("MailboxSIRPermissions", FeatureCategory.AdminPermissions, typeof(bool), ServicePlanSkus.Datacenter);
			OrganizationSettingsSchema.PublicFoldersEnabled = new FeatureDefinition("PublicFoldersEnabled", typeof(bool), ServicePlanSkus.Datacenter, new FeatureCategory[]
			{
				FeatureCategory.OrgWideConfiguration,
				FeatureCategory.RoleGroupRoleAssignment,
				FeatureCategory.AdminPermissions
			});
			OrganizationSettingsSchema.QuarantineEnabled = new FeatureDefinition("QuarantineEnabled", typeof(bool), ServicePlanSkus.Datacenter, new FeatureCategory[]
			{
				FeatureCategory.OrgWideConfiguration,
				FeatureCategory.RoleGroupRoleAssignment,
				FeatureCategory.AdminPermissions
			});
			OrganizationSettingsSchema.UseServicePlanAsCounterInstanceName = new FeatureDefinition("UseServicePlanAsCounterInstanceName", FeatureCategory.OrgWideConfiguration, typeof(bool), ServicePlanSkus.Datacenter);
			OrganizationSettingsSchema.RIMRoleGroupEnabled = new FeatureDefinition("RIMRoleGroupEnabled", FeatureCategory.RoleGroupRoleAssignment, typeof(bool), ServicePlanSkus.Datacenter);
			OrganizationSettingsSchema.CalendarVersionStoreEnabled = new FeatureDefinition("CalendarVersionStoreEnabled", typeof(bool), ServicePlanSkus.Datacenter, new FeatureCategory[]
			{
				FeatureCategory.OrgWideConfiguration,
				FeatureCategory.AdminPermissions
			});
			OrganizationSettingsSchema.HostedSpamFilteringPolicyCustomizationEnabled = new FeatureDefinition("HostedSpamFilteringPolicyCustomizationEnabled", typeof(bool), ServicePlanSkus.Datacenter, new FeatureCategory[]
			{
				FeatureCategory.OrgWideConfiguration,
				FeatureCategory.AdminPermissions
			});
			OrganizationSettingsSchema.MalwareFilteringPolicyCustomizationEnabled = new FeatureDefinition("MalwareFilteringPolicyCustomizationEnabled", typeof(bool), ServicePlanSkus.All, new FeatureCategory[]
			{
				FeatureCategory.OrgWideConfiguration,
				FeatureCategory.AdminPermissions
			});
			OrganizationSettingsSchema.EXOCoreFeatures = new FeatureDefinition("EXOCoreFeatures", typeof(bool), ServicePlanSkus.All, new FeatureCategory[]
			{
				FeatureCategory.OrgWideConfiguration,
				FeatureCategory.RoleGroupRoleAssignment,
				FeatureCategory.AdminPermissions
			});
			OrganizationSettingsSchema.MessageTrace = new FeatureDefinition("MessageTrace", typeof(bool), ServicePlanSkus.All, new FeatureCategory[]
			{
				FeatureCategory.OrgWideConfiguration,
				FeatureCategory.AdminPermissions
			});
			OrganizationSettingsSchema.AcceptedDomains = new FeatureDefinition("AcceptedDomains", typeof(bool), ServicePlanSkus.All, new FeatureCategory[]
			{
				FeatureCategory.OrgWideConfiguration,
				FeatureCategory.AdminPermissions
			});
			OrganizationSettingsSchema.ServiceConnectors = new FeatureDefinition("ServiceConnectors", typeof(bool), ServicePlanSkus.All, new FeatureCategory[]
			{
				FeatureCategory.OrgWideConfiguration,
				FeatureCategory.AdminPermissions
			});
			OrganizationSettingsSchema.SoftDeletedFeatureManagementPermissions = new FeatureDefinition("SoftDeletedFeatureManagementPermissions", FeatureCategory.AdminPermissions, typeof(bool), ServicePlanSkus.Datacenter);
			OrganizationSettingsSchema.PilotEnabled = new FeatureDefinition("PilotEnabled", FeatureCategory.OrgWideConfiguration, typeof(bool), ServicePlanSkus.Datacenter);
			OrganizationSettingsSchema.DataLossPreventionEnabled = new FeatureDefinition("DataLossPreventionEnabled", typeof(bool), ServicePlanSkus.All, new FeatureCategory[]
			{
				FeatureCategory.OrgWideConfiguration,
				FeatureCategory.AdminPermissions
			});
		}

		public static readonly FeatureDefinition ManagedFoldersPermissions;

		public static readonly FeatureDefinition SearchMessagePermissions;

		public static readonly FeatureDefinition ProfileUpdatePermissions;

		public static readonly FeatureDefinition OpenDomainProfileUpdatePermissions;

		public static readonly FeatureDefinition JournalingRulesPermissions;

		public static readonly FeatureDefinition ModeratedRecipientsPermissions;

		public static readonly FeatureDefinition TransportRulesPermissions;

		public static readonly FeatureDefinition UMAutoAttendantPermissions;

		public static readonly FeatureDefinition ChangeMailboxPlanAssignmentPermissions;

		public static readonly FeatureDefinition TeamMailboxPermissions;

		public static readonly FeatureDefinition ConfigCustomizationsPermissions;

		public static readonly FeatureDefinition UMSMSMsgWaitingPermissions;

		public static readonly FeatureDefinition UMPBXPermissions;

		public static readonly FeatureDefinition UMCloudServicePermissions;

		public static readonly FeatureDefinition OutlookAnywherePermissions;

		public static readonly FeatureDefinition PopPermissions;

		public static readonly FeatureDefinition ImapPermissions;

		public static readonly FeatureDefinition ActiveSyncPermissions;

		public static readonly FeatureDefinition DeviceFiltersSetupEnabled;

		public static readonly FeatureDefinition EwsPermissions;

		public static readonly FeatureDefinition UMPermissions;

		public static readonly FeatureDefinition UMFaxPermissions;

		public static readonly FeatureDefinition UMOutDialingPermissions;

		public static readonly FeatureDefinition UMPersonalAutoAttendantPermissions;

		public static readonly FeatureDefinition MailTipsPermissions;

		public static readonly FeatureDefinition OWAPermissions;

		public static readonly FeatureDefinition SMSPermissions;

		public static readonly FeatureDefinition SetHiddenFromAddressListPermissions;

		public static readonly FeatureDefinition NewUserPasswordManagementPermissions;

		public static readonly FeatureDefinition NewUserResetPasswordOnNextLogonPermissions;

		public static readonly FeatureDefinition UserLiveIdManagementPermissions;

		public static readonly FeatureDefinition MSOIdPermissions;

		public static readonly FeatureDefinition ResetUserPasswordManagementPermissions;

		public static readonly FeatureDefinition UserMailboxAccessPermissions;

		public static readonly FeatureDefinition MailboxRecoveryPermissions;

		public static readonly FeatureDefinition PopSyncPermissions;

		public static readonly FeatureDefinition AddSecondaryDomainPermissions;

		public static readonly FeatureDefinition HotmailSyncPermissions;

		public static readonly FeatureDefinition ImapSyncPermissions;

		public static readonly FeatureDefinition OrganizationalAffinityPermissions;

		public static readonly FeatureDefinition MessageTrackingPermissions;

		public static readonly FeatureDefinition ActiveSyncDeviceDataAccessPermissions;

		public static readonly FeatureDefinition MOWADeviceDataAccessPermissions;

		public static readonly FeatureDefinition GroupAsGroupSyncPermissions;

		public static readonly FeatureDefinition LitigationHoldPermissions;

		public static readonly FeatureDefinition ArchivePermissions;

		public static readonly FeatureDefinition PermissionManagementEnabled;

		public static readonly FeatureDefinition PrivacyLink;

		public static readonly FeatureDefinition ApplicationImpersonationEnabled;

		public static readonly FeatureDefinition MailTipsEnabled;

		public static readonly FeatureDefinition AddressListsEnabled;

		public static readonly FeatureDefinition FastRecipientCountingDisabled;

		public static readonly FeatureDefinition OfflineAddressBookEnabled;

		public static readonly FeatureDefinition OpenDomainRoutingEnabled;

		public static readonly FeatureDefinition AddOutlookAcceptedDomains;

		public static readonly FeatureDefinition GALSyncEnabled;

		public static readonly FeatureDefinition CommonConfiguration;

		public static readonly FeatureDefinition HideAdminAccessWarningEnabled;

		public static readonly FeatureDefinition SkipToUAndParentalControlCheckEnabled;

		public static readonly FeatureDefinition SMTPAddressCheckWithAcceptedDomainEnabled;

		public static readonly FeatureDefinition AutoReplyEnabled;

		public static readonly FeatureDefinition AutoForwardEnabled;

		public static readonly FeatureDefinition MSOSyncEnabled;

		public static readonly FeatureDefinition AllowDeleteOfExternalIdentityUponRemove;

		public static readonly FeatureDefinition SearchMessageEnabled;

		public static readonly FeatureDefinition OwaInstantMessagingType;

		public static readonly FeatureDefinition RecipientManagementPermissions;

		public static readonly FeatureDefinition DistributionListCountQuota;

		public static readonly FeatureDefinition MailboxCountQuota;

		public static readonly FeatureDefinition MailUserCountQuota;

		public static readonly FeatureDefinition ContactCountQuota;

		public static readonly FeatureDefinition TeamMailboxCountQuota;

		public static readonly FeatureDefinition PublicFolderMailboxCountQuota;

		public static readonly FeatureDefinition MailPublicFolderCountQuota;

		public static readonly FeatureDefinition SupervisionPermissions;

		public static readonly FeatureDefinition SupervisionEnabled;

		public static readonly FeatureDefinition TemplateTenant;

		public static readonly FeatureDefinition TransportRulesCollectionsEnabled;

		public static readonly FeatureDefinition SyncAccountsEnabled;

		public static readonly FeatureDefinition RecipientMailSubmissionRateQuota;

		public static readonly FeatureDefinition ImapMigrationPermissions;

		public static readonly FeatureDefinition HotmailMigrationPermissions;

		public static readonly FeatureDefinition ExchangeMigrationPermissions;

		public static readonly FeatureDefinition MultiEngineAVEnabled;

		public static readonly FeatureDefinition CommonHydrateableObjectsSharedEnabled;

		public static readonly FeatureDefinition AdvancedHydrateableObjectsSharedEnabled;

		public static readonly FeatureDefinition ShareableConfigurationEnabled;

		public static readonly FeatureDefinition IRMPremiumFeaturesPermissions;

		public static readonly FeatureDefinition RBACManagementPermissions;

		public static readonly FeatureDefinition PerMBXPlanRoleAssignmentPolicyEnabled;

		public static readonly FeatureDefinition RoleAssignmentPolicyPermissions;

		public static readonly FeatureDefinition PerMBXPlanOWAPolicyEnabled;

		public static readonly FeatureDefinition OWAMailboxPolicyPermissions;

		public static readonly FeatureDefinition PerMBXPlanRetentionPolicyEnabled;

		public static readonly FeatureDefinition ReducedOutOfTheBoxMrmTagsEnabled;

		public static readonly FeatureDefinition AddressBookPolicyPermissions;

		public static readonly FeatureDefinition LicenseEnforcementEnabled;

		public static readonly FeatureDefinition PerimeterSafelistingUIMode;

		public static readonly FeatureDefinition ExchangeHostedFilteringAdminCenterAvailabilityEnabled;

		public static readonly FeatureDefinition ExchangeHostedFilteringPerimeterEnabled;

		public static readonly FeatureDefinition ApplicationImpersonationRegularRoleAssignmentEnabled;

		public static readonly FeatureDefinition MailboxImportExportRegularRoleAssignmentEnabled;

		public static readonly FeatureDefinition MailboxQuotaPermissions;

		public static readonly FeatureDefinition MailboxSIRPermissions;

		public static readonly FeatureDefinition PublicFoldersEnabled;

		public static readonly FeatureDefinition QuarantineEnabled;

		public static readonly FeatureDefinition UseServicePlanAsCounterInstanceName;

		public static readonly FeatureDefinition RIMRoleGroupEnabled;

		public static readonly FeatureDefinition CalendarVersionStoreEnabled;

		public static readonly FeatureDefinition HostedSpamFilteringPolicyCustomizationEnabled;

		public static readonly FeatureDefinition MalwareFilteringPolicyCustomizationEnabled;

		public static readonly FeatureDefinition EXOCoreFeatures;

		public static readonly FeatureDefinition MessageTrace;

		public static readonly FeatureDefinition AcceptedDomains;

		public static readonly FeatureDefinition ServiceConnectors;

		public static readonly FeatureDefinition SoftDeletedFeatureManagementPermissions;

		public static readonly FeatureDefinition PilotEnabled;

		public static readonly FeatureDefinition DataLossPreventionEnabled;
	}
}
