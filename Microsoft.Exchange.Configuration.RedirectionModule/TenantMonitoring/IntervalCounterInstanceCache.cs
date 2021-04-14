using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using Microsoft.Exchange.Configuration.RedirectionModule;
using Microsoft.Exchange.Configuration.RedirectionModule.EventLog;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.RedirectionModule;
using Microsoft.Exchange.Threading;

namespace Microsoft.Exchange.Configuration.TenantMonitoring
{
	internal static class IntervalCounterInstanceCache
	{
		static IntervalCounterInstanceCache()
		{
			int intValueFromRegistry = Globals.GetIntValueFromRegistry("SYSTEM\\CurrentControlSet\\Services\\MSExchange Tenant Monitoring", "UpdateIntervalSeconds", 1200, 0);
			int intValueFromRegistry2 = Globals.GetIntValueFromRegistry("SYSTEM\\CurrentControlSet\\Services\\MSExchange Tenant Monitoring", "InactivityTimeoutSeconds", 3600, 0);
			IntervalCounterInstanceCache.updateInterval = TimeSpan.FromSeconds((double)intValueFromRegistry);
			IntervalCounterInstanceCache.inactivityTimeout = TimeSpan.FromSeconds((double)intValueFromRegistry2);
			if (PerformanceCounterCategory.Exists("MSExchangeTenantMonitoring"))
			{
				IntervalCounterInstanceCache.counterUpdateTimer = new Timer(IntervalCounterInstanceCache.updateInterval.TotalMilliseconds);
				IntervalCounterInstanceCache.counterUpdateTimer.AutoReset = true;
				IntervalCounterInstanceCache.counterUpdateTimer.Elapsed += IntervalCounterInstanceCache.UpdateCounters;
				AppDomain.CurrentDomain.DomainUnload += IntervalCounterInstanceCache.ApplicationDomainUnload;
				IntervalCounterInstanceCache.counterUpdateTimer.Start();
			}
		}

		public static TimeSpan UpdateInterval
		{
			get
			{
				return IntervalCounterInstanceCache.updateInterval;
			}
			set
			{
				IntervalCounterInstanceCache.updateInterval = value;
				if (IntervalCounterInstanceCache.counterUpdateTimer != null)
				{
					IntervalCounterInstanceCache.counterUpdateTimer.Stop();
					IntervalCounterInstanceCache.counterUpdateTimer.Interval = IntervalCounterInstanceCache.updateInterval.TotalMilliseconds;
					IntervalCounterInstanceCache.counterUpdateTimer.Start();
				}
			}
		}

		public static TimeSpan InactivityTimeout
		{
			get
			{
				return IntervalCounterInstanceCache.inactivityTimeout;
			}
			set
			{
				IntervalCounterInstanceCache.inactivityTimeout = value;
			}
		}

		public static void ApplicationDomainUnload(object sender, EventArgs e)
		{
			if (IntervalCounterInstanceCache.counterUpdateTimer != null)
			{
				IntervalCounterInstanceCache.counterUpdateTimer.Stop();
				IntervalCounterInstanceCache.counterUpdateTimer.Dispose();
				IntervalCounterInstanceCache.counterUpdateTimer = null;
			}
		}

		internal static void IncrementIntervalCounter(string instanceName, CounterType counterType)
		{
			IntervalCounterInstance instance = IntervalCounterInstanceCache.GetInstance(instanceName);
			if (instance == null)
			{
				return;
			}
			instance.Increment(counterType);
		}

