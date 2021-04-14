using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Transport.Sync.Common.Properties.XSO;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation.Schema
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ContactPropertyManager : XSOPropertyManager
	{
		private ContactPropertyManager()
		{
			this.birthDate = new XSOProperty<ExDateTime?>(this, ContactSchema.Birthday);
			this.birthDateLocal = new XSOProperty<ExDateTime?>(this, ContactSchema.BirthdayLocal);
			this.businessAddressCity = new XSOProperty<string>(this, ContactSchema.WorkAddressCity);
			this.businessAddressCountry = new XSOProperty<string>(this, ContactSchema.WorkAddressCountry);
			this.businessAddressPostalCode = new XSOProperty<string>(this, ContactSchema.WorkAddressPostalCode);
			this.businessAddressState = new XSOProperty<string>(this, ContactSchema.WorkAddressState);
			this.businessAddressStreet = new XSOProperty<string>(this, ContactSchema.WorkAddressStreet);
			this.businessFaxNumber = new XSOProperty<string>(this, ContactSchema.WorkFax);
			this.businessTelephoneNumber = new XSOProperty<string>(this, ContactSchema.BusinessPhoneNumber);
			this.companyName = new XSOProperty<string>(this, ContactSchema.CompanyName);
			this.displayName = new XSOProperty<string>(this, ContactSchema.FullName);
			this.email1Address = new EmailProperty(this, ContactSchema.Email1, ContactSchema.Email1EmailAddress);
			this.email2Address = new EmailProperty(this, ContactSchema.Email2, ContactSchema.Email2EmailAddress);
			this.email3Address = new EmailProperty(this, ContactSchema.Email3, ContactSchema.Email3EmailAddress);
			this.fileAs = new FileAsProperty(this);
			this.firstName = new XSOProperty<string>(this, ContactSchema.GivenName);
			this.hobbies = new XSOProperty<string>(this, ContactSchema.Hobbies);
			this.homeAddressCity = new XSOProperty<string>(this, ContactSchema.HomeCity);
			this.homeAddressCountry = new XSOProperty<string>(this, ContactSchema.HomeCountry);
			this.homeAddressPostalCode = new XSOProperty<string>(this, ContactSchema.HomePostalCode);
			this.homeAddressState = new XSOProperty<string>(this, ContactSchema.HomeState);
			this.homeAddressStreet = new XSOProperty<string>(this, ContactSchema.HomeStreet);
			this.homeTelephoneNumber = new XSOProperty<string>(this, ContactSchema.HomePhone);
			this.iMAddress = new XSOProperty<string>(this, ContactSchema.IMAddress);
			this.jobTile = new XSOProperty<string>(this, ContactSchema.Title);
			this.lastModifiedTime = new XSOProperty<ExDateTime?>(this, StoreObjectSchema.LastModifiedTime);
			this.lastName = new XSOProperty<string>(this, ContactSchema.Surname);
			this.location = new XSOProperty<string>(this, ContactSchema.Location);
			this.middleName = new XSOProperty<string>(this, ContactSchema.MiddleName);
			this.mobileTelephoneNumber = new XSOProperty<string>(this, ContactSchema.MobilePhone);
			this.oscContactSources = new XSOProperty<byte[]>(this, ContactSchema.OscContactSourcesForContact);
			this.otherTelephoneNumber = new XSOProperty<string>(this, ContactSchema.OtherTelephone);
			this.partnerNetworkContactType = new XSOProperty<string>(this, ContactSchema.PartnerNetworkContactType);
			this.partnerNetworkId = new XSOProperty<string>(this, ContactSchema.PartnerNetworkId);
			this.partnerNetworkProfilePhotoUrl = new XSOProperty<string>(this, ContactSchema.PartnerNetworkProfilePhotoUrl);
			this.partnerNetworkThumbnailPhotoUrl = new XSOProperty<string>(this, ContactSchema.PartnerNetworkThumbnailPhotoUrl);
			this.partnerNetworkUserId = new XSOProperty<string>(this, ContactSchema.PartnerNetworkUserId);
			this.peopleConnectionCreationTime = new XSOProperty<ExDateTime?>(this, ContactSchema.PeopleConnectionCreationTime);
			this.protectedEmailAddress = new XSOProperty<string>(this, ContactProtectedPropertiesSchema.ProtectedEmailAddress);
			this.protectedPhoneNumber = new XSOProperty<string>(this, ContactProtectedPropertiesSchema.ProtectedPhoneNumber);
			this.schools = new XSOProperty<string>(this, ContactSchema.Schools);
			this.webPage = new XSOProperty<string>(this, ContactSchema.BusinessHomePage);
		}

		internal static ContactPropertyManager Instance
		{
			get
			{
				return ContactPropertyManager.instance;
			}
		}

		internal XSOProperty<ExDateTime?> BirthDate
		{
			get
			{
				return this.birthDate;
			}
		}

		internal XSOProperty<ExDateTime?> BirthDateLocal
		{
			get
			{
				return this.birthDateLocal;
			}
		}

		internal XSOProperty<string> BusinessAddressCity
		{
			get
			{
				return this.businessAddressCity;
			}
		}

		internal XSOProperty<string> BusinessAddressCountry
		{
			get
			{
				return this.businessAddressCountry;
			}
		}

		internal XSOProperty<string> BusinessAddressPostalCode
		{
			get
			{
				return this.businessAddressPostalCode;
			}
		}

		internal XSOProperty<string> BusinessAddressState
		{
			get
			{
				return this.businessAddressState;
			}
		}

		internal XSOProperty<string> BusinessAddressStreet
		{
			get
			{
				return this.businessAddressStreet;
			}
		}

		internal XSOProperty<string> BusinessFaxNumber
		{
			get
			{
				return this.businessFaxNumber;
			}
		}

		internal XSOProperty<string> BusinessTelephoneNumber
		{
			get
			{
				return this.businessTelephoneNumber;
			}
		}

		internal XSOProperty<string> CompanyName
		{
			get
			{
				return this.companyName;
			}
		}

		internal XSOProperty<string> DisplayName
		{
			get
			{
				return this.displayName;
			}
		}

		internal EmailProperty Email1Address
		{
			get
			{
				return this.email1Address;
			}
		}

		internal EmailProperty Email2Address
		{
			get
			{
				return this.email2Address;
			}
		}

		internal EmailProperty Email3Address
		{
			get
			{
				return this.email3Address;
			}
		}

		internal FileAsProperty FileAs
		{
			get
			{
				return this.fileAs;
			}
		}

		internal XSOProperty<string> FirstName
		{
			get
			{
				return this.firstName;
			}
		}

		internal XSOProperty<string> Hobbies
		{
			get
			{
				return this.hobbies;
			}
		}

		internal XSOProperty<string> HomeAddressCity
		{
			get
			{
				return this.homeAddressCity;
			}
		}

		internal XSOProperty<string> HomeAddressCountry
		{
			get
			{
				return this.homeAddressCountry;
			}
		}

		internal XSOProperty<string> HomeAddressPostalCode
		{
			get
			{
				return this.homeAddressPostalCode;
			}
		}

		internal XSOProperty<string> HomeAddressState
		{
			get
			{
				return this.homeAddressState;
			}
		}

		internal XSOProperty<string> HomeAddressStreet
		{
			get
			{
				return this.homeAddressStreet;
			}
		}

		internal XSOProperty<string> HomeTelephoneNumber
		{
			get
			{
				return this.homeTelephoneNumber;
			}
		}

		internal XSOProperty<string> IMAddress
		{
			get
			{
				return this.iMAddress;
			}
		}

		internal XSOProperty<string> JobTile
		{
			get
			{
				return this.jobTile;
			}
		}

		internal XSOProperty<string> LastName
		{
			get
			{
				return this.lastName;
			}
		}

		internal XSOProperty<ExDateTime?> LastModifiedTime
		{
			get
			{
				return this.lastModifiedTime;
			}
		}

		internal XSOProperty<string> Location
		{
			get
			{
				return this.location;
			}
		}

		internal XSOProperty<string> MiddleName
		{
			get
			{
				return this.middleName;
			}
		}

		internal XSOProperty<string> MobileTelephoneNumber
		{
			get
			{
				return this.mobileTelephoneNumber;
			}
		}

		internal XSOProperty<string> OtherTelephoneNumber
		{
			get
			{
				return this.otherTelephoneNumber;
			}
		}

		internal XSOProperty<byte[]> OscContactSources
		{
			get
			{
				return this.oscContactSources;
			}
		}

		internal XSOProperty<string> PartnerNetworkContactType
		{
			get
			{
				return this.partnerNetworkContactType;
			}
		}

		internal XSOProperty<string> PartnerNetworkId
		{
			get
			{
				return this.partnerNetworkId;
			}
		}

		internal XSOProperty<string> PartnerNetworkProfilePhotoUrl
		{
			get
			{
				return this.partnerNetworkProfilePhotoUrl;
			}
		}

		internal XSOProperty<string> PartnerNetworkThumbnailPhotoUrl
		{
			get
			{
				return this.partnerNetworkThumbnailPhotoUrl;
			}
		}

		internal XSOProperty<string> PartnerNetworkUserId
		{
			get
			{
				return this.partnerNetworkUserId;
			}
		}

		internal XSOProperty<ExDateTime?> PeopleConnectionCreationTime
		{
			get
			{
				return this.peopleConnectionCreationTime;
			}
		}

		internal XSOProperty<string> ProtectedEmailAddress
		{
			get
			{
				return this.protectedEmailAddress;
			}
		}

		internal XSOProperty<string> ProtectedPhoneNumber
		{
			get
			{
				return this.protectedPhoneNumber;
			}
		}

		internal XSOProperty<string> Schools
		{
			get
			{
				return this.schools;
			}
		}

		internal XSOProperty<string> WebPage
		{
			get
			{
				return this.webPage;
			}
		}

		private static readonly ContactPropertyManager instance = new ContactPropertyManager();

		private readonly XSOProperty<ExDateTime?> birthDate;

		private readonly XSOProperty<ExDateTime?> birthDateLocal;

		private readonly XSOProperty<string> businessAddressCity;

		private readonly XSOProperty<string> businessAddressCountry;

		private readonly XSOProperty<string> businessAddressPostalCode;

		private readonly XSOProperty<string> businessAddressState;

		private readonly XSOProperty<string> businessAddressStreet;

		private readonly XSOProperty<string> businessFaxNumber;

		private readonly XSOProperty<string> businessTelephoneNumber;

		private readonly XSOProperty<string> companyName;

		private readonly XSOProperty<string> displayName;

		private readonly EmailProperty email1Address;

		private readonly EmailProperty email2Address;

		private readonly EmailProperty email3Address;

		private readonly FileAsProperty fileAs;

		private readonly XSOProperty<string> firstName;

		private readonly XSOProperty<string> homeAddressCity;

		private readonly XSOProperty<string> hobbies;

		private readonly XSOProperty<string> homeAddressCountry;

		private readonly XSOProperty<string> homeAddressPostalCode;

		private readonly XSOProperty<string> homeAddressState;

		private readonly XSOProperty<string> homeAddressStreet;

		private readonly XSOProperty<string> homeTelephoneNumber;

		private readonly XSOProperty<string> iMAddress;

		private readonly XSOProperty<string> jobTile;

		private readonly XSOProperty<string> lastName;

		private readonly XSOProperty<ExDateTime?> lastModifiedTime;

		private readonly XSOProperty<string> location;

		private readonly XSOProperty<string> middleName;

		private readonly XSOProperty<string> mobileTelephoneNumber;

		private readonly XSOProperty<byte[]> oscContactSources;

		private readonly XSOProperty<string> otherTelephoneNumber;

		private readonly XSOProperty<string> partnerNetworkContactType;

		private readonly XSOProperty<string> partnerNetworkId;

		private readonly XSOProperty<string> partnerNetworkProfilePhotoUrl;

		private readonly XSOProperty<string> partnerNetworkThumbnailPhotoUrl;

		private readonly XSOProperty<string> partnerNetworkUserId;

		private readonly XSOProperty<ExDateTime?> peopleConnectionCreationTime;

		private readonly XSOProperty<string> protectedEmailAddress;

		private readonly XSOProperty<string> protectedPhoneNumber;

		private readonly XSOProperty<string> schools;

		private readonly XSOProperty<string> webPage;
	}
}
