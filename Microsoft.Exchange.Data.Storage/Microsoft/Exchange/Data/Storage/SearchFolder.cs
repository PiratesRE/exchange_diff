using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SearchFolder : Folder, ISearchFolder, IFolder, IStoreObject, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		internal SearchFolder(CoreFolder coreFolder) : base(coreFolder)
		{
		}

		public static SearchFolder Create(StoreSession session, StoreId parentFolderId)
		{
			return (SearchFolder)Folder.Create(session, parentFolderId, StoreObjectType.SearchFolder);
		}

		public static SearchFolder Create(StoreSession session, StoreId parentFolderId, string displayName, CreateMode createMode)
		{
			return (SearchFolder)Folder.Create(session, parentFolderId, StoreObjectType.SearchFolder, displayName, createMode);
		}

		public new static SearchFolder Create(StoreSession session, StoreId parentFolderId, StoreObjectType folderType)
		{
			throw new NotSupportedException();
		}

		public new static SearchFolder Create(StoreSession session, StoreId parentFolderId, StoreObjectType folderType, string displayName, CreateMode createMode)
		{
			throw new NotSupportedException();
		}

		public new static SearchFolder Bind(StoreSession session, StoreId folderId)
		{
			return SearchFolder.Bind(session, folderId, null);
		}

		public new static SearchFolder Bind(StoreSession session, StoreId folderId, params PropertyDefinition[] propsToReturn)
		{
			return SearchFolder.Bind(session, folderId, (ICollection<PropertyDefinition>)propsToReturn);
		}

		public new static SearchFolder Bind(StoreSession session, StoreId folderId, ICollection<PropertyDefinition> propsToReturn)
		{
			propsToReturn = InternalSchema.Combine<PropertyDefinition>(FolderSchema.Instance.AutoloadProperties, propsToReturn);
			return Folder.InternalBind<SearchFolder>(session, folderId, propsToReturn);
		}

		public static SearchFolder Bind(MailboxSession session, DefaultFolderType defaultFolderType)
		{
			return SearchFolder.Bind(session, defaultFolderType, null);
		}

		public static SearchFolder Bind(MailboxSession session, DefaultFolderType defaultFolderType, ICollection<PropertyDefinition> propsToReturn)
		{
			EnumValidator.ThrowIfInvalid<DefaultFolderType>(defaultFolderType, "defaultFolderType");
			DefaultFolder defaultFolder = session.InternalGetDefaultFolder(defaultFolderType);
			if (defaultFolder.StoreObjectType != StoreObjectType.OutlookSearchFolder && defaultFolder.StoreObjectType != StoreObjectType.SearchFolder)
			{
				throw new ArgumentOutOfRangeException("defaultFolderType");
			}
			StoreObjectId folderId = session.SafeGetDefaultFolderId(defaultFolderType);
			ObjectNotFoundException ex = null;
			for (int i = 0; i < 2; i++)
			{
				try
				{
					return SearchFolder.Bind(session, folderId, propsToReturn);
				}
				catch (ObjectNotFoundException ex2)
				{
					ex = ex2;
					ExTraceGlobals.StorageTracer.Information<DefaultFolderType>(0L, "SearchFolder::Bind(defaultFolderType): attempting to recreate {0}.", defaultFolderType);
					if (!session.TryFixDefaultFolderId(defaultFolderType, out folderId))
					{
						throw;
					}
				}
			}
			throw ex;
		}

		private static SetSearchCriteriaFlags CalculateSearchCriteriaFlags(bool deepTraversal, bool useCiForComplexQueries, SetSearchCriteriaFlags statisticsOnly, bool failNonContentIndexedSearch, SearchFolder.SearchType searchType)
		{
			EnumValidator.ThrowIfInvalid<SearchFolder.SearchType>(searchType, "searchType");
			SetSearchCriteriaFlags setSearchCriteriaFlags = SetSearchCriteriaFlags.None;
			if (deepTraversal)
			{
				setSearchCriteriaFlags |= SetSearchCriteriaFlags.Recursive;
			}
			else
			{
				setSearchCriteriaFlags |= SetSearchCriteriaFlags.Shallow;
			}
			if (useCiForComplexQueries)
			{
				setSearchCriteriaFlags |= SetSearchCriteriaFlags.UseCiForComplexQueries;
			}
			setSearchCriteriaFlags |= statisticsOnly;
			if (failNonContentIndexedSearch)
			{
				setSearchCriteriaFlags |= SetSearchCriteriaFlags.FailNonContentIndexedSearch;
			}
			switch (searchType)
			{
			case SearchFolder.SearchType.RunOnce:
				setSearchCriteriaFlags |= (SetSearchCriteriaFlags.ContentIndexed | SetSearchCriteriaFlags.Static);
				break;
			case SearchFolder.SearchType.ContinousUpdate:
				setSearchCriteriaFlags |= SetSearchCriteriaFlags.NonContentIndexed;
				break;
			}
			return setSearchCriteriaFlags | SetSearchCriteriaFlags.Restart;
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<SearchFolder>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				this.DisposeCurrentSearch();
			}
			base.InternalDispose(disposing);
		}

		public SearchFolderCriteria GetSearchCriteria()
		{
			this.CheckDisposed("GetSearchCriteria");
			if (SearchFolder.TestInjectCriteriaFailure != null)
			{
				SearchFolder.TestInjectCriteriaFailure();
			}
			return base.CoreFolder.GetSearchCriteria(true);
		}

		public void ApplyContinuousSearch(SearchFolderCriteria searchFolderCriteria)
		{
			this.CheckDisposed("ApplyContinuousSearch");
			ExTraceGlobals.StorageTracer.Information<SearchFolderCriteria>((long)this.GetHashCode(), "SearchFolder::ApplyContinuousSearch. SearchCriteria = {0}", searchFolderCriteria);
			SetSearchCriteriaFlags setSearchCriteriaFlags;
			this.PreprocessSearch(searchFolderCriteria, SearchFolder.SearchType.ContinousUpdate, out setSearchCriteriaFlags);
			base.CoreFolder.SetSearchCriteria(searchFolderCriteria, setSearchCriteriaFlags);
		}

		public void ApplyOneTimeSearch(SearchFolderCriteria searchFolderCriteria)
		{
			this.CheckDisposed("ApplyOneTimeSearch");
			ExTraceGlobals.StorageTracer.Information<SearchFolderCriteria>((long)this.GetHashCode(), "SearchFolder::ApplyOneTimeSearch. SearchCriteria = {0}", searchFolderCriteria);
			SetSearchCriteriaFlags setSearchCriteriaFlags;
			this.PreprocessSearch(searchFolderCriteria, SearchFolder.SearchType.RunOnce, out setSearchCriteriaFlags);
			base.CoreFolder.SetSearchCriteria(searchFolderCriteria, setSearchCriteriaFlags);
		}

		public IAsyncResult BeginApplyContinuousSearch(SearchFolderCriteria searchFolderCriteria, AsyncCallback asyncCallback, object state)
		{
			this.CheckDisposed("BeginApplyContinuousSearch");
			ExTraceGlobals.StorageTracer.Information<SearchFolderCriteria>((long)this.GetHashCode(), "SearchFolder::BeginApplyContinuousSearch. SearchCriteria = {0}", searchFolderCriteria);
			SetSearchCriteriaFlags setSearchCriteriaFlags;
			this.PreprocessSearch(searchFolderCriteria, SearchFolder.SearchType.ContinousUpdate, out setSearchCriteriaFlags);
			this.asyncSearch = new SearchFolderAsyncSearch(base.Session, base.Id.ObjectId, asyncCallback, state);
			base.CoreFolder.SetSearchCriteria(searchFolderCriteria, setSearchCriteriaFlags);
			return this.asyncSearch.AsyncResult;
		}

		public void EndApplyContinuousSearch(IAsyncResult asyncResult)
		{
			this.CheckDisposed("EndApplyContinuousSearch");
			ExTraceGlobals.StorageTracer.Information((long)this.GetHashCode(), "SearchFolder::EndApplyContinuousSearch.");
			this.EndApplySearch(asyncResult);
		}

		public IAsyncResult BeginApplyOneTimeSearch(SearchFolderCriteria searchFolderCriteria, AsyncCallback asyncCallback, object state)
		{
			this.CheckDisposed("BeginApplyOneTimeSearch");
			ExTraceGlobals.StorageTracer.Information<SearchFolderCriteria>((long)this.GetHashCode(), "SearchFolder::BeginApplyOneTimeSearch. SearchCriteria = {0}", searchFolderCriteria);
			SetSearchCriteriaFlags setSearchCriteriaFlags;
			this.PreprocessSearch(searchFolderCriteria, SearchFolder.SearchType.RunOnce, out setSearchCriteriaFlags);
			this.asyncSearch = new SearchFolderAsyncSearch(base.Session, base.Id.ObjectId, asyncCallback, state);
			base.CoreFolder.SetSearchCriteria(searchFolderCriteria, setSearchCriteriaFlags);
			return this.asyncSearch.AsyncResult;
		}

		public void EndApplyOneTimeSearch(IAsyncResult asyncResult)
		{
			this.CheckDisposed("EndApplyOneTimeSearch");
			ExTraceGlobals.StorageTracer.Information((long)this.GetHashCode(), "SearchFolder::EndApplyOneTimeSearch.");
			this.EndApplySearch(asyncResult);
		}

		public bool IsPopulated()
		{
			this.CheckDisposed("IsPopulated");
			ExTraceGlobals.StorageTracer.Information((long)this.GetHashCode(), "SearchFolder::IsPopulated.");
			SearchFolderCriteria searchCriteria = this.GetSearchCriteria();
			return (searchCriteria.SearchState & SearchState.Rebuild) != SearchState.Rebuild;
		}

		protected override void CheckItemBelongsToThisFolder(StoreObjectId storeObjectIds)
		{
		}

		private void EndApplySearch(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			if (this.asyncSearch == null)
			{
				throw new InvalidOperationException(ServerStrings.ExNoSearchHasBeenInitiated);
			}
			if (this.asyncSearch.AsyncResult != asyncResult)
			{
				throw new ArgumentException(ServerStrings.ExInvalidAsyncResult, "asyncResult");
			}
			this.asyncSearch.AsyncResult.AsyncWaitHandle.WaitOne();
			this.DisposeCurrentSearch();
		}

		private void DisposeCurrentSearch()
		{
			if (this.asyncSearch != null)
			{
				this.asyncSearch.Dispose();
				this.asyncSearch = null;
			}
		}

		private void PreprocessSearch(SearchFolderCriteria searchFolderCriteria, SearchFolder.SearchType searchType, out SetSearchCriteriaFlags searchCriteriaFlags)
		{
			if (searchFolderCriteria == null)
			{
				throw new ArgumentNullException("searchFolderCriteria");
			}
			if (this.IsDirty)
			{
				throw new InvalidOperationException(ServerStrings.ExMustSaveFolderToApplySearch);
			}
			this.DisposeCurrentSearch();
			SetSearchCriteriaFlags setSearchCriteriaFlags = SetSearchCriteriaFlags.None;
			if (searchFolderCriteria.StatisticsOnly)
			{
				setSearchCriteriaFlags |= SetSearchCriteriaFlags.StatisticsOnly;
			}
			if (searchFolderCriteria.EstimateCountOnly)
			{
				setSearchCriteriaFlags |= SetSearchCriteriaFlags.EstimateCountOnly;
			}
			searchCriteriaFlags = SearchFolder.CalculateSearchCriteriaFlags(searchFolderCriteria.DeepTraversal, searchFolderCriteria.UseCiForComplexQueries, setSearchCriteriaFlags, searchFolderCriteria.FailNonContentIndexedSearch, searchType);
		}

		internal static SearchFolder.CriteriaFailureDelegate TestInjectCriteriaFailure;

		private SearchFolderAsyncSearch asyncSearch;

		internal delegate void CriteriaFailureDelegate();

		private enum SearchType
		{
			RunOnce,
			ContinousUpdate
		}
	}
}
