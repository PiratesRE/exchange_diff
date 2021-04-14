using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class ProxyTokenCache : BaseWebCache<string, SerializedSecurityAccessToken>
	{
		public ProxyTokenCache() : base("_PTC_", SlidingOrAbsoluteTimeout.Absolute, ProxySuggesterSidCache.TimeoutInMinutes + 1)
		{
		}

		public static ProxyTokenCache Singleton
		{
			get
			{
				return ProxyTokenCache.singleton;
			}
		}

		private const string ProxyTokenCachePrefix = "_PTC_";

		private static readonly ProxyTokenCache singleton = new ProxyTokenCache();
	}
}
