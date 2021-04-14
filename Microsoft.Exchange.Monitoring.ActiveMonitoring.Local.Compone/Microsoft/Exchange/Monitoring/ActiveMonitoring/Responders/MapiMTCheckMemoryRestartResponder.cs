using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Audit;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Web.Administration;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Responders
{
	public class MapiMTCheckMemoryRestartResponder : RestartServiceResponder
	{
		internal string AppPoolName { get; set; }

		internal static ResponderDefinition CreateDefinition(string name, string alertMask, ServiceHealthStatus targetHealthState, int serviceStopTimeoutInSeconds = 15, int serviceStartTimeoutInSeconds = 120, int serviceStartDelayInSeconds = 0, bool isMasterWorker = false, DumpMode dumpOnRestartMode = DumpMode.None, string dumpPath = null, double minimumFreeDiskPercent = 15.0, int maximumDumpDurationInSeconds = 0, string serviceName = "Exchange", string additionalProcessNameToKill = null, bool restartEnabled = true, bool enabled = true, string throttleGroupName = "Dag")
		{
			ResponderDefinition responderDefinition = RestartServiceResponder.CreateDefinition(name, alertMask, "MSExchangeRPC", targetHealthState, serviceStopTimeoutInSeconds, serviceStartTimeoutInSeconds, serviceStartDelayInSeconds, isMasterWorker, dumpOnRestartMode, dumpPath, minimumFreeDiskPercent, maximumDumpDurationInSeconds, serviceName, additionalProcessNameToKill, restartEnabled, enabled, throttleGroupName, false);
			responderDefinition.AssemblyPath = MapiMTCheckMemoryRestartResponder.assemblyPath;
			responderDefinition.TypeName = MapiMTCheckMemoryRestartResponder.responderTypeName;
			responderDefinition.Attributes["AppPoolName"] = "MSExchangeRpcProxyAppPool";
			RestartResponderChecker[] array = new RestartResponderChecker[]
			{
				new MemoryRestartResponderChecker(null),
				new PerformanceCounterRestartResponderCheckers(null)
			};
			foreach (RestartResponderChecker restartResponderChecker in array)
			{
				responderDefinition.Attributes[restartResponderChecker.KeyOfEnabled] = restartResponderChecker.EnabledByDefault.ToString();
				responderDefinition.Attributes[restartResponderChecker.KeyOfSetting] = restartResponderChecker.DefaultSetting;
			}
			RecoveryActionRunner.SetThrottleProperties(responderDefinition, throttleGroupName, RecoveryActionId.RpcClientAccessRestart, "MSExchangeRPC", null);
			return responderDefinition;
		}

		protected override void InitializeAttributes()
		{
			AttributeHelper attributeHelper = new AttributeHelper(base.Definition);
			this.AppPoolName = attributeHelper.GetString("AppPoolName", true, null);
			base.InitializeServiceAttributes(attributeHelper);
			base.InitializeDumpAttributes(attributeHelper);
		}

		protected override void DoResponderWork(CancellationToken cancellationToken)
		{
			this.InitializeAttributes();
			RecoveryActionRunner recoveryActionRunner = new RecoveryActionRunner(RecoveryActionId.RpcClientAccessRestart, base.WindowsServiceName, this, true, cancellationToken, null);
			recoveryActionRunner.Execute(delegate()
			{
				this.InternalRestartAppPoolAndService(cancellationToken);
			});
		}

		private bool IsRunningOnLowResource()
		{
			if (this.restartResponderCheckers == null)
			{
				this.restartResponderCheckers = new RestartResponderChecker[]
				{
					new MemoryRestartResponderChecker(base.Definition),
					new PerformanceCounterRestartResponderCheckers(base.Definition)
				};
			}
			foreach (RestartResponderChecker restartResponderChecker in this.restartResponderCheckers)
			{
				if (!restartResponderChecker.IsRestartAllowed)
				{
					ResponderResult result = base.Result;
					result.StateAttribute1 += restartResponderChecker.SkipReasonOrException;
					return true;
				}
				if (restartResponderChecker.SkipReasonOrException != null)
				{
					ResponderResult result2 = base.Result;
					result2.StateAttribute1 += restartResponderChecker.SkipReasonOrException;
				}
			}
			return false;
		}

		internal void InternalRestartAppPoolAndService(CancellationToken cancellationToken)
		{
			MapiMTCheckMemoryRestartResponder.<>c__DisplayClass4 CS$<>8__locals1 = new MapiMTCheckMemoryRestartResponder.<>c__DisplayClass4();
			CS$<>8__locals1.cancellationToken = cancellationToken;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.entryId = Guid.NewGuid().ToString();
			if (this.EnsureIISServicesStarted(CS$<>8__locals1.cancellationToken))
			{
				throw new InvalidOperationException("IIS service was stopped, service start has been requested. App pool recycle couldn't be executed.");
			}
			using (ApplicationPoolHelper appPoolHelper = new ApplicationPoolHelper(this.AppPoolName))
			{
				Privilege.RunWithPrivilege("SeDebugPrivilege", true, delegate
				{
					if (CS$<>8__locals1.<>4__this.IsRunningOnLowResource())
					{
						return;
					}
					bool flag = false;
					using (ServiceHelper serviceHelper = new ServiceHelper(CS$<>8__locals1.<>4__this.WindowsServiceName, CS$<>8__locals1.cancellationToken))
					{
						List<int> workerProcessIds = appPoolHelper.WorkerProcessIds;
						if (CS$<>8__locals1.<>4__this.DumpArgs.IsDumpRequested)
						{
							CS$<>8__locals1.<>4__this.DumpOneWorkerProcess(workerProcessIds, CS$<>8__locals1.cancellationToken);
						}
						if (workerProcessIds == null || workerProcessIds.Count == 0)
						{
							appPoolHelper.Initialize();
							if (appPoolHelper.ApplicationPool.State == 1)
							{
								appPoolHelper.ApplicationPool.AutoStart = false;
								if (appPoolHelper.ApplicationPool.Stop() != 3)
								{
									throw new InvalidOperationException(string.Format("Failed to stop application pool (poolName={0}, state={1})", CS$<>8__locals1.<>4__this.AppPoolName, appPoolHelper.ApplicationPool.State));
								}
							}
						}
						else
						{
							ProcessHelper.Kill(workerProcessIds, ProcessKillMode.SelfOnly, CS$<>8__locals1.entryId);
						}
						using (Process process = serviceHelper.GetProcess())
						{
							if (process != null)
							{
								int id = process.Id;
								if (CS$<>8__locals1.<>4__this.DumpArgs.IsDumpRequested)
								{
									CS$<>8__locals1.<>4__this.ThrottledDumpProcess(process, CS$<>8__locals1.cancellationToken);
								}
								if (!CS$<>8__locals1.<>4__this.RestartEnabled)
								{
									return;
								}
								Process[] array = null;
								if (!string.IsNullOrEmpty(CS$<>8__locals1.<>4__this.AdditionalProcessNameToKill))
								{
									array = Process.GetProcessesByName(CS$<>8__locals1.<>4__this.AdditionalProcessNameToKill);
								}
								flag = true;
								if (CS$<>8__locals1.<>4__this.IsMasterAndWorker)
								{
									ProcessHelper.Kill(process, CS$<>8__locals1.<>4__this.MasterAndWorkerKillMode, CS$<>8__locals1.entryId);
								}
								else
								{
									ProcessHelper.Kill(process, ProcessKillMode.SelfOnly, CS$<>8__locals1.entryId);
								}
								if (array != null)
								{
									foreach (Process process2 in array)
									{
										using (process2)
										{
											try
											{
												ProcessHelper.KillProcess(process2, true, CS$<>8__locals1.entryId);
											}
											catch (Win32Exception)
											{
											}
										}
									}
								}
								serviceHelper.WaitUntilProcessGoesAway(id, CS$<>8__locals1.<>4__this.StopTimeout);
							}
						}
						if (flag)
						{
							serviceHelper.Sleep(CS$<>8__locals1.<>4__this.StartDelay);
						}
						serviceHelper.Start();
						serviceHelper.WaitForStatus(ServiceControllerStatus.Running, CS$<>8__locals1.<>4__this.StartTimeout);
					}
					if (appPoolHelper.ApplicationPool.State == 3)
					{
						appPoolHelper.ApplicationPool.AutoStart = true;
						ObjectState objectState = appPoolHelper.ApplicationPool.Start();
						if (objectState != null && objectState != 1)
						{
							throw new InvalidOperationException(string.Format("Failed to start application pool (poolName={0}, state={1})", CS$<>8__locals1.<>4__this.AppPoolName, objectState));
						}
					}
				});
			}
		}

		protected bool EnsureIISServicesStarted(CancellationToken cancellationToken)
		{
			bool serviceStartAttempted = false;
			int num = Interlocked.CompareExchange(ref MapiMTCheckMemoryRestartResponder.currentlyCheckingServiceStatus, 1, 0);
			if (num != 0)
			{
				return false;
			}
			try
			{
				Privilege.RunWithPrivilege("SeDebugPrivilege", true, delegate
				{
					foreach (string serviceName in MapiMTCheckMemoryRestartResponder.IisServiceNames)
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
				MapiMTCheckMemoryRestartResponder.currentlyCheckingServiceStatus = 0;
			}
			return serviceStartAttempted;
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

		private const string RpcClientAccessServiceName = "MSExchangeRPC";

		private static readonly string assemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string responderTypeName = typeof(MapiMTCheckMemoryRestartResponder).FullName;

		private static readonly string[] IisServiceNames = new string[]
		{
			"WAS",
			"W3SVC",
			"IISADMIN"
		};

		private static int currentlyCheckingServiceStatus = 0;

		private RestartResponderChecker[] restartResponderCheckers;

		public new static class AttributeNames
		{
			internal const string AdditionalProcessNameToKill = "AdditionalProcessNameToKill";

			internal const string AppPoolName = "AppPoolName";

			internal const string DumpOnRestart = "DumpOnRestart";

			internal const string DumpPath = "DumpPath";

			internal const string IsMasterAndWorker = "IsMasterAndWorker";

			internal const string MasterAndWorkerKillMode = "MasterAndWorkerKillMode";

			internal const string MaximumDumpDurationInSeconds = "MaximumDumpDurationInSeconds";

			internal const string MinimumFreeDiskPercent = "MinimumFreeDiskPercent";

			internal const string RestartEnabled = "RestartEnabled";

			internal const string ServiceStartDelay = "ServiceStartDelay";

			internal const string ServiceStartTimeout = "ServiceStartTimeout";

			internal const string ServiceStopTimeout = "ServiceStopTimeout";

			internal const string throttleGroupName = "throttleGroupName";

			internal const string WindowsServiceName = "WindowsServiceName";
		}

		internal new class DefaultValues
		{
			internal static string DumpPath
			{
				get
				{
					if (string.IsNullOrEmpty(MapiMTCheckMemoryRestartResponder.DefaultValues.dumpPath))
					{
						try
						{
							MapiMTCheckMemoryRestartResponder.DefaultValues.dumpPath = Path.Combine(ExchangeSetupContext.InstallPath, "Dumps");
						}
						catch (SetupVersionInformationCorruptException)
						{
							MapiMTCheckMemoryRestartResponder.DefaultValues.dumpPath = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
						}
					}
					return MapiMTCheckMemoryRestartResponder.DefaultValues.dumpPath;
				}
			}

			internal const int RecurrenceIntervalSeconds = 300;

			internal const int WaitIntervalSeconds = 30;

			internal const int TimeoutSeconds = 150;

			internal const int MaxRetryAttempts = 3;

			internal const DumpMode DumpOnRestart = DumpMode.None;

			internal const double MinimumFreeDiskPercent = 15.0;

			internal const int MaximumDumpDurationInSeconds = 0;

			internal const bool RestartEnabled = true;

			internal const string ServiceName = "Exchange";

			internal const bool Enabled = true;

			internal const bool IsMasterWorker = false;

			internal const int ServiceStopTimeoutInSeconds = 15;

			internal const int ServiceStartTimeoutInSeconds = 120;

			internal const int ServiceStartDelayInSeconds = 0;

			private static string dumpPath = string.Empty;
		}
	}
}
