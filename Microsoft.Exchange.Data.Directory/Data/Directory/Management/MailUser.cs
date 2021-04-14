using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.Provisioning;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[ProvisioningObjectTag("MailUser")]
	[Serializable]
	public class MailUser : MailEnabledOrgPerson, IExternalAndEmailAddresses
	{
		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return MailUser.schema;
			}
		}

		public MailUser()
		{
			base.SetObjectClass("user");
		}

		public MailUser(ADUser dataObject) : base(dataObject)
		{
		}

		internal static MailUser FromDataObject(ADUser dataObject)
		{
			if (dataObject == null)
			{
				return null;
			}
			return new MailUser(dataObject);
		}

		public bool DeliverToMailboxAndForward
		{
			get
			{
				return (bool)this[MailUserSchema.DeliverToMailboxAndForward];
			}
			internal set
			{
				this[MailUserSchema.DeliverToMailboxAndForward] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Guid ExchangeGuid
		{
			get
			{
				return (Guid)this[MailUserSchema.ExchangeGuid];
			}
			set
			{
				this[MailUserSchema.ExchangeGuid] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Guid? MailboxContainerGuid
		{
			get
			{
				return (Guid?)this[MailUserSchema.MailboxContainerGuid];
			}
			set
			{
				this[MailUserSchema.MailboxContainerGuid] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<Guid> AggregatedMailboxGuids
		{
			get
			{
				return (MultiValuedProperty<Guid>)this[MailUserSchema.AggregatedMailboxGuids];
			}
		}

		[Parameter(Mandatory = false)]
		public Guid ArchiveGuid
		{
			get
			{
				return (Guid)this[MailUserSchema.ArchiveGuid];
			}
			set
			{
				this[MailUserSchema.ArchiveGuid] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> ArchiveName
		{
			get
			{
				return (MultiValuedProperty<string>)this[MailUserSchema.ArchiveName];
			}
			set
			{
				this[MailUserSchema.ArchiveName] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> ArchiveQuota
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[MailUserSchema.ArchiveQuota];
			}
			set
			{
				this[MailUserSchema.ArchiveQuota] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> ArchiveWarningQuota
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[MailUserSchema.ArchiveWarningQuota];
			}
			set
			{
				this[MailUserSchema.ArchiveWarningQuota] = value;
			}
		}

		public ADObjectId ForwardingAddress
		{
			get
			{
				return (ADObjectId)this[MailUserSchema.ForwardingAddress];
			}
			internal set
			{
				this[MailUserSchema.ForwardingAddress] = value;
			}
		}

		public ADObjectId ArchiveDatabase
		{
			get
			{
				return (ADObjectId)this[MailUserSchema.ArchiveDatabase];
			}
			internal set
			{
				this[MailUserSchema.ArchiveDatabase] = value;
			}
		}

		public ArchiveStatusFlags ArchiveStatus
		{
			get
			{
				return (ArchiveStatusFlags)this[MailUserSchema.ArchiveStatus];
			}
			internal set
			{
				this[MailUserSchema.ArchiveStatus] = value;
			}
		}

		public ADObjectId DisabledArchiveDatabase
		{
			get
			{
				return (ADObjectId)this[MailUserSchema.DisabledArchiveDatabase];
			}
			internal set
			{
				this[MailUserSchema.DisabledArchiveDatabase] = value;
			}
		}

		public Guid DisabledArchiveGuid
		{
			get
			{
				return (Guid)this[MailUserSchema.DisabledArchiveGuid];
			}
		}

		public MailboxProvisioningConstraint MailboxProvisioningConstraint
		{
			get
			{
				return (MailboxProvisioningConstraint)this[MailUserSchema.MailboxProvisioningConstraint];
			}
		}

		public MultiValuedProperty<MailboxProvisioningConstraint> MailboxProvisioningPreferences
		{
			get
			{
				return (MultiValuedProperty<MailboxProvisioningConstraint>)this[MailUserSchema.MailboxProvisioningPreferences];
			}
		}

		public UserAccountControlFlags ExchangeUserAccountControl
		{
			get
			{
				return (UserAccountControlFlags)this[MailUserSchema.ExchangeUserAccountControl];
			}
		}

		[Parameter(Mandatory = false)]
		public ProxyAddress ExternalEmailAddress
		{
			get
			{
				return (ProxyAddress)this[MailUserSchema.ExternalEmailAddress];
			}
			set
			{
				this[MailUserSchema.ExternalEmailAddress] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool UsePreferMessageFormat
		{
			get
			{
				return (bool)this[MailUserSchema.UsePreferMessageFormat];
			}
			set
			{
				this[MailUserSchema.UsePreferMessageFormat] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SmtpAddress JournalArchiveAddress
		{
			get
			{
				return (SmtpAddress)this[MailUserSchema.JournalArchiveAddress];
			}
			set
			{
				this[MailUserSchema.JournalArchiveAddress] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MessageFormat MessageFormat
		{
			get
			{
				return (MessageFormat)this[MailUserSchema.MessageFormat];
			}
			set
			{
				this[MailUserSchema.MessageFormat] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MessageBodyFormat MessageBodyFormat
		{
			get
			{
				return (MessageBodyFormat)this[MailUserSchema.MessageBodyFormat];
			}
			set
			{
				this[MailUserSchema.MessageBodyFormat] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MacAttachmentFormat MacAttachmentFormat
		{
			get
			{
				return (MacAttachmentFormat)this[MailUserSchema.MacAttachmentFormat];
			}
			set
			{
				this[MailUserSchema.MacAttachmentFormat] = value;
			}
		}

		public MultiValuedProperty<string> ProtocolSettings
		{
			get
			{
				return (MultiValuedProperty<string>)this[MailUserSchema.ProtocolSettings];
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<int> RecipientLimits
		{
			get
			{
				return (Unlimited<int>)this[MailUserSchema.RecipientLimits];
			}
			set
			{
				this[MailUserSchema.RecipientLimits] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string SamAccountName
		{
			get
			{
				return (string)this[MailUserSchema.SamAccountName];
			}
			set
			{
				this[MailUserSchema.SamAccountName] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public UseMapiRichTextFormat UseMapiRichTextFormat
		{
			get
			{
				return (UseMapiRichTextFormat)this[MailUserSchema.UseMapiRichTextFormat];
			}
			set
			{
				this[MailUserSchema.UseMapiRichTextFormat] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string UserPrincipalName
		{
			get
			{
				return (string)this[MailUserSchema.UserPrincipalName];
			}
			set
			{
				this[MailUserSchema.UserPrincipalName] = value;
			}
		}

		internal NetID NetID
		{
			get
			{
				return (NetID)this[MailUserSchema.NetID];
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		[Parameter(Mandatory = false)]
		public SmtpAddress WindowsLiveID
		{
			get
			{
				return (SmtpAddress)this[MailUserSchema.WindowsLiveID];
			}
			set
			{
				this[MailUserSchema.WindowsLiveID] = value;
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

		public ADObjectId MailboxMoveTargetMDB
		{
			get
			{
				return (ADObjectId)this[MailUserSchema.MailboxMoveTargetMDB];
			}
		}

		public ADObjectId MailboxMoveSourceMDB
		{
			get
			{
				return (ADObjectId)this[MailUserSchema.MailboxMoveSourceMDB];
			}
		}

		public RequestFlags MailboxMoveFlags
		{
			get
			{
				return (RequestFlags)this[MailUserSchema.MailboxMoveFlags];
			}
		}

		public string MailboxMoveRemoteHostName
		{
			get
			{
				return (string)this[MailUserSchema.MailboxMoveRemoteHostName];
			}
		}

		public string MailboxMoveBatchName
		{
			get
			{
				return (string)this[MailUserSchema.MailboxMoveBatchName];
			}
		}

		public RequestStatus MailboxMoveStatus
		{
			get
			{
				return (RequestStatus)this[MailUserSchema.MailboxMoveStatus];
			}
		}

		public string MailboxRelease
		{
			get
			{
				return (string)this[MailUserSchema.MailboxRelease];
			}
		}

		public string ArchiveRelease
		{
			get
			{
				return (string)this[MailUserSchema.ArchiveRelease];
			}
		}

		[Parameter(Mandatory = false)]
		public string ImmutableId
		{
			get
			{
				return (string)this[MailUserSchema.ImmutableId];
			}
			set
			{
				this[MailUserSchema.ImmutableId] = value;
			}
		}

		public MultiValuedProperty<Capability> PersistedCapabilities
		{
			get
			{
				return (MultiValuedProperty<Capability>)this[MailUserSchema.PersistedCapabilities];
			}
		}

		[Parameter(Mandatory = false)]
		public bool? SKUAssigned
		{
			get
			{
				return (bool?)this[MailUserSchema.SKUAssigned];
			}
			set
			{
				this[MailUserSchema.SKUAssigned] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ResetPasswordOnNextLogon
		{
			get
			{
				return (bool)this[MailUserSchema.ResetPasswordOnNextLogon];
			}
			set
			{
				this[MailUserSchema.ResetPasswordOnNextLogon] = value;
			}
		}

		public DateTime? WhenMailboxCreated
		{
			get
			{
				return (DateTime?)this[MailUserSchema.WhenMailboxCreated];
			}
		}

		[Parameter(Mandatory = false)]
		public bool LitigationHoldEnabled
		{
			get
			{
				return (bool)this[MailUserSchema.LitigationHoldEnabled];
			}
			set
			{
				this[MailUserSchema.LitigationHoldEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool SingleItemRecoveryEnabled
		{
			get
			{
				return (bool)this[MailUserSchema.SingleItemRecoveryEnabled];
			}
			set
			{
				this[MailUserSchema.SingleItemRecoveryEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool RetentionHoldEnabled
		{
			get
			{
				return (bool)this[MailUserSchema.ElcExpirationSuspensionEnabled];
			}
			set
			{
				this[MailUserSchema.ElcExpirationSuspensionEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DateTime? EndDateForRetentionHold
		{
			get
			{
				return (DateTime?)this[MailUserSchema.ElcExpirationSuspensionEndDate];
			}
			set
			{
				this[MailUserSchema.ElcExpirationSuspensionEndDate] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DateTime? StartDateForRetentionHold
		{
			get
			{
				return (DateTime?)this[MailUserSchema.ElcExpirationSuspensionStartDate];
			}
			set
			{
				this[MailUserSchema.ElcExpirationSuspensionStartDate] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string RetentionComment
		{
			get
			{
				return (string)this[MailUserSchema.RetentionComment];
			}
			set
			{
				this[MailUserSchema.RetentionComment] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string RetentionUrl
		{
			get
			{
				return (string)this[MailUserSchema.RetentionUrl];
			}
			set
			{
				this[MailUserSchema.RetentionUrl] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DateTime? LitigationHoldDate
		{
			get
			{
				return (DateTime?)this[MailUserSchema.LitigationHoldDate];
			}
			set
			{
				this[MailUserSchema.LitigationHoldDate] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string LitigationHoldOwner
		{
			get
			{
				return (string)this[MailUserSchema.LitigationHoldOwner];
			}
			set
			{
				this[MailUserSchema.LitigationHoldOwner] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan RetainDeletedItemsFor
		{
			get
			{
				return (EnhancedTimeSpan)this[MailUserSchema.RetainDeletedItemsFor];
			}
			set
			{
				this[MailUserSchema.RetainDeletedItemsFor] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool CalendarVersionStoreDisabled
		{
			get
			{
				return (bool)this[MailUserSchema.CalendarVersionStoreDisabled];
			}
			set
			{
				this[MailUserSchema.CalendarVersionStoreDisabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public CountryInfo UsageLocation
		{
			get
			{
				return (CountryInfo)this[MailUserSchema.UsageLocation];
			}
			set
			{
				this[MailUserSchema.UsageLocation] = value;
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

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> RecoverableItemsQuota
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[MailUserSchema.RecoverableItemsQuota];
			}
			set
			{
				this[MailUserSchema.RecoverableItemsQuota] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> RecoverableItemsWarningQuota
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[MailUserSchema.RecoverableItemsWarningQuota];
			}
			set
			{
				this[MailUserSchema.RecoverableItemsWarningQuota] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<byte[]> UserCertificate
		{
			get
			{
				return (MultiValuedProperty<byte[]>)this[MailUserSchema.UserCertificate];
			}
			set
			{
				this[MailUserSchema.UserCertificate] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<byte[]> UserSMimeCertificate
		{
			get
			{
				return (MultiValuedProperty<byte[]>)this[MailUserSchema.UserSMimeCertificate];
			}
			set
			{
				this[MailUserSchema.UserSMimeCertificate] = value;
			}
		}

		private static MailUserSchema schema = ObjectSchema.GetInstance<MailUserSchema>();
	}
}
