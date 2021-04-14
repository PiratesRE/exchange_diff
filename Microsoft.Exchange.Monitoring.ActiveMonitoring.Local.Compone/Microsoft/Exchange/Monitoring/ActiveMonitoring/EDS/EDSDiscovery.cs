using System;
using System.Configuration;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.EDS
{
	public sealed class EDSDiscovery : MaintenanceWorkItem
	{
		private static string GetRoleBasedEscalationTeam()
		{
			ExchangeServerRoleEndpoint exchangeServerRoleEndpoint = LocalEndpointManager.Instance.ExchangeServerRoleEndpoint;
			if (exchangeServerRoleEndpoint != null)
			{
				if (exchangeServerRoleEndpoint.IsCentralAdminDatabaseRoleInstalled)
				{
					return ExchangeComponent.Osp.EscalationTeam;
				}
				if (exchangeServerRoleEndpoint.IsCentralAdminFrontEndRoleInstalled || exchangeServerRoleEndpoint.IsCentralAdminRoleInstalled)
				{
					return ExchangeComponent.CentralAdmin.EscalationTeam;
				}
			}
			return "Performance";
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			WTFDiagnostics.TraceFunction(ExTraceGlobals.EDSTracer, base.TraceContext, "EDSDiscovery.DoWork", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\EDS\\EDSDiscovery.cs", 166);
			this.CreateEDSServiceContext();
			this.CreateTotalCPUAlertContext();
			if (!LocalEndpointManager.Instance.ExchangeServerRoleEndpoint.IsCentralAdminDatabaseRoleInstalled)
			{
				this.CreateLowMemoryAlertContext();
			}
			this.CreateEDSJobPoisonedAlertContext();
			this.CreateSqlOutputStreamInRetryAlertContext();
		}

		private void CreateEDSServiceContext()
		{
			bool enabled = bool.Parse(base.Definition.Attributes["EDSServiceRunningEnabled"]);
			int recurrenceIntervalSeconds = int.Parse(base.Definition.Attributes["EDSServiceRunningProbeRecurrenceIntervalSeconds"]);
			ProbeDefinition definition = this.CreateProbeDefinition("EDSServiceRunningProbe", typeof(GenericServiceProbe), "MSExchangeDiagnostics", recurrenceIntervalSeconds, enabled);
			base.Broker.AddWorkDefinition<ProbeDefinition>(definition, base.TraceContext);
			int recurrenceIntervalSeconds2 = int.Parse(base.Definition.Attributes["EDSServiceRunningMonitorRecurrenceIntervalSeconds"]);
			int monitoringIntervalSeconds = int.Parse(base.Definition.Attributes["EDSServiceRunningMonitorMonitoringIntervalSeconds"]);
			int monitoringThreshold = int.Parse(base.Definition.Attributes["EDSServiceRunningMonitorMonitoringThreshold"]);
			int transitionTimeoutSeconds = int.Parse(base.Definition.Attributes["EDSServiceRunningMonitorUnhealthyStateTransitionTimeOut"]);
			MonitorDefinition monitorDefinition = this.CreateMonitorDefinition("EDSServiceRunningMonitor", EDSDiscovery.OverallXFailuresMonitorType, "EDSServiceRunningProbe/MSExchangeDiagnostics", ExchangeComponent.Eds.Name, "MSExchangeDiagnostics", recurrenceIntervalSeconds2, monitoringIntervalSeconds, monitoringThreshold, enabled);
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Degraded, 0),
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, transitionTimeoutSeconds)
			};
			monitorDefinition.ServicePriority = 1;
			monitorDefinition.ScenarioDescription = "Validate EDS health is not impacted by any issues";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			int num = int.Parse(base.Definition.Attributes["EDSServiceRestartTimeoutSeconds"]);
			int waitIntervalSeconds = int.Parse(base.Definition.Attributes["EDSServiceRestartResponderThrottleSeconds"]);
			ResponderDefinition responderDefinition = this.CreateResponderDefinition("MSExchangeDiagnostics", EDSDiscovery.RestartServiceResponderType, "EDSServiceRestartResponder", monitorDefinition.Name, monitorDefinition.Name, recurrenceIntervalSeconds2, waitIntervalSeconds);
			responderDefinition.TargetHealthState = ServiceHealthStatus.Degraded;
			responderDefinition.Attributes["ServiceStartTimeout"] = TimeSpan.FromSeconds((double)num).ToString();
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
			ResponderDefinition responderDefinition2 = this.CreateEscalateResponderDefinition("EDSServiceRunningMonitor", ExchangeComponent.Eds.Name, "MSExchangeDiagnostics", Strings.EscalationSubjectUnhealthy, Strings.EscalationMessageFailuresUnhealthy(Strings.EDSServiceNotRunningEscalationMessage), enabled, NotificationServiceClass.Urgent, "Performance");
			responderDefinition2.TargetHealthState = ServiceHealthStatus.Unhealthy;
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition2, base.TraceContext);
		}

		private void CreateTotalCPUAlertContext()
		{
			this.CreateCustomTotalCPUAlertContext("_Error", NotificationServiceClass.Urgent, Strings.CPUOverThresholdErrorEscalationSubject);
			this.CreateCustomTotalCPUAlertContext("_Warning", NotificationServiceClass.UrgentInTraining, Strings.CPUOverThresholdWarningEscalationSubject);
		}

		private void CreateCustomTotalCPUAlertContext(string alertSuffix, NotificationServiceClass escalationType, string escalationSubject)
		{
			bool enabled = bool.Parse(base.Definition.Attributes["TotalCPUAlertEnabled"]);
			int recurrenceIntervalSeconds = int.Parse(base.Definition.Attributes["TotalCPUAlertMonitorRecurrenceIntervalSeconds"]);
			int monitoringIntervalSeconds = int.Parse(base.Definition.Attributes["TotalCPUAlertMonitorMonitoringIntervalSeconds"]);
			int monitoringThreshold = int.Parse(base.Definition.Attributes["TotalCPUAlertMonitorMonitoringThreshold"]);
			int num = int.Parse(base.Definition.Attributes["F1TraceDurationSeconds"]);
			int durationInSeconds = int.Parse(base.Definition.Attributes["ExmonTraceDurationSeconds"]);
			int durationInSeconds2 = int.Parse(base.Definition.Attributes["ADDiagTraceDurationSeconds"]);
			string sampleMask = NotificationItem.GenerateResultName(ExchangeComponent.Eds.Name, "ExchangeProcessorTimeTrigger" + alertSuffix, "_total");
			string text = "TotalCPUOverThresholdMonitor" + alertSuffix;
			string str = "TotalCPUOverThresholdResponder" + alertSuffix;
			MonitorDefinition monitorDefinition = this.CreateMonitorDefinition(text, EDSDiscovery.OverallXFailuresMonitorType, sampleMask, ExchangeComponent.Eds.Name, ExchangeComponent.Eds.Name, recurrenceIntervalSeconds, monitoringIntervalSeconds, monitoringThreshold, enabled);
			monitorDefinition.Attributes.Add("MinimumSampleCount", "0");
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 0),
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy1, 15),
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy2, 30),
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, 300)
			};
			monitorDefinition.ServicePriority = 1;
			monitorDefinition.ScenarioDescription = "Validate EDS based server health is not impacted by CPU performance issues";
			monitorDefinition.TargetScopes = "{Dag}";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			string alertMask = text + "/" + ExchangeComponent.Eds.Name;
			if (VariantConfiguration.InvariantNoFlightingSnapshot.ActiveMonitoring.F1TraceResponder.Enabled)
			{
				string name = str + "F1Trace";
				ResponderDefinition definition = F1TraceResponder.CreateDefinition(name, ExchangeComponent.Eds.Name, ExchangeComponent.Eds.Name, alertMask, ExchangeComponent.Eds.Name, ServiceHealthStatus.Unhealthy, 2, TimeSpan.FromSeconds((double)num), 0, string.Empty, string.Empty);
				base.Broker.AddWorkDefinition<ResponderDefinition>(definition, base.TraceContext);
			}
			if (VariantConfiguration.InvariantNoFlightingSnapshot.ActiveMonitoring.TraceLogResponder.Enabled)
			{
				ServiceHealthStatus targetHealthState = ServiceHealthStatus.Unhealthy1;
				ExchangeServerRoleEndpoint exchangeServerRoleEndpoint = LocalEndpointManager.Instance.ExchangeServerRoleEndpoint;
				if (exchangeServerRoleEndpoint != null && exchangeServerRoleEndpoint.IsMailboxRoleInstalled)
				{
					string name2 = str + "ExmonTrace";
					TraceLogResponder.TraceAttributes traceAttributes = new TraceLogResponder.TraceAttributes(name2, ExchangeComponent.Eds.Name, ExchangeComponent.Eds.Name, alertMask, ExchangeComponent.Eds.Name, ServiceHealthStatus.Unhealthy1, "2EACCEDF-8648-453e-9250-27F0069F71D2", string.Empty, string.Empty, durationInSeconds, string.Empty, sampleMask, false);
					ResponderDefinition definition2 = TraceLogResponder.CreateDefinition(traceAttributes);
					base.Broker.AddWorkDefinition<ResponderDefinition>(definition2, base.TraceContext);
					targetHealthState = ServiceHealthStatus.Unhealthy2;
				}
				WindowsServerRoleEndpoint windowsServerRoleEndpoint = LocalEndpointManager.Instance.WindowsServerRoleEndpoint;
				if (windowsServerRoleEndpoint != null && windowsServerRoleEndpoint.IsDirectoryServiceRoleInstalled)
				{
					string name3 = str + "ADDiagTrace";
					TraceLogResponder.TraceAttributes traceAttributes2 = new TraceLogResponder.TraceAttributes(name3, ExchangeComponent.Eds.Name, ExchangeComponent.Eds.Name, alertMask, ExchangeComponent.Eds.Name, targetHealthState, "1C83B2FC-C04F-11D1-8AFC-00C04FC21914,8E598056-8993-11D2-819E-0000F875A064,BBA3ADD2-C229-4CDB-AE2B-57EB6966B0C4,24DB8964-E6BC-11D1-916A-0000F8045B04,CC85922F-DB41-11D2-9244-006008269001,C92CF544-91B3-4DC0-8E11-C580339A0BF8", string.Empty, string.Empty, durationInSeconds2, string.Empty, sampleMask, true);
					ResponderDefinition definition3 = TraceLogResponder.CreateDefinition(traceAttributes2);
					base.Broker.AddWorkDefinition<ResponderDefinition>(definition3, base.TraceContext);
				}
			}
			ResponderDefinition responderDefinition = this.CreateEscalateResponderWithMessageDefinition(text, ExchangeComponent.Eds.Name, ExchangeComponent.Eds.Name, escalationSubject, "{Probe.Error}", enabled, escalationType, EDSDiscovery.GetRoleBasedEscalationTeam());
			responderDefinition.TargetHealthState = ServiceHealthStatus.Unrecoverable;
			responderDefinition.WaitIntervalSeconds = int.Parse(base.Definition.Attributes["EDSServiceEscalateResponderThrottleSeconds"]);
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
		}

		private void CreateLowMemoryAlertContext()
		{
			bool enabled = bool.Parse(base.Definition.Attributes["LowMemoryAlertEnabled"]);
			int recurrenceIntervalSeconds = int.Parse(base.Definition.Attributes["LowMemoryAlertMonitorRecurrenceIntervalSeconds"]);
			int monitoringIntervalSeconds = int.Parse(base.Definition.Attributes["LowMemoryAlertMonitorMonitoringIntervalSeconds"]);
			int monitoringThreshold = int.Parse(base.Definition.Attributes["LowMemoryAlertMonitorMonitoringThreshold"]);
			string sampleMask = NotificationItem.GenerateResultName(ExchangeComponent.Eds.Name, "MemoryPercentAvailableTrigger_Error", null);
			MonitorDefinition monitorDefinition = this.CreateMonitorDefinition("LowMemoryThresholdMonitor_Error", EDSDiscovery.OverallXFailuresMonitorType, sampleMask, ExchangeComponent.Eds.Name, ExchangeComponent.Eds.Name, recurrenceIntervalSeconds, monitoringIntervalSeconds, monitoringThreshold, enabled);
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, 0)
			};
			monitorDefinition.Attributes.Add("MinimumSampleCount", "0");
			monitorDefinition.ServicePriority = 1;
			monitorDefinition.ScenarioDescription = "Validate server health is not impacted by system-wide physical memory availability";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			ResponderDefinition responderDefinition = this.CreateEscalateResponderDefinition("LowMemoryThresholdMonitor_Error", ExchangeComponent.Eds.Name, ExchangeComponent.Eds.Name, Strings.LowMemoryUnderThresholdErrorEscalationSubject, "{Probe.Error}", enabled, NotificationServiceClass.Urgent, EDSDiscovery.GetRoleBasedEscalationTeam());
			responderDefinition.TargetHealthState = ServiceHealthStatus.Unrecoverable;
			responderDefinition.WaitIntervalSeconds = int.Parse(base.Definition.Attributes["EDSServiceEscalateResponderThrottleSeconds"]);
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
		}

		private void CreateEDSJobPoisonedAlertContext()
		{
			bool enabled = bool.Parse(base.Definition.Attributes["EDSJobPoisonedAlertEnabled"]);
			int recurrenceIntervalSeconds = int.Parse(base.Definition.Attributes["EDSJobPoisonedAlertMonitorRecurrenceIntervalSeconds"]);
			int monitoringIntervalSeconds = int.Parse(base.Definition.Attributes["EDSJobPoisonedAlertMonitorMonitoringIntervalSeconds"]);
			int monitoringThreshold = int.Parse(base.Definition.Attributes["EDSJobPoisonedAlertMonitorMonitoringThreshold"]);
			string sampleMask = NotificationItem.GenerateResultName(ExchangeComponent.Eds.Name, "PoisonManager", null);
			MonitorDefinition monitorDefinition = this.CreateMonitorDefinition("EDSJobPoisonedMonitor", EDSDiscovery.OverallXFailuresMonitorType, sampleMask, ExchangeComponent.Eds.Name, ExchangeComponent.Eds.Name, recurrenceIntervalSeconds, monitoringIntervalSeconds, monitoringThreshold, enabled);
			monitorDefinition.ServicePriority = 1;
			monitorDefinition.ScenarioDescription = "Validate EDS service health is not impacted any poisoned job issues";
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, 0)
			};
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			ResponderDefinition responderDefinition = this.CreateEscalateResponderDefinition("EDSJobPoisonedMonitor", ExchangeComponent.Eds.Name, ExchangeComponent.Eds.Name, Strings.EDSJobPoisonedEscalationMessage, "{Probe.Error}", enabled, NotificationServiceClass.Scheduled, "Performance");
			responderDefinition.TargetHealthState = ServiceHealthStatus.Unrecoverable;
			responderDefinition.WaitIntervalSeconds = int.Parse(base.Definition.Attributes["EDSServiceEscalateResponderThrottleSeconds"]);
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
		}

		private void CreateSqlOutputStreamInRetryAlertContext()
		{
			bool enabled = bool.Parse(base.Definition.Attributes["SqlOutputStreamInRetryEnabled"]);
			int recurrenceIntervalSeconds = int.Parse(base.Definition.Attributes["SqlOutputStreamInRetryMonitorRecurrenceIntervalSeconds"]);
			int monitoringIntervalSeconds = int.Parse(base.Definition.Attributes["SqlOutputStreamInRetryMonitorMonitoringIntervalSeconds"]);
			int monitoringThreshold = int.Parse(base.Definition.Attributes["SqlOutputStreamInRetryMonitorMonitoringThreshold"]);
			string sampleMask = NotificationItem.GenerateResultName(ExchangeComponent.Eds.Name, "SqlOutputStreamConsecutiveRetriesForASpecifiedTime", null);
			MonitorDefinition monitorDefinition = this.CreateMonitorDefinition("SqlOutputStreamInRetryMonitor", EDSDiscovery.OverallXFailuresMonitorType, sampleMask, ExchangeComponent.Eds.Name, ExchangeComponent.Eds.Name, recurrenceIntervalSeconds, monitoringIntervalSeconds, monitoringThreshold, enabled);
			monitorDefinition.ServicePriority = 1;
			monitorDefinition.ScenarioDescription = "Validate EDS service health is not impacted by SQL output stream issues";
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, 0)
			};
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			ResponderDefinition responderDefinition = this.CreateEscalateResponderDefinition("SqlOutputStreamInRetryMonitor", ExchangeComponent.Eds.Name, ExchangeComponent.Eds.Name, Strings.SqlOutputStreamInRetryEscalationMessage, "{Probe.Error}", enabled, NotificationServiceClass.Scheduled, "Performance");
			responderDefinition.TargetHealthState = ServiceHealthStatus.Unrecoverable;
			responderDefinition.WaitIntervalSeconds = int.Parse(base.Definition.Attributes["EDSServiceEscalateResponderThrottleSeconds"]);
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
		}

		private ProbeDefinition CreateProbeDefinition(string probeName, Type probeType, string targetResource, int recurrenceIntervalSeconds, bool enabled)
		{
			ProbeDefinition probeDefinition = new ProbeDefinition();
			probeDefinition.AssemblyPath = probeType.Assembly.Location;
			probeDefinition.TypeName = probeType.FullName;
			probeDefinition.Name = probeName;
			probeDefinition.RecurrenceIntervalSeconds = recurrenceIntervalSeconds;
			probeDefinition.TimeoutSeconds = recurrenceIntervalSeconds;
			probeDefinition.MaxRetryAttempts = 3;
			probeDefinition.TargetResource = targetResource;
			probeDefinition.ServiceName = ExchangeComponent.Eds.Name;
			probeDefinition.Enabled = enabled;
			WTFDiagnostics.TraceInformation<string, string>(ExTraceGlobals.EDSTracer, base.TraceContext, "EDSDiscovery.CreateProbeDefinition: Created ProbeDefinition '{0}' for '{1}'.", probeName, targetResource, null, "CreateProbeDefinition", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\EDS\\EDSDiscovery.cs", 633);
			return probeDefinition;
		}

		private MonitorDefinition CreateMonitorDefinition(string monitorName, Type monitorType, string sampleMask, string serviceName, string targetResource, int recurrenceIntervalSeconds, int monitoringIntervalSeconds, int monitoringThreshold, bool enabled)
		{
			MonitorDefinition monitorDefinition = new MonitorDefinition();
			monitorDefinition.Name = monitorName;
			monitorDefinition.AssemblyPath = monitorType.Assembly.Location;
			monitorDefinition.TypeName = monitorType.FullName;
			monitorDefinition.SampleMask = sampleMask;
			monitorDefinition.ServiceName = serviceName;
			monitorDefinition.TargetResource = targetResource;
			monitorDefinition.Component = ExchangeComponent.Eds;
			monitorDefinition.RecurrenceIntervalSeconds = recurrenceIntervalSeconds;
			monitorDefinition.InsufficientSamplesIntervalSeconds = Math.Max(5 * monitorDefinition.RecurrenceIntervalSeconds, Convert.ToInt32(ConfigurationManager.AppSettings["InsufficientSamplesIntervalInSeconds"]));
			monitorDefinition.TimeoutSeconds = recurrenceIntervalSeconds;
			monitorDefinition.MonitoringIntervalSeconds = monitoringIntervalSeconds;
			monitorDefinition.MonitoringThreshold = (double)monitoringThreshold;
			monitorDefinition.Enabled = enabled;
			WTFDiagnostics.TraceInformation<string, string>(ExTraceGlobals.EDSTracer, base.TraceContext, "EDSDiscovery.CreateMonitorDefinition: Created MonitorDefinition '{0}' for '{1}'", monitorName, targetResource, null, "CreateMonitorDefinition", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\EDS\\EDSDiscovery.cs", 682);
			return monitorDefinition;
		}

		private ResponderDefinition CreateEscalateResponderDefinition(string monitorName, string serviceName, string targetResource, string escalationSubject, string escalationMessage, bool enabled, NotificationServiceClass notificationType, string roleBasedEscalationTeam)
		{
			string text = monitorName + "EscalateResponder";
			string alertMask = string.IsNullOrEmpty(targetResource) ? monitorName : (monitorName + "/" + targetResource);
			ResponderDefinition result = EscalateResponder.CreateDefinition(text, serviceName, monitorName, alertMask, targetResource, ServiceHealthStatus.None, roleBasedEscalationTeam, escalationSubject, escalationMessage, enabled, notificationType, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			WTFDiagnostics.TraceInformation<string, string>(ExTraceGlobals.EDSTracer, base.TraceContext, "EDSDiscovery.CreateEscalateResponderDefinition: Created Escalate ResponderDefinition '{0}' for '{1}'", text, targetResource, null, "CreateEscalateResponderDefinition", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\EDS\\EDSDiscovery.cs", 728);
			return result;
		}

		private ResponderDefinition CreateEscalateResponderWithMessageDefinition(string monitorName, string serviceName, string targetResource, string escalationSubject, string escalationMessage, bool enabled, NotificationServiceClass notificationType, string roleBasedEscalationTeam)
		{
			string text = monitorName + "EscalateResponder";
			string alertMask = string.IsNullOrEmpty(targetResource) ? monitorName : (monitorName + "/" + targetResource);
			ResponderDefinition result = EscalateResponderWithCustomMessage.CreateDefinition(text, serviceName, monitorName, alertMask, targetResource, ServiceHealthStatus.None, roleBasedEscalationTeam, escalationSubject, escalationMessage, enabled, notificationType, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59");
			WTFDiagnostics.TraceInformation<string, string>(ExTraceGlobals.EDSTracer, base.TraceContext, "EDSDiscovery.CreateEscalateResponderDefinition: Created Escalate ResponderDefinition '{0}' for '{1}'", text, targetResource, null, "CreateEscalateResponderWithMessageDefinition", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\EDS\\EDSDiscovery.cs", 775);
			return result;
		}

		private ResponderDefinition CreateResponderDefinition(string targetResource, Type responderType, string responderName, string alertMask, string alertTypeId, int recurrenceIntervalSeconds, int waitIntervalSeconds)
		{
			WTFDiagnostics.TraceDebug<string, string>(ExTraceGlobals.StoreTracer, base.TraceContext, "EDSDiscovery.DoWork: Creating {0} for {1}", responderName, targetResource, null, "CreateResponderDefinition", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\EDS\\EDSDiscovery.cs", 805);
			ResponderDefinition responderDefinition = new ResponderDefinition();
			responderDefinition.AssemblyPath = responderType.Assembly.Location;
			responderDefinition.TypeName = responderType.FullName;
			responderDefinition.Name = responderName;
			responderDefinition.RecurrenceIntervalSeconds = recurrenceIntervalSeconds;
			responderDefinition.TimeoutSeconds = recurrenceIntervalSeconds;
			responderDefinition.MaxRetryAttempts = 3;
			responderDefinition.Attributes["WindowsServiceName"] = targetResource;
			responderDefinition.AlertMask = alertMask;
			responderDefinition.AlertTypeId = alertTypeId;
			responderDefinition.ServiceName = ExchangeComponent.Eds.Name;
			responderDefinition.WaitIntervalSeconds = waitIntervalSeconds;
			WTFDiagnostics.TraceDebug<string, string>(ExTraceGlobals.StoreTracer, base.TraceContext, "StoreDiscovery.DoWork: Created {0} for {1}", responderName, targetResource, null, "CreateResponderDefinition", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\EDS\\EDSDiscovery.cs", 826);
			return responderDefinition;
		}

		internal const string ServiceRunningProbeName = "EDSServiceRunningProbe";

		private const int MaxRetryAttempt = 3;

		private const int NumberOfProcessesCollectedByF1Trace = 2;

		private const string DefaultEscalationTeam = "Performance";

		private const string ServiceName = "MSExchangeDiagnostics";

		private const string CPUAlertComponentName = "ExchangeProcessorTimeTrigger";

		private const string LowMemoryAlertComponentName = "MemoryPercentAvailableTrigger_Error";

		private const string EDSJobPoisonedAlertComponentName = "PoisonManager";

		private const string SqlOutputStreamInRetryComponentName = "SqlOutputStreamConsecutiveRetriesForASpecifiedTime";

		private const string ExmonTraceGuid = "2EACCEDF-8648-453e-9250-27F0069F71D2";

		private const string ADDiagTraceGuid = "1C83B2FC-C04F-11D1-8AFC-00C04FC21914,8E598056-8993-11D2-819E-0000F875A064,BBA3ADD2-C229-4CDB-AE2B-57EB6966B0C4,24DB8964-E6BC-11D1-916A-0000F8045B04,CC85922F-DB41-11D2-9244-006008269001,C92CF544-91B3-4DC0-8E11-C580339A0BF8";

		private const string ServiceRunningMonitorName = "EDSServiceRunningMonitor";

		private const string ServiceRestartResponderName = "EDSServiceRestartResponder";

		private const string TotalCPUOverThresholdMonitorName = "TotalCPUOverThresholdMonitor";

		private const string TotalCPUOverThresholdResponderName = "TotalCPUOverThresholdResponder";

		private const string LowMemoryUnderThresholdMonitorName = "LowMemoryThresholdMonitor_Error";

		private const string EDSJobPoisonedMonitorName = "EDSJobPoisonedMonitor";

		private const string SqlOutputStreamInRetryMonitorName = "SqlOutputStreamInRetryMonitor";

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly Type OverallXFailuresMonitorType = typeof(OverallXFailuresMonitor);

		private static readonly Type RestartServiceResponderType = typeof(RestartServiceResponder);
	}
}
