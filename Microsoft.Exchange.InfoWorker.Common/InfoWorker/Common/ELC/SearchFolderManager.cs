using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Common;

namespace Microsoft.Exchange.InfoWorker.Common.ELC
{
	internal class SearchFolderManager
	{
		internal SearchFolderManager(MailboxSession mailboxSession)
		{
			this.mailboxSession = mailboxSession;
		}

		public override string ToString()
		{
			if (this.toString == null)
			{
				this.toString = "Mailbox:" + this.mailboxSession.MailboxOwner.ToString() + " in SearchFolderManager.";
			}
			return this.toString;
		}

		internal Folder GetAutoTaggedMailSearchFolder()
		{
			SearchFolder searchFolder = this.CreateSearchFolder("AutoTaggedMailSearchFolder" + Guid.NewGuid().ToString());
			QueryFilter queryFilter = this.CreateAutoTaggedSearchFolderQuery();
			this.PopulateSearchFolder(searchFolder, queryFilter);
			return searchFolder;
		}

		internal Folder GetNoArchiveTagSearchFolder()
		{
			bool flag = false;
			SearchFolder searchFolder = null;
			Folder result;
			try
			{
				searchFolder = this.CreateSearchFolder("NoArchiveTagSearchFolder8534F96D-4183-41fb-8A05-9B7112AE2100");
				SearchFolderCriteria searchFolderCriteria = new SearchFolderCriteria(SearchFolderManager.NoArchiveTagFolderQuery, new StoreId[]
				{
					this.mailboxSession.GetDefaultFolderId(DefaultFolderType.Root)
				});
				searchFolderCriteria.DeepTraversal = true;
				SearchFolderCriteria searchFolderCriteria2 = null;
				try
				{
					searchFolderCriteria2 = searchFolder.GetSearchCriteria();
				}
				catch (ObjectNotInitializedException)
				{
					SearchFolderManager.Tracer.TraceDebug((long)this.GetHashCode(), "{0}: SearchFolderCriteria could not be retrieved from search folder. Need to set it.");
				}
				if (searchFolderCriteria2 == null || !this.SearchCriterionMatches(searchFolderCriteria2, searchFolderCriteria))
				{
					this.PopulateDynamicSearchFolder(searchFolder, searchFolderCriteria);
				}
				flag = true;
				result = searchFolder;
			}
			finally
			{
				if (!flag && searchFolder != null)
				{
					searchFolder.Dispose();
				}
			}
			return result;
		}

		private QueryFilter CreateAutoTaggedSearchFolderQuery()
		{
			QueryFilter queryFilter = new ExistsFilter(StoreObjectSchema.PolicyTag);
			QueryFilter queryFilter2 = new AndFilter(new QueryFilter[]
			{
				new ExistsFilter(StoreObjectSchema.RetentionFlags),
				new BitMaskFilter(StoreObjectSchema.RetentionFlags, 4UL, true)
			});
			return new AndFilter(new QueryFilter[]
			{
				queryFilter,
				queryFilter2
			});
		}

		private SearchFolder CreateSearchFolder(string searchFolderName)
		{
			bool flag = false;
			SearchFolder searchFolder = null;
			SearchFolder result;
			try
			{
				searchFolder = SearchFolder.Create(this.mailboxSession, this.mailboxSession.GetDefaultFolderId(DefaultFolderType.SearchFolders), searchFolderName, CreateMode.OpenIfExists);
				searchFolder.Save();
				searchFolder.Load();
				SearchFolderManager.Tracer.TraceDebug<SearchFolderManager>((long)this.GetHashCode(), "{0}: Created or opened archive search folder.", this);
				flag = true;
				result = searchFolder;
			}
			finally
			{
				if (!flag && searchFolder != null)
				{
					searchFolder.Dispose();
				}
			}
			return result;
		}

		private void PopulateSearchFolder(SearchFolder searchFolder, QueryFilter queryFilter)
		{
			IAsyncResult asyncResult = searchFolder.BeginApplyOneTimeSearch(new SearchFolderCriteria(queryFilter, new StoreId[]
			{
				this.mailboxSession.GetDefaultFolderId(DefaultFolderType.Root)
			})
			{
				DeepTraversal = true
			}, null, null);
			bool flag = asyncResult.AsyncWaitHandle.WaitOne(SearchFolderManager.MaximumSearchTime, false);
			if (flag)
			{
				searchFolder.EndApplyOneTimeSearch(asyncResult);
			}
			else
			{
				SearchFolderManager.Tracer.TraceDebug<SearchFolderManager>((long)this.GetHashCode(), "{0}: ELC Auto Tag search timed out.", this);
			}
			searchFolder.Save();
			searchFolder.Load();
		}

		private void PopulateDynamicSearchFolder(SearchFolder searchFolder, SearchFolderCriteria searchCriteria)
		{
			IAsyncResult asyncResult = searchFolder.BeginApplyContinuousSearch(searchCriteria, null, null);
			bool flag = asyncResult.AsyncWaitHandle.WaitOne(SearchFolderManager.MaximumSearchTime, false);
			if (flag)
			{
				searchFolder.EndApplyContinuousSearch(asyncResult);
			}
			else
			{
				SearchFolderManager.Tracer.TraceDebug<SearchFolderManager>((long)this.GetHashCode(), "{0}: ELC Non Archive Tag search timed out.", this);
			}
			searchFolder.Save();
			searchFolder.Load();
		}

		private bool SearchCriterionMatches(SearchFolderCriteria existingCriteria, SearchFolderCriteria desiredCriteria)
		{
			if (existingCriteria.SearchQuery == null || !existingCriteria.SearchQuery.Equals(desiredCriteria.SearchQuery))
			{
				SearchFolderManager.Tracer.TraceDebug<SearchFolderManager>((long)this.GetHashCode(), "{0}: The existingCriteria.SearchQuery does not match the desiredCriteria.SearchQuery", this);
				return false;
			}
			if (existingCriteria.FolderScope == null || existingCriteria.FolderScope.Length != desiredCriteria.FolderScope.Length || !existingCriteria.FolderScope[0].Equals(this.mailboxSession.GetDefaultFolderId(DefaultFolderType.Root)))
			{
				SearchFolderManager.Tracer.TraceDebug<SearchFolderManager>((long)this.GetHashCode(), "{0}: The existingCriteria.FolderScope does not match the desiredCriteria.FolderScope", this);
				return false;
			}
			if (existingCriteria.DeepTraversal != desiredCriteria.DeepTraversal)
			{
				SearchFolderManager.Tracer.TraceDebug<SearchFolderManager>((long)this.GetHashCode(), "{0}: The existingCriteria.DeepTraversal does not match the desiredCriteria.DeepTraversal", this);
				return false;
			}
			SearchFolderManager.Tracer.TraceDebug<SearchFolderManager>((long)this.GetHashCode(), "{0}: The criteria match.", this);
			return true;
		}

		private const int AutoTagBit = 4;

		private const string ArchiveSearchFolderGuid = "8534F96D-4183-41fb-8A05-9B7112AE2100";

		private static readonly TimeSpan MaximumSearchTime = TimeSpan.FromSeconds(300.0);

		protected static readonly Trace Tracer = ExTraceGlobals.AutoTaggingTracer;

		private static readonly QueryFilter NoArchiveTagFolderQuery = new NotFilter(new ExistsFilter(StoreObjectSchema.ArchiveTag));

		private string toString;

		private MailboxSession mailboxSession;
	}
}
