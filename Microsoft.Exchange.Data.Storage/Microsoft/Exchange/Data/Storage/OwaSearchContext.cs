using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Search;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class OwaSearchContext
	{
		public string SearchQueryFilterString { get; set; }

		public QueryFilter SearchQueryFilter { get; set; }

		public SortBy[] SearchSortBy { get; set; }

		public SearchScope SearchScope { get; set; }

		public StoreId FolderIdToSearch { get; set; }

		public StoreId SearchFolderId { get; set; }

		public bool IncludeDeletedItems { get; set; }

		public bool IsFilteredView { get; set; }

		public OwaViewFilter ViewFilter { get; set; }

		public bool IsSearchFailed { get; set; }

		public bool IsSearchInProgress { get; set; }

		public string ClientSearchFolderIdentity { get; set; }

		public bool IsResetCache { get; set; }

		public bool WaitForSearchComplete { get; set; }

		public bool OptimizedSearch { get; set; }

		public ViewFilterActions ViewFilterActions { get; set; }

		public string FromFilter { get; set; }

		public int SearchTimeoutInMilliseconds
		{
			get
			{
				return this.searchTimeoutInMilliseconds;
			}
			set
			{
				this.searchTimeoutInMilliseconds = value;
			}
		}

		public int MaximumTemporaryFilteredViewPerUser
		{
			get
			{
				return this.maximumTemporaryFilteredViewPerUser;
			}
			set
			{
				this.maximumTemporaryFilteredViewPerUser = value;
			}
		}

		public bool IsWarmUpSearch { get; set; }

		public SearchContextType SearchContextType { get; set; }

		public KeyValuePair<string, string>[] HighlightTerms { get; set; }

		public ExTimeZone RequestTimeZone { get; set; }

		private int searchTimeoutInMilliseconds = 60000;

		private int maximumTemporaryFilteredViewPerUser = 20;
	}
}
