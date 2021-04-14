using System;
using System.Web;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class ProxySuggesterSidCache : BaseWebCache<SuggesterSidCompositeKey, string>
	{
		internal ProxySuggesterSidCache() : base("_PSCC_", SlidingOrAbsoluteTimeout.Absolute, ProxySuggesterSidCache.TimeoutInMinutes)
		{
		}

		public static ProxySuggesterSidCache Singleton
		{
			get
			{
				return ProxySuggesterSidCache.singleton;
			}
		}

		public static int TimeoutInMinutes
		{
			get
			{
				return 5;
			}
		}

		public void Remove(SuggesterSidCompositeKey key)
		{
			HttpRuntime.Cache.Remove(this.BuildKey(key));
		}

		private const string ProxySuggesterSidKeyPrefix = "_PSCC_";

		private static ProxySuggesterSidCache singleton = new ProxySuggesterSidCache();
	}
}
