using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal class MailboxSchema : MailEnabledOrgPersonSchema
	{
		internal override ADObjectSchema GetParentSchema()
		{
			return ObjectSchema.GetInstance<ADUserSchema>();
		}

		public static readonly ADPropertyDefinition AdminDisplayVersion = ADUserSchema.AdminDisplayVersion;

		public static readonly ADPropertyDefinition Database = ADMailboxRecipientSchema.Database;

		public static readonly ADPropertyDefinition PreviousDatabase = ADUserSchema.PreviousDatabase;

		public static readonly ADPropertyDefinition UseDatabaseRetentionDefaults = ADUserSchema.UseDatabaseRetentionDefaults;

		public static readonly ADPropertyDefinition RetainDeletedItemsUntilBackup = ADUserSchema.RetainDeletedItemsUntilBackup;

		public static readonly ADPropertyDefinition DeliverToMailboxAndForward = ADMailboxRecipientSchema.DeliverToMailboxAndForward;

		public static readonly ADPropertyDefinition LitigationHoldEnabled = ADUserSchema.LitigationHoldEnabled;

		public static readonly ADPropertyDefinition LitigationHoldDuration = ADRecipientSchema.LitigationHoldDuration;

		public static readonly ADPropertyDefinition SingleItemRecoveryEnabled = ADUserSchema.SingleItemRecoveryEnabled;

		public static readonly ADPropertyDefinition ElcExpirationSuspensionEnabled = ADUserSchema.ElcExpirationSuspensionEnabled;

		public static readonly ADPropertyDefinition ElcExpirationSuspensionEndDate = ADUserSchema.ElcExpirationSuspensionEndDate;

		public static readonly ADPropertyDefinition ElcExpirationSuspensionStartDate = ADUserSchema.ElcExpirationSuspensionStartDate;

		public static readonly ADPropertyDefinition RetentionComment = ADUserSchema.RetentionComment;

		public static readonly ADPropertyDefinition RetentionUrl = ADUserSchema.RetentionUrl;

		public static readonly ADPropertyDefinition LitigationHoldDate = ADUserSchema.LitigationHoldDate;

		public static readonly ADPropertyDefinition LitigationHoldOwner = ADUserSchema.LitigationHoldOwner;

		public static readonly ADPropertyDefinition ManagedFolderMailboxPolicy = ADUserSchema.ManagedFolderMailboxPolicy;

		public static readonly ADPropertyDefinition RetentionPolicy = ADUserSchema.RetentionPolicy;

		public static readonly ADPropertyDefinition AddressBookPolicy = ADRecipientSchema.AddressBookPolicy;

		public static readonly ADPropertyDefinition ShouldUseDefaultRetentionPolicy = ADUserSchema.ShouldUseDefaultRetentionPolicy;

		public static readonly ADPropertyDefinition MessageTrackingReadStatusDisabled = ADRecipientSchema.MessageTrackingReadStatusDisabled;

		public static readonly ADPropertyDefinition CalendarRepairDisabled = ADUserSchema.CalendarRepairDisabled;

		public static readonly ADPropertyDefinition ExchangeGuid = ADMailboxRecipientSchema.ExchangeGuid;

		public static readonly ADPropertyDefinition MailboxContainerGuid = ADUserSchema.MailboxContainerGuid;

		public static readonly ADPropertyDefinition AggregatedMailboxGuids = ADUserSchema.AggregatedMailboxGuids;

		public static readonly ADPropertyDefinition MailboxLocations = ADRecipientSchema.MailboxLocations;

		public static readonly ADPropertyDefinition UnifiedMailbox = IADMailStorageSchema.UnifiedMailbox;

		public static readonly ADPropertyDefinition ExchangeSecurityDescriptor = ADMailboxRecipientSchema.ExchangeSecurityDescriptor;

		public static readonly ADPropertyDefinition ExchangeUserAccountControl = ADUserSchema.ExchangeUserAccountControl;

		public static readonly ADPropertyDefinition ExternalOofOptions = ADMailboxRecipientSchema.ExternalOofOptions;

		public static readonly ADPropertyDefinition ForwardingAddress = ADRecipientSchema.ForwardingAddress;

		public static readonly ADPropertyDefinition ForwardingSmtpAddress = ADRecipientSchema.ForwardingSmtpAddress;

		public static readonly ADPropertyDefinition RetainDeletedItemsFor = ADMailboxRecipientSchema.RetainDeletedItemsFor;

		public static readonly ADPropertyDefinition IsMailboxEnabled = ADMailboxRecipientSchema.IsMailboxEnabled;

		public static readonly ADPropertyDefinition LanguagesRaw = ADOrgPersonSchema.LanguagesRaw;

		public static readonly ADPropertyDefinition Languages = ADOrgPersonSchema.Languages;

		public static readonly ADPropertyDefinition OfflineAddressBook = ADMailboxRecipientSchema.OfflineAddressBook;

		public static readonly ADPropertyDefinition ProhibitSendQuota = ADMailboxRecipientSchema.ProhibitSendQuota;

		public static readonly ADPropertyDefinition ProhibitSendReceiveQuota = ADMailboxRecipientSchema.ProhibitSendReceiveQuota;

		public static readonly ADPropertyDefinition RecoverableItemsQuota = ADUserSchema.RecoverableItemsQuota;

		public static readonly ADPropertyDefinition RecoverableItemsWarningQuota = ADUserSchema.RecoverableItemsWarningQuota;

		public static readonly ADPropertyDefinition CalendarLoggingQuota = ADUserSchema.CalendarLoggingQuota;

		public static readonly ADPropertyDefinition ProtocolSettings = ADRecipientSchema.ReadOnlyProtocolSettings;

		public static readonly ADPropertyDefinition DowngradeHighPriorityMessagesEnabled = ADUserSchema.DowngradeHighPriorityMessagesEnabled;

		public static readonly ADPropertyDefinition RecipientLimits = ADRecipientSchema.RecipientLimits;

		public static readonly ADPropertyDefinition IsLinked = ADRecipientSchema.IsLinked;

		public static readonly ADPropertyDefinition IsRootPublicFolderMailbox = ADRecipientSchema.IsRootPublicFolderMailbox;

		public static readonly ADPropertyDefinition IsShared = ADRecipientSchema.IsShared;

		public static readonly ADPropertyDefinition IsResource = ADRecipientSchema.IsResource;

		public static readonly ADPropertyDefinition MasterAccountSid = ADRecipientSchema.MasterAccountSid;

		public static readonly ADPropertyDefinition LinkedMasterAccount = ADRecipientSchema.LinkedMasterAccount;

		public static readonly ADPropertyDefinition ResetPasswordOnNextLogon = ADUserSchema.ResetPasswordOnNextLogon;

		public static readonly ADPropertyDefinition ResourceCapacity = ADRecipientSchema.ResourceCapacity;

		public static readonly ADPropertyDefinition ResourceCustom = ADRecipientSchema.ResourceCustom;

		public static readonly ADPropertyDefinition ResourceType = ADRecipientSchema.ResourceType;

		public static readonly ADPropertyDefinition SamAccountName = ADMailboxRecipientSchema.SamAccountName;

		public static readonly ADPropertyDefinition SCLDeleteThreshold = ADRecipientSchema.SCLDeleteThreshold;

		public static readonly ADPropertyDefinition SCLDeleteEnabled = ADRecipientSchema.SCLDeleteEnabled;

		public static readonly ADPropertyDefinition SCLRejectThreshold = ADRecipientSchema.SCLRejectThreshold;

		public static readonly ADPropertyDefinition SCLRejectEnabled = ADRecipientSchema.SCLRejectEnabled;

		public static readonly ADPropertyDefinition SCLQuarantineThreshold = ADRecipientSchema.SCLQuarantineThreshold;

		public static readonly ADPropertyDefinition SCLQuarantineEnabled = ADRecipientSchema.SCLQuarantineEnabled;

		public static readonly ADPropertyDefinition SCLJunkThreshold = ADRecipientSchema.SCLJunkThreshold;

		public static readonly ADPropertyDefinition SCLJunkEnabled = ADRecipientSchema.SCLJunkEnabled;

		public static readonly ADPropertyDefinition AntispamBypassEnabled = ADRecipientSchema.AntispamBypassEnabled;

		public static readonly ADPropertyDefinition ServerLegacyDN = ADMailboxRecipientSchema.ServerLegacyDN;

		public static readonly ADPropertyDefinition ServerName = ADMailboxRecipientSchema.ServerName;

		public static readonly ADPropertyDefinition UseDatabaseQuotaDefaults = ADMailboxRecipientSchema.UseDatabaseQuotaDefaults;

		public static readonly ADPropertyDefinition IssueWarningQuota = ADMailboxRecipientSchema.IssueWarningQuota;

		public static readonly ADPropertyDefinition RulesQuota = ADMailboxRecipientSchema.RulesQuota;

		public static readonly ADPropertyDefinition Office = ADUserSchema.Office;

		public static readonly ADPropertyDefinition UserPrincipalName = ADUserSchema.UserPrincipalName;

		public static readonly ADPropertyDefinition NetID = ADUserSchema.NetID;

		public static readonly ADPropertyDefinition OriginalNetID = ADUserSchema.OriginalNetID;

		public static readonly ADPropertyDefinition UMEnabled = ADUserSchema.UMEnabled;

		public static readonly ADPropertyDefinition MaxSafeSenders = ADUserSchema.MaxSafeSenders;

		public static readonly ADPropertyDefinition MaxBlockedSenders = ADUserSchema.MaxBlockedSenders;

		public static readonly ADPropertyDefinition WindowsLiveID = ADRecipientSchema.WindowsLiveID;

		public static readonly ADPropertyDefinition MailboxPlan = ADRecipientSchema.MailboxPlan;

		public static readonly ADPropertyDefinition RoleAssignmentPolicy = ADRecipientSchema.RoleAssignmentPolicy;

		public static readonly ADPropertyDefinition ThrottlingPolicy = ADRecipientSchema.ThrottlingPolicy;

		public static readonly ADPropertyDefinition ArchiveDatabase = ADUserSchema.ArchiveDatabase;

		public static readonly ADPropertyDefinition ArchiveGuid = ADUserSchema.ArchiveGuid;

		public static readonly ADPropertyDefinition ArchiveName = ADUserSchema.ArchiveName;

		public static readonly ADPropertyDefinition ArchiveQuota = ADUserSchema.ArchiveQuota;

		public static readonly ADPropertyDefinition ArchiveWarningQuota = ADUserSchema.ArchiveWarningQuota;

		public static readonly ADPropertyDefinition ArchiveDomain = ADUserSchema.ArchiveDomain;

		public static readonly ADPropertyDefinition ArchiveStatus = ADUserSchema.ArchiveStatus;

		public static readonly ADPropertyDefinition ArchiveState = ADUserSchema.ArchiveState;

		public static readonly ADPropertyDefinition IsAuxMailbox = ADUserSchema.IsAuxMailbox;

		public static readonly ADPropertyDefinition AuxMailboxParentObjectId = ADUserSchema.AuxMailboxParentObjectId;

		public static readonly ADPropertyDefinition ChildAuxMailboxObjectIds = ADUserSchema.AuxMailboxParentObjectIdBL;

		public static readonly ADPropertyDefinition MailboxRelationType = ADUserSchema.MailboxRelationType;

		public static readonly ADPropertyDefinition JournalArchiveAddress = ADRecipientSchema.JournalArchiveAddress;

		public static readonly ADPropertyDefinition RemoteRecipientType = ADUserSchema.RemoteRecipientType;

		public static readonly ADPropertyDefinition DisabledArchiveDatabase = ADUserSchema.DisabledArchiveDatabase;

		public static readonly ADPropertyDefinition DisabledArchiveGuid = ADUserSchema.DisabledArchiveGuid;

		public static readonly ADPropertyDefinition QueryBaseDNRestrictionEnabled = ADRecipientSchema.QueryBaseDNRestrictionEnabled;

		public static readonly ADPropertyDefinition SharingPolicy = ADUserSchema.SharingPolicy;

		public static readonly ADPropertyDefinition RemoteAccountPolicy = ADUserSchema.RemoteAccountPolicy;

		public static readonly ADPropertyDefinition RemotePowerShellEnabled = ADRecipientSchema.RemotePowerShellEnabled;

		public static readonly ADPropertyDefinition MailboxMoveTargetMDB = ADUserSchema.MailboxMoveTargetMDB;

		public static readonly ADPropertyDefinition MailboxMoveSourceMDB = ADUserSchema.MailboxMoveSourceMDB;

		public static readonly ADPropertyDefinition MailboxMoveTargetArchiveMDB = ADUserSchema.MailboxMoveTargetArchiveMDB;

		public static readonly ADPropertyDefinition MailboxMoveSourceArchiveMDB = ADUserSchema.MailboxMoveSourceArchiveMDB;

		public static readonly ADPropertyDefinition MailboxMoveFlags = ADUserSchema.MailboxMoveFlags;

		public static readonly ADPropertyDefinition MailboxMoveRemoteHostName = ADUserSchema.MailboxMoveRemoteHostName;

		public static readonly ADPropertyDefinition MailboxMoveBatchName = ADUserSchema.MailboxMoveBatchName;

		public static readonly ADPropertyDefinition MailboxMoveStatus = ADUserSchema.MailboxMoveStatus;

		public static readonly ADPropertyDefinition MailboxRelease = ADUserSchema.MailboxRelease;

		public static readonly ADPropertyDefinition ArchiveRelease = ADUserSchema.ArchiveRelease;

		public static readonly ADPropertyDefinition IsPersonToPersonTextMessagingEnabled = ADRecipientSchema.IsPersonToPersonTextMessagingEnabled;

		public static readonly ADPropertyDefinition IsMachineToPersonTextMessagingEnabled = ADRecipientSchema.IsMachineToPersonTextMessagingEnabled;

		public static readonly ADPropertyDefinition UserSMimeCertificate = ADRecipientSchema.SMimeCertificate;

		public static readonly ADPropertyDefinition UserCertificate = ADRecipientSchema.Certificate;

		public static readonly ADPropertyDefinition CalendarVersionStoreDisabled = ADUserSchema.CalendarVersionStoreDisabled;

		public static readonly ADPropertyDefinition ImmutableId = ADRecipientSchema.ImmutableId;

		public static readonly ADPropertyDefinition PersistedCapabilities = SharedPropertyDefinitions.PersistedCapabilities;

		public static readonly ADPropertyDefinition SKUAssigned = ADRecipientSchema.SKUAssigned;

		public static readonly ADPropertyDefinition WhenMailboxCreated = ADMailboxRecipientSchema.WhenMailboxCreated;

		public static readonly ADPropertyDefinition SourceAnchor = ADUserSchema.SourceAnchor;

		public static readonly ADPropertyDefinition AuditEnabled = ADRecipientSchema.AuditEnabled;

		public static readonly ADPropertyDefinition DefaultPublicFolderMailboxValue = ADRecipientSchema.DefaultPublicFolderMailbox;

		public static readonly ADPropertyDefinition IsExcludedFromServingHierarchy = ADRecipientSchema.IsExcludedFromServingHierarchy;

		public static readonly ADPropertyDefinition IsHierarchyReady = ADRecipientSchema.IsHierarchyReady;

		public static readonly ADPropertyDefinition AuditAdmin = ADRecipientSchema.AuditAdmin;

		public static readonly ADPropertyDefinition AuditDelegate = ADRecipientSchema.AuditDelegate;

		public static readonly ADPropertyDefinition AuditDelegateAdmin = ADRecipientSchema.AuditDelegateAdmin;

		public static readonly ADPropertyDefinition AuditOwner = ADRecipientSchema.AuditOwner;

		public static readonly ADPropertyDefinition AuditLogAgeLimit = ADRecipientSchema.AuditLogAgeLimit;

		public static readonly ADPropertyDefinition ReconciliationId = ADRecipientSchema.ReconciliationId;

		public static readonly ADPropertyDefinition UsageLocation = ADRecipientSchema.UsageLocation;

		public static readonly ADPropertyDefinition IsSoftDeletedByRemove = ADRecipientSchema.IsSoftDeletedByRemove;

		public static readonly ADPropertyDefinition IsSoftDeletedByDisable = ADRecipientSchema.IsSoftDeletedByDisable;

		public static readonly ADPropertyDefinition IsInactiveMailbox = ADRecipientSchema.IsInactiveMailbox;

		public static readonly ADPropertyDefinition WhenSoftDeleted = ADRecipientSchema.WhenSoftDeleted;

		public static readonly ADPropertyDefinition IncludeInGarbageCollection = ADRecipientSchema.IncludeInGarbageCollection;

		public static readonly ADPropertyDefinition QueryBaseDN = ADUserSchema.QueryBaseDN;

		public static readonly ADPropertyDefinition InPlaceHolds = ADRecipientSchema.InPlaceHolds;

		public static readonly ADPropertyDefinition MailboxProvisioningConstraint = ADRecipientSchema.PersistedMailboxProvisioningConstraint;

		public static readonly ADPropertyDefinition MailboxProvisioningPreferences = ADRecipientSchema.MailboxProvisioningPreferences;

		public static readonly ADPropertyDefinition UCSImListMigrationCompleted = ADRecipientSchema.UCSImListMigrationCompleted;

		public static readonly ADPropertyDefinition GeneratedOfflineAddressBooks = ADRecipientSchema.GeneratedOfflineAddressBooks;

		public static readonly ADPropertyDefinition MessageCopyForSentAsEnabled = ADRecipientSchema.MessageCopyForSentAsEnabled;

		public static readonly ADPropertyDefinition MessageCopyForSendOnBehalfEnabled = ADRecipientSchema.MessageCopyForSendOnBehalfEnabled;
	}
}
