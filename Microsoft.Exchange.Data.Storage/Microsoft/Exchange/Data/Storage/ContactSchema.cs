using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ContactSchema : ContactBaseSchema
	{
		public new static ContactSchema Instance
		{
			get
			{
				if (ContactSchema.instance == null)
				{
					ContactSchema.instance = new ContactSchema();
				}
				return ContactSchema.instance;
			}
		}

		protected override ICollection<PropertyRule> PropertyRules
		{
			get
			{
				if (this.propertyRulesCache == null)
				{
					this.propertyRulesCache = base.PropertyRules.Concat(ContactSchema.ContactSchemaPropertyRules);
				}
				return this.propertyRulesCache;
			}
		}

		internal override void CoreObjectUpdate(CoreItem coreItem, CoreItemOperation operation)
		{
			Contact.CoreObjectUpdateFileAs(coreItem);
			base.CoreObjectUpdate(coreItem, operation);
			Contact.CoreObjectUpdatePhysicalAddresses(coreItem);
		}

		// Note: this type is marked as 'beforefieldinit'.
		static ContactSchema()
		{
			PropertyRule[] array = new PropertyRule[4];
			array[0] = PropertyRuleLibrary.PersonIdRule;
			array[1] = PropertyRuleLibrary.EnhancedLocation;
			array[2] = new SequenceCompositePropertyRule(string.Empty, delegate(ILocationIdentifierSetter lidSetter)
			{
				lidSetter.SetLocationIdentifier(34080U, LastChangeAction.SequenceCompositePropertyRuleApplied);
			}, new PropertyRule[]
			{
				PropertyRuleLibrary.EmailAddressUpdateRule,
				PropertyRuleLibrary.ContactDisplayNameRule
			});
			array[3] = PropertyRuleLibrary.OscContactSourcesForContactRule;
			ContactSchema.ContactSchemaPropertyRules = array;
			ContactSchema.instance = null;
		}

		[LegalTracking]
		[DetectCodepage]
		public static readonly StorePropertyDefinition AssistantName = InternalSchema.AssistantName;

		[LegalTracking]
		public static readonly StorePropertyDefinition AssistantPhoneNumber = InternalSchema.AssistantPhoneNumber;

		[LegalTracking]
		[DetectCodepage]
		public static readonly StorePropertyDefinition BillingInformation = InternalSchema.BillingInformation;

		[LegalTracking]
		public static readonly StorePropertyDefinition Birthday = InternalSchema.Birthday;

		[LegalTracking]
		public static readonly StorePropertyDefinition BirthdayLocal = InternalSchema.BirthdayLocal;

		[LegalTracking]
		public static readonly StorePropertyDefinition NotInBirthdayCalendar = InternalSchema.NotInBirthdayCalendar;

		[Autoload]
		[LegalTracking]
		public static readonly StorePropertyDefinition BusinessHomePage = InternalSchema.BusinessHomePage;

		[LegalTracking]
		public static readonly StorePropertyDefinition LegacyWebPage = InternalSchema.LegacyWebPage;

		[LegalTracking]
		[Autoload]
		public static readonly StorePropertyDefinition BusinessPhoneNumber = InternalSchema.BusinessPhoneNumber;

		[LegalTracking]
		public static readonly StorePropertyDefinition BusinessPhoneNumber2 = InternalSchema.BusinessPhoneNumber2;

		[LegalTracking]
		public static readonly StorePropertyDefinition CallbackPhone = InternalSchema.CallbackPhone;

		[LegalTracking]
		public static readonly StorePropertyDefinition CarPhone = InternalSchema.CarPhone;

		[DetectCodepage]
		[LegalTracking]
		public static readonly StorePropertyDefinition Children = InternalSchema.Children;

		[LegalTracking]
		public static readonly StorePropertyDefinition Companies = InternalSchema.Companies;

		[LegalTracking]
		[DetectCodepage]
		public static readonly StorePropertyDefinition CompanyName = InternalSchema.CompanyName;

		[DetectCodepage]
		[LegalTracking]
		public static readonly StorePropertyDefinition Department = InternalSchema.Department;

		[LegalTracking]
		[DetectCodepage]
		public static readonly StorePropertyDefinition DisplayNamePrefix = InternalSchema.DisplayNamePrefix;

		[Autoload]
		[LegalTracking]
		public static readonly StorePropertyDefinition Email1 = InternalSchema.ContactEmail1;

		[LegalTracking]
		[Autoload]
		public static readonly StorePropertyDefinition Email2 = InternalSchema.ContactEmail2;

		[Autoload]
		[LegalTracking]
		public static readonly StorePropertyDefinition Email3 = InternalSchema.ContactEmail3;

		[LegalTracking]
		[Autoload]
		public static readonly StorePropertyDefinition Email1AddrType = InternalSchema.Email1AddrType;

		[LegalTracking]
		[DetectCodepage]
		public static readonly StorePropertyDefinition Email1DisplayName = InternalSchema.Email1DisplayName;

		[Autoload]
		[LegalTracking]
		public static readonly StorePropertyDefinition Email1EmailAddress = InternalSchema.Email1EmailAddress;

		[DetectCodepage]
		[LegalTracking]
		public static readonly StorePropertyDefinition Email1OriginalDisplayName = InternalSchema.Email1OriginalDisplayName;

		[Autoload]
		[LegalTracking]
		public static readonly StorePropertyDefinition Email1OriginalEntryID = InternalSchema.Email1OriginalEntryID;

		[LegalTracking]
		[Autoload]
		public static readonly StorePropertyDefinition Email2AddrType = InternalSchema.Email2AddrType;

		[LegalTracking]
		[DetectCodepage]
		public static readonly StorePropertyDefinition Email2DisplayName = InternalSchema.Email2DisplayName;

		[LegalTracking]
		[Autoload]
		public static readonly StorePropertyDefinition Email2EmailAddress = InternalSchema.Email2EmailAddress;

		[LegalTracking]
		[DetectCodepage]
		public static readonly StorePropertyDefinition Email2OriginalDisplayName = InternalSchema.Email2OriginalDisplayName;

		[LegalTracking]
		[Autoload]
		public static readonly StorePropertyDefinition Email2OriginalEntryID = InternalSchema.Email2OriginalEntryID;

		[LegalTracking]
		[Autoload]
		public static readonly StorePropertyDefinition Email3AddrType = InternalSchema.Email3AddrType;

		[LegalTracking]
		[DetectCodepage]
		public static readonly StorePropertyDefinition Email3DisplayName = InternalSchema.Email3DisplayName;

		[LegalTracking]
		[Autoload]
		public static readonly StorePropertyDefinition Email3EmailAddress = InternalSchema.Email3EmailAddress;

		[LegalTracking]
		[DetectCodepage]
		public static readonly StorePropertyDefinition Email3OriginalDisplayName = InternalSchema.Email3OriginalDisplayName;

		[Autoload]
		[LegalTracking]
		public static readonly StorePropertyDefinition Email3OriginalEntryID = InternalSchema.Email3OriginalEntryID;

		[Autoload]
		[LegalTracking]
		public static readonly StorePropertyDefinition ContactBusinessFax = InternalSchema.ContactBusinessFax;

		[LegalTracking]
		[Autoload]
		public static readonly StorePropertyDefinition ContactHomeFax = InternalSchema.ContactHomeFax;

		[LegalTracking]
		[Autoload]
		public static readonly StorePropertyDefinition ContactOtherFax = InternalSchema.ContactOtherFax;

		[LegalTracking]
		[Autoload]
		public static readonly StorePropertyDefinition FileAsId = InternalSchema.FileAsId;

		[DetectCodepage]
		[LegalTracking]
		public static readonly StorePropertyDefinition GALContactType = InternalSchema.GALContactType;

		[DetectCodepage]
		public static readonly StorePropertyDefinition GALUMDialplanId = InternalSchema.GALUMDialplanId;

		[LegalTracking]
		[DetectCodepage]
		public static readonly StorePropertyDefinition Generation = InternalSchema.Generation;

		[LegalTracking]
		[DetectCodepage]
		public static readonly StorePropertyDefinition GivenName = InternalSchema.GivenName;

		[DetectCodepage]
		[LegalTracking]
		public static readonly StorePropertyDefinition HasPicture = InternalSchema.HasPicture;

		[DetectCodepage]
		[LegalTracking]
		public static readonly StorePropertyDefinition HomePostOfficeBox = InternalSchema.HomePostOfficeBox;

		[DetectCodepage]
		[LegalTracking]
		public static readonly StorePropertyDefinition HomeCity = InternalSchema.HomeCity;

		[LegalTracking]
		[DetectCodepage]
		public static readonly StorePropertyDefinition HomeCountry = InternalSchema.HomeCountry;

		[DetectCodepage]
		[LegalTracking]
		public static readonly StorePropertyDefinition HomeAddressInternal = InternalSchema.HomeAddressInternal;

		[LegalTracking]
		[DetectCodepage]
		public static readonly StorePropertyDefinition HomeLatitude = InternalSchema.HomeLatitude;

		[DetectCodepage]
		[LegalTracking]
		public static readonly StorePropertyDefinition HomeLongitude = InternalSchema.HomeLongitude;

		[DetectCodepage]
		[LegalTracking]
		public static readonly StorePropertyDefinition HomeAltitude = InternalSchema.HomeAltitude;

		[LegalTracking]
		[DetectCodepage]
		public static readonly StorePropertyDefinition HomeAccuracy = InternalSchema.HomeAccuracy;

		[DetectCodepage]
		[LegalTracking]
		public static readonly StorePropertyDefinition HomeAltitudeAccuracy = InternalSchema.HomeAltitudeAccuracy;

		[LegalTracking]
		[DetectCodepage]
		public static readonly StorePropertyDefinition HomeLocationUri = InternalSchema.HomeLocationUri;

		[LegalTracking]
		[DetectCodepage]
		public static readonly StorePropertyDefinition HomeLocationSource = InternalSchema.HomeLocationSource;

		[Autoload]
		[LegalTracking]
		public static readonly StorePropertyDefinition HomeFax = InternalSchema.HomeFax;

		[Autoload]
		[LegalTracking]
		public static readonly StorePropertyDefinition SelectedPreferredPhoneNumber = InternalSchema.SelectedPreferredPhoneNumber;

		[Autoload]
		[LegalTracking]
		public static readonly StorePropertyDefinition HomePhone = InternalSchema.HomePhone;

		[LegalTracking]
		public static readonly StorePropertyDefinition HomePhone2 = InternalSchema.HomePhone2;

		[LegalTracking]
		[DetectCodepage]
		public static readonly StorePropertyDefinition HomePostalCode = InternalSchema.HomePostalCode;

		[LegalTracking]
		[DetectCodepage]
		public static readonly StorePropertyDefinition HomeState = InternalSchema.HomeState;

		[DetectCodepage]
		[LegalTracking]
		public static readonly StorePropertyDefinition HomeStreet = InternalSchema.HomeStreet;

		[Autoload]
		[LegalTracking]
		public static readonly StorePropertyDefinition IMAddress = InternalSchema.IMAddress;

		[LegalTracking]
		[Autoload]
		public static readonly StorePropertyDefinition IMAddress2 = InternalSchema.IMAddress2;

		[Autoload]
		[LegalTracking]
		public static readonly StorePropertyDefinition IMAddress3 = InternalSchema.IMAddress3;

		[LegalTracking]
		public static readonly StorePropertyDefinition MMS = InternalSchema.MMS;

		[DetectCodepage]
		[LegalTracking]
		public static readonly StorePropertyDefinition Initials = InternalSchema.Initials;

		[LegalTracking]
		public static readonly StorePropertyDefinition InternationalIsdnNumber = InternalSchema.InternationalIsdnNumber;

		[LegalTracking]
		[DetectCodepage]
		public static readonly StorePropertyDefinition IsFavorite = InternalSchema.IsFavorite;

		[DetectCodepage]
		[LegalTracking]
		public static readonly StorePropertyDefinition IsPromotedContact = InternalSchema.IsPromotedContact;

		[Autoload]
		public static readonly StorePropertyDefinition Linked = InternalSchema.Linked;

		[LegalTracking]
		[DetectCodepage]
		public static readonly StorePropertyDefinition Manager = InternalSchema.Manager;

		[LegalTracking]
		[Autoload]
		[DetectCodepage]
		public static readonly StorePropertyDefinition MiddleName = InternalSchema.MiddleName;

		[LegalTracking]
		public static readonly StorePropertyDefinition Mileage = InternalSchema.Mileage;

		[Autoload]
		[LegalTracking]
		public static readonly StorePropertyDefinition MobilePhone = InternalSchema.MobilePhone;

		[DetectCodepage]
		[LegalTracking]
		public static readonly StorePropertyDefinition Nickname = InternalSchema.Nickname;

		[LegalTracking]
		[DetectCodepage]
		public static readonly StorePropertyDefinition OfficeLocation = InternalSchema.OfficeLocation;

		public static readonly StorePropertyDefinition PersonType = new PersonTypeProperty();

		[DetectCodepage]
		[LegalTracking]
		public static readonly StorePropertyDefinition Location = InternalSchema.Location;

		[LegalTracking]
		public static readonly StorePropertyDefinition OrganizationMainPhone = InternalSchema.OrganizationMainPhone;

		[LegalTracking]
		[DetectCodepage]
		public static readonly StorePropertyDefinition OtherPostOfficeBox = InternalSchema.OtherPostOfficeBox;

		[DetectCodepage]
		[LegalTracking]
		public static readonly StorePropertyDefinition OtherCity = InternalSchema.OtherCity;

		[LegalTracking]
		[DetectCodepage]
		public static readonly StorePropertyDefinition OtherCountry = InternalSchema.OtherCountry;

		[LegalTracking]
		[DetectCodepage]
		public static readonly StorePropertyDefinition OtherLatitude = InternalSchema.OtherLatitude;

		[DetectCodepage]
		[LegalTracking]
		public static readonly StorePropertyDefinition OtherLongitude = InternalSchema.OtherLongitude;

		[DetectCodepage]
		[LegalTracking]
		public static readonly StorePropertyDefinition OtherAltitude = InternalSchema.OtherAltitude;

		[LegalTracking]
		[DetectCodepage]
		public static readonly StorePropertyDefinition OtherAccuracy = InternalSchema.OtherAccuracy;

		[DetectCodepage]
		[LegalTracking]
		public static readonly StorePropertyDefinition OtherAltitudeAccuracy = InternalSchema.OtherAltitudeAccuracy;

		[LegalTracking]
		[DetectCodepage]
		public static readonly StorePropertyDefinition OtherLocationUri = InternalSchema.OtherLocationUri;

		[DetectCodepage]
		[LegalTracking]
		public static readonly StorePropertyDefinition OtherLocationSource = InternalSchema.OtherLocationSource;

		[LegalTracking]
		[Autoload]
		public static readonly StorePropertyDefinition OtherFax = InternalSchema.OtherFax;

		[LegalTracking]
		public static readonly StorePropertyDefinition OtherMobile = InternalSchema.CarPhone;

		[DetectCodepage]
		[LegalTracking]
		public static readonly StorePropertyDefinition OtherPostalCode = InternalSchema.OtherPostalCode;

		[DetectCodepage]
		[LegalTracking]
		public static readonly StorePropertyDefinition OtherState = InternalSchema.OtherState;

		[DetectCodepage]
		[LegalTracking]
		public static readonly StorePropertyDefinition OtherStreet = InternalSchema.OtherStreet;

		[LegalTracking]
		[Autoload]
		public static readonly StorePropertyDefinition OtherTelephone = InternalSchema.OtherTelephone;

		[LegalTracking]
		public static readonly StorePropertyDefinition Pager = InternalSchema.Pager;

		[LegalTracking]
		public static readonly StorePropertyDefinition PartnerNetworkId = InternalSchema.PartnerNetworkId;

		[LegalTracking]
		public static readonly StorePropertyDefinition PartnerNetworkUserId = InternalSchema.PartnerNetworkUserId;

		[LegalTracking]
		public static readonly StorePropertyDefinition PartnerNetworkThumbnailPhotoUrl = InternalSchema.PartnerNetworkThumbnailPhotoUrl;

		[LegalTracking]
		public static readonly StorePropertyDefinition PartnerNetworkProfilePhotoUrl = InternalSchema.PartnerNetworkProfilePhotoUrl;

		public static readonly StorePropertyDefinition PartnerNetworkContactType = InternalSchema.PartnerNetworkContactType;

		[LegalTracking]
		public static readonly StorePropertyDefinition PostalAddressId = InternalSchema.PostalAddressId;

		[DetectCodepage]
		[LegalTracking]
		public static readonly StorePropertyDefinition Profession = InternalSchema.Profession;

		[LegalTracking]
		public static readonly StorePropertyDefinition RadioPhone = InternalSchema.RadioPhone;

		[DetectCodepage]
		[LegalTracking]
		public static readonly StorePropertyDefinition RelevanceScore = InternalSchema.RelevanceScore;

		[LegalTracking]
		[DetectCodepage]
		public static readonly StorePropertyDefinition Schools = InternalSchema.Schools;

		[DetectCodepage]
		[LegalTracking]
		public static readonly StorePropertyDefinition SpouseName = InternalSchema.SpouseName;

		[DetectCodepage]
		[LegalTracking]
		public static readonly StorePropertyDefinition Surname = InternalSchema.Surname;

		public static readonly StorePropertyDefinition TelUri = InternalSchema.TelUri;

		public static readonly StorePropertyDefinition ImContactSipUriAddress = InternalSchema.ImContactSipUriAddress;

		[DetectCodepage]
		[LegalTracking]
		public static readonly StorePropertyDefinition Title = InternalSchema.Title;

		[LegalTracking]
		[DetectCodepage]
		public static readonly StorePropertyDefinition FullName = InternalSchema.DisplayName;

		public static readonly StorePropertyDefinition WeddingAnniversary = InternalSchema.WeddingAnniversary;

		public static readonly StorePropertyDefinition WeddingAnniversaryLocal = InternalSchema.WeddingAnniversaryLocal;

		[DetectCodepage]
		[LegalTracking]
		public static readonly StorePropertyDefinition WorkPostOfficeBox = InternalSchema.WorkPostOfficeBox;

		[LegalTracking]
		[DetectCodepage]
		public static readonly StorePropertyDefinition WorkAddressCity = InternalSchema.WorkAddressCity;

		[LegalTracking]
		[DetectCodepage]
		public static readonly StorePropertyDefinition WorkAddressCountry = InternalSchema.WorkAddressCountry;

		[DetectCodepage]
		[LegalTracking]
		public static readonly StorePropertyDefinition WorkLatitude = InternalSchema.WorkLatitude;

		[DetectCodepage]
		[LegalTracking]
		public static readonly StorePropertyDefinition WorkLongitude = InternalSchema.WorkLongitude;

		[DetectCodepage]
		[LegalTracking]
		public static readonly StorePropertyDefinition WorkAltitude = InternalSchema.WorkAltitude;

		[LegalTracking]
		[DetectCodepage]
		public static readonly StorePropertyDefinition WorkAccuracy = InternalSchema.WorkAccuracy;

		[LegalTracking]
		[DetectCodepage]
		public static readonly StorePropertyDefinition WorkAltitudeAccuracy = InternalSchema.WorkAltitudeAccuracy;

		[LegalTracking]
		[DetectCodepage]
		public static readonly StorePropertyDefinition WorkLocationUri = InternalSchema.WorkLocationUri;

		[DetectCodepage]
		[LegalTracking]
		public static readonly StorePropertyDefinition WorkLocationSource = InternalSchema.WorkLocationSource;

		[DetectCodepage]
		[LegalTracking]
		public static readonly StorePropertyDefinition WorkAddressPostalCode = InternalSchema.WorkAddressPostalCode;

		[LegalTracking]
		[DetectCodepage]
		public static readonly StorePropertyDefinition WorkAddressState = InternalSchema.WorkAddressState;

		[DetectCodepage]
		[LegalTracking]
		public static readonly StorePropertyDefinition WorkAddressStreet = InternalSchema.WorkAddressStreet;

		[Autoload]
		[LegalTracking]
		public static readonly StorePropertyDefinition WorkFax = InternalSchema.FaxNumber;

		[LegalTracking]
		public static readonly StorePropertyDefinition PersonalHomePage = InternalSchema.PersonalHomePage;

		[LegalTracking]
		[Autoload]
		public static readonly StorePropertyDefinition YomiFirstName = InternalSchema.YomiFirstName;

		[Autoload]
		[LegalTracking]
		public static readonly StorePropertyDefinition YomiLastName = InternalSchema.YomiLastName;

		[LegalTracking]
		[Autoload]
		public static readonly StorePropertyDefinition YomiCompany = InternalSchema.YomiCompany;

		[DetectCodepage]
		[LegalTracking]
		public static readonly StorePropertyDefinition HomeAddress = InternalSchema.HomeAddress;

		[LegalTracking]
		[DetectCodepage]
		public static readonly StorePropertyDefinition BusinessAddress = InternalSchema.BusinessAddress;

		[DetectCodepage]
		[LegalTracking]
		public static readonly StorePropertyDefinition OtherAddress = InternalSchema.OtherAddress;

		[LegalTracking]
		public static readonly StorePropertyDefinition PrimaryTelephoneNumber = InternalSchema.PrimaryTelephoneNumber;

		[LegalTracking]
		public static readonly StorePropertyDefinition TtyTddPhoneNumber = InternalSchema.TtyTddPhoneNumber;

		[LegalTracking]
		public static readonly StorePropertyDefinition TelexNumber = InternalSchema.TelexNumber;

		[LegalTracking]
		public static readonly StorePropertyDefinition CustomerId = InternalSchema.CustomerId;

		[LegalTracking]
		public static readonly StorePropertyDefinition GovernmentIdNumber = InternalSchema.GovernmentIdNumber;

		[LegalTracking]
		public static readonly StorePropertyDefinition Account = InternalSchema.Account;

		[LegalTracking]
		public static readonly StorePropertyDefinition UserX509Certificates = InternalSchema.UserX509Certificates;

		[LegalTracking]
		public static readonly StorePropertyDefinition OutlookCardDesign = InternalSchema.OutlookCardDesign;

		[LegalTracking]
		[DetectCodepage]
		public static readonly StorePropertyDefinition UserText1 = InternalSchema.UserText1;

		[DetectCodepage]
		[LegalTracking]
		public static readonly StorePropertyDefinition UserText2 = InternalSchema.UserText2;

		[DetectCodepage]
		[LegalTracking]
		public static readonly StorePropertyDefinition UserText3 = InternalSchema.UserText3;

		[LegalTracking]
		[DetectCodepage]
		public static readonly StorePropertyDefinition UserText4 = InternalSchema.UserText4;

		public static readonly StorePropertyDefinition FreeBusyUrl = InternalSchema.FreeBusyUrl;

		[LegalTracking]
		[DetectCodepage]
		public static readonly StorePropertyDefinition Hobbies = InternalSchema.Hobbies;

		[LegalTracking]
		public static readonly StorePropertyDefinition MobilePhone2 = InternalSchema.MobilePhone2;

		[LegalTracking]
		public static readonly StorePropertyDefinition OtherPhone2 = InternalSchema.OtherPhone2;

		[LegalTracking]
		public static readonly StorePropertyDefinition HomePhoneAttributes = InternalSchema.HomePhoneAttributes;

		[LegalTracking]
		public static readonly StorePropertyDefinition WorkPhoneAttributes = InternalSchema.WorkPhoneAttributes;

		[LegalTracking]
		public static readonly StorePropertyDefinition MobilePhoneAttributes = InternalSchema.MobilePhoneAttributes;

		[LegalTracking]
		public static readonly StorePropertyDefinition OtherPhoneAttributes = InternalSchema.OtherPhoneAttributes;

		[LegalTracking]
		public static readonly StorePropertyDefinition PrimarySmtpAddress = InternalSchema.PrimarySmtpAddress;

		[Autoload]
		public static readonly StorePropertyDefinition SideEffects = InternalSchema.SideEffects;

		public static readonly StorePropertyDefinition LinkRejectHistory = new LinkRejectHistoryProperty();

		public static readonly StorePropertyDefinition[] EmailAddressProperties = new StorePropertyDefinition[]
		{
			ContactSchema.Email1EmailAddress,
			ContactSchema.Email2EmailAddress,
			ContactSchema.Email3EmailAddress,
			ContactProtectedPropertiesSchema.ProtectedEmailAddress
		};

		public static readonly StorePropertyDefinition GALLinkID = InternalSchema.GALLinkID;

		public static readonly StorePropertyDefinition SmtpAddressCache = InternalSchema.SmtpAddressCache;

		public static readonly StorePropertyDefinition GALLinkState = InternalSchema.GALLinkState;

		public static readonly StorePropertyDefinition UserApprovedLink = InternalSchema.UserApprovedLink;

		public static readonly StorePropertyDefinition LinkChangeHistory = InternalSchema.LinkChangeHistory;

		public static readonly StorePropertyDefinition OscContactSourcesForContact = InternalSchema.OscContactSourcesForContact;

		public static readonly StorePropertyDefinition PeopleConnectionCreationTime = InternalSchema.PeopleConnectionCreationTime;

		public static readonly StorePropertyDefinition AddressBookEntryId = InternalSchema.AddressBookEntryId;

		public static readonly StorePropertyDefinition PersonId = InternalSchema.PersonId;

		public static readonly StorePropertyDefinition AttributionDisplayName = InternalSchema.AttributionDisplayName;

		public static readonly StorePropertyDefinition IsWritable = InternalSchema.IsContactWritable;

		private static readonly PropertyRule[] ContactSchemaPropertyRules;

		private static ContactSchema instance;

		private ICollection<PropertyRule> propertyRulesCache;
	}
}
