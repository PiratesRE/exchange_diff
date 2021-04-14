using System;
using System.Threading;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Diagnostics.Audit;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal class ForceRebootHelper
	{
		internal static Tuple<RecoveryActionEntry, RecoveryActionEntry> PublishRebootActionEntryIfRequired()
		{
			lock (ForceRebootHelper.publishLock)
			{
				try
				{
					return ForceRebootHelper.PublishRebootActionEntryIfRequiredInternal();
				}
				catch (Exception ex)
				{
					ManagedAvailabilityCrimsonEvents.ActiveMonitoringUnexpectedError.Log<string, string>("PublishRebootActionEntryIfRequired", ex.Message);
				}
			}
			return null;
		}

		internal static void ReportBugcheckToOtherServersInGroup(string[] servers, string reportingServer, RecoveryActionId actionId, string requester, string statsAsString, string timeStampStr, TimeSpan rpcTimeout, TimeSpan overallTimeout)
		{
			Math.Ceiling((double)servers.Length * 70.0 / 100.0);
			using (DistributedAction distributedAction = new DistributedAction(servers, -1, false))
			{
				distributedAction.Run(delegate(string serverName)
				{
					RpcLogCrimsonEventImpl.SendRequest(serverName, "BugcheckReportedByRemoteServer", false, rpcTimeout, new object[]
					{
						reportingServer,
						actionId,
						requester,
						statsAsString,
						timeStampStr
					});
				}, overallTimeout);
			}
		}

		private static Tuple<RecoveryActionEntry, RecoveryActionEntry> PublishRebootActionEntryIfRequiredInternal()
		{
			RecoveryActionEntry recoveryActionEntry = null;
			RecoveryActionEntry recoveryActionEntry2 = null;
			Exception ex = null;
			DateTime dateTime = RecoveryActionHelper.GetSystemBootTime(out ex);
			if (dateTime == DateTime.MinValue || ex != null)
			{
				ManagedAvailabilityCrimsonEvents.ActiveMonitoringUnexpectedError.Log<string, string>(string.Format("GetSystemBootTime() failed (BootTime={0})", dateTime), (ex != null) ? ex.Message : "<null>");
				dateTime = ExDateTime.Now.LocalTime;
			}
			DateTime propertyDateTime = RegistryHelper.GetPropertyDateTime("SystemStartTime", DateTime.MinValue, null, null, false);
			DateTime propertyDateTime2 = RegistryHelper.GetPropertyDateTime("ActualBugCheckInitiatedTime", DateTime.MinValue, null, null, false);
			WTFDiagnostics.TraceDebug<DateTime, DateTime, DateTime>(ExTraceGlobals.RecoveryActionTracer, ForceRebootHelper.traceContext, "PublishRebootActionEntryIfRequiredInternal: (LastMarkedSystemStartTime: {0}, LastBugcheckReportedTime: {1}, currentSystemStartTime: {2})", propertyDateTime, propertyDateTime2, dateTime, null, "PublishRebootActionEntryIfRequiredInternal", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Utils\\ForceRebootHelper.cs", 159);
			if (dateTime > propertyDateTime)
			{
				WTFDiagnostics.TraceDebug<DateTime, DateTime>(ExTraceGlobals.RecoveryActionTracer, ForceRebootHelper.traceContext, "Publishing the last reboot status to the action table. (currentStartTime: {0}, lastMarkedStartTime: {1})", dateTime, propertyDateTime, null, "PublishRebootActionEntryIfRequiredInternal", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Utils\\ForceRebootHelper.cs", 170);
				PersistedGlobalActionEntry persistedGlobalActionEntry = PersistedGlobalActionEntry.ReadFromFile(RecoveryActionId.ForceReboot);
				DateTime dateTime2 = DateTime.MinValue;
				string finishContext = string.Empty;
				bool flag = false;
				if (persistedGlobalActionEntry != null && persistedGlobalActionEntry.ReportedTime > propertyDateTime2)
				{
					dateTime2 = persistedGlobalActionEntry.ReportedTime;
					finishContext = persistedGlobalActionEntry.FinishEntryContext;
					recoveryActionEntry = persistedGlobalActionEntry.ConvertToRecoveryActionStartEntry();
					flag = true;
				}
				if (recoveryActionEntry == null)
				{
					recoveryActionEntry = RecoveryActionHelper.ConstructStartActionEntry(RecoveryActionId.ForceReboot, Environment.MachineName, "ManagedAvailabilityStartup", dateTime, null, null, "System Generated - No trace of xml log about the responder that initiated bugcheck", null);
					TimeSpan t = TimeSpan.FromMinutes(5.0);
					if (BugcheckSimulator.Instance.IsEnabled)
					{
						t = BugcheckSimulator.Instance.Duration;
					}
					recoveryActionEntry.StartTime = dateTime - t;
				}
				if (dateTime2 == DateTime.MinValue)
				{
					dateTime2 = recoveryActionEntry.StartTime;
				}
				recoveryActionEntry.CustomArg3 = "This entry was constructed post reboot by Managed availability startup";
				recoveryActionEntry.Write(null);
				recoveryActionEntry2 = RecoveryActionHelper.ConstructFinishActionEntry(recoveryActionEntry, null, new DateTime?(dateTime), finishContext);
				recoveryActionEntry2.CustomArg3 = "This entry was constructed post reboot by Managed availability startup";
				recoveryActionEntry2.Write(null);
				RegistryHelper.SetPropertyDateTime("SystemStartTime", dateTime, null, null, false);
				if (flag)
				{
					RegistryHelper.SetPropertyDateTime("ActualBugCheckInitiatedTime", dateTime2, null, null, false);
				}
			}
			else
			{
				WTFDiagnostics.TraceDebug(ExTraceGlobals.RecoveryActionTracer, ForceRebootHelper.traceContext, "Skipped publishing the last reboot status to the action table since it was already reported.", null, "PublishRebootActionEntryIfRequiredInternal", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Utils\\ForceRebootHelper.cs", 255);
			}
			return new Tuple<RecoveryActionEntry, RecoveryActionEntry>(recoveryActionEntry, recoveryActionEntry2);
		}

		internal static void PerformBugcheck()
		{
			Exception exception = null;
			Privilege.RunWithPrivilege("SeDebugPrivilege", true, delegate
			{
				if (!BugcheckSimulator.Instance.IsEnabled)
				{
					WTFDiagnostics.TraceDebug(ExTraceGlobals.RecoveryActionTracer, ForceRebootHelper.traceContext, "Attempting to kill wininit.exe", null, "PerformBugcheck", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Utils\\ForceRebootHelper.cs", 280);
					exception = BugcheckHelper.KillWinInitProcess();
					return;
				}
				BugcheckSimulator.Instance.TakeActionIfRequired();
			});
			if (exception != null)
			{
				throw new BugCheckActionFailedException(exception.Message, exception);
			}
			Thread.Sleep(TimeSpan.FromSeconds(10.0));
			throw new BugCheckActionFailedException("Killing WinInit process did not bugcheck the machine");
		}

		public const int BootTimeCalculationAdjustmentInSeconds = 10;

		private const string PostCreationHint = "This entry was constructed post reboot by Managed availability startup";

		private static TracingContext traceContext = TracingContext.Default;

		private static object publishLock = new object();
	}
}
