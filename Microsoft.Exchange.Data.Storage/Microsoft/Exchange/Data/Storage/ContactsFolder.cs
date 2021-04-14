using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ContactsFolder : Folder, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		internal ContactsFolder(CoreFolder coreFolder) : base(coreFolder)
		{
			if (base.IsNew)
			{
				this.Initialize();
			}
		}

		public static ContactsFolder Create(StoreSession session, StoreId parentId)
		{
			return (ContactsFolder)Folder.Create(session, parentId, StoreObjectType.ContactsFolder);
		}

		public new static ContactsFolder Create(StoreSession session, StoreId parentFolderId, StoreObjectType folderType)
		{
			EnumValidator.ThrowIfInvalid<StoreObjectType>(folderType, StoreObjectType.ContactsFolder);
			return ContactsFolder.Create(session, parentFolderId);
		}

		public new static ContactsFolder Create(StoreSession session, StoreId parentFolderId, StoreObjectType folderType, string displayName, CreateMode createMode)
		{
			EnumValidator.ThrowIfInvalid<CreateMode>(createMode, "createMode");
			EnumValidator.ThrowIfInvalid<StoreObjectType>(folderType, StoreObjectType.ContactsFolder);
			return (ContactsFolder)Folder.Create(session, parentFolderId, StoreObjectType.ContactsFolder, displayName, createMode);
		}

		public new static ContactsFolder Bind(StoreSession session, StoreId folderId)
		{
			return ContactsFolder.Bind(session, folderId, null);
		}

		public new static ContactsFolder Bind(StoreSession session, StoreId folderId, ICollection<PropertyDefinition> prefetchProperties)
		{
			prefetchProperties = InternalSchema.Combine<PropertyDefinition>(FolderSchema.Instance.AutoloadProperties, prefetchProperties);
			return Folder.InternalBind<ContactsFolder>(session, folderId, prefetchProperties);
		}

		public static ContactsFolder Bind(MailboxSession session, DefaultFolderType defaultFolderType)
		{
			return ContactsFolder.Bind(session, defaultFolderType, null);
		}

		public static ContactsFolder Bind(MailboxSession session, DefaultFolderType defaultFolderType, ICollection<PropertyDefinition> prefetchProperties)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			EnumValidator.ThrowIfInvalid<DefaultFolderType>(defaultFolderType, DefaultFolderType.Contacts);
			StoreObjectId defaultFolderId = session.GetDefaultFolderId(defaultFolderType);
			if (defaultFolderId == null)
			{
				throw new ObjectNotFoundException(ServerStrings.ExDefaultFolderNotFound(defaultFolderType));
			}
			return ContactsFolder.Bind(session, defaultFolderId, prefetchProperties);
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<ContactsFolder>(this);
		}

		public object[][] ResolveAmbiguousNameView(string ambiguousName, int limit, SortBy[] sortBy, params PropertyDefinition[] propsToReturn)
		{
			this.CheckDisposed("ResolveAmbiguousNameView");
			if (propsToReturn == null)
			{
				throw new ArgumentNullException("propsToReturn");
			}
			if (propsToReturn.Length <= 0)
			{
				throw new ArgumentException("propsToReturn contains no properties.");
			}
			if (!this.IsValidAmbiguousName(ambiguousName))
			{
				throw new ArgumentException("ambiguousName is invalid ambiguous name");
			}
			object[][] result;
			using (ForwardOnlyFilteredReader forwardOnlyFilteredReader = new ContactsFolder.AnrContactsReader(this, ambiguousName, sortBy, propsToReturn))
			{
				result = this.FetchFromReader("ResolveAmbiguousNameView", forwardOnlyFilteredReader, ContactsFolder.NormalizeLimit(limit, StorageLimits.Instance.AmbiguousNamesViewResultsLimit));
			}
			return result;
		}

		public bool IsValidAmbiguousName(string ambiguousName)
		{
			this.CheckDisposed("IsValidAmbiguousName");
			if (ambiguousName == null)
			{
				throw new ArgumentNullException("ambiguousName");
			}
			if (ambiguousName.Length == 0)
			{
				throw new ArgumentException("ambiguousName");
			}
			return new ContactsFolder.AnrCriteria(ambiguousName, base.Session.InternalPreferedCulture).IsValid;
		}

		public object[][] FindSomeoneView(string ambiguousName, int limit, SortBy[] sortBy, params PropertyDefinition[] propsToReturn)
		{
			this.CheckDisposed("FindSomeoneView");
			if (propsToReturn == null)
			{
				throw new ArgumentNullException("propsToReturn");
			}
			if (propsToReturn.Length <= 0)
			{
				throw new ArgumentException("propsToReturn contains no properties");
			}
			if (!this.IsValidAmbiguousName(ambiguousName))
			{
				throw new ArgumentException("ambiguousName is invalid ambiguous name");
			}
			object[][] result;
			using (ForwardOnlyFilteredReader forwardOnlyFilteredReader = new ContactsFolder.FindSomeoneContactsReader(this, ambiguousName, sortBy, propsToReturn))
			{
				result = this.FetchFromReader("FindSomeone", forwardOnlyFilteredReader, ContactsFolder.NormalizeLimit(limit, StorageLimits.Instance.AmbiguousNamesViewResultsLimit));
			}
			return result;
		}

		public object[][] FindNamesView(IDictionary<PropertyDefinition, object> dictionary, int limit, SortBy[] sortBy, params PropertyDefinition[] propsToReturn)
		{
			this.CheckDisposed("FindNamesView");
			if (propsToReturn == null)
			{
				throw new ArgumentNullException("propsToReturn");
			}
			if (propsToReturn.Length <= 0)
			{
				throw new ArgumentException("propsToReturn contains no properties");
			}
			object[][] result;
			using (ForwardOnlyFilteredReader forwardOnlyFilteredReader = new ContactsFolder.FindNamesContactsReader(this, dictionary ?? new Dictionary<PropertyDefinition, object>(), sortBy, propsToReturn))
			{
				result = this.FetchFromReader("FindNamesView", forwardOnlyFilteredReader, ContactsFolder.NormalizeLimit(limit, StorageLimits.Instance.FindNamesViewResultsLimit));
			}
			return result;
		}

		public FindInfo<Contact> FindByEmailAddress(string emailAddress, params PropertyDefinition[] prefetchProperties)
		{
			this.CheckDisposed("FindByEmailAddress");
			if (emailAddress == null)
			{
				throw new ArgumentNullException("emailAddress");
			}
			IDictionary<PropertyDefinition, object> dictionary = new SortedDictionary<PropertyDefinition, object>();
			dictionary.Add(InternalSchema.EmailAddress, emailAddress);
			for (uint num = 3U; num > 0U; num -= 1U)
			{
				object[][] array = this.FindNamesView(dictionary, 2, new SortBy[]
				{
					new SortBy(InternalSchema.DisplayName, SortOrder.Ascending)
				}, new PropertyDefinition[]
				{
					InternalSchema.ItemId
				});
				if (array.Length > 0)
				{
					try
					{
						return new FindInfo<Contact>((array.Length > 1) ? FindStatus.FoundMultiple : FindStatus.Found, Contact.Bind(base.Session, (VersionedId)array[0][0], prefetchProperties));
					}
					catch (ObjectNotFoundException)
					{
						ExTraceGlobals.StorageTracer.Information<string>((long)this.GetHashCode(), "ContactsFolder::FindByEmailAddress. Race condition: an matching contact was deleted before we had a chance to bind to it, emailAddress=\"{0}\"", emailAddress);
						if (array.Length == 1)
						{
							break;
						}
					}
				}
			}
			return new FindInfo<Contact>(FindStatus.NotFound, null);
		}

		private static int NormalizeLimit(int limit, int maxLimit)
		{
			if (limit == 0 || limit == -1 || limit > maxLimit)
			{
				return maxLimit;
			}
			if (limit > 0)
			{
				return limit;
			}
			throw new ArgumentOutOfRangeException("limit");
		}

		private object[][] FetchFromReader(string callingMethod, ForwardOnlyFilteredReader reader, int rowsToFetch)
		{
			uint num = 3U;
			object[][] nextAsView;
			for (;;)
			{
				try
				{
					nextAsView = reader.GetNextAsView(rowsToFetch);
					break;
				}
				catch (ObjectNotFoundException)
				{
					ExTraceGlobals.StorageTracer.Information<string, uint>((long)this.GetHashCode(), "ContactsFolder::{0}. Race condition: a matching contact was deleted between the two iteration phases of a filter, attemptsLeft=\"{1}\"", callingMethod, num);
					if (num == 0U)
					{
						throw;
					}
				}
				num -= 1U;
			}
			return nextAsView;
		}

		private void Initialize()
		{
			this[InternalSchema.ContainerClass] = "IPF.Contact";
		}

		internal const uint NumberOfRetriesInARaceCondition = 2U;

		internal const float PrefetchModeThreshold = 2.14748365E+09f;

		private class AnrContactsReader : ForwardOnlyFilteredReader
		{
			internal AnrContactsReader(Folder contacts, string ambiguousName, SortBy[] sortBy, params PropertyDefinition[] propertiesToReturn) : base(ContactsFolder.AnrContactsReader.GetPropertySets(propertiesToReturn), (float)propertiesToReturn.Length / (float)ContactsFolder.AnrCriteria.AnrProperties.Count < 2.14748365E+09f)
			{
				this.contacts = contacts;
				this.filterCriteria = new Predicate<PropertyBag>(new ContactsFolder.AnrCriteria(ambiguousName, contacts.Session.InternalPreferedCulture).IsMatch);
				this.sortBy = sortBy;
				this.unfilteredPropertyBag = new QueryResultPropertyBag(this.contacts.Session, base.PropertySets.GetMergedSet());
			}

			public override DisposeTracker GetDisposeTracker()
			{
				return DisposeTracker.Get<ContactsFolder.AnrContactsReader>(this);
			}

			internal override object[][] GetNextAsView(int rowcount)
			{
				byte[] lastEntryId = null;
				return this.GetNextAsView(delegate(object[] transformedForFilterRow)
				{
					if (transformedForFilterRow != null)
					{
						byte[] array = (byte[])transformedForFilterRow[0];
						bool result = rowcount-- > 0 || Util.ValueEquals(lastEntryId, array);
						lastEntryId = array;
						return result;
					}
					return rowcount > 0;
				});
			}

			protected override object[][] TransformRow(object[] unfilteredRow)
			{
				this.unfilteredPropertyBag.SetQueryResultRow(unfilteredRow);
				List<Participant> list = new List<Participant>();
				string valueOrDefault = this.unfilteredPropertyBag.GetValueOrDefault<string>(InternalSchema.ItemClass);
				if (ObjectClass.IsDistributionList(valueOrDefault))
				{
					list.Add(Microsoft.Exchange.Data.Storage.PropertyBag.CheckPropertyValue<Participant>(InternalSchema.DistributionListParticipant, base.PropertySets.TryGetProperty(unfilteredRow, InternalSchema.DistributionListParticipant), null));
				}
				else if (ObjectClass.IsContact(valueOrDefault))
				{
					foreach (ContactEmailSlotParticipantProperty contactEmailSlotParticipantProperty in ContactEmailSlotParticipantProperty.AllInstances.Values)
					{
						list.Add(Microsoft.Exchange.Data.Storage.PropertyBag.CheckPropertyValue<Participant>(contactEmailSlotParticipantProperty, base.PropertySets.TryGetProperty(unfilteredRow, contactEmailSlotParticipantProperty), null));
					}
				}
				List<object[]> list2 = new List<object[]>();
				foreach (Participant participant in list)
				{
					if (!(participant == null) && !(participant.IsRoutable(this.contacts.Session) == false))
					{
						object[] array = (object[])unfilteredRow.Clone();
						foreach (ContactEmailSlotParticipantProperty contactEmailSlotParticipantProperty2 in ContactEmailSlotParticipantProperty.AllInstances.Values)
						{
							base.PropertySets.DeleteProperty(array, contactEmailSlotParticipantProperty2);
							foreach (NativeStorePropertyDefinition propertyDefinition in contactEmailSlotParticipantProperty2.EmailSlotProperties)
							{
								base.PropertySets.DeleteProperty(array, propertyDefinition);
							}
						}
						base.PropertySets.SetProperty(array, InternalSchema.AnrViewParticipant, participant);
						base.PropertySets.SetProperty(array, ParticipantSchema.DisplayName, participant.DisplayName);
						base.PropertySets.SetProperty(array, ParticipantSchema.EmailAddress, participant.EmailAddress);
						base.PropertySets.SetProperty(array, ParticipantSchema.RoutingType, participant.RoutingType);
						this.unfilteredPropertyBag.SetQueryResultRow(array);
						if (this.filterCriteria(this.unfilteredPropertyBag))
						{
							list2.Add(array);
						}
					}
				}
				return list2.ToArray();
			}

			private static ForwardOnlyFilteredReader.PropertySetMixer GetPropertySets(params PropertyDefinition[] propertiesToReturn)
			{
				List<PropertyDefinition> list = new List<PropertyDefinition>();
				list.Add(ContactsFolder.AnrContactsReader.idProperty);
				list.AddRange(ContactsFolder.AnrCriteria.AnrProperties.Cast<PropertyDefinition>());
				Util.AddRange<PropertyDefinition, ContactEmailSlotParticipantProperty>(list, ContactEmailSlotParticipantProperty.AllInstances.Values);
				list.AddRange(ContactsFolder.AnrContactsReader.additionalProperties);
				ForwardOnlyFilteredReader.PropertySetMixer propertySetMixer = new ForwardOnlyFilteredReader.PropertySetMixer();
				propertySetMixer.AddSet(ForwardOnlyFilteredReader.PropertySet.Identification, new PropertyDefinition[]
				{
					ContactsFolder.AnrContactsReader.idProperty
				});
				propertySetMixer.AddSet(ForwardOnlyFilteredReader.PropertySet.ForFilter, list.ToArray());
				propertySetMixer.AddSet(ForwardOnlyFilteredReader.PropertySet.Requested, propertiesToReturn);
				return propertySetMixer;
			}

			protected override QueryResult MakeQuery(params PropertyDefinition[] propertiesToReturn)
			{
				return this.contacts.ItemQuery(ItemQueryType.None, null, this.sortBy, propertiesToReturn);
			}

			private static readonly PropertyDefinition[] additionalProperties = new PropertyDefinition[]
			{
				InternalSchema.DistributionListParticipant,
				InternalSchema.ItemClass
			};

			private static readonly PropertyDefinition idProperty = InternalSchema.EntryId;

			private readonly Folder contacts;

			private readonly Predicate<PropertyBag> filterCriteria;

			private readonly SortBy[] sortBy;

			private readonly QueryResultPropertyBag unfilteredPropertyBag;
		}

		private class AnrCriteria
		{
			public AnrCriteria(string ambiguousName, CultureInfo culture)
			{
				this.culture = culture;
				this.anrChunks = this.GetAnrChunks(ambiguousName);
			}

			private static StorePropertyDefinition[] GetAnrProperties()
			{
				List<StorePropertyDefinition> list = new List<StorePropertyDefinition>(12);
				list.AddRange(new StorePropertyDefinition[]
				{
					StoreObjectSchema.DisplayName,
					ContactSchema.GivenName,
					ContactSchema.Surname,
					ParticipantSchema.DisplayName,
					ParticipantSchema.EmailAddress
				});
				foreach (ContactEmailSlotParticipantProperty contactEmailSlotParticipantProperty in ContactEmailSlotParticipantProperty.AllInstances.Values)
				{
					list.Add(contactEmailSlotParticipantProperty.DisplayNamePropertyDefinition);
					list.Add(contactEmailSlotParticipantProperty.EmailAddressPropertyDefinition);
				}
				return list.ToArray();
			}

			public bool IsMatch(PropertyBag propertyBag)
			{
				string[] array = (string[])this.anrChunks.Clone();
				int num = array.Length;
				foreach (StorePropertyDefinition propertyDefinition in ContactsFolder.AnrCriteria.AnrProperties)
				{
					string valueOrDefault = propertyBag.GetValueOrDefault<string>(propertyDefinition);
					if (valueOrDefault != null)
					{
						foreach (string source in this.GetAnrChunks(valueOrDefault))
						{
							for (int j = 0; j < array.Length; j++)
							{
								if (array[j] != null && this.culture.CompareInfo.IsPrefix(source, array[j], CompareOptions.IgnoreCase))
								{
									array[j] = null;
									if (--num == 0)
									{
										return true;
									}
								}
							}
						}
					}
				}
				return false;
			}

			private string[] GetAnrChunks(string ambiguousName)
			{
				return ambiguousName.Split(ContactsFolder.AnrCriteria.AnrChunkSeparators, StringSplitOptions.RemoveEmptyEntries);
			}

			public bool IsValid
			{
				get
				{
					return this.anrChunks.Length > 0;
				}
			}

			public static readonly char[] AnrChunkSeparators = new char[]
			{
				' ',
				','
			};

			public static readonly ReadOnlyCollection<StorePropertyDefinition> AnrProperties = new ReadOnlyCollection<StorePropertyDefinition>(ContactsFolder.AnrCriteria.GetAnrProperties());

			private readonly CultureInfo culture;

			private readonly string[] anrChunks;
		}

		private class FindNamesContactsReader : ForwardOnlyFilteredReader
		{
			public FindNamesContactsReader(Folder contacts, IDictionary<PropertyDefinition, object> dictionary, SortBy[] sortBy, PropertyDefinition[] propertiesToReturn) : base(ContactsFolder.FindNamesContactsReader.GetPropertySets(dictionary, propertiesToReturn), (float)propertiesToReturn.Length / (float)dictionary.Count < 2.14748365E+09f)
			{
				this.contacts = contacts;
				this.sortBy = sortBy;
				this.culture = contacts.Session.InternalPreferedCulture;
				this.expectedValues = Util.CollectionToArray<object>(dictionary.Values);
				this.forFilterProperties = base.PropertySets.GetSet(ForwardOnlyFilteredReader.PropertySet.ForFilter);
			}

			public override DisposeTracker GetDisposeTracker()
			{
				return DisposeTracker.Get<ContactsFolder.FindNamesContactsReader>(this);
			}

			protected override bool EvaluateFilterCriteria(object[] forFilterRow)
			{
				bool[] array = new bool[3];
				for (int i = 0; i < this.expectedValues.Length; i++)
				{
					if (ContactsFolder.FindNamesContactsReader.emailProperties.ContainsKey(this.forFilterProperties[i]))
					{
						for (int j = 0; j < 3; j++)
						{
							if (!array[j])
							{
								int num = Array.IndexOf<PropertyDefinition>(this.forFilterProperties, ContactsFolder.FindNamesContactsReader.emailProperties[this.forFilterProperties[i]][j]);
								array[j] = !this.IsMatch(forFilterRow[num], this.expectedValues[i]);
							}
						}
					}
					else if (!this.IsMatch(forFilterRow[i], this.expectedValues[i]))
					{
						return false;
					}
				}
				bool[] array2 = array;
				for (int k = 0; k < array2.Length; k++)
				{
					if (!array2[k])
					{
						return true;
					}
				}
				return false;
			}

			private static ForwardOnlyFilteredReader.PropertySetMixer GetPropertySets(IDictionary<PropertyDefinition, object> dictionary, PropertyDefinition[] propertiesToReturn)
			{
				ForwardOnlyFilteredReader.PropertySetMixer propertySetMixer = new ForwardOnlyFilteredReader.PropertySetMixer();
				propertySetMixer.AddSet(ForwardOnlyFilteredReader.PropertySet.Identification, new PropertyDefinition[]
				{
					InternalSchema.EntryId
				});
				propertySetMixer.AddSet(ForwardOnlyFilteredReader.PropertySet.ForFilter, ContactsFolder.FindNamesContactsReader.GetForFilterSet(dictionary.Keys));
				propertySetMixer.AddSet(ForwardOnlyFilteredReader.PropertySet.Requested, propertiesToReturn);
				return propertySetMixer;
			}

			private static PropertyDefinition[] GetForFilterSet(IEnumerable<PropertyDefinition> forFilterPropertiesFromConsumer)
			{
				List<PropertyDefinition> list = new List<PropertyDefinition>(forFilterPropertiesFromConsumer);
				foreach (PropertyDefinition key in forFilterPropertiesFromConsumer)
				{
					if (ContactsFolder.FindNamesContactsReader.emailProperties.ContainsKey(key))
					{
						list.AddRange(ContactsFolder.FindNamesContactsReader.emailProperties[key]);
					}
				}
				return list.ToArray();
			}

			private bool IsMatch(object actualValue, object expectedValue)
			{
				if (actualValue is string && expectedValue is string)
				{
					return this.culture.CompareInfo.IsPrefix((string)actualValue, (string)expectedValue, CompareOptions.IgnoreCase);
				}
				return object.Equals(actualValue, expectedValue);
			}

			protected override QueryResult MakeQuery(params PropertyDefinition[] propertiesToReturn)
			{
				return this.contacts.ItemQuery(ItemQueryType.None, null, this.sortBy, propertiesToReturn);
			}

			protected override bool ShouldIntercept(PropertyDefinition property)
			{
				return base.ShouldIntercept(property) || ContactsFolder.FindNamesContactsReader.emailProperties.ContainsKey(property);
			}

			private readonly Folder contacts;

			private readonly CultureInfo culture;

			private static readonly Dictionary<PropertyDefinition, PropertyDefinition[]> emailProperties = Util.AddElements<Dictionary<PropertyDefinition, PropertyDefinition[]>, KeyValuePair<PropertyDefinition, PropertyDefinition[]>>(new Dictionary<PropertyDefinition, PropertyDefinition[]>(), new KeyValuePair<PropertyDefinition, PropertyDefinition[]>[]
			{
				Util.Pair<PropertyDefinition, PropertyDefinition[]>(InternalSchema.EmailDisplayName, new PropertyDefinition[]
				{
					InternalSchema.Email1DisplayName,
					InternalSchema.Email2DisplayName,
					InternalSchema.Email3DisplayName
				}),
				Util.Pair<PropertyDefinition, PropertyDefinition[]>(InternalSchema.EmailAddress, new PropertyDefinition[]
				{
					InternalSchema.Email1EmailAddress,
					InternalSchema.Email2EmailAddress,
					InternalSchema.Email3EmailAddress
				}),
				Util.Pair<PropertyDefinition, PropertyDefinition[]>(InternalSchema.EmailRoutingType, new PropertyDefinition[]
				{
					InternalSchema.Email1AddrType,
					InternalSchema.Email2AddrType,
					InternalSchema.Email3AddrType
				})
			});

			private readonly object[] expectedValues;

			private readonly PropertyDefinition[] forFilterProperties;

			private readonly SortBy[] sortBy;
		}

		private class FindSomeoneContactsReader : ForwardOnlyFilteredReader
		{
			internal FindSomeoneContactsReader(Folder contacts, string ambiguousName, SortBy[] sortBy, params PropertyDefinition[] propertiesToReturn) : base(ContactsFolder.FindSomeoneContactsReader.GetPropertySets(propertiesToReturn), (float)propertiesToReturn.Length / (float)ContactsFolder.AnrCriteria.AnrProperties.Count < 2.14748365E+09f)
			{
				this.contacts = contacts;
				this.filterCriteria = new Predicate<PropertyBag>(new ContactsFolder.AnrCriteria(ambiguousName, contacts.Session.InternalPreferedCulture).IsMatch);
				this.sortBy = sortBy;
				this.forFilterPropertyBag = new QueryResultPropertyBag(this.contacts.Session, base.PropertySets.GetSet(ForwardOnlyFilteredReader.PropertySet.ForFilter));
			}

			public override DisposeTracker GetDisposeTracker()
			{
				return DisposeTracker.Get<ContactsFolder.FindSomeoneContactsReader>(this);
			}

			protected override bool EvaluateFilterCriteria(object[] forFilterRow)
			{
				this.forFilterPropertyBag.SetQueryResultRow(forFilterRow);
				return this.filterCriteria(this.forFilterPropertyBag);
			}

			private static ForwardOnlyFilteredReader.PropertySetMixer GetPropertySets(params PropertyDefinition[] propertiesToReturn)
			{
				ForwardOnlyFilteredReader.PropertySetMixer propertySetMixer = new ForwardOnlyFilteredReader.PropertySetMixer();
				propertySetMixer.AddSet(ForwardOnlyFilteredReader.PropertySet.Identification, new PropertyDefinition[]
				{
					ContactsFolder.FindSomeoneContactsReader.idProperty
				});
				propertySetMixer.AddSet(ForwardOnlyFilteredReader.PropertySet.ForFilter, Util.CollectionToArray<StorePropertyDefinition>(ContactsFolder.AnrCriteria.AnrProperties));
				propertySetMixer.AddSet(ForwardOnlyFilteredReader.PropertySet.Requested, propertiesToReturn);
				return propertySetMixer;
			}

			protected override QueryResult MakeQuery(params PropertyDefinition[] propertiesToReturn)
			{
				return this.contacts.ItemQuery(ItemQueryType.None, null, this.sortBy, propertiesToReturn);
			}

			private static readonly PropertyDefinition idProperty = InternalSchema.EntryId;

			private readonly Folder contacts;

			private readonly Predicate<PropertyBag> filterCriteria;

			private readonly SortBy[] sortBy;

			private readonly QueryResultPropertyBag forFilterPropertyBag;
		}
	}
}
