using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.ProcessManager
{
	internal sealed class RateLimiter<TKey> : IRateLimiter<TKey>
	{
		public bool TryFetchToken(TKey key, int rateLimitPerMinute)
		{
			bool result;
			lock (this.syncRoot)
			{
				TokenRateLimiter tokenRateLimiter = this.GetTokenRateLimiter(key);
				result = tokenRateLimiter.TryFetchToken(rateLimitPerMinute);
			}
			return result;
		}

		public void ReleaseUnusedToken(TKey key)
		{
			lock (this.syncRoot)
			{
				TokenRateLimiter tokenRateLimiter = this.GetTokenRateLimiter(key);
				tokenRateLimiter.ReleaseUnusedToken();
			}
		}

		public void CleanupIdleEntries(DateTime currentTime)
		{
			List<TKey> list = null;
			lock (this.syncRoot)
			{
				foreach (KeyValuePair<TKey, TokenRateLimiter> keyValuePair in this.keyToTokenRateLimiterMap)
				{
					if (keyValuePair.Value.IsIdle(currentTime))
					{
						if (list == null)
						{
							list = new List<TKey>(10);
						}
						list.Add(keyValuePair.Key);
					}
				}
				if (list != null)
				{
					foreach (TKey key in list)
					{
						this.keyToTokenRateLimiterMap.Remove(key);
					}
				}
			}
		}

		private TokenRateLimiter GetTokenRateLimiter(TKey key)
		{
			TokenRateLimiter tokenRateLimiter;
			if (!this.keyToTokenRateLimiterMap.TryGetValue(key, out tokenRateLimiter))
			{
				tokenRateLimiter = new TokenRateLimiter();
				this.keyToTokenRateLimiterMap.Add(key, tokenRateLimiter);
			}
			return tokenRateLimiter;
		}

		private const int KeyToTokenRateLimiterMapInitialCapacity = 10;

		private Dictionary<TKey, TokenRateLimiter> keyToTokenRateLimiterMap = new Dictionary<TKey, TokenRateLimiter>(10);

		private object syncRoot = new object();
	}
}
