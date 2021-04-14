using System;
using System.Globalization;
using System.Management.Automation;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory.Provisioning;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[ProvisioningObjectTag("SyncMailUser")]
	[Serializable]
	public class SyncMailUser : MailUser
	{
		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return SyncMailUser.schema;
			}
		}

		public SyncMailUser()
		{
			base.SetObjectClass("user");
		}

		public SyncMailUser(ADUser dataObject) : base(dataObject)
		{
		}

		internal new static SyncMailUser FromDataObject(ADUser dataObject)
		{
			if (dataObject == null)
			{
				return null;
			}
			return new SyncMailUser(dataObject);
		}

		public new MultiValuedProperty<ADObjectId> BypassModerationFrom
		{
			get
			{
				return base.BypassModerationFrom;
			}
			internal set
			{
				base.BypassModerationFrom = value;
			}
		}

		public new MultiValuedProperty<ADObjectId> BypassModerationFromDLMembers
		{
			get
			{
				return base.BypassModerationFromDLMembers;
			}
			internal set
			{
				base.BypassModerationFromDLMembers = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<CultureInfo> Languages
		{
			get
			{
				return (MultiValuedProperty<CultureInfo>)this[SyncMailUserSchema.Languages];
			}
			set
			{
				this[SyncMailUserSchema.Languages] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public new bool DeliverToMailboxAndForward
		{
			get
			{
				return base.DeliverToMailboxAndForward;
			}
			set
			{
				base.DeliverToMailboxAndForward = value;
			}
		}

		public new ADObjectId ForwardingAddress
		{
			get
			{
				return base.ForwardingAddress;
			}
			set
			{
				base.ForwardingAddress = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string AssistantName
		{
			get
			{
				return (string)this[SyncMailUserSchema.AssistantName];
			}
			set
			{
				this[SyncMailUserSchema.AssistantName] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public byte[] BlockedSendersHash
		{
			get
			{
				return (byte[])this[SyncMailUserSchema.BlockedSendersHash];
			}
			set
			{
				this[SyncMailUserSchema.BlockedSendersHash] = value;
			}
		}

		public MultiValuedProperty<byte[]> Certificate
		{
			get
			{
				return (MultiValuedProperty<byte[]>)this[SyncMailUserSchema.Certificate];
			}
		}

		[Parameter(Mandatory = false)]
		public SecurityIdentifier MasterAccountSid
		{
			get
			{
				return (SecurityIdentifier)this[SyncMailUserSchema.MasterAccountSid];
			}
			set
			{
				this[SyncMailUserSchema.MasterAccountSid] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Notes
		{
			get
			{
				return (string)this[SyncMailUserSchema.Notes];
			}
			set
			{
				this[SyncMailUserSchema.Notes] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientDisplayType? RecipientDisplayType
		{
			get
			{
				return (RecipientDisplayType?)this[SyncMailUserSchema.RecipientDisplayType];
			}
			set
			{
				this[SyncMailUserSchema.RecipientDisplayType] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ExchangeResourceType? ResourceType
		{
			get
			{
				return (ExchangeResourceType?)this[ADRecipientSchema.ResourceType];
			}
			set
			{
				this[ADRecipientSchema.ResourceType] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public byte[] SafeRecipientsHash
		{
			get
			{
				return (byte[])this[SyncMailUserSchema.SafeRecipientsHash];
			}
			set
			{
				this[SyncMailUserSchema.SafeRecipientsHash] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public byte[] SafeSendersHash
		{
			get
			{
				return (byte[])this[SyncMailUserSchema.SafeSendersHash];
			}
			set
			{
				this[SyncMailUserSchema.SafeSendersHash] = value;
			}
		}

		public MultiValuedProperty<byte[]> SMimeCertificate
		{
			get
			{
				return (MultiValuedProperty<byte[]>)this[SyncMailUserSchema.SMimeCertificate];
			}
		}

		[Parameter(Mandatory = false)]
		public byte[] Picture
		{
			get
			{
				return (byte[])this[SyncMailUserSchema.ThumbnailPhoto];
			}
			set
			{
				this[SyncMailUserSchema.ThumbnailPhoto] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string DirSyncId
		{
			get
			{
				return (string)this[SyncMailUserSchema.DirSyncId];
			}
			set
			{
				this[SyncMailUserSchema.DirSyncId] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string City
		{
			get
			{
				return (string)this[SyncMailUserSchema.City];
			}
			set
			{
				this[SyncMailUserSchema.City] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Company
		{
			get
			{
				return (string)this[SyncMailUserSchema.Company];
			}
			set
			{
				this[SyncMailUserSchema.Company] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public CountryInfo CountryOrRegion
		{
			get
			{
				return (CountryInfo)this[SyncMailUserSchema.CountryOrRegion];
			}
			set
			{
				this[SyncMailUserSchema.CountryOrRegion] = value;
			}
		}

		public string C
		{
			get
			{
				return (string)this[SyncMailUserSchema.C];
			}
			set
			{
				this[SyncMailUserSchema.C] = value;
			}
		}

		public string Co
		{
			get
			{
				return (string)this[SyncMailUserSchema.Co];
			}
			set
			{
				this[SyncMailUserSchema.Co] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int CountryCode
		{
			get
			{
				return (int)this[SyncMailUserSchema.CountryCode];
			}
			set
			{
				this[SyncMailUserSchema.CountryCode] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Department
		{
			get
			{
				return (string)this[SyncMailUserSchema.Department];
			}
			set
			{
				this[SyncMailUserSchema.Department] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Fax
		{
			get
			{
				return (string)this[SyncMailUserSchema.Fax];
			}
			set
			{
				this[SyncMailUserSchema.Fax] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string FirstName
		{
			get
			{
				return (string)this[SyncMailUserSchema.FirstName];
			}
			set
			{
				this[SyncMailUserSchema.FirstName] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string HomePhone
		{
			get
			{
				return (string)this[SyncMailUserSchema.HomePhone];
			}
			set
			{
				this[SyncMailUserSchema.HomePhone] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Initials
		{
			get
			{
				return (string)this[SyncMailUserSchema.Initials];
			}
			set
			{
				this[SyncMailUserSchema.Initials] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string LastName
		{
			get
			{
				return (string)this[SyncMailUserSchema.LastName];
			}
			set
			{
				this[SyncMailUserSchema.LastName] = value;
			}
		}

		public ADObjectId Manager
		{
			get
			{
				return (ADObjectId)this[SyncMailUserSchema.Manager];
			}
		}

		[Parameter(Mandatory = false)]
		public string MobilePhone
		{
			get
			{
				return (string)this[SyncMailUserSchema.MobilePhone];
			}
			set
			{
				this[SyncMailUserSchema.MobilePhone] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Office
		{
			get
			{
				return (string)this[SyncMailUserSchema.Office];
			}
			set
			{
				this[SyncMailUserSchema.Office] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> OtherFax
		{
			get
			{
				return (MultiValuedProperty<string>)this[SyncMailUserSchema.OtherFax];
			}
			set
			{
				this[SyncMailUserSchema.OtherFax] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> OtherHomePhone
		{
			get
			{
				return (MultiValuedProperty<string>)this[SyncMailUserSchema.OtherHomePhone];
			}
			set
			{
				this[SyncMailUserSchema.OtherHomePhone] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> OtherTelephone
		{
			get
			{
				return (MultiValuedProperty<string>)this[SyncMailUserSchema.OtherTelephone];
			}
			set
			{
				this[SyncMailUserSchema.OtherTelephone] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Pager
		{
			get
			{
				return (string)this[SyncMailUserSchema.Pager];
			}
			set
			{
				this[SyncMailUserSchema.Pager] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Phone
		{
			get
			{
				return (string)this[SyncMailUserSchema.Phone];
			}
			set
			{
				this[SyncMailUserSchema.Phone] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string PostalCode
		{
			get
			{
				return (string)this[SyncMailUserSchema.PostalCode];
			}
			set
			{
				this[SyncMailUserSchema.PostalCode] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int? ResourceCapacity
		{
			get
			{
				return (int?)this[SyncMailUserSchema.ResourceCapacity];
			}
			set
			{
				this[SyncMailUserSchema.ResourceCapacity] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> ResourceCustom
		{
			get
			{
				return (MultiValuedProperty<string>)this[SyncMailUserSchema.ResourceCustom];
			}
			set
			{
				this[SyncMailUserSchema.ResourceCustom] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> ResourceMetaData
		{
			get
			{
				return (MultiValuedProperty<string>)this[SyncMailUserSchema.ResourceMetaData];
			}
			set
			{
				this[SyncMailUserSchema.ResourceMetaData] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string ResourcePropertiesDisplay
		{
			get
			{
				return (string)this[SyncMailUserSchema.ResourcePropertiesDisplay];
			}
			set
			{
				this[SyncMailUserSchema.ResourcePropertiesDisplay] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> ResourceSearchProperties
		{
			get
			{
				return (MultiValuedProperty<string>)this[SyncMailUserSchema.ResourceSearchProperties];
			}
			set
			{
				this[SyncMailUserSchema.ResourceSearchProperties] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string StateOrProvince
		{
			get
			{
				return (string)this[SyncMailUserSchema.StateOrProvince];
			}
			set
			{
				this[SyncMailUserSchema.StateOrProvince] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string StreetAddress
		{
			get
			{
				return (string)this[SyncMailUserSchema.StreetAddress];
			}
			set
			{
				this[SyncMailUserSchema.StreetAddress] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string TelephoneAssistant
		{
			get
			{
				return (string)this[SyncMailUserSchema.TelephoneAssistant];
			}
			set
			{
				this[SyncMailUserSchema.TelephoneAssistant] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Title
		{
			get
			{
				return (string)this[SyncMailUserSchema.Title];
			}
			set
			{
				this[SyncMailUserSchema.Title] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string WebPage
		{
			get
			{
				return (string)this[SyncMailUserSchema.WebPage];
			}
			set
			{
				this[SyncMailUserSchema.WebPage] = value;
			}
		}

		public string IntendedMailboxPlanName
		{
			get
			{
				return (string)this[SyncMailUserSchema.IntendedMailboxPlanName];
			}
			internal set
			{
				this[SyncMailUserSchema.IntendedMailboxPlanName] = value;
			}
		}

		public ADObjectId IntendedMailboxPlan
		{
			get
			{
				return (ADObjectId)this[SyncMailUserSchema.IntendedMailboxPlan];
			}
			set
			{
				this[SyncMailUserSchema.IntendedMailboxPlan] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int? SeniorityIndex
		{
			get
			{
				return (int?)this[SyncMailUserSchema.SeniorityIndex];
			}
			set
			{
				this[SyncMailUserSchema.SeniorityIndex] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string PhoneticDisplayName
		{
			get
			{
				return (string)this[SyncMailUserSchema.PhoneticDisplayName];
			}
			set
			{
				this[SyncMailUserSchema.PhoneticDisplayName] = value;
			}
		}

		public SecurityIdentifier Sid
		{
			get
			{
				return (SecurityIdentifier)this[SyncMailUserSchema.Sid];
			}
		}

		public MultiValuedProperty<SecurityIdentifier> SidHistory
		{
			get
			{
				return (MultiValuedProperty<SecurityIdentifier>)this[SyncMailUserSchema.SidHistory];
			}
		}

		public ReleaseTrack? ReleaseTrack
		{
			get
			{
				return (ReleaseTrack?)this[SyncMailUserSchema.ReleaseTrack];
			}
			set
			{
				this[SyncMailUserSchema.ReleaseTrack] = value;
			}
		}

		public bool EndOfList
		{
			get
			{
				return (bool)this[SyncMailUserSchema.EndOfList];
			}
			internal set
			{
				this[SyncMailUserSchema.EndOfList] = value;
			}
		}

		public byte[] Cookie
		{
			get
			{
				return (byte[])this[SyncMailUserSchema.Cookie];
			}
			internal set
			{
				this[SyncMailUserSchema.Cookie] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IsCalculatedTargetAddress
		{
			get
			{
				return (bool)this[SyncMailUserSchema.IsCalculatedTargetAddress];
			}
			set
			{
				this[SyncMailUserSchema.IsCalculatedTargetAddress] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string OnPremisesObjectId
		{
			get
			{
				return (string)this[SyncMailUserSchema.OnPremisesObjectId];
			}
			set
			{
				this[SyncMailUserSchema.OnPremisesObjectId] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IsDirSynced
		{
			get
			{
				return (bool)this[SyncMailUserSchema.IsDirSynced];
			}
			set
			{
				this[SyncMailUserSchema.IsDirSynced] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> DirSyncAuthorityMetadata
		{
			get
			{
				return (MultiValuedProperty<string>)this[SyncMailUserSchema.DirSyncAuthorityMetadata];
			}
			set
			{
				this[SyncMailUserSchema.DirSyncAuthorityMetadata] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RemoteRecipientType RemoteRecipientType
		{
			get
			{
				return (RemoteRecipientType)this[SyncMailUserSchema.RemoteRecipientType];
			}
			set
			{
				this[SyncMailUserSchema.RemoteRecipientType] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ExcludedFromBackSync
		{
			get
			{
				return (bool)this[SyncMailUserSchema.ExcludedFromBackSync];
			}
			set
			{
				this[SyncMailUserSchema.ExcludedFromBackSync] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> InPlaceHoldsRaw
		{
			get
			{
				return (MultiValuedProperty<string>)this[SyncMailUserSchema.InPlaceHoldsRaw];
			}
			set
			{
				this[SyncMailUserSchema.InPlaceHoldsRaw] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ElcMailboxFlags ElcMailboxFlags
		{
			get
			{
				return (ElcMailboxFlags)this[SyncMailUserSchema.ElcMailboxFlags];
			}
			set
			{
				this[SyncMailUserSchema.ElcMailboxFlags] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool MailboxAuditEnabled
		{
			get
			{
				return (bool)this[SyncMailUserSchema.AuditEnabled];
			}
			set
			{
				this[SyncMailUserSchema.AuditEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan MailboxAuditLogAgeLimit
		{
			get
			{
				return (EnhancedTimeSpan)this[SyncMailUserSchema.AuditLogAgeLimit];
			}
			set
			{
				this[SyncMailUserSchema.AuditLogAgeLimit] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MailboxAuditOperations AuditAdminOperations
		{
			get
			{
				return (MailboxAuditOperations)this[SyncMailUserSchema.AuditAdminFlags];
			}
			set
			{
				this[SyncMailUserSchema.AuditAdminFlags] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MailboxAuditOperations AuditDelegateAdminOperations
		{
			get
			{
				return (MailboxAuditOperations)this[SyncMailUserSchema.AuditDelegateAdminFlags];
			}
			set
			{
				this[SyncMailUserSchema.AuditDelegateAdminFlags] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MailboxAuditOperations AuditDelegateOperations
		{
			get
			{
				return (MailboxAuditOperations)this[SyncMailUserSchema.AuditDelegateFlags];
			}
			set
			{
				this[SyncMailUserSchema.AuditDelegateFlags] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MailboxAuditOperations AuditOwnerOperations
		{
			get
			{
				return (MailboxAuditOperations)this[SyncMailUserSchema.AuditOwnerFlags];
			}
			set
			{
				this[SyncMailUserSchema.AuditOwnerFlags] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool BypassAudit
		{
			get
			{
				return (bool)this[SyncMailUserSchema.AuditBypassEnabled];
			}
			set
			{
				this[SyncMailUserSchema.AuditBypassEnabled] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> SiteMailboxOwners
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[SyncMailUserSchema.SiteMailboxOwners];
			}
			set
			{
				this[SyncMailUserSchema.SiteMailboxOwners] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> SiteMailboxUsers
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[SyncMailUserSchema.SiteMailboxUsers];
			}
			set
			{
				this[SyncMailUserSchema.SiteMailboxUsers] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DateTime? SiteMailboxClosedTime
		{
			get
			{
				return (DateTime?)this[SyncMailUserSchema.SiteMailboxClosedTime];
			}
			set
			{
				this[SyncMailUserSchema.SiteMailboxClosedTime] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Uri SharePointUrl
		{
			get
			{
				return (Uri)this[SyncMailUserSchema.SharePointUrl];
			}
			set
			{
				this[SyncMailUserSchema.SharePointUrl] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AccountDisabled
		{
			get
			{
				return (bool)this[SyncMailUserSchema.AccountDisabled];
			}
			set
			{
				this[SyncMailUserSchema.AccountDisabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DateTime? StsRefreshTokensValidFrom
		{
			get
			{
				return (DateTime?)this[SyncMailUserSchema.StsRefreshTokensValidFrom];
			}
			set
			{
				this[SyncUserSchema.StsRefreshTokensValidFrom] = value;
			}
		}

		private static SyncMailUserSchema schema = ObjectSchema.GetInstance<SyncMailUserSchema>();
	}
}
