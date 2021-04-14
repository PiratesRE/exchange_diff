using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ServerLocator;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.HttpProxy.Common;
using Microsoft.Exchange.HttpProxy.EventLogs;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.SharedCache.Client;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.HttpProxy
{
	internal class MailboxServerCache
	{
		private MailboxServerCache()
		{
			this.serversCache = new ExactTimeoutCache<Guid, MailboxServerCacheEntry>(new RemoveItemDelegate<Guid, MailboxServerCacheEntry>(this.OnRemoveItem), null, delegate(Exception ex)
			{
				Diagnostics.ReportException(ex, FrontEndHttpProxyEventLogConstants.Tuple_InternalServerError, null, "Exception from MailboxServerCache: {0}");
			}, MailboxServerCache.MailboxServerCacheMaxSize.Value, false);
			if (HttpProxySettings.MailboxServerLocatorSharedCacheEnabled.Value)
			{
				this.sharedCacheClient = new SharedCacheClient(WellKnownSharedCache.MailboxServerLocator, "MailboxServerLocator_" + HttpProxyGlobals.ProtocolType, HttpProxySettings.GlobalSharedCacheRpcTimeout.Value, ConcurrencyGuards.SharedCache);
			}
			if (MailboxServerCache.BackgroundServerRefreshEnabled.Value)
			{
				ThreadPool.QueueUserWorkItem(new WaitCallback(this.CacheRefreshEntry));
			}
		}

		public static MailboxServerCache Instance
		{
			get
			{
				if (MailboxServerCache.instance == null)
				{
					lock (MailboxServerCache.staticLock)
					{
						if (MailboxServerCache.instance == null)
						{
							MailboxServerCache.instance = new MailboxServerCache();
						}
					}
				}
				return MailboxServerCache.instance;
			}
		}

		public bool LazyRefreshDisabled { get; set; }

		internal bool AlwaysRefresh
		{
			get
			{
				return this.alwaysRefresh;
			}
			set
			{
				this.alwaysRefresh = value;
			}
		}

		private static TimeSpan MailboxServerCacheAbsoluteTimeout
		{
			get
			{
				if (HttpProxySettings.MailboxServerLocatorSharedCacheEnabled.Value)
				{
					return MailboxServerCache.MailboxServerCacheAbsoluteTimeoutWithSharedCache.Value;
				}
				return MailboxServerCache.MailboxServerCacheAbsoluteTimeoutInMemoryCache.Value;
			}
		}

		public bool TryGet(Guid database, IRequestContext requestContext, out BackEndServer backEndServer)
		{
			if (requestContext == null)
			{
				throw new ArgumentNullException("requestContext");
			}
			backEndServer = null;
			PerfCounters.HttpProxyCacheCountersInstance.BackEndServerLocalCacheHitsRateBase.Increment();
			PerfCounters.HttpProxyCacheCountersInstance.BackEndServerOverallCacheHitsRateBase.Increment();
			PerfCounters.IncrementMovingPercentagePerformanceCounterBase(PerfCounters.HttpProxyCacheCountersInstance.MovingPercentageBackEndServerLocalCacheHitsRate);
			PerfCounters.IncrementMovingPercentagePerformanceCounterBase(PerfCounters.HttpProxyCacheCountersInstance.MovingPercentageBackEndServerOverallCacheHitsRate);
			MailboxServerCacheEntry mailboxServerCacheEntry = null;
			bool flag = false;
			bool flag2 = this.serversCache.TryGetValue(database, out mailboxServerCacheEntry);
			if (flag2)
			{
				if (MailboxServerCache.IsE14ServerStale(mailboxServerCacheEntry))
				{
					this.Remove(database, requestContext);
					return false;
				}
				backEndServer = mailboxServerCacheEntry.BackEndServer;
				PerfCounters.HttpProxyCacheCountersInstance.BackEndServerLocalCacheHitsRate.Increment();
				PerfCounters.UpdateMovingPercentagePerformanceCounter(PerfCounters.HttpProxyCacheCountersInstance.MovingPercentageBackEndServerLocalCacheHitsRate);
				if (mailboxServerCacheEntry.IsDueForRefresh(MailboxServerCache.GetRefreshInterval(backEndServer)))
				{
					flag = true;
				}
			}
			if (HttpProxySettings.MailboxServerLocatorSharedCacheEnabled.Value && (!flag2 || flag))
			{
				MailboxServerCacheEntry sharedCacheEntry = null;
				long latency = 0L;
				string diagInfo = null;
				bool latency2 = LatencyTracker.GetLatency<bool>(() => this.sharedCacheClient.TryGet<MailboxServerCacheEntry>(database.ToString(), requestContext.ActivityId, out sharedCacheEntry, out diagInfo), out latency);
				requestContext.LogSharedCacheCall(latency, diagInfo);
				if (latency2 && (!flag2 || sharedCacheEntry.LastRefreshTime > mailboxServerCacheEntry.LastRefreshTime))
				{
					this.Add(database, sharedCacheEntry, requestContext, false);
					mailboxServerCacheEntry = sharedCacheEntry;
					flag2 = true;
				}
			}
			if (flag2)
			{
				backEndServer = mailboxServerCacheEntry.BackEndServer;
				PerfCounters.HttpProxyCacheCountersInstance.BackEndServerOverallCacheHitsRate.Increment();
				PerfCounters.UpdateMovingPercentagePerformanceCounter(PerfCounters.HttpProxyCacheCountersInstance.MovingPercentageBackEndServerOverallCacheHitsRate);
				if (!this.LazyRefreshDisabled && mailboxServerCacheEntry.IsDueForRefresh(MailboxServerCache.GetRefreshInterval(mailboxServerCacheEntry.BackEndServer)))
				{
					if (MailboxServerCache.BackgroundServerRefreshEnabled.Value)
					{
						this.RegisterRefresh(new DatabaseWithForest(database, mailboxServerCacheEntry.ResourceForest, requestContext.ActivityId));
					}
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(requestContext.Logger, "ServerLocatorRefresh", database);
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(requestContext.Logger, "RefreshingCacheEntry", mailboxServerCacheEntry.ToString());
				}
			}
			return flag2;
		}

		public BackEndServer ServerLocatorEndGetServer(MailboxServerLocator locator, IAsyncResult asyncResult, IRequestContext requestContext)
		{
			return this.ServerLocatorEndGetServer(locator, asyncResult, requestContext.ActivityId);
		}

		public BackEndServer ServerLocatorEndGetServer(MailboxServerLocator locator, IAsyncResult asyncResult, Guid initiatingRequestId)
		{
			if (locator == null)
			{
				throw new ArgumentNullException("locator");
			}
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			BackEndServer result = locator.EndGetServer(asyncResult);
			this.PopulateCache(locator.AvailabilityGroupServers, locator.ResourceForestFqdn, initiatingRequestId, locator.IsSourceCachedData, this.AlwaysRefresh || !locator.IsSourceCachedData);
			return result;
		}

		public void Remove(Guid database, IRequestContext requestContext)
		{
			this.serversCache.Remove(database);
			if (HttpProxySettings.MailboxServerLocatorSharedCacheEnabled.Value)
			{
				long latency = 0L;
				string diagInfo = null;
				LatencyTracker.GetLatency<bool>(() => this.sharedCacheClient.TryRemove(database.ToString(), requestContext.ActivityId, out diagInfo), out latency);
				if (requestContext != null)
				{
					requestContext.LogSharedCacheCall(latency, diagInfo);
				}
			}
		}

		public BackEndServer GetRandomE15Server(IRequestContext requestContext)
		{
			BackEndServer backEndServer = LocalSiteMailboxServerCache.Instance.TryGetRandomE15Server(requestContext);
			if (backEndServer != null)
			{
				return backEndServer;
			}
			ExTraceGlobals.VerboseTracer.TraceDebug((long)this.GetHashCode(), "[MailboxServerCache::GetRandomE15Server]: Could not get any available server from local site E15 server list. Will query ServiceDiscovery.");
			return HttpProxyBackEndHelper.GetAnyBackEndServer();
		}

		public void Add(Guid database, BackEndServer server, string resourceForestFqdn, bool isSourceCachedData, Guid initiatingRequestId)
		{
			MailboxServerCacheEntry entry = new MailboxServerCacheEntry(server, resourceForestFqdn, DateTime.UtcNow, isSourceCachedData);
			this.Add(database, entry, initiatingRequestId, null, true);
		}

		public void Add(Guid database, BackEndServer server, string resourceForestFqdn, bool isSourceCachedData, IRequestContext requestContext)
		{
			MailboxServerCacheEntry entry = new MailboxServerCacheEntry(server, resourceForestFqdn, DateTime.UtcNow, isSourceCachedData);
			this.Add(database, entry, requestContext.ActivityId, requestContext, true);
		}

		internal bool TryGetCacheLastRefreshTimeForTest(Guid database, out ExDateTime lastRefreshTime)
		{
			MailboxServerCacheEntry mailboxServerCacheEntry;
			if (this.serversCache.TryGetValue(database, out mailboxServerCacheEntry))
			{
				lastRefreshTime = (ExDateTime)mailboxServerCacheEntry.LastRefreshTime;
				return true;
			}
			lastRefreshTime = ExDateTime.UtcNow;
			return false;
		}

		private static bool IsLocalSiteE15MailboxServer(BackEndServer server, string resourceForest)
		{
			if (!server.IsE15OrHigher)
			{
				return false;
			}
			if ((!Utilities.IsPartnerHostedOnly && !VariantConfiguration.InvariantNoFlightingSnapshot.Cafe.NoCrossForestServerLocate.Enabled) || string.IsNullOrEmpty(resourceForest) || string.Equals(HttpProxyGlobals.LocalMachineForest.Member, resourceForest, StringComparison.OrdinalIgnoreCase))
			{
				ServiceTopology currentServiceTopology = ServiceTopology.GetCurrentServiceTopology("f:\\15.00.1497\\sources\\dev\\cafe\\src\\HttpProxy\\Cache\\MailboxServerCache.cs", "IsLocalSiteE15MailboxServer", 517);
				Site other = null;
				if (!currentServiceTopology.TryGetSite(server.Fqdn, out other))
				{
					return false;
				}
				if (HttpProxyGlobals.LocalSite.Member.Equals(other))
				{
					return true;
				}
			}
			return false;
		}

		private static bool IsE14ServerStale(MailboxServerCacheEntry cacheEntry)
		{
			return !cacheEntry.BackEndServer.IsE15OrHigher && !DownLevelServerManager.IsServerDiscoverable(cacheEntry.BackEndServer.Fqdn);
		}

		private static TimeSpan GetRefreshInterval(BackEndServer server)
		{
			if (server.IsE15OrHigher)
			{
				return MailboxServerCache.MailboxServerCacheRefreshInterval.Value;
			}
			return MailboxServerCache.MailboxServerCacheDownLevelServerRefreshInterval.Value;
		}

		private void Add(Guid database, MailboxServerCacheEntry entry, IRequestContext requestContext, bool addToSharedCache)
		{
			this.Add(database, entry, requestContext.ActivityId, requestContext, addToSharedCache);
		}

		private void Add(Guid database, MailboxServerCacheEntry entry, Guid initiatingRequestId, IRequestContext requestContext, bool addToSharedCache = true)
		{
			ExTraceGlobals.VerboseTracer.TraceDebug<Guid, BackEndServer, bool>((long)this.GetHashCode(), "[MailboxServerCache::Add]: Adding database {0} with server {1} to cache. addToSharedCache={2}", database, entry.BackEndServer, addToSharedCache);
			this.serversCache.TryInsertAbsolute(database, entry, MailboxServerCache.MailboxServerCacheAbsoluteTimeout);
			if (HttpProxySettings.MailboxServerLocatorSharedCacheEnabled.Value && addToSharedCache)
			{
				long latency = 0L;
				string diagInfo = null;
				LatencyTracker.GetLatency<bool>(() => this.sharedCacheClient.TryInsert(database.ToString(), entry, entry.LastRefreshTime, initiatingRequestId, out diagInfo), out latency);
				if (requestContext != null)
				{
					requestContext.LogSharedCacheCall(latency, diagInfo);
				}
			}
			LocalSiteMailboxServerCache.Instance.Add(database, entry.BackEndServer, entry.ResourceForest);
		}

		private void OnRemoveItem(Guid database, MailboxServerCacheEntry entry, RemoveReason reason)
		{
			LocalSiteMailboxServerCache.Instance.Remove(database);
			this.UpdateSizeCounter();
		}

		private void UpdateSizeCounter()
		{
			if (MailboxServerCache.MailboxServerCacheSizeCounterUpdateEnabled.Value)
			{
				PerfCounters.HttpProxyCacheCountersInstance.BackEndServerCacheSize.RawValue = (long)this.serversCache.Count;
			}
		}

		private void UpdateQueueLengthCounter()
		{
			PerfCounters.HttpProxyCacheCountersInstance.BackEndServerCacheRefreshingQueueLength.RawValue = (long)this.refreshQueue.Count;
		}

		private void UpdateRefreshingStatusCounter(bool isRefreshing)
		{
			PerfCounters.HttpProxyCacheCountersInstance.BackEndServerCacheRefreshingStatus.RawValue = (isRefreshing ? 1L : 0L);
		}

		private void RegisterRefresh(DatabaseWithForest database)
		{
			lock (this.refreshLock)
			{
				ExTraceGlobals.VerboseTracer.TraceDebug<string>((long)this.GetHashCode(), "[MailboxServerCache::RegisterRefresh]: Enqueueing database {0}.", database.Database.ToString());
				this.refreshQueue.Enqueue(database);
				this.UpdateQueueLengthCounter();
				MailboxServerCache.refreshWorkerSignal.Set();
			}
		}

		private void CacheRefreshEntry(object extraData)
		{
			ExTraceGlobals.VerboseTracer.TraceDebug((long)this.GetHashCode(), "[MailboxServerCache::CacheRefreshEntry]: Refresh thread starting.");
			for (;;)
			{
				try
				{
					this.UpdateRefreshingStatusCounter(true);
					Diagnostics.Logger.LogEvent(FrontEndHttpProxyEventLogConstants.Tuple_RefreshingBackEndServerCache, null, new object[]
					{
						HttpProxyGlobals.ProtocolType
					});
					this.InternalRefresh();
				}
				catch (Exception exception)
				{
					Diagnostics.ReportException(exception, FrontEndHttpProxyEventLogConstants.Tuple_InternalServerError, null, "Exception from CacheRefreshEntry: {0}");
				}
				finally
				{
					this.UpdateRefreshingStatusCounter(false);
				}
				try
				{
					MailboxServerCache.refreshWorkerSignal.WaitOne();
					continue;
				}
				catch (AbandonedMutexException)
				{
				}
				break;
			}
		}

		private void InternalRefresh()
		{
			for (;;)
			{
				DatabaseWithForest databaseWithForest;
				lock (this.refreshLock)
				{
					if (this.refreshQueue.Count == 0)
					{
						break;
					}
					databaseWithForest = this.refreshQueue.Dequeue();
					this.UpdateQueueLengthCounter();
				}
				if (this.IsDueForRefresh(databaseWithForest.Database))
				{
					this.RefreshDatabase(databaseWithForest);
				}
			}
		}

		private void RefreshDatabase(DatabaseWithForest database)
		{
			ExTraceGlobals.VerboseTracer.TraceDebug<string>((long)this.GetHashCode(), "[MailboxServerCache::RefreshDatabase]: Refreshing cache for database {0}.", database.Database.ToString());
			Diagnostics.Logger.LogEvent(FrontEndHttpProxyEventLogConstants.Tuple_RefreshingDatabaseBackEndServer, null, new object[]
			{
				HttpProxyGlobals.ProtocolType,
				database.Database,
				database.ResourceForest
			});
			Dictionary<Guid, BackEndServer> dictionary = null;
			bool flag = true;
			try
			{
				using (MailboxServerLocator mailboxServerLocator = MailboxServerLocator.CreateWithResourceForestFqdn(database.Database, string.IsNullOrEmpty(database.ResourceForest) ? null : new Fqdn(database.ResourceForest)))
				{
					PerfCounters.HttpProxyCountersInstance.MailboxServerLocatorCalls.Increment();
					mailboxServerLocator.GetServer();
					dictionary = mailboxServerLocator.AvailabilityGroupServers;
					flag = mailboxServerLocator.IsSourceCachedData;
					PerfCounters.HttpProxyCountersInstance.MailboxServerLocatorLatency.RawValue = mailboxServerLocator.Latency;
					PerfCounters.HttpProxyCountersInstance.MailboxServerLocatorAverageLatency.IncrementBy(mailboxServerLocator.Latency);
					PerfCounters.HttpProxyCountersInstance.MailboxServerLocatorAverageLatencyBase.Increment();
					PerfCounters.UpdateMovingAveragePerformanceCounter(PerfCounters.HttpProxyCountersInstance.MovingAverageMailboxServerLocatorLatency, mailboxServerLocator.Latency);
					PerfCounters.IncrementMovingPercentagePerformanceCounterBase(PerfCounters.HttpProxyCountersInstance.MovingPercentageMailboxServerLocatorRetriedCalls);
					if (mailboxServerLocator.LocatorServiceHosts.Length > 1)
					{
						PerfCounters.HttpProxyCountersInstance.MailboxServerLocatorRetriedCalls.Increment();
						PerfCounters.UpdateMovingPercentagePerformanceCounter(PerfCounters.HttpProxyCountersInstance.MovingPercentageMailboxServerLocatorRetriedCalls);
					}
				}
			}
			catch (Exception ex)
			{
				ExTraceGlobals.VerboseTracer.TraceError<Guid, string, Exception>((long)this.GetHashCode(), "[MailboxServerCache::CacheRefreshEntry]: MailboxServerLocator throws exception when locating database {0} in forest {1}. Error: {2}", database.Database, database.ResourceForest, ex);
				Diagnostics.Logger.LogEvent(FrontEndHttpProxyEventLogConstants.Tuple_ErrorRefreshingDatabaseBackEndServer, database.Database.ToString(), new object[]
				{
					HttpProxyGlobals.ProtocolType,
					database.Database,
					database.ResourceForest,
					ex.ToString()
				});
				if (ex is ServerLocatorClientException || ex is ServerLocatorClientTransientException || ex is MailboxServerLocatorException || ex is AmServerTransientException || ex is AmServerException)
				{
					PerfCounters.HttpProxyCountersInstance.MailboxServerLocatorFailedCalls.Increment();
					PerfCounters.UpdateMovingPercentagePerformanceCounter(PerfCounters.HttpProxyCountersInstance.MovingPercentageMailboxServerLocatorFailedCalls);
				}
				else if (!(ex is DatabaseNotFoundException) && !(ex is ADTransientException) && !(ex is DataValidationException) && !(ex is DataSourceOperationException))
				{
					throw;
				}
			}
			if (dictionary != null)
			{
				this.PopulateCache(dictionary, database.ResourceForest, database.InitiatingRequestId, flag, this.AlwaysRefresh || !flag);
			}
		}

		private void PopulateCache(Dictionary<Guid, BackEndServer> servers, string resourceForest, Guid initiatingRequestId, bool isSourceCachedData, bool force)
		{
			if (servers != null)
			{
				foreach (KeyValuePair<Guid, BackEndServer> keyValuePair in servers)
				{
					if (force || this.ShouldRefresh(keyValuePair.Key, isSourceCachedData))
					{
						this.Add(keyValuePair.Key, keyValuePair.Value, resourceForest, isSourceCachedData, initiatingRequestId);
					}
				}
				this.UpdateSizeCounter();
			}
		}

		private bool ShouldRefresh(Guid database, bool isSourceCachedData)
		{
			MailboxServerCacheEntry mailboxServerCacheEntry;
			return !this.serversCache.TryGetValue(database, out mailboxServerCacheEntry) || mailboxServerCacheEntry.ShouldRefresh(MailboxServerCache.MailboxServerNonCachedEntryWithCachedEntryRefeshInterval.Value, isSourceCachedData);
		}

		private bool IsDueForRefresh(Guid database)
		{
			MailboxServerCacheEntry mailboxServerCacheEntry;
			return !this.serversCache.TryGetValue(database, out mailboxServerCacheEntry) || mailboxServerCacheEntry.IsDueForRefresh(MailboxServerCache.GetRefreshInterval(mailboxServerCacheEntry.BackEndServer));
		}

		private static readonly TimeSpanAppSettingsEntry MailboxServerCacheAbsoluteTimeoutInMemoryCache = new TimeSpanAppSettingsEntry(HttpProxySettings.Prefix("MailboxServerCache.InMemoryOnly.AbsoluteTimeout"), TimeSpanUnit.Minutes, TimeSpan.FromMinutes(1440.0), ExTraceGlobals.VerboseTracer);

		private static readonly TimeSpanAppSettingsEntry MailboxServerCacheAbsoluteTimeoutWithSharedCache = new TimeSpanAppSettingsEntry(HttpProxySettings.Prefix("MailboxServerCache.WithSharedCache.AbsoluteTimeout"), TimeSpanUnit.Minutes, TimeSpan.FromMinutes(3.0), ExTraceGlobals.VerboseTracer);

		private static readonly TimeSpanAppSettingsEntry MailboxServerCacheRefreshInterval = new TimeSpanAppSettingsEntry(HttpProxySettings.Prefix("MailboxServerCacheRefreshInterval"), TimeSpanUnit.Minutes, TimeSpan.FromMinutes(30.0), ExTraceGlobals.VerboseTracer);

		private static readonly TimeSpanAppSettingsEntry MailboxServerCacheDownLevelServerRefreshInterval = new TimeSpanAppSettingsEntry(HttpProxySettings.Prefix("MailboxServerCacheDownLevelServerRefreshInterval"), TimeSpanUnit.Minutes, TimeSpan.FromMinutes(10.0), ExTraceGlobals.VerboseTracer);

		private static readonly TimeSpanAppSettingsEntry MailboxServerNonCachedEntryWithCachedEntryRefeshInterval = new TimeSpanAppSettingsEntry(HttpProxySettings.Prefix("MailboxServerNonCachedEntryWithCachedEntryRefeshInterval"), TimeSpanUnit.Minutes, TimeSpan.FromMinutes(10.0), ExTraceGlobals.VerboseTracer);

		private static readonly IntAppSettingsEntry MailboxServerCacheMaxSize = new IntAppSettingsEntry(HttpProxySettings.Prefix("MailboxServerCacheMaxSize"), 200000, ExTraceGlobals.VerboseTracer);

		private static readonly BoolAppSettingsEntry BackgroundServerRefreshEnabled = new BoolAppSettingsEntry(HttpProxySettings.Prefix("BackgroundServerRefreshEnabled"), true, ExTraceGlobals.VerboseTracer);

		private static readonly BoolAppSettingsEntry MailboxServerCacheSizeCounterUpdateEnabled = new BoolAppSettingsEntry(HttpProxySettings.Prefix("MailboxServerCacheSizeCounterUpdateEnabled"), false, ExTraceGlobals.VerboseTracer);

		private static MailboxServerCache instance = null;

		private static object staticLock = new object();

		private static AutoResetEvent refreshWorkerSignal = new AutoResetEvent(false);

		private bool alwaysRefresh = HttpProxySettings.MaxRetryOnError.Value <= 0;

		private ExactTimeoutCache<Guid, MailboxServerCacheEntry> serversCache;

		private Queue<DatabaseWithForest> refreshQueue = new Queue<DatabaseWithForest>();

		private object refreshLock = new object();

		private SharedCacheClient sharedCacheClient;
	}
}
