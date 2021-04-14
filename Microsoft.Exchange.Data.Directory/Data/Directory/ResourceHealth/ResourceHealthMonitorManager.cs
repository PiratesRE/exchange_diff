using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security;
using System.Threading;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Exchange.WorkloadManagement;
using Microsoft.Win32;

namespace Microsoft.Exchange.Data.Directory.ResourceHealth
{
	internal class ResourceHealthMonitorManager : IDisposable
	{
		static ResourceHealthMonitorManager()
		{
			ResourceHealthMonitorManager.Active = false;
		}

		private static void CheckRegistryActive()
		{
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\MSExchange ResourceHealth", false))
				{
					if (registryKey == null)
					{
						ResourceHealthMonitorManager.Active = false;
					}
					else
					{
						object value = registryKey.GetValue(ResourceHealthMonitorManager.resourceHealthComponent.ToString());
						if (value == null || !(value is int))
						{
							ResourceHealthMonitorManager.Active = false;
						}
						else
						{
							ResourceHealthMonitorManager.Active = ((int)value != 0);
						}
					}
				}
			}
			catch (SecurityException)
			{
				ExTraceGlobals.ResourceHealthManagerTracer.TraceError<ResourceHealthComponent>(0L, "[ResourceHealthMonitorManager::CheckRegistryActive] Security exception encountered while retrieving {0} registry value.", ResourceHealthMonitorManager.resourceHealthComponent);
			}
			catch (UnauthorizedAccessException)
			{
				ExTraceGlobals.ResourceHealthManagerTracer.TraceError<ResourceHealthComponent>(0L, "[ResourceHealthMonitorManagaer::CheckRegistryActive] Security exception encountered while retrieving {0} registry value", ResourceHealthMonitorManager.resourceHealthComponent);
			}
		}

		public static void Initialize(ResourceHealthComponent resourceHealthComponent)
		{
			if (resourceHealthComponent == ResourceHealthComponent.None)
			{
				throw new ArgumentException("You should not use ResourceHealthComponent.None when initializing the ResourceHealthMonitorManager.");
			}
			if (!ResourceHealthMonitorManager.Initialized)
			{
				ResourceHealthMonitorManager.resourceHealthComponent = resourceHealthComponent;
				ResourceHealthMonitorManager.CheckRegistryActive();
				return;
			}
			if (resourceHealthComponent != ResourceHealthMonitorManager.resourceHealthComponent)
			{
				throw new InvalidOperationException("ResourceHealthMonitorManager was already initialized with budget type: " + resourceHealthComponent);
			}
		}

		public static bool Initialized
		{
			get
			{
				return ResourceHealthMonitorManager.resourceHealthComponent != ResourceHealthComponent.None;
			}
		}

		public static ResourceHealthMonitorManager Singleton
		{
			get
			{
				return ResourceHealthMonitorManager.singleton;
			}
		}

		internal List<ResourceKey> ResourceKeys
		{
			get
			{
				return this.monitors.Keys;
			}
		}

		private ResourceHealthMonitorManager()
		{
			this.monitors = new ExactTimeoutCache<ResourceKey, CacheableResourceHealthMonitor>(new RemoveItemDelegate<ResourceKey, CacheableResourceHealthMonitor>(this.HandleMonitorRemove), new ShouldRemoveDelegate<ResourceKey, CacheableResourceHealthMonitor>(this.HandleResourceCacheShouldRemove), new UnhandledExceptionDelegate(this.HandleTimeoutCacheWorkerException), 100000, true);
			this.pollingMonitors = new ExactTimeoutCache<ResourceKey, IResourceHealthPoller>(null, new ShouldRemoveDelegate<ResourceKey, IResourceHealthPoller>(this.HandlePollingItemShouldRemove), new UnhandledExceptionDelegate(this.HandleTimeoutCacheWorkerException), 100000, false);
		}

		~ResourceHealthMonitorManager()
		{
			this.Dispose(false);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool fromDispose)
		{
			if (!this.disposed && fromDispose)
			{
				lock (this.instanceLock)
				{
					this.monitors.Dispose();
					this.monitors = null;
					this.pollingMonitors.Dispose();
					this.pollingMonitors = null;
				}
				this.disposed = true;
			}
		}

		public static bool Active { get; private set; }

		internal static void SetActiveForTest(bool active)
		{
			ResourceHealthMonitorManager.Active = active;
		}

		internal static void UpdateActiveFromRegistry()
		{
			if (!ResourceHealthMonitorManager.Initialized)
			{
				throw new InvalidOperationException("ResourceHealthMonitorManager must first be initialized.");
			}
			ResourceHealthMonitorManager.CheckRegistryActive();
		}

		internal static TimeSpan MonitorSlidingExpiration { get; set; } = TimeSpan.FromMinutes(5.0);

