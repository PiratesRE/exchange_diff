using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Search.Responders
{
	public class SearchEscalateResponder : EscalateResponder
	{
		public static string EscalateDailySchedulePattern
		{
			get
			{
				return SearchEscalateResponder.escalateDailySchedulePattern;
			}
			set
			{
				SearchEscalateResponder.escalateDailySchedulePattern = value;
			}
		}

		internal SearchEscalateResponder.EscalateModes EscalateMode { get; set; }

		internal bool UrgentInTraining { get; set; }

		internal bool CollectLogsAfterEscalate { get; set; }

		internal string CollectDumpForProcess { get; set; }

		internal string SearchMonitoringLogPath { get; set; }

		protected override bool IncludeHealthSetEscalationInfo
		{
			get
			{
				return !LocalEndpointManager.IsDataCenter && base.IncludeHealthSetEscalationInfo;
			}
		}

		public static ResponderDefinition CreateDefinition(string name, string serviceName, string alertTypeId, string alertMask, string targetResource, ServiceHealthStatus targetHealthState, string escalationTeam, string escalationSubjectUnhealthy, string escalationMessageUnhealthy, SearchEscalateResponder.EscalateModes escalateMode = SearchEscalateResponder.EscalateModes.Scheduled, bool urgentInTraining = true)
		{
			ResponderDefinition responderDefinition = EscalateResponder.CreateDefinition(name, serviceName, alertTypeId, alertMask, targetResource, targetHealthState, escalationTeam, escalationSubjectUnhealthy, escalationMessageUnhealthy, true, NotificationServiceClass.Urgent, 14400, SearchEscalateResponder.EscalateDailySchedulePattern, false);
			responderDefinition.RecurrenceIntervalSeconds = 0;
			responderDefinition.TimeoutSeconds = 600;
			responderDefinition.AssemblyPath = SearchEscalateResponder.AssemblyPath;
			responderDefinition.TypeName = SearchEscalateResponder.TypeName;
			responderDefinition.Attributes["EscalateMode"] = escalateMode.ToString();
			responderDefinition.Attributes["UrgentInTraining"] = urgentInTraining.ToString();
			if (escalateMode == SearchEscalateResponder.EscalateModes.Urgent)
			{
				if (urgentInTraining)
				{
					responderDefinition.NotificationServiceClass = NotificationServiceClass.UrgentInTraining;
				}
				else
				{
					responderDefinition.NotificationServiceClass = NotificationServiceClass.Urgent;
				}
			}
			else
			{
				responderDefinition.NotificationServiceClass = NotificationServiceClass.Scheduled;
			}
			return responderDefinition;
		}

		internal override EscalationState GetEscalationState(bool? isHealthy, CancellationToken cancellationToken)
		{
			this.InitializeAttributes();
			if (isHealthy != null && !isHealthy.Value)
			{
				this.SetNotificationServiceClass(cancellationToken);
			}
			return base.GetEscalationState(isHealthy, cancellationToken);
		}

		protected virtual void InitializeAttributes()
		{
			AttributeHelper attributeHelper = new AttributeHelper(base.Definition);
			this.EscalateMode = attributeHelper.GetEnum<SearchEscalateResponder.EscalateModes>("EscalateMode", false, SearchEscalateResponder.EscalateModes.Scheduled);
			this.UrgentInTraining = attributeHelper.GetBool("UrgentInTraining", false, true);
			this.CollectLogsAfterEscalate = attributeHelper.GetBool("CollectLogsAfterEscalate", false, true);
			this.CollectDumpForProcess = attributeHelper.GetString("CollectDumpForProcess", false, SearchEscalateResponder.DefaultValues.CollectDumpForProcess);
			string installPath = ExchangeSetupContext.InstallPath;
			this.SearchMonitoringLogPath = Path.Combine(installPath, "Logging\\Monitoring\\Search");
		}

		protected override void AfterEscalate(CancellationToken cancellationToken)
		{
			try
			{
				this.CollectLogs();
			}
			catch (Exception ex)
			{
				SearchMonitoringHelper.LogInfo(this, "Exception caught collecting logs: {0}", new object[]
				{
					ex.ToString()
				});
			}
			try
			{
				this.CollectDump(cancellationToken);
			}
			catch (Exception ex2)
			{
				SearchMonitoringHelper.LogInfo(this, "Exception caught collecting dump: {0}", new object[]
				{
					ex2.ToString()
				});
			}
		}

		private bool IsDatabaseCopyActiveOnLocalServer()
		{
			return SearchMonitoringHelper.IsDatabaseActive(base.Definition.TargetResource);
		}

		private void SetNotificationServiceClass(CancellationToken cancellationToken)
		{
			switch (this.EscalateMode)
			{
			case SearchEscalateResponder.EscalateModes.Urgent:
				base.Definition.NotificationServiceClass = NotificationServiceClass.Urgent;
				break;
			case SearchEscalateResponder.EscalateModes.Scheduled:
				base.Definition.NotificationServiceClass = NotificationServiceClass.Scheduled;
				break;
			case SearchEscalateResponder.EscalateModes.UrgentOnActive:
				if (!this.IsDatabaseCopyActiveOnLocalServer())
				{
					SearchMonitoringHelper.LogInfo(this, "Database is inactive. Setting NotificationServiceClass to 'Scheduled'.", new object[0]);
					base.Definition.NotificationServiceClass = NotificationServiceClass.Scheduled;
				}
				else
				{
					SearchMonitoringHelper.LogInfo(this, "Database is active. Setting NotificationServiceClass to 'Urgent'.", new object[0]);
					base.Definition.NotificationServiceClass = NotificationServiceClass.Urgent;
				}
				break;
			case SearchEscalateResponder.EscalateModes.ReadFromProbeResult:
			{
				ProbeResult probeResult = null;
				try
				{
					probeResult = WorkItemResultHelper.GetLastFailedProbeResult(this, base.Broker, cancellationToken);
				}
				catch (Exception ex)
				{
					SearchMonitoringHelper.LogInfo(this, "Caught exception reading last failed probe result: '{0}'.", new object[]
					{
						ex
					});
				}
				if (probeResult != null)
				{
					NotificationServiceClass notificationServiceClass;
					if (Enum.TryParse<NotificationServiceClass>(probeResult.StateAttribute22, out notificationServiceClass))
					{
						SearchMonitoringHelper.LogInfo(this, "Got NotificationServiceClass from last failed probe result: '{0}'.", new object[]
						{
							notificationServiceClass
						});
						base.Definition.NotificationServiceClass = notificationServiceClass;
					}
					else
					{
						SearchMonitoringHelper.LogInfo(this, "Failed to parse the NotificationServiceClass from StateAttribute22.", new object[0]);
					}
				}
				else
				{
					SearchMonitoringHelper.LogInfo(this, "Failed to read last failed probe result.", new object[0]);
				}
				break;
			}
			}
			if (base.Definition.NotificationServiceClass == NotificationServiceClass.Urgent && this.UrgentInTraining)
			{
				base.Definition.NotificationServiceClass = NotificationServiceClass.UrgentInTraining;
			}
		}

		private void CollectLogs()
		{
			if (!LocalEndpointManager.IsDataCenter || !this.CollectLogsAfterEscalate)
			{
				return;
			}
			string installPath = ExchangeSetupContext.InstallPath;
			string[] array = new string[]
			{
				"Bin\\Search\\Ceres\\Diagnostics\\Logs",
				"Logging\\Search",
				"Logging\\Search\\Crawler",
				"Logging\\Monitoring\\Search",
				"Logging\\Search\\GracefulDegradation"
			};
			DateTime t = DateTime.UtcNow.AddHours(-3.0);
			if (!Directory.Exists(this.SearchMonitoringLogPath))
			{
				Directory.CreateDirectory(this.SearchMonitoringLogPath);
			}
			string[] files = Directory.GetFiles(this.SearchMonitoringLogPath, "*.zip", SearchOption.TopDirectoryOnly);
			if (files.Length >= 10)
			{
				FileInfo fileInfo = new FileInfo(files[0]);
				for (int i = 1; i < files.Length; i++)
				{
					FileInfo fileInfo2 = new FileInfo(files[i]);
					if (fileInfo2.LastWriteTimeUtc < fileInfo.LastWriteTimeUtc)
					{
						fileInfo = fileInfo2;
					}
				}
				fileInfo.Delete();
			}
			string text = Environment.MachineName + "-" + base.Definition.AlertTypeId;
			if (!string.IsNullOrEmpty(base.Definition.TargetResource))
			{
				text = text + "-" + base.Definition.TargetResource;
			}
			string text2 = Path.Combine(this.SearchMonitoringLogPath, text);
			string text3 = Path.Combine(this.SearchMonitoringLogPath, text + ".zip");
			if (Directory.Exists(text2))
			{
				Directory.Delete(text2, true);
			}
			if (File.Exists(text3))
			{
				Directory.Delete(text3);
			}
			Directory.CreateDirectory(text2);
			foreach (string path in array)
			{
				string path2 = Path.Combine(installPath, path);
				if (Directory.Exists(path2))
				{
					string[] files2 = Directory.GetFiles(path2, "*.log", SearchOption.TopDirectoryOnly);
					foreach (string fileName in files2)
					{
						FileInfo fileInfo3 = new FileInfo(fileName);
						if (fileInfo3.LastWriteTimeUtc >= t)
						{
							fileInfo3.CopyTo(Path.Combine(text2, fileInfo3.Name));
						}
					}
				}
			}
			ZipFile.CreateFromDirectory(text2, text3, CompressionLevel.Optimal, false);
			Directory.Delete(text2, true);
		}

		private void CollectDump(CancellationToken cancellationToken)
		{
			if (!LocalEndpointManager.IsDataCenter || string.IsNullOrEmpty(this.CollectDumpForProcess))
			{
				return;
			}
			string collectDumpForProcess = this.CollectDumpForProcess;
			Process[] array;
			if (collectDumpForProcess.StartsWith("noderunner", StringComparison.OrdinalIgnoreCase))
			{
				array = SearchMonitoringHelper.GetProcessesForNodeRunner(collectDumpForProcess);
			}
			else
			{
				array = (Process.GetProcessesByName(collectDumpForProcess) ?? new Process[0]);
			}
			foreach (Process process in array)
			{
				this.ThrottledDumpProcess(process, cancellationToken);
			}
		}

		private void ThrottledDumpProcess(Process process, CancellationToken cancellationToken)
		{
			RecoveryActionRunner recoveryActionRunner = new RecoveryActionRunner(RecoveryActionId.WatsonDump, this.CollectDumpForProcess, this, false, cancellationToken, null);
			recoveryActionRunner.Execute(delegate(RecoveryActionEntry startEntry)
			{
				this.InternalDumpProcess(process, cancellationToken);
			});
		}

		private void InternalDumpProcess(Process process, CancellationToken cancellationToken)
		{
			ProcessDumpHelper processDumpHelper = new ProcessDumpHelper(new CommonDumpParameters
			{
				Mode = DumpMode.FullDump,
				MaximumDurationInSeconds = 1200,
				Path = this.SearchMonitoringLogPath
			}, cancellationToken);
			string text = processDumpHelper.Generate(process, base.Definition.Name);
			SearchMonitoringHelper.LogInfo(this, "Dump is collected for process {0} at '{1}'.", new object[]
			{
				process.Id,
				text
			});
		}

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(SearchEscalateResponder).FullName;

		private static string escalateDailySchedulePattern = "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday/00:00/17:00";

		public enum EscalateModes
		{
			Urgent,
			Scheduled,
			UrgentOnActive,
			ReadFromProbeResult
		}

		internal new static class AttributeNames
		{
			internal const string EscalateMode = "EscalateMode";

			internal const string UrgentInTraining = "UrgentInTraining";

			internal const string CollectLogsAfterEscalate = "CollectLogsAfterEscalate";

			internal const string CollectDumpForProcess = "CollectDumpForProcess";
		}

		internal new static class DefaultValues
		{
			internal const SearchEscalateResponder.EscalateModes EscalateMode = SearchEscalateResponder.EscalateModes.Scheduled;

			internal const bool UrgentInTraining = true;

			internal const bool CollectLogsAfterEscalate = true;

			internal static readonly string CollectDumpForProcess = string.Empty;
		}
	}
}
