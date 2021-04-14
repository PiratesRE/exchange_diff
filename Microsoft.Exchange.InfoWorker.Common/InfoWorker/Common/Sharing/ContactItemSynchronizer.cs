using System;
using System.IO;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.InfoWorker.Common.Sharing
{
	internal class ContactItemSynchronizer : ItemSynchronizer
	{
		public ContactItemSynchronizer(LocalFolder localFolder) : base(localFolder)
		{
		}

		public override void Sync(ItemType item, MailboxSession mailboxSession, ExchangeService exchangeService)
		{
			ContactItemType contactItemType = item as ContactItemType;
			if (contactItemType == null)
			{
				ItemSynchronizer.Tracer.TraceDebug<ContactItemSynchronizer, string>((long)this.GetHashCode(), "{0}: Found non-Contact item in a Contact folder: {1}. Skipping.", this, item.ItemId.Id);
				return;
			}
			string id = contactItemType.ItemId.Id;
			Contact contact = base.FindLocalCopy(id, mailboxSession) as Contact;
			bool flag = false;
			try
			{
				if (contactItemType.Sensitivity != SensitivityChoicesType.Normal)
				{
					ItemSynchronizer.Tracer.TraceDebug<ContactItemSynchronizer, string>((long)this.GetHashCode(), "{0}: Remote item {1} is private and will not be synced.", this, id);
					return;
				}
				if (contact == null)
				{
					ItemSynchronizer.Tracer.TraceDebug<ContactItemSynchronizer, string>((long)this.GetHashCode(), "{0}: Creating a local copy of Contact item: {1}", this, id);
					contact = Contact.Create(mailboxSession, this.localFolder.Id);
				}
				ItemSynchronizer.Tracer.TraceDebug<ContactItemSynchronizer, string>((long)this.GetHashCode(), "{0}: Updating properties for Contact item: {1}", this, id);
				contact.SetOrDeleteProperty(ContactSchema.AssistantName, contactItemType.AssistantName);
				contact.SetOrDeleteProperty(ContactSchema.BusinessHomePage, contactItemType.BusinessHomePage);
				contact.SetOrDeleteProperty(ContactSchema.CompanyName, contactItemType.CompanyName);
				contact.SetOrDeleteProperty(ContactSchema.Department, contactItemType.Department);
				contact.SetOrDeleteProperty(StoreObjectSchema.DisplayName, contactItemType.DisplayName);
				contact.SetOrDeleteProperty(ContactSchema.Manager, contactItemType.Manager);
				contact.SetOrDeleteProperty(ContactSchema.Mileage, contactItemType.Mileage);
				contact.SetOrDeleteProperty(ContactSchema.OfficeLocation, contactItemType.OfficeLocation);
				contact.SetOrDeleteProperty(ContactSchema.Profession, contactItemType.Profession);
				contact.SetOrDeleteProperty(ContactSchema.SpouseName, contactItemType.SpouseName);
				contact.SetOrDeleteProperty(ContactSchema.Title, contactItemType.JobTitle);
				if (contactItemType.CompleteName != null)
				{
					contact.SetOrDeleteProperty(ContactSchema.DisplayNamePrefix, contactItemType.CompleteName.Title);
					contact.SetOrDeleteProperty(ContactSchema.FullName, contactItemType.CompleteName.FullName);
					contact.SetOrDeleteProperty(ContactSchema.Generation, contactItemType.CompleteName.Suffix);
					contact.SetOrDeleteProperty(ContactSchema.GivenName, contactItemType.CompleteName.FirstName);
					contact.SetOrDeleteProperty(ContactSchema.Initials, contactItemType.CompleteName.Initials);
					contact.SetOrDeleteProperty(ContactSchema.MiddleName, contactItemType.CompleteName.MiddleName);
					contact.SetOrDeleteProperty(ContactSchema.Nickname, contactItemType.CompleteName.Nickname);
					contact.SetOrDeleteProperty(ContactSchema.Surname, contactItemType.CompleteName.LastName);
				}
				else
				{
					contact.DeleteProperties(new PropertyDefinition[]
					{
						ContactSchema.DisplayNamePrefix,
						ContactSchema.FullName,
						ContactSchema.Generation,
						ContactSchema.GivenName,
						ContactSchema.Initials,
						ContactSchema.MiddleName,
						ContactSchema.Nickname,
						ContactSchema.Surname
					});
				}
				contact.FileAs = this.Convert(contactItemType.FileAsMapping);
				if (contactItemType.Children != null && contactItemType.Children.Length > 0)
				{
					contact[ContactSchema.Children] = contactItemType.Children;
				}
				else
				{
					contact.Delete(ContactSchema.Children);
				}
				contact.Categories.Clear();
				if (contactItemType.Categories != null)
				{
					contact.Categories.AddRange(contactItemType.Categories);
				}
				if (contactItemType.BirthdaySpecified)
				{
					contact[ContactSchema.Birthday] = contactItemType.Birthday;
				}
				else
				{
					contact.Delete(ContactSchema.Birthday);
				}
				if (contactItemType.WeddingAnniversarySpecified)
				{
					contact[ContactSchema.WeddingAnniversary] = contactItemType.WeddingAnniversary;
				}
				else
				{
					contact.Delete(ContactSchema.WeddingAnniversary);
				}
				if (contactItemType.PhoneNumbers != null)
				{
					foreach (PhoneNumberDictionaryEntryType phoneNumberDictionaryEntryType in contactItemType.PhoneNumbers)
					{
						contact.SetOrDeleteProperty(this.Convert(phoneNumberDictionaryEntryType.Key), phoneNumberDictionaryEntryType.Value);
					}
				}
				contact.EmailAddresses.Clear();
				if (contactItemType.EmailAddresses != null)
				{
					foreach (EmailAddressDictionaryEntryType emailAddressDictionaryEntryType in contactItemType.EmailAddresses)
					{
						contact.EmailAddresses.Add(this.Convert(emailAddressDictionaryEntryType.Key), new Participant(null, emailAddressDictionaryEntryType.Value, null));
					}
				}
				if (contactItemType.ImAddresses != null && contactItemType.ImAddresses.Length > 0 && contactItemType.ImAddresses[0].Value != null)
				{
					contact.ImAddress = contactItemType.ImAddresses[0].Value;
				}
				else
				{
					contact.Delete(ContactSchema.IMAddress);
				}
				this.SyncNotes(contactItemType, contact);
				this.SyncPhysicalAddresses(contactItemType, contact);
				ItemSynchronizer.Tracer.TraceDebug<ContactItemSynchronizer, string>((long)this.GetHashCode(), "{0}: Copying Id: {1}", this, id);
				contact[SharingSchema.ExternalSharingMasterId] = contactItemType.ItemId.Id;
				ItemSynchronizer.Tracer.TraceDebug<ContactItemSynchronizer, string>((long)this.GetHashCode(), "{0}: Saving local item {1}", this, id);
				contact.Save(SaveMode.NoConflictResolution);
				flag = true;
			}
			finally
			{
				if (!flag && contact != null && contact.Id != null)
				{
					this.localFolder.SelectItemToDelete(contact.Id.ObjectId);
				}
				if (contact != null)
				{
					contact.Dispose();
				}
			}
			PerformanceCounters.ContactItemsSynced.Increment();
		}

		private void SyncNotes(ContactItemType remoteItem, Contact localItem)
		{
			string text = string.Empty;
			BodyFormat bodyFormat = BodyFormat.TextPlain;
			if (remoteItem.Body != null)
			{
				text = remoteItem.Body.Value;
				bodyFormat = this.Convert(remoteItem.Body.BodyType1);
				ItemSynchronizer.Tracer.TraceDebug<ContactItemSynchronizer, string>((long)this.GetHashCode(), "{0}: Copying body: {1}", this, text);
			}
			using (TextWriter textWriter = localItem.Body.OpenTextWriter(bodyFormat))
			{
				textWriter.Write(text);
			}
		}

		private void SyncPhysicalAddresses(ContactItemType remoteItem, Contact localItem)
		{
			localItem.DeleteProperties(new PropertyDefinition[]
			{
				ContactSchema.WorkAddressCity,
				ContactSchema.WorkAddressCountry,
				ContactSchema.WorkAddressPostalCode,
				ContactSchema.WorkAddressState,
				ContactSchema.WorkAddressStreet,
				ContactSchema.HomeCity,
				ContactSchema.HomeCountry,
				ContactSchema.HomePostalCode,
				ContactSchema.HomeState,
				ContactSchema.HomeStreet,
				ContactSchema.OtherCity,
				ContactSchema.OtherCountry,
				ContactSchema.OtherPostalCode,
				ContactSchema.OtherState,
				ContactSchema.OtherStreet
			});
			if (remoteItem.PhysicalAddresses != null)
			{
				foreach (PhysicalAddressDictionaryEntryType physicalAddressDictionaryEntryType in remoteItem.PhysicalAddresses)
				{
					switch (physicalAddressDictionaryEntryType.Key)
					{
					case PhysicalAddressKeyType.Home:
						localItem.SetOrDeleteProperty(ContactSchema.HomeCity, physicalAddressDictionaryEntryType.City);
						localItem.SetOrDeleteProperty(ContactSchema.HomeCountry, physicalAddressDictionaryEntryType.CountryOrRegion);
						localItem.SetOrDeleteProperty(ContactSchema.HomePostalCode, physicalAddressDictionaryEntryType.PostalCode);
						localItem.SetOrDeleteProperty(ContactSchema.HomeState, physicalAddressDictionaryEntryType.State);
						localItem.SetOrDeleteProperty(ContactSchema.HomeStreet, physicalAddressDictionaryEntryType.Street);
						break;
					case PhysicalAddressKeyType.Business:
						localItem.SetOrDeleteProperty(ContactSchema.WorkAddressCity, physicalAddressDictionaryEntryType.City);
						localItem.SetOrDeleteProperty(ContactSchema.WorkAddressCountry, physicalAddressDictionaryEntryType.CountryOrRegion);
						localItem.SetOrDeleteProperty(ContactSchema.WorkAddressPostalCode, physicalAddressDictionaryEntryType.PostalCode);
						localItem.SetOrDeleteProperty(ContactSchema.WorkAddressState, physicalAddressDictionaryEntryType.State);
						localItem.SetOrDeleteProperty(ContactSchema.WorkAddressStreet, physicalAddressDictionaryEntryType.Street);
						break;
					case PhysicalAddressKeyType.Other:
						localItem.SetOrDeleteProperty(ContactSchema.OtherCity, physicalAddressDictionaryEntryType.City);
						localItem.SetOrDeleteProperty(ContactSchema.OtherCountry, physicalAddressDictionaryEntryType.CountryOrRegion);
						localItem.SetOrDeleteProperty(ContactSchema.OtherPostalCode, physicalAddressDictionaryEntryType.PostalCode);
						localItem.SetOrDeleteProperty(ContactSchema.OtherState, physicalAddressDictionaryEntryType.State);
						localItem.SetOrDeleteProperty(ContactSchema.OtherStreet, physicalAddressDictionaryEntryType.Street);
						break;
					}
				}
			}
			if (remoteItem.PostalAddressIndexSpecified)
			{
				localItem[ContactSchema.PostalAddressId] = this.Convert(remoteItem.PostalAddressIndex);
				return;
			}
			localItem.Delete(ContactSchema.PostalAddressId);
		}

		protected override Item Bind(MailboxSession mailboxSession, StoreId storeId)
		{
			return Contact.Bind(mailboxSession, storeId, new PropertyDefinition[0]);
		}

		private EmailAddressIndex Convert(EmailAddressKeyType type)
		{
			switch (type)
			{
			case EmailAddressKeyType.EmailAddress1:
				return EmailAddressIndex.Email1;
			case EmailAddressKeyType.EmailAddress2:
				return EmailAddressIndex.Email2;
			case EmailAddressKeyType.EmailAddress3:
				return EmailAddressIndex.Email3;
			default:
				throw new InvalidContactException();
			}
		}

		private PhysicalAddressType Convert(PhysicalAddressIndexType indexType)
		{
			switch (indexType)
			{
			case PhysicalAddressIndexType.None:
				return PhysicalAddressType.None;
			case PhysicalAddressIndexType.Home:
				return PhysicalAddressType.Home;
			case PhysicalAddressIndexType.Business:
				return PhysicalAddressType.Business;
			case PhysicalAddressIndexType.Other:
				return PhysicalAddressType.Other;
			default:
				throw new InvalidContactException();
			}
		}

		private BodyFormat Convert(BodyTypeType bodyType)
		{
			switch (bodyType)
			{
			case BodyTypeType.HTML:
				return BodyFormat.TextHtml;
			case BodyTypeType.Text:
				return BodyFormat.TextPlain;
			default:
				throw new InvalidContactException();
			}
		}

		private FileAsMapping Convert(FileAsMappingType type)
		{
			switch (type)
			{
			case FileAsMappingType.None:
				return FileAsMapping.None;
			case FileAsMappingType.LastCommaFirst:
				return FileAsMapping.LastCommaFirst;
			case FileAsMappingType.FirstSpaceLast:
				return FileAsMapping.FirstSpaceLast;
			case FileAsMappingType.Company:
				return FileAsMapping.Company;
			case FileAsMappingType.LastCommaFirstCompany:
				return FileAsMapping.LastCommaFirstCompany;
			case FileAsMappingType.CompanyLastFirst:
				return FileAsMapping.CompanyLastFirst;
			case FileAsMappingType.LastFirst:
				return FileAsMapping.LastFirst;
			case FileAsMappingType.LastFirstCompany:
				return FileAsMapping.LastFirstCompany;
			case FileAsMappingType.CompanyLastCommaFirst:
				return FileAsMapping.CompanyLastCommaFirst;
			case FileAsMappingType.LastFirstSuffix:
				return FileAsMapping.LastFirstSuffix;
			case FileAsMappingType.LastSpaceFirstCompany:
				return FileAsMapping.LastSpaceFirstCompany;
			case FileAsMappingType.CompanyLastSpaceFirst:
				return FileAsMapping.CompanyLastSpaceFirst;
			case FileAsMappingType.LastSpaceFirst:
				return FileAsMapping.LastSpaceFirst;
			case FileAsMappingType.DisplayName:
				return FileAsMapping.DisplayName;
			case FileAsMappingType.FirstName:
				return FileAsMapping.GivenName;
			case FileAsMappingType.LastFirstMiddleSuffix:
				return FileAsMapping.LastFirstMiddleSuffix;
			case FileAsMappingType.LastName:
				return FileAsMapping.LastName;
			case FileAsMappingType.Empty:
				return FileAsMapping.Empty;
			default:
				throw new InvalidContactException();
			}
		}

		private PropertyDefinition Convert(PhoneNumberKeyType type)
		{
			switch (type)
			{
			case PhoneNumberKeyType.AssistantPhone:
				return ContactSchema.AssistantPhoneNumber;
			case PhoneNumberKeyType.BusinessFax:
				return ContactSchema.WorkFax;
			case PhoneNumberKeyType.BusinessPhone:
				return ContactSchema.BusinessPhoneNumber;
			case PhoneNumberKeyType.BusinessPhone2:
				return ContactSchema.BusinessPhoneNumber2;
			case PhoneNumberKeyType.Callback:
				return ContactSchema.CallbackPhone;
			case PhoneNumberKeyType.CarPhone:
				return ContactSchema.CarPhone;
			case PhoneNumberKeyType.CompanyMainPhone:
				return ContactSchema.OrganizationMainPhone;
			case PhoneNumberKeyType.HomeFax:
				return ContactSchema.HomeFax;
			case PhoneNumberKeyType.HomePhone:
				return ContactSchema.HomePhone;
			case PhoneNumberKeyType.HomePhone2:
				return ContactSchema.HomePhone2;
			case PhoneNumberKeyType.Isdn:
				return ContactSchema.InternationalIsdnNumber;
			case PhoneNumberKeyType.MobilePhone:
				return ContactSchema.MobilePhone;
			case PhoneNumberKeyType.OtherFax:
				return ContactSchema.OtherFax;
			case PhoneNumberKeyType.OtherTelephone:
				return ContactSchema.OtherTelephone;
			case PhoneNumberKeyType.Pager:
				return ContactSchema.Pager;
			case PhoneNumberKeyType.PrimaryPhone:
				return ContactSchema.PrimaryTelephoneNumber;
			case PhoneNumberKeyType.RadioPhone:
				return ContactSchema.RadioPhone;
			case PhoneNumberKeyType.Telex:
				return ContactSchema.TelexNumber;
			case PhoneNumberKeyType.TtyTddPhone:
				return ContactSchema.TtyTddPhoneNumber;
			default:
				throw new InvalidContactException();
			}
		}
	}
}
