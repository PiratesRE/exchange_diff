using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.HA;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage.Cluster.DirectoryServices
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class MiniServerLookupCache : IFindMiniServer
	{
		public MiniServerLookupCache(IADToplogyConfigurationSession adSession) : this(adSession, MiniServerLookupCache.TimeToLive, MiniServerLookupCache.TimeToNegativeLive, MiniServerLookupCache.CacheLockTimeout, MiniServerLookupCache.AdOperationTimeout, MiniServerLookupCache.MaximumTimeToLive)
		{
		}

		public MiniServerLookupCache(IADToplogyConfigurationSession adSession, TimeSpan timeToLive, TimeSpan timeToNegativeLive) : this(adSession, timeToLive, timeToNegativeLive, MiniServerLookupCache.CacheLockTimeout, MiniServerLookupCache.AdOperationTimeout, MiniServerLookupCache.MaximumTimeToLive)
		{
		}

		public MiniServerLookupCache(IADToplogyConfigurationSession adSession, TimeSpan timeToLive, TimeSpan timeToNegativeLive, TimeSpan cacheLockTimeout, TimeSpan adOperationTimeout) : this(adSession, timeToLive, timeToNegativeLive, cacheLockTimeout, adOperationTimeout, MiniServerLookupCache.MaximumTimeToLive)
		{
		}

		public MiniServerLookupCache(IADToplogyConfigurationSession adSession, TimeSpan timeToLive, TimeSpan timeToNegativeLive, TimeSpan cacheLockTimeout, TimeSpan adOperationTimeout, TimeSpan maximumCacheTimeout)
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

		public IADToplogyConfigurationSession AdSession { get; set; }

		public bool MinimizeObjects { get; set; }

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
					MiniServerLookupCache.Tracer.TraceError((long)this.GetHashCode(), "MiniServerLookupCache.Clear cound not clear cache due to lock timeout");
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

		public IADServer FindMiniServerByFqdn(string serverFqdn)
		{
			string nodeNameFromFqdn = MachineName.GetNodeNameFromFqdn(serverFqdn);
			return this.FindMiniServerByShortName(nodeNameFromFqdn);
		}

		public IADServer FindMiniServerByShortName(string shortName)
		{
			Exception ex;
			return this.FindMiniServerByShortNameEx(shortName, out ex);
		}

		public IADServer FindMiniServerByShortNameEx(string shortName, out Exception ex)
		{
			Exception tempEx = null;
			IADServer result = this.LookupOrFindMiniServer(shortName, delegate
			{
				IADServer result = null;
				tempEx = ADUtils.RunADOperation(delegate()
				{
					result = this.AdSession.FindMiniServerByName(shortName);
				}, 2);
				if (tempEx != null)
				{
					StorageGlobals.EventLogger.LogEvent(StorageEventLogConstants.Tuple_ErrorActiveManagerClientADError, tempEx.Message, new object[0]);
					MiniServerLookupCache.Tracer.TraceDebug<Exception>((long)this.GetHashCode(), "MiniServerLookupCache.FindMiniServerByFqdn got an exception: {0}", tempEx);
				}
				return result;
			});
			ex = tempEx;
			return result;
		}

		public IADServer ReadMiniServerByObjectId(ADObjectId serverId)
		{
			string name = serverId.Name;
			return this.LookupOrFindMiniServer(name, delegate
			{
				IADServer result = null;
				Exception ex = ADUtils.RunADOperation(delegate()
				{
					result = this.AdSession.ReadMiniServer(serverId);
				}, 2);
				if (ex != null)
				{
					MiniServerLookupCache.Tracer.TraceDebug<Exception>((long)this.GetHashCode(), "MiniServerLookupCache.ReadMiniServerByObjectId got an exception: {0}", ex);
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

		private static bool ShouldExpireCacheEntry(MiniServerCacheEntry entry)
		{
			return DateTime.UtcNow.CompareTo(entry.TimeToExpire) > 0;
		}

		private static bool MaximumTimeToLiveExpired(MiniServerCacheEntry entry)
		{
			return DateTime.UtcNow.CompareTo(entry.MaximumTimeToExpire) > 0;
		}

		private IADServer LookupOrFindMiniServer(string serverShortName, FindMiniServerCacheFailure serverLookup)
		{
			ExTraceGlobals.ActiveManagerClientTracer.TraceFunction<string>(0L, "LookupOrFindMiniServer({0})", serverShortName);
			MiniServerCacheEntry miniServerCacheEntry = null;
			bool flag = false;
			bool flag2 = false;
			try
			{
				flag = this.m_rwLock.TryEnterReadLock(MiniServerLookupCache.CacheLockTimeout);
				if (flag)
				{
					bool flag3 = this.m_cache.TryGetValue(serverShortName, out miniServerCacheEntry);
					bool flag4 = false;
					if (flag3)
					{
						flag4 = MiniServerLookupCache.ShouldExpireCacheEntry(miniServerCacheEntry);
						ExTraceGlobals.ActiveManagerClientTracer.TraceDebug<string, MiniServerCacheEntry, bool>((long)this.GetHashCode(), "LookupOrFindMiniServer( {0} ) was found in the cache: {1}, and shouldExpireEntry={2}", serverShortName, miniServerCacheEntry, flag4);
					}
					if (!flag3 || flag4)
					{
						flag2 = true;
						MiniServerLookupCache.m_perfCounters.GetServerForDatabaseClientServerInformationCacheMisses.Increment();
					}
					else
					{
						MiniServerLookupCache.m_perfCounters.GetServerForDatabaseClientServerInformationCacheHits.Increment();
					}
				}
				else
				{
					MiniServerLookupCache.Tracer.TraceError((long)this.GetHashCode(), "Timeout waiting for the read lock in MiniServerLookupCache.LookupOrFindAdObject()");
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
				IADServer updatedServer = null;
				try
				{
					InvokeWithTimeout.Invoke(delegate()
					{
						updatedServer = serverLookup();
					}, this.m_adOperationTimeout);
				}
				catch (TimeoutException ex)
				{
					StorageGlobals.EventLogger.LogEvent(StorageEventLogConstants.Tuple_ErrorActiveManagerClientADTimeout, serverShortName, new object[]
					{
						serverShortName,
						this.m_adOperationTimeout
					});
					MiniServerLookupCache.Tracer.TraceError<string>((long)this.GetHashCode(), "Timeout on ad query: {0}", ex.Message);
					flag5 = true;
				}
				if (updatedServer != null && this.MinimizeObjects)
				{
					updatedServer.Minimize();
				}
				miniServerCacheEntry = new MiniServerCacheEntry(updatedServer, this.m_timeToLive, this.m_timeToNegativeLive, this.m_maximumTimeToLive);
				bool flag6 = false;
				try
				{
					flag6 = this.m_rwLock.TryEnterWriteLock(MiniServerLookupCache.CacheLockTimeout);
					if (flag6)
					{
						if (updatedServer == null && flag5)
						{
							MiniServerCacheEntry miniServerCacheEntry2 = null;
							bool flag7 = this.m_cache.TryGetValue(serverShortName, out miniServerCacheEntry2);
							if (flag7 && !MiniServerLookupCache.MaximumTimeToLiveExpired(miniServerCacheEntry2))
							{
								miniServerCacheEntry = miniServerCacheEntry2;
								ExTraceGlobals.ActiveManagerClientTracer.TraceDebug<IADServer, MiniServerCacheEntry>((long)this.GetHashCode(), "New ad object was not found, but found possibly stale result '{0}' in the cache as {1}.", miniServerCacheEntry.MiniServerData, miniServerCacheEntry);
							}
							else
							{
								flag5 = false;
							}
						}
						if (!flag5)
						{
							this.m_cache[serverShortName] = miniServerCacheEntry;
							ExTraceGlobals.ActiveManagerClientTracer.TraceDebug<IADServer, MiniServerCacheEntry>((long)this.GetHashCode(), "Stored server '{0}' in the cache as {1}.", miniServerCacheEntry.MiniServerData, miniServerCacheEntry);
						}
					}
					else
					{
						MiniServerLookupCache.Tracer.TraceError((long)this.GetHashCode(), "Timeout waiting for write lock in MiniServerLookupCache.LookupOrFindAdObject()");
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
			MiniServerLookupCache.m_perfCounters.GetServerForDatabaseClientServerInformationCacheEntries.RawValue = (long)this.m_cache.Count;
			return miniServerCacheEntry.MiniServerData;
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

		private static readonly ActiveManagerClientPerfmonInstance m_perfCounters = MiniServerLookupCache.GetPerfCounters();

		private Dictionary<string, MiniServerCacheEntry> m_cache = new Dictionary<string, MiniServerCacheEntry>(8, StringComparer.OrdinalIgnoreCase);

		[NonSerialized]
		private ReaderWriterLockSlim m_rwLock = new ReaderWriterLockSlim();
	}
}
