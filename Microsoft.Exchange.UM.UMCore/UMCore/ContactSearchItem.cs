using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCommon.MessageContent;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class ContactSearchItem : IEquatable<ContactSearchItem>
	{
		private ContactSearchItem()
		{
		}

		internal static StorePropertyDefinition[] ContactSearchPropertyDefinitions
		{
			get
			{
				return ContactSearchItem.contactSearchPropertyDefinitions;
			}
		}

		internal string Id
		{
			get
			{
				return this.id;
			}
		}

		internal string FullName
		{
			get
			{
				return this.fullName;
			}
		}

		internal PhoneNumber Phone
		{
			get
			{
				return this.phone;
			}
		}

		internal PhoneNumber HomePhone
		{
			get
			{
				return this.homePhone;
			}
		}

		internal PhoneNumber BusinessPhone
		{
			get
			{
				return this.businessPhone;
			}
		}

		internal PhoneNumber BusinessPhoneForTts
		{
			get
			{
				return this.businessPhoneForTts;
			}
		}

		internal PhoneNumber MobilePhone
		{
			get
			{
				return this.mobilePhone;
			}
		}

		internal string Alias
		{
			get
			{
				if (this.recipient == null)
				{
					return null;
				}
				return this.recipient.Alias;
			}
		}

		internal bool HasAlias
		{
			get
			{
				return !string.IsNullOrEmpty(this.Alias);
			}
		}

		internal bool HasBusinessAddress
		{
			get
			{
				return !this.businessAddress.IsEmpty;
			}
		}

		internal bool HasBusinessNumber
		{
			get
			{
				if (this.recipient != null)
				{
					return !PhoneNumber.IsNullOrEmpty(this.businessPhoneForTts);
				}
				return !PhoneNumber.IsNullOrEmpty(this.businessPhone);
			}
		}

		internal bool HasHomeAddress
		{
			get
			{
				return !string.IsNullOrEmpty(this.homeAddress);
			}
		}

		internal bool HasOtherAddress
		{
			get
			{
				return !string.IsNullOrEmpty(this.otherAddress);
			}
		}

		internal bool HasMobileNumber
		{
			get
			{
				return !PhoneNumber.IsNullOrEmpty(this.mobilePhone);
			}
		}

		internal bool HasHomeNumber
		{
			get
			{
				return !PhoneNumber.IsNullOrEmpty(this.homePhone);
			}
		}

		internal bool HasEmail1
		{
			get
			{
				return 0 < this.contactEmailAddresses.Count && !string.IsNullOrEmpty(this.contactEmailAddresses[0]);
			}
		}

		internal bool HasEmail2
		{
			get
			{
				return 1 < this.contactEmailAddresses.Count && !string.IsNullOrEmpty(this.contactEmailAddresses[1]);
			}
		}

		internal bool HasEmail3
		{
			get
			{
				return 2 < this.contactEmailAddresses.Count && !string.IsNullOrEmpty(this.contactEmailAddresses[2]);
			}
		}

		internal bool IsGroup { get; private set; }

		internal bool GroupHasEmail { get; private set; }

		internal string PrimarySmtpAddress
		{
			get
			{
				return this.primarySmtpAddress;
			}
			set
			{
				this.primarySmtpAddress = value;
			}
		}

		internal StorePropertyDefinition[] PropertyDefinitions { get; set; }

		internal List<string> ContactEmailAddresses
		{
			get
			{
				return this.contactEmailAddresses;
			}
		}

		internal string Email1DisplayName
		{
			get
			{
				return this.email1DisplayName;
			}
		}

		internal ADRecipient Recipient
		{
			get
			{
				return this.recipient;
			}
		}

		internal string CompanyName
		{
			get
			{
				return this.companyName;
			}
		}

		internal string LastName
		{
			get
			{
				return this.lastName;
			}
		}

		internal string FirstName
		{
			get
			{
				return this.firstName;
			}
		}

		internal string OfficeLocation
		{
			get
			{
				return this.officeLocation;
			}
		}

		internal string ContactHomeAddress
		{
			get
			{
				return this.homeAddress;
			}
		}

		internal LocalizedString BusinessAddress
		{
			get
			{
				return this.businessAddress;
			}
		}

		internal string OtherAddress
		{
			get
			{
				return this.otherAddress;
			}
		}

		internal string HomeAddress
		{
			get
			{
				return this.homeAddress;
			}
		}

		internal string PersonId
		{
			get
			{
				return this.personId;
			}
		}

		internal string GALLinkId
		{
			get
			{
				return this.galLinkId;
			}
		}

		internal string ContactLastNameFirstNameDtmfMap
		{
			get
			{
				return this.lastNameFirstNameDtmfMap;
			}
		}

		internal string DtmfEmailAlias
		{
			get
			{
				return this.emailAliasDtmfMap;
			}
		}

		internal UMDialPlan TargetDialPlan
		{
			get
			{
				return this.targetDialPlan;
			}
			set
			{
				this.targetDialPlan = value;
			}
		}

		internal bool NeedDisambiguation
		{
			get
			{
				return this.needDisambiguation;
			}
			set
			{
				this.needDisambiguation = value;
			}
		}

		internal Participant EmailParticipant
		{
			get
			{
				return this.emailParticipant;
			}
		}

		internal bool IsPersonalContact { get; set; }

		internal bool IsFromRecipientCache
		{
			get
			{
				return !string.IsNullOrEmpty(this.partnerNetworkId) && StringComparer.OrdinalIgnoreCase.Equals(this.partnerNetworkId, WellKnownNetworkNames.RecipientCache);
			}
		}

		public bool Equals(ContactSearchItem other)
		{
			return this.Id.Equals(other.Id, StringComparison.OrdinalIgnoreCase);
		}

		public int CompareTo(ContactSearchItem other)
		{
			return string.Compare(this.Id, other.Id, StringComparison.OrdinalIgnoreCase);
		}

		internal static ContactSearchItem CreateFromRecipient(ADRecipient r)
		{
			if (r == null)
			{
				throw new ArgumentNullException("recipient");
			}
			ContactSearchItem contactSearchItem = new ContactSearchItem();
			contactSearchItem.recipient = r;
			contactSearchItem.fullName = contactSearchItem.recipient.DisplayName.Trim();
			contactSearchItem.primarySmtpAddress = contactSearchItem.recipient.PrimarySmtpAddress.ToString().Trim();
			contactSearchItem.id = contactSearchItem.recipient.Id.ToString();
			foreach (string text in contactSearchItem.recipient.UMDtmfMap)
			{
				if (text.StartsWith("emailAddress:", StringComparison.OrdinalIgnoreCase))
				{
					contactSearchItem.emailAliasDtmfMap = text;
					break;
				}
			}
			if (!string.IsNullOrEmpty(contactSearchItem.recipient.LegacyExchangeDN))
			{
				contactSearchItem.emailParticipant = new Participant(contactSearchItem.recipient.DisplayName, contactSearchItem.recipient.LegacyExchangeDN, "EX");
			}
			else
			{
				SmtpAddress smtpAddress = contactSearchItem.recipient.PrimarySmtpAddress;
				if (SmtpAddress.IsValidSmtpAddress(contactSearchItem.recipient.PrimarySmtpAddress.ToString()))
				{
					contactSearchItem.emailParticipant = new Participant(contactSearchItem.recipient.DisplayName, contactSearchItem.recipient.PrimarySmtpAddress.ToString(), "SMTP");
				}
			}
			IADOrgPerson iadorgPerson = r as IADOrgPerson;
			if (iadorgPerson != null)
			{
				contactSearchItem.phone = ContactSearchItem.CreatePhoneNumber(iadorgPerson.Phone);
				contactSearchItem.businessPhone = contactSearchItem.phone;
				contactSearchItem.mobilePhone = ContactSearchItem.CreatePhoneNumber(iadorgPerson.MobilePhone);
				contactSearchItem.homePhone = ContactSearchItem.CreatePhoneNumber(iadorgPerson.HomePhone);
				string text2 = string.IsNullOrEmpty(iadorgPerson.Office) ? string.Empty : iadorgPerson.Office.Trim();
				string text3 = string.IsNullOrEmpty(iadorgPerson.StreetAddress) ? string.Empty : iadorgPerson.StreetAddress.Trim();
				string text4 = string.IsNullOrEmpty(iadorgPerson.City) ? string.Empty : iadorgPerson.City.Trim();
				string text5 = string.IsNullOrEmpty(iadorgPerson.StateOrProvince) ? string.Empty : iadorgPerson.StateOrProvince.Trim();
				string text6 = string.IsNullOrEmpty(iadorgPerson.PostalCode) ? string.Empty : iadorgPerson.PostalCode.Trim();
				string text7 = (string)iadorgPerson.CountryOrRegion;
				if (!string.IsNullOrEmpty(text2) || !string.IsNullOrEmpty(text3) || !string.IsNullOrEmpty(text4) || !string.IsNullOrEmpty(text5) || !string.IsNullOrEmpty(text6) || !string.IsNullOrEmpty(text7))
				{
					contactSearchItem.businessAddress = Strings.OfficeAddress(text2, text3, text4, text5, text6, text7);
				}
			}
			return contactSearchItem;
		}

		internal static ContactSearchItem CreateSearchItem(object[] propertyArray, StorePropertyDefinition[] propertyDefinitions)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, null, "Entering ContactSearchItem.CreateSearchItem", new object[0]);
			ContactSearchItem contactSearchItem = new ContactSearchItem();
			contactSearchItem.PropertyDefinitions = propertyDefinitions;
			contactSearchItem.fullName = contactSearchItem.GetProperty(propertyArray, StoreObjectSchema.DisplayName);
			contactSearchItem.itemClass = contactSearchItem.GetProperty(propertyArray, StoreObjectSchema.ItemClass);
			contactSearchItem.partnerNetworkId = contactSearchItem.GetProperty(propertyArray, ContactSchema.PartnerNetworkId);
			contactSearchItem.IsPersonalContact = true;
			int num = Array.IndexOf<StorePropertyDefinition>(propertyDefinitions, ItemSchema.Id);
			VersionedId versionedId = (VersionedId)propertyArray[num];
			contactSearchItem.id = versionedId.ToBase64String();
			if (ObjectClass.IsOfClass(contactSearchItem.itemClass, "IPM.DistList"))
			{
				if (string.IsNullOrEmpty(contactSearchItem.fullName))
				{
					CallIdTracer.TraceWarning(ExTraceGlobals.DirectorySearchTracer, null, "The contact search item not created because fullName is null or empty", new object[0]);
					return null;
				}
				contactSearchItem.lastNameFirstNameDtmfMap = DtmfString.DtmfEncode(contactSearchItem.fullName);
			}
			else if (string.IsNullOrEmpty(contactSearchItem.fullName))
			{
				contactSearchItem.companyName = contactSearchItem.GetProperty(propertyArray, ContactSchema.CompanyName);
				if (string.IsNullOrEmpty(contactSearchItem.companyName))
				{
					CallIdTracer.TraceWarning(ExTraceGlobals.DirectorySearchTracer, null, "The contact search item not created because fullName and company is null or empty", new object[0]);
					return null;
				}
				contactSearchItem.lastNameFirstNameDtmfMap = DtmfString.DtmfEncode(contactSearchItem.companyName);
				contactSearchItem.fullName = contactSearchItem.companyName;
			}
			else
			{
				contactSearchItem.lastName = contactSearchItem.GetProperty(propertyArray, ContactSchema.Surname);
				contactSearchItem.firstName = contactSearchItem.GetProperty(propertyArray, ContactSchema.GivenName);
				contactSearchItem.email1DisplayName = contactSearchItem.GetProperty(propertyArray, ContactSchema.Email1DisplayName);
				PersonId property = XsoUtil.GetProperty<PersonId>(propertyArray, ContactSchema.PersonId, propertyDefinitions);
				if (property != null)
				{
					contactSearchItem.personId = property.ToBase64String();
				}
				contactSearchItem.galLinkId = XsoUtil.GetProperty<Guid>(propertyArray, ContactSchema.GALLinkID, propertyDefinitions).ToString();
				string text = string.Empty;
				if (!string.IsNullOrEmpty(contactSearchItem.lastName))
				{
					text += contactSearchItem.lastName;
				}
				if (!string.IsNullOrEmpty(contactSearchItem.firstName))
				{
					text += contactSearchItem.firstName;
				}
				if (string.IsNullOrEmpty(text))
				{
					contactSearchItem = null;
				}
				else
				{
					contactSearchItem.lastNameFirstNameDtmfMap = DtmfString.DtmfEncode(text);
				}
			}
			return contactSearchItem;
		}

		internal static ContactSearchItem CreateSelectedResult(MailboxSession session, object[] propertyArray, StorePropertyDefinition[] propertyDefinitions)
		{
			ContactSearchItem contactSearchItem = new ContactSearchItem();
			contactSearchItem.PropertyDefinitions = propertyDefinitions;
			contactSearchItem.mailboxGuid = session.MailboxGuid;
			contactSearchItem.propertyArray = propertyArray;
			contactSearchItem.itemClass = contactSearchItem.GetProperty(propertyArray, StoreObjectSchema.ItemClass);
			contactSearchItem.fullName = contactSearchItem.GetProperty(propertyArray, StoreObjectSchema.DisplayName);
			int num = Array.IndexOf<StorePropertyDefinition>(propertyDefinitions, ItemSchema.Id);
			VersionedId versionedId = (VersionedId)propertyArray[num];
			contactSearchItem.id = versionedId.ToBase64String();
			contactSearchItem.IsPersonalContact = true;
			if (ObjectClass.IsOfClass(contactSearchItem.itemClass, "IPM.DistList"))
			{
				contactSearchItem.InitializeSelectedGroup(session, versionedId, propertyArray);
			}
			else
			{
				contactSearchItem.InitializeSelectedPersonalContact(propertyArray);
			}
			return contactSearchItem;
		}

		internal static void AddSearchItems(UMMailboxRecipient subscriber, IDictionary<PropertyDefinition, object> searchFilter, List<ContactSearchItem> list, int maxItemCount)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, null, "AddSearchItems maxItemCount = {0}", new object[]
			{
				maxItemCount
			});
			ExAssert.RetailAssert(maxItemCount > 0, "maxItemCount {0} <= 0", new object[]
			{
				maxItemCount
			});
			if (list.Count >= maxItemCount)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, null, "List already has {0} items, maxItemCount = {1}", new object[]
				{
					list.Count,
					maxItemCount
				});
				return;
			}
			using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = subscriber.CreateSessionLock())
			{
				using (ContactsFolder contactsFolder = ContactsFolder.Bind(mailboxSessionLock.Session, mailboxSessionLock.Session.GetDefaultFolderId(DefaultFolderType.Contacts)))
				{
					object[][] array = contactsFolder.FindNamesView(searchFilter, maxItemCount - list.Count, new SortBy[]
					{
						new SortBy(StoreObjectSchema.DisplayName, SortOrder.Ascending)
					}, ContactSearchItem.ContactSearchPropertyDefinitions);
					for (int i = 0; i < array.Length; i++)
					{
						ContactSearchItem contactSearchItem = ContactSearchItem.CreateSearchItem(array[i], ContactSearchItem.ContactSearchPropertyDefinitions);
						if (contactSearchItem != null && !list.Contains(contactSearchItem))
						{
							list.Add(contactSearchItem);
						}
					}
				}
			}
		}

		internal static void AddMOWASearchItems(UMMailboxRecipient subscriber, IDictionary<PropertyDefinition, object> searchFilter, List<ContactSearchItem> list, int maxItemCount)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, null, "Entering ContactSearchItem.AddMOWASearchItems maxItemCount = {0}", new object[]
			{
				maxItemCount
			});
			ExAssert.RetailAssert(maxItemCount > 0, "maxItemCount {0} <= 0", new object[]
			{
				maxItemCount
			});
			if (list.Count >= maxItemCount)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, null, "List already has {0} items, maxItemCount = {1}", new object[]
				{
					list.Count,
					maxItemCount
				});
				return;
			}
			DefaultFolderType defaultFolderType = DefaultFolderType.AllContacts;
			List<QueryFilter> list2 = new List<QueryFilter>(searchFilter.Count);
			foreach (KeyValuePair<PropertyDefinition, object> keyValuePair in searchFilter)
			{
				QueryFilter item = new ComparisonFilter(ComparisonOperator.Equal, keyValuePair.Key, keyValuePair.Value);
				list2.Add(item);
			}
			QueryFilter queryFilter = QueryFilter.AndTogether(list2.ToArray());
			using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = subscriber.CreateSessionLock())
			{
				using (Folder folder = Folder.Bind(mailboxSessionLock.Session, defaultFolderType))
				{
					using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.None, queryFilter, null, ContactSearchItem.ContactSearchPropertyDefinitions))
					{
						object[][] rows = queryResult.GetRows(100);
						Dictionary<Tuple<string, string, string>, ContactSearchItem> dictionary = new Dictionary<Tuple<string, string, string>, ContactSearchItem>(rows.Length);
						while (rows != null && rows.Length > 0)
						{
							int num = 0;
							while (num < rows.Length && dictionary.Count + list.Count < maxItemCount)
							{
								ContactSearchItem contactSearchItem = ContactSearchItem.CreateSearchItem(rows[num], ContactSearchItem.ContactSearchPropertyDefinitions);
								if (contactSearchItem == null)
								{
									CallIdTracer.TraceWarning(ExTraceGlobals.DirectorySearchTracer, null, "The contact search item is not added to grammar, because its null", new object[0]);
								}
								else
								{
									string item2 = (contactSearchItem.PersonId != null) ? contactSearchItem.PersonId : Guid.NewGuid().ToString();
									Tuple<string, string, string> key = Tuple.Create<string, string, string>(item2, contactSearchItem.FullName, contactSearchItem.GALLinkId);
									if (!dictionary.ContainsKey(key))
									{
										dictionary.Add(key, contactSearchItem);
									}
								}
								num++;
							}
							if (dictionary.Count + list.Count >= maxItemCount)
							{
								break;
							}
							rows = queryResult.GetRows(100);
						}
						foreach (ContactSearchItem item3 in dictionary.Values)
						{
							list.Add(item3);
						}
					}
				}
			}
		}

		internal static ContactSearchItem GetSelectedSearchItemFromId(UMMailboxRecipient subscriber, string id)
		{
			ContactSearchItem result = null;
			IDictionary<PropertyDefinition, object> dictionary = new SortedDictionary<PropertyDefinition, object>();
			dictionary.Add(ItemSchema.Id, VersionedId.Deserialize(id));
			using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = subscriber.CreateSessionLock())
			{
				using (ContactsFolder contactsFolder = ContactsFolder.Bind(mailboxSessionLock.Session, mailboxSessionLock.Session.GetDefaultFolderId(DefaultFolderType.Contacts)))
				{
					object[][] array = contactsFolder.FindNamesView(dictionary, 0, null, PersonalContactInfo.ContactPropertyDefinitions);
					if (array.Length == 1)
					{
						result = ContactSearchItem.CreateSelectedResult(mailboxSessionLock.Session, array[0], PersonalContactInfo.ContactPropertyDefinitions);
					}
				}
			}
			return result;
		}

		internal void SetEmailAddresses(ActivityManager manager)
		{
			for (int i = 0; i < 3; i++)
			{
				string text = null;
				if (i < this.ContactEmailAddresses.Count)
				{
					text = this.ContactEmailAddresses[i];
				}
				manager.WriteVariable("email" + Convert.ToString(i + 1, CultureInfo.InvariantCulture), text);
				manager.WriteVariable("haveEmail" + Convert.ToString(i + 1, CultureInfo.InvariantCulture), !string.IsNullOrEmpty(text));
			}
		}

		internal void SetVariablesForTts(ActivityManager manager, BaseUMCallSession vo)
		{
			this.SetEmailAddresses(manager);
			this.SetHomeNumber(manager, vo);
			this.SetBusinessNumber(manager, vo);
			this.SetMobileNumber(manager, vo);
		}

		internal void SetBusinessPhoneForDialPlan(UMDialPlan originatingDialPlan)
		{
			PIIMessage data = PIIMessage.Create(PIIType._UserDisplayName, this.recipient.DisplayName);
			CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, data, "SetBusinessPhoneForDialPlan() Recipient=_UserDisplayName DialPlan={0}.", new object[]
			{
				originatingDialPlan.Name
			});
			PhoneNumber phoneNumber = null;
			DialPermissions.GetBestOfficeNumber(this.recipient, originatingDialPlan, out phoneNumber);
			this.phone = phoneNumber;
			this.businessPhone = phoneNumber;
			this.businessPhoneForTts = phoneNumber;
			IADOrgPerson iadorgPerson = this.recipient as IADOrgPerson;
			string text = null;
			if (iadorgPerson != null && originatingDialPlan.URIType == UMUriType.SipName && DialPermissions.TryGetDialplanExtension(this.recipient, originatingDialPlan, out text))
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, "SetBusinessPhoneForDialPlan() This is a UMEnabled result in SipName dialplan. RTCSipLine = '{0}' GalPhone = '{1}'", new object[]
				{
					iadorgPerson.RtcSipLine,
					iadorgPerson.Phone
				});
				string text2 = Utils.TrimSpaces(iadorgPerson.RtcSipLine);
				string text3 = Utils.TrimSpaces(iadorgPerson.Phone);
				if (text2 != null)
				{
					int num = text2.IndexOf("tel:", StringComparison.OrdinalIgnoreCase);
					if (num != -1 && text2.Length > num + 4)
					{
						text2 = text2.Substring(num + 4);
					}
				}
				PhoneNumber phoneNumber2 = null;
				if (text2 != null && PhoneNumber.TryParse(originatingDialPlan, text2, out phoneNumber2))
				{
					this.businessPhoneForTts = phoneNumber2;
				}
				else if (text3 != null && PhoneNumber.TryParse(originatingDialPlan, text3, out phoneNumber2))
				{
					this.businessPhoneForTts = phoneNumber2;
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, "SetBusinessPhoneForDialPlan() This is a UMEnabled result in SipName dialplan. Setting businessPhoneForTTS  = '{0}'", new object[]
				{
					this.businessPhoneForTts
				});
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.DirectorySearchTracer, this, "SetBusinessPhoneForDialPlan() businessPhone = {0} businessPhoneForTts = {1}", new object[]
			{
				this.businessPhone,
				this.businessPhoneForTts
			});
		}

		internal ContactInfo ToContactInfo(FoundByType selectedPhoneType)
		{
			ContactInfo result = null;
			if (this.IsPersonalContact)
			{
				result = new PersonalContactInfo(this.mailboxGuid, this.propertyArray, selectedPhoneType, false);
			}
			else
			{
				IADOrgPerson iadorgPerson = this.Recipient as IADOrgPerson;
				if (iadorgPerson != null)
				{
					result = new ADContactInfo(iadorgPerson, selectedPhoneType);
				}
			}
			return result;
		}

		private static PhoneNumber CreatePhoneNumber(string dialString)
		{
			string text = string.IsNullOrEmpty(dialString) ? string.Empty : dialString.Trim();
			PhoneNumber result = null;
			if (PhoneNumber.TryParse(text, out result))
			{
				return result;
			}
			return null;
		}

		private void InitializeSelectedGroup(MailboxSession session, VersionedId vid, object[] propertyArray)
		{
			using (DistributionList distributionList = (DistributionList)Item.Bind(session, vid))
			{
				this.IsGroup = true;
				Participant[] array = DistributionList.ExpandDeep(session, distributionList.StoreObjectId);
				if (array.Length > 0)
				{
					this.GroupHasEmail = true;
					this.emailParticipant = distributionList.GetAsParticipant();
				}
			}
		}

		private void InitializeSelectedPersonalContact(object[] propertyArray)
		{
			this.lastName = this.GetProperty(propertyArray, ContactSchema.Surname);
			this.firstName = this.GetProperty(propertyArray, ContactSchema.GivenName);
			string property = this.GetProperty(propertyArray, ContactSchema.Email1EmailAddress);
			if (!string.IsNullOrEmpty(property))
			{
				this.InitializeEmailAddresses(property, ContactSchema.Email1AddrType);
			}
			this.email1DisplayName = this.GetProperty(propertyArray, ContactSchema.Email1DisplayName);
			string property2 = this.GetProperty(propertyArray, ContactSchema.Email2EmailAddress);
			if (!string.IsNullOrEmpty(property2))
			{
				this.InitializeEmailAddresses(property2, ContactSchema.Email2AddrType);
			}
			string property3 = this.GetProperty(propertyArray, ContactSchema.Email3EmailAddress);
			if (!string.IsNullOrEmpty(property3))
			{
				this.InitializeEmailAddresses(property3, ContactSchema.Email3AddrType);
			}
			this.companyName = this.GetProperty(propertyArray, ContactSchema.CompanyName);
			this.officeLocation = this.GetProperty(propertyArray, ContactSchema.OfficeLocation);
			this.homeAddress = this.GetProperty(propertyArray, ContactSchema.HomeAddress);
			string property4 = this.GetProperty(propertyArray, ContactSchema.BusinessAddress);
			if (!string.IsNullOrEmpty(property4))
			{
				this.businessAddress = new LocalizedString(property4);
			}
			this.otherAddress = this.GetProperty(propertyArray, ContactSchema.OtherAddress);
			this.businessPhone = ContactSearchItem.CreatePhoneNumber(this.GetProperty(propertyArray, ContactSchema.BusinessPhoneNumber) ?? this.GetProperty(propertyArray, ContactSchema.BusinessPhoneNumber2));
			this.homePhone = ContactSearchItem.CreatePhoneNumber(this.GetProperty(propertyArray, ContactSchema.HomePhone) ?? this.GetProperty(propertyArray, ContactSchema.HomePhone2));
			this.mobilePhone = ContactSearchItem.CreatePhoneNumber(this.GetProperty(propertyArray, ContactSchema.MobilePhone) ?? this.GetProperty(propertyArray, ContactSchema.MobilePhone2));
		}

		private void InitializeEmailAddresses(string emailAddress, StorePropertyDefinition addrTypePropertyId)
		{
			string property = this.GetProperty(this.propertyArray, addrTypePropertyId);
			if (!string.IsNullOrEmpty(property))
			{
				if (property.Equals("EX", StringComparison.OrdinalIgnoreCase))
				{
					Participant source = new Participant(this.fullName, emailAddress, "EX");
					Participant participant = Participant.TryConvertTo(source, "SMTP");
					if (participant != null)
					{
						this.contactEmailAddresses.Add(participant.EmailAddress);
						if (this.emailParticipant == null)
						{
							this.emailParticipant = participant;
							this.primarySmtpAddress = emailAddress;
							return;
						}
					}
				}
				else if (property.Equals("SMTP", StringComparison.OrdinalIgnoreCase))
				{
					if (this.emailParticipant == null)
					{
						this.emailParticipant = new Participant(this.fullName, emailAddress, "SMTP");
						this.primarySmtpAddress = emailAddress;
					}
					this.contactEmailAddresses.Add(emailAddress);
				}
			}
		}

		private string GetProperty(object[] results, StorePropertyDefinition propertyId)
		{
			int num = Array.IndexOf<StorePropertyDefinition>(this.PropertyDefinitions, propertyId);
			object obj = results[num];
			string text = obj as string;
			if (!string.IsNullOrEmpty(text))
			{
				text = text.Trim();
			}
			return text;
		}

		private void SetHomeNumber(ActivityManager manager, BaseUMCallSession vo)
		{
			manager.WriteVariable("haveDialableHomeNumber", Util.IsDialableNumber(this.homePhone, vo, this.recipient));
		}

		private void SetBusinessNumber(ActivityManager manager, BaseUMCallSession vo)
		{
			manager.WriteVariable("haveDialableBusinessNumber", Util.IsDialableNumber(this.businessPhone, vo, this.recipient));
		}

		private void SetMobileNumber(ActivityManager manager, BaseUMCallSession vo)
		{
			manager.WriteVariable("haveDialableMobileNumber", Util.IsDialableNumber(this.mobilePhone, vo, this.recipient));
		}

		private static StorePropertyDefinition[] contactSearchPropertyDefinitions = new StorePropertyDefinition[]
		{
			ItemSchema.Id,
			StoreObjectSchema.ItemClass,
			ContactSchema.GivenName,
			ContactSchema.Surname,
			StoreObjectSchema.DisplayName,
			ContactSchema.Email1DisplayName,
			ContactSchema.CompanyName,
			ContactSchema.PersonId,
			ContactSchema.GALLinkID,
			ContactSchema.PartnerNetworkId
		};

		private string itemClass;

		private string id;

		private string fullName;

		private PhoneNumber phone;

		private PhoneNumber homePhone;

		private PhoneNumber businessPhone;

		private PhoneNumber businessPhoneForTts;

		private PhoneNumber mobilePhone;

		private string primarySmtpAddress;

		private List<string> contactEmailAddresses = new List<string>();

		private string email1DisplayName;

		private ADRecipient recipient;

		private string companyName;

		private string lastName;

		private string firstName;

		private string officeLocation;

		private string homeAddress;

		private string personId;

		private string galLinkId;

		private string partnerNetworkId;

		private LocalizedString businessAddress;

		private string otherAddress;

		private string lastNameFirstNameDtmfMap;

		private string emailAliasDtmfMap;

		private UMDialPlan targetDialPlan;

		private bool needDisambiguation;

		private Participant emailParticipant;

		private object[] propertyArray;

		private Guid mailboxGuid;
	}
}
