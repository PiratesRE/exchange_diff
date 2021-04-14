using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal static class DictionaryExtensions
	{
		public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
		{
			return dictionary.GetOrAdd(key, () => value);
		}

		public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> objectBuilder)
		{
			TValue tvalue;
			if (!dictionary.TryGetValue(key, out tvalue))
			{
				tvalue = objectBuilder();
				dictionary[key] = tvalue;
			}
			return tvalue;
		}
	}
}
