using System;

namespace Microsoft.Exchange.Collections
{
	public interface ISortKey<TKey> where TKey : IComparable<TKey>
	{
		TKey SortKey { get; }
	}
}
