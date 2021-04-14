using System;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage.ResourceHealth
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PingerCache : LazyLookupExactTimeoutCache<Guid, IMdbSystemMailboxPinger>
	{
		public static Func<Guid, IMdbSystemMailboxPinger> CreatePingerTestHook { get; set; }

		private PingerCache() : base(1000000, true, TimeSpan.MaxValue, CacheFullBehavior.ExpireExisting)
		{
		}

		protected override IMdbSystemMailboxPinger CreateOnCacheMiss(Guid key, ref bool shouldAdd)
		{
			shouldAdd = true;
			if (PingerCache.CreatePingerTestHook != null)
			{
				return PingerCache.CreatePingerTestHook(key);
			}
			IDatabaseInformation databaseInformation = DatabaseInformationCache.Singleton.Get(key);
			if (databaseInformation != null)
			{
				ExTraceGlobals.DatabasePingerTracer.TraceDebug<string, Guid>((long)this.GetHashCode(), "[PingerCache.CreateOnCacheMiss] Acquire database information for Mdb: {0}, Guid: {1}", databaseInformation.DatabaseName, databaseInformation.DatabaseGuid);
				return new MdbSystemMailboxPinger(key);
			}
			ExTraceGlobals.DatabasePingerTracer.TraceDebug<Guid>((long)this.GetHashCode(), "[PingerCache.CreateOnCacheMiss] Failed to get database information for Mdb Guid: {0}.  NOT creating pinger - caching null instead.", key);
			return null;
		}

		protected override void CleanupValue(Guid key, IMdbSystemMailboxPinger value)
		{
			IDisposable disposable = value as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}

		public static readonly PingerCache Singleton = new PingerCache();
	}
}
