using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ContactsEnumerator<T> : IEnumerable<!0>, IEnumerable
	{
		private ContactsEnumerator(IMailboxSession session, DefaultFolderType folderType, SortBy[] sortColumns, PropertyDefinition[] properties, Func<IStorePropertyBag, T> converter, ContactsEnumerator<T>.SupportedContactItemClasses includedItemClasses, IXSOFactory xsoFactory)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(properties, "properties");
			Util.ThrowOnNullArgument(converter, "converter");
			Util.ThrowOnNullArgument(xsoFactory, "xsoFactory");
			this.session = session;
			this.folderType = folderType;
			this.sortColumns = sortColumns;
			this.properties = PropertyDefinitionCollection.Merge<PropertyDefinition>(ContactsEnumerator<T>.requiredProperties, properties);
			this.converter = converter;
			this.includedItemClasses = includedItemClasses;
			this.xsoFactory = xsoFactory;
		}

		public static ContactsEnumerator<T> CreateContactsOnlyEnumerator(IMailboxSession session, DefaultFolderType folderType, PropertyDefinition[] properties, Func<IStorePropertyBag, T> converter, IXSOFactory xsoFactory)
		{
			return new ContactsEnumerator<T>(session, folderType, null, properties, converter, ContactsEnumerator<T>.SupportedContactItemClasses.Contacts, xsoFactory);
		}

		public static ContactsEnumerator<T> CreateContactsOnlyEnumerator(IMailboxSession session, DefaultFolderType folderType, SortBy[] sortColumns, PropertyDefinition[] properties, Func<IStorePropertyBag, T> converter, IXSOFactory xsoFactory)
		{
			return new ContactsEnumerator<T>(session, folderType, sortColumns, properties, converter, ContactsEnumerator<T>.SupportedContactItemClasses.Contacts, xsoFactory);
		}

		public static ContactsEnumerator<T> CreateContactsAndPdlsEnumerator(IMailboxSession session, DefaultFolderType folderType, PropertyDefinition[] properties, Func<IStorePropertyBag, T> converter, IXSOFactory xsoFactory)
		{
			return new ContactsEnumerator<T>(session, folderType, null, properties, converter, ContactsEnumerator<T>.SupportedContactItemClasses.Contacts | ContactsEnumerator<T>.SupportedContactItemClasses.Pdls, xsoFactory);
		}

		public IEnumerator<T> GetEnumerator()
		{
			if (this.session.GetDefaultFolderId(this.folderType) == null)
			{
				this.session.CreateDefaultFolder(this.folderType);
			}
			using (IFolder allContactsFolder = this.xsoFactory.BindToFolder(this.session, this.folderType))
			{
				using (IQueryResult contactsQuery = allContactsFolder.IItemQuery(ItemQueryType.None, null, this.sortColumns, this.properties))
				{
					IStorePropertyBag[] contacts = contactsQuery.GetPropertyBags(100);
					while (contacts.Length > 0)
					{
						foreach (IStorePropertyBag contact in contacts)
						{
							if (contact != null && !(contact.TryGetProperty(ItemSchema.Id) is PropertyError) && this.ShouldEnumerateItemClass(contact))
							{
								yield return this.converter(contact);
							}
						}
						contacts = contactsQuery.GetPropertyBags(100);
					}
				}
			}
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotSupportedException("Must use the generic version of GetEnumerator.");
		}

		private bool ShouldEnumerateItemClass(IStorePropertyBag contactPropertyBag)
		{
			string text = contactPropertyBag.TryGetProperty(StoreObjectSchema.ItemClass) as string;
			if (!string.IsNullOrEmpty(text))
			{
				if (ObjectClass.IsContact(text))
				{
					return (this.includedItemClasses & ContactsEnumerator<T>.SupportedContactItemClasses.Contacts) == ContactsEnumerator<T>.SupportedContactItemClasses.Contacts;
				}
				if (ObjectClass.IsDistributionList(text))
				{
					return (this.includedItemClasses & ContactsEnumerator<T>.SupportedContactItemClasses.Pdls) == ContactsEnumerator<T>.SupportedContactItemClasses.Pdls;
				}
			}
			return false;
		}

		private const int ChunkSize = 100;

		private static readonly PropertyDefinition[] requiredProperties = new PropertyDefinition[]
		{
			ItemSchema.Id,
			StoreObjectSchema.ItemClass
		};

		private readonly IXSOFactory xsoFactory;

		private readonly IMailboxSession session;

		private readonly PropertyDefinition[] properties;

		private readonly DefaultFolderType folderType;

		private readonly Func<IStorePropertyBag, T> converter;

		private readonly ContactsEnumerator<T>.SupportedContactItemClasses includedItemClasses;

		private readonly SortBy[] sortColumns;

		[Flags]
		private enum SupportedContactItemClasses
		{
			None = 0,
			Contacts = 1,
			Pdls = 2
		}
	}
}
