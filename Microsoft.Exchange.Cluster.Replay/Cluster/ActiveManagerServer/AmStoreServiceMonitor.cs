using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Rpc.ActiveManager;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmStoreServiceMonitor : AmServiceMonitor
	{
		internal AmStoreServiceMonitor() : base("MSExchangeIS")
		{
		}

		private static bool IsTotalInstanceName(string instanceName)
		{
			string y = "_Total";
			return StringComparer.InvariantCultureIgnoreCase.Equals(instanceName, y);
		}

		private static void ReportKillStarted()
		{
			AmStoreServiceMonitor.s_killWasTriggered = true;
			AmSystemEventCode eventCode = AmSystemEventCode.StoreServiceUnexpectedlyStopped;
			AmConfig config = AmSystemManager.Instance.Config;
			if (!config.IsUnknown && !config.IsStandalone)
			{
				AmServerName currentPAM = config.DagConfig.CurrentPAM;
				if (config.DagConfig.IsNodePubliclyUp(currentPAM))
				{
					AmTrace.Diagnostic("Reporting to PAM ({0}) that store process is being killed.", new object[]
					{
						currentPAM
					});
					AmStoreServiceMonitor.ReportStoreStatus(currentPAM, eventCode, AmServerName.LocalComputerName);
				}
			}
		}

		public static bool WasKillTriggered()
		{
			return AmStoreServiceMonitor.s_killWasTriggered;
		}

		private static bool ReportStoreStatus(AmServerName serverToContact, AmSystemEventCode eventCode, AmServerName reportingServer)
		{
			Exception ex = AmHelper.HandleKnownExceptions(delegate(object param0, EventArgs param1)
			{
				AmRpcClientHelper.ReportSystemEvent(serverToContact.Fqdn, (int)eventCode, reportingServer.Fqdn);
			});
			if (ex != null)
			{
				AmServiceMonitor.Tracer.TraceError(0L, "Failed to report status to PAM (pam={0}, eventCode={1}, reportingServer={2}): error={3}", new object[]
				{
					serverToContact,
					eventCode,
					reportingServer,
					ex.Message
				});
				return false;
			}
			return true;
		}

		private void ReportStart()
		{
			AmConfig config = AmSystemManager.Instance.Config;
			this.ReportStoreStartedToReplayManager();
			if (config.IsUnknown)
			{
				AmServiceMonitor.Tracer.TraceError(0L, "Store service start detected, but configuration is in unknown state");
				return;
			}
			if (config.IsStandalone)
			{
				AmStoreServiceMonitor.ReportStoreStatus(AmServerName.LocalComputerName, AmSystemEventCode.StoreServiceStarted, AmServerName.LocalComputerName);
				this.m_isStartReported = true;
				return;
			}
			AmServerName currentPAM = config.DagConfig.CurrentPAM;
			if (config.DagConfig.IsNodePubliclyUp(currentPAM))
			{
				AmStoreServiceMonitor.ReportStoreStatus(currentPAM, AmSystemEventCode.StoreServiceStarted, AmServerName.LocalComputerName);
				this.m_isStartReported = true;
				return;
			}
			AmServiceMonitor.Tracer.TraceInformation<AmServerName>(0, 0L, "Store service monitor is not reporting the store start to the PAM, since PAM on '{0}'is not up yet. Store monitor will retry once the node is up", currentPAM);
		}

		private void ReportStop()
		{
			AmConfig config = AmSystemManager.Instance.Config;
			AmServiceMonitor.Tracer.TraceInformation(0, 0L, "Store service stopped unexpectedly.");
			AmSystemEventCode eventCode = AmSystemEventCode.StoreServiceUnexpectedlyStopped;
			if (config.IsUnknown)
			{
				AmServiceMonitor.Tracer.TraceError(0L, "Store service stop detected, but configuration is in unknown state");
				return;
			}
			if (config.IsStandalone)
			{
				AmStoreServiceMonitor.ReportStoreStatus(AmServerName.LocalComputerName, eventCode, AmServerName.LocalComputerName);
				this.m_isStopReported = true;
				this.m_isStopObserved = false;
				return;
			}
			AmServerName currentPAM = config.DagConfig.CurrentPAM;
			if (config.DagConfig.IsNodePubliclyUp(currentPAM))
			{
				AmStoreServiceMonitor.ReportStoreStatus(currentPAM, eventCode, AmServerName.LocalComputerName);
				this.m_isStopReported = true;
				this.m_isStopObserved = false;
				return;
			}
			AmServiceMonitor.Tracer.TraceInformation<AmServerName>(0, 0L, "Store service monitor is not reporting the store stop to the PAM, since PAM on '{0}'is not up yet. Store monitor will retry once the node is up", currentPAM);
		}

		protected override void OnStart()
		{
			AmStoreServiceMonitor.s_killWasTriggered = false;
			this.m_isStartReported = false;
			this.m_isStopObserved = false;
			try
			{
				ExTraceGlobals.FaultInjectionTracer.TraceTest(2154179901U);
				this.ReportStart();
			}
			catch (ClusterException arg)
			{
				AmServiceMonitor.Tracer.TraceError<ClusterException>(0L, "Failed to set store service status clusdb. Exception={0}", arg);
			}
			if (!this.m_isStartReported)
			{
				ReplayCrimsonEvents.StoreServiceStartDetectedButConfigInvalid.Log();
			}
			this.OnWaitingForStop();
		}

		protected override void OnStop()
		{
			this.m_isStopReported = false;
			this.m_isStopObserved = true;
			AmTrace.Debug("AmStoreServiceMonitor.OnStop clearing AM counters.", new object[0]);
			ExTraceGlobals.FaultInjectionTracer.TraceTest(3764792637U);
			foreach (string instanceName in ActiveManagerPerfmon.GetInstanceNames())
			{
				if (!AmStoreServiceMonitor.IsTotalInstanceName(instanceName))
				{
					ActiveManagerPerfmon.ResetInstance(instanceName);
				}
			}
			this.OnWaitingForStart();
		}

		protected override void OnWaitingForStop()
		{
			if (!this.m_isStartReported)
			{
				try
				{
					this.ReportStart();
				}
				catch (ClusterException arg)
				{
					AmServiceMonitor.Tracer.TraceError<ClusterException>(0L, "Failed to set store service status in clusdb. Exception={0}", arg);
				}
			}
		}

		protected override void OnWaitingForStart()
		{
			if (this.m_isStopObserved && !this.m_isStopReported)
			{
				try
				{
					this.ReportStop();
				}
				catch (ClusterException arg)
				{
					AmServiceMonitor.Tracer.TraceError<ClusterException>(0L, "Failed to notify store stopped status to active manager. Exception={0}", arg);
				}
			}
		}

		protected override bool IsServiceReady()
		{
			bool flag = AmStoreHelper.IsStoreRunning(AmServerName.LocalComputerName);
			AmTrace.Debug("AmStoreHelper.IsStoreRunning() returned {0}", new object[]
			{
				flag
			});
			return flag;
		}

		private bool ReportStoreStartedToReplayManager()
		{
			bool result = false;
			IRunConfigurationUpdater configurationUpdater = Dependencies.ConfigurationUpdater;
			try
			{
				configurationUpdater.RunConfigurationUpdater(false, ReplayConfigChangeHints.AmStoreServiceStartDetected);
				result = true;
			}
			catch (TaskServerTransientException arg)
			{
				AmServiceMonitor.Tracer.TraceError<TaskServerTransientException>(0L, "ReportStoreStartedToReplayManager() failed with exception: {0}", arg);
			}
			catch (TaskServerException arg2)
			{
				AmServiceMonitor.Tracer.TraceError<TaskServerException>(0L, "ReportStoreStartedToReplayManager() failed with exception: {0}", arg2);
			}
			return result;
		}

		public static Exception KillStoreIfRunningBefore(DateTime limitTimeUtc, string reason)
		{
			Exception ex = null;
			if (AmStoreServiceMonitor.s_lastKillTimeUtc != null && AmStoreServiceMonitor.s_lastKillTimeUtc >= limitTimeUtc)
			{
				AmServiceMonitor.Tracer.TraceDebug<DateTime>(0L, "KillStoreIfRunningBefore ignores Kill since store last killed at {0}UTC", AmStoreServiceMonitor.s_lastKillTimeUtc.Value);
				return ex;
			}
			if (Interlocked.CompareExchange(ref AmStoreServiceMonitor.s_numThreadsInStoreKill, 1, 0) == 1)
			{
				AmServiceMonitor.Tracer.TraceDebug(0L, "KillStoreIfRunningBefore ignores Kill since another thread is currently killing store.");
				return ex;
			}
			Exception result;
			try
			{
				AmServiceMonitor.Tracer.TraceDebug<string>(0L, "KillStoreIfRunningBefore killing now. Reason: {0}", reason);
				ex = AmStoreServiceMonitor.TryCrashingStoreGracefully();
				if (ex != null)
				{
					ReplayEventLogConstants.Tuple_AmFailedToStopService.LogEvent(null, new object[]
					{
						"MSExchangeIS",
						ex.ToString(),
						reason
					});
				}
				else
				{
					ReplayEventLogConstants.Tuple_AmKilledStoreToForceDismount.LogEvent(null, new object[]
					{
						reason
					});
				}
				AmStoreServiceMonitor.s_lastKillTimeUtc = new DateTime?(DateTime.UtcNow);
				result = ex;
			}
			finally
			{
				if (Interlocked.CompareExchange(ref AmStoreServiceMonitor.s_numThreadsInStoreKill, 0, 1) == 0)
				{
					DiagCore.RetailAssert(false, "We should not have more than 1 thread in KillStore()", new object[0]);
				}
			}
			return result;
		}

		internal static Exception TryCrashingStoreGracefully()
		{
			Exception ex = null;
			Process storeProcess = null;
			EventWaitHandle crashControlAckEvent = null;
			try
			{
				ReplayCrimsonEvents.InitiatingGracefulStoreCrash.Log();
				storeProcess = ServiceOperations.GetServiceProcess("MSExchangeIS", out ex);
				if (ex == null)
				{
					ex = ServiceOperations.RunOperation(delegate(object param0, EventArgs param1)
					{
						if (!RegistryParameters.KillStoreInsteadOfWatsonOnTimeout)
						{
							crashControlAckEvent = new EventWaitHandle(false, EventResetMode.ManualReset, "Global\\17B584B2-A9E0-45CF-87CB-7774112D6CB9");
							ThreadPool.QueueUserWorkItem(delegate(object param0)
							{
								Exception ex2 = ServiceOperations.ControlService("MSExchangeIS", 130);
								if (ex2 != null)
								{
									AmTrace.Debug("ControlService() failed with {0}", new object[]
									{
										ex2.Message
									});
								}
							});
						}
						else
						{
							AmTrace.Diagnostic("Killing store instead of taking a Watson dump due to registry override.", new object[0]);
						}
						if (crashControlAckEvent != null)
						{
							if (crashControlAckEvent.WaitOne(RegistryParameters.StoreCrashControlCodeAckTimeoutInMSec))
							{
								AmStoreServiceMonitor.ReportKillStarted();
								Stopwatch stopwatch = new Stopwatch();
								stopwatch.Start();
								if (!storeProcess.WaitForExit(RegistryParameters.StoreWatsonDumpTimeoutInMSec))
								{
									AmTrace.Diagnostic("Store process did not finish taking dump in {0} msecs", new object[]
									{
										RegistryParameters.StoreWatsonDumpTimeoutInMSec
									});
								}
								else
								{
									AmTrace.Diagnostic("Store process finished taking dump in {0} msecs", new object[]
									{
										stopwatch.Elapsed.TotalMilliseconds
									});
								}
							}
							else
							{
								AmTrace.Diagnostic("Store failed to acknowledge that it received the crash control code in {0} msecs.", new object[]
								{
									RegistryParameters.StoreCrashControlCodeAckTimeoutInMSec
								});
								AmStoreServiceMonitor.ReportKillStarted();
							}
						}
						else
						{
							AmStoreServiceMonitor.ReportKillStarted();
						}
						if (!storeProcess.HasExited)
						{
							if (crashControlAckEvent != null)
							{
								AmTrace.Diagnostic("Store process is still running even after the graceful attempt. Force killing it.", new object[0]);
							}
							storeProcess.Kill();
							TimeSpan timeSpan = TimeSpan.FromMilliseconds((double)RegistryParameters.StoreKillBugcheckTimeoutInMSec);
							if (!storeProcess.WaitForExit(RegistryParameters.StoreKillBugcheckTimeoutInMSec))
							{
								ExDateTime storeKillBugcheckDisabledTime = RegistryParameters.StoreKillBugcheckDisabledTime;
								string text = string.Format("Store process is still running {0} secs after attempt to force kill it.", timeSpan.TotalSeconds);
								if (storeKillBugcheckDisabledTime > ExDateTime.UtcNow)
								{
									AmTrace.Debug("Store bugcheck has been disabled by regkey '{0}' until '{1}'.", new object[]
									{
										"StoreKillBugcheckDisabledTime",
										storeKillBugcheckDisabledTime
									});
									ReplayCrimsonEvents.StoreBugCheckDisabledUntilTime.LogPeriodic<string, string, ExDateTime>(Environment.MachineName, DiagCore.DefaultEventSuppressionInterval, text, "StoreKillBugcheckDisabledTime", storeKillBugcheckDisabledTime);
									return;
								}
								AmTrace.Debug("Attempting to bugcheck the system. Reason: {0}", new object[]
								{
									text
								});
								BugcheckHelper.TriggerBugcheckIfRequired(DateTime.UtcNow, text);
								return;
							}
							else
							{
								AmTrace.Diagnostic("Store process has been forcefully killed.", new object[0]);
							}
						}
					});
				}
			}
			finally
			{
				ReplayCrimsonEvents.FinishedGracefulStoreCrash.Log<string>((ex != null) ? ex.Message : "<none>");
				if (crashControlAckEvent != null)
				{
					crashControlAckEvent.Close();
				}
				if (storeProcess != null)
				{
					storeProcess.Dispose();
				}
			}
			return ex;
		}

		private const int StoreStoppedEventId = 906;

		private const int StoreStoppedEventPidIndex = 1;

		public const int StoreDismountCrashServiceControlCode = 130;

		public const string StoreDismountCrashAckEventName = "Global\\17B584B2-A9E0-45CF-87CB-7774112D6CB9";

		private static TimeSpan storeStoppedEventSearchMaxDuration = new TimeSpan(0, 5, 0);

		private bool m_isStartReported;

		private bool m_isStopReported;

		private bool m_isStopObserved;

		private static DateTime? s_lastKillTimeUtc;

		private static bool s_killWasTriggered = false;

		private static int s_numThreadsInStoreKill = 0;
	}
}
