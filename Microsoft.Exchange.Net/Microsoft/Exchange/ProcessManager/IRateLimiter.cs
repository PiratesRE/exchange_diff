using System;

namespace Microsoft.Exchange.ProcessManager
{
	public interface IRateLimiter<TKey>
	{
		bool TryFetchToken(TKey key, int rateLimitPerMinute);

		void ReleaseUnusedToken(TKey key);

		void CleanupIdleEntries(DateTime currentTime);
	}
}
