using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ContactsSearchFolderCriteria
	{
		private ContactsSearchFolderCriteria(DefaultFolderType defaultFolderType, DefaultFolderType[] scopeDefaultFolderTypes)
		{
			this.defaultFolderType = defaultFolderType;
			this.scopeDefaultFolderTypes = scopeDefaultFolderTypes;
		}

		public static void ApplyOneTimeSearchFolderCriteria(IXSOFactory xsoFactory, IMailboxSession mailboxSession, ISearchFolder searchFolder, SearchFolderCriteria searchFolderCriteria)
		{
			ContactsSearchFolderCriteria.ApplySearchFolderCriteria(xsoFactory, mailboxSession, searchFolder, searchFolderCriteria, new Action<SearchFolderCriteria>(searchFolder.ApplyOneTimeSearch));
		}

		public static void ApplyContinuousSearchFolderCriteria(IXSOFactory xsoFactory, IMailboxSession mailboxSession, ISearchFolder searchFolder, SearchFolderCriteria searchFolderCriteria)
		{
			ContactsSearchFolderCriteria.ApplySearchFolderCriteria(xsoFactory, mailboxSession, searchFolder, searchFolderCriteria, new Action<SearchFolderCriteria>(searchFolder.ApplyContinuousSearch));
		}

		public static void WaitForSearchFolderPopulation(IXSOFactory xsoFactory, IMailboxSession mailboxSession, ISearchFolder searchFolder)
		{
			if (ContactsSearchFolderCriteria.IsSearchFolderPopulated(searchFolder))
			{
				ContactsSearchFolderCriteria.Tracer.TraceDebug((long)mailboxSession.GetHashCode(), "Search folder is already populated. No wait required.");
				return;
			}
			ContactsSearchFolderCriteria.Tracer.TraceDebug((long)mailboxSession.GetHashCode(), "Waiting for search folder to complete current population.");
			searchFolder.Load();
			using (SearchFolderAsyncSearch searchFolderAsyncSearch = new SearchFolderAsyncSearch((MailboxSession)mailboxSession, searchFolder.Id.ObjectId, null, null))
			{
				if (ContactsSearchFolderCriteria.IsSearchFolderPopulated(searchFolder))
				{
					ContactsSearchFolderCriteria.Tracer.TraceDebug((long)mailboxSession.GetHashCode(), "Search folder population completed.");
				}
				else
				{
					bool arg = searchFolderAsyncSearch.AsyncResult.AsyncWaitHandle.WaitOne(ContactsSearchFolderCriteria.SearchInProgressTimeout);
					ContactsSearchFolderCriteria.Tracer.TraceDebug<bool>((long)mailboxSession.GetHashCode(), "Done waiting, search folder population completed: {0}.", arg);
				}
			}
		}

		private static void ApplySearchFolderCriteria(IXSOFactory xsoFactory, IMailboxSession mailboxSession, ISearchFolder searchFolder, SearchFolderCriteria searchFolderCriteria, Action<SearchFolderCriteria> applySearchCriteriaAction)
		{
			ArgumentValidator.ThrowIfNull("xsoFactory", xsoFactory);
			ArgumentValidator.ThrowIfNull("mailboxSession", mailboxSession);
			ArgumentValidator.ThrowIfNull("searchFolder", searchFolder);
			ArgumentValidator.ThrowIfNull("searchFolderCriteria", searchFolderCriteria);
			ContactsSearchFolderCriteria.Tracer.TraceDebug<SearchFolderCriteria>((long)mailboxSession.GetHashCode(), "Applying search folder criteria: {0}", searchFolderCriteria);
			try
			{
				applySearchCriteriaAction(searchFolderCriteria);
			}
			catch (ObjectNotFoundException)
			{
				List<StoreObjectId> list = new List<StoreObjectId>(searchFolderCriteria.FolderScope.Length);
				foreach (StoreId storeId in searchFolderCriteria.FolderScope)
				{
					list.Add((StoreObjectId)storeId);
				}
				searchFolderCriteria.FolderScope = ContactsSearchFolderCriteria.RemoveDeletedFoldersFromCollection(xsoFactory, mailboxSession, list);
				applySearchCriteriaAction(searchFolderCriteria);
			}
		}

		public static StoreObjectId[] RemoveDeletedFoldersFromCollection(IXSOFactory xsoFactory, IMailboxSession mailboxSession, IEnumerable<StoreObjectId> folderIds)
		{
			ArgumentValidator.ThrowIfNull("xsoFactory", xsoFactory);
			ArgumentValidator.ThrowIfNull("mailboxSession", mailboxSession);
			ArgumentValidator.ThrowIfNull("folderIds", folderIds);
			List<StoreObjectId> list = new List<StoreObjectId>(folderIds);
			list.RemoveAll((StoreObjectId folderId) => ContactsSearchFolderCriteria.IsDeletedFolder(mailboxSession, folderId));
			return list.ToArray();
		}

		public static StoreObjectId[] GetMyContactExtendedFolders(IMailboxSession mailboxSession, StoreObjectId[] myContactsFolders, bool forceCreate)
		{
			ArgumentValidator.ThrowIfNull("mailboxSession", mailboxSession);
			ArgumentValidator.ThrowIfNull("myContactsFolders", myContactsFolders);
			StoreObjectId[] folderIds = ContactsSearchFolderCriteria.GetFolderIds(mailboxSession, forceCreate, ContactsSearchFolderCriteria.BaseScopeForMyContactExtended);
			HashSet<StoreObjectId> hashSet = new HashSet<StoreObjectId>(StoreId.EqualityComparer);
			hashSet.UnionWith(folderIds);
			hashSet.UnionWith(myContactsFolders);
			StoreObjectId[] array = new StoreObjectId[hashSet.Count];
			hashSet.CopyTo(array);
			return array;
		}

		public static void UpdateFolderScope(IXSOFactory xsoFactory, IMailboxSession mailboxSession, ISearchFolder searchFolder, StoreObjectId[] folderScope)
		{
			ArgumentValidator.ThrowIfNull("xsoFactory", xsoFactory);
			ArgumentValidator.ThrowIfNull("mailboxSession", mailboxSession);
			ArgumentValidator.ThrowIfNull("searchFolder", searchFolder);
			ArgumentValidator.ThrowIfNull("folderScope", folderScope);
			SearchFolderCriteria searchFolderCriteria;
			try
			{
				searchFolderCriteria = searchFolder.GetSearchCriteria();
			}
			catch (ObjectNotInitializedException)
			{
				searchFolderCriteria = null;
			}
			if (searchFolderCriteria != null && ContactsSearchFolderCriteria.MatchFolderScope(searchFolderCriteria.FolderScope, folderScope))
			{
				return;
			}
			SearchFolderCriteria searchFolderCriteria2 = ContactsSearchFolderCriteria.CreateSearchCriteria(folderScope);
			ContactsSearchFolderCriteria.Tracer.TraceDebug<SearchFolderCriteria, SearchFolderCriteria>((long)searchFolder.GetHashCode(), "Updating MyContactsFolder Search Criteria since it is different from the current one. Current:{0}, New:{1}.", searchFolderCriteria, searchFolderCriteria2);
			ContactsSearchFolderCriteria.ApplyContinuousSearchFolderCriteria(xsoFactory, mailboxSession, searchFolder, searchFolderCriteria2);
			ContactsSearchFolderCriteria.WaitForSearchFolderPopulation(xsoFactory, mailboxSession, searchFolder);
		}

		public static SearchFolderCriteria CreateSearchCriteria(StoreObjectId[] folderScope)
		{
			return new SearchFolderCriteria(ContactsSearchFolderCriteria.QueryFilter, folderScope)
			{
				DeepTraversal = false
			};
		}

		public DefaultFolderType[] ScopeDefaultFolderTypes
		{
			get
			{
				return this.scopeDefaultFolderTypes;
			}
		}

		public StoreObjectId[] GetDefaultFolderScope(IMailboxSession session, bool forceCreate)
		{
			ArgumentValidator.ThrowIfNull("session", session);
			return ContactsSearchFolderCriteria.GetFolderIds(session, forceCreate, this.scopeDefaultFolderTypes);
		}

		public StoreObjectId[] GetExistingDefaultFolderScope(DefaultFolderContext context)
		{
			ArgumentValidator.ThrowIfNull("context", context);
			List<StoreObjectId> list = new List<StoreObjectId>(this.scopeDefaultFolderTypes.Length);
			foreach (DefaultFolderType defaultFolderType in this.scopeDefaultFolderTypes)
			{
				StoreObjectId storeObjectId = context[defaultFolderType];
				if (storeObjectId != null)
				{
					list.Add(storeObjectId);
				}
			}
			return list.ToArray();
		}

		public void UpdateFolderScope(IXSOFactory xsoFactory, IMailboxSession mailboxSession, StoreObjectId[] scope)
		{
			ArgumentValidator.ThrowIfNull("xsoFactory", xsoFactory);
			ArgumentValidator.ThrowIfNull("mailboxSession", mailboxSession);
			ArgumentValidator.ThrowIfNull("scope", scope);
			StoreObjectId defaultFolderId = mailboxSession.GetDefaultFolderId(this.defaultFolderType);
			if (defaultFolderId == null)
			{
				mailboxSession.CreateDefaultFolder(this.defaultFolderType);
				defaultFolderId = mailboxSession.GetDefaultFolderId(this.defaultFolderType);
			}
			using (ISearchFolder searchFolder = xsoFactory.BindToSearchFolder(mailboxSession, defaultFolderId))
			{
				ContactsSearchFolderCriteria.UpdateFolderScope(xsoFactory, mailboxSession, searchFolder, scope);
			}
		}

		private static StoreObjectId[] GetFolderIds(IMailboxSession session, bool forceCreate, DefaultFolderType[] scopeDefaultFolderTypes)
		{
			List<StoreObjectId> list = new List<StoreObjectId>(scopeDefaultFolderTypes.Length);
			foreach (DefaultFolderType arg in scopeDefaultFolderTypes)
			{
				StoreObjectId defaultFolderId = session.GetDefaultFolderId(arg);
				if (forceCreate && defaultFolderId == null)
				{
					ContactsSearchFolderCriteria.Tracer.TraceDebug<DefaultFolderType>(0L, "Default folder {0} not yet created. Explicitly creating it now.", arg);
					session.CreateDefaultFolder(arg);
					defaultFolderId = session.GetDefaultFolderId(arg);
				}
				if (defaultFolderId != null)
				{
					list.Add(defaultFolderId);
				}
			}
			return list.ToArray();
		}

		private static bool IsDeletedFolder(IStoreSession session, StoreObjectId folderId)
		{
			try
			{
				StoreObjectId parentFolderId = session.GetParentFolderId(folderId);
				if (parentFolderId != null)
				{
					return false;
				}
			}
			catch (ObjectNotFoundException)
			{
			}
			ContactsSearchFolderCriteria.Tracer.TraceDebug<StoreObjectId>(0L, "Folder with does not exist, removed from search folder criteria: {0}", folderId);
			return true;
		}

		private static bool MatchFolderScope(StoreId[] existingFolderScope, StoreId[] newFolderScope)
		{
			HashSet<StoreId> hashSet = new HashSet<StoreId>(existingFolderScope, StoreId.EqualityComparer);
			HashSet<StoreId> equals = new HashSet<StoreId>(newFolderScope, StoreId.EqualityComparer);
			return hashSet.SetEquals(equals);
		}

		private static bool IsSearchFolderPopulated(ISearchFolder folder)
		{
			SearchFolderCriteria searchCriteria = folder.GetSearchCriteria();
			return (searchCriteria.SearchState & SearchState.Rebuild) != SearchState.Rebuild;
		}

		private static readonly Trace Tracer = ExTraceGlobals.MyContactsFolderTracer;

		private static readonly TimeSpan SearchInProgressTimeout = TimeSpan.FromSeconds(30.0);

		private static readonly QueryFilter QueryFilter = new OrFilter(new QueryFilter[]
		{
			new ComparisonFilter(ComparisonOperator.Equal, StoreObjectSchema.ItemClass, "IPM.Contact"),
			new ComparisonFilter(ComparisonOperator.Equal, StoreObjectSchema.ItemClass, "IPM.DistList")
		});

		private readonly DefaultFolderType defaultFolderType;

		private readonly DefaultFolderType[] scopeDefaultFolderTypes;

		private static readonly DefaultFolderType[] BaseScopeForMyContactExtended = new DefaultFolderType[]
		{
			DefaultFolderType.QuickContacts,
			DefaultFolderType.RecipientCache
		};

		public static readonly ContactsSearchFolderCriteria MyContacts = new ContactsSearchFolderCriteria(DefaultFolderType.MyContacts, new DefaultFolderType[]
		{
			DefaultFolderType.Contacts,
			DefaultFolderType.QuickContacts
		});

		public static readonly ContactsSearchFolderCriteria MyContactsExtended = new ContactsSearchFolderCriteria(DefaultFolderType.MyContactsExtended, new DefaultFolderType[]
		{
			DefaultFolderType.Contacts,
			DefaultFolderType.QuickContacts,
			DefaultFolderType.RecipientCache
		});
	}
}
