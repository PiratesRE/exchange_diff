using System;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Search.AqsParser;

namespace Microsoft.Exchange.Services.Core.Search
{
	internal class SearchFilterGenerator
	{
		private SearchFilterGenerator(QueryFilter advancedQueryFilter, CultureInfo userCultureInfo)
		{
			this.advancedQueryFilter = advancedQueryFilter;
			this.userCultureInfo = userCultureInfo;
		}

		public static QueryFilter Execute(string searchString, CultureInfo userCultureInfo, QueryFilter advancedQueryFilter)
		{
			SearchFilterGenerator searchFilterGenerator = new SearchFilterGenerator(advancedQueryFilter, userCultureInfo);
			return searchFilterGenerator.Execute(searchString);
		}

		public QueryFilter Execute(string searchString)
		{
			if (searchString != null)
			{
				this.queryFilter = AqsParser.ParseAndBuildQuery(searchString, AqsParser.ParseOption.SuppressError, this.userCultureInfo, RescopedAll.Default, null, null);
			}
			if (this.advancedQueryFilter != null)
			{
				if (this.queryFilter == null)
				{
					this.queryFilter = this.advancedQueryFilter;
				}
				else
				{
					this.queryFilter = new AndFilter(new QueryFilter[]
					{
						this.queryFilter,
						this.advancedQueryFilter
					});
				}
			}
			if (this.queryFilter == null)
			{
				return null;
			}
			return this.queryFilter;
		}

		private QueryFilter advancedQueryFilter;

		private CultureInfo userCultureInfo;

		private QueryFilter queryFilter;
	}
}
