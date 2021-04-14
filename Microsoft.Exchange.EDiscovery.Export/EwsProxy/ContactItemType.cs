using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class ContactItemType : ItemType
	{
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

		public FileAsMappingType FileAsMapping
		{
			get
			{
				return this.fileAsMappingField;
			}
			set
			{
				this.fileAsMappingField = value;
			}
		}

		[XmlIgnore]
		public bool FileAsMappingSpecified
		{
			get
			{
				return this.fileAsMappingFieldSpecified;
			}
			set
			{
				this.fileAsMappingFieldSpecified = value;
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

		public string Initials
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

		public CompleteNameType CompleteName
		{
			get
			{
				return this.completeNameField;
			}
			set
			{
				this.completeNameField = value;
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

		[XmlArrayItem("Entry", IsNullable = false)]
		public EmailAddressDictionaryEntryType[] EmailAddresses
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

		[XmlArrayItem("Entry", IsNullable = false)]
		public PhysicalAddressDictionaryEntryType[] PhysicalAddresses
		{
			get
			{
				return this.physicalAddressesField;
			}
			set
			{
				this.physicalAddressesField = value;
			}
		}

		[XmlArrayItem("Entry", IsNullable = false)]
		public PhoneNumberDictionaryEntryType[] PhoneNumbers
		{
			get
			{
				return this.phoneNumbersField;
			}
			set
			{
				this.phoneNumbersField = value;
			}
		}

		public string AssistantName
		{
			get
			{
				return this.assistantNameField;
			}
			set
			{
				this.assistantNameField = value;
			}
		}

		public DateTime Birthday
		{
			get
			{
				return this.birthdayField;
			}
			set
			{
				this.birthdayField = value;
			}
		}

		[XmlIgnore]
		public bool BirthdaySpecified
		{
			get
			{
				return this.birthdayFieldSpecified;
			}
			set
			{
				this.birthdayFieldSpecified = value;
			}
		}

		[XmlElement(DataType = "anyURI")]
		public string BusinessHomePage
		{
			get
			{
				return this.businessHomePageField;
			}
			set
			{
				this.businessHomePageField = value;
			}
		}

		[XmlArrayItem("String", IsNullable = false)]
		public string[] Children
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

		[XmlArrayItem("String", IsNullable = false)]
		public string[] Companies
		{
			get
			{
				return this.companiesField;
			}
			set
			{
				this.companiesField = value;
			}
		}

		public ContactSourceType ContactSource
		{
			get
			{
				return this.contactSourceField;
			}
			set
			{
				this.contactSourceField = value;
			}
		}

		[XmlIgnore]
		public bool ContactSourceSpecified
		{
			get
			{
				return this.contactSourceFieldSpecified;
			}
			set
			{
				this.contactSourceFieldSpecified = value;
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

		[XmlArrayItem("Entry", IsNullable = false)]
		public ImAddressDictionaryEntryType[] ImAddresses
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

		public string JobTitle
		{
			get
			{
				return this.jobTitleField;
			}
			set
			{
				this.jobTitleField = value;
			}
		}

		public string Manager
		{
			get
			{
				return this.managerField;
			}
			set
			{
				this.managerField = value;
			}
		}

		public string Mileage
		{
			get
			{
				return this.mileageField;
			}
			set
			{
				this.mileageField = value;
			}
		}

		public string OfficeLocation
		{
			get
			{
				return this.officeLocationField;
			}
			set
			{
				this.officeLocationField = value;
			}
		}

		public PhysicalAddressIndexType PostalAddressIndex
		{
			get
			{
				return this.postalAddressIndexField;
			}
			set
			{
				this.postalAddressIndexField = value;
			}
		}

		[XmlIgnore]
		public bool PostalAddressIndexSpecified
		{
			get
			{
				return this.postalAddressIndexFieldSpecified;
			}
			set
			{
				this.postalAddressIndexFieldSpecified = value;
			}
		}

		public string Profession
		{
			get
			{
				return this.professionField;
			}
			set
			{
				this.professionField = value;
			}
		}

		public string SpouseName
		{
			get
			{
				return this.spouseNameField;
			}
			set
			{
				this.spouseNameField = value;
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

		public DateTime WeddingAnniversary
		{
			get
			{
				return this.weddingAnniversaryField;
			}
			set
			{
				this.weddingAnniversaryField = value;
			}
		}

		[XmlIgnore]
		public bool WeddingAnniversarySpecified
		{
			get
			{
				return this.weddingAnniversaryFieldSpecified;
			}
			set
			{
				this.weddingAnniversaryFieldSpecified = value;
			}
		}

		public bool HasPicture
		{
			get
			{
				return this.hasPictureField;
			}
			set
			{
				this.hasPictureField = value;
			}
		}

		[XmlIgnore]
		public bool HasPictureSpecified
		{
			get
			{
				return this.hasPictureFieldSpecified;
			}
			set
			{
				this.hasPictureFieldSpecified = value;
			}
		}

		public string PhoneticFullName
		{
			get
			{
				return this.phoneticFullNameField;
			}
			set
			{
				this.phoneticFullNameField = value;
			}
		}

		public string PhoneticFirstName
		{
			get
			{
				return this.phoneticFirstNameField;
			}
			set
			{
				this.phoneticFirstNameField = value;
			}
		}

		public string PhoneticLastName
		{
			get
			{
				return this.phoneticLastNameField;
			}
			set
			{
				this.phoneticLastNameField = value;
			}
		}

		public string Alias
		{
			get
			{
				return this.aliasField;
			}
			set
			{
				this.aliasField = value;
			}
		}

		public string Notes
		{
			get
			{
				return this.notesField;
			}
			set
			{
				this.notesField = value;
			}
		}

		[XmlElement(DataType = "base64Binary")]
		public byte[] Photo
		{
			get
			{
				return this.photoField;
			}
			set
			{
				this.photoField = value;
			}
		}

		[XmlArrayItem("Base64Binary", DataType = "base64Binary", IsNullable = false)]
		public byte[][] UserSMIMECertificate
		{
			get
			{
				return this.userSMIMECertificateField;
			}
			set
			{
				this.userSMIMECertificateField = value;
			}
		}

		[XmlArrayItem("Base64Binary", DataType = "base64Binary", IsNullable = false)]
		public byte[][] MSExchangeCertificate
		{
			get
			{
				return this.mSExchangeCertificateField;
			}
			set
			{
				this.mSExchangeCertificateField = value;
			}
		}

		public string DirectoryId
		{
			get
			{
				return this.directoryIdField;
			}
			set
			{
				this.directoryIdField = value;
			}
		}

		public SingleRecipientType ManagerMailbox
		{
			get
			{
				return this.managerMailboxField;
			}
			set
			{
				this.managerMailboxField = value;
			}
		}

		[XmlArrayItem("Mailbox", IsNullable = false)]
		public EmailAddressType[] DirectReports
		{
			get
			{
				return this.directReportsField;
			}
			set
			{
				this.directReportsField = value;
			}
		}

		private string fileAsField;

		private FileAsMappingType fileAsMappingField;

		private bool fileAsMappingFieldSpecified;

		private string displayNameField;

		private string givenNameField;

		private string initialsField;

		private string middleNameField;

		private string nicknameField;

		private CompleteNameType completeNameField;

		private string companyNameField;

		private EmailAddressDictionaryEntryType[] emailAddressesField;

		private PhysicalAddressDictionaryEntryType[] physicalAddressesField;

		private PhoneNumberDictionaryEntryType[] phoneNumbersField;

		private string assistantNameField;

		private DateTime birthdayField;

		private bool birthdayFieldSpecified;

		private string businessHomePageField;

		private string[] childrenField;

		private string[] companiesField;

		private ContactSourceType contactSourceField;

		private bool contactSourceFieldSpecified;

		private string departmentField;

		private string generationField;

		private ImAddressDictionaryEntryType[] imAddressesField;

		private string jobTitleField;

		private string managerField;

		private string mileageField;

		private string officeLocationField;

		private PhysicalAddressIndexType postalAddressIndexField;

		private bool postalAddressIndexFieldSpecified;

		private string professionField;

		private string spouseNameField;

		private string surnameField;

		private DateTime weddingAnniversaryField;

		private bool weddingAnniversaryFieldSpecified;

		private bool hasPictureField;

		private bool hasPictureFieldSpecified;

		private string phoneticFullNameField;

		private string phoneticFirstNameField;

		private string phoneticLastNameField;

		private string aliasField;

		private string notesField;

		private byte[] photoField;

		private byte[][] userSMIMECertificateField;

		private byte[][] mSExchangeCertificateField;

		private string directoryIdField;

		private SingleRecipientType managerMailboxField;

		private EmailAddressType[] directReportsField;
	}
}
