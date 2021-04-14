using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Transport.Sync.Common;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation.Schema
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class XSOSyncContact : DisposeTrackableBase, ISyncContact, ISyncObject, IDisposeTrackable, IDisposable
	{
		internal XSOSyncContact(Item contact) : this(contact, true)
		{
		}

		private XSOSyncContact(Item contact, bool ownContact)
		{
			SyncUtilities.ThrowIfArgumentNull("contact", contact);
			this.contact = contact;
			this.ownContact = ownContact;
		}

		public string FirstName
		{
			get
			{
				base.CheckDisposed();
				return this.contactPropertyManager.FirstName.ReadProperty(this.contact);
			}
			private set
			{
				base.CheckDisposed();
				this.contactPropertyManager.FirstName.WriteProperty(this.contact, value);
			}
		}

		public string Hobbies
		{
			get
			{
				base.CheckDisposed();
				return this.contactPropertyManager.Hobbies.ReadProperty(this.contact);
			}
			private set
			{
				base.CheckDisposed();
				this.contactPropertyManager.Hobbies.WriteProperty(this.contact, value);
			}
		}

		public string MiddleName
		{
			get
			{
				base.CheckDisposed();
				return this.contactPropertyManager.MiddleName.ReadProperty(this.contact);
			}
			private set
			{
				base.CheckDisposed();
				this.contactPropertyManager.MiddleName.WriteProperty(this.contact, value);
			}
		}

		public string LastName
		{
			get
			{
				base.CheckDisposed();
				return this.contactPropertyManager.LastName.ReadProperty(this.contact);
			}
			private set
			{
				base.CheckDisposed();
				this.contactPropertyManager.LastName.WriteProperty(this.contact, value);
			}
		}

		public string JobTitle
		{
			get
			{
				base.CheckDisposed();
				return this.contactPropertyManager.JobTile.ReadProperty(this.contact);
			}
			private set
			{
				base.CheckDisposed();
				this.contactPropertyManager.JobTile.WriteProperty(this.contact, value);
			}
		}

		public string FileAs
		{
			get
			{
				base.CheckDisposed();
				return this.contactPropertyManager.FileAs.ReadProperty(this.contact);
			}
			private set
			{
				base.CheckDisposed();
				this.contactPropertyManager.FileAs.WriteProperty(this.contact, value);
			}
		}

		public string BusinessTelephoneNumber
		{
			get
			{
				base.CheckDisposed();
				return this.contactPropertyManager.BusinessTelephoneNumber.ReadProperty(this.contact);
			}
			private set
			{
				base.CheckDisposed();
				this.contactPropertyManager.BusinessTelephoneNumber.WriteProperty(this.contact, value);
			}
		}

		public string HomeTelephoneNumber
		{
			get
			{
				base.CheckDisposed();
				return this.contactPropertyManager.HomeTelephoneNumber.ReadProperty(this.contact);
			}
			private set
			{
				base.CheckDisposed();
				this.contactPropertyManager.HomeTelephoneNumber.WriteProperty(this.contact, value);
			}
		}

		public string MobileTelephoneNumber
		{
			get
			{
				base.CheckDisposed();
				return this.contactPropertyManager.MobileTelephoneNumber.ReadProperty(this.contact);
			}
			private set
			{
				base.CheckDisposed();
				this.contactPropertyManager.MobileTelephoneNumber.WriteProperty(this.contact, value);
			}
		}

		public string BusinessFaxNumber
		{
			get
			{
				base.CheckDisposed();
				return this.contactPropertyManager.BusinessFaxNumber.ReadProperty(this.contact);
			}
			private set
			{
				base.CheckDisposed();
				this.contactPropertyManager.BusinessFaxNumber.WriteProperty(this.contact, value);
			}
		}

		public string OtherTelephoneNumber
		{
			get
			{
				base.CheckDisposed();
				return this.contactPropertyManager.OtherTelephoneNumber.ReadProperty(this.contact);
			}
			private set
			{
				base.CheckDisposed();
				this.contactPropertyManager.OtherTelephoneNumber.WriteProperty(this.contact, value);
			}
		}

		public string CompanyName
		{
			get
			{
				base.CheckDisposed();
				return this.contactPropertyManager.CompanyName.ReadProperty(this.contact);
			}
			private set
			{
				base.CheckDisposed();
				this.contactPropertyManager.CompanyName.WriteProperty(this.contact, value);
			}
		}

		public string Email1Address
		{
			get
			{
				base.CheckDisposed();
				return this.contactPropertyManager.Email1Address.ReadProperty(this.contact);
			}
			private set
			{
				base.CheckDisposed();
				this.contactPropertyManager.Email1Address.WriteProperty(this.contact, value);
			}
		}

		public string Email2Address
		{
			get
			{
				base.CheckDisposed();
				return this.contactPropertyManager.Email2Address.ReadProperty(this.contact);
			}
			private set
			{
				base.CheckDisposed();
				this.contactPropertyManager.Email2Address.WriteProperty(this.contact, value);
			}
		}

		public string Email3Address
		{
			get
			{
				base.CheckDisposed();
				return this.contactPropertyManager.Email3Address.ReadProperty(this.contact);
			}
			private set
			{
				base.CheckDisposed();
				this.contactPropertyManager.Email3Address.WriteProperty(this.contact, value);
			}
		}

		public string Webpage
		{
			get
			{
				base.CheckDisposed();
				return this.contactPropertyManager.WebPage.ReadProperty(this.contact);
			}
			private set
			{
				base.CheckDisposed();
				this.contactPropertyManager.WebPage.WriteProperty(this.contact, value);
			}
		}

		public string BusinessAddressStreet
		{
			get
			{
				base.CheckDisposed();
				return this.contactPropertyManager.BusinessAddressStreet.ReadProperty(this.contact);
			}
			private set
			{
				base.CheckDisposed();
				this.contactPropertyManager.BusinessAddressStreet.WriteProperty(this.contact, value);
			}
		}

		public string BusinessAddressCity
		{
			get
			{
				base.CheckDisposed();
				return this.contactPropertyManager.BusinessAddressCity.ReadProperty(this.contact);
			}
			private set
			{
				base.CheckDisposed();
				this.contactPropertyManager.BusinessAddressCity.WriteProperty(this.contact, value);
			}
		}

		public string BusinessAddressState
		{
			get
			{
				base.CheckDisposed();
				return this.contactPropertyManager.BusinessAddressState.ReadProperty(this.contact);
			}
			private set
			{
				base.CheckDisposed();
				this.contactPropertyManager.BusinessAddressState.WriteProperty(this.contact, value);
			}
		}

		public string BusinessAddressPostalCode
		{
			get
			{
				base.CheckDisposed();
				return this.contactPropertyManager.BusinessAddressPostalCode.ReadProperty(this.contact);
			}
			private set
			{
				base.CheckDisposed();
				this.contactPropertyManager.BusinessAddressPostalCode.WriteProperty(this.contact, value);
			}
		}

		public string BusinessAddressCountry
		{
			get
			{
				base.CheckDisposed();
				return this.contactPropertyManager.BusinessAddressCountry.ReadProperty(this.contact);
			}
			private set
			{
				base.CheckDisposed();
				this.contactPropertyManager.BusinessAddressCountry.WriteProperty(this.contact, value);
			}
		}

		public string HomeAddressStreet
		{
			get
			{
				base.CheckDisposed();
				return this.contactPropertyManager.HomeAddressStreet.ReadProperty(this.contact);
			}
			private set
			{
				base.CheckDisposed();
				this.contactPropertyManager.HomeAddressStreet.WriteProperty(this.contact, value);
			}
		}

		public string HomeAddressCity
		{
			get
			{
				base.CheckDisposed();
				return this.contactPropertyManager.HomeAddressCity.ReadProperty(this.contact);
			}
			private set
			{
				base.CheckDisposed();
				this.contactPropertyManager.HomeAddressCity.WriteProperty(this.contact, value);
			}
		}

		public string HomeAddressState
		{
			get
			{
				base.CheckDisposed();
				return this.contactPropertyManager.HomeAddressState.ReadProperty(this.contact);
			}
			private set
			{
				base.CheckDisposed();
				this.contactPropertyManager.HomeAddressState.WriteProperty(this.contact, value);
			}
		}

		public string HomeAddressPostalCode
		{
			get
			{
				base.CheckDisposed();
				return this.contactPropertyManager.HomeAddressPostalCode.ReadProperty(this.contact);
			}
			private set
			{
				base.CheckDisposed();
				this.contactPropertyManager.HomeAddressPostalCode.WriteProperty(this.contact, value);
			}
		}

		public string HomeAddressCountry
		{
			get
			{
				base.CheckDisposed();
				return this.contactPropertyManager.HomeAddressCountry.ReadProperty(this.contact);
			}
			private set
			{
				base.CheckDisposed();
				this.contactPropertyManager.HomeAddressCountry.WriteProperty(this.contact, value);
			}
		}

		public string IMAddress
		{
			get
			{
				base.CheckDisposed();
				return this.contactPropertyManager.IMAddress.ReadProperty(this.contact);
			}
			private set
			{
				base.CheckDisposed();
				this.contactPropertyManager.IMAddress.WriteProperty(this.contact, value);
			}
		}

		public ExDateTime? BirthDate
		{
			get
			{
				base.CheckDisposed();
				return this.contactPropertyManager.BirthDate.ReadProperty(this.contact);
			}
			private set
			{
				base.CheckDisposed();
				this.contactPropertyManager.BirthDate.WriteProperty(this.contact, value);
			}
		}

		public ExDateTime? BirthDateLocal
		{
			get
			{
				base.CheckDisposed();
				return this.contactPropertyManager.BirthDateLocal.ReadProperty(this.contact);
			}
			private set
			{
				base.CheckDisposed();
				this.contactPropertyManager.BirthDateLocal.WriteProperty(this.contact, value);
			}
		}

		public string DisplayName
		{
			get
			{
				base.CheckDisposed();
				return this.contactPropertyManager.DisplayName.ReadProperty(this.contact);
			}
			private set
			{
				base.CheckDisposed();
				this.contactPropertyManager.DisplayName.WriteProperty(this.contact, value);
			}
		}

		public string Location
		{
			get
			{
				base.CheckDisposed();
				return this.contactPropertyManager.Location.ReadProperty(this.contact);
			}
			private set
			{
				base.CheckDisposed();
				this.contactPropertyManager.Location.WriteProperty(this.contact, value);
			}
		}

		public byte[] OscContactSources
		{
			get
			{
				base.CheckDisposed();
				return this.contactPropertyManager.OscContactSources.ReadProperty(this.contact);
			}
			private set
			{
				base.CheckDisposed();
				this.contactPropertyManager.OscContactSources.WriteProperty(this.contact, value);
			}
		}

		public string PartnerNetworkId
		{
			get
			{
				base.CheckDisposed();
				return this.contactPropertyManager.PartnerNetworkId.ReadProperty(this.contact);
			}
			private set
			{
				base.CheckDisposed();
				this.contactPropertyManager.PartnerNetworkId.WriteProperty(this.contact, value);
			}
		}

		public string PartnerNetworkUserId
		{
			get
			{
				base.CheckDisposed();
				return this.contactPropertyManager.PartnerNetworkUserId.ReadProperty(this.contact);
			}
			private set
			{
				base.CheckDisposed();
				this.contactPropertyManager.PartnerNetworkUserId.WriteProperty(this.contact, value);
			}
		}

		public string PartnerNetworkThumbnailPhotoUrl
		{
			get
			{
				base.CheckDisposed();
				return this.contactPropertyManager.PartnerNetworkThumbnailPhotoUrl.ReadProperty(this.contact);
			}
			private set
			{
				base.CheckDisposed();
				this.contactPropertyManager.PartnerNetworkThumbnailPhotoUrl.WriteProperty(this.contact, value);
			}
		}

		public string PartnerNetworkProfilePhotoUrl
		{
			get
			{
				base.CheckDisposed();
				return this.contactPropertyManager.PartnerNetworkProfilePhotoUrl.ReadProperty(this.contact);
			}
			private set
			{
				base.CheckDisposed();
				this.contactPropertyManager.PartnerNetworkProfilePhotoUrl.WriteProperty(this.contact, value);
			}
		}

		public string PartnerNetworkContactType
		{
			get
			{
				base.CheckDisposed();
				return this.contactPropertyManager.PartnerNetworkContactType.ReadProperty(this.contact);
			}
			private set
			{
				base.CheckDisposed();
				this.contactPropertyManager.PartnerNetworkContactType.WriteProperty(this.contact, value);
			}
		}

		public ExDateTime? PeopleConnectionCreationTime
		{
			get
			{
				base.CheckDisposed();
				return this.contactPropertyManager.PeopleConnectionCreationTime.ReadProperty(this.contact);
			}
			private set
			{
				base.CheckDisposed();
				this.contactPropertyManager.PeopleConnectionCreationTime.WriteProperty(this.contact, value);
			}
		}

		public string ProtectedEmailAddress
		{
			get
			{
				base.CheckDisposed();
				return this.contactPropertyManager.ProtectedEmailAddress.ReadProperty(this.contact);
			}
			private set
			{
				base.CheckDisposed();
				this.contactPropertyManager.ProtectedEmailAddress.WriteProperty(this.contact, value);
			}
		}

		public string ProtectedPhoneNumber
		{
			get
			{
				base.CheckDisposed();
				return this.contactPropertyManager.ProtectedPhoneNumber.ReadProperty(this.contact);
			}
			private set
			{
				base.CheckDisposed();
				this.contactPropertyManager.ProtectedPhoneNumber.WriteProperty(this.contact, value);
			}
		}

		public string Schools
		{
			get
			{
				base.CheckDisposed();
				return this.contactPropertyManager.Schools.ReadProperty(this.contact);
			}
			private set
			{
				base.CheckDisposed();
				this.contactPropertyManager.Schools.WriteProperty(this.contact, value);
			}
		}

		public ExDateTime? LastModifiedTime
		{
			get
			{
				base.CheckDisposed();
				return this.contactPropertyManager.LastModifiedTime.ReadProperty(this.contact);
			}
			private set
			{
				base.CheckDisposed();
				this.contactPropertyManager.LastModifiedTime.WriteProperty(this.contact, value);
			}
		}

		public SchemaType Type
		{
			get
			{
				base.CheckDisposed();
				return SchemaType.Contact;
			}
		}

		public static void CopyPropertiesFromISyncContact(Item contact, ISyncContact syncContact)
		{
			SyncUtilities.ThrowIfArgumentNull("contact", contact);
			SyncUtilities.ThrowIfArgumentNull("syncContact", syncContact);
			using (XSOSyncContact xsosyncContact = new XSOSyncContact(contact, false))
			{
				xsosyncContact.CopyPropertiesFromISyncContact(syncContact);
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.ownContact && this.contact != null)
				{
					this.contact.Dispose();
				}
				this.contact = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<XSOSyncContact>(this);
		}

		private void CopyPropertiesFromISyncContact(ISyncContact syncContact)
		{
			this.BirthDate = syncContact.BirthDate;
			this.BirthDateLocal = syncContact.BirthDateLocal;
			this.BusinessAddressCity = syncContact.BusinessAddressCity;
			this.BusinessAddressCountry = syncContact.BusinessAddressCountry;
			this.BusinessAddressPostalCode = syncContact.BusinessAddressPostalCode;
			this.BusinessAddressState = syncContact.BusinessAddressState;
			this.BusinessAddressStreet = syncContact.BusinessAddressStreet;
			this.BusinessFaxNumber = syncContact.BusinessFaxNumber;
			this.BusinessTelephoneNumber = syncContact.BusinessTelephoneNumber;
			this.CompanyName = syncContact.CompanyName;
			this.DisplayName = syncContact.DisplayName;
			this.Email1Address = syncContact.Email1Address;
			this.Email2Address = syncContact.Email2Address;
			this.Email3Address = syncContact.Email3Address;
			this.FileAs = syncContact.FileAs;
			this.FirstName = syncContact.FirstName;
			this.Hobbies = syncContact.Hobbies;
			this.HomeAddressCity = syncContact.HomeAddressCity;
			this.HomeAddressCountry = syncContact.HomeAddressCountry;
			this.HomeAddressPostalCode = syncContact.HomeAddressPostalCode;
			this.HomeAddressState = syncContact.HomeAddressState;
			this.HomeAddressStreet = syncContact.HomeAddressStreet;
			this.HomeTelephoneNumber = syncContact.HomeTelephoneNumber;
			this.IMAddress = syncContact.IMAddress;
			this.JobTitle = syncContact.JobTitle;
			this.LastName = syncContact.LastName;
			this.LastModifiedTime = syncContact.LastModifiedTime;
			this.Location = syncContact.Location;
			this.MiddleName = syncContact.MiddleName;
			this.MobileTelephoneNumber = syncContact.MobileTelephoneNumber;
			this.OscContactSources = syncContact.OscContactSources;
			this.OtherTelephoneNumber = syncContact.OtherTelephoneNumber;
			this.PartnerNetworkContactType = syncContact.PartnerNetworkContactType;
			this.PartnerNetworkId = syncContact.PartnerNetworkId;
			this.PartnerNetworkProfilePhotoUrl = syncContact.PartnerNetworkProfilePhotoUrl;
			this.PartnerNetworkThumbnailPhotoUrl = syncContact.PartnerNetworkThumbnailPhotoUrl;
			this.PartnerNetworkUserId = syncContact.PartnerNetworkUserId;
			this.PeopleConnectionCreationTime = syncContact.PeopleConnectionCreationTime;
			this.ProtectedEmailAddress = syncContact.ProtectedEmailAddress;
			this.ProtectedPhoneNumber = syncContact.ProtectedPhoneNumber;
			this.Schools = syncContact.Schools;
			this.Webpage = syncContact.Webpage;
		}

		private readonly ContactPropertyManager contactPropertyManager = ContactPropertyManager.Instance;

		private Item contact;

		private readonly bool ownContact;
	}
}
