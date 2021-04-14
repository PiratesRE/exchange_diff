using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.Search
{
	internal class OrFilterConverter : BaseNonLeafFilterConverter
	{
		protected override QueryFilter CreateNonLeafFilter(QueryFilter[] childFilters)
		{
			if (!this.IsAcceptableChildCount(childFilters.Length))
			{
				ExTraceGlobals.SearchTracer.TraceError<int>((long)this.GetHashCode(), "[OrFilterConverter::CreateNonLeafFilter] Expected one or more child filters but found {0}", childFilters.Length);
				throw new InvalidRestrictionException(CoreResources.IDs.ErrorInvalidRestriction);
			}
			return new OrFilter(childFilters);
		}

		protected override bool IsAcceptableChildCount(int childCount)
		{
			return childCount > 0;
		}

		protected override SearchExpressionType CreateSearchExpression()
		{
			return new OrType();
		}

		internal override int GetQueryFilterChildCount(QueryFilter queryFilter)
		{
			OrFilter orFilter = queryFilter as OrFilter;
			return orFilter.FilterCount;
		}

		internal override QueryFilter GetQueryFilterChild(QueryFilter queryFilter, int childIndex)
		{
			OrFilter orFilter = queryFilter as OrFilter;
			if (childIndex < orFilter.FilterCount)
			{
				return orFilter.Filters[childIndex];
			}
			return null;
		}
	}
}
