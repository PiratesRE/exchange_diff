using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.LogAnalyzer.Analyzers.EventLog;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.ProcessIsolation.Monitors
{
	public class ProcessIsolationMonitor : OverallXFailuresMonitor
	{
		internal static List<SubComponentConfiguration> SubComponentConfigurationList
		{
			get
			{
				return ProcessIsolationMonitor.subComponentConfigurationList;
			}
		}

		internal static Dictionary<string, ProcessConfiguration> ProcessConfigurationDictionary
		{
			get
			{
				return ProcessIsolationMonitor.processConfigurationDictionary;
			}
		}

		internal static List<string> ProcessExclusionList
		{
			get
			{
				return ProcessIsolationMonitor.processExclusionList;
			}
		}

		public static MonitorDefinition CreateMonitor(string processName, ProcessTrigger triggerType)
		{
			string sampleMask = string.Empty;
			if (processName == "Default")
			{
				sampleMask = NotificationItem.GenerateResultName(ExchangeComponent.Eds.Name, triggerType.ToString(), null);
			}
			else
			{
				sampleMask = NotificationItem.GenerateResultName(ExchangeComponent.Eds.Name, triggerType.ToString(), processName);
			}
			ProcessConfiguration processConfiguration = ProcessIsolationDiscovery.GetProcessConfiguration(processName);
			Component component = ExchangeComponent.Eds;
			if (processConfiguration != null)
			{
				LocalEndpointManager instance = LocalEndpointManager.Instance;
				if (instance.ExchangeServerRoleEndpoint.IsCafeRoleInstalled && !instance.ExchangeServerRoleEndpoint.IsMailboxRoleInstalled && processConfiguration.Component != ExchangeComponent.Eds && processConfiguration.Component != ExchangeComponent.ProcessIsolation)
				{
					component = ExchangeComponent.Cafe;
				}
				else
				{
					component = processConfiguration.Component;
				}
			}
			string name = ProcessIsolationMonitor.BuildMonitorName(triggerType, processName);
			MonitorDefinition monitorDefinition = new MonitorDefinition();
			monitorDefinition.AssemblyPath = Assembly.GetExecutingAssembly().Location;
			monitorDefinition.TypeName = typeof(ProcessIsolationMonitor).FullName;
			monitorDefinition.Name = name;
			monitorDefinition.SampleMask = sampleMask;
			monitorDefinition.ServiceName = component.Name;
			monitorDefinition.Component = component;
			monitorDefinition.MaxRetryAttempts = 0;
			monitorDefinition.Enabled = true;
			monitorDefinition.TimeoutSeconds = ProcessIsolationMonitor.recurrenceIntervalSeconds / 2;
			monitorDefinition.MonitoringThreshold = (double)ProcessIsolationMonitor.numberOfFailuresToAlert;
			monitorDefinition.MonitoringIntervalSeconds = ProcessIsolationMonitor.alertingDuration;
			monitorDefinition.RecurrenceIntervalSeconds = ProcessIsolationMonitor.recurrenceIntervalSeconds;
			monitorDefinition.TargetResource = processName;
			monitorDefinition.Attributes["TriggerType"] = triggerType.ToString();
			return monitorDefinition;
		}

		internal static string BuildMonitorName(ProcessTrigger triggerType, string processName)
		{
			string text = ProcessIsolationMonitor.GenerateMonitorNameUsingTriggerType(triggerType);
			int maxLength = ProcessIsolationMonitor.maxCharsMonitorName - text.Length - 2;
			string uniqueShortProcessName = ProcessIsolationMonitor.GetUniqueShortProcessName(processName, maxLength);
			return text + '.' + uniqueShortProcessName;
		}

		internal static string BuildSubComponentName(ProcessTrigger triggerType, string processName, string subComponent)
		{
			string text = ProcessIsolationMonitor.GenerateMonitorNameUsingTriggerType(triggerType);
			if (text.Length + processName.Length + subComponent.Length < 63)
			{
				return string.Format("{0}.{1}_{2}", text, processName, subComponent);
			}
			processName = ProcessIsolationMonitor.GetUniqueShortProcessName(processName, 20);
			int num = 62 - text.Length - processName.Length;
			return string.Format("{0}.{1}_{2}", text, processName, (subComponent.Length > num) ? subComponent.Substring(0, num - 1) : subComponent);
		}

		internal static string GenerateMonitorNameUsingTriggerType(ProcessTrigger triggerType)
		{
			string result = string.Empty;
			switch (triggerType)
			{
			case ProcessTrigger.PrivateWorkingSetTrigger_Warning:
				result = "PrivateWorkingSetWarning";
				break;
			case ProcessTrigger.PrivateWorkingSetTrigger_Error:
				result = "PrivateWorkingSetError";
				break;
			case ProcessTrigger.ProcessProcessorTimeTrigger_Warning:
				result = "ProcessProcessorTimeWarning";
				break;
			case ProcessTrigger.ProcessProcessorTimeTrigger_Error:
				result = "ProcessProcessorTimeError";
				break;
			case ProcessTrigger.ExchangeCrashEventTrigger_Error:
				result = "CrashEvent";
				break;
			case ProcessTrigger.LongRunningWatsonTrigger_Warning:
				result = "LongRunningWatsonWarning";
				break;
			case ProcessTrigger.LongRunningWerMgrTrigger_Warning:
				result = "LongRunningWerMgrWarning";
				break;
			}
			return result;
		}

		internal static Dictionary<MonitorStateTransition, ResponderDefinitionDelegate> GetMonitorStateTransitions(string process, ProcessTrigger triggerType)
		{
			ProcessConfiguration processConfiguration = ProcessIsolationDiscovery.GetProcessConfiguration(process);
			Dictionary<MonitorStateTransition, ResponderDefinitionDelegate> result = null;
			switch (triggerType)
			{
			case ProcessTrigger.PrivateWorkingSetTrigger_Warning:
				result = processConfiguration.Responders.PrivateWorkingSetTriggerWarningResponders;
				break;
			case ProcessTrigger.PrivateWorkingSetTrigger_Error:
				result = processConfiguration.Responders.PrivateWorkingSetTriggerErrorResponders;
				break;
			case ProcessTrigger.ProcessProcessorTimeTrigger_Warning:
				result = processConfiguration.Responders.ProcessProcessorTimeTriggerWarningResponders;
				break;
			case ProcessTrigger.ProcessProcessorTimeTrigger_Error:
				result = processConfiguration.Responders.ProcessProcessorTimeTriggerErrorResponders;
				break;
			case ProcessTrigger.ExchangeCrashEventTrigger_Error:
				result = processConfiguration.Responders.ExchangeCrashEventTriggerErrorResponders;
				break;
			case ProcessTrigger.LongRunningWatsonTrigger_Warning:
			case ProcessTrigger.LongRunningWerMgrTrigger_Warning:
				result = processConfiguration.Responders.WatsonWarningResponders;
				break;
			}
			return result;
		}

		internal static bool IsMailboxRoleInstalled()
		{
			LocalEndpointManager instance = LocalEndpointManager.Instance;
			return instance.ExchangeServerRoleEndpoint != null && instance.ExchangeServerRoleEndpoint.IsMailboxRoleInstalled;
		}

		protected override void DoMonitorWork(CancellationToken cancellationToken)
		{
			this.SetPercentSuccessNumbers(cancellationToken).ContinueWith(delegate(Task t)
			{
				if ((double)this.Result.TotalFailedCount >= this.Definition.MonitoringThreshold)
				{
					if (!string.Equals(this.Definition.TargetResource, "Default", StringComparison.OrdinalIgnoreCase))
					{
						this.Result.IsAlert = !ProcessIsolationMonitor.ProcessExclusionList.Contains(this.Definition.TargetResource, StringComparer.OrdinalIgnoreCase);
					}
					else
					{
						IOrderedEnumerable<ProbeResult> orderedEnumerable = from r in this.Broker.GetProbeResults(this.Definition.SampleMask, this.MonitoringWindowStartTime, this.Result.ExecutionStartTime)
						where r.ResultType == ResultType.Failed
						orderby r.ExecutionEndTime
						select r;
						HashSet<string> additionalDefaultProcesses = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
						int probeResultCount = orderedEnumerable.Count<ProbeResult>();
						this.Broker.AsDataAccessQuery<ProbeResult>(orderedEnumerable).ExecuteAsync(delegate(ProbeResult r)
						{
							int num = r.ResultName.LastIndexOf("/");
							if (num > 0)
							{
								string text = r.ResultName.Substring(num + 1);
								bool flag = false;
								string[] array = text.Split(new char[]
								{
									'_'
								});
								if (array.Length > 0)
								{
									StackTraceAnalysisProcessNames stackTraceAnalysisProcessNames;
									flag = Enum.TryParse<StackTraceAnalysisProcessNames>(array[0], true, out stackTraceAnalysisProcessNames);
								}
								if (ProcessIsolationDiscovery.GetProcessConfiguration(text) == null && !ProcessIsolationMonitor.ProcessExclusionList.Contains(text, StringComparer.OrdinalIgnoreCase) && !flag)
								{
									this.Result.IsAlert = true;
									this.Result.LastFailedProbeId = r.WorkItemId;
									this.Result.LastFailedProbeResultId = r.ResultId;
									if (probeResultCount > 1)
									{
										additionalDefaultProcesses.Add(text);
									}
								}
							}
						}, cancellationToken, this.TraceContext);
						if (additionalDefaultProcesses.Count > 1)
						{
							this.Result.StateAttribute1 = string.Format("Monitor with ResultName:{0} is a catch-all monitor and has captured {1} processes that are not tracked by processConfigurationDictionary in ProcessIsolationMonitor.cs. The list is as follows:{2}", this.Result.ResultName, additionalDefaultProcesses.Count, string.Join("\r\n", additionalDefaultProcesses));
						}
					}
				}
				else
				{
					this.Result.IsAlert = false;
				}
				WTFDiagnostics.TraceFunction(ExTraceGlobals.CommonComponentsTracer, this.TraceContext, "OverallXFailuresMonitor: Finished analyzing probe results.", null, "DoMonitorWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\ProcessIsolation\\ProcessIsolationMonitor.cs", 645);
			}, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled, TaskScheduler.Default);
		}

		private static string GetUniqueShortProcessName(string processName, int maxLength)
		{
			if (string.IsNullOrEmpty(processName))
			{
				return processName;
			}
			if (maxLength < 4)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			string[] array = processName.Split(new char[]
			{
				'.'
			});
			int num = 0;
			string text;
			if (array != null && array.Length > 2)
			{
				int num2 = array.Length;
				while (num2 - num > 2)
				{
					stringBuilder.Append(char.ToUpper(array[num][0]) + ".");
					num++;
				}
				text = stringBuilder.ToString() + array[num2 - 2] + "." + array[num2 - 1];
			}
			else
			{
				text = processName;
			}
			if (text.Length > maxLength)
			{
				int startIndex = text.Length - maxLength + 3;
				return "..." + text.Substring(startIndex, maxLength - 3);
			}
			return text;
		}

		public const string DefaultProcessName = "Default";

		internal const int NumberOfGenericComponents = 5;

		private static readonly int maxCharsMonitorName = 50;

		private static int numberOfFailuresToAlert = 1;

		private static int alertingDuration = 2100;

		private static int recurrenceIntervalSeconds = 300;

		private static List<string> processExclusionList = new List<string>();

		private static List<SubComponentConfiguration> subComponentConfigurationList = new List<SubComponentConfiguration>
		{
			new SubComponentConfiguration(3, 0, ExchangeComponent.FrontendTransport, ProcessTrigger.ExchangeCrashEventTrigger_Error, false),
			new SubComponentConfiguration(3, 1, ExchangeComponent.Dal, ProcessTrigger.ExchangeCrashEventTrigger_Error, true),
			new SubComponentConfiguration(3, 2, ExchangeComponent.TransportExtensibility, ProcessTrigger.ExchangeCrashEventTrigger_Error, true),
			new SubComponentConfiguration(3, 3, ExchangeComponent.AntiSpam, ProcessTrigger.ExchangeCrashEventTrigger_Error, true),
			new SubComponentConfiguration(3, 4, ExchangeComponent.AntiSpam, ProcessTrigger.ExchangeCrashEventTrigger_Error, true),
			new SubComponentConfiguration(3, 8, ExchangeComponent.TransportExtensibility, ProcessTrigger.ExchangeCrashEventTrigger_Error, false),
			new SubComponentConfiguration(3, 5, ExchangeComponent.TransportExtensibility, ProcessTrigger.ExchangeCrashEventTrigger_Error, false),
			new SubComponentConfiguration(3, 6, ExchangeComponent.TransportExtensibility, ProcessTrigger.ExchangeCrashEventTrigger_Error, false),
			new SubComponentConfiguration(3, 50, ExchangeComponent.TransportExtensibility, ProcessTrigger.ExchangeCrashEventTrigger_Error, true),
			new SubComponentConfiguration(3, 54, ExchangeComponent.TransportExtensibility, ProcessTrigger.ExchangeCrashEventTrigger_Error, false),
			new SubComponentConfiguration(0, 7, ExchangeComponent.TransportExtensibility, ProcessTrigger.ExchangeCrashEventTrigger_Error, false),
			new SubComponentConfiguration(0, 0, ExchangeComponent.TransportExtensibility, ProcessTrigger.ExchangeCrashEventTrigger_Error, false),
			new SubComponentConfiguration(0, 8, ExchangeComponent.TransportExtensibility, ProcessTrigger.ExchangeCrashEventTrigger_Error, false),
			new SubComponentConfiguration(0, 6, ExchangeComponent.TransportExtensibility, ProcessTrigger.ExchangeCrashEventTrigger_Error, false),
			new SubComponentConfiguration(0, 10, ExchangeComponent.MrmArchive, ProcessTrigger.ExchangeCrashEventTrigger_Error, true),
			new SubComponentConfiguration(0, 11, ExchangeComponent.TransportExtensibility, ProcessTrigger.ExchangeCrashEventTrigger_Error, false),
			new SubComponentConfiguration(0, 12, ExchangeComponent.AntiSpam, ProcessTrigger.ExchangeCrashEventTrigger_Error, true),
			new SubComponentConfiguration(0, 13, ExchangeComponent.TransportExtensibility, ProcessTrigger.ExchangeCrashEventTrigger_Error, false),
			new SubComponentConfiguration(0, 9, ExchangeComponent.TransportExtensibility, ProcessTrigger.ExchangeCrashEventTrigger_Error, false),
			new SubComponentConfiguration(0, 14, ExchangeComponent.TransportExtensibility, ProcessTrigger.ExchangeCrashEventTrigger_Error, false),
			new SubComponentConfiguration(0, 15, ExchangeComponent.TransportExtensibility, ProcessTrigger.ExchangeCrashEventTrigger_Error, false),
			new SubComponentConfiguration(0, 16, ExchangeComponent.TransportExtensibility, ProcessTrigger.ExchangeCrashEventTrigger_Error, false),
			new SubComponentConfiguration(0, 17, ExchangeComponent.TransportExtensibility, ProcessTrigger.ExchangeCrashEventTrigger_Error, true),
			new SubComponentConfiguration(0, 18, ExchangeComponent.TransportExtensibility, ProcessTrigger.ExchangeCrashEventTrigger_Error, false),
			new SubComponentConfiguration(0, 19, ExchangeComponent.TransportExtensibility, ProcessTrigger.ExchangeCrashEventTrigger_Error, false),
			new SubComponentConfiguration(0, 20, ExchangeComponent.LiveId, ProcessTrigger.ExchangeCrashEventTrigger_Error, true),
			new SubComponentConfiguration(0, 21, ExchangeComponent.MrmArchive, ProcessTrigger.ExchangeCrashEventTrigger_Error, true),
			new SubComponentConfiguration(0, 47, ExchangeComponent.TransportExtensibility, ProcessTrigger.ExchangeCrashEventTrigger_Error, false),
			new SubComponentConfiguration(0, 22, ExchangeComponent.TransportExtensibility, ProcessTrigger.ExchangeCrashEventTrigger_Error, false),
			new SubComponentConfiguration(0, 23, ExchangeComponent.MrmArchive, ProcessTrigger.ExchangeCrashEventTrigger_Error, true),
			new SubComponentConfiguration(0, 24, ExchangeComponent.MrmArchive, ProcessTrigger.ExchangeCrashEventTrigger_Error, true),
			new SubComponentConfiguration(0, 25, ExchangeComponent.TransportExtensibility, ProcessTrigger.ExchangeCrashEventTrigger_Error, false),
			new SubComponentConfiguration(0, 26, ExchangeComponent.TransportExtensibility, ProcessTrigger.ExchangeCrashEventTrigger_Error, false),
			new SubComponentConfiguration(0, 27, ExchangeComponent.TransportExtensibility, ProcessTrigger.ExchangeCrashEventTrigger_Error, false),
			new SubComponentConfiguration(0, 52, ExchangeComponent.Security, ProcessTrigger.ExchangeCrashEventTrigger_Error, true),
			new SubComponentConfiguration(0, 53, ExchangeComponent.TransportExtensibility, ProcessTrigger.ExchangeCrashEventTrigger_Error, false),
			new SubComponentConfiguration(0, 54, ExchangeComponent.TransportExtensibility, ProcessTrigger.ExchangeCrashEventTrigger_Error, false),
			new SubComponentConfiguration(1, 0, ExchangeComponent.TransportExtensibility, ProcessTrigger.ExchangeCrashEventTrigger_Error, false),
			new SubComponentConfiguration(1, 28, ExchangeComponent.TransportExtensibility, ProcessTrigger.ExchangeCrashEventTrigger_Error, false),
			new SubComponentConfiguration(1, 29, ExchangeComponent.TransportExtensibility, ProcessTrigger.ExchangeCrashEventTrigger_Error, false),
			new SubComponentConfiguration(1, 30, ExchangeComponent.UM, ProcessTrigger.ExchangeCrashEventTrigger_Error, true),
			new SubComponentConfiguration(1, 31, ExchangeComponent.MrmArchive, ProcessTrigger.ExchangeCrashEventTrigger_Error, true),
			new SubComponentConfiguration(1, 32, ExchangeComponent.MailboxMigration, ProcessTrigger.ExchangeCrashEventTrigger_Error, true),
			new SubComponentConfiguration(1, 33, ExchangeComponent.TransportExtensibility, ProcessTrigger.ExchangeCrashEventTrigger_Error, false),
			new SubComponentConfiguration(1, 34, ExchangeComponent.PeopleConnect, ProcessTrigger.ExchangeCrashEventTrigger_Error, true),
			new SubComponentConfiguration(1, 35, ExchangeComponent.Owa, ProcessTrigger.ExchangeCrashEventTrigger_Error, true),
			new SubComponentConfiguration(1, 36, ExchangeComponent.UnifiedGroups, ProcessTrigger.ExchangeCrashEventTrigger_Error, true),
			new SubComponentConfiguration(1, 37, ExchangeComponent.MrmArchive, ProcessTrigger.ExchangeCrashEventTrigger_Error, true),
			new SubComponentConfiguration(1, 38, ExchangeComponent.UM, ProcessTrigger.ExchangeCrashEventTrigger_Error, true),
			new SubComponentConfiguration(1, 39, ExchangeComponent.UM, ProcessTrigger.ExchangeCrashEventTrigger_Error, true),
			new SubComponentConfiguration(1, 40, ExchangeComponent.TransportExtensibility, ProcessTrigger.ExchangeCrashEventTrigger_Error, false),
			new SubComponentConfiguration(1, 41, ExchangeComponent.Calendaring, ProcessTrigger.ExchangeCrashEventTrigger_Error, true),
			new SubComponentConfiguration(1, 42, ExchangeComponent.Search, ProcessTrigger.ExchangeCrashEventTrigger_Error, true),
			new SubComponentConfiguration(1, 43, ExchangeComponent.Inference, ProcessTrigger.ExchangeCrashEventTrigger_Error, true),
			new SubComponentConfiguration(1, 49, ExchangeComponent.UnifiedGroups, ProcessTrigger.ExchangeCrashEventTrigger_Error, true),
			new SubComponentConfiguration(1, 51, ExchangeComponent.Search, ProcessTrigger.ExchangeCrashEventTrigger_Error, true),
			new SubComponentConfiguration(2, 44, ExchangeComponent.TransportExtensibility, ProcessTrigger.ExchangeCrashEventTrigger_Error, false),
			new SubComponentConfiguration(2, 45, ExchangeComponent.TransportExtensibility, ProcessTrigger.ExchangeCrashEventTrigger_Error, false),
			new SubComponentConfiguration(2, 46, ExchangeComponent.TransportExtensibility, ProcessTrigger.ExchangeCrashEventTrigger_Error, false)
		};

		private static Dictionary<string, ProcessConfiguration> processConfigurationDictionary = new Dictionary<string, ProcessConfiguration>(StringComparer.OrdinalIgnoreCase)
		{
			{
				"Default",
				new ProcessConfiguration(ExchangeComponent.ProcessIsolation, ProcessType.Unknown, ProcessResponderConfiguration.CreateBugsAndEscalateResponse)
			},
			{
				"dfpowa",
				new ProcessConfiguration(ExchangeComponent.OwaDependency, ProcessType.AppPool, ProcessResponderConfiguration.NoResponse)
			},
			{
				"dfpowa1",
				new ProcessConfiguration(ExchangeComponent.OwaDependency, ProcessType.AppPool, ProcessResponderConfiguration.NoResponse)
			},
			{
				"dfpowa2",
				new ProcessConfiguration(ExchangeComponent.OwaDependency, ProcessType.AppPool, ProcessResponderConfiguration.NoResponse)
			},
			{
				"dfpowa3",
				new ProcessConfiguration(ExchangeComponent.OwaDependency, ProcessType.AppPool, ProcessResponderConfiguration.NoResponse)
			},
			{
				"dfpowa4",
				new ProcessConfiguration(ExchangeComponent.OwaDependency, ProcessType.AppPool, ProcessResponderConfiguration.NoResponse)
			},
			{
				"dfpowa5",
				new ProcessConfiguration(ExchangeComponent.OwaDependency, ProcessType.AppPool, ProcessResponderConfiguration.NoResponse)
			},
			{
				"dfpowa6",
				new ProcessConfiguration(ExchangeComponent.OwaDependency, ProcessType.AppPool, ProcessResponderConfiguration.NoResponse)
			},
			{
				"dw20",
				new ProcessConfiguration(ExchangeComponent.Eds, ProcessType.Program, ProcessResponderConfiguration.WatsonDiagKillEscalateResponse)
			},
			{
				"edgetransport",
				new ProcessConfiguration(ExchangeComponent.HubTransport, ProcessType.Service, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse, new Dictionary<string, object>
				{
					{
						"ServiceName",
						"MSExchangeTransport"
					}
				})
			},
			{
				"eseutil",
				new ProcessConfiguration(ExchangeComponent.DataProtection, ProcessType.Program, ProcessResponderConfiguration.EscalateOnlyResponse)
			},
			{
				"fms",
				new ProcessConfiguration(ExchangeComponent.AMFMSService, ProcessType.Service, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse)
			},
			{
				"Microsoft.Exchange.AntiMalware.Service",
				new ProcessConfiguration(ExchangeComponent.AMService, ProcessType.Service, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse)
			},
			{
				"hostcontrollerservice",
				new ProcessConfiguration(ExchangeComponent.Search, ProcessType.Service, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse)
			},
			{
				"microsoft.exchange.antispamupdatesvc",
				new ProcessConfiguration(ExchangeComponent.AntiSpam, ProcessType.Service, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse)
			},
			{
				"microsoft.exchange.audit.service",
				new ProcessConfiguration(ExchangeComponent.Security, ProcessType.Service, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse)
			},
			{
				"microsoft.exchange.diagnostics.service",
				new ProcessConfiguration(ExchangeComponent.Eds, ProcessType.Service, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse)
			},
			{
				"microsoft.exchange.directory.topologyservice",
				new ProcessConfiguration(ExchangeComponent.AD, ProcessType.Service, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse)
			},
			{
				"microsoft.exchange.imap4",
				new ProcessConfiguration(ExchangeComponent.ImapProtocol, ProcessType.Service, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse)
			},
			{
				"microsoft.exchange.imap4service",
				new ProcessConfiguration(ExchangeComponent.ImapProtocol, ProcessType.Service, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse)
			},
			{
				"microsoft.exchange.management.centralAdmin.centraladminservice",
				new ProcessConfiguration(ExchangeComponent.CentralAdmin, ProcessType.Service, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse)
			},
			{
				"microsoft.exchange.management.datamining.exchangefileupload",
				new ProcessConfiguration(ExchangeComponent.Datamining, ProcessType.Service, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse)
			},
			{
				"microsoft.exchange.management.forwardsync",
				new ProcessConfiguration(ExchangeComponent.Provisioning, ProcessType.Service, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse)
			},
			{
				"microsoft.exchange.monitoring",
				new ProcessConfiguration(ExchangeComponent.Monitoring, ProcessType.Service, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse)
			},
			{
				"microsoft.exchange.pop3",
				new ProcessConfiguration(ExchangeComponent.PopProtocol, ProcessType.Service, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse)
			},
			{
				"microsoft.exchange.pop3service",
				new ProcessConfiguration(ExchangeComponent.PopProtocol, ProcessType.Service, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse)
			},
			{
				"microsoft.exchange.protectedservicehost",
				new ProcessConfiguration(ExchangeComponent.AD, ProcessType.Service, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse)
			},
			{
				"microsoft.exchange.rpcclientaccess.service",
				new ProcessConfiguration(ExchangeComponent.OutlookProtocol, ProcessType.Service, ProcessResponderConfiguration.CollectDiagnosticsWatsonEscalateResponse)
			},
			{
				"microsoft.exchange.search.service",
				new ProcessConfiguration(ExchangeComponent.Search, ProcessType.Service, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse, new Dictionary<string, object>
				{
					{
						"CrashEventNotificationServiceClass",
						NotificationServiceClass.Scheduled
					}
				})
			},
			{
				"microsoft.exchange.servicehost",
				new ProcessConfiguration(ExchangeComponent.ProcessIsolation, ProcessType.Service, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse)
			},
			{
				"microsoft.exchange.sharedcache",
				new ProcessConfiguration(ExchangeComponent.SharedCache, ProcessType.Service, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse)
			},
			{
				"microsoft.exchange.store.service",
				new ProcessConfiguration(ExchangeComponent.Store, ProcessType.Service, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse)
			},
			{
				"microsoft.exchange.store.worker",
				new ProcessConfiguration(ExchangeComponent.Store, ProcessType.Service, ProcessResponderConfiguration.NoResponse)
			},
			{
				"microsoft.exchange.transportsyncmanagersvc",
				new ProcessConfiguration(ExchangeComponent.MailboxMigration, ProcessType.Service, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse)
			},
			{
				"microsoft.exchange.um.callrouter",
				new ProcessConfiguration(ExchangeComponent.UMCallRouter, ProcessType.Service, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse)
			},
			{
				"microsoft.office.bigdata.dataloader",
				new ProcessConfiguration(ExchangeComponent.Datamining, ProcessType.Service, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse)
			},
			{
				"microsoft.office.datacenter.torus.deployment",
				new ProcessConfiguration(ExchangeComponent.Security, ProcessType.Service, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse)
			},
			{
				"msexchangeautodiscoverapppool",
				new ProcessConfiguration(ExchangeComponent.AutodiscoverProtocol, ProcessType.AppPool, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse)
			},
			{
				"msexchangedagmgmt",
				new ProcessConfiguration(ExchangeComponent.DataProtection, ProcessType.Service, ProcessResponderConfiguration.CollectDiagnosticsWatsonEscalateResponse)
			},
			{
				"msexchangedelivery",
				new ProcessConfiguration(ExchangeComponent.MailboxTransport, ProcessType.Service, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse)
			},
			{
				"msexchangeecpapppool",
				(!FfoLocalEndpointManager.IsForefrontForOfficeDatacenter) ? new ProcessConfiguration(ExchangeComponent.Ecp, ProcessType.AppPool, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse) : new ProcessConfiguration(ExchangeComponent.FfoUmc, ProcessType.AppPool, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse)
			},
			{
				"msexchangeuccapppool",
				new ProcessConfiguration(ExchangeComponent.FfoUcc, ProcessType.AppPool, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse)
			},
			{
				"msexchangeencryptionapppool",
				new ProcessConfiguration(ExchangeComponent.E4E, ProcessType.AppPool, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse)
			},
			{
				"msexchangefrontendtransport",
				new ProcessConfiguration(ExchangeComponent.FrontendTransport, ProcessType.Service, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse)
			},
			{
				"msexchangehmhost",
				new ProcessConfiguration(ExchangeComponent.Monitoring, ProcessType.Service, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse)
			},
			{
				"msexchangehmworker",
				new ProcessConfiguration(ExchangeComponent.Monitoring, ProcessType.Service, ProcessResponderConfiguration.CollectDiagnosticsWatsonEscalateResponse)
			},
			{
				"msexchangemapiaddressbookapppool",
				new ProcessConfiguration(ExchangeComponent.AD, ProcessType.AppPool, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse)
			},
			{
				"msexchangemailboxassistants",
				new ProcessConfiguration(ExchangeComponent.ProcessIsolation, ProcessType.Service, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse)
			},
			{
				"msexchangemailboxreplication",
				new ProcessConfiguration(ExchangeComponent.MailboxMigration, ProcessType.Service, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse)
			},
			{
				"msexchangemapifrontendapppool",
				new ProcessConfiguration(ExchangeComponent.OutlookMapiProxy, ProcessType.AppPool, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse)
			},
			{
				"msexchangemapimailboxapppool",
				new ProcessConfiguration(ExchangeComponent.OutlookProtocol, ProcessType.AppPool, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse)
			},
			{
				"msexchangemigrationworkflow",
				new ProcessConfiguration(ExchangeComponent.MailboxMigration, ProcessType.Service, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse)
			},
			{
				"msexchangeoabapppool",
				new ProcessConfiguration(ExchangeComponent.Oab, ProcessType.AppPool, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse)
			},
			{
				"msexchangeoutlookserviceapppool",
				new ProcessConfiguration(ExchangeComponent.HxServiceMail, ProcessType.AppPool, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse)
			},
			{
				"msexchangeowaapppool",
				new ProcessConfiguration(ExchangeComponent.OwaProtocol, ProcessType.AppPool, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse)
			},
			{
				"msexchangeowacalendarapppool",
				new ProcessConfiguration(ExchangeComponent.Calendaring, ProcessType.AppPool, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse)
			},
			{
				"msexchangepowershellapppool",
				new ProcessConfiguration(ExchangeComponent.Rps, ProcessType.AppPool, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse)
			},
			{
				"msexchangepowershellfrontendapppool",
				new ProcessConfiguration(ExchangeComponent.Psws, ProcessType.AppPool, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse)
			},
			{
				"msexchangepowershellliveidapppool",
				new ProcessConfiguration(ExchangeComponent.Rps, ProcessType.AppPool, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse)
			},
			{
				"msexchangepowershellliveidfrontendapppool",
				new ProcessConfiguration(ExchangeComponent.RpsProxy, ProcessType.AppPool, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse)
			},
			{
				"msexchangepswsapppool",
				new ProcessConfiguration(ExchangeComponent.Psws, ProcessType.AppPool, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse)
			},
			{
				"msexchangepswsfrontendapppool",
				new ProcessConfiguration(ExchangeComponent.RwsProxy, ProcessType.AppPool, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse)
			},
			{
				"msexchangepushnotificationsapppool",
				new ProcessConfiguration(ExchangeComponent.PushNotificationsProtocol, ProcessType.AppPool, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse)
			},
			{
				"msexchangerepl",
				new ProcessConfiguration(ExchangeComponent.DataProtection, ProcessType.Service, ProcessResponderConfiguration.KillProcessEscalateResponse)
			},
			{
				"msexchangereportingwebserviceapppool",
				new ProcessConfiguration(ExchangeComponent.RwsProxy, ProcessType.AppPool, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse)
			},
			{
				"msexchangerpcproxyapppool",
				new ProcessConfiguration(ExchangeComponent.OutlookProtocol, ProcessType.AppPool, ProcessResponderConfiguration.CollectDiagnosticsWatsonEscalateResponse)
			},
			{
				"msexchangerpcproxyfrontendapppool",
				new ProcessConfiguration(ExchangeComponent.OutlookProxy, ProcessType.AppPool, ProcessResponderConfiguration.CollectDiagnosticsWatsonEscalateResponse)
			},
			{
				"msexchangeservicesapppool",
				new ProcessConfiguration(ExchangeComponent.EwsProtocol, ProcessType.AppPool, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse)
			},
			{
				"msexchangesubmission",
				new ProcessConfiguration(ExchangeComponent.MailboxTransport, ProcessType.Service, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse)
			},
			{
				"msexchangesyncapppool",
				new ProcessConfiguration(ExchangeComponent.ActiveSyncProtocol, ProcessType.AppPool, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse)
			},
			{
				"msexchangethrottling",
				new ProcessConfiguration(ExchangeComponent.HubTransport, ProcessType.Service, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse)
			},
			{
				"msexchangetransport",
				new ProcessConfiguration(ExchangeComponent.HubTransport, ProcessType.Service, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse)
			},
			{
				"msexchangetransportlogsearch",
				new ProcessConfiguration(ExchangeComponent.HubTransport, ProcessType.Service, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse)
			},
			{
				"msexchangexropapppool",
				new ProcessConfiguration(ExchangeComponent.ProcessIsolation, ProcessType.AppPool, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse)
			},
			{
				"msmessagetracingclient",
				new ProcessConfiguration(ExchangeComponent.MessageTracing, ProcessType.Service, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse)
			},
			{
				"ExtendedReportAppPool",
				new ProcessConfiguration(ExchangeComponent.ExtendedReportWeb, ProcessType.AppPool, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse)
			},
			{
				"noderunner#indexnode1",
				new ProcessConfiguration(ExchangeComponent.Search, ProcessType.Program, ProcessResponderConfiguration.RestartIndexNodeRunnerEscalateResponse, new Dictionary<string, object>
				{
					{
						"NodeName",
						"IndexNode1"
					},
					{
						"ProcessorAffinityCount",
						3
					},
					{
						"AvoidProcessorCount",
						3
					}
				}, new Func<bool>(ProcessIsolationMonitor.IsMailboxRoleInstalled))
			},
			{
				"noderunner#adminnode1",
				new ProcessConfiguration(ExchangeComponent.Search, ProcessType.Program, ProcessResponderConfiguration.RestartNodeRunnerEscalateResponse, new Dictionary<string, object>
				{
					{
						"NodeName",
						"AdminNode1"
					},
					{
						"ProcessorAffinityCount",
						1
					}
				}, new Func<bool>(ProcessIsolationMonitor.IsMailboxRoleInstalled))
			},
			{
				"noderunner#contentenginenode1",
				new ProcessConfiguration(ExchangeComponent.Search, ProcessType.Program, ProcessResponderConfiguration.RestartNodeRunnerEscalateResponse, new Dictionary<string, object>
				{
					{
						"NodeName",
						"ContentEngineNode1"
					},
					{
						"ProcessorAffinityCount",
						3
					},
					{
						"AvoidProcessorCount",
						3
					}
				}, new Func<bool>(ProcessIsolationMonitor.IsMailboxRoleInstalled))
			},
			{
				"noderunner#interactionenginenode1",
				new ProcessConfiguration(ExchangeComponent.Search, ProcessType.Program, ProcessResponderConfiguration.RestartNodeRunnerEscalateResponse, new Dictionary<string, object>
				{
					{
						"NodeName",
						"InteractionEngineNode1"
					},
					{
						"ProcessorAffinityCount",
						2
					},
					{
						"AvoidProcessorCount",
						3
					}
				}, new Func<bool>(ProcessIsolationMonitor.IsMailboxRoleInstalled))
			},
			{
				"scanningprocess",
				new ProcessConfiguration(ExchangeComponent.Fips, ProcessType.Service, ProcessResponderConfiguration.CollectDiagnosticsWatsonEscalateResponse)
			},
			{
				"sftracing",
				new ProcessConfiguration(ExchangeComponent.ProcessIsolation, ProcessType.Service, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse)
			},
			{
				"umservice",
				new ProcessConfiguration(ExchangeComponent.UMProtocol, ProcessType.Service, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse)
			},
			{
				"umworkerprocess",
				new ProcessConfiguration(ExchangeComponent.UMProtocol, ProcessType.Program, ProcessResponderConfiguration.CollectDiagnosticsWatsonEscalateResponse)
			},
			{
				"wsbexchange",
				new ProcessConfiguration(ExchangeComponent.Store, ProcessType.Service, ProcessResponderConfiguration.CollectDiagnosticsWatsonRestartEscalateResponse)
			},
			{
				"werfault",
				new ProcessConfiguration(ExchangeComponent.Eds, ProcessType.Program, ProcessResponderConfiguration.WatsonDiagKillEscalateResponse)
			},
			{
				"wermgr",
				new ProcessConfiguration(ExchangeComponent.Eds, ProcessType.Program, ProcessResponderConfiguration.WatsonDiagKillEscalateResponse)
			}
		};
	}
}
