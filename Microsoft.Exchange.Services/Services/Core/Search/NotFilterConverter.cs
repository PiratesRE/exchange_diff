using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.Search
{
	internal class NotFilterConverter : BaseNonLeafFilterConverter
	{
		protected override QueryFilter CreateNonLeafFilter(QueryFilter[] childFilters)
		{
			return new NotFilter(childFilters[0]);
		}

		protected override bool IsAcceptableChildCount(int childCount)
		{
			return childCount == 1;
		}

		protected override SearchExpressionType CreateSearchExpression()
		{
			return new NotType();
		}

		internal override int GetQueryFilterChildCount(QueryFilter queryFilter)
		{
			return 1;
		}

		internal override QueryFilter GetQueryFilterChild(QueryFilter queryFilter, int childIndex)
		{
			NotFilter notFilter = queryFilter as NotFilter;
			if (childIndex == 0)
			{
				return notFilter.Filter;
			}
			return null;
		}
	}
}
