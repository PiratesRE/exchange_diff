using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Collections
{
	internal class MruEqualityComparer<TKey> : IEqualityComparer<TKey>
	{
		public MruEqualityComparer(IComparer<TKey> comparer)
		{
			this.comparer = comparer;
		}

		public bool Equals(TKey x, TKey y)
		{
			return this.comparer.Compare(x, y) == 0;
		}

		public int GetHashCode(TKey obj)
		{
			return obj.GetHashCode();
		}

		private IComparer<TKey> comparer;
	}
}
