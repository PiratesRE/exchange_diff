using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class PersonaType
	{
		public ItemIdType PersonaId;

		[XmlElement("PersonaType")]
		public string PersonaType1;

		public string PersonaObjectStatus;

		public DateTime CreationTime;

		[XmlIgnore]
		public bool CreationTimeSpecified;

		[XmlArrayItem("BodyContentAttributedValue", IsNullable = false)]
		public BodyContentAttributedValueType[] Bodies;

		public string DisplayNameFirstLastSortKey;

		public string DisplayNameLastFirstSortKey;

		public string CompanyNameSortKey;

		public string HomeCitySortKey;

		public string WorkCitySortKey;

		public string DisplayNameFirstLastHeader;

		public string DisplayNameLastFirstHeader;

		public string DisplayName;

		public string DisplayNameFirstLast;

		public string DisplayNameLastFirst;

		public string FileAs;

		public string FileAsId;

		public string DisplayNamePrefix;

		public string GivenName;

		public string MiddleName;

		public string Surname;

		public string Generation;

		public string Nickname;

		public string YomiCompanyName;

		public string YomiFirstName;

		public string YomiLastName;

		public string Title;

		public string Department;

		public string CompanyName;

		public string Location;

		public EmailAddressType EmailAddress;

		[XmlArrayItem("Address", IsNullable = false)]
		public EmailAddressType[] EmailAddresses;

		public PersonaPhoneNumberType PhoneNumber;

		public string ImAddress;

		public string HomeCity;

		public string WorkCity;

		public int RelevanceScore;

		[XmlIgnore]
		public bool RelevanceScoreSpecified;

		[XmlArrayItem("FolderId", IsNullable = false)]
		public FolderIdType[] FolderIds;

		[XmlArrayItem("Attribution", IsNullable = false)]
		public PersonaAttributionType[] Attributions;

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] DisplayNames;

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] FileAses;

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] FileAsIds;

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] DisplayNamePrefixes;

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] GivenNames;

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] MiddleNames;

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] Surnames;

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] Generations;

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] Nicknames;

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] Initials;

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] YomiCompanyNames;

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] YomiFirstNames;

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] YomiLastNames;

		[XmlArrayItem("PhoneNumberAttributedValue", IsNullable = false)]
		public PhoneNumberAttributedValueType[] BusinessPhoneNumbers;

		[XmlArrayItem("PhoneNumberAttributedValue", IsNullable = false)]
		public PhoneNumberAttributedValueType[] BusinessPhoneNumbers2;

		[XmlArrayItem("PhoneNumberAttributedValue", IsNullable = false)]
		public PhoneNumberAttributedValueType[] HomePhones;

		[XmlArrayItem("PhoneNumberAttributedValue", IsNullable = false)]
		public PhoneNumberAttributedValueType[] HomePhones2;

		[XmlArrayItem("PhoneNumberAttributedValue", IsNullable = false)]
		public PhoneNumberAttributedValueType[] MobilePhones;

		[XmlArrayItem("PhoneNumberAttributedValue", IsNullable = false)]
		public PhoneNumberAttributedValueType[] MobilePhones2;

		[XmlArrayItem("PhoneNumberAttributedValue", IsNullable = false)]
		public PhoneNumberAttributedValueType[] AssistantPhoneNumbers;

		[XmlArrayItem("PhoneNumberAttributedValue", IsNullable = false)]
		public PhoneNumberAttributedValueType[] CallbackPhones;

		[XmlArrayItem("PhoneNumberAttributedValue", IsNullable = false)]
		public PhoneNumberAttributedValueType[] CarPhones;

		[XmlArrayItem("PhoneNumberAttributedValue", IsNullable = false)]
		public PhoneNumberAttributedValueType[] HomeFaxes;

		[XmlArrayItem("PhoneNumberAttributedValue", IsNullable = false)]
		public PhoneNumberAttributedValueType[] OrganizationMainPhones;

		[XmlArrayItem("PhoneNumberAttributedValue", IsNullable = false)]
		public PhoneNumberAttributedValueType[] OtherFaxes;

		[XmlArrayItem("PhoneNumberAttributedValue", IsNullable = false)]
		public PhoneNumberAttributedValueType[] OtherTelephones;

		[XmlArrayItem("PhoneNumberAttributedValue", IsNullable = false)]
		public PhoneNumberAttributedValueType[] OtherPhones2;

		[XmlArrayItem("PhoneNumberAttributedValue", IsNullable = false)]
		public PhoneNumberAttributedValueType[] Pagers;

		[XmlArrayItem("PhoneNumberAttributedValue", IsNullable = false)]
		public PhoneNumberAttributedValueType[] RadioPhones;

		[XmlArrayItem("PhoneNumberAttributedValue", IsNullable = false)]
		public PhoneNumberAttributedValueType[] TelexNumbers;

		[XmlArrayItem("PhoneNumberAttributedValue", IsNullable = false)]
		public PhoneNumberAttributedValueType[] TTYTDDPhoneNumbers;

		[XmlArrayItem("PhoneNumberAttributedValue", IsNullable = false)]
		public PhoneNumberAttributedValueType[] WorkFaxes;

		[XmlArrayItem("EmailAddressAttributedValue", IsNullable = false)]
		public EmailAddressAttributedValueType[] Emails1;

		[XmlArrayItem("EmailAddressAttributedValue", IsNullable = false)]
		public EmailAddressAttributedValueType[] Emails2;

		[XmlArrayItem("EmailAddressAttributedValue", IsNullable = false)]
		public EmailAddressAttributedValueType[] Emails3;

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] BusinessHomePages;

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] PersonalHomePages;

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] OfficeLocations;

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] ImAddresses;

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] ImAddresses2;

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] ImAddresses3;

		[XmlArrayItem("PostalAddressAttributedValue", IsNullable = false)]
		public PostalAddressAttributedValueType[] BusinessAddresses;

		[XmlArrayItem("PostalAddressAttributedValue", IsNullable = false)]
		public PostalAddressAttributedValueType[] HomeAddresses;

		[XmlArrayItem("PostalAddressAttributedValue", IsNullable = false)]
		public PostalAddressAttributedValueType[] OtherAddresses;

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] Titles;

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] Departments;

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] CompanyNames;

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] Managers;

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] AssistantNames;

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] Professions;

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] SpouseNames;

		[XmlArrayItem("StringArrayAttributedValue", IsNullable = false)]
		public StringArrayAttributedValueType[] Children;

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] Schools;

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] Hobbies;

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] WeddingAnniversaries;

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] Birthdays;

		[XmlArrayItem("StringAttributedValue", IsNullable = false)]
		public StringAttributedValueType[] Locations;

		[XmlArrayItem("ExtendedPropertyAttributedValue", IsNullable = false)]
		public ExtendedPropertyAttributedValueType[] ExtendedProperties;
	}
}
