using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class ContactSchema : Schema
	{
		static ContactSchema()
		{
			XmlElementInformation[] xmlElements = new XmlElementInformation[]
			{
				ContactSchema.FileAs,
				ContactSchema.FileAsMapping,
				ContactSchema.DisplayName,
				ContactSchema.GivenName,
				ContactSchema.Initials,
				ContactSchema.MiddleName,
				ContactSchema.Nickname,
				ContactSchema.CompleteName,
				ContactSchema.CompanyName,
				ContactSchema.EmailAddressesContainer,
				ContactSchema.EmailAddressEmailAddress1,
				ContactSchema.EmailAddressEmailAddress2,
				ContactSchema.EmailAddressEmailAddress3,
				ContactSchema.PhysicalAddressesContainer,
				ContactSchema.PhysicalAddressesBusinessContainer,
				ContactSchema.PhysicalAddressBusinessStreet,
				ContactSchema.PhysicalAddressBusinessCity,
				ContactSchema.PhysicalAddressBusinessState,
				ContactSchema.PhysicalAddressBusinessCountryOrRegion,
				ContactSchema.PhysicalAddressBusinessPostalCode,
				ContactSchema.PhysicalAddressesHomeContainer,
				ContactSchema.PhysicalAddressHomeStreet,
				ContactSchema.PhysicalAddressHomeCity,
				ContactSchema.PhysicalAddressHomeState,
				ContactSchema.PhysicalAddressHomeCountryOrRegion,
				ContactSchema.PhysicalAddressHomePostalCode,
				ContactSchema.PhysicalAddressesOtherContainer,
				ContactSchema.PhysicalAddressOtherStreet,
				ContactSchema.PhysicalAddressOtherCity,
				ContactSchema.PhysicalAddressOtherState,
				ContactSchema.PhysicalAddressOtherCountryOrRegion,
				ContactSchema.PhysicalAddressOtherPostalCode,
				ContactSchema.PhoneNumbersContainer,
				ContactSchema.PhoneNumberAssistantPhone,
				ContactSchema.PhoneNumberBusinessFax,
				ContactSchema.PhoneNumberBusinessPhone,
				ContactSchema.PhoneNumberBusinessPhone2,
				ContactSchema.PhoneNumberCallback,
				ContactSchema.PhoneNumberCarPhone,
				ContactSchema.PhoneNumberCompanyMainPhone,
				ContactSchema.PhoneNumberHomeFax,
				ContactSchema.PhoneNumberHomePhone,
				ContactSchema.PhoneNumberHomePhone2,
				ContactSchema.PhoneNumberIsdn,
				ContactSchema.PhoneNumberMobilePhone,
				ContactSchema.PhoneNumberOtherFax,
				ContactSchema.PhoneNumberOtherTelephone,
				ContactSchema.PhoneNumberPager,
				ContactSchema.PhoneNumberPrimaryPhone,
				ContactSchema.PhoneNumberRadioPhone,
				ContactSchema.PhoneNumberTelex,
				ContactSchema.PhoneNumberTtyTddPhone,
				ContactSchema.AssistantName,
				ContactSchema.Birthday,
				ContactSchema.BirthdayLocal,
				ContactSchema.BusinessHomePage,
				ContactSchema.Children,
				ContactSchema.Companies,
				ContactSchema.Department,
				ContactSchema.Generation,
				ContactSchema.ImAddressesContainer,
				ContactSchema.ImAddressImAddress1,
				ContactSchema.ImAddressImAddress2,
				ContactSchema.ImAddressImAddress3,
				ContactSchema.JobTitle,
				ContactSchema.Manager,
				ContactSchema.Mileage,
				ContactSchema.OfficeLocation,
				ContactSchema.PostalAddressIndex,
				ContactSchema.Profession,
				ContactSchema.SpouseName,
				ContactSchema.Surname,
				ContactSchema.WeddingAnniversary,
				ContactSchema.WeddingAnniversaryLocal,
				ContactSchema.HasPicture,
				ContactSchema.PersonaType
			};
			ContactSchema.schema = new ContactSchema(xmlElements);
		}

		private ContactSchema(XmlElementInformation[] xmlElements) : base(xmlElements)
		{
		}

		public static Schema GetSchema()
		{
			return ContactSchema.schema;
		}

		private static string DictionaryEntryPath(string keyValue)
		{
			if (string.IsNullOrEmpty(keyValue))
			{
				return null;
			}
			return "/Entry[@Key='" + keyValue + "']";
		}

		private static ContainerInformation CreateDictionaryEntryContainerInformation(PropertyUriEnum propertyUriEnum, string attributeKeyValue, ExchangeVersion effectiveVersion)
		{
			return new ContainerInformation(propertyUriEnum.ToString() + ContactSchema.DictionaryEntryPath(attributeKeyValue), ServiceXml.GetFullyQualifiedName(propertyUriEnum.ToString() + ContactSchema.DictionaryEntryPath(attributeKeyValue)), effectiveVersion);
		}

		private static PropertyInformation CreateDictionaryEntryPropertyInformation(DictionaryUriEnum dictionaryUriEnum, PropertyUriEnum propertyUriEnum, string attributeKeyValue, ExchangeVersion effectiveVersion, PropertyDefinition[] propertyDefinitions, PropertyCommand.CreatePropertyCommand createPropertyCommand)
		{
			return new PropertyInformation(propertyUriEnum.ToString(), ServiceXml.GetFullyQualifiedName(propertyUriEnum.ToString() + ContactSchema.DictionaryEntryPath(attributeKeyValue)), ServiceXml.DefaultNamespaceUri, effectiveVersion, propertyDefinitions, new DictionaryPropertyUri(dictionaryUriEnum, attributeKeyValue), createPropertyCommand, PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsDeleteUpdateCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand | PropertyInformationAttributes.ImplementsToXmlForPropertyBagCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand | PropertyInformationAttributes.ImplementsToServiceObjectForPropertyBagCommand);
		}

		private static PropertyInformation CreatePhysicalAddressPropertyInformation(string attributeKeyValue, string xmlElementName, DictionaryUriEnum dictionaryUriEnum, ExchangeVersion effectiveVersion, PropertyDefinition propertyDefinition, PropertyCommand.CreatePropertyCommand createPropertyCommand)
		{
			return new PropertyInformation(PropertyUriEnum.PhysicalAddresses.ToString(), ServiceXml.GetFullyQualifiedName(PropertyUriEnum.PhysicalAddresses.ToString() + ContactSchema.DictionaryEntryPath(attributeKeyValue) + "/" + xmlElementName), ServiceXml.DefaultNamespaceUri, effectiveVersion, new PropertyDefinition[]
			{
				propertyDefinition
			}, new DictionaryPropertyUri(dictionaryUriEnum, attributeKeyValue), createPropertyCommand, PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsDeleteUpdateCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand | PropertyInformationAttributes.ImplementsToXmlForPropertyBagCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand | PropertyInformationAttributes.ImplementsToServiceObjectForPropertyBagCommand);
		}

		private static PropertyInformation CreatePhonePropertyInformation(string attributeKeyValue, ExchangeVersion effectiveVersion, PropertyDefinition propertyDefinition)
		{
			return ContactSchema.CreateDictionaryEntryPropertyInformation(DictionaryUriEnum.PhoneNumber, PropertyUriEnum.PhoneNumbers, attributeKeyValue, effectiveVersion, new PropertyDefinition[]
			{
				propertyDefinition
			}, new PropertyCommand.CreatePropertyCommand(ContactDictionaryEntryProperty.CreateCommandForPhoneNumbers));
		}

		private static PropertyInformation CreateImAddressPropertyInformation(string attributeKeyValue, ExchangeVersion effectiveVersion, PropertyDefinition propertyDefinition)
		{
			return ContactSchema.CreateDictionaryEntryPropertyInformation(DictionaryUriEnum.ImAddress, PropertyUriEnum.ImAddresses, attributeKeyValue, effectiveVersion, new PropertyDefinition[]
			{
				propertyDefinition
			}, new PropertyCommand.CreatePropertyCommand(ContactDictionaryEntryProperty.CreateCommandForImAddresses));
		}

		private static PropertyInformation CreateEmailAddressPropertyInformation(string attributeKeyValue, ExchangeVersion effectiveVersion, PropertyDefinition propertyDefinition)
		{
			return ContactSchema.CreateDictionaryEntryPropertyInformation(DictionaryUriEnum.EmailAddress, PropertyUriEnum.EmailAddresses, attributeKeyValue, effectiveVersion, new PropertyDefinition[]
			{
				propertyDefinition
			}, new PropertyCommand.CreatePropertyCommand(ContactDictionaryEntryProperty.CreateCommandForEmailAddresses));
		}

		private const string BusinessValue = "Business";

		private const string HomeValue = "Home";

		private const string OtherValue = "Other";

		private const string StreetValue = "Street";

		private const string CityValue = "City";

		private const string CountryOrRegionValue = "CountryOrRegion";

		private const string StateValue = "State";

		private const string PostalCodeValue = "PostalCode";

		private static Schema schema;

		public static readonly PropertyInformation AssistantName = new PropertyInformation(PropertyUriEnum.AssistantName, ExchangeVersion.Exchange2007, ContactSchema.AssistantName, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsDeleteUpdateCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand | PropertyInformationAttributes.ImplementsToXmlForPropertyBagCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand | PropertyInformationAttributes.ImplementsToServiceObjectForPropertyBagCommand);

		public static readonly PropertyInformation Birthday = new PropertyInformation(PropertyUriEnum.Birthday, ExchangeVersion.Exchange2007, ContactSchema.Birthday, new PropertyCommand.CreatePropertyCommand(DateTimeProperty.CreateCommand), PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsDeleteUpdateCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand | PropertyInformationAttributes.ImplementsToXmlForPropertyBagCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand | PropertyInformationAttributes.ImplementsToServiceObjectForPropertyBagCommand);

		public static readonly PropertyInformation BirthdayLocal = new PropertyInformation(PropertyUriEnum.BirthdayLocal, ExchangeVersion.Exchange2012, ContactSchema.BirthdayLocal, new PropertyCommand.CreatePropertyCommand(DateTimeProperty.CreateCommand), PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsDeleteUpdateCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand | PropertyInformationAttributes.ImplementsToXmlForPropertyBagCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand | PropertyInformationAttributes.ImplementsToServiceObjectForPropertyBagCommand);

		public static readonly PropertyInformation BusinessHomePage = new PropertyInformation(PropertyUriEnum.BusinessHomePage, ExchangeVersion.Exchange2007, ContactSchema.BusinessHomePage, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsDeleteUpdateCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand | PropertyInformationAttributes.ImplementsToXmlForPropertyBagCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand | PropertyInformationAttributes.ImplementsToServiceObjectForPropertyBagCommand);

		public static readonly PropertyInformation Children = new ArrayPropertyInformation(PropertyUriEnum.Children.ToString(), ExchangeVersion.Exchange2007, "String", ContactSchema.Children, new PropertyUri(PropertyUriEnum.Children), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand));

		public static readonly PropertyInformation Companies = new ArrayPropertyInformation(PropertyUriEnum.Companies.ToString(), ExchangeVersion.Exchange2007, "String", ContactSchema.Companies, new PropertyUri(PropertyUriEnum.Companies), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand));

		public static readonly PropertyInformation CompanyName = new PropertyInformation(PropertyUriEnum.CompanyName, ExchangeVersion.Exchange2007, ContactSchema.CompanyName, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsDeleteUpdateCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand | PropertyInformationAttributes.ImplementsToXmlForPropertyBagCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand | PropertyInformationAttributes.ImplementsToServiceObjectForPropertyBagCommand);

		public static readonly PropertyInformation CompleteName = new PropertyInformation(PropertyUriEnum.CompleteName.ToString(), ServiceXml.GetFullyQualifiedName(PropertyUriEnum.CompleteName.ToString()), ServiceXml.DefaultNamespaceUri, ExchangeVersion.Exchange2007, new PropertyDefinition[]
		{
			ContactSchema.DisplayNamePrefix,
			ContactSchema.GivenName,
			ContactSchema.MiddleName,
			ContactSchema.Surname,
			ContactSchema.Generation,
			ContactSchema.Initials,
			StoreObjectSchema.DisplayName,
			ContactSchema.Nickname,
			ContactSchema.YomiFirstName,
			ContactSchema.YomiLastName
		}, new PropertyUri(PropertyUriEnum.CompleteName), new PropertyCommand.CreatePropertyCommand(CompleteNameProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation DisplayName = new PropertyInformation(PropertyUriEnum.DisplayName, ExchangeVersion.Exchange2007, StoreObjectSchema.DisplayName, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsDeleteUpdateCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand | PropertyInformationAttributes.ImplementsToXmlForPropertyBagCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand | PropertyInformationAttributes.ImplementsToServiceObjectForPropertyBagCommand);

		public static readonly PropertyInformation Department = new PropertyInformation(PropertyUriEnum.Department, ExchangeVersion.Exchange2007, ContactSchema.Department, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsDeleteUpdateCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand | PropertyInformationAttributes.ImplementsToXmlForPropertyBagCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand | PropertyInformationAttributes.ImplementsToServiceObjectForPropertyBagCommand);

		public static readonly ContainerInformation EmailAddressesContainer = ContactSchema.CreateDictionaryEntryContainerInformation(PropertyUriEnum.EmailAddresses, null, ExchangeVersion.Exchange2007);

		public static readonly PropertyInformation EmailAddressEmailAddress1 = ContactSchema.CreateEmailAddressPropertyInformation("EmailAddress1", ExchangeVersion.Exchange2007, ContactSchema.Email1EmailAddress);

		public static readonly PropertyInformation EmailAddressEmailAddress2 = ContactSchema.CreateEmailAddressPropertyInformation("EmailAddress2", ExchangeVersion.Exchange2007, ContactSchema.Email2EmailAddress);

		public static readonly PropertyInformation EmailAddressEmailAddress3 = ContactSchema.CreateEmailAddressPropertyInformation("EmailAddress3", ExchangeVersion.Exchange2007, ContactSchema.Email3EmailAddress);

		public static readonly PropertyInformation FileAs = new PropertyInformation(PropertyUriEnum.FileAs, ExchangeVersion.Exchange2007, ContactBaseSchema.FileAs, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsDeleteUpdateCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand | PropertyInformationAttributes.ImplementsToXmlForPropertyBagCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand | PropertyInformationAttributes.ImplementsToServiceObjectForPropertyBagCommand);

		public static readonly PropertyInformation FileAsMapping = new PropertyInformation(PropertyUriEnum.FileAsMapping, ExchangeVersion.Exchange2007, ContactSchema.FileAsId, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommandForDoNonReturnNonRepresentableProperty), PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsDeleteUpdateCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand | PropertyInformationAttributes.ImplementsToXmlForPropertyBagCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand | PropertyInformationAttributes.ImplementsToServiceObjectForPropertyBagCommand);

		public static readonly PropertyInformation Generation = new PropertyInformation(PropertyUriEnum.Generation, ExchangeVersion.Exchange2007, ContactSchema.Generation, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsDeleteUpdateCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand | PropertyInformationAttributes.ImplementsToXmlForPropertyBagCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand | PropertyInformationAttributes.ImplementsToServiceObjectForPropertyBagCommand);

		public static readonly PropertyInformation GivenName = new PropertyInformation(PropertyUriEnum.GivenName, ExchangeVersion.Exchange2007, ContactSchema.GivenName, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsDeleteUpdateCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand | PropertyInformationAttributes.ImplementsToXmlForPropertyBagCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand | PropertyInformationAttributes.ImplementsToServiceObjectForPropertyBagCommand);

		public static readonly ContainerInformation ImAddressesContainer = ContactSchema.CreateDictionaryEntryContainerInformation(PropertyUriEnum.ImAddresses, null, ExchangeVersion.Exchange2007);

		public static readonly PropertyInformation ImAddressImAddress1 = ContactSchema.CreateImAddressPropertyInformation("ImAddress1", ExchangeVersion.Exchange2007, ContactSchema.IMAddress);

		public static readonly PropertyInformation ImAddressImAddress2 = ContactSchema.CreateImAddressPropertyInformation("ImAddress2", ExchangeVersion.Exchange2007, ContactSchema.IMAddress2);

		public static readonly PropertyInformation ImAddressImAddress3 = ContactSchema.CreateImAddressPropertyInformation("ImAddress3", ExchangeVersion.Exchange2007, ContactSchema.IMAddress3);

		public static readonly PropertyInformation Initials = new PropertyInformation(PropertyUriEnum.Initials, ExchangeVersion.Exchange2007, ContactSchema.Initials, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsDeleteUpdateCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand | PropertyInformationAttributes.ImplementsToXmlForPropertyBagCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand | PropertyInformationAttributes.ImplementsToServiceObjectForPropertyBagCommand);

		public static readonly PropertyInformation Mileage = new PropertyInformation(PropertyUriEnum.Mileage, ExchangeVersion.Exchange2007, ContactSchema.Mileage, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsDeleteUpdateCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand | PropertyInformationAttributes.ImplementsToXmlForPropertyBagCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand | PropertyInformationAttributes.ImplementsToServiceObjectForPropertyBagCommand);

		public static readonly PropertyInformation JobTitle = new PropertyInformation(PropertyUriEnum.JobTitle, ExchangeVersion.Exchange2007, ContactSchema.Title, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsDeleteUpdateCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand | PropertyInformationAttributes.ImplementsToXmlForPropertyBagCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand | PropertyInformationAttributes.ImplementsToServiceObjectForPropertyBagCommand);

		public static readonly PropertyInformation PostalAddressIndex = new PropertyInformation(PropertyUriEnum.PostalAddressIndex, ExchangeVersion.Exchange2007, ContactSchema.PostalAddressId, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsDeleteUpdateCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand | PropertyInformationAttributes.ImplementsToXmlForPropertyBagCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand | PropertyInformationAttributes.ImplementsToServiceObjectForPropertyBagCommand);

		public static readonly PropertyInformation Manager = new PropertyInformation(PropertyUriEnum.Manager, ExchangeVersion.Exchange2007, ContactSchema.Manager, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsDeleteUpdateCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand | PropertyInformationAttributes.ImplementsToXmlForPropertyBagCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand | PropertyInformationAttributes.ImplementsToServiceObjectForPropertyBagCommand);

		public static readonly PropertyInformation MiddleName = new PropertyInformation(PropertyUriEnum.MiddleName, ExchangeVersion.Exchange2007, ContactSchema.MiddleName, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsDeleteUpdateCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand | PropertyInformationAttributes.ImplementsToXmlForPropertyBagCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand | PropertyInformationAttributes.ImplementsToServiceObjectForPropertyBagCommand);

		public static readonly PropertyInformation Nickname = new PropertyInformation(PropertyUriEnum.Nickname, ExchangeVersion.Exchange2007, ContactSchema.Nickname, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsDeleteUpdateCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand | PropertyInformationAttributes.ImplementsToXmlForPropertyBagCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand | PropertyInformationAttributes.ImplementsToServiceObjectForPropertyBagCommand);

		public static readonly PropertyInformation OfficeLocation = new PropertyInformation(PropertyUriEnum.OfficeLocation, ExchangeVersion.Exchange2007, ContactSchema.OfficeLocation, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsDeleteUpdateCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand | PropertyInformationAttributes.ImplementsToXmlForPropertyBagCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand | PropertyInformationAttributes.ImplementsToServiceObjectForPropertyBagCommand);

		public static readonly PropertyInformation PersonaType = new PropertyInformation(PropertyUriEnum.PersonaType, ExchangeVersion.Exchange2012, ContactSchema.PersonType, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly ContainerInformation PhoneNumbersContainer = ContactSchema.CreateDictionaryEntryContainerInformation(PropertyUriEnum.PhoneNumbers, null, ExchangeVersion.Exchange2007);

		public static readonly PropertyInformation PhoneNumberAssistantPhone = ContactSchema.CreatePhonePropertyInformation("AssistantPhone", ExchangeVersion.Exchange2007, ContactSchema.AssistantPhoneNumber);

		public static readonly PropertyInformation PhoneNumberBusinessFax = ContactSchema.CreatePhonePropertyInformation("BusinessFax", ExchangeVersion.Exchange2007, ContactSchema.WorkFax);

		public static readonly PropertyInformation PhoneNumberBusinessPhone = ContactSchema.CreatePhonePropertyInformation("BusinessPhone", ExchangeVersion.Exchange2007, ContactSchema.BusinessPhoneNumber);

		public static readonly PropertyInformation PhoneNumberBusinessPhone2 = ContactSchema.CreatePhonePropertyInformation("BusinessPhone2", ExchangeVersion.Exchange2007, ContactSchema.BusinessPhoneNumber2);

		public static readonly PropertyInformation PhoneNumberCallback = ContactSchema.CreatePhonePropertyInformation("Callback", ExchangeVersion.Exchange2007, ContactSchema.CallbackPhone);

		public static readonly PropertyInformation PhoneNumberCarPhone = ContactSchema.CreatePhonePropertyInformation("CarPhone", ExchangeVersion.Exchange2007, ContactSchema.CarPhone);

		public static readonly PropertyInformation PhoneNumberCompanyMainPhone = ContactSchema.CreatePhonePropertyInformation("CompanyMainPhone", ExchangeVersion.Exchange2007, ContactSchema.OrganizationMainPhone);

		public static readonly PropertyInformation PhoneNumberHomeFax = ContactSchema.CreatePhonePropertyInformation("HomeFax", ExchangeVersion.Exchange2007, ContactSchema.HomeFax);

		public static readonly PropertyInformation PhoneNumberHomePhone = ContactSchema.CreatePhonePropertyInformation("HomePhone", ExchangeVersion.Exchange2007, ContactSchema.HomePhone);

		public static readonly PropertyInformation PhoneNumberHomePhone2 = ContactSchema.CreatePhonePropertyInformation("HomePhone2", ExchangeVersion.Exchange2007, ContactSchema.HomePhone2);

		public static readonly PropertyInformation PhoneNumberIsdn = ContactSchema.CreatePhonePropertyInformation("Isdn", ExchangeVersion.Exchange2007, ContactSchema.InternationalIsdnNumber);

		public static readonly PropertyInformation PhoneNumberMobilePhone = ContactSchema.CreatePhonePropertyInformation("MobilePhone", ExchangeVersion.Exchange2007, ContactSchema.MobilePhone);

		public static readonly PropertyInformation PhoneNumberOtherTelephone = ContactSchema.CreatePhonePropertyInformation("OtherTelephone", ExchangeVersion.Exchange2007, ContactSchema.OtherTelephone);

		public static readonly PropertyInformation PhoneNumberOtherFax = ContactSchema.CreatePhonePropertyInformation("OtherFax", ExchangeVersion.Exchange2007, ContactSchema.OtherFax);

		public static readonly PropertyInformation PhoneNumberPager = ContactSchema.CreatePhonePropertyInformation("Pager", ExchangeVersion.Exchange2007, ContactSchema.Pager);

		public static readonly PropertyInformation PhoneNumberPrimaryPhone = ContactSchema.CreatePhonePropertyInformation("PrimaryPhone", ExchangeVersion.Exchange2007, ContactSchema.PrimaryTelephoneNumber);

		public static readonly PropertyInformation PhoneNumberRadioPhone = ContactSchema.CreatePhonePropertyInformation("RadioPhone", ExchangeVersion.Exchange2007, ContactSchema.RadioPhone);

		public static readonly PropertyInformation PhoneNumberTelex = ContactSchema.CreatePhonePropertyInformation("Telex", ExchangeVersion.Exchange2007, ContactSchema.TelexNumber);

		public static readonly PropertyInformation PhoneNumberTtyTddPhone = ContactSchema.CreatePhonePropertyInformation("TtyTddPhone", ExchangeVersion.Exchange2007, ContactSchema.TtyTddPhoneNumber);

		public static readonly ContainerInformation PhysicalAddressesContainer = ContactSchema.CreateDictionaryEntryContainerInformation(PropertyUriEnum.PhysicalAddresses, null, ExchangeVersion.Exchange2007);

		public static readonly ContainerInformation PhysicalAddressesBusinessContainer = ContactSchema.CreateDictionaryEntryContainerInformation(PropertyUriEnum.PhysicalAddresses, "Business", ExchangeVersion.Exchange2007);

		public static readonly PropertyInformation PhysicalAddressBusinessStreet = ContactSchema.CreatePhysicalAddressPropertyInformation("Business", "Street", DictionaryUriEnum.PhysicalAddressStreet, ExchangeVersion.Exchange2007, ContactSchema.WorkAddressStreet, new PropertyCommand.CreatePropertyCommand(ContactDictionaryEntryNestedProperty.CreateCommandForStreet));

		public static readonly PropertyInformation PhysicalAddressBusinessCity = ContactSchema.CreatePhysicalAddressPropertyInformation("Business", "City", DictionaryUriEnum.PhysicalAddressCity, ExchangeVersion.Exchange2007, ContactSchema.WorkAddressCity, new PropertyCommand.CreatePropertyCommand(ContactDictionaryEntryNestedProperty.CreateCommandForCity));

		public static readonly PropertyInformation PhysicalAddressBusinessState = ContactSchema.CreatePhysicalAddressPropertyInformation("Business", "State", DictionaryUriEnum.PhysicalAddressState, ExchangeVersion.Exchange2007, ContactSchema.WorkAddressState, new PropertyCommand.CreatePropertyCommand(ContactDictionaryEntryNestedProperty.CreateCommandForState));

		public static readonly PropertyInformation PhysicalAddressBusinessCountryOrRegion = ContactSchema.CreatePhysicalAddressPropertyInformation("Business", "CountryOrRegion", DictionaryUriEnum.PhysicalAddressCountryOrRegion, ExchangeVersion.Exchange2007, ContactSchema.WorkAddressCountry, new PropertyCommand.CreatePropertyCommand(ContactDictionaryEntryNestedProperty.CreateCommandForCountryOrRegion));

		public static readonly PropertyInformation PhysicalAddressBusinessPostalCode = ContactSchema.CreatePhysicalAddressPropertyInformation("Business", "PostalCode", DictionaryUriEnum.PhysicalAddressPostalCode, ExchangeVersion.Exchange2007, ContactSchema.WorkAddressPostalCode, new PropertyCommand.CreatePropertyCommand(ContactDictionaryEntryNestedProperty.CreateCommandForPostalCode));

		public static readonly ContainerInformation PhysicalAddressesHomeContainer = ContactSchema.CreateDictionaryEntryContainerInformation(PropertyUriEnum.PhysicalAddresses, "Home", ExchangeVersion.Exchange2007);

		public static readonly PropertyInformation PhysicalAddressHomeStreet = ContactSchema.CreatePhysicalAddressPropertyInformation("Home", "Street", DictionaryUriEnum.PhysicalAddressStreet, ExchangeVersion.Exchange2007, ContactSchema.HomeStreet, new PropertyCommand.CreatePropertyCommand(ContactDictionaryEntryNestedProperty.CreateCommandForStreet));

		public static readonly PropertyInformation PhysicalAddressHomeCity = ContactSchema.CreatePhysicalAddressPropertyInformation("Home", "City", DictionaryUriEnum.PhysicalAddressCity, ExchangeVersion.Exchange2007, ContactSchema.HomeCity, new PropertyCommand.CreatePropertyCommand(ContactDictionaryEntryNestedProperty.CreateCommandForCity));

		public static readonly PropertyInformation PhysicalAddressHomeState = ContactSchema.CreatePhysicalAddressPropertyInformation("Home", "State", DictionaryUriEnum.PhysicalAddressState, ExchangeVersion.Exchange2007, ContactSchema.HomeState, new PropertyCommand.CreatePropertyCommand(ContactDictionaryEntryNestedProperty.CreateCommandForState));

		public static readonly PropertyInformation PhysicalAddressHomeCountryOrRegion = ContactSchema.CreatePhysicalAddressPropertyInformation("Home", "CountryOrRegion", DictionaryUriEnum.PhysicalAddressCountryOrRegion, ExchangeVersion.Exchange2007, ContactSchema.HomeCountry, new PropertyCommand.CreatePropertyCommand(ContactDictionaryEntryNestedProperty.CreateCommandForCountryOrRegion));

		public static readonly PropertyInformation PhysicalAddressHomePostalCode = ContactSchema.CreatePhysicalAddressPropertyInformation("Home", "PostalCode", DictionaryUriEnum.PhysicalAddressPostalCode, ExchangeVersion.Exchange2007, ContactSchema.HomePostalCode, new PropertyCommand.CreatePropertyCommand(ContactDictionaryEntryNestedProperty.CreateCommandForPostalCode));

		public static readonly ContainerInformation PhysicalAddressesOtherContainer = ContactSchema.CreateDictionaryEntryContainerInformation(PropertyUriEnum.PhysicalAddresses, "Other", ExchangeVersion.Exchange2007);

		public static readonly PropertyInformation PhysicalAddressOtherStreet = ContactSchema.CreatePhysicalAddressPropertyInformation("Other", "Street", DictionaryUriEnum.PhysicalAddressStreet, ExchangeVersion.Exchange2007, ContactSchema.OtherStreet, new PropertyCommand.CreatePropertyCommand(ContactDictionaryEntryNestedProperty.CreateCommandForStreet));

		public static readonly PropertyInformation PhysicalAddressOtherCity = ContactSchema.CreatePhysicalAddressPropertyInformation("Other", "City", DictionaryUriEnum.PhysicalAddressCity, ExchangeVersion.Exchange2007, ContactSchema.OtherCity, new PropertyCommand.CreatePropertyCommand(ContactDictionaryEntryNestedProperty.CreateCommandForCity));

		public static readonly PropertyInformation PhysicalAddressOtherState = ContactSchema.CreatePhysicalAddressPropertyInformation("Other", "State", DictionaryUriEnum.PhysicalAddressState, ExchangeVersion.Exchange2007, ContactSchema.OtherState, new PropertyCommand.CreatePropertyCommand(ContactDictionaryEntryNestedProperty.CreateCommandForState));

		public static readonly PropertyInformation PhysicalAddressOtherCountryOrRegion = ContactSchema.CreatePhysicalAddressPropertyInformation("Other", "CountryOrRegion", DictionaryUriEnum.PhysicalAddressCountryOrRegion, ExchangeVersion.Exchange2007, ContactSchema.OtherCountry, new PropertyCommand.CreatePropertyCommand(ContactDictionaryEntryNestedProperty.CreateCommandForCountryOrRegion));

		public static readonly PropertyInformation PhysicalAddressOtherPostalCode = ContactSchema.CreatePhysicalAddressPropertyInformation("Other", "PostalCode", DictionaryUriEnum.PhysicalAddressPostalCode, ExchangeVersion.Exchange2007, ContactSchema.OtherPostalCode, new PropertyCommand.CreatePropertyCommand(ContactDictionaryEntryNestedProperty.CreateCommandForPostalCode));

		public static readonly PropertyInformation Profession = new PropertyInformation(PropertyUriEnum.Profession, ExchangeVersion.Exchange2007, ContactSchema.Profession, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsDeleteUpdateCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand | PropertyInformationAttributes.ImplementsToXmlForPropertyBagCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand | PropertyInformationAttributes.ImplementsToServiceObjectForPropertyBagCommand);

		public static readonly PropertyInformation SpouseName = new PropertyInformation(PropertyUriEnum.SpouseName, ExchangeVersion.Exchange2007, ContactSchema.SpouseName, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsDeleteUpdateCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand | PropertyInformationAttributes.ImplementsToXmlForPropertyBagCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand | PropertyInformationAttributes.ImplementsToServiceObjectForPropertyBagCommand);

		public static readonly PropertyInformation Surname = new PropertyInformation(PropertyUriEnum.Surname, ExchangeVersion.Exchange2007, ContactSchema.Surname, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsDeleteUpdateCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand | PropertyInformationAttributes.ImplementsToXmlForPropertyBagCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand | PropertyInformationAttributes.ImplementsToServiceObjectForPropertyBagCommand);

		public static readonly PropertyInformation WeddingAnniversary = new PropertyInformation(PropertyUriEnum.WeddingAnniversary, ExchangeVersion.Exchange2007, ContactSchema.WeddingAnniversary, new PropertyCommand.CreatePropertyCommand(DateTimeProperty.CreateCommand), PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsDeleteUpdateCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand | PropertyInformationAttributes.ImplementsToXmlForPropertyBagCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand | PropertyInformationAttributes.ImplementsToServiceObjectForPropertyBagCommand);

		public static readonly PropertyInformation WeddingAnniversaryLocal = new PropertyInformation(PropertyUriEnum.WeddingAnniversaryLocal, ExchangeVersion.Exchange2012, ContactSchema.WeddingAnniversaryLocal, new PropertyCommand.CreatePropertyCommand(DateTimeProperty.CreateCommand), PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsDeleteUpdateCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand | PropertyInformationAttributes.ImplementsToXmlForPropertyBagCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand | PropertyInformationAttributes.ImplementsToServiceObjectForPropertyBagCommand);

		public static PropertyDefinition HasPicturePropertyDef = GuidIdPropertyDefinition.CreateCustom("hasPicture", typeof(bool), new Guid("{00062004-0000-0000-C000-000000000046}"), 32789, PropertyFlags.None);

		public static readonly PropertyInformation HasPicture = new PropertyInformation(PropertyUriEnum.HasPicture, ExchangeVersion.Exchange2010, ContactSchema.HasPicturePropertyDef, new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);
	}
}
