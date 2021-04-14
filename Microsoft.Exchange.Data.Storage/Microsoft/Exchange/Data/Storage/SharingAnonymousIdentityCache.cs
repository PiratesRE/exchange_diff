using System;
using System.Security.Principal;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SharingAnonymousIdentityCache : LazyLookupTimeoutCache<SharingAnonymousIdentityCacheKey, SharingAnonymousIdentityCacheValue>
	{
		private SharingAnonymousIdentityCache() : base(SharingAnonymousIdentityCache.SharingAnonymousIdentityCacheBuckets.Value, SharingAnonymousIdentityCache.SharingAnonymousIdentityCacheCacheBucketSize.Value, false, SharingAnonymousIdentityCache.SharingAnonymousIdentityCacheTimeToExpire.Value, SharingAnonymousIdentityCache.SharingAnonymousIdentityCacheTimeToLive.Value)
		{
		}

		protected override SharingAnonymousIdentityCacheValue CreateOnCacheMiss(SharingAnonymousIdentityCacheKey key, ref bool shouldAdd)
		{
			SecurityIdentifier sid = null;
			string folderId = key.Lookup(out sid);
			return new SharingAnonymousIdentityCacheValue(folderId, sid);
		}

		public static SharingAnonymousIdentityCache Instance
		{
			get
			{
				return SharingAnonymousIdentityCache.instance;
			}
		}

		private static readonly IntAppSettingsEntry SharingAnonymousIdentityCacheCacheBucketSize = new IntAppSettingsEntry("SharingAnonymousIdentityCacheCacheBucketSize", 1000, ExTraceGlobals.SharingTracer);

		private static readonly IntAppSettingsEntry SharingAnonymousIdentityCacheBuckets = new IntAppSettingsEntry("SharingAnonymousIdentityCacheBuckets", 5, ExTraceGlobals.SharingTracer);

		private static readonly TimeSpanAppSettingsEntry SharingAnonymousIdentityCacheTimeToExpire = new TimeSpanAppSettingsEntry("SharingAnonymousIdentityCacheTimeToExpire", TimeSpanUnit.Minutes, TimeSpan.FromMinutes(2.0), ExTraceGlobals.SharingTracer);

		private static readonly TimeSpanAppSettingsEntry SharingAnonymousIdentityCacheTimeToLive = new TimeSpanAppSettingsEntry("SharingAnonymousIdentityCacheTimeToLive", TimeSpanUnit.Minutes, TimeSpan.FromMinutes(20.0), ExTraceGlobals.SharingTracer);

		private static readonly SharingAnonymousIdentityCache instance = new SharingAnonymousIdentityCache();
	}
}
