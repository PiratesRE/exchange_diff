using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class ContactItemType : ItemType
	{
		public string FileAs;

		public FileAsMappingType FileAsMapping;

		[XmlIgnore]
		public bool FileAsMappingSpecified;

		public string DisplayName;

		public string GivenName;

		public string Initials;

		public string MiddleName;

		public string Nickname;

		public CompleteNameType CompleteName;

		public string CompanyName;

		[XmlArrayItem("Entry", IsNullable = false)]
		public EmailAddressDictionaryEntryType[] EmailAddresses;

		[XmlArrayItem("Entry", IsNullable = false)]
		public PhysicalAddressDictionaryEntryType[] PhysicalAddresses;

		[XmlArrayItem("Entry", IsNullable = false)]
		public PhoneNumberDictionaryEntryType[] PhoneNumbers;

		public string AssistantName;

		public DateTime Birthday;

		[XmlIgnore]
		public bool BirthdaySpecified;

		[XmlElement(DataType = "anyURI")]
		public string BusinessHomePage;

		[XmlArrayItem("String", IsNullable = false)]
		public string[] Children;

		[XmlArrayItem("String", IsNullable = false)]
		public string[] Companies;

		public ContactSourceType ContactSource;

		[XmlIgnore]
		public bool ContactSourceSpecified;

		public string Department;

		public string Generation;

		[XmlArrayItem("Entry", IsNullable = false)]
		public ImAddressDictionaryEntryType[] ImAddresses;

		public string JobTitle;

		public string Manager;

		public string Mileage;

		public string OfficeLocation;

		public PhysicalAddressIndexType PostalAddressIndex;

		[XmlIgnore]
		public bool PostalAddressIndexSpecified;

		public string Profession;

		public string SpouseName;

		public string Surname;

		public DateTime WeddingAnniversary;

		[XmlIgnore]
		public bool WeddingAnniversarySpecified;

		public bool HasPicture;

		[XmlIgnore]
		public bool HasPictureSpecified;

		public string PhoneticFullName;

		public string PhoneticFirstName;

		public string PhoneticLastName;

		public string Alias;

		public string Notes;

		[XmlElement(DataType = "base64Binary")]
		public byte[] Photo;

		[XmlArrayItem("Base64Binary", DataType = "base64Binary", IsNullable = false)]
		public byte[][] UserSMIMECertificate;

		[XmlArrayItem("Base64Binary", DataType = "base64Binary", IsNullable = false)]
		public byte[][] MSExchangeCertificate;

		public string DirectoryId;

		public SingleRecipientType ManagerMailbox;

		[XmlArrayItem("Mailbox", IsNullable = false)]
		public EmailAddressType[] DirectReports;
	}
}
