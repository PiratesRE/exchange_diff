using System;

namespace System.Collections.Generic
{
	internal interface IArraySortHelper<TKey, TValue>
	{
		void Sort(TKey[] keys, TValue[] values, int index, int length, IComparer<TKey> comparer);
	}
}
