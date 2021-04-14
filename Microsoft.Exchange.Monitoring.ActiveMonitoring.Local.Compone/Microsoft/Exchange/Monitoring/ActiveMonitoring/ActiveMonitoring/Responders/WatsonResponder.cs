using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Search;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;
using Microsoft.Web.Administration;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders
{
	public class WatsonResponder : ResponderWorkItem
	{
		public static ResponderDefinition CreateDefinition(string name, string serviceName, string alertTypeId, string alertMask, string targetResource, ServiceHealthStatus targetHealthState, string exceptionType, string watsonEventType, ReportOptions reportOptions)
		{
			if (targetHealthState == ServiceHealthStatus.None)
			{
				throw new ArgumentException("The responder does not support ServiceHealthStatus.None as target health state.", "targetHealthState");
			}
			if (string.IsNullOrEmpty(exceptionType))
			{
				throw new ArgumentNullException("Parameter cannot be null or empty.", "exceptionType");
			}
			if (string.IsNullOrEmpty(watsonEventType))
			{
				throw new ArgumentNullException("Parameter cannot be null or empty.", "watsonEventType");
			}
			if (!WatsonResponder.SupportedWatsonEventSet.Contains(watsonEventType))
			{
				throw new ArgumentException(string.Format("The responder does not support the event type '{0}'", watsonEventType));
			}
			if (string.IsNullOrWhiteSpace(targetResource))
			{
				throw new ArgumentException("The responder needs to have a process that needs a watson.", "targetResource");
			}
			ResponderDefinition responderDefinition = new ResponderDefinition();
			responderDefinition.AssemblyPath = WatsonResponder.AssemblyPath;
			responderDefinition.TypeName = WatsonResponder.TypeName;
			responderDefinition.Name = name;
			responderDefinition.ServiceName = serviceName;
			responderDefinition.AlertTypeId = alertTypeId;
			responderDefinition.AlertMask = alertMask;
			responderDefinition.TargetResource = targetResource;
			responderDefinition.TargetHealthState = targetHealthState;
			responderDefinition.RecurrenceIntervalSeconds = 300;
			responderDefinition.WaitIntervalSeconds = 300;
			responderDefinition.TimeoutSeconds = 300;
			responderDefinition.MaxRetryAttempts = 3;
			responderDefinition.Enabled = VariantConfiguration.InvariantNoFlightingSnapshot.ActiveMonitoring.WatsonResponder.Enabled;
			responderDefinition.Attributes["EdsProcessName"] = targetResource;
			responderDefinition.Attributes["ExceptionType"] = exceptionType;
			responderDefinition.Attributes["WatsonEventType"] = watsonEventType;
			responderDefinition.Attributes["ReportOptions"] = reportOptions.ToString();
			responderDefinition.Attributes["FilesToUpload"] = string.Empty;
			responderDefinition.Attributes["AllowDefaultCallStack"] = true.ToString();
			RecoveryActionRunner.SetThrottleProperties(responderDefinition, "Dag", RecoveryActionId.Watson, targetResource, null);
			return responderDefinition;
		}

		protected static void TraceDebug(TracingContext traceContext, string formatString)
		{
			WTFDiagnostics.TraceDebug(ExTraceGlobals.CommonComponentsTracer, traceContext, formatString, null, "TraceDebug", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Responders\\WatsonResponder.cs", 148);
		}

		protected override void DoResponderWork(CancellationToken cancellationToken)
		{
			RecoveryActionRunner recoveryActionRunner = new RecoveryActionRunner(RecoveryActionId.Watson, base.Definition.TargetResource, this, true, cancellationToken, null);
			base.Result.StateAttribute1 = base.Definition.TargetResource;
			base.Result.StateAttribute2 = base.Definition.TargetExtension;
			recoveryActionRunner.Execute(delegate()
			{
				IDataAccessQuery<ResponderResult> lastSuccessfulResponderResult = this.Broker.GetLastSuccessfulResponderResult(this.Definition);
				Task<ResponderResult> task = lastSuccessfulResponderResult.ExecuteAsync(cancellationToken, this.TraceContext);
				WatsonResponder.TraceDebug(this.TraceContext, "WatsonResponder.DoResponderWork: analyzing last result");
				task.Continue(delegate(ResponderResult lastResponderResult)
				{
					DateTime startTime = SqlDateTime.MinValue.Value;
					if (lastResponderResult != null)
					{
						startTime = lastResponderResult.ExecutionStartTime;
					}
					IDataAccessQuery<MonitorResult> lastSuccessfulMonitorResult = this.Broker.GetLastSuccessfulMonitorResult(this.Definition.AlertMask, startTime, this.Result.ExecutionStartTime);
					Task<MonitorResult> task2 = lastSuccessfulMonitorResult.ExecuteAsync(cancellationToken, this.TraceContext);
					task2.Continue(delegate(MonitorResult lastMonitorResult)
					{
						string text = this.Definition.Attributes["EdsProcessName"];
						string text2 = this.Definition.Attributes["WatsonEventType"];
						string text3 = this.Definition.Attributes["ExceptionType"];
						ReportOptions reportOptions;
						if (!Enum.TryParse<ReportOptions>(this.Definition.Attributes["ReportOptions"], out reportOptions))
						{
							reportOptions = ReportOptions.None;
						}
						bool flag;
						if (!bool.TryParse(this.Definition.Attributes["AllowDefaultCallStack"], out flag))
						{
							flag = true;
						}
						string formatString = string.Format("WatsonResponder.DoResponderWork: Setting up {0} Watson on {1} with exception: {2}. ReportOptions: {3}.", new object[]
						{
							text2,
							text,
							text3,
							reportOptions
						});
						WatsonResponder.TraceDebug(this.TraceContext, formatString);
						Process[] processesFromEdsName = WatsonResponder.GetProcessesFromEdsName(text, this.Definition.TargetExtension);
						if (processesFromEdsName.Length > 0)
						{
							foreach (Process process in processesFromEdsName)
							{
								using (process)
								{
									string text4 = this.CollectCallstack(process);
									if (string.IsNullOrEmpty(text4) || (!flag && text4.Equals(WatsonResponder.DefaultCallStack, StringComparison.OrdinalIgnoreCase)))
									{
										this.Result.RecoveryResult = ServiceRecoveryResult.Skipped;
										this.Result.IsRecoveryAttempted = false;
									}
									else
									{
										string extraData = this.GetExtraData(cancellationToken);
										string[] filesToUpload = this.GetFilesToUpload();
										Exception exceptionObject = this.GetExceptionObject(text3, text4);
										exceptionObject.Source = WatsonResponder.GetWatsonBugRoutingProcessName(text);
										ExWatson.SendExternalProcessWatsonReportWithFiles(process, text2, exceptionObject, extraData, filesToUpload, reportOptions);
										WatsonResponder.TraceDebug(this.TraceContext, string.Format("WatsonResponder.DoResponderWork: {0} Watson sent on {1} with Exception {2}", text2, text, text3));
									}
								}
							}
							return;
						}
						WatsonResponder.TraceDebug(this.TraceContext, string.Format("WatsonResponder.DoResponderWork: Failed to retrieve valid process id for {0}.", text));
					}, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled);
				}, cancellationToken, TaskContinuationOptions.AttachedToParent);
			});
		}

		internal virtual string CollectCallstack(Process process)
		{
			return WatsonResponder.DefaultCallStack;
		}

		protected virtual string[] GetFilesToUpload()
		{
			string text = base.Definition.Attributes["FilesToUpload"];
			if (string.IsNullOrEmpty(text))
			{
				return new string[0];
			}
			return text.Split(new char[]
			{
				';'
			});
		}

		protected virtual string GetExtraData(CancellationToken cancellationToken)
		{
			string text = string.Empty;
			ProbeResult lastFailedProbeResult = WorkItemResultHelper.GetLastFailedProbeResult(this, base.Broker, cancellationToken);
			if (lastFailedProbeResult != null)
			{
				text = lastFailedProbeResult.Error;
				if (!string.IsNullOrEmpty(text))
				{
					text = text.Replace("\\r\\n", Environment.NewLine);
				}
			}
			return text;
		}

		internal virtual Exception GetExceptionObject(string exceptionType, string callstack)
		{
			if (string.IsNullOrEmpty(exceptionType))
			{
				throw new ArgumentException("Parameter may not be null or empty", "exceptionType");
			}
			switch (exceptionType)
			{
			case "ProcessProcessorTimeWarningException":
				return new WatsonResponder.ProcessProcessorTimeException(callstack);
			case "ProcessProcessorTimeErrorException":
				return new WatsonResponder.ProcessProcessorTimeException(callstack);
			case "PrivateWorkingSetWarningException":
				return new WatsonResponder.PrivateWorkingSetException(callstack);
			case "PrivateWorkingSetErrorException":
				return new WatsonResponder.PrivateWorkingSetException(callstack);
			case "VersionBucketsAllocatedWatsonResponderException":
				return new WatsonResponder.VersionBucketsAllocatedException(callstack);
			case "DatabasePercentRPCRequestsWatsonResponderException":
				return new WatsonResponder.DatabasePercentRPCRequestsException(callstack);
			case "MailboxAssistantsWatermarksWatsonResponderException":
				return new WatsonResponder.MailboxAssistantsWatermarksBehindException(callstack);
			}
			return new WatsonResponder.ResponderException(string.Empty, callstack);
		}

		private static string GetWatsonBugRoutingProcessName(string processName)
		{
			if (processName.EndsWith("apppool", StringComparison.OrdinalIgnoreCase) && !processName.StartsWith("w3wp#", StringComparison.OrdinalIgnoreCase))
			{
				return "w3wp#" + processName;
			}
			return processName;
		}

		private static Process[] GetProcessesFromEdsName(string processName, string targetExtension)
		{
			if (processName.StartsWith("noderunner#", StringComparison.OrdinalIgnoreCase))
			{
				return WatsonResponder.GetProcessesForNodeRunner(processName);
			}
			if (processName.EndsWith("apppool", StringComparison.OrdinalIgnoreCase))
			{
				using (ServerManager serverManager = new ServerManager())
				{
					return ApplicationPoolHelper.GetRunningProcessesForAppPool(serverManager, processName);
				}
			}
			if (processName.Equals("Microsoft.Exchange.Store.Worker.exe", StringComparison.InvariantCultureIgnoreCase))
			{
				return StoreMonitoringHelpers.GetStoreWorkerProcess(targetExtension);
			}
			return Process.GetProcessesByName(processName) ?? new Process[0];
		}

		private static Process[] GetProcessesForNodeRunner(string nodeRunnerInstanceName)
		{
			Dictionary<string, int> nodeProcessIds = SearchMonitoringHelper.GetNodeProcessIds();
			foreach (string text in nodeProcessIds.Keys)
			{
				if (nodeRunnerInstanceName.EndsWith(text, StringComparison.OrdinalIgnoreCase))
				{
					return new Process[]
					{
						Process.GetProcessById(nodeProcessIds[text])
					};
				}
			}
			return new Process[0];
		}

		public static readonly string DefaultCallStack = "Unknown";

		private static readonly string TypeName = typeof(WatsonResponder).FullName;

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly HashSet<string> SupportedWatsonEventSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"E12",
			"E12IIS"
		};

		public class ResponderException : Exception
		{
			public ResponderException(string message, string callstack) : base(message)
			{
				this.stackTrace = callstack;
			}

			public override string StackTrace
			{
				get
				{
					return this.stackTrace.ToString();
				}
			}

			public override string ToString()
			{
				return string.Format("{0}: {1}{2}{3}", new object[]
				{
					base.GetType(),
					this.Message,
					Environment.NewLine,
					this.stackTrace
				});
			}

			private readonly string stackTrace;
		}

		public class ProcessProcessorTimeException : WatsonResponder.ResponderException
		{
			public ProcessProcessorTimeException(string callstack) : base("Process has exceeded its processor time threshold. The following hot path was parsed from the F1:", callstack)
			{
			}

			private const string ExceptionMessage = "Process has exceeded its processor time threshold. The following hot path was parsed from the F1:";
		}

		public class VersionBucketsAllocatedException : WatsonResponder.ResponderException
		{
			public VersionBucketsAllocatedException(string callstack) : base("Version Buckets Allocated perf counter has exceeded its threshold", callstack)
			{
			}

			private const string ExceptionMessage = "Version Buckets Allocated perf counter has exceeded its threshold";
		}

		public class DatabasePercentRPCRequestsException : WatsonResponder.ResponderException
		{
			public DatabasePercentRPCRequestsException(string callstack) : base("% RPC Requests perf counter has exceeded its threshold", callstack)
			{
			}

			private const string ExceptionMessage = "% RPC Requests perf counter has exceeded its threshold";
		}

		public class MailboxAssistantsWatermarksBehindException : WatsonResponder.ResponderException
		{
			public MailboxAssistantsWatermarksBehindException(string callstack) : base("Mailbox assistants watermarks have been behind past current threshold", callstack)
			{
			}

			private const string ExceptionMessage = "Mailbox assistants watermarks have been behind past current threshold";
		}

		public class PrivateWorkingSetException : WatsonResponder.ResponderException
		{
			public PrivateWorkingSetException(string callstack) : base("Process has exceeded its private working set threshold", callstack)
			{
			}

			private const string ExceptionMessage = "Process has exceeded its private working set threshold";
		}

		internal static class AttributeNames
		{
			internal const string EdsProcessName = "EdsProcessName";

			internal const string ExceptionType = "ExceptionType";

			internal const string WatsonEventType = "WatsonEventType";

			internal const string ReportOptions = "ReportOptions";

			internal const string FilesToUpload = "FilesToUpload";

			internal const string AllowDefaultCallStack = "AllowDefaultCallStack";
		}
	}
}
