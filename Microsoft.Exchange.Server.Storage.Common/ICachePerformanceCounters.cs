using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public interface ICachePerformanceCounters
	{
		ExPerformanceCounter CacheSize { get; }

		ExPerformanceCounter CacheLookups { get; }

		ExPerformanceCounter CacheMisses { get; }

		ExPerformanceCounter CacheHits { get; }

		ExPerformanceCounter CacheInserts { get; }

		ExPerformanceCounter CacheRemoves { get; }

		ExPerformanceCounter CacheExpirationQueueLength { get; }
	}
}
