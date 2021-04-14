using System;
using System.Diagnostics;
using Microsoft.Exchange.DxStore.HA.Events;
using Microsoft.Exchange.Threading;

namespace Microsoft.Exchange.Cluster.Shared
{
	public class PerformanceTracker
	{
		public PerformanceTracker()
		{
			this.apiExecutionPeriodicLogDuration = TimeSpan.FromMilliseconds((double)RegistryParameters.DistributedStoreApiExecutionPeriodicLogDurationInMs);
			this.CurrentProcessName = Process.GetCurrentProcess().ProcessName;
		}

		public string CurrentProcessName { get; private set; }

		public void UpdateStart(DistributedStoreKey key, StoreKind storeKind, bool isPrimary)
		{
			lock (this.locker)
			{
				PerformanceEntry orAdd = this.GetOrAdd(storeKind, isPrimary);
				orAdd.RecordStart();
			}
		}

		public void LogExecution(DistributedStoreKey key, StoreKind storeKind, bool isPrimary, RequestInfo req, long latencyInMs, Exception exception, bool isSkipped)
		{
			if (!isPrimary && !RegistryParameters.DistributedStoreIsLogShadowApiResult)
			{
				return;
			}
			bool flag = exception == null && !isSkipped;
			string text = req.OperationCategory.ToString();
			string text2 = (key != null) ? key.FullKeyName : string.Empty;
			string debugStr = req.DebugStr;
			string text3 = (key != null) ? key.InstanceId.ToString() : string.Empty;
			string text4 = string.Empty;
			if (RegistryParameters.DistributedStoreIsLogApiExecutionCallstack)
			{
				text4 = new StackTrace(3, true).ToString();
			}
			if (flag)
			{
				if (RegistryParameters.DistributedStoreIsLogApiSuccess)
				{
					if (this.apiExecutionPeriodicLogDuration != TimeSpan.Zero)
					{
						DxStoreHACrimsonEvents.ApiExecutionSuccess.LogPeriodic<string, bool, StoreKind, bool, string, OperationType, string, long, string, bool, string, string, string, string>(text, this.apiExecutionPeriodicLogDuration, text, isPrimary, storeKind, true, text2, req.OperationType, req.InitiatedTime.ToString("o"), latencyInMs, string.Empty, false, debugStr, text3, text4, this.CurrentProcessName);
						return;
					}
					DxStoreHACrimsonEvents.ApiExecutionSuccess.Log<string, bool, StoreKind, bool, string, OperationType, string, long, string, bool, string, string, string, string>(text, isPrimary, storeKind, true, text2, req.OperationType, req.InitiatedTime.ToString("o"), latencyInMs, string.Empty, false, debugStr, text3, text4, this.CurrentProcessName);
				}
				return;
			}
			string text5 = isSkipped ? "<ApiSkipped>" : exception.ToString();
			string text6 = text + ((exception == null) ? string.Empty : exception.GetType().Name);
			if (this.apiExecutionPeriodicLogDuration != TimeSpan.Zero)
			{
				DxStoreHACrimsonEvents.ApiExecutionFailed.LogPeriodic<string, bool, StoreKind, bool, string, OperationType, string, long, string, bool, string, string, string, string>(text6, this.apiExecutionPeriodicLogDuration, text, isPrimary, storeKind, false, text2, req.OperationType, req.InitiatedTime.ToString("o"), latencyInMs, text5.Substring(0, Math.Min(text5.Length, 5000)), isSkipped, debugStr, text3, text4, this.CurrentProcessName);
				return;
			}
			DxStoreHACrimsonEvents.ApiExecutionFailed.Log<string, bool, StoreKind, bool, string, OperationType, string, long, string, bool, string, string, string, string>(text, isPrimary, storeKind, false, text2, req.OperationType, req.InitiatedTime.ToString("o"), latencyInMs, text5.Substring(0, Math.Min(text5.Length, 5000)), isSkipped, debugStr, text3, text4, this.CurrentProcessName);
		}

		public void UpdateFinish(DistributedStoreKey key, StoreKind storeKind, bool isPrimary, RequestInfo req, long latencyInMs, Exception exception, bool isSkipped)
		{
			this.LogExecution(key, storeKind, isPrimary, req, latencyInMs, exception, isSkipped);
			lock (this.locker)
			{
				PerformanceEntry orAdd = this.GetOrAdd(storeKind, isPrimary);
				orAdd.RecordFinish(req, latencyInMs, exception, isSkipped);
			}
		}

		public void Start()
		{
			lock (this.timerLock)
			{
				if (this.timer == null)
				{
					this.timer = new GuardedTimer(delegate(object o)
					{
						this.PublishConsolidatedPerformanceEvents();
					}, null, 0L, (long)RegistryParameters.DistributedStorePerfTrackerFlushInMs);
				}
			}
		}

		public void Stop()
		{
			this.PublishConsolidatedPerformanceEvents();
			lock (this.timerLock)
			{
				if (this.timer != null)
				{
					this.timer.Dispose(true);
					this.timer = null;
				}
			}
		}

		public void PublishConsolidatedPerformanceEvents()
		{
			try
			{
				this.PublishConsolidatedPerformanceEventsInternal();
			}
			catch (Exception ex)
			{
				DxStoreHACrimsonEvents.FailedToPublishPerfStats.Log<string>(ex.ToString());
			}
		}

		private PerformanceEntry GetOrAdd(StoreKind storeKind, bool isPrimary)
		{
			PerformanceEntry performanceEntry = (storeKind == StoreKind.Clusdb) ? this.clusdbPerfEntry : this.dxstorePerfEntry;
			if (performanceEntry == null)
			{
				performanceEntry = new PerformanceEntry(storeKind, isPrimary);
				if (storeKind == StoreKind.Clusdb)
				{
					this.clusdbPerfEntry = performanceEntry;
				}
				else
				{
					this.dxstorePerfEntry = performanceEntry;
				}
			}
			return performanceEntry;
		}

		private void PublishConsolidatedPerformanceEventsInternal()
		{
			PerformanceEntry performanceEntry;
			PerformanceEntry performanceEntry2;
			lock (this.locker)
			{
				performanceEntry = this.clusdbPerfEntry;
				performanceEntry2 = this.dxstorePerfEntry;
				this.clusdbPerfEntry = null;
				this.dxstorePerfEntry = null;
			}
			if (performanceEntry != null)
			{
				performanceEntry.PublishEvent(this.CurrentProcessName);
			}
			if (performanceEntry2 != null)
			{
				performanceEntry2.PublishEvent(this.CurrentProcessName);
			}
		}

		private readonly object timerLock = new object();

		private readonly object locker = new object();

		private readonly TimeSpan apiExecutionPeriodicLogDuration;

		private GuardedTimer timer;

		private PerformanceEntry clusdbPerfEntry;

		private PerformanceEntry dxstorePerfEntry;
	}
}
