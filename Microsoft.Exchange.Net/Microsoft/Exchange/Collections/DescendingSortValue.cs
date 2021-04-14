using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Collections
{
	public struct DescendingSortValue<T> : ISortKey<DescendingSortValue<T>>, IComparable<DescendingSortValue<T>> where T : IComparable<T>
	{
		public DescendingSortValue(T item)
		{
			this.item = item;
		}

		public T Item
		{
			get
			{
				return this.item;
			}
		}

		public DescendingSortValue<T> SortKey
		{
			get
			{
				return this;
			}
		}

		public int CompareTo(DescendingSortValue<T> otherItem)
		{
			return Comparer<T>.Default.Compare(otherItem.item, this.item);
		}

		private readonly T item;
	}
}
