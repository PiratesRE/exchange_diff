using System;
using System.Management.Automation;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory.Provisioning;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[ProvisioningObjectTag("SyncMailbox")]
	[Serializable]
	public class SyncMailbox : Mailbox
	{
		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return SyncMailbox.schema;
			}
		}

		public SyncMailbox()
		{
			base.SetObjectClass("user");
		}

		public SyncMailbox(ADUser dataObject) : base(dataObject)
		{
		}

		internal new static SyncMailbox FromDataObject(ADUser dataObject)
		{
			if (dataObject == null)
			{
				return null;
			}
			return new SyncMailbox(dataObject);
		}

		[Parameter(Mandatory = false)]
		public string AssistantName
		{
			get
			{
				return (string)this[SyncMailboxSchema.AssistantName];
			}
			set
			{
				this[SyncMailboxSchema.AssistantName] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public byte[] BlockedSendersHash
		{
			get
			{
				return (byte[])this[SyncMailboxSchema.BlockedSendersHash];
			}
			set
			{
				this[SyncMailboxSchema.BlockedSendersHash] = value;
			}
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

		public MultiValuedProperty<byte[]> Certificate
		{
			get
			{
				return (MultiValuedProperty<byte[]>)this[MailboxSchema.UserCertificate];
			}
		}

		[Parameter(Mandatory = false)]
		public SecurityIdentifier MasterAccountSid
		{
			get
			{
				return (SecurityIdentifier)this[MailboxSchema.MasterAccountSid];
			}
			set
			{
				this[MailboxSchema.MasterAccountSid] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Notes
		{
			get
			{
				return (string)this[SyncMailboxSchema.Notes];
			}
			set
			{
				this[SyncMailboxSchema.Notes] = value;
			}
		}

		public RecipientDisplayType? RecipientDisplayType
		{
			get
			{
				return (RecipientDisplayType?)this[SyncMailboxSchema.RecipientDisplayType];
			}
			internal set
			{
				this[SyncMailboxSchema.RecipientDisplayType] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public byte[] SafeRecipientsHash
		{
			get
			{
				return (byte[])this[SyncMailboxSchema.SafeRecipientsHash];
			}
			set
			{
				this[SyncMailboxSchema.SafeRecipientsHash] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public byte[] SafeSendersHash
		{
			get
			{
				return (byte[])this[SyncMailboxSchema.SafeSendersHash];
			}
			set
			{
				this[SyncMailboxSchema.SafeSendersHash] = value;
			}
		}

		public MultiValuedProperty<byte[]> SMimeCertificate
		{
			get
			{
				return (MultiValuedProperty<byte[]>)this[MailboxSchema.UserSMimeCertificate];
			}
		}

		[Parameter(Mandatory = false)]
		public byte[] Picture
		{
			get
			{
				return (byte[])this[SyncMailboxSchema.ThumbnailPhoto];
			}
			set
			{
				this[SyncMailboxSchema.ThumbnailPhoto] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public byte[] SpokenName
		{
			get
			{
				return (byte[])this[SyncMailboxSchema.UMSpokenName];
			}
			set
			{
				this[SyncMailboxSchema.UMSpokenName] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string DirSyncId
		{
			get
			{
				return (string)this[SyncMailboxSchema.DirSyncId];
			}
			set
			{
				this[SyncMailboxSchema.DirSyncId] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string City
		{
			get
			{
				return (string)this[SyncMailboxSchema.City];
			}
			set
			{
				this[SyncMailboxSchema.City] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Company
		{
			get
			{
				return (string)this[SyncMailboxSchema.Company];
			}
			set
			{
				this[SyncMailboxSchema.Company] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public CountryInfo CountryOrRegion
		{
			get
			{
				return (CountryInfo)this[SyncMailboxSchema.CountryOrRegion];
			}
			set
			{
				this[SyncMailboxSchema.CountryOrRegion] = value;
			}
		}

		public string C
		{
			get
			{
				return (string)this[SyncMailboxSchema.C];
			}
			set
			{
				this[SyncMailboxSchema.C] = value;
			}
		}

		public string Co
		{
			get
			{
				return (string)this[SyncMailboxSchema.Co];
			}
			set
			{
				this[SyncMailboxSchema.Co] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int CountryCode
		{
			get
			{
				return (int)this[SyncMailboxSchema.CountryCode];
			}
			set
			{
				this[SyncMailboxSchema.CountryCode] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Department
		{
			get
			{
				return (string)this[SyncMailboxSchema.Department];
			}
			set
			{
				this[SyncMailboxSchema.Department] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Fax
		{
			get
			{
				return (string)this[SyncMailboxSchema.Fax];
			}
			set
			{
				this[SyncMailboxSchema.Fax] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string FirstName
		{
			get
			{
				return (string)this[SyncMailboxSchema.FirstName];
			}
			set
			{
				this[SyncMailboxSchema.FirstName] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string HomePhone
		{
			get
			{
				return (string)this[SyncMailboxSchema.HomePhone];
			}
			set
			{
				this[SyncMailboxSchema.HomePhone] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Initials
		{
			get
			{
				return (string)this[SyncMailboxSchema.Initials];
			}
			set
			{
				this[SyncMailboxSchema.Initials] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string LastName
		{
			get
			{
				return (string)this[SyncMailboxSchema.LastName];
			}
			set
			{
				this[SyncMailboxSchema.LastName] = value;
			}
		}

		public ADObjectId Manager
		{
			get
			{
				return (ADObjectId)this[SyncMailboxSchema.Manager];
			}
		}

		[Parameter(Mandatory = false)]
		public string MobilePhone
		{
			get
			{
				return (string)this[SyncMailboxSchema.MobilePhone];
			}
			set
			{
				this[SyncMailboxSchema.MobilePhone] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> OtherFax
		{
			get
			{
				return (MultiValuedProperty<string>)this[SyncMailboxSchema.OtherFax];
			}
			set
			{
				this[SyncMailboxSchema.OtherFax] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> OtherHomePhone
		{
			get
			{
				return (MultiValuedProperty<string>)this[SyncMailboxSchema.OtherHomePhone];
			}
			set
			{
				this[SyncMailboxSchema.OtherHomePhone] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> OtherTelephone
		{
			get
			{
				return (MultiValuedProperty<string>)this[SyncMailboxSchema.OtherTelephone];
			}
			set
			{
				this[SyncMailboxSchema.OtherTelephone] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Pager
		{
			get
			{
				return (string)this[SyncMailboxSchema.Pager];
			}
			set
			{
				this[SyncMailboxSchema.Pager] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Phone
		{
			get
			{
				return (string)this[SyncMailboxSchema.Phone];
			}
			set
			{
				this[SyncMailboxSchema.Phone] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string PostalCode
		{
			get
			{
				return (string)this[SyncMailboxSchema.PostalCode];
			}
			set
			{
				this[SyncMailboxSchema.PostalCode] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string StateOrProvince
		{
			get
			{
				return (string)this[SyncMailboxSchema.StateOrProvince];
			}
			set
			{
				this[SyncMailboxSchema.StateOrProvince] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string StreetAddress
		{
			get
			{
				return (string)this[SyncMailboxSchema.StreetAddress];
			}
			set
			{
				this[SyncMailboxSchema.StreetAddress] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string TelephoneAssistant
		{
			get
			{
				return (string)this[SyncMailboxSchema.TelephoneAssistant];
			}
			set
			{
				this[SyncMailboxSchema.TelephoneAssistant] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Title
		{
			get
			{
				return (string)this[SyncMailboxSchema.Title];
			}
			set
			{
				this[SyncMailboxSchema.Title] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string WebPage
		{
			get
			{
				return (string)this[SyncMailboxSchema.WebPage];
			}
			set
			{
				this[SyncMailboxSchema.WebPage] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int? SeniorityIndex
		{
			get
			{
				return (int?)this[SyncMailboxSchema.SeniorityIndex];
			}
			set
			{
				this[SyncMailboxSchema.SeniorityIndex] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string PhoneticDisplayName
		{
			get
			{
				return (string)this[SyncMailboxSchema.PhoneticDisplayName];
			}
			set
			{
				this[SyncMailboxSchema.PhoneticDisplayName] = value;
			}
		}

		public SecurityIdentifier Sid
		{
			get
			{
				return (SecurityIdentifier)this[SyncMailboxSchema.Sid];
			}
		}

		public MultiValuedProperty<SecurityIdentifier> SidHistory
		{
			get
			{
				return (MultiValuedProperty<SecurityIdentifier>)this[SyncMailboxSchema.SidHistory];
			}
		}

		public ReleaseTrack? ReleaseTrack
		{
			get
			{
				return (ReleaseTrack?)this[SyncMailboxSchema.ReleaseTrack];
			}
			set
			{
				this[SyncMailboxSchema.ReleaseTrack] = value;
			}
		}

		public bool EndOfList
		{
			get
			{
				return (bool)this[SyncMailboxSchema.EndOfList];
			}
			internal set
			{
				this[SyncMailboxSchema.EndOfList] = value;
			}
		}

		public byte[] Cookie
		{
			get
			{
				return (byte[])this[SyncMailboxSchema.Cookie];
			}
			internal set
			{
				this[SyncMailboxSchema.Cookie] = value;
			}
		}

		public string MailboxPlanName
		{
			get
			{
				return (string)this[SyncMailboxSchema.MailboxPlanName];
			}
			internal set
			{
				this[SyncMailboxSchema.MailboxPlanName] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string OnPremisesObjectId
		{
			get
			{
				return (string)this[SyncMailboxSchema.OnPremisesObjectId];
			}
			set
			{
				this[SyncMailboxSchema.OnPremisesObjectId] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IsDirSynced
		{
			get
			{
				return (bool)this[SyncMailboxSchema.IsDirSynced];
			}
			set
			{
				this[SyncMailboxSchema.IsDirSynced] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> DirSyncAuthorityMetadata
		{
			get
			{
				return (MultiValuedProperty<string>)this[SyncMailboxSchema.DirSyncAuthorityMetadata];
			}
			set
			{
				this[SyncMailboxSchema.DirSyncAuthorityMetadata] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ExcludedFromBackSync
		{
			get
			{
				return (bool)this[SyncMailboxSchema.ExcludedFromBackSync];
			}
			set
			{
				this[SyncMailboxSchema.ExcludedFromBackSync] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> InPlaceHoldsRaw
		{
			get
			{
				return (MultiValuedProperty<string>)this[SyncMailboxSchema.InPlaceHoldsRaw];
			}
			set
			{
				this[SyncMailboxSchema.InPlaceHoldsRaw] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool LEOEnabled
		{
			get
			{
				return (bool)this[SyncMailboxSchema.LEOEnabled];
			}
			set
			{
				this[SyncMailboxSchema.LEOEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AccountDisabled
		{
			get
			{
				return (bool)this[SyncMailboxSchema.AccountDisabled];
			}
			set
			{
				this[SyncMailboxSchema.AccountDisabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DateTime? StsRefreshTokensValidFrom
		{
			get
			{
				return (DateTime?)this[SyncMailboxSchema.StsRefreshTokensValidFrom];
			}
			set
			{
				this[SyncMailboxSchema.StsRefreshTokensValidFrom] = value;
			}
		}

		private static SyncMailboxSchema schema = ObjectSchema.GetInstance<SyncMailboxSchema>();
	}
}
