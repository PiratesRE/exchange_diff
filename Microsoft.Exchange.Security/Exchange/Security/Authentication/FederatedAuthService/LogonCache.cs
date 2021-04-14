using System;
using System.Collections.Generic;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Diagnostics.Components.Security;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Security.Authentication.FederatedAuthService
{
	internal class LogonCache
	{
		public LogonCache(int validCacheSize, int validLifetime, int level1CacheSize, int level1CacheLifetime, int level2CacheSize, int level2CacheLifetime, int level2CacheListSize)
		{
			this.validLogons = new MruDictionaryCache<string, PuidCredInfo>(validCacheSize, validLifetime);
			this.failedLogons = new MruDictionaryCache<string, HashedCredInfo>(level1CacheSize, level1CacheLifetime);
			this.repeatedFailedLogons = new MruDictionaryCache<string, List<HashedCredInfo>>(level2CacheSize, level2CacheLifetime);
			this.level2Passwords = level2CacheListSize;
		}

		public int ValidCredsCount
		{
			get
			{
				return this.validLogons.Count;
			}
		}

		public int InvalidCredsCount
		{
			get
			{
				return this.failedLogons.Count + this.repeatedFailedLogons.Count;
			}
		}

		public bool TryGetEntry(string key, byte[] hash, out PuidCredInfo entry)
		{
			entry = null;
			PuidCredInfo puidCredInfo;
			if (this.validLogons.TryGetValue(key, out puidCredInfo) && puidCredInfo.Time.AddMinutes((double)puidCredInfo.LifeTimeInMinutes).CompareTo(ExDateTime.UtcNow) > 0 && HashExtension.CompareHash(puidCredInfo.Hash, hash))
			{
				entry = puidCredInfo;
				return true;
			}
			return false;
		}

		public void Add(string key, ExDateTime time, string puid, byte[] hash, int lifeTimeInMinutes, string tag, int traceId, string passwordExpiry, UserType userType, bool appPassword, bool requestPasswordConfidenceCheckInBackend = false)
		{
			ExTraceGlobals.AuthenticationTracer.TraceDebug((long)traceId, "LogonCache.Add key='{0}' puid='{1}' time='{2}' tag='{3}'", new object[]
			{
				key,
				puid,
				time,
				tag
			});
			this.RemoveBadPassword(key, hash);
			this.validLogons.Add(key, new PuidCredInfo(time, puid, hash, tag, lifeTimeInMinutes, passwordExpiry, userType, appPassword, requestPasswordConfidenceCheckInBackend));
		}

		public void Add(string key, ExDateTime time, string puid, byte[] hash, int lifeTimeInMinutes, string tag, int traceId, UserType userType, bool requestPasswordConfidenceCheckInBackend = false)
		{
			ExTraceGlobals.AuthenticationTracer.TraceDebug((long)traceId, "LogonCache.Add key='{0}' puid='{1}' time='{2}' tag='{3}'", new object[]
			{
				key,
				puid,
				time,
				tag
			});
			this.RemoveBadPassword(key, hash);
			this.validLogons.Add(key, new PuidCredInfo(time, puid, hash, tag, lifeTimeInMinutes, null, userType, false, requestPasswordConfidenceCheckInBackend));
		}

		public void Remove(string key)
		{
			this.validLogons.Remove(key);
		}

		public bool Add(string key, ExDateTime time, byte[] hash, CredFailure mode, string tag, UserType userType, int traceId)
		{
			ExTraceGlobals.AuthenticationTracer.TraceDebug((long)traceId, "LogonCache.Add key='{0}' hash='{1}' mode='{2}' time='{3}' tag='{4}'", new object[]
			{
				key,
				BitConverter.ToString(hash).Replace("-", ""),
				mode,
				time,
				tag
			});
			bool result;
			lock (this)
			{
				PuidCredInfo puidCredInfo;
				if (this.TryGetEntry(key, hash, out puidCredInfo))
				{
					this.Remove(key);
				}
				List<HashedCredInfo> list;
				HashedCredInfo hashedCredInfo;
				if (this.repeatedFailedLogons.TryGetValue(key, out list))
				{
					ExTraceGlobals.AuthenticationTracer.TraceDebug((long)traceId, "key entry exists in level 2 failed logon cache");
					hashedCredInfo = new HashedCredInfo(hash, time, mode, tag, userType);
					this.AddToList(list, hashedCredInfo);
					result = true;
				}
				else if (this.failedLogons.TryGetValue(key, out hashedCredInfo))
				{
					ExTraceGlobals.AuthenticationTracer.TraceDebug((long)traceId, "key entry exists in level 1 failed logon cache, promoting to level 2");
					hashedCredInfo.Mode = mode;
					hashedCredInfo.Tag = tag;
					hashedCredInfo.UserType = userType;
					list = new List<HashedCredInfo>(1);
					this.AddToList(list, hashedCredInfo);
					this.repeatedFailedLogons.Add(key, list);
					this.failedLogons.Remove(key);
					result = true;
				}
				else
				{
					ExTraceGlobals.AuthenticationTracer.TraceDebug((long)traceId, "entry doesn't exist, adding to level 1 failed logon cache");
					hashedCredInfo = new HashedCredInfo(hash, time, mode, tag, userType);
					this.failedLogons.Add(key, hashedCredInfo);
					result = false;
				}
			}
			return result;
		}

		public bool CheckBadPassword(string key, byte[] hash, out HashedCredInfo entry, out bool repeated)
		{
			LogonCacheConfig config = LogonCacheConfig.Config;
			ExDateTime now = ExDateTime.UtcNow;
			entry = null;
			repeated = false;
			HashedCredInfo hashedCredInfo;
			if (this.failedLogons.TryGetValue(key, out hashedCredInfo) && !hashedCredInfo.IsExpired(config, now) && HashExtension.CompareHash(hash, hashedCredInfo.Hash))
			{
				entry = hashedCredInfo;
				return true;
			}
			List<HashedCredInfo> list;
			if (this.repeatedFailedLogons.TryGetValue(key, out list))
			{
				int num = list.FindIndex((HashedCredInfo x) => !x.IsExpired(config, now) && HashExtension.CompareHash(x.Hash, hash));
				if (num >= 0)
				{
					entry = list[num];
					repeated = true;
					return true;
				}
			}
			return false;
		}

		public void RemoveBadPassword(string key, byte[] hash)
		{
			lock (this)
			{
				HashedCredInfo hashedCredInfo;
				if (this.failedLogons.TryGetValue(key, out hashedCredInfo) && HashExtension.CompareHash(hash, hashedCredInfo.Hash))
				{
					this.failedLogons.Remove(key);
				}
				List<HashedCredInfo> list;
				if (this.repeatedFailedLogons.TryGetValue(key, out list))
				{
					int num = list.FindIndex((HashedCredInfo x) => HashExtension.CompareHash(x.Hash, hash));
					if (num >= 0)
					{
						list.RemoveAt(num);
					}
				}
			}
		}

		private void AddToList(List<HashedCredInfo> list, HashedCredInfo entry)
		{
			ExDateTime utcNow = ExDateTime.UtcNow;
			for (int i = list.Count - 1; i >= 0; i--)
			{
				if (list[i].IsExpired(LogonCacheConfig.Config, utcNow))
				{
					list.RemoveAt(i);
				}
			}
			int num = list.FindIndex((HashedCredInfo x) => HashExtension.CompareHash(x.Hash, entry.Hash));
			if (num >= 0)
			{
				list[num].Mode = entry.Mode;
				list[num].Tag = entry.Tag;
				return;
			}
			if (list.Count >= this.level2Passwords)
			{
				list.RemoveAt(0);
			}
			list.Add(entry);
		}

		private MruDictionaryCache<string, PuidCredInfo> validLogons;

		private MruDictionaryCache<string, HashedCredInfo> failedLogons;

		private MruDictionaryCache<string, List<HashedCredInfo>> repeatedFailedLogons;

		private readonly int level2Passwords = 5;
	}
}
