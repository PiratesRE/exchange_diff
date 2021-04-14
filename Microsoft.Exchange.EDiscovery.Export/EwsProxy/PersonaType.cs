using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public class PersonaType
	{
		public ItemIdType PersonaId
		{
			get
			{
				return this.personaIdField;
			}
			set
			{
				this.personaIdField = value;
			}
		}

		[XmlElement("PersonaType")]
		public string PersonaType1
		{
			get
			{
				return this.personaType1Field;
			}
			set
			{
				this.personaType1Field = value;
			}
		}

		public string PersonaObjectStatus
		{
			get
			{
				return this.personaObjectStatusField;
			}
			set
			{
				this.personaObjectStatusField = value;
			}
		}

		public DateTime CreationTime
		{
			get
			{
				return this.creationTimeField;
			}
			set
			{
				this.creationTimeField = value;
			}
		}

		[XmlIgnore]
		public bool CreationTimeSpecified
		{
			get
			{
				return this.creationTimeFieldSpecified;
			}
			set
			{
				this.creationTimeFieldSpecified = value;
			}
		}

		[XmlArrayItem("BodyContentAttributedValue", IsNullable = false)]
		public BodyContentAttributedValueType[] Bodies
		{
			get
			{
				return this.bodiesField;
			}
			set
			{
				this.bodiesField = value;
			}
		}

		public string DisplayNameFirstLastSortKey
		{
			get
			{
				return this.displayNameFirstLastSortKeyField;
			}
			set
			{
				this.displayNameFirstLastSortKeyField = value;
			}
		}

		public string DisplayNameLastFirstSortKey
		{
			get
			{
				return this.displayNameLastFirstSortKeyField;
			}
			set
			{
				this.displayNameLastFirstSortKeyField = value;
			}
		}

		public string CompanyNameSortKey
		{
			get
			{
				return this.companyNameSortKeyField;
			}
			set
			{
				this.companyNameSortKeyField = value;
			}
		}

		public string HomeCitySortKey
		{
			get
			{
				return this.homeCitySortKeyField;
			}
			set
			{
				this.homeCitySortKeyField = value;
			}
		}

		public string WorkCitySortKey
		{
			get
			{
				return this.workCitySortKeyField;
			}
			set
			{
				this.workCitySortKeyField = value;
			}
		}

		public string DisplayNameFirstLastHeader
		{
			get
			{
				return this.displayNameFirstLastHeaderField;
			}
			set
			{
				this.displayNameFirstLastHeaderField = value;
			}
		}

		public string DisplayNameLastFirstHeader
		{
			get
			{
				return this.displayNameLastFirstHeaderField;
			}
			set
			{
				this.displayNameLastFirstHeaderField = value;
			}
		}

		public string DisplayName
		{
			get
			{
				return this.displayNameField;
			}
			set
			{
				this.displayNameField = value;
			}
		}

		public string DisplayNameFirstLast
		{
			get
			{
				return this.displayNameFirstLastField;
			}
			set
			{
				this.displayNameFirstLastField = value;
			}
		}

		public string DisplayNameLastFirst
		{
			get
			{
				return this.displayNameLastFirstField;
			}
			set
			{
				this.displayNameLastFirstField = value;
			}
		}

		public string FileAs
		{
			get
			{
				return this.fileAsField;
			}
			set
			{
				this.fileAsField = value;
			}
		}

		public string FileAsId
		{
			get
			{
				return this.fileAsIdField;
			}
			set
			{
				this.fileAsIdField = value;
			}
		}

		public string DisplayNamePrefix
		{
			get
			{
				return this.displayNamePrefixField;
			}
			set
			{
				this.displayNamePrefixField = value;
			}
		}

		public string GivenName
		{
			get
			{
				return this.givenNameField;
			}
			set
			{
				this.givenNameField = value;
			}
		}

		public string MiddleName
		{
			get
			{
				return this.middleNameField;
			}
			set
			{
				this.middleNameField = value;
			}
		}

		public string Surname
		{
			get
			{
				return this.surnameField;
			}
			set
			{
				this.surnameField = value;
			}
		}

		public string Generation
		{
			get
			{
				return this.generationField;
			}
			set
			{
				this.generationField = value;
			}
		}

		public string Nickname
		{
			get
			{
				return this.nicknameField;
			}
			set
			{
				this.nicknameField = value;
			}
		}

		public string YomiCompanyName
		{
			get
			{
				return this.yomiCompanyNameField;
			}
			set
			{
				this.yomiCompanyNameField = value;
			}
		}

		public string YomiFirstName
		{
			get
			{
				return this.yomiFirstNameField;
			}
			set
			{
				this.yomiFirstNameField = value;
			}
		}

		public string YomiLastName
		{
			get
			{
				return this.yomiLastNameField;
			}
			set
			{
				this.yomiLastNameField = value;
			}
		}

		public string Title
		{
			get
			{
				return this.titleField;
			}
			set
			{
				this.titleField = value;
			}
		}

		public string Department
		{
			get
			{
				return this.departmentField;
			}
			set
			{
				this.departmentField = value;
			}
		}

		public string CompanyName
		{
			get
			{
				return this.companyNameField;
			}
			set
			{
				this.companyNameField = value;
			}
		}

		public string Location
		{
			get
			{
				return this.locationField;
			}
			set
			{
				this.locationField = value;
			}
		}

		public EmailAddressType EmailAddress
		{
			get
			{
				return this.emailAddressField;
			}
			set
			{
				this.emailAddressField = value;
			}
		}

		[XmlArrayItem("Address", IsNullable = false)]
		public EmailAddressType[] EmailAddresses
		{
			get
			{
				return this.emailAddressesField;
			}
			set
			{
				this.emailAddressesField = value;
			}
		}

		public PersonaPhoneNumberType PhoneNumber
		{
			get
			{
				return this.phoneNumberField;
			}
			set
			{
				this.phoneNumberField = value;
			}
		}

		public string ImAddress
		{
			get
			{
				return this.imAddressField;
			}
			set
			{
				this.imAddressField = value;
			}
		}

		public string HomeCity
		{
			get
			{
				return this.homeCityField;
			}
			set
			{
				this.homeCityField = value;
			}
		}

		public string WorkCity
		{
			get
			{
				return this.workCityField;
			}
			set
			{
				this.workCityField = value;
			}
		}

		public int RelevanceScore
		{
			get
			{
				return this.relevanceScoreField;
			}
			set
			{
				this.relevanceScoreField = value;
			}
		}

		[XmlIgnore]
		public bool RelevanceScoreSpecified
		{
			get
			{
				return this.relevanceScoreFieldSpecified;
			}
			set
			{
				this.relevanceScoreFieldSpecified = value;
			}
		}

		[XmlArrayItem("FolderId", IsNullable = false)]
		public FolderIdType[] FolderIds
		{
			get
			{
				return this.folderIdsField;
			}
			set
			{
				this.folderIdsField = value;
			}
		}

		[XmlArrayItem("Attribution", IsNullable = false)]
		public PersonaAttributionType[] Attributions
		{
			get
			{
				return this.attributionsField;
			}
			set
			{
				this.attributionsField = value;
			}
		}

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] DisplayNames
		{
			get
			{
				return this.displayNamesField;
			}
			set
			{
				this.displayNamesField = value;
			}
		}

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] FileAses
		{
			get
			{
				return this.fileAsesField;
			}
			set
			{
				this.fileAsesField = value;
			}
		}

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] FileAsIds
		{
			get
			{
				return this.fileAsIdsField;
			}
			set
			{
				this.fileAsIdsField = value;
			}
		}

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] DisplayNamePrefixes
		{
			get
			{
				return this.displayNamePrefixesField;
			}
			set
			{
				this.displayNamePrefixesField = value;
			}
		}

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] GivenNames
		{
			get
			{
				return this.givenNamesField;
			}
			set
			{
				this.givenNamesField = value;
			}
		}

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] MiddleNames
		{
			get
			{
				return this.middleNamesField;
			}
			set
			{
				this.middleNamesField = value;
			}
		}

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] Surnames
		{
			get
			{
				return this.surnamesField;
			}
			set
			{
				this.surnamesField = value;
			}
		}

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] Generations
		{
			get
			{
				return this.generationsField;
			}
			set
			{
				this.generationsField = value;
			}
		}

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] Nicknames
		{
			get
			{
				return this.nicknamesField;
			}
			set
			{
				this.nicknamesField = value;
			}
		}

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] Initials
		{
			get
			{
				return this.initialsField;
			}
			set
			{
				this.initialsField = value;
			}
		}

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] YomiCompanyNames
		{
			get
			{
				return this.yomiCompanyNamesField;
			}
			set
			{
				this.yomiCompanyNamesField = value;
			}
		}

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] YomiFirstNames
		{
			get
			{
				return this.yomiFirstNamesField;
			}
			set
			{
				this.yomiFirstNamesField = value;
			}
		}

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] YomiLastNames
		{
			get
			{
				return this.yomiLastNamesField;
			}
			set
			{
				this.yomiLastNamesField = value;
			}
		}

		[XmlArrayItem("PhoneNumberAttributedValue", IsNullable = false)]
		public PhoneNumberAttributedValueType[] BusinessPhoneNumbers
		{
			get
			{
				return this.businessPhoneNumbersField;
			}
			set
			{
				this.businessPhoneNumbersField = value;
			}
		}

		[XmlArrayItem("PhoneNumberAttributedValue", IsNullable = false)]
		public PhoneNumberAttributedValueType[] BusinessPhoneNumbers2
		{
			get
			{
				return this.businessPhoneNumbers2Field;
			}
			set
			{
				this.businessPhoneNumbers2Field = value;
			}
		}

		[XmlArrayItem("PhoneNumberAttributedValue", IsNullable = false)]
		public PhoneNumberAttributedValueType[] HomePhones
		{
			get
			{
				return this.homePhonesField;
			}
			set
			{
				this.homePhonesField = value;
			}
		}

		[XmlArrayItem("PhoneNumberAttributedValue", IsNullable = false)]
		public PhoneNumberAttributedValueType[] HomePhones2
		{
			get
			{
				return this.homePhones2Field;
			}
			set
			{
				this.homePhones2Field = value;
			}
		}

		[XmlArrayItem("PhoneNumberAttributedValue", IsNullable = false)]
		public PhoneNumberAttributedValueType[] MobilePhones
		{
			get
			{
				return this.mobilePhonesField;
			}
			set
			{
				this.mobilePhonesField = value;
			}
		}

		[XmlArrayItem("PhoneNumberAttributedValue", IsNullable = false)]
		public PhoneNumberAttributedValueType[] MobilePhones2
		{
			get
			{
				return this.mobilePhones2Field;
			}
			set
			{
				this.mobilePhones2Field = value;
			}
		}

		[XmlArrayItem("PhoneNumberAttributedValue", IsNullable = false)]
		public PhoneNumberAttributedValueType[] AssistantPhoneNumbers
		{
			get
			{
				return this.assistantPhoneNumbersField;
			}
			set
			{
				this.assistantPhoneNumbersField = value;
			}
		}

		[XmlArrayItem("PhoneNumberAttributedValue", IsNullable = false)]
		public PhoneNumberAttributedValueType[] CallbackPhones
		{
			get
			{
				return this.callbackPhonesField;
			}
			set
			{
				this.callbackPhonesField = value;
			}
		}

		[XmlArrayItem("PhoneNumberAttributedValue", IsNullable = false)]
		public PhoneNumberAttributedValueType[] CarPhones
		{
			get
			{
				return this.carPhonesField;
			}
			set
			{
				this.carPhonesField = value;
			}
		}

		[XmlArrayItem("PhoneNumberAttributedValue", IsNullable = false)]
		public PhoneNumberAttributedValueType[] HomeFaxes
		{
			get
			{
				return this.homeFaxesField;
			}
			set
			{
				this.homeFaxesField = value;
			}
		}

		[XmlArrayItem("PhoneNumberAttributedValue", IsNullable = false)]
		public PhoneNumberAttributedValueType[] OrganizationMainPhones
		{
			get
			{
				return this.organizationMainPhonesField;
			}
			set
			{
				this.organizationMainPhonesField = value;
			}
		}

		[XmlArrayItem("PhoneNumberAttributedValue", IsNullable = false)]
		public PhoneNumberAttributedValueType[] OtherFaxes
		{
			get
			{
				return this.otherFaxesField;
			}
			set
			{
				this.otherFaxesField = value;
			}
		}

		[XmlArrayItem("PhoneNumberAttributedValue", IsNullable = false)]
		public PhoneNumberAttributedValueType[] OtherTelephones
		{
			get
			{
				return this.otherTelephonesField;
			}
			set
			{
				this.otherTelephonesField = value;
			}
		}

		[XmlArrayItem("PhoneNumberAttributedValue", IsNullable = false)]
		public PhoneNumberAttributedValueType[] OtherPhones2
		{
			get
			{
				return this.otherPhones2Field;
			}
			set
			{
				this.otherPhones2Field = value;
			}
		}

		[XmlArrayItem("PhoneNumberAttributedValue", IsNullable = false)]
		public PhoneNumberAttributedValueType[] Pagers
		{
			get
			{
				return this.pagersField;
			}
			set
			{
				this.pagersField = value;
			}
		}

		[XmlArrayItem("PhoneNumberAttributedValue", IsNullable = false)]
		public PhoneNumberAttributedValueType[] RadioPhones
		{
			get
			{
				return this.radioPhonesField;
			}
			set
			{
				this.radioPhonesField = value;
			}
		}

		[XmlArrayItem("PhoneNumberAttributedValue", IsNullable = false)]
		public PhoneNumberAttributedValueType[] TelexNumbers
		{
			get
			{
				return this.telexNumbersField;
			}
			set
			{
				this.telexNumbersField = value;
			}
		}

		[XmlArrayItem("PhoneNumberAttributedValue", IsNullable = false)]
		public PhoneNumberAttributedValueType[] TTYTDDPhoneNumbers
		{
			get
			{
				return this.tTYTDDPhoneNumbersField;
			}
			set
			{
				this.tTYTDDPhoneNumbersField = value;
			}
		}

		[XmlArrayItem("PhoneNumberAttributedValue", IsNullable = false)]
		public PhoneNumberAttributedValueType[] WorkFaxes
		{
			get
			{
				return this.workFaxesField;
			}
			set
			{
				this.workFaxesField = value;
			}
		}

		[XmlArrayItem("EmailAddressAttributedValue", IsNullable = false)]
		public EmailAddressAttributedValueType[] Emails1
		{
			get
			{
				return this.emails1Field;
			}
			set
			{
				this.emails1Field = value;
			}
		}

		[XmlArrayItem("EmailAddressAttributedValue", IsNullable = false)]
		public EmailAddressAttributedValueType[] Emails2
		{
			get
			{
				return this.emails2Field;
			}
			set
			{
				this.emails2Field = value;
			}
		}

		[XmlArrayItem("EmailAddressAttributedValue", IsNullable = false)]
		public EmailAddressAttributedValueType[] Emails3
		{
			get
			{
				return this.emails3Field;
			}
			set
			{
				this.emails3Field = value;
			}
		}

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] BusinessHomePages
		{
			get
			{
				return this.businessHomePagesField;
			}
			set
			{
				this.businessHomePagesField = value;
			}
		}

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] PersonalHomePages
		{
			get
			{
				return this.personalHomePagesField;
			}
			set
			{
				this.personalHomePagesField = value;
			}
		}

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] OfficeLocations
		{
			get
			{
				return this.officeLocationsField;
			}
			set
			{
				this.officeLocationsField = value;
			}
		}

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] ImAddresses
		{
			get
			{
				return this.imAddressesField;
			}
			set
			{
				this.imAddressesField = value;
			}
		}

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] ImAddresses2
		{
			get
			{
				return this.imAddresses2Field;
			}
			set
			{
				this.imAddresses2Field = value;
			}
		}

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] ImAddresses3
		{
			get
			{
				return this.imAddresses3Field;
			}
			set
			{
				this.imAddresses3Field = value;
			}
		}

		[XmlArrayItem("PostalAddressAttributedValue", IsNullable = false)]
		public PostalAddressAttributedValueType[] BusinessAddresses
		{
			get
			{
				return this.businessAddressesField;
			}
			set
			{
				this.businessAddressesField = value;
			}
		}

		[XmlArrayItem("PostalAddressAttributedValue", IsNullable = false)]
		public PostalAddressAttributedValueType[] HomeAddresses
		{
			get
			{
				return this.homeAddressesField;
			}
			set
			{
				this.homeAddressesField = value;
			}
		}

		[XmlArrayItem("PostalAddressAttributedValue", IsNullable = false)]
		public PostalAddressAttributedValueType[] OtherAddresses
		{
			get
			{
				return this.otherAddressesField;
			}
			set
			{
				this.otherAddressesField = value;
			}
		}

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] Titles
		{
			get
			{
				return this.titlesField;
			}
			set
			{
				this.titlesField = value;
			}
		}

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] Departments
		{
			get
			{
				return this.departmentsField;
			}
			set
			{
				this.departmentsField = value;
			}
		}

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] CompanyNames
		{
			get
			{
				return this.companyNamesField;
			}
			set
			{
				this.companyNamesField = value;
			}
		}

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] Managers
		{
			get
			{
				return this.managersField;
			}
			set
			{
				this.managersField = value;
			}
		}

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] AssistantNames
		{
			get
			{
				return this.assistantNamesField;
			}
			set
			{
				this.assistantNamesField = value;
			}
		}

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] Professions
		{
			get
			{
				return this.professionsField;
			}
			set
			{
				this.professionsField = value;
			}
		}

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] SpouseNames
		{
			get
			{
				return this.spouseNamesField;
			}
			set
			{
				this.spouseNamesField = value;
			}
		}

		[XmlArrayItem("StringArrayAttributedValue", IsNullable = false)]
		public StringArrayAttributedValueType[] Children
		{
			get
			{
				return this.childrenField;
			}
			set
			{
				this.childrenField = value;
			}
		}

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] Schools
		{
			get
			{
				return this.schoolsField;
			}
			set
			{
				this.schoolsField = value;
			}
		}

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] Hobbies
		{
			get
			{
				return this.hobbiesField;
			}
			set
			{
				this.hobbiesField = value;
			}
		}

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] WeddingAnniversaries
		{
			get
			{
				return this.weddingAnniversariesField;
			}
			set
			{
				this.weddingAnniversariesField = value;
			}
		}

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] Birthdays
		{
			get
			{
				return this.birthdaysField;
			}
			set
			{
				this.birthdaysField = value;
			}
		}

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] Locations
		{
			get
			{
				return this.locationsField;
			}
			set
			{
				this.locationsField = value;
			}
		}

		[XmlArrayItem("ExtendedPropertyAttributedValue", IsNullable = false)]
		public ExtendedPropertyAttributedValueType[] ExtendedProperties
		{
			get
			{
				return this.extendedPropertiesField;
			}
			set
			{
				this.extendedPropertiesField = value;
			}
		}

		private ItemIdType personaIdField;

		private string personaType1Field;

		private string personaObjectStatusField;

		private DateTime creationTimeField;

		private bool creationTimeFieldSpecified;

		private BodyContentAttributedValueType[] bodiesField;

		private string displayNameFirstLastSortKeyField;

		private string displayNameLastFirstSortKeyField;

		private string companyNameSortKeyField;

		private string homeCitySortKeyField;

		private string workCitySortKeyField;

		private string displayNameFirstLastHeaderField;

		private string displayNameLastFirstHeaderField;

		private string displayNameField;

		private string displayNameFirstLastField;

		private string displayNameLastFirstField;

		private string fileAsField;

		private string fileAsIdField;

		private string displayNamePrefixField;

		private string givenNameField;

		private string middleNameField;

		private string surnameField;

		private string generationField;

		private string nicknameField;

		private string yomiCompanyNameField;

		private string yomiFirstNameField;

		private string yomiLastNameField;

		private string titleField;

		private string departmentField;

		private string companyNameField;

		private string locationField;

		private EmailAddressType emailAddressField;

		private EmailAddressType[] emailAddressesField;

		private PersonaPhoneNumberType phoneNumberField;

		private string imAddressField;

		private string homeCityField;

		private string workCityField;

		private int relevanceScoreField;

		private bool relevanceScoreFieldSpecified;

		private FolderIdType[] folderIdsField;

		private PersonaAttributionType[] attributionsField;

		private StringAttributedValueType[] displayNamesField;

		private StringAttributedValueType[] fileAsesField;

		private StringAttributedValueType[] fileAsIdsField;

		private StringAttributedValueType[] displayNamePrefixesField;

		private StringAttributedValueType[] givenNamesField;

		private StringAttributedValueType[] middleNamesField;

		private StringAttributedValueType[] surnamesField;

		private StringAttributedValueType[] generationsField;

		private StringAttributedValueType[] nicknamesField;

		private StringAttributedValueType[] initialsField;

		private StringAttributedValueType[] yomiCompanyNamesField;

		private StringAttributedValueType[] yomiFirstNamesField;

		private StringAttributedValueType[] yomiLastNamesField;

		private PhoneNumberAttributedValueType[] businessPhoneNumbersField;

		private PhoneNumberAttributedValueType[] businessPhoneNumbers2Field;

		private PhoneNumberAttributedValueType[] homePhonesField;

		private PhoneNumberAttributedValueType[] homePhones2Field;

		private PhoneNumberAttributedValueType[] mobilePhonesField;

		private PhoneNumberAttributedValueType[] mobilePhones2Field;

		private PhoneNumberAttributedValueType[] assistantPhoneNumbersField;

		private PhoneNumberAttributedValueType[] callbackPhonesField;

		private PhoneNumberAttributedValueType[] carPhonesField;

		private PhoneNumberAttributedValueType[] homeFaxesField;

		private PhoneNumberAttributedValueType[] organizationMainPhonesField;

		private PhoneNumberAttributedValueType[] otherFaxesField;

		private PhoneNumberAttributedValueType[] otherTelephonesField;

		private PhoneNumberAttributedValueType[] otherPhones2Field;

		private PhoneNumberAttributedValueType[] pagersField;

		private PhoneNumberAttributedValueType[] radioPhonesField;

		private PhoneNumberAttributedValueType[] telexNumbersField;

		private PhoneNumberAttributedValueType[] tTYTDDPhoneNumbersField;

		private PhoneNumberAttributedValueType[] workFaxesField;

		private EmailAddressAttributedValueType[] emails1Field;

		private EmailAddressAttributedValueType[] emails2Field;

		private EmailAddressAttributedValueType[] emails3Field;

		private StringAttributedValueType[] businessHomePagesField;

		private StringAttributedValueType[] personalHomePagesField;

		private StringAttributedValueType[] officeLocationsField;

		private StringAttributedValueType[] imAddressesField;

		private StringAttributedValueType[] imAddresses2Field;

		private StringAttributedValueType[] imAddresses3Field;

		private PostalAddressAttributedValueType[] businessAddressesField;

		private PostalAddressAttributedValueType[] homeAddressesField;

		private PostalAddressAttributedValueType[] otherAddressesField;

		private StringAttributedValueType[] titlesField;

		private StringAttributedValueType[] departmentsField;

		private StringAttributedValueType[] companyNamesField;

		private StringAttributedValueType[] managersField;

		private StringAttributedValueType[] assistantNamesField;

		private StringAttributedValueType[] professionsField;

		private StringAttributedValueType[] spouseNamesField;

		private StringArrayAttributedValueType[] childrenField;

		private StringAttributedValueType[] schoolsField;

		private StringAttributedValueType[] hobbiesField;

		private StringAttributedValueType[] weddingAnniversariesField;

		private StringAttributedValueType[] birthdaysField;

		private StringAttributedValueType[] locationsField;

		private ExtendedPropertyAttributedValueType[] extendedPropertiesField;
	}
}
