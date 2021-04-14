using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Inference.PeopleRelevance;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("PersonaType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "PersonaType")]
	[Serializable]
	public class Persona : ServiceObject
	{
		[XmlElement]
		[DataMember(Order = 1)]
		public ItemId PersonaId
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<ItemId>(PersonaSchema.PersonaId);
			}
			set
			{
				base.PropertyBag[PersonaSchema.PersonaId] = value;
			}
		}

		[DataMember(Name = "PersonaTypeString", Order = 2)]
		[XmlElement(ElementName = "PersonaType")]
		public string PersonaType
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(PersonaSchema.PersonaType);
			}
			set
			{
				base.PropertyBag[PersonaSchema.PersonaType] = value;
			}
		}

		[DataMember(Name = "PersonaObjectStatusString", EmitDefaultValue = false, Order = 3)]
		public string PersonaObjectStatus
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		[XmlElement]
		[DataMember(Name = "CreationTimeString", EmitDefaultValue = false, Order = 4)]
		[DateTimeString]
		public string CreationTime
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(PersonaSchema.CreationTime);
			}
			set
			{
				base.PropertyBag[PersonaSchema.CreationTime] = value;
			}
		}

		[DataMember(Name = "BodiesArray", EmitDefaultValue = false, Order = 5)]
		[XmlArray]
		[XmlArrayItem("BodyContentAttributedValue", typeof(BodyContentAttributedValue))]
		public BodyContentAttributedValue[] Bodies
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<BodyContentAttributedValue[]>(PersonaSchema.Bodies);
			}
			set
			{
				base.PropertyBag[PersonaSchema.Bodies] = value;
			}
		}

		[DataMember(Name = "IsFavorite", EmitDefaultValue = false, Order = 6)]
		[XmlIgnore]
		public bool IsFavorite
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<bool>(PersonaSchema.IsFavorite);
			}
			set
			{
				base.PropertyBag[PersonaSchema.IsFavorite] = value;
			}
		}

		[DataMember(Name = "UnreadCount", EmitDefaultValue = false, Order = 7)]
		[XmlIgnore]
		public int UnreadCount
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<int>(PersonaSchema.UnreadCount);
			}
			set
			{
				base.PropertyBag[PersonaSchema.UnreadCount] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 10)]
		[XmlElement]
		public string DisplayNameFirstLastSortKey
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(PersonaSchema.DisplayNameFirstLastSortKey);
			}
			set
			{
				base.PropertyBag[PersonaSchema.DisplayNameFirstLastSortKey] = value;
			}
		}

		[XmlElement]
		[DataMember(EmitDefaultValue = false, Order = 11)]
		public string DisplayNameLastFirstSortKey
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(PersonaSchema.DisplayNameLastFirstSortKey);
			}
			set
			{
				base.PropertyBag[PersonaSchema.DisplayNameLastFirstSortKey] = value;
			}
		}

		[XmlElement]
		[DataMember(EmitDefaultValue = false, Order = 12)]
		public string CompanyNameSortKey
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(PersonaSchema.CompanyNameSortKey);
			}
			set
			{
				base.PropertyBag[PersonaSchema.CompanyNameSortKey] = value;
			}
		}

		[XmlElement]
		[DataMember(EmitDefaultValue = false, Order = 13)]
		public string HomeCitySortKey
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(PersonaSchema.HomeCitySortKey);
			}
			set
			{
				base.PropertyBag[PersonaSchema.HomeCitySortKey] = value;
			}
		}

		[XmlElement]
		[DataMember(EmitDefaultValue = false, Order = 14)]
		public string WorkCitySortKey
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(PersonaSchema.WorkCitySortKey);
			}
			set
			{
				base.PropertyBag[PersonaSchema.WorkCitySortKey] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 20)]
		[XmlElement]
		public string DisplayNameFirstLastHeader
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(PersonaSchema.DisplayNameFirstLastHeader);
			}
			set
			{
				base.PropertyBag[PersonaSchema.DisplayNameFirstLastHeader] = value;
			}
		}

		[XmlElement]
		[DataMember(EmitDefaultValue = false, Order = 21)]
		public string DisplayNameLastFirstHeader
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(PersonaSchema.DisplayNameLastFirstHeader);
			}
			set
			{
				base.PropertyBag[PersonaSchema.DisplayNameLastFirstHeader] = value;
			}
		}

		[XmlElement]
		[DataMember(EmitDefaultValue = false, Order = 50)]
		public string DisplayName
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(PersonaSchema.DisplayName);
			}
			set
			{
				base.PropertyBag[PersonaSchema.DisplayName] = value;
			}
		}

		[XmlElement]
		[DataMember(EmitDefaultValue = false, Order = 51)]
		public string DisplayNameFirstLast
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(PersonaSchema.DisplayNameFirstLast);
			}
			set
			{
				base.PropertyBag[PersonaSchema.DisplayNameFirstLast] = value;
			}
		}

		[XmlElement]
		[DataMember(EmitDefaultValue = false, Order = 52)]
		public string DisplayNameLastFirst
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(PersonaSchema.DisplayNameLastFirst);
			}
			set
			{
				base.PropertyBag[PersonaSchema.DisplayNameLastFirst] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 53)]
		[XmlElement]
		public string FileAs
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(PersonaSchema.FileAs);
			}
			set
			{
				base.PropertyBag[PersonaSchema.FileAs] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 54)]
		[XmlElement]
		public string FileAsId
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(PersonaSchema.FileAsId);
			}
			set
			{
				base.PropertyBag[PersonaSchema.FileAsId] = value;
			}
		}

		[XmlElement]
		[DataMember(EmitDefaultValue = false, Order = 55)]
		public string DisplayNamePrefix
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(PersonaSchema.DisplayNamePrefix);
			}
			set
			{
				base.PropertyBag[PersonaSchema.DisplayNamePrefix] = value;
			}
		}

		[XmlElement]
		[DataMember(EmitDefaultValue = false, Order = 56)]
		public string GivenName
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(PersonaSchema.GivenName);
			}
			set
			{
				base.PropertyBag[PersonaSchema.GivenName] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 57)]
		[XmlElement]
		public string MiddleName
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(PersonaSchema.MiddleName);
			}
			set
			{
				base.PropertyBag[PersonaSchema.MiddleName] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 58)]
		[XmlElement]
		public string Surname
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(PersonaSchema.Surname);
			}
			set
			{
				base.PropertyBag[PersonaSchema.Surname] = value;
			}
		}

		[XmlElement]
		[DataMember(EmitDefaultValue = false, Order = 59)]
		public string Generation
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(PersonaSchema.Generation);
			}
			set
			{
				base.PropertyBag[PersonaSchema.Generation] = value;
			}
		}

		[XmlElement]
		[DataMember(EmitDefaultValue = false, Order = 60)]
		public string Nickname
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(PersonaSchema.Nickname);
			}
			set
			{
				base.PropertyBag[PersonaSchema.Nickname] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 61)]
		[XmlElement]
		public string YomiFirstName
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(PersonaSchema.YomiFirstName);
			}
			set
			{
				base.PropertyBag[PersonaSchema.YomiFirstName] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 62)]
		[XmlElement]
		public string YomiLastName
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(PersonaSchema.YomiLastName);
			}
			set
			{
				base.PropertyBag[PersonaSchema.YomiLastName] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 63)]
		[XmlElement]
		public string YomiCompanyName
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(PersonaSchema.YomiCompanyName);
			}
			set
			{
				base.PropertyBag[PersonaSchema.YomiCompanyName] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 64)]
		[XmlElement]
		public string Title
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(PersonaSchema.Title);
			}
			set
			{
				base.PropertyBag[PersonaSchema.Title] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 65)]
		[XmlElement]
		public string Department
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(PersonaSchema.Department);
			}
			set
			{
				base.PropertyBag[PersonaSchema.Department] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 66)]
		[XmlElement]
		public string CompanyName
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(PersonaSchema.CompanyName);
			}
			set
			{
				base.PropertyBag[PersonaSchema.CompanyName] = value;
			}
		}

		[XmlElement]
		[DataMember(EmitDefaultValue = false, Order = 67)]
		public string Location
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(PersonaSchema.Location);
			}
			set
			{
				base.PropertyBag[PersonaSchema.Location] = value;
			}
		}

		[XmlElement]
		[DataMember(EmitDefaultValue = false, Order = 68)]
		public EmailAddressWrapper EmailAddress
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<EmailAddressWrapper>(PersonaSchema.EmailAddress);
			}
			set
			{
				base.PropertyBag[PersonaSchema.EmailAddress] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 69)]
		[XmlArray]
		[XmlArrayItem("Address", typeof(EmailAddressWrapper))]
		public EmailAddressWrapper[] EmailAddresses
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<EmailAddressWrapper[]>(PersonaSchema.EmailAddresses);
			}
			set
			{
				base.PropertyBag[PersonaSchema.EmailAddresses] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 70)]
		[XmlElement]
		public PhoneNumber PhoneNumber
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<PhoneNumber>(PersonaSchema.PhoneNumber);
			}
			set
			{
				base.PropertyBag[PersonaSchema.PhoneNumber] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 71)]
		[XmlElement]
		public string ImAddress
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(PersonaSchema.ImAddress);
			}
			set
			{
				base.PropertyBag[PersonaSchema.ImAddress] = value;
			}
		}

		[XmlElement]
		[DataMember(EmitDefaultValue = false, Order = 72)]
		public string HomeCity
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(PersonaSchema.HomeCity);
			}
			set
			{
				base.PropertyBag[PersonaSchema.HomeCity] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 73)]
		[XmlElement]
		public string WorkCity
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(PersonaSchema.WorkCity);
			}
			set
			{
				base.PropertyBag[PersonaSchema.WorkCity] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 74)]
		[XmlIgnore]
		public string Alias
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(PersonaSchema.Alias);
			}
			set
			{
				base.PropertyBag[PersonaSchema.Alias] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 80)]
		[XmlElement]
		public int RelevanceScore
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<int>(PersonaSchema.RelevanceScore, int.MaxValue);
			}
			set
			{
				base.PropertyBag[PersonaSchema.RelevanceScore] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 100)]
		[XmlArray]
		[XmlArrayItem("FolderId", typeof(FolderId))]
		public FolderId[] FolderIds
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<FolderId[]>(PersonaSchema.FolderIds);
			}
			set
			{
				base.PropertyBag[PersonaSchema.FolderIds] = value;
			}
		}

		[XmlArray]
		[XmlArrayItem("Attribution", typeof(Attribution))]
		[DataMember(Name = "AttributionsArray", EmitDefaultValue = false, Order = 102)]
		public Attribution[] Attributions
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<Attribution[]>(PersonaSchema.Attributions);
			}
			set
			{
				base.PropertyBag[PersonaSchema.Attributions] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 103)]
		[XmlIgnore]
		public EmailAddressWrapper[] Members
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<EmailAddressWrapper[]>(PersonaSchema.Members);
			}
			set
			{
				base.PropertyBag[PersonaSchema.Members] = value;
			}
		}

		[DataMember(Name = "ThirdPartyPhotoUrlsArray", EmitDefaultValue = false, Order = 104)]
		[XmlIgnore]
		[XmlArrayItem("StringAttributedValue", typeof(StringAttributedValue))]
		public StringAttributedValue[] ThirdPartyPhotoUrls
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<StringAttributedValue[]>(PersonaSchema.ThirdPartyPhotoUrls);
			}
			set
			{
				base.PropertyBag[PersonaSchema.ThirdPartyPhotoUrls] = value;
			}
		}

		[XmlArrayItem("StringAttributedValue", typeof(StringAttributedValue))]
		[XmlArray]
		[DataMember(Name = "DisplayNamesArray", EmitDefaultValue = false, Order = 200)]
		public StringAttributedValue[] DisplayNames
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<StringAttributedValue[]>(PersonaSchema.DisplayNames);
			}
			set
			{
				base.PropertyBag[PersonaSchema.DisplayNames] = value;
			}
		}

		[DataMember(Name = "FileAsesArray", EmitDefaultValue = false, Order = 201)]
		[XmlArray]
		[XmlArrayItem("StringAttributedValue", typeof(StringAttributedValue))]
		public StringAttributedValue[] FileAses
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<StringAttributedValue[]>(PersonaSchema.FileAses);
			}
			set
			{
				base.PropertyBag[PersonaSchema.FileAses] = value;
			}
		}

		[DataMember(Name = "FileAsIdsArray", EmitDefaultValue = false, Order = 202)]
		[XmlArrayItem("StringAttributedValue", typeof(StringAttributedValue))]
		[XmlArray]
		public StringAttributedValue[] FileAsIds
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<StringAttributedValue[]>(PersonaSchema.FileAsIds);
			}
			set
			{
				base.PropertyBag[PersonaSchema.FileAsIds] = value;
			}
		}

		[DataMember(Name = "DisplayNamePrefixesArray", EmitDefaultValue = false, Order = 203)]
		[XmlArrayItem("StringAttributedValue", typeof(StringAttributedValue))]
		[XmlArray]
		public StringAttributedValue[] DisplayNamePrefixes
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<StringAttributedValue[]>(PersonaSchema.DisplayNamePrefixes);
			}
			set
			{
				base.PropertyBag[PersonaSchema.DisplayNamePrefixes] = value;
			}
		}

		[DataMember(Name = "GivenNamesArray", EmitDefaultValue = false, Order = 204)]
		[XmlArray]
		[XmlArrayItem("StringAttributedValue", typeof(StringAttributedValue))]
		public StringAttributedValue[] GivenNames
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<StringAttributedValue[]>(PersonaSchema.GivenNames);
			}
			set
			{
				base.PropertyBag[PersonaSchema.GivenNames] = value;
			}
		}

		[XmlArrayItem("StringAttributedValue", typeof(StringAttributedValue))]
		[XmlArray]
		[DataMember(Name = "MiddleNamesArray", EmitDefaultValue = false, Order = 205)]
		public StringAttributedValue[] MiddleNames
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<StringAttributedValue[]>(PersonaSchema.MiddleNames);
			}
			set
			{
				base.PropertyBag[PersonaSchema.MiddleNames] = value;
			}
		}

		[XmlArray]
		[DataMember(Name = "SurnamesArray", EmitDefaultValue = false, Order = 206)]
		[XmlArrayItem("StringAttributedValue", typeof(StringAttributedValue))]
		public StringAttributedValue[] Surnames
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<StringAttributedValue[]>(PersonaSchema.Surnames);
			}
			set
			{
				base.PropertyBag[PersonaSchema.Surnames] = value;
			}
		}

		[XmlArrayItem("StringAttributedValue", typeof(StringAttributedValue))]
		[DataMember(Name = "GenerationsArray", EmitDefaultValue = false, Order = 207)]
		[XmlArray]
		public StringAttributedValue[] Generations
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<StringAttributedValue[]>(PersonaSchema.Generations);
			}
			set
			{
				base.PropertyBag[PersonaSchema.Generations] = value;
			}
		}

		[XmlArray]
		[XmlArrayItem("StringAttributedValue", typeof(StringAttributedValue))]
		[DataMember(Name = "NicknamesArray", EmitDefaultValue = false, Order = 208)]
		public StringAttributedValue[] Nicknames
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<StringAttributedValue[]>(PersonaSchema.Nicknames);
			}
			set
			{
				base.PropertyBag[PersonaSchema.Nicknames] = value;
			}
		}

		[DataMember(Name = "InitialsArray", EmitDefaultValue = false, Order = 209)]
		[XmlArray]
		[XmlArrayItem("StringAttributedValue", typeof(StringAttributedValue))]
		public StringAttributedValue[] Initials
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<StringAttributedValue[]>(PersonaSchema.Initials);
			}
			set
			{
				base.PropertyBag[PersonaSchema.Initials] = value;
			}
		}

		[XmlArrayItem("StringAttributedValue", typeof(StringAttributedValue))]
		[DataMember(Name = "YomiFirstNamesArray", EmitDefaultValue = false, Order = 210)]
		[XmlArray]
		public StringAttributedValue[] YomiFirstNames
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<StringAttributedValue[]>(PersonaSchema.YomiFirstNames);
			}
			set
			{
				base.PropertyBag[PersonaSchema.YomiFirstNames] = value;
			}
		}

		[XmlArrayItem("StringAttributedValue", typeof(StringAttributedValue))]
		[DataMember(Name = "YomiLastNamesArray", EmitDefaultValue = false, Order = 211)]
		[XmlArray]
		public StringAttributedValue[] YomiLastNames
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<StringAttributedValue[]>(PersonaSchema.YomiLastNames);
			}
			set
			{
				base.PropertyBag[PersonaSchema.YomiLastNames] = value;
			}
		}

		[DataMember(Name = "YomiCompanyNamesArray", EmitDefaultValue = false, Order = 212)]
		[XmlArray]
		[XmlArrayItem("StringAttributedValue", typeof(StringAttributedValue))]
		public StringAttributedValue[] YomiCompanyNames
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<StringAttributedValue[]>(PersonaSchema.YomiCompanyNames);
			}
			set
			{
				base.PropertyBag[PersonaSchema.YomiCompanyNames] = value;
			}
		}

		[XmlArray]
		[XmlArrayItem("PhoneNumberAttributedValue", typeof(PhoneNumberAttributedValue))]
		[DataMember(Name = "BusinessPhoneNumbersArray", EmitDefaultValue = false, Order = 300)]
		public PhoneNumberAttributedValue[] BusinessPhoneNumbers
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<PhoneNumberAttributedValue[]>(PersonaSchema.BusinessPhoneNumbers);
			}
			set
			{
				base.PropertyBag[PersonaSchema.BusinessPhoneNumbers] = value;
			}
		}

		[DataMember(Name = "BusinessPhoneNumbers2Array", EmitDefaultValue = false, Order = 301)]
		[XmlArray]
		[XmlArrayItem("PhoneNumberAttributedValue", typeof(PhoneNumberAttributedValue))]
		public PhoneNumberAttributedValue[] BusinessPhoneNumbers2
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<PhoneNumberAttributedValue[]>(PersonaSchema.BusinessPhoneNumbers2);
			}
			set
			{
				base.PropertyBag[PersonaSchema.BusinessPhoneNumbers2] = value;
			}
		}

		[XmlArray]
		[XmlArrayItem("PhoneNumberAttributedValue", typeof(PhoneNumberAttributedValue))]
		[DataMember(Name = "HomePhonesArray", EmitDefaultValue = false, Order = 302)]
		public PhoneNumberAttributedValue[] HomePhones
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<PhoneNumberAttributedValue[]>(PersonaSchema.HomePhones);
			}
			set
			{
				base.PropertyBag[PersonaSchema.HomePhones] = value;
			}
		}

		[XmlArrayItem("PhoneNumberAttributedValue", typeof(PhoneNumberAttributedValue))]
		[DataMember(Name = "HomePhones2Array", EmitDefaultValue = false, Order = 304)]
		[XmlArray]
		public PhoneNumberAttributedValue[] HomePhones2
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<PhoneNumberAttributedValue[]>(PersonaSchema.HomePhones2);
			}
			set
			{
				base.PropertyBag[PersonaSchema.HomePhones2] = value;
			}
		}

		[XmlArray]
		[DataMember(Name = "MobilePhonesArray", EmitDefaultValue = false, Order = 305)]
		[XmlArrayItem("PhoneNumberAttributedValue", typeof(PhoneNumberAttributedValue))]
		public PhoneNumberAttributedValue[] MobilePhones
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<PhoneNumberAttributedValue[]>(PersonaSchema.MobilePhones);
			}
			set
			{
				base.PropertyBag[PersonaSchema.MobilePhones] = value;
			}
		}

		[XmlArray]
		[XmlArrayItem("PhoneNumberAttributedValue", typeof(PhoneNumberAttributedValue))]
		[DataMember(Name = "MobilePhones2Array", EmitDefaultValue = false, Order = 306)]
		public PhoneNumberAttributedValue[] MobilePhones2
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<PhoneNumberAttributedValue[]>(PersonaSchema.MobilePhones2);
			}
			set
			{
				base.PropertyBag[PersonaSchema.MobilePhones2] = value;
			}
		}

		[DataMember(Name = "AssistantPhoneNumbersArray", EmitDefaultValue = false, Order = 307)]
		[XmlArray]
		[XmlArrayItem("PhoneNumberAttributedValue", typeof(PhoneNumberAttributedValue))]
		public PhoneNumberAttributedValue[] AssistantPhoneNumbers
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<PhoneNumberAttributedValue[]>(PersonaSchema.AssistantPhoneNumbers);
			}
			set
			{
				base.PropertyBag[PersonaSchema.AssistantPhoneNumbers] = value;
			}
		}

		[XmlArrayItem("PhoneNumberAttributedValue", typeof(PhoneNumberAttributedValue))]
		[XmlArray]
		[DataMember(Name = "CallbackPhonesArray", EmitDefaultValue = false, Order = 308)]
		public PhoneNumberAttributedValue[] CallbackPhones
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<PhoneNumberAttributedValue[]>(PersonaSchema.CallbackPhones);
			}
			set
			{
				base.PropertyBag[PersonaSchema.CallbackPhones] = value;
			}
		}

		[XmlArrayItem("PhoneNumberAttributedValue", typeof(PhoneNumberAttributedValue))]
		[DataMember(Name = "CarPhonesArray", EmitDefaultValue = false, Order = 309)]
		[XmlArray]
		public PhoneNumberAttributedValue[] CarPhones
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<PhoneNumberAttributedValue[]>(PersonaSchema.CarPhones);
			}
			set
			{
				base.PropertyBag[PersonaSchema.CarPhones] = value;
			}
		}

		[DataMember(Name = "HomeFaxesArray", EmitDefaultValue = false, Order = 313)]
		[XmlArrayItem("PhoneNumberAttributedValue", typeof(PhoneNumberAttributedValue))]
		[XmlArray]
		public PhoneNumberAttributedValue[] HomeFaxes
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<PhoneNumberAttributedValue[]>(PersonaSchema.HomeFaxes);
			}
			set
			{
				base.PropertyBag[PersonaSchema.HomeFaxes] = value;
			}
		}

		[DataMember(Name = "OrganizationMainPhonesArray", EmitDefaultValue = false, Order = 314)]
		[XmlArrayItem("PhoneNumberAttributedValue", typeof(PhoneNumberAttributedValue))]
		[XmlArray]
		public PhoneNumberAttributedValue[] OrganizationMainPhones
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<PhoneNumberAttributedValue[]>(PersonaSchema.OrganizationMainPhones);
			}
			set
			{
				base.PropertyBag[PersonaSchema.OrganizationMainPhones] = value;
			}
		}

		[DataMember(Name = "OtherFaxesArray", EmitDefaultValue = false, Order = 315)]
		[XmlArray]
		[XmlArrayItem("PhoneNumberAttributedValue", typeof(PhoneNumberAttributedValue))]
		public PhoneNumberAttributedValue[] OtherFaxes
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<PhoneNumberAttributedValue[]>(PersonaSchema.OtherFaxes);
			}
			set
			{
				base.PropertyBag[PersonaSchema.OtherFaxes] = value;
			}
		}

		[XmlArray]
		[DataMember(Name = "OtherTelephonesArray", EmitDefaultValue = false, Order = 316)]
		[XmlArrayItem("PhoneNumberAttributedValue", typeof(PhoneNumberAttributedValue))]
		public PhoneNumberAttributedValue[] OtherTelephones
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<PhoneNumberAttributedValue[]>(PersonaSchema.OtherTelephones);
			}
			set
			{
				base.PropertyBag[PersonaSchema.OtherTelephones] = value;
			}
		}

		[XmlArrayItem("PhoneNumberAttributedValue", typeof(PhoneNumberAttributedValue))]
		[DataMember(Name = "OtherPhones2Array", EmitDefaultValue = false, Order = 317)]
		[XmlArray]
		public PhoneNumberAttributedValue[] OtherPhones2
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<PhoneNumberAttributedValue[]>(PersonaSchema.OtherPhones2);
			}
			set
			{
				base.PropertyBag[PersonaSchema.OtherPhones2] = value;
			}
		}

		[XmlArray]
		[XmlArrayItem("PhoneNumberAttributedValue", typeof(PhoneNumberAttributedValue))]
		[DataMember(Name = "PagersArray", EmitDefaultValue = false, Order = 318)]
		public PhoneNumberAttributedValue[] Pagers
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<PhoneNumberAttributedValue[]>(PersonaSchema.Pagers);
			}
			set
			{
				base.PropertyBag[PersonaSchema.Pagers] = value;
			}
		}

		[DataMember(Name = "RadioPhonesArray", EmitDefaultValue = false, Order = 319)]
		[XmlArrayItem("PhoneNumberAttributedValue", typeof(PhoneNumberAttributedValue))]
		[XmlArray]
		public PhoneNumberAttributedValue[] RadioPhones
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<PhoneNumberAttributedValue[]>(PersonaSchema.RadioPhones);
			}
			set
			{
				base.PropertyBag[PersonaSchema.RadioPhones] = value;
			}
		}

		[XmlArray]
		[DataMember(Name = "TelexNumbersArray", EmitDefaultValue = false, Order = 320)]
		[XmlArrayItem("PhoneNumberAttributedValue", typeof(PhoneNumberAttributedValue))]
		public PhoneNumberAttributedValue[] TelexNumbers
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<PhoneNumberAttributedValue[]>(PersonaSchema.TelexNumbers);
			}
			set
			{
				base.PropertyBag[PersonaSchema.TelexNumbers] = value;
			}
		}

		[XmlArray]
		[DataMember(Name = "TTYTDDPhoneNumbersArray", EmitDefaultValue = false, Order = 321)]
		[XmlArrayItem("PhoneNumberAttributedValue", typeof(PhoneNumberAttributedValue))]
		public PhoneNumberAttributedValue[] TTYTDDPhoneNumbers
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<PhoneNumberAttributedValue[]>(PersonaSchema.TTYTDDPhoneNumbers);
			}
			set
			{
				base.PropertyBag[PersonaSchema.TTYTDDPhoneNumbers] = value;
			}
		}

		[XmlArray]
		[XmlArrayItem("PhoneNumberAttributedValue", typeof(PhoneNumberAttributedValue))]
		[DataMember(Name = "WorkFaxesArray", EmitDefaultValue = false, Order = 322)]
		public PhoneNumberAttributedValue[] WorkFaxes
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<PhoneNumberAttributedValue[]>(PersonaSchema.WorkFaxes);
			}
			set
			{
				base.PropertyBag[PersonaSchema.WorkFaxes] = value;
			}
		}

		[DataMember(Name = "Emails1Array", EmitDefaultValue = false, Order = 400)]
		[XmlArrayItem("EmailAddressAttributedValue", typeof(EmailAddressAttributedValue))]
		[XmlArray]
		public EmailAddressAttributedValue[] Emails1
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<EmailAddressAttributedValue[]>(PersonaSchema.Emails1);
			}
			set
			{
				base.PropertyBag[PersonaSchema.Emails1] = value;
			}
		}

		[XmlArray]
		[DataMember(Name = "Emails2Array", EmitDefaultValue = false, Order = 401)]
		[XmlArrayItem("EmailAddressAttributedValue", typeof(EmailAddressAttributedValue))]
		public EmailAddressAttributedValue[] Emails2
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<EmailAddressAttributedValue[]>(PersonaSchema.Emails2);
			}
			set
			{
				base.PropertyBag[PersonaSchema.Emails2] = value;
			}
		}

		[XmlArrayItem("EmailAddressAttributedValue", typeof(EmailAddressAttributedValue))]
		[DataMember(Name = "Emails3Array", EmitDefaultValue = false, Order = 402)]
		[XmlArray]
		public EmailAddressAttributedValue[] Emails3
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<EmailAddressAttributedValue[]>(PersonaSchema.Emails3);
			}
			set
			{
				base.PropertyBag[PersonaSchema.Emails3] = value;
			}
		}

		[XmlArrayItem("StringAttributedValue", typeof(StringAttributedValue))]
		[DataMember(Name = "BusinessHomePagesArray", EmitDefaultValue = false, Order = 500)]
		[XmlArray]
		public StringAttributedValue[] BusinessHomePages
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<StringAttributedValue[]>(PersonaSchema.BusinessHomePages);
			}
			set
			{
				base.PropertyBag[PersonaSchema.BusinessHomePages] = value;
			}
		}

		[XmlArray]
		[XmlArrayItem("StringAttributedValue", typeof(StringAttributedValue))]
		[DataMember(Name = "PersonalHomePagesArray", EmitDefaultValue = false, Order = 501)]
		public StringAttributedValue[] PersonalHomePages
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<StringAttributedValue[]>(PersonaSchema.PersonalHomePages);
			}
			set
			{
				base.PropertyBag[PersonaSchema.PersonalHomePages] = value;
			}
		}

		[XmlArrayItem("StringAttributedValue", typeof(StringAttributedValue))]
		[DataMember(Name = "OfficeLocationsArray", EmitDefaultValue = false, Order = 502)]
		[XmlArray]
		public StringAttributedValue[] OfficeLocations
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<StringAttributedValue[]>(PersonaSchema.OfficeLocations);
			}
			set
			{
				base.PropertyBag[PersonaSchema.OfficeLocations] = value;
			}
		}

		[XmlArrayItem("StringAttributedValue", typeof(StringAttributedValue))]
		[DataMember(Name = "ImAddressesArray", EmitDefaultValue = false, Order = 503)]
		[XmlArray]
		public StringAttributedValue[] ImAddresses
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<StringAttributedValue[]>(PersonaSchema.ImAddresses);
			}
			set
			{
				base.PropertyBag[PersonaSchema.ImAddresses] = value;
			}
		}

		[XmlArrayItem("StringAttributedValue", typeof(StringAttributedValue))]
		[DataMember(Name = "ImAddresses2Array", EmitDefaultValue = false, Order = 504)]
		[XmlArray]
		public StringAttributedValue[] ImAddresses2
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<StringAttributedValue[]>(PersonaSchema.ImAddresses2);
			}
			set
			{
				base.PropertyBag[PersonaSchema.ImAddresses2] = value;
			}
		}

		[XmlArrayItem("StringAttributedValue", typeof(StringAttributedValue))]
		[XmlArray]
		[DataMember(Name = "ImAddresses3Array", EmitDefaultValue = false, Order = 505)]
		public StringAttributedValue[] ImAddresses3
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<StringAttributedValue[]>(PersonaSchema.ImAddresses3);
			}
			set
			{
				base.PropertyBag[PersonaSchema.ImAddresses3] = value;
			}
		}

		[XmlArray]
		[XmlArrayItem("PostalAddressAttributedValue", typeof(PostalAddressAttributedValue))]
		[DataMember(Name = "BusinessAddressesArray", EmitDefaultValue = false, Order = 600)]
		public PostalAddressAttributedValue[] BusinessAddresses
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<PostalAddressAttributedValue[]>(PersonaSchema.BusinessAddresses);
			}
			set
			{
				base.PropertyBag[PersonaSchema.BusinessAddresses] = value;
			}
		}

		[XmlArray]
		[XmlArrayItem("PostalAddressAttributedValue", typeof(PostalAddressAttributedValue))]
		[DataMember(Name = "HomeAddressesArray", EmitDefaultValue = false, Order = 601)]
		public PostalAddressAttributedValue[] HomeAddresses
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<PostalAddressAttributedValue[]>(PersonaSchema.HomeAddresses);
			}
			set
			{
				base.PropertyBag[PersonaSchema.HomeAddresses] = value;
			}
		}

		[DataMember(Name = "OtherAddressesArray", EmitDefaultValue = false, Order = 602)]
		[XmlArrayItem("PostalAddressAttributedValue", typeof(PostalAddressAttributedValue))]
		[XmlArray]
		public PostalAddressAttributedValue[] OtherAddresses
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<PostalAddressAttributedValue[]>(PersonaSchema.OtherAddresses);
			}
			set
			{
				base.PropertyBag[PersonaSchema.OtherAddresses] = value;
			}
		}

		[XmlArray]
		[XmlArrayItem("StringAttributedValue", typeof(StringAttributedValue))]
		[DataMember(Name = "TitlesArray", EmitDefaultValue = false, Order = 700)]
		public StringAttributedValue[] Titles
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<StringAttributedValue[]>(PersonaSchema.Titles);
			}
			set
			{
				base.PropertyBag[PersonaSchema.Titles] = value;
			}
		}

		[XmlArrayItem("StringAttributedValue", typeof(StringAttributedValue))]
		[DataMember(Name = "DepartmentsArray", EmitDefaultValue = false, Order = 701)]
		[XmlArray]
		public StringAttributedValue[] Departments
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<StringAttributedValue[]>(PersonaSchema.Departments);
			}
			set
			{
				base.PropertyBag[PersonaSchema.Departments] = value;
			}
		}

		[XmlArrayItem("StringAttributedValue", typeof(StringAttributedValue))]
		[DataMember(Name = "CompanyNamesArray", EmitDefaultValue = false, Order = 702)]
		[XmlArray]
		public StringAttributedValue[] CompanyNames
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<StringAttributedValue[]>(PersonaSchema.CompanyNames);
			}
			set
			{
				base.PropertyBag[PersonaSchema.CompanyNames] = value;
			}
		}

		[DataMember(Name = "ManagersArray", EmitDefaultValue = false, Order = 703)]
		[XmlArray]
		[XmlArrayItem("StringAttributedValue", typeof(StringAttributedValue))]
		public StringAttributedValue[] Managers
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<StringAttributedValue[]>(PersonaSchema.Managers);
			}
			set
			{
				base.PropertyBag[PersonaSchema.Managers] = value;
			}
		}

		[XmlArrayItem("StringAttributedValue", typeof(StringAttributedValue))]
		[DataMember(Name = "AssistantNamesArray", EmitDefaultValue = false, Order = 704)]
		[XmlArray]
		public StringAttributedValue[] AssistantNames
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<StringAttributedValue[]>(PersonaSchema.AssistantNames);
			}
			set
			{
				base.PropertyBag[PersonaSchema.AssistantNames] = value;
			}
		}

		[XmlArray]
		[DataMember(Name = "ProfessionsArray", EmitDefaultValue = false, Order = 705)]
		[XmlArrayItem("StringAttributedValue", typeof(StringAttributedValue))]
		public StringAttributedValue[] Professions
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<StringAttributedValue[]>(PersonaSchema.Professions);
			}
			set
			{
				base.PropertyBag[PersonaSchema.Professions] = value;
			}
		}

		[XmlArray]
		[DataMember(Name = "SpouseNamesArray", EmitDefaultValue = false, Order = 706)]
		[XmlArrayItem("StringAttributedValue", typeof(StringAttributedValue))]
		public StringAttributedValue[] SpouseNames
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<StringAttributedValue[]>(PersonaSchema.SpouseNames);
			}
			set
			{
				base.PropertyBag[PersonaSchema.SpouseNames] = value;
			}
		}

		[DataMember(Name = "ChildrenArray", EmitDefaultValue = false, Order = 707)]
		[XmlArrayItem("StringArrayAttributedValue", typeof(StringArrayAttributedValue))]
		[XmlArray]
		public StringArrayAttributedValue[] Children
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<StringArrayAttributedValue[]>(PersonaSchema.Children);
			}
			set
			{
				base.PropertyBag[PersonaSchema.Children] = value;
			}
		}

		[XmlArray]
		[DataMember(Name = "HobbiesArray", EmitDefaultValue = false, Order = 708)]
		[XmlArrayItem("StringAttributedValue", typeof(StringAttributedValue))]
		public StringAttributedValue[] Hobbies
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<StringAttributedValue[]>(PersonaSchema.Hobbies);
			}
			set
			{
				base.PropertyBag[PersonaSchema.Hobbies] = value;
			}
		}

		[DataMember(Name = "WeddingAnniversariesArray", EmitDefaultValue = false, Order = 709)]
		[XmlArrayItem("StringAttributedValue", typeof(StringAttributedValue))]
		[XmlArray]
		public StringAttributedValue[] WeddingAnniversaries
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<StringAttributedValue[]>(PersonaSchema.WeddingAnniversaries);
			}
			set
			{
				base.PropertyBag[PersonaSchema.WeddingAnniversaries] = value;
			}
		}

		[XmlArrayItem("StringAttributedValue", typeof(StringAttributedValue))]
		[DataMember(Name = "BirthdaysArray", EmitDefaultValue = false, Order = 710)]
		[XmlArray]
		public StringAttributedValue[] Birthdays
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<StringAttributedValue[]>(PersonaSchema.Birthdays);
			}
			set
			{
				base.PropertyBag[PersonaSchema.Birthdays] = value;
			}
		}

		[DataMember(Name = "LocationsArray", EmitDefaultValue = false, Order = 711)]
		[XmlArray]
		[XmlArrayItem("StringAttributedValue", typeof(StringAttributedValue))]
		public StringAttributedValue[] Locations
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<StringAttributedValue[]>(PersonaSchema.Locations);
			}
			set
			{
				base.PropertyBag[PersonaSchema.Locations] = value;
			}
		}

		[XmlArrayItem("StringAttributedValue", typeof(StringAttributedValue))]
		[DataMember(Name = "SchoolsArray", EmitDefaultValue = false, Order = 712)]
		[XmlArray]
		public StringAttributedValue[] Schools
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<StringAttributedValue[]>(PersonaSchema.Schools);
			}
			set
			{
				base.PropertyBag[PersonaSchema.Schools] = value;
			}
		}

		[DataMember(Name = "BirthdaysLocalArray", EmitDefaultValue = false, Order = 713)]
		[XmlIgnore]
		public StringAttributedValue[] BirthdaysLocal
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<StringAttributedValue[]>(PersonaSchema.BirthdaysLocal);
			}
			set
			{
				base.PropertyBag[PersonaSchema.BirthdaysLocal] = value;
			}
		}

		[XmlIgnore]
		[DataMember(Name = "WeddingAnniversariesLocalArray", EmitDefaultValue = false, Order = 714)]
		public StringAttributedValue[] WeddingAnniversariesLocal
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<StringAttributedValue[]>(PersonaSchema.WeddingAnniversariesLocal);
			}
			set
			{
				base.PropertyBag[PersonaSchema.WeddingAnniversariesLocal] = value;
			}
		}

		[DataMember(Name = "ExtendedPropertiesArray", EmitDefaultValue = false, Order = 900)]
		[XmlArray]
		[XmlArrayItem("ExtendedPropertyAttributedValue", typeof(ExtendedPropertyAttributedValue))]
		public ExtendedPropertyAttributedValue[] ExtendedProperties
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<ExtendedPropertyAttributedValue[]>(PersonaSchema.ExtendedProperties);
			}
			set
			{
				base.PropertyBag[PersonaSchema.ExtendedProperties] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 901)]
		[XmlIgnore]
		public Guid ADObjectId
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<Guid>(PersonaSchema.ADObjectId);
			}
			set
			{
				base.PropertyBag[PersonaSchema.ADObjectId] = value;
			}
		}

		internal static Persona LoadFromPropertyBag(StoreSession storeSession, IDictionary<PropertyDefinition, object> xsoPropertyBag, ToServiceObjectForPropertyBagPropertyList propertyList)
		{
			PersonId storeId = (PersonId)xsoPropertyBag[PersonSchema.Id];
			IdAndSession idAndSession = new IdAndSession(storeId, storeSession);
			return Persona.LoadFromPropertyBag(idAndSession, xsoPropertyBag, propertyList);
		}

		internal static Persona LoadFromPropertyBag(StoreSession storeSession, StoreId parentFolderId, IDictionary<PropertyDefinition, object> xsoPropertyBag, ToServiceObjectForPropertyBagPropertyList propertyList)
		{
			PersonId storeId = (PersonId)xsoPropertyBag[PersonSchema.Id];
			IdAndSession idAndSession = new IdAndSession(storeId, parentFolderId, storeSession);
			return Persona.LoadFromPropertyBag(idAndSession, xsoPropertyBag, propertyList);
		}

		private static Persona LoadFromPropertyBag(IdAndSession idAndSession, IDictionary<PropertyDefinition, object> xsoPropertyBag, ToServiceObjectForPropertyBagPropertyList propertyList)
		{
			Persona.Tracer.TraceDebug((long)idAndSession.Id.GetHashCode(), "Persona.LoadFromPropertyBag: Entering");
			Persona persona = new Persona();
			propertyList.ConvertPropertiesToServiceObject(persona, xsoPropertyBag, idAndSession);
			HashSet<PropertyPath> properties = propertyList.GetProperties();
			persona.LoadCultureBasedData(properties, idAndSession.Session.PreferedCulture);
			Persona.Tracer.TraceDebug((long)idAndSession.Id.GetHashCode(), "Persona.LoadFromPropertyBag: Exiting");
			return persona;
		}

		internal static Persona LoadFromPersonaId(StoreSession storeSession, IRecipientSession adRecipientSession, ItemId personaId, PersonaResponseShape personaShape, PropertyDefinition[] extendedPropertyDefinitions = null, StoreId folderId = null)
		{
			Persona.Tracer.TraceDebug<string>(Persona.TraceIdFromItemId(personaId), "Persona.LoadFromPersonaId: Entering, with PersonaId = {0}", (personaId == null) ? "(null)" : personaId.Id);
			Persona result;
			if (IdConverter.EwsIdIsActiveDirectoryObject(personaId.GetId()))
			{
				result = Persona.LoadFromADObjectId((MailboxSession)storeSession, adRecipientSession, IdConverter.EwsIdToADObjectId(personaId.GetId()), personaShape);
			}
			else
			{
				result = Persona.LoadFromPersonId(storeSession, IdConverter.EwsIdToPersonId(personaId.GetId()), personaShape, extendedPropertyDefinitions, folderId);
			}
			return result;
		}

		internal static Persona LoadFromPersonaIdWithGalAggregation(MailboxSession mailboxSession, IRecipientSession adRecipientSession, ItemId personaId, PersonaResponseShape personaShape, PropertyDefinition[] extendedPropertyDefinitions)
		{
			Persona.Tracer.TraceDebug<string>(Persona.TraceIdFromItemId(personaId), "Persona.LoadFromPersonaId: Entering, with PersonaId = {0}", (personaId == null) ? "(null)" : personaId.Id);
			Persona result;
			if (IdConverter.EwsIdIsActiveDirectoryObject(personaId.GetId()))
			{
				ADObjectId adObjectId = IdConverter.EwsIdToADObjectId(personaId.GetId());
				result = Persona.LoadFromADObjectIdWithMailboxAggregation(mailboxSession, adRecipientSession, adObjectId, personaShape);
			}
			else
			{
				result = Persona.LoadFromPersonIdWithGalAggregation(mailboxSession, IdConverter.EwsIdToPersonId(personaId.GetId()), personaShape, extendedPropertyDefinitions);
			}
			return result;
		}

		internal static Persona LoadFromEmailAddressWithGalAggregation(MailboxSession mailboxSession, IRecipientSession adRecipientSession, EmailAddressWrapper emailAddress, PersonaResponseShape personaShape)
		{
			Persona.Tracer.TraceDebug<string, string>((long)emailAddress.GetHashCode(), "Loading Person by Email - Address:{0}, MailboxType:{1}.", emailAddress.EmailAddress, emailAddress.MailboxType);
			Persona.Tracer.TraceDebug((long)emailAddress.GetHashCode(), "Lookup Email Address in AD.");
			Persona persona = Persona.FindPersonaInADByEmailAddress(mailboxSession, adRecipientSession, emailAddress.EmailAddress, personaShape);
			if (persona != null)
			{
				Persona.Tracer.TraceDebug<string>((long)emailAddress.GetHashCode(), "Found a match in GAL, return this to user - Persona Id:{0}.", persona.PersonaId.Id);
				return persona;
			}
			PersonId personId = Person.FindPersonIdByEmailAddress(mailboxSession, new XSOFactory(), emailAddress.EmailAddress);
			if (personId == null)
			{
				Persona.Tracer.TraceDebug((long)emailAddress.GetHashCode(), "No match found in personal contacts.");
				return null;
			}
			Persona.Tracer.TraceDebug<PersonId>((long)emailAddress.GetHashCode(), "Found a match in Personal Contacts with Person Id: {0}.", personId);
			return Persona.LoadFromPersonIdWithGalAggregation(mailboxSession, personId, personaShape, null);
		}

		internal static IEnumerable<Persona> LoadFromLegacyDNs(MailboxSession mailboxSession, IRecipientSession adRecipientSession, string[] legacyDNs, PersonaResponseShape personaShape)
		{
			ToServiceObjectForPropertyBagPropertyList propertyList = Persona.GetPropertyListForPersonaResponseShape(personaShape);
			PropertyDefinition[] requestedProperties = propertyList.GetPropertyDefinitions();
			ADPersonToContactConverterSet converterSet = ADPersonToContactConverterSet.FromPersonProperties(requestedProperties, null);
			Result<ADRawEntry>[] adResults = adRecipientSession.FindByLegacyExchangeDNs(legacyDNs, converterSet.ADProperties);
			for (int i = 0; i < adResults.Length; i++)
			{
				Result<ADRawEntry> result = adResults[i];
				if (result.Error == null && result.Data != null)
				{
					yield return Persona.LoadFromADRawEntry(result.Data, mailboxSession, converterSet, propertyList);
				}
				else
				{
					Persona.Tracer.TraceDebug<string, ProviderError>((long)result.GetHashCode(), "Persona.LoadFromLegacyDNs: Could not find AD entry for legacyDN {0}. Error: {1}.", legacyDNs[i], result.Error);
					yield return new Persona
					{
						EmailAddress = new EmailAddressWrapper
						{
							EmailAddress = legacyDNs[i],
							RoutingType = "EX"
						}
					};
				}
			}
			yield break;
		}

		internal static IEnumerable<Persona> LoadFromProxyAddresses(MailboxSession mailboxSession, IRecipientSession adRecipientSession, ProxyAddress[] proxyAddresses, PersonaResponseShape personaShape)
		{
			if (proxyAddresses != null && proxyAddresses.Any<ProxyAddress>())
			{
				ToServiceObjectForPropertyBagPropertyList propertyList = Persona.GetPropertyListForPersonaResponseShape(personaShape);
				PropertyDefinition[] requestedProperties = propertyList.GetPropertyDefinitions();
				ADPersonToContactConverterSet converterSet = ADPersonToContactConverterSet.FromPersonProperties(requestedProperties, null);
				Result<ADRecipient>[] adResults = adRecipientSession.FindByProxyAddresses(proxyAddresses);
				for (int i = 0; i < adResults.Length; i++)
				{
					Result<ADRecipient> result = adResults[i];
					if (result.Error == null && result.Data != null)
					{
						yield return Persona.LoadFromADRawEntry(result.Data, mailboxSession, converterSet, propertyList);
					}
					else
					{
						Persona.Tracer.TraceDebug<ProxyAddress, ProviderError>((long)result.GetHashCode(), "Persona.LoadFromProxyAddresses: Could not find AD entry for proxy address {0}. Error: {1}.", proxyAddresses[i], result.Error);
						EmailAddressWrapper emailWrapper = new EmailAddressWrapper
						{
							EmailAddress = proxyAddresses[i].ProxyAddressString,
							RoutingType = "SMTP"
						};
						yield return new Persona
						{
							EmailAddress = emailWrapper
						};
					}
				}
			}
			yield break;
		}

		internal static Persona UpdatePersona(StoreSession storeSession, PersonId personId, ItemId personaId, ICollection<StoreObjectPropertyChange> propertyChanges, PersonType personType, StoreId folderId = null)
		{
			Persona.Tracer.TraceDebug<PersonId, PersonType>(PersonId.TraceId(personId), "Persona.UpdatePersona: Entering, with PersonId = {0} and personType = {1}", personId, personType);
			ToServiceObjectForPropertyBagPropertyList toServiceObjectForPropertyBagPropertyList = Persona.FullPersonaPropertyList;
			PersonaResponseShape personaShape = Persona.FullPersonaShapeWithFolderIds;
			if (Persona.IsBodiesRequested(propertyChanges))
			{
				toServiceObjectForPropertyBagPropertyList = Persona.FullPersonaPropertyListWithBodies;
				personaShape = Persona.FullPersonaShapeWithFolderIdsAndBodies;
			}
			Person person = Person.Load(storeSession, personId, toServiceObjectForPropertyBagPropertyList.GetPropertyDefinitions(), null, folderId);
			PersonId personId2 = person.UpdatePerson(storeSession, propertyChanges, personType == PersonType.DistributionList);
			ItemId personaId2 = IdConverter.PersonaIdFromPersonId(storeSession.MailboxGuid, personId2);
			Persona result = Persona.LoadFromPersonaId(storeSession, null, personaId2, personaShape, null, folderId);
			Persona.Tracer.TraceDebug(PersonId.TraceId(personId), "Persona.UpdatePersona: Exiting with personType = {0}");
			return result;
		}

		internal static void DeletePersona(StoreSession storeSession, PersonId personId, ItemId personaId, StoreId deleteInFolder)
		{
			Persona.Tracer.TraceDebug<PersonId, StoreId>(PersonId.TraceId(personId), "Persona.DeletePersona: Entering, with PersonId = {0} and DeleteInFolder = {1}", personId, deleteInFolder);
			Person person = Person.Load(storeSession, personId, null, null, deleteInFolder);
			DeleteItemFlags deleteFlags = (storeSession is PublicFolderSession) ? DeleteItemFlags.SoftDelete : DeleteItemFlags.MoveToDeletedItems;
			if (person != null)
			{
				person.Delete(storeSession, deleteFlags, deleteInFolder);
			}
			Persona.Tracer.TraceDebug(PersonId.TraceId(personId), "Persona.DeletePersona: Exiting");
		}

		internal static Persona CreatePersona(StoreSession storeSession, ICollection<StoreObjectPropertyChange> propertyChanges, StoreId parentFolder, ItemId personaId, PersonType personType)
		{
			Persona.Tracer.TraceDebug<PersonType>(0L, "Persona.CreatePersona: Entering with personType = {0}", personType);
			PersonaResponseShape personaShape = Persona.FullPersonaShapeWithFolderIds;
			if (Persona.IsBodiesRequested(propertyChanges))
			{
				personaShape = Persona.FullPersonaShapeWithFolderIdsAndBodies;
			}
			PersonId personId = (personaId == null) ? PersonId.CreateNew() : IdConverter.EwsIdToPersonId(personaId.GetId());
			PersonId personId2 = Person.CreatePerson(storeSession, personId, propertyChanges, parentFolder, personType == PersonType.DistributionList);
			ItemId personaId2 = IdConverter.PersonaIdFromPersonId(storeSession.MailboxGuid, personId2);
			Persona persona = Persona.LoadFromPersonaId(storeSession, null, personaId2, personaShape, null, parentFolder);
			MailboxSession mailboxSession = storeSession as MailboxSession;
			if (mailboxSession != null && persona.EmailAddresses != null && persona.EmailAddresses.Length > 0)
			{
				MdbMaskedPeopleModelDataBinder mdbMaskedPeopleModelDataBinder = MdbMaskedPeopleModelDataBinderFactory.Current.CreateInstance(mailboxSession);
				MaskedPeopleModelItem modelData = mdbMaskedPeopleModelDataBinder.GetModelData();
				if (modelData != null)
				{
					bool flag = false;
					foreach (EmailAddressWrapper emailAddressWrapper in persona.EmailAddresses)
					{
						string emailAddress = emailAddressWrapper.EmailAddress;
						if (!string.IsNullOrEmpty(emailAddress))
						{
							for (int j = 0; j < modelData.ContactList.Count; j++)
							{
								if (emailAddress.Equals(modelData.ContactList[j].EmailAddress, StringComparison.OrdinalIgnoreCase))
								{
									Persona.Tracer.TraceDebug<string>(0L, "Persona.CreatePersona: Unmasking {0}.", emailAddress);
									modelData.ContactList.RemoveAt(j);
									flag = true;
									break;
								}
							}
						}
						else
						{
							Persona.Tracer.TraceDebug(0L, "Persona.CreatePersona: Persona's email address is null or empty.");
						}
					}
					if (flag)
					{
						Persona.Tracer.TraceDebug(0L, "Persona.CreatePersona: Saving changes to masked model.");
						MdbModelUtils.WriteModelItem<MaskedPeopleModelItem, MdbMaskedPeopleModelDataBinder>(mdbMaskedPeopleModelDataBinder, modelData);
					}
				}
				else
				{
					Persona.Tracer.TraceDebug(0L, "Persona.CreatePersona: Model could not be retrieved from the Masked contact FAI.");
				}
			}
			else
			{
				Persona.Tracer.TraceDebug(0L, "Persona.CreatePersona: Persona was created with no email addresses.");
			}
			Persona.Tracer.TraceDebug<PersonType>(0L, "Persona.CreatePersona: Exiting with personType = {0}", personType);
			return persona;
		}

		private static EmailAddressWrapper EWSEmailAddressFromXsoEmailAddress(EmailAddress emailAddress)
		{
			return new EmailAddressWrapper
			{
				Name = emailAddress.Name,
				EmailAddress = emailAddress.Address,
				RoutingType = emailAddress.RoutingType,
				OriginalDisplayName = emailAddress.OriginalDisplayName
			};
		}

		internal static ToServiceObjectForPropertyBagPropertyList GetPropertyListForPersonaResponseShape(PersonaResponseShape personaResponseShape)
		{
			if (personaResponseShape == null)
			{
				personaResponseShape = Persona.DefaultPersonaShape;
			}
			ToServiceObjectForPropertyBagPropertyList toServiceObjectForPropertyBagPropertyList = PropertyList.CreateToServiceObjectForPropertyBagPropertyList(personaResponseShape, Schema.Persona);
			if (personaResponseShape.BaseShape == ShapeEnum.AllProperties)
			{
				foreach (PropertyInformation propertyInformation in PersonaSchema.AllPropertiesExclusionList)
				{
					bool flag = false;
					if (personaResponseShape.AdditionalProperties != null)
					{
						foreach (PropertyPath propertyPath in personaResponseShape.AdditionalProperties)
						{
							if (propertyPath.Equals(propertyInformation.PropertyPath))
							{
								flag = true;
								break;
							}
						}
					}
					if (!flag)
					{
						toServiceObjectForPropertyBagPropertyList.Remove(propertyInformation.PropertyPath);
					}
				}
			}
			return toServiceObjectForPropertyBagPropertyList;
		}

		internal static Persona LoadFromPersonIdWithGalAggregation(MailboxSession mailboxSession, PersonId personId, PersonaResponseShape personaShape, PropertyDefinition[] extendedPropertyDefinitions)
		{
			Persona.Tracer.TraceDebug<PersonId>(PersonId.TraceId(personId), "Persona.LoadFromPersonIdWithGalAggregation: Entering, with PersonId = {0}", personId);
			ToServiceObjectForPropertyBagPropertyList propertyListForPersonaResponseShape = Persona.GetPropertyListForPersonaResponseShape(personaShape);
			PropertyDefinition[] propertyDefinitions = propertyListForPersonaResponseShape.GetPropertyDefinitions();
			Person person = Person.LoadWithGALAggregation(mailboxSession, personId, propertyDefinitions, extendedPropertyDefinitions);
			Persona result = null;
			if (person != null)
			{
				Dictionary<PropertyDefinition, object> properties = Persona.GetProperties(person.PropertyBag, propertyListForPersonaResponseShape.GetPropertyDefinitions());
				result = Persona.LoadFromPropertyBag(mailboxSession, properties, propertyListForPersonaResponseShape);
			}
			Persona.Tracer.TraceDebug(PersonId.TraceId(personId), "Persona.LoadFromPersonIdWithGalAggregation: Exiting");
			return result;
		}

		private static Persona LoadFromPersonId(StoreSession storeSession, PersonId personId, PersonaResponseShape personaShape, PropertyDefinition[] extendedPropertyDefinitions, StoreId folderId = null)
		{
			Persona.Tracer.TraceDebug<PersonId>(PersonId.TraceId(personId), "Persona.LoadFromPersonId: Entering, with PersonId = {0}", personId);
			ToServiceObjectForPropertyBagPropertyList propertyListForPersonaResponseShape = Persona.GetPropertyListForPersonaResponseShape(personaShape);
			PropertyDefinition[] propertyDefinitions = propertyListForPersonaResponseShape.GetPropertyDefinitions();
			Person person = Person.Load(storeSession, personId, propertyDefinitions, extendedPropertyDefinitions, folderId);
			Persona result = null;
			if (person != null)
			{
				Dictionary<PropertyDefinition, object> properties = Persona.GetProperties(person.PropertyBag, propertyListForPersonaResponseShape.GetPropertyDefinitions());
				if (folderId != null)
				{
					result = Persona.LoadFromPropertyBag(storeSession, folderId, properties, propertyListForPersonaResponseShape);
				}
				else
				{
					result = Persona.LoadFromPropertyBag(storeSession, properties, propertyListForPersonaResponseShape);
				}
			}
			Persona.Tracer.TraceDebug(PersonId.TraceId(personId), "Persona.LoadFromPersonId: Exiting");
			return result;
		}

		private static Persona LoadFromADObjectIdWithMailboxAggregation(MailboxSession mailboxSession, IRecipientSession adRecipientSession, ADObjectId adObjectId, PersonaResponseShape personaShape)
		{
			Persona.Tracer.TraceDebug<ADObjectId>((long)adObjectId.GetHashCode(), "Persona.LoadFromADObjectIdWithMailboxAggregation: ADObjectId = {0}", adObjectId);
			return Persona.LoadFromADPersonWithMailboxAggregation(mailboxSession, (ADPropertyDefinition[] adProperties) => adRecipientSession.ReadADRawEntry(adObjectId, adProperties), personaShape);
		}

		private static Persona LoadFromADPersonWithMailboxAggregation(MailboxSession mailboxSession, Func<ADPropertyDefinition[], ADRawEntry> adPersonLoader, PersonaResponseShape personaShape)
		{
			ToServiceObjectForPropertyBagPropertyList propertyListForPersonaResponseShape = Persona.GetPropertyListForPersonaResponseShape(personaShape);
			PropertyDefinition[] propertyDefinitions = propertyListForPersonaResponseShape.GetPropertyDefinitions();
			ADPersonToContactConverterSet adpersonToContactConverterSet = ADPersonToContactConverterSet.FromPersonProperties(propertyDefinitions, null);
			ADRawEntry adrawEntry = adPersonLoader(adpersonToContactConverterSet.ADProperties);
			if (adrawEntry == null)
			{
				Persona.Tracer.TraceDebug(0L, "No AD Person found, there is nothing else to do.");
				return null;
			}
			Persona.Tracer.TraceDebug<Guid>((long)adrawEntry.Id.GetHashCode(), "Found AD Person with Id: {0}.", adrawEntry.Id.ObjectGuid);
			Person person = Person.FindPersonLinkedToADEntry(mailboxSession, adrawEntry, propertyDefinitions);
			if (person == null)
			{
				Persona.Tracer.TraceDebug((long)adrawEntry.Id.GetHashCode(), "No Person linked to AD found, just load Persona with AD Data alone.");
				return Persona.LoadFromADRawEntry(adrawEntry, mailboxSession, adpersonToContactConverterSet, propertyListForPersonaResponseShape);
			}
			Dictionary<PropertyDefinition, object> properties = Persona.GetProperties(person.PropertyBag, propertyDefinitions);
			return Persona.LoadFromPropertyBag(mailboxSession, properties, propertyListForPersonaResponseShape);
		}

		private static Persona LoadFromADObjectId(MailboxSession mailboxSession, IRecipientSession adRecipientSession, ADObjectId adObjectId, PersonaResponseShape personaShape)
		{
			Persona.Tracer.TraceDebug<ADObjectId>((long)adObjectId.GetHashCode(), "Persona.LoadFromADObjectId: Entering, with ADObjectId = {0}", adObjectId);
			ToServiceObjectForPropertyBagPropertyList propertyListForPersonaResponseShape = Persona.GetPropertyListForPersonaResponseShape(personaShape);
			PropertyDefinition[] propertyDefinitions = propertyListForPersonaResponseShape.GetPropertyDefinitions();
			ADPersonToContactConverterSet adpersonToContactConverterSet = ADPersonToContactConverterSet.FromPersonProperties(propertyDefinitions, null);
			ADRawEntry adrawEntry = adRecipientSession.ReadADRawEntry(adObjectId, adpersonToContactConverterSet.ADProperties);
			Persona result = null;
			if (adrawEntry != null)
			{
				result = Persona.LoadFromADRawEntry(adrawEntry, mailboxSession, adpersonToContactConverterSet, propertyListForPersonaResponseShape);
			}
			Persona.Tracer.TraceDebug((long)adObjectId.GetHashCode(), "Persona.LoadFromADObjectId: Exiting");
			return result;
		}

		private static Persona FindPersonaInADByEmailAddress(MailboxSession mailboxSession, IRecipientSession adRecipientSession, string emailAddress, PersonaResponseShape personaShape)
		{
			Persona.Tracer.TraceDebug<string>((long)emailAddress.GetHashCode(), "Persona.LoadFromADWithEmailAddress: Entering, with emailAddress = {0}", emailAddress);
			ProxyAddress proxyAddress = null;
			if (!ProxyAddress.TryParse(emailAddress, out proxyAddress))
			{
				Persona.Tracer.TraceDebug((long)emailAddress.GetHashCode(), "Persona.LoadFromADWithEmailAddress: Invalid proxy address, skip looking into AD.");
				return null;
			}
			return Persona.LoadFromADPersonWithMailboxAggregation(mailboxSession, (ADPropertyDefinition[] adProperties) => adRecipientSession.FindByProxyAddress(proxyAddress, adProperties), personaShape);
		}

		private static long TraceIdFromItemId(ItemId itemId)
		{
			return (long)((itemId == null || itemId.Id == null) ? 0 : itemId.Id.GetHashCode());
		}

		internal static Dictionary<PropertyDefinition, object> GetProperties(IStorePropertyBag propertyBag, PropertyDefinition[] properties)
		{
			object[] properties2 = propertyBag.GetProperties(properties);
			Dictionary<PropertyDefinition, object> dictionary = new Dictionary<PropertyDefinition, object>(properties2.Length);
			for (int i = 0; i < properties.Length; i++)
			{
				dictionary[properties[i]] = properties2[i];
			}
			return dictionary;
		}

		private static void LoadCustomProperty(PropertyInformation property, HashSet<PropertyPath> properties, Action propertyLoader)
		{
			if (!properties.Contains(property.PropertyPath))
			{
				return;
			}
			if (!PersonaSchema.EwsCalculatedProperties.Contains(property.PropertyPath))
			{
				throw new InvalidOperationException(string.Format("Please aggregate the property {0} in XSO. Do not add new properties to EwsCalculatedXmlElements.", property.LocalName));
			}
			propertyLoader();
		}

		private static bool IsBodiesRequested(ICollection<StoreObjectPropertyChange> propertyChanges)
		{
			foreach (StoreObjectPropertyChange storeObjectPropertyChange in propertyChanges)
			{
				if (storeObjectPropertyChange.PropertyDefinition.Name == PersonSchema.Bodies.Name)
				{
					return true;
				}
			}
			return false;
		}

		private void LoadCultureBasedData(HashSet<PropertyPath> properties, CultureInfo cultureInfo)
		{
			Persona.Tracer.TraceDebug(0L, "Persona.LoadCultureBasedData:: Loading Sort Keys ...");
			Persona.LoadCustomProperty(PersonaSchema.DisplayNameFirstLastSortKey, properties, delegate
			{
				this.DisplayNameFirstLastSortKey = PeopleStringUtils.ComputeSortKey(cultureInfo, this.DisplayNameFirstLast);
			});
			Persona.LoadCustomProperty(PersonaSchema.DisplayNameLastFirstSortKey, properties, delegate
			{
				this.DisplayNameLastFirstSortKey = PeopleStringUtils.ComputeSortKey(cultureInfo, this.DisplayNameLastFirst);
			});
			Persona.LoadCustomProperty(PersonaSchema.CompanyNameSortKey, properties, delegate
			{
				this.CompanyNameSortKey = PeopleStringUtils.ComputeSortKey(cultureInfo, this.CompanyName);
			});
			Persona.LoadCustomProperty(PersonaSchema.HomeCitySortKey, properties, delegate
			{
				this.HomeCitySortKey = PeopleStringUtils.ComputeSortKey(cultureInfo, this.HomeCity);
			});
			Persona.LoadCustomProperty(PersonaSchema.WorkCitySortKey, properties, delegate
			{
				this.WorkCitySortKey = PeopleStringUtils.ComputeSortKey(cultureInfo, this.WorkCity);
			});
			Persona.Tracer.TraceDebug(0L, "Persona.LoadCultureBasedData:: Loading Headers ...");
			Persona.LoadCustomProperty(PersonaSchema.DisplayNameFirstLastHeader, properties, delegate
			{
				this.DisplayNameFirstLastHeader = PeopleHeaderProvider.Instance.GetHeader(this.DisplayNameFirstLast, cultureInfo);
			});
			Persona.LoadCustomProperty(PersonaSchema.DisplayNameLastFirstHeader, properties, delegate
			{
				this.DisplayNameLastFirstHeader = PeopleHeaderProvider.Instance.GetHeader(this.DisplayNameLastFirst, cultureInfo);
			});
		}

		internal float SpeechConfidence { get; set; }

		internal static Persona LoadFromADRawEntry(ADRawEntry adRawEntry, MailboxSession mailboxSession, ADPersonToContactConverterSet converterSet, ToServiceObjectForPropertyBagPropertyList propertyList)
		{
			IStorePropertyBag contactPropertyBag = converterSet.Convert(adRawEntry);
			return Persona.LoadFromADContact(contactPropertyBag, mailboxSession, converterSet, propertyList);
		}

		internal static Persona LoadFromADContact(IStorePropertyBag contactPropertyBag, MailboxSession mailboxSession, ADPersonToContactConverterSet converterSet, ToServiceObjectForPropertyBagPropertyList propertyList)
		{
			PersonId value = PersonId.CreateNew();
			contactPropertyBag[ContactSchema.PersonId] = value;
			PersonPropertyAggregationContext context = new PersonPropertyAggregationContext(new IStorePropertyBag[]
			{
				contactPropertyBag
			}, mailboxSession.ContactFolders, mailboxSession.ClientInfoString);
			IStorePropertyBag propertyBag = ApplicationAggregatedProperty.Aggregate(context, converterSet.PersonProperties);
			IDictionary<PropertyDefinition, object> properties = Persona.GetProperties(propertyBag, propertyList.GetPropertyDefinitions());
			Persona persona = Persona.LoadFromPropertyBag(mailboxSession, properties, propertyList);
			persona.ADObjectId = (Guid)contactPropertyBag[ContactSchema.GALLinkID];
			persona.PersonaId = IdConverter.PersonaIdFromADObjectId(persona.ADObjectId);
			return persona;
		}

		internal override StoreObjectType StoreObjectType
		{
			get
			{
				throw new InvalidOperationException("Persona objects don't have a store object type. This method should not be called.");
			}
		}

		internal override void AddExtendedPropertyValue(ExtendedPropertyType extendedProperty)
		{
			throw new InvalidOperationException("Persona objects don't have extended properties. This method should not be called.");
		}

		private static readonly Trace Tracer = ExTraceGlobals.GetPersonaCallTracer;

		private static readonly string MailboxTypeMailbox = MailboxHelper.MailboxTypeType.Mailbox.ToString();

		internal static readonly PersonaResponseShape IdOnlyPersonaShape = new PersonaResponseShape(ShapeEnum.IdOnly);

		internal static readonly PersonaResponseShape DefaultPersonaShape = new PersonaResponseShape(ShapeEnum.Default);

		internal static readonly PersonaResponseShape FullPersonaShape = new PersonaResponseShape(ShapeEnum.AllProperties);

		internal static readonly PersonaResponseShape FullPersonaShapeWithFolderIds = new PersonaResponseShape(ShapeEnum.AllProperties, new PropertyPath[]
		{
			PersonaSchema.FolderIds.PropertyPath
		});

		internal static readonly PersonaResponseShape FullPersonaShapeWithBodies = new PersonaResponseShape(ShapeEnum.AllProperties, new PropertyPath[]
		{
			PersonaSchema.Bodies.PropertyPath
		});

		internal static readonly PersonaResponseShape FullPersonaShapeWithFolderIdsAndBodies = new PersonaResponseShape(ShapeEnum.AllProperties, new PropertyPath[]
		{
			PersonaSchema.FolderIds.PropertyPath,
			PersonaSchema.Bodies.PropertyPath
		});

		internal static readonly PersonaResponseShape IdAndEmailPersonaShape = new PersonaResponseShape(ShapeEnum.IdOnly, new PropertyPath[]
		{
			PersonaSchema.EmailAddress.PropertyPath
		});

		internal static readonly ToServiceObjectForPropertyBagPropertyList DefaultPersonaPropertyList = Persona.GetPropertyListForPersonaResponseShape(Persona.DefaultPersonaShape);

		internal static readonly ToServiceObjectForPropertyBagPropertyList FullPersonaPropertyList = Persona.GetPropertyListForPersonaResponseShape(Persona.FullPersonaShape);

		internal static readonly ToServiceObjectForPropertyBagPropertyList FullPersonaPropertyListWithBodies = Persona.GetPropertyListForPersonaResponseShape(Persona.FullPersonaShapeWithBodies);

		internal static PropertyDefinition[] DefaultPersonaProperties = Persona.DefaultPersonaPropertyList.GetPropertyDefinitions();
	}
}
