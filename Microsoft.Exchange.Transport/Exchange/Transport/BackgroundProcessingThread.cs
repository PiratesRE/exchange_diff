using System;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Transport.DiagnosticsAggregationService;
using Microsoft.Exchange.Transport.RemoteDelivery;
using Microsoft.Exchange.Transport.Storage.IPFiltering;

namespace Microsoft.Exchange.Transport
{
	internal class BackgroundProcessingThread : BackgroundProcessingThreadBase, IStartableTransportComponent, ITransportComponent
	{
		public BackgroundProcessingThread(BackgroundProcessingThread.ServerComponentStateChangedHandler serverComponentStateChangedHandler) : base(TimeSpan.FromSeconds(1.0))
		{
			this.serverComponentStateChangedHandler = serverComponentStateChangedHandler;
		}

		public string CurrentState
		{
			get
			{
				return null;
			}
		}

		public void Load()
		{
		}

		public void Unload()
		{
		}

		public string OnUnhandledException(Exception e)
		{
			return null;
		}

		public void Pause()
		{
		}

		public void Continue()
		{
		}

		protected override void Run()
		{
			DateTime utcNow = DateTime.UtcNow;
			this.filterPruned = utcNow;
			this.lastScan = utcNow;
			this.lastTimeThrottlingManagerSwept = utcNow;
			this.lastCatMonitored = utcNow;
			this.lastDsnMonitored = utcNow;
			this.lastQueueLevelLogTime = utcNow;
			this.lastTimeBasedPerfCounterUpdate = utcNow;
			this.lastServiceStateCheckTime = utcNow;
			if (Components.Configuration.ProcessTransportRole == ProcessTransportRole.Hub)
			{
				this.serviceStateHelper = new ServiceStateHelper(Components.Configuration, ServerComponentStates.GetComponentId(ServerComponentEnum.HubTransport));
			}
			else if (Components.Configuration.ProcessTransportRole == ProcessTransportRole.Edge)
			{
				this.serviceStateHelper = new ServiceStateHelper(Components.Configuration, ServerComponentStates.GetComponentId(ServerComponentEnum.EdgeTransport));
			}
			base.Run();
		}

