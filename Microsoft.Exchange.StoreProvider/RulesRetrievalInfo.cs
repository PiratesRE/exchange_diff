using System;

namespace Microsoft.Mapi
{
	internal enum RulesRetrievalInfo
	{
		None,
		CacheHit,
		CacheMiss,
		CacheCorruption,
		CacheNotSupport
	}
}
