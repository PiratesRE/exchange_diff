using System;

namespace Microsoft.Exchange.ProcessManager
{
	internal class TokenRateLimiter
	{
		public bool TryFetchToken(int ratePerMinute)
		{
			if (++this.tokens <= ratePerMinute)
			{
				return true;
			}
			DateTime utcNow = DateTime.UtcNow;
			if (this.IsIdle(utcNow))
			{
				this.lastTokensReset = utcNow;
				this.tokens = 1;
				return true;
			}
			return false;
		}

		public void ReleaseUnusedToken()
		{
			if (this.tokens > 0)
			{
				this.tokens--;
			}
		}

		public bool IsIdle(DateTime currentTime)
		{
			return currentTime.Ticks >= this.lastTokensReset.Ticks + 600000000L;
		}

		private void SetLastTokensResetTime(DateTime time)
		{
			this.lastTokensReset = time;
		}

		private int tokens;

		private DateTime lastTokensReset = DateTime.UtcNow;
	}
}
