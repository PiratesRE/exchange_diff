using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal class MailUserSchema : MailEnabledOrgPersonSchema
	{
		internal override ADObjectSchema GetParentSchema()
		{
			return ObjectSchema.GetInstance<ADUserSchema>();
		}

		public static readonly ADPropertyDefinition ExchangeUserAccountControl = ADUserSchema.ExchangeUserAccountControl;

		public static readonly ADPropertyDefinition ExternalEmailAddress = ADRecipientSchema.ExternalEmailAddress;

		public static readonly ADPropertyDefinition UsePreferMessageFormat = ADRecipientSchema.UsePreferMessageFormat;

		public static readonly ADPropertyDefinition MessageFormat = ADRecipientSchema.MessageFormat;

		public static readonly ADPropertyDefinition MessageBodyFormat = ADRecipientSchema.MessageBodyFormat;

		public static readonly ADPropertyDefinition MacAttachmentFormat = ADRecipientSchema.MacAttachmentFormat;

		public static readonly ADPropertyDefinition ProtocolSettings = ADRecipientSchema.ReadOnlyProtocolSettings;

		public static readonly ADPropertyDefinition RecipientLimits = ADRecipientSchema.RecipientLimits;

		public static readonly ADPropertyDefinition RemotePowerShellEnabled = ADRecipientSchema.RemotePowerShellEnabled;

		public static readonly ADPropertyDefinition SamAccountName = ADMailboxRecipientSchema.SamAccountName;

		public static readonly ADPropertyDefinition UseMapiRichTextFormat = ADRecipientSchema.UseMapiRichTextFormat;

		public static readonly ADPropertyDefinition UserPrincipalName = ADUserSchema.UserPrincipalName;

		public static readonly ADPropertyDefinition ImmutableId = ADRecipientSchema.ImmutableId;

		public static readonly ADPropertyDefinition WindowsLiveID = ADRecipientSchema.WindowsLiveID;

		public static readonly ADPropertyDefinition NetID = ADUserSchema.NetID;

		public static readonly ADPropertyDefinition DeliverToMailboxAndForward = ADMailboxRecipientSchema.DeliverToMailboxAndForward;

		public static readonly ADPropertyDefinition ExchangeGuid = ADMailboxRecipientSchema.ExchangeGuid;

		public static readonly ADPropertyDefinition MailboxContainerGuid = ADUserSchema.MailboxContainerGuid;

		public static readonly ADPropertyDefinition AggregatedMailboxGuids = ADUserSchema.AggregatedMailboxGuids;

		public static readonly ADPropertyDefinition UnifiedMailbox = ADUserSchema.UnifiedMailbox;

		public static readonly ADPropertyDefinition ForwardingAddress = ADRecipientSchema.ForwardingAddress;

		public static readonly ADPropertyDefinition ArchiveGuid = ADUserSchema.ArchiveGuid;

		public static readonly ADPropertyDefinition ArchiveName = ADUserSchema.ArchiveName;

		public static readonly ADPropertyDefinition ArchiveQuota = ADUserSchema.ArchiveQuota;

		public static readonly ADPropertyDefinition ArchiveWarningQuota = ADUserSchema.ArchiveWarningQuota;

		public static readonly ADPropertyDefinition ArchiveDatabase = ADUserSchema.ArchiveDatabase;

		public static readonly ADPropertyDefinition ArchiveStatus = ADUserSchema.ArchiveStatus;

		public static readonly ADPropertyDefinition JournalArchiveAddress = ADRecipientSchema.JournalArchiveAddress;

		public static readonly ADPropertyDefinition DisabledArchiveDatabase = ADUserSchema.DisabledArchiveDatabase;

		public static readonly ADPropertyDefinition DisabledArchiveGuid = ADUserSchema.DisabledArchiveGuid;

		public static readonly ADPropertyDefinition LitigationHoldEnabled = ADUserSchema.LitigationHoldEnabled;

		public static readonly ADPropertyDefinition RetentionComment = ADUserSchema.RetentionComment;

		public static readonly ADPropertyDefinition RetentionUrl = ADUserSchema.RetentionUrl;

		public static readonly ADPropertyDefinition LitigationHoldDate = ADUserSchema.LitigationHoldDate;

		public static readonly ADPropertyDefinition LitigationHoldOwner = ADUserSchema.LitigationHoldOwner;

		public static readonly ADPropertyDefinition SingleItemRecoveryEnabled = ADUserSchema.SingleItemRecoveryEnabled;

		public static readonly ADPropertyDefinition CalendarVersionStoreDisabled = ADUserSchema.CalendarVersionStoreDisabled;

		public static readonly ADPropertyDefinition RetainDeletedItemsFor = ADMailboxRecipientSchema.RetainDeletedItemsFor;

		public static readonly ADPropertyDefinition ElcExpirationSuspensionEnabled = ADUserSchema.ElcExpirationSuspensionEnabled;

		public static readonly ADPropertyDefinition ElcExpirationSuspensionEndDate = ADUserSchema.ElcExpirationSuspensionEndDate;

		public static readonly ADPropertyDefinition ElcExpirationSuspensionStartDate = ADUserSchema.ElcExpirationSuspensionStartDate;

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

		public static readonly ADPropertyDefinition PersistedCapabilities = SharedPropertyDefinitions.PersistedCapabilities;

		public static readonly ADPropertyDefinition SKUAssigned = ADRecipientSchema.SKUAssigned;

		public static readonly ADPropertyDefinition WhenMailboxCreated = ADMailboxRecipientSchema.WhenMailboxCreated;

		public static readonly ADPropertyDefinition ResetPasswordOnNextLogon = ADUserSchema.ResetPasswordOnNextLogon;

		public static readonly ADPropertyDefinition UsageLocation = ADRecipientSchema.UsageLocation;

		public static readonly ADPropertyDefinition IsSoftDeletedByRemove = ADRecipientSchema.IsSoftDeletedByRemove;

		public static readonly ADPropertyDefinition IsSoftDeletedByDisable = ADRecipientSchema.IsSoftDeletedByDisable;

		public static readonly ADPropertyDefinition WhenSoftDeleted = ADRecipientSchema.WhenSoftDeleted;

		public static readonly ADPropertyDefinition InPlaceHolds = ADRecipientSchema.InPlaceHolds;

		public static readonly ADPropertyDefinition RecoverableItemsQuota = ADUserSchema.RecoverableItemsQuota;

		public static readonly ADPropertyDefinition RecoverableItemsWarningQuota = ADUserSchema.RecoverableItemsWarningQuota;

		public static readonly ADPropertyDefinition UserCertificate = ADRecipientSchema.Certificate;

		public static readonly ADPropertyDefinition UserSMimeCertificate = ADRecipientSchema.SMimeCertificate;

		public static readonly ADPropertyDefinition MailboxProvisioningConstraint = ADRecipientSchema.MailboxProvisioningConstraint;

		public static readonly ADPropertyDefinition MailboxProvisioningPreferences = ADRecipientSchema.MailboxProvisioningPreferences;
	}
}
