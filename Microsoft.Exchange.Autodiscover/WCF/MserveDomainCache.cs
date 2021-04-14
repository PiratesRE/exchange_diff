using System;
using System.Threading;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Autodiscover;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MserveDomainCache : LazyLookupTimeoutCache<string, string>
	{
		private MserveDomainCache() : base(1, MserveDomainCache.cacheSize.Value, false, MserveDomainCache.cacheTimeToLive.Value)
		{
		}

		public static MserveDomainCache Singleton
		{
			get
			{
				return MserveDomainCache.singleton;
			}
		}

		protected override string CreateOnCacheMiss(string key, ref bool shouldAdd)
		{
			int num = Interlocked.Increment(ref MserveDomainCache.concurrentMserveLookups);
			string redirectServer;
			try
			{
				if (num > MserveDomainCache.concurrentLookupMaximum.Value)
				{
					throw new OverBudgetException();
				}
				redirectServer = MServe.GetRedirectServer(string.Format("E5CB63F56E8B4b69A1F70C192276D6AD@{0}", key));
			}
			finally
			{
				Interlocked.Decrement(ref MserveDomainCache.concurrentMserveLookups);
			}
			return redirectServer;
		}

		private static int concurrentMserveLookups = 0;

		private static TimeSpanAppSettingsEntry cacheTimeToLive = new TimeSpanAppSettingsEntry("MserveDomainCacheTimeToLive", TimeSpanUnit.Seconds, TimeSpan.FromMinutes(30.0), ExTraceGlobals.FrameworkTracer);

		private static IntAppSettingsEntry cacheSize = new IntAppSettingsEntry("MserveDomainCacheMaxItems", 4000, ExTraceGlobals.FrameworkTracer);

		private static IntAppSettingsEntry concurrentLookupMaximum = new IntAppSettingsEntry("MserveDomainDomainConcurrentLookupMax", 10, ExTraceGlobals.FrameworkTracer);

		private static MserveDomainCache singleton = new MserveDomainCache();
	}
}
