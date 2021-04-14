using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;
using Microsoft.Win32;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Fips.Responders
{
	public class CollectFIPSLogsResponder : ResponderWorkItem
	{
		private string LogDestination { get; set; }

		private int LifeTimeSeconds { get; set; }

		internal static ResponderDefinition CreateDefinition(string name, string serviceName, string alertTypeId, string alertMask, string targetResource, ServiceHealthStatus targetHealthState, int recurrenceIntervalSeconds = 900, int waitIntervalSeconds = 28800, int timeoutSeconds = 600, int maxRetryAttempts = 1, string logDestination = null, int logLifeTimeSeconds = 432000, bool enabled = true)
		{
			ResponderDefinition responderDefinition = new ResponderDefinition();
			responderDefinition.AssemblyPath = CollectFIPSLogsResponder.AssemblyPath;
			responderDefinition.TypeName = CollectFIPSLogsResponder.TypeName;
			responderDefinition.Name = name;
			responderDefinition.ServiceName = serviceName;
			responderDefinition.AlertTypeId = alertTypeId;
			responderDefinition.AlertMask = alertMask;
			responderDefinition.TargetResource = targetResource;
			responderDefinition.TargetHealthState = targetHealthState;
			responderDefinition.RecurrenceIntervalSeconds = recurrenceIntervalSeconds;
			responderDefinition.WaitIntervalSeconds = waitIntervalSeconds;
			responderDefinition.TimeoutSeconds = timeoutSeconds;
			responderDefinition.MaxRetryAttempts = maxRetryAttempts;
			responderDefinition.Attributes["LifeTimeSeconds"] = logLifeTimeSeconds.ToString();
			if (!string.IsNullOrEmpty(logDestination))
			{
				CollectFIPSLogsResponder.TestDirectoryPath(logDestination);
				responderDefinition.Attributes["LogDestination"] = logDestination;
			}
			else
			{
				responderDefinition.Attributes["LogDestination"] = CollectFIPSLogsResponder.DefaultValues.LogDestination;
			}
			responderDefinition.Enabled = enabled;
			return responderDefinition;
		}

		protected override void DoResponderWork(CancellationToken cancellationToken)
		{
			this.InitializeAttributes();
			IDataAccessQuery<ResponderResult> lastSuccessfulRecoveryAttemptedResponderResult = base.Broker.GetLastSuccessfulRecoveryAttemptedResponderResult(base.Definition, TimeSpan.FromSeconds((double)base.Definition.WaitIntervalSeconds));
			Task<ResponderResult> task = lastSuccessfulRecoveryAttemptedResponderResult.ExecuteAsync(cancellationToken, base.TraceContext);
			task.Continue(delegate(ResponderResult lastResponderResult)
			{
				if (lastResponderResult != null)
				{
					base.Result.IsThrottled = (DateTime.Parse(lastResponderResult.StateAttribute1) > base.Result.ExecutionStartTime);
				}
				WTFDiagnostics.TraceDebug<string>(ExTraceGlobals.FIPSTracer, base.TraceContext, "CollectFIPSLogsResponder.DoResponderWork: Throttled:{1}", base.Result.IsThrottled.ToString(), null, "DoResponderWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\FIPS\\CollectFIPSLogsResponder.cs", 157);
				if (!base.Result.IsThrottled)
				{
					Dictionary<string, string> dictionary = new Dictionary<string, string>();
					if (this.LogDestination != null)
					{
						dictionary.Add("Path", this.LogDestination);
					}
					FipsUtils.RunFipsCmdlet<string>("Start-Diagnostics", dictionary);
					WTFDiagnostics.TraceDebug(ExTraceGlobals.FIPSTracer, base.TraceContext, "Log collection succeeded.", null, "DoResponderWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\FIPS\\CollectFIPSLogsResponder.cs", 173);
					OldFileDeletionPolicy policy = new OldFileDeletionPolicy(TimeSpan.FromSeconds((double)this.LifeTimeSeconds));
					FileDeleter.Delete(this.LogDestination, policy);
					WTFDiagnostics.TraceDebug(ExTraceGlobals.FIPSTracer, base.TraceContext, "Logs have been successfully deleted as per the specified file deletion policy.", null, "DoResponderWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\FIPS\\CollectFIPSLogsResponder.cs", 182);
					base.Result.StateAttribute1 = DateTime.UtcNow.AddSeconds((double)base.Definition.WaitIntervalSeconds).ToString();
					return;
				}
				base.Result.StateAttribute1 = lastResponderResult.StateAttribute1;
			}, cancellationToken, TaskContinuationOptions.AttachedToParent);
		}

		private static void TestDirectoryPath(string directoryPath)
		{
			try
			{
				Directory.CreateDirectory(directoryPath);
			}
			catch (Exception ex)
			{
				string message = string.Format("Invalid Log Destination. Exception: {0}", ex.Message);
				WTFDiagnostics.TraceDebug(ExTraceGlobals.FIPSTracer, TracingContext.Default, message, null, "TestDirectoryPath", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\FIPS\\CollectFIPSLogsResponder.cs", 215);
				throw;
			}
		}

		private void InitializeAttributes()
		{
			this.LogDestination = base.Definition.Attributes["LogDestination"];
			this.LifeTimeSeconds = int.Parse(base.Definition.Attributes["LifeTimeSeconds"]);
		}

		private const string CmdletStartDiagnostics = "Start-Diagnostics";

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(CollectFIPSLogsResponder).FullName;

		internal static class AttributeNames
		{
			internal const string LifeTimeSeconds = "LifeTimeSeconds";

			internal const string LogDestination = "LogDestination";

			internal const string RecurrenceIntervalSeconds = "RecurrenceIntervalSeconds";

			internal const string WaitIntervalSeconds = "WaitIntervalSeconds";

			internal const string TimeoutSeconds = "TimeoutSeconds";

			internal const string MaxRetryAttempts = "MaxRetryAttempts";

			internal const string Enabled = "Enabled";
		}

		internal static class DefaultValues
		{
			internal static string LogDestination
			{
				get
				{
					if (CollectFIPSLogsResponder.DefaultValues.logDestination == null)
					{
						using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeLabs\\"))
						{
							if (registryKey != null)
							{
								string value = (string)registryKey.GetValue("FipsDiagnosticsLogPath");
								if (!string.IsNullOrEmpty(value))
								{
									CollectFIPSLogsResponder.DefaultValues.logDestination = value;
								}
							}
						}
					}
					return CollectFIPSLogsResponder.DefaultValues.logDestination;
				}
			}

			internal const int LifeTimeSeconds = 432000;

			internal const int RecurrenceIntervalSeconds = 900;

			internal const int WaitIntervalSeconds = 28800;

			internal const int TimeoutSeconds = 600;

			internal const int MaxRetryAttempts = 1;

			internal const bool Enabled = true;

			private const string ExchangeLabsRegKey = "SOFTWARE\\Microsoft\\ExchangeLabs\\";

			private static string logDestination;
		}
	}
}
