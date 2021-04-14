using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Configuration.DelegatedAuthentication
{
	internal class DelegatedPrincipalCache
	{
		static DelegatedPrincipalCache()
		{
			DelegatedPrincipalCache.delegatedCache = new Dictionary<DelegatedPrincipalCache.CacheKey, DelegatedPrincipalCacheData>();
		}

		internal static bool TrySetEntry(string targetOrgId, string userId, string securityToken, DelegatedPrincipalCacheData data)
		{
			DelegatedPrincipalCache.CacheKey key = new DelegatedPrincipalCache.CacheKey(targetOrgId, userId, securityToken);
			bool result = false;
			lock (DelegatedPrincipalCache.syncObj)
			{
				if (DelegatedPrincipalCache.delegatedCache.Count < 5000)
				{
					DelegatedPrincipalCache.delegatedCache[key] = data;
					result = true;
				}
			}
			return result;
		}

		internal static DelegatedPrincipalCacheData GetEntry(string targetOrgId, string userId, string securityToken)
		{
			DelegatedPrincipalCache.CacheKey key = new DelegatedPrincipalCache.CacheKey(targetOrgId, userId, securityToken);
			DelegatedPrincipalCacheData delegatedPrincipalCacheData = null;
			if (DelegatedPrincipalCache.delegatedCache.ContainsKey(key))
			{
				lock (DelegatedPrincipalCache.syncObj)
				{
					if (DelegatedPrincipalCache.delegatedCache.TryGetValue(key, out delegatedPrincipalCacheData))
					{
						if (delegatedPrincipalCacheData != null && DelegatedPrincipalCache.IsCacheBucketExpired(delegatedPrincipalCacheData))
						{
							DelegatedPrincipalCache.delegatedCache.Remove(key);
							delegatedPrincipalCacheData = null;
						}
						else
						{
							delegatedPrincipalCacheData.LastReadTime = DateTime.UtcNow;
						}
					}
				}
			}
			return delegatedPrincipalCacheData;
		}

		internal static void Cleanup()
		{
			if (DelegatedPrincipalCache.cleanupTime > DateTime.UtcNow)
			{
				return;
			}
			lock (DelegatedPrincipalCache.syncObj)
			{
				if (!(DelegatedPrincipalCache.cleanupTime > DateTime.UtcNow))
				{
					List<DelegatedPrincipalCache.CacheKey> list = new List<DelegatedPrincipalCache.CacheKey>();
					foreach (DelegatedPrincipalCache.CacheKey cacheKey in DelegatedPrincipalCache.delegatedCache.Keys)
					{
						DelegatedPrincipalCacheData data = DelegatedPrincipalCache.delegatedCache[cacheKey];
						if (DelegatedPrincipalCache.IsCacheBucketExpired(data))
						{
							list.Add(cacheKey);
						}
					}
					foreach (DelegatedPrincipalCache.CacheKey key in list)
					{
						DelegatedPrincipalCache.delegatedCache.Remove(key);
					}
					DelegatedPrincipalCache.cleanupTime = DateTime.UtcNow.AddHours(6.0);
				}
			}
		}

		internal static DateTime NextScheduleCacheCleanUp()
		{
			return DelegatedPrincipalCache.cleanupTime;
		}

		internal static bool RemoveEntry(string targetOrgId, string userId, string securityToken)
		{
			DelegatedPrincipalCache.CacheKey key = new DelegatedPrincipalCache.CacheKey(targetOrgId, userId, securityToken);
			bool result;
			lock (DelegatedPrincipalCache.syncObj)
			{
				result = DelegatedPrincipalCache.delegatedCache.Remove(key);
			}
			return result;
		}

		private static bool IsCacheBucketExpired(DelegatedPrincipalCacheData data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			return data.IsExpired() && data.LastReadTime.AddMinutes(6.0) < DateTime.UtcNow;
		}

		private const int ReadExpirationTimeWindowInMinutes = 6;

		private const int CacheCleanupTimeInHours = 6;

		private const int MaximumCacheSize = 5000;

		private static Dictionary<DelegatedPrincipalCache.CacheKey, DelegatedPrincipalCacheData> delegatedCache;

		private static object syncObj = new object();

		private static DateTime cleanupTime = DateTime.UtcNow.AddHours(6.0);

		private class CacheKey : IEquatable<DelegatedPrincipalCache.CacheKey>
		{
			internal CacheKey(string targetOrgId, string userId) : this(targetOrgId, userId, null)
			{
			}

			internal CacheKey(string targetOrgId, string userId, string securityToken)
			{
				if (string.IsNullOrEmpty(targetOrgId))
				{
					throw new ArgumentNullException("targetOrgId");
				}
				if (string.IsNullOrEmpty(userId))
				{
					throw new ArgumentNullException("userId");
				}
				this.targetOrgId = targetOrgId;
				this.userId = userId;
				this.securityToken = securityToken;
			}

			public override bool Equals(object obj)
			{
				return this.Equals(obj as DelegatedPrincipalCache.CacheKey);
			}

			public override int GetHashCode()
			{
				return this.userId.GetHashCode();
			}

			public bool Equals(DelegatedPrincipalCache.CacheKey other)
			{
				return other != null && (this.targetOrgId.Equals(other.targetOrgId) && this.userId.Equals(other.userId)) && string.Equals(this.securityToken, other.securityToken);
			}

			private string targetOrgId;

			private string userId;

			private string securityToken;
		}
	}
}
