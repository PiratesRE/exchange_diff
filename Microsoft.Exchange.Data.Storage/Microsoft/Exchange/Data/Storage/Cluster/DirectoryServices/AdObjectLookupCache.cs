using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Threading;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.HA;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.Diagnostics.Performance;
using Microsoft.Exchange.Net;
using Microsoft.Win32;

namespace Microsoft.Exchange.Data.Storage.Cluster.DirectoryServices
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class AdObjectLookupCache<TADWrapperObject> : IFindAdObject<TADWrapperObject> where TADWrapperObject : class, IADObjectCommon
	{
		public bool MinimizeObjects { get; set; }

		private static bool LogAdLatency
		{
			get
			{
				if (AdObjectLookupCache<TADWrapperObject>.s_logAdLatency == null)
				{
					AdObjectLookupCache<TADWrapperObject>.s_logAdLatency = new bool?(false);
					Exception ex = null;
					try
					{
						using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\HA\\AdObjectLookupCache"))
						{
							if (registryKey != null)
							{
								int value = RegistryReader.Instance.GetValue<int>(registryKey, null, "LogADLatency", 0);
								if (value != 0)
								{
									AdObjectLookupCache<TADWrapperObject>.s_logAdLatency = new bool?(true);
								}
							}
						}
					}
					catch (SecurityException ex2)
					{
						ex = ex2;
					}
					catch (IOException ex3)
					{
						ex = ex3;
					}
					catch (UnauthorizedAccessException ex4)
					{
						ex = ex4;
					}
					if (ex != null)
					{
						AdObjectLookupCache<TADWrapperObject>.Tracer.TraceError<Exception>(0L, "LogAdLatency failed to read regkey: {0}", ex);
						AdObjectLookupCache<TADWrapperObject>.s_logAdLatency = null;
					}
				}
				return AdObjectLookupCache<TADWrapperObject>.s_logAdLatency != null && AdObjectLookupCache<TADWrapperObject>.s_logAdLatency.Value;
			}
		}

		public AdObjectLookupCache(IADToplogyConfigurationSession adSession) : this(adSession, AdObjectLookupCache<TADWrapperObject>.TimeToLive, AdObjectLookupCache<TADWrapperObject>.TimeToNegativeLive, AdObjectLookupCache<TADWrapperObject>.CacheLockTimeout, AdObjectLookupCache<TADWrapperObject>.AdOperationTimeout, AdObjectLookupCache<TADWrapperObject>.MaximumTimeToLive)
		{
		}

		public AdObjectLookupCache(IADToplogyConfigurationSession adSession, TimeSpan timeToLive, TimeSpan timeToNegativeLive) : this(adSession, timeToLive, timeToNegativeLive, AdObjectLookupCache<TADWrapperObject>.CacheLockTimeout, AdObjectLookupCache<TADWrapperObject>.AdOperationTimeout, AdObjectLookupCache<TADWrapperObject>.MaximumTimeToLive)
		{
		}

		public AdObjectLookupCache(IADToplogyConfigurationSession adSession, TimeSpan timeToLive, TimeSpan timeToNegativeLive, TimeSpan cacheLockTimeout, TimeSpan adOperationTimeout) : this(adSession, timeToLive, timeToNegativeLive, cacheLockTimeout, adOperationTimeout, AdObjectLookupCache<TADWrapperObject>.MaximumTimeToLive)
		{
		}

		public AdObjectLookupCache(IADToplogyConfigurationSession adSession, TimeSpan timeToLive, TimeSpan timeToNegativeLive, TimeSpan cacheLockTimeout, TimeSpan adOperationTimeout, TimeSpan maximumCacheTimeout)
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

		public void Clear()
		{
			bool flag = false;
			try
			{
				int num = 0;
				while (num < 2 && !flag)
				{
					flag = this.m_rwLock.TryEnterWriteLock(this.m_cacheLockTimeout);
					num++;
				}
				if (flag)
				{
					this.m_cache.Clear();
				}
				else
				{
					AdObjectLookupCache<TADWrapperObject>.Tracer.TraceError((long)this.GetHashCode(), "AdObjectLookupCache.Clear cound not clear cache due to lock timeout");
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

		public TADWrapperObject ReadAdObjectByObjectId(ADObjectId objectId)
		{
			Exception ex;
			return this.ReadAdObjectByObjectIdEx(objectId, out ex);
		}

		public TADWrapperObject ReadAdObjectByObjectIdEx(ADObjectId objectId, out Exception exception)
		{
			TADWrapperObject result = default(TADWrapperObject);
			string name = objectId.Name;
			Exception tempEx = null;
			exception = null;
			result = this.LookupOrFindAdObject(name, delegate
			{
				TADWrapperObject adObject = default(TADWrapperObject);
				tempEx = ADUtils.RunADOperation(delegate()
				{
					adObject = this.AdSession.ReadADObject<TADWrapperObject>(objectId);
				}, 2);
				if (tempEx != null)
				{
					StorageGlobals.EventLogger.LogEvent(StorageEventLogConstants.Tuple_ErrorActiveManagerClientADError, tempEx.Message, new object[0]);
					AdObjectLookupCache<TADWrapperObject>.Tracer.TraceError<Exception>((long)this.GetHashCode(), "AdObjectLookupCache.ReadAdObjectByObjectIdEx got exception: {0}", tempEx);
				}
				return adObject;
			}, AdObjectLookupFlags.None);
			exception = tempEx;
			return result;
		}

		public TADWrapperObject FindAdObjectByGuid(Guid objectGuid)
		{
			return this.LookupOrFindAdObject(objectGuid.ToString(), () => SimpleAdObjectLookup<TADWrapperObject>.FindAdObjectTypeByGuidStatic(this.AdSession, objectGuid), AdObjectLookupFlags.None);
		}

		public TADWrapperObject FindAdObjectByGuidEx(Guid objectGuid, AdObjectLookupFlags flags)
		{
			return this.FindAdObjectByGuidEx(objectGuid, flags, NullPerformanceDataLogger.Instance);
		}

		public TADWrapperObject FindAdObjectByGuidEx(Guid objectGuid, AdObjectLookupFlags flags, IPerformanceDataLogger perfLogger)
		{
			return this.LookupOrFindAdObject(objectGuid.ToString(), () => SimpleAdObjectLookup<TADWrapperObject>.FindAdObjectTypeByGuidStatic(this.AdSession, objectGuid, perfLogger), flags);
		}

		public TADWrapperObject FindAdObjectByQuery(QueryFilter queryFilter)
		{
			return this.LookupOrFindAdObject(queryFilter.ToString(), () => SimpleAdObjectLookup<TADWrapperObject>.FindAdObjectWithQueryStatic(this.AdSession, queryFilter), AdObjectLookupFlags.None);
		}

		public TADWrapperObject FindAdObjectByQueryEx(QueryFilter queryFilter, AdObjectLookupFlags flags)
		{
			return this.LookupOrFindAdObject(queryFilter.ToString(), () => SimpleAdObjectLookup<TADWrapperObject>.FindAdObjectWithQueryStatic(this.AdSession, queryFilter), flags);
		}

		public TADWrapperObject FindServerByFqdn(string fqdn)
		{
			Exception ex;
			return this.FindServerByFqdnWithException(fqdn, out ex);
		}

		public TADWrapperObject FindServerByFqdnWithException(string fqdn, out Exception exception)
		{
			ExAssert.RetailAssert(typeof(TADWrapperObject) == typeof(IADServer), "This function should only be called with Server objects!");
			if (typeof(TADWrapperObject) != typeof(IADServer))
			{
				throw new NotImplementedException("This only works for Server objects.");
			}
			exception = null;
			Exception ex = null;
			AdObjectLookupFlags flags = AdObjectLookupFlags.None;
			string shortName = MachineName.GetNodeNameFromFqdn(fqdn);
			TADWrapperObject result = this.LookupOrFindAdObject(shortName, () => SimpleAdObjectLookup<TADWrapperObject>.FindAdObjectByServerNameStatic(this.AdSession, shortName, out ex), flags);
			exception = ex;
			return result;
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

		private static bool ShouldExpireCacheEntry(AdObjectCacheEntry<TADWrapperObject> entry)
		{
			return DateTime.UtcNow.CompareTo(entry.TimeToExpire) > 0;
		}

		private static bool MaximumTimeToLiveExpired(AdObjectCacheEntry<TADWrapperObject> entry)
		{
			return DateTime.UtcNow.CompareTo(entry.MaximumTimeToExpire) > 0;
		}

		internal bool CheckAndSetADLock(string identifyingName)
		{
			bool flag = false;
			bool flag2 = false;
			try
			{
				int num = 0;
				while (num < 2 && !flag2)
				{
					flag2 = this.m_performingADLookupLock.TryEnterUpgradeableReadLock(AdObjectLookupCache<TADWrapperObject>.CacheLockTimeout);
					num++;
				}
				if (flag2)
				{
					flag = this.m_performingADLookup.Contains(identifyingName);
					if (flag)
					{
						goto IL_A6;
					}
					bool flag3 = false;
					try
					{
						int num2 = 0;
						while (num2 < 2 && !flag3)
						{
							flag3 = this.m_performingADLookupLock.TryEnterWriteLock(AdObjectLookupCache<TADWrapperObject>.CacheLockTimeout);
							num2++;
						}
						if (flag3)
						{
							flag = this.m_performingADLookup.Contains(identifyingName);
							if (!flag)
							{
								this.m_performingADLookup.Add(identifyingName);
							}
						}
						else
						{
							ExAssert.RetailAssert(false, "Timeout waiting for write lock in AdObjectLookupCache.CheckAndSetADLock()");
						}
						goto IL_A6;
					}
					finally
					{
						if (flag3)
						{
							this.m_performingADLookupLock.ExitWriteLock();
						}
					}
				}
				ExAssert.RetailAssert(false, "Timeout waiting for upgradable read lock in AdObjectLookupCache.CheckAndSetADLock()");
				IL_A6:;
			}
			finally
			{
				if (flag2)
				{
					this.m_performingADLookupLock.ExitUpgradeableReadLock();
				}
			}
			return !flag;
		}

		internal void ReleaseADLock(string identifyingName)
		{
			bool flag = false;
			try
			{
				int num = 0;
				while (num < 2 && !flag)
				{
					flag = this.m_performingADLookupLock.TryEnterUpgradeableReadLock(AdObjectLookupCache<TADWrapperObject>.CacheLockTimeout);
					num++;
				}
				if (flag)
				{
					if (!this.m_performingADLookup.Contains(identifyingName))
					{
						goto IL_9C;
					}
					bool flag2 = false;
					try
					{
						int num2 = 0;
						while (num2 < 2 && !flag2)
						{
							flag2 = this.m_performingADLookupLock.TryEnterWriteLock(AdObjectLookupCache<TADWrapperObject>.CacheLockTimeout);
							num2++;
						}
						if (flag2)
						{
							if (this.m_performingADLookup.Contains(identifyingName))
							{
								this.m_performingADLookup.Remove(identifyingName);
							}
						}
						else
						{
							ExAssert.RetailAssert(false, "Timeout waiting for write lock in DatabaseLocationCache.ReleaseADLock()");
						}
						goto IL_9C;
					}
					finally
					{
						if (flag2)
						{
							this.m_performingADLookupLock.ExitWriteLock();
						}
					}
				}
				ExAssert.RetailAssert(false, "Timeout waiting for upgradable read lock in DatabaseLocationCache.ReleaseADLock()");
				IL_9C:;
			}
			finally
			{
				if (flag)
				{
					this.m_performingADLookupLock.ExitUpgradeableReadLock();
				}
			}
		}

		private TADWrapperObject LookupOrFindAdObject(string identifyingName, AdObjectLookupCache<TADWrapperObject>.FindAdObjectCacheFailure objectLookupFunction, AdObjectLookupFlags flags)
		{
			ExTraceGlobals.ActiveManagerClientTracer.TraceFunction<string>(0L, "LookupOrFindAdObject({0})", identifyingName);
			AdObjectCacheEntry<TADWrapperObject> adObjectCacheEntry = null;
			bool flag = false;
			bool flag2 = (flags & AdObjectLookupFlags.ReadThrough) != AdObjectLookupFlags.None;
			bool flag3 = false;
			try
			{
				int num = 0;
				while (num < 2 && !flag)
				{
					flag = this.m_rwLock.TryEnterReadLock(AdObjectLookupCache<TADWrapperObject>.CacheLockTimeout);
					num++;
				}
				if (flag)
				{
					flag3 = this.m_cache.TryGetValue(identifyingName, out adObjectCacheEntry);
					bool flag4 = false;
					if (flag3)
					{
						flag4 = AdObjectLookupCache<TADWrapperObject>.ShouldExpireCacheEntry(adObjectCacheEntry);
						ExTraceGlobals.ActiveManagerClientTracer.TraceDebug((long)this.GetHashCode(), "LookupOrFindAdObject( {0} ) was found in the cache: {1}, and shouldExpireEntry={2}, flags={3}", new object[]
						{
							identifyingName,
							adObjectCacheEntry,
							flag4,
							flags
						});
					}
					if (!flag3 || flag4)
					{
						flag2 = true;
					}
				}
				else
				{
					AdObjectLookupCache<TADWrapperObject>.Tracer.TraceError((long)this.GetHashCode(), "Timeout waiting for the read lock in AdObjectLookupCache.LookupOrFindAdObject()");
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
				bool flag6 = false;
				TADWrapperObject updatedObject = default(TADWrapperObject);
				try
				{
					flag5 = this.CheckAndSetADLock(identifyingName);
					if (flag5)
					{
						InvokeWithTimeout.Invoke(delegate()
						{
							updatedObject = objectLookupFunction();
						}, this.m_adOperationTimeout);
					}
					else if (!flag3 || AdObjectLookupCache<TADWrapperObject>.MaximumTimeToLiveExpired(adObjectCacheEntry))
					{
						int num2 = 10;
						int num3 = 10;
						for (int i = 0; i < num3; i++)
						{
							Thread.Sleep(num2);
							bool flag7 = false;
							try
							{
								int num4 = 0;
								while (num4 < 2 && !flag7)
								{
									flag7 = this.m_rwLock.TryEnterReadLock(AdObjectLookupCache<TADWrapperObject>.CacheLockTimeout);
									num4++;
								}
								if (flag7)
								{
									flag3 = this.m_cache.TryGetValue(identifyingName, out adObjectCacheEntry);
								}
							}
							finally
							{
								if (flag7)
								{
									this.m_rwLock.ExitReadLock();
								}
							}
							if (flag3 && !AdObjectLookupCache<TADWrapperObject>.MaximumTimeToLiveExpired(adObjectCacheEntry))
							{
								if (AdObjectLookupCache<TADWrapperObject>.LogAdLatency)
								{
									StorageGlobals.EventLogger.LogEvent(StorageEventLogConstants.Tuple_ActiveManagerClientAnotherThreadCompleted, identifyingName, new object[]
									{
										identifyingName,
										num2 * i
									});
								}
								AdObjectLookupCache<TADWrapperObject>.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Another thread finished doing query for object {0}.", identifyingName);
								flag6 = true;
								break;
							}
						}
						if (!flag3 || AdObjectLookupCache<TADWrapperObject>.MaximumTimeToLiveExpired(adObjectCacheEntry))
						{
							StorageGlobals.EventLogger.LogEvent(StorageEventLogConstants.Tuple_ActiveManagerClientAnotherThreadInADCallTimeout, identifyingName, new object[]
							{
								identifyingName,
								num2 * num3
							});
							AdObjectLookupCache<TADWrapperObject>.Tracer.TraceDebug<string, int>((long)this.GetHashCode(), "Another thread doing query for object {0}, however this thread didn't complete in {1} msec.", identifyingName, num2 * num3);
							updatedObject = objectLookupFunction();
						}
					}
					else
					{
						if (AdObjectLookupCache<TADWrapperObject>.LogAdLatency)
						{
							StorageGlobals.EventLogger.LogEvent(StorageEventLogConstants.Tuple_ActiveManagerClientAnotherThreadInADCall, identifyingName, new object[]
							{
								identifyingName
							});
						}
						AdObjectLookupCache<TADWrapperObject>.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Another thread doing query for object {0}.", identifyingName);
						flag6 = true;
					}
				}
				catch (TimeoutException ex)
				{
					StorageGlobals.EventLogger.LogEvent(StorageEventLogConstants.Tuple_ErrorActiveManagerClientADTimeout, identifyingName, new object[]
					{
						identifyingName,
						this.m_adOperationTimeout
					});
					AdObjectLookupCache<TADWrapperObject>.Tracer.TraceError<string>((long)this.GetHashCode(), "Timeout on ad query: {0}", ex.Message);
					flag6 = true;
				}
				finally
				{
					if (flag5)
					{
						this.ReleaseADLock(identifyingName);
					}
				}
				if (updatedObject != null && this.MinimizeObjects)
				{
					updatedObject.Minimize();
				}
				adObjectCacheEntry = new AdObjectCacheEntry<TADWrapperObject>(updatedObject, this.m_timeToLive, this.m_timeToNegativeLive, this.m_maximumTimeToLive);
				bool flag8 = false;
				try
				{
					int num5 = 0;
					while (num5 < 2 && !flag8)
					{
						flag8 = this.m_rwLock.TryEnterWriteLock(AdObjectLookupCache<TADWrapperObject>.CacheLockTimeout);
						num5++;
					}
					if (flag8)
					{
						if (updatedObject == null && flag6)
						{
							AdObjectCacheEntry<TADWrapperObject> adObjectCacheEntry2 = null;
							flag3 = this.m_cache.TryGetValue(identifyingName, out adObjectCacheEntry2);
							if (flag3 && !AdObjectLookupCache<TADWrapperObject>.MaximumTimeToLiveExpired(adObjectCacheEntry2))
							{
								adObjectCacheEntry = adObjectCacheEntry2;
								ExTraceGlobals.ActiveManagerClientTracer.TraceDebug<TADWrapperObject, AdObjectCacheEntry<TADWrapperObject>>((long)this.GetHashCode(), "New ad object was not found, but found possibly stale result '{0}' in the cache as {1}.", adObjectCacheEntry.AdObjectData, adObjectCacheEntry);
							}
							else
							{
								flag6 = false;
							}
						}
						if (!flag6)
						{
							this.m_cache[identifyingName] = adObjectCacheEntry;
							ExTraceGlobals.ActiveManagerClientTracer.TraceDebug<TADWrapperObject, AdObjectCacheEntry<TADWrapperObject>>((long)this.GetHashCode(), "Stored object '{0}' in the cache as {1}.", adObjectCacheEntry.AdObjectData, adObjectCacheEntry);
						}
					}
					else
					{
						AdObjectLookupCache<TADWrapperObject>.Tracer.TraceError((long)this.GetHashCode(), "Timeout waiting for write lock in AdObjectLookupCache.LookupOrFindAdObject()");
					}
				}
				finally
				{
					if (flag8)
					{
						this.m_rwLock.ExitWriteLock();
					}
				}
			}
			return adObjectCacheEntry.AdObjectData;
		}

		private const string RegistryPath = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\HA\\AdObjectLookupCache";

		private const string RegistryLogAdLatency = "LogADLatency";

		private static bool? s_logAdLatency = null;

		private readonly TimeSpan m_cacheLockTimeout;

		private readonly TimeSpan m_adOperationTimeout;

		private readonly HashSet<string> m_performingADLookup = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

		private readonly ReaderWriterLockSlim m_performingADLookupLock = new ReaderWriterLockSlim();

		internal static readonly TimeSpan CacheLockTimeout = TimeSpan.FromSeconds(60.0);

		internal static readonly TimeSpan AdOperationTimeout = TimeSpan.FromSeconds(60.0);

		internal static readonly TimeSpan TimeToLive = new TimeSpan(0, 5, 0);

		internal static readonly TimeSpan TimeToNegativeLive = new TimeSpan(0, 0, 30);

		internal static readonly TimeSpan MaximumTimeToLive = new TimeSpan(0, 10, 0);

		private readonly TimeSpan m_maximumTimeToLive;

		private readonly TimeSpan m_timeToLive;

		private readonly TimeSpan m_timeToNegativeLive;

		private Dictionary<string, AdObjectCacheEntry<TADWrapperObject>> m_cache = new Dictionary<string, AdObjectCacheEntry<TADWrapperObject>>(8, StringComparer.OrdinalIgnoreCase);

		[NonSerialized]
		private ReaderWriterLockSlim m_rwLock = new ReaderWriterLockSlim();

		internal delegate TADWrapperObject FindAdObjectCacheFailure();
	}
}