		internal static TimeSpan DummyMonitorAbsoluteExpiration { get; set; } = TimeSpan.FromMinutes(1.0);

		public IResourceLoadMonitor Get(ResourceKey key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			CacheableResourceHealthMonitor cacheableResourceHealthMonitor = null;
			if (!this.monitors.TryGetValue(key, out cacheableResourceHealthMonitor))
			{
				bool flag = false;
				bool flag2 = false;
				bool flag3 = true;
				CacheableResourceHealthMonitor cacheableResourceHealthMonitor2 = null;
				try
				{
					if (ResourceHealthMonitorManager.Active && ExTraceGlobals.ResourceHealthManagerTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						StackTrace arg = new StackTrace();
						ExTraceGlobals.ResourceHealthManagerTracer.TraceDebug<ResourceHealthComponent, ResourceKey, StackTrace>((long)this.GetHashCode(), "[ResourceHealthMonitorManager.Get] Creating new monitor for component '{0}'.  Resource Identity: {1}, Stack Trace: {2}", ResourceHealthMonitorManager.resourceHealthComponent, key, arg);
					}
					cacheableResourceHealthMonitor2 = ((ResourceHealthMonitorManager.Active && key.MetricType != ResourceMetricType.None) ? key.CreateMonitor() : new DummyResourceHealthMonitor(key));
					lock (this.instanceLock)
					{
						if (!this.monitors.TryGetValue(key, out cacheableResourceHealthMonitor))
						{
							flag3 = false;
							flag2 = true;
							if (cacheableResourceHealthMonitor2 is DummyResourceHealthMonitor)
							{
								ExTraceGlobals.ResourceHealthManagerTracer.TraceDebug<ResourceKey>((long)this.GetHashCode(), "[ResourceHealthMonitorManager.Get] Dummy monitor created for resource key {0}", key);
								this.monitors.TryAddAbsolute(key, cacheableResourceHealthMonitor2, ResourceHealthMonitorManager.DummyMonitorAbsoluteExpiration);
							}
							else
							{
								this.monitors.TryAddAbsolute(key, cacheableResourceHealthMonitor2, ResourceHealthMonitorManager.MonitorSlidingExpiration);
							}
							cacheableResourceHealthMonitor = cacheableResourceHealthMonitor2;
							flag = true;
						}
					}
				}
				finally
				{
					if (flag3 && cacheableResourceHealthMonitor2 != null)
					{
						cacheableResourceHealthMonitor2.Expire();
						IDisposable disposable = cacheableResourceHealthMonitor2 as IDisposable;
						if (disposable != null)
						{
							disposable.Dispose();
						}
						cacheableResourceHealthMonitor2 = null;
					}
				}
				if (flag2)
				{
					ExTraceGlobals.ResourceHealthManagerTracer.TraceDebug<ResourceKey>((long)this.GetHashCode(), "[ResourceHealthMonitorManager::Get] Cache miss for key '{0}'", key);
				}
				if (flag)
				{
					IResourceHealthPoller pollHealth = cacheableResourceHealthMonitor as IResourceHealthPoller;
					if (pollHealth != null)
					{
						using (ActivityContext.SuppressThreadScope())
						{
							ThreadPool.QueueUserWorkItem(delegate(object state)
							{
								this.HandlePollingItemShouldRemove(key, pollHealth);
							});
						}
						ExTraceGlobals.ClientThrottlingTracer.TraceDebug<ResourceKey>((long)this.GetHashCode(), "[ResourceHealthMonitorManager::Get] Adding monitor as polling monitor: {0}", key);
						this.pollingMonitors.TryAddAbsolute(key, pollHealth, pollHealth.Interval);
					}
				}
			}
			return cacheableResourceHealthMonitor.CreateWrapper();
		}

		public void Remove(ResourceKey resourceKey)
		{
			lock (this.instanceLock)
			{
				this.monitors.Remove(resourceKey);
				this.pollingMonitors.Remove(resourceKey);
			}
		}

		internal void Clear()
		{
			lock (this.instanceLock)
			{
				this.monitors.Clear();
				this.pollingMonitors.Clear();
			}
		}

		internal bool IsCached(ResourceKey resourceKey)
		{
			return this.monitors.Contains(resourceKey);
		}

		private bool HandleResourceCacheShouldRemove(ResourceKey key, CacheableResourceHealthMonitor monitor)
		{
			bool flag = false;
			bool result;
			try
			{
				if (monitor is IResourceHealthPoller)
				{
					if (!Monitor.TryEnter(monitor, 0))
					{
						ExTraceGlobals.ResourceHealthManagerTracer.TraceDebug<ResourceKey>((long)this.GetHashCode(), "[ResourceHealthMonitorManager.HandleResourceCacheShouldRemove] Could not grab the monitor - extending life in cache for key: {0}", key);
						return false;
					}
					flag = true;
				}
				bool flag2 = true;
				if (monitor != null)
				{
					flag2 = (DateTime.UtcNow - monitor.LastAccessUtc > ResourceHealthMonitorManager.MonitorSlidingExpiration && monitor.ShouldRemoveResourceFromCache());
				}
				if (flag2)
				{
					this.pollingMonitors.Remove(key);
					monitor.Expire();
				}
				result = flag2;
			}
			finally
			{
				if (flag)
				{
					Monitor.Exit(monitor);
				}
			}
			return result;
		}

