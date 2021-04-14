using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Audit;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery
{
	public class RestartServiceResponder : ResponderWorkItem
	{
		public string WindowsServiceName { get; set; }

		public TimeSpan StopTimeout { get; set; }

		public TimeSpan StartTimeout { get; set; }

		public TimeSpan StartDelay { get; set; }

		public bool IsMasterAndWorker { get; set; }

		public ProcessKillMode MasterAndWorkerKillMode { get; set; }

		public CommonDumpParameters DumpArgs { get; set; }

		public string AdditionalProcessNameToKill { get; set; }

		public bool RestartEnabled { get; set; }

		public static ResponderDefinition CreateDefinition(string responderName, string monitorName, string windowsServiceName, ServiceHealthStatus responderTargetState, int serviceStopTimeoutInSeconds = 15, int serviceStartTimeoutInSeconds = 120, int serviceStartDelayInSeconds = 0, bool isMasterWorker = false, DumpMode dumpOnRestartMode = DumpMode.None, string dumpPath = null, double minimumFreeDiskPercent = 15.0, int maximumDumpDurationInSeconds = 0, string serviceName = "Exchange", string additionalProcessNameToKill = null, bool restartEnabled = true, bool enabled = true, string throttleGroupName = null, bool dumpIgnoreRegistryLimit = false)
		{
			ResponderDefinition responderDefinition = new ResponderDefinition();
			responderDefinition.AssemblyPath = RestartServiceResponder.AssemblyPath;
			responderDefinition.TypeName = RestartServiceResponder.TypeName;
			responderDefinition.Name = responderName;
			responderDefinition.ServiceName = serviceName;
			responderDefinition.AlertTypeId = "*";
			responderDefinition.AlertMask = monitorName;
			responderDefinition.RecurrenceIntervalSeconds = ((dumpOnRestartMode == DumpMode.FullDump) ? 1800 : 300);
			responderDefinition.TimeoutSeconds = ((dumpOnRestartMode == DumpMode.FullDump) ? 1740 : 240);
			responderDefinition.MaxRetryAttempts = 3;
			responderDefinition.TargetHealthState = responderTargetState;
			responderDefinition.WaitIntervalSeconds = 30;
			responderDefinition.Enabled = enabled;
			if (string.IsNullOrEmpty(dumpPath))
			{
				dumpPath = RestartServiceResponder.DefaultValues.DumpPath;
			}
			if (string.IsNullOrEmpty(additionalProcessNameToKill))
			{
				additionalProcessNameToKill = RestartServiceResponder.DefaultValues.AdditionalProcessNameToKill;
			}
			responderDefinition.Attributes["WindowsServiceName"] = windowsServiceName;
			responderDefinition.Attributes["ServiceStopTimeout"] = TimeSpan.FromSeconds((double)serviceStopTimeoutInSeconds).ToString();
			responderDefinition.Attributes["ServiceStartTimeout"] = TimeSpan.FromSeconds((double)serviceStartTimeoutInSeconds).ToString();
			responderDefinition.Attributes["ServiceStartDelay"] = TimeSpan.FromSeconds((double)serviceStartDelayInSeconds).ToString();
			responderDefinition.Attributes["IsMasterAndWorker"] = isMasterWorker.ToString();
			responderDefinition.Attributes["DumpOnRestart"] = dumpOnRestartMode.ToString();
			responderDefinition.Attributes["DumpPath"] = dumpPath;
			responderDefinition.Attributes["DumpIgnoreRegistryLimit"] = dumpIgnoreRegistryLimit.ToString();
			responderDefinition.Attributes["MinimumFreeDiskPercent"] = minimumFreeDiskPercent.ToString();
			responderDefinition.Attributes["MaximumDumpDurationInSeconds"] = maximumDumpDurationInSeconds.ToString();
			responderDefinition.Attributes["AdditionalProcessNameToKill"] = additionalProcessNameToKill;
			responderDefinition.Attributes["RestartEnabled"] = restartEnabled.ToString();
			RecoveryActionRunner.SetThrottleProperties(responderDefinition, throttleGroupName, RecoveryActionId.RestartService, windowsServiceName, null);
			return responderDefinition;
		}

		protected void InitializeServiceAttributes(AttributeHelper attributeHelper)
		{
			this.WindowsServiceName = attributeHelper.GetString("WindowsServiceName", true, null);
			this.StopTimeout = attributeHelper.GetTimeSpan("ServiceStopTimeout", false, TimeSpan.FromSeconds(15.0), null, null);
			this.StartTimeout = attributeHelper.GetTimeSpan("ServiceStartTimeout", false, TimeSpan.FromSeconds(120.0), null, null);
			this.StartDelay = attributeHelper.GetTimeSpan("ServiceStartDelay", false, TimeSpan.FromSeconds(0.0), null, null);
			this.IsMasterAndWorker = attributeHelper.GetBool("IsMasterAndWorker", false, false);
			this.MasterAndWorkerKillMode = attributeHelper.GetEnum<ProcessKillMode>("MasterAndWorkerKillMode", false, this.IsMasterAndWorker ? ProcessKillMode.SelfAndChildren : ProcessKillMode.SelfOnly);
			this.AdditionalProcessNameToKill = attributeHelper.GetString("AdditionalProcessNameToKill", false, RestartServiceResponder.DefaultValues.AdditionalProcessNameToKill);
			this.RestartEnabled = attributeHelper.GetBool("RestartEnabled", false, true);
		}

		protected void InitializeDumpAttributes(AttributeHelper attributeHelper)
		{
			this.DumpArgs = new CommonDumpParameters
			{
				Mode = attributeHelper.GetEnum<DumpMode>("DumpOnRestart", false, DumpMode.None),
				Path = attributeHelper.GetString("DumpPath", false, RestartServiceResponder.DefaultValues.DumpPath),
				MinimumFreeSpace = attributeHelper.GetDouble("MinimumFreeDiskPercent", false, 15.0, new double?(0.0), new double?(100.0)),
				MaximumDurationInSeconds = attributeHelper.GetInt("MaximumDumpDurationInSeconds", false, 0, null, null),
				IgnoreRegistryOverride = attributeHelper.GetBool("DumpIgnoreRegistryLimit", false, false)
			};
		}

		protected virtual void InitializeAttributes()
		{
			AttributeHelper attributeHelper = new AttributeHelper(base.Definition);
			this.InitializeServiceAttributes(attributeHelper);
			this.InitializeDumpAttributes(attributeHelper);
		}

		protected string InternalDumpProcess(Process process, string requester, CancellationToken cancellationToken)
		{
			ProcessDumpHelper processDumpHelper = new ProcessDumpHelper(this.DumpArgs, cancellationToken);
			return processDumpHelper.Generate(process, base.Definition.Name);
		}

		protected void ThrottledDumpProcess(Process process, CancellationToken cancellationToken)
		{
			RecoveryActionRunner recoveryActionRunner = new RecoveryActionRunner(RecoveryActionId.WatsonDump, this.WindowsServiceName, this, false, cancellationToken, null);
			recoveryActionRunner.Execute(delegate(RecoveryActionEntry startEntry)
			{
				startEntry.CustomArg1 = this.InternalDumpProcess(process, this.Definition.Name, cancellationToken);
			});
		}

		protected void InternalRestartService(RecoveryActionEntry startEntry, CancellationToken cancellationToken)
		{
			Privilege.RunWithPrivilege("SeDebugPrivilege", true, delegate
			{
				bool flag = false;
				using (ServiceHelper serviceHelper = new ServiceHelper(this.WindowsServiceName, cancellationToken))
				{
					using (Process process = serviceHelper.GetProcess())
					{
						if (process != null)
						{
							int id = process.Id;
							if (this.DumpArgs.IsDumpRequested)
							{
								this.ThrottledDumpProcess(process, cancellationToken);
							}
							if (!this.RestartEnabled)
							{
								return;
							}
							Process[] array = null;
							if (!string.IsNullOrEmpty(this.AdditionalProcessNameToKill))
							{
								array = Process.GetProcessesByName(this.AdditionalProcessNameToKill);
							}
							flag = true;
							if (this.IsMasterAndWorker)
							{
								ProcessHelper.Kill(process, this.MasterAndWorkerKillMode, startEntry.InstanceId);
							}
							else
							{
								ProcessHelper.Kill(process, ProcessKillMode.SelfOnly, startEntry.InstanceId);
							}
							if (array != null)
							{
								foreach (Process process2 in array)
								{
									using (process2)
									{
										try
										{
											ProcessHelper.KillProcess(process2, true, startEntry.InstanceId);
										}
										catch (Win32Exception)
										{
										}
									}
								}
							}
							serviceHelper.WaitUntilProcessGoesAway(id, this.StopTimeout);
						}
					}
					if (flag)
					{
						serviceHelper.Sleep(this.StartDelay);
					}
					serviceHelper.Start();
					serviceHelper.WaitForStatus(ServiceControllerStatus.Running, this.StartTimeout);
				}
			});
		}

		protected override void DoResponderWork(CancellationToken cancellationToken)
		{
			this.InitializeAttributes();
			RecoveryActionRunner recoveryActionRunner = new RecoveryActionRunner(RecoveryActionId.RestartService, this.WindowsServiceName, this, true, cancellationToken, null);
			recoveryActionRunner.Execute(delegate(RecoveryActionEntry startEntry)
			{
				this.InternalRestartService(startEntry, cancellationToken);
			});
		}

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(RestartServiceResponder).FullName;

		public static class AttributeNames
		{
			public const string WindowsServiceName = "WindowsServiceName";

			public const string ServiceStopTimeout = "ServiceStopTimeout";

			public const string ServiceStartTimeout = "ServiceStartTimeout";

			public const string ServiceStartDelay = "ServiceStartDelay";

			public const string IsMasterAndWorker = "IsMasterAndWorker";

			public const string MasterAndWorkerKillMode = "MasterAndWorkerKillMode";

			public const string DumpOnRestart = "DumpOnRestart";

			public const string DumpPath = "DumpPath";

			public const string DumpIgnoreRegistryLimit = "DumpIgnoreRegistryLimit";

			public const string MinimumFreeDiskPercent = "MinimumFreeDiskPercent";

			public const string MaximumDumpDurationInSeconds = "MaximumDumpDurationInSeconds";

			public const string AdditionalProcessNameToKill = "AdditionalProcessNameToKill";

			public const string RestartEnabled = "RestartEnabled";

			public const string ThrottleGroupName = "throttleGroupName";
		}

		public class DefaultValues
		{
			public static string DumpPath
			{
				get
				{
					if (string.IsNullOrEmpty(RestartServiceResponder.DefaultValues.dumpPath))
					{
						try
						{
							RestartServiceResponder.DefaultValues.dumpPath = Path.Combine(ExchangeSetupContext.InstallPath, "Dumps");
						}
						catch (SetupVersionInformationCorruptException)
						{
							RestartServiceResponder.DefaultValues.dumpPath = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
						}
					}
					return RestartServiceResponder.DefaultValues.dumpPath;
				}
			}

			internal static string AdditionalProcessNameToKill
			{
				get
				{
					return string.Empty;
				}
			}

			public const int ServiceStopTimeoutInSeconds = 15;

			public const int ServiceStartTimeoutInSeconds = 120;

			public const int ServiceStartDelayInSeconds = 0;

			public const DumpMode DumpOnRestart = DumpMode.None;

			public const bool DumpIgnoreRegistryLimit = false;

			public const double MinimumFreeDiskPercent = 15.0;

			public const int MaximumDumpDurationInSeconds = 0;

			public const bool IsMasterWorker = false;

			public const string ServiceName = "Exchange";

			internal const bool RestartEnabled = true;

			internal const bool Enabled = true;

			private static string dumpPath = string.Empty;
		}
	}
}
