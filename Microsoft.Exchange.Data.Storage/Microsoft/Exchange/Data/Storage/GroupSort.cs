using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class GroupSort : SortBy
	{
		public GroupSort(PropertyDefinition columnDefinition, SortOrder sortOrder, Aggregate aggregate) : base(columnDefinition, sortOrder)
		{
			EnumValidator.ThrowIfInvalid<Aggregate>(aggregate, "aggregate");
			this.aggregate = aggregate;
		}

		public Aggregate Aggregate
		{
			get
			{
				return this.aggregate;
			}
		}

		private readonly Aggregate aggregate;
	}
}
