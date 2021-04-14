using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IPerson
	{
		PersonId PersonId { get; }

		PersonType PersonType { get; }

		ExDateTime CreationTime { get; }

		string DisplayName { get; }

		string FileAs { get; }

		string FileAsId { get; }

		string DisplayNamePrefix { get; }

		string GivenName { get; }

		string MiddleName { get; }

		string Surname { get; }

		string Generation { get; }

		string Nickname { get; }

		string Alias { get; }

		string YomiFirstName { get; }

		string YomiLastName { get; }

		string Title { get; }

		string Department { get; }

		string CompanyName { get; }

		string Location { get; }

		Participant EmailAddress { get; }

		IEnumerable<Participant> EmailAddresses { get; }

		PhoneNumber PhoneNumber { get; }

		string ImAddress { get; }

		string HomeCity { get; }

		IEnumerable<StoreObjectId> FolderIds { get; }

		IEnumerable<Attribution> Attributions { get; }

		IEnumerable<AttributedValue<string>> DisplayNames { get; }

		IEnumerable<AttributedValue<string>> FileAses { get; }

		IEnumerable<AttributedValue<string>> FileAsIds { get; }

		IEnumerable<AttributedValue<string>> DisplayNamePrefixes { get; }

		IEnumerable<AttributedValue<string>> GivenNames { get; }

		IEnumerable<AttributedValue<string>> MiddleNames { get; }

		IEnumerable<AttributedValue<string>> Surnames { get; }

		IEnumerable<AttributedValue<string>> Generations { get; }

		IEnumerable<AttributedValue<string>> Nicknames { get; }

		IEnumerable<AttributedValue<string>> Initials { get; }

		IEnumerable<AttributedValue<string>> YomiFirstNames { get; }

		IEnumerable<AttributedValue<string>> YomiLastNames { get; }

		IEnumerable<AttributedValue<PhoneNumber>> BusinessPhoneNumbers { get; }

		IEnumerable<AttributedValue<PhoneNumber>> BusinessPhoneNumbers2 { get; }

		IEnumerable<AttributedValue<PhoneNumber>> HomePhones { get; }

		IEnumerable<AttributedValue<PhoneNumber>> HomePhones2 { get; }

		IEnumerable<AttributedValue<PhoneNumber>> MobilePhones { get; }

		IEnumerable<AttributedValue<PhoneNumber>> MobilePhones2 { get; }

		IEnumerable<AttributedValue<PhoneNumber>> AssistantPhoneNumbers { get; }

		IEnumerable<AttributedValue<PhoneNumber>> CallbackPhones { get; }

		IEnumerable<AttributedValue<PhoneNumber>> CarPhones { get; }

		IEnumerable<AttributedValue<PhoneNumber>> HomeFaxes { get; }

		IEnumerable<AttributedValue<PhoneNumber>> OrganizationMainPhones { get; }

		IEnumerable<AttributedValue<PhoneNumber>> OtherFaxes { get; }

		IEnumerable<AttributedValue<PhoneNumber>> OtherTelephones { get; }

		IEnumerable<AttributedValue<PhoneNumber>> OtherPhones2 { get; }

		IEnumerable<AttributedValue<PhoneNumber>> Pagers { get; }

		IEnumerable<AttributedValue<PhoneNumber>> RadioPhones { get; }

		IEnumerable<AttributedValue<PhoneNumber>> TelexNumbers { get; }

		IEnumerable<AttributedValue<PhoneNumber>> TTYTDDPhoneNumbers { get; }

		IEnumerable<AttributedValue<PhoneNumber>> WorkFaxes { get; }

		IEnumerable<AttributedValue<Participant>> Emails1 { get; }

		IEnumerable<AttributedValue<Participant>> Emails2 { get; }

		IEnumerable<AttributedValue<Participant>> Emails3 { get; }

		IEnumerable<AttributedValue<string>> BusinessHomePages { get; }

		IEnumerable<AttributedValue<string>> PersonalHomePages { get; }

		IEnumerable<AttributedValue<string>> OfficeLocations { get; }

		IEnumerable<AttributedValue<string>> ImAddresses { get; }

		IEnumerable<AttributedValue<PostalAddress>> BusinessAddresses { get; }

		IEnumerable<AttributedValue<PostalAddress>> HomeAddresses { get; }

		IEnumerable<AttributedValue<PostalAddress>> OtherAddresses { get; }

		IEnumerable<AttributedValue<string>> Titles { get; }

		IEnumerable<AttributedValue<string>> Departments { get; }

		IEnumerable<AttributedValue<string>> CompanyNames { get; }

		IEnumerable<AttributedValue<string>> Managers { get; }

		IEnumerable<AttributedValue<string>> AssistantNames { get; }

		IEnumerable<AttributedValue<string>> Professions { get; }

		IEnumerable<AttributedValue<string>> SpouseNames { get; }

		IEnumerable<AttributedValue<string[]>> Children { get; }

		IEnumerable<AttributedValue<string>> Hobbies { get; }

		IEnumerable<AttributedValue<ExDateTime>> WeddingAnniversaries { get; }

		IEnumerable<AttributedValue<ExDateTime>> Birthdays { get; }

		IEnumerable<AttributedValue<ExDateTime>> WeddingAnniversariesLocal { get; }

		IEnumerable<AttributedValue<ExDateTime>> BirthdaysLocal { get; }

		IEnumerable<AttributedValue<string>> Locations { get; }
	}
}
