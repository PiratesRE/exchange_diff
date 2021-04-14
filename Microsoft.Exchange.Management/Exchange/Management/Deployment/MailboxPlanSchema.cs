using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.Deployment
{
	internal class MailboxPlanSchema : ServicePlanElementSchema
	{
		public static readonly FeatureDefinition AutoGroupPermissions = new FeatureDefinition("AutoGroupPermissions", FeatureCategory.MailboxPlanRoleAssignment, typeof(bool), ServicePlanSkus.All);

		public static readonly FeatureDefinition ActiveSyncPermissions = new FeatureDefinition("ActiveSyncPermissions", FeatureCategory.MailboxPlanPermissions, typeof(bool), ServicePlanSkus.All);

		public static readonly FeatureDefinition ImapPermissions = new FeatureDefinition("ImapPermissions", FeatureCategory.MailboxPlanPermissions, typeof(bool), ServicePlanSkus.All);

		public static readonly FeatureDefinition MailTipsPermissions = new FeatureDefinition("MailTipsPermissions", FeatureCategory.MailboxPlanPermissions, typeof(bool), ServicePlanSkus.All);

		public static readonly FeatureDefinition ModeratedRecipientsPermissions = new FeatureDefinition("ModeratedRecipientsPermissions", FeatureCategory.MailboxPlanPermissions, typeof(bool), ServicePlanSkus.All);

		public static readonly FeatureDefinition PopPermissions = new FeatureDefinition("PopPermissions", FeatureCategory.MailboxPlanPermissions, typeof(bool), ServicePlanSkus.All);

		public static readonly FeatureDefinition ProfileUpdatePermissions = new FeatureDefinition("ProfileUpdatePermissions", FeatureCategory.MailboxPlanPermissions, typeof(bool), ServicePlanSkus.All);

		public static readonly FeatureDefinition OpenDomainProfileUpdatePermissions = new FeatureDefinition("OpenDomainProfileUpdatePermissions", FeatureCategory.MailboxPlanPermissions, typeof(bool), ServicePlanSkus.All);

		public static readonly FeatureDefinition TeamMailboxPermissions = new FeatureDefinition("TeamMailboxPermissions", FeatureCategory.MailboxPlanPermissions, typeof(bool), ServicePlanSkus.All);

		public static readonly FeatureDefinition SMSPermissions = new FeatureDefinition("SMSPermissions", FeatureCategory.MailboxPlanPermissions, typeof(bool), ServicePlanSkus.All);

		public static readonly FeatureDefinition UMCloudServicePermissions = new FeatureDefinition("UMCloudServicePermissions", FeatureCategory.MailboxPlanPermissions, typeof(bool), ServicePlanSkus.Datacenter);

		public static readonly FeatureDefinition UMPermissions = new FeatureDefinition("UMPermissions", FeatureCategory.MailboxPlanPermissions, typeof(bool), ServicePlanSkus.Datacenter);

		public static readonly FeatureDefinition UMSMSMsgWaitingPermissions = new FeatureDefinition("UMSMSMsgWaitingPermissions", FeatureCategory.MailboxPlanPermissions, typeof(bool), ServicePlanSkus.Datacenter);

		public static readonly FeatureDefinition PopSyncPermissions = new FeatureDefinition("PopSyncPermissions", FeatureCategory.MailboxPlanPermissions, typeof(bool), ServicePlanSkus.Datacenter);

		public static readonly FeatureDefinition EXOCoreFeatures = new FeatureDefinition("EXOCoreFeatures", FeatureCategory.MailboxPlanPermissions, typeof(bool), ServicePlanSkus.All);

		public static readonly FeatureDefinition HotmailSyncPermissions = new FeatureDefinition("HotmailSyncPermissions", FeatureCategory.MailboxPlanPermissions, typeof(bool), ServicePlanSkus.Datacenter);

		public static readonly FeatureDefinition ImapSyncPermissions = new FeatureDefinition("ImapSyncPermissions", FeatureCategory.MailboxPlanPermissions, typeof(bool), ServicePlanSkus.Datacenter);

		public static readonly FeatureDefinition ResetUserPasswordManagementPermissions = new FeatureDefinition("ResetUserPasswordManagementPermissions", FeatureCategory.MailboxPlanPermissions, typeof(bool), ServicePlanSkus.All);

		public static readonly FeatureDefinition UserMailboxAccessPermissions = new FeatureDefinition("UserMailboxAccessPermissions", FeatureCategory.MailboxPlanPermissions, typeof(bool), ServicePlanSkus.All);

		public static readonly FeatureDefinition OrganizationalAffinityPermissions = new FeatureDefinition("OrganizationalAffinityPermissions", FeatureCategory.MailboxPlanPermissions, typeof(bool), ServicePlanSkus.All);

		public static readonly FeatureDefinition MessageTrackingPermissions = new FeatureDefinition("MessageTrackingPermissions", FeatureCategory.MailboxPlanPermissions, typeof(bool), ServicePlanSkus.All);

		public static readonly FeatureDefinition ActiveSyncDeviceDataAccessPermissions = new FeatureDefinition("ActiveSyncDeviceDataAccessPermissions", FeatureCategory.MailboxPlanPermissions, typeof(bool), ServicePlanSkus.All);

		public static readonly FeatureDefinition MOWADeviceDataAccessPermissions = new FeatureDefinition("MOWADeviceDataAccessPermissions", FeatureCategory.MailboxPlanPermissions, typeof(bool), ServicePlanSkus.All);

		public static readonly FeatureDefinition ViewSupervisionListPermissions = new FeatureDefinition("ViewSupervisionListPermissions", FeatureCategory.MailboxPlanPermissions, typeof(bool), ServicePlanSkus.Datacenter);

		public static readonly FeatureDefinition MaxReceiveTransportQuota = new FeatureDefinition("MaxReceiveTransportQuota", FeatureCategory.MailboxPlanConfiguration, typeof(string), ServicePlanSkus.All);

		public static readonly FeatureDefinition MaxSendTransportQuota = new FeatureDefinition("MaxSendTransportQuota", FeatureCategory.MailboxPlanConfiguration, typeof(string), ServicePlanSkus.All);

		public static readonly FeatureDefinition MaxRecipientsTransportQuota = new FeatureDefinition("MaxRecipientsTransportQuota", FeatureCategory.MailboxPlanConfiguration, typeof(string), ServicePlanSkus.All);

		public static readonly FeatureDefinition ProhibitSendReceiveMaiboxQuota = new FeatureDefinition("ProhibitSendReceiveMaiboxQuota", FeatureCategory.MailboxPlanConfiguration, typeof(string), ServicePlanSkus.All);

		public static readonly FeatureDefinition ArchiveQuota = new FeatureDefinition("ArchiveQuota", FeatureCategory.MailboxPlanConfiguration, typeof(string), ServicePlanSkus.All);

		public static readonly FeatureDefinition ImapEnabled = new FeatureDefinition("ImapEnabled", FeatureCategory.MailboxPlanConfiguration, typeof(bool), ServicePlanSkus.All);

		public static readonly FeatureDefinition PopEnabled = new FeatureDefinition("PopEnabled", FeatureCategory.MailboxPlanConfiguration, typeof(bool), ServicePlanSkus.All);

		public static readonly FeatureDefinition ShowInAddressListsEnabled = new FeatureDefinition("ShowInAddressListsEnabled", FeatureCategory.MailboxPlanConfiguration, typeof(bool), ServicePlanSkus.All);

		public static readonly FeatureDefinition OutlookAnywhereEnabled = new FeatureDefinition("OutlookAnywhereEnabled", FeatureCategory.MailboxPlanConfiguration, typeof(bool), ServicePlanSkus.All);

		public static readonly FeatureDefinition ActiveSyncEnabled = new FeatureDefinition("ActiveSyncEnabled", FeatureCategory.MailboxPlanConfiguration, typeof(bool), ServicePlanSkus.All);

		public static readonly FeatureDefinition MOWAEnabled = new FeatureDefinition("MOWAEnabled", FeatureCategory.MailboxPlanConfiguration, typeof(bool), ServicePlanSkus.All);

		public static readonly FeatureDefinition EwsEnabled = new FeatureDefinition("EwsEnabled", FeatureCategory.MailboxPlanConfiguration, typeof(bool), ServicePlanSkus.All);

		public static readonly FeatureDefinition OrganizationalQueryBaseDNEnabled = new FeatureDefinition("OrganizationalQueryBaseDNEnabled", FeatureCategory.MailboxPlanConfiguration, typeof(bool), ServicePlanSkus.All);

		public static readonly FeatureDefinition UMEnabled = new FeatureDefinition("UMEnabled", FeatureCategory.MailboxPlanConfiguration, typeof(UMDeploymentModeOptions), ServicePlanSkus.Datacenter);

		public static readonly FeatureDefinition SkipResetPasswordOnFirstLogonEnabled = new FeatureDefinition("SkipResetPasswordOnFirstLogonEnabled", FeatureCategory.MailboxPlanSatellite, typeof(bool), ServicePlanSkus.All);

		public static readonly FeatureDefinition SyncAccountsEnabled = new FeatureDefinition("SyncAccountsEnabled", FeatureCategory.MailboxPlanSatellite, typeof(bool), ServicePlanSkus.Datacenter);

		public static readonly FeatureDefinition SyncAccountsSyncNowEnabled = new FeatureDefinition("SyncAccountsSyncNowEnabled", FeatureCategory.MailboxPlanSatellite, typeof(bool), ServicePlanSkus.Datacenter);

		public static readonly FeatureDefinition SyncAccountsMaxAccountsQuota = new FeatureDefinition("SyncAccountsMaxAccountsQuota", FeatureCategory.MailboxPlanSatellite, typeof(string), ServicePlanSkus.Datacenter);

		public static readonly FeatureDefinition SyncAccountsPollingInterval = new FeatureDefinition("SyncAccountsPollingInterval", FeatureCategory.MailboxPlanSatellite, typeof(string), ServicePlanSkus.Datacenter);

		public static readonly FeatureDefinition SyncAccountsTimeBeforeInactive = new FeatureDefinition("SyncAccountsTimeBeforeInactive", FeatureCategory.MailboxPlanSatellite, typeof(string), ServicePlanSkus.Datacenter);

		public static readonly FeatureDefinition SyncAccountsTimeBeforeDormant = new FeatureDefinition("SyncAccountsTimeBeforeDormant", FeatureCategory.MailboxPlanSatellite, typeof(string), ServicePlanSkus.Datacenter);

		public static readonly FeatureDefinition SkuCapability = new FeatureDefinition("SkuCapability", FeatureCategory.MailboxPlanConfiguration, typeof(Capability), ServicePlanSkus.Datacenter);

		public static readonly FeatureDefinition SingleItemRecoveryEnabled = new FeatureDefinition("SingleItemRecoveryEnabled", FeatureCategory.MailboxPlanConfiguration, typeof(bool), ServicePlanSkus.Datacenter);
	}
}
