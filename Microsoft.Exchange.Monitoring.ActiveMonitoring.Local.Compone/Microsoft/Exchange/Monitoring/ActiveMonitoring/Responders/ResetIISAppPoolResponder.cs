using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Audit;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Responders
{
	public class ResetIISAppPoolResponder : ResponderWorkItem
	{
		internal string AppPoolName { get; set; }

		internal CommonDumpParameters DumpArgs { get; set; }

		internal static ResponderDefinition CreateDefinition(string responderName, string monitorName, string appPoolName, ServiceHealthStatus responderTargetState, DumpMode dumpOnRestartMode = DumpMode.None, string dumpPath = null, double minimumFreeDiskPercent = 15.0, int maximumDumpDurationInSeconds = 0, string serviceName = "Exchange", bool enabled = true, string throttleGroupName = null)
		{
			ResponderDefinition responderDefinition = new ResponderDefinition();
			responderDefinition.AssemblyPath = ResetIISAppPoolResponder.AssemblyPath;
			responderDefinition.TypeName = ResetIISAppPoolResponder.TypeName;
			responderDefinition.Name = responderName;
			responderDefinition.ServiceName = serviceName;
			responderDefinition.AlertTypeId = "*";
			responderDefinition.AlertMask = monitorName;
			responderDefinition.RecurrenceIntervalSeconds = 300;
			responderDefinition.TimeoutSeconds = 300;
			responderDefinition.MaxRetryAttempts = 3;
			responderDefinition.TargetHealthState = responderTargetState;
			responderDefinition.WaitIntervalSeconds = 30;
			responderDefinition.Enabled = enabled;
			if (string.IsNullOrEmpty(dumpPath))
			{
				dumpPath = ResetIISAppPoolResponder.DefaultValues.DumpPath;
			}
			responderDefinition.Attributes["AppPoolName"] = appPoolName;
			responderDefinition.Attributes["DumpOnRestart"] = dumpOnRestartMode.ToString();
			responderDefinition.Attributes["DumpPath"] = dumpPath;
			responderDefinition.Attributes["MinimumFreeDiskPercent"] = minimumFreeDiskPercent.ToString();
			responderDefinition.Attributes["MaximumDumpDurationInSeconds"] = maximumDumpDurationInSeconds.ToString();
			RecoveryActionRunner.SetThrottleProperties(responderDefinition, throttleGroupName, RecoveryActionId.RecycleApplicationPool, appPoolName, null);
			return responderDefinition;
		}

		internal virtual void InitializeAttributes()
		{
			AttributeHelper attributeHelper = new AttributeHelper(base.Definition);
			this.AppPoolName = attributeHelper.GetString("AppPoolName", true, null);
			this.InitializeDumpAttributes(attributeHelper);
		}

		internal void InternalRecyleAppPool(RecoveryActionEntry startEntry, CancellationToken cancellationToken)
		{
			if (this.EnsureIISServicesStarted(cancellationToken))
			{
				throw new InvalidOperationException("IIS service was stopped, service start has been requested. App pool recycle couldn't be executed.");
			}
			using (ApplicationPoolHelper appPoolHelper = new ApplicationPoolHelper(this.AppPoolName))
			{
				List<int> processIds = appPoolHelper.WorkerProcessIds;
				Privilege.RunWithPrivilege("SeDebugPrivilege", true, delegate
				{
					if (this.DumpArgs.IsDumpRequested)
					{
						this.DumpOneWorkerProcess(processIds, cancellationToken);
					}
					if (processIds == null || processIds.Count == 0)
					{
						appPoolHelper.Recycle();
						return;
					}
					ProcessHelper.Kill(processIds, ProcessKillMode.SelfOnly, startEntry.InstanceId);
				});
			}
		}

		protected bool EnsureIISServicesStarted(CancellationToken cancellationToken)
		{
			bool serviceStartAttempted = false;
			int num = Interlocked.CompareExchange(ref ResetIISAppPoolResponder.currentlyCheckingServiceStatus, 1, 0);
			if (num != 0)
			{
				return false;
			}
			try
			{
				Privilege.RunWithPrivilege("SeDebugPrivilege", true, delegate
				{
					foreach (string serviceName in ResetIISAppPoolResponder.IisServiceNames)
					{
						using (ServiceHelper serviceHelper = new ServiceHelper(serviceName, cancellationToken))
						{
							serviceStartAttempted = (serviceHelper.Start() || serviceStartAttempted);
						}
					}
				});
			}
			finally
			{
				ResetIISAppPoolResponder.currentlyCheckingServiceStatus = 0;
			}
			return serviceStartAttempted;
		}

		protected void InitializeDumpAttributes(AttributeHelper attributeHelper)
		{
			this.DumpArgs = new CommonDumpParameters
			{
				Mode = attributeHelper.GetEnum<DumpMode>("DumpOnRestart", false, DumpMode.None),
				Path = attributeHelper.GetString("DumpPath", false, ResetIISAppPoolResponder.DefaultValues.DumpPath),
				MinimumFreeSpace = attributeHelper.GetDouble("MinimumFreeDiskPercent", false, 15.0, new double?(0.0), new double?(100.0)),
				MaximumDurationInSeconds = attributeHelper.GetInt("MaximumDumpDurationInSeconds", false, 0, null, null)
			};
		}

		protected override void DoResponderWork(CancellationToken cancellationToken)
		{
			this.InitializeAttributes();
			RecoveryActionRunner recoveryActionRunner = new RecoveryActionRunner(RecoveryActionId.RecycleApplicationPool, this.AppPoolName, this, true, cancellationToken, null);
			recoveryActionRunner.Execute(delegate(RecoveryActionEntry startEntry)
			{
				this.InternalRecyleAppPool(startEntry, cancellationToken);
			});
		}

		private void DumpOneWorkerProcess(List<int> pids, CancellationToken cancellationToken)
		{
			if (pids.Count <= 0)
			{
				return;
			}
			RecoveryActionRunner recoveryActionRunner = new RecoveryActionRunner(RecoveryActionId.WatsonDump, this.AppPoolName, this, false, cancellationToken, null);
			recoveryActionRunner.Execute(delegate(RecoveryActionEntry startEntry)
			{
				foreach (int processId in pids)
				{
					using (Process processByIdBestEffort = ProcessHelper.GetProcessByIdBestEffort(processId))
					{
						ProcessDumpHelper processDumpHelper = new ProcessDumpHelper(this.DumpArgs, cancellationToken);
						try
						{
							startEntry.Context = processDumpHelper.Generate(processByIdBestEffort, this.Definition.Name);
							break;
						}
						catch (Exception)
						{
						}
					}
				}
			});
		}

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(ResetIISAppPoolResponder).FullName;

		private static readonly string[] IisServiceNames = new string[]
		{
			"WAS",
			"W3SVC",
			"IISADMIN"
		};

		private static int currentlyCheckingServiceStatus = 0;

		internal static class AttributeNames
		{
			internal const string AppPoolName = "AppPoolName";

			internal const string DumpOnRestart = "DumpOnRestart";

			internal const string DumpPath = "DumpPath";

			internal const string MinimumFreeDiskPercent = "MinimumFreeDiskPercent";

			internal const string MaximumDumpDurationInSeconds = "MaximumDumpDurationInSeconds";

			internal const string throttleGroupName = "throttleGroupName";
		}

		internal class DefaultValues
		{
			internal static string DumpPath
			{
				get
				{
					if (string.IsNullOrEmpty(ResetIISAppPoolResponder.DefaultValues.dumpPath))
					{
						try
						{
							ResetIISAppPoolResponder.DefaultValues.dumpPath = Path.Combine(ExchangeSetupContext.InstallPath, "Dumps");
						}
						catch (SetupVersionInformationCorruptException)
						{
							ResetIISAppPoolResponder.DefaultValues.dumpPath = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
						}
					}
					return ResetIISAppPoolResponder.DefaultValues.dumpPath;
				}
			}

			internal const DumpMode DumpOnRestart = DumpMode.None;

			internal const double MinimumFreeDiskPercent = 15.0;

			internal const int MaximumDumpDurationInSeconds = 0;

			internal const string ServiceName = "Exchange";

			internal const bool Enabled = true;

			private static string dumpPath = string.Empty;
		}
	}
}
