using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.MailboxTransport.ContentAggregation.Schema;
using Microsoft.Exchange.Net.LinkedIn;
using Microsoft.Exchange.Transport.Sync.Common;

namespace Microsoft.Exchange.Transport.Sync.Worker.Framework.Provider.LinkedIn
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class LinkedInContact : DisposeTrackableBase, ISyncContact, ISyncObject, IDisposeTrackable, IDisposable
	{
		internal LinkedInContact(LinkedInPerson contact, ExDateTime peopleConnectionCreationTime)
		{
			SyncUtilities.ThrowIfArgumentNull("contact", contact);
			SyncUtilities.ThrowIfArgumentNullOrEmpty("contact.Id", contact.Id);
			this.contact = contact;
			this.lastModifiedTime = new ExDateTime?(ExDateTime.UtcNow);
			this.peopleConnectionCreationTime = new ExDateTime?(peopleConnectionCreationTime);
			this.SetJobTitle();
			this.SetPhoneNumbers();
			this.SetImAddress();
			this.SetBirthDate();
			this.InitializeOscContactSources();
			this.SetPhotoUrl();
			this.SuppressDisposeTracker();
		}

		public SchemaType Type
		{
			get
			{
				base.CheckDisposed();
				return SchemaType.Contact;
			}
		}

		public string FirstName
		{
			get
			{
				base.CheckDisposed();
				return this.contact.FirstName;
			}
		}

		public string Hobbies
		{
			get
			{
				base.CheckDisposed();
				return null;
			}
		}

		public string MiddleName
		{
			get
			{
				base.CheckDisposed();
				return null;
			}
		}

		public string LastName
		{
			get
			{
				base.CheckDisposed();
				return this.contact.LastName;
			}
		}

		public string JobTitle
		{
			get
			{
				base.CheckDisposed();
				return this.jobTitle;
			}
		}

		public string FileAs
		{
			get
			{
				base.CheckDisposed();
				return null;
			}
		}

		public string BusinessTelephoneNumber
		{
			get
			{
				base.CheckDisposed();
				return this.businessPhoneNumber;
			}
		}

		public string HomeTelephoneNumber
		{
			get
			{
				base.CheckDisposed();
				return this.homePhoneNumber;
			}
		}

		public string MobileTelephoneNumber
		{
			get
			{
				base.CheckDisposed();
				return this.mobilePhoneNumber;
			}
		}

		public string BusinessFaxNumber
		{
			get
			{
				base.CheckDisposed();
				return null;
			}
		}

		public string OtherTelephoneNumber
		{
			get
			{
				base.CheckDisposed();
				return null;
			}
		}

		public string CompanyName
		{
			get
			{
				base.CheckDisposed();
				if (this.contact.ThreeCurrentPositions != null && this.contact.ThreeCurrentPositions.Positions != null && this.contact.ThreeCurrentPositions.Positions.Count > 0 && this.contact.ThreeCurrentPositions.Positions[0].Company != null)
				{
					return this.contact.ThreeCurrentPositions.Positions[0].Company.Name;
				}
				return null;
			}
		}

		public string Email1Address
		{
			get
			{
				base.CheckDisposed();
				return this.contact.EmailAddress;
			}
		}

		public string Email2Address
		{
			get
			{
				base.CheckDisposed();
				return null;
			}
		}

		public string Email3Address
		{
			get
			{
				base.CheckDisposed();
				return null;
			}
		}

		public string Webpage
		{
			get
			{
				base.CheckDisposed();
				return this.contact.PublicProfileUrl;
			}
		}

		public string BusinessAddressStreet
		{
			get
			{
				base.CheckDisposed();
				return null;
			}
		}

		public string BusinessAddressCity
		{
			get
			{
				base.CheckDisposed();
				return null;
			}
		}

		public string BusinessAddressState
		{
			get
			{
				base.CheckDisposed();
				return null;
			}
		}

		public string BusinessAddressPostalCode
		{
			get
			{
				base.CheckDisposed();
				return null;
			}
		}

		public string BusinessAddressCountry
		{
			get
			{
				base.CheckDisposed();
				return null;
			}
		}

		public string HomeAddressStreet
		{
			get
			{
				base.CheckDisposed();
				return null;
			}
		}

		public string HomeAddressCity
		{
			get
			{
				base.CheckDisposed();
				return null;
			}
		}

		public string HomeAddressState
		{
			get
			{
				base.CheckDisposed();
				return null;
			}
		}

		public string HomeAddressPostalCode
		{
			get
			{
				base.CheckDisposed();
				return null;
			}
		}

		public string HomeAddressCountry
		{
			get
			{
				base.CheckDisposed();
				return null;
			}
		}

		public string IMAddress
		{
			get
			{
				base.CheckDisposed();
				return this.imAddress;
			}
		}

		public ExDateTime? LastModifiedTime
		{
			get
			{
				base.CheckDisposed();
				return this.lastModifiedTime;
			}
		}

		public ExDateTime? BirthDate
		{
			get
			{
				base.CheckDisposed();
				return this.birthDate;
			}
		}

		public ExDateTime? BirthDateLocal
		{
			get
			{
				base.CheckDisposed();
				return this.birthDate;
			}
		}

		public string DisplayName
		{
			get
			{
				base.CheckDisposed();
				return null;
			}
		}

		public string Location
		{
			get
			{
				base.CheckDisposed();
				return null;
			}
		}

		public byte[] OscContactSources
		{
			get
			{
				base.CheckDisposed();
				return this.oscContactSources;
			}
		}

		public string PartnerNetworkId
		{
			get
			{
				base.CheckDisposed();
				return "LinkedIn";
			}
		}

		public string PartnerNetworkUserId
		{
			get
			{
				base.CheckDisposed();
				return this.contact.Id;
			}
		}

		public string PartnerNetworkThumbnailPhotoUrl
		{
			get
			{
				base.CheckDisposed();
				return this.photoUrl;
			}
		}

		public string PartnerNetworkProfilePhotoUrl
		{
			get
			{
				base.CheckDisposed();
				return this.photoUrl;
			}
		}

		public string PartnerNetworkContactType
		{
			get
			{
				base.CheckDisposed();
				return null;
			}
		}

		public ExDateTime? PeopleConnectionCreationTime
		{
			get
			{
				base.CheckDisposed();
				return this.peopleConnectionCreationTime;
			}
		}

		public string ProtectedEmailAddress
		{
			get
			{
				base.CheckDisposed();
				return null;
			}
		}

		public string ProtectedPhoneNumber
		{
			get
			{
				base.CheckDisposed();
				return null;
			}
		}

		public string Schools
		{
			get
			{
				base.CheckDisposed();
				return null;
			}
		}

		protected override void InternalDispose(bool disposing)
		{
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<LinkedInContact>(this);
		}

		private void SetJobTitle()
		{
			if (this.contact.ThreeCurrentPositions != null && this.contact.ThreeCurrentPositions.Positions != null)
			{
				this.jobTitle = this.contact.ThreeCurrentPositions.Positions[0].Title;
				return;
			}
			this.jobTitle = this.contact.Headline;
		}

		private void SetPhoneNumbers()
		{
			if (this.contact.PhoneNumbers != null && this.contact.PhoneNumbers.Numbers != null)
			{
				foreach (LinkedInPhoneNumber linkedInPhoneNumber in this.contact.PhoneNumbers.Numbers)
				{
					if (StringComparer.InvariantCultureIgnoreCase.Equals(linkedInPhoneNumber.Type, "mobile"))
					{
						this.mobilePhoneNumber = linkedInPhoneNumber.Number;
					}
					else if (StringComparer.InvariantCultureIgnoreCase.Equals(linkedInPhoneNumber.Type, "work"))
					{
						this.businessPhoneNumber = linkedInPhoneNumber.Number;
					}
					else if (StringComparer.InvariantCultureIgnoreCase.Equals(linkedInPhoneNumber.Type, "home"))
					{
						this.homePhoneNumber = linkedInPhoneNumber.Number;
					}
				}
			}
		}

		private void SetImAddress()
		{
			if (this.contact.IMAccounts != null && this.contact.IMAccounts.Accounts != null && this.contact.IMAccounts.Accounts.Count > 0 && this.contact.IMAccounts.Accounts[0] != null)
			{
				this.imAddress = this.contact.IMAccounts.Accounts[0].IMAccountType + ":" + this.contact.IMAccounts.Accounts[0].IMAccountName;
			}
		}

		private void SetBirthDate()
		{
			if (this.contact.Birthdate != null && this.contact.Birthdate.Month > 0 && this.contact.Birthdate.Day > 0)
			{
				int year = (this.contact.Birthdate.Year > 0) ? this.contact.Birthdate.Year : 1604;
				try
				{
					this.birthDate = new ExDateTime?(new ExDateTime(ExTimeZone.UtcTimeZone, year, this.contact.Birthdate.Month, this.contact.Birthdate.Day));
				}
				catch (ArgumentOutOfRangeException)
				{
				}
			}
		}

		private void InitializeOscContactSources()
		{
			this.oscContactSources = OscContactSourcesForContactWriter.Instance.Write(OscProviderGuids.LinkedIn, "LinkedIn", this.contact.Id);
		}

		private void SetPhotoUrl()
		{
			if (this.contact.PictureUrls != null && this.contact.PictureUrls.Urls != null && this.contact.PictureUrls.Urls.Count > 0)
			{
				this.photoUrl = this.contact.PictureUrls.Urls[0];
			}
		}

		private const string LinkedInPartnerNetworkId = "LinkedIn";

		private const string HomePhoneNumberType = "home";

		private const string MobilePhoneNumberType = "mobile";

		private const string WorkPhoneNumberType = "work";

		private readonly LinkedInPerson contact;

		private readonly ExDateTime? lastModifiedTime;

		private string jobTitle;

		private string businessPhoneNumber;

		private string mobilePhoneNumber;

		private string homePhoneNumber;

		private string imAddress;

		private ExDateTime? birthDate;

		private byte[] oscContactSources;

		private readonly ExDateTime? peopleConnectionCreationTime;

		private string photoUrl;
	}
}
