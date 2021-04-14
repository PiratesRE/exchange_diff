using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.Search
{
	internal class AndFilterConverter : BaseNonLeafFilterConverter
	{
		protected override QueryFilter CreateNonLeafFilter(QueryFilter[] childFilters)
		{
			if (!this.IsAcceptableChildCount(childFilters.Length))
			{
				ExTraceGlobals.SearchTracer.TraceError<int>((long)this.GetHashCode(), "[AndFilterConverter::CreateNonLeafFilter] Expected one or more child filters but found {0}", childFilters.Length);
				throw new InvalidRestrictionException(CoreResources.IDs.ErrorInvalidRestriction);
			}
			return new AndFilter(childFilters);
		}

		protected override bool IsAcceptableChildCount(int childCount)
		{
			return childCount > 0;
		}

		protected override SearchExpressionType CreateSearchExpression()
		{
			return new AndType();
		}

		internal override int GetQueryFilterChildCount(QueryFilter queryFilter)
		{
			AndFilter andFilter = queryFilter as AndFilter;
			return andFilter.FilterCount;
		}

		internal override QueryFilter GetQueryFilterChild(QueryFilter queryFilter, int childIndex)
		{
			AndFilter andFilter = queryFilter as AndFilter;
			if (childIndex < andFilter.FilterCount)
			{
				return andFilter.Filters[childIndex];
			}
			return null;
		}
	}
}
