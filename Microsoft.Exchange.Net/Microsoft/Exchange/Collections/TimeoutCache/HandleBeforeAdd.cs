using System;

namespace Microsoft.Exchange.Collections.TimeoutCache
{
	internal delegate bool HandleBeforeAdd<K, T>(K key, T value, TimeoutCacheBucket<K, T> bucket);
}
