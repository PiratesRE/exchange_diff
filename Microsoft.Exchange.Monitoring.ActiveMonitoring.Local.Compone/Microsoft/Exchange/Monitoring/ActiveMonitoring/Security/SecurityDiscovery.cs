using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local.Components.Common.Probes;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;
using Microsoft.Win32;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Security
{
	public sealed class SecurityDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			WTFDiagnostics.TraceInformation(ExTraceGlobals.SecurityTracer, base.TraceContext, "SecurityDiscovery:: DoWork(): Started Execution.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Security\\SecurityDiscovery.cs", 111);
			if (!LocalEndpointManager.IsDataCenter)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.SecurityTracer, base.TraceContext, "SecurityDiscovery:: DoWork(): Exited Execution. Security alerts should be monitored only in Datacenter environments and not in on-prem or Enterprise.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Security\\SecurityDiscovery.cs", 119);
				return;
			}
			if (FfoLocalEndpointManager.IsForefrontForOfficeDatacenter)
			{
				this.EscalationTeam = "Ops SE";
				this.EscalationService = "EOP";
			}
			this.CreateEventLogContext("SecurityEventLogCleared", "Security", "Microsoft-Windows-Eventlog", 1102, 0, NotificationServiceClass.Scheduled, null);
			this.CreateEventLogContext("AntimalwareClientDisabled", "System", "Microsoft Antimalware", 5001, 5000, NotificationServiceClass.Urgent, null);
			this.CreateEventLogContext("AntimalwareHistoryDeleted", "System", "Microsoft Antimalware", 1014, 1013, NotificationServiceClass.Urgent, null);
			this.CreateEventLogContext("AntimalwareActionFailed", "System", "Microsoft Antimalware", 1119, 1117, NotificationServiceClass.Urgent, null);
			if (FfoLocalEndpointManager.IsForefrontForOfficeDatacenter)
			{
				this.CreateEventLogContext("AntimalwareMalwareDetected", "System", "Microsoft Antimalware", 1116, 0, NotificationServiceClass.Urgent, Strings.SecurityAlertMalwareDetectedEscalationMessage);
			}
			if (!this.IsGallatin())
			{
				this.CreateProbeMonitorResponderForProcessWatcher("ProcessWatcherEvent", "Application", "MSExchangeIA", "1059", string.Empty, NotificationServiceClass.Scheduled);
			}
			this.CreateEventLogContext("AppLockerError-MisConfigurationInAppLockerPolicy", "Application", "MSExchangeIA", 1063, 0, NotificationServiceClass.UrgentInTraining, null);
			this.CreateEventLogContext("AppLockerDeploymentError-CouldNotRetrieveSID", "Application", "AppLocker Deployment", 1, 0, NotificationServiceClass.UrgentInTraining, null);
			this.CreateEventLogContext("AppLockerDeploymentError-CouldNotCreateGPOContainerAndWMIFilter", "Application", "AppLocker Deployment", 2, 0, NotificationServiceClass.UrgentInTraining, null);
			this.CreateEventLogContext("AppLockerDeploymentError", "Application", "AppLocker Deployment", 3, 0, NotificationServiceClass.UrgentInTraining, null);
			this.CreateEventLogContext("AppLockerDeploymentError-CouldNotSetErrorMessageAndStartupService", "Application", "AppLocker Deployment", 4, 0, NotificationServiceClass.UrgentInTraining, null);
			this.CreateEventLogContext("AppLockerError-UserRanProcessOutsideLockboxProcessExemption", "Application", "MSExchangeIA", 1065, 0, NotificationServiceClass.UrgentInTraining, null);
			WTFDiagnostics.TraceInformation(ExTraceGlobals.SecurityTracer, base.TraceContext, "SecurityDiscovery:: DoWork(): Completed Execution.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Security\\SecurityDiscovery.cs", 177);
		}

		private void CreateEventLogContext(string alertName, string logName, string providerName, int redEventId, int greenEventId = 0, NotificationServiceClass notificationType = NotificationServiceClass.Scheduled, string escalationMessage = null)
		{
			EventLogSubscription subscription = this.CreateEventLogNotification(alertName, logName, providerName, redEventId, greenEventId);
			MonitorDefinition monitorDefinition = this.CreateOverallXFailuresMonitor(alertName, subscription);
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate Security health is not impacted by issues reported in event logs";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			base.Broker.AddWorkDefinition<ResponderDefinition>(this.CreateEscalateResponder(alertName, monitorDefinition, notificationType, escalationMessage ?? Strings.SecurityAlertEscalationMessage(alertName)), base.TraceContext);
		}

		private bool IsGallatin()
		{
			bool result = false;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeLabs"))
			{
				if (registryKey == null || registryKey.GetValue("ServiceName") == null)
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.SecurityTracer, base.TraceContext, "Registry does not have SuppressNotifications key. Notifications are not suppressed.", null, "IsGallatin", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Security\\SecurityDiscovery.cs", 211);
				}
				else if (string.Equals(registryKey.GetValue("ServiceName").ToString(), "Gallatin", StringComparison.CurrentCultureIgnoreCase))
				{
					result = true;
				}
			}
			return result;
		}

		private void CreateProbeMonitorResponder(string name, string logName, string providerName, string redEventIds, string greenEventIds)
		{
			this.CreateProbeMonitorResponder(name, logName, providerName, redEventIds, greenEventIds, NotificationServiceClass.Scheduled, null);
		}

		private void CreateProbeMonitorResponder(string alertName, string logName, string providerName, string redEventIds, string greenEventIds, NotificationServiceClass notificationType, string escalationMessage = null)
		{
			ProbeDefinition probeDefinition = this.CreateGenericEventProbe(alertName, logName, providerName, redEventIds, greenEventIds);
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			MonitorDefinition monitorDefinition = this.CreateOverallXFailuresMonitor(alertName, probeDefinition);
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate Security health is not impacted by issues reported in event logs";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			base.Broker.AddWorkDefinition<ResponderDefinition>(this.CreateEscalateResponder(alertName, monitorDefinition, notificationType, escalationMessage ?? Strings.SecurityAlertEscalationMessage(alertName)), base.TraceContext);
		}

		private void CreateProbeMonitorResponderForProcessWatcher(string alertName, string logName, string providerName, string redEventIds, string greenEventIds, NotificationServiceClass notificationType)
		{
			ProbeDefinition probeDefinition = this.CreateGenericEventProbe(alertName, logName, providerName, redEventIds, greenEventIds);
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			MonitorDefinition monitorDefinition = this.CreateOverallXFailuresMonitor(alertName, probeDefinition);
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate Security health is not impacted by issues reported by Process Watcher";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			base.Broker.AddWorkDefinition<ResponderDefinition>(this.CreateEscalateResponderDefinitionForProcessWatcher(alertName, monitorDefinition, notificationType), base.TraceContext);
		}

		private ProbeDefinition CreateGenericEventProbe(string alertName, string logName, string providerName, string redEventId, string greenEventId)
		{
			ProbeDefinition probeDefinition = new ProbeDefinition();
			probeDefinition.AssemblyPath = SecurityDiscovery.AssemblyPath;
			probeDefinition.ServiceName = ExchangeComponent.Security.Name;
			probeDefinition.TypeName = typeof(SecurityDiscovery.SecurityEventLogProbe).FullName;
			probeDefinition.Name = string.Format("Security{0}Probe", alertName);
			probeDefinition.RecurrenceIntervalSeconds = SecurityDiscovery.RecurrenceIntervalSeconds;
			probeDefinition.TimeoutSeconds = ((SecurityDiscovery.RecurrenceIntervalSeconds != 0) ? (SecurityDiscovery.RecurrenceIntervalSeconds / 2) : 0);
			probeDefinition.MaxRetryAttempts = 2;
			probeDefinition.TargetResource = logName;
			probeDefinition.Attributes[GenericEventLogProbe.LogNameAttrName] = logName;
			probeDefinition.Attributes[GenericEventLogProbe.ProviderNameAttrName] = providerName;
			probeDefinition.Attributes[GenericEventLogProbe.RedEventsAttrName] = redEventId;
			probeDefinition.Attributes[GenericEventLogProbe.GreenEventsAttrName] = greenEventId;
			return probeDefinition;
		}

		private EventLogSubscription CreateEventLogNotification(string alertName, string logName, string providerName, int redEventId, int greenEventId = 0)
		{
			string name = string.Format("Security{0}Probe", alertName);
			EventLogSubscription eventLogSubscription;
			if (greenEventId == 0)
			{
				eventLogSubscription = new EventLogSubscription(name, TimeSpan.FromSeconds(1800.0), new EventMatchingRule(logName, providerName, new int[]
				{
					redEventId
				}, -1, false, false, null, null), null, null, null);
			}
			else
			{
				eventLogSubscription = new EventLogSubscription(name, TimeSpan.FromSeconds(1800.0), new EventMatchingRule(logName, providerName, new int[]
				{
					redEventId
				}, -1, false, false, null, null), new EventMatchingRule(logName, providerName, new int[]
				{
					greenEventId
				}, -1, false, false, null, null), null, null);
			}
			EventLogNotification.Instance.AddSubscription(eventLogSubscription);
			return eventLogSubscription;
		}

		private MonitorDefinition CreateOverallXFailuresMonitor(string alertName, ProbeDefinition probe)
		{
			MonitorDefinition monitorDefinition = new MonitorDefinition();
			monitorDefinition.AssemblyPath = typeof(OverallXFailuresMonitor).Assembly.Location;
			monitorDefinition.ServiceName = ExchangeComponent.Security.Name;
			monitorDefinition.TypeName = SecurityDiscovery.OverallXFailuresMonitorType.FullName;
			monitorDefinition.Name = string.Format("Security{0}Monitor", alertName);
			monitorDefinition.RecurrenceIntervalSeconds = SecurityDiscovery.RecurrenceIntervalSeconds;
			monitorDefinition.InsufficientSamplesIntervalSeconds = Math.Max(5 * monitorDefinition.RecurrenceIntervalSeconds, Convert.ToInt32(ConfigurationManager.AppSettings["InsufficientSamplesIntervalInSeconds"]));
			monitorDefinition.TimeoutSeconds = ((SecurityDiscovery.RecurrenceIntervalSeconds != 0) ? (SecurityDiscovery.RecurrenceIntervalSeconds / 2) : 0);
			monitorDefinition.MaxRetryAttempts = 2;
			monitorDefinition.SampleMask = probe.ConstructWorkItemResultName();
			monitorDefinition.MonitoringIntervalSeconds = SecurityDiscovery.MonitoringIntervalSeconds;
			monitorDefinition.TargetResource = probe.TargetResource;
			monitorDefinition.Enabled = true;
			monitorDefinition.MonitoringThreshold = 1.0;
			monitorDefinition.Component = ExchangeComponent.Security;
			return monitorDefinition;
		}

		private MonitorDefinition CreateOverallXFailuresMonitor(string alertName, EventLogSubscription subscription)
		{
			MonitorDefinition monitorDefinition = new MonitorDefinition();
			monitorDefinition.AssemblyPath = typeof(OverallXFailuresMonitor).Assembly.Location;
			monitorDefinition.ServiceName = ExchangeComponent.Security.Name;
			monitorDefinition.TypeName = SecurityDiscovery.OverallXFailuresMonitorType.FullName;
			monitorDefinition.Name = string.Format("Security{0}Monitor", alertName);
			monitorDefinition.RecurrenceIntervalSeconds = SecurityDiscovery.RecurrenceIntervalSeconds;
			monitorDefinition.InsufficientSamplesIntervalSeconds = Math.Max(5 * monitorDefinition.RecurrenceIntervalSeconds, Convert.ToInt32(ConfigurationManager.AppSettings["InsufficientSamplesIntervalInSeconds"]));
			monitorDefinition.TimeoutSeconds = ((SecurityDiscovery.RecurrenceIntervalSeconds != 0) ? (SecurityDiscovery.RecurrenceIntervalSeconds / 2) : 0);
			monitorDefinition.MaxRetryAttempts = 2;
			monitorDefinition.SampleMask = EventLogNotification.ConstructResultMask(subscription.Name, null);
			monitorDefinition.MonitoringIntervalSeconds = SecurityDiscovery.MonitoringIntervalSeconds;
			monitorDefinition.TargetResource = Environment.MachineName;
			monitorDefinition.Enabled = true;
			monitorDefinition.MonitoringThreshold = 1.0;
			monitorDefinition.Component = ExchangeComponent.Security;
			return monitorDefinition;
		}

		private ResponderDefinition CreateEscalateResponderDefinitionForProcessWatcher(string alertName, MonitorDefinition monitor, NotificationServiceClass notificationType)
		{
			string empty = string.Empty;
			string escalationMessageUnhealthy = string.Empty;
			new List<ResponderDefinition>();
			escalationMessageUnhealthy = Strings.SecurityAlertEscalationMessage(alertName);
			ResponderDefinition responderDefinition = EscalateResponder.CreateDefinition(string.Format("Security{0}Responder", alertName), ExchangeComponent.Security.Name, "ProcessWatcherEventBase", monitor.ConstructWorkItemResultName(), monitor.TargetResource, ServiceHealthStatus.None, this.EscalationTeam, alertName, escalationMessageUnhealthy, true, notificationType, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			responderDefinition.AssemblyPath = Assembly.GetExecutingAssembly().Location;
			responderDefinition.TypeName = typeof(SecurityDiscovery.ProcessWatcherEscalateResponder).FullName;
			return responderDefinition;
		}

		private ResponderDefinition CreateEscalateResponder(string alertName, MonitorDefinition monitor, NotificationServiceClass notificationType, string escalationMessage)
		{
			string empty = string.Empty;
			ResponderDefinition responderDefinition = EscalateResponder.CreateDefinition(string.Format("Security{0}Responder", alertName), ExchangeComponent.Security.Name, alertName, monitor.ConstructWorkItemResultName(), monitor.TargetResource, ServiceHealthStatus.None, this.EscalationTeam, alertName, escalationMessage, true, notificationType, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			if (!string.IsNullOrEmpty(this.EscalationService))
			{
				responderDefinition.EscalationService = this.EscalationService;
			}
			return responderDefinition;
		}

		private const string SecurityLogName = "Security";

		private const string SystemLogName = "System";

		private const string ApplicationLogName = "Application";

		private const string MicrosoftWindowsSecurityAuditingProvider = "Microsoft-Windows-Security-Auditing";

		private const string MicrosoftWindowsEventLogProvider = "Microsoft-Windows-Eventlog";

		private const string MicrosoftAntimalwareProvider = "Microsoft Antimalware";

		private const string MicrosoftExchangeIAProvider = "MSExchangeIA";

		private const string AppLockerDeploymentProvider = "AppLocker Deployment";

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly Type GenericEventLogProbeType = typeof(GenericEventLogProbe);

		private static readonly Type OverallXFailuresMonitorType = typeof(OverallXFailuresMonitor);

		private static readonly int RecurrenceIntervalSeconds = 480;

		private static readonly int MonitoringIntervalSeconds = 86400;

		private string EscalationTeam = "Security";

		private string EscalationService;

		public class SecurityEventLogProbe : GenericEventLogProbe
		{
			protected override void OnRedEvent(CentralEventLogWatcher.EventRecordMini redEvent)
			{
				string text = string.Empty;
				if (redEvent.CustomizedProperties != null && redEvent.CustomizedProperties.Count > 0)
				{
					text = string.Join(",", redEvent.CustomizedProperties.ToArray());
				}
				base.Result.StateAttribute11 = string.Format("Failed - Found Red Event {0} ({1}) : {2}", redEvent.EventId, (redEvent.TimeCreated != null) ? redEvent.TimeCreated.Value.ToUniversalTime().ToString() : "NULL", (text.Length > 900) ? text.Substring(0, 900) : text);
				throw new GenericEventLogProbe.RedEventFoundException(base.Result.StateAttribute11);
			}
		}

		public class ProcessWatcherEscalateResponder : EscalateResponder
		{
			protected override void InvokeNewServiceAlert(Guid alertGuid, string alertTypeId, string alertName, string alertDescription, DateTime raisedTime, string escalationTeam, string service, string alertSource, bool isDatacenter, bool urgent, string environment, string location, string forest, string dag, string site, string region, string capacityUnit, string rack, string alertCategory)
			{
				string text = "ProcessWatcherEvent";
				bool flag = false;
				string value = "The following process was started";
				string value2 = string.Empty;
				string str = string.Empty;
				string text2 = string.Empty;
				try
				{
					int num = alertDescription.IndexOf("-------------------------------------------------------------------------------");
					if (num > 0)
					{
						str = alertDescription.Substring(num, alertDescription.Length - num);
					}
					else
					{
						num = alertDescription.Length;
					}
					int num2 = alertDescription.IndexOf(value);
					if (num2 > 0)
					{
						value2 = alertDescription.Substring(0, num2);
						text2 = alertDescription.Substring(num2, num - num2).Trim();
						string[] array = null;
						if (text2.Contains("\\n") || text2.Contains("\\r"))
						{
							array = text2.Split(new string[]
							{
								"\\r",
								"\\n"
							}, StringSplitOptions.RemoveEmptyEntries);
						}
						else
						{
							array = text2.Split(new char[]
							{
								'\r',
								'\n'
							}, StringSplitOptions.RemoveEmptyEntries);
						}
						if (array.Length % 6 == 0)
						{
							for (int i = 0; i < array.Length; i += 6)
							{
								StringBuilder stringBuilder = new StringBuilder();
								string[] array2 = array[i + 1].Split(new string[]
								{
									": "
								}, StringSplitOptions.None);
								string str2 = (array2.Length > 1) ? array2[1] : "Unknown";
								stringBuilder.Append(value2);
								stringBuilder.Append(array[i] + "\r\n");
								stringBuilder.Append(array[i + 1] + "\r\n");
								stringBuilder.Append(array[i + 2] + "\r\n");
								stringBuilder.Append(array[i + 3] + "\r\n");
								stringBuilder.Append(array[i + 4] + "\r\n");
								stringBuilder.Append(array[i + 5] + "\r\n");
								stringBuilder.Append(str + "\r\n");
								try
								{
									base.InvokeNewServiceAlert(alertGuid, text + "-" + str2, alertName + "-" + str2, stringBuilder.ToString(), raisedTime, escalationTeam, service, alertSource, isDatacenter, urgent, environment, location, forest, dag, site, region, capacityUnit, rack, alertCategory);
								}
								catch (Exception ex)
								{
									WTFDiagnostics.TraceInformation(ExTraceGlobals.SecurityTracer, base.TraceContext, "ProcessWatcherEscalateResponder.InvokeNewServiceAlert:Unexpected failure. An error occurred while invoking new alert. " + ex.Message, null, "InvokeNewServiceAlert", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Security\\SecurityDiscovery.cs", 652);
								}
							}
						}
						else
						{
							flag = true;
							WTFDiagnostics.TraceInformation(ExTraceGlobals.SecurityTracer, base.TraceContext, "ProcessWatcherEscalateResponder.InvokeNewServiceAlert:Unexpected failure. An error occurred while parsing the alert description.", null, "InvokeNewServiceAlert", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Security\\SecurityDiscovery.cs", 662);
						}
					}
					else
					{
						flag = true;
					}
				}
				catch (Exception ex2)
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.SecurityTracer, base.TraceContext, "ProcessWatcherEscalateResponder.InvokeNewServiceAlert:Unexpected failure. An error occurred while invoking new alert. " + ex2.Message, null, "InvokeNewServiceAlert", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Security\\SecurityDiscovery.cs", 676);
					flag = true;
				}
				if (flag)
				{
					base.InvokeNewServiceAlert(alertGuid, text, alertName, alertDescription, raisedTime, escalationTeam, service, alertSource, isDatacenter, urgent, environment, location, forest, dag, site, region, capacityUnit, rack, alertCategory);
				}
			}
		}
	}
}
