using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class RecursiveContactsEnumerator : IEnumerable<IStorePropertyBag>, IEnumerable
	{
		public RecursiveContactsEnumerator(IMailboxSession session, IXSOFactory xsoFactory, DefaultFolderType folderType, params PropertyDefinition[] properties)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(properties, "properties");
			this.session = session;
			this.xsoFactory = xsoFactory;
			this.folderType = folderType;
			this.properties = PropertyDefinitionCollection.Merge<PropertyDefinition>(RecursiveContactsEnumerator.RequiredProperties, properties);
		}

		public IEnumerator<IStorePropertyBag> GetEnumerator()
		{
			ContactFoldersEnumeratorOptions foldersEnumeratorOptions = ContactFoldersEnumeratorOptions.SkipHiddenFolders | ContactFoldersEnumeratorOptions.SkipDeletedFolders | ContactFoldersEnumeratorOptions.IncludeParentFolder;
			ContactFoldersEnumerator foldersEnumerator = new ContactFoldersEnumerator(this.session, new XSOFactory(), this.folderType, foldersEnumeratorOptions, new PropertyDefinition[0]);
			foreach (IStorePropertyBag folderPropertyBag in foldersEnumerator)
			{
				VersionedId folderId = folderPropertyBag.GetValueOrDefault<VersionedId>(FolderSchema.Id, null);
				IFolder folder;
				try
				{
					folder = this.xsoFactory.BindToFolder(this.session, folderId.ObjectId);
				}
				catch (ObjectNotFoundException)
				{
					RecursiveContactsEnumerator.Tracer.TraceError<VersionedId, Guid>((long)this.GetHashCode(), "Failed to bind to folder. FolderId: {0}. Mailbox: {1}.", folderId, this.session.MailboxOwner.MailboxInfo.MailboxGuid);
					continue;
				}
				try
				{
					using (IQueryResult contactsQuery = folder.IItemQuery(ItemQueryType.None, null, null, this.properties))
					{
						IStorePropertyBag[] contacts = contactsQuery.GetPropertyBags(100);
						while (contacts.Length > 0)
						{
							foreach (IStorePropertyBag contactPropertyBag in contacts)
							{
								if (contactPropertyBag != null && !(contactPropertyBag.TryGetProperty(ItemSchema.Id) is PropertyError) && ObjectClass.IsContact(contactPropertyBag.GetValueOrDefault<string>(StoreObjectSchema.ItemClass, null)))
								{
									yield return contactPropertyBag;
								}
							}
							contacts = contactsQuery.GetPropertyBags(100);
						}
					}
				}
				finally
				{
					folder.Dispose();
				}
			}
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotSupportedException("Must use the generic version of GetEnumerator.");
		}

		private const int ChunkSize = 100;

		protected static readonly Trace Tracer = ExTraceGlobals.ContactsEnumeratorTracer;

		private static readonly PropertyDefinition[] RequiredProperties = new PropertyDefinition[]
		{
			ItemSchema.Id,
			StoreObjectSchema.ItemClass
		};

		private readonly IMailboxSession session;

		private readonly IXSOFactory xsoFactory;

		private readonly PropertyDefinition[] properties;

		private readonly DefaultFolderType folderType;
	}
}
