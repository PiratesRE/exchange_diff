using System;

namespace Microsoft.Exchange.Common.Cache
{
	internal interface ISimpleCache<K, V>
	{
		bool TryGetValue(K key, out V value);

		bool TryAddValue(K key, V value);
	}
}
