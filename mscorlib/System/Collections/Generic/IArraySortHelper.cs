using System;

namespace System.Collections.Generic
{
	internal interface IArraySortHelper<TKey>
	{
		void Sort(TKey[] keys, int index, int length, IComparer<TKey> comparer);

		int BinarySearch(TKey[] keys, int index, int length, TKey value, IComparer<TKey> comparer);
	}
}
