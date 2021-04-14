using System;

namespace Microsoft.Exchange.Security.OAuth
{
	internal sealed class ACSTokenLifeTime
	{
		public static ACSTokenLifeTime Instance
		{
			get
			{
				return ACSTokenLifeTime.instance;
			}
		}

		public void SetValue(TimeSpan lifetime)
		{
			if (!this.initialized)
			{
				lock (this.lockObj)
				{
					if (!this.initialized)
					{
						this.acsTokenCacheSlidingExpiration = TimeSpan.FromMinutes(lifetime.TotalMinutes / 3.0);
						this.remaingLifetimeLimitToRefreshACSToken = TimeSpan.FromMinutes(lifetime.TotalMinutes * 2.0 / 3.0);
						this.initialized = true;
					}
				}
			}
		}

		public TimeSpan RemaingLifetimeLimitToRefreshACSToken
		{
			get
			{
				if (!this.initialized)
				{
					return ACSTokenLifeTime.defaultRemaingLifetimeLimitToRefreshACSToken;
				}
				return this.remaingLifetimeLimitToRefreshACSToken;
			}
		}

		public TimeSpan ACSTokenCacheSlidingExpiration
		{
			get
			{
				if (!this.initialized)
				{
					return ACSTokenLifeTime.defaultACSTokenCacheSlidingExpiration;
				}
				return this.acsTokenCacheSlidingExpiration;
			}
		}

		private static readonly TimeSpan defaultRemaingLifetimeLimitToRefreshACSToken = TimeSpan.FromHours(16.0);

		private static readonly TimeSpan defaultACSTokenCacheSlidingExpiration = TimeSpan.FromHours(8.0);

		private static readonly ACSTokenLifeTime instance = new ACSTokenLifeTime();

		private object lockObj = new object();

		private bool initialized;

		private TimeSpan remaingLifetimeLimitToRefreshACSToken;

		private TimeSpan acsTokenCacheSlidingExpiration;
	}
}
