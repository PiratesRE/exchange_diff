using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class Person : IPerson
	{
		private Person(PersonId id, IStorePropertyBag aggregatedProperties, List<IStorePropertyBag> contacts, StoreSession session)
		{
			this.personId = id;
			this.aggregatedProperties = aggregatedProperties;
			this.contacts = contacts;
			this.storeSession = session;
		}

		public static Person Load(MailboxSession session, PersonId personId, ICollection<PropertyDefinition> properties)
		{
			return Person.Load(session, personId, properties, null, null);
		}

		public static Person Load(StoreSession session, PersonId personId, ICollection<PropertyDefinition> properties, PropertyDefinition[] extendedProperties, StoreId folderId = null)
		{
			Person.Tracer.TraceDebug<string>(PersonId.TraceId(personId), "Person.Load: Entering, with personId = {0}", (personId == null) ? "(null)" : personId.ToString());
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(personId, "personId");
			PropertyDefinition[] propertiesToLoad = Person.GetPropertiesToLoad(properties, extendedProperties);
			List<IStorePropertyBag> contactsWithPersonId = Person.GetContactsWithPersonId(session, personId, propertiesToLoad, folderId);
			Person result = null;
			if (contactsWithPersonId != null && contactsWithPersonId.Count > 0)
			{
				Person.Tracer.TraceDebug<int>(PersonId.TraceId(personId), "Person.Load: Found {0} contacts in this Person", contactsWithPersonId.Count);
				result = Person.LoadFromContacts(personId, contactsWithPersonId, session, properties, extendedProperties);
			}
			Person.Tracer.TraceDebug(PersonId.TraceId(personId), "Person.Load: Exiting");
			return result;
		}

		public static List<IStorePropertyBag> LoadContactsFromPublicFolder(PublicFolderSession session, PersonId personId, StoreId folderId, PropertyDefinition[] columns)
		{
			ComparisonFilter queryFilter = new ComparisonFilter(ComparisonOperator.Equal, InternalSchema.PersonId, personId);
			List<IStorePropertyBag> result;
			using (Folder folder = Folder.Bind(session, folderId, null))
			{
				using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.None, queryFilter, null, columns))
				{
					IStorePropertyBag[] propertyBags = queryResult.GetPropertyBags(1);
					result = propertyBags.ToList<IStorePropertyBag>();
				}
			}
			return result;
		}

		public static Person LoadWithGALAggregation(MailboxSession session, PersonId personId, ICollection<PropertyDefinition> personProperties, PropertyDefinition[] extendedProperties)
		{
			Person.Tracer.TraceDebug<string>(PersonId.TraceId(personId), "Person.LoadWithGALAggregation: Entering, with personId = {0}", (personId == null) ? "(null)" : personId.ToString());
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(personId, "personId");
			PropertyDefinition[] propertiesToLoad = Person.GetPropertiesToLoad(personProperties, extendedProperties);
			HashSet<PropertyDefinition> contactPropertiesToLoad = Person.GetContactPropertiesToLoad(propertiesToLoad, Person.GALAggregationRequiredStoreProperties);
			AllPersonContactsEnumerator contactsEnumerator = AllPersonContactsEnumerator.Create(session, personId, contactPropertiesToLoad);
			List<IStorePropertyBag> list = Person.LoadFromEnumerator(contactsEnumerator);
			Person result = null;
			if (list.Count > 0)
			{
				Person.Tracer.TraceDebug<int>(PersonId.TraceId(personId), "Person.LoadWithGALAggregation: Found {0} contacts in this Person", list.Count);
				Person.LoadGALDataIfPersonIsGALLinked(session, personId, propertiesToLoad, list, contactPropertiesToLoad);
				result = Person.LoadFromContacts(personId, list, session, personProperties, extendedProperties);
			}
			Person.Tracer.TraceDebug(PersonId.TraceId(personId), "Person.LoadWithGALAggregation: Exiting");
			return result;
		}

		public static Person FindPersonLinkedToADEntry(MailboxSession session, ADRawEntry adRawEntry, ICollection<PropertyDefinition> properties)
		{
			ArgumentValidator.ThrowIfNull("session", session);
			ArgumentValidator.ThrowIfNull("adRawEntry", adRawEntry);
			Person.Tracer.TraceDebug<Guid>((long)adRawEntry.GetHashCode(), "Person.FindPersonLinkedToADEntry: with ADObjectId = {0}.", adRawEntry.Id.ObjectGuid);
			PersonId personId = Person.FindPersonIdByGALLinkID(session, adRawEntry.Id.ObjectGuid);
			if (personId == null)
			{
				Person.Tracer.TraceDebug((long)adRawEntry.GetHashCode(), "Person.FindPersonLinkedToADEntry: No Person found with the matching GALLinkID.");
				return null;
			}
			Person.Tracer.TraceDebug<PersonId>((long)adRawEntry.GetHashCode(), "Person.FindPersonLinkedToADEntry: Person {0} found with the matching GALLinkID.", personId);
			PropertyDefinition[] propertiesToLoad = Person.GetPropertiesToLoad(properties, null);
			AllPersonContactsEnumerator contactsEnumerator = AllPersonContactsEnumerator.Create(session, personId, propertiesToLoad);
			List<IStorePropertyBag> list = Person.LoadFromEnumerator(contactsEnumerator);
			if (list.Count == 0)
			{
				Person.Tracer.TraceDebug((long)adRawEntry.GetHashCode(), "Person.FindPersonLinkedToADEntry: No personal contacts loaded for the given person Id.");
				return null;
			}
			Person.Tracer.TraceDebug<int>((long)adRawEntry.GetHashCode(), "Person.FindPersonLinkedToADEntry: Loaded {0} personal contacts for the given personId.", list.Count);
			IStorePropertyBag item = Person.ConvertADRawEntryToContact(adRawEntry, personId, properties);
			list.Add(item);
			return Person.LoadFromContacts(personId, list, session, properties, null);
		}

		public static PersonId FindPersonIdByEmailAddress(IMailboxSession session, IXSOFactory xsoFactory, string emailAddress)
		{
			return Person.FindPersonIdByEmailAddress(session, xsoFactory, session.GetDefaultFolderId(DefaultFolderType.MyContacts), emailAddress);
		}

		public static PersonId FindPersonIdByEmailAddress(IMailboxSession session, IXSOFactory xsoFactory, StoreObjectId folderId, string emailAddress)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(xsoFactory, "xsoFactory");
			Util.ThrowOnNullArgument(folderId, "folderId");
			Util.ThrowOnNullOrEmptyArgument(emailAddress, "emailAddress");
			Person.Tracer.TraceDebug<string, StoreObjectId>((long)emailAddress.GetHashCode(), "Person.FindPersonIdByEmailAddress: Find match for EmailAddress = {0} in {1}.", emailAddress, folderId);
			using (IFolder folder = xsoFactory.BindToFolder(session, folderId))
			{
				foreach (IStorePropertyBag storePropertyBag in new ContactsByEmailAddressEnumerator(folder, Person.PersonIdProperty, emailAddress))
				{
					PersonId valueOrDefault = storePropertyBag.GetValueOrDefault<PersonId>(ContactSchema.PersonId, null);
					if (valueOrDefault != null)
					{
						Person.Tracer.TraceDebug<PersonId>((long)emailAddress.GetHashCode(), "Person.FindPersonIdByEmailAddress: Match found - Person Id : {0}.", valueOrDefault);
						return valueOrDefault;
					}
				}
			}
			Person.Tracer.TraceDebug((long)emailAddress.GetHashCode(), "Person.FindPersonIdByEmailAddress: No Match found.");
			return null;
		}

		public static Person LoadFromContacts(PersonId personId, List<IStorePropertyBag> contacts, StoreSession session, ICollection<PropertyDefinition> requestedProperties, PropertyDefinition[] extendedProperties)
		{
			Util.ThrowOnNullArgument(contacts, "contacts");
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(personId, "personId");
			Util.ThrowOnArgumentOutOfRangeOnLessThan(contacts.Count, 1, "contacts");
			Person.Tracer.TraceDebug<string>(PersonId.TraceId(personId), "Person.LoadFromContacts: Entering, with personId = {0}", personId.ToString());
			Person.Tracer.TraceDebug<int>(PersonId.TraceId(personId), "Person.LoadFromContacts: Found {0} contacts in this Person", contacts.Count);
			PersonPropertyAggregationContext aggregationContext = new PersonPropertyAggregationContext(contacts, session.ContactFolders, session.ClientInfoString);
			IStorePropertyBag storePropertyBag = Person.InternalGetAggregatedProperties(aggregationContext, requestedProperties, extendedProperties);
			Person result = new Person(personId, storePropertyBag, contacts, session);
			Person.Tracer.TraceDebug(PersonId.TraceId(personId), "Person.LoadFromContacts: Exiting");
			return result;
		}

		public static Person LoadNotes(StoreSession session, PersonId personId, int requestedBytesToFetch, StoreId folderId = null)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(personId, "personId");
			Util.ThrowOnNullArgument(requestedBytesToFetch, "requestedBytesToFetch");
			int maxBytesToReadFromStore = Math.Min(requestedBytesToFetch, Person.MaxNotesBytes);
			List<IStorePropertyBag> contactsWithPersonId = Person.GetContactsWithPersonId(session, personId, Person.PropertiesToLoadForNotes, folderId);
			MemoryPropertyBag memoryPropertyBag = new MemoryPropertyBag();
			memoryPropertyBag[ContactSchema.PersonId] = personId;
			List<AttributedValue<PersonNotes>> list = new List<AttributedValue<PersonNotes>>(contactsWithPersonId.Count);
			for (int i = 0; i < contactsWithPersonId.Count; i++)
			{
				VersionedId versionedIdForPropertyBag = Person.GetVersionedIdForPropertyBag(contactsWithPersonId[i]);
				PersonId valueOrDefault = contactsWithPersonId[i].GetValueOrDefault<PersonId>(ContactSchema.PersonId, null);
				if (personId.Equals(valueOrDefault))
				{
					using (Item item = Item.Bind(session, versionedIdForPropertyBag, Person.PropertiesToLoadForNotes))
					{
						Body body = item.Body;
						PersonNotes personNotes = Person.ReadPersonNotes(body, maxBytesToReadFromStore);
						if (personNotes != null)
						{
							AttributedValue<PersonNotes> item2 = new AttributedValue<PersonNotes>(personNotes, new string[]
							{
								i.ToString(CultureInfo.InvariantCulture)
							});
							list.Add(item2);
						}
					}
				}
			}
			MailboxSession mailboxSession = session as MailboxSession;
			if (mailboxSession != null)
			{
				PersonNotes personNotes2 = Person.LoadGALNotesIfPersonIsGALLinked(mailboxSession, personId, requestedBytesToFetch, contactsWithPersonId);
				if (personNotes2 != null)
				{
					list.Add(new AttributedValue<PersonNotes>(personNotes2, new string[]
					{
						contactsWithPersonId.Count.ToString(CultureInfo.InvariantCulture)
					}));
				}
			}
			memoryPropertyBag[Person.SimpleVirtualPersonaBodiesProperty] = list;
			return new Person(personId, memoryPropertyBag.AsIStorePropertyBag(), contactsWithPersonId, session);
		}

		public static PersonId CreatePerson(StoreSession session, PersonId personId, ICollection<StoreObjectPropertyChange> propertyChanges, StoreId parentFolder, bool isGroup)
		{
			Person.Tracer.TraceDebug<string, bool>(PersonId.TraceId(personId), "Person.CreatePerson: Entering, with personId = {0} and isGroup = {1}", (personId == null) ? "(null)" : personId.ToString(), isGroup);
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(session, "propertyChanges");
			Util.ThrowOnNullArgument(personId, "personId");
			Person person = new Person(personId, null, new List<IStorePropertyBag>(12), null);
			PersonId result;
			if (!isGroup)
			{
				result = person.CreateContact(session, propertyChanges, parentFolder);
			}
			else
			{
				result = person.CreateGroup(session, propertyChanges, parentFolder);
			}
			Person.Tracer.TraceDebug<bool>(PersonId.TraceId(personId), "Person.CreatePerson: Exiting, with isGroup = {0}", isGroup);
			return result;
		}

		internal static PersonId FindPersonIdByGALLinkID(MailboxSession session, Guid galLinkID)
		{
			ArgumentValidator.ThrowIfNull("session", session);
			ArgumentValidator.ThrowIfEmpty("galLinkID", galLinkID);
			Person.Tracer.TraceDebug<Guid>((long)galLinkID.GetHashCode(), "Person.FindPersonIdByGALLinkID: Find match for GALLinkID = {0}.", galLinkID);
			ContactsByGALLinkIdEnumerator contactsByGALLinkIdEnumerator = new ContactsByGALLinkIdEnumerator(session, DefaultFolderType.MyContacts, galLinkID, Person.PersonIdProperty);
			foreach (IStorePropertyBag storePropertyBag in contactsByGALLinkIdEnumerator)
			{
				PersonId valueOrDefault = storePropertyBag.GetValueOrDefault<PersonId>(ContactSchema.PersonId, null);
				if (valueOrDefault != null)
				{
					Person.Tracer.TraceDebug<PersonId>((long)galLinkID.GetHashCode(), "Person.FindPersonIdByGALLinkID: Match found - Person Id : {0}.", valueOrDefault);
					return valueOrDefault;
				}
			}
			Person.Tracer.TraceDebug((long)galLinkID.GetHashCode(), "Person.FindPersonIdByGALLinkID: No Match found.");
			return null;
		}

		public PersonId UpdatePerson(StoreSession session, ICollection<StoreObjectPropertyChange> propertyChanges, bool isGroup)
		{
			Person.Tracer.TraceDebug<PersonId, bool>(PersonId.TraceId(this.PersonId), "Person.UpdatePerson: Entering, this.PersonId = {0} and isGroup = {1}", this.PersonId, isGroup);
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(propertyChanges, "propertyChanges");
			IEnumerable<IStorePropertyBag> enumerable = from c in this.contacts
			where Person.IsContactWriteable(c)
			select c;
			PersonId result;
			if (enumerable.Any<IStorePropertyBag>())
			{
				Dictionary<VersionedId, List<StoreObjectPropertyChange>> dictionary = new Dictionary<VersionedId, List<StoreObjectPropertyChange>>();
				Dictionary<string, int> dictionary2 = new Dictionary<string, int>();
				foreach (StoreObjectPropertyChange storeObjectPropertyChange in propertyChanges)
				{
					if (!isGroup)
					{
						storeObjectPropertyChange.PropertyDefinition = Person.GetValidatedPropertyDefinition(storeObjectPropertyChange.PropertyDefinition, Person.personPropertiesToContactPropertiesMap);
					}
					else
					{
						storeObjectPropertyChange.PropertyDefinition = Person.GetValidatedPropertyDefinition(storeObjectPropertyChange.PropertyDefinition, Person.personPropertiesToGroupPropertiesMap);
					}
					storeObjectPropertyChange.IsPropertyValidated = true;
					bool flag = false;
					int num = 0;
					if (!dictionary2.ContainsKey(storeObjectPropertyChange.PropertyDefinition.Name))
					{
						dictionary2.Add(storeObjectPropertyChange.PropertyDefinition.Name, num);
					}
					foreach (IStorePropertyBag storePropertyBag in enumerable)
					{
						if (storeObjectPropertyChange.PropertyDefinition.Name == PersonSchema.Bodies.Name || storeObjectPropertyChange.PropertyDefinition.Name == PersonSchema.Members.Name)
						{
							VersionedId versionedIdForPropertyBag = Person.GetVersionedIdForPropertyBag(storePropertyBag);
							StoreObjectPropertyChange change = new StoreObjectPropertyChange((storeObjectPropertyChange.PropertyDefinition.Name == PersonSchema.Bodies.Name) ? ItemSchema.RtfBody : DistributionListSchema.Members, storeObjectPropertyChange.OldValue, storeObjectPropertyChange.NewValue);
							Person.AddToContactChanges(dictionary, versionedIdForPropertyBag, change);
							flag = true;
						}
						else
						{
							bool flag2 = false;
							bool flag3 = Person.IsValidContactForUpdate(storePropertyBag, storeObjectPropertyChange.PropertyDefinition, storeObjectPropertyChange.OldValue, num, dictionary2, out flag2);
							if (flag3)
							{
								VersionedId versionedIdForPropertyBag2 = Person.GetVersionedIdForPropertyBag(storePropertyBag);
								Person.AddToContactChanges(dictionary, versionedIdForPropertyBag2, storeObjectPropertyChange);
								flag = true;
								if (Person.IsEmptyValue(storeObjectPropertyChange.OldValue))
								{
									dictionary2[storeObjectPropertyChange.PropertyDefinition.Name] = num + 1;
									break;
								}
							}
							num++;
							if (!flag3 && Person.IsEmptyValue(storeObjectPropertyChange.OldValue) && !flag2)
							{
								dictionary2[storeObjectPropertyChange.PropertyDefinition.Name] = num;
							}
						}
					}
					if (!flag)
					{
						Person.Tracer.TraceDebug(PersonId.TraceId(this.PersonId), "Person.UpdatePerson: No valid contact found for update. Checking if we can find space in Outlook contact.");
						IStorePropertyBag propertyBag = null;
						if (!Person.FindEmptyPropertyContact(enumerable, storeObjectPropertyChange.PropertyDefinition, out propertyBag))
						{
							throw new SaveConflictException(ServerStrings.ExSaveFailedBecauseOfConflicts(this.PersonId));
						}
						VersionedId versionedIdForPropertyBag3 = Person.GetVersionedIdForPropertyBag(propertyBag);
						Person.AddToContactChanges(dictionary, versionedIdForPropertyBag3, storeObjectPropertyChange);
						flag = true;
					}
				}
				if (!isGroup)
				{
					result = Person.UpdateContacts(session, dictionary, this.personId);
				}
				else
				{
					result = Person.UpdateGroup(session, dictionary, this.personId);
				}
			}
			else
			{
				if (isGroup)
				{
					throw new NotImplementedException("Cannot Save a GAL DL as a PDL");
				}
				this.CheckAndAddNamePropertiesForNewContact(propertyChanges);
				result = this.CreateContact(session, propertyChanges, session.GetDefaultFolderId(DefaultFolderType.Contacts));
			}
			Person.Tracer.TraceDebug<bool>(PersonId.TraceId(this.PersonId), "Person.UpdatePerson: Exiting with isGroup = {0}", isGroup);
			return result;
		}

		public void Delete(StoreSession session, DeleteItemFlags deleteFlags, StoreId deleteInFolder)
		{
			Person.Tracer.TraceDebug<PersonId, StoreId>(PersonId.TraceId(this.PersonId), "Person.Delete: Entering, this.PersonId = {0} and DeleteInFolder = {1}", this.PersonId, deleteInFolder);
			Util.ThrowOnNullArgument(session, "session");
			EnumValidator.ThrowIfInvalid<DeleteItemFlags>(deleteFlags, "deleteFlags");
			if (this.contacts == null || this.contacts.Count == 0)
			{
				Person.Tracer.TraceDebug(PersonId.TraceId(this.PersonId), "Person.Delete: Exiting (no contacts to delete)");
				return;
			}
			IEnumerable<IStorePropertyBag> source = from contact in this.contacts
			where Person.CanContactBeDeleted(contact, deleteInFolder)
			select contact;
			VersionedId[] array = (from contact in source
			select contact.GetValueOrDefault<VersionedId>(ItemSchema.Id, null) into id
			where id != null
			select id).ToArray<VersionedId>();
			if (array.Length == 0)
			{
				Person.Tracer.TraceDebug(PersonId.TraceId(this.PersonId), "Person.Delete: Exiting (no contacts to delete)");
				return;
			}
			AggregateOperationResult aggregateOperationResult = session.Delete(deleteFlags, array);
			if (aggregateOperationResult.OperationResult == OperationResult.Succeeded)
			{
				try
				{
					MailboxSession mailboxSession = session as MailboxSession;
					if (mailboxSession != null && mailboxSession.LogonType != LogonType.Delegated && mailboxSession.Capabilities.CanHaveJunkEmailRule && !mailboxSession.MailboxOwner.ObjectId.IsNullOrEmpty())
					{
						JunkEmailRule junkEmailRule = mailboxSession.JunkEmailRule;
						if (junkEmailRule.IsContactsFolderTrusted)
						{
							junkEmailRule.SynchronizeContactsCache();
							junkEmailRule.Save();
						}
					}
				}
				catch (Exception ex)
				{
					Person.Tracer.TraceDebug(PersonId.TraceId(this.PersonId), string.Format("Person.Delete: Hit exception when update contacts cache. {0}", ex.Message));
				}
			}
			switch (aggregateOperationResult.OperationResult)
			{
			case OperationResult.Succeeded:
				Person.Tracer.TraceDebug(PersonId.TraceId(this.PersonId), "Person.Delete: Exiting (success)");
				return;
			case OperationResult.Failed:
			case OperationResult.PartiallySucceeded:
				throw aggregateOperationResult.GroupOperationResults.First((GroupOperationResult singleResult) => singleResult.OperationResult == OperationResult.Failed).Exception;
			default:
				Person.Tracer.TraceDebug(PersonId.TraceId(this.PersonId), "Person.Delete: Exiting (unexpected exit path)");
				return;
			}
		}

		public string CalculateChangeKey()
		{
			Person.Tracer.TraceDebug<PersonId>(PersonId.TraceId(this.PersonId), "Person.CalculateChangeKey: Entering, this.PersonId = {0}", this.PersonId);
			List<byte[]> list = new List<byte[]>();
			int num = 0;
			for (int i = 0; i < this.contacts.Count; i++)
			{
				VersionedId valueOrDefault = this.contacts[i].GetValueOrDefault<VersionedId>(ItemSchema.Id, null);
				if (valueOrDefault != null)
				{
					byte[] array = valueOrDefault.ChangeKeyAsByteArray();
					if (array != null)
					{
						if (array.Length > num)
						{
							num = array.Length;
						}
						list.Add(array);
					}
				}
			}
			if (list.Count == 0)
			{
				Person.Tracer.TraceDebug(PersonId.TraceId(this.PersonId), "Person.CalculateChangeKey: Exiting (returning null; found no usable changekeys even though at least one contact is part of this Person)");
				return null;
			}
			byte[] array2 = new byte[num + 8];
			BitConverter.GetBytes(Convert.ToInt32(StoreObjectType.Person)).CopyTo(array2, 0);
			BitConverter.GetBytes(num).CopyTo(array2, 4);
			for (int j = 0; j < list.Count; j++)
			{
				for (int k = 0; k < list[j].Length; k++)
				{
					byte[] array3 = array2;
					int num2 = k + 8;
					array3[num2] ^= list[j][k];
				}
			}
			string text = Convert.ToBase64String(array2);
			Person.Tracer.TraceDebug<string>(PersonId.TraceId(this.PersonId), "Person.CalculateChangeKey: Exiting (returning {0})", text);
			return text;
		}

		public Stream GetAttachedPhoto(out string partnerNetworkId, out StoreObjectId contactId)
		{
			Person.Tracer.TraceDebug<PersonId>(PersonId.TraceId(this.PersonId), "Person.GetAttachedPhoto: Entering, this.PersonId = {0}", this.PersonId);
			contactId = null;
			Stream stream = null;
			partnerNetworkId = null;
			StoreObjectId valueOrDefault = this.aggregatedProperties.GetValueOrDefault<StoreObjectId>(PersonSchema.PhotoContactEntryId, null);
			if (valueOrDefault != null)
			{
				using (Contact contact = Item.Bind(this.storeSession, valueOrDefault, ItemBindOption.None, new PropertyDefinition[]
				{
					ContactSchema.PartnerNetworkId
				}) as Contact)
				{
					stream = contact.GetPhotoStream();
					if (stream != null)
					{
						contactId = valueOrDefault;
						partnerNetworkId = contact.PartnerNetworkId;
					}
				}
			}
			Person.Tracer.TraceDebug<string>(PersonId.TraceId(this.PersonId), "Person.GetAttachedPhoto: Exiting (returning {0})", (stream == null || stream.Length == 0L) ? "no photo" : "photo");
			return stream;
		}

		public IStorePropertyBag PropertyBag
		{
			get
			{
				return this.aggregatedProperties;
			}
		}

		public PersonId PersonId
		{
			get
			{
				return this.personId;
			}
		}

		public PersonType PersonType
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<PersonType>(PersonSchema.PersonType, PersonType.Unknown);
			}
		}

		public Guid GALLinkId
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<Guid>(PersonSchema.GALLinkID, Guid.Empty);
			}
		}

		public ExDateTime CreationTime
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<ExDateTime>(PersonSchema.CreationTime, ExDateTime.MinValue);
			}
		}

		public bool IsFavorite
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<bool>(PersonSchema.IsFavorite, false);
			}
		}

		public string DisplayName
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<string>(PersonSchema.DisplayName, null);
			}
		}

		public string DisplayNameFirstLast
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<string>(PersonSchema.DisplayNameFirstLast, null);
			}
		}

		public string DisplayNameLastFirst
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<string>(PersonSchema.DisplayNameLastFirst, null);
			}
		}

		public string FileAs
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<string>(PersonSchema.FileAs, null);
			}
		}

		public string FileAsId
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<string>(PersonSchema.FileAsId, null);
			}
		}

		public string DisplayNamePrefix
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<string>(PersonSchema.DisplayNamePrefix, null);
			}
		}

		public string GivenName
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<string>(PersonSchema.GivenName, null);
			}
		}

		public string MiddleName
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<string>(PersonSchema.MiddleName, null);
			}
		}

		public string Surname
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<string>(PersonSchema.Surname, null);
			}
		}

		public string Generation
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<string>(PersonSchema.Generation, null);
			}
		}

		public string Nickname
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<string>(PersonSchema.Nickname, null);
			}
		}

		public string Alias
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<string>(PersonSchema.Alias, null);
			}
		}

		public string YomiCompanyName
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<string>(PersonSchema.YomiCompanyName, null);
			}
		}

		public string YomiFirstName
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<string>(PersonSchema.YomiFirstName, null);
			}
		}

		public string YomiLastName
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<string>(PersonSchema.YomiLastName, null);
			}
		}

		public string Title
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<string>(PersonSchema.Title, null);
			}
		}

		public string Department
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<string>(PersonSchema.Department, null);
			}
		}

		public string CompanyName
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<string>(PersonSchema.CompanyName, null);
			}
		}

		public string Location
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<string>(PersonSchema.Location, null);
			}
		}

		public Participant EmailAddress
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<Participant>(PersonSchema.EmailAddress, null);
			}
		}

		public PhoneNumber PhoneNumber
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<PhoneNumber>(PersonSchema.PhoneNumber, null);
			}
		}

		public string ImAddress
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<string>(PersonSchema.IMAddress, null);
			}
		}

		public string HomeCity
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<string>(PersonSchema.HomeCity, null);
			}
		}

		public string WorkCity
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<string>(PersonSchema.WorkCity, null);
			}
		}

		public IEnumerable<AttributedValue<PersonNotes>> Bodies
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<PersonNotes>>>(PersonSchema.Bodies, null);
			}
		}

		public int RelevanceScore
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<int>(PersonSchema.RelevanceScore, int.MaxValue);
			}
		}

		public IEnumerable<Participant> EmailAddresses
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<Participant[]>(PersonSchema.EmailAddresses, null);
			}
		}

		public IEnumerable<StoreObjectId> FolderIds
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<StoreObjectId>>(PersonSchema.FolderIds, null);
			}
		}

		public IEnumerable<Attribution> Attributions
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<Attribution>>(PersonSchema.Attributions, null);
			}
		}

		public IEnumerable<Participant> Members
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<Participant[]>(PersonSchema.Members, null);
			}
		}

		public IEnumerable<AttributedValue<string>> DisplayNames
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<string>>>(PersonSchema.DisplayNames, null);
			}
		}

		public IEnumerable<AttributedValue<string>> FileAses
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<string>>>(PersonSchema.FileAses, null);
			}
		}

		public IEnumerable<AttributedValue<string>> FileAsIds
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<string>>>(PersonSchema.FileAsIds, null);
			}
		}

		public IEnumerable<AttributedValue<string>> DisplayNamePrefixes
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<string>>>(PersonSchema.DisplayNamePrefixes, null);
			}
		}

		public IEnumerable<AttributedValue<string>> GivenNames
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<string>>>(PersonSchema.GivenNames, null);
			}
		}

		public IEnumerable<AttributedValue<string>> MiddleNames
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<string>>>(PersonSchema.MiddleNames, null);
			}
		}

		public IEnumerable<AttributedValue<string>> Surnames
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<string>>>(PersonSchema.Surnames, null);
			}
		}

		public IEnumerable<AttributedValue<string>> Generations
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<string>>>(PersonSchema.Generations, null);
			}
		}

		public IEnumerable<AttributedValue<string>> Nicknames
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<string>>>(PersonSchema.Nicknames, null);
			}
		}

		public IEnumerable<AttributedValue<string>> Initials
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<string>>>(PersonSchema.Initials, null);
			}
		}

		public IEnumerable<AttributedValue<string>> YomiCompanyNames
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<string>>>(PersonSchema.YomiCompanyNames, null);
			}
		}

		public IEnumerable<AttributedValue<string>> YomiFirstNames
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<string>>>(PersonSchema.YomiFirstNames, null);
			}
		}

		public IEnumerable<AttributedValue<string>> YomiLastNames
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<string>>>(PersonSchema.YomiLastNames, null);
			}
		}

		public IEnumerable<AttributedValue<PhoneNumber>> BusinessPhoneNumbers
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<PhoneNumber>>>(PersonSchema.BusinessPhoneNumbers, null);
			}
		}

		public IEnumerable<AttributedValue<PhoneNumber>> BusinessPhoneNumbers2
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<PhoneNumber>>>(PersonSchema.BusinessPhoneNumbers2, null);
			}
		}

		public IEnumerable<AttributedValue<PhoneNumber>> HomePhones
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<PhoneNumber>>>(PersonSchema.HomePhones, null);
			}
		}

		public IEnumerable<AttributedValue<PhoneNumber>> HomePhones2
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<PhoneNumber>>>(PersonSchema.HomePhones2, null);
			}
		}

		public IEnumerable<AttributedValue<PhoneNumber>> MobilePhones
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<PhoneNumber>>>(PersonSchema.MobilePhones, null);
			}
		}

		public IEnumerable<AttributedValue<PhoneNumber>> MobilePhones2
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<PhoneNumber>>>(PersonSchema.MobilePhones2, null);
			}
		}

		public IEnumerable<AttributedValue<PhoneNumber>> AssistantPhoneNumbers
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<PhoneNumber>>>(PersonSchema.AssistantPhoneNumbers, null);
			}
		}

		public IEnumerable<AttributedValue<PhoneNumber>> CallbackPhones
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<PhoneNumber>>>(PersonSchema.CallbackPhones, null);
			}
		}

		public IEnumerable<AttributedValue<PhoneNumber>> CarPhones
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<PhoneNumber>>>(PersonSchema.CarPhones, null);
			}
		}

		public IEnumerable<AttributedValue<PhoneNumber>> HomeFaxes
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<PhoneNumber>>>(PersonSchema.HomeFaxes, null);
			}
		}

		public IEnumerable<AttributedValue<PhoneNumber>> OrganizationMainPhones
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<PhoneNumber>>>(PersonSchema.OrganizationMainPhones, null);
			}
		}

		public IEnumerable<AttributedValue<PhoneNumber>> OtherFaxes
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<PhoneNumber>>>(PersonSchema.OtherFaxes, null);
			}
		}

		public IEnumerable<AttributedValue<PhoneNumber>> OtherTelephones
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<PhoneNumber>>>(PersonSchema.OtherTelephones, null);
			}
		}

		public IEnumerable<AttributedValue<PhoneNumber>> OtherPhones2
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<PhoneNumber>>>(PersonSchema.OtherPhones2, null);
			}
		}

		public IEnumerable<AttributedValue<PhoneNumber>> Pagers
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<PhoneNumber>>>(PersonSchema.Pagers, null);
			}
		}

		public IEnumerable<AttributedValue<PhoneNumber>> RadioPhones
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<PhoneNumber>>>(PersonSchema.RadioPhones, null);
			}
		}

		public IEnumerable<AttributedValue<PhoneNumber>> TelexNumbers
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<PhoneNumber>>>(PersonSchema.TelexNumbers, null);
			}
		}

		public IEnumerable<AttributedValue<PhoneNumber>> TTYTDDPhoneNumbers
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<PhoneNumber>>>(PersonSchema.TtyTddPhoneNumbers, null);
			}
		}

		public IEnumerable<AttributedValue<PhoneNumber>> WorkFaxes
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<PhoneNumber>>>(PersonSchema.WorkFaxes, null);
			}
		}

		public IEnumerable<AttributedValue<Participant>> Emails1
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<Participant>>>(PersonSchema.Emails1, null);
			}
		}

		public IEnumerable<AttributedValue<Participant>> Emails2
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<Participant>>>(PersonSchema.Emails2, null);
			}
		}

		public IEnumerable<AttributedValue<Participant>> Emails3
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<Participant>>>(PersonSchema.Emails3, null);
			}
		}

		public IEnumerable<AttributedValue<string>> BusinessHomePages
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<string>>>(PersonSchema.BusinessHomePages, null);
			}
		}

		public IEnumerable<AttributedValue<string>> Schools
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<string>>>(PersonSchema.Schools, null);
			}
		}

		public IEnumerable<AttributedValue<string>> PersonalHomePages
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<string>>>(PersonSchema.PersonalHomePages, null);
			}
		}

		public IEnumerable<AttributedValue<string>> OfficeLocations
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<string>>>(PersonSchema.OfficeLocations, null);
			}
		}

		public IEnumerable<AttributedValue<string>> ImAddresses
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<string>>>(PersonSchema.IMAddresses, null);
			}
		}

		public IEnumerable<AttributedValue<string>> ImAddresses2
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<string>>>(PersonSchema.IMAddresses2, null);
			}
		}

		public IEnumerable<AttributedValue<string>> ImAddresses3
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<string>>>(PersonSchema.IMAddresses3, null);
			}
		}

		public IEnumerable<AttributedValue<PostalAddress>> BusinessAddresses
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<PostalAddress>>>(PersonSchema.BusinessAddresses, null);
			}
		}

		public IEnumerable<AttributedValue<PostalAddress>> HomeAddresses
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<PostalAddress>>>(PersonSchema.HomeAddresses, null);
			}
		}

		public IEnumerable<AttributedValue<PostalAddress>> OtherAddresses
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<PostalAddress>>>(PersonSchema.OtherAddresses, null);
			}
		}

		public IEnumerable<AttributedValue<string>> Titles
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<string>>>(PersonSchema.Titles, null);
			}
		}

		public IEnumerable<AttributedValue<string>> Departments
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<string>>>(PersonSchema.Departments, null);
			}
		}

		public IEnumerable<AttributedValue<string>> CompanyNames
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<string>>>(PersonSchema.CompanyNames, null);
			}
		}

		public IEnumerable<AttributedValue<string>> Managers
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<string>>>(PersonSchema.Managers, null);
			}
		}

		public IEnumerable<AttributedValue<string>> AssistantNames
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<string>>>(PersonSchema.AssistantNames, null);
			}
		}

		public IEnumerable<AttributedValue<string>> Professions
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<string>>>(PersonSchema.Professions, null);
			}
		}

		public IEnumerable<AttributedValue<string>> SpouseNames
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<string>>>(PersonSchema.SpouseNames, null);
			}
		}

		public IEnumerable<AttributedValue<string[]>> Children
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<string[]>>>(PersonSchema.Children, null);
			}
		}

		public IEnumerable<AttributedValue<string>> Hobbies
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<string>>>(PersonSchema.Hobbies, null);
			}
		}

		public IEnumerable<AttributedValue<ExDateTime>> WeddingAnniversaries
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<ExDateTime>>>(PersonSchema.WeddingAnniversaries, null);
			}
		}

		public IEnumerable<AttributedValue<ExDateTime>> Birthdays
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<ExDateTime>>>(PersonSchema.Birthdays, null);
			}
		}

		public IEnumerable<AttributedValue<ExDateTime>> WeddingAnniversariesLocal
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<ExDateTime>>>(PersonSchema.WeddingAnniversariesLocal, null);
			}
		}

		public IEnumerable<AttributedValue<ExDateTime>> BirthdaysLocal
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<ExDateTime>>>(PersonSchema.BirthdaysLocal, null);
			}
		}

		public IEnumerable<AttributedValue<string>> Locations
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<string>>>(PersonSchema.Locations, null);
			}
		}

		public IEnumerable<AttributedValue<ContactExtendedPropertyData>> ExtendedProperties
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<ContactExtendedPropertyData>>>(PersonSchema.ExtendedProperties, null);
			}
		}

		public IEnumerable<AttributedValue<string>> ThirdPartyPhotoUrls
		{
			get
			{
				return this.aggregatedProperties.GetValueOrDefault<IEnumerable<AttributedValue<string>>>(PersonSchema.ThirdPartyPhotoUrls, null);
			}
		}

		public string PreferredThirdPartyPhotoUrl
		{
			get
			{
				IEnumerable<AttributedValue<string>> thirdPartyPhotoUrls = this.ThirdPartyPhotoUrls;
				if (thirdPartyPhotoUrls == null)
				{
					return string.Empty;
				}
				return (from attributedUrl in thirdPartyPhotoUrls
				select attributedUrl.Value).FirstOrDefault<string>() ?? string.Empty;
			}
		}

		public IEnumerable<PersonId> GetSuggestions()
		{
			return Person.GetSuggestions((MailboxSession)this.storeSession, this.contacts);
		}

		private static List<IStorePropertyBag> GetContactsWithPersonId(StoreSession session, PersonId personId, PropertyDefinition[] propertiesToLoad, StoreId folderId)
		{
			List<IStorePropertyBag> result;
			if (session.IsPublicFolderSession)
			{
				if (folderId == null)
				{
					throw new LocalizedException(ServerStrings.NeedFolderIdForPublicFolder);
				}
				result = Person.LoadContactsFromPublicFolder((PublicFolderSession)session, personId, folderId, propertiesToLoad);
			}
			else
			{
				AllPersonContactsEnumerator contactsEnumerator = AllPersonContactsEnumerator.Create((MailboxSession)session, personId, propertiesToLoad);
				result = Person.LoadFromEnumerator(contactsEnumerator);
			}
			return result;
		}

		private static IEnumerable<PersonId> GetSuggestions(MailboxSession mailboxSession, IList<IStorePropertyBag> contacts)
		{
			Person.Tracer.TraceDebug(0L, "Person.GetSuggestions: Entering");
			IList<ContactInfoForSuggestion> personContacts = ContactInfoForSuggestion.ConvertAll(contacts);
			IEnumerable<ContactInfoForSuggestion> contactsEnumerator = ContactInfoForSuggestion.GetContactsEnumerator(mailboxSession);
			IList<ContactLinkingSuggestion> suggestions = ContactLinkingSuggestion.GetSuggestions(mailboxSession.Culture, personContacts, contactsEnumerator);
			List<PersonId> list = new List<PersonId>(suggestions.Count);
			foreach (ContactLinkingSuggestion contactLinkingSuggestion in suggestions)
			{
				list.Add(contactLinkingSuggestion.PersonId);
			}
			Person.Tracer.TraceDebug<int>(0L, "Person.GetSuggestions: Exiting (returning {0} suggestions)", list.Count);
			return list;
		}

		private static bool IsEmptyValue(object value)
		{
			if (value == null)
			{
				return true;
			}
			if (value is PropertyError)
			{
				return PropertyError.IsPropertyNotFound(value);
			}
			return value is string && string.IsNullOrEmpty(value as string);
		}

		private static PropertyDefinition GetValidatedPropertyDefinition(PropertyDefinition propertyDefinition, Dictionary<string, PropertyDefinition> propertiesMap)
		{
			if (propertyDefinition == null)
			{
				throw new ObjectNotFoundException(ServerStrings.ExGetPropsFailed);
			}
			PropertyDefinition result = null;
			if (!propertiesMap.TryGetValue(propertyDefinition.Name, out result))
			{
				result = propertyDefinition;
			}
			return result;
		}

		private static bool IsValidContactForUpdate(IStorePropertyBag contact, PropertyDefinition propertyDefinition, object oldValue, int contactNumber, Dictionary<string, int> propertyToContact, out bool isCurrentValueEmpty)
		{
			isCurrentValueEmpty = false;
			if (!Person.IsContactWriteable(contact))
			{
				return false;
			}
			object obj = contact.TryGetProperty(propertyDefinition);
			isCurrentValueEmpty = Person.IsEmptyValue(obj);
			if (isCurrentValueEmpty && Person.IsEmptyValue(oldValue))
			{
				return propertyToContact[propertyDefinition.Name] == contactNumber;
			}
			if (isCurrentValueEmpty != Person.IsEmptyValue(oldValue))
			{
				return false;
			}
			if (oldValue is string && obj is string)
			{
				string text = oldValue.ToString().ToLower();
				string value = obj.ToString().ToLower();
				return text.Equals(value);
			}
			return oldValue.Equals(obj);
		}

		private static bool FindEmptyPropertyContact(IEnumerable<IStorePropertyBag> writableContacts, PropertyDefinition propertyDefinition, out IStorePropertyBag result)
		{
			result = null;
			foreach (IStorePropertyBag storePropertyBag in writableContacts)
			{
				object value = storePropertyBag.TryGetProperty(propertyDefinition);
				if (Person.IsEmptyValue(value))
				{
					result = storePropertyBag;
					return true;
				}
			}
			return false;
		}

		private static bool IsValidForBodyUpdate(Body body, object oldValue, out bool isCurrentValueEmpty)
		{
			Util.ThrowOnNullArgument(oldValue, "oldValue");
			Util.ThrowOnNullArgument(body, "body");
			bool result = false;
			isCurrentValueEmpty = false;
			PersonNotes personNotes = Person.ReadPersonNotes(body, Person.MaxNotesBytes);
			if (personNotes == null)
			{
				if (Person.IsEmptyValue(oldValue))
				{
					isCurrentValueEmpty = true;
					return true;
				}
				return false;
			}
			else
			{
				if (personNotes.IsTruncated)
				{
					return false;
				}
				if (personNotes.NotesBody != null && (string)oldValue == personNotes.NotesBody)
				{
					result = true;
				}
				if (Person.IsEmptyValue(personNotes.NotesBody) && Person.IsEmptyValue(oldValue))
				{
					isCurrentValueEmpty = true;
				}
				return result;
			}
		}

		private static PersonNotes ReadPersonNotes(Body contactBody, int maxBytesToReadFromStore)
		{
			bool isTruncated;
			string text;
			using (TextReader textReader = contactBody.OpenTextReader(BodyFormat.TextPlain))
			{
				long size = contactBody.Size;
				if (size < (long)maxBytesToReadFromStore)
				{
					isTruncated = false;
					text = textReader.ReadToEnd();
				}
				else
				{
					isTruncated = true;
					char[] array = new char[maxBytesToReadFromStore];
					textReader.Read(array, 0, maxBytesToReadFromStore);
					text = new string(array);
				}
				text = text.TrimEnd(Person.BodyTrimChars);
			}
			PersonNotes result = null;
			if (!string.IsNullOrWhiteSpace(text))
			{
				result = new PersonNotes(text, isTruncated);
			}
			return result;
		}

		private static void AddToContactChanges(Dictionary<VersionedId, List<StoreObjectPropertyChange>> contactChanges, VersionedId id, StoreObjectPropertyChange change)
		{
			List<StoreObjectPropertyChange> list;
			if (contactChanges.ContainsKey(id))
			{
				list = contactChanges[id];
			}
			else
			{
				list = new List<StoreObjectPropertyChange>();
				contactChanges.Add(id, list);
			}
			list.Add(change);
		}

		private static PersonId UpdateContacts(StoreSession session, Dictionary<VersionedId, List<StoreObjectPropertyChange>> contactChanges, PersonId personIdOfContact)
		{
			Person.Tracer.TraceDebug(0L, "Person.UpdateContacts: Entering");
			PersonId result = null;
			bool flag = false;
			bool flag2 = false;
			List<StoreObjectPropertyChange> list = new List<StoreObjectPropertyChange>();
			foreach (VersionedId versionedId in contactChanges.Keys)
			{
				PropertyDefinition[] changedProperties = Person.GetChangedProperties(contactChanges[versionedId]);
				using (Contact contact = Item.Bind(session, versionedId, ItemBindOption.None, changedProperties) as Contact)
				{
					bool flag3 = false;
					List<StoreObjectPropertyChange> list2 = new List<StoreObjectPropertyChange>();
					foreach (StoreObjectPropertyChange storeObjectPropertyChange in contactChanges[versionedId])
					{
						if (storeObjectPropertyChange.PropertyDefinition.Name == ItemSchema.RtfBody.Name)
						{
							flag = true;
							bool flag4 = false;
							if (!flag3 && !Person.IsPropertyChangeInList(list, storeObjectPropertyChange) && Person.IsValidForBodyUpdate(contact.Body, storeObjectPropertyChange.OldValue, out flag4))
							{
								list2.Add(storeObjectPropertyChange);
								flag2 = true;
								if (flag4)
								{
									flag3 = flag4;
									list.Add(storeObjectPropertyChange);
								}
							}
						}
						else
						{
							list2.Add(storeObjectPropertyChange);
						}
					}
					result = Person.ApplyContactChangesAndSave(session, contact, list2);
				}
			}
			if (flag && !flag2)
			{
				throw new SaveConflictException(ServerStrings.ExSaveFailedBecauseOfConflicts(personIdOfContact));
			}
			Person.Tracer.TraceDebug(0L, "Person.UpdateContacts: Exiting");
			return result;
		}

		private static PersonId UpdateGroup(StoreSession session, Dictionary<VersionedId, List<StoreObjectPropertyChange>> groupChanges, PersonId personIdOfGroup)
		{
			Person.Tracer.TraceDebug(0L, "Person.UpdateGroup: Entering");
			PersonId result = null;
			bool flag = false;
			bool flag2 = false;
			foreach (VersionedId versionedId in groupChanges.Keys)
			{
				PropertyDefinition[] changedProperties = Person.GetChangedProperties(groupChanges[versionedId]);
				using (DistributionList distributionList = Item.Bind(session, versionedId, ItemBindOption.None, changedProperties) as DistributionList)
				{
					bool flag3 = false;
					List<StoreObjectPropertyChange> list = new List<StoreObjectPropertyChange>();
					foreach (StoreObjectPropertyChange storeObjectPropertyChange in groupChanges[versionedId])
					{
						if (storeObjectPropertyChange.PropertyDefinition.Name == ItemSchema.RtfBody.Name)
						{
							flag = true;
							bool flag4 = false;
							if (!flag3 && Person.IsValidForBodyUpdate(distributionList.Body, storeObjectPropertyChange.OldValue, out flag4))
							{
								list.Add(storeObjectPropertyChange);
								flag2 = true;
								if (flag4)
								{
									flag3 = flag4;
								}
							}
						}
						else
						{
							list.Add(storeObjectPropertyChange);
						}
					}
					result = Person.ApplyGroupChangesAndSave(distributionList, list);
				}
			}
			if (flag && !flag2)
			{
				throw new SaveConflictException(ServerStrings.ExSaveFailedBecauseOfConflicts(personIdOfGroup));
			}
			Person.Tracer.TraceDebug(0L, "Person.UpdateGroup: Exiting");
			return result;
		}

		private static bool IsPropertyChangeInList(List<StoreObjectPropertyChange> changeList, StoreObjectPropertyChange changeToCheck)
		{
			foreach (StoreObjectPropertyChange storeObjectPropertyChange in changeList)
			{
				if (storeObjectPropertyChange.PropertyDefinition.Name == changeToCheck.PropertyDefinition.Name && storeObjectPropertyChange.OldValue == changeToCheck.OldValue && storeObjectPropertyChange.NewValue == changeToCheck.NewValue)
				{
					return true;
				}
			}
			return false;
		}

		private static PersonId ApplyContactChangesAndSave(StoreSession session, Contact contact, ICollection<StoreObjectPropertyChange> changes)
		{
			Person.Tracer.TraceDebug(0L, "Person.ApplyContactChangesAndSave: Entering");
			string valueOrDefault = contact.GetValueOrDefault<string>(ContactSchema.Email1EmailAddress, string.Empty);
			string valueOrDefault2 = contact.GetValueOrDefault<string>(ContactSchema.Email2EmailAddress, string.Empty);
			string valueOrDefault3 = contact.GetValueOrDefault<string>(ContactSchema.Email3EmailAddress, string.Empty);
			foreach (StoreObjectPropertyChange storeObjectPropertyChange in changes)
			{
				if (!storeObjectPropertyChange.IsPropertyValidated)
				{
					storeObjectPropertyChange.PropertyDefinition = Person.GetValidatedPropertyDefinition(storeObjectPropertyChange.PropertyDefinition, Person.personPropertiesToContactPropertiesMap);
					storeObjectPropertyChange.IsPropertyValidated = true;
				}
				if (Person.IsEmptyValue(storeObjectPropertyChange.NewValue))
				{
					contact.Delete(storeObjectPropertyChange.PropertyDefinition);
				}
				else if (storeObjectPropertyChange.PropertyDefinition.Type == typeof(ExDateTime))
				{
					if (storeObjectPropertyChange.PropertyDefinition.Name == ContactSchema.BirthdayLocal.Name || storeObjectPropertyChange.PropertyDefinition.Name == ContactSchema.WeddingAnniversaryLocal.Name)
					{
						contact[storeObjectPropertyChange.PropertyDefinition] = storeObjectPropertyChange.NewValue;
						ExDateTime exDateTime = ExDateTime.Parse(session.ExTimeZone, storeObjectPropertyChange.NewValue.ToString());
						if (storeObjectPropertyChange.PropertyDefinition.Name == ContactSchema.BirthdayLocal.Name)
						{
							contact[ContactSchema.Birthday] = exDateTime;
						}
						if (storeObjectPropertyChange.PropertyDefinition.Name == ContactSchema.WeddingAnniversaryLocal.Name)
						{
							contact[ContactSchema.WeddingAnniversary] = exDateTime;
						}
					}
					else
					{
						contact[storeObjectPropertyChange.PropertyDefinition] = ExDateTime.Parse(storeObjectPropertyChange.NewValue.ToString());
					}
				}
				else
				{
					if (storeObjectPropertyChange.PropertyDefinition.Name == PersonSchema.Bodies.Name || storeObjectPropertyChange.PropertyDefinition.Name == ItemSchema.RtfBody.Name)
					{
						Body body = contact.Body;
						Util.ThrowOnNullArgument(body, "contactBody");
						using (TextWriter textWriter = body.OpenTextWriter(BodyFormat.TextPlain))
						{
							textWriter.Write(storeObjectPropertyChange.NewValue);
							continue;
						}
					}
					contact[storeObjectPropertyChange.PropertyDefinition] = storeObjectPropertyChange.NewValue;
				}
			}
			Person.SetEmailAddressRelatedFields(contact, valueOrDefault, valueOrDefault2, valueOrDefault3);
			ConflictResolutionResult conflictResolutionResult = contact.Save(SaveMode.ResolveConflicts);
			if (conflictResolutionResult.SaveStatus == SaveResult.IrresolvableConflict)
			{
				throw new SaveConflictException(ServerStrings.ExSaveFailedBecauseOfConflicts(contact.ParentId));
			}
			contact.Load(Person.LoadPropertiesAfterSave);
			PersonId valueOrDefault4 = contact.GetValueOrDefault<PersonId>(ContactSchema.PersonId, null);
			Person.Tracer.TraceDebug(0L, "Person.ApplyContactChangesAndSave: Exiting");
			return valueOrDefault4;
		}

		private static PersonId ApplyGroupChangesAndSave(DistributionList group, ICollection<StoreObjectPropertyChange> changes)
		{
			Person.Tracer.TraceDebug(0L, "Person.ApplyGroupChangesAndSave: Entering");
			foreach (StoreObjectPropertyChange storeObjectPropertyChange in changes)
			{
				if (!storeObjectPropertyChange.IsPropertyValidated)
				{
					storeObjectPropertyChange.PropertyDefinition = Person.GetValidatedPropertyDefinition(storeObjectPropertyChange.PropertyDefinition, Person.personPropertiesToGroupPropertiesMap);
					storeObjectPropertyChange.IsPropertyValidated = true;
				}
				if (storeObjectPropertyChange.PropertyDefinition.Name == PersonSchema.Members.Name)
				{
					if (!Person.IsEmptyValue(storeObjectPropertyChange.NewValue) && Person.IsEmptyValue(storeObjectPropertyChange.OldValue))
					{
						Participant participant = (Participant)storeObjectPropertyChange.NewValue;
						bool flag = false;
						foreach (DistributionListMember distributionListMember in group)
						{
							Participant participant2 = distributionListMember.Participant;
							if (participant2 != null && participant2.AreAddressesEqual(participant))
							{
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							group.Add(participant);
							continue;
						}
						continue;
					}
					else
					{
						if (!Person.IsEmptyValue(storeObjectPropertyChange.NewValue) || Person.IsEmptyValue(storeObjectPropertyChange.OldValue))
						{
							continue;
						}
						Participant v = (Participant)storeObjectPropertyChange.OldValue;
						using (IEnumerator<DistributionListMember> enumerator3 = group.GetEnumerator())
						{
							while (enumerator3.MoveNext())
							{
								DistributionListMember distributionListMember2 = enumerator3.Current;
								Participant participant3 = distributionListMember2.Participant;
								if (participant3 != null && participant3.AreAddressesEqual(v))
								{
									group.Remove(distributionListMember2);
									break;
								}
							}
							continue;
						}
					}
				}
				if (Person.IsEmptyValue(storeObjectPropertyChange.NewValue))
				{
					group.Delete(storeObjectPropertyChange.PropertyDefinition);
				}
				else
				{
					if (storeObjectPropertyChange.PropertyDefinition.Name == PersonSchema.Bodies.Name || storeObjectPropertyChange.PropertyDefinition.Name == ItemSchema.RtfBody.Name)
					{
						Body body = group.Body;
						Util.ThrowOnNullArgument(body, "groupBody");
						using (TextWriter textWriter = body.OpenTextWriter(BodyFormat.TextPlain))
						{
							textWriter.Write(storeObjectPropertyChange.NewValue);
							continue;
						}
					}
					group[storeObjectPropertyChange.PropertyDefinition] = storeObjectPropertyChange.NewValue;
				}
			}
			ConflictResolutionResult conflictResolutionResult = group.Save(SaveMode.ResolveConflicts);
			if (conflictResolutionResult.SaveStatus == SaveResult.IrresolvableConflict)
			{
				throw new SaveConflictException(ServerStrings.ExSaveFailedBecauseOfConflicts(group.ParentId));
			}
			group.Load(Person.LoadPropertiesAfterSave);
			PersonId valueOrDefault = group.GetValueOrDefault<PersonId>(ContactSchema.PersonId, null);
			Person.Tracer.TraceDebug(0L, "Person.ApplyGroupChangesAndSave: Exiting");
			return valueOrDefault;
		}

		private static void SetEmailAddressRelatedFields(Contact contact, string oldEmail1EmailAddress, string oldEmail2EmailAddress, string oldEmail3EmailAddress)
		{
			Person.AdjustEmailAddressRelatedProperties(contact, EmailAddressProperties.Email1, oldEmail1EmailAddress);
			Person.AdjustEmailAddressRelatedProperties(contact, EmailAddressProperties.Email2, oldEmail2EmailAddress);
			Person.AdjustEmailAddressRelatedProperties(contact, EmailAddressProperties.Email3, oldEmail3EmailAddress);
		}

		private static void AdjustEmailAddressRelatedProperties(Contact contact, EmailAddressProperties emailAddressProperties, string oldEmailEmailAddress)
		{
			string valueOrDefault = contact.GetValueOrDefault<string>(emailAddressProperties.Address, null);
			if (!string.IsNullOrEmpty(valueOrDefault))
			{
				string valueOrDefault2 = contact.GetValueOrDefault<string>(emailAddressProperties.DisplayName, null);
				if (valueOrDefault2 == null || valueOrDefault2.Equals(oldEmailEmailAddress))
				{
					contact[emailAddressProperties.DisplayName] = valueOrDefault;
				}
				Participant participant;
				if (Participant.TryParse(valueOrDefault, out participant))
				{
					if (participant.RoutingType != null)
					{
						contact[emailAddressProperties.RoutingType] = participant.RoutingType;
					}
					else
					{
						contact[emailAddressProperties.OriginalDisplayName] = participant.OriginalDisplayName;
						contact[emailAddressProperties.RoutingType] = string.Empty;
						contact[emailAddressProperties.Address] = string.Empty;
					}
					ParticipantEntryId participantEntryId = ParticipantEntryId.FromParticipant(participant, ParticipantEntryIdConsumer.ContactEmailSlot);
					if (participantEntryId == null)
					{
						contact.Delete(emailAddressProperties.OriginalEntryId);
						return;
					}
					contact[emailAddressProperties.OriginalEntryId] = participantEntryId.ToByteArray();
				}
			}
		}

		private static PropertyDefinition[] GetChangedProperties(List<StoreObjectPropertyChange> changes)
		{
			HashSet<PropertyDefinition> hashSet = new HashSet<PropertyDefinition>();
			foreach (StoreObjectPropertyChange storeObjectPropertyChange in changes)
			{
				if (!hashSet.Contains(storeObjectPropertyChange.PropertyDefinition))
				{
					hashSet.Add(storeObjectPropertyChange.PropertyDefinition);
				}
			}
			return hashSet.ToArray<PropertyDefinition>();
		}

		private static bool IsContactFromExternalNetwork(IStorePropertyBag contact)
		{
			if (contact == null)
			{
				return false;
			}
			string valueOrDefault = contact.GetValueOrDefault<string>(ContactSchema.PartnerNetworkId, string.Empty);
			return !string.Equals(valueOrDefault, WellKnownNetworkNames.RecipientCache, StringComparison.OrdinalIgnoreCase) && !string.Equals(valueOrDefault, WellKnownNetworkNames.QuickContacts, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(valueOrDefault);
		}

		private static bool IsContactWriteable(IStorePropertyBag contact)
		{
			if (contact == null)
			{
				return false;
			}
			string valueOrDefault = contact.GetValueOrDefault<string>(ContactSchema.PartnerNetworkId, string.Empty);
			return string.IsNullOrEmpty(valueOrDefault);
		}

		private static bool CanContactBeDeleted(IStorePropertyBag contact, StoreId deleteInFolder)
		{
			bool flag = Person.IsContactFromExternalNetwork(contact);
			if (deleteInFolder == null)
			{
				return !flag;
			}
			return !flag && Person.BelongsToFolder(contact, deleteInFolder);
		}

		private static bool BelongsToFolder(IStorePropertyBag contact, StoreId targetFolder)
		{
			if (contact == null || targetFolder == null)
			{
				return false;
			}
			StoreObjectId valueOrDefault = contact.GetValueOrDefault<StoreObjectId>(StoreObjectSchema.ParentItemId, null);
			return valueOrDefault != null && valueOrDefault.Equals(targetFolder);
		}

		private static List<IStorePropertyBag> LoadFromEnumerator(AllPersonContactsEnumerator contactsEnumerator)
		{
			List<IStorePropertyBag> list = new List<IStorePropertyBag>(12);
			foreach (IStorePropertyBag item in contactsEnumerator)
			{
				list.Add(item);
			}
			return list;
		}

		private static PropertyDefinition[] GetPropertiesToLoad(ICollection<PropertyDefinition> properties, PropertyDefinition[] extendedProperties)
		{
			if (properties != null)
			{
				if (properties.Any((PropertyDefinition property) => !PersonSchema.Instance.AllProperties.Contains(property)))
				{
					throw new ArgumentException("properties must be from PersonSchema", "properties");
				}
			}
			if (extendedProperties != null)
			{
				if (extendedProperties.Any((PropertyDefinition property) => PersonSchema.Instance.AllProperties.Contains(property)))
				{
					throw new ArgumentException("extendedProperties cannot be from PersonSchema", "extendedProperties");
				}
			}
			return PropertyDefinitionCollection.Merge<PropertyDefinition>(new IEnumerable<PropertyDefinition>[]
			{
				Person.RequiredProperties,
				properties,
				extendedProperties
			});
		}

		private static HashSet<PropertyDefinition> GetContactPropertiesToLoad(ICollection<PropertyDefinition> personProperties, IEnumerable<PropertyDefinition> additionalContactProperties)
		{
			HashSet<PropertyDefinition> hashSet = new HashSet<PropertyDefinition>();
			foreach (PropertyDefinition propertyDefinition in personProperties)
			{
				ApplicationAggregatedProperty applicationAggregatedProperty = propertyDefinition as ApplicationAggregatedProperty;
				if (applicationAggregatedProperty != null)
				{
					foreach (PropertyDependency propertyDependency in applicationAggregatedProperty.Dependencies)
					{
						hashSet.Add(propertyDependency.Property);
					}
				}
			}
			if (additionalContactProperties != null)
			{
				foreach (PropertyDefinition item in additionalContactProperties)
				{
					hashSet.Add(item);
				}
			}
			return hashSet;
		}

		private static VersionedId GetVersionedIdForPropertyBag(IStorePropertyBag propertyBag)
		{
			VersionedId valueOrDefault = propertyBag.GetValueOrDefault<VersionedId>(ItemSchema.Id, null);
			if (valueOrDefault == null)
			{
				throw new ObjectNotFoundException(ServerStrings.ExInvalidItemId);
			}
			return valueOrDefault;
		}

		private void CheckAndAddNamePropertiesForNewContact(ICollection<StoreObjectPropertyChange> propertyChanges)
		{
			IEnumerable<StoreObjectPropertyChange> source = from c in propertyChanges
			where (c.PropertyDefinition.Name == ContactSchema.GivenName.Name || c.PropertyDefinition.Name == ContactSchema.Surname.Name) && !Person.IsEmptyValue(c.NewValue)
			select c;
			if (source.Any<StoreObjectPropertyChange>())
			{
				return;
			}
			if (!Person.IsEmptyValue(this.GivenName))
			{
				propertyChanges.Add(new StoreObjectPropertyChange(ContactSchema.GivenName, null, this.GivenName, true));
			}
			if (!Person.IsEmptyValue(this.Surname))
			{
				propertyChanges.Add(new StoreObjectPropertyChange(ContactSchema.Surname, null, this.Surname, true));
			}
		}

		private PersonId CreateContact(StoreSession session, ICollection<StoreObjectPropertyChange> propertyChanges, StoreId parentFolder)
		{
			Person.Tracer.TraceDebug(PersonId.TraceId(this.PersonId), "Person.CreateContact: Entering");
			PersonId result = null;
			this.AddDefaultStoreObjectPropertyChanges(session, propertyChanges, parentFolder);
			using (Contact contact = Contact.Create(session, parentFolder))
			{
				result = Person.ApplyContactChangesAndSave(session, contact, propertyChanges);
			}
			Person.Tracer.TraceDebug(PersonId.TraceId(this.PersonId), "Person.CreateContact: Exiting");
			return result;
		}

		private PersonId CreateGroup(StoreSession session, ICollection<StoreObjectPropertyChange> propertyChanges, StoreId parentFolder)
		{
			Person.Tracer.TraceDebug(PersonId.TraceId(this.PersonId), "Person.CreateGroup: Entering");
			this.AddDefaultStoreObjectPropertyChanges(session, propertyChanges, parentFolder);
			PersonId result;
			using (DistributionList distributionList = DistributionList.Create(session, parentFolder))
			{
				result = Person.ApplyGroupChangesAndSave(distributionList, propertyChanges);
			}
			Person.Tracer.TraceDebug(PersonId.TraceId(this.PersonId), "Person.CreateGroup: Exiting");
			return result;
		}

		private void AddDefaultStoreObjectPropertyChanges(StoreSession session, ICollection<StoreObjectPropertyChange> propertyChanges, StoreId parentFolder)
		{
			propertyChanges.Add(new StoreObjectPropertyChange(ContactSchema.PersonId, null, this.PersonId, true));
			propertyChanges.Add(new StoreObjectPropertyChange(ContactSchema.FileAsId, null, FileAsMapping.LastCommaFirst, true));
			MailboxSession mailboxSession = session as MailboxSession;
			if (mailboxSession != null && mailboxSession.IsDefaultFolderType(parentFolder) == DefaultFolderType.RecipientCache)
			{
				propertyChanges.Add(new StoreObjectPropertyChange(ContactSchema.PartnerNetworkId, null, WellKnownNetworkNames.RecipientCache, true));
				propertyChanges.Add(new StoreObjectPropertyChange(ContactSchema.RelevanceScore, null, 2147483646, true));
			}
		}

		public static IStorePropertyBag InternalGetAggregatedProperties(PersonPropertyAggregationContext aggregationContext, ICollection<PropertyDefinition> requestedProperties, PropertyDefinition[] extendedProperties)
		{
			PersonSchemaProperties personSchemaProperties = new PersonSchemaProperties(extendedProperties, new IEnumerable<PropertyDefinition>[]
			{
				new List<PropertyDefinition>(Person.RequiredProperties),
				requestedProperties
			});
			return ApplicationAggregatedProperty.Aggregate(aggregationContext, personSchemaProperties.All);
		}

		private static void LoadGALDataIfPersonIsGALLinked(MailboxSession session, PersonId personId, ICollection<PropertyDefinition> personProperties, List<IStorePropertyBag> contacts, HashSet<PropertyDefinition> contactProperties)
		{
			Guid valueOrDefault = contacts[0].GetValueOrDefault<Guid>(ContactSchema.GALLinkID, Guid.Empty);
			if (valueOrDefault == Guid.Empty)
			{
				return;
			}
			Person.Tracer.TraceDebug<Guid>(PersonId.TraceId(personId), "Person is GAL Linked, load information from AD - GAL Link ID: {0}.", valueOrDefault);
			ADPersonToContactConverterSet adpersonToContactConverterSet = ADPersonToContactConverterSet.FromContactProperties(personProperties.ToArray<PropertyDefinition>(), contactProperties);
			PropertyDefinition[] adProperties = PropertyDefinitionCollection.Merge<ADPropertyDefinition>(adpersonToContactConverterSet.ADProperties, ContactInfoForLinkingFromDirectory.RequiredADProperties);
			string[] valueOrDefault2 = contacts[0].GetValueOrDefault<string[]>(ContactSchema.SmtpAddressCache, Array<string>.Empty);
			GALLinkingFixer gallinkingFixer = new GALLinkingFixer(session, personId, valueOrDefault, contacts, contactProperties);
			DirectoryPersonSearcher directoryPersonSearcher = new DirectoryPersonSearcher(session.MailboxOwner);
			ADRawEntry adrawEntry = directoryPersonSearcher.FindByAdObjectIdGuidOrSmtpAddressCache(valueOrDefault, valueOrDefault2, adProperties);
			if (adrawEntry == null)
			{
				Person.Tracer.TraceDebug<Guid>(PersonId.TraceId(personId), "No AD contact found by ADObjectId {0} or by SmptAddressCache, it is likely deleted, unlink it from the person.", valueOrDefault);
				gallinkingFixer.TryUnlinkContactsFromGAL();
				return;
			}
			gallinkingFixer.TryUpdateGALLinkingPropertiesIfChanged(adrawEntry);
			IStorePropertyBag storePropertyBag = adpersonToContactConverterSet.Convert(adrawEntry);
			storePropertyBag[ContactSchema.PersonId] = personId;
			contacts.Add(storePropertyBag);
		}

		private static PersonNotes LoadGALNotesIfPersonIsGALLinked(MailboxSession session, PersonId personId, int requestedBytesToFetch, List<IStorePropertyBag> contactPropertyBags)
		{
			Util.ThrowOnNullArgument(contactPropertyBags, "contactPropertyBags");
			if (contactPropertyBags.Count == 0)
			{
				return null;
			}
			Guid valueOrDefault = contactPropertyBags[0].GetValueOrDefault<Guid>(ContactSchema.GALLinkID, Guid.Empty);
			if (valueOrDefault == Guid.Empty)
			{
				return null;
			}
			Person.Tracer.TraceDebug<Guid>(PersonId.TraceId(personId), "Person is GAL Linked, load notes from AD - GAL Link ID: {0}.", valueOrDefault);
			DirectoryPersonSearcher directoryPersonSearcher = new DirectoryPersonSearcher(session.MailboxOwner);
			ADRecipient adrecipient = directoryPersonSearcher.FindADRecipientByObjectId(valueOrDefault);
			if (adrecipient == null)
			{
				return null;
			}
			IADOrgPerson iadorgPerson = adrecipient as IADOrgPerson;
			if (iadorgPerson == null || string.IsNullOrWhiteSpace(iadorgPerson.Notes))
			{
				return null;
			}
			PersonNotes result;
			if (iadorgPerson.Notes.Length > requestedBytesToFetch)
			{
				result = new PersonNotes(iadorgPerson.Notes.Substring(0, requestedBytesToFetch), true);
			}
			else
			{
				result = new PersonNotes(iadorgPerson.Notes, false);
			}
			return result;
		}

		private static IStorePropertyBag ConvertADRawEntryToContact(ADRawEntry adRawEntry, PersonId personId, ICollection<PropertyDefinition> properties)
		{
			ADPersonToContactConverterSet adpersonToContactConverterSet = ADPersonToContactConverterSet.FromPersonProperties(properties.ToArray<PropertyDefinition>(), null);
			IStorePropertyBag storePropertyBag = adpersonToContactConverterSet.Convert(adRawEntry);
			storePropertyBag[ContactSchema.PersonId] = personId;
			return storePropertyBag;
		}

		// Note: this type is marked as 'beforefieldinit'.
		static Person()
		{
			char[] bodyTrimChars = new char[1];
			Person.BodyTrimChars = bodyTrimChars;
			Person.LoadPropertiesAfterSave = new StorePropertyDefinition[]
			{
				ContactSchema.PersonId
			};
			Person.PropertiesToLoadForNotes = new PropertyDefinition[]
			{
				ContactSchema.PersonId,
				ItemSchema.Id,
				ItemSchema.RtfBody,
				PersonSchema.Attributions
			};
			Person.SimpleVirtualPersonaBodiesProperty = new SimpleVirtualPropertyDefinition("InternalStorage:" + PersonSchema.Bodies.Name, PersonSchema.Bodies.Type, PersonSchema.Bodies.PropertyFlags, new PropertyDefinitionConstraint[0]);
			Person.RequiredProperties = new PropertyDefinition[]
			{
				PersonSchema.Id,
				PersonSchema.Attributions
			};
			Person.PersonIdProperty = new PropertyDefinition[]
			{
				ContactSchema.PersonId
			};
			Person.personPropertiesToContactPropertiesMap = new Dictionary<string, PropertyDefinition>(StringComparer.OrdinalIgnoreCase)
			{
				{
					PersonSchema.DisplayName.Name,
					StoreObjectSchema.DisplayName
				},
				{
					PersonSchema.GivenName.Name,
					ContactSchema.GivenName
				},
				{
					PersonSchema.Surname.Name,
					ContactSchema.Surname
				},
				{
					PersonSchema.CompanyName.Name,
					ContactSchema.CompanyName
				},
				{
					PersonSchema.FileAs.Name,
					ContactBaseSchema.FileAs
				}
			};
			Person.personPropertiesToGroupPropertiesMap = new Dictionary<string, PropertyDefinition>(StringComparer.OrdinalIgnoreCase)
			{
				{
					PersonSchema.DisplayName.Name,
					StoreObjectSchema.DisplayName
				}
			};
			Person.GALAggregationRequiredStoreProperties = ContactInfoForLinking.Properties;
		}

		public const int CommonContactsPerPerson = 12;

		public const int RelevanceScoreForIrrelevantEntries = 2147483647;

		private static readonly Trace Tracer = ExTraceGlobals.PersonTracer;

		public static readonly PropertyDefinition[] SuggestionProperties = ContactInfoForSuggestion.Properties;

		private static int MaxNotesBytes = 2097152;

		private static readonly char[] BodyTrimChars;

		private static readonly StorePropertyDefinition[] LoadPropertiesAfterSave;

		private static readonly PropertyDefinition[] PropertiesToLoadForNotes;

		private static readonly SimpleVirtualPropertyDefinition SimpleVirtualPersonaBodiesProperty;

		public static readonly PropertyDefinition[] RequiredProperties;

		private static readonly PropertyDefinition[] PersonIdProperty;

		private static readonly Dictionary<string, PropertyDefinition> personPropertiesToContactPropertiesMap;

		private static readonly Dictionary<string, PropertyDefinition> personPropertiesToGroupPropertiesMap;

		private readonly List<IStorePropertyBag> contacts;

		private readonly IStorePropertyBag aggregatedProperties;

		private readonly PersonId personId;

		private readonly StoreSession storeSession;

		private static readonly PropertyDefinition[] GALAggregationRequiredStoreProperties;
	}
}
