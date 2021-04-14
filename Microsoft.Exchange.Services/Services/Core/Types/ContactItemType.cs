using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.DataConverter;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "Contact")]
	[Serializable]
	public class ContactItemType : ItemType
	{
		[DataMember(EmitDefaultValue = false, Order = 1)]
		public string FileAs
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(ContactSchema.FileAs);
			}
			set
			{
				base.PropertyBag[ContactSchema.FileAs] = value;
			}
		}

		[DataMember(Name = "FileAsMapping", EmitDefaultValue = false, Order = 2)]
		[XmlElement("FileAsMapping")]
		public string FileAsMapping
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(ContactSchema.FileAsMapping);
			}
			set
			{
				base.PropertyBag[ContactSchema.FileAsMapping] = value;
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool FileAsMappingSpecified
		{
			get
			{
				return base.PropertyBag.Contains(ContactSchema.FileAsMapping);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 3)]
		public string DisplayName
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(ContactSchema.DisplayName);
			}
			set
			{
				base.PropertyBag[ContactSchema.DisplayName] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 4)]
		public string GivenName
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(ContactSchema.GivenName);
			}
			set
			{
				base.PropertyBag[ContactSchema.GivenName] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 5)]
		public string Initials
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(ContactSchema.Initials);
			}
			set
			{
				base.PropertyBag[ContactSchema.Initials] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 6)]
		public string MiddleName
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(ContactSchema.MiddleName);
			}
			set
			{
				base.PropertyBag[ContactSchema.MiddleName] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 7)]
		public string Nickname
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(ContactSchema.Nickname);
			}
			set
			{
				base.PropertyBag[ContactSchema.Nickname] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 8)]
		public CompleteNameType CompleteName
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<CompleteNameType>(ContactSchema.CompleteName);
			}
			set
			{
				base.PropertyBag[ContactSchema.CompleteName] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 9)]
		public string CompanyName
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(ContactSchema.CompanyName);
			}
			set
			{
				base.PropertyBag[ContactSchema.CompanyName] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 10)]
		[XmlArrayItem("Entry", IsNullable = false)]
		public EmailAddressDictionaryEntryType[] EmailAddresses
		{
			get
			{
				return this.GetEmailAddresses();
			}
			set
			{
				this.SetEmailAddresses(value);
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 11)]
		[XmlArrayItem("Entry", IsNullable = false)]
		public PhysicalAddressDictionaryEntryType[] PhysicalAddresses
		{
			get
			{
				return this.GetPhysicalAddresses();
			}
			set
			{
				this.SetPhysicalAddresses(value);
			}
		}

		[XmlArrayItem("Entry", IsNullable = false)]
		[DataMember(EmitDefaultValue = false, Order = 12)]
		public PhoneNumberDictionaryEntryType[] PhoneNumbers
		{
			get
			{
				return this.GetIndexedProperties<PhoneNumberDictionaryEntryType, PhoneNumberKeyType>(ContactItemType.phoneNumberToPropInfoMap, (PhoneNumberKeyType k, string v) => new PhoneNumberDictionaryEntryType(k, v));
			}
			set
			{
				if (value != null)
				{
					for (int i = 0; i < value.Length; i++)
					{
						PhoneNumberDictionaryEntryType phoneNumberDictionaryEntryType = value[i];
						PropertyInformation property = ContactItemType.phoneNumberToPropInfoMap[phoneNumberDictionaryEntryType.Key];
						this[property] = phoneNumberDictionaryEntryType.Value;
					}
				}
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 13)]
		public string AssistantName
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(ContactSchema.AssistantName);
			}
			set
			{
				base.PropertyBag[ContactSchema.AssistantName] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 14)]
		public string Birthday
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(ContactSchema.Birthday);
			}
			set
			{
				base.PropertyBag[ContactSchema.Birthday] = value;
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool BirthdaySpecified
		{
			get
			{
				return base.PropertyBag.Contains(ContactSchema.Birthday);
			}
			set
			{
			}
		}

		[XmlElement(DataType = "anyURI")]
		[DataMember(EmitDefaultValue = false, Order = 15)]
		public string BusinessHomePage
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(ContactSchema.BusinessHomePage);
			}
			set
			{
				base.PropertyBag[ContactSchema.BusinessHomePage] = value;
			}
		}

		[XmlArrayItem("String", IsNullable = false)]
		[DataMember(EmitDefaultValue = false, Order = 16)]
		public string[] Children
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string[]>(ContactSchema.Children);
			}
			set
			{
				base.PropertyBag[ContactSchema.Children] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 17)]
		[XmlArrayItem("String", IsNullable = false)]
		public string[] Companies
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string[]>(ContactSchema.Companies);
			}
			set
			{
				base.PropertyBag[ContactSchema.Companies] = value;
			}
		}

		[XmlElement]
		[IgnoreDataMember]
		public ContactSourceType ContactSource
		{
			get
			{
				return this.contactSource;
			}
			set
			{
				this.contactSource = value;
			}
		}

		[DataMember(Name = "ContactSource", EmitDefaultValue = false, Order = 18)]
		[XmlIgnore]
		public string ContactSourceString
		{
			get
			{
				if (!this.contactSourceSpecified)
				{
					return null;
				}
				return EnumUtilities.ToString<ContactSourceType>(this.contactSource);
			}
			set
			{
				this.contactSource = EnumUtilities.Parse<ContactSourceType>(value);
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool ContactSourceSpecified
		{
			get
			{
				return this.contactSourceSpecified;
			}
			set
			{
				this.contactSourceSpecified = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 19)]
		public string Department
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(ContactSchema.Department);
			}
			set
			{
				base.PropertyBag[ContactSchema.Department] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 20)]
		public string Generation
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(ContactSchema.Generation);
			}
			set
			{
				base.PropertyBag[ContactSchema.Generation] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 21)]
		[XmlArrayItem("Entry", IsNullable = false)]
		public ImAddressDictionaryEntryType[] ImAddresses
		{
			get
			{
				return this.GetIndexedProperties<ImAddressDictionaryEntryType, ImAddressKeyType>(ContactItemType.imAddressToPropInfoMap, (ImAddressKeyType k, string v) => new ImAddressDictionaryEntryType(k, v));
			}
			set
			{
				if (value != null)
				{
					for (int i = 0; i < value.Length; i++)
					{
						ImAddressDictionaryEntryType imAddressDictionaryEntryType = value[i];
						PropertyInformation property = ContactItemType.imAddressToPropInfoMap[imAddressDictionaryEntryType.Key];
						this[property] = imAddressDictionaryEntryType.Value;
					}
				}
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 22)]
		public string JobTitle
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(ContactSchema.JobTitle);
			}
			set
			{
				base.PropertyBag[ContactSchema.JobTitle] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 23)]
		public string Manager
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(ContactSchema.Manager);
			}
			set
			{
				base.PropertyBag[ContactSchema.Manager] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 24)]
		public string Mileage
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(ContactSchema.Mileage);
			}
			set
			{
				base.PropertyBag[ContactSchema.Mileage] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 25)]
		public string OfficeLocation
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(ContactSchema.OfficeLocation);
			}
			set
			{
				base.PropertyBag[ContactSchema.OfficeLocation] = value;
			}
		}

		[IgnoreDataMember]
		[XmlElement]
		public PhysicalAddressIndexType PostalAddressIndex
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<PhysicalAddressIndexType>(ContactSchema.PostalAddressIndex);
			}
			set
			{
				base.PropertyBag[ContactSchema.PostalAddressIndex] = value;
			}
		}

		[DataMember(Name = "PostalAddressIndex", EmitDefaultValue = false, Order = 26)]
		[XmlIgnore]
		public string PostalAddressIndexString
		{
			get
			{
				if (!this.PostalAddressIndexSpecified)
				{
					return null;
				}
				return EnumUtilities.ToString<PhysicalAddressIndexType>(this.PostalAddressIndex);
			}
			set
			{
				this.PostalAddressIndex = EnumUtilities.Parse<PhysicalAddressIndexType>(value);
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool PostalAddressIndexSpecified
		{
			get
			{
				return base.PropertyBag.Contains(ContactSchema.PostalAddressIndex);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 27)]
		public string Profession
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(ContactSchema.Profession);
			}
			set
			{
				base.PropertyBag[ContactSchema.Profession] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 28)]
		public string SpouseName
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(ContactSchema.SpouseName);
			}
			set
			{
				base.PropertyBag[ContactSchema.SpouseName] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 29)]
		public string Surname
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(ContactSchema.Surname);
			}
			set
			{
				base.PropertyBag[ContactSchema.Surname] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 30)]
		public string WeddingAnniversary
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(ContactSchema.WeddingAnniversary);
			}
			set
			{
				base.PropertyBag[ContactSchema.WeddingAnniversary] = value;
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool WeddingAnniversarySpecified
		{
			get
			{
				return base.PropertyBag.Contains(ContactSchema.WeddingAnniversary);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 31)]
		public bool HasPicture
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<bool>(ContactSchema.HasPicture);
			}
			set
			{
				base.PropertyBag[ContactSchema.HasPicture] = value;
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool HasPictureSpecified
		{
			get
			{
				return base.PropertyBag.Contains(ContactSchema.HasPicture);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 32)]
		public string PhoneticFullName
		{
			get
			{
				return this.GetDirectoryProperty("PhoneticFullName") as string;
			}
			set
			{
				this.SetDirectoryProperty("PhoneticFullName", value);
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 33)]
		public string PhoneticFirstName
		{
			get
			{
				return this.GetDirectoryProperty("PhoneticFirstName") as string;
			}
			set
			{
				this.SetDirectoryProperty("PhoneticFirstName", value);
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 34)]
		public string PhoneticLastName
		{
			get
			{
				return this.GetDirectoryProperty("PhoneticLastName") as string;
			}
			set
			{
				this.SetDirectoryProperty("PhoneticLastName", value);
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 35)]
		public string Alias
		{
			get
			{
				return this.GetDirectoryProperty("Alias") as string;
			}
			set
			{
				this.SetDirectoryProperty("Alias", value);
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 36)]
		public string Notes
		{
			get
			{
				return this.GetDirectoryProperty("Notes") as string;
			}
			set
			{
				this.SetDirectoryProperty("Notes", value);
			}
		}

		[XmlElement(DataType = "base64Binary")]
		[IgnoreDataMember]
		public byte[] Photo
		{
			get
			{
				return this.GetDirectoryProperty("Photo") as byte[];
			}
			set
			{
				this.SetDirectoryProperty("Photo", value);
			}
		}

		[XmlIgnore]
		[DataMember(Name = "Photo", EmitDefaultValue = false, Order = 37, IsRequired = false)]
		public string PhotoString
		{
			get
			{
				byte[] array = this.GetDirectoryProperty("Photo") as byte[];
				if (array != null)
				{
					return Convert.ToBase64String(array);
				}
				return null;
			}
			set
			{
				if (value != null)
				{
					this.SetDirectoryProperty("Photo", Convert.FromBase64String(value));
					return;
				}
				this.SetDirectoryProperty("Photo", value);
			}
		}

		[XmlArrayItem("Base64Binary", DataType = "base64Binary", Type = typeof(byte[]))]
		[IgnoreDataMember]
		[XmlArray]
		public byte[][] UserSMIMECertificate
		{
			get
			{
				return this.GetDirectoryProperty("UserSMIMECertificate") as byte[][];
			}
			set
			{
				this.SetDirectoryProperty("UserSMIMECertificate", value);
			}
		}

		[DataMember(Name = "UserSMIMECertificate", EmitDefaultValue = false, Order = 38, IsRequired = false)]
		[XmlIgnore]
		public string[] UserSMIMECertificateString
		{
			get
			{
				return ContactItemType.Base64StringArrayFromJaggedByteArrayArray(this.GetDirectoryProperty("UserSMIMECertificate") as byte[][]);
			}
			set
			{
				this.SetDirectoryProperty("UserSMIMECertificate", ContactItemType.JaggedByteArrayArrayFromBase64StringArray(value));
			}
		}

		[XmlArray]
		[XmlArrayItem("Base64Binary", DataType = "base64Binary", Type = typeof(byte[]))]
		[IgnoreDataMember]
		public byte[][] MSExchangeCertificate
		{
			get
			{
				return this.GetDirectoryProperty("MSExchangeCertificate") as byte[][];
			}
			set
			{
				this.SetDirectoryProperty("MSExchangeCertificate", value);
			}
		}

		[XmlIgnore]
		[DataMember(Name = "MSExchangeCertificate", EmitDefaultValue = false, Order = 39, IsRequired = false)]
		public string[] MSExchangeCertificateString
		{
			get
			{
				return ContactItemType.Base64StringArrayFromJaggedByteArrayArray(this.GetDirectoryProperty("MSExchangeCertificate") as byte[][]);
			}
			set
			{
				this.SetDirectoryProperty("MSExchangeCertificate", ContactItemType.JaggedByteArrayArrayFromBase64StringArray(value));
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 40)]
		public string DirectoryId
		{
			get
			{
				return this.GetDirectoryProperty("DirectoryId") as string;
			}
			set
			{
				this.SetDirectoryProperty("DirectoryId", value);
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 41)]
		public SingleRecipientType ManagerMailbox
		{
			get
			{
				return this.GetDirectoryProperty("ManagerMailbox") as SingleRecipientType;
			}
			set
			{
				this.SetDirectoryProperty("ManagerMailbox", value);
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 42)]
		[XmlArrayItem("Mailbox", Type = typeof(EmailAddressWrapper))]
		public EmailAddressWrapper[] DirectReports
		{
			get
			{
				return this.GetDirectoryProperty("DirectReports") as EmailAddressWrapper[];
			}
			set
			{
				this.SetDirectoryProperty("DirectReports", value);
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 43)]
		public string BirthdayLocal
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(ContactSchema.BirthdayLocal);
			}
			set
			{
				base.PropertyBag[ContactSchema.BirthdayLocal] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 44)]
		public string WeddingAnniversaryLocal
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(ContactSchema.WeddingAnniversaryLocal);
			}
			set
			{
				base.PropertyBag[ContactSchema.WeddingAnniversaryLocal] = value;
			}
		}

		internal override StoreObjectType StoreObjectType
		{
			get
			{
				return StoreObjectType.Contact;
			}
		}

		private static byte[][] JaggedByteArrayArrayFromBase64StringArray(string[] inArray)
		{
			if (inArray != null)
			{
				List<byte[]> list = new List<byte[]>(inArray.Length);
				foreach (string text in inArray)
				{
					if (text != null)
					{
						list.Add(Convert.FromBase64String(text));
					}
					else
					{
						list.Add(null);
					}
				}
				return list.ToArray();
			}
			return null;
		}

		private static string[] Base64StringArrayFromJaggedByteArrayArray(byte[][] inArray)
		{
			if (inArray != null)
			{
				List<string> list = new List<string>(inArray.Length);
				foreach (byte[] array in inArray)
				{
					if (array != null)
					{
						list.Add(Convert.ToBase64String(array));
					}
					else
					{
						list.Add(null);
					}
				}
				return list.ToArray();
			}
			return null;
		}

		private object GetDirectoryProperty(string key)
		{
			object result;
			if (this.directoryProperties.TryGetValue(key, out result))
			{
				return result;
			}
			return null;
		}

		private void SetDirectoryProperty(string key, object value)
		{
			if (this.directoryProperties.ContainsKey(key))
			{
				this.directoryProperties[key] = value;
				return;
			}
			this.directoryProperties.Add(key, value);
		}

		private TEntry[] GetIndexedProperties<TEntry, TKey>(Dictionary<TKey, PropertyInformation> map, Func<TKey, string, TEntry> createEntryFunc)
		{
			List<TEntry> list = new List<TEntry>();
			foreach (KeyValuePair<TKey, PropertyInformation> keyValuePair in map)
			{
				string valueOrDefault = base.GetValueOrDefault<string>(keyValuePair.Value);
				if (valueOrDefault != null)
				{
					TEntry item = createEntryFunc(keyValuePair.Key, valueOrDefault);
					list.Add(item);
				}
			}
			if (list.Count > 0)
			{
				return list.ToArray();
			}
			return null;
		}

		private void SetPropertyIfValueIsSet(PropertyInformation propInfo, string value)
		{
			if (value != null)
			{
				this[propInfo] = value;
			}
		}

		private EmailAddressDictionaryEntryType[] GetEmailAddresses()
		{
			List<EmailAddressDictionaryEntryType> list = null;
			foreach (PropertyInformation propertyInfo in ContactItemType.emailAddressProps)
			{
				EmailAddressDictionaryEntryType valueOrDefault = base.GetValueOrDefault<EmailAddressDictionaryEntryType>(propertyInfo);
				if (valueOrDefault != null)
				{
					if (list == null)
					{
						list = new List<EmailAddressDictionaryEntryType>();
					}
					list.Add(valueOrDefault);
				}
			}
			if (list == null)
			{
				return null;
			}
			return list.ToArray();
		}

		private void SetEmailAddresses(EmailAddressDictionaryEntryType[] value)
		{
			if (value != null)
			{
				foreach (EmailAddressDictionaryEntryType emailAddressDictionaryEntryType in value)
				{
					PropertyInformation property = ContactItemType.emailAddressToPropInfoMap[emailAddressDictionaryEntryType.Key];
					this[property] = emailAddressDictionaryEntryType;
				}
			}
		}

		private PhysicalAddressDictionaryEntryType[] GetPhysicalAddresses()
		{
			List<PhysicalAddressDictionaryEntryType> list = null;
			list = this.AddPhysicalAddressIfNeeded(list, PhysicalAddressKeyType.Business, ContactSchema.PhysicalAddressBusinessStreet, ContactSchema.PhysicalAddressBusinessCity, ContactSchema.PhysicalAddressBusinessState, ContactSchema.PhysicalAddressBusinessCountryOrRegion, ContactSchema.PhysicalAddressBusinessPostalCode);
			list = this.AddPhysicalAddressIfNeeded(list, PhysicalAddressKeyType.Home, ContactSchema.PhysicalAddressHomeStreet, ContactSchema.PhysicalAddressHomeCity, ContactSchema.PhysicalAddressHomeState, ContactSchema.PhysicalAddressHomeCountryOrRegion, ContactSchema.PhysicalAddressHomePostalCode);
			list = this.AddPhysicalAddressIfNeeded(list, PhysicalAddressKeyType.Other, ContactSchema.PhysicalAddressOtherStreet, ContactSchema.PhysicalAddressOtherCity, ContactSchema.PhysicalAddressOtherState, ContactSchema.PhysicalAddressOtherCountryOrRegion, ContactSchema.PhysicalAddressOtherPostalCode);
			if (list == null)
			{
				return null;
			}
			return list.ToArray();
		}

		private List<PhysicalAddressDictionaryEntryType> AddPhysicalAddressIfNeeded(List<PhysicalAddressDictionaryEntryType> addressList, PhysicalAddressKeyType key, PropertyInformation streetProp, PropertyInformation cityProp, PropertyInformation stateProp, PropertyInformation countryOrRegionProp, PropertyInformation postalCodeProp)
		{
			if (base.IsSet(streetProp) || base.IsSet(cityProp) || base.IsSet(stateProp) || base.IsSet(countryOrRegionProp) || base.IsSet(postalCodeProp))
			{
				PhysicalAddressDictionaryEntryType item = new PhysicalAddressDictionaryEntryType
				{
					Key = key,
					Street = base.GetValueOrDefault<string>(streetProp),
					City = base.GetValueOrDefault<string>(cityProp),
					State = base.GetValueOrDefault<string>(stateProp),
					CountryOrRegion = base.GetValueOrDefault<string>(countryOrRegionProp),
					PostalCode = base.GetValueOrDefault<string>(postalCodeProp)
				};
				if (addressList == null)
				{
					addressList = new List<PhysicalAddressDictionaryEntryType>();
				}
				addressList.Add(item);
			}
			return addressList;
		}

		private void SetPhysicalAddresses(PhysicalAddressDictionaryEntryType[] value)
		{
			foreach (PhysicalAddressDictionaryEntryType physicalAddressDictionaryEntryType in value)
			{
				switch (physicalAddressDictionaryEntryType.Key)
				{
				case PhysicalAddressKeyType.Home:
					this.SetPropertyIfValueIsSet(ContactSchema.PhysicalAddressHomeStreet, physicalAddressDictionaryEntryType.Street);
					this.SetPropertyIfValueIsSet(ContactSchema.PhysicalAddressHomeCity, physicalAddressDictionaryEntryType.City);
					this.SetPropertyIfValueIsSet(ContactSchema.PhysicalAddressHomeState, physicalAddressDictionaryEntryType.State);
					this.SetPropertyIfValueIsSet(ContactSchema.PhysicalAddressHomePostalCode, physicalAddressDictionaryEntryType.PostalCode);
					this.SetPropertyIfValueIsSet(ContactSchema.PhysicalAddressHomeCountryOrRegion, physicalAddressDictionaryEntryType.CountryOrRegion);
					break;
				case PhysicalAddressKeyType.Business:
					this.SetPropertyIfValueIsSet(ContactSchema.PhysicalAddressBusinessStreet, physicalAddressDictionaryEntryType.Street);
					this.SetPropertyIfValueIsSet(ContactSchema.PhysicalAddressBusinessCity, physicalAddressDictionaryEntryType.City);
					this.SetPropertyIfValueIsSet(ContactSchema.PhysicalAddressBusinessState, physicalAddressDictionaryEntryType.State);
					this.SetPropertyIfValueIsSet(ContactSchema.PhysicalAddressBusinessPostalCode, physicalAddressDictionaryEntryType.PostalCode);
					this.SetPropertyIfValueIsSet(ContactSchema.PhysicalAddressBusinessCountryOrRegion, physicalAddressDictionaryEntryType.CountryOrRegion);
					break;
				case PhysicalAddressKeyType.Other:
					this.SetPropertyIfValueIsSet(ContactSchema.PhysicalAddressOtherStreet, physicalAddressDictionaryEntryType.Street);
					this.SetPropertyIfValueIsSet(ContactSchema.PhysicalAddressOtherCity, physicalAddressDictionaryEntryType.City);
					this.SetPropertyIfValueIsSet(ContactSchema.PhysicalAddressOtherState, physicalAddressDictionaryEntryType.State);
					this.SetPropertyIfValueIsSet(ContactSchema.PhysicalAddressOtherPostalCode, physicalAddressDictionaryEntryType.PostalCode);
					this.SetPropertyIfValueIsSet(ContactSchema.PhysicalAddressOtherCountryOrRegion, physicalAddressDictionaryEntryType.CountryOrRegion);
					break;
				}
			}
		}

		private const string DirId = "DirectoryId";

		private const string DirPhoneticFullName = "PhoneticFullName";

		private const string DirPhoneticFirstName = "PhoneticFirstName";

		private const string DirPhoneticLastName = "PhoneticLastName";

		private const string DirAlias = "Alias";

		private const string DirNotes = "Notes";

		private const string DirPhoto = "Photo";

		private const string DirUserSMIMECertificate = "UserSMIMECertificate";

		private const string DirMSExchangeCertificate = "MSExchangeCertificate";

		private const string DirManagerMailbox = "ManagerMailbox";

		private const string DirDirectReports = "DirectReports";

		private static Dictionary<ImAddressKeyType, PropertyInformation> imAddressToPropInfoMap = new Dictionary<ImAddressKeyType, PropertyInformation>
		{
			{
				ImAddressKeyType.ImAddress1,
				ContactSchema.ImAddressImAddress1
			},
			{
				ImAddressKeyType.ImAddress2,
				ContactSchema.ImAddressImAddress2
			},
			{
				ImAddressKeyType.ImAddress3,
				ContactSchema.ImAddressImAddress3
			}
		};

		private static Dictionary<EmailAddressKeyType, PropertyInformation> emailAddressToPropInfoMap = new Dictionary<EmailAddressKeyType, PropertyInformation>
		{
			{
				EmailAddressKeyType.EmailAddress1,
				ContactSchema.EmailAddressEmailAddress1
			},
			{
				EmailAddressKeyType.EmailAddress2,
				ContactSchema.EmailAddressEmailAddress2
			},
			{
				EmailAddressKeyType.EmailAddress3,
				ContactSchema.EmailAddressEmailAddress3
			}
		};

		private static Dictionary<PhoneNumberKeyType, PropertyInformation> phoneNumberToPropInfoMap = new Dictionary<PhoneNumberKeyType, PropertyInformation>
		{
			{
				PhoneNumberKeyType.AssistantPhone,
				ContactSchema.PhoneNumberAssistantPhone
			},
			{
				PhoneNumberKeyType.BusinessFax,
				ContactSchema.PhoneNumberBusinessFax
			},
			{
				PhoneNumberKeyType.BusinessPhone,
				ContactSchema.PhoneNumberBusinessPhone
			},
			{
				PhoneNumberKeyType.BusinessPhone2,
				ContactSchema.PhoneNumberBusinessPhone2
			},
			{
				PhoneNumberKeyType.Callback,
				ContactSchema.PhoneNumberCallback
			},
			{
				PhoneNumberKeyType.CarPhone,
				ContactSchema.PhoneNumberCarPhone
			},
			{
				PhoneNumberKeyType.CompanyMainPhone,
				ContactSchema.PhoneNumberCompanyMainPhone
			},
			{
				PhoneNumberKeyType.HomeFax,
				ContactSchema.PhoneNumberHomeFax
			},
			{
				PhoneNumberKeyType.HomePhone,
				ContactSchema.PhoneNumberHomePhone
			},
			{
				PhoneNumberKeyType.HomePhone2,
				ContactSchema.PhoneNumberHomePhone2
			},
			{
				PhoneNumberKeyType.Isdn,
				ContactSchema.PhoneNumberIsdn
			},
			{
				PhoneNumberKeyType.MobilePhone,
				ContactSchema.PhoneNumberMobilePhone
			},
			{
				PhoneNumberKeyType.OtherFax,
				ContactSchema.PhoneNumberOtherFax
			},
			{
				PhoneNumberKeyType.OtherTelephone,
				ContactSchema.PhoneNumberOtherTelephone
			},
			{
				PhoneNumberKeyType.Pager,
				ContactSchema.PhoneNumberPager
			},
			{
				PhoneNumberKeyType.PrimaryPhone,
				ContactSchema.PhoneNumberPrimaryPhone
			},
			{
				PhoneNumberKeyType.RadioPhone,
				ContactSchema.PhoneNumberRadioPhone
			},
			{
				PhoneNumberKeyType.Telex,
				ContactSchema.PhoneNumberTelex
			},
			{
				PhoneNumberKeyType.TtyTddPhone,
				ContactSchema.PhoneNumberTtyTddPhone
			}
		};

		private static PropertyInformation[] emailAddressProps = new PropertyInformation[]
		{
			ContactSchema.EmailAddressEmailAddress1,
			ContactSchema.EmailAddressEmailAddress2,
			ContactSchema.EmailAddressEmailAddress3
		};

		private Dictionary<string, object> directoryProperties = new Dictionary<string, object>();

		private ContactSourceType contactSource = ContactSourceType.Store;

		private bool contactSourceSpecified;
	}
}
