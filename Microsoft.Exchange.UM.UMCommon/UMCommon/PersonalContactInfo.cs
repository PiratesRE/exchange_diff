using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;

namespace Microsoft.Exchange.UM.UMCommon
{
	[Serializable]
	internal class PersonalContactInfo : ContactInfo
	{
		internal PersonalContactInfo(Guid userMailboxGuid, Contact contact)
		{
			this.title = PersonalContactInfo.GetProperty(contact, ContactSchema.Title);
			this.fullName = (PersonalContactInfo.GetProperty(contact, StoreObjectSchema.DisplayName) ?? (PersonalContactInfo.GetProperty(contact, ContactSchema.GivenName) + " " + PersonalContactInfo.GetProperty(contact, ContactSchema.Surname)));
			this.firstName = PersonalContactInfo.GetProperty(contact, ContactSchema.GivenName);
			this.lastName = PersonalContactInfo.GetProperty(contact, ContactSchema.Surname);
			this.companyName = PersonalContactInfo.GetProperty(contact, ContactSchema.CompanyName);
			this.workAddressCity = PersonalContactInfo.GetProperty(contact, ContactSchema.WorkAddressCity);
			this.workAddressCountry = PersonalContactInfo.GetProperty(contact, ContactSchema.WorkAddressCountry);
			this.businessPhone1 = this.GetPhoneNumber(contact, ContactSchema.BusinessPhoneNumber);
			this.businessPhone2 = this.GetPhoneNumber(contact, ContactSchema.BusinessPhoneNumber2);
			this.businessPhone = (this.businessPhone1 ?? this.businessPhone2);
			this.homePhone1 = this.GetPhoneNumber(contact, ContactSchema.HomePhone);
			this.homePhone2 = this.GetPhoneNumber(contact, ContactSchema.HomePhone2);
			this.homePhone = (this.homePhone1 ?? this.homePhone2);
			this.mobilePhone = this.GetPhoneNumber(contact, ContactSchema.MobilePhone);
			this.mobilePhone2 = this.GetPhoneNumber(contact, ContactSchema.MobilePhone2);
			this.mobilePhone = (this.mobilePhone ?? this.mobilePhone2);
			this.businessFax = PersonalContactInfo.GetProperty(contact, ContactSchema.WorkFax);
			IDictionary<EmailAddressIndex, Participant> emailAddresses = contact.EmailAddresses;
			this.emailAddress = PersonalContactInfo.GetSmtpAddress(emailAddresses.Values);
			this.instantMessageAddress = PersonalContactInfo.GetProperty(contact, ContactSchema.IMAddress);
			this.storeObjectId = contact.Id.ObjectId;
			this.ewsId = StoreId.StoreIdToEwsId(userMailboxGuid, this.storeObjectId);
		}

