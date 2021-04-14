using System;
using System.Collections.Generic;
using System.Globalization;
using System.Management.Automation;
using System.Reflection;
using System.Security.AccessControl;
using Microsoft.Exchange.Data.Directory.Provisioning;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[ProvisioningObjectTag("Mailbox")]
	[Serializable]
	public class Mailbox : MailEnabledOrgPerson
	{
		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return Mailbox.schema;
			}
		}

		public Mailbox()
		{
			base.SetObjectClass("user");
		}

		public Mailbox(ADUser dataObject) : base(dataObject)
		{
		}

		internal static Mailbox FromDataObject(ADUser dataObject)
		{
			if (dataObject == null)
			{
				return null;
			}
			return new Mailbox(dataObject);
		}

		protected override IEnumerable<PropertyInfo> CloneableProperties
		{
			get
			{
				IEnumerable<PropertyInfo> result;
				if ((result = Mailbox.cloneableProps) == null)
				{
					result = (Mailbox.cloneableProps = ADPresentationObject.GetCloneableProperties(this));
				}
				return result;
			}
		}

		protected override IEnumerable<PropertyInfo> CloneableOnceProperties
		{
			get
			{
				IEnumerable<PropertyInfo> result;
				if ((result = Mailbox.cloneableOnceProps) == null)
				{
					result = (Mailbox.cloneableOnceProps = ADPresentationObject.GetCloneableOnceProperties(this));
				}
				return result;
			}
		}

		protected override IEnumerable<PropertyInfo> CloneableEnabledStateProperties
		{
			get
			{
				IEnumerable<PropertyInfo> result;
				if ((result = Mailbox.cloneableEnabledStateProps) == null)
				{
					result = (Mailbox.cloneableEnabledStateProps = ADPresentationObject.GetCloneableEnabledStateProperties(this));
				}
				return result;
			}
		}

		public ADObjectId Database
		{
			get
			{
				return (ADObjectId)this[MailboxSchema.Database];
			}
			internal set
			{
				this[MailboxSchema.Database] = value;
			}
		}

		public MailboxProvisioningConstraint MailboxProvisioningConstraint
		{
			get
			{
				return (MailboxProvisioningConstraint)this[MailboxSchema.MailboxProvisioningConstraint];
			}
		}

		public bool MessageCopyForSentAsEnabled
		{
			get
			{
				return (bool)this[MailboxSchema.MessageCopyForSentAsEnabled];
			}
		}

		public bool MessageCopyForSendOnBehalfEnabled
		{
			get
			{
				return (bool)this[MailboxSchema.MessageCopyForSendOnBehalfEnabled];
			}
		}

		public MultiValuedProperty<MailboxProvisioningConstraint> MailboxProvisioningPreferences
		{
			get
			{
				return (MultiValuedProperty<MailboxProvisioningConstraint>)this[MailboxSchema.MailboxProvisioningPreferences];
			}
		}

		internal ADObjectId PreviousDatabase
		{
			get
			{
				return (ADObjectId)this[MailboxSchema.PreviousDatabase];
			}
			set
			{
				this[MailboxSchema.PreviousDatabase] = value;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ADRecipient.PublicFolderMailboxObjectVersion;
			}
		}

		[Parameter(Mandatory = false)]
		[ProvisionalCloneOnce(CloneSet.CloneLimitedSet)]
		public bool UseDatabaseRetentionDefaults
		{
			get
			{
				return (bool)this[MailboxSchema.UseDatabaseRetentionDefaults];
			}
			set
			{
				this[MailboxSchema.UseDatabaseRetentionDefaults] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool RetainDeletedItemsUntilBackup
		{
			get
			{
				return (bool)this[MailboxSchema.RetainDeletedItemsUntilBackup];
			}
			set
			{
				this[MailboxSchema.RetainDeletedItemsUntilBackup] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool DeliverToMailboxAndForward
		{
			get
			{
				return (bool)this[MailboxSchema.DeliverToMailboxAndForward];
			}
			set
			{
				this[MailboxSchema.DeliverToMailboxAndForward] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IsExcludedFromServingHierarchy
		{
			get
			{
				return (bool)this[MailboxSchema.IsExcludedFromServingHierarchy];
			}
			set
			{
				this[MailboxSchema.IsExcludedFromServingHierarchy] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IsHierarchyReady
		{
			get
			{
				return (bool)this[MailboxSchema.IsHierarchyReady];
			}
			set
			{
				this[MailboxSchema.IsHierarchyReady] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool LitigationHoldEnabled
		{
			get
			{
				return (bool)this[MailboxSchema.LitigationHoldEnabled];
			}
			set
			{
				this[MailboxSchema.LitigationHoldEnabled] = value;
			}
		}

		[ProvisionalCloneOnce(CloneSet.CloneExtendedSet)]
		[Parameter(Mandatory = false)]
		public bool SingleItemRecoveryEnabled
		{
			get
			{
				return (bool)this[MailboxSchema.SingleItemRecoveryEnabled];
			}
			set
			{
				this[MailboxSchema.SingleItemRecoveryEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool RetentionHoldEnabled
		{
			get
			{
				return (bool)this[MailboxSchema.ElcExpirationSuspensionEnabled];
			}
			set
			{
				this[MailboxSchema.ElcExpirationSuspensionEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DateTime? EndDateForRetentionHold
		{
			get
			{
				return (DateTime?)this[MailboxSchema.ElcExpirationSuspensionEndDate];
			}
			set
			{
				this[MailboxSchema.ElcExpirationSuspensionEndDate] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DateTime? StartDateForRetentionHold
		{
			get
			{
				return (DateTime?)this[MailboxSchema.ElcExpirationSuspensionStartDate];
			}
			set
			{
				this[MailboxSchema.ElcExpirationSuspensionStartDate] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string RetentionComment
		{
			get
			{
				return (string)this[MailboxSchema.RetentionComment];
			}
			set
			{
				this[MailboxSchema.RetentionComment] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string RetentionUrl
		{
			get
			{
				return (string)this[MailboxSchema.RetentionUrl];
			}
			set
			{
				this[MailboxSchema.RetentionUrl] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DateTime? LitigationHoldDate
		{
			get
			{
				return (DateTime?)this[MailboxSchema.LitigationHoldDate];
			}
			set
			{
				this[MailboxSchema.LitigationHoldDate] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string LitigationHoldOwner
		{
			get
			{
				return (string)this[MailboxSchema.LitigationHoldOwner];
			}
			set
			{
				this[MailboxSchema.LitigationHoldOwner] = value;
			}
		}

		public Unlimited<EnhancedTimeSpan>? LitigationHoldDuration
		{
			get
			{
				Unlimited<EnhancedTimeSpan>? result = (Unlimited<EnhancedTimeSpan>?)this[MailboxSchema.LitigationHoldDuration];
				if (result == null)
				{
					return new Unlimited<EnhancedTimeSpan>?(Unlimited<EnhancedTimeSpan>.UnlimitedValue);
				}
				return result;
			}
			set
			{
				this[MailboxSchema.LitigationHoldDuration] = value;
			}
		}

		public ADObjectId ManagedFolderMailboxPolicy
		{
			get
			{
				return (ADObjectId)this[MailboxSchema.ManagedFolderMailboxPolicy];
			}
			set
			{
				this[MailboxSchema.ManagedFolderMailboxPolicy] = value;
			}
		}

		[ProvisionalCloneOnce(CloneSet.CloneLimitedSet)]
		public ADObjectId RetentionPolicy
		{
			get
			{
				return (ADObjectId)this[MailboxSchema.RetentionPolicy];
			}
			set
			{
				this[MailboxSchema.RetentionPolicy] = value;
			}
		}

		public ADObjectId AddressBookPolicy
		{
			get
			{
				return (ADObjectId)this[MailboxSchema.AddressBookPolicy];
			}
			set
			{
				this[MailboxSchema.AddressBookPolicy] = value;
			}
		}

		internal bool ShouldUseDefaultRetentionPolicy
		{
			get
			{
				return (bool)this[MailboxSchema.ShouldUseDefaultRetentionPolicy];
			}
			set
			{
				this[MailboxSchema.ShouldUseDefaultRetentionPolicy] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool CalendarRepairDisabled
		{
			get
			{
				return (bool)this[MailboxSchema.CalendarRepairDisabled];
			}
			set
			{
				this[MailboxSchema.CalendarRepairDisabled] = value;
			}
		}

		public Guid ExchangeGuid
		{
			get
			{
				return (Guid)this[MailboxSchema.ExchangeGuid];
			}
		}

		public Guid? MailboxContainerGuid
		{
			get
			{
				return (Guid?)this[MailboxSchema.MailboxContainerGuid];
			}
			set
			{
				this[MailboxSchema.MailboxContainerGuid] = value;
			}
		}

		public CrossTenantObjectId UnifiedMailbox
		{
			get
			{
				return (CrossTenantObjectId)this[MailboxSchema.UnifiedMailbox];
			}
			set
			{
				this[MailboxSchema.UnifiedMailbox] = value;
			}
		}

		public IList<IMailboxLocationInfo> MailboxLocations
		{
			get
			{
				MailboxLocationCollection mailboxLocationCollection = (MailboxLocationCollection)this[MailboxSchema.MailboxLocations];
				if (mailboxLocationCollection != null)
				{
					return mailboxLocationCollection.GetMailboxLocations();
				}
				return Mailbox.EmptyMailboxLocationInfo;
			}
		}

		public MultiValuedProperty<Guid> AggregatedMailboxGuids
		{
			get
			{
				return (MultiValuedProperty<Guid>)this[MailboxSchema.AggregatedMailboxGuids];
			}
		}

		public RawSecurityDescriptor ExchangeSecurityDescriptor
		{
			get
			{
				return (RawSecurityDescriptor)this[MailboxSchema.ExchangeSecurityDescriptor];
			}
		}

		public UserAccountControlFlags ExchangeUserAccountControl
		{
			get
			{
				return (UserAccountControlFlags)this[MailboxSchema.ExchangeUserAccountControl];
			}
		}

		public ServerVersion AdminDisplayVersion
		{
			get
			{
				return (ServerVersion)this[MailboxSchema.AdminDisplayVersion];
			}
			internal set
			{
				this[MailboxSchema.AdminDisplayVersion] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool MessageTrackingReadStatusEnabled
		{
			get
			{
				return !(bool)this[MailboxSchema.MessageTrackingReadStatusDisabled];
			}
			set
			{
				this[MailboxSchema.MessageTrackingReadStatusDisabled] = !value;
			}
		}

		[Parameter(Mandatory = false)]
		public ExternalOofOptions ExternalOofOptions
		{
			get
			{
				return (ExternalOofOptions)this[MailboxSchema.ExternalOofOptions];
			}
			set
			{
				this[MailboxSchema.ExternalOofOptions] = value;
			}
		}

		public ADObjectId ForwardingAddress
		{
			get
			{
				return (ADObjectId)this[MailboxSchema.ForwardingAddress];
			}
			set
			{
				this[MailboxSchema.ForwardingAddress] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ProxyAddress ForwardingSmtpAddress
		{
			get
			{
				return (ProxyAddress)this[MailboxSchema.ForwardingSmtpAddress];
			}
			set
			{
				this[MailboxSchema.ForwardingSmtpAddress] = value;
			}
		}

		[ProvisionalClone(CloneSet.CloneLimitedSet)]
		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan RetainDeletedItemsFor
		{
			get
			{
				return (EnhancedTimeSpan)this[MailboxSchema.RetainDeletedItemsFor];
			}
			set
			{
				this[MailboxSchema.RetainDeletedItemsFor] = value;
			}
		}

		public bool IsMailboxEnabled
		{
			get
			{
				return (bool)this[MailboxSchema.IsMailboxEnabled];
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<CultureInfo> Languages
		{
			get
			{
				return (MultiValuedProperty<CultureInfo>)this[MailboxSchema.Languages];
			}
			set
			{
				this[MailboxSchema.Languages] = value;
			}
		}

		public ADObjectId OfflineAddressBook
		{
			get
			{
				return (ADObjectId)this[MailboxSchema.OfflineAddressBook];
			}
			set
			{
				this[MailboxSchema.OfflineAddressBook] = value;
			}
		}

		[ProvisionalClone(CloneSet.CloneExtendedSet)]
		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> ProhibitSendQuota
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[MailboxSchema.ProhibitSendQuota];
			}
			set
			{
				this[MailboxSchema.ProhibitSendQuota] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ProvisionalClone(CloneSet.CloneExtendedSet)]
		public Unlimited<ByteQuantifiedSize> ProhibitSendReceiveQuota
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[MailboxSchema.ProhibitSendReceiveQuota];
			}
			set
			{
				this[MailboxSchema.ProhibitSendReceiveQuota] = value;
			}
		}

		[ProvisionalClone(CloneSet.CloneExtendedSet)]
		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> RecoverableItemsQuota
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[MailboxSchema.RecoverableItemsQuota];
			}
			set
			{
				this[MailboxSchema.RecoverableItemsQuota] = value;
			}
		}

		[ProvisionalClone(CloneSet.CloneExtendedSet)]
		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> RecoverableItemsWarningQuota
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[MailboxSchema.RecoverableItemsWarningQuota];
			}
			set
			{
				this[MailboxSchema.RecoverableItemsWarningQuota] = value;
			}
		}

		[ProvisionalClone(CloneSet.CloneExtendedSet)]
		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> CalendarLoggingQuota
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[MailboxSchema.CalendarLoggingQuota];
			}
			set
			{
				this[MailboxSchema.CalendarLoggingQuota] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool DowngradeHighPriorityMessagesEnabled
		{
			get
			{
				return (bool)this[MailboxSchema.DowngradeHighPriorityMessagesEnabled];
			}
			set
			{
				this[MailboxSchema.DowngradeHighPriorityMessagesEnabled] = value;
			}
		}

		public MultiValuedProperty<string> ProtocolSettings
		{
			get
			{
				return (MultiValuedProperty<string>)this[MailboxSchema.ProtocolSettings];
			}
		}

		[ProvisionalClone(CloneSet.CloneLimitedSet)]
		[Parameter(Mandatory = false)]
		public Unlimited<int> RecipientLimits
		{
			get
			{
				return (Unlimited<int>)this[MailboxSchema.RecipientLimits];
			}
			set
			{
				this[MailboxSchema.RecipientLimits] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ImListMigrationCompleted
		{
			get
			{
				return (bool)this[UMMailboxSchema.UCSImListMigrationCompleted];
			}
			set
			{
				this[UMMailboxSchema.UCSImListMigrationCompleted] = value;
			}
		}

		public bool IsResource
		{
			get
			{
				return (bool)this[MailboxSchema.IsResource];
			}
		}

		public bool IsLinked
		{
			get
			{
				return (bool)this[MailboxSchema.IsLinked];
			}
		}

		public bool IsShared
		{
			get
			{
				return (bool)this[MailboxSchema.IsShared];
			}
		}

		public bool IsRootPublicFolderMailbox { get; internal set; }

		public string LinkedMasterAccount
		{
			get
			{
				return (string)this[MailboxSchema.LinkedMasterAccount];
			}
		}

		[Parameter(Mandatory = false)]
		public bool ResetPasswordOnNextLogon
		{
			get
			{
				return (bool)this[MailboxSchema.ResetPasswordOnNextLogon];
			}
			set
			{
				this[MailboxSchema.ResetPasswordOnNextLogon] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int? ResourceCapacity
		{
			get
			{
				return (int?)this[MailboxSchema.ResourceCapacity];
			}
			set
			{
				this[MailboxSchema.ResourceCapacity] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> ResourceCustom
		{
			get
			{
				return (MultiValuedProperty<string>)this[MailboxSchema.ResourceCustom];
			}
			set
			{
				this[MailboxSchema.ResourceCustom] = value;
			}
		}

		public ExchangeResourceType? ResourceType
		{
			get
			{
				return (ExchangeResourceType?)this[MailboxSchema.ResourceType];
			}
		}

		public bool? RoomMailboxAccountEnabled
		{
			get
			{
				if (this.ResourceType != ExchangeResourceType.Room)
				{
					return null;
				}
				return new bool?((this.ExchangeUserAccountControl & UserAccountControlFlags.AccountDisabled) == UserAccountControlFlags.None);
			}
		}

		[Parameter(Mandatory = false)]
		public string SamAccountName
		{
			get
			{
				return (string)this[MailboxSchema.SamAccountName];
			}
			set
			{
				this[MailboxSchema.SamAccountName] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int? SCLDeleteThreshold
		{
			get
			{
				return (int?)this[MailboxSchema.SCLDeleteThreshold];
			}
			set
			{
				this[MailboxSchema.SCLDeleteThreshold] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? SCLDeleteEnabled
		{
			get
			{
				return (bool?)this[MailboxSchema.SCLDeleteEnabled];
			}
			set
			{
				this[MailboxSchema.SCLDeleteEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int? SCLRejectThreshold
		{
			get
			{
				return (int?)this[MailboxSchema.SCLRejectThreshold];
			}
			set
			{
				this[MailboxSchema.SCLRejectThreshold] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? SCLRejectEnabled
		{
			get
			{
				return (bool?)this[MailboxSchema.SCLRejectEnabled];
			}
			set
			{
				this[MailboxSchema.SCLRejectEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int? SCLQuarantineThreshold
		{
			get
			{
				return (int?)this[MailboxSchema.SCLQuarantineThreshold];
			}
			set
			{
				this[MailboxSchema.SCLQuarantineThreshold] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? SCLQuarantineEnabled
		{
			get
			{
				return (bool?)this[MailboxSchema.SCLQuarantineEnabled];
			}
			set
			{
				this[MailboxSchema.SCLQuarantineEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int? SCLJunkThreshold
		{
			get
			{
				return (int?)this[MailboxSchema.SCLJunkThreshold];
			}
			set
			{
				this[MailboxSchema.SCLJunkThreshold] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? SCLJunkEnabled
		{
			get
			{
				return (bool?)this[MailboxSchema.SCLJunkEnabled];
			}
			set
			{
				this[MailboxSchema.SCLJunkEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AntispamBypassEnabled
		{
			get
			{
				return (bool)this[MailboxSchema.AntispamBypassEnabled];
			}
			set
			{
				this[MailboxSchema.AntispamBypassEnabled] = value;
			}
		}

		public string ServerLegacyDN
		{
			get
			{
				return (string)this[MailboxSchema.ServerLegacyDN];
			}
		}

		public string ServerName
		{
			get
			{
				return (string)this[MailboxSchema.ServerName];
			}
		}

		[ProvisionalClone(CloneSet.CloneExtendedSet)]
		[Parameter(Mandatory = false)]
		public bool? UseDatabaseQuotaDefaults
		{
			get
			{
				return (bool?)this[MailboxSchema.UseDatabaseQuotaDefaults];
			}
			set
			{
				this[MailboxSchema.UseDatabaseQuotaDefaults] = value;
			}
		}

		[ProvisionalClone(CloneSet.CloneExtendedSet)]
		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> IssueWarningQuota
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[MailboxSchema.IssueWarningQuota];
			}
			set
			{
				this[MailboxSchema.IssueWarningQuota] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize RulesQuota
		{
			get
			{
				return (ByteQuantifiedSize)this[MailboxSchema.RulesQuota];
			}
			set
			{
				this[MailboxSchema.RulesQuota] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Office
		{
			get
			{
				return (string)this[MailboxSchema.Office];
			}
			set
			{
				this[MailboxSchema.Office] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string UserPrincipalName
		{
			get
			{
				return (string)this[MailboxSchema.UserPrincipalName];
			}
			set
			{
				this[MailboxSchema.UserPrincipalName] = value;
			}
		}

		public bool UMEnabled
		{
			get
			{
				return (bool)this[MailboxSchema.UMEnabled];
			}
		}

		[Parameter(Mandatory = false)]
		public int? MaxSafeSenders
		{
			get
			{
				return (int?)this[MailboxSchema.MaxSafeSenders];
			}
			set
			{
				this[MailboxSchema.MaxSafeSenders] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int? MaxBlockedSenders
		{
			get
			{
				return (int?)this[MailboxSchema.MaxBlockedSenders];
			}
			set
			{
				this[MailboxSchema.MaxBlockedSenders] = value;
			}
		}

		public NetID NetID
		{
			get
			{
				return (NetID)this[MailboxSchema.NetID];
			}
		}

		public NetID ReconciliationId { get; internal set; }

		[Parameter(Mandatory = false)]
		public SmtpAddress WindowsLiveID
		{
			get
			{
				return (SmtpAddress)this[MailboxSchema.WindowsLiveID];
			}
			set
			{
				this[MailboxSchema.WindowsLiveID] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SmtpAddress MicrosoftOnlineServicesID
		{
			get
			{
				return this.WindowsLiveID;
			}
			set
			{
				this.WindowsLiveID = value;
			}
		}

		[ProvisionalClone(CloneSet.CloneLimitedSet)]
		public ADObjectId ThrottlingPolicy
		{
			get
			{
				return (ADObjectId)this[MailboxSchema.ThrottlingPolicy];
			}
			internal set
			{
				this[MailboxSchema.ThrottlingPolicy] = value;
			}
		}

		[ProvisionalClone(CloneSet.CloneLimitedSet)]
		public ADObjectId RoleAssignmentPolicy
		{
			get
			{
				return (ADObjectId)this[MailboxSchema.RoleAssignmentPolicy];
			}
			internal set
			{
				this[MailboxSchema.RoleAssignmentPolicy] = value;
			}
		}

		public ADObjectId DefaultPublicFolderMailbox { get; internal set; }

		internal ADObjectId DefaultPublicFolderMailboxValue
		{
			get
			{
				return (ADObjectId)this[MailboxSchema.DefaultPublicFolderMailboxValue];
			}
			set
			{
				this[MailboxSchema.DefaultPublicFolderMailboxValue] = value;
			}
		}

		public ADObjectId SharingPolicy
		{
			get
			{
				return this.sharingPolicy ?? ((ADObjectId)this[MailboxSchema.SharingPolicy]);
			}
			internal set
			{
				this.sharingPolicy = value;
			}
		}

		[ProvisionalClone(CloneSet.CloneLimitedSet)]
		public ADObjectId RemoteAccountPolicy
		{
			get
			{
				return (ADObjectId)this[MailboxSchema.RemoteAccountPolicy];
			}
			set
			{
				this[MailboxSchema.RemoteAccountPolicy] = value;
			}
		}

		public ADObjectId MailboxPlan
		{
			get
			{
				return (ADObjectId)this[MailboxSchema.MailboxPlan];
			}
			set
			{
				this[MailboxSchema.MailboxPlan] = value;
			}
		}

		public ADObjectId ArchiveDatabase
		{
			get
			{
				return (ADObjectId)this[MailboxSchema.ArchiveDatabase];
			}
			internal set
			{
				this[MailboxSchema.ArchiveDatabase] = value;
			}
		}

		public Guid ArchiveGuid
		{
			get
			{
				return (Guid)this[MailboxSchema.ArchiveGuid];
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> ArchiveName
		{
			get
			{
				return (MultiValuedProperty<string>)this[MailboxSchema.ArchiveName];
			}
			set
			{
				this[MailboxSchema.ArchiveName] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SmtpAddress JournalArchiveAddress
		{
			get
			{
				return (SmtpAddress)this[MailboxSchema.JournalArchiveAddress];
			}
			set
			{
				this[MailboxSchema.JournalArchiveAddress] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ProvisionalClone(CloneSet.CloneExtendedSet)]
		public Unlimited<ByteQuantifiedSize> ArchiveQuota
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[MailboxSchema.ArchiveQuota];
			}
			set
			{
				this[MailboxSchema.ArchiveQuota] = value;
			}
		}

		[ProvisionalClone(CloneSet.CloneExtendedSet)]
		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> ArchiveWarningQuota
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[MailboxSchema.ArchiveWarningQuota];
			}
			set
			{
				this[MailboxSchema.ArchiveWarningQuota] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SmtpDomain ArchiveDomain
		{
			get
			{
				return (SmtpDomain)this[MailboxSchema.ArchiveDomain];
			}
			set
			{
				this[MailboxSchema.ArchiveDomain] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ArchiveStatusFlags ArchiveStatus
		{
			get
			{
				return (ArchiveStatusFlags)this[MailboxSchema.ArchiveStatus];
			}
			set
			{
				this[MailboxSchema.ArchiveStatus] = value;
			}
		}

		public ArchiveState ArchiveState
		{
			get
			{
				return (ArchiveState)this[MailboxSchema.ArchiveState];
			}
		}

		public bool IsAuxMailbox
		{
			get
			{
				return (bool)this[MailboxSchema.IsAuxMailbox];
			}
		}

		public ADObjectId AuxMailboxParentObjectId
		{
			get
			{
				return (ADObjectId)this[MailboxSchema.AuxMailboxParentObjectId];
			}
			set
			{
				this[MailboxSchema.AuxMailboxParentObjectId] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> ChildAuxMailboxObjectIds
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[MailboxSchema.ChildAuxMailboxObjectIds];
			}
		}

		public MailboxRelationType MailboxRelationType
		{
			get
			{
				return (MailboxRelationType)this[MailboxSchema.MailboxRelationType];
			}
		}

		[Parameter(Mandatory = false)]
		public RemoteRecipientType RemoteRecipientType
		{
			get
			{
				return (RemoteRecipientType)this[MailboxSchema.RemoteRecipientType];
			}
			set
			{
				this[MailboxSchema.RemoteRecipientType] = value;
			}
		}

		public ADObjectId DisabledArchiveDatabase
		{
			get
			{
				return (ADObjectId)this[MailboxSchema.DisabledArchiveDatabase];
			}
			internal set
			{
				this[MailboxSchema.DisabledArchiveDatabase] = value;
			}
		}

		public Guid DisabledArchiveGuid
		{
			get
			{
				return (Guid)this[MailboxSchema.DisabledArchiveGuid];
			}
		}

		public ADObjectId QueryBaseDN
		{
			get
			{
				return (ADObjectId)this[MailboxSchema.QueryBaseDN];
			}
		}

		[ProvisionalClone(CloneSet.CloneLimitedSet)]
		[Parameter(Mandatory = false)]
		public bool QueryBaseDNRestrictionEnabled
		{
			get
			{
				return (bool)this[MailboxSchema.QueryBaseDNRestrictionEnabled];
			}
			set
			{
				this[MailboxSchema.QueryBaseDNRestrictionEnabled] = value;
			}
		}

		public ADObjectId MailboxMoveTargetMDB
		{
			get
			{
				return (ADObjectId)this[MailboxSchema.MailboxMoveTargetMDB];
			}
		}

		public ADObjectId MailboxMoveSourceMDB
		{
			get
			{
				return (ADObjectId)this[MailboxSchema.MailboxMoveSourceMDB];
			}
		}

		public RequestFlags MailboxMoveFlags
		{
			get
			{
				return (RequestFlags)this[MailboxSchema.MailboxMoveFlags];
			}
		}

		public string MailboxMoveRemoteHostName
		{
			get
			{
				return (string)this[MailboxSchema.MailboxMoveRemoteHostName];
			}
		}

		public string MailboxMoveBatchName
		{
			get
			{
				return (string)this[MailboxSchema.MailboxMoveBatchName];
			}
		}

		public RequestStatus MailboxMoveStatus
		{
			get
			{
				return (RequestStatus)this[MailboxSchema.MailboxMoveStatus];
			}
		}

		public string MailboxRelease
		{
			get
			{
				return (string)this[MailboxSchema.MailboxRelease];
			}
		}

		public string ArchiveRelease
		{
			get
			{
				return (string)this[MailboxSchema.ArchiveRelease];
			}
		}

		public bool IsPersonToPersonTextMessagingEnabled
		{
			get
			{
				return (bool)this[MailboxSchema.IsPersonToPersonTextMessagingEnabled];
			}
		}

		public bool IsMachineToPersonTextMessagingEnabled
		{
			get
			{
				return (bool)this[MailboxSchema.IsMachineToPersonTextMessagingEnabled];
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<byte[]> UserSMimeCertificate
		{
			get
			{
				return (MultiValuedProperty<byte[]>)this[MailboxSchema.UserSMimeCertificate];
			}
			set
			{
				this[MailboxSchema.UserSMimeCertificate] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<byte[]> UserCertificate
		{
			get
			{
				return (MultiValuedProperty<byte[]>)this[MailboxSchema.UserCertificate];
			}
			set
			{
				this[MailboxSchema.UserCertificate] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool CalendarVersionStoreDisabled
		{
			get
			{
				return (bool)this[MailboxSchema.CalendarVersionStoreDisabled];
			}
			set
			{
				this[MailboxSchema.CalendarVersionStoreDisabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string ImmutableId
		{
			get
			{
				return (string)this[MailboxSchema.ImmutableId];
			}
			set
			{
				this[MailboxSchema.ImmutableId] = value;
			}
		}

		[ProvisionalClone(CloneSet.CloneLimitedSet)]
		public MultiValuedProperty<Capability> PersistedCapabilities
		{
			get
			{
				return (MultiValuedProperty<Capability>)this[MailboxSchema.PersistedCapabilities];
			}
		}

		[Parameter(Mandatory = false)]
		public bool? SKUAssigned
		{
			get
			{
				return (bool?)this[MailboxSchema.SKUAssigned];
			}
			set
			{
				this[MailboxSchema.SKUAssigned] = value;
			}
		}

		[ProvisionalCloneOnce(CloneSet.CloneExtendedSet)]
		[Parameter(Mandatory = false)]
		public bool AuditEnabled
		{
			get
			{
				return (bool)this[MailboxSchema.AuditEnabled];
			}
			set
			{
				this[MailboxSchema.AuditEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ProvisionalCloneOnce(CloneSet.CloneExtendedSet)]
		public EnhancedTimeSpan AuditLogAgeLimit
		{
			get
			{
				return (EnhancedTimeSpan)this[MailboxSchema.AuditLogAgeLimit];
			}
			set
			{
				this[MailboxSchema.AuditLogAgeLimit] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ProvisionalCloneOnce(CloneSet.CloneExtendedSet)]
		public MultiValuedProperty<MailboxAuditOperations> AuditAdmin
		{
			get
			{
				return (MultiValuedProperty<MailboxAuditOperations>)this[MailboxSchema.AuditAdmin];
			}
			set
			{
				this[MailboxSchema.AuditAdmin] = value;
				this.AuditDelegateAdmin = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ProvisionalCloneOnce(CloneSet.CloneExtendedSet)]
		public MultiValuedProperty<MailboxAuditOperations> AuditDelegate
		{
			get
			{
				return (MultiValuedProperty<MailboxAuditOperations>)this[MailboxSchema.AuditDelegate];
			}
			set
			{
				this[MailboxSchema.AuditDelegate] = value;
			}
		}

		internal MultiValuedProperty<MailboxAuditOperations> AuditDelegateAdmin
		{
			get
			{
				return (MultiValuedProperty<MailboxAuditOperations>)this[MailboxSchema.AuditDelegateAdmin];
			}
			set
			{
				this[MailboxSchema.AuditDelegateAdmin] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<MailboxAuditOperations> AuditOwner
		{
			get
			{
				return (MultiValuedProperty<MailboxAuditOperations>)this[MailboxSchema.AuditOwner];
			}
			set
			{
				this[MailboxSchema.AuditOwner] = value;
			}
		}

		public DateTime? WhenMailboxCreated
		{
			get
			{
				return (DateTime?)this[MailboxSchema.WhenMailboxCreated];
			}
		}

		public string SourceAnchor
		{
			get
			{
				return (string)this[MailboxSchema.SourceAnchor];
			}
		}

		[Parameter(Mandatory = false)]
		public CountryInfo UsageLocation
		{
			get
			{
				return (CountryInfo)this[MailboxSchema.UsageLocation];
			}
			set
			{
				this[MailboxSchema.UsageLocation] = value;
			}
		}

		public bool IsSoftDeletedByRemove
		{
			get
			{
				return (bool)this[MailboxSchema.IsSoftDeletedByRemove];
			}
			set
			{
				this[MailboxSchema.IsSoftDeletedByRemove] = value;
			}
		}

		public bool IsSoftDeletedByDisable
		{
			get
			{
				return (bool)this[MailboxSchema.IsSoftDeletedByDisable];
			}
			set
			{
				this[MailboxSchema.IsSoftDeletedByDisable] = value;
			}
		}

		public bool IsInactiveMailbox
		{
			get
			{
				return (bool)this[MailboxSchema.IsInactiveMailbox];
			}
			set
			{
				this[MailboxSchema.IsInactiveMailbox] = value;
			}
		}

		public bool IncludeInGarbageCollection
		{
			get
			{
				return (bool)this[MailboxSchema.IncludeInGarbageCollection];
			}
			set
			{
				this[MailboxSchema.IncludeInGarbageCollection] = value;
			}
		}

		public DateTime? WhenSoftDeleted
		{
			get
			{
				return (DateTime?)this[MailboxSchema.WhenSoftDeleted];
			}
			set
			{
				this[MailboxSchema.WhenSoftDeleted] = value;
			}
		}

		public MultiValuedProperty<string> InPlaceHolds
		{
			get
			{
				return (MultiValuedProperty<string>)this[MailboxSchema.InPlaceHolds];
			}
			set
			{
				this[MailboxSchema.InPlaceHolds] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> GeneratedOfflineAddressBooks
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[MailboxSchema.GeneratedOfflineAddressBooks];
			}
		}

		private static MailboxSchema schema = ObjectSchema.GetInstance<MailboxSchema>();

		private static readonly IMailboxLocationInfo[] EmptyMailboxLocationInfo = new IMailboxLocationInfo[0];

		private static IEnumerable<PropertyInfo> cloneableProps;

		private static IEnumerable<PropertyInfo> cloneableOnceProps;

		private static IEnumerable<PropertyInfo> cloneableEnabledStateProps;

		private ADObjectId sharingPolicy;
	}
}
