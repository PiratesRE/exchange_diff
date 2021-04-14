using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	internal class PaginationQueryFilter
	{
		internal PaginationQueryFilter(PagingInfo pagingInfo)
		{
			Util.ThrowOnNull(pagingInfo, "pagingInfo");
			this.BuildPaginationQueryFilters(pagingInfo);
		}

		private void BuildPaginationQueryFilters(PagingInfo pagingInfo)
		{
			if (pagingInfo.SortValue == null || pagingInfo.SortValue.SortColumnValue == null)
			{
				return;
			}
			this.equalsQueryFilter = new ComparisonFilter(ComparisonOperator.Equal, pagingInfo.SortValue.SortColumn, pagingInfo.SortValue.SortColumnValue);
			if (pagingInfo.Direction != PageDirection.Next)
			{
				if (pagingInfo.Direction == PageDirection.Previous)
				{
					if (pagingInfo.SortBy.SortOrder == SortOrder.Ascending)
					{
						this.comparisionQueryFilter = new ComparisonFilter(ComparisonOperator.LessThan, pagingInfo.SortValue.SortColumn, pagingInfo.SortValue.SortColumnValue);
						return;
					}
					this.comparisionQueryFilter = new ComparisonFilter(ComparisonOperator.GreaterThan, pagingInfo.SortValue.SortColumn, pagingInfo.SortValue.SortColumnValue);
				}
				return;
			}
			if (pagingInfo.SortBy.SortOrder == SortOrder.Ascending)
			{
				this.comparisionQueryFilter = new ComparisonFilter(ComparisonOperator.GreaterThan, pagingInfo.SortValue.SortColumn, pagingInfo.SortValue.SortColumnValue);
				return;
			}
			this.comparisionQueryFilter = new ComparisonFilter(ComparisonOperator.LessThan, pagingInfo.SortValue.SortColumn, pagingInfo.SortValue.SortColumnValue);
		}

		internal QueryFilter EqualsQueryFilter
		{
			get
			{
				return this.equalsQueryFilter;
			}
		}

		internal QueryFilter ComparisionQueryFilter
		{
			get
			{
				return this.comparisionQueryFilter;
			}
		}

		public override bool Equals(object obj)
		{
			PaginationQueryFilter paginationQueryFilter = obj as PaginationQueryFilter;
			return paginationQueryFilter != null && this.equalsQueryFilter == paginationQueryFilter.equalsQueryFilter && this.comparisionQueryFilter == paginationQueryFilter.comparisionQueryFilter;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		private QueryFilter equalsQueryFilter;

		private QueryFilter comparisionQueryFilter;
	}
}