		private static void UpdateCounters(object source, ElapsedEventArgs e)
		{
			ExTraceGlobals.TenantMonitoringTracer.TraceFunction(0L, "Enter UpdateCounters.");
			List<string> list = new List<string>(16);
			try
			{
				IntervalCounterInstanceCache.readerWriterLock.AcquireReaderLock(5000);
			}
			catch (TimeoutException ex)
			{
				IntervalCounterInstanceCache.LogReaderWriterLockTimeoutEvent(ex, "IntervalCounterInstanceCache.UpdateCounters.AcquireReaderLock");
				ExTraceGlobals.TenantMonitoringTracer.TraceFunction(0L, "Exit UpdateCounters.");
				return;
			}
			try
			{
				foreach (KeyValuePair<string, IntervalCounterInstance> keyValuePair in IntervalCounterInstanceCache.instanceDictionary)
				{
					ExTraceGlobals.TenantMonitoringTracer.Information<string, DateTime, TimeSpan>(0L, "Update counter for key {0}. Last Update {1}. Current inactivityTimeout {2}", keyValuePair.Key, keyValuePair.Value.LastUpdateTime, IntervalCounterInstanceCache.inactivityTimeout);
					if (keyValuePair.Value.LastUpdateTime + IntervalCounterInstanceCache.inactivityTimeout < DateTime.UtcNow)
					{
						list.Add(keyValuePair.Key);
					}
					keyValuePair.Value.CalculateIntervalDataAndUpdateCounters(keyValuePair.Key);
				}
			}
			finally
			{
				IntervalCounterInstanceCache.readerWriterLock.ReleaseReaderLock();
			}
			if (list.Count > 0)
			{
				try
				{
					IntervalCounterInstanceCache.readerWriterLock.AcquireWriterLock(5000);
				}
				catch (TimeoutException ex2)
				{
					IntervalCounterInstanceCache.LogReaderWriterLockTimeoutEvent(ex2, "IntervalCounterInstanceCache.UpdateCounters.AcquireWriterLock");
					ExTraceGlobals.TenantMonitoringTracer.TraceFunction(0L, "Exit UpdateCounters.");
					return;
				}
				try
				{
					foreach (string text in list)
					{
						ExTraceGlobals.TenantMonitoringTracer.Information<string>(0L, "Remove entry {0} from cache.", text);
						IntervalCounterInstanceCache.instanceDictionary.Remove(text);
						MSExchangeTenantMonitoring.RemoveInstance(text);
					}
				}
				finally
				{
					IntervalCounterInstanceCache.readerWriterLock.ReleaseWriterLock();
				}
			}
			ExTraceGlobals.TenantMonitoringTracer.TraceFunction(0L, "Exit UpdateCounters.");
		}

		private static IntervalCounterInstance GetInstance(string instanceName)
		{
			try
			{
				IntervalCounterInstanceCache.readerWriterLock.AcquireReaderLock(5000);
			}
			catch (TimeoutException ex)
			{
				IntervalCounterInstanceCache.LogReaderWriterLockTimeoutEvent(ex, "IntervalCounterInstanceCache.GetInstance.AcquireReaderLock");
				return null;
			}
			IntervalCounterInstance intervalCounterInstance;
			try
			{
				IntervalCounterInstanceCache.instanceDictionary.TryGetValue(instanceName, out intervalCounterInstance);
			}
			finally
			{
				IntervalCounterInstanceCache.readerWriterLock.ReleaseReaderLock();
			}
			if (intervalCounterInstance == null)
			{
				try
				{
					IntervalCounterInstanceCache.readerWriterLock.AcquireWriterLock(5000);
				}
				catch (TimeoutException ex2)
				{
					IntervalCounterInstanceCache.LogReaderWriterLockTimeoutEvent(ex2, "IntervalCounterInstanceCache.GetInstance.AcquireWriterLock");
					return null;
				}
				try
				{
					if (!IntervalCounterInstanceCache.instanceDictionary.TryGetValue(instanceName, out intervalCounterInstance))
					{
						ExTraceGlobals.TenantMonitoringTracer.Information<string>(0L, "Create perf counter instance for {0}.", instanceName);
						intervalCounterInstance = new IntervalCounterInstance();
						IntervalCounterInstanceCache.instanceDictionary[instanceName] = intervalCounterInstance;
					}
				}
				finally
				{
					IntervalCounterInstanceCache.readerWriterLock.ReleaseWriterLock();
				}
			}
			return intervalCounterInstance;
		}

		private static void LogReaderWriterLockTimeoutEvent(TimeoutException ex, string methodName)
		{
			ExTraceGlobals.TenantMonitoringTracer.TraceError<string, TimeoutException>(0L, "Method {0} throws TimeoutException {1}.", methodName, ex);
			Logger.LogEvent(IntervalCounterInstanceCache.eventLogger, TaskEventLogConstants.Tuple_ReaderWriterLock_Timeout, methodName, new object[]
			{
				ex
			});
		}

		private const int LockTimeoutInMilliseconds = 5000;

		private static readonly ExEventLog eventLogger = new ExEventLog(ExTraceGlobals.RedirectionTracer.Category, "MSExchange LiveId Redirection Module");

		private static Dictionary<string, IntervalCounterInstance> instanceDictionary = new Dictionary<string, IntervalCounterInstance>(100, StringComparer.OrdinalIgnoreCase);

		private static FastReaderWriterLock readerWriterLock = new FastReaderWriterLock();

		private static Timer counterUpdateTimer;

		private static TimeSpan updateInterval;

		private static TimeSpan inactivityTimeout;
	}
}
