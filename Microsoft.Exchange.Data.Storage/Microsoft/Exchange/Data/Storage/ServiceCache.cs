using System;
using System.Threading;
using Microsoft.Exchange.Data.Directory.ExchangeTopology;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ServiceCache
	{
		private ServiceCache()
		{
			this.cachePresentEvent = new ManualResetEvent(false);
			this.semaphore = new Semaphore(1, 1);
			this.stopTimerEvent = new AutoResetEvent(false);
			this.notificationHandler = new ADNotificationHandler();
			this.isFirstLoad = true;
			this.dropCacheOnInactivity = true;
		}

		internal static ExchangeTopologyScope Scope
		{
			get
			{
				return ServiceCache.Instance.notificationHandler.Scope;
			}
			set
			{
				ServiceCache.Instance.notificationHandler.Scope = value;
			}
		}

		internal static ServiceTopology GetCurrentLegacyServiceTopology()
		{
			return ServiceCache.GetCurrentLegacyServiceTopology(ServiceDiscovery.ExchangeTopologyBridge.GetServiceTopologyDefaultTimeout);
		}

		internal static ServiceTopology GetCurrentLegacyServiceTopology(TimeSpan getServiceTopologyTimeout)
		{
			ServiceTopology currentServiceTopology = ServiceCache.GetCurrentServiceTopology(getServiceTopologyTimeout);
			ServiceTopology serviceTopology = ServiceCache.Instance.legacyServiceTopologyInstance;
			if (serviceTopology == null)
			{
				serviceTopology = currentServiceTopology.ToLegacyServiceTopology("f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\ServiceDiscovery\\ServiceCache.cs", "GetCurrentLegacyServiceTopology", 144);
				ServiceCache.Instance.legacyServiceTopologyInstance = serviceTopology;
			}
			return serviceTopology;
		}

		internal static ServiceTopology GetCurrentServiceTopology()
		{
			return ServiceCache.GetCurrentServiceTopology(ServiceDiscovery.ExchangeTopologyBridge.GetServiceTopologyDefaultTimeout);
		}

		internal static ServiceTopology GetCurrentServiceTopology(TimeSpan getServiceTopologyTimeout)
		{
			ServiceCache instance = ServiceCache.Instance;
			instance.DropCacheIfNeeded();
			instance.TriggerCacheRefreshIfNeeded();
			ServiceTopology serviceTopology = instance.serviceTopologyInstance;
			if (serviceTopology == null)
			{
				if (!instance.cachePresentEvent.WaitOne(getServiceTopologyTimeout, false))
				{
					throw new ReadTopologyTimeoutException(ServerStrings.ExReadTopologyTimeout);
				}
				serviceTopology = instance.serviceTopologyInstance;
			}
			serviceTopology.IncrementRequestCount("f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\ServiceDiscovery\\ServiceCache.cs", "GetCurrentServiceTopology", 188);
			return serviceTopology;
		}

		internal static void TriggerCacheRefreshDueToNotification()
		{
			ServiceCache.Instance.TriggerCacheRefresh(ServiceCache.CachePopulateReason.TopologyChangeNotification, null);
		}

		internal static void TriggerCacheRefreshDueToCacheMiss(ServiceTopology requestingTopology)
		{
			Util.ThrowOnNullArgument(requestingTopology, "requestingTopology");
			ServiceCache.Instance.TriggerCacheRefresh(ServiceCache.CachePopulateReason.CacheMissDetected, requestingTopology);
		}

		internal static void Purge()
		{
			ServiceCache instance = ServiceCache.Instance;
			instance.semaphore.WaitOne();
			try
			{
				instance.DropCache();
				instance.isFirstLoad = true;
				instance.notificationHandler.UnRegisterExchangeTopologyNotification();
				instance.dropCacheOnInactivity = false;
				if (!(ServiceDiscovery.ExchangeTopologyBridge is ExchangeTopologyBridge))
				{
					instance.notificationHandler.RegisterExchangeTopologyNotificationIfNeeded();
				}
			}
			finally
			{
				instance.semaphore.Release();
			}
		}

		private void TriggerCacheRefresh(ServiceCache.CachePopulateReason reason, ServiceTopology requestingTopology)
		{
			if (requestingTopology != null)
			{
				ServiceTopology serviceTopology = this.serviceTopologyInstance;
				if (requestingTopology != serviceTopology)
				{
					return;
				}
				if (serviceTopology != null && serviceTopology.CreationTime + ServiceDiscovery.ExchangeTopologyBridge.MinExpirationTimeForCacheDueToCacheMiss > ExDateTime.UtcNow)
				{
					return;
				}
			}
			bool flag = false;
			bool flag2 = this.semaphore.WaitOne(0, false);
			try
			{
				if (flag2 && (requestingTopology == null || requestingTopology == this.serviceTopologyInstance))
				{
					this.StopCacheRefreshTimer();
					this.DropCacheIfNeeded();
					using (ActivityContext.SuppressThreadScope())
					{
						flag = ThreadPool.QueueUserWorkItem(new WaitCallback(this.PopulateServiceCache), reason);
					}
				}
			}
			finally
			{
				if (flag2 && !flag)
				{
					this.semaphore.Release();
				}
			}
		}

		private void TriggerCacheRefreshIfNeeded()
		{
			if (this.serviceTopologyInstance == null)
			{
				this.TriggerCacheRefresh(ServiceCache.CachePopulateReason.CacheNotPresent, null);
			}
		}

		private void DropCacheIfNeeded()
		{
			ServiceTopology serviceTopology = this.serviceTopologyInstance;
			ServiceTopology serviceTopology2 = this.legacyServiceTopologyInstance;
			ExDateTime utcNow = ExDateTime.UtcNow;
			ExDateTime exDateTime = ExDateTime.MaxValue;
			if (serviceTopology != null)
			{
				exDateTime = serviceTopology.CreationTime;
			}
			if (serviceTopology2 != null && serviceTopology2.CreationTime < exDateTime)
			{
				exDateTime = serviceTopology2.CreationTime;
			}
			if (exDateTime != ExDateTime.MaxValue && utcNow - exDateTime > ServiceDiscovery.ExchangeTopologyBridge.CacheExpirationTimeout)
			{
				this.DropCache();
			}
		}

		private void DropCache()
		{
			this.serviceTopologyInstance = null;
			this.legacyServiceTopologyInstance = null;
			this.cachePresentEvent.Reset();
			ExTraceGlobals.ServiceDiscoveryTracer.TraceDebug(0L, "ServiceCache::DropCache. Cached service topology instance was dropped");
		}

		private void StartCacheRefreshTimer()
		{
			this.cacheTimerRegisteredWaitHandle = ThreadPool.RegisterWaitForSingleObject(this.stopTimerEvent, new WaitOrTimerCallback(this.TimerCallback), null, ServiceDiscovery.ExchangeTopologyBridge.CacheTimerRefreshTimeout, true);
		}

		private void StopCacheRefreshTimer()
		{
			if (this.cacheTimerRegisteredWaitHandle != null)
			{
				this.cacheTimerRegisteredWaitHandle.Unregister(null);
			}
		}

		private void PopulateServiceCache(object obj)
		{
			ServiceCache.CachePopulateReason cachePopulateReason = (ServiceCache.CachePopulateReason)obj;
			Exception ex = null;
			try
			{
				try
				{
					DateTime dateTime = (cachePopulateReason == ServiceCache.CachePopulateReason.CacheMissDetected || this.serviceTopologyInstance == null) ? DateTime.MinValue : this.serviceTopologyInstance.DiscoveryStarted;
					bool forceRefresh = false;
					if (this.dropCacheOnInactivity && dateTime != DateTime.MinValue && this.serviceTopologyInstance.TopologyRequestCount <= 1)
					{
						this.DropCache();
						return;
					}
					if (cachePopulateReason == ServiceCache.CachePopulateReason.CacheMissDetected || (cachePopulateReason == ServiceCache.CachePopulateReason.CacheNotPresent && this.isFirstLoad))
					{
						forceRefresh = true;
					}
					ExchangeTopology exchangeTopology = ServiceDiscovery.ExchangeTopologyBridge.ReadExchangeTopology(dateTime, ExchangeTopologyScope.Complete, forceRefresh);
					if (exchangeTopology != null)
					{
						this.serviceTopologyInstance = new ServiceTopology(exchangeTopology, "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\ServiceDiscovery\\ServiceCache.cs", "PopulateServiceCache", 426);
					}
					else if (this.serviceTopologyInstance != null)
					{
						this.serviceTopologyInstance.CreationTime = ExDateTime.Now;
					}
					this.isFirstLoad = false;
					if (this.serviceTopologyInstance != null)
					{
						this.cachePresentEvent.Set();
					}
					else
					{
						ExTraceGlobals.ServiceDiscoveryTracer.TraceDebug(0L, "ServiceCache::ServiceDiscovery.ExchangeTopologyBridge.ReadExchangeTopology returned null. This is unexpected");
					}
				}
				catch (ServiceDiscoveryPermanentException ex2)
				{
					ex = ex2;
				}
				catch (ServiceDiscoveryTransientException ex3)
				{
					ex = ex3;
				}
				this.StartCacheRefreshTimer();
			}
			finally
			{
				this.semaphore.Release();
			}
			if (ex == null)
			{
				ExTraceGlobals.ServiceDiscoveryTracer.TraceDebug<ServiceCache.CachePopulateReason>(0L, "ServiceCache::PopulateServiceCache. Successfully populated ServiceTopology. Reason to populate cache = {0}", cachePopulateReason);
				ProcessInfoEventLogger.Log(StorageEventLogConstants.Tuple_PopulatedServiceTopology, null, new object[]
				{
					cachePopulateReason
				});
				return;
			}
			string text = ex.ToString().TruncateToUseInEventLog();
			ExTraceGlobals.ServiceDiscoveryTracer.TraceError<ServiceCache.CachePopulateReason, string>(0L, "ServiceCache::PopulateServiceCache. Failed to populate a ServiceTopology. Reason to populate cache = {0}. Error = {1}.", cachePopulateReason, text);
			ProcessInfoEventLogger.Log(StorageEventLogConstants.Tuple_ErrorPopulatingServiceTopology, null, new object[]
			{
				cachePopulateReason,
				text
			});
		}

		private void TimerCallback(object state, bool timedOut)
		{
			if (timedOut)
			{
				this.TriggerCacheRefresh(ServiceCache.CachePopulateReason.RefreshTimeout, null);
			}
		}

		private static readonly ServiceCache Instance = new ServiceCache();

		private readonly ManualResetEvent cachePresentEvent;

		private readonly Semaphore semaphore;

		private readonly AutoResetEvent stopTimerEvent;

		private readonly ADNotificationHandler notificationHandler;

		private RegisteredWaitHandle cacheTimerRegisteredWaitHandle;

		private ServiceTopology serviceTopologyInstance;

		private ServiceTopology legacyServiceTopologyInstance;

		private bool isFirstLoad;

		private bool dropCacheOnInactivity;

		internal enum CachePopulateReason
		{
			CacheNotPresent,
			RefreshTimeout,
			TopologyChangeNotification,
			CacheMissDetected
		}
	}
}
