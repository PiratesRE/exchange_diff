using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PersonSchema : Schema
	{
		public new static PersonSchema Instance
		{
			get
			{
				return PersonSchema.instance;
			}
		}

		public static readonly StorePropertyDefinition Id = new ApplicationAggregatedProperty("Id", typeof(PersonId), PropertyFlags.None, PersonPropertyAggregationStrategy.PersonIdProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty CreationTime = new ApplicationAggregatedProperty("CreationTime", typeof(ExDateTime), PropertyFlags.None, PersonPropertyAggregationStrategy.CreationTimeProperty, SortByAndFilterStrategy.CreateSimpleSort(InternalSchema.InternalPersonCreationTime));

		public static readonly ApplicationAggregatedProperty IsFavorite = new ApplicationAggregatedProperty("IsFavorite", typeof(bool), PropertyFlags.None, PersonPropertyAggregationStrategy.IsFavoriteProperty, SortByAndFilterStrategy.CreateSimpleSort(InternalSchema.IsFavorite));

		public static readonly ApplicationAggregatedProperty DisplayName = new ApplicationAggregatedProperty("DisplayName", typeof(string), PropertyFlags.None, PersonPropertyAggregationStrategy.CreateNameProperty(InternalSchema.DisplayName), SortByAndFilterStrategy.CreateSimpleSort(InternalSchema.InternalPersonDisplayName));

		public static readonly ApplicationAggregatedProperty FileAs = new ApplicationAggregatedProperty("FileAs", typeof(string), PropertyFlags.None, PersonPropertyAggregationStrategy.CreateNameProperty(InternalSchema.FileAsString), SortByAndFilterStrategy.CreateSimpleSort(InternalSchema.InternalPersonFileAs));

		public static readonly ApplicationAggregatedProperty FileAsId = new ApplicationAggregatedProperty("FileAsId", typeof(string), PropertyFlags.None, PersonPropertyAggregationStrategy.FileAsIdProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty Nickname = new ApplicationAggregatedProperty("Nickname", typeof(string), PropertyFlags.None, PersonPropertyAggregationStrategy.CreateNameProperty(InternalSchema.Nickname), SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty DisplayNamePrefix = new ApplicationAggregatedProperty("DisplayNamePrefix", typeof(string), PropertyFlags.None, PersonPropertyAggregationStrategy.CreateNameProperty(InternalSchema.DisplayNamePrefix), SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty GivenName = new ApplicationAggregatedProperty("GivenName", typeof(string), PropertyFlags.None, PersonPropertyAggregationStrategy.CreateNameProperty(InternalSchema.GivenName), SortByAndFilterStrategy.CreateSimpleSort(InternalSchema.InternalPersonGivenName));

		public static readonly ApplicationAggregatedProperty MiddleName = new ApplicationAggregatedProperty("MiddleName", typeof(string), PropertyFlags.None, PersonPropertyAggregationStrategy.CreateNameProperty(InternalSchema.MiddleName), SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty Surname = new ApplicationAggregatedProperty("Surname", typeof(string), PropertyFlags.None, PersonPropertyAggregationStrategy.CreateNameProperty(InternalSchema.Surname), SortByAndFilterStrategy.CreateSimpleSort(InternalSchema.InternalPersonSurname));

		public static readonly ApplicationAggregatedProperty Generation = new ApplicationAggregatedProperty("Generation", typeof(string), PropertyFlags.None, PersonPropertyAggregationStrategy.CreateNameProperty(InternalSchema.Generation), SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty YomiFirstName = new ApplicationAggregatedProperty("YomiFirstName", typeof(string), PropertyFlags.None, PersonPropertyAggregationStrategy.CreateNameProperty(InternalSchema.YomiFirstName), SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty YomiLastName = new ApplicationAggregatedProperty("YomiLastName", typeof(string), PropertyFlags.None, PersonPropertyAggregationStrategy.CreateNameProperty(InternalSchema.YomiLastName), SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty YomiCompanyName = new ApplicationAggregatedProperty("YomiCompanyName", typeof(string), PropertyFlags.None, PersonPropertyAggregationStrategy.CreateNameProperty(InternalSchema.YomiCompany), SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty Title = new ApplicationAggregatedProperty("Title", typeof(string), PropertyFlags.None, PersonPropertyAggregationStrategy.CreatePriorityPropertyAggregation(InternalSchema.Title), SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty Department = new ApplicationAggregatedProperty("Department", typeof(string), PropertyFlags.None, PersonPropertyAggregationStrategy.CreatePriorityPropertyAggregation(InternalSchema.Department), SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty CompanyName = new ApplicationAggregatedProperty("CompanyName", typeof(string), PropertyFlags.None, PersonPropertyAggregationStrategy.CreatePriorityPropertyAggregation(InternalSchema.CompanyName), SortByAndFilterStrategy.CreateSimpleSort(InternalSchema.InternalPersonCompanyName));

		public static readonly ApplicationAggregatedProperty Alias = new ApplicationAggregatedProperty("Alias", typeof(string), PropertyFlags.None, PersonPropertyAggregationStrategy.CreatePriorityPropertyAggregation(InternalSchema.Account), SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty Location = new ApplicationAggregatedProperty("Location", typeof(string), PropertyFlags.None, PersonPropertyAggregationStrategy.CreatePriorityPropertyAggregation(InternalSchema.Location), SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty IMAddress = new ApplicationAggregatedProperty("IMAddress", typeof(string), PropertyFlags.None, PersonPropertyAggregationStrategy.IMAddressProperty, SortByAndFilterStrategy.SimpleCanQuery);

		public static readonly ApplicationAggregatedProperty HomeCity = new ApplicationAggregatedProperty("HomeCity", typeof(string), PropertyFlags.None, PersonPropertyAggregationStrategy.CreatePriorityPropertyAggregation(InternalSchema.HomeCity), SortByAndFilterStrategy.CreateSimpleSort(InternalSchema.InternalPersonHomeCity));

		public static readonly ApplicationAggregatedProperty EmailAddresses = new ApplicationAggregatedProperty("EmailAddresses", typeof(Participant[]), PropertyFlags.Multivalued, PersonPropertyAggregationStrategy.EmailAddressesProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty PostalAddresses = new ApplicationAggregatedProperty("PostalAddresses", typeof(PostalAddress[]), PropertyFlags.Multivalued, PersonPropertyAggregationStrategy.PostalAddressesProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty PostalAddressesWithDetails = new ApplicationAggregatedProperty("PostalAddressesWithDetails", typeof(PostalAddress[]), PropertyFlags.Multivalued, PersonPropertyAggregationStrategy.PostalAddressesWithDetailsProperty, SortByAndFilterStrategy.None);

		public static readonly PropertyDefinition ContactItemIds = new ApplicationAggregatedProperty("ContactItemIds", typeof(StoreObjectId[]), PropertyFlags.Multivalued, PropertyAggregationStrategy.EntryIdsProperty, SortByAndFilterStrategy.None);

		public static readonly PropertyDefinition RelevanceScore = new ApplicationAggregatedProperty("RelevanceScore", typeof(int), PropertyFlags.None, PersonPropertyAggregationStrategy.RelevanceScoreProperty, SortByAndFilterStrategy.CreateSimpleSort(InternalSchema.InternalPersonRelevanceScore));

		public static readonly PropertyDefinition GALLinkID = new ApplicationAggregatedProperty("GALLinkID", typeof(Guid), PropertyFlags.None, PersonPropertyAggregationStrategy.GALLinkIDProperty, SortByAndFilterStrategy.None);

		public static readonly PropertyDefinition PhotoContactEntryId = new ApplicationAggregatedProperty("PhotoContactEntryId", typeof(StoreObjectId), PropertyFlags.None, PersonPropertyAggregationStrategy.PhotoContactEntryIdProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty ThirdPartyPhotoUrls = new ApplicationAggregatedProperty("ThirdPartyPhotoUrls", typeof(IEnumerable<AttributedValue<string>>), PropertyFlags.None, PersonPropertyAggregationStrategy.AttributedThirdPartyPhotoUrlsProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty WorkCity = new ApplicationAggregatedProperty("WorkCity", typeof(string), PropertyFlags.None, PersonPropertyAggregationStrategy.CreatePriorityPropertyAggregation(InternalSchema.WorkAddressCity), SortByAndFilterStrategy.CreateSimpleSort(InternalSchema.InternalPersonWorkCity));

		public static readonly ApplicationAggregatedProperty DisplayNameFirstLast = new ApplicationAggregatedProperty("DisplayNameFirstLast", typeof(string), PropertyFlags.None, PersonPropertyAggregationStrategy.CreateNameProperty(InternalSchema.DisplayNameFirstLast), SortByAndFilterStrategy.CreateSimpleSort(InternalSchema.InternalPersonDisplayNameFirstLast));

		public static readonly ApplicationAggregatedProperty DisplayNameLastFirst = new ApplicationAggregatedProperty("DisplayNameLastFirst", typeof(string), PropertyFlags.None, PersonPropertyAggregationStrategy.CreateNameProperty(InternalSchema.DisplayNameLastFirst), SortByAndFilterStrategy.CreateSimpleSort(InternalSchema.InternalPersonDisplayNameLastFirst));

		public static readonly ApplicationAggregatedProperty EmailAddress = new ApplicationAggregatedProperty("EmailAddress", typeof(Participant), PropertyFlags.None, PersonPropertyAggregationStrategy.EmailAddressProperty, SortByAndFilterStrategy.SimpleCanQuery);

		public static readonly ApplicationAggregatedProperty PostalAddress = new ApplicationAggregatedProperty("PostalAddress", typeof(string), PropertyFlags.None, PersonPropertyAggregationStrategy.PostalAddressProperty, SortByAndFilterStrategy.SimpleCanQuery);

		public static readonly ApplicationAggregatedProperty PersonType = new ApplicationAggregatedProperty("PersonType", typeof(PersonType), PropertyFlags.None, PersonPropertyAggregationStrategy.PersonTypeProperty, SortByAndFilterStrategy.PersonType);

		public static readonly ApplicationAggregatedProperty Bodies = new ApplicationAggregatedProperty("Body", typeof(IEnumerable<AttributedValue<PersonNotes>>), PropertyFlags.None, PropertyAggregationStrategy.None, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty FolderIds = new ApplicationAggregatedProperty("FolderIds", typeof(IEnumerable<StoreObjectId>), PropertyFlags.None, PersonPropertyAggregationStrategy.FolderIdsProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty DisplayNameFirstLastSortKey = new ApplicationAggregatedProperty("DisplayNameFirstLastSortKey", typeof(string), PropertyFlags.None, PropertyAggregationStrategy.None, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty DisplayNameLastFirstSortKey = new ApplicationAggregatedProperty("DisplayNameLastFirstSortKey", typeof(string), PropertyFlags.None, PropertyAggregationStrategy.None, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty CompanyNameSortKey = new ApplicationAggregatedProperty("CompanyNameSortKey", typeof(string), PropertyFlags.None, PropertyAggregationStrategy.None, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty HomeCitySortKey = new ApplicationAggregatedProperty("HomeCitySortKey", typeof(string), PropertyFlags.None, PropertyAggregationStrategy.None, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty WorkCitySortKey = new ApplicationAggregatedProperty("WorkCitySortKey", typeof(string), PropertyFlags.None, PropertyAggregationStrategy.None, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty DisplayNameFirstLastHeader = new ApplicationAggregatedProperty("DisplayNameFirstLastHeader", typeof(string), PropertyFlags.None, PropertyAggregationStrategy.None, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty DisplayNameLastFirstHeader = new ApplicationAggregatedProperty("DisplayNameLastFirstHeader", typeof(string), PropertyFlags.None, PropertyAggregationStrategy.None, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty Attributions = new ApplicationAggregatedProperty("Attributions", typeof(IEnumerable<Attribution>), PropertyFlags.None, PersonPropertyAggregationStrategy.AttributionsProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty Members = new ApplicationAggregatedProperty("Members", typeof(Participant[]), PropertyFlags.None, PropertyAggregationStrategy.None, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty PhoneNumber = new ApplicationAggregatedProperty("PhoneNumber", typeof(PhoneNumber), PropertyFlags.None, PropertyAggregationStrategy.None, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty DisplayNames = new ApplicationAggregatedProperty("DisplayNames", typeof(IEnumerable<AttributedValue<string>>), PropertyFlags.None, PersonPropertyAggregationStrategy.AttributedDisplayNamesProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty FileAses = new ApplicationAggregatedProperty("FileAses", typeof(IEnumerable<AttributedValue<string>>), PropertyFlags.None, PersonPropertyAggregationStrategy.AttributedFileAsesProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty FileAsIds = new ApplicationAggregatedProperty("FileAsIds", typeof(IEnumerable<AttributedValue<string>>), PropertyFlags.None, PersonPropertyAggregationStrategy.AttributedFileAsIdsProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty DisplayNamePrefixes = new ApplicationAggregatedProperty("DisplayNamePrefixes", typeof(IEnumerable<AttributedValue<string>>), PropertyFlags.None, PersonPropertyAggregationStrategy.AttributedDisplayNamePrefixesProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty GivenNames = new ApplicationAggregatedProperty("GivenNames", typeof(IEnumerable<AttributedValue<string>>), PropertyFlags.None, PersonPropertyAggregationStrategy.AttributedGivenNamesProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty MiddleNames = new ApplicationAggregatedProperty("MiddleNames", typeof(IEnumerable<AttributedValue<string>>), PropertyFlags.None, PersonPropertyAggregationStrategy.AttributedMiddleNamesProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty Surnames = new ApplicationAggregatedProperty("Surnames", typeof(IEnumerable<AttributedValue<string>>), PropertyFlags.None, PersonPropertyAggregationStrategy.AttributedSurnamesProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty Generations = new ApplicationAggregatedProperty("Generations", typeof(IEnumerable<AttributedValue<string>>), PropertyFlags.None, PersonPropertyAggregationStrategy.AttributedGenerationsProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty Nicknames = new ApplicationAggregatedProperty("Nicknames", typeof(IEnumerable<AttributedValue<string>>), PropertyFlags.None, PersonPropertyAggregationStrategy.AttributedNicknamesProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty Initials = new ApplicationAggregatedProperty("Initials", typeof(IEnumerable<AttributedValue<string>>), PropertyFlags.None, PersonPropertyAggregationStrategy.AttributedInitialsProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty YomiCompanyNames = new ApplicationAggregatedProperty("YomiCompanyNames", typeof(IEnumerable<AttributedValue<string>>), PropertyFlags.None, PersonPropertyAggregationStrategy.AttributedYomiCompanyNamesProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty YomiFirstNames = new ApplicationAggregatedProperty("YomiFirstNames", typeof(IEnumerable<AttributedValue<string>>), PropertyFlags.None, PersonPropertyAggregationStrategy.AttributedYomiFirstNamesProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty YomiLastNames = new ApplicationAggregatedProperty("YomiLastNames", typeof(IEnumerable<AttributedValue<string>>), PropertyFlags.None, PersonPropertyAggregationStrategy.AttributedYomiLastNamesProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty BusinessPhoneNumbers = new ApplicationAggregatedProperty("BusinessPhoneNumbers", typeof(IEnumerable<AttributedValue<PhoneNumber>>), PropertyFlags.None, PersonPropertyAggregationStrategy.AttributedBusinessPhoneNumbersProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty BusinessPhoneNumbers2 = new ApplicationAggregatedProperty("BusinessPhoneNumbers2", typeof(IEnumerable<AttributedValue<PhoneNumber>>), PropertyFlags.None, PersonPropertyAggregationStrategy.AttributedBusinessPhoneNumbers2Property, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty HomePhones = new ApplicationAggregatedProperty("HomePhones", typeof(IEnumerable<AttributedValue<PhoneNumber>>), PropertyFlags.None, PersonPropertyAggregationStrategy.AttributedHomePhonesProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty HomePhones2 = new ApplicationAggregatedProperty("HomePhones2", typeof(IEnumerable<AttributedValue<PhoneNumber>>), PropertyFlags.None, PersonPropertyAggregationStrategy.AttributedHomePhones2Property, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty MobilePhones = new ApplicationAggregatedProperty("MobilePhones", typeof(IEnumerable<AttributedValue<PhoneNumber>>), PropertyFlags.None, PersonPropertyAggregationStrategy.AttributedMobilePhonesProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty MobilePhones2 = new ApplicationAggregatedProperty("MobilePhones2", typeof(IEnumerable<AttributedValue<PhoneNumber>>), PropertyFlags.None, PersonPropertyAggregationStrategy.AttributedMobilePhones2Property, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty AssistantPhoneNumbers = new ApplicationAggregatedProperty("AssistantPhoneNumbers", typeof(IEnumerable<AttributedValue<PhoneNumber>>), PropertyFlags.None, PersonPropertyAggregationStrategy.AttributedAssistantPhoneNumbersProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty CallbackPhones = new ApplicationAggregatedProperty("CallbackPhones", typeof(IEnumerable<AttributedValue<PhoneNumber>>), PropertyFlags.None, PersonPropertyAggregationStrategy.AttributedCallbackPhonesProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty CarPhones = new ApplicationAggregatedProperty("CarPhones", typeof(IEnumerable<AttributedValue<PhoneNumber>>), PropertyFlags.None, PersonPropertyAggregationStrategy.AttributedCarPhonesProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty HomeFaxes = new ApplicationAggregatedProperty("HomeFaxes", typeof(IEnumerable<AttributedValue<PhoneNumber>>), PropertyFlags.None, PersonPropertyAggregationStrategy.AttributedHomeFaxesProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty OrganizationMainPhones = new ApplicationAggregatedProperty("OrganizationMainPhones", typeof(IEnumerable<AttributedValue<PhoneNumber>>), PropertyFlags.None, PersonPropertyAggregationStrategy.AttributedOrganizationMainPhonesProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty OtherFaxes = new ApplicationAggregatedProperty("OtherFaxes", typeof(IEnumerable<AttributedValue<PhoneNumber>>), PropertyFlags.None, PersonPropertyAggregationStrategy.AttributedOtherFaxesProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty OtherTelephones = new ApplicationAggregatedProperty("OtherTelephones", typeof(IEnumerable<AttributedValue<PhoneNumber>>), PropertyFlags.None, PersonPropertyAggregationStrategy.AttributedOtherTelephonesProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty OtherPhones2 = new ApplicationAggregatedProperty("OtherPhones2", typeof(IEnumerable<AttributedValue<PhoneNumber>>), PropertyFlags.None, PersonPropertyAggregationStrategy.AttributedOtherPhones2Property, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty Pagers = new ApplicationAggregatedProperty("Pagers", typeof(IEnumerable<AttributedValue<PhoneNumber>>), PropertyFlags.None, PersonPropertyAggregationStrategy.AttributedPagersProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty RadioPhones = new ApplicationAggregatedProperty("RadioPhones", typeof(IEnumerable<AttributedValue<PhoneNumber>>), PropertyFlags.None, PersonPropertyAggregationStrategy.AttributedRadioPhonesProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty TelexNumbers = new ApplicationAggregatedProperty("TelexNumbers", typeof(IEnumerable<AttributedValue<PhoneNumber>>), PropertyFlags.None, PersonPropertyAggregationStrategy.AttributedTelexNumbersProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty TtyTddPhoneNumbers = new ApplicationAggregatedProperty("TtyTddPhoneNumbers", typeof(IEnumerable<AttributedValue<PhoneNumber>>), PropertyFlags.None, PersonPropertyAggregationStrategy.AttributedTtyTddPhoneNumbersProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty WorkFaxes = new ApplicationAggregatedProperty("WorkFaxes", typeof(IEnumerable<AttributedValue<PhoneNumber>>), PropertyFlags.None, PersonPropertyAggregationStrategy.AttributedWorkFaxesProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty Emails1 = new ApplicationAggregatedProperty("Emails1", typeof(IEnumerable<AttributedValue<Participant>>), PropertyFlags.None, PersonPropertyAggregationStrategy.AttributedEmails1Property, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty Emails2 = new ApplicationAggregatedProperty("Emails2", typeof(IEnumerable<AttributedValue<Participant>>), PropertyFlags.None, PersonPropertyAggregationStrategy.AttributedEmails2Property, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty Emails3 = new ApplicationAggregatedProperty("Emails3", typeof(IEnumerable<AttributedValue<Participant>>), PropertyFlags.None, PersonPropertyAggregationStrategy.AttributedEmails3Property, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty BusinessHomePages = new ApplicationAggregatedProperty("BusinessHomePages", typeof(IEnumerable<AttributedValue<string>>), PropertyFlags.None, PersonPropertyAggregationStrategy.AttributedBusinessHomePagesProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty Schools = new ApplicationAggregatedProperty("Schools", typeof(IEnumerable<AttributedValue<string>>), PropertyFlags.None, PersonPropertyAggregationStrategy.AttributedSchoolsProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty PersonalHomePages = new ApplicationAggregatedProperty("PersonalHomePages", typeof(IEnumerable<AttributedValue<string>>), PropertyFlags.None, PersonPropertyAggregationStrategy.AttributedPersonalHomePagesProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty OfficeLocations = new ApplicationAggregatedProperty("OfficeLocations", typeof(IEnumerable<AttributedValue<string>>), PropertyFlags.None, PersonPropertyAggregationStrategy.AttributedOfficeLocationsProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty IMAddresses = new ApplicationAggregatedProperty("IMAddresses", typeof(IEnumerable<AttributedValue<string>>), PropertyFlags.None, PersonPropertyAggregationStrategy.AttributedIMAddressesProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty IMAddresses2 = new ApplicationAggregatedProperty("IMAddresses2", typeof(IEnumerable<AttributedValue<string>>), PropertyFlags.None, PersonPropertyAggregationStrategy.AttributedIMAddresses2Property, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty IMAddresses3 = new ApplicationAggregatedProperty("IMAddresses3", typeof(IEnumerable<AttributedValue<string>>), PropertyFlags.None, PersonPropertyAggregationStrategy.AttributedIMAddresses3Property, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty Titles = new ApplicationAggregatedProperty("Titles", typeof(IEnumerable<AttributedValue<string>>), PropertyFlags.None, PersonPropertyAggregationStrategy.AttributedTitlesProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty Departments = new ApplicationAggregatedProperty("Departments", typeof(IEnumerable<AttributedValue<string>>), PropertyFlags.None, PersonPropertyAggregationStrategy.AttributedDepartmentsProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty CompanyNames = new ApplicationAggregatedProperty("CompanyNames", typeof(IEnumerable<AttributedValue<string>>), PropertyFlags.None, PersonPropertyAggregationStrategy.AttributedCompanyNamesProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty Managers = new ApplicationAggregatedProperty("Managers", typeof(IEnumerable<AttributedValue<string>>), PropertyFlags.None, PersonPropertyAggregationStrategy.AttributedManagersProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty AssistantNames = new ApplicationAggregatedProperty("AssistantNames", typeof(IEnumerable<AttributedValue<string>>), PropertyFlags.None, PersonPropertyAggregationStrategy.AttributedAssistantNamesProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty Professions = new ApplicationAggregatedProperty("Professions", typeof(IEnumerable<AttributedValue<string>>), PropertyFlags.None, PersonPropertyAggregationStrategy.AttributedProfessionsProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty SpouseNames = new ApplicationAggregatedProperty("SpouseNames", typeof(IEnumerable<AttributedValue<string>>), PropertyFlags.None, PersonPropertyAggregationStrategy.AttributedSpouseNamesProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty Children = new ApplicationAggregatedProperty("Children", typeof(IEnumerable<AttributedValue<string[]>>), PropertyFlags.None, PersonPropertyAggregationStrategy.AttributedChildrenProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty Hobbies = new ApplicationAggregatedProperty("Hobbies", typeof(IEnumerable<AttributedValue<string>>), PropertyFlags.None, PersonPropertyAggregationStrategy.AttributedHobbiesProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty WeddingAnniversaries = new ApplicationAggregatedProperty("WeddingAnniversaries", typeof(IEnumerable<AttributedValue<ExDateTime>>), PropertyFlags.None, PersonPropertyAggregationStrategy.AttributedWeddingAnniversariesProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty Birthdays = new ApplicationAggregatedProperty("Birthdays", typeof(IEnumerable<AttributedValue<ExDateTime>>), PropertyFlags.None, PersonPropertyAggregationStrategy.AttributedBirthdaysProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty WeddingAnniversariesLocal = new ApplicationAggregatedProperty("WeddingAnniversariesLocal", typeof(IEnumerable<AttributedValue<ExDateTime>>), PropertyFlags.None, PersonPropertyAggregationStrategy.AttributedWeddingAnniversariesLocalProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty BirthdaysLocal = new ApplicationAggregatedProperty("BirthdaysLocal", typeof(IEnumerable<AttributedValue<ExDateTime>>), PropertyFlags.None, PersonPropertyAggregationStrategy.AttributedBirthdaysLocalProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty Locations = new ApplicationAggregatedProperty("Locations", typeof(IEnumerable<AttributedValue<string>>), PropertyFlags.None, PersonPropertyAggregationStrategy.AttributedLocationsProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty ExtendedProperties = new ApplicationAggregatedProperty("ExtendedProperties", typeof(IEnumerable<AttributedValue<ContactExtendedPropertyData>>), PropertyFlags.None, PropertyAggregationStrategy.None, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty HomeAddresses = new ApplicationAggregatedProperty("HomeAddresses", typeof(IEnumerable<AttributedValue<PostalAddress>>), PropertyFlags.None, PersonPropertyAggregationStrategy.AttributedHomeAddressesProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty BusinessAddresses = new ApplicationAggregatedProperty("BusinessAddresses", typeof(IEnumerable<AttributedValue<PostalAddress>>), PropertyFlags.None, PersonPropertyAggregationStrategy.AttributedWorkAddressesProperty, SortByAndFilterStrategy.None);

		public static readonly ApplicationAggregatedProperty OtherAddresses = new ApplicationAggregatedProperty("OtherAddresses", typeof(IEnumerable<AttributedValue<PostalAddress>>), PropertyFlags.None, PersonPropertyAggregationStrategy.AttributedOtherAddressesProperty, SortByAndFilterStrategy.None);

		private static readonly PersonSchema instance = new PersonSchema();
	}
}