		internal PersonalContactInfo(Guid userMailboxGuid, object[] result, FoundByType foundBy, bool loadPhoneNumbersAndDisplayNameOnly)
		{
			StorePropertyDefinition[] propertyDefinitions = loadPhoneNumbersAndDisplayNameOnly ? PersonalContactInfo.contactResolutionPropertyDefinitions : PersonalContactInfo.contactPropertyDefinitions;
			this.foundBy = foundBy;
			if (!loadPhoneNumbersAndDisplayNameOnly)
			{
				this.title = XsoUtil.GetProperty(result, ContactSchema.Title, propertyDefinitions);
				this.firstName = XsoUtil.GetProperty(result, ContactSchema.GivenName, propertyDefinitions);
				this.lastName = XsoUtil.GetProperty(result, ContactSchema.Surname, propertyDefinitions);
				this.companyName = XsoUtil.GetProperty(result, ContactSchema.CompanyName, propertyDefinitions);
				this.workAddressCity = XsoUtil.GetProperty(result, ContactSchema.WorkAddressCity, propertyDefinitions);
				this.workAddressCountry = XsoUtil.GetProperty(result, ContactSchema.WorkAddressCountry, propertyDefinitions);
				this.instantMessageAddress = XsoUtil.GetProperty(result, ContactSchema.IMAddress, propertyDefinitions);
				this.emailAddress = PersonalContactInfo.GetSmtpAddress(new Participant[]
				{
					XsoUtil.GetProperty<Participant>(result, ContactSchema.Email1, propertyDefinitions),
					XsoUtil.GetProperty<Participant>(result, ContactSchema.Email2, propertyDefinitions),
					XsoUtil.GetProperty<Participant>(result, ContactSchema.Email3, propertyDefinitions)
				});
			}
			this.fullName = XsoUtil.GetProperty(result, StoreObjectSchema.DisplayName, propertyDefinitions);
			this.homePhone1 = this.GetPhoneNumber(result, ContactSchema.HomePhone, propertyDefinitions);
			this.homePhone2 = this.GetPhoneNumber(result, ContactSchema.HomePhone2, propertyDefinitions);
			this.homePhone = (this.homePhone1 ?? this.homePhone2);
			this.mobilePhone = this.GetPhoneNumber(result, ContactSchema.MobilePhone, propertyDefinitions);
			this.mobilePhone2 = this.GetPhoneNumber(result, ContactSchema.MobilePhone2, propertyDefinitions);
			this.mobilePhone = (this.mobilePhone ?? this.mobilePhone2);
			this.businessFax = XsoUtil.GetProperty(result, ContactSchema.WorkFax, propertyDefinitions);
			this.businessPhone1 = this.GetPhoneNumber(result, ContactSchema.BusinessPhoneNumber, propertyDefinitions);
			this.businessPhone2 = this.GetPhoneNumber(result, ContactSchema.BusinessPhoneNumber2, propertyDefinitions);
			this.businessPhone = (this.businessPhone1 ?? this.businessPhone2);
			this.personId = XsoUtil.GetProperty<PersonId>(result, ContactSchema.PersonId, propertyDefinitions);
			this.partnerNetworkId = XsoUtil.GetProperty<string>(result, ContactSchema.PartnerNetworkId, propertyDefinitions);
			VersionedId property = XsoUtil.GetProperty<VersionedId>(result, ItemSchema.Id, propertyDefinitions);
			this.storeObjectId = property.ObjectId;
			this.ewsId = StoreId.StoreIdToEwsId(userMailboxGuid, this.storeObjectId);
		}

		internal static StorePropertyDefinition[] ContactPropertyDefinitions
		{
			get
			{
				return PersonalContactInfo.contactPropertyDefinitions;
			}
		}

		internal static StorePropertyDefinition[] ContactPhonePropertyDefinitions
		{
			get
			{
				return PersonalContactInfo.contactResolutionPropertyDefinitions;
			}
		}

		internal string BusinessPhone1
		{
			get
			{
				return this.businessPhone1;
			}
		}

		internal string BusinessPhone2
		{
			get
			{
				return this.businessPhone2;
			}
		}

		internal string HomePhone1
		{
			get
			{
				return this.homePhone1;
			}
		}

		internal string HomePhone2
		{
			get
			{
				return this.homePhone2;
			}
		}

		internal string MobilePhone2
		{
			get
			{
				return this.mobilePhone2;
			}
		}

		internal override string Title
		{
			get
			{
				return this.title;
			}
		}

		internal override string Company
		{
			get
			{
				return this.companyName;
			}
		}

		internal override string DisplayName
		{
			get
			{
				return this.fullName;
			}
		}

		internal override string FirstName
		{
			get
			{
				return this.firstName;
			}
		}

		internal override string LastName
		{
			get
			{
				return this.lastName;
			}
		}

		internal override string BusinessPhone
		{
			get
			{
				return this.businessPhone;
			}
			set
			{
				this.businessPhone = value;
			}
		}

		internal override string MobilePhone
		{
			get
			{
				return this.mobilePhone;
			}
			set
			{
				this.mobilePhone = value;
			}
		}

		internal override string FaxNumber
		{
			get
			{
				return this.businessFax;
			}
		}

		internal override string HomePhone
		{
			get
			{
				return this.homePhone;
			}
			set
			{
				this.homePhone = value;
			}
		}

		internal override string IMAddress
		{
			get
			{
				return this.instantMessageAddress;
			}
		}

