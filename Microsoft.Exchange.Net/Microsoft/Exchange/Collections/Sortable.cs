using System;
using System.Diagnostics;

namespace Microsoft.Exchange.Collections
{
	[DebuggerDisplay("{Value}")]
	internal struct Sortable<T> : ISortKey<T> where T : IComparable<T>
	{
		public Sortable(T value)
		{
			this.Value = value;
		}

		public T SortKey
		{
			get
			{
				return this.Value;
			}
		}

		public T Value;
	}
}
