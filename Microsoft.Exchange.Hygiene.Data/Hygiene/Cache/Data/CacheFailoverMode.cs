using System;

namespace Microsoft.Exchange.Hygiene.Cache.Data
{
	internal enum CacheFailoverMode
	{
		CacheOnly,
		CacheThenDatabase,
		DatabaseOnly,
		Default,
		DefaultThenDatabase,
		BloomFilter
	}
}