		internal override string EMailAddress
		{
			get
			{
				return this.emailAddress;
			}
		}

		internal override FoundByType FoundBy
		{
			get
			{
				return this.foundBy;
			}
		}

		internal override string Id
		{
			get
			{
				return this.storeObjectId.ToBase64String();
			}
		}

		internal override string EwsId
		{
			get
			{
				return this.ewsId;
			}
		}

		internal override string EwsType
		{
			get
			{
				return "Contact";
			}
		}

		internal override string City
		{
			get
			{
				return this.workAddressCity;
			}
		}

		internal override string Country
		{
			get
			{
				return this.workAddressCountry;
			}
		}

		internal override ICollection<string> SanitizedPhoneNumbers
		{
			get
			{
				return this.sanitizedNumbers;
			}
		}

		internal PersonId PersonId
		{
			get
			{
				return this.personId;
			}
		}

		internal string PartnerNetworkId
		{
			get
			{
				return this.partnerNetworkId;
			}
		}

		internal new static ContactInfo FindContactByCallerId(UMSubscriber calledUser, PhoneNumber callerId)
		{
			if (calledUser == null)
			{
				return null;
			}
			List<PersonalContactInfo> uniqueMatches = PersonalContactInfo.GetUniqueMatches(PersonalContactInfo.FindMatchingContacts(calledUser, callerId, false, true));
			PIIMessage data = PIIMessage.Create(PIIType._UserDisplayName, calledUser.DisplayName);
			CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, null, data, "User _UserDisplayName Matched Contacts = {0}. If there is more than one match, then it will be treated as no matches", new object[]
			{
				uniqueMatches.Count
			});
			if (uniqueMatches.Count == 1)
			{
				return uniqueMatches[0];
			}
			if (uniqueMatches.Count > 1)
			{
				return new MultipleResolvedContactInfo(uniqueMatches, callerId, calledUser.MessageSubmissionCulture);
			}
			return null;
		}

		internal static List<PersonalContactInfo> FindAllMatchingContacts(UMSubscriber calledUser, PhoneNumber callerId)
		{
			return PersonalContactInfo.FindMatchingContacts(calledUser, callerId, true, true);
		}

