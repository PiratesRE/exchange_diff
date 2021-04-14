using System;

namespace Microsoft.Exchange.Common.Cache
{
	internal interface ICachePerformanceCounters
	{
		void Accessed(AccessStatus accessStatus);

		void SizeUpdated(long cacheSize);
	}
}