		protected override void RunMain(DateTime now)
		{
			Components.SmtpInComponent.UpdateTime(now);
			IStoreDriver storeDriver;
			bool flag = Components.TryGetStoreDriver(out storeDriver);
			if (now - this.filterPruned > BackgroundProcessingThread.FilterInterval)
			{
				IPFilterLists.Cleanup();
				this.filterPruned = now;
			}
			if (now - this.lastScan > BackgroundProcessingThread.SlowScanInterval)
			{
				TimeSpan t = now - BackgroundProcessingThread.SystemStartTime;
				if (!BackgroundProcessingThread.reportedCrashingClearEvent && t > BackgroundProcessingThread.CrashingClearEventTime)
				{
					BackgroundProcessingThread.eventLogger.LogEvent(TransportEventLogConstants.Tuple_ServerLivingForConsiderableTime, null, new object[]
					{
						t.ToString()
					});
					BackgroundProcessingThread.reportedCrashingClearEvent = true;
				}
				if (flag)
				{
					Components.StoreDriver.ExpireOldSubmissionConnections();
				}
				QueueManager.StartUpdateAllQueues();
				Components.CategorizerComponent.TimedUpdate();
				IProcessingQuotaComponent processingQuotaComponent;
				if (Components.TryGetProcessingQuotaComponent(out processingQuotaComponent))
				{
					processingQuotaComponent.TimedUpdate();
				}
				this.lastScan = now;
			}
			if (now - this.lastTimeBasedPerfCounterUpdate > BackgroundProcessingThread.TimeBasedPerfCounterUpdateInterval)
			{
				Components.QueueManager.GetQueuedRecipientsByAge().TimedUpdate();
				IQueueQuotaComponent queueQuotaComponent;
				if (Components.TryGetQueueQuotaComponent(out queueQuotaComponent))
				{
					queueQuotaComponent.TimedUpdate();
				}
				this.lastTimeBasedPerfCounterUpdate = now;
			}
			if (this.serviceStateHelper != null && now - this.lastServiceStateCheckTime > BackgroundProcessingThread.ServiceStateCheckInterval)
			{
				Components.RemoteDeliveryComponent.GetEndToEndLatencyBuckets().Flush();
				bool flag2 = this.serviceStateHelper.CheckState(this.startupServiceState);
				if (flag2 && this.serverComponentStateChangedHandler != null)
				{
					this.serverComponentStateChangedHandler();
				}
				this.lastServiceStateCheckTime = now;
			}
			if (now - this.lastTimeThrottlingManagerSwept > BackgroundProcessingThread.FiveMinuteInterval)
			{
				Components.MessageThrottlingComponent.MessageThrottlingManager.CleanupIdleEntries();
				Components.UnhealthyTargetFilterComponent.CleanupExpiredEntries();
				this.lastTimeThrottlingManagerSwept = now;
			}
			if (Components.ResourceManager.IsMonitoringEnabled && now - Components.ResourceManager.LastTimeResourceMonitored > Components.ResourceManager.MonitorInterval)
			{
				Components.ResourceManager.OnMonitorResource(null);
			}
			if (now - this.lastCatMonitored > BackgroundProcessingThread.CategorizerMonitorInterval)
			{
				Components.CategorizerComponent.MonitorJobs();
				this.lastCatMonitored = now;
			}
			if (now - this.lastDsnMonitored > BackgroundProcessingThread.FiveMinuteInterval)
			{
				Components.DsnGenerator.MonitorJobs();
				this.lastDsnMonitored = now;
			}
			if (now - this.lastQueueLevelLogTime > Components.Configuration.AppConfig.QueueConfiguration.QueueLoggingInterval)
			{
				this.LogQueueInfo();
				this.lastQueueLevelLogTime = now;
			}
		}

		private void LogQueueInfo()
		{
			DiagnosticsAggregationHelper.LogQueueInfo(Components.Configuration.LocalServer.TransportServer.QueueLogPath);
		}

		public static readonly TimeSpan SlowScanInterval = TimeSpan.FromSeconds(5.0);

		private static readonly TimeSpan TimeBasedPerfCounterUpdateInterval = TimeSpan.FromSeconds(5.0);

		private static readonly TimeSpan FiveMinuteInterval = TimeSpan.FromMinutes(5.0);

		private static readonly TimeSpan FilterInterval = TimeSpan.FromHours(6.0);

		private static readonly TimeSpan CrashingClearEventTime = TimeSpan.FromMinutes(30.0);

		private static readonly TimeSpan ServiceStateCheckInterval = TimeSpan.FromSeconds(30.0);

		private static readonly DateTime SystemStartTime = DateTime.UtcNow;

		private static readonly TimeSpan CategorizerMonitorInterval = Components.TransportAppConfig.Resolver.JobHealthUpdateInterval;

		private static bool reportedCrashingClearEvent = false;

		private static ExEventLog eventLogger = new ExEventLog(ExTraceGlobals.GeneralTracer.Category, TransportEventLog.GetEventSource());

		private DateTime filterPruned;

		private DateTime lastScan;

		private DateTime lastTimeThrottlingManagerSwept;

		private DateTime lastCatMonitored;

		private DateTime lastDsnMonitored;

		private DateTime lastQueueLevelLogTime;

		private DateTime lastTimeBasedPerfCounterUpdate;

		private DateTime lastServiceStateCheckTime;

		private ServiceStateHelper serviceStateHelper;

		private BackgroundProcessingThread.ServerComponentStateChangedHandler serverComponentStateChangedHandler;

		public delegate void ServerComponentStateChangedHandler();
	}
}
