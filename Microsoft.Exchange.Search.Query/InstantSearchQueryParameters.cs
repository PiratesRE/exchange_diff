using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Search.Query
{
	internal class InstantSearchQueryParameters
	{
		public InstantSearchQueryParameters(string kqlQuery, QueryFilter additionalFilter, QueryOptions queryOptions)
		{
			this.KqlQuery = kqlQuery;
			this.AdditionalFilter = additionalFilter;
			this.QueryOptions = queryOptions;
			if (kqlQuery == null)
			{
				if (additionalFilter == null)
				{
					throw new ArgumentException("Must specify kqlQuery and/or additionalFilter");
				}
				this.RequestType = InstantSearchQueryParameters.QueryType.PureQueryFilter;
			}
			else
			{
				if (additionalFilter == null)
				{
					this.RequestType = InstantSearchQueryParameters.QueryType.PureKql;
				}
				else
				{
					this.RequestType = InstantSearchQueryParameters.QueryType.KqlWithQueryFilter;
				}
				this.EmptyPrewarmingQuery = (kqlQuery == string.Empty);
			}
			this.MaximumRefinersCount = 5;
		}

		public string KqlQuery { get; private set; }

		public QueryFilter AdditionalFilter { get; private set; }

		public RefinementFilter RefinementFilter { get; set; }

		public QueryOptions QueryOptions { get; private set; }

		public QuerySuggestionSources QuerySuggestionSources { get; set; }

		public IReadOnlyCollection<PropertyDefinition> Refiners { get; set; }

		public IReadOnlyCollection<SortBy> SortSpec { get; set; }

		public bool EmptyPrewarmingQuery { get; private set; }

		public bool DeepTraversal { get; set; }

		public int? MaximumResultCount { get; set; }

		public int MaximumRefinersCount { get; set; }

		public IReadOnlyList<StoreId> FolderScope { get; set; }

		internal InstantSearchQueryParameters.QueryType RequestType { get; private set; }

		public bool HasOption(QueryOptions options)
		{
			return (this.QueryOptions & options) != QueryOptions.None;
		}

		internal const int DefaultMaximumRefinersCount = 5;

		internal enum QueryType
		{
			PureKql,
			KqlWithQueryFilter,
			PureQueryFilter
		}
	}
}
