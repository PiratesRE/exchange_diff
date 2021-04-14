using System;

namespace Microsoft.Exchange.Collections.TimeoutCache
{
	internal delegate bool ShouldRemoveDelegate<K, T>(K key, T value);
}
