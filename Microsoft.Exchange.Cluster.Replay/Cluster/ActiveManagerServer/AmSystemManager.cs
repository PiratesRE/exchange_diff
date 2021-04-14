using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Threading;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.DxStore.HA;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Rpc.ActiveManager;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmSystemManager : ChangePoller
	{
		private AmSystemManager() : base(true)
		{
			this.Config = AmConfig.UnknownConfig;
		}

		public static Microsoft.Exchange.Diagnostics.Trace Tracer
		{
			get
			{
				return ExTraceGlobals.AmSystemManagerTracer;
			}
		}

		internal static AmSystemManager Instance
		{
			get
			{
				return AmSystemManager.sm_instance;
			}
		}

		internal static void TestResetDefaultInstance()
		{
			AmSystemManager.sm_instance = new AmSystemManager();
		}

		internal AmUnhandledExceptionHandler UnhandledExceptionHandler { get; private set; }

		internal AmDatabaseQueueManager DatabaseQueueManager { get; private set; }

		internal AmSystemEventQueue SystemEventQueue { get; private set; }

		internal AmConfigManager ConfigManager { get; private set; }

		internal AmClusterServiceMonitor ClusterServiceMonitor { get; private set; }

		internal AmStoreServiceMonitor StoreServiceMonitor { get; private set; }

		internal AmDbNodeAttemptTable DbNodeAttemptTable { get; private set; }

		internal AmStoreStateMarker StoreStateMarker { get; private set; }

		internal AmPeriodicEventManager PeriodicEventManager { get; private set; }

		internal AmClusterMonitor ClusterMonitor { get; private set; }

		internal AmNetworkMonitor NetworkMonitor { get; private set; }

		internal AmServerNameCacheManager ServerNameCacheManager { get; private set; }

		internal AmDelayedConfigDisposer DelayedConfigDisposer { get; private set; }

		internal AmPerfCounterUpdater AmPerfCounterUpdater { get; private set; }

		internal AmServiceKillStatusContainer ServiceKillStatusContainer { get; private set; }

		internal AmLastKnownGoodConfig LastKnownGoodConfig { get; private set; }

		internal AmTransientFailoverSuppressor TransientFailoverSuppressor { get; private set; }

		internal AmPamMonitor PamMonitor { get; private set; }

		internal AmDatabaseStateTracker DatabaseStateTracker { get; private set; }

		internal AmSystemFailoverOnReplayDownTracker SystemFailoverOnReplayDownTracker { get; private set; }

		internal AmCachedLastLogUpdater PamCachedLastLogUpdater { get; private set; }

		internal AmClusdbPeriodicCleanup ClusdbPeriodicCleanup { get; private set; }

		internal DataStorePeriodicChecker DataStorePeriodicChecker { get; private set; }

		internal bool IsShutdown
		{
			get
			{
				return this.m_fShutdown;
			}
		}

		internal EventWaitHandle ShutdownEvent
		{
			get
			{
				return this.m_shutdownEvent;
			}
		}

		internal ExDateTime? SystemShutdownStartTime { get; set; }

		internal bool IsSystemShutdownInProgress
		{
			get
			{
				return this.SystemShutdownStartTime != null;
			}
		}

		internal AmServerDbStatusInfoCache StatusInfoCache
		{
			get
			{
				return this.m_dbStatusInfoCache;
			}
		}

		internal AmConfig Config { get; private set; }

		internal ManualOneShotEvent ConfigInitializedEvent
		{
			get
			{
				return this.m_configInitializedEvent;
			}
		}

		private bool StoreKilledToForceDismount { get; set; }

		private bool DatabasesForceDismountedLocally { get; set; }

		public override void PrepareToStop()
		{
			AmTrace.Entering("AmSystemManager.PrepareToStop()", new object[0]);
			base.PrepareToStop();
			if (this.SystemEventQueue != null)
			{
				this.SystemEventQueue.Stop();
			}
			AmTrace.Leaving("AmSystemManager.PrepareToStop()", new object[0]);
		}

		internal bool EnqueueEvent(AmEvtBase evt, bool isHighPriority)
		{
			bool flag = false;
			lock (this.m_locker)
			{
				if (!this.m_fShutdown && this.SystemEventQueue != null)
				{
					flag = this.SystemEventQueue.Enqueue(evt, isHighPriority);
					if (flag)
					{
						bool flag3 = !AmPeriodicEventManager.IsPeriodicEvent(evt) || RegistryParameters.AmEnableCrimsonLoggingPeriodicEventProcessing;
						if (flag3)
						{
							ReplayCrimsonEvents.AmSystemEventEnqueued.Log<AmEvtBase>(evt);
						}
					}
					this.SystemEventQueue.ArrivalEvent.Set();
				}
			}
			return flag;
		}

		internal void ProcessEvents()
		{
			while (!this.m_fShutdown)
			{
				this.SystemEventQueue.ArrivalEvent.WaitOne(RegistryParameters.AmSystemManagerEventWaitTimeoutInMSec, false);
				if (this.m_fShutdown)
				{
					return;
				}
				while (!this.m_fShutdown)
				{
					AmEvtBase amEvtBase = this.SystemEventQueue.Dequeue();
					if (amEvtBase == null)
					{
						break;
					}
					AmFaultInject.SleepIfRequired(AmSleepTag.GenericSystemEventProcessingDelay);
					this.InvokeHandler(amEvtBase);
				}
				if (this.PeriodicEventManager != null)
				{
					this.PeriodicEventManager.EnqueuePeriodicEventIfRequired();
				}
			}
		}

		internal void StartClusterServiceMonitor()
		{
			if (this.ClusterServiceMonitor == null)
			{
				this.ClusterServiceMonitor = new AmClusterServiceMonitor();
				this.ClusterServiceMonitor.Start();
			}
		}

		internal void UpdateAmPerfCounterUpdaterDatabasesList(List<Guid> allDatabases)
		{
			if (this.AmPerfCounterUpdater != null)
			{
				this.AmPerfCounterUpdater.Update(allDatabases);
				return;
			}
			AmTrace.Diagnostic("UpdateAmPerfCounterUpdaterDatabasesList() was called, but AmPerfCounterUpdater is null.", new object[0]);
		}

		protected override void PollerThread()
		{
			AmTrace.Entering("AmSystemManager.PollerThread", new object[0]);
			this.OneTimeInitialize();
			while (!this.m_fShutdown)
			{
				Exception ex = AmHelper.HandleKnownExceptions(delegate(object param0, EventArgs param1)
				{
					try
					{
						this.Initialize();
						this.ProcessEvents();
					}
					finally
					{
						this.Cleanup();
					}
				});
				if (ex != null)
				{
					AmTrace.Diagnostic("System manager encountered an exception while trying to process events: {0}", new object[]
					{
						ex
					});
				}
				if (!this.m_fShutdown)
				{
					Thread.Sleep(5000);
				}
			}
			this.OneTimeCleanup();
			AmTrace.Leaving("AmSystemManager.PollerThread", new object[0]);
		}

		private void OneTimeInitialize()
		{
			AmTrace.Entering("AmSystemManager.OneTimeInitialize", new object[0]);
			lock (this.m_locker)
			{
				if (this.DelayedConfigDisposer == null)
				{
					this.DelayedConfigDisposer = new AmDelayedConfigDisposer();
					this.DelayedConfigDisposer.Start();
				}
			}
		}

		private void OneTimeCleanup()
		{
			AmTrace.Entering("AmSystemManager.OneTimeCleanup", new object[0]);
			lock (this.m_locker)
			{
				if (this.DelayedConfigDisposer != null)
				{
					this.DelayedConfigDisposer.Stop();
					this.DelayedConfigDisposer = null;
				}
			}
		}

		public static void SetEnabledSubComponentFlags(EnableSubComponentFlag flag)
		{
			AmSystemManager.enabledSubComponentFlags = flag;
		}

		private static bool IsSubComponentEnabled(EnableSubComponentFlag componentFlag)
		{
			return AmSystemManager.enabledSubComponentFlags.HasFlag(componentFlag);
		}

		private void Initialize()
		{
			AmTrace.Entering("AmSystemManager.Initialize", new object[0]);
			lock (this.m_locker)
			{
				if (this.UnhandledExceptionHandler == null)
				{
					this.UnhandledExceptionHandler = new AmUnhandledExceptionHandler();
				}
				if (this.LastKnownGoodConfig == null)
				{
					this.LastKnownGoodConfig = AmLastKnownGoodConfig.ConstructLastKnownGoodConfigFromPersistentState();
				}
				if (AmSystemManager.IsSubComponentEnabled(EnableSubComponentFlag.TransientFailoverSuppressor) && this.TransientFailoverSuppressor == null)
				{
					this.TransientFailoverSuppressor = new AmTransientFailoverSuppressor();
				}
				if (AmSystemManager.IsSubComponentEnabled(EnableSubComponentFlag.ServiceKillStatusContainer) && this.ServiceKillStatusContainer == null)
				{
					this.ServiceKillStatusContainer = new AmServiceKillStatusContainer();
				}
				if (AmSystemManager.IsSubComponentEnabled(EnableSubComponentFlag.ServerNameCacheManager) && this.ServerNameCacheManager == null)
				{
					this.ServerNameCacheManager = new AmServerNameCacheManager();
					this.ServerNameCacheManager.Start();
				}
				if (AmSystemManager.IsSubComponentEnabled(EnableSubComponentFlag.DbNodeAttemptTable) && this.DbNodeAttemptTable == null)
				{
					this.DbNodeAttemptTable = new AmDbNodeAttemptTable();
				}
				if (AmSystemManager.IsSubComponentEnabled(EnableSubComponentFlag.SystemEventQueue) && this.SystemEventQueue == null)
				{
					this.SystemEventQueue = new AmSystemEventQueue();
				}
				if (AmSystemManager.IsSubComponentEnabled(EnableSubComponentFlag.DatabaseQueueManager) && this.DatabaseQueueManager == null)
				{
					this.DatabaseQueueManager = new AmDatabaseQueueManager();
				}
				if (this.ConfigManager == null)
				{
					this.ConfigManager = new AmConfigManager(Dependencies.ReplayAdObjectLookup);
				}
				if (AmSystemManager.IsSubComponentEnabled(EnableSubComponentFlag.PamCachedLastLogUpdater) && this.PamCachedLastLogUpdater == null)
				{
					this.PamCachedLastLogUpdater = new AmCachedLastLogUpdater();
					this.PamCachedLastLogUpdater.Start();
				}
				if (AmSystemManager.IsSubComponentEnabled(EnableSubComponentFlag.StoreStateMarker) && this.StoreStateMarker == null)
				{
					this.StoreStateMarker = new AmStoreStateMarker();
				}
				if (AmSystemManager.IsSubComponentEnabled(EnableSubComponentFlag.PeriodicEventManager) && this.PeriodicEventManager == null)
				{
					this.PeriodicEventManager = new AmPeriodicEventManager();
				}
				if (AmSystemManager.IsSubComponentEnabled(EnableSubComponentFlag.ClusterMonitor) && this.ClusterMonitor == null)
				{
					this.ClusterMonitor = new AmClusterMonitor();
				}
				if (AmSystemManager.IsSubComponentEnabled(EnableSubComponentFlag.NetworkMonitor) && this.NetworkMonitor == null)
				{
					this.NetworkMonitor = new AmNetworkMonitor();
				}
				if (AmSystemManager.IsSubComponentEnabled(EnableSubComponentFlag.AmPerfCounterUpdater) && this.AmPerfCounterUpdater == null)
				{
					this.AmPerfCounterUpdater = new AmPerfCounterUpdater();
					this.AmPerfCounterUpdater.Start();
				}
				if (AmSystemManager.IsSubComponentEnabled(EnableSubComponentFlag.ClusdbPeriodicCleanup) && this.ClusdbPeriodicCleanup == null && RegistryParameters.ClusdbPeriodicCleanupIntervalInSecs > 0)
				{
					this.ClusdbPeriodicCleanup = new AmClusdbPeriodicCleanup();
					this.ClusdbPeriodicCleanup.Start();
				}
				if (AmSystemManager.IsSubComponentEnabled(EnableSubComponentFlag.DataStorePeriodicChecker) && this.DataStorePeriodicChecker == null && !RegistryParameters.DistributedStoreDisableDualClientMode)
				{
					this.DataStorePeriodicChecker = new DataStorePeriodicChecker();
					this.DataStorePeriodicChecker.Start();
				}
			}
			this.ConfigManager.Start();
			AmTrace.Leaving("AmSystemManager.Initialize", new object[0]);
		}

		private void Cleanup()
		{
			AmTrace.Entering("AmSystemManager.Cleanup", new object[0]);
			if (this.m_fShutdown)
			{
				AmTrace.Debug("Setting system manager config to unknown since the system manager is shutting down", new object[0]);
				this.Config = new AmConfig(ReplayStrings.AmServiceShuttingDown);
			}
			else
			{
				AmTrace.Debug("Setting system manager config to unknown since System manager event processing has abnormally terminated.", new object[0]);
				this.Config = new AmConfig();
			}
			if (this.ConfigManager != null)
			{
				this.ConfigManager.Stop();
			}
			if (this.PamMonitor != null)
			{
				this.PamMonitor.Stop();
			}
			if (this.ServerNameCacheManager != null)
			{
				this.ServerNameCacheManager.Stop();
			}
			if (this.PeriodicEventManager != null)
			{
				this.PeriodicEventManager.Stop();
			}
			if (this.SystemEventQueue != null)
			{
				this.SystemEventQueue.Stop();
			}
			if (this.DatabaseQueueManager != null)
			{
				if (this.m_fShutdown)
				{
					TimeSpan timeSpan = TimeSpan.FromSeconds((double)RegistryParameters.DbQueueMgrStopLimitInSecs);
					try
					{
						InvokeWithTimeout.Invoke(delegate()
						{
							this.DatabaseQueueManager.Stop();
						}, timeSpan);
						goto IL_119;
					}
					catch (TimeoutException)
					{
						AmTrace.Diagnostic(string.Format("DatabaseQueueManager hit timeout ({0}) during service stop.", timeSpan), new object[0]);
						goto IL_119;
					}
				}
				this.DatabaseQueueManager.Stop();
			}
			IL_119:
			if (this.ClusterServiceMonitor != null)
			{
				this.ClusterServiceMonitor.Stop();
			}
			if (this.StoreServiceMonitor != null)
			{
				this.StoreServiceMonitor.Stop();
			}
			if (this.AmPerfCounterUpdater != null)
			{
				this.AmPerfCounterUpdater.Stop();
			}
			if (this.DbNodeAttemptTable != null)
			{
				this.DbNodeAttemptTable.ClearFailedTime();
			}
			if (this.StoreStateMarker != null)
			{
				this.StoreStateMarker.Clear();
			}
			if (this.ServiceKillStatusContainer != null)
			{
				this.ServiceKillStatusContainer.Cleanup();
			}
			if (this.TransientFailoverSuppressor != null)
			{
				this.TransientFailoverSuppressor.RemoveAllEntries(false);
			}
			if (this.DataStorePeriodicChecker != null)
			{
				this.DataStorePeriodicChecker.Stop();
			}
			if (this.UnhandledExceptionHandler != null)
			{
				this.UnhandledExceptionHandler.Cleanup();
			}
			if (this.ConfigInitializedEvent != null)
			{
				this.ConfigInitializedEvent.Close();
			}
			if (this.DatabaseStateTracker != null)
			{
				this.DatabaseStateTracker.Cleanup();
			}
			if (this.SystemFailoverOnReplayDownTracker != null)
			{
				this.SystemFailoverOnReplayDownTracker.Cleanup();
			}
			if (this.PamCachedLastLogUpdater != null)
			{
				this.PamCachedLastLogUpdater.Stop();
			}
			if (this.ClusdbPeriodicCleanup != null)
			{
				this.ClusdbPeriodicCleanup.Stop();
			}
			lock (this.m_locker)
			{
				this.SystemEventQueue = null;
				this.DatabaseQueueManager = null;
				this.ConfigManager = null;
				this.ClusterServiceMonitor = null;
				this.StoreServiceMonitor = null;
				this.DbNodeAttemptTable = null;
				this.PeriodicEventManager = null;
				this.StoreStateMarker = null;
				this.AmPerfCounterUpdater = null;
				this.ServiceKillStatusContainer = null;
				this.ClusdbPeriodicCleanup = null;
				this.DataStorePeriodicChecker = null;
			}
			AmTrace.Leaving("AmSystemManager.Cleanup", new object[0]);
		}

		private void AssertTimerCallback(object obj)
		{
			string text = obj.ToString();
			ReplayCrimsonEvents.AmSystemEventProcessingIsTakingTooLong.Log<string, int>(text, RegistryParameters.AmSystemEventAssertOnHangTimeoutInMSec);
		}

		private void InvokeHandler(AmEvtBase evt)
		{
			bool isSuccess = false;
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			Exception ex = null;
			string text = evt.ToString();
			bool flag = !AmPeriodicEventManager.IsPeriodicEvent(evt) || RegistryParameters.AmEnableCrimsonLoggingPeriodicEventProcessing;
			if (flag)
			{
				ReplayCrimsonEvents.AmSystemEventProcessingStarted.Log<string>(text);
			}
			Timer timer = new Timer(new TimerCallback(this.AssertTimerCallback), evt, RegistryParameters.AmSystemEventAssertOnHangTimeoutInMSec, -1);
			try
			{
				ex = AmHelper.HandleKnownExceptions(delegate(object param0, EventArgs param1)
				{
					this.InvokeHandlerInternal(evt);
					isSuccess = true;
				});
			}
			finally
			{
				timer.Change(-1, -1);
				timer.Dispose();
				stopwatch.Stop();
				if (isSuccess)
				{
					if (flag)
					{
						ReplayCrimsonEvents.AmSystemEventProcessingFinished.Log<string, TimeSpan>(text, stopwatch.Elapsed);
					}
				}
				else
				{
					bool flag2 = ex == null;
					if (flag || flag2)
					{
						ReplayCrimsonEvents.AmSystemEventProcessingFinishedWithException.Log<string, TimeSpan, string>(text, stopwatch.Elapsed, flag2 ? "<unhandled exception>" : ex.Message);
					}
				}
			}
		}

		private bool IsEventPeriodic(Type eventType)
		{
			return eventType == typeof(AmEvtPeriodicDbStateAnalyze) || eventType == typeof(AmEvtPeriodicDbStateRestore) || eventType == typeof(AmEvtPeriodicCheckMismountedDatabase);
		}

		private void RefreshADConfigAsync(Type evtType)
		{
			ThreadPool.QueueUserWorkItem(delegate(object o)
			{
				try
				{
					Dependencies.ADConfig.Refresh(string.Format("AmSystemManager evt={0}", evtType));
				}
				catch (Exception arg)
				{
					AmSystemManager.Tracer.TraceError<Exception>(0L, "Dependencies.ADConfig.Refresh failed with {0}", arg);
				}
			});
		}

		private void InvokeHandlerInternal(AmEvtBase evt)
		{
			Type type = evt.GetType();
			if (!this.IsEventPeriodic(type))
			{
				this.RefreshADConfigAsync(type);
			}
			AmSystemManager.Tracer.TraceDebug<AmEvtBase>(0L, "Started processing event: {0}", evt);
			if (type == typeof(AmEvtConfigChanged))
			{
				this.OnEvtConfigChanged((AmEvtConfigChanged)evt);
			}
			else if (type == typeof(AmEvtNodeStateChanged))
			{
				this.OnEventNodeStateChanged((AmEvtNodeStateChanged)evt);
			}
			else if (type == typeof(AmEvtNodeDownForLongTime))
			{
				this.OnEventNodeDownForLongTime((AmEvtNodeDownForLongTime)evt);
			}
			else if (type == typeof(AmEvtNodeAdded))
			{
				this.OnEvtNodeAdded((AmEvtNodeAdded)evt);
			}
			else if (type == typeof(AmEvtNodeRemoved))
			{
				this.OnEvtNodeRemoved((AmEvtNodeRemoved)evt);
			}
			else if (type == typeof(AmEvtClusterStateChanged))
			{
				this.OnEvtClusterStateChanged((AmEvtClusterStateChanged)evt);
			}
			else if (type == typeof(AmEvtClussvcStopped))
			{
				this.OnEvtClussvcStopped((AmEvtClussvcStopped)evt);
			}
			else if (type == typeof(AmEvtSystemStartup))
			{
				this.OnEvtSystemStartup((AmEvtSystemStartup)evt);
			}
			else if (type == typeof(AmEvtStoreServiceStarted))
			{
				this.OnEvtStoreServiceStarted((AmEvtStoreServiceStarted)evt);
			}
			else if (type == typeof(AmEvtStoreServiceStopped))
			{
				this.OnEvtStoreServiceStopped((AmEvtStoreServiceStopped)evt);
			}
			else if (type == typeof(AmEvtPeriodicDbStateAnalyze))
			{
				this.OnEvtPeriodicDbStateAnalyze((AmEvtPeriodicDbStateAnalyze)evt);
			}
			else if (type == typeof(AmEvtPeriodicDbStateRestore))
			{
				this.OnEvtPeriodicDbStateRestore((AmEvtPeriodicDbStateRestore)evt);
			}
			else if (type == typeof(AmEvtPeriodicCheckMismountedDatabase))
			{
				this.OnEvtPeriodicCheckMismountedDatabase((AmEvtPeriodicCheckMismountedDatabase)evt);
			}
			else if (type == typeof(AmEvtSwitchoverOnShutdown))
			{
				this.OnEvtSwitchoverOnShutdown((AmEvtSwitchoverOnShutdown)evt);
			}
			else if (type == typeof(AmEvtMoveAllDatabasesOnAdminRequest))
			{
				this.OnEvtMoveAllDatabasesOnAdminRequest((AmEvtMoveAllDatabasesOnAdminRequest)evt);
			}
			else if (type == typeof(AmEvtMoveAllDatabasesOnComponentRequest))
			{
				this.OnEvtMoveAllDatabasesOnComponentRequest((AmEvtMoveAllDatabasesOnComponentRequest)evt);
			}
			else if (type == typeof(AmEvtRejoinNodeForDac))
			{
				this.OnEvtRejoinNodeForDac((AmEvtRejoinNodeForDac)evt);
			}
			else if (type == typeof(AmEvtMapiNetworkFailure))
			{
				this.OnEventMapiNetworkFailure((AmEvtMapiNetworkFailure)evt);
			}
			else if (type == typeof(AmEvtGroupOwnerButUnknown))
			{
				this.OnEventGroupOwnerButUnknown((AmEvtGroupOwnerButUnknown)evt);
			}
			else if (type == typeof(AmEvtSystemFailoverOnReplayDown))
			{
				this.OnEvtSystemFailoverOnReplayDown((AmEvtSystemFailoverOnReplayDown)evt);
			}
			else
			{
				AmTrace.Diagnostic("System manager event processing encountered an unknown event: {0}", new object[]
				{
					evt
				});
			}
			AmSystemManager.Tracer.TraceDebug<AmEvtBase>(0L, "Finished processing event: {0}", evt);
		}

		private void OnEvtConfigChanged(AmEvtConfigChanged evt)
		{
			AmTrace.Entering("AmSystemManager.OnEvtConfigChanged().", new object[0]);
			AmConfig previousConfig = evt.PreviousConfig;
			AmConfig newConfig = evt.NewConfig;
			if (newConfig != null && !newConfig.IsUnknown)
			{
				AmSystemManager.Tracer.TraceDebug(0L, "AmSystemManager.OnEvtConfigChanged(). Update LastKnownGoodConfig");
				this.LastKnownGoodConfig.Update(newConfig);
				AmSystemManager.Tracer.TraceDebug(0L, "AmSystemManager.OnEvtConfigChanged(). Update LastKnownGoodConfig - done");
			}
			this.Config = newConfig;
			this.ControlDatabaseStateTracker(newConfig);
			this.ControlSystemFailoverOnReplayDownTracker(newConfig, previousConfig);
			this.ConfigInitializedEvent.Set();
			AmSystemManager.Tracer.TraceDebug<AmRole, AmRole, AmConfigChangedFlags>(0L, "Config changed. (New role={0}, Prev role={1}, Change flags='{2}')", newConfig.Role, previousConfig.Role, evt.ChangeFlags);
			if (previousConfig.Role != newConfig.Role)
			{
				ReplayCrimsonEvents.RoleChanged.Log<AmRole, AmRole>(newConfig.Role, previousConfig.Role);
				ThirdPartyManager.Instance.AmRoleNotify(newConfig);
				if (this.PamCachedLastLogUpdater != null)
				{
					this.PamCachedLastLogUpdater.Cleanup();
				}
				if (newConfig.IsPAM)
				{
					if (this.TransientFailoverSuppressor != null)
					{
						this.TransientFailoverSuppressor.Initialize();
					}
				}
				else if (this.TransientFailoverSuppressor != null)
				{
					this.TransientFailoverSuppressor.RemoveAllEntries(false);
				}
			}
			if (evt.ChangeFlags == AmConfigChangedFlags.None || evt.ChangeFlags == AmConfigChangedFlags.LastError)
			{
				AmSystemManager.Tracer.TraceDebug<AmConfigChangedFlags, string>(0L, "Ignoring config change. (ChangeFlags={0}, LastError={1})", evt.ChangeFlags, newConfig.LastError);
			}
			else
			{
				if (newConfig.IsUnknown)
				{
					AmSystemManager.Tracer.TraceDebug(0L, "Detected Unknown role. Store service monitor will be stopped. It will be restarted when a valid AM role is discovered");
					this.StopStoreServiceMonitor();
					if (newConfig.IsUnknownTriggeredByADError)
					{
						AmSystemManager.Tracer.TraceDebug(0L, "Ensure we do not keep group ownership when in the Unknown role");
						this.TryToDisownGroup();
					}
					else
					{
						AmSystemManager.Tracer.TraceDebug(0L, "AmConfig is in unknown state, but it is not triggered by AD issue");
					}
				}
				else
				{
					this.m_checkDbWatch.Reset();
					this.m_checkDbWatch.Start();
					if (newConfig.IsDecidingAuthority)
					{
						if (newConfig.IsDebugOptionsEnabled())
						{
							ReplayCrimsonEvents.DebugOptionsEnabled.Log();
						}
						if (newConfig.IsPAM && previousConfig.IsSAM)
						{
							AmServerName currentPAM = previousConfig.DagConfig.CurrentPAM;
							if (!AmServerName.IsNullOrEmpty(currentPAM))
							{
								AmNodeState nodeState = newConfig.DagConfig.GetNodeState(currentPAM);
								if (nodeState == AmNodeState.Down)
								{
									AmEvtNodeStateChanged amEvtNodeStateChanged = new AmEvtNodeStateChanged(currentPAM, nodeState);
									amEvtNodeStateChanged.Notify();
								}
							}
						}
						AmEvtSystemStartup amEvtSystemStartup = new AmEvtSystemStartup();
						amEvtSystemStartup.Notify();
					}
					else if (newConfig.IsSAM)
					{
						AmSystemManager.Tracer.TraceDebug(0L, "Config change detected and check databases issued on a SAM");
						AmEvtPeriodicCheckMismountedDatabase amEvtPeriodicCheckMismountedDatabase = new AmEvtPeriodicCheckMismountedDatabase();
						amEvtPeriodicCheckMismountedDatabase.Notify();
					}
					if (newConfig.IsStandalone)
					{
						this.StopClusterServiceMonitor();
					}
					if (newConfig.IsPamOrSam)
					{
						this.StartClusterServiceMonitor();
					}
					this.StoreKilledToForceDismount = false;
					this.DatabasesForceDismountedLocally = false;
					this.StartStoreServiceMonitor();
				}
				this.ControlPamMonitor(newConfig);
			}
			AmTrace.Leaving("AmSystemManager.OnEvtConfigChanged().", new object[0]);
		}

		private void ControlDatabaseStateTracker(AmConfig cfg)
		{
			AmDatabaseStateTracker databaseStateTracker = this.DatabaseStateTracker;
			this.DatabaseStateTracker = null;
			if (databaseStateTracker != null)
			{
				databaseStateTracker.Cleanup();
			}
			if (cfg.IsPAM && !RegistryParameters.DatabaseStateTrackerDisabled)
			{
				this.DatabaseStateTracker = new AmDatabaseStateTracker();
				this.DatabaseStateTracker.Initialize();
			}
		}

		private void ControlSystemFailoverOnReplayDownTracker(AmConfig cfg, AmConfig prevCfg)
		{
			if (!RegistryParameters.OnReplDownFailoverEnabled)
			{
				return;
			}
			AmSystemFailoverOnReplayDownTracker amSystemFailoverOnReplayDownTracker = this.SystemFailoverOnReplayDownTracker;
			if (cfg.IsPAM)
			{
				if (amSystemFailoverOnReplayDownTracker == null || prevCfg == null || prevCfg.Role != AmRole.PAM)
				{
					amSystemFailoverOnReplayDownTracker = new AmSystemFailoverOnReplayDownTracker();
					amSystemFailoverOnReplayDownTracker.InitializeFromClusdb();
					this.SystemFailoverOnReplayDownTracker = amSystemFailoverOnReplayDownTracker;
					return;
				}
			}
			else if (amSystemFailoverOnReplayDownTracker != null)
			{
				this.SystemFailoverOnReplayDownTracker = null;
				amSystemFailoverOnReplayDownTracker.Cleanup();
			}
		}

		private void ControlPamMonitor(AmConfig cfg)
		{
			if (cfg.IsSAM)
			{
				if (this.PamMonitor == null)
				{
					this.PamMonitor = new AmPamMonitor();
					this.PamMonitor.Start();
					return;
				}
			}
			else if (this.PamMonitor != null)
			{
				this.PamMonitor.Stop();
				this.PamMonitor = null;
			}
		}

		private void OnEventNodeStateChanged(AmEvtNodeStateChanged evt)
		{
			ReplayCrimsonEvents.NodeStateChangeDetected.Log<string, AmNodeState>(evt.NodeName.Fqdn, evt.State);
			if (this.Config.IsPAM)
			{
				if (evt.State == AmNodeState.Down)
				{
					if (this.Config.IsIgnoreServerDebugOptionEnabled(evt.NodeName))
					{
						ReplayCrimsonEvents.OperationNotPerformedDueToDebugOption.Log<string, string, string>(evt.NodeName.NetbiosName, AmDebugOptions.IgnoreServerFromAutomaticActions.ToString(), "System failover request ignored");
						return;
					}
					AmSystemFailover amSystemFailover = new AmSystemFailover(evt.NodeName, AmDbActionReason.NodeDown, false, false);
					amSystemFailover.Run();
					return;
				}
				else if (AmClusterNode.IsNodeUp(evt.State))
				{
					if (AmSystemManager.Instance.DbNodeAttemptTable != null)
					{
						AmSystemManager.Instance.DbNodeAttemptTable.ClearFailedTime(evt.NodeName);
					}
					if (AmSystemManager.Instance.TransientFailoverSuppressor != null)
					{
						AmSystemManager.Instance.TransientFailoverSuppressor.RemoveEntry(evt.NodeName, true, "NodeUp");
					}
				}
			}
		}

		private void OnEventNodeDownForLongTime(AmEvtNodeDownForLongTime evt)
		{
			if (this.Config.IsPAM)
			{
				AmSystemFailover amSystemFailover = new AmSystemFailover(evt.NodeName, AmDbActionReason.NodeDownConfirmed, true, false);
				amSystemFailover.Run();
			}
		}

		private void OnEventMapiNetworkFailure(AmEvtMapiNetworkFailure evt)
		{
			AmSystemManager.Tracer.TraceDebug<AmServerName>(0L, "OnEventMapiNetworkFailure({0})", evt.NodeName);
			IAmCluster cluster = this.ClusterMonitor.Cluster;
			if (cluster == null)
			{
				AmSystemManager.Tracer.TraceError((long)this.GetHashCode(), "Ignoring event because AmClusterMonitor.Cluster is null");
				return;
			}
			if (this.NetworkMonitor.AreAnyMapiNicsUp(evt.NodeName))
			{
				AmSystemManager.Tracer.TraceDebug<AmServerName>(0L, "Ignoring event. NICs must now be up on {0}.", evt.NodeName);
				ReplayEventLogConstants.Tuple_AmIgnoredMapiNetFailureBecauseMapiLooksUp.LogEvent(evt.NodeName.NetbiosName, new object[]
				{
					evt.NodeName.NetbiosName
				});
				return;
			}
			if (evt.NodeName.IsLocalComputerName)
			{
				this.HandleLocalMapiNetworkFailure(cluster, evt);
				return;
			}
			this.HandleRemoteMapiNetworkFailure(cluster, evt);
		}

		private void HandleLocalMapiNetworkFailure(IAmCluster cluster, AmEvtMapiNetworkFailure evt)
		{
			bool flag = false;
			try
			{
				ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 1585, "HandleLocalMapiNetworkFailure", "f:\\15.00.1497\\sources\\dev\\cluster\\src\\Replay\\ActiveManager\\AmSystemManager.cs");
				topologyConfigurationSession.ServerTimeout = new TimeSpan?(TimeSpan.FromSeconds(5.0));
				topologyConfigurationSession.FindServerByName(Environment.MachineName);
				flag = true;
			}
			catch (ADTransientException arg)
			{
				AmSystemManager.Tracer.TraceError<ADTransientException>(0L, "HandleLocalMapiNetworkFailure got AD error: {0}", arg);
			}
			Exception ex;
			if (flag)
			{
				AmClusterNodeNetworkStatus amClusterNodeNetworkStatus = new AmClusterNodeNetworkStatus();
				if (!this.NetworkMonitor.AreAnyMapiNicsUp(evt.NodeName))
				{
					amClusterNodeNetworkStatus.ClusterErrorOverride = true;
					ReplayCrimsonEvents.AmMapiAccessExpectedByAD.Log();
				}
				ex = AmClusterNodeStatusAccessor.Write(cluster, evt.NodeName, amClusterNodeNetworkStatus);
				if (ex != null)
				{
					ReplayCrimsonEvents.AmNodeStatusUpdateFailed.Log<string, string>(amClusterNodeNetworkStatus.ToString(), ex.Message);
				}
				ReplayEventLogConstants.Tuple_AmIgnoredMapiNetFailureBecauseADIsWorking.LogEvent(evt.NodeName.NetbiosName, new object[]
				{
					evt.NodeName.NetbiosName
				});
				return;
			}
			AmClusterNodeNetworkStatus amClusterNodeNetworkStatus2 = new AmClusterNodeNetworkStatus();
			amClusterNodeNetworkStatus2.HasADAccess = false;
			ex = AmClusterNodeStatusAccessor.Write(cluster, evt.NodeName, amClusterNodeNetworkStatus2);
			if (ex != null)
			{
				ReplayCrimsonEvents.AmNodeStatusUpdateFailed.Log<string, string>(amClusterNodeNetworkStatus2.ToString(), ex.Message);
			}
			ReplayEventLogConstants.Tuple_AMDetectedMapiNetworkFailure.LogEvent(evt.NodeName.NetbiosName, new object[]
			{
				evt.NodeName.NetbiosName
			});
			this.StopStoreServiceMonitor();
			this.KillStoreToForceDismount();
			this.TryToDisownGroup();
			AmSystemManager.Instance.ConfigManager.TriggerRefresh(true);
		}

		private void HandleRemoteMapiNetworkFailure(IAmCluster cluster, AmEvtMapiNetworkFailure evt)
		{
			if (!this.Config.IsPAM)
			{
				ReplayEventLogConstants.Tuple_AmIgnoredMapiNetFailureBecauseNotThePam.LogEvent(evt.NodeName.NetbiosName, new object[]
				{
					evt.NodeName.NetbiosName,
					AmServerName.LocalComputerName.NetbiosName
				});
				return;
			}
			Exception ex;
			AmNodeState nodeState = cluster.GetNodeState(evt.NodeName, out ex);
			if (ex != null)
			{
				AmSystemManager.Tracer.TraceError<Exception>(0L, "OnEventMapiNetworkFailure:GetNodeState fails:{0}", ex);
				return;
			}
			if (!AmClusterNode.IsNodeUp(nodeState))
			{
				ReplayEventLogConstants.Tuple_AmIgnoredMapiNetFailureBecauseNodeNotUp.LogEvent(evt.NodeName.NetbiosName, new object[]
				{
					evt.NodeName.NetbiosName
				});
				return;
			}
			TimeSpan value = new TimeSpan(0, 0, 5);
			DateTime t = DateTime.UtcNow.Add(value);
			AmClusterNodeNetworkStatus amClusterNodeNetworkStatus;
			for (;;)
			{
				amClusterNodeNetworkStatus = AmClusterNodeStatusAccessor.Read(cluster, evt.NodeName, out ex);
				if ((amClusterNodeNetworkStatus != null && !amClusterNodeNetworkStatus.IsHealthy) || DateTime.UtcNow > t)
				{
					break;
				}
				Thread.Sleep(1000);
			}
			if (amClusterNodeNetworkStatus != null && amClusterNodeNetworkStatus.IsHealthy && amClusterNodeNetworkStatus.ClusterErrorOverride)
			{
				ReplayEventLogConstants.Tuple_AmIgnoredMapiNetFailureBecauseADIsWorking.LogEvent(evt.NodeName.NetbiosName, new object[]
				{
					evt.NodeName.NetbiosName
				});
				return;
			}
			ReplayEventLogConstants.Tuple_AMDetectedMapiNetworkFailure.LogEvent(evt.NodeName.NetbiosName, new object[]
			{
				evt.NodeName.NetbiosName
			});
			AmSystemFailover amSystemFailover = new AmSystemFailover(evt.NodeName, AmDbActionReason.MapiNetFailure, false, false);
			amSystemFailover.Run();
		}

		private void OnEvtSystemStartup(AmEvtSystemStartup evt)
		{
			if (this.Config.IsDecidingAuthority)
			{
				AmStartupAutoMounter amStartupAutoMounter = new AmStartupAutoMounter();
				amStartupAutoMounter.Run();
			}
		}

		private void OnEvtStoreServiceStarted(AmEvtStoreServiceStarted evt)
		{
			if (this.Config.IsDecidingAuthority)
			{
				if (this.Config.IsIgnoreServerDebugOptionEnabled(evt.NodeName))
				{
					ReplayCrimsonEvents.OperationNotPerformedDueToDebugOption.Log<string, string, string>(evt.NodeName.NetbiosName, AmDebugOptions.IgnoreServerFromAutomaticActions.ToString(), "OnEvtStoreServiceStarted");
					return;
				}
				if (AmSystemManager.Instance.StoreStateMarker.CheckIfStoreStartMarkedAndClear(evt.NodeName))
				{
					AmStoreStartupAutoMounter amStoreStartupAutoMounter = new AmStoreStartupAutoMounter(evt.NodeName);
					amStoreStartupAutoMounter.Run();
					return;
				}
				AmTrace.Info("Ignoring automount request for node '{0}' since system startup mounter had already taken care of it", new object[]
				{
					evt.NodeName
				});
			}
		}

		private void OnEvtStoreServiceStopped(AmEvtStoreServiceStopped evt)
		{
			if (this.Config.IsDecidingAuthority)
			{
				if (this.Config.IsIgnoreServerDebugOptionEnabled(evt.NodeName))
				{
					ReplayCrimsonEvents.OperationNotPerformedDueToDebugOption.Log<string, string, string>(evt.NodeName.NetbiosName, AmDebugOptions.IgnoreServerFromAutomaticActions.ToString(), "OnEvtStoreServiceStopped");
					return;
				}
				AmBatchMarkDismounted amBatchMarkDismounted = new AmBatchMarkDismounted(evt.NodeName, AmDbActionReason.StoreStopped);
				amBatchMarkDismounted.Run();
				if (!evt.IsGracefulStop && this.Config.IsPAM)
				{
					AmSystemFailover amSystemFailover = new AmSystemFailover(evt.NodeName, AmDbActionReason.StoreStopped, true, false);
					amSystemFailover.Run();
				}
			}
		}

		private void OnEvtPeriodicDbStateAnalyze(AmEvtPeriodicDbStateAnalyze evt)
		{
			if (this.Config.IsDecidingAuthority)
			{
				AmPeriodicDatabaseStateAnalyzer amPeriodicDatabaseStateAnalyzer = new AmPeriodicDatabaseStateAnalyzer();
				amPeriodicDatabaseStateAnalyzer.Run();
			}
		}

		private void OnEvtPeriodicDbStateRestore(AmEvtPeriodicDbStateRestore evt)
		{
			if (this.Config.IsDecidingAuthority)
			{
				AmPeriodicDatabaseStateRestorer amPeriodicDatabaseStateRestorer = new AmPeriodicDatabaseStateRestorer(evt.Context);
				amPeriodicDatabaseStateRestorer.Run();
			}
		}

		private void OnEvtPeriodicCheckMismountedDatabase(AmEvtPeriodicCheckMismountedDatabase evt)
		{
			if (this.Config.IsSAM)
			{
				AmPeriodicSplitbrainChecker amPeriodicSplitbrainChecker = new AmPeriodicSplitbrainChecker();
				amPeriodicSplitbrainChecker.Run();
			}
		}

		private void OnEvtSwitchoverOnShutdown(AmEvtSwitchoverOnShutdown evt)
		{
			if (this.Config.IsPAM)
			{
				if (this.Config.IsIgnoreServerDebugOptionEnabled(evt.NodeName))
				{
					ReplayCrimsonEvents.OperationNotPerformedDueToDebugOption.Log<string, string, string>(evt.NodeName.NetbiosName, AmDebugOptions.IgnoreServerFromAutomaticActions.ToString(), "OnEvtSwitchoverOnShutdown");
					return;
				}
				new AmSystemSwitchover(evt.NodeName, AmDbActionReason.SystemShutdown)
				{
					CompletionCallback = new BatchOperationCompletedDelegate(evt.SwitchoverCompletedCallback),
					CustomStatus = AmDbActionStatus.AcllSuccessful
				}.Run();
			}
		}

		private void OnEvtMoveAllDatabasesOnAdminRequest(AmEvtMoveAllDatabasesOnAdminRequest evt)
		{
			if (this.Config.IsPAM)
			{
				if (AmSystemManager.Instance.TransientFailoverSuppressor != null)
				{
					AmSystemManager.Instance.TransientFailoverSuppressor.AdminRequestedForRemoval(evt.NodeName, "Move-ActiveMailboxDatabase -Server");
				}
				new AmSystemSwitchoverOnAdminMove(evt.MoveArgs)
				{
					CompletionCallback = new BatchOperationCompletedDelegate(evt.SwitchoverCompletedCallback)
				}.Run();
				return;
			}
			throw new AmInvalidConfiguration(this.Config.LastError);
		}

		private void OnEvtMoveAllDatabasesOnComponentRequest(AmEvtMoveAllDatabasesOnComponentRequest evt)
		{
			if (this.Config.IsPAM)
			{
				new AmSystemSwitchoverOnComponentMove(evt.MoveArgs)
				{
					CompletionCallback = new BatchOperationCompletedDelegate(evt.SwitchoverCompletedCallback)
				}.Run();
				return;
			}
			throw new AmInvalidConfiguration(this.Config.LastError);
		}

		private void OnEvtClussvcStopped(AmEvtClussvcStopped evt)
		{
			this.InitiateDismountAllLocalDatabasesToAvoidSplitBrain("OnEvtClussvcStopped");
		}

		private void OnEvtClusterStateChanged(AmEvtClusterStateChanged evt)
		{
			this.InitiateDismountAllLocalDatabasesToAvoidSplitBrain("OnEvtClusterStateChanged");
		}

		private void InitiateDismountAllLocalDatabasesToAvoidSplitBrain(string hint)
		{
			bool flag = false;
			int num;
			int num2;
			if (!RegistryParameters.IsTransientFailoverSuppressionEnabled)
			{
				flag = true;
			}
			else if (!AmTransientFailoverSuppressor.CheckIfMajorityNodesReachable(out num, out num2))
			{
				flag = true;
			}
			else
			{
				ReplayCrimsonEvents.IgnoredImmediateDismountSinceMajorityReachable.Log<int, int>(num, num2);
			}
			if (flag)
			{
				if (this.DatabasesForceDismountedLocally)
				{
					AmSystemManager.Tracer.TraceError<string>(0L, "Force-dismount-DBs already issued once. Possible message storm? Event hint: {0}", hint);
					return;
				}
				this.DatabasesForceDismountedLocally = true;
				ThreadPool.QueueUserWorkItem(delegate(object obj)
				{
					this.DismountAllLocalDatabasesToAvoidSplitBrain(obj as string);
				}, hint);
			}
		}

		private void DismountAllLocalDatabasesToAvoidSplitBrain(string hint)
		{
			if (this.Config.IsIgnoreServerDebugOptionEnabled(AmServerName.LocalComputerName))
			{
				ReplayCrimsonEvents.OperationNotPerformedDueToDebugOption.Log<string, string, string>(AmServerName.LocalComputerName.NetbiosName, AmDebugOptions.IgnoreServerFromAutomaticActions.ToString(), hint);
				return;
			}
			AmStoreHelper.DismountAll(hint);
		}

		private void KillStoreToForceDismount()
		{
			if (this.StoreKilledToForceDismount)
			{
				AmSystemManager.Tracer.TraceError(0L, "Store already killed. Possible message storm?");
				return;
			}
			this.StoreKilledToForceDismount = true;
			AmSystemManager.Tracer.TraceDebug(0L, "killing store");
			string text = "KillStoreToForceDismount";
			ReplayEventLogConstants.Tuple_AmKilledStoreToForceDismount.LogEvent(null, new object[0]);
			ReplayCrimsonEvents.ForceDismountingDatabases.Log<AmServerName, string>(AmServerName.LocalComputerName, text);
			Exception ex = ServiceOperations.KillService("MSExchangeIS", text);
			if (ex != null)
			{
				ReplayEventLogConstants.Tuple_AmFailedToStopService.LogEvent(string.Empty, new object[]
				{
					"MSExchangeIS",
					ex.ToString()
				});
			}
		}

		private void TryToDisownGroup()
		{
			IAmCluster amCluster = null;
			IAmClusterGroup amClusterGroup = null;
			try
			{
				amCluster = ClusterFactory.Instance.Open();
				amClusterGroup = amCluster.FindCoreClusterGroup();
				AmServerName ownerNode = amClusterGroup.OwnerNode;
				if (!ownerNode.IsLocalComputerName)
				{
					AmSystemManager.Tracer.TraceDebug<AmServerName>(0L, "TryToDisownGroup skipped because group owner is {0}", ownerNode);
				}
				else
				{
					ReplayCrimsonEvents.AmUnknownRoleButGroupOwner.Log();
					List<AmServerName> serversReportedAsPubliclyUp = AmSystemManager.Instance.NetworkMonitor.GetServersReportedAsPubliclyUp();
					TimeSpan timeout = new TimeSpan(0, 0, 30);
					foreach (AmServerName amServerName in serversReportedAsPubliclyUp)
					{
						if (!amServerName.IsLocalComputerName)
						{
							using (AmClusterNodeStatusAccessor amClusterNodeStatusAccessor = new AmClusterNodeStatusAccessor(amCluster, amServerName, DxStoreKeyAccessMode.Read))
							{
								AmClusterNodeNetworkStatus amClusterNodeNetworkStatus = amClusterNodeStatusAccessor.Read();
								if (amClusterNodeNetworkStatus == null || !amClusterNodeNetworkStatus.HasADAccess)
								{
									AmSystemManager.Tracer.TraceError<AmServerName>(0L, "Skipped move because {0} is reporting AD failed", amServerName);
									continue;
								}
							}
							try
							{
								AmSystemManager.Tracer.TraceDebug<AmServerName>(0L, "TryToDisownGroup: Trying to move ClusterGroup to {0}", amServerName);
								ReplayCrimsonEvents.AmUnknownRoleButGroupOwnerMoveAttemptStart.Log<AmServerName>(amServerName);
								AmClusterGroup.MoveClusterGroupWithTimeout(AmServerName.LocalComputerName, amServerName, timeout);
								AmSystemManager.Tracer.TraceDebug<AmServerName>(0L, "PAM should now be on {0}", amServerName);
								ReplayCrimsonEvents.AmUnknownRoleButGroupOwnerMoveSuccess.Log<AmServerName>(amServerName);
								return;
							}
							catch (ClusterException ex)
							{
								AmSystemManager.Tracer.TraceError<AmServerName, ClusterException>(0L, "TryToMoveGroup({0}) failed: {1}", amServerName, ex);
								ReplayCrimsonEvents.AmUnknownRoleButGroupOwnerMoveFail.Log<AmServerName, string>(amServerName, ex.Message);
							}
						}
					}
					AmSystemManager.Tracer.TraceError(0L, "No new PAM was appointed");
					ReplayCrimsonEvents.AmUnknownRoleButGroupOwnerNoOwnerChosen.Log();
				}
			}
			catch (SerializationException ex2)
			{
				AmSystemManager.Tracer.TraceError<SerializationException>(0L, "TryToMoveGroup failed: {0}", ex2);
				ReplayCrimsonEvents.AmUnknownRoleButGroupOwnerClusterFail.Log<string>(ex2.ToString());
			}
			catch (ClusterException ex3)
			{
				AmSystemManager.Tracer.TraceError<ClusterException>(0L, "TryToMoveGroup failed: {0}", ex3);
				if (amCluster != null)
				{
					ReplayCrimsonEvents.AmUnknownRoleButGroupOwnerClusterFail.Log<string>(ex3.Message);
				}
			}
			finally
			{
				if (amClusterGroup != null)
				{
					amClusterGroup.Dispose();
				}
				if (amCluster != null)
				{
					amCluster.Dispose();
				}
			}
		}

		private void OnEventGroupOwnerButUnknown(AmEvtGroupOwnerButUnknown evt)
		{
			if (this.Config.IsPamOrSam)
			{
				AmRole role = this.Config.Role;
				AmSystemManager.Tracer.TraceDebug<AmRole>(0L, "TryToDisownGroup: no longer Unknown: {0}", role);
				ReplayCrimsonEvents.AmUnknownRoleButGroupOwnerNoLongerUnknown.Log<AmRole>(role);
				return;
			}
			this.TryToDisownGroup();
		}

		private void OnEvtNodeAdded(AmEvtNodeAdded evt)
		{
			AmSystemManager.Instance.ConfigManager.TriggerRefresh(true);
		}

		private void OnEvtNodeRemoved(AmEvtNodeRemoved evt)
		{
			AmSystemManager.Instance.ConfigManager.TriggerRefresh(true);
		}

		private void OnEvtSystemFailoverOnReplayDown(AmEvtSystemFailoverOnReplayDown evt)
		{
			AmSystemFailover amSystemFailover = new AmSystemFailover(evt.ServerName, AmDbActionReason.ReplayDown, false, true);
			amSystemFailover.Run();
		}

		private void OnEvtRejoinNodeForDac(AmEvtRejoinNodeForDac evt)
		{
			IADDatabaseAvailabilityGroup dag = evt.Dag;
			if (ClusterFactory.Instance.IsEvicted(AmServerName.LocalComputerName))
			{
				ReplayEventLogConstants.Tuple_NodeNotInCluster.LogEvent(AmServerName.LocalComputerName.NetbiosName, new object[]
				{
					AmServerName.LocalComputerName.NetbiosName,
					dag.Name
				});
				return;
			}
			if (!dag.StartedMailboxServers.Contains(AmServerName.LocalComputerName.Fqdn))
			{
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(false, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 2305, "OnEvtRejoinNodeForDac", "f:\\15.00.1497\\sources\\dev\\cluster\\src\\Replay\\ActiveManager\\AmSystemManager.cs");
				DatabaseAvailabilityGroup databaseAvailabilityGroup = tenantOrTopologyConfigurationSession.Read<DatabaseAvailabilityGroup>(dag.Id);
				MultiValuedProperty<string> startedMailboxServers = databaseAvailabilityGroup.StartedMailboxServers;
				if (!startedMailboxServers.Contains(AmServerName.LocalComputerName.Fqdn))
				{
					startedMailboxServers.Add(AmServerName.LocalComputerName.Fqdn);
					databaseAvailabilityGroup.StartedMailboxServers = startedMailboxServers;
					try
					{
						tenantOrTopologyConfigurationSession.Save(databaseAvailabilityGroup);
					}
					catch (ADExternalException arg)
					{
						AmSystemManager.Tracer.TraceDebug<ADExternalException>(0L, "Failed to update AD (error={0})", arg);
					}
					catch (ADTransientException arg2)
					{
						AmSystemManager.Tracer.TraceDebug<ADTransientException>(0L, "Failed to update AD (error={0})", arg2);
					}
				}
			}
		}

		private void StartStoreServiceMonitor()
		{
			if (this.StoreServiceMonitor == null)
			{
				this.StoreServiceMonitor = new AmStoreServiceMonitor();
				this.StoreServiceMonitor.Start();
			}
		}

		private void StopStoreServiceMonitor()
		{
			if (this.StoreServiceMonitor != null)
			{
				this.StoreServiceMonitor.Stop();
				this.StoreServiceMonitor = null;
			}
		}

		private void StopClusterServiceMonitor()
		{
			if (this.ClusterServiceMonitor != null)
			{
				this.ClusterServiceMonitor.Stop();
				this.ClusterServiceMonitor = null;
			}
		}

		private static AmSystemManager sm_instance = new AmSystemManager();

		private static EnableSubComponentFlag enabledSubComponentFlags = EnableSubComponentFlag.All;

		private object m_locker = new object();

		private Stopwatch m_checkDbWatch = new Stopwatch();

		private AmServerDbStatusInfoCache m_dbStatusInfoCache = new AmServerDbStatusInfoCache();

		private ManualOneShotEvent m_configInitializedEvent = new ManualOneShotEvent("ConfigInitializedEventOccurred");
	}
}
