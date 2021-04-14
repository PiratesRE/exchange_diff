using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Net
{
	public static class DictionaryExtensions
	{
		public static Dictionary<TKey, TValue> ShallowCopy<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
		{
			Dictionary<TKey, TValue> dictionary2 = new Dictionary<TKey, TValue>(dictionary.Count, dictionary.Comparer);
			foreach (KeyValuePair<TKey, TValue> keyValuePair in dictionary)
			{
				dictionary2.Add(keyValuePair.Key, keyValuePair.Value);
			}
			return dictionary2;
		}
	}
}
