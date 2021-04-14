using System;
using System.Management.Automation;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory.Provisioning;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[ProvisioningObjectTag("SyncMailContact")]
	[Serializable]
	public class SyncMailContact : MailContact
	{
		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return SyncMailContact.schema;
			}
		}

		public SyncMailContact()
		{
			base.SetObjectClass("contact");
		}

		public SyncMailContact(ADContact dataObject) : base(dataObject)
		{
		}

		internal new static SyncMailContact FromDataObject(ADContact dataObject)
		{
			if (dataObject == null)
			{
				return null;
			}
			return new SyncMailContact(dataObject);
		}

		[Parameter(Mandatory = false)]
		public string AssistantName
		{
			get
			{
				return (string)this[SyncMailContactSchema.AssistantName];
			}
			set
			{
				this[SyncMailContactSchema.AssistantName] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public byte[] BlockedSendersHash
		{
			get
			{
				return (byte[])this[SyncMailContactSchema.BlockedSendersHash];
			}
			set
			{
				this[SyncMailContactSchema.BlockedSendersHash] = value;
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

		public string ImmutableId
		{
			get
			{
				return (string)this[SyncMailContactSchema.ImmutableId];
			}
		}

		[Parameter(Mandatory = false)]
		public SecurityIdentifier MasterAccountSid
		{
			get
			{
				return (SecurityIdentifier)this[SyncMailContactSchema.MasterAccountSid];
			}
			set
			{
				this[SyncMailContactSchema.MasterAccountSid] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Notes
		{
			get
			{
				return (string)this[SyncMailContactSchema.Notes];
			}
			set
			{
				this[SyncMailContactSchema.Notes] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientDisplayType? RecipientDisplayType
		{
			get
			{
				return (RecipientDisplayType?)this[SyncMailContactSchema.RecipientDisplayType];
			}
			set
			{
				this[SyncMailContactSchema.RecipientDisplayType] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public byte[] SafeRecipientsHash
		{
			get
			{
				return (byte[])this[SyncMailContactSchema.SafeRecipientsHash];
			}
			set
			{
				this[SyncMailContactSchema.SafeRecipientsHash] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public byte[] SafeSendersHash
		{
			get
			{
				return (byte[])this[SyncMailContactSchema.SafeSendersHash];
			}
			set
			{
				this[SyncMailContactSchema.SafeSendersHash] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string DirSyncId
		{
			get
			{
				return (string)this[SyncMailContactSchema.DirSyncId];
			}
			set
			{
				this[SyncMailContactSchema.DirSyncId] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string City
		{
			get
			{
				return (string)this[SyncMailContactSchema.City];
			}
			set
			{
				this[SyncMailContactSchema.City] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Company
		{
			get
			{
				return (string)this[SyncMailContactSchema.Company];
			}
			set
			{
				this[SyncMailContactSchema.Company] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public CountryInfo CountryOrRegion
		{
			get
			{
				return (CountryInfo)this[SyncMailContactSchema.CountryOrRegion];
			}
			set
			{
				this[SyncMailContactSchema.CountryOrRegion] = value;
			}
		}

		public string C
		{
			get
			{
				return (string)this[SyncMailContactSchema.C];
			}
			set
			{
				this[SyncMailContactSchema.C] = value;
			}
		}

		public string Co
		{
			get
			{
				return (string)this[SyncMailContactSchema.Co];
			}
			set
			{
				this[SyncMailContactSchema.Co] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int CountryCode
		{
			get
			{
				return (int)this[SyncMailContactSchema.CountryCode];
			}
			set
			{
				this[SyncMailContactSchema.CountryCode] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Department
		{
			get
			{
				return (string)this[SyncMailContactSchema.Department];
			}
			set
			{
				this[SyncMailContactSchema.Department] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Fax
		{
			get
			{
				return (string)this[SyncMailContactSchema.Fax];
			}
			set
			{
				this[SyncMailContactSchema.Fax] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string FirstName
		{
			get
			{
				return (string)this[SyncMailContactSchema.FirstName];
			}
			set
			{
				this[SyncMailContactSchema.FirstName] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string HomePhone
		{
			get
			{
				return (string)this[SyncMailContactSchema.HomePhone];
			}
			set
			{
				this[SyncMailContactSchema.HomePhone] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Initials
		{
			get
			{
				return (string)this[SyncMailContactSchema.Initials];
			}
			set
			{
				this[SyncMailContactSchema.Initials] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string LastName
		{
			get
			{
				return (string)this[SyncMailContactSchema.LastName];
			}
			set
			{
				this[SyncMailContactSchema.LastName] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string MobilePhone
		{
			get
			{
				return (string)this[SyncMailContactSchema.MobilePhone];
			}
			set
			{
				this[SyncMailContactSchema.MobilePhone] = value;
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
		public string Office
		{
			get
			{
				return (string)this[SyncMailContactSchema.Office];
			}
			set
			{
				this[SyncMailContactSchema.Office] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> OtherFax
		{
			get
			{
				return (MultiValuedProperty<string>)this[SyncMailContactSchema.OtherFax];
			}
			set
			{
				this[SyncMailContactSchema.OtherFax] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> OtherHomePhone
		{
			get
			{
				return (MultiValuedProperty<string>)this[SyncMailContactSchema.OtherHomePhone];
			}
			set
			{
				this[SyncMailContactSchema.OtherHomePhone] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> OtherTelephone
		{
			get
			{
				return (MultiValuedProperty<string>)this[SyncMailContactSchema.OtherTelephone];
			}
			set
			{
				this[SyncMailContactSchema.OtherTelephone] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Pager
		{
			get
			{
				return (string)this[SyncMailContactSchema.Pager];
			}
			set
			{
				this[SyncMailContactSchema.Pager] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Phone
		{
			get
			{
				return (string)this[SyncMailContactSchema.Phone];
			}
			set
			{
				this[SyncMailContactSchema.Phone] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string PostalCode
		{
			get
			{
				return (string)this[SyncMailContactSchema.PostalCode];
			}
			set
			{
				this[SyncMailContactSchema.PostalCode] = value;
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
		public int? ResourceCapacity
		{
			get
			{
				return (int?)this[ADRecipientSchema.ResourceCapacity];
			}
			set
			{
				this[ADRecipientSchema.ResourceCapacity] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string StateOrProvince
		{
			get
			{
				return (string)this[SyncMailContactSchema.StateOrProvince];
			}
			set
			{
				this[SyncMailContactSchema.StateOrProvince] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string StreetAddress
		{
			get
			{
				return (string)this[SyncMailContactSchema.StreetAddress];
			}
			set
			{
				this[SyncMailContactSchema.StreetAddress] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string TelephoneAssistant
		{
			get
			{
				return (string)this[SyncMailContactSchema.TelephoneAssistant];
			}
			set
			{
				this[SyncMailContactSchema.TelephoneAssistant] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Title
		{
			get
			{
				return (string)this[SyncMailContactSchema.Title];
			}
			set
			{
				this[SyncMailContactSchema.Title] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string WebPage
		{
			get
			{
				return (string)this[SyncMailContactSchema.WebPage];
			}
			set
			{
				this[SyncMailContactSchema.WebPage] = value;
			}
		}

		public bool EndOfList
		{
			get
			{
				return (bool)this[SyncMailContactSchema.EndOfList];
			}
			internal set
			{
				this[SyncMailContactSchema.EndOfList] = value;
			}
		}

		public byte[] Cookie
		{
			get
			{
				return (byte[])this[SyncMailContactSchema.Cookie];
			}
			internal set
			{
				this[SyncMailContactSchema.Cookie] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int? SeniorityIndex
		{
			get
			{
				return (int?)this[SyncMailContactSchema.SeniorityIndex];
			}
			set
			{
				this[SyncMailContactSchema.SeniorityIndex] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string PhoneticDisplayName
		{
			get
			{
				return (string)this[SyncMailContactSchema.PhoneticDisplayName];
			}
			set
			{
				this[SyncMailContactSchema.PhoneticDisplayName] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string OnPremisesObjectId
		{
			get
			{
				return (string)this[SyncMailContactSchema.OnPremisesObjectId];
			}
			set
			{
				this[SyncMailContactSchema.OnPremisesObjectId] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IsDirSynced
		{
			get
			{
				return (bool)this[SyncMailContactSchema.IsDirSynced];
			}
			set
			{
				this[SyncMailContactSchema.IsDirSynced] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> DirSyncAuthorityMetadata
		{
			get
			{
				return (MultiValuedProperty<string>)this[SyncMailContactSchema.DirSyncAuthorityMetadata];
			}
			set
			{
				this[SyncMailContactSchema.DirSyncAuthorityMetadata] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ExcludedFromBackSync
		{
			get
			{
				return (bool)this[SyncMailContactSchema.ExcludedFromBackSync];
			}
			set
			{
				this[SyncMailContactSchema.ExcludedFromBackSync] = value;
			}
		}

		private static SyncMailContactSchema schema = ObjectSchema.GetInstance<SyncMailContactSchema>();
	}
}
