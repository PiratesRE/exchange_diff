using System;
using System.Diagnostics;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics.Components.Autodiscover;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.PartnerToken;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	internal sealed class MailboxAccessPartnerInfoCache : LazyLookupTimeoutCache<MailboxAccessPartnerInfoCacheKey, PartnerInfo>
	{
		private MailboxAccessPartnerInfoCache() : base(1, MailboxAccessPartnerInfoCache.cacheSize.Value, false, MailboxAccessPartnerInfoCache.cacheTimeToLive.Value)
		{
		}

		public static MailboxAccessPartnerInfoCache Singleton
		{
			get
			{
				MailboxAccessPartnerInfoCache result;
				lock (MailboxAccessPartnerInfoCache.lockObj)
				{
					if (MailboxAccessPartnerInfoCache.singleton == null)
					{
						MailboxAccessPartnerInfoCache.singleton = new MailboxAccessPartnerInfoCache();
					}
					result = MailboxAccessPartnerInfoCache.singleton;
				}
				return result;
			}
		}

		protected override PartnerInfo CreateOnCacheMiss(MailboxAccessPartnerInfoCacheKey key, ref bool shouldAdd)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			try
			{
				for (int i = 0; i < MailboxAccessPartnerInfoCache.lookupRetryMax.Value; i++)
				{
					try
					{
						return PartnerInfo.CreateFromADObjectId(key.ADObjectId, key.OrganizationId);
					}
					catch (ADTransientException arg)
					{
						ExTraceGlobals.FrameworkTracer.TraceDebug<int, ADTransientException>(0L, "[MailboxAccessPartnerInfoCache::CreateOnCacheMiss] PartnerInfo.CreateFromSid will retry for {0} times, for ADTransientException {1}", i, arg);
					}
				}
			}
			finally
			{
				PerformanceCounters.UpdateAveragePartnerInfoQueryTime(stopwatch.ElapsedMilliseconds);
			}
			ExTraceGlobals.FrameworkTracer.TraceWarning(0L, "[MailboxAccessPartnerInfoCache::CreateOnCacheMiss] PartnerInfo.CreateFromSid will return Invalid after several retries");
			return PartnerInfo.Invalid;
		}

		private static readonly TimeSpanAppSettingsEntry cacheTimeToLive = new TimeSpanAppSettingsEntry("PartnerInfoCacheTimeToLive", TimeSpanUnit.Seconds, TimeSpan.FromMinutes(60.0), ExTraceGlobals.FrameworkTracer);

		private static readonly IntAppSettingsEntry cacheSize = new IntAppSettingsEntry("PartnerInfoCacheMaxItems", 1024, ExTraceGlobals.FrameworkTracer);

		private static readonly IntAppSettingsEntry lookupRetryMax = new IntAppSettingsEntry("PartnerInfoCacheLookupRetryMax", 3, ExTraceGlobals.FrameworkTracer);

		private static readonly object lockObj = new object();

		private static MailboxAccessPartnerInfoCache singleton = null;
	}
}
