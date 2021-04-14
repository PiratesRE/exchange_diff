using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Collections
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MruDictionaryInternalKey<T> : IComparer<MruDictionaryInternalKey<T>>, IComparable<MruDictionaryInternalKey<T>>
	{
		public MruDictionaryInternalKey(T originalKey, IComparer<T> originalKeyComparer, DateTime lastAccessedTime)
		{
			this.LastAccessedTime = lastAccessedTime;
			this.OriginalKey = originalKey;
			this.OriginalKeyComparer = originalKeyComparer;
		}

		public int Compare(MruDictionaryInternalKey<T> x, MruDictionaryInternalKey<T> y)
		{
			int num = Comparer<DateTime>.Default.Compare(x.LastAccessedTime, y.LastAccessedTime);
			if (num == 0)
			{
				num = this.OriginalKeyComparer.Compare(x.OriginalKey, y.OriginalKey);
			}
			return num;
		}

		public int CompareTo(MruDictionaryInternalKey<T> other)
		{
			return this.Compare(this, other);
		}

		public readonly T OriginalKey;

		public readonly IComparer<T> OriginalKeyComparer;

		public readonly DateTime LastAccessedTime;
	}
}
