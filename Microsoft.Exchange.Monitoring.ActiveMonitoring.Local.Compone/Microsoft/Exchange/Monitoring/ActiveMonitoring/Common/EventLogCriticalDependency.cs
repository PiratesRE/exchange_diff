using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal sealed class EventLogCriticalDependency : ICriticalDependency
	{
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		private static extern bool MoveFileEx(string lpExistingFileName, string lpNewFileName, EventLogCriticalDependency.MoveFileFlags dwFlags);

		internal EventLogCriticalDependency(Trace trace, TracingContext traceContext)
		{
			this.trace = trace;
			this.traceContext = traceContext;
		}

		string ICriticalDependency.Name
		{
			get
			{
				return "EventLogCriticalDependency";
			}
		}

		TimeSpan ICriticalDependency.RetestDelay
		{
			get
			{
				return EventLogCriticalDependency.RetestDelay;
			}
		}

		string ICriticalDependency.EscalationService
		{
			get
			{
				return "Exchange";
			}
		}

		string ICriticalDependency.EscalationTeam
		{
			get
			{
				return "Monitoring";
			}
		}

		public bool TestCriticalDependency()
		{
			bool result = true;
			foreach (string text in EventLogCriticalDependency.CrimsonChannels)
			{
				using (EventLogReader eventLogReader = new EventLogReader(text, PathType.LogName))
				{
					try
					{
						eventLogReader.Seek(SeekOrigin.End, 0L);
						using (EventRecord eventRecord = eventLogReader.ReadEvent())
						{
							if (eventRecord != null)
							{
								WTFDiagnostics.TraceInformation<string, string, string>(this.trace, this.traceContext, "'{0}': event log '{1}' successfully read. The most recent event has timestamp '{2}'", "EventLogCriticalDependency", text, eventRecord.TimeCreated.ToString(), null, "TestCriticalDependency", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\CriticalDependencyVerification\\EventLogCriticalDependency.cs", 176);
							}
							else
							{
								WTFDiagnostics.TraceInformation<string, string>(this.trace, this.traceContext, "'{0}': event log '{1}' appears empty but not corrupt.", "EventLogCriticalDependency", text, null, "TestCriticalDependency", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\CriticalDependencyVerification\\EventLogCriticalDependency.cs", 181);
							}
						}
					}
					catch (Exception ex)
					{
						if (ex is EventLogException)
						{
							WTFDiagnostics.TraceError<string, string, string>(this.trace, this.traceContext, "'{0}': event log '{1}' appears corrupt and will be queued for deletion. Reading the log failed with exception: {2}", "EventLogCriticalDependency", text, ex.ToString(), null, "TestCriticalDependency", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\CriticalDependencyVerification\\EventLogCriticalDependency.cs", 191);
							this.corruptLogs.Add(text);
						}
						else
						{
							WTFDiagnostics.TraceError<string, string, string>(this.trace, this.traceContext, "'{0}': event log '{1}' failed but the exception does not match a known pattern of corruption: {2}", "EventLogCriticalDependency", text, ex.ToString(), null, "TestCriticalDependency", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\CriticalDependencyVerification\\EventLogCriticalDependency.cs", 201);
						}
						result = false;
					}
				}
			}
			return result;
		}

		public bool FixCriticalDependency()
		{
			bool result = true;
			if (this.corruptLogs.Count == 0)
			{
				return false;
			}
			foreach (string logName in this.corruptLogs)
			{
				string text;
				using (EventLogConfiguration eventLogConfiguration = new EventLogConfiguration(logName))
				{
					text = Environment.ExpandEnvironmentVariables(eventLogConfiguration.LogFilePath);
				}
				if (EventLogCriticalDependency.MoveFileEx(text, null, EventLogCriticalDependency.MoveFileFlags.MOVEFILE_DELAY_UNTIL_REBOOT))
				{
					WTFDiagnostics.TraceInformation<string, string>(this.trace, this.traceContext, "'{0}': queued corrupt event log file '{1}' for deletion upon next reboot.", "EventLogCriticalDependency", text, null, "FixCriticalDependency", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\CriticalDependencyVerification\\EventLogCriticalDependency.cs", 253);
				}
				else
				{
					result = false;
					WTFDiagnostics.TraceError<string, string>(this.trace, this.traceContext, "'{0}': tried but failed to queue corrupt event log file '{1}' for deletion upon next reboot.", "EventLogCriticalDependency", text, null, "FixCriticalDependency", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\CriticalDependencyVerification\\EventLogCriticalDependency.cs", 258);
				}
			}
			return result;
		}

		private const string Name = "EventLogCriticalDependency";

		private const string escalationService = "Exchange";

		private const string escalationTeam = "Monitoring";

		private static TimeSpan RetestDelay = TimeSpan.FromSeconds(5.0);

		private static readonly string[] CrimsonChannels = new string[]
		{
			"Microsoft-Exchange-ActiveMonitoring/MaintenanceDefinition",
			"Microsoft-Exchange-ActiveMonitoring/MaintenanceResult",
			"Microsoft-Exchange-ActiveMonitoring/ProbeDefinition",
			"Microsoft-Exchange-ActiveMonitoring/ProbeResult",
			"Microsoft-Exchange-ActiveMonitoring/MonitorDefinition",
			"Microsoft-Exchange-ActiveMonitoring/MonitorResult",
			"Microsoft-Exchange-ActiveMonitoring/ResponderDefinition",
			"Microsoft-Exchange-ActiveMonitoring/ResponderResult",
			"Microsoft-Exchange-ManagedAvailability/InvokeNowRequest",
			"Microsoft-Exchange-ManagedAvailability/InvokeNowResult",
			"Microsoft-Exchange-ManagedAvailability/Monitoring",
			"Microsoft-Exchange-ManagedAvailability/RecoveryActionLogs",
			"Microsoft-Exchange-ManagedAvailability/RecoveryActionResults",
			"Microsoft-Exchange-ManagedAvailability/RemoteActionLogs",
			"Microsoft-Exchange-ManagedAvailability/StartupNotification",
			"Microsoft-Exchange-ManagedAvailability/ThrottlingConfig",
			"Application",
			"System"
		};

		private List<string> corruptLogs = new List<string>();

		private Trace trace;

		private TracingContext traceContext;

		[Flags]
		private enum MoveFileFlags : uint
		{
			MOVEFILE_REPLACE_EXISTING = 1U,
			MOVEFILE_COPY_ALLOWED = 2U,
			MOVEFILE_DELAY_UNTIL_REBOOT = 4U,
			MOVEFILE_WRITE_THROUGH = 8U,
			MOVEFILE_CREATE_HARDLINK = 16U,
			MOVEFILE_FAIL_IF_NOT_TRACKABLE = 32U
		}
	}
}
