using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class PersonaSchema : Schema
	{
		static PersonaSchema()
		{
			XmlElementInformation[] xmlElements = new XmlElementInformation[]
			{
				PersonaSchema.PersonaId,
				PersonaSchema.PersonaType,
				PersonaSchema.CreationTime,
				PersonaSchema.IsFavorite,
				PersonaSchema.DisplayNameFirstLastSortKey,
				PersonaSchema.DisplayNameLastFirstSortKey,
				PersonaSchema.CompanyNameSortKey,
				PersonaSchema.HomeCitySortKey,
				PersonaSchema.WorkCitySortKey,
				PersonaSchema.DisplayNameFirstLastHeader,
				PersonaSchema.DisplayNameLastFirstHeader,
				PersonaSchema.DisplayName,
				PersonaSchema.DisplayNameFirstLast,
				PersonaSchema.DisplayNameLastFirst,
				PersonaSchema.FileAs,
				PersonaSchema.FileAsId,
				PersonaSchema.DisplayNamePrefix,
				PersonaSchema.GivenName,
				PersonaSchema.MiddleName,
				PersonaSchema.Surname,
				PersonaSchema.Generation,
				PersonaSchema.Nickname,
				PersonaSchema.YomiCompanyName,
				PersonaSchema.YomiFirstName,
				PersonaSchema.YomiLastName,
				PersonaSchema.Title,
				PersonaSchema.Department,
				PersonaSchema.CompanyName,
				PersonaSchema.Location,
				PersonaSchema.EmailAddress,
				PersonaSchema.EmailAddresses,
				PersonaSchema.PhoneNumber,
				PersonaSchema.ImAddress,
				PersonaSchema.HomeCity,
				PersonaSchema.WorkCity,
				PersonaSchema.Alias,
				PersonaSchema.RelevanceScore,
				PersonaSchema.FolderIds,
				PersonaSchema.Attributions,
				PersonaSchema.Members,
				PersonaSchema.DisplayNames,
				PersonaSchema.FileAses,
				PersonaSchema.FileAsIds,
				PersonaSchema.DisplayNamePrefixes,
				PersonaSchema.GivenNames,
				PersonaSchema.MiddleNames,
				PersonaSchema.Surnames,
				PersonaSchema.Generations,
				PersonaSchema.Nicknames,
				PersonaSchema.Initials,
				PersonaSchema.YomiCompanyNames,
				PersonaSchema.YomiFirstNames,
				PersonaSchema.YomiLastNames,
				PersonaSchema.BusinessPhoneNumbers,
				PersonaSchema.BusinessPhoneNumbers2,
				PersonaSchema.HomePhones,
				PersonaSchema.HomePhones2,
				PersonaSchema.MobilePhones,
				PersonaSchema.MobilePhones2,
				PersonaSchema.AssistantPhoneNumbers,
				PersonaSchema.CallbackPhones,
				PersonaSchema.CarPhones,
				PersonaSchema.HomeFaxes,
				PersonaSchema.OrganizationMainPhones,
				PersonaSchema.OtherFaxes,
				PersonaSchema.OtherTelephones,
				PersonaSchema.OtherPhones2,
				PersonaSchema.Pagers,
				PersonaSchema.RadioPhones,
				PersonaSchema.TelexNumbers,
				PersonaSchema.TTYTDDPhoneNumbers,
				PersonaSchema.WorkFaxes,
				PersonaSchema.Emails1,
				PersonaSchema.Emails2,
				PersonaSchema.Emails3,
				PersonaSchema.BusinessHomePages,
				PersonaSchema.PersonalHomePages,
				PersonaSchema.OfficeLocations,
				PersonaSchema.ImAddresses,
				PersonaSchema.ImAddresses2,
				PersonaSchema.ImAddresses3,
				PersonaSchema.BusinessAddresses,
				PersonaSchema.HomeAddresses,
				PersonaSchema.OtherAddresses,
				PersonaSchema.Titles,
				PersonaSchema.Departments,
				PersonaSchema.CompanyNames,
				PersonaSchema.Managers,
				PersonaSchema.AssistantNames,
				PersonaSchema.Professions,
				PersonaSchema.SpouseNames,
				PersonaSchema.Children,
				PersonaSchema.Hobbies,
				PersonaSchema.WeddingAnniversaries,
				PersonaSchema.Birthdays,
				PersonaSchema.WeddingAnniversariesLocal,
				PersonaSchema.BirthdaysLocal,
				PersonaSchema.Locations,
				PersonaSchema.ExtendedProperties,
				PersonaSchema.Schools,
				PersonaSchema.ThirdPartyPhotoUrls,
				PersonaSchema.UnreadCount,
				PersonaSchema.ADObjectId
			};
			PersonaSchema.EwsCalculatedProperties = new HashSet<PropertyPath>
			{
				PersonaSchema.DisplayNameFirstLastSortKey.PropertyPath,
				PersonaSchema.DisplayNameLastFirstSortKey.PropertyPath,
				PersonaSchema.CompanyNameSortKey.PropertyPath,
				PersonaSchema.HomeCitySortKey.PropertyPath,
				PersonaSchema.WorkCitySortKey.PropertyPath,
				PersonaSchema.DisplayNameFirstLastHeader.PropertyPath,
				PersonaSchema.DisplayNameLastFirstHeader.PropertyPath,
				PersonaSchema.Bodies.PropertyPath,
				PersonaSchema.ExtendedProperties.PropertyPath,
				PersonaSchema.UnreadCount.PropertyPath
			};
			PersonaSchema.AllPropertiesExclusionList = new HashSet<PropertyInformation>
			{
				PersonaSchema.DisplayNameFirstLastSortKey,
				PersonaSchema.DisplayNameLastFirstSortKey,
				PersonaSchema.CompanyNameSortKey,
				PersonaSchema.HomeCitySortKey,
				PersonaSchema.WorkCitySortKey,
				PersonaSchema.DisplayNameFirstLastHeader,
				PersonaSchema.DisplayNameLastFirstHeader,
				PersonaSchema.FolderIds,
				PersonaSchema.Members,
				PersonaSchema.ThirdPartyPhotoUrls,
				PersonaSchema.Alias,
				PersonaSchema.UnreadCount
			};
			PersonaSchema.schema = new PersonaSchema(xmlElements, PersonaSchema.PersonaId);
		}

		private PersonaSchema(XmlElementInformation[] xmlElements, PropertyInformation personaIdPropertyInformation) : base(xmlElements, personaIdPropertyInformation)
		{
		}

		public static Schema GetSchema()
		{
			return PersonaSchema.schema;
		}

		private static Schema schema;

		public static readonly HashSet<PropertyInformation> AllPropertiesExclusionList;

		public static readonly HashSet<PropertyPath> EwsCalculatedProperties;

		public static readonly PropertyInformation PersonaId = new PropertyInformation("PersonaId", ServiceXml.GetFullyQualifiedName("PersonaId"), ServiceXml.DefaultNamespaceUri, ExchangeVersion.Exchange2012, new PropertyDefinition[]
		{
			PersonSchema.Id,
			PersonSchema.GALLinkID
		}, new PropertyUri(PropertyUriEnum.PersonaId), new PropertyCommand.CreatePropertyCommand(PersonaIdProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation PersonaType = new PropertyInformation("PersonaType", ExchangeVersion.Exchange2012, PersonSchema.PersonType, new PropertyUri(PropertyUriEnum.PersonaType), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation ADObjectId = new PropertyInformation("ADObjectId", ExchangeVersion.Exchange2012, PersonSchema.GALLinkID, new PropertyUri(PropertyUriEnum.PersonaADObjectId), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation CreationTime = new PropertyInformation("CreationTime", ExchangeVersion.Exchange2012, PersonSchema.CreationTime, new PropertyUri(PropertyUriEnum.PersonaCreationTime), new PropertyCommand.CreatePropertyCommand(DateTimeProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation IsFavorite = new PropertyInformation("IsFavorite", ExchangeVersion.Exchange2012, PersonSchema.IsFavorite, new PropertyUri(PropertyUriEnum.PersonaIsFavorite), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation DisplayName = new PropertyInformation(PropertyUriEnum.PersonaDisplayName, ExchangeVersion.Exchange2012, PersonSchema.DisplayName, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation DisplayNameFirstLast = new PropertyInformation(PropertyUriEnum.PersonaDisplayNameFirstLast, ExchangeVersion.Exchange2012, PersonSchema.DisplayNameFirstLast, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation DisplayNameLastFirst = new PropertyInformation(PropertyUriEnum.PersonaDisplayNameLastFirst, ExchangeVersion.Exchange2012, PersonSchema.DisplayNameLastFirst, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation DisplayNameFirstLastSortKey = new PropertyInformation(PropertyUriEnum.PersonaDisplayNameFirstLastSortKey, ExchangeVersion.Exchange2012, PersonSchema.DisplayNameFirstLastSortKey, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation DisplayNameLastFirstSortKey = new PropertyInformation(PropertyUriEnum.PersonaDisplayNameLastFirstSortKey, ExchangeVersion.Exchange2012, PersonSchema.DisplayNameLastFirstSortKey, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation CompanyNameSortKey = new PropertyInformation(PropertyUriEnum.PersonaCompanyNameSortKey, ExchangeVersion.Exchange2012, PersonSchema.CompanyNameSortKey, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation HomeCitySortKey = new PropertyInformation(PropertyUriEnum.PersonaHomeCitySortKey, ExchangeVersion.Exchange2012, PersonSchema.HomeCitySortKey, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation WorkCitySortKey = new PropertyInformation(PropertyUriEnum.PersonaWorkCitySortKey, ExchangeVersion.Exchange2012, PersonSchema.WorkCitySortKey, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation DisplayNameFirstLastHeader = new PropertyInformation(PropertyUriEnum.PersonaDisplayNameFirstLastHeader, ExchangeVersion.Exchange2012, PersonSchema.DisplayNameFirstLastHeader, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation DisplayNameLastFirstHeader = new PropertyInformation(PropertyUriEnum.PersonaDisplayNameLastFirstHeader, ExchangeVersion.Exchange2012, PersonSchema.DisplayNameLastFirstHeader, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation FileAs = new PropertyInformation(PropertyUriEnum.PersonaFileAs, ExchangeVersion.Exchange2012, PersonSchema.FileAs, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation FileAsId = new PropertyInformation(PropertyUriEnum.PersonaFileAsId, ExchangeVersion.Exchange2012, PersonSchema.FileAsId, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation DisplayNamePrefix = new PropertyInformation(PropertyUriEnum.PersonaDisplayNamePrefix, ExchangeVersion.Exchange2012, PersonSchema.DisplayNamePrefix, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation GivenName = new PropertyInformation(PropertyUriEnum.PersonaGivenName, ExchangeVersion.Exchange2012, PersonSchema.GivenName, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation MiddleName = new PropertyInformation(PropertyUriEnum.PersonaMiddleName, ExchangeVersion.Exchange2012, PersonSchema.MiddleName, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation Surname = new PropertyInformation(PropertyUriEnum.PersonaSurname, ExchangeVersion.Exchange2012, PersonSchema.Surname, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation Generation = new PropertyInformation(PropertyUriEnum.PersonaGeneration, ExchangeVersion.Exchange2012, PersonSchema.Generation, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation Nickname = new PropertyInformation(PropertyUriEnum.PersonaNickname, ExchangeVersion.Exchange2012, PersonSchema.Nickname, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation Alias = new PropertyInformation(PropertyUriEnum.PersonaAlias, ExchangeVersion.Exchange2012, PersonSchema.Alias, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation YomiFirstName = new PropertyInformation(PropertyUriEnum.PersonaYomiFirstName, ExchangeVersion.Exchange2012, PersonSchema.YomiFirstName, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation YomiCompanyName = new PropertyInformation(PropertyUriEnum.PersonaYomiCompanyName, ExchangeVersion.Exchange2012, PersonSchema.YomiCompanyName, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation YomiLastName = new PropertyInformation(PropertyUriEnum.PersonaYomiLastName, ExchangeVersion.Exchange2012, PersonSchema.YomiLastName, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation Title = new PropertyInformation(PropertyUriEnum.PersonaTitle, ExchangeVersion.Exchange2012, PersonSchema.Title, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation Department = new PropertyInformation(PropertyUriEnum.PersonaDepartment, ExchangeVersion.Exchange2012, PersonSchema.Department, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation CompanyName = new PropertyInformation(PropertyUriEnum.PersonaCompanyName, ExchangeVersion.Exchange2012, PersonSchema.CompanyName, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation Location = new PropertyInformation(PropertyUriEnum.PersonaLocation, ExchangeVersion.Exchange2012, PersonSchema.Location, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation EmailAddress = new PropertyInformation(PropertyUriEnum.PersonaEmailAddress, ExchangeVersion.Exchange2012, PersonSchema.EmailAddress, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation EmailAddresses = new ArrayPropertyInformation("EmailAddresses", ExchangeVersion.Exchange2012, "EmailAddress", PersonSchema.EmailAddresses, new PropertyUri(PropertyUriEnum.EmailAddresses), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand));

		public static readonly PropertyInformation PhoneNumber = new PropertyInformation(PropertyUriEnum.PersonaPhoneNumber, ExchangeVersion.Exchange2012, PersonSchema.PhoneNumber, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation ImAddress = new PropertyInformation(PropertyUriEnum.PersonaImAddress, ExchangeVersion.Exchange2012, PersonSchema.IMAddress, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation HomeCity = new PropertyInformation(PropertyUriEnum.PersonaHomeCity, ExchangeVersion.Exchange2012, PersonSchema.HomeCity, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation WorkCity = new PropertyInformation(PropertyUriEnum.PersonaWorkCity, ExchangeVersion.Exchange2012, PersonSchema.WorkCity, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation RelevanceScore = new PropertyInformation(PropertyUriEnum.PersonaRelevanceScore, ExchangeVersion.Exchange2012, PersonSchema.RelevanceScore, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation UnreadCount = new PropertyInformation(PropertyUriEnum.PersonaUnreadCount, ExchangeVersion.Exchange2012, null, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation Bodies = new ArrayPropertyInformation("PersonaBodies", ExchangeVersion.Exchange2012, "BodyContentAttributedValue", PersonSchema.Bodies, new PropertyUri(PropertyUriEnum.PersonaBodies), new PropertyCommand.CreatePropertyCommand(PersonaBodiesProperty.CreateCommand));

		public static readonly PropertyInformation FolderIds = new ArrayPropertyInformation("FolderIds", ExchangeVersion.Exchange2012, "FolderId", PersonSchema.FolderIds, new PropertyUri(PropertyUriEnum.PersonaFolderIds), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand));

		public static readonly PropertyInformation Attributions = new ArrayPropertyInformation("Attributions", ExchangeVersion.Exchange2012, "Attribution", PersonSchema.Attributions, new PropertyUri(PropertyUriEnum.PersonaAttributions), new PropertyCommand.CreatePropertyCommand(AttributionProperty.CreateCommand));

		public static readonly PropertyInformation Members = new ArrayPropertyInformation("Members", ExchangeVersion.Exchange2012, "EmailAddressWrapper", PersonSchema.Members, new PropertyUri(PropertyUriEnum.PersonaMembers), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand));

		public static readonly PropertyInformation DisplayNames = new ArrayPropertyInformation("DisplayNames", ExchangeVersion.Exchange2012, "StringAttributedValue", PersonSchema.DisplayNames, new PropertyUri(PropertyUriEnum.PersonaDisplayNames), new PropertyCommand.CreatePropertyCommand(StringAttributedValueProperty.CreateCommand));

		public static readonly PropertyInformation FileAses = new ArrayPropertyInformation("FileAses", ExchangeVersion.Exchange2012, "StringAttributedValue", PersonSchema.FileAses, new PropertyUri(PropertyUriEnum.PersonaFileAses), new PropertyCommand.CreatePropertyCommand(StringAttributedValueProperty.CreateCommand));

		public static readonly PropertyInformation FileAsIds = new ArrayPropertyInformation("FileAsIds", ExchangeVersion.Exchange2012, "StringAttributedValue", PersonSchema.FileAsIds, new PropertyUri(PropertyUriEnum.PersonaFileAsIds), new PropertyCommand.CreatePropertyCommand(StringAttributedValueProperty.CreateCommand));

		public static readonly PropertyInformation DisplayNamePrefixes = new ArrayPropertyInformation("DisplayNamePrefixes", ExchangeVersion.Exchange2012, "StringAttributedValue", PersonSchema.DisplayNamePrefixes, new PropertyUri(PropertyUriEnum.PersonaDisplayNamePrefixes), new PropertyCommand.CreatePropertyCommand(StringAttributedValueProperty.CreateCommand));

		public static readonly PropertyInformation GivenNames = new ArrayPropertyInformation("GivenNames", ExchangeVersion.Exchange2012, "StringAttributedValue", PersonSchema.GivenNames, new PropertyUri(PropertyUriEnum.PersonaGivenNames), new PropertyCommand.CreatePropertyCommand(StringAttributedValueProperty.CreateCommand));

		public static readonly PropertyInformation MiddleNames = new ArrayPropertyInformation("MiddleNames", ExchangeVersion.Exchange2012, "StringAttributedValue", PersonSchema.MiddleNames, new PropertyUri(PropertyUriEnum.PersonaMiddleNames), new PropertyCommand.CreatePropertyCommand(StringAttributedValueProperty.CreateCommand));

		public static readonly PropertyInformation Surnames = new ArrayPropertyInformation("Surnames", ExchangeVersion.Exchange2012, "StringAttributedValue", PersonSchema.Surnames, new PropertyUri(PropertyUriEnum.PersonaSurnames), new PropertyCommand.CreatePropertyCommand(StringAttributedValueProperty.CreateCommand));

		public static readonly PropertyInformation Generations = new ArrayPropertyInformation("Generations", ExchangeVersion.Exchange2012, "StringAttributedValue", PersonSchema.Generations, new PropertyUri(PropertyUriEnum.PersonaGenerations), new PropertyCommand.CreatePropertyCommand(StringAttributedValueProperty.CreateCommand));

		public static readonly PropertyInformation Nicknames = new ArrayPropertyInformation("Nicknames", ExchangeVersion.Exchange2012, "StringAttributedValue", PersonSchema.Nicknames, new PropertyUri(PropertyUriEnum.PersonaNicknames), new PropertyCommand.CreatePropertyCommand(StringAttributedValueProperty.CreateCommand));

		public static readonly PropertyInformation Initials = new ArrayPropertyInformation("Initials", ExchangeVersion.Exchange2012, "StringAttributedValue", PersonSchema.Initials, new PropertyUri(PropertyUriEnum.PersonaInitials), new PropertyCommand.CreatePropertyCommand(StringAttributedValueProperty.CreateCommand));

		public static readonly PropertyInformation YomiCompanyNames = new ArrayPropertyInformation("YomiCompanyNames", ExchangeVersion.Exchange2012, "StringAttributedValue", PersonSchema.YomiCompanyNames, new PropertyUri(PropertyUriEnum.PersonaYomiCompanyNames), new PropertyCommand.CreatePropertyCommand(StringAttributedValueProperty.CreateCommand));

		public static readonly PropertyInformation YomiFirstNames = new ArrayPropertyInformation("YomiFirstNames", ExchangeVersion.Exchange2012, "StringAttributedValue", PersonSchema.YomiFirstNames, new PropertyUri(PropertyUriEnum.PersonaYomiFirstNames), new PropertyCommand.CreatePropertyCommand(StringAttributedValueProperty.CreateCommand));

		public static readonly PropertyInformation YomiLastNames = new ArrayPropertyInformation("YomiLastNames", ExchangeVersion.Exchange2012, "StringAttributedValue", PersonSchema.YomiLastNames, new PropertyUri(PropertyUriEnum.PersonaYomiLastNames), new PropertyCommand.CreatePropertyCommand(StringAttributedValueProperty.CreateCommand));

		public static readonly PropertyInformation BusinessPhoneNumbers = new ArrayPropertyInformation("BusinessPhoneNumbers", ExchangeVersion.Exchange2012, "PhoneNumberAttributedValue", PersonSchema.BusinessPhoneNumbers, new PropertyUri(PropertyUriEnum.PersonaBusinessPhoneNumbers), new PropertyCommand.CreatePropertyCommand(PhoneNumberAttributedValueProperty.CreateCommand));

		public static readonly PropertyInformation BusinessPhoneNumbers2 = new ArrayPropertyInformation("BusinessPhoneNumbers2", ExchangeVersion.Exchange2012, "PhoneNumberAttributedValue", PersonSchema.BusinessPhoneNumbers2, new PropertyUri(PropertyUriEnum.PersonaBusinessPhoneNumbers2), new PropertyCommand.CreatePropertyCommand(PhoneNumberAttributedValueProperty.CreateCommand));

		public static readonly PropertyInformation HomePhones = new ArrayPropertyInformation("HomePhones", ExchangeVersion.Exchange2012, "PhoneNumberAttributedValue", PersonSchema.HomePhones, new PropertyUri(PropertyUriEnum.PersonaHomePhones), new PropertyCommand.CreatePropertyCommand(PhoneNumberAttributedValueProperty.CreateCommand));

		public static readonly PropertyInformation HomePhones2 = new ArrayPropertyInformation("HomePhones2", ExchangeVersion.Exchange2012, "PhoneNumberAttributedValue", PersonSchema.HomePhones2, new PropertyUri(PropertyUriEnum.PersonaHomePhones2), new PropertyCommand.CreatePropertyCommand(PhoneNumberAttributedValueProperty.CreateCommand));

		public static readonly PropertyInformation MobilePhones = new ArrayPropertyInformation("MobilePhones", ExchangeVersion.Exchange2012, "PhoneNumberAttributedValue", PersonSchema.MobilePhones, new PropertyUri(PropertyUriEnum.PersonaMobilePhones), new PropertyCommand.CreatePropertyCommand(PhoneNumberAttributedValueProperty.CreateCommand));

		public static readonly PropertyInformation MobilePhones2 = new ArrayPropertyInformation("MobilePhones2", ExchangeVersion.Exchange2012, "PhoneNumberAttributedValue", PersonSchema.MobilePhones2, new PropertyUri(PropertyUriEnum.PersonaMobilePhones2), new PropertyCommand.CreatePropertyCommand(PhoneNumberAttributedValueProperty.CreateCommand));

		public static readonly PropertyInformation AssistantPhoneNumbers = new ArrayPropertyInformation("AssistantPhoneNumbers", ExchangeVersion.Exchange2012, "PhoneNumberAttributedValue", PersonSchema.AssistantPhoneNumbers, new PropertyUri(PropertyUriEnum.PersonaAssistantPhoneNumbers), new PropertyCommand.CreatePropertyCommand(PhoneNumberAttributedValueProperty.CreateCommand));

		public static readonly PropertyInformation CallbackPhones = new ArrayPropertyInformation("CallbackPhones", ExchangeVersion.Exchange2012, "PhoneNumberAttributedValue", PersonSchema.CallbackPhones, new PropertyUri(PropertyUriEnum.PersonaCallbackPhones), new PropertyCommand.CreatePropertyCommand(PhoneNumberAttributedValueProperty.CreateCommand));

		public static readonly PropertyInformation CarPhones = new ArrayPropertyInformation("CarPhones", ExchangeVersion.Exchange2012, "PhoneNumberAttributedValue", PersonSchema.CarPhones, new PropertyUri(PropertyUriEnum.PersonaCarPhones), new PropertyCommand.CreatePropertyCommand(PhoneNumberAttributedValueProperty.CreateCommand));

		public static readonly PropertyInformation HomeFaxes = new ArrayPropertyInformation("HomeFaxes", ExchangeVersion.Exchange2012, "PhoneNumberAttributedValue", PersonSchema.HomeFaxes, new PropertyUri(PropertyUriEnum.PersonaHomeFaxes), new PropertyCommand.CreatePropertyCommand(PhoneNumberAttributedValueProperty.CreateCommand));

		public static readonly PropertyInformation OrganizationMainPhones = new ArrayPropertyInformation("OrganizationMainPhones", ExchangeVersion.Exchange2012, "PhoneNumberAttributedValue", PersonSchema.OrganizationMainPhones, new PropertyUri(PropertyUriEnum.PersonaOrganizationMainPhones), new PropertyCommand.CreatePropertyCommand(PhoneNumberAttributedValueProperty.CreateCommand));

		public static readonly PropertyInformation OtherFaxes = new ArrayPropertyInformation("OtherFaxes", ExchangeVersion.Exchange2012, "PhoneNumberAttributedValue", PersonSchema.OtherFaxes, new PropertyUri(PropertyUriEnum.PersonaOtherFaxes), new PropertyCommand.CreatePropertyCommand(PhoneNumberAttributedValueProperty.CreateCommand));

		public static readonly PropertyInformation OtherTelephones = new ArrayPropertyInformation("OtherTelephones", ExchangeVersion.Exchange2012, "PhoneNumberAttributedValue", PersonSchema.OtherTelephones, new PropertyUri(PropertyUriEnum.PersonaOtherTelephones), new PropertyCommand.CreatePropertyCommand(PhoneNumberAttributedValueProperty.CreateCommand));

		public static readonly PropertyInformation OtherPhones2 = new ArrayPropertyInformation("OtherPhones2", ExchangeVersion.Exchange2012, "PhoneNumberAttributedValue", PersonSchema.OtherPhones2, new PropertyUri(PropertyUriEnum.PersonaOtherPhones2), new PropertyCommand.CreatePropertyCommand(PhoneNumberAttributedValueProperty.CreateCommand));

		public static readonly PropertyInformation Pagers = new ArrayPropertyInformation("Pagers", ExchangeVersion.Exchange2012, "PhoneNumberAttributedValue", PersonSchema.Pagers, new PropertyUri(PropertyUriEnum.PersonaPagers), new PropertyCommand.CreatePropertyCommand(PhoneNumberAttributedValueProperty.CreateCommand));

		public static readonly PropertyInformation RadioPhones = new ArrayPropertyInformation("RadioPhones", ExchangeVersion.Exchange2012, "PhoneNumberAttributedValue", PersonSchema.RadioPhones, new PropertyUri(PropertyUriEnum.PersonaRadioPhones), new PropertyCommand.CreatePropertyCommand(PhoneNumberAttributedValueProperty.CreateCommand));

		public static readonly PropertyInformation TelexNumbers = new ArrayPropertyInformation("TelexNumbers", ExchangeVersion.Exchange2012, "PhoneNumberAttributedValue", PersonSchema.TelexNumbers, new PropertyUri(PropertyUriEnum.PersonaTelexNumbers), new PropertyCommand.CreatePropertyCommand(PhoneNumberAttributedValueProperty.CreateCommand));

		public static readonly PropertyInformation TTYTDDPhoneNumbers = new ArrayPropertyInformation("TTYTDDPhoneNumbers", ExchangeVersion.Exchange2012, "PhoneNumberAttributedValue", PersonSchema.TtyTddPhoneNumbers, new PropertyUri(PropertyUriEnum.PersonaTTYTDDPhoneNumbers), new PropertyCommand.CreatePropertyCommand(PhoneNumberAttributedValueProperty.CreateCommand));

		public static readonly PropertyInformation WorkFaxes = new ArrayPropertyInformation("WorkFaxes", ExchangeVersion.Exchange2012, "PhoneNumberAttributedValue", PersonSchema.WorkFaxes, new PropertyUri(PropertyUriEnum.PersonaWorkFaxes), new PropertyCommand.CreatePropertyCommand(PhoneNumberAttributedValueProperty.CreateCommand));

		public static readonly PropertyInformation Emails1 = new ArrayPropertyInformation("Emails1", ExchangeVersion.Exchange2012, "EmailAddressAttributedValue", PersonSchema.Emails1, new PropertyUri(PropertyUriEnum.PersonaEmails1), new PropertyCommand.CreatePropertyCommand(EmailAddressAttributedValueProperty.CreateCommand));

		public static readonly PropertyInformation Emails2 = new ArrayPropertyInformation("Emails2", ExchangeVersion.Exchange2012, "EmailAddressAttributedValue", PersonSchema.Emails2, new PropertyUri(PropertyUriEnum.PersonaEmails2), new PropertyCommand.CreatePropertyCommand(EmailAddressAttributedValueProperty.CreateCommand));

		public static readonly PropertyInformation Emails3 = new ArrayPropertyInformation("Emails3", ExchangeVersion.Exchange2012, "EmailAddressAttributedValue", PersonSchema.Emails3, new PropertyUri(PropertyUriEnum.PersonaEmails3), new PropertyCommand.CreatePropertyCommand(EmailAddressAttributedValueProperty.CreateCommand));

		public static readonly PropertyInformation BusinessHomePages = new ArrayPropertyInformation("BusinessHomePages", ExchangeVersion.Exchange2012, "StringAttributedValue", PersonSchema.BusinessHomePages, new PropertyUri(PropertyUriEnum.PersonaBusinessHomePages), new PropertyCommand.CreatePropertyCommand(StringAttributedValueProperty.CreateCommand));

		public static readonly PropertyInformation Schools = new ArrayPropertyInformation("Schools", ExchangeVersion.Exchange2012, "StringAttributedValue", PersonSchema.Schools, new PropertyUri(PropertyUriEnum.PersonaSchools), new PropertyCommand.CreatePropertyCommand(StringAttributedValueProperty.CreateCommand));

		public static readonly PropertyInformation PersonalHomePages = new ArrayPropertyInformation("PersonalHomePages", ExchangeVersion.Exchange2012, "StringAttributedValue", PersonSchema.PersonalHomePages, new PropertyUri(PropertyUriEnum.PersonaPersonalHomePages), new PropertyCommand.CreatePropertyCommand(StringAttributedValueProperty.CreateCommand));

		public static readonly PropertyInformation OfficeLocations = new ArrayPropertyInformation("OfficeLocations", ExchangeVersion.Exchange2012, "StringAttributedValue", PersonSchema.OfficeLocations, new PropertyUri(PropertyUriEnum.PersonaOfficeLocations), new PropertyCommand.CreatePropertyCommand(StringAttributedValueProperty.CreateCommand));

		public static readonly PropertyInformation ImAddresses = new ArrayPropertyInformation("ImAddresses", ExchangeVersion.Exchange2012, "StringAttributedValue", PersonSchema.IMAddresses, new PropertyUri(PropertyUriEnum.PersonaImAddresses), new PropertyCommand.CreatePropertyCommand(StringAttributedValueProperty.CreateCommand));

		public static readonly PropertyInformation ImAddresses2 = new ArrayPropertyInformation("ImAddresses2", ExchangeVersion.Exchange2012, "StringAttributedValue", PersonSchema.IMAddresses2, new PropertyUri(PropertyUriEnum.PersonaImAddresses2), new PropertyCommand.CreatePropertyCommand(StringAttributedValueProperty.CreateCommand));

		public static readonly PropertyInformation ImAddresses3 = new ArrayPropertyInformation("ImAddresses3", ExchangeVersion.Exchange2012, "StringAttributedValue", PersonSchema.IMAddresses3, new PropertyUri(PropertyUriEnum.PersonaImAddresses3), new PropertyCommand.CreatePropertyCommand(StringAttributedValueProperty.CreateCommand));

		public static readonly PropertyInformation BusinessAddresses = new ArrayPropertyInformation("BusinessAddresses", ExchangeVersion.Exchange2012, "PostalAddressAttributedValue", PersonSchema.BusinessAddresses, new PropertyUri(PropertyUriEnum.PersonaBusinessAddresses), new PropertyCommand.CreatePropertyCommand(PostalAddressAttributedValueProperty.CreateCommand));

		public static readonly PropertyInformation HomeAddresses = new ArrayPropertyInformation("HomeAddresses", ExchangeVersion.Exchange2012, "PostalAddressAttributedValue", PersonSchema.HomeAddresses, new PropertyUri(PropertyUriEnum.PersonaHomeAddresses), new PropertyCommand.CreatePropertyCommand(PostalAddressAttributedValueProperty.CreateCommand));

		public static readonly PropertyInformation OtherAddresses = new ArrayPropertyInformation("OtherAddresses", ExchangeVersion.Exchange2012, "PostalAddressAttributedValue", PersonSchema.OtherAddresses, new PropertyUri(PropertyUriEnum.PersonaOtherAddresses), new PropertyCommand.CreatePropertyCommand(PostalAddressAttributedValueProperty.CreateCommand));

		public static readonly PropertyInformation Titles = new ArrayPropertyInformation("Titles", ExchangeVersion.Exchange2012, "StringAttributedValue", PersonSchema.Titles, new PropertyUri(PropertyUriEnum.PersonaTitles), new PropertyCommand.CreatePropertyCommand(StringAttributedValueProperty.CreateCommand));

		public static readonly PropertyInformation Departments = new ArrayPropertyInformation("Departments", ExchangeVersion.Exchange2012, "StringAttributedValue", PersonSchema.Departments, new PropertyUri(PropertyUriEnum.PersonaDepartments), new PropertyCommand.CreatePropertyCommand(StringAttributedValueProperty.CreateCommand));

		public static readonly PropertyInformation CompanyNames = new ArrayPropertyInformation("CompanyNames", ExchangeVersion.Exchange2012, "StringAttributedValue", PersonSchema.CompanyNames, new PropertyUri(PropertyUriEnum.PersonaCompanyNames), new PropertyCommand.CreatePropertyCommand(StringAttributedValueProperty.CreateCommand));

		public static readonly PropertyInformation Managers = new ArrayPropertyInformation("Managers", ExchangeVersion.Exchange2012, "StringAttributedValue", PersonSchema.Managers, new PropertyUri(PropertyUriEnum.PersonaManagers), new PropertyCommand.CreatePropertyCommand(StringAttributedValueProperty.CreateCommand));

		public static readonly PropertyInformation AssistantNames = new ArrayPropertyInformation("AssistantNames", ExchangeVersion.Exchange2012, "StringAttributedValue", PersonSchema.AssistantNames, new PropertyUri(PropertyUriEnum.PersonaAssistantNames), new PropertyCommand.CreatePropertyCommand(StringAttributedValueProperty.CreateCommand));

		public static readonly PropertyInformation Professions = new ArrayPropertyInformation("Professions", ExchangeVersion.Exchange2012, "StringAttributedValue", PersonSchema.Professions, new PropertyUri(PropertyUriEnum.PersonaProfessions), new PropertyCommand.CreatePropertyCommand(StringAttributedValueProperty.CreateCommand));

		public static readonly PropertyInformation SpouseNames = new ArrayPropertyInformation("SpouseNames", ExchangeVersion.Exchange2012, "StringAttributedValue", PersonSchema.SpouseNames, new PropertyUri(PropertyUriEnum.PersonaSpouseNames), new PropertyCommand.CreatePropertyCommand(StringAttributedValueProperty.CreateCommand));

		public static readonly PropertyInformation Children = new ArrayPropertyInformation("Children", ExchangeVersion.Exchange2012, "StringArrayAttributedValue", PersonSchema.Children, new PropertyUri(PropertyUriEnum.PersonaChildren), new PropertyCommand.CreatePropertyCommand(StringArrayAttributedValueProperty.CreateCommand));

		public static readonly PropertyInformation Hobbies = new ArrayPropertyInformation("Hobbies", ExchangeVersion.Exchange2012, "StringAttributedValue", PersonSchema.Hobbies, new PropertyUri(PropertyUriEnum.PersonaHobbies), new PropertyCommand.CreatePropertyCommand(StringAttributedValueProperty.CreateCommand));

		public static readonly PropertyInformation WeddingAnniversaries = new ArrayPropertyInformation("WeddingAnniversaries", ExchangeVersion.Exchange2012, "StringAttributedValue", PersonSchema.WeddingAnniversaries, new PropertyUri(PropertyUriEnum.PersonaWeddingAnniversaries), new PropertyCommand.CreatePropertyCommand(StringAttributedValueProperty.CreateCommand));

		public static readonly PropertyInformation WeddingAnniversariesLocal = new ArrayPropertyInformation("WeddingAnniversariesLocal", ExchangeVersion.Exchange2012, "StringAttributedValue", PersonSchema.WeddingAnniversariesLocal, new PropertyUri(PropertyUriEnum.PersonaWeddingAnniversariesLocal), new PropertyCommand.CreatePropertyCommand(StringAttributedValueProperty.CreateCommand));

		public static readonly PropertyInformation Birthdays = new ArrayPropertyInformation("Birthdays", ExchangeVersion.Exchange2012, "StringAttributedValue", PersonSchema.Birthdays, new PropertyUri(PropertyUriEnum.PersonaBirthdays), new PropertyCommand.CreatePropertyCommand(StringAttributedValueProperty.CreateCommand));

		public static readonly PropertyInformation BirthdaysLocal = new ArrayPropertyInformation("BirthdaysLocal", ExchangeVersion.Exchange2012, "StringAttributedValue", PersonSchema.BirthdaysLocal, new PropertyUri(PropertyUriEnum.PersonaBirthdaysLocal), new PropertyCommand.CreatePropertyCommand(StringAttributedValueProperty.CreateCommand));

		public static readonly PropertyInformation Locations = new ArrayPropertyInformation("Locations", ExchangeVersion.Exchange2012, "StringAttributedValue", PersonSchema.Locations, new PropertyUri(PropertyUriEnum.PersonaLocations), new PropertyCommand.CreatePropertyCommand(StringAttributedValueProperty.CreateCommand));

		public static readonly PropertyInformation ExtendedProperties = new ArrayPropertyInformation("ExtendedProperties", ExchangeVersion.Exchange2012, "ExtendedPropertyAttributedValue", PersonSchema.ExtendedProperties, new PropertyUri(PropertyUriEnum.PersonaExtendedProperties), new PropertyCommand.CreatePropertyCommand(ExtendedPropertyAttributedValueProperty.CreateCommand));

		public static readonly PropertyInformation ThirdPartyPhotoUrls = new ArrayPropertyInformation("ThirdPartyPhotoUrls", ExchangeVersion.Exchange2012, "StringAttributedValue", PersonSchema.ThirdPartyPhotoUrls, new PropertyUri(PropertyUriEnum.PersonaThirdPartyPhotoUrls), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand));
	}
}
