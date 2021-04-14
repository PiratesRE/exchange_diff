using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal struct GroupByAndOrder
	{
		public GroupByAndOrder(PropertyDefinition groupByColumn, GroupSort groupSortColumn)
		{
			this.GroupByColumn = groupByColumn;
			this.GroupSortColumn = groupSortColumn;
		}

		public readonly PropertyDefinition GroupByColumn;

		public readonly GroupSort GroupSortColumn;
	}
}
