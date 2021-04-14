using System;
using System.Collections.Generic;
using Microsoft.Exchange.AddressBook.Nspi;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.AddressBook.Service;
using Microsoft.Mapi;

namespace Microsoft.Exchange.AddressBook.Service
{
	internal class ModCache
	{
		internal ModCache(NspiContext nspiContext, int expiryTimeInSeconds)
		{
			this.nspiContext = nspiContext;
			this.logExpiry = TimeSpan.FromSeconds((double)expiryTimeInSeconds);
			this.cache = new Dictionary<ModCache.LogKey, ModCache.Log>();
		}

		internal void RecordMods(int mid, PropTag propTag, IList<int> mids, bool isDeletion)
		{
			ModCache.ModCacheTracer.TraceDebug((long)this.nspiContext.ContextHandle, "ModCache.Log: mid='{0}', propTag={1}, mids.Count={2}, isDeletion={3}", new object[]
			{
				mid,
				propTag,
				mids.Count,
				isDeletion
			});
			ModCache.LogKey key = new ModCache.LogKey(mid, propTag);
			ModCache.Log log;
			if (!this.cache.TryGetValue(key, out log))
			{
				log = new ModCache.Log(mids.Count);
				this.cache.Add(key, log);
			}
			foreach (int key2 in mids)
			{
				log.Mods[key2] = isDeletion;
			}
		}

		internal bool HasMods(int mid, PropTag propTag)
		{
			return this.cache.ContainsKey(new ModCache.LogKey(mid, propTag));
		}

		internal void PurgeMods(int mid, PropTag propTag)
		{
			this.cache.Remove(new ModCache.LogKey(mid, propTag));
		}

		internal QueryFilter ApplyMods(QueryFilter filter, int mid, PropTag propTag)
		{
			ModCache.ModCacheTracer.TraceDebug<int, PropTag>((long)this.nspiContext.ContextHandle, "ModCache.ApplyMods: mid='{0}', propTag={1}", mid, propTag);
			QueryFilter queryFilter = filter;
			ModCache.Log log;
			if (!this.cache.TryGetValue(new ModCache.LogKey(mid, propTag), out log))
			{
				ModCache.ModCacheTracer.TraceDebug((long)this.nspiContext.ContextHandle, "Mod log not available");
				return queryFilter;
			}
			if (log.Mods.Count == 0)
			{
				ModCache.ModCacheTracer.TraceDebug((long)this.nspiContext.ContextHandle, "Mod log not available");
				return queryFilter;
			}
			List<QueryFilter> list = null;
			List<QueryFilter> list2 = null;
			foreach (KeyValuePair<int, bool> keyValuePair in log.Mods)
			{
				Guid guid;
				EphemeralIdTable.NamingContext namingContext;
				if (!this.nspiContext.EphemeralIdTable.GetGuid(keyValuePair.Key, out guid, out namingContext))
				{
					throw new InvalidOperationException("Mid in modcache without a guid");
				}
				if (!keyValuePair.Value)
				{
					if (list == null)
					{
						list = new List<QueryFilter>(1);
					}
					list.Add(new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Guid, guid));
				}
				else
				{
					if (list2 == null)
					{
						list2 = new List<QueryFilter>(1);
					}
					list2.Add(new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Guid, guid));
				}
			}
			if (list != null)
			{
				ModCache.ModCacheTracer.TraceDebug<int>((long)this.nspiContext.ContextHandle, "Applying {0} cached SET mod(s)", list.Count);
				list.Add(queryFilter);
				queryFilter = new OrFilter(list.ToArray());
			}
			if (list2 != null)
			{
				ModCache.ModCacheTracer.TraceDebug<int>((long)this.nspiContext.ContextHandle, "Applying {0} cached DELETE mod(s)", list2.Count);
				queryFilter = new AndFilter(new QueryFilter[]
				{
					queryFilter,
					new NotFilter(new OrFilter(list2.ToArray()))
				});
			}
			return queryFilter;
		}

		internal bool PurgeExpiredLogs()
		{
			List<ModCache.LogKey> list = null;
			int num = 0;
			int num2 = 0;
			foreach (KeyValuePair<ModCache.LogKey, ModCache.Log> keyValuePair in this.cache)
			{
				ModCache.LogKey key = keyValuePair.Key;
				ModCache.Log value = keyValuePair.Value;
				num += value.Mods.Count;
				if (value.LastUpdate + this.logExpiry < DateTime.UtcNow)
				{
					if (list == null)
					{
						list = new List<ModCache.LogKey>(1);
					}
					list.Add(key);
					num2 += value.Mods.Count;
				}
			}
			if (list != null)
			{
				foreach (ModCache.LogKey key2 in list)
				{
					this.cache.Remove(key2);
				}
				ModCache.ModCacheTracer.TraceDebug<int, int, int>((long)this.nspiContext.ContextHandle, "ModCache.PurgeExpiredLogs: Purged {0} expired logs, total: {1}, removed: {2}", list.Count, num, num2);
			}
			return num == num2;
		}

		private static readonly Trace ModCacheTracer = ExTraceGlobals.ModCacheTracer;

		private readonly TimeSpan logExpiry;

		private Dictionary<ModCache.LogKey, ModCache.Log> cache;

		private NspiContext nspiContext;

		private class LogKey
		{
			internal LogKey(int mid, PropTag propTag)
			{
				this.mid = mid;
				this.propTag = propTag;
			}

			public override int GetHashCode()
			{
				return this.mid ^ (int)this.propTag;
			}

			public override bool Equals(object other)
			{
				ModCache.LogKey logKey = other as ModCache.LogKey;
				return logKey != null && logKey.propTag == this.propTag && logKey.mid == this.mid;
			}

			private int mid;

			private PropTag propTag;
		}

		private class Log
		{
			internal Log(int capacity)
			{
				this.mods = new Dictionary<int, bool>(capacity);
				this.lastUpdate = DateTime.UtcNow;
			}

			internal Dictionary<int, bool> Mods
			{
				get
				{
					return this.mods;
				}
			}

			internal DateTime LastUpdate
			{
				get
				{
					return this.lastUpdate;
				}
			}

			private Dictionary<int, bool> mods;

			private DateTime lastUpdate;
		}
	}
}