		private static List<PersonalContactInfo> FindMatchingContacts(UMSubscriber calledUser, PhoneNumber callerId, bool loadPhoneNumbersOnly, bool findAllMatchingContacts)
		{
			List<PersonalContactInfo> list = new List<PersonalContactInfo>();
			if (calledUser == null)
			{
				return list;
			}
			if (!calledUser.HasContactsFolder)
			{
				PIIMessage data = PIIMessage.Create(PIIType._UserDisplayName, calledUser.DisplayName);
				CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, null, data, "User _UserDisplayName does not have a contacts folder.", new object[0]);
				return list;
			}
			StorePropertyDefinition[] contactPhonePropertyDefinitions = PersonalContactInfo.ContactPropertyDefinitions;
			new SortBy(StoreObjectSchema.DisplayName, SortOrder.Ascending);
			if (loadPhoneNumbersOnly)
			{
				contactPhonePropertyDefinitions = PersonalContactInfo.ContactPhonePropertyDefinitions;
			}
			List<KeyValuePair<PhoneNumber, List<string>>> list2 = new List<KeyValuePair<PhoneNumber, List<string>>>(2);
			list2.Add(new KeyValuePair<PhoneNumber, List<string>>(callerId, callerId.GetOptionalPrefixes(calledUser.DialPlan)));
			PhoneNumber phoneNumber = callerId.Extend(calledUser.DialPlan);
			if (!phoneNumber.Equals(callerId))
			{
				list2.Add(new KeyValuePair<PhoneNumber, List<string>>(phoneNumber, phoneNumber.GetOptionalPrefixes(calledUser.DialPlan)));
			}
			FoundByType foundByType = FoundByType.NotSpecified;
			using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = calledUser.CreateSessionLock())
			{
				DefaultFolderType defaultFolderType = DefaultFolderType.AllContacts;
				if (mailboxSessionLock.Session.GetDefaultFolderId(defaultFolderType) == null)
				{
					defaultFolderType = DefaultFolderType.Contacts;
				}
				using (Folder folder = Folder.Bind(mailboxSessionLock.Session, defaultFolderType))
				{
					using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.None, null, null, contactPhonePropertyDefinitions))
					{
						object[][] rows = queryResult.GetRows(100);
						while (rows != null && rows.Length > 0)
						{
							foreach (object[] result in rows)
							{
								foundByType = FoundByType.NotSpecified;
								string property = XsoUtil.GetProperty<string>(result, ContactSchema.BusinessPhoneNumber, contactPhonePropertyDefinitions);
								string property2 = XsoUtil.GetProperty<string>(result, ContactSchema.BusinessPhoneNumber2, contactPhonePropertyDefinitions);
								string property3 = XsoUtil.GetProperty<string>(result, ContactSchema.HomePhone, contactPhonePropertyDefinitions);
								string property4 = XsoUtil.GetProperty<string>(result, ContactSchema.HomePhone2, contactPhonePropertyDefinitions);
								string property5 = XsoUtil.GetProperty<string>(result, ContactSchema.MobilePhone, contactPhonePropertyDefinitions);
								string property6 = XsoUtil.GetProperty<string>(result, ContactSchema.MobilePhone2, contactPhonePropertyDefinitions);
								foreach (KeyValuePair<PhoneNumber, List<string>> keyValuePair in list2)
								{
									if (keyValuePair.Key.IsMatch(property, keyValuePair.Value) || keyValuePair.Key.IsMatch(property2, keyValuePair.Value))
									{
										foundByType = FoundByType.BusinessPhone;
										break;
									}
									if (keyValuePair.Key.IsMatch(property3, keyValuePair.Value) || keyValuePair.Key.IsMatch(property4, keyValuePair.Value))
									{
										foundByType = FoundByType.HomePhone;
										break;
									}
									if (keyValuePair.Key.IsMatch(property5, keyValuePair.Value) || keyValuePair.Key.IsMatch(property6, keyValuePair.Value))
									{
										foundByType = FoundByType.MobilePhone;
										break;
									}
								}
								if (foundByType != FoundByType.NotSpecified)
								{
									list.Add(new PersonalContactInfo(mailboxSessionLock.Session.MailboxOwner.MailboxInfo.MailboxGuid, result, foundByType, loadPhoneNumbersOnly));
									if (!findAllMatchingContacts)
									{
										break;
									}
								}
							}
							rows = queryResult.GetRows(100);
						}
					}
				}
			}
			return list;
		}

		private static string GetProperty(Contact contact, StorePropertyDefinition propertyId)
		{
			object obj = contact.TryGetProperty(propertyId);
			if (obj == null || obj is PropertyError)
			{
				return null;
			}
			string input = (string)obj;
			return Utils.TrimSpaces(input);
		}

		private static string GetSmtpAddress(ICollection<Participant> emailAddrList)
		{
			string text = null;
			string text2 = null;
			foreach (Participant participant in emailAddrList)
			{
				if (!(participant == null))
				{
					text = (participant.TryGetProperty(ParticipantSchema.SmtpAddress) as string);
					if (text != null)
					{
						break;
					}
					if (text2 == null)
					{
						string text3 = participant.TryGetProperty(ParticipantSchema.EmailAddressForDisplay) as string;
						if (text3 != null && SmtpAddress.IsValidSmtpAddress(text3))
						{
							text2 = text3;
						}
					}
				}
			}
			return text ?? text2;
		}

		private static List<PersonalContactInfo> GetUniqueMatches(List<PersonalContactInfo> matches)
		{
			if (matches == null || matches.Count <= 1)
			{
				return matches;
			}
			Dictionary<string, PersonalContactInfo> dictionary = new Dictionary<string, PersonalContactInfo>();
			foreach (PersonalContactInfo personalContactInfo in matches)
			{
				if (personalContactInfo.PersonId == null)
				{
					string key = Guid.NewGuid().ToString();
					dictionary.Add(key, personalContactInfo);
				}
				else
				{
					string key = personalContactInfo.PersonId.ToBase64String();
					if (!dictionary.ContainsKey(key))
					{
						dictionary.Add(key, personalContactInfo);
					}
					else if (dictionary[key].PartnerNetworkId != null)
					{
						dictionary[key] = personalContactInfo;
					}
				}
			}
			List<PersonalContactInfo> list = new List<PersonalContactInfo>();
			foreach (KeyValuePair<string, PersonalContactInfo> keyValuePair in dictionary)
			{
				list.Add(keyValuePair.Value);
			}
			return list;
		}

		private string GetPhoneNumber(Contact contact, StorePropertyDefinition propertyDef)
		{
			string property = PersonalContactInfo.GetProperty(contact, propertyDef);
			this.AddToSanitizedNumbersListIfNecessary(property);
			return property;
		}

		private string GetPhoneNumber(object[] result, StorePropertyDefinition propId, StorePropertyDefinition[] propertyDefinitions)
		{
			string property = XsoUtil.GetProperty(result, propId, propertyDefinitions);
			this.AddToSanitizedNumbersListIfNecessary(property);
			return property;
		}

		private void AddToSanitizedNumbersListIfNecessary(string number)
		{
			number = DtmfString.SanitizePhoneNumber(number);
			if (!string.IsNullOrEmpty(number) && !this.sanitizedNumbers.Contains(number))
			{
				this.sanitizedNumbers.Add(number);
			}
		}

		private static StorePropertyDefinition[] contactPropertyDefinitions = new StorePropertyDefinition[]
		{
			ItemSchema.Id,
			StoreObjectSchema.ItemClass,
			ContactSchema.GivenName,
			ContactSchema.Surname,
			StoreObjectSchema.DisplayName,
			ContactSchema.Title,
			ContactSchema.OfficeLocation,
			ContactSchema.CompanyName,
			ContactSchema.BusinessPhoneNumber,
			ContactSchema.BusinessPhoneNumber2,
			ContactSchema.MobilePhone,
			ContactSchema.MobilePhone2,
			ContactSchema.HomePhone,
			ContactSchema.HomePhone2,
			ContactSchema.WorkFax,
			ContactSchema.Email1,
			ContactSchema.Email1EmailAddress,
			ContactSchema.Email1DisplayName,
			ContactSchema.Email1AddrType,
			ContactSchema.Email2,
			ContactSchema.Email2EmailAddress,
			ContactSchema.Email2AddrType,
			ContactSchema.Email3,
			ContactSchema.Email3EmailAddress,
			ContactSchema.Email3AddrType,
			ContactSchema.IMAddress,
			ContactSchema.HomeAddress,
			ContactSchema.BusinessAddress,
			ContactSchema.OtherAddress,
			ContactSchema.WorkAddressCity,
			ContactSchema.WorkAddressCountry,
			ContactSchema.PartnerNetworkId,
			ContactSchema.PersonId
		};

		private static StorePropertyDefinition[] contactResolutionPropertyDefinitions = new StorePropertyDefinition[]
		{
			ContactSchema.BusinessPhoneNumber,
			ContactSchema.BusinessPhoneNumber2,
			ContactSchema.MobilePhone,
			ContactSchema.MobilePhone2,
			ContactSchema.HomePhone,
			ContactSchema.HomePhone2,
			ContactSchema.WorkFax,
			ItemSchema.Id,
			StoreObjectSchema.DisplayName,
			ContactSchema.PartnerNetworkId,
			ContactSchema.PersonId
		};

		private readonly string partnerNetworkId;

		private string title;

		private string companyName;

		private string fullName;

		private string firstName;

		private string lastName;

		private string businessPhone;

		private string businessPhone1;

		private string businessPhone2;

		private string workAddressCity;

		private string workAddressCountry;

		private string mobilePhone;

		private string mobilePhone2;

		private string homePhone;

		private string homePhone1;

		private string homePhone2;

		private string businessFax;

		private string instantMessageAddress;

		private string emailAddress;

		private PersonId personId;

		private FoundByType foundBy;

		private ICollection<string> sanitizedNumbers = new List<string>(10);

		private StoreObjectId storeObjectId;

		private string ewsId;
	}
}
