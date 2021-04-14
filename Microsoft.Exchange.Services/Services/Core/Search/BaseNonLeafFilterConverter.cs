using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.Search
{
	internal abstract class BaseNonLeafFilterConverter : BaseSingleFilterConverter
	{
		protected abstract SearchExpressionType CreateSearchExpression();

		protected abstract bool IsAcceptableChildCount(int childCount);

		protected abstract QueryFilter CreateNonLeafFilter(QueryFilter[] childFilters);

		internal override bool IsLeafFilter
		{
			get
			{
				return false;
			}
		}

		internal abstract int GetQueryFilterChildCount(QueryFilter queryFilter);

		internal abstract QueryFilter GetQueryFilterChild(QueryFilter queryFilter, int childIndex);

		internal QueryFilter ConvertToQueryFilter(Stack<QueryFilter> workingStack)
		{
			int num = 0;
			QueryFilter[] array = new QueryFilter[workingStack.Count];
			while (workingStack.Count > 0)
			{
				array[num++] = workingStack.Pop();
			}
			return this.CreateNonLeafFilter(array);
		}

		internal SearchExpressionType ConvertToSearchExpresson(Stack<SearchExpressionType> workingStack)
		{
			if (!this.IsAcceptableChildCount(workingStack.Count))
			{
				ExTraceGlobals.SearchTracer.TraceError<int>((long)this.GetHashCode(), "[BaseNonLeafFilterConverter::ConvertToSearchExpresson] Incorrect child filter count: {0}", workingStack.Count);
				throw new InvalidRestrictionException(CoreResources.IDs.ErrorInvalidRestriction);
			}
			SearchExpressionType searchExpressionType = this.CreateSearchExpression();
			List<SearchExpressionType> list = new List<SearchExpressionType>();
			while (workingStack.Count > 0)
			{
				SearchExpressionType item = workingStack.Pop();
				list.Add(item);
			}
			INonLeafSearchExpressionType nonLeafSearchExpressionType = searchExpressionType as INonLeafSearchExpressionType;
			if (nonLeafSearchExpressionType != null)
			{
				nonLeafSearchExpressionType.Items = list.ToArray();
			}
			return searchExpressionType;
		}
	}
}
