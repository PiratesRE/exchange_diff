using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Exchange.Extensions
{
	public static class CollectionExtensions
	{
		public static D AddPair<D, K, V>(this D dictionary, K key, V value) where D : ICollection<KeyValuePair<K, V>>
		{
			dictionary.Add(new KeyValuePair<K, V>(key, value));
			return dictionary;
		}

		public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> elements)
		{
			if (collection == null)
			{
				throw new ArgumentNullException("collection");
			}
			if (elements == null)
			{
				throw new ArgumentNullException("elements");
			}
			foreach (T item in elements)
			{
				collection.Add(item);
			}
		}

		public static bool IsNullOrEmpty(this ICollection collection)
		{
			return collection == null || 0 == collection.Count;
		}

		public static void ValidateRange(this ICollection collection, int index, int length)
		{
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", index, "Index must be positive.");
			}
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length", length, "Length must be positive.");
			}
			if (index > collection.Count)
			{
				throw new ArgumentOutOfRangeException("index", index, "Index must be in the range of valid indexes for the collection.");
			}
			if (length > collection.Count - index)
			{
				throw new ArgumentOutOfRangeException("length", length, "Index and length must refer to locations within the collection.");
			}
		}

		internal static bool Equals<TKey, TValue>(this IDictionary<TKey, TValue> dictionary1, IDictionary<TKey, TValue> dictionary2)
		{
			if (dictionary1 == null)
			{
				return dictionary2 == null;
			}
			if (dictionary2 == null)
			{
				return false;
			}
			if (dictionary2.Count != dictionary1.Count)
			{
				return false;
			}
			foreach (KeyValuePair<TKey, TValue> keyValuePair in dictionary1)
			{
				TValue tvalue;
				if (!dictionary2.TryGetValue(keyValuePair.Key, out tvalue) || !tvalue.Equals(keyValuePair.Value))
				{
					return false;
				}
			}
			return true;
		}

		internal static bool Equals<T>(this List<T> left, List<T> right, IEqualityComparer<T> comparer)
		{
			if (comparer == null)
			{
				throw new ArgumentNullException("comparer");
			}
			if (left == null)
			{
				return null == right;
			}
			if (right == null)
			{
				return null == left;
			}
			if (left.Count != right.Count)
			{
				return false;
			}
			for (int i = 0; i < left.Count; i++)
			{
				if (!comparer.Equals(left[i], right[i]))
				{
					return false;
				}
			}
			return true;
		}
	}
}