		private void HandleTimeoutCacheWorkerException(Exception unhandledException)
		{
			ExTraceGlobals.ResourceHealthManagerTracer.TraceError<Exception>((long)this.GetHashCode(), "[ResourceHealthMonitorManager.HandleTimeoutCacheWorkerException] Encountered exception on timeout cache worker thread.  Exception: {0}", unhandledException);
			if (!(unhandledException is ThreadAbortException) && !(unhandledException is AppDomainUnloadedException))
			{
				ExWatson.SendReport(unhandledException);
			}
		}

		private bool HandlePollingItemShouldRemove(ResourceKey key, IResourceHealthPoller value)
		{
			ExTraceGlobals.ResourceHealthManagerTracer.TraceDebug<ResourceKey>((long)this.GetHashCode(), "[ResourceHealthMonitorManager.HandlePollingItemShouldRemove] Firing poll for monitor {0}", key);
			if (!VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).WorkloadManagement.GetObject<IResourceSettings>(key.MetricType, new object[0]).Enabled)
			{
				return false;
			}
			CacheableResourceHealthMonitor cacheableResourceHealthMonitor = value as CacheableResourceHealthMonitor;
			if (cacheableResourceHealthMonitor.Expired)
			{
				ExTraceGlobals.ResourceHealthManagerTracer.TraceDebug<ResourceKey>((long)this.GetHashCode(), "[ResourceHealthMonitorManager.HandlePollingItemShouldRemove] Monitor has already expired - will not poll for key: {0}", key);
				return false;
			}
			bool flag = false;
			try
			{
				if (!Monitor.TryEnter(value, 0))
				{
					ExTraceGlobals.ResourceHealthManagerTracer.TraceDebug<ResourceKey>((long)this.GetHashCode(), "[ResourceHealthMonitorManager.HandlePollingItemShouldRemove] Could not obtain lock.  Will not poll for key: {0}", key);
					return false;
				}
				flag = true;
				if (cacheableResourceHealthMonitor.Expired)
				{
					return false;
				}
				if (!value.IsActive)
				{
					return false;
				}
				bool flag2 = false;
				try
				{
					lock (this.instanceLock)
					{
						if (!this.outstandingPollCalls.ContainsKey(key))
						{
							this.outstandingPollCalls[key] = true;
							flag2 = true;
						}
					}
					if (flag2)
					{
						try
						{
							value.Execute();
						}
						catch (DataSourceTransientException ex)
						{
							ExTraceGlobals.ResourceHealthManagerTracer.TraceError<ResourceKey, string>((long)this.GetHashCode(), "[ResourceHealthMonitorManager.HandlePollingItemShouldRemove] Failed to set metrics for monitor {0}. Exception: {1}", key, ex.Message);
							return false;
						}
					}
				}
				finally
				{
					if (flag2)
					{
						lock (this.instanceLock)
						{
							this.outstandingPollCalls.Remove(key);
						}
					}
				}
			}
			finally
			{
				if (flag)
				{
					Monitor.Exit(value);
				}
			}
			return false;
		}

		private void HandleMonitorRemove(ResourceKey key, CacheableResourceHealthMonitor value, RemoveReason reason)
		{
			ExTraceGlobals.ResourceHealthManagerTracer.TraceDebug<ResourceKey, RemoveReason>((long)this.GetHashCode(), "[ResourceHealthMonitorManager.HandleMonitorRemove] Removing monitor {0} due to reason {1}", key, reason);
			if (!value.Expired)
			{
				value.Expire();
			}
			IDisposable disposable = value as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}

		public const string MSExchangeResourceHealthRegistryPath = "SYSTEM\\CurrentControlSet\\Services\\MSExchange ResourceHealth";

		private static ResourceHealthComponent resourceHealthComponent = ResourceHealthComponent.None;

		private static ResourceHealthMonitorManager singleton = new ResourceHealthMonitorManager();

		private ExactTimeoutCache<ResourceKey, CacheableResourceHealthMonitor> monitors;

		private ExactTimeoutCache<ResourceKey, IResourceHealthPoller> pollingMonitors;

		private object instanceLock = new object();

		private bool disposed;

		private Dictionary<ResourceKey, bool> outstandingPollCalls = new Dictionary<ResourceKey, bool>();
	}
}
