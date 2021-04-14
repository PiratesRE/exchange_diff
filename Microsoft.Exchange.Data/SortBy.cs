using System;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data
{
	public class SortBy
	{
		public SortBy(PropertyDefinition columnDefinition, SortOrder sortOrder)
		{
			if (columnDefinition == null)
			{
				throw new ArgumentNullException("columnDefinition");
			}
			EnumValidator.ThrowIfInvalid<SortOrder>(sortOrder, "sortOrder");
			this.columnDefinition = columnDefinition;
			this.sortOrder = sortOrder;
		}

		internal PropertyDefinition ColumnDefinition
		{
			get
			{
				return this.columnDefinition;
			}
		}

		internal SortOrder SortOrder
		{
			get
			{
				return this.sortOrder;
			}
		}

		private readonly PropertyDefinition columnDefinition;

		private readonly SortOrder sortOrder;
	}
}
