using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SearchFolderCriteria
	{
		public SearchFolderCriteria(QueryFilter searchQuery, StoreId[] folderScope) : this(searchQuery, folderScope, Microsoft.Mapi.SearchState.None)
		{
		}

		internal SearchFolderCriteria(QueryFilter searchQuery, StoreId[] folderScope, SearchState searchState)
		{
			SearchFolderCriteria.CheckFolderScope(folderScope);
			this.FolderScope = folderScope;
			this.SearchQuery = searchQuery;
			this.searchState = searchState;
		}

		public bool DeepTraversal
		{
			get
			{
				return (this.searchState & Microsoft.Mapi.SearchState.Recursive) == Microsoft.Mapi.SearchState.Recursive;
			}
			set
			{
				if (value)
				{
					this.searchState |= Microsoft.Mapi.SearchState.Recursive;
					return;
				}
				this.searchState &= ~Microsoft.Mapi.SearchState.Recursive;
			}
		}

		public bool UseCiForComplexQueries
		{
			get
			{
				return (this.searchState & Microsoft.Mapi.SearchState.UseCiForComplexQueries) == Microsoft.Mapi.SearchState.UseCiForComplexQueries;
			}
			set
			{
				if (value)
				{
					this.searchState |= Microsoft.Mapi.SearchState.UseCiForComplexQueries;
					return;
				}
				this.searchState &= ~Microsoft.Mapi.SearchState.UseCiForComplexQueries;
			}
		}

		public bool StatisticsOnly
		{
			get
			{
				return (this.searchState & Microsoft.Mapi.SearchState.StatisticsOnly) == Microsoft.Mapi.SearchState.StatisticsOnly;
			}
			set
			{
				if (value)
				{
					this.searchState |= Microsoft.Mapi.SearchState.StatisticsOnly;
					return;
				}
				this.searchState &= ~Microsoft.Mapi.SearchState.StatisticsOnly;
			}
		}

		public bool FailNonContentIndexedSearch
		{
			get
			{
				return (this.searchState & Microsoft.Mapi.SearchState.CiOnly) == Microsoft.Mapi.SearchState.CiOnly;
			}
			set
			{
				if (value)
				{
					this.searchState |= Microsoft.Mapi.SearchState.CiOnly;
					return;
				}
				this.searchState &= ~Microsoft.Mapi.SearchState.CiOnly;
			}
		}

		public bool EstimateCountOnly
		{
			get
			{
				return (this.searchState & Microsoft.Mapi.SearchState.EstimateCountOnly) == Microsoft.Mapi.SearchState.EstimateCountOnly;
			}
			set
			{
				if (value)
				{
					this.searchState |= Microsoft.Mapi.SearchState.EstimateCountOnly;
					return;
				}
				this.searchState &= ~Microsoft.Mapi.SearchState.EstimateCountOnly;
			}
		}

		public int? MaximumResultsCount
		{
			get
			{
				return this.maximumResultsCount;
			}
			set
			{
				this.maximumResultsCount = value;
			}
		}

		public QueryFilter SearchQuery
		{
			get
			{
				return this.searchQuery;
			}
			set
			{
				this.searchQuery = value;
			}
		}

		public StoreId[] FolderScope
		{
			get
			{
				return this.folderScope;
			}
			set
			{
				SearchFolderCriteria.CheckFolderScope(value);
				this.folderScope = value;
			}
		}

		public SearchState SearchState
		{
			get
			{
				return (SearchState)this.searchState;
			}
		}

		public override string ToString()
		{
			return string.Format("Filter = {0}. FolderScopeCount = {1}. DeepTraversal = {2}. UseCiForComplexQueries = {3}", new object[]
			{
				this.SearchQuery,
				(this.folderScope != null) ? this.folderScope.Length : 0,
				this.DeepTraversal,
				this.UseCiForComplexQueries
			});
		}

		private static void CheckFolderScope(StoreId[] folderScope)
		{
			if (folderScope != null)
			{
				for (int i = 0; i < folderScope.Length; i++)
				{
					if (folderScope[i] == null)
					{
						throw new ArgumentException("folderScope cannot contain null values.");
					}
				}
			}
		}

		private SearchState searchState;

		private QueryFilter searchQuery;

		private StoreId[] folderScope;

		private int? maximumResultsCount = null;
	}
}
