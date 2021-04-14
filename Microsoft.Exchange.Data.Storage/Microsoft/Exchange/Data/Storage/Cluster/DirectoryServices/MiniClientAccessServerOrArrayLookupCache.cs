using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage.Cluster.DirectoryServices
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class MiniClientAccessServerOrArrayLookupCache : IFindMiniClientAccessServerOrArray
	{
		public MiniClientAccessServerOrArrayLookupCache(ITopologyConfigurationSession adSession) : this(adSession, MiniClientAccessServerOrArrayLookupCache.TimeToLive, MiniClientAccessServerOrArrayLookupCache.TimeToNegativeLive, MiniClientAccessServerOrArrayLookupCache.CacheLockTimeout, MiniClientAccessServerOrArrayLookupCache.AdOperationTimeout, MiniClientAccessServerOrArrayLookupCache.MaximumTimeToLive)
		{
		}

		public MiniClientAccessServerOrArrayLookupCache(ITopologyConfigurationSession adSession, TimeSpan timeToLive, TimeSpan timeToNegativeLive) : this(adSession, timeToLive, timeToNegativeLive, MiniClientAccessServerOrArrayLookupCache.CacheLockTimeout, MiniClientAccessServerOrArrayLookupCache.AdOperationTimeout, MiniClientAccessServerOrArrayLookupCache.MaximumTimeToLive)
		{
		}

		public MiniClientAccessServerOrArrayLookupCache(ITopologyConfigurationSession adSession, TimeSpan timeToLive, TimeSpan timeToNegativeLive, TimeSpan cacheLockTimeout, TimeSpan adOperationTimeout) : this(adSession, timeToLive, timeToNegativeLive, cacheLockTimeout, adOperationTimeout, MiniClientAccessServerOrArrayLookupCache.MaximumTimeToLive)
		{
		}

		public MiniClientAccessServerOrArrayLookupCache(ITopologyConfigurationSession adSession, TimeSpan timeToLive, TimeSpan timeToNegativeLive, TimeSpan cacheLockTimeout, TimeSpan adOperationTimeout, TimeSpan maximumCacheTimeout)
		{
			this.AdSession = adSession;
			this.m_timeToLive = timeToLive;
			this.m_timeToNegativeLive = timeToNegativeLive;
			this.m_cacheLockTimeout = cacheLockTimeout;
			this.m_adOperationTimeout = adOperationTimeout;
			this.m_maximumTimeToLive = maximumCacheTimeout;
		}

		public static Microsoft.Exchange.Diagnostics.Trace Tracer
		{
			get
			{
				return ExTraceGlobals.ActiveManagerClientTracer;
			}
		}

		private ITopologyConfigurationSession AdSession
		{
			get
			{
				return this.adSession;
			}
			set
			{
				this.adSession = value;
				this.cdsAdSession = ADSessionFactory.CreateWrapper(this.adSession);
			}
		}

		public void Clear()
		{
			bool flag = false;
			try
			{
				flag = this.m_rwLock.TryEnterWriteLock(this.m_cacheLockTimeout);
				if (flag)
				{
					this.m_cache.Clear();
				}
				else
				{
					MiniClientAccessServerOrArrayLookupCache.Tracer.TraceError((long)this.GetHashCode(), "MiniClientAccessServerOrArrayLookup.Clear cound not clear cache due to lock timeout");
				}
			}
			finally
			{
				if (flag)
				{
					this.m_rwLock.ExitWriteLock();
				}
			}
		}

		public IADMiniClientAccessServerOrArray FindMiniClientAccessServerOrArrayByFqdn(string serverFqdn)
		{
			return this.LookupOrFindMiniClientAccessServerOrArray(serverFqdn, () => SimpleMiniClientAccessServerOrArrayLookup.FindMiniCasOrArrayByFqdn(this.cdsAdSession, serverFqdn));
		}

		public IADMiniClientAccessServerOrArray FindMiniClientAccessServerOrArrayByLegdn(string serverLegdn)
		{
			return this.LookupOrFindMiniClientAccessServerOrArray(serverLegdn, () => SimpleMiniClientAccessServerOrArrayLookup.FindMiniCasOrArrayByLegdn(this.cdsAdSession, serverLegdn));
		}

		public IADMiniClientAccessServerOrArray ReadMiniClientAccessServerOrArrayByObjectId(ADObjectId serverId)
		{
			string name = serverId.Name;
			return this.LookupOrFindMiniClientAccessServerOrArray(name, delegate
			{
				IADMiniClientAccessServerOrArray result = null;
				Exception ex = ADUtils.RunADOperation(delegate()
				{
					result = this.cdsAdSession.ReadMiniClientAccessServerOrArray(serverId);
				}, 2);
				if (ex != null)
				{
					StorageGlobals.EventLogger.LogEvent(StorageEventLogConstants.Tuple_ErrorActiveManagerClientADError, ex.Message, new object[0]);
					MiniClientAccessServerOrArrayLookupCache.Tracer.TraceDebug<Exception>((long)this.GetHashCode(), "MiniClientAccessServerOrArrayLookupCache.ReadMiniClientAccessServerOrArrayByObjectId got an exception: {0}", ex);
				}
				return result;
			});
		}

		public IADMiniClientAccessServerOrArray FindMiniClientAccessServerOrArrayWithClientAccess(ADObjectId siteId, ADObjectId preferredServerId)
		{
			return this.LookupOrFindMiniClientAccessServerOrArray(string.Format("siteid={0}", siteId.ToString()), delegate
			{
				IADMiniClientAccessServerOrArray result = null;
				Exception ex = ADUtils.RunADOperation(delegate()
				{
					result = SimpleMiniClientAccessServerOrArrayLookup.FindMiniCasOrArrayWithClientAccess(this, this.AdSession, siteId, preferredServerId);
				}, 2);
				if (ex != null)
				{
					MiniClientAccessServerOrArrayLookupCache.Tracer.TraceDebug<Exception>((long)this.GetHashCode(), "MiniClientAccessServerOrArrayLookupCache.ReadMiniClientAccessServerOrArrayByObjectId got an exception: {0}", ex);
				}
				return result;
			});
		}

		internal static ActiveManagerClientPerfmonInstance GetPerfCounters()
		{
			ActiveManagerClientPerfmonInstance instance;
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				string instanceName = string.Format("{0} - {1}", currentProcess.ProcessName, currentProcess.Id);
				instance = ActiveManagerClientPerfmon.GetInstance(instanceName);
			}
			return instance;
		}

		private static bool ShouldExpireCacheEntry(MiniClientAccessServerOrArrayCacheEntry entry)
		{
			return DateTime.UtcNow.CompareTo(entry.TimeToExpire) > 0;
		}

		private static bool MaximumTimeToLiveExpired(MiniClientAccessServerOrArrayCacheEntry entry)
		{
			return DateTime.UtcNow.CompareTo(entry.MaximumTimeToExpire) > 0;
		}

		private IADMiniClientAccessServerOrArray LookupOrFindMiniClientAccessServerOrArray(string serverKey, FindMiniClientAccessServerOrArrayCacheFailure serverLookup)
		{
			ExTraceGlobals.ActiveManagerClientTracer.TraceFunction<string>(0L, "LookupOrFindMiniClientAccessServerOrArray({0})", serverKey);
			MiniClientAccessServerOrArrayCacheEntry miniClientAccessServerOrArrayCacheEntry = null;
			bool flag = false;
			bool flag2 = false;
			try
			{
				flag = this.m_rwLock.TryEnterReadLock(MiniClientAccessServerOrArrayLookupCache.CacheLockTimeout);
				if (flag)
				{
					bool flag3 = this.m_cache.TryGetValue(serverKey, out miniClientAccessServerOrArrayCacheEntry);
					bool flag4 = false;
					if (flag3)
					{
						flag4 = MiniClientAccessServerOrArrayLookupCache.ShouldExpireCacheEntry(miniClientAccessServerOrArrayCacheEntry);
						ExTraceGlobals.ActiveManagerClientTracer.TraceDebug<string, MiniClientAccessServerOrArrayCacheEntry, bool>((long)this.GetHashCode(), "LookupOrFindMiniClientAccessServerOrArray( {0} ) was found in the cache: {1}, and shouldExpireEntry={2}", serverKey, miniClientAccessServerOrArrayCacheEntry, flag4);
					}
					if (!flag3 || flag4)
					{
						flag2 = true;
						MiniClientAccessServerOrArrayLookupCache.m_perfCounters.GetServerForDatabaseClientServerInformationCacheMisses.Increment();
					}
					else
					{
						MiniClientAccessServerOrArrayLookupCache.m_perfCounters.GetServerForDatabaseClientServerInformationCacheHits.Increment();
					}
				}
				else
				{
					MiniClientAccessServerOrArrayLookupCache.Tracer.TraceError((long)this.GetHashCode(), "Timeout waiting for the read lock in MiniClientAccessServerOrArrayLookupCache.LookupOrFindAdObject()");
					flag2 = true;
				}
			}
			finally
			{
				if (flag)
				{
					this.m_rwLock.ExitReadLock();
				}
			}
			if (flag2)
			{
				bool flag5 = false;
				IADMiniClientAccessServerOrArray updatedServer = null;
				try
				{
					InvokeWithTimeout.Invoke(delegate()
					{
						updatedServer = serverLookup();
					}, this.m_adOperationTimeout);
				}
				catch (TimeoutException ex)
				{
					StorageGlobals.EventLogger.LogEvent(StorageEventLogConstants.Tuple_ErrorActiveManagerClientADTimeout, serverKey, new object[]
					{
						serverKey,
						this.m_adOperationTimeout
					});
					MiniClientAccessServerOrArrayLookupCache.Tracer.TraceError<string>((long)this.GetHashCode(), "Timeout on ad query: {0}", ex.Message);
					flag5 = true;
				}
				miniClientAccessServerOrArrayCacheEntry = new MiniClientAccessServerOrArrayCacheEntry(updatedServer, this.m_timeToLive, this.m_timeToNegativeLive, this.m_maximumTimeToLive);
				bool flag6 = false;
				try
				{
					flag6 = this.m_rwLock.TryEnterWriteLock(MiniClientAccessServerOrArrayLookupCache.CacheLockTimeout);
					if (flag6)
					{
						if (updatedServer == null && flag5)
						{
							MiniClientAccessServerOrArrayCacheEntry miniClientAccessServerOrArrayCacheEntry2 = null;
							bool flag7 = this.m_cache.TryGetValue(serverKey, out miniClientAccessServerOrArrayCacheEntry2);
							if (flag7 && !MiniClientAccessServerOrArrayLookupCache.MaximumTimeToLiveExpired(miniClientAccessServerOrArrayCacheEntry2))
							{
								miniClientAccessServerOrArrayCacheEntry = miniClientAccessServerOrArrayCacheEntry2;
								ExTraceGlobals.ActiveManagerClientTracer.TraceDebug<IADMiniClientAccessServerOrArray, MiniClientAccessServerOrArrayCacheEntry>((long)this.GetHashCode(), "New ad object was not found, but found possibly stale result '{0}' in the cache as {1}.", miniClientAccessServerOrArrayCacheEntry.MiniClientAccessServerOrArrayData, miniClientAccessServerOrArrayCacheEntry);
							}
							else
							{
								flag5 = false;
							}
						}
						if (!flag5)
						{
							this.m_cache[serverKey] = miniClientAccessServerOrArrayCacheEntry;
							ExTraceGlobals.ActiveManagerClientTracer.TraceDebug<IADMiniClientAccessServerOrArray, MiniClientAccessServerOrArrayCacheEntry>((long)this.GetHashCode(), "Stored server '{0}' in the cache as {1}.", miniClientAccessServerOrArrayCacheEntry.MiniClientAccessServerOrArrayData, miniClientAccessServerOrArrayCacheEntry);
						}
					}
					else
					{
						MiniClientAccessServerOrArrayLookupCache.Tracer.TraceError((long)this.GetHashCode(), "Timeout waiting for write lock in MiniServerLookupCache.LookupOrFindAdObject()");
					}
				}
				finally
				{
					if (flag6)
					{
						this.m_rwLock.ExitWriteLock();
					}
				}
			}
			MiniClientAccessServerOrArrayLookupCache.m_perfCounters.GetServerForDatabaseClientServerInformationCacheEntries.RawValue = (long)this.m_cache.Count;
			return miniClientAccessServerOrArrayCacheEntry.MiniClientAccessServerOrArrayData;
		}

		private readonly TimeSpan m_cacheLockTimeout;

		private readonly TimeSpan m_adOperationTimeout;

		internal static readonly TimeSpan CacheLockTimeout = TimeSpan.FromSeconds(60.0);

		internal static readonly TimeSpan AdOperationTimeout = TimeSpan.FromSeconds(60.0);

		internal static readonly TimeSpan TimeToLive = new TimeSpan(0, 20, 0);

		internal static readonly TimeSpan TimeToNegativeLive = new TimeSpan(0, 1, 0);

		internal static readonly TimeSpan MaximumTimeToLive = new TimeSpan(0, 30, 0);

		private readonly TimeSpan m_maximumTimeToLive;

		private readonly TimeSpan m_timeToLive;

		private readonly TimeSpan m_timeToNegativeLive;

		private static readonly ActiveManagerClientPerfmonInstance m_perfCounters = MiniClientAccessServerOrArrayLookupCache.GetPerfCounters();

		private ITopologyConfigurationSession adSession;

		private IADToplogyConfigurationSession cdsAdSession;

		private Dictionary<string, MiniClientAccessServerOrArrayCacheEntry> m_cache = new Dictionary<string, MiniClientAccessServerOrArrayCacheEntry>(8, StringComparer.OrdinalIgnoreCase);

		[NonSerialized]
		private ReaderWriterLockSlim m_rwLock = new ReaderWriterLockSlim();
	}
}
