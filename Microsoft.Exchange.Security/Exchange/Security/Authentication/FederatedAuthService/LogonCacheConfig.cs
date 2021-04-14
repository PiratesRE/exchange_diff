using System;

namespace Microsoft.Exchange.Security.Authentication.FederatedAuthService
{
	internal class LogonCacheConfig
	{
		private LogonCacheConfig(int badCredsLifetime, int badCredsRecoverableLifetime)
		{
			this.badCredsLifetime = badCredsLifetime;
			this.badCredsRecoverableLifetime = badCredsRecoverableLifetime;
		}

		public static LogonCacheConfig Config
		{
			get
			{
				return LogonCacheConfig.config;
			}
			private set
			{
				LogonCacheConfig.config = value;
			}
		}

		public static void Initialize(int badCredsLifetime, int badCredsRecoverableLifetime)
		{
			LogonCacheConfig.config = new LogonCacheConfig(badCredsLifetime, badCredsRecoverableLifetime);
		}

		public readonly int badCredsLifetime;

		public readonly int badCredsRecoverableLifetime;

		private static LogonCacheConfig config;
	}
}
