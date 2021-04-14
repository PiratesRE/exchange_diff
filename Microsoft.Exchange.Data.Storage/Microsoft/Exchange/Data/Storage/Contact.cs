using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class Contact : ContactBase, IContact, IContactBase, IItem, IStoreObject, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		internal Contact(ICoreItem coreItem) : base(coreItem)
		{
			if (base.IsNew)
			{
				this.Initialize();
			}
			this.completeName = new Contact.CallbackCompleteName(this);
			this.emailAddresses = new Contact.EmailAddressDictionary(this);
		}

		public static Contact Create(StoreSession session, StoreId contactFolderId)
		{
			return ItemBuilder.CreateNewItem<Contact>(session, contactFolderId, ItemCreateInfo.ContactInfo);
		}

		public new static Contact Bind(StoreSession session, StoreId storeId, params PropertyDefinition[] propsToReturn)
		{
			return Contact.Bind(session, storeId, (ICollection<PropertyDefinition>)propsToReturn);
		}

		public new static Contact Bind(StoreSession session, StoreId storeId, ICollection<PropertyDefinition> propsToReturn)
		{
			return ItemBuilder.ItemBind<Contact>(session, storeId, ContactSchema.Instance, propsToReturn);
		}

		public static void ImportVCard(Contact contact, Stream dataStream, Encoding encoding, InboundConversionOptions options)
		{
			Util.ThrowOnNullArgument(contact, "contact");
			Util.ThrowOnNullArgument(dataStream, "dataStream");
			Util.ThrowOnNullArgument(encoding, "encoding");
			Util.ThrowOnNullArgument(options, "options");
			if (!options.IgnoreImceaDomain)
			{
				Util.ThrowOnNullOrEmptyArgument(options.ImceaEncapsulationDomain, "options.ImceaEncapsulationDomain");
			}
			contact.Load(InternalSchema.ContentConversionProperties);
			InboundVCardConverter.Convert(dataStream, encoding, contact, options);
		}

		public static void ExportVCard(Contact contact, Stream dataStream, OutboundConversionOptions options)
		{
			Util.ThrowOnNullArgument(dataStream, "dataStream");
			Util.ThrowOnNullArgument(contact, "contact");
			Util.ThrowOnNullArgument(options, "options");
			Util.ThrowOnNullOrEmptyArgument(options.ImceaEncapsulationDomain, "options.ImceaEncapsulationDomain");
			if (!contact.HasAllPropertiesLoaded)
			{
				throw new ArgumentException("Properties not loaded for contact");
			}
			OutboundVCardConverter.Convert(dataStream, Encoding.UTF8, contact, options, new ConversionLimitsTracker(options.Limits));
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<Contact>(this);
		}

		public string ImAddress
		{
			get
			{
				this.CheckDisposed("ImAddress::get");
				return base.GetValueOrDefault<string>(ContactSchema.IMAddress, string.Empty);
			}
			set
			{
				this.CheckDisposed("ImAddress::set");
				this[ContactSchema.IMAddress] = value;
			}
		}

		public CompleteName CompleteName
		{
			get
			{
				this.CheckDisposed("CompleteName::get");
				return this.completeName;
			}
		}

		public IDictionary<PhysicalAddressType, PhysicalAddress> PhysicalAddresses
		{
			get
			{
				this.CheckDisposed("PhysicalAddresses::get");
				throw new NotImplementedException();
			}
		}

		public IDictionary<EmailAddressIndex, Participant> EmailAddresses
		{
			get
			{
				this.CheckDisposed("EmailAddresses::get");
				this.EnsureParticipantsLoaded();
				return this.emailAddresses;
			}
		}

		public string JobTitle
		{
			get
			{
				this.CheckDisposed("JobTitle::get");
				return base.GetValueOrDefault<string>(ContactSchema.Title, string.Empty);
			}
			set
			{
				this.CheckDisposed("JobTitle::set");
				this[ContactSchema.Title] = value;
			}
		}

		public string Company
		{
			get
			{
				this.CheckDisposed("Company::get");
				return base.GetValueOrDefault<string>(ContactSchema.CompanyName, string.Empty);
			}
			set
			{
				this.CheckDisposed("Company::set");
				this[ContactSchema.CompanyName] = value;
			}
		}

		public string Department
		{
			get
			{
				this.CheckDisposed("Department::get");
				return base.GetValueOrDefault<string>(ContactSchema.Department, string.Empty);
			}
			set
			{
				this.CheckDisposed("Department::set");
				this[ContactSchema.Department] = value;
			}
		}

		public PersonType PersonType
		{
			get
			{
				this.CheckDisposed("PersonType::get");
				return base.GetValueOrDefault<PersonType>(ContactSchema.PersonType, PersonType.Unknown);
			}
			set
			{
				this.CheckDisposed("PersonType::set");
				this[ContactSchema.PersonType] = value;
			}
		}

		public bool IsFavorite
		{
			get
			{
				this.CheckDisposed("IsFavorite::get");
				return base.GetValueOrDefault<bool>(ContactSchema.IsFavorite, false);
			}
			set
			{
				this.CheckDisposed("IsFavorite::set");
				this[ContactSchema.IsFavorite] = value;
			}
		}

		public bool IsPromotedContact
		{
			get
			{
				this.CheckDisposed("IsPromotedContact::get");
				return base.GetValueOrDefault<bool>(ContactSchema.IsPromotedContact, false);
			}
			set
			{
				this.CheckDisposed("IsPromotedContact::set");
				this[ContactSchema.IsPromotedContact] = value;
			}
		}

		public int RelevanceScore
		{
			get
			{
				this.CheckDisposed("RelevanceScore::get");
				return base.GetValueOrDefault<int>(ContactSchema.RelevanceScore, int.MaxValue);
			}
			set
			{
				this.CheckDisposed("RelevanceScore::set");
				this[ContactSchema.RelevanceScore] = value;
			}
		}

		public string PartnerNetworkId
		{
			get
			{
				this.CheckDisposed("PartnerNetworkId::get");
				return base.GetValueOrDefault<string>(ContactSchema.PartnerNetworkId, string.Empty);
			}
			set
			{
				this.CheckDisposed("PartnerNetworkId::set");
				this[ContactSchema.PartnerNetworkId] = value;
			}
		}

		public string OfficeLocation
		{
			get
			{
				this.CheckDisposed("OfficeLocation::get");
				return base.GetValueOrDefault<string>(ContactSchema.OfficeLocation, string.Empty);
			}
			set
			{
				this.CheckDisposed("OfficeLocation::set");
				this[ContactSchema.OfficeLocation] = value;
			}
		}

		public string AssistantName
		{
			get
			{
				this.CheckDisposed("AssistantName::get");
				return base.GetValueOrDefault<string>(ContactSchema.AssistantName, string.Empty);
			}
			set
			{
				this.CheckDisposed("AssistantName::set");
				this[ContactSchema.AssistantName] = value;
			}
		}

		public FileAsMapping FileAs
		{
			get
			{
				return base.GetValueOrDefault<FileAsMapping>(ContactSchema.FileAsId, FileAsMapping.None);
			}
			set
			{
				this.CheckDisposed("FileAs::set");
				EnumValidator.ThrowIfInvalid<FileAsMapping>(value, "value");
				this[ContactSchema.FileAsId] = (int)value;
			}
		}

		public CultureInfo Culture
		{
			get
			{
				this.CheckDisposed("Culture::get");
				throw new NotImplementedException();
			}
			set
			{
				this.CheckDisposed("Culture::set");
				throw new NotImplementedException();
			}
		}

		public LocationSource HomeLocationSource
		{
			get
			{
				this.CheckDisposed("HomeLocationSource::get");
				return (LocationSource)base.GetValueOrDefault<int>(ContactSchema.HomeLocationSource, 0);
			}
			set
			{
				this.CheckDisposed("HomeLocationSource::set");
				EnumValidator.ThrowIfInvalid<LocationSource>(value, "HomeLocationSource");
				this[ContactSchema.HomeLocationSource] = value;
			}
		}

		public string HomeLocationUri
		{
			get
			{
				this.CheckDisposed("HomeLocationUri::get");
				return base.GetValueOrDefault<string>(ContactSchema.HomeLocationUri, null);
			}
			set
			{
				this.CheckDisposed("HomeLocationUri::set");
				base.SetOrDeleteProperty(ContactSchema.HomeLocationUri, value);
			}
		}

		public double? HomeLatitude
		{
			get
			{
				this.CheckDisposed("HomeLatitude::get");
				return base.GetValueOrDefault<double?>(ContactSchema.HomeLatitude, null);
			}
			set
			{
				this.CheckDisposed("HomeLatitude::set");
				if (value.Equals(base.GetValueAsNullable<double>(ContactSchema.HomeLatitude)))
				{
					return;
				}
				base.SetOrDeleteProperty(ContactSchema.HomeLatitude, value);
			}
		}

		public double? HomeLongitude
		{
			get
			{
				this.CheckDisposed("HomeLongitude::get");
				return base.GetValueOrDefault<double?>(ContactSchema.HomeLongitude, null);
			}
			set
			{
				this.CheckDisposed("HomeLongitude::set");
				if (value.Equals(base.GetValueAsNullable<double>(ContactSchema.HomeLongitude)))
				{
					return;
				}
				base.SetOrDeleteProperty(ContactSchema.HomeLongitude, value);
			}
		}

		public double? HomeAltitude
		{
			get
			{
				this.CheckDisposed("HomeAltitude::get");
				return base.GetValueOrDefault<double?>(ContactSchema.HomeAltitude, null);
			}
			set
			{
				this.CheckDisposed("HomeAltitude::set");
				if (value.Equals(base.GetValueAsNullable<double>(ContactSchema.HomeAltitude)))
				{
					return;
				}
				base.SetOrDeleteProperty(ContactSchema.HomeAltitude, value);
			}
		}

		public double? HomeAccuracy
		{
			get
			{
				this.CheckDisposed("HomeAccuracy::get");
				return base.GetValueOrDefault<double?>(ContactSchema.HomeAccuracy, null);
			}
			set
			{
				this.CheckDisposed("HomeAccuracy::set");
				if (value.Equals(base.GetValueAsNullable<double>(ContactSchema.HomeAccuracy)))
				{
					return;
				}
				base.SetOrDeleteProperty(ContactSchema.HomeAccuracy, value);
			}
		}

		public double? HomeAltitudeAccuracy
		{
			get
			{
				this.CheckDisposed("HomeAltitudeAccuracy::get");
				return base.GetValueOrDefault<double?>(ContactSchema.HomeAltitudeAccuracy, null);
			}
			set
			{
				this.CheckDisposed("HomeAltitudeAccuracy::set");
				if (value.Equals(base.GetValueAsNullable<double>(ContactSchema.HomeAltitudeAccuracy)))
				{
					return;
				}
				base.SetOrDeleteProperty(ContactSchema.HomeAltitudeAccuracy, value);
			}
		}

		public LocationSource WorkLocationSource
		{
			get
			{
				this.CheckDisposed("WorkLocationSource::get");
				return (LocationSource)base.GetValueOrDefault<int>(ContactSchema.WorkLocationSource, 0);
			}
			set
			{
				this.CheckDisposed("WorkLocationSource::set");
				EnumValidator.ThrowIfInvalid<LocationSource>(value, "WorkLocationSource");
				base.SetOrDeleteProperty(ContactSchema.WorkLocationSource, value);
			}
		}

		public string WorkLocationUri
		{
			get
			{
				this.CheckDisposed("WorkLocationUri::get");
				return base.GetValueOrDefault<string>(ContactSchema.WorkLocationUri, null);
			}
			set
			{
				this.CheckDisposed("WorkLocationUri::set");
				base.SetOrDeleteProperty(ContactSchema.WorkLocationUri, value);
			}
		}

		public double? WorkLatitude
		{
			get
			{
				this.CheckDisposed("WorkLatitude::get");
				return base.GetValueOrDefault<double?>(ContactSchema.WorkLatitude, null);
			}
			set
			{
				this.CheckDisposed("WorkLatitude::set");
				if (value.Equals(base.GetValueAsNullable<double>(ContactSchema.WorkLatitude)))
				{
					return;
				}
				base.SetOrDeleteProperty(ContactSchema.WorkLatitude, value);
			}
		}

		public double? WorkLongitude
		{
			get
			{
				this.CheckDisposed("WorkLongitude::get");
				return base.GetValueOrDefault<double?>(ContactSchema.WorkLongitude, null);
			}
			set
			{
				this.CheckDisposed("WorkLongitude::set");
				if (value.Equals(base.GetValueAsNullable<double>(ContactSchema.WorkLongitude)))
				{
					return;
				}
				base.SetOrDeleteProperty(ContactSchema.WorkLongitude, value);
			}
		}

		public double? WorkAltitude
		{
			get
			{
				this.CheckDisposed("WorkAltitude::get");
				return base.GetValueOrDefault<double?>(ContactSchema.WorkAltitude, null);
			}
			set
			{
				this.CheckDisposed("WorkAltitude::set");
				if (value.Equals(base.GetValueAsNullable<double>(ContactSchema.WorkAltitude)))
				{
					return;
				}
				base.SetOrDeleteProperty(ContactSchema.WorkAltitude, value);
			}
		}

		public double? WorkAccuracy
		{
			get
			{
				this.CheckDisposed("WorkAccuracy::get");
				return base.GetValueOrDefault<double?>(ContactSchema.WorkAccuracy, null);
			}
			set
			{
				this.CheckDisposed("WorkAccuracy::set");
				if (value.Equals(base.GetValueAsNullable<double>(ContactSchema.WorkAccuracy)))
				{
					return;
				}
				base.SetOrDeleteProperty(ContactSchema.WorkAccuracy, value);
			}
		}

		public double? WorkAltitudeAccuracy
		{
			get
			{
				this.CheckDisposed("WorkAltitudeAccuracy::get");
				return base.GetValueOrDefault<double?>(ContactSchema.WorkAltitudeAccuracy, null);
			}
			set
			{
				this.CheckDisposed("WorkAltitudeAccuracy::set");
				if (value.Equals(base.GetValueAsNullable<double>(ContactSchema.WorkAltitudeAccuracy)))
				{
					return;
				}
				base.SetOrDeleteProperty(ContactSchema.WorkAltitudeAccuracy, value);
			}
		}

		public LocationSource OtherLocationSource
		{
			get
			{
				this.CheckDisposed("OtherLocationSource::get");
				return (LocationSource)base.GetValueOrDefault<int>(ContactSchema.OtherLocationSource, 0);
			}
			set
			{
				this.CheckDisposed("OtherLocationSource::set");
				EnumValidator.ThrowIfInvalid<LocationSource>(value, "OtherLocationSource");
				this[ContactSchema.OtherLocationSource] = value;
			}
		}

		public string OtherLocationUri
		{
			get
			{
				this.CheckDisposed("OtherLocationUri::get");
				return base.GetValueOrDefault<string>(ContactSchema.OtherLocationUri, null);
			}
			set
			{
				this.CheckDisposed("OtherLocationUri::set");
				base.SetOrDeleteProperty(ContactSchema.OtherLocationUri, value);
			}
		}

		public double? OtherLatitude
		{
			get
			{
				this.CheckDisposed("OtherLatitude::get");
				return base.GetValueOrDefault<double?>(ContactSchema.OtherLatitude, null);
			}
			set
			{
				this.CheckDisposed("OtherLatitude::set");
				if (value.Equals(base.GetValueAsNullable<double>(ContactSchema.OtherLatitude)))
				{
					return;
				}
				base.SetOrDeleteProperty(ContactSchema.OtherLatitude, value);
			}
		}

		public double? OtherLongitude
		{
			get
			{
				this.CheckDisposed("OtherLongitude::get");
				return base.GetValueOrDefault<double?>(ContactSchema.OtherLongitude, null);
			}
			set
			{
				this.CheckDisposed("OtherLongitude::set");
				if (value.Equals(base.GetValueAsNullable<double>(ContactSchema.OtherLongitude)))
				{
					return;
				}
				base.SetOrDeleteProperty(ContactSchema.OtherLongitude, value);
			}
		}

		public double? OtherAltitude
		{
			get
			{
				this.CheckDisposed("OtherAltitude::get");
				return base.GetValueOrDefault<double?>(ContactSchema.OtherAltitude, null);
			}
			set
			{
				this.CheckDisposed("OtherAltitude::set");
				if (value.Equals(base.GetValueAsNullable<double>(ContactSchema.OtherAltitude)))
				{
					return;
				}
				base.SetOrDeleteProperty(ContactSchema.OtherAltitude, value);
			}
		}

		public double? OtherAccuracy
		{
			get
			{
				this.CheckDisposed("OtherAccuracy::get");
				return base.GetValueOrDefault<double?>(ContactSchema.OtherAccuracy, null);
			}
			set
			{
				this.CheckDisposed("OtherAccuracy::set");
				if (value.Equals(base.GetValueAsNullable<double>(ContactSchema.OtherAccuracy)))
				{
					return;
				}
				base.SetOrDeleteProperty(ContactSchema.OtherAccuracy, value);
			}
		}

		public double? OtherAltitudeAccuracy
		{
			get
			{
				this.CheckDisposed("OtherAltitudeAccuracy::get");
				return base.GetValueOrDefault<double?>(ContactSchema.OtherAltitudeAccuracy, null);
			}
			set
			{
				this.CheckDisposed("OtherAltitudeAccuracy::set");
				if (value.Equals(base.GetValueAsNullable<double>(ContactSchema.OtherAltitudeAccuracy)))
				{
					return;
				}
				base.SetOrDeleteProperty(ContactSchema.OtherAltitudeAccuracy, value);
			}
		}

		public override Schema Schema
		{
			get
			{
				this.CheckDisposed("Schema::get");
				return ContactSchema.Instance;
			}
		}

		public Stream GetPhotoStream()
		{
			foreach (AttachmentHandle handle in base.AttachmentCollection)
			{
				using (Attachment attachment = base.AttachmentCollection.Open(handle, null))
				{
					if (attachment.IsContactPhoto)
					{
						StreamAttachment streamAttachment = attachment as StreamAttachment;
						if (streamAttachment != null)
						{
							return streamAttachment.TryGetContentStream(PropertyOpenMode.ReadOnly);
						}
					}
				}
			}
			return null;
		}

		private void EnsureParticipantsLoaded()
		{
			if (!this.areEmailAddressesLoaded)
			{
				foreach (KeyValuePair<EmailAddressIndex, ContactEmailSlotParticipantProperty> keyValuePair in ContactEmailSlotParticipantProperty.AllInstances)
				{
					this.emailAddresses.InternalSet(keyValuePair.Key, base.GetValueOrDefault<Participant>(keyValuePair.Value));
				}
				this.areEmailAddressesLoaded = true;
			}
		}

		private void OnEmailAddressChanged(EmailAddressIndex emailAddressIndex)
		{
			base.SetOrDeleteProperty(ContactEmailSlotParticipantProperty.AllInstances[emailAddressIndex], this.emailAddresses[emailAddressIndex]);
		}

		private void Initialize()
		{
			base.Load(new PropertyDefinition[]
			{
				InternalSchema.ItemClass
			});
			string itemClass = base.TryGetProperty(InternalSchema.ItemClass) as string;
			if (!ObjectClass.IsContact(itemClass))
			{
				this[InternalSchema.ItemClass] = "IPM.Contact";
			}
			this[ContactSchema.FileAsId] = -1;
		}

		protected override void OnBeforeSave()
		{
			base.OnBeforeSave();
			this.junkEmailUpdates.Clear();
			foreach (ContactEmailSlotParticipantProperty contactEmailSlotParticipantProperty in ContactEmailSlotParticipantProperty.AllInstances.Values)
			{
				if (base.PropertyBag.IsPropertyDirty(contactEmailSlotParticipantProperty.EmailAddressPropertyDefinition) || base.PropertyBag.IsPropertyDirty(contactEmailSlotParticipantProperty.RoutingTypePropertyDefinition))
				{
					Participant valueOrDefault = base.GetValueOrDefault<Participant>(contactEmailSlotParticipantProperty);
					if (valueOrDefault != null && valueOrDefault.RoutingType == "SMTP")
					{
						this.junkEmailUpdates.Add(valueOrDefault.EmailAddress);
					}
				}
			}
			this.OnBeforeSaveUpdateInteropValues();
			SideEffects sideEffects = base.GetValueOrDefault<SideEffects>(ContactSchema.SideEffects);
			sideEffects |= SideEffects.CoerceToInbox;
			this[ContactSchema.SideEffects] = sideEffects;
		}

		protected override void OnAfterSave(ConflictResolutionResult acrResults)
		{
			base.OnAfterSave(acrResults);
			if (acrResults.SaveStatus != SaveResult.IrresolvableConflict)
			{
				this.areEmailAddressesLoaded = false;
				this.UpdateJunkEmailContacts();
			}
		}

		private void OnBeforeSaveUpdateInteropValues()
		{
			this.OnBeforeSaveUpdateFaxParticipant(ContactSchema.WorkFax, EmailAddressIndex.BusinessFax);
			this.OnBeforeSaveUpdateFaxParticipant(ContactSchema.HomeFax, EmailAddressIndex.HomeFax);
			this.OnBeforeSaveUpdateFaxParticipant(ContactSchema.OtherFax, EmailAddressIndex.OtherFax);
			EmailListType emailListType = EmailListType.None;
			List<int> list = new List<int>(6);
			EmailAddressIndex[] array = new EmailAddressIndex[]
			{
				EmailAddressIndex.Email1,
				EmailAddressIndex.Email2,
				EmailAddressIndex.Email3,
				EmailAddressIndex.BusinessFax,
				EmailAddressIndex.HomeFax,
				EmailAddressIndex.OtherFax
			};
			int i = 0;
			int num = 1;
			while (i < array.Length)
			{
				Participant participant = this.EmailAddresses[array[i]];
				if (participant != null && participant.RoutingType != null)
				{
					emailListType |= (EmailListType)num;
					list.Add(i);
				}
				i++;
				num <<= 1;
			}
			this[InternalSchema.EmailListType] = (int)emailListType;
			if (list.Count > 0)
			{
				this[InternalSchema.EmailList] = list.ToArray();
			}
			else
			{
				base.DeleteProperties(new PropertyDefinition[]
				{
					InternalSchema.EmailList
				});
			}
			base.SetOrDeleteProperty(ContactSchema.LegacyWebPage, base.TryGetProperty(ContactSchema.BusinessHomePage));
		}

		private void OnBeforeSaveUpdateFaxParticipant(StorePropertyDefinition prop, EmailAddressIndex index)
		{
			if (base.IsNew || base.PropertyBag.IsPropertyDirty(prop))
			{
				string valueOrDefault = base.GetValueOrDefault<string>(prop);
				if (string.IsNullOrEmpty(valueOrDefault))
				{
					this.EmailAddresses.Remove(index);
					return;
				}
				Participant participant = this.EmailAddresses[index];
				if (participant == null || participant.EmailAddress != valueOrDefault || participant.RoutingType != "FAX")
				{
					string valueOrDefault2 = base.GetValueOrDefault<string>(StoreObjectSchema.DisplayName);
					Participant value = new Participant(valueOrDefault2, valueOrDefault, "FAX");
					this.EmailAddresses[index] = value;
				}
			}
		}

		internal static void CoreObjectUpdateFileAs(CoreItem coreItem)
		{
			PersistablePropertyBag persistablePropertyBag = Microsoft.Exchange.Data.Storage.CoreObject.GetPersistablePropertyBag(coreItem);
			InternalSchema.FileAsString.UpdateCompositePropertyValue(persistablePropertyBag);
			InternalSchema.FileAsString.UpdateFullNameAndSubject(persistablePropertyBag);
		}

		internal static void CoreObjectUpdatePhysicalAddresses(CoreItem coreItem)
		{
			PersistablePropertyBag persistablePropertyBag = Microsoft.Exchange.Data.Storage.CoreObject.GetPersistablePropertyBag(coreItem);
			InternalSchema.HomeAddress.UpdateCompositePropertyValue(persistablePropertyBag);
			InternalSchema.BusinessAddress.UpdateCompositePropertyValue(persistablePropertyBag);
			InternalSchema.OtherAddress.UpdateCompositePropertyValue(persistablePropertyBag);
		}

		internal new static void CoreObjectUpdateAllAttachmentsHidden(CoreItem coreItem)
		{
			Microsoft.Exchange.Data.Storage.Item.EnsureAllAttachmentsHiddenValue(coreItem, true);
		}

		private void UpdateJunkEmailContacts()
		{
			if (this.junkEmailUpdates.Count > 0)
			{
				MailboxSession mailboxSession = base.Session as MailboxSession;
				if (mailboxSession != null && mailboxSession.LogonType != LogonType.Delegated && mailboxSession.Capabilities.CanHaveJunkEmailRule && !mailboxSession.MailboxOwner.ObjectId.IsNullOrEmpty())
				{
					JunkEmailRule junkEmailRule = mailboxSession.JunkEmailRule;
					if (junkEmailRule.IsContactsFolderTrusted)
					{
						try
						{
							junkEmailRule.SynchronizeContactsCache();
							junkEmailRule.Save();
						}
						catch (JunkEmailValidationException)
						{
						}
						catch (DataSourceOperationException ex)
						{
							throw StorageGlobals.TranslateDirectoryException(ServerStrings.ADException, ex, this, "Contact.UpdateJunkEmailContacts. Failed due to directory exception {0}.", new object[]
							{
								ex
							});
						}
						catch (DataSourceTransientException ex2)
						{
							throw StorageGlobals.TranslateDirectoryException(ServerStrings.ADException, ex2, this, "Contact.UpdateJunkEmailContacts. Failed due to directory exception {0}.", new object[]
							{
								ex2
							});
						}
					}
				}
			}
		}

		private bool areEmailAddressesLoaded;

		private IList<string> junkEmailUpdates = new List<string>();

		private readonly CompleteName completeName;

		private readonly Contact.EmailAddressDictionary emailAddresses;

		private delegate void ModifyDistributionListsDelegate(DistributionList dl, DistributionListMember member);

		private class CallbackCompleteName : CompleteName
		{
			public CallbackCompleteName(Contact contact)
			{
				this.contact = contact;
			}

			public override string FirstName
			{
				get
				{
					this.contact.CheckDisposed("CompleteName::FirstName::get");
					return this.contact.GetValueOrDefault<string>(ContactSchema.GivenName, string.Empty);
				}
				set
				{
					this.contact.CheckDisposed("CompleteName::FirstName::set");
					this.contact[ContactSchema.GivenName] = value;
				}
			}

			public override string FullName
			{
				get
				{
					this.contact.CheckDisposed("CompleteName::FullName::get");
					return this.contact.GetValueOrDefault<string>(ContactSchema.FullName, string.Empty);
				}
				set
				{
					this.contact.CheckDisposed("CompleteName::FullName::set");
					this.contact[ContactSchema.FullName] = value;
				}
			}

			public override string Initials
			{
				get
				{
					this.contact.CheckDisposed("CompleteName::Initials::get");
					return this.contact.GetValueOrDefault<string>(ContactSchema.Initials, string.Empty);
				}
				set
				{
					this.contact.CheckDisposed("CompleteName::Initials::set");
					this.contact[ContactSchema.Initials] = value;
				}
			}

			public override string LastName
			{
				get
				{
					this.contact.CheckDisposed("CompleteName::LastName::get");
					return this.contact.GetValueOrDefault<string>(ContactSchema.Surname, string.Empty);
				}
				set
				{
					this.contact.CheckDisposed("CompleteName::LastName::set");
					this.contact[ContactSchema.Surname] = value;
				}
			}

			public override string MiddleName
			{
				get
				{
					this.contact.CheckDisposed("CompleteName::MiddleName::get");
					return this.contact.GetValueOrDefault<string>(ContactSchema.MiddleName, string.Empty);
				}
				set
				{
					this.contact.CheckDisposed("CompleteName::MiddleName::set");
					this.contact[ContactSchema.MiddleName] = value;
				}
			}

			public override string Nickname
			{
				get
				{
					this.contact.CheckDisposed("CompleteName::Nickname::get");
					return this.contact.GetValueOrDefault<string>(ContactSchema.Nickname, string.Empty);
				}
				set
				{
					this.contact.CheckDisposed("CompleteName::Nickname::set");
					this.contact[ContactSchema.Nickname] = value;
				}
			}

			public override string Suffix
			{
				get
				{
					this.contact.CheckDisposed("CompleteName::Suffix::get");
					return this.contact.GetValueOrDefault<string>(ContactSchema.Generation, string.Empty);
				}
				set
				{
					this.contact.CheckDisposed("CompleteName::Suffix::set");
					this.contact[ContactSchema.Generation] = value;
				}
			}

			public override string Title
			{
				get
				{
					this.contact.CheckDisposed("CompleteName::Title::get");
					return this.contact.GetValueOrDefault<string>(ContactSchema.DisplayNamePrefix, string.Empty);
				}
				set
				{
					this.contact.CheckDisposed("CompleteName::Title::set");
					this.contact[ContactSchema.DisplayNamePrefix] = value;
				}
			}

			public override string YomiCompany
			{
				get
				{
					this.contact.CheckDisposed("CompleteName::YomiCompany::get");
					return this.contact.GetValueOrDefault<string>(ContactSchema.YomiCompany, string.Empty);
				}
				set
				{
					this.contact.CheckDisposed("CompleteName::YomiCompany::set");
					this.contact[ContactSchema.YomiCompany] = value;
				}
			}

			public override string YomiFirstName
			{
				get
				{
					this.contact.CheckDisposed("CompleteName::YomiFirstName::get");
					return this.contact.GetValueOrDefault<string>(ContactSchema.YomiFirstName, string.Empty);
				}
				set
				{
					this.contact.CheckDisposed("CompleteName::YomiFirstName::set");
					this.contact[ContactSchema.YomiFirstName] = value;
				}
			}

			public override string YomiLastName
			{
				get
				{
					this.contact.CheckDisposed("CompleteName::YomiLastName::get");
					return this.contact.GetValueOrDefault<string>(ContactSchema.YomiLastName, string.Empty);
				}
				set
				{
					this.contact.CheckDisposed("CompleteName::YomiLastName::set");
					this.contact[ContactSchema.YomiLastName] = value;
				}
			}

			private readonly Contact contact;
		}

		private class EmailAddressDictionary : Dictionary<EmailAddressIndex, Participant>, IDictionary<EmailAddressIndex, Participant>, ICollection<KeyValuePair<EmailAddressIndex, Participant>>, IEnumerable<KeyValuePair<EmailAddressIndex, Participant>>, IEnumerable
		{
			public EmailAddressDictionary(Contact contact)
			{
				this.contact = contact;
			}

			public new Participant this[EmailAddressIndex key]
			{
				get
				{
					EnumValidator.ThrowIfInvalid<EmailAddressIndex>(key, "key");
					if (base.ContainsKey(key))
					{
						return base[key];
					}
					return null;
				}
				set
				{
					EnumValidator.ThrowIfInvalid<EmailAddressIndex>(key, "key");
					this.InternalSet(key, value);
					this.contact.OnEmailAddressChanged(key);
				}
			}

			public new void Add(EmailAddressIndex key, Participant value)
			{
				EnumValidator.ThrowIfInvalid<EmailAddressIndex>(key, "key");
				Util.ThrowOnNullArgument(value, "value");
				Contact.EmailAddressDictionary.Check(key);
				base.Add(key, value);
				this.contact.OnEmailAddressChanged(key);
			}

			public new bool Remove(EmailAddressIndex key)
			{
				EnumValidator.ThrowIfInvalid<EmailAddressIndex>(key, "key");
				bool result = base.Remove(key);
				this.contact.OnEmailAddressChanged(key);
				return result;
			}

			void ICollection<KeyValuePair<EmailAddressIndex, Participant>>.Add(KeyValuePair<EmailAddressIndex, Participant> item)
			{
				this.Add(item.Key, item.Value);
			}

			public new void Clear()
			{
				List<EmailAddressIndex> list = new List<EmailAddressIndex>(base.Keys);
				base.Clear();
				foreach (EmailAddressIndex emailAddressIndex in list)
				{
					this.contact.OnEmailAddressChanged(emailAddressIndex);
				}
			}

			public bool Remove(KeyValuePair<EmailAddressIndex, Participant> item)
			{
				return this.Remove(item.Key);
			}

			private static void Check(EmailAddressIndex emailAddressIndex)
			{
				EnumValidator.ThrowIfInvalid<EmailAddressIndex>(emailAddressIndex, Contact.EmailAddressDictionary.validSet);
			}

			internal void InternalSet(EmailAddressIndex key, Participant value)
			{
				EnumValidator.AssertValid<EmailAddressIndex>(key);
				Contact.EmailAddressDictionary.Check(key);
				if (value != null)
				{
					base[key] = value;
					return;
				}
				base.Remove(key);
			}

			private readonly Contact contact;

			private static EmailAddressIndex[] validSet = new EmailAddressIndex[]
			{
				EmailAddressIndex.Email1,
				EmailAddressIndex.Email2,
				EmailAddressIndex.Email3,
				EmailAddressIndex.BusinessFax,
				EmailAddressIndex.HomeFax,
				EmailAddressIndex.OtherFax
			};
		}
	}
}
