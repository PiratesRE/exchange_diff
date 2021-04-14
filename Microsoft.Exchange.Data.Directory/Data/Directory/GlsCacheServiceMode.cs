using System;

namespace Microsoft.Exchange.Data.Directory
{
	internal enum GlsCacheServiceMode
	{
		CacheDisabled,
		CacheAsExceptionFallback,
		CacheAsNotFoundFallback,
		LiveServiceAsExceptionFallback,
		LiveServiceAsNotFoundCallback,
		CacheOnly
	}
}
