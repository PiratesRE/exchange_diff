using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AuditPolicyCache
	{
		public AuditPolicyCache(int maxCount, double entryExpirationInSeconds, double timeForCacheEntryUpdateInSeconds)
		{
			this.EntryExpirationInSeconds = ((entryExpirationInSeconds < 1.0) ? 1.0 : entryExpirationInSeconds);
			this.Cache = new UpdatableCache<OrganizationId, AuditPolicyCacheEntry>(maxCount, timeForCacheEntryUpdateInSeconds);
		}

		public bool GetAuditPolicy(OrganizationId orgId, out AuditPolicyCacheEntry cacheEntry, out bool expired, DateTime? currentUtcTime = null)
		{
			return this.Cache.GetCacheEntry(orgId, out cacheEntry, out expired, currentUtcTime ?? DateTime.UtcNow);
		}

		internal bool UpdateAuditPolicy(OrganizationId orgId, ref AuditPolicyCacheEntry cacheEntry, DateTime? expirationUtc = null)
		{
			return this.Cache.UpdateCacheEntry(orgId, ref cacheEntry, expirationUtc ?? DateTime.UtcNow.AddSeconds(this.EntryExpirationInSeconds));
		}

		private const double CacheEntryExpirationInSeconds = 900.0;

		private const double TimeForCacheEntryUpdateInSeconds = 60.0;

		private readonly UpdatableCache<OrganizationId, AuditPolicyCacheEntry> Cache;

		private readonly double EntryExpirationInSeconds;

		public static readonly AuditPolicyCache Instance = new AuditPolicyCache(10007, 900.0, 60.0);
	}
}
